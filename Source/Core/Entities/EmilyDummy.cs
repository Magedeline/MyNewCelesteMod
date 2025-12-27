namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/EmilyDummy")]
    public class EmilyDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.White,
            Color2 = Color.Orange,
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
        public float FloatSpeed = 98f;
        public float FloatAccel = 196f;
        public float Floatness = 2.3f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Emily specific fields - bravery and energy abilities
        private bool braveryActive = false;
        private float braveryTime = 0f;
        private const float MAX_BRAVERY_TIME = 3.2f;
        private SoundSource braverySound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.4f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public EmilyDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public EmilyDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.35f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Emily);
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
            
            Add(Wave = new SineWave(0.24f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Orange, 1f, 23, 68));
            
            braverySound = new SoundSource();
            Add(braverySound);
            
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
                Vector2 targetPosition = player.Position + new Vector2(index * 18f - 36f, -17f);
                float distance = Vector2.Distance(Position, targetPosition);
                
                if (distance > 24f)
                {
                    Vector2 direction = (targetPosition - Position).SafeNormalize();
                    Position += direction * FloatSpeed * Engine.DeltaTime;
                }
                else
                {
                    Vector2 drift = (targetPosition - Position) * 0.13f;
                    Position += drift * Engine.DeltaTime;
                }
                
                // Handle bravery timer
                if (braveryTime > 0f)
                {
                    braveryTime -= Engine.DeltaTime;
                    if (braveryTime <= 0f)
                    {
                        braveryActive = false;
                    }
                }
                
                // Face player direction with energetic movements
                if (player.Speed.X != 0f)
                {
                    Sprite.Scale.X = Math.Sign(player.Speed.X) * -1f;
                }
            }
            
            base.Update();
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Allow player to climb on Emily - provides bravery boost and energy assistance
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -128f); // Bravery-powered boost
                Audio.Play("event:/char/madeline/jump", Position);
                braverySound.Play("event:/char/madeline/dash_red_left");
                
                SceneAs<Level>().Particles.Emit(P_Vanish, 11, Center, Vector2.One * 8f);
                
                // Activate bravery - provides energy and confidence
                braveryTime = MAX_BRAVERY_TIME;
                braveryActive = true;
                
                // Create energy bursts for movement assistance
                Add(new Coroutine(CreateEnergyBursts(player.Position)));
            }
        }

        private IEnumerator CreateEnergyBursts(Vector2 position)
        {
            // Create energy bursts that provide movement confidence
            for (float t = 0f; t < 2.3f; t += Engine.DeltaTime)
            {
                if (t % 0.3f < Engine.DeltaTime) // Energy burst timing
                {
                    SceneAs<Level>().ParticlesFG.Emit(P_Vanish, 4, 
                        position + Vector2.UnitX * Calc.Random.Range(-14f, 14f) + Vector2.UnitY * Calc.Random.Range(-6f, 6f), 
                        Vector2.One * 2.5f);
                }
                
                // Energy provides movement confidence and assistance
                if (player != null && Vector2.Distance(player.Position, position) < 30f)
                {
                    // Boost dash recovery and movement confidence
                    if (player.Speed.Y > 0f && player.Speed.Y < 80f)
                    {
                        player.Speed.Y *= 0.94f; // Confident fall reduction
                    }
                    // Boost horizontal movement for confident dashes
                    if (Math.Abs(player.Speed.X) > 100f)
                    {
                        player.Speed.X *= 1.03f; // Small confidence boost to dashes
                    }
                }
                
                yield return null;
            }
            
            yield break;
        }

        public override void Render()
        {
            // Render with bravery energy effects when active
            if (braveryActive)
            {
                float energy = (float)Math.Sin(braveryTime * 7f) * 0.3f + 0.7f;
                Draw.Circle(Position, 9f + energy * 2f, Color.Orange * (0.3f + energy * 0.2f), 3);
                
                // Draw energy sparks
                for (int i = 0; i < 6; i++)
                {
                    float angle = braveryTime * 4f + i * MathHelper.Pi / 3f;
                    Vector2 sparkPos = Position + Calc.AngleToVector(angle, 6f + energy * 3f);
                    Draw.Point(sparkPos, Color.Orange * energy);
                }
            }
            
            base.Render();
        }
    }
}



