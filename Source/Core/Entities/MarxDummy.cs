namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/MarxDummy")]
    public class MarxDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.Purple,
            Color2 = Color.Magenta,
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.7f,
            LifeMax = 1.3f,
            SpeedMin = 15f,
            SpeedMax = 25f,
            DirectionRange = (float)Math.PI * 2f
        };

        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;
        public float FloatSpeed = 120f;
        public float FloatAccel = 240f;
        public float Floatness = 4f; // More chaotic movement
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Marx specific fields - chaotic teleportation and trickery
        private bool tricksterMode = false;
        private float tricksterTime = 0f;
        private const float MAX_TRICKSTER_TIME = 2.5f;
        private SoundSource chaosSound;
        private Vector2 lastTeleportPos;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.2f; // Fast, unpredictable following
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public MarxDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public MarxDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.3f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Marx);
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
                if (anim == "teleport" && currentAnimationFrame == 3)
                {
                    Audio.Play("event:/char/marx/teleport", Position);
                }
            };
            
            Add(Wave = new SineWave(0.4f, 0f)); // Erratic floating
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Purple, 1.1f, 26, 75));
            
            chaosSound = new SoundSource();
            Add(chaosSound);
            
            Add(new PlayerCollider(new Action<global::Celeste.Player>(OnPlayer), null, null));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            player = scene.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null)
            {
                following = true;
                lastTeleportPos = Position;
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
                Vector2 targetPosition = player.Position + new Vector2(index * 25f - 50f, -15f);
                float distance = Vector2.Distance(Position, targetPosition);
                
                // Marx has chaotic movement patterns
                if (distance > 40f)
                {
                    // Occasional teleportation for dramatic effect
                    if (Calc.Random.Chance(0.05f) && Vector2.Distance(lastTeleportPos, targetPosition) > 30f)
                    {
                        Add(new Coroutine(TeleportToTarget(targetPosition)));
                        lastTeleportPos = targetPosition;
                    }
                    else
                    {
                        // Erratic movement toward target
                        Vector2 direction = (targetPosition - Position).SafeNormalize();
                        Vector2 chaos = new Vector2(
                            (float)Math.Sin(Scene.TimeActive * 3f) * 0.3f,
                            (float)Math.Cos(Scene.TimeActive * 4f) * 0.2f
                        );
                        Position += (direction + chaos) * FloatSpeed * Engine.DeltaTime;
                    }
                }
                else
                {
                    // Orbit around target position
                    float angle = Scene.TimeActive * 2f + index * MathHelper.TwoPi / 3f;
                    Vector2 orbit = new Vector2(
                        (float)Math.Cos(angle) * 20f,
                        (float)Math.Sin(angle) * 10f
                    );
                    Position += (targetPosition + orbit - Position) * 0.1f * Engine.DeltaTime * 60f;
                }
                
                // Handle trickster mode timer
                if (tricksterTime > 0f)
                {
                    tricksterTime -= Engine.DeltaTime;
                    if (tricksterTime <= 0f)
                    {
                        tricksterMode = false;
                    }
                }
                
                // Face player direction with occasional random flips
                if (player.Speed.X != 0f || Calc.Random.Chance(0.02f))
                {
                    Sprite.Scale.X = Calc.Random.Chance(0.9f) ? Math.Sign(player.Speed.X) * -1f : Math.Sign(player.Speed.X);
                }
            }
            
            base.Update();
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Allow player to climb on Marx - provides chaotic teleportation boost
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -160f); // Very strong boost
                
                // Marx's special ability: brief teleportation assistance
                Vector2 teleportTarget = player.Position + new Vector2(
                    Math.Sign(player.Speed.X) * 60f,
                    -40f
                );
                
                // Check if teleport target is valid (not in wall)
                if (!SceneAs<Level>().CollideCheck<Solid>(teleportTarget))
                {
                    player.Position = teleportTarget;
                    Audio.Play("event:/char/marx/chaos_teleport", Position);
                    
                    // Trickster mode - brief invincibility and speed boost
                    tricksterTime = MAX_TRICKSTER_TIME;
                    tricksterMode = true;
                }
                else
                {
                    Audio.Play("event:/char/marx/boost", Position);
                }
                
                chaosSound.Play("event:/char/marx/trickery");
                SceneAs<Level>().Particles.Emit(P_Vanish, 15, Center, Vector2.One * 10f);
                
                // Create chaos effect
                Add(new Coroutine(CreateChaosEffect()));
            }
        }

        private IEnumerator TeleportToTarget(Vector2 target)
        {
            // Teleportation animation
            SceneAs<Level>().Particles.Emit(P_Vanish, 12, Position, Vector2.One * 8f);
            Sprite.Play("teleport", false, false);
            
            yield return 0.2f;
            
            Position = target;
            SceneAs<Level>().Particles.Emit(P_Vanish, 12, Position, Vector2.One * 8f);
            Audio.Play("event:/char/marx/teleport_arrive", Position);
            
            yield return 0.1f;
            Sprite.Play("idle", false, false);
        }

        private IEnumerator CreateChaosEffect()
        {
            // Create swirling chaos particles around the player
            for (float t = 0f; t < 1.5f; t += Engine.DeltaTime)
            {
                float angle = t * 8f;
                Vector2 chaosPos = player.Position + new Vector2(
                    (float)Math.Cos(angle) * 24f,
                    (float)Math.Sin(angle) * 12f
                );
                
                SceneAs<Level>().ParticlesFG.Emit(P_Vanish, 2, chaosPos, Vector2.One * 3f);
                yield return null;
            }
        }

        public override void Render()
        {
            // Render with chaotic aura when trickster mode is active
            if (tricksterMode)
            {
                float pulse = (float)Math.Sin(tricksterTime * 8f);
                Draw.Circle(Position, 12f + pulse * 4f, Color.Purple * 0.5f, 4);
                Draw.Circle(Position, 8f - pulse * 2f, Color.Magenta * 0.3f, 3);
            }
            
            base.Render();
        }
    }
}



