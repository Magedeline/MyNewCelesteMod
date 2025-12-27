using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Cutscenes
{
    public class CS08_StarJumpEnd : CutsceneEntity
    {
        public const string Flag = "plateaumod_2";
        private bool waiting = true;
        private bool shaking;
        private NPC starJumpController;
        private global::Celeste.Player player;
        private Bonfire bonfire;
        private BadelineDummy badeline;
        private MagolorDummy magolor;
        private RalseiDummy ralsei;
        private Solid plateauMod;
        private BreathingMinigame breathing;
        private List<ReflectionTentacles> tentacles = new List<ReflectionTentacles>();
        private Vector2 playerStart;
        private Vector2 cameraStart;
        private float anxietyFade;
        private SineWave anxietySine;
        private float anxietyJitter;
        private bool hidingNorthernLights;
        private bool charactersSpinning;
        private float maddySine;
        private float maddySineTarget;
        private float maddySineAnchorY;
        private SoundSource shakingLoopSfx;
        private bool charaCircling;
        private BreathingRumbler rumbler;
        private int tentacleIndex;
        private NPC starJumpCutsceneControl;

        public CS08_StarJumpEnd(NPC starJumpController, global::Celeste.Player player, Vector2 playerStart, Vector2 cameraStart)
            : base(true, false)
        {
            base.Depth = 10100;
            this.starJumpController = starJumpController;
            this.player = player;
            this.playerStart = playerStart;
            this.cameraStart = cameraStart;
            base.Add(this.anxietySine = new SineWave(0.3f, 0f));
        }

        public CS08_StarJumpEnd(NPC starJumpCutsceneControl, global::Celeste.Player player)
        {
            this.starJumpCutsceneControl = starJumpCutsceneControl;
            this.player = player;
        }

        public override void Added(Scene scene)
        {
            this.Level = (scene as Level);
            this.bonfire = scene.Entities.FindFirst<Bonfire>();
            this.plateauMod = scene.Entities.FindFirst<Solid>();
        }

        public override void Update()
        {
            base.Update();
            if (this.waiting && this.player.Y <= (float)(this.Level.Bounds.Top + 160))
            {
                this.waiting = false;
                base.Start();
            }
            if (this.shaking)
            {
                this.Level.Shake(0.2f);
            }
            if (this.Level != null && this.Level.OnInterval(0.1f))
            {
                this.anxietyJitter = Calc.Random.Range(-0.1f, 0.1f);
            }
            Distort.Anxiety = this.anxietyFade * Math.Max(0f, 0f + this.anxietyJitter + this.anxietySine.Value * 0.6f);
            this.maddySine = Calc.Approach(this.maddySine, this.maddySineTarget, 12f * Engine.DeltaTime);
            if (this.maddySine > 0f)
            {
                this.player.Y = this.maddySineAnchorY + (float)Math.Sin((double)(this.Level.TimeActive * 2f)) * 3f * this.maddySine;
            }
        }

        public override void OnBegin(Level level)
        {
            base.Add(new Coroutine(this.Cutscene(level), true));
        }

        private IEnumerator Cutscene(Level level)
        {
            StarJumpController controller = level.Entities.FindFirst<StarJumpController>();
            if (controller != null)
            {
                controller.RemoveSelf();
            }
            foreach (CelesteStarJumpBlock block in level.Entities.FindAll<CelesteStarJumpBlock>())
            {
                block.Collidable = false;
            }
            int center = level.Bounds.X + 160;
            Vector2 cutsceneCenter = new Vector2((float)center, (float)(level.Bounds.Top + 150));
            NorthernLights bg = level.Background.Get<NorthernLights>();
            level.CameraOffset.Y = -30f;
            base.Add(new Coroutine(CutsceneEntity.CameraTo(cutsceneCenter + new Vector2(-160f, -70f), 1.5f, Ease.CubeOut, 0f), true));
            base.Add(new Coroutine(CutsceneEntity.CameraTo(cutsceneCenter + new Vector2(-160f, -120f), 2f, Ease.CubeInOut, 1.5f), true));
            Tween.Set(this, Tween.TweenMode.Oneshot, 3f, Ease.CubeInOut, t => { bg.OffsetY = t.Eased * 32f; }, null);
            if (this.player.StateMachine.State == 19)
            {
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }
            this.player.Dashes = 0;
            this.player.StateMachine.State = 19;
            this.player.DummyGravity = false;
            this.player.DummyAutoAnimate = false;
            this.player.Sprite.Play("fallSlow", false, false);
            this.player.Dashes = 1;
            this.player.Speed = new Vector2(0f, -80f);
            this.player.Facing = Facings.Right;
            this.player.ForceCameraUpdate = false;
            while (this.player.Speed.Length() > 0f || this.player.Position != cutsceneCenter)
            {
                this.player.Speed = Calc.Approach(this.player.Speed, Vector2.Zero, 200f * Engine.DeltaTime);
                this.player.Position = Calc.Approach(this.player.Position, cutsceneCenter, 64f * Engine.DeltaTime);
                yield return null;
            }
            this.player.Sprite.Play("spin", false, false);
            yield return 3.5f;
            this.player.Facing = Facings.Right;
            level.Add(this.badeline = new BadelineDummy(this.player.Position));
            level.Add(this.magolor = new MagolorDummy(this.player.Position + new Vector2(32f, 0f)));
            level.Add(this.ralsei = new RalseiDummy(this.player.Position + new Vector2(-32f, 0f)));
            level.Displacement.AddBurst(this.player.Position, 0.5f, 8f, 48f, 0.5f, null, null);
            Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
            this.player.CreateSplitParticles();
            Audio.Play("event:/char/badeline/maddy_split");
            this.badeline.Sprite.Scale.X = -1f;
            Vector2 start = this.player.Position;
            Vector2 target = cutsceneCenter + new Vector2(-30f, 0f);
            this.maddySineAnchorY = cutsceneCenter.Y;
            for (float p = 0f; p <= 1f; p += 2f * Engine.DeltaTime)
            {
                yield return null;
                if (p > 1f)
                {
                    p = 1f;
                }
                this.player.Position = Vector2.Lerp(start, target, Ease.CubeOut(p));
                this.badeline.Position = new Vector2((float)center + ((float)center - this.player.X), this.player.Y);
            }
            start = default(Vector2);
            target = default(Vector2);
            this.charactersSpinning = true;
            base.Add(new Coroutine(this.SpinCharacters(), true));
            this.SetMusicLayer(2);
            yield return 1f;
            yield return Textbox.Say("ch8_dreaming", new Func<IEnumerator> []
            {
                new Func<IEnumerator>(this.TentaclesAppear),
                new Func<IEnumerator>(this.TentaclesGrab),
                new Func<IEnumerator>(this.FeatherMinigame),
                new Func<IEnumerator>(this.EndFeatherMinigame),
                new Func<IEnumerator>(this.StartCirclingPlayer)
            });
            Audio.Play("event:/game/06_reflection/badeline_pull_whooshdown");
            base.Add(new Coroutine(this.BadelineFlyDown(), true));
            yield return 0.7f;
            foreach (FlyFeather feather in level.Entities.FindAll<FlyFeather>())
            {
                feather.RemoveSelf();
            }
            foreach (CelesteStarJumpBlock block2 in level.Entities.FindAll<CelesteStarJumpBlock>())
            {
                block2.RemoveSelf();
            }
            foreach (CelesteJumpThru jumpThru in level.Entities.FindAll<CelesteJumpThru>())
            {
                jumpThru.RemoveSelf();
            }
            level.Shake(0.3f);
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
            level.CameraOffset.Y = 0f;
            this.player.Sprite.Play("tentacle_pull", false, false);
            this.player.Speed.Y = 160f;
            FallEffects.Show(true);
            for (float p = 0f; p < 1f; p += Engine.DeltaTime / 3f)
            {
                global::Celeste.Player player = this.player;
                player.Speed.Y = player.Speed.Y + Engine.DeltaTime * 100f;
                if (this.player.X < (float)(level.Bounds.X + 32))
                {
                    this.player.X = (float)(level.Bounds.X + 32);
                }
                if (this.player.X > (float)(level.Bounds.Right - 32))
                {
                    this.player.X = (float)(level.Bounds.Right - 32);
                }
                if (p > 0.7f)
                {
                    Level level2 = level;
                    level2.CameraOffset.Y = level2.CameraOffset.Y - 100f * Engine.DeltaTime;
                }
                foreach (ReflectionTentacles tentacle in this.tentacles)
                {
                    tentacle.Nodes[0] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 300f);
                    tentacle.Nodes[1] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 600f);
                }
                FallEffects.SpeedMultiplier += Engine.DeltaTime * 0.75f;
                Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
                yield return null;
            }
            Audio.Play("event:/game/06_reflection/badeline_pull_impact");
            FallEffects.Show(false);
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            level.Flash(Color.White, false);
            level.Session.Dreaming = false;
            level.CameraOffset.Y = 0f;
            level.Camera.Position = this.cameraStart;
            this.SetBloom(0f);
            this.bonfire.SetMode(Bonfire.Mode.Smoking);
            this.plateauMod.Depth = this.player.Depth + 10;
            this.player.Position = this.playerStart + new Vector2(0f, 8f);
            this.player.Speed = Vector2.Zero;
            this.player.Sprite.Play("tentacle_dangling", false, false);
            this.player.Facing = Facings.Left;
            foreach (ReflectionTentacles tentacle in this.tentacles)
            {
                tentacle.Index = 0;
                tentacle.Nodes[0] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 32f);
                tentacle.Nodes[1] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 400f);
                tentacle.SnapTentacles();
            }
            this.shaking = true;
            base.Add(this.shakingLoopSfx = new SoundSource());
            this.shakingLoopSfx.Play("event:/game/06_reflection/badeline_pull_rumble_loop", null, 0f);
            yield return Textbox.Say("ch8_magolor_watchout", new Func<IEnumerator>[0]);
            Audio.Play("event:/game/06_reflection/badeline_pull_cliffbreak");
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Long);
            this.shakingLoopSfx.Stop(true);
            this.shaking = false;
            int debrisIndex = 0;
            while ((float)debrisIndex < this.plateauMod.Width)
            {
                level.Add(Engine.Pooler.Create<Debris>().Init(this.plateauMod.Position + new Vector2((float)debrisIndex + Calc.Random.NextFloat(8f), Calc.Random.NextFloat(8f)), '3', true).BlastFrom(this.plateauMod.Center + new Vector2(0f, 8f)));
                level.Add(Engine.Pooler.Create<Debris>().Init(this.plateauMod.Position + new Vector2((float)debrisIndex + Calc.Random.NextFloat(8f), Calc.Random.NextFloat(8f)), '3', true).BlastFrom(this.plateauMod.Center + new Vector2(0f, 8f)));
                debrisIndex += 8;
            }
            this.plateauMod.RemoveSelf();
            this.bonfire.RemoveSelf();
            level.Shake(0.3f);
            this.player.Speed.Y = 160f;
            this.player.Sprite.Play("tentacle_pull", false, false);
            this.player.ForceCameraUpdate = false;
            FadeWipe wipe = new FadeWipe(level, false, delegate
            {
                this.EndCutscene(level, true);
            });
            wipe.Duration = 3f;
            target = level.Camera.Position;
            start = level.Camera.Position + new Vector2(0f, 400f);
            while (wipe.Percent < 1f)
            {
                level.Camera.Position = Vector2.Lerp(target, start, Ease.CubeIn(wipe.Percent));
                global::Celeste.Player player2 = this.player;
                player2.Speed.Y = player2.Speed.Y + 400f * Engine.DeltaTime;
                foreach (ReflectionTentacles tentacle in this.tentacles)
                {
                    tentacle.Nodes[0] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 300f);
                    tentacle.Nodes[1] = new Vector2((float)level.Bounds.Center.X, this.player.Y + 600f);
                }
                yield return null;
            }
            yield break;
        }

        private void SetMusicLayer(int index)
        {
            for (int i = 1; i <= 3; i++)
            {
                this.Level.Session.Audio.Music.Layer(i, index == i);
            }
            this.Level.Session.Audio.Apply(false);
        }

        private IEnumerator TentaclesAppear()
        {
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            if (this.tentacleIndex == 0)
            {
                Audio.Play("event:/game/06_reflection/badeline_freakout_1");
            }
            else if (this.tentacleIndex == 1)
            {
                Audio.Play("event:/game/06_reflection/badeline_freakout_2");
            }
            else if (this.tentacleIndex == 2)
            {
                Audio.Play("event:/game/06_reflection/badeline_freakout_3");
            }
            else
            {
                Audio.Play("event:/game/06_reflection/badeline_freakout_4");
            }
            if (!this.hidingNorthernLights)
            {
                base.Add(new Coroutine(this.NorthernLightsDown(), true));
                this.hidingNorthernLights = true;
            }
            this.Level.Shake(0.3f);
            this.anxietyFade += 0.1f;
            if (this.tentacleIndex == 0)
            {
                this.SetMusicLayer(3);
            }
            int tentacleStartY = 400;
            int tentacleEndY = 140;
            List<Vector2> nodes = new List<Vector2>
            {
                new Vector2(this.Level.Camera.X + 160f, this.Level.Camera.Y + tentacleStartY),
                new Vector2(this.Level.Camera.X + 160f, this.Level.Camera.Y + tentacleStartY + 200f)
            };
            ReflectionTentacles tentacle = new ReflectionTentacles();
            tentacle.Create(0f, 0, this.tentacles.Count, nodes);
            tentacle.Nodes[0] = new Vector2(tentacle.Nodes[0].X, this.Level.Camera.Y + tentacleEndY);
            this.Level.Add(tentacle);
            this.tentacles.Add(tentacle);
            this.charactersSpinning = false;
            this.tentacleIndex++;
            this.badeline.Sprite.Play("angry", false, false);
            this.maddySineTarget = 1f;
            yield return null;
        }

        private IEnumerator TentaclesGrab()
        {
            this.maddySineTarget = 0f;
            Audio.Play("event:/game/06_reflection/badeline_freakout_5");
            this.player.Sprite.Play("tentacle_grab", false, false);
            yield return 0.1f;
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            this.Level.Shake(0.3f);
            this.rumbler = new BreathingRumbler(this.player.Position);
            this.Level.Add(this.rumbler);
        }

        private IEnumerator StartCirclingPlayer()
        {
            base.Add(new Coroutine(this.BadelineCirclePlayer(), true));
            Vector2 from = this.player.Position;
            Vector2 to = new Vector2((float)this.Level.Bounds.Center.X, this.player.Y);
            Tween.Set(this, Tween.TweenMode.Oneshot, 0.5f, Ease.CubeOut, t =>
            {
                this.player.Position = Vector2.Lerp(from, to, t.Eased);
            }, null);
            yield return null;
        }

        private IEnumerator BadelineCirclePlayer()
        {
            float offset = 0f;
            float dist = (this.badeline.Position - this.player.Position).Length();
            this.charaCircling = true;
            while (this.charaCircling)
            {
                offset -= Engine.DeltaTime * 4f;
                dist = Calc.Approach(dist, 24f, Engine.DeltaTime * 32f);
                this.badeline.Position = this.player.Position + Calc.AngleToVector(offset, dist);
                int num = Math.Sign(this.player.X - this.badeline.X);
                if (num != 0)
                {
                    this.badeline.Sprite.Scale.X = (float)num;
                }
                if (this.Level.OnInterval(0.1f))
                {
                    TrailManager.Add(this.badeline, global::Celeste.Player.NormalHairColor, 1f, false, false);
                }
                yield return null;
            }
            this.badeline.Sprite.Scale.X = -1f;
            yield return this.badeline.FloatTo(this.player.Position + new Vector2(40f, -16f), new int?(-1), false, false, false);
        }

        private IEnumerator FeatherMinigame()
        {
            this.breathing = new BreathingMinigame(false, this.rumbler);
            this.Level.Add(this.breathing);
            while (!this.breathing.Pausing)
            {
                yield return null;
            }
        }

        private IEnumerator EndFeatherMinigame()
        {
            this.charaCircling = false;
            this.breathing.Pausing = false;
            while (!this.breathing.Completed)
            {
                yield return null;
            }
            this.breathing = null;
        }

        private IEnumerator BadelineFlyDown()
        {
            this.badeline.Sprite.Play("fallFast", false, false);
            this.badeline.FloatSpeed = 600f;
            this.badeline.FloatAccel = 1200f;
            yield return this.badeline.FloatTo(new Vector2(this.badeline.X, this.Level.Camera.Y + 200f), null, true, true, false);
            this.badeline.RemoveSelf();
        }

        private IEnumerator NorthernLightsDown()
        {
            NorthernLights bg = this.Level.Background.Get<NorthernLights>();
            if (bg != null)
            {
                while (bg.NorthernLightsAlpha > 0f)
                {
                    bg.NorthernLightsAlpha -= Engine.DeltaTime * 0.5f;
                    yield return null;
                }
            }
        }

        private IEnumerator SpinCharacters()
        {
            Vector2 maddyStart = this.player.Position;
            Vector2 badelineStart = this.badeline.Position;
            Vector2 center = (maddyStart + badelineStart) / 2f;
            float dist = Math.Abs(maddyStart.X - center.X);
            float timer = 1.5707964f;
            this.player.Sprite.Play("spin", false, false);
            this.badeline.Sprite.Play("spin", false, false);
            this.badeline.Sprite.Scale.X = 1f;
            while (this.charactersSpinning)
            {
                int num = (int)(timer / 6.2831855f * 14f + 10f);
                this.player.Sprite.SetAnimationFrame(num);
                this.badeline.Sprite.SetAnimationFrame(num + 7);
                float num2 = (float)Math.Sin((double)timer);
                float num3 = (float)Math.Cos((double)timer);
                this.player.Position = center - new Vector2(num2 * dist, num3 * 8f);
                this.badeline.Position = center + new Vector2(num2 * dist, num3 * 8f);
                timer += Engine.DeltaTime * 2f;
                yield return null;
            }
            this.player.Facing = Facings.Right;
            this.player.Sprite.Play("fallSlow", false, false);
            this.badeline.Sprite.Scale.X = -1f;
            this.badeline.Sprite.Play("angry", false, false);
            this.badeline.AutoAnimator.Enabled = false;
            Vector2 maddyFrom = this.player.Position;
            Vector2 badelineFrom = this.badeline.Position;
            for (float p = 0f; p < 1f; p += Engine.DeltaTime * 3f)
            {
                this.player.Position = Vector2.Lerp(maddyFrom, maddyStart, Ease.CubeOut(p));
                this.badeline.Position = Vector2.Lerp(badelineFrom, badelineStart, Ease.CubeOut(p));
                yield return null;
            }
        }

        public override void OnEnd(Level level)
        {
            if (this.rumbler != null)
            {
                this.rumbler.RemoveSelf();
                this.rumbler = null;
            }
            if (this.breathing != null)
            {
                this.breathing.RemoveSelf();
            }
            this.SetBloom(0f);
            level.Session.Audio.Music.Event = null;
            level.Session.Audio.Apply(false);
            level.Remove(this.player);
            level.UnloadLevel();
            level.EndCutscene();
            level.Session.SetFlag("plateaumod_2", true);
            level.SnapColorGrade(AreaData.Get(level).ColorGrade);
            level.Session.Dreaming = false;
            level.Session.FirstLevel = false;
            if (this.WasSkipped)
            {
                level.OnEndOfFrame += delegate
                {
                    level.Session.Level = "a-00";
                    level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Left, (float)level.Bounds.Bottom)));
                    level.LoadLevel(global::Celeste.Player.IntroTypes.None, false);
                    FallEffects.Show(false);
                    level.Session.Audio.Music.Event = "event:/Ingeste/Music/lvl8/main";
                    level.Session.Audio.Apply(false);
                };
                return;
            }
            Engine.Scene = new CS08_OverworldReflectionsFall(level, delegate
            {
                Audio.SetAmbience(null, true);
                level.Session.Level = "lvl_a-00";
                level.Session.RespawnPoint = new Vector2?(level.GetSpawnPoint(new Vector2((float)level.Bounds.Center.X, (float)level.Bounds.Top)));
                level.LoadLevel(global::Celeste.Player.IntroTypes.Fall, false);
                level.Add(new BackgroundFadeIn(Color.Black, 2f, 30f));
                level.Entities.UpdateLists();
                foreach (Entity entity in level.Tracker.GetEntities<CrystalStaticSpinner>())
                {
                    ((CrystalStaticSpinner)entity).ForceInstantiate();
                }
            });
        }

        private void SetBloom(float add)
        {
            this.Level.Session.BloomBaseAdd = add;
            this.Level.Bloom.Base = AreaData.Get(this.Level).BloomBase + add;
        }
    }
}



