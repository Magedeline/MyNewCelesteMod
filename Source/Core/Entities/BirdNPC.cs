using DesoloZantas.Core.Core.Player;

namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/BirdNPCMod")]
    [Tracked(true)]
    public class BirdNpc : Actor
    {
        public static ParticleType PFeather;
        public static string FlownFlag = "bird_fly_away_";
        public IngesteStateMachine.Facings Facing = IngesteStateMachine.Facings.Left;
        public Sprite Sprite;
        public Vector2 StartPosition;
        public VertexLight Light;
        public bool AutoFly;
        public EntityID EntityId;
        public bool FlyAwayUp = true;
        public float WaitForLightningPostDelay;
        public bool DisableFlapSfx;
        public Coroutine TutorialRoutine;
        public Modes Mode;
        public BirdTutorialGui Gui;
        public Level Level;
        public Vector2[] Nodes;
        public StaticMover StaticMover;
        public bool OnlyOnce;
        public bool OnlyIfPlayerLeft;

        static BirdNpc()
        {
            // Initialize feather particle type
            PFeather = new ParticleType
            {
                Source = GFX.Game["particles/feather"],
                Color = Color.White,
                Color2 = Color.Gray,
                ColorMode = ParticleType.ColorModes.Choose,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.8f,
                LifeMax = 1.2f,
                Size = 1f,
                SizeRange = 0.5f,
                SpeedMin = 10f,
                SpeedMax = 20f,
                Direction = (float)Math.PI / 2f,
                DirectionRange = (float)Math.PI / 4f,
                Acceleration = new Vector2(0f, 10f),
                RotationMode = ParticleType.RotationModes.SameAsDirection
            };
        }

        public BirdNpc(Vector2 position, Modes mode) : base(position)
        {
            Add(Sprite = GFX.SpriteBank.Create("bird"));
            Sprite.Scale.X = (float)Facing;
            Sprite.UseRawDeltaTime = true;
            Sprite.OnFrameChange = OnSpriteFrameChange;
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.White, 1f, 8, 32));
            StartPosition = Position;
            setMode(mode);
        }

        private void setMode(Modes mode)
        {
            Mode = mode;

            switch (mode)
            {
                case Modes.ClimbingTutorial:
                    Add(new Coroutine((IEnumerator)ClimbingTutorial()));
                    break;

                case Modes.DashingTutorial:
                    // Add logic for DashingTutorial if required
                    break;

                case Modes.DreamJumpTutorial:
                    // Add logic for DreamJumpTutorial if required
                    break;

                case Modes.SuperWallJumpTutorial:
                    // Add logic for SuperWallJumpTutorial if required
                    break;

                case Modes.HyperJumpTutorial:
                    // Add logic for HyperJumpTutorial if required
                    break;

                case Modes.FlyAway:
                    if (AutoFly) Add(new Coroutine(startleAndFlyAway()));
                    break;

                case Modes.Sleeping:
                    this.Sprite.Play("sleep");
                    break;

                case Modes.MoveToNodes:
                    Add(new Coroutine(moveToNodesRoutine()));
                    break;

                case Modes.WaitForLightningOff:
                    Add(new Coroutine(waitForLightningOffRoutine()));
                    break;

                case Modes.None:
                default:
                    break;
            }
        }

        private IEnumerator waitForLightningOffRoutine()
        {
            while (Level.Session.BloomBaseAdd > 0.1f) yield return null;

            yield return WaitForLightningPostDelay;
        }

        private IEnumerator moveToNodesRoutine()
        {
            if (Nodes == null || Nodes.Length == 0) yield break;

            foreach (var target in Nodes)
            {
                while (Vector2.Distance(Position, target) > 1f)
                {
                    var direction = (target - Position).SafeNormalize();
                    Position += direction * 60f * Engine.DeltaTime;

                    Facing = direction.X > 0 ? IngesteStateMachine.Facings.Right : IngesteStateMachine.Facings.Left;

                    yield return null;
                }

                Position = target;
                yield return 0.2f;
            }
        }

        private IEnumerator startleAndFlyAway()
        {
            if (Level.Session.GetFlag(FlownFlag + Level.Session.Level)) yield break;

            Level.Session.SetFlag(FlownFlag + Level.Session.Level);
            this.Sprite.Play("fly");
            Light.Visible = false;

            if (!DisableFlapSfx) Audio.Play("event:/game/general/bird_startle", Position);

            var flyDirection = FlyAwayUp ? new Vector2(0f, -1f) : new Vector2((float)Facing, -0.5f);
            flyDirection.Normalize();

            for (var i = 0; i < 6; i++)
            {
                Level.Particles.Emit(PFeather, 1, Position + new Vector2(0f, -6f), Vector2.One * 4f);
                yield return 0.05f;
            }

            while (true)
            {
                Position += flyDirection * 60f * Engine.DeltaTime;
                if (Position.Y < Level.Camera.Top - 16f || Position.X < Level.Camera.Left - 16f ||
                    Position.X > Level.Camera.Right + 16f)
                    break;

                yield return null;
            }

            RemoveSelf();
        }

        public BirdNpc(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Enum(nameof(Mode), Modes.None))
        {
            EntityId = new EntityID(data.Level.Name, data.ID);
            Nodes = data.NodesOffset(offset);
            OnlyOnce = data.Bool(nameof(OnlyOnce));
            OnlyIfPlayerLeft = data.Bool(nameof(OnlyIfPlayerLeft));
            AutoFly = data.Bool(nameof(AutoFly), false);
            FlyAwayUp = data.Bool(nameof(FlyAwayUp), true);
            WaitForLightningPostDelay = data.Float(nameof(WaitForLightningPostDelay), 0f);
            DisableFlapSfx = data.Bool(nameof(DisableFlapSfx), false);
        }

        private void OnSpriteFrameChange(string spr)
        {
            if (Level != null && X > Level.Camera.Left + 64 && X < Level.Camera.Right - 64 &&
                (spr == "peck" || spr == "peckRare") && Sprite.CurrentAnimationFrame == 6)
            {
                Audio.Play("event:/game/general/bird_peck", Position);
            }
            if (Level != null && Level.Session.Area.ID == 10 && !DisableFlapSfx)
            {
                FlapSfxCheck(Sprite);
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level = scene as Level;
            if (Mode == Modes.ClimbingTutorial && Level.Session.GetLevelFlag("2"))
            {
                RemoveSelf();
            }
            else if (Mode == Modes.FlyAway && Level.Session.GetFlag(FlownFlag + Level.Session.Level))
            {
                RemoveSelf();
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (Mode == Modes.SuperWallJumpTutorial)
            {
                var player = scene.Tracker.GetEntity<global::Celeste.Player>();
                if (player != null && player.Y < Y + 32)
                {
                    RemoveSelf();
                }
            }
            if (OnlyIfPlayerLeft)
            {
                var player = Level.Tracker.GetEntity<global::Celeste.Player>();
                if (player != null && player.X > X)
                {
                    RemoveSelf();
                }
            }
        }

        public override bool IsRiding(Solid solid)
        {
            return Scene.CollideCheck(new Rectangle((int)X - 4, (int)Y, 8, 2), solid);
        }

        public override void Update()
        {
            Sprite.Scale.X = (float)Facing;
            base.Update();
        }

        public IEnumerable<IEnumerator> ClimbingTutorial()
        {
            var player = Scene.Tracker.GetEntity<global::Celeste.Player>();
            while (Math.Abs(player.X - X) > 120)
            {
                yield return null;
            }
            var tutorial1 = new BirdTutorialGui(this, new Vector2(0f, -16f), Dialog.Clean("tutorial_climb"), new object[]
            {
                Dialog.Clean("tutorial_hold"),
                BirdTutorialGui.ButtonPrompt.Grab
            });
            var tutorial2 = new BirdTutorialGui(this, new Vector2(0f, -16f), Dialog.Clean("tutorial_climb"), new object[]
            {
                BirdTutorialGui.ButtonPrompt.Grab,
                "+",
                new Vector2(0f, -1f)
            });
            bool first = true;
            bool willEnd;
            do
            {
                yield return showTutorial(tutorial1, first);
                first = false;
                while (player.StateMachine.State != 1 && player.Y > Y)
                {
                    yield return null;
                }
                if (player.Y > Y)
                {
                    Audio.Play("event:/ui/game/tutorial_note_flip_back");
                    yield return hideTutorial();
                }
                while (player.Scene != null && (!player.OnGround() || player.StateMachine.State == 1))
                {
                    yield return null;
                }
                willEnd = player.Y <= Y + 4;
                if (!willEnd)
                {
                    Audio.Play("event:/ui/game/tutorial_note_flip_front");
                }
                yield return hideTutorial();
            } while (!willEnd);
            yield return startleAndFlyAway();
        }

        private void add(BirdTutorialGui components)
        {
            Gui = components;
            Level.Add(Gui);
        }

        private IEnumerator hideTutorial()
        {
            if (Gui != null)
            {
                Gui.RemoveSelf();
                Gui = null;
            }

            yield break;
        }

        private IEnumerator showTutorial(BirdTutorialGui tutorial, bool first)
        {
            if (first)
            {
                Gui = tutorial;
                Level.Add(Gui);
                yield return null;
            }
            else
            {
                Gui.Visible = true;
                yield return null;
            }
        }

        // Methods used by cutscenes
        public IEnumerator Startle(string sfx = null, float duration = 0.5f, Vector2? targetOffset = null)
        {
            Sprite.Play("fly");

            if (!string.IsNullOrEmpty(sfx))
            {
                Audio.Play(sfx, Position);
            }
            else if (!DisableFlapSfx)
            {
                Audio.Play("event:/game/general/bird_startle", Position);
            }

            Vector2 startPos = Position;
            Vector2 offset = targetOffset ?? new Vector2(0f, -16f);

            for (float t = 0f; t < 1f; t += Engine.DeltaTime / duration)
            {
                Position = startPos + offset * Ease.CubeOut(t);
                yield return null;
            }

            Sprite.Play("idle");
        }

        public IEnumerator FlyAway(float delay = 0f)
        {
            if (delay > 0f)
            {
                yield return delay;
            }

            yield return startleAndFlyAway();
        }

        public IEnumerator FlyTo(Vector2 target, float duration = 1f, bool playSound = true)
        {
            Sprite.Play("fly");

            if (playSound && !DisableFlapSfx)
            {
                Audio.Play("event:/game/general/bird_startle", Position);
            }

            Vector2 startPos = Position;

            // Set facing direction
            if (target.X > Position.X)
            {
                Facing = IngesteStateMachine.Facings.Right;
            }
            else if (target.X < Position.X)
            {
                Facing = IngesteStateMachine.Facings.Left;
            }

            for (float t = 0f; t < 1f; t += Engine.DeltaTime / duration)
            {
                Position = Vector2.Lerp(startPos, target, Ease.SineInOut(t));

                // Add some bobbing motion
                float bobAmount = (float)Math.Sin(t * Math.PI * 4f) * 4f;
                Position += new Vector2(0f, bobAmount);

                yield return null;
            }

            Position = target;
            Sprite.Play("idle");
        }

        public IEnumerator Caw()
        {
            Audio.Play("event:/game/general/bird_caw", Position);
            Sprite.Play("peck");

            while (Sprite.Animating)
            {
                yield return null;
            }

            Sprite.Play("idle");
        }

        public static void FlapSfxCheck(Sprite sprite)
        {
            if (sprite.Entity?.Scene is Level level)
            {
                var camera = level.Camera;
                var renderPosition = sprite.RenderPosition;
                if (renderPosition.X < camera.X - 32 || renderPosition.Y < camera.Y - 32 ||
                    renderPosition.X > camera.X + 320 + 32 || renderPosition.Y > camera.Y + 180 + 32)
                {
                    return;
                }
            }
            var currentAnimationId = sprite.CurrentAnimationID;
            var currentAnimationFrame = sprite.CurrentAnimationFrame;
            if ((currentAnimationId == "hover" && currentAnimationFrame == 0) ||
                (currentAnimationId == "hoverStressed" && currentAnimationFrame == 0) ||
                (currentAnimationId == "fly" && currentAnimationFrame == 0) ||
                (currentAnimationId == "flyupIdle" && currentAnimationFrame == 0))
            {
                Audio.Play("event:/new_content/game/10_farewell/bird_wingflap", sprite.RenderPosition);
            }
        }

        public enum Modes
        {
            ClimbingTutorial,
            DashingTutorial,
            DreamJumpTutorial,
            SuperWallJumpTutorial,
            HyperJumpTutorial,
            FlyAway,
            None,
            Sleeping,
            MoveToNodes,
            WaitForLightningOff
        }
    }
}




