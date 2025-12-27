namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/AsrielDummy")]
    public class AsrielDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.White,
            Color2 = Color.LightBlue,
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
        public float FloatSpeed = 110f;
        public float FloatAccel = 220f;
        public float Floatness = 2.5f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Asriel specific fields - compassion and hope abilities
        private bool compassionActive = false;
        private float compassionTime = 0f;
        private const float MAX_COMPASSION_TIME = 3f;
        private SoundSource hopeSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.4f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public AsrielDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public AsrielDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            followBehindIndexDelay = 0.35f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Asriel);
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
                    Audio.Play("event:/char/asriel/footstep", Position);
                }
            };
            
            Add(Wave = new SineWave(0.2f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.White, 1f, 22, 70));
            
            hopeSound = new SoundSource();
            Add(hopeSound);
            
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
                Vector2 targetPosition = player.Position + new Vector2(index * 18f - 36f, -18f);
                float distance = Vector2.Distance(Position, targetPosition);
                
                if (distance > 24f)
                {
                    // Move toward target position using Position manipulation
                    Vector2 direction = (targetPosition - Position).SafeNormalize();
                    Position += direction * FloatSpeed * Engine.DeltaTime;
                }
                else
                {
                    // Stay close to target with gentle drift
                    Vector2 drift = (targetPosition - Position) * 0.15f;
                    Position += drift * Engine.DeltaTime;
                }
                
                // Handle compassion timer
                if (compassionTime > 0f)
                {
                    compassionTime -= Engine.DeltaTime;
                    if (compassionTime <= 0f)
                    {
                        compassionActive = false;
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
            // Allow player to climb on Asriel - provides hope boost and temporary invincibility
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -140f); // Strong hope-powered boost
                Audio.Play("event:/char/asriel/hope_boost", Position);
                hopeSound.Play("event:/char/asriel/compassion");
                
                SceneAs<Level>().Particles.Emit(P_Vanish, 12, Center, Vector2.One * 8f);
                
                // Activate compassion - briefly protects from damage
                compassionTime = MAX_COMPASSION_TIME;
                compassionActive = true;
                
                // Create hope aura that helps with platforming
                Add(new Coroutine(CreateHopeAura(player.Position)));
            }
        }

        private IEnumerator CreateHopeAura(Vector2 position)
        {
            // Create a gentle aura that provides slight assistance with jumps
            for (float t = 0f; t < 2f; t += Engine.DeltaTime)
            {
                SceneAs<Level>().ParticlesFG.Emit(P_Vanish, 2, 
                    position + Vector2.UnitX * Calc.Random.Range(-16f, 16f) + Vector2.UnitY * Calc.Random.Range(-8f, 8f), 
                    Vector2.One * 2f);
                
                // If player is falling near this position, provide slight upward assistance
                if (player != null && Vector2.Distance(player.Position, position) < 32f && player.Speed.Y > 0f)
                {
                    player.Speed.Y *= 0.98f; // Slight fall reduction
                }
                
                yield return null;
            }
            
            yield break;
        }

        public override void Render()
        {
            // Render with compassion glow when active
            if (compassionActive)
            {
                Draw.Circle(Position, 8f + (float)Math.Sin(compassionTime * 4f) * 2f, Color.White * 0.4f, 3);
            }
            
            base.Render();
        }
    }
}



