namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/BandanaWaddleDeeDummy")]
    public class BandanaWaddleDeeDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.Orange,
            Color2 = Color.Yellow,
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
        public float FloatSpeed = 100f;
        public float FloatAccel = 200f;
        public float Floatness = 2f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Bandana Waddle Dee specific fields - spear abilities
        private bool spearActive = false;
        private Vector2 spearDirection = Vector2.Zero;
        private float spearChargeTime = 0f;
        private const float MAX_SPEAR_CHARGE = 1.5f;
        private SoundSource spearSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.55f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public BandanaWaddleDeeDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public BandanaWaddleDeeDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            followBehindIndexDelay = 0.4f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.BandanaWaddleDee);
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
                    Audio.Play("event:/char/bandana_waddle_dee/footstep", Position);
                }
            };
            
            Add(Wave = new SineWave(0.25f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Orange, 1f, 20, 60));
            
            spearSound = new SoundSource();
            Add(spearSound);
            
            Add(new PlayerCollider(OnPlayer, null, null));
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            if (index > 0) // Only start following if we're a follower instance
            {
                Add(new Coroutine(StartFollowing(scene as Level)));
            }
        }

        public IEnumerator StartFollowing(Level level)
        {
            Hovering = true;
            while ((player = Scene.Tracker.GetEntity<global::Celeste.Player>()) == null || player.Dead)
            {
                yield return null;
            }

            Vector2 to = player.Position;
            yield return followBehindIndexDelay;

            if (!Visible)
            {
                PopIntoExistence(0.5f);
            }

            Sprite.Play("walk", false, false);
            Hovering = false;
            yield return TweenToPlayer(to);
            
            Collidable = true;
            following = true;
        }

        private IEnumerator TweenToPlayer(Vector2 to)
        {
            Vector2 from = Position;
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, followBehindTime - 0.1f, true);
            tween.OnUpdate = delegate(Tween t)
            {
                Position = Vector2.Lerp(from, to, t.Eased);
                if (to.X != from.X)
                {
                    Sprite.Scale.X = Math.Abs(Sprite.Scale.X) * Math.Sign(to.X - from.X);
                }
            };
            Add(tween);
            yield return tween.Duration;
        }

        // Bandana Waddle Dee's spear thrust - creates platforms for dashing
        public IEnumerator PerformSpearThrust(Vector2 direction)
        {
            if (spearActive) yield break;
            
            spearActive = true;
            spearDirection = direction.SafeNormalize();
            
            spearSound.Play("event:/char/bandana_waddle_dee/spear_charge");
            Sprite.Play("spear_charge", false, false);
            
            // Charge up spear
            spearChargeTime = 0f;
            while (spearChargeTime < MAX_SPEAR_CHARGE)
            {
                spearChargeTime += Engine.DeltaTime;
                Light.Alpha = 0.5f + (spearChargeTime / MAX_SPEAR_CHARGE) * 0.5f;
                yield return null;
            }
            
            // Thrust!
            spearSound.Play("event:/char/bandana_waddle_dee/spear_thrust");
            Sprite.Play("spear_thrust", false, false);
            
            // Create temporary spear platform for player to dash on
            Vector2 spearEnd = Position + spearDirection * 48f;
            Add(new Coroutine(CreateSpearPlatform(spearEnd)));
            
            yield return 0.5f;
            
            // Retract spear
            Sprite.Play("idle", false, false);
            Light.Alpha = 1f;
            spearActive = false;
        }

        private IEnumerator CreateSpearPlatform(Vector2 position)
        {
            // Create a temporary solid platform that player can dash off of
            Entity spearPlatform = new Entity(position)
            {
                Collider = new Hitbox(24f, 4f, -12f, -2f),
                Collidable = true
            };
            
            Scene.Add(spearPlatform);
            
            // Platform lasts for 2 seconds
            yield return 2f;
            
            if (spearPlatform.Scene != null)
            {
                spearPlatform.RemoveSelf();
            }
        }

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/bandana_waddle_dee/appear", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
            level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/bandana_waddle_dee/disappear", Position);
            Shockwave();
            SceneAs<Level>().Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
            RemoveSelf();
        }

        private void Shockwave()
        {
            SceneAs<Level>().Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
        }

        public override void Update()
        {
            if (player != null && following)
            {
                Position = Calc.Approach(Position, player.Position, 450f * Engine.DeltaTime);
                Sprite.Scale.X = Math.Sign(player.Position.X - Position.X);
                
                // Help player by creating spear platforms when they need them
                if (player.Speed.Y > 100f && !spearActive) // Player is falling
                {
                    Vector2 spearDirection = new Vector2(Math.Sign(player.Speed.X), 0.2f).SafeNormalize();
                    Add(new Coroutine(PerformSpearThrust(spearDirection)));
                }
            }
            
            if (Sprite.Scale.X != 0f)
            {
                Hair.Facing = (Facings)Math.Sign(Sprite.Scale.X);
            }

            if (Hovering)
            {
                hoveringTimer += Engine.DeltaTime;
                Sprite.Y = (float)(Math.Sin(hoveringTimer * 2f) * 4.0);
            }
            else
            {
                Sprite.Y = Calc.Approach(Sprite.Y, 0f, Engine.DeltaTime * 4f);
            }

            base.Update();
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Allow player to climb on Bandana Waddle Dee - provides stable platform
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -100f);
                Audio.Play("event:/char/bandana_waddle_dee/boost", Position);
                SceneAs<Level>().Particles.Emit(P_Vanish, 6, Center, Vector2.One * 4f);
                
                // Create a temporary spear platform for enhanced jumping
                Add(new Coroutine(CreateSpearPlatform(player.Position + Vector2.UnitY * 16f)));
            }
        }

        private void PopIntoExistence(float duration)
        {
            Visible = true;
            Sprite.Scale = Vector2.Zero;
            Sprite.Color = Color.Transparent;
            Hair.Visible = false;
            
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, duration, true);
            tween.OnUpdate = delegate(Tween t)
            {
                Sprite.Scale = Vector2.One * t.Eased;
                Sprite.Color = Color.White * t.Eased;
            };
            Add(tween);
        }

        public override void Render()
        {
            Vector2 renderPosition = Sprite.RenderPosition;
            Sprite.RenderPosition = Sprite.RenderPosition.Floor();
            base.Render();
            Sprite.RenderPosition = renderPosition;
            
            // Render spear if active
            if (spearActive)
            {
                Vector2 spearEnd = Position + spearDirection * (48f * (spearChargeTime / MAX_SPEAR_CHARGE));
                Draw.Line(Position, spearEnd, Color.Brown, 3f);
                Draw.Circle(spearEnd, 2f, Color.Silver, 4);
            }
        }
    }
}



