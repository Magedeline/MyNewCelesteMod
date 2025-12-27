namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/GooeySiDummy")]
    public class GooeySiDummy : Entity
    {
        public static ParticleType P_Vanish;
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;
        public float FloatSpeed = 120f;
        public float FloatAccel = 240f;
        public float Floatness = 2f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Gooey-specific fields for tongue ability
        private bool tongueExtended = false;
        private Vector2 tongueDirection = Vector2.Zero;
        private float tongueLength = 0f;
        private const float MAX_TONGUE_LENGTH = 64f;
        private SoundSource tongueSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.55f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public GooeySiDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public GooeySiDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.4f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Gooey);
            Sprite.Play("fallSlow", false, false);
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
                    Audio.Play("event:/char/gooey/footstep", Position);
                }
            };
            
            Add(Wave = new SineWave(0.25f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.CornflowerBlue, 1f, 20, 60));
            
            tongueSound = new SoundSource();
            Add(tongueSound);
            
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

        // Gooey's unique tongue ability - can grab walls to assist with movement
        public IEnumerator UseTongueGrab(Vector2 direction)
        {
            if (tongueExtended) yield break;
            
            tongueExtended = true;
            tongueDirection = direction.SafeNormalize();
            tongueSound.Play("event:/char/gooey/tongue_extend");
            
            Sprite.Play("tongue_extend", false, false);
            
            // Extend tongue to find grabbable surface
            float tongueSpeed = 200f;
            while (tongueLength < MAX_TONGUE_LENGTH)
            {
                tongueLength += tongueSpeed * Engine.DeltaTime;
                Vector2 tongueEnd = Position + tongueDirection * tongueLength;
                
                // Check for solid surface
                if (Scene.CollideCheck<Solid>(tongueEnd))
                {
                    // Found surface - pull towards it
                    yield return PullToSurface(tongueEnd);
                    break;
                }
                yield return null;
            }
            
            // Retract tongue
            yield return RetractTongue();
        }

        private IEnumerator PullToSurface(Vector2 target)
        {
            Vector2 startPos = Position;
            Vector2 pullDirection = (target - Position).SafeNormalize();
            float pullDistance = Vector2.Distance(Position, target) - 16f; // Stop short of surface
            
            Sprite.Play("tongue_pull", false, false);
            tongueSound.Play("event:/char/gooey/tongue_pull");
            
            float pullSpeed = 150f;
            float pullTime = pullDistance / pullSpeed;
            
            for (float t = 0f; t < pullTime; t += Engine.DeltaTime)
            {
                Position = startPos + pullDirection * (pullSpeed * t);
                yield return null;
            }
        }

        private IEnumerator RetractTongue()
        {
            Sprite.Play("tongue_retract", false, false);
            tongueSound.Play("event:/char/gooey/tongue_retract");
            
            while (tongueLength > 0f)
            {
                tongueLength = Calc.Approach(tongueLength, 0f, 300f * Engine.DeltaTime);
                yield return null;
            }
            
            tongueExtended = false;
            Sprite.Play("idle", false, false);
        }

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/gooey/appear", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
            level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/gooey/disappear", Position);
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
                Position = Calc.Approach(Position, player.Position, 500f * Engine.DeltaTime);
                Sprite.Scale.X = Math.Sign(player.Position.X - Position.X);
                
                // Check if player needs help - use tongue grab if stuck
                if (player.Speed.Length() < 10f && !tongueExtended)
                {
                    Vector2 direction = new Vector2(Sprite.Scale.X, -0.5f);
                    Add(new Coroutine(UseTongueGrab(direction)));
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
            // Allow player to climb on Gooey - boost upward with tongue grab ability
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -120f);
                Audio.Play("event:/char/gooey/boost", Position);
                SceneAs<Level>().Particles.Emit(P_Vanish, 6, Center, Vector2.One * 4f);
                
                // Gooey's special ability: tongue grab to nearby walls
                Vector2 wallCheckLeft = Position + Vector2.UnitX * -24f;
                Vector2 wallCheckRight = Position + Vector2.UnitX * 24f;
                
                bool wallLeft = SceneAs<Level>().CollideCheck<Solid>(wallCheckLeft);
                bool wallRight = SceneAs<Level>().CollideCheck<Solid>(wallCheckRight);
                
                if (wallLeft || wallRight)
                {
                    // Pull player toward the nearest wall using "tongue"
                    Vector2 wallDirection = wallLeft ? Vector2.UnitX * -1f : Vector2.UnitX;
                    player.Speed.X += wallDirection.X * 60f;
                    
                    Audio.Play("event:/char/gooey/tongue_grab", Position);
                    
                    // Visual tongue effect
                    SceneAs<Level>().ParticlesFG.Emit(global::Celeste.ParticleTypes.Dust, 8, 
                        Position + wallDirection * 12f, Vector2.One * 6f);
                }
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
            
            // Render tongue if extended
            if (tongueExtended && tongueLength > 0f)
            {
                Vector2 tongueEnd = Position + tongueDirection * tongueLength;
                Draw.Line(Position, tongueEnd, Color.Pink, 2f);
            }
        }
    }
}



