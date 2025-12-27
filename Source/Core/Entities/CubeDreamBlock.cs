namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// Cube-styled dream block that activates after Chara mirror cutscene
    /// </summary>
    [CustomEntity("Ingeste/CubeDreamBlock")]
    [Tracked]
    public class CubeDreamBlock : Entity
    {
        private struct DreamParticle
        {
            public Vector2 Position;
            public Color Color;
            public float Alpha;
            public float Speed;
            public Vector2 Direction;
        }

        private Sprite sprite;
        private VertexLight light;
        private SoundSource dreamSfx;
        private Level level;
        
        // Configuration
        private bool isActivated = false;
        private string requiredCutsceneFlag;
        private bool requiresCutscene;
        private bool oneUse;
        private bool hasBeenUsed = false;
        
        // Visual properties
        private Color cubeColor;
        private Color dreamColor;
        private float width, height;
        private bool fastMoving;
        private bool below;
        
        // Dream state
        private bool playerInside = false;
        private float dashSpeed = 240f;
        private Vector2 dashDirection;
        private bool isDashing = false;
        private DreamParticle[] particles;
        private int particleCount = 32;
        private ParticleType dreamBlockParticleType;

        public bool IsActivated => isActivated;

        public CubeDreamBlock(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            width = data.Width;
            height = data.Height;
            oneUse = data.Bool("oneUse", false);
            fastMoving = data.Bool("fastMoving", false);
            below = data.Bool("below", false);
            requiredCutsceneFlag = data.Attr("requiredCutsceneFlag", "chara_mirror_cutscene_completed");
            requiresCutscene = data.Bool("requiresCutscene", true);
            
            string cubeColorHex = data.Attr("cubeColor", "4B0082");  // Indigo
            string dreamColorHex = data.Attr("dreamColor", "FF69B4"); // Hot Pink
            
            if (TryParseColor(cubeColorHex, out Color cColor))
                cubeColor = cColor;
            else
                cubeColor = Color.Indigo;
                
            if (TryParseColor(dreamColorHex, out Color dColor))
                dreamColor = dColor;
            else
                dreamColor = Color.HotPink;

            Depth = below ? 5000 : -11000;
            Collider = new Hitbox(width, height);
            
            // Initialize ParticleTypes as the correct type
            dreamBlockParticleType = ParticleTypes.Dust;

            SetupComponents();
            InitializeParticles();
        }

        private void SetupComponents()
        {
            // Create cube sprite
            sprite = new Sprite(GFX.Game, "objects/Ingeste/cubeDreamBlock/");
            sprite.AddLoop("inactive", "cube_inactive", 0.1f);
            sprite.AddLoop("active", "cube_active", 0.05f);
            sprite.AddLoop("dream_idle", "dream_idle", 0.08f);
            sprite.AddLoop("dream_active", "dream_active", 0.03f);
            sprite.Play("inactive");
            sprite.Color = cubeColor;
            Add(sprite);

            // Lighting
            light = new VertexLight(cubeColor, 0.5f, (int)width + 16, (int)height + 16);
            Add(light);

            // Sound
            dreamSfx = new SoundSource();
            Add(dreamSfx);
        }

        private void InitializeParticles()
        {
            particles = new DreamParticle[particleCount];
            for (int i = 0; i < particleCount; i++)
            {
                ResetParticle(ref particles[i]);
            }
        }

        private void ResetParticle(ref DreamParticle particle)
        {
            particle.Position = new Vector2(
                X + Calc.Random.NextFloat() * width,
                Y + Calc.Random.NextFloat() * height
            );
            particle.Color = Color.Lerp(cubeColor, dreamColor, Calc.Random.NextFloat());
            particle.Alpha = Calc.Random.Range(0.3f, 0.8f);
            particle.Speed = Calc.Random.Range(10f, 30f);
            particle.Direction = Calc.AngleToVector(Calc.Random.NextFloat() * 360f * Calc.DegToRad, 1f);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
            
            CheckActivationStatus();
        }

        private void CheckActivationStatus()
        {
            if (!requiresCutscene)
            {
                ActivateBlock();
                return;
            }

            // Check if the required cutscene flag is set
            if (level?.Session != null && level.Session.GetFlag(requiredCutsceneFlag))
            {
                ActivateBlock();
            }
        }

        private void ActivateBlock()
        {
            if (isActivated) return;
            
            isActivated = true;
            sprite.Play("dream_idle");
            sprite.Color = dreamColor;
            light.Color = dreamColor;
            light.Alpha = 0.8f;
            
            // Play activation sound
            Audio.Play("event:/game/06_reflection/dreamblock_sequence", Position);
        }

        public override void Update()
        {
            base.Update();
            
            // Recheck activation status
            if (!isActivated && requiresCutscene)
            {
                CheckActivationStatus();
            }

            if (!isActivated) return;

            var player = Scene.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null)
            {
                HandlePlayerInteraction(player);
            }
            
            UpdateParticles();
            UpdateVisualEffects();
        }

        private void HandlePlayerInteraction(global::Celeste.Player player)
        {
            bool wasInside = playerInside;
            playerInside = CollideCheck(player);

            if (playerInside && !wasInside)
            {
                OnPlayerEnter(player);
            }
            else if (!playerInside && wasInside)
            {
                OnPlayerExit(player);
            }

            if (playerInside)
            {
                HandleDreamMovement(player);

                void HandleDreamMovement(global::Celeste.Player player1)
                {
                    if (isDashing) return;
                    // Allow player to move freely inside the block
                    Vector2 input = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
                    if (input.LengthSquared() > 0.1f)
                    {
                        input.Normalize();
                        player1.Speed = input * dashSpeed;
                    }
                    else
                    {
                        player1.Speed = Vector2.Zero;
                    }
                }
            }
        }

        private void OnPlayerEnter(global::Celeste.Player player)
        {
            if (oneUse && hasBeenUsed) return;

            // Play enter sound
            dreamSfx.Play("event:/game/06_reflection/dreamblock_enter");
            
            // Visual feedback
            sprite.Play("dream_active");
            
            // Start dream dash capability
            Add(new Coroutine(DreamDashCoroutine(player)));
        }

        private void OnPlayerExit(global::Celeste.Player player)
        {
            sprite.Play("dream_idle");
            isDashing = false;
            
            if (oneUse && playerInside)
            {
                hasBeenUsed = true;
                Add(new Coroutine(DissolveBlock()));
            }
        }

        private IEnumerator DreamDashCoroutine(global::Celeste.Player player)
        {
            while (playerInside && isActivated && !(oneUse && hasBeenUsed))
            {
                // Check for dash input
                if (Input.Dash.Pressed && player.DashDir.LengthSquared() > 0.25f)
                {
                    yield return PerformDreamDash(player);
                }
                yield return null;
            }
        }

        private IEnumerator PerformDreamDash(global::Celeste.Player player)
        {
            isDashing = true;
            dashDirection = player.DashDir.SafeNormalize();
            
            // Play dash sound
            Audio.Play("event:/game/06_reflection/dreamblock_travel", Position);
            
            // Create dash effect
            CreateParticleEffect(player.Position);
            
            // Move player through the dream block
            Vector2 startPos = player.Position;
            Vector2 endPos = CalculateExitPosition(startPos, dashDirection);
            
            float dashTime = fastMoving ? 0.15f : 0.3f;
            float timer = 0f;
            
            // Make player move through solid blocks
            player.StateMachine.State = global::Celeste.Player.StDreamDash;
            
            while (timer < dashTime)
            {
                timer += Engine.DeltaTime;
                float progress = Ease.SineInOut(timer / dashTime);
                player.Position = Vector2.Lerp(startPos, endPos, progress);
                
                // Create particle trail
                if (Scene.OnInterval(0.02f))
                {
                    CreateDashParticles(player.Position);
                }
                
                yield return null;
            }
            
            player.Position = endPos;
            player.StateMachine.State = global::Celeste.Player.StNormal;
            
            // Restore dash if consumed
            if (level?.Session?.Inventory != null && level.Session.Inventory.Dashes == 0)
            {
                level.Session.Inventory.Dashes = 1;
            }
            
            isDashing = false;
        }

        private Vector2 CalculateExitPosition(Vector2 startPos, Vector2 direction)
        {
            // Calculate where the player should exit based on dash direction
            Vector2 currentPos = startPos;
            float stepSize = 2f;
            
            while (CollideCheck(new Rectangle((int)currentPos.X - 4, (int)currentPos.Y - 11, 8, 11)))
            {
                currentPos += direction * stepSize;
                
                // Prevent infinite loop
                if (Vector2.Distance(startPos, currentPos) > Math.Max(width, height) + 100f)
                    break;
            }
            
            return currentPos;
        }

        private bool CollideCheck(Rectangle other)
        {
            return CollideRect(other);
        }

        private void CreateDashParticles(Vector2 position)
        {
            if (level == null) return;

            float particleDir = (-dashDirection).Angle();
            SceneAs<Level>().Particles.Emit(dreamBlockParticleType, position, particleDir * Calc.RadToDeg);
        }

        private void CreateParticleEffect(Vector2 position)
        {
            if (level == null) return;

            // Create multiple particles in random directions
            for (int i = 0; i < 8; i++)
            {
                float angle = Calc.Random.NextFloat() * 360f * Calc.DegToRad;
                float speed = Calc.Random.Range(15f, 35f);
                // Only pass the angle (float) as direction, not a Vector2
                SceneAs<Level>().Particles.Emit(dreamBlockParticleType, position, angle);
            }
        }

        private IEnumerator DissolveBlock()
        {        
            float timer = 0f;
            float dissolveTime = 2f;
            
            while (timer < dissolveTime)
            {
                timer += Engine.DeltaTime;
                float progress = timer / dissolveTime;
                
                sprite.Color = Color.Lerp(dreamColor, Color.Transparent, progress);
                light.Alpha = 1f - progress;
                
                yield return null;
            }
            
            RemoveSelf();
        }

        private void UpdateParticles()
        {
            if (!isActivated) return;
            
            for (int i = 0; i < particleCount; i++)
            {
                particles[i].Position += particles[i].Direction * particles[i].Speed * Engine.DeltaTime;
                
                // Reset particle if it goes outside bounds
                if (particles[i].Position.X < X - 10f || particles[i].Position.X > X + width + 10f ||
                    particles[i].Position.Y < Y - 10f || particles[i].Position.Y > Y + height + 10f)
                {
                    ResetParticle(ref particles[i]);
                }
                
                // Pulse alpha
                particles[i].Alpha = 0.5f + (float)Math.Sin(Scene.TimeActive * 3f + i * 0.5f) * 0.3f;
            }
        }

        private void UpdateVisualEffects()
        {
            if (!isActivated) return;
            
            // Pulsing light effect
            light.Alpha = 0.6f + (float)Math.Sin(Scene.TimeActive * 2f) * 0.2f;
            
            // More intense effects when player is inside
            if (playerInside)
            {
                light.Alpha += 0.3f;
                sprite.Color = Color.Lerp(dreamColor, Color.White, (float)Math.Sin(Scene.TimeActive * 8f) * 0.2f + 0.2f);
            }
        }

        public override void Render()
        {
            if (!isActivated)
            {
                // Render inactive cube
                DrawCubeOutline(cubeColor * 0.3f, 2);
            }
            else
            {
                // Render dream particles
                RenderParticles();
                
                // Render dream effect border
                DrawDreamBorder();
            }
            
            base.Render();
        }

        private void DrawCubeOutline(Color color, int thickness)
        {
            // Draw cube wireframe
            Draw.HollowRect(X, Y, width, height, color);

            // Draw cube depth lines
            Vector2 offset = new Vector2(4, -4);
            Draw.Line(new Vector2(X, Y) + offset, new Vector2(X, Y), color, thickness);
            Draw.Line(new Vector2(X + width, Y) + offset, new Vector2(X + width, Y), color, thickness);
            Draw.Line(new Vector2(X, Y + height) + offset, new Vector2(X, Y + height), color, thickness);
            Draw.Line(new Vector2(X + width, Y + height) + offset, new Vector2(X + width, Y + height), color, thickness);
        }

        private void RenderParticles()
        {
            for (int i = 0; i < particleCount; i++)
            {
                Color particleColor = particles[i].Color * particles[i].Alpha;
                Draw.Point(particles[i].Position, particleColor);
            }
        }

        private void DrawDreamBorder()
        {
            Color borderColor = dreamColor * 0.6f;

            if (playerInside)
            {
                borderColor = Color.White * 0.8f;
            }

            // Use the 5-argument overload (no thickness parameter)
            Draw.HollowRect(X - 1, Y - 1, width + 2, height + 2, borderColor);
        }

        private static bool TryParseColor(string hex, out Color color)
        {
            color = Color.White;
            if (string.IsNullOrEmpty(hex)) return false;
            
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);
            
            try
            {
                if (hex.Length == 6)
                {
                    int r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    int g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    int b = Convert.ToInt32(hex.Substring(4, 2), 16);
                    color = new Color(r, g, b);
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}



