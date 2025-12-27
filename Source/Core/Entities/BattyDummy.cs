namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/BattyDummy")]
    public class BattyDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.White,
            Color2 = Color.DarkViolet,
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.8f,
            LifeMax = 1.4f,
            SpeedMin = 12f,
            SpeedMax = 24f,
            DirectionRange = (float)Math.PI * 2f
        };

        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;
        public float FloatSpeed = 115f;
        public float FloatAccel = 230f;
        public float Floatness = 3.0f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Batty specific fields - flight and echo abilities
        private bool flightActive = false;
        private float flightTime = 0f;
        private const float MAX_FLIGHT_TIME = 2.8f;
        private SoundSource flightSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.4f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public BattyDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public BattyDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            followBehindIndexDelay = 0.35f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Batty);
            Sprite.Play("idle", false, false);
            Sprite.Scale.X = -1f;
            Hair.Border = Color.Black;
            Hair.Facing = Facings.Left;
            Add(Hair);
            Add(Sprite);
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            Sprite.OnFrameChange = delegate(string anim)
            {
                int currentAnimationFrame = Sprite.CurrentAnimationFrame;
                if ((anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || 
                    (anim == "runSlow" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || 
                    (anim == "runFast" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)))
                {
                    Audio.Play("event:/char/madeline/footstep", Position);
                }
            };
            
            Add(Wave = new SineWave(0.25f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.DarkViolet, 1f, 26, 75));
            
            flightSound = new SoundSource();
            Add(flightSound);
            
            Add(new PlayerCollider(new Action<global::Celeste.Player>(OnPlayer), null, null));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            player = scene.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null)
            {
                following = true;
            }
        }

        public override void Update()
        {
            if (!Scene.OnInterval(0.1f))
            {
                base.Update();
                return;
            }
            
            if (player != null)
            {
                Vector2 targetPosition = player.Position + new Vector2(index * 18f - 36f, -22f);
                float distance = Vector2.Distance(Position, targetPosition);
                
                if (distance > 24f)
                {
                    Vector2 direction = (targetPosition - Position).SafeNormalize();
                    Position += direction * FloatSpeed * Engine.DeltaTime;
                }
                else
                {
                    // Batty has more erratic flight patterns
                    Vector2 drift = (targetPosition - Position) * 0.15f;
                    drift += Vector2.UnitY * (float)Math.Sin(Engine.Scene.TimeActive * 3f) * 8f * Engine.DeltaTime;
                    Position += drift * Engine.DeltaTime;
                }
                
                // Handle flight timer
                if (flightTime > 0f)
                {
                    flightTime -= Engine.DeltaTime;
                    if (flightTime <= 0f)
                    {
                        flightActive = false;
                    }
                }
                
                // Face player direction
                if (player.Speed.X != 0f)
                {
                    Sprite.Scale.X = Math.Sign(player.Speed.X) * -1f;
                }
            }
            
            base.Update();
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Allow player to climb on Batty - provides flight assistance and echo guidance
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -135f); // Flight-powered boost
                Audio.Play("event:/char/madeline/jump_super", Position);
                flightSound.Play("event:/char/madeline/dash_pink_left");
                
                SceneAs<Level>().Particles.Emit(P_Vanish, 15, Center, Vector2.One * 10f);
                
                // Activate flight - provides gliding assistance
                flightTime = MAX_FLIGHT_TIME;
                flightActive = true;
                
                // Create echo waves for navigation assistance
                Add(new Coroutine(CreateEchoWaves(player.Position)));
            }
        }

        private IEnumerator CreateEchoWaves(Vector2 position)
        {
            // Create echo waves that provide navigation assistance
            for (float t = 0f; t < 2f; t += Engine.DeltaTime)
            {
                if (t % 0.4f < Engine.DeltaTime) // Echo pulse timing
                {
                    SceneAs<Level>().ParticlesFG.Emit(P_Vanish, 5, 
                        position + Vector2.UnitX * Calc.Random.Range(-20f, 20f) + Vector2.UnitY * Calc.Random.Range(-10f, 10f), 
                        Vector2.One * 3f);
                }
                
                // Echo waves provide gliding assistance
                if (player != null && Vector2.Distance(player.Position, position) < 36f)
                {
                    // When falling, provide significant gliding assistance
                    if (player.Speed.Y > 30f)
                    {
                        player.Speed.Y *= 0.92f; // Strong gliding assistance
                    }
                    // Also provide slight horizontal boost for better navigation
                    if (Math.Abs(player.Speed.X) > 0f)
                    {
                        player.Speed.X *= 1.02f; // Small horizontal boost
                    }
                }
                
                yield return null;
            }
            
            yield break;
        }

        public override void Render()
        {
            // Render with wing flap effects when active
            if (flightActive)
            {
                float wingFlap = (float)Math.Sin(flightTime * 12f) * 0.4f + 0.6f;
                Draw.Circle(Position, 10f + wingFlap * 4f, Color.DarkViolet * (0.25f + wingFlap * 0.15f), 4);
                
                // Draw wing patterns
                float angle = Engine.Scene.TimeActive * 6f;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 wingPos = Position + Calc.AngleToVector(angle + i * MathHelper.TwoPi / 3f, 8f + wingFlap * 3f);
                    Draw.Line(Position, wingPos, Color.DarkViolet * 0.7f);
                }
            }
            
            base.Render();
        }
    }
}



