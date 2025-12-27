namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/NessDummy")]
    public class NessDummy : Entity
    {
        public static ParticleType P_Vanish;
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;
        public float FloatSpeed = 115f;
        public float FloatAccel = 230f;
        public float Floatness = 2.5f;
        public Vector2 floatNormal = new Vector2(0f, 1f);
        
        // Ness specific fields - PSI abilities
        private bool psiActive = false;
        private float psiCooldown = 0f;
        private const float PSI_COOLDOWN_TIME = 4f;
        private SoundSource psiSound;
        private Vector2 psiTargetPos = Vector2.Zero;
        
        // Follower specific fields
        protected int index;
        protected global::Celeste.Player player;
        protected bool following;
        protected float followBehindTime = 1.55f;
        protected float followBehindIndexDelay;
        public bool Hovering;
        protected float hoveringTimer;

        public NessDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
        }

        // Standard Everest constructor for map loading
        public NessDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            this.index = index;
            followBehindIndexDelay = 0.4f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -6f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Ness);
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
                    Audio.Play("event:/char/ness/footstep", Position);
                }
            };
            
            Add(Wave = new SineWave(0.25f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = floatNormal * f * Floatness;
            };
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.MediumPurple, 1f, 20, 60));
            
            psiSound = new SoundSource();
            Add(psiSound);
            
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

        // Ness's PSI Teleport - creates temporary platforms and teleports to help player
        public IEnumerator UsePSITeleport(Vector2 targetPosition)
        {
            if (psiActive || psiCooldown > 0f) yield break;
            
            psiActive = true;
            psiCooldown = PSI_COOLDOWN_TIME;
            psiTargetPos = targetPosition;
            
            psiSound.Play("event:/char/ness/psi_charge");
            Sprite.Play("psi_charge", false, false);
            
            // PSI charging effect
            Light.Color = Color.Cyan;
            Light.Alpha = 2f;
            
            // Create charging particles
            for (int i = 0; i < 12; i++)
            {
                Vector2 particlePos = Position + new Vector2(
                    (float)(Calc.Random.NextDouble() - 0.5) * 16f,
                    (float)(Calc.Random.NextDouble() - 0.5) * 16f
                );
                SceneAs<Level>().ParticlesFG.Emit(FlyFeather.P_Collect, particlePos);
            }
            
            yield return 0.5f;
            
            // Teleport!
            psiSound.Play("event:/char/ness/psi_teleport");
            
            // Disappear effect
            SceneAs<Level>().Displacement.AddBurst(Position, 0.6f, 16f, 48f, 0.3f, null, null);
            Visible = false;
            
            yield return 0.3f;
            
            // Reappear at target position
            Position = psiTargetPos;
            Visible = true;
            SceneAs<Level>().Displacement.AddBurst(Position, 0.6f, 16f, 48f, 0.3f, null, null);
            
            // Create PSI platform for player
            Add(new Coroutine(CreatePSIPlatform(Position + Vector2.UnitY * 8f)));
            
            // Reset appearance
            Light.Color = Color.MediumPurple;
            Light.Alpha = 1f;
            Sprite.Play("idle", false, false);
            psiActive = false;
        }

        private IEnumerator CreatePSIPlatform(Vector2 position)
        {
            // Create a temporary psychic platform that player can stand on
            Entity psiPlatform = new Entity(position)
            {
                Collider = new Hitbox(32f, 4f, -16f, -2f),
                Collidable = true
            };
            
            // Add visual component for the platform
            psiPlatform.Add(new Coroutine(AnimatePSIPlatform(psiPlatform)));
            Scene.Add(psiPlatform);
            
            // Platform lasts for 4 seconds
            yield return 4f;
            
            // Fade out platform
            if (psiPlatform.Scene != null)
            {
                SceneAs<Level>().Particles.Emit(FlyFeather.P_Collect, 8, psiPlatform.Position, Vector2.One * 8f);
                psiPlatform.RemoveSelf();
            }
        }

        private IEnumerator AnimatePSIPlatform(Entity platform)
        {
            float timer = 0f;
            while (platform.Scene != null)
            {
                timer += Engine.DeltaTime;
                
                // Create glowing particles around platform
                if (Calc.Random.Chance(0.2f))
                {
                    Vector2 particlePos = platform.Position + new Vector2(
                        (float)(Calc.Random.NextDouble() - 0.5) * 32f,
                        (float)(Calc.Random.NextDouble() - 0.5) * 4f
                    );
                    SceneAs<Level>().ParticlesFG.Emit(FlyFeather.P_Collect, particlePos);
                }
                
                yield return null;
            }
        }

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/ness/appear", Position);
                psiSound.Play("event:/char/ness/psi_appear");
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
            level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/ness/disappear", Position);
            psiSound.Play("event:/char/ness/psi_disappear");
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
            // Update PSI cooldown
            if (psiCooldown > 0f)
            {
                psiCooldown -= Engine.DeltaTime;
            }
            
            if (player != null && following && !psiActive)
            {
                Position = Calc.Approach(Position, player.Position, 460f * Engine.DeltaTime);
                Sprite.Scale.X = Math.Sign(player.Position.X - Position.X);
                
                // Use PSI teleport to help player reach high places
                Vector2 toPlayer = player.Position - Position;
                if (toPlayer.Length() > 120f && psiCooldown <= 0f)
                {
                    // Teleport closer to player and create platform
                    Vector2 teleportTarget = player.Position + new Vector2(
                        -Math.Sign(toPlayer.X) * 24f, 
                        -40f
                    );
                    Add(new Coroutine(UsePSITeleport(teleportTarget)));
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
            // Allow player to climb on Ness - provides PSI boost
            if (player.StateMachine.State == global::Celeste.Player.StClimb && player.Bottom <= Top + 4f)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -125f);
                Audio.Play("event:/char/ness/psi_boost", Position);
                
                // PSI boost effect
                Light.Alpha = 1.5f;
                SceneAs<Level>().ParticlesFG.Emit(FlyFeather.P_Collect, 10, Center, Vector2.One * 8f);
                
                // Create temporary PSI platform above for extra help
                if (psiCooldown <= 0f)
                {
                    Add(new Coroutine(CreatePSIPlatform(player.Position + Vector2.UnitY * -32f)));
                    psiCooldown = PSI_COOLDOWN_TIME / 2f; // Shorter cooldown for boost usage
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
            
            // Render PSI aura when active
            if (psiActive)
            {
                Draw.Circle(Position, 12f, Color.Cyan * 0.3f, 8);
                Draw.Circle(Position, 8f, Color.MediumPurple * 0.5f, 6);
            }
            
            // Render PSI cooldown indicator
            if (psiCooldown > 0f)
            {
                float cooldownPercent = psiCooldown / PSI_COOLDOWN_TIME;
                Vector2 barPos = Position + Vector2.UnitY * -20f;
                Draw.Rect(barPos.X - 8f, barPos.Y, 16f, 2f, Color.Black * 0.5f);
                Draw.Rect(barPos.X - 8f, barPos.Y, 16f * (1f - cooldownPercent), 2f, Color.Cyan);
            }
        }
    }
}



