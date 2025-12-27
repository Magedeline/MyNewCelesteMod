using System.Runtime.CompilerServices;
using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Cutscenes
{
    public class CS06_Gondola : CutsceneEntity
    {
        private enum GondolaStates
        {
            Stopped,
            MovingToCenter,
            InCenter,
            Shaking,
            MovingToEnd
        }

        private readonly MagolorDummy magolor;
        private BadelineDummy badeline;
        private RalseiDummy ralsei;
        private CharaDummy chara;
        private readonly GondolaMod gondola;
        private readonly global::Celeste.Player player;
        private Parallax loopingCloud;
        private Parallax bottomCloud;
        private WindSnowFG windSnowFg;
        private float LoopCloudsAt;
        private readonly List<CharaReflectionTentacles> tentacles = [];
        private SoundSource moveLoopSfx;
        private SoundSource haltLoopSfx;
        private float gondolaPercent;
        private bool AutoSnapCharacters;
        private float magolorXOffset;
        private float badelineXOffset;
        private float ralseiXOffset;
        private float playerXOffset;
        private float gondolaSpeed;
        private float shakeTimer;
        private const float gondolaMaxSpeed = 64f;
        private float anxiety;
        private float anxietyStutter;
        private readonly float anxietyRumble;
        private BreathingRumbler rumbler;
        private GondolaStates gondolaState;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public CS06_Gondola(MagolorDummy magolor, GondolaMod gondola, global::Celeste.Player player)
            : base(fadeInOnSkip: false, endingChapterAfter: true)
        {
            this.magolor = magolor;
            this.gondola = gondola;
            this.player = player;
        }

        public CS06_Gondola(global::Celeste.Player player, GondolaMod gondola)
        {
            this.player = player;
            this.gondola = gondola;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void OnBegin(Level level)
        {
            level.RegisterAreaComplete();
            foreach (Backdrop backdrop in level.Foreground.Backdrops)
            {
                if (backdrop is WindSnowFG)
                {
                    windSnowFg = backdrop as WindSnowFG;
                }
            }
            Add(moveLoopSfx = new SoundSource());
            Add(haltLoopSfx = new SoundSource());
            Add(new Coroutine(Cutscene()));
        }

        private IEnumerator Cutscene()
        {
            player.StateMachine.State = 11;
            yield return player.DummyWalkToExact((int)gondola.X + 16);
            while (!player.OnGround())
            {
                yield return null;
            }
            Audio.SetMusic("event:/Ingeste/music/lvl1/magolor");
            yield return Textbox.Say("CH6_END", EnterMagolor, CheckOnMagolor, GetUpMagolor, LookAtLever, PullLever, WaitABit, WaitForCenter, SelfieThenStallsOut, MovePlayerLeft, SnapLeverOff, DarknessAppears, DarknessConsumes, CantBreath, StartBreathing, Ascend, WaitABit, BadelineTakesOutPhone, FaceBadeline);
            yield return ShowPhoto();
            EndCutscene(Level);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void OnEnd(Level level)
        {
            if (rumbler != null)
            {
                rumbler.RemoveSelf();
                rumbler = null;
            }
            level.CompleteArea(true, false, false);
            if (!WasSkipped)
            {
                SpotlightWipe.Modifier = 120f;
                SpotlightWipe.FocusPoint = new Vector2(320f, 180f) / 2f;
            }
        }

        private IEnumerator EnterMagolor()
        {
            player.Facing = Facings.Left;
            yield return 0.2f;
            yield return PanCamera(new Vector2(Level.Bounds.Left, magolor.Y - 90f), 1f);
            magolor.Visible = true;
            float magolorStartX = magolor.X;
            
            // Move Magolor manually using Tween
            Tween move1 = Tween.Create(Tween.TweenMode.Oneshot, Ease.Linear, 0.6f, true);
            Vector2 start1 = magolor.Position;
            Vector2 target1 = new(magolorStartX + 35f, magolor.Y);
            move1.OnUpdate = t => magolor.Position = Vector2.Lerp(start1, target1, t.Eased);
            Add(move1);
            yield return 0.6f;
            
            Tween move2 = Tween.Create(Tween.TweenMode.Oneshot, Ease.Linear, 0.6f, true);
            Vector2 start2 = magolor.Position;
            Vector2 target2 = new(magolorStartX + 60f, magolor.Y);
            move2.OnUpdate = t => magolor.Position = Vector2.Lerp(start2, target2, t.Eased);
            Add(move2);
            yield return 0.6f;
            Audio.Play("event:/game/04_cliffside/gondola_theo_fall", magolor.Position);
            magolor.Sprite.Play("idleEdge");
            yield return 1f;
            magolor.Sprite.Play("falling");
            magolor.X += 4f;
            magolor.Depth = -10010;
            float speed = 80f;
            while (magolor.Y < player.Y)
            {
                magolor.Y += speed * Engine.DeltaTime;
                speed += 120f * Engine.DeltaTime;
                yield return null;
            }
            Level.DirectionalShake(new Vector2(0f, 1f));
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            magolor.Y = player.Y;
            magolor.Sprite.Play("hitGround");
            magolor.Sprite.Rate = 0f;
            magolor.Depth = 1000;
            magolor.Sprite.Scale = new Vector2(1.3f, 0.8f);
            yield return 0.5f;
            Vector2 start = magolor.Sprite.Scale;
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 2f, start: true);
            tween.OnUpdate = [MethodImpl(MethodImplOptions.NoInlining)] (Tween t) =>
            {
                magolor.Sprite.Scale.X = MathHelper.Lerp(start.X, 1f, t.Eased);
                magolor.Sprite.Scale.Y = MathHelper.Lerp(start.Y, 1f, t.Eased);
            };
            Add(tween);
            yield return PanCamera(new Vector2(Level.Bounds.Left, magolor.Y - 120f), 1f);
            yield return 0.6f;
        }

        private IEnumerator CheckOnMagolor()
        {
            yield return player.DummyWalkTo(gondola.X - 18f);
        }

        private IEnumerator GetUpMagolor()
        {
            yield return 1.4f;
            Audio.Play("event:/game/04_cliffside/gondola_theo_recover", magolor.Position);
            magolor.Sprite.Rate = 1f;
            magolor.Sprite.Play("recoverGround");
            yield return 1.6f;
            
            // Move Magolor to final position using Tween
            Tween moveToFinal = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 1f, true);
            Vector2 moveStart = magolor.Position;
            Vector2 moveTarget = new(gondola.X - 50f, player.Y);
            moveToFinal.OnUpdate = t => magolor.Position = Vector2.Lerp(moveStart, moveTarget, t.Eased);
            Add(moveToFinal);
            yield return 1f;
            yield return 0.2f;
        }

        private IEnumerator LookAtLever()
        {
            // Move Magolor to lever position using Tween
            Tween moveToLever = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.8f, true);
            Vector2 leverStart = magolor.Position;
            Vector2 leverTarget = new(gondola.X + 7f, magolor.Y);
            moveToLever.OnUpdate = t => magolor.Position = Vector2.Lerp(leverStart, leverTarget, t.Eased);
            Add(moveToLever);
            yield return 0.8f;
            
            player.Facing = Facings.Right;
            magolor.Sprite.Scale.X = -1f;
        }

        private IEnumerator PullLever()
        {
            Add(new Coroutine(player.DummyWalkToExact((int)gondola.X - 7)));
            magolor.Sprite.Scale.X = -1f;
            yield return 0.2f;
            Audio.Play("event:/game/04_cliffside/gondola_theo_lever_start", magolor.Position);
            magolor.Sprite.Play("pullVent");
            yield return 1f;
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            gondola.Lever.Play("pulled");
            magolor.Sprite.Play("fallVent");
            yield return 0.6f;
            Level.Shake();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            yield return 0.5f;
            yield return PanCamera(gondola.Position + new Vector2(-160f, -120f), 1f);
            yield return 0.5f;
            Level.Background.Backdrops.Add(loopingCloud = new Parallax(GFX.Game["bgs/04/bgCloudLoop"]));
            Level.Background.Backdrops.Add(bottomCloud = new Parallax(GFX.Game["bgs/04/bgCloud"]));
            Parallax parallax = loopingCloud;
            Parallax parallax2 = bottomCloud;
            bool loopX = true;
            parallax2.LoopX = true;
            parallax.LoopX = loopX;
            Parallax parallax3 = loopingCloud;
            Parallax parallax4 = bottomCloud;
            loopX = false;
            parallax4.LoopY = false;
            parallax3.LoopY = loopX;
            loopingCloud.Position.Y = Level.Camera.Top - (float)loopingCloud.Texture.Height - (float)bottomCloud.Texture.Height;
            bottomCloud.Position.Y = Level.Camera.Top - (float)bottomCloud.Texture.Height;
            LoopCloudsAt = bottomCloud.Position.Y;
            AutoSnapCharacters = true;
            magolorXOffset = magolor.X - gondola.X;
            playerXOffset = player.X - gondola.X;
            player.StateMachine.State = 17;
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, null, 16f, start: true);
            tween.OnUpdate = [MethodImpl(MethodImplOptions.NoInlining)] (Tween t) =>
            {
                if (Audio.CurrentMusic == "event:/Ingeste/music/lvl1/magolor")
                {
                    Audio.SetMusicParam("fade", 1f - t.Eased);
                }
            };
            Add(tween);
            SoundSource soundSource = new();
            soundSource.Position = gondola.LeftCliffside.Position;
            soundSource.Play("event:/game/04_cliffside/gondola_cliffmechanism_start");
            Add(soundSource);
            moveLoopSfx.Play("event:/game/04_cliffside/gondola_movement_loop");
            Level.Shake();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.FullSecond);
            gondolaSpeed = 32f;
            gondola.RotationSpeed = 1f;
            gondolaState = GondolaStates.MovingToCenter;
            yield return 1f;
            yield return MoveMagolorOnGondola(12f, changeFacing: false);
            yield return 0.2f;
            magolor.Sprite.Scale.X = -1f;
        }

        private IEnumerator WaitABit()
        {
            yield return 1f;
        }

        private IEnumerator WaitForCenter()
        {
            while (gondolaState != GondolaStates.InCenter)
            {
                yield return null;
            }
            magolor.Sprite.Scale.X = 1f;
            yield return 1f;
            yield return MovePlayerOnGondola(-20f);
            yield return 0.5f;
        }

        private IEnumerator SelfieThenStallsOut()
        {
            Audio.SetMusic("event:/Ingeste/music/lvl6/minigame");
            Add(new Coroutine(Level.ZoomTo(new Vector2(160f, 110f), 2f, 0.5f)));
            yield return 0.3f;
            magolor.Sprite.Scale.X = 1f;
            yield return 0.2f;
            Add(new Coroutine(MovePlayerOnGondola(magolorXOffset - 8f)));
            yield return 0.4f;
            Audio.Play("event:/game/04_cliffside/gondola_theoselfie_halt", magolor.Position);
            magolor.Sprite.Play("holdOutPhone");
            yield return 1.5f;
            magolorXOffset += 4f;
            playerXOffset += 4f;
            if (badeline != null) badelineXOffset += 4f;
            if (ralsei != null) ralseiXOffset += 4f;
            gondola.RotationSpeed = -1f;
            gondolaState = GondolaStates.Stopped;
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            magolor.Sprite.Play("takeSelfieImmediate");
            Add(new Coroutine(PanCamera(gondola.Position + (gondola.Destination - gondola.Position).SafeNormalize() * 32f + new Vector2(-160f, -120f), 0.3f, Ease.CubeOut)));
            yield return 0.5f;
            Level.Flash(Color.White);
            
            // Create Badeline if she doesn't exist
            if (badeline == null)
            {
                badeline = new BadelineDummy(Vector2.Zero);
                Level.Add(badeline);
            }
            badeline.Appear(Level);
            badeline.Floatness = 0f;
            badeline.Depth = -1000000;
            badelineXOffset = -24f;
            
            // Create Ralsei if he doesn't exist
            if (ralsei == null)
            {
                ralsei = new RalseiDummy(Vector2.Zero);
                Level.Add(ralsei);
            }
            ralseiXOffset = -48f;
            
            moveLoopSfx.Stop();
            haltLoopSfx.Play("event:/game/04_cliffside/gondola_halted_loop");
            gondolaState = GondolaStates.Shaking;
            yield return PanCamera(gondola.Position + new Vector2(-160f, -120f), 1f);
            
            // Add Chara jumpscare
            yield return 0.8f;
            Level.Flash(Color.Red);
            Audio.Play("event:/char/badeline/maddy_split", Level.Camera.Position);
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            
            if (chara == null)
            {
                chara = new CharaDummy(player.Position + new Vector2(80f, -32f));
                Level.Add(chara);
            }
            chara.Floatness = 0f;
            chara.Depth = -1000000;
            chara.Sprite.Scale = Vector2.One * 1.2f;
            
            yield return 1.2f;
            
            // Chara disappears
            Level.Flash(Color.Black);
            Audio.Play("event:/char/badeline/level_entry", chara?.Position ?? Vector2.Zero);
            
            if (chara != null)
            {
                Tween fadeTween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, 0.5f, true);
                fadeTween.OnUpdate = t => {
                    if (chara?.Sprite != null)
                    {
                        chara.Sprite.Color = Color.White * (1f - t.Eased);
                    }
                };
                fadeTween.OnComplete = t => {
                    chara?.RemoveSelf();
                    chara = null;
                };
                Add(fadeTween);
            }
            
            yield return 1f;
        }

        private IEnumerator MovePlayerLeft()
        {
            yield return MovePlayerOnGondola(-20f);
            magolor.Sprite.Scale.X = -1f;
            yield return 0.5f;
            yield return MovePlayerOnGondola(20f);
            yield return 0.5f;
            yield return MovePlayerOnGondola(-10f);
            yield return 0.5f;
            player.Facing = Facings.Right;
        }

        private IEnumerator SnapLeverOff()
        {
            yield return MoveMagolorOnGondola(7f);
            Audio.Play("event:/game/04_cliffside/gondola_theo_lever_fail", magolor.Position);
            magolor.Sprite.Play("pullVent");
            yield return 1f;
            magolor.Sprite.Play("fallVent");
            yield return 1f;
            gondola.BreakLever();
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Level.Shake();
            yield return 2.5f;
        }

        private IEnumerator DarknessAppears()
        {
            Audio.SetMusicParam("calm", 0f);
            yield return 0.25f;
            player.Sprite.Play("tired");
            yield return 0.25f;
            if (badeline != null)
            {
                badeline.Vanish();
            }
            yield return 0.3f;
            Level.NextColorGrade("panicattack", 1f);
            Level.Shake();
            Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
            BurstTentacles(3, 90f);
            Audio.Play("event:/Ingeste/game/06_stronghold/gondola_scaryvines_01", gondola.Position);
            for (float p = 0f; p < 1f; p += Engine.DeltaTime / 2f)
            {
                yield return null;
                Level.Background.Fade = p;
                anxiety = p;
                if (windSnowFg != null)
                {
                    windSnowFg.Alpha = 1f - p;
                }
            }
            yield return 0.25f;
        }

        private IEnumerator DarknessConsumes()
        {
            Level.Shake();
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Audio.Play("event:/Ingeste/game/06_stronghold/gondola_scaryvines_02", gondola.Position);
            BurstTentacles(2, 60f);
            yield return MoveMagolorOnGondola(0f);
            magolor.Sprite.Play("comfortStart");
        }

        private IEnumerator CantBreath()
        {
            Level.Shake();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            Audio.Play("event:/Ingeste/game/06_stronghold/gondola_scaryvines_03", gondola.Position);
            BurstTentacles(1, 30f);
            BurstTentacles(0, 0f, 100f);
            rumbler = new BreathingRumbler(player.Position);
            Scene.Add(rumbler);
            yield return null;
        }

        private IEnumerator StartBreathing()
        {
            BreathingMinigame breathing = new(winnable: true, rumbler);
            Scene.Add(breathing);
            while (!breathing.Completed)
            {
                yield return null;
            }
            foreach (CharaReflectionTentacles tentacle in tentacles)
            {
                tentacle.RemoveSelf();
            }
            anxiety = 0f;
            Level.Background.Fade = 0f;
            Level.SnapColorGrade(null);
            gondola.CancelPullSides();
            Level.ResetZoom();
            yield return 0.5f;
            Audio.Play("event:/game/04_cliffside/gondola_restart", gondola.Position);
            yield return 1f;
            moveLoopSfx.Play("event:/game/04_cliffside/gondola_movement_loop");
            haltLoopSfx.Stop();
            Level.Shake();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            gondolaState = GondolaStates.InCenter;
            gondola.RotationSpeed = 0.5f;
            yield return 1.2f;
        }

        private IEnumerator Ascend()
        {
            gondolaState = GondolaStates.MovingToEnd;
            while (gondolaState != GondolaStates.Stopped)
            {
                yield return null;
            }
            Level.Shake();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            moveLoopSfx.Stop();
            Audio.Play("event:/game/04_cliffside/gondola_finish", gondola.Position);
            gondola.RotationSpeed = 0.5f;
            yield return 0.1f;
            while (gondola.Rotation > 0f)
            {
                yield return null;
            }
            gondola.Rotation = (gondola.RotationSpeed = 0f);
            Level.Shake();
            AutoSnapCharacters = false;
            player.StateMachine.State = 11;
            player.Position = player.Position.Floor();
            while (player.CollideCheck<Solid>())
            {
                player.Y--;
            }
            magolor.Position.Y = player.Position.Y;
            magolor.Sprite.Play("comfortRecover");
            magolor.Sprite.Scale.X = 1f;
            
            if (badeline != null)
            {
                badeline.Position.Y = player.Position.Y;
            }
            if (ralsei != null)
            {
                ralsei.Position.Y = player.Position.Y;
            }
            
            yield return player.DummyWalkTo(gondola.X + 80f);
            player.DummyAutoAnimate = false;
            player.Sprite.Play("tired");
            
            // Move Magolor to final position using Tween
            Tween moveToEnd = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 1f, true);
            Vector2 endStart = magolor.Position;
            Vector2 endTarget = new(gondola.X + 64f, magolor.Y);
            moveToEnd.OnUpdate = t => magolor.Position = Vector2.Lerp(endStart, endTarget, t.Eased);
            Add(moveToEnd);
            yield return 1f;
            yield return 0.5f;
        }

        private IEnumerator BadelineTakesOutPhone()
        {
            player.Facing = Facings.Right;
            yield return 0.25f;
            
            // Recreate Badeline if she doesn't exist
            if (badeline == null)
            {
                badeline = new BadelineDummy(Vector2.Zero);
                Level.Add(badeline);
                badeline.Position = new Vector2(gondola.X + 50f, player.Y);
            }
            badeline.Sprite.Play("usePhone");
            yield return 2f;
        }

        private IEnumerator FaceBadeline()
        {
            player.DummyAutoAnimate = true;
            yield return 0.2f;
            player.Facing = Facings.Left;
            yield return 0.2f;
        }

        private IEnumerator ShowPhoto()
        {
            if (badeline != null)
            {
                badeline.Sprite.Scale.X = -1f;
                yield return 0.25f;
                yield return player.DummyWalkTo(badeline.X + 5f);
            }
            yield return 1f;
            Selfie selfie = new(SceneAs<Level>());
            Scene.Add(selfie);
            yield return selfie.OpenRoutine("selfieGondola");
            yield return selfie.WaitForInput();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override void Update()
        {
            base.Update();
            if (anxietyRumble > 0f)
            {
                Input.RumbleSpecific(anxietyRumble, 0.1f);
            }
            if (base.Scene.OnInterval(0.05f))
            {
                anxietyStutter = Calc.Random.NextFloat(0.1f);
            }
            Distort.AnxietyOrigin = new Vector2(0.5f, 0.5f);
            Distort.Anxiety = anxiety * 0.2f + anxietyStutter * anxiety;
            if (moveLoopSfx != null && gondola != null)
            {
                moveLoopSfx.Position = gondola.Position;
            }
            if (haltLoopSfx != null && gondola != null)
            {
                haltLoopSfx.Position = gondola.Position;
            }
            if (gondolaState == GondolaStates.MovingToCenter)
            {
                MoveGondolaTowards(0.5f);
                if (gondolaPercent >= 0.5f)
                {
                    gondolaState = GondolaStates.InCenter;
                }
            }
            else if (gondolaState == GondolaStates.InCenter)
            {
                Vector2 vector = (gondola.Destination - gondola.Position).SafeNormalize() * gondolaSpeed;
                if (loopingCloud != null)
                {
                    loopingCloud.CameraOffset.X += vector.X * Engine.DeltaTime;
                    loopingCloud.CameraOffset.Y += vector.Y * Engine.DeltaTime;
                }
                if (windSnowFg != null && loopingCloud != null)
                {
                    windSnowFg.CameraOffset = loopingCloud.CameraOffset;
                }
                if (loopingCloud != null)
                {
                    loopingCloud.LoopY = true;
                }
            }
            else if (gondolaState != GondolaStates.Stopped)
            {
                if (gondolaState == GondolaStates.Shaking)
                {
                    Level.Wind.X = -400f;
                    if (shakeTimer <= 0f && (gondola.Rotation == 0f || gondola.Rotation < -0.25f))
                    {
                        shakeTimer = 1f;
                        gondola.RotationSpeed = 0.5f;
                    }
                    shakeTimer -= Engine.DeltaTime;
                }
                else if (gondolaState == GondolaStates.MovingToEnd)
                {
                    MoveGondolaTowards(1f);
                    if (gondolaPercent >= 1f)
                    {
                        gondolaState = GondolaStates.Stopped;
                    }
                }
            }
            if (loopingCloud != null && !loopingCloud.LoopY && Level.Camera.Bottom < LoopCloudsAt)
            {
                loopingCloud.LoopY = true;
            }
            if (AutoSnapCharacters)
            {
                magolor.Position = gondola.GetRotatedFloorPositionAt(magolorXOffset);
                player.Position = gondola.GetRotatedFloorPositionAt(playerXOffset);
                if (badeline != null)
                {
                    badeline.Position = gondola.GetRotatedFloorPositionAt(badelineXOffset, 20f);
                }
                if (ralsei != null)
                {
                    ralsei.Position = gondola.GetRotatedFloorPositionAt(ralseiXOffset);
                }
                if (chara != null)
                {
                    chara.Position = gondola.GetRotatedFloorPositionAt(-36f, 25f);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void MoveGondolaTowards(float percent)
        {
            float num = (gondola.Start - gondola.Destination).Length();
            gondolaSpeed = Calc.Approach(gondolaSpeed, 64f, 120f * Engine.DeltaTime);
            gondolaPercent = Calc.Approach(gondolaPercent, percent, gondolaSpeed / num * Engine.DeltaTime);
            gondola.Position = (gondola.Start + (gondola.Destination - gondola.Start) * gondolaPercent).Floor();
            Level.Camera.Position = gondola.Position + new Vector2(-160f, -120f);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IEnumerator PanCamera(Vector2 to, float duration, Ease.Easer ease = null)
        {
            if (ease == null)
            {
                ease = Ease.CubeInOut;
            }
            Vector2 from = Level.Camera.Position;
            for (float t = 0f; t < 1f; t += Engine.DeltaTime / duration)
            {
                yield return null;
                Level.Camera.Position = from + (to - from) * ease(Math.Min(t, 1f));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IEnumerator MovePlayerOnGondola(float x)
        {
            player.Sprite.Play("walk");
            player.Facing = (Facings)Math.Sign(x - playerXOffset);
            while (playerXOffset != x)
            {
                playerXOffset = Calc.Approach(playerXOffset, x, 48f * Engine.DeltaTime);
                yield return null;
            }
            player.Sprite.Play("idle");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IEnumerator MoveMagolorOnGondola(float x, bool changeFacing = true)
        {
            magolor.Sprite.Play("walk");
            if (changeFacing)
            {
                magolor.Sprite.Scale.X = Math.Sign(x - magolorXOffset);
            }
            while (magolorXOffset != x)
            {
                magolorXOffset = Calc.Approach(magolorXOffset, x, 48f * Engine.DeltaTime);
                yield return null;
            }
            magolor.Sprite.Play("idle");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void BurstTentacles(int layer, float dist, float from = 200f)
        {
            Vector2 vector = Level.Camera.Position + new Vector2(160f, 90f);
            CharaReflectionTentacles reflectionTentacles = [];
            reflectionTentacles.Create(0f, 0, layer, new List<Vector2>
            {
                vector + new Vector2(0f - from, 0f),
                vector + new Vector2(-800f, 0f)
            });
            reflectionTentacles.SnapTentacles();
            reflectionTentacles.Nodes[0] = vector + new Vector2(0f - dist, 0f);
            CharaReflectionTentacles reflectionTentacles2 = new();
            reflectionTentacles2.Create(0f, 0, layer, new List<Vector2>
            {
                vector + new Vector2(from, 0f),
                vector + new Vector2(800f, 0f)
            });
            reflectionTentacles2.SnapTentacles();
            reflectionTentacles2.Nodes[0] = vector + new Vector2(dist, 0f);
            tentacles.Add(reflectionTentacles);
            tentacles.Add(reflectionTentacles2);
            Level.Add(reflectionTentacles);
            Level.Add(reflectionTentacles2);
        }
    }
}



