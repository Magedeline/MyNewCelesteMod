namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/CloverDummy")]
    public class CloverDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.White,
            Color2 = Color.Green,
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
        public float FloatSpeed = 95f;
        public float FloatAccel = 190f;
        public float Floatness = 2.2f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Clover specific fields - justice and shooting star abilities
        private bool justiceActive = false;
        private float justiceTime = 0f;
        private const float MAX_JUSTICE_TIME = 3f;
        private SoundSource justiceSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.4f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public CloverDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public CloverDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.35f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Clover);
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
            
            Add(Wave = new SineWave(0.22f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Yellow, 1f, 22, 65));
            
            justiceSound = new SoundSource();
            Add(justiceSound);
            
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
                    Vector2 direction = (targetPosition - Position).SafeNormalize();
                    Position += direction * FloatSpeed * Engine.DeltaTime;
                }
                else
                {
                    Vector2 drift = (targetPosition - Position) * 0.12f;
                    Position += drift * Engine.DeltaTime;
                }
                
                // Handle justice timer
                if (justiceTime > 0f)
                {
                    justiceTime -= Engine.DeltaTime;
                    if (justiceTime <= 0f)
                    {
                        justiceActive = false;
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
            // Allow player to climb on Clover - provides justice boost and star trail
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -130f); // Justice-powered boost
                Audio.Play("event:/char/madeline/jump_super", Position);
                justiceSound.Play("event:/char/madeline/grab");
                
                SceneAs<Level>().Particles.Emit(P_Vanish, 10, Center, Vector2.One * 7f);
                
                // Activate justice - creates star trail assistance
                justiceTime = MAX_JUSTICE_TIME;
                justiceActive = true;
                
                // Create shooting star trail for guidance
                Add(new Coroutine(CreateStarTrail(player.Position)));
            }
        }

        private IEnumerator CreateStarTrail(Vector2 position)
        {
            // Create a star trail that provides directional guidance
            for (float t = 0f; t < 2f; t += Engine.DeltaTime)
            {
                SceneAs<Level>().ParticlesFG.Emit(P_Vanish, 2, 
                    position + Vector2.UnitX * Calc.Random.Range(-8f, 8f) + Vector2.UnitY * Calc.Random.Range(-12f, 4f), 
                    Vector2.One * 1.5f);
                
                // If player is falling or struggling, provide slight guidance
                if (player != null && Vector2.Distance(player.Position, position) < 28f)
                {
                    // Star trail provides gentle course correction
                    if (player.Speed.Y > 50f)
                    {
                        player.Speed.Y *= 0.96f; // Gentle fall reduction
                    }
                }
                
                yield return null;
            }
            
            yield break;
        }

        public override void Render()
        {
            // Render with justice star glow when active
            if (justiceActive)
            {
                Draw.Circle(Position, 7f + (float)Math.Sin(justiceTime * 6f) * 1.5f, Color.Yellow * 0.35f, 2);
                // Draw star points
                float angle = justiceTime * 3f;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 starPoint = Position + Calc.AngleToVector(angle + i * MathHelper.TwoPi / 5f, 8f);
                    Draw.Point(starPoint, Color.Yellow * 0.6f);
                }
            }
            
            base.Render();
        }
    }
}



