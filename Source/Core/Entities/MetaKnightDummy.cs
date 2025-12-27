namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/MetaKnightDummy")]
    public class MetaKnightDummy : Entity
    {
        public static ParticleType P_Vanish;
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;
        public float FloatSpeed = 140f; // Faster than normal
        public float FloatAccel = 280f;
        public float Floatness = 2f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Meta Knight specific fields - enhanced dash abilities
        private bool isDashReady = true;
        private float dashCooldown = 0f;
        private const float DASH_COOLDOWN_TIME = 0.5f;
        private SoundSource swordSound;
        private float swordTrailTimer = 0f;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.55f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public MetaKnightDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public MetaKnightDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.4f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.MetaKnight);
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
                    Audio.Play("event:/char/metaknight/footstep", Position);
                }
            };
            
            Add(Wave = new SineWave(0.25f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.DarkBlue, 1f, 20, 60));
            
            swordSound = new SoundSource();
            Add(swordSound);
            
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

        // Meta Knight's enhanced dash ability - can perform aerial sword strikes
        public IEnumerator PerformSwordDash(Vector2 direction)
        {
            if (!isDashReady) yield break;
            
            isDashReady = false;
            dashCooldown = DASH_COOLDOWN_TIME;
            
            swordSound.Play("event:/char/metaknight/sword_dash");
            Sprite.Play("sword_dash", false, false);
            
            // Start sword trail timer
            swordTrailTimer = 0.5f;
            
            Vector2 startPos = Position;
            Vector2 dashDirection = direction.SafeNormalize();
            float dashDistance = 80f;
            float dashSpeed = 400f;
            float dashTime = dashDistance / dashSpeed;
            
            // Add blue dash particles
            SceneAs<Level>().ParticlesBG.Emit(global::Celeste.ParticleTypes.Dust, 12, Center, Vector2.One * 8f);
            
            for (float t = 0f; t < dashTime; t += Engine.DeltaTime)
            {
                Vector2 newPos = startPos + dashDirection * (dashSpeed * t);
                
                // Check for collision with solid
                if (!Scene.CollideCheck<Solid>(newPos))
                {
                    Position = newPos;
                }
                else
                {
                    break; // Stop at solid collision
                }
                
                // Create dash particles for trail effect
                if (swordTrailTimer > 0f)
                {
                    SceneAs<Level>().ParticlesBG.Emit(global::Celeste.ParticleTypes.Dust, 2, Center, Vector2.One * 4f);
                }
                
                yield return null;
            }
            
            swordTrailTimer = 0f;
            Sprite.Play("idle", false, false);
        }

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/metaknight/appear", Position);
                swordSound.Play("event:/char/metaknight/sword_unsheath");
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
            level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/metaknight/disappear", Position);
            swordSound.Play("event:/char/metaknight/sword_sheath");
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
            // Update dash cooldown
            if (dashCooldown > 0f)
            {
                dashCooldown -= Engine.DeltaTime;
                if (dashCooldown <= 0f)
                {
                    isDashReady = true;
                }
            }
            
            // Update sword trail timer
            if (swordTrailTimer > 0f)
            {
                swordTrailTimer -= Engine.DeltaTime;
            }
            
            if (player != null && following)
            {
                Position = Calc.Approach(Position, player.Position, 550f * Engine.DeltaTime); // Faster follow speed
                Sprite.Scale.X = Math.Sign(player.Position.X - Position.X);
                
                // Auto-dash to catch up if player is far away
                float distanceToPlayer = Vector2.Distance(Position, player.Position);
                if (distanceToPlayer > 100f && isDashReady)
                {
                    Vector2 dashDirection = (player.Position - Position).SafeNormalize();
                    Add(new Coroutine(PerformSwordDash(dashDirection)));
                }
            }
            
            if (Sprite.Scale.X != 0f)
            {
                Hair.Facing = (Facings)Math.Sign(Sprite.Scale.X);
            }

            if (Hovering)
            {
                hoveringTimer += Engine.DeltaTime;
                Sprite.Y = (float)(Math.Sin(hoveringTimer * 2f) * 3.0);
            }
            else
            {
                Sprite.Y = Calc.Approach(Sprite.Y, 0f, Engine.DeltaTime * 4f);
            }

            base.Update();
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Allow player to climb on Meta Knight - provides enhanced dash boost
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -140f);
                player.Speed.X += Math.Sign(player.Speed.X) * 40f; // Add horizontal boost
                Audio.Play("event:/char/metaknight/boost", Position);
                swordSound.Play("event:/char/metaknight/sword_boost");
                SceneAs<Level>().Particles.Emit(P_Vanish, 8, Center, Vector2.One * 6f);
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
        }
    }
}



