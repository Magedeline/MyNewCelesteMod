namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/KirbyClassicDummy")]
    public class KirbyClassicDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.Pink,
            Color2 = Color.White,
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.5f,
            LifeMax = 1.0f,
            SpeedMin = 8f,
            SpeedMax = 16f,
            DirectionRange = (float)Math.PI * 2f
        };

        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;
        public float FloatSpeed = 95f;
        public float FloatAccel = 180f;
        public float Floatness = 1.5f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Kirby Classic specific fields - simple puff abilities
        private bool puffed = false;
        private float puffTime = 0f;
        private const float MAX_PUFF_TIME = 1.8f;
        private SoundSource puffSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.6f; // Classic, steady following
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public KirbyClassicDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public KirbyClassicDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.4f * index;
            
            Collider = new Hitbox(5f, 5f, -2.5f, -6f); // Smaller classic Kirby
            Sprite = new PlayerSprite(PlayerSpriteMode.KirbyClassic);
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
                if ((anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 4)) || 
                    (anim == "runSlow" && (currentAnimationFrame == 0 || currentAnimationFrame == 4)))
                {
                    Audio.Play("event:/char/kirby_classic/footstep", Position);
                }
                else if (anim == "puff" && currentAnimationFrame == 1)
                {
                    Audio.Play("event:/char/kirby_classic/puff_up", Position);
                }
            };
            
            Add(Wave = new SineWave(0.15f, 0f)); // Gentle floating
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -6f), Color.Pink, 0.8f, 18, 50));
            
            puffSound = new SoundSource();
            Add(puffSound);
            
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
                Vector2 targetPosition = player.Position + new Vector2(index * 16f - 32f, -12f);
                float distance = Vector2.Distance(Position, targetPosition);
                
                if (distance > 28f)
                {
                    // Classic Kirby movement - simple and reliable
                    Vector2 direction = (targetPosition - Position).SafeNormalize();
                    Position += direction * FloatSpeed * Engine.DeltaTime;
                }
                else
                {
                    // Stay close with gentle bobbing
                    Vector2 drift = (targetPosition - Position) * 0.12f;
                    Position += drift * Engine.DeltaTime * 60f;
                }
                
                // Handle puff timer
                if (puffTime > 0f)
                {
                    puffTime -= Engine.DeltaTime;
                    if (puffTime <= 0f)
                    {
                        puffed = false;
                        Sprite.Play("idle", false, false);
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
            // Allow player to climb on Kirby Classic - provides classic puff boost
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -125f); // Moderate boost
                Audio.Play("event:/char/kirby_classic/boost", Position);
                SceneAs<Level>().Particles.Emit(P_Vanish, 8, Center, Vector2.One * 5f);
                
                // Kirby Classic's special ability: puff up to slow fall temporarily
                puffTime = MAX_PUFF_TIME;
                puffed = true;
                Sprite.Play("puff", false, false);
                
                puffSound.Play("event:/char/kirby_classic/puff_ability");
                
                // Create puff assistance
                Add(new Coroutine(CreatePuffAssistance(player)));
            }
        }

        private IEnumerator CreatePuffAssistance(global::Celeste.Player targetPlayer)
        {
            // While puffed, provide gentle upward lift to nearby player
            float assistTime = MAX_PUFF_TIME;
            
            while (assistTime > 0f && targetPlayer != null)
            {
                assistTime -= Engine.DeltaTime;
                
                // If player is falling and near Kirby Classic, provide gentle lift
                float distance = Vector2.Distance(targetPlayer.Position, Position);
                if (distance < 40f && targetPlayer.Speed.Y > 50f)
                {
                    targetPlayer.Speed.Y *= 0.95f; // Gentle fall reduction
                    
                    // Puff particles
                    SceneAs<Level>().ParticlesFG.Emit(P_Vanish, 1, 
                        Position + new Vector2(Calc.Random.Range(-8f, 8f), Calc.Random.Range(-4f, 4f)), 
                        Vector2.One * 2f);
                }
                
                yield return null;
            }
            
            yield break;
        }

        public override void Render()
        {
            // Render with puff glow when puffed
            if (puffed)
            {
                float puffGlow = (float)Math.Sin(puffTime * 6f) * 0.3f + 0.5f;
                Draw.Circle(Position, 10f, Color.Pink * puffGlow, 2);
            }
            
            base.Render();
        }
    }
}



