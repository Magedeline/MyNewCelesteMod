namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/KineDummy")]
    public class KineDummy : Entity
    {
        public static ParticleType P_Vanish;
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;
        public float FloatSpeed = 130f; // Fast swimming
        public float FloatAccel = 260f;
        public float Floatness = 3f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Kine specific fields - swimming and dash abilities
        private bool isSwimming = true; // Kine always "swims" through air
        private float swimTimer = 0f;
        private SoundSource swimSound;
        private Vector2 swimVelocity = Vector2.Zero;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.55f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public KineDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public KineDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.4f * index;
            
            Collider = new Hitbox(6f, 8f, -3f, -4f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Kine);
            Sprite.Play("swim", false, false);
            Sprite.Scale.X = -1f;
            Hair.Border = Color.Black;
            Hair.Facing = Facings.Left;
            Add(Hair);
            Add(Sprite);
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            Sprite.OnFrameChange = delegate(string anim)
            {
                int currentAnimationFrame = Sprite.CurrentAnimationFrame;
                if (anim == "swim" && currentAnimationFrame == 0)
                {
                    Audio.Play("event:/char/kine/swim", Position);
                }
            };
            
            Add(Wave = new SineWave(0.4f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.CornflowerBlue, 1f, 20, 60));
            
            swimSound = new SoundSource();
            Add(swimSound);
            
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

            Sprite.Play("swim", false, false);
            Hovering = false;
            yield return TweenToPlayer(to);
            
            Collidable = true;
            following = true;
        }

        private IEnumerator TweenToPlayer(Vector2 to)
        {
            Vector2 from = Position;
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, followBehindTime - 0.1f, true);
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

        // Kine's water dash - creates temporary water current for player
        public IEnumerator CreateWaterCurrent(Vector2 direction)
        {
            swimSound.Play("event:/char/kine/water_dash");
            Sprite.Play("dash", false, false);
            
            Vector2 currentDirection = direction.SafeNormalize();
            Vector2 currentPos = Position + currentDirection * 32f;
            
            // Create water current effect that helps player movement
            Entity waterCurrent = new Entity(currentPos)
            {
                Collider = new Hitbox(48f, 16f, -24f, -8f),
                Collidable = false // Non-solid but detectable
            };
            
            // Add water current component that affects player movement
            waterCurrent.Add(new PlayerCollider(OnPlayerInCurrent));
            Scene.Add(waterCurrent);
            
            // Emit water particles
            for (int i = 0; i < 20; i++)
            {
                Vector2 particlePos = currentPos + new Vector2(
                    (float)(Calc.Random.NextDouble() - 0.5) * 48f,
                    (float)(Calc.Random.NextDouble() - 0.5) * 16f
                );
                SceneAs<Level>().ParticlesFG.Emit(global::Celeste.ParticleTypes.Dust, particlePos);
            }
            
            // Current lasts for 2 seconds
            yield return 2f;
            
            if (waterCurrent.Scene != null)
            {
                waterCurrent.RemoveSelf();
            }
            
            Sprite.Play("swim", false, false);
        }

        private void OnPlayerInCurrent(global::Celeste.Player player)
        {
            // Give player water movement boost
            if (player.Speed.Length() > 0f)
            {
                Vector2 boost = player.Speed.SafeNormalize() * 30f;
                player.Speed += boost * Engine.DeltaTime;
            }
            
            // Reduce falling speed in current
            if (player.Speed.Y > 0f)
            {
                player.Speed.Y *= 0.8f;
            }
        }

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/kine/appear", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
            level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/kine/disappear", Position);
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
            swimTimer += Engine.DeltaTime;
            
            if (player != null && following)
            {
                // Smooth swimming movement
                Vector2 targetPos = player.Position;
                Vector2 diff = targetPos - Position;
                
                if (diff.Length() > 80f)
                {
                    // Create water current to help catch up
                    Add(new Coroutine(CreateWaterCurrent(diff.SafeNormalize())));
                }
                
                // Smooth swimming approach
                swimVelocity = Vector2.Lerp(swimVelocity, diff.SafeNormalize() * FloatSpeed, Engine.DeltaTime * 2f);
                Position += swimVelocity * Engine.DeltaTime;
                
                // Face movement direction
                if (swimVelocity.X != 0f)
                {
                    Sprite.Scale.X = Math.Sign(swimVelocity.X);
                }
            }
            
            if (Sprite.Scale.X != 0f)
            {
                Hair.Facing = (Facings)Math.Sign(Sprite.Scale.X);
            }

            // Swimming animation and movement
            Sprite.Y = (float)(Math.Sin(swimTimer * 3f) * 6.0);
            Sprite.Rotation = (float)(Math.Sin(swimTimer * 2f) * 0.1f);

            base.Update();
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Allow player to ride Kine - provides water-like movement boost
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -100f);
                player.Speed.X += Math.Sign(player.Speed.X) * 50f; // Strong horizontal boost
                Audio.Play("event:/char/kine/boost", Position);
                
                // Create water splash effect
                SceneAs<Level>().ParticlesFG.Emit(global::Celeste.ParticleTypes.Dust, 10, Center, Vector2.One * 8f);
                
                // Create temporary water current
                Vector2 currentDir = new Vector2(Math.Sign(player.Speed.X), -0.3f);
                Add(new Coroutine(CreateWaterCurrent(currentDir)));
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
            
            // Render water trail effect
            if (swimVelocity.Length() > 50f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 trailPos = Position - swimVelocity.SafeNormalize() * i * 8f;
                    float alpha = (3f - i) / 3f * 0.5f;
                    Draw.Circle(trailPos, 2f, Color.CornflowerBlue * alpha, 4);
                }
            }
        }
    }
}



