namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/StarsieDummy")]
    public class StarsieDummy : Entity
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

        public StarsieDummy(Vector2 position, int index = 0) : base(position)
        {
            this.index = index;
            followBehindIndexDelay = 0.4f * index;
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Starsie);
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
                    Audio.Play("event:/char/starsie/footstep", Position);
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
            Audio.Play("event:/char/starsie/level_entry", Position);
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
                Audio.Play("event:/char/starsie/appear", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
            level.Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/starsie/disappear", Position);
            Shockwave();
            SceneAs<Level>().Particles.Emit(P_Vanish, 12, Center, Vector2.One * 6f);
            RemoveSelf();
        }

        private void Shockwave()
        {
            SceneAs<Level>().Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f, null, null);
        }

        public IEnumerator FloatTo(Vector2 target, int? turnAtEndTo = null, bool faceDirection = true, bool fadeLight = false, bool quickEnd = false)
        {
            Sprite.Play("fallSlow", false, false);
            if (faceDirection && Math.Sign(target.X - X) != 0)
            {
                Sprite.Scale.X = (float)Math.Sign(target.X - X);
            }
            Vector2 vector = (target - Position).SafeNormalize();
            Vector2 perp = new Vector2(-vector.Y, vector.X);
            float speed = 0f;
            while (Position != target)
            {
                speed = Calc.Approach(speed, FloatSpeed, FloatAccel * Engine.DeltaTime);
                Position = Calc.Approach(Position, target, speed * Engine.DeltaTime);
                Floatness = Calc.Approach(Floatness, 4f, 8f * Engine.DeltaTime);
                floatNormal = Calc.Approach(floatNormal, perp, Engine.DeltaTime * 12f);
                if (fadeLight)
                {
                    Light.Alpha = Calc.Approach(Light.Alpha, 0f, Engine.DeltaTime * 2f);
                }
                yield return null;
            }
            if (quickEnd)
            {
                Floatness = 2f;
            }
            else
            {
                while (Floatness != 2f)
                {
                    Floatness = Calc.Approach(Floatness, 2f, 8f * Engine.DeltaTime);
                    yield return null;
                }
            }
            if (turnAtEndTo != null)
            {
                Sprite.Scale.X = (float)turnAtEndTo.Value;
            }
            yield break;
        }

        public IEnumerator WalkTo(float x, float speed = 64f)
        {
            Floatness = 0f;
            Sprite.Play("walk", false, false);
            if (Math.Sign(x - X) != 0)
            {
                Sprite.Scale.X = (float)Math.Sign(x - X);
            }
            while (X != x)
            {
                X = Calc.Approach(X, x, Engine.DeltaTime * speed);
                yield return null;
            }
            Sprite.Play("idle", false, false);
            yield break;
        }

        public IEnumerator SmashBlock(Vector2 target)
        {
            SceneAs<Level>().Displacement.AddBurst(Position, 0.5f, 24f, 96f, 1f, null, null);
            Sprite.Play("dreamDashLoop", false, false);
            Vector2 from = Position;
            for (float p = 0f; p < 1f; p += Engine.DeltaTime * 6f)
            {
                Position = from + (target - from) * Ease.CubeOut(p);
                yield return null;
            }
            var dashBlock = Scene.Entities.FindFirst<DashBlock>();
            if (dashBlock != null)
            {
                // Use the correct method to break the DashBlock.
                // According to the type signatures, DashBlock has a private IEnumerator break(...) method,
                // but not a public Break method. The public API for breaking a DashBlock is typically
                // to call its OnDash method, which returns DashCollisionResults and triggers the break logic.
                // We'll call OnDash with a dummy Player and direction.

                // If you have access to the current player, use that. Otherwise, you may need to get it from the scene.
                var player = Scene.Tracker.GetEntity<global::Celeste.Player>();
                if (player != null)
                {
                    dashBlock.GetType().GetMethod("OnDash", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)
                        ?.Invoke(dashBlock, new object[] { player, new Vector2(0f, -1f) });
                }
            }
            Sprite.Play("idle", false, false);
            for (float p = 0f; p < 1f; p += Engine.DeltaTime * 4f)
            {
                Position = target + (from - target) * Ease.CubeOut(p);
                yield return null;
            }
            Sprite.Play("fallSlow", false, false);
            yield break;
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
            Hair.Visible = false;
            
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, duration, true);
            tween.OnUpdate = delegate(Tween t)
            {
                Sprite.Scale = Vector2.One * t.Eased;
                Sprite.Color = Color.White * t.Eased;
            };
            Add(tween);
        }

        // Your existing dummy methods remain unchanged...
    }
}




