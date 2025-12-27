namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/KingDDDDummy")]
    public class KingDDDDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.Purple,
            Color2 = Color.Blue,
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.6f,
            LifeMax = 1.2f,
            SpeedMin = 10f,
            SpeedMax = 20f,
            DirectionRange = (float)Math.PI * 2f
        };

        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;
        public float FloatSpeed = 80f;
        public float FloatAccel = 150f;
        public float Floatness = 3f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // King DDD specific fields - hammer abilities and royal power
        private bool hammerCharged = false;
        private float hammerChargeTime = 0f;
        private const float MAX_HAMMER_CHARGE = 2f;
        private SoundSource hammerSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.8f; // Slower follow due to size
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public KingDDDDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public KingDDDDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.5f * index;
            
            Collider = new Hitbox(8f, 8f, -4f, -8f); // Larger hitbox for King DDD
            Sprite = new PlayerSprite(PlayerSpriteMode.KingDDD);
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
                    Audio.Play("event:/char/king_ddd/footstep", Position);
                }
            };
            
            Add(Wave = new SineWave(0.3f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Gold, 1.2f, 24, 80));
            
            hammerSound = new SoundSource();
            Add(hammerSound);
            
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
                Vector2 targetPosition = player.Position + new Vector2(index * 20f - 40f, -20f);
                float distance = Vector2.Distance(Position, targetPosition);
                
                if (distance > 32f)
                {
                    // Move toward target position using Position manipulation
                    Vector2 direction = (targetPosition - Position).SafeNormalize();
                    Position += direction * FloatSpeed * Engine.DeltaTime;
                }
                else
                {
                    // Stay close to target with slight drift
                    Vector2 drift = (targetPosition - Position) * 0.1f;
                    Position += drift * Engine.DeltaTime;
                }
                
                // Handle hammer charging
                if (hammerChargeTime > 0f)
                {
                    hammerChargeTime -= Engine.DeltaTime;
                    if (hammerChargeTime <= 0f)
                    {
                        hammerCharged = false;
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
            // Allow player to climb on King DDD - provides powerful hammer boost
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 6f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -150f); // Strong boost
                player.Speed.X += Math.Sign(Sprite.Scale.X) * 20f;
                
                Audio.Play("event:/char/king_ddd/hammer_boost", Position);
                hammerSound.Play("event:/char/king_ddd/royal_power");
                
                SceneAs<Level>().Particles.Emit(P_Vanish, 10, Center, Vector2.One * 6f);
                
                // Create hammer shockwave for platform breaking
                Add(new Coroutine(CreateHammerShockwave(player.Position + Vector2.UnitY * 24f)));
                
                // Charge hammer for next use
                hammerChargeTime = MAX_HAMMER_CHARGE;
                hammerCharged = true;
            }
        }

        private IEnumerator CreateHammerShockwave(Vector2 position)
        {
            // Create a powerful shockwave that can break certain blocks
            SceneAs<Level>().Displacement.AddBurst(position, 0.8f, 32f, 128f, 1.5f, null, null);
            SceneAs<Level>().Particles.Emit(P_Vanish, 15, position, Vector2.One * 12f);
            
            // Visual hammer effect
            for (float t = 0f; t < 1f; t += Engine.DeltaTime * 2f)
            {
                SceneAs<Level>().ParticlesFG.Emit(global::Celeste.ParticleTypes.Dust, 3, 
                    position + Vector2.UnitX * Calc.Random.Range(-32f, 32f), Vector2.One * 4f);
                yield return null;
            }
            
            yield break;
        }

        public override void Render()
        {
            // Render with royal glow when hammer is charged
            if (hammerCharged)
            {
                Draw.Rect(X - 2, Y - 2, 4, 4, Color.Gold * 0.6f);
            }
            
            base.Render();
        }
    }
}



