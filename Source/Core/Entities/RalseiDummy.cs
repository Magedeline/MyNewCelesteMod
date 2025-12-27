namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/RalseiDummy")]
    public class RalseiDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.White,
            Color2 = Color.PaleVioletRed,
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
        public float FloatSpeed = 120f;
        public float FloatAccel = 240f;
        public float Floatness = 2f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.55f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;
        protected Dictionary<string, SoundSource> loopingSounds = new();
        protected List<SoundSource> inactiveLoopingSounds = new();

        public RalseiDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
            followBehindIndexDelay = 0.4f * index;
            
            Depth = -1;
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Ralsei);
            Sprite.Play("fallSlow", false, false);
            Sprite.Scale.X = -1f;
            
            // Skip hair initialization to avoid compatibility issues
            Hair = null;
            if (Hair != null)
            {
                Add(Hair);
            }
            Add(Sprite);
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            Sprite.OnFrameChange = delegate(string anim)
            {
                int currentAnimationFrame = Sprite.CurrentAnimationFrame;
                if ((anim == "walk" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || 
                    (anim == "runSlow" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)) || 
                    (anim == "runFast" && (currentAnimationFrame == 0 || currentAnimationFrame == 6)))
                {
                    Audio.Play("event:/char/badeline/footstep", Position);
                }
            };
            
            Add(Wave = new SineWave(0.25f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.PaleVioletRed, 1f, 20, 60));
            
            if (index > 0)
            {
                Add(new PlayerCollider(OnPlayer, null, null));
            }
        }

        // Constructor for loading from map data
        public RalseiDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, 0)
        {
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
            Audio.Play("event:/char/badeline/level_entry", Position);
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

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/badeline/appear", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
            level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/badeline/disappear", Position);
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
            base.Update();
            
            if (player != null && following)
            {
                if (player.Dead)
                {
                    Sprite.Play("laugh", false, false);
                    Sprite.X = (float)(Math.Sin(hoveringTimer) * 4.0);
                    Hovering = true;
                    hoveringTimer += Engine.DeltaTime * 2f;
                    Depth = -12500;
                    foreach (var sound in loopingSounds.Values)
                    {
                        sound.Stop(true);
                    }
                }
                else
                {
                    Position = Calc.Approach(Position, player.Position, 500f * Engine.DeltaTime);
                    Sprite.Scale.X = Math.Sign(player.Position.X - Position.X);
                }
            }

            if (Sprite.Scale.X != 0f && Hair != null)
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
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Friendly interaction - do nothing by default
        }

        private void PopIntoExistence(float duration)
        {
            Visible = true;
            Sprite.Scale = Vector2.Zero;
            Sprite.Color = Color.Transparent;
            if (Hair != null)
            {
                Hair.Visible = false;
            }
            
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, duration, true);
            tween.OnUpdate = delegate(Tween t)
            {
                Sprite.Scale = Vector2.One * t.Eased;
                Sprite.Color = Color.White * t.Eased;
            };
            Add(tween);
        }

        /// <summary>
        /// Float to a target position with specified parameters.
        /// </summary>
        /// <param name="targetPosition">Target position to float to</param>
        /// <param name="faceDirection">Direction to face (-1 left, 1 right, 0 no change)</param>
        /// <param name="useSpeedCalculation">Whether to calculate duration based on speed</param>
        /// <param name="easeIntoMovement">Whether to use easing</param>
        /// <param name="speed">Movement speed in pixels per second</param>
        public IEnumerator FloatTo(Vector2 targetPosition, int faceDirection, bool useSpeedCalculation, bool easeIntoMovement, float speed)
        {
            if (faceDirection != 0)
            {
                Sprite.Scale.X = faceDirection;
            }
            
            Vector2 startPosition = Position;
            float duration = useSpeedCalculation ? Vector2.Distance(startPosition, targetPosition) / speed : 1f;
            if (duration <= 0f) duration = 0.1f;
            
            for (float t = 0f; t < 1f; t += Engine.DeltaTime / duration)
            {
                float eased = easeIntoMovement ? Ease.SineInOut(t) : t;
                Position = Vector2.Lerp(startPosition, targetPosition, eased);
                yield return null;
            }
            Position = targetPosition;
        }

        public IEnumerator FloatTo(Vector2 targetPosition, int? nodeID, bool useSpeedCalculation, bool easeIntoMovement, float speed)
        {
            Vector2 startPosition = Position;
            float duration = useSpeedCalculation ? Vector2.Distance(startPosition, targetPosition) / speed : 1f;
            
            for (float t = 0f; t < 1f; t += Engine.DeltaTime / duration)
            {
                float eased = easeIntoMovement ? Ease.SineInOut(t) : t;
                Position = Vector2.Lerp(startPosition, targetPosition, eased);
                yield return null;
            }
            Position = targetPosition;
        }

        // Overload that matches the original usage pattern
        public IEnumerator FloatTo(Vector2 targetPosition, bool p1, bool p2, bool p3, float speed)
        {
            Vector2 startPosition = Position;
            float duration = Vector2.Distance(startPosition, targetPosition) / speed;
            
            for (float t = 0f; t < 1f; t += Engine.DeltaTime / duration)
            {
                float eased = p3 ? Ease.SineInOut(t) : t;
                Position = Vector2.Lerp(startPosition, targetPosition, eased);
                yield return null;
            }
            Position = targetPosition;
        }

        public IEnumerator WalkTo(float targetX)
        {
            if (Sprite != null)
            {
                Sprite.Play("walk");
                Sprite.Scale.X = Math.Sign(targetX - Position.X);
            }
            
            Vector2 startPosition = Position;
            Vector2 targetPosition = new Vector2(targetX, Position.Y);
            float duration = Math.Abs(targetX - Position.X) / 48f; // Walking speed similar to player
            
            for (float t = 0f; t < 1f; t += Engine.DeltaTime / duration)
            {
                Position = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
            Position = targetPosition;
            
            if (Sprite != null)
            {
                Sprite.Play("idle");
            }
        }

        internal IEnumerator FloatTo(Vector2 vector2, object value, bool v)
        {
            throw new NotImplementedException();
        }
    }
}




