namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/MelodyDummy")]
    public class MelodyDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.White,
            Color2 = Color.Purple,
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
        public float FloatSpeed = 105f;
        public float FloatAccel = 210f;
        public float Floatness = 2.8f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Melody specific fields - music and rhythm abilities
        private bool musicActive = false;
        private float musicTime = 0f;
        private const float MAX_MUSIC_TIME = 3.5f;
        private SoundSource musicSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.4f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public MelodyDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public MelodyDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.35f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Melody);
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
            
            Add(Wave = new SineWave(0.3f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Purple, 1f, 24, 72));
            
            musicSound = new SoundSource();
            Add(musicSound);
            
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
                Vector2 targetPosition = player.Position + new Vector2(index * 18f - 36f, -20f);
                float distance = Vector2.Distance(Position, targetPosition);
                
                if (distance > 24f)
                {
                    Vector2 direction = (targetPosition - Position).SafeNormalize();
                    Position += direction * FloatSpeed * Engine.DeltaTime;
                }
                else
                {
                    Vector2 drift = (targetPosition - Position) * 0.08f;
                    Position += drift * Engine.DeltaTime;
                }
                
                // Handle music timer
                if (musicTime > 0f)
                {
                    musicTime -= Engine.DeltaTime;
                    if (musicTime <= 0f)
                    {
                        musicActive = false;
                    }
                }
                
                // Face player direction with musical sway
                if (player.Speed.X != 0f)
                {
                    Sprite.Scale.X = Math.Sign(player.Speed.X) * -1f;
                }
            }
            
            base.Update();
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Allow player to climb on Melody - provides music-powered boost and rhythm assistance
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -125f); // Music-powered boost
                Audio.Play("event:/char/madeline/jump", Position);
                musicSound.Play("event:/char/madeline/dash_pink_right");
                
                SceneAs<Level>().Particles.Emit(P_Vanish, 12, Center, Vector2.One * 8f);
                
                // Activate music - creates rhythmic assistance
                musicTime = MAX_MUSIC_TIME;
                musicActive = true;
                
                // Create musical notes that provide rhythm assistance
                Add(new Coroutine(CreateMusicalNotes(player.Position)));
            }
        }

        private IEnumerator CreateMusicalNotes(Vector2 position)
        {
            // Create musical notes that provide rhythmic platforming assistance
            for (float t = 0f; t < 2.5f; t += Engine.DeltaTime)
            {
                if (t % 0.25f < Engine.DeltaTime) // Beat timing
                {
                    SceneAs<Level>().ParticlesFG.Emit(P_Vanish, 3, 
                        position + Vector2.UnitX * Calc.Random.Range(-16f, 16f) + Vector2.UnitY * Calc.Random.Range(-8f, 8f), 
                        Vector2.One * 2f);
                }
                
                // Musical rhythm provides timing assistance
                if (player != null && Vector2.Distance(player.Position, position) < 32f)
                {
                    // On beat, provide slight assistance
                    if ((int)(t * 4f) % 2 == 0 && player.Speed.Y > 0f)
                    {
                        player.Speed.Y *= 0.97f; // Rhythmic fall reduction
                    }
                }
                
                yield return null;
            }
            
            yield break;
        }

        public override void Render()
        {
            // Render with musical note effects when active
            if (musicActive)
            {
                float pulse = (float)Math.Sin(musicTime * 8f) * 0.5f + 0.5f;
                Draw.Circle(Position, 8f + pulse * 3f, Color.Purple * (0.3f + pulse * 0.2f), 3);
                
                // Draw musical notes floating around
                for (int i = 0; i < 4; i++)
                {
                    float angle = musicTime * 2f + i * MathHelper.PiOver2;
                    Vector2 notePos = Position + Calc.AngleToVector(angle, 12f + pulse * 4f);
                    Draw.Point(notePos, Color.Purple * 0.8f);
                }
            }
            
            base.Render();
        }
    }
}



