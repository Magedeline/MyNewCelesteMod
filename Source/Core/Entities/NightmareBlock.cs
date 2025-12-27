namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// Nightmare block that kills the player after being inside for 5 seconds
    /// </summary>
    [CustomEntity("Ingeste/NightmareBlock")]
    [Tracked]
    public class NightmareBlock : Entity
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
        private SoundSource nightmareSfx;
        private Level level;
        
        // Configuration
        private bool isActivated = false;
        private string requiredCutsceneFlag;
        private bool requiresCutscene;
        private bool oneUse;
        private bool hasBeenUsed = false;
        
        // Visual properties
        private Color cubeColor;
        private Color nightmareColor;
        private float width, height;
        private bool fastMoving;
        private bool below;
        
        // Nightmare state
        private bool playerInside = false;
        private float playerInsideTimer = 0f;
        private global::Celeste.Player trackedPlayer = null;
        private const float max_inside_time = 5f;
        private DreamParticle[] particles;
        private int particleCount = 32;
        private ParticleType nightmareBlockParticleType;
        
        // Activation effects
        private float whiteFill;
        private float whiteHeight = 1f;
        private Shaker shaker;

        public bool IsActivated => isActivated;

        public NightmareBlock(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            width = data.Width;
            height = data.Height;
            oneUse = data.Bool("oneUse", false);
            fastMoving = data.Bool("fastMoving", false);
            below = data.Bool("below", false);
            requiredCutsceneFlag = data.Attr("requiredCutsceneFlag", "nightmare_cutscene_completed");
            requiresCutscene = data.Bool("requiresCutscene", true);
            
            string cubeColorHex = data.Attr("cubeColor", "8B0000");  // Dark Red
            string nightmareColorHex = data.Attr("nightmareColor", "FF0000"); // Red
            
            if (TryParseColor(cubeColorHex, out Color cColor))
                cubeColor = cColor;
            else
                cubeColor = Calc.HexToColor("8B0000");
                
            if (TryParseColor(nightmareColorHex, out Color nColor))
                nightmareColor = nColor;
            else
                nightmareColor = Color.Red;

            Depth = below ? 5000 : -11000;
            Collider = new Hitbox(width, height);
            
            // Initialize ParticleTypes as the correct type
            nightmareBlockParticleType = ParticleTypes.Dust;

            SetupComponents();
            InitializeParticles();
        }

        private void SetupComponents()
        {
            // Create cube sprite (can reuse dream block sprites or create custom ones)
            sprite = new Sprite(GFX.Game, "objects/Ingeste/nightmareBlock/");
            sprite.AddLoop("inactive", "cube_inactive", 0.1f);
            sprite.AddLoop("active", "cube_active", 0.05f);
            sprite.AddLoop("nightmare_idle", "nightmare_idle", 0.08f);
            sprite.AddLoop("nightmare_active", "nightmare_active", 0.03f);
            sprite.Play("inactive");
            sprite.Color = cubeColor;
            Add(sprite);

            // Lighting
            light = new VertexLight(cubeColor, 0.5f, (int)width + 16, (int)height + 16);
            Add(light);

            // Sound
            nightmareSfx = new SoundSource();
            Add(nightmareSfx);
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
            particle.Color = Color.Lerp(cubeColor, nightmareColor, Calc.Random.NextFloat());
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
            sprite.Play("nightmare_idle");
            sprite.Color = nightmareColor;
            light.Color = nightmareColor;
            light.Alpha = 0.8f;
            
            // Play activation sound
            Audio.Play("event:/game/06_reflection/dreamblock_sequence", Position);
        }

        public IEnumerator Activate()
        {
            NightmareBlock nightmareBlock = this;
            Level level = nightmareBlock.SceneAs<Level>();
            yield return 1f;
            Input.Rumble(RumbleStrength.Light, RumbleLength.Long);
            
            // Use a lambda for the shaker's onShake callback
            nightmareBlock.Add((Component)(nightmareBlock.shaker = new Shaker(onShake: v => nightmareBlock.Position += v)));
            nightmareBlock.shaker.Interval = 0.02f;
            nightmareBlock.shaker.On = true;
            
            float p;
            for (p = 0.0f; (double)p < 1.0; p += Engine.DeltaTime)
            {
                nightmareBlock.whiteFill = Ease.CubeIn(p);
                yield return null;
            }
            
            nightmareBlock.shaker.On = false;
            yield return 0.5f;
            nightmareBlock.ActivateNoRoutine();
            nightmareBlock.whiteHeight = 1f;
            nightmareBlock.whiteFill = 1f;
            
            for (p = 1f; (double)p > 0.0; p -= Engine.DeltaTime * 0.5f)
            {
                nightmareBlock.whiteHeight = p;
                if (level.OnInterval(0.1f))
                {
                    for (int index = 0; (double)index < (double)nightmareBlock.width; index += 4)
                        level.ParticlesFG.Emit(nightmareBlock.nightmareBlockParticleType, new Vector2(nightmareBlock.X + (float)index, (float)((double)nightmareBlock.Y + (double)nightmareBlock.height * (double)nightmareBlock.whiteHeight + 1.0)));
                }
                if (level.OnInterval(0.1f))
                    level.Shake();
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
                yield return null;
            }
            
            while ((double)nightmareBlock.whiteFill > 0.0)
            {
                nightmareBlock.whiteFill -= Engine.DeltaTime * 3f;
                yield return null;
            }
        }

        public void ActivateNoRoutine()
        {
            if (!isActivated)
            {
                ActivateBlock();
            }
            
            whiteHeight = 0.0f;
            whiteFill = 0.0f;
            
            if (shaker != null)
            {
                shaker.On = false;
            }
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
                HandleNightmareTimer(player);
            }
        }

        private void HandleNightmareTimer(global::Celeste.Player player)
        {
            // Timer logic - kill player after max_inside_time seconds
            playerInsideTimer += Engine.DeltaTime;
            
            // Visual warning as time progresses
            float dangerPercent = playerInsideTimer / max_inside_time;
            
            if (playerInsideTimer >= max_inside_time)
            {
                // Kill the player
                player.Die(Vector2.Zero, false, false);
                playerInsideTimer = 0f;
            }
        }

        private void OnPlayerEnter(global::Celeste.Player player)
        {
            if (oneUse && hasBeenUsed) return;

            playerInsideTimer = 0f;
            trackedPlayer = player;
            
            // Play enter sound
            nightmareSfx.Play("event:/game/06_reflection/dreamblock_enter");
            
            // Visual feedback
            sprite.Play("nightmare_active");
        }

        private void OnPlayerExit(global::Celeste.Player player)
        {
            sprite.Play("nightmare_idle");
            playerInsideTimer = 0f;
            trackedPlayer = null;
            
            if (oneUse && playerInside)
            {
                hasBeenUsed = true;
                Add(new Coroutine(DissolveBlock()));
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
                
                sprite.Color = Color.Lerp(nightmareColor, Color.Transparent, progress);
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
                // Increase intensity as timer approaches max
                float dangerPercent = playerInsideTimer / max_inside_time;
                light.Alpha += 0.3f + dangerPercent * 0.4f;
                
                // Pulse faster as danger increases
                float pulseSpeed = 8f + dangerPercent * 16f;
                sprite.Color = Color.Lerp(nightmareColor, Color.White, 
                    (float)Math.Sin(Scene.TimeActive * pulseSpeed) * 0.2f + 0.2f + dangerPercent * 0.3f);
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
                // Render nightmare particles
                RenderParticles();
                
                // Render nightmare effect border
                DrawNightmareBorder();
            }
            
            // Render white fill during activation
            if ((double)whiteFill > 0.0)
            {
                Draw.Rect(X, Y, width, height * whiteHeight, Color.White * whiteFill);
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

        private void DrawNightmareBorder()
        {
            Color borderColor = nightmareColor * 0.6f;

            if (playerInside)
            {
                // Make border more intense and pulse as danger increases
                float dangerPercent = playerInsideTimer / max_inside_time;
                float pulseFactor = (float)Math.Sin(Scene.TimeActive * (8f + dangerPercent * 16f)) * 0.4f + 0.6f;
                borderColor = Color.Lerp(nightmareColor, Color.White, dangerPercent) * pulseFactor;
            }

            // Draw hollow rectangle border
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




