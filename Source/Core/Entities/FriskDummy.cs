namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/FriskDummy")]
    public class FriskDummy : Entity
    {
        public static ParticleType P_Vanish;
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;
        public float FloatSpeed = 110f;
        public float FloatAccel = 220f;
        public float Floatness = 2f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Frisk specific fields - determination ability
        private bool determinationActive = false;
        private float determinationTimer = 0f;
        private const float DETERMINATION_DURATION = 3f;
        private SoundSource determinationSound;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.55f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public FriskDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
            followBehindIndexDelay = 0.4f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Frisk);
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
                    Audio.Play("event:/char/frisk/footstep", Position);
                }
            };
            
            Add(Wave = new SineWave(0.25f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Red, 1f, 20, 60));
            
            determinationSound = new SoundSource();
            Add(determinationSound);
            
            Add(new PlayerCollider(OnPlayer, null, null));
        }

        // Constructor for loading from map data
        public FriskDummy(EntityData data, Vector2 offset)
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

        // Frisk's determination ability - temporary invincibility and speed boost
        public IEnumerator ActivateDetermination()
        {
            if (determinationActive) yield break;
            
            determinationActive = true;
            determinationTimer = DETERMINATION_DURATION;
            
            determinationSound.Play("event:/char/frisk/determination");
            Sprite.Play("determination", false, false);
            
            // Determination glow effect
            Light.Color = Color.Yellow;
            Light.Alpha = 1.5f;
            
            // Add red heart particles
            SceneAs<Level>().ParticlesBG.Emit(Strawberry.PGlow, 15, Center, Vector2.One * 12f);
            
            while (determinationTimer > 0f)
            {
                determinationTimer -= Engine.DeltaTime;
                
                // Pulse the light
                Light.Alpha = 1f + (float)Math.Sin(determinationTimer * 10f) * 0.5f;
                
                // Speed boost for player if nearby
                if (player != null && Vector2.Distance(Position, player.Position) < 32f)
                {
                    player.Speed *= 1.1f; // Small speed multiplier
                }
                
                yield return null;
            }
            
            // End determination
            Light.Color = Color.Red;
            Light.Alpha = 1f;
            Sprite.Play("idle", false, false);
            determinationActive = false;
        }

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/frisk/appear", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
            level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/frisk/disappear", Position);
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
                Position = Calc.Approach(Position, player.Position, 480f * Engine.DeltaTime);
                Sprite.Scale.X = Math.Sign(player.Position.X - Position.X);
                
                // Activate determination when player is in danger (low health, falling, etc.)
                if (player.Speed.Y > 150f && !determinationActive) // Player is falling fast
                {
                    Add(new Coroutine(ActivateDetermination()));
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
            // Allow player to climb on Frisk - provides determination boost
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -120f);
                Audio.Play("event:/char/frisk/boost", Position);
                SceneAs<Level>().Particles.Emit(Strawberry.PGlow, 8, Center, Vector2.One * 6f);
                
                // Brief determination activation
                if (!determinationActive)
                {
                    Add(new Coroutine(ActivateDetermination()));
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
            
            // Render determination heart if active
            if (determinationActive)
            {
                Vector2 heartPos = Position + Vector2.UnitY * -16f;
                Draw.Rect(heartPos.X - 2f, heartPos.Y - 2f, 4f, 4f, Color.Red);
            }
        }
    }
}



