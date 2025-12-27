namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/CooDummy")]
    public class CooDummy : Entity
    {
        public static ParticleType P_Vanish;
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;
        public float FloatSpeed = 150f; // Fast flying
        public float FloatAccel = 300f;
        public float Floatness = 4f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Coo specific fields - flying and wind dash abilities
        private bool isFlying = true;
        private float wingFlapTimer = 0f;
        private SoundSource flySound;
        private Vector2 flyVelocity = Vector2.Zero;
        private bool creatingWindCurrent = false;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.55f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public CooDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public CooDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.4f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -3f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Coo);
            Sprite.Play("fly", false, false);
            Sprite.Scale.X = -1f;
            Hair.Border = Color.Black;
            Hair.Facing = Facings.Left;
            Add(Hair);
            Add(Sprite);
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            Sprite.OnFrameChange = delegate(string anim)
            {
                int currentAnimationFrame = Sprite.CurrentAnimationFrame;
                if (anim == "fly" && (currentAnimationFrame == 0 || currentAnimationFrame == 3))
                {
                    Audio.Play("event:/char/coo/wing_flap", Position);
                }
            };
            
            Add(Wave = new SineWave(0.3f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.LightBlue, 1f, 20, 60));
            
            flySound = new SoundSource();
            Add(flySound);
            
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

            Vector2 to = player.Position + Vector2.UnitY * -20f; // Fly above player
            yield return followBehindIndexDelay;

            if (!Visible)
            {
                PopIntoExistence(0.5f);
            }

            Sprite.Play("fly", false, false);
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

        // Coo's wind dash - creates updrafts and air currents for player
        public IEnumerator CreateWindCurrent(Vector2 direction)
        {
            if (creatingWindCurrent) yield break;
            
            creatingWindCurrent = true;
            flySound.Play("event:/char/coo/wind_dash");
            Sprite.Play("wind_dash", false, false);
            
            Vector2 windDirection = direction.SafeNormalize();
            Vector2 windPos = Position + windDirection * 40f;
            
            // Create wind current effect that lifts player
            Entity windCurrent = new Entity(windPos)
            {
                Collider = new Hitbox(32f, 48f, -16f, -24f),
                Collidable = false
            };
            
            // Add wind effect component
            windCurrent.Add(new PlayerCollider(OnPlayerInWind));
            Scene.Add(windCurrent);
            
            // Create wind particles
            for (int i = 0; i < 30; i++)
            {
                Vector2 particlePos = windPos + new Vector2(
                    (float)(Calc.Random.NextDouble() - 0.5) * 32f,
                    (float)(Calc.Random.NextDouble() - 0.5) * 48f
                );
                SceneAs<Level>().ParticlesFG.Emit(global::Celeste.ParticleTypes.Dust, particlePos);
            }
            
            // Wind current lasts for 3 seconds
            yield return 3f;
            
            if (windCurrent.Scene != null)
            {
                windCurrent.RemoveSelf();
            }
            
            Sprite.Play("fly", false, false);
            creatingWindCurrent = false;
        }

        private void OnPlayerInWind(global::Celeste.Player player)
        {
            // Give player upward lift and horizontal assist
            player.Speed.Y = Math.Min(player.Speed.Y, player.Speed.Y - 40f * Engine.DeltaTime);
            
            // Horizontal wind assist
            if (Math.Abs(player.Speed.X) < 200f)
            {
                player.Speed.X += Math.Sign(player.Speed.X) * 20f * Engine.DeltaTime;
            }
            
            // Add wind particles around player
            if (Calc.Random.Chance(0.3f))
            {
                SceneAs<Level>().ParticlesFG.Emit(global::Celeste.ParticleTypes.Dust, player.Position + new Vector2(
                    (float)(Calc.Random.NextDouble() - 0.5) * 16f,
                    (float)(Calc.Random.NextDouble() - 0.5) * 16f
                ));
            }
        }

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/coo/appear", Position);
                flySound.Play("event:/char/coo/wing_flap");
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
            level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/coo/disappear", Position);
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
            wingFlapTimer += Engine.DeltaTime;
            
            if (player != null && following)
            {
                // Fly above and slightly behind player
                Vector2 targetPos = player.Position + new Vector2(-16f * Math.Sign(Sprite.Scale.X), -24f);
                Vector2 diff = targetPos - Position;
                
                // Create wind current if player is falling or struggling
                if (player.Speed.Y > 120f && !creatingWindCurrent)
                {
                    Add(new Coroutine(CreateWindCurrent(Vector2.UnitY * -1f)));
                }
                
                // Smooth flying movement
                flyVelocity = Vector2.Lerp(flyVelocity, diff.SafeNormalize() * FloatSpeed, Engine.DeltaTime * 3f);
                Position += flyVelocity * Engine.DeltaTime;
                
                // Face movement direction
                if (flyVelocity.X != 0f)
                {
                    Sprite.Scale.X = Math.Sign(flyVelocity.X);
                }
            }
            
            if (Sprite.Scale.X != 0f)
            {
                Hair.Facing = (Facings)Math.Sign(Sprite.Scale.X);
            }

            // Flying animation - smooth bobbing motion
            Sprite.Y = (float)(Math.Sin(wingFlapTimer * 4f) * 3.0);
            Sprite.Rotation = (float)(Math.Sin(wingFlapTimer * 2f) * 0.05f);

            base.Update();
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            // Allow player to grab onto Coo - provides flying carry ability
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 8f)
            {
                // Carry player upward
                player.Speed.Y = Math.Min(player.Speed.Y, -130f);
                player.Speed.X += Math.Sign(Sprite.Scale.X) * 30f;
                
                Audio.Play("event:/char/coo/carry", Position);
                flySound.Play("event:/char/coo/wing_flap");
                
                // Create wind particles
                SceneAs<Level>().ParticlesFG.Emit(global::Celeste.ParticleTypes.Dust, 8, Center, Vector2.One * 12f);
                
                // Create wind current to help player
                Vector2 carryDirection = new Vector2(Math.Sign(Sprite.Scale.X), -0.7f);
                Add(new Coroutine(CreateWindCurrent(carryDirection)));
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
            
            // Render wind trail effect when flying fast
            if (flyVelocity.Length() > 80f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 trailPos = Position - flyVelocity.SafeNormalize() * i * 6f;
                    float alpha = (4f - i) / 4f * 0.4f;
                    Draw.Circle(trailPos, 1.5f, Color.LightBlue * alpha, 6);
                }
            }
        }
    }
}



