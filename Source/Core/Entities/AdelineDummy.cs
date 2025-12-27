namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/AdelineDummy")]
    public class AdelineDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.White,
            Color2 = Color.Pink,
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
        public float FloatSpeed = 100f;
        public float FloatAccel = 200f;
        public float Floatness = 2.0f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Adeline specific fields - paint and art abilities
        private bool paintActive = false;
        private float paintTime = 0f;
        private const float MAX_PAINT_TIME = 2.5f;
        private SoundSource paintSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.4f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public AdelineDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public AdelineDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            followBehindIndexDelay = 0.35f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Adeline);
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
            
            Add(Wave = new SineWave(0.2f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.White, 1f, 20, 60));
            
            paintSound = new SoundSource();
            Add(paintSound);
            
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
                Vector2 targetPosition = player.Position + new Vector2(index * 18f - 36f, -16f);
                float distance = Vector2.Distance(Position, targetPosition);
                
                if (distance > 24f)
                {
                    Vector2 direction = (targetPosition - Position).SafeNormalize();
                    Position += direction * FloatSpeed * Engine.DeltaTime;
                }
                else
                {
                    Vector2 drift = (targetPosition - Position) * 0.1f;
                    Position += drift * Engine.DeltaTime;
                }
                
                // Handle paint timer
                if (paintTime > 0f)
                {
                    paintTime -= Engine.DeltaTime;
                    if (paintTime <= 0f)
                    {
                        paintActive = false;
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
            // Allow player to climb on Adeline - provides paint platform assistance
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -120f); // Paint-powered boost
                Audio.Play("event:/char/madeline/jump", Position);
                paintSound.Play("event:/char/madeline/grab");
                
                SceneAs<Level>().Particles.Emit(P_Vanish, 8, Center, Vector2.One * 6f);
                
                // Activate paint ability - creates temporary platforms
                paintTime = MAX_PAINT_TIME;
                paintActive = true;
                
                // Create paint platforms for additional jumping assistance
                Add(new Coroutine(CreatePaintPlatforms(player.Position)));
            }
        }

        private IEnumerator CreatePaintPlatforms(Vector2 position)
        {
            // Create temporary paint platforms that help with jumping
            for (float t = 0f; t < 1.5f; t += Engine.DeltaTime)
            {
                SceneAs<Level>().ParticlesFG.Emit(P_Vanish, 1, 
                    position + Vector2.UnitX * Calc.Random.Range(-12f, 12f) + Vector2.UnitY * Calc.Random.Range(-4f, 4f), 
                    Vector2.One * 1f);
                
                // Create brief invisible platform assistance
                if (player != null && Vector2.Distance(player.Position, position) < 24f && player.Speed.Y > -20f)
                {
                    // Slight upward assistance when near paint area
                    if (player.Speed.Y > 0f)
                    {
                        player.Speed.Y *= 0.95f; // Small fall reduction
                    }
                }
                
                yield return null;
            }
            
            yield break;
        }

        public override void Render()
        {
            // Render with paint sparkles when active
            if (paintActive)
            {
                Draw.Circle(Position, 6f + (float)Math.Sin(paintTime * 5f) * 1f, Color.Pink * 0.3f, 2);
            }
            
            base.Render();
        }
    }
}



