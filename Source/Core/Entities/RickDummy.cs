namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/RickDummy")]
    public class RickDummy : Entity
    {
        public static ParticleType P_Vanish = new ParticleType
        {
            Size = 1f,
            Color = Color.Brown,
            Color2 = Color.Orange,
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
        public float FloatSpeed = 90f;
        public float FloatAccel = 180f;
        public float Floatness = 1f; // Rick stays grounded
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Rick specific fields - ground dash abilities and wall climbing
        private bool wallClimbing = false;
        private Vector2 climbDirection = Vector2.Zero;
        private SoundSource climbSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.55f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public RickDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public RickDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.4f * index;
            
            Collider = new Hitbox(8f, 8f, -4f, -8f); // Slightly larger for Rick
            Sprite = new PlayerSprite(PlayerSpriteMode.Rick);
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
                    Audio.Play("event:/char/rick/footstep", Position);
                }
            };
            
            Add(Wave = new SineWave(0.25f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Brown, 1f, 20, 60));
            
            climbSound = new SoundSource();
            Add(climbSound);
            
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

        // Rick's wall climbing ability - can climb vertical surfaces
        public IEnumerator ClimbWall(Vector2 direction)
        {
            if (wallClimbing) yield break;
            
            wallClimbing = true;
            climbDirection = direction.SafeNormalize();
            
            climbSound.Play("event:/char/rick/climb_start");
            Sprite.Play("climb", false, false);
            
            // Climb up the wall
            float climbSpeed = 80f;
            float climbDistance = 48f;
            float climbTime = climbDistance / climbSpeed;
            
            Vector2 startPos = Position;
            
            for (float t = 0f; t < climbTime; t += Engine.DeltaTime)
            {
                Vector2 newPos = startPos + climbDirection * (climbSpeed * t);
                
                // Check if still touching wall
                if (Scene.CollideCheck<Solid>(newPos + climbDirection * 2f))
                {
                    Position = newPos;
                }
                else
                {
                    // Reached top of wall
                    Position = newPos + climbDirection * 8f; // Move past wall edge
                    break;
                }
                yield return null;
            }
            
            climbSound.Play("event:/char/rick/climb_end");
            Sprite.Play("idle", false, false);
            wallClimbing = false;
        }

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/rick/appear", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
            level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/rick/disappear", Position);
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
            if (player != null && following && !wallClimbing)
            {
                Position = Calc.Approach(Position, player.Position, 400f * Engine.DeltaTime);
                Sprite.Scale.X = Math.Sign(player.Position.X - Position.X);
                
                // Check if player needs wall climbing help
                Vector2 toPlayer = player.Position - Position;
                if (Math.Abs(toPlayer.Y) > 32f && Math.Abs(toPlayer.X) < 16f) // Player is above
                {
                    // Check for wall to climb
                    Vector2 wallCheck = Position + new Vector2(Math.Sign(toPlayer.X), 0f) * 8f;
                    if (Scene.CollideCheck<Solid>(wallCheck))
                    {
                        Add(new Coroutine(ClimbWall(Vector2.UnitY * -1f)));
                    }
                }
            }
            
            if (Sprite.Scale.X != 0f)
            {
                Hair.Facing = (Facings)Math.Sign(Sprite.Scale.X);
            }

            if (Hovering)
            {
                hoveringTimer += Engine.DeltaTime;
                Sprite.Y = (float)(Math.Sin(hoveringTimer * 1.5f) * 2.0); // Less floating for Rick
            }
            else
            {
                Sprite.Y = Calc.Approach(Sprite.Y, 0f, Engine.DeltaTime * 4f);
            }

            base.Update();
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Allow player to climb on Rick - provides solid ground platform and wall climbing boost
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -110f);
                Audio.Play("event:/char/rick/boost", Position);
                SceneAs<Level>().Particles.Emit(P_Vanish, 6, Center, Vector2.One * 4f);
                
                // If near wall, help player climb
                Vector2 wallDirection = new Vector2(Math.Sign(player.Speed.X), -0.5f);
                Vector2 wallCheck = player.Position + wallDirection * 12f;
                if (Scene.CollideCheck<Solid>(wallCheck))
                {
                    player.Speed.Y = Math.Min(player.Speed.Y, -140f);
                    player.Speed.X += Math.Sign(player.Speed.X) * 20f;
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
            
            // Render climbing indicators if wall climbing
            if (wallClimbing)
            {
                Vector2 climbPos = Position + climbDirection * 8f;
                Draw.Circle(climbPos, 3f, Color.Brown, 6);
            }
        }
    }
}



