namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/OdinDummy")]
    public class OdinDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.White,
            Color2 = Color.Blue,
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
        
        protected int index;
        protected global::Celeste.Player player;

        public OdinDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public OdinDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Odin);
            Sprite.Play("idle", false, false);
            Sprite.Scale.X = -1f;
            Hair.Border = Color.Black;
            Hair.Facing = Facings.Left;
            Add(Hair);
            Add(Sprite);
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            Add(Wave = new SineWave(0.2f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = Vector2.UnitY * f * 2.5f;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Blue, 1f, 20, 60));
            
            Add(new PlayerCollider(new Action<global::Celeste.Player>(OnPlayer), null, null));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            player = scene.Tracker.GetEntity<global::Celeste.Player>();
        }

        public override void Update()
        {
            if (player != null && Scene.OnInterval(0.1f))
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
                
                if (player.Speed.X != 0f)
                {
                    Sprite.Scale.X = Math.Sign(player.Speed.X) * -1f;
                }
            }
            
            base.Update();
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Allow player to climb on Odin - provides perseverance boost
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -125f); // perseverance-powered boost
                Audio.Play("event:/char/madeline/jump", Position);
                SceneAs<Level>().Particles.Emit(P_Vanish, 10, Center, Vector2.One * 7f);
            }
        }
    }
}



