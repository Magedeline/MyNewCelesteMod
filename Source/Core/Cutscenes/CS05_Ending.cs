#nullable enable
namespace DesoloZantas.Core.Core.Cutscenes
{
    public class Cs05Ending : CutsceneEntity
    {
        public const string FLAG = "oshiroEnding_mod";
        private ResortRoofEnding roof;
        private AngryOshiro angryOshiro;
        private global::Celeste.Player player;
        private Entity oshiro;
        private Sprite? oshiroSprite;
        private FMOD.Studio.EventInstance smashSfx;
        private bool smashRumble;

        public Cs05Ending() : base(false, true)
        {
            smashRumble = false;
            Depth = -1000000;
            roof = null!;
            angryOshiro = null!;
            player = null!;
            oshiro = null!;
            smashSfx = null!;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            var roof = SceneAs<Level>()?.Tracker.GetEntity<ResortRoofEnding>() ??
                       throw new Exception("ResortRoofEnding entity not found in the scene.");
            var player = SceneAs<Level>()?.Tracker.GetEntity<global::Celeste.Player>() ??
                         throw new Exception("Player entity not found in the scene.");
            var angryOshiro = roof.Scene.Tracker.GetEntity<AngryOshiro>() ??
                              throw new Exception("AngryOshiro entity not found in the scene.");
            Entity oshiro = angryOshiro;
            var oshiroSprite = oshiro.Get<Sprite>();
            var smashSfx = Audio.Play("event:/char/oshiro/smash", oshiro.Position);

            this.roof = roof;
            this.player = player;
            this.angryOshiro = angryOshiro;
            this.oshiro = oshiro;
            this.oshiroSprite = oshiroSprite;
            this.smashSfx = smashSfx;
        }

        public override void OnBegin(Level level)
        {
            level.RegisterAreaComplete();
            Add(new Coroutine(cutscene(level)));
        }

        public override void Update()
        {
            base.Update();
            if (!smashRumble)
                return;
            Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
        }

        private IEnumerator cutscene(Level level)
        {
            Cs05Ending cs05Ending = this;
            cs05Ending.player.StateMachine.State = 11;
            cs05Ending.player.StateMachine.Locked = true;
            cs05Ending.player.ForceCameraUpdate = false;
            cs05Ending.Add(new Coroutine(cs05Ending.player.DummyRunTo((float)(cs05Ending.roof.X + (double)cs05Ending.roof.Width - 32.0), true)));
            yield return null;
            cs05Ending.player.DummyAutoAnimate = false;
            yield return 0.5f;
            cs05Ending.angryOshiro = cs05Ending.Scene.Entities.FindFirst<AngryOshiro>();
            cs05Ending.Add(new Coroutine(cs05Ending.moveGhostTo(new Vector2(cs05Ending.roof.X + 40f, cs05Ending.roof.Y - 12f))));
            yield return 1f;
            cs05Ending.player.DummyAutoAnimate = true;
            yield return level.ZoomTo(new Vector2(130f, 60f), 2f, 0.5f);
            cs05Ending.player.Facing = Facings.Left;
            yield return 0.5f;
            yield return Textbox.Say("CH5_OSHIRO_CHASE_END", cs05Ending.ghostSmash);
            yield return cs05Ending.ghostSmash(0.5f, true);
            Audio.SetMusic(null);
            cs05Ending.oshiroSprite = null;
            BgFlash? bgFlash = new BgFlash();
            bgFlash.Alpha = 1f;
            level.Add(bgFlash);
            Distort.GameRate = 0.0f;
            Sprite sprite = GFX.SpriteBank.Create("oshiro_boss_lightning");
            sprite.Position = cs05Ending.angryOshiro.Position + new Vector2(140f, -100f);
            sprite.Rotation = Calc.Angle(sprite.Position, cs05Ending.angryOshiro.Position + new Vector2(0.0f, 10f));
            sprite.Play("once");
            cs05Ending.Add(sprite);
            yield return null;
            Celeste.Celeste.Freeze(0.3f);
            yield return null;
            level.Shake();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            cs05Ending.smashRumble = false;
            yield return 0.2f;
            Distort.GameRate = 1f;
            level.Flash(Color.White);
            cs05Ending.player.DummyGravity = false;
            cs05Ending.angryOshiro.Sprite.Play("transformBack");
            cs05Ending.player.Sprite.Play("fall");
            cs05Ending.roof.BeginFalling = true;
            yield return null;
            Engine.TimeRate = 0.01f;
            cs05Ending.player.Sprite.Play("fallFast");
            cs05Ending.player.DummyGravity = true;
            cs05Ending.player.Speed.Y = -200f;
            cs05Ending.player.Speed.X = 300f;
            Vector2 oshiroFallSpeed = new Vector2(-100f, -250f);
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.SineInOut, 1.5f, true);
            // ISSUE: reference to a compiler-generated method
            tween.OnUpdate = delegate (Tween t)
            {
                angryOshiro.Sprite.Rotation = t.Eased * -100f * 0.017453292f;
            };
            cs05Ending.Add(tween);
            float t;
            for (t = 0.0f; t < 2.0; t += Engine.DeltaTime)
            {
                oshiroFallSpeed.X = Calc.Approach(oshiroFallSpeed.X, 0.0f, Engine.DeltaTime * 400f);
                oshiroFallSpeed.Y += Engine.DeltaTime * 800f;
                AngryOshiro angryOshiro = cs05Ending.angryOshiro;
                angryOshiro.Position += oshiroFallSpeed * Engine.DeltaTime;
                bgFlash.Alpha = Calc.Approach(bgFlash.Alpha, 0.0f, Engine.RawDeltaTime);
                Engine.TimeRate = Calc.Approach(Engine.TimeRate, 1f, Engine.RawDeltaTime * 0.6f);
                yield return null;
            }
            level.DirectionalShake(new Vector2(0.0f, -1f), 0.5f);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Long);
            yield return 1f;
            bgFlash = null;
            oshiroFallSpeed = new Vector2();
            while (!cs05Ending.player.OnGround())
                cs05Ending.player.MoveV(1f);
            cs05Ending.player.DummyAutoAnimate = false;
            cs05Ending.player.Sprite.Play("tired");
            cs05Ending.angryOshiro.RemoveSelf();
            Scene scene = cs05Ending.Scene;
            Cs05Ending cs03Ending2 = cs05Ending;
            Rectangle bounds = level.Bounds;
            Entity entity1;
            Entity entity2 = entity1 = new Entity(new Vector2(bounds.Left + 110, cs05Ending.player.Y));
            cs03Ending2.oshiro = entity1;
            Entity entity3 = entity2;
            scene.Add(entity3);
            cs05Ending.oshiro.Add(cs05Ending.oshiroSprite = GFX.SpriteBank.Create(nameof(oshiro)));
            cs05Ending.oshiroSprite.Play("fall");
            cs05Ending.oshiroSprite.Scale.X = 1f;
            cs05Ending.oshiro.Collider = new Hitbox(8f, 8f, -4f, -8f);
            cs05Ending.oshiro.Add(new VertexLight(new Vector2(0.0f, -8f), Color.White, 1f, 16, 32));
            yield return CutsceneEntity.CameraTo(cs05Ending.player.CameraTarget + new Vector2(0.0f, 40f), 1f, Ease.CubeOut);
            yield return 1.5f;
            Audio.SetMusic("event:/music/lvl3/intro");
            yield return 3f;
            Audio.Play("event:/char/oshiro/chat_get_up", cs05Ending.oshiro.Position);
            cs05Ending.oshiroSprite.Play("recover");
            float target = cs05Ending.oshiro.Y + 4f;
            while (cs05Ending.oshiro.Y != (double)target)
            {
                cs05Ending.oshiro.Y = Calc.Approach(cs05Ending.oshiro.Y, target, 6f * Engine.DeltaTime);
                yield return null;
            }
            yield return 0.6f;
            yield return Textbox.Say("CH5_ENDING", cs05Ending.oshiroTurns);
            cs05Ending.Add(new Coroutine(CutsceneEntity.CameraTo(level.Camera.Position + new Vector2(-80f, 0.0f), 3f)));
            yield return 0.5f;
            cs05Ending.oshiroSprite.Scale.X = -1f;
            yield return 0.2f;
            t = 0.0f;
            cs05Ending.oshiro.Add(new SoundSource("event:/char/oshiro/move_08_roof07_exit"));
            while (cs05Ending.oshiro.X > (double)(level.Bounds.Left - 16))
            {
                cs05Ending.oshiro.X -= 40f * Engine.DeltaTime;
                cs05Ending.oshiroSprite.Y = (float)Math.Sin(t += Engine.DeltaTime * 2f) * 2f;
                cs05Ending.oshiro.CollideFirst<Door>()?.Open(cs05Ending.oshiro.X);
                yield return null;
            }
            cs05Ending.Add(new Coroutine(CutsceneEntity.CameraTo(level.Camera.Position + new Vector2(80f, 0.0f), 2f)));
            yield return 1.2f;
            cs05Ending.player.DummyAutoAnimate = true;
            yield return cs05Ending.player.DummyWalkTo(cs05Ending.player.X - 16f);
            yield return 2f;
            cs05Ending.player.Facing = Facings.Right;
            yield return 1f;
            cs05Ending.player.ForceCameraUpdate = false;
            cs05Ending.player.Add(new Coroutine(cs05Ending.runPlayerRight()));
            endCutscene(new global::Celeste.Level { level });
        }

        private void endCutscene(Level level)
        {
            if (level == null) return;
        }

        private IEnumerator oshiroTurns()
        {
            yield return 1f;
            if (oshiroSprite != null) oshiroSprite.Scale.X = -1f;
            yield return 0.2f;
        }

        private IEnumerator moveGhostTo(Vector2 target)
        {
            target.Y -= angryOshiro.Height / 2f;
            angryOshiro.EnterDummyMode();
            angryOshiro.Collidable = false;
            while (angryOshiro.Position != target)
            {
                angryOshiro.Position = Calc.Approach(angryOshiro.Position, target, 64f * Engine.DeltaTime);
                yield return null;
            }
        }

        private IEnumerator ghostSmash()
        {
            yield return ghostSmash(0.0f, false);
        }

        private IEnumerator ghostSmash(float topDelay, bool final)
        {
            Cs05Ending cs05Ending = this;
            if (cs05Ending.angryOshiro != null)
            {
                cs05Ending.smashSfx = !final ? Audio.Play("event:/char/oshiro/boss_slam_first", cs05Ending.angryOshiro.Position) : Audio.Play("event:/char/oshiro/boss_slam_final", cs05Ending.angryOshiro.Position);
                float from = cs05Ending.angryOshiro.Y;
                float to = cs05Ending.angryOshiro.Y - 32f;
                float p;
                for (p = 0.0f; p < 1.0; p += Engine.DeltaTime * 2f)
                {
                    cs05Ending.angryOshiro.Y = MathHelper.Lerp(from, to, Ease.CubeOut(p));
                    yield return null;
                }
                yield return topDelay;
                float ground = from + 20f;
                for (p = 0.0f; p < 1.0; p += Engine.DeltaTime * 8f)
                {
                    cs05Ending.angryOshiro.Y = MathHelper.Lerp(to, ground, Ease.CubeOut(p));
                    yield return null;
                }
                cs05Ending.angryOshiro.Squish();
                cs05Ending.Level.Shake(0.5f);
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
                cs05Ending.smashRumble = true;
                cs05Ending.roof.StartShaking(0.5f);
                if (!final)
                {
                    for (p = 0.0f; p < 1.0; p += Engine.DeltaTime * 16f)
                    {
                        cs05Ending.angryOshiro.Y = MathHelper.Lerp(ground, from, Ease.CubeOut(p));
                        yield return null;
                    }
                }
                else
                    cs05Ending.angryOshiro.Y = (float)((ground + (double)from) / 2.0);
                if (cs05Ending.angryOshiro != null)
                {
                    cs05Ending.player.DummyAutoAnimate = false;
                    cs05Ending.player.Sprite.Play("shaking");
                    cs05Ending.roof.Wobble(cs05Ending.angryOshiro, final);
                    if (!final)
                        yield return 0.5f;
                }
            }
        }

        private IEnumerator runPlayerRight()
        {
            yield return 0.75f;
            yield return player.DummyRunTo(player.X + 128f);
        }

        public override void OnEnd(Level level)
        {
            Audio.SetMusic("event:/music/lvl3/intro");
            Audio.Stop(smashSfx);
            level.CompleteArea(true, true, true); // Specify the parameters explicitly
            SpotlightWipe.FocusPoint = new Vector2(192f, 120f);
        }

        private class BgFlash : Entity
        {
            public float Alpha;

            public BgFlash() => Depth = 10100;

            public override void Render()
            {
                Camera? camera = (Scene as Level)?.Camera;
                if (camera != null) Draw.Rect(camera.X - 10f, camera.Y - 10f, 340f, 200f, Color.Black * Alpha);
            }
        }
    }
}




