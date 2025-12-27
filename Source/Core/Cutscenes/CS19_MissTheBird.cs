using FMOD.Studio;
using FlingBirdIntroMod = DesoloZantas.Core.Core.Entities.FlingBirdIntro;
using CutsceneNode = DesoloZantas.Core.Core.Entities.CutsceneNode;
using Facings = Celeste.Facings;

namespace DesoloZantas.Core.Core.Cutscenes
{
    public class CS19_MissTheBird : CutsceneEntity
    {
        public const string Flag = "MissTheBird";
        private global::Celeste.Player player;
        private FlingBirdIntroMod flingBirdmod;
        private BirdNPC bird;
        private Coroutine zoomRoutine;
        private EventInstance crashMusicSfx;

        public CS19_MissTheBird(global::Celeste.Player player, FlingBirdIntroMod flingBirdmod) : base(true, false)
        {
            this.player = player;
            this.flingBirdmod = flingBirdmod;
            Add(new LevelEndingHook(delegate {
                Audio.Stop(this.crashMusicSfx, true);
            }));
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(Cutscene(level), true));
            StartMusic();
            TriggerEnvironmentalEvents();
        }

        private IEnumerator Cutscene(Level level)
        {
            Audio.SetMusicParam("bird_grab", 1f);
            this.crashMusicSfx = Audio.Play("event:/Ingeste/final_content/music/lvl19/cinematic/bird_crash_first");
            // If you want to use DoGrabbingRoutine, uncomment below:
            // yield return this.flingBirdmod.DoGrabbingRoutine(this.player);
            this.bird = new BirdNPC(this.flingBirdmod.BirdEndPosition, BirdNPC.Modes.None);
            level.Add(this.bird);
            this.flingBirdmod.RemoveSelf();
            yield return null;
            level.ResetZoom();
            level.Shake(0.5f);
            this.player.Position = this.player.Position.Floor();
            this.player.DummyGravity = true;
            this.player.DummyAutoAnimate = false;
            this.player.DummyFriction = false;
            this.player.ForceCameraUpdate = true;
            this.player.Speed = new Vector2(200f, 200f);
            this.bird.Position += Vector2.UnitX * 16f;
            this.bird.Add(new Coroutine(this.bird.Startle(null, 0.5f, new Vector2(3f, 0.25f)), true));
            Add(Alarm.Create(Alarm.AlarmMode.Oneshot, delegate {
                Add(new Coroutine(this.bird.FlyAway(0.2f), true));
                this.bird.Position += new Vector2(0f, -4f);
            }, 0.8f, true));
            // Ground the player safely with a timeout to avoid potential infinite loops
            float maxGroundTime = 2f;
            while (!this.player.OnGround() && maxGroundTime > 0f)
            {
                this.player.MoveVExact(1);
                maxGroundTime -= Engine.DeltaTime;
                yield return null;
            }

            // Avoid global time scaling to prevent perceived freezes
            this.player.Sprite.Play("roll");
            while (this.player.Speed.X != 0f)
            {
                this.player.Speed.X = Calc.Approach(this.player.Speed.X, 0f, 120f * Engine.DeltaTime);
                if (Scene.OnInterval(0.1f))
                    Dust.BurstFG(this.player.Position, -1.5707964f, 2);
                yield return null;
            }
            this.player.Speed.X = 0f;
            this.player.DummyFriction = true;
            yield return 0.25f;
            Add(this.zoomRoutine = new Coroutine(level.ZoomTo(new Vector2(160f, 110f), 1.5f, 6f), true));
            yield return 1.5f;
            this.player.Sprite.Play("rollGetUp");
            yield return 0.5f;
            this.player.ForceCameraUpdate = false;
            yield return Textbox.Say("CH19_MISS_THE_BIRD", new Func<IEnumerator>[] {
                StandUpFaceLeft,
                TakeStepLeft,
                TakeStepRight,
                FlickerBlackhole,
                FlickerJumpscareBlackhole,
                OpenBlackhole
            });
            StartMusic();
            EndCutscene(level);
        }

        private IEnumerator StandUpFaceLeft()
        {
            while (!this.zoomRoutine.Finished)
                yield return null;
            yield return 0.2f;
            Audio.Play("event:/char/madeline/stand", this.player.Position);
            this.player.DummyAutoAnimate = true;
            this.player.Sprite.Play("idle");
            yield return 0.2f;
            this.player.Facing = Facings.Left;
            yield return 0.5f;
        }

        private IEnumerator TakeStepLeft()
        {
            yield return this.player.DummyWalkTo(this.player.X - 16f);
        }

        private IEnumerator TakeStepRight()
        {
            yield return this.player.DummyWalkTo(this.player.X + 32f);
        }

        private IEnumerator FlickerBlackhole()
        {
            yield return 0.5f;
            Audio.Play("event:/Ingeste/final_content/music/lvl19/cinematic/els_intro_laugh");
            yield return MoonGlitchBackgroundTrigger.GlitchRoutine(0.5f, false);
            yield return this.player.DummyWalkTo(this.player.X - 8f, true);
            yield return 0.4f;
        }

        private IEnumerator FlickerJumpscareBlackhole()
        {
            yield return 0.5f;
            Audio.Play("event:/Ingeste/final_content/music/lvl19/cinematic/els_intro_scream");
            yield return MoonGlitchBackgroundTrigger.GlitchRoutine(0.5f, false);
            yield return this.player.DummyWalkTo(this.player.X - 8f, true);
            yield return 0.4f;
        }

        private IEnumerator OpenBlackhole()
        {
            yield return 0.2f;
            this.Level.ResetZoom();
            this.Level.Flash(Color.White);
            this.Level.Shake(0.4f);
            this.Level.Add(new LightningStrike(new Vector2(this.player.X, this.Level.Bounds.Top), 80, 240f));
            this.Level.Add(new LightningStrike(new Vector2(this.player.X - 100f, this.Level.Bounds.Top), 90, 240f, 0.5f));
            Audio.Play("event:/new_content/game/10_farewell/lightning_strike");
            TriggerEnvironmentalEvents();
            StartMusic();
            yield return 1.2f;
        }

        private void StartMusic()
        {
            this.Level.Session.Audio.Music.Event = "event:/Ingeste/final_content/music/lvl19/part03";
            this.Level.Session.Audio.Ambience.Event = "event:/Ingeste/final_content/env/19_vortex";
            this.Level.Session.Audio.Apply();
        }

        private void TriggerEnvironmentalEvents()
        {
            CutsceneNode cutsceneNode = CutsceneNode.Find("player_skip");
            if (cutsceneNode != null)
                RumbleTrigger.ManuallyTrigger(((Entity)cutsceneNode).X, 0f);
            Scene.Entities.FindFirst<MoonGlitchBackgroundTrigger>()?.Invoke();
        }

        public override void OnEnd(Level level)
        {
            Audio.Stop(this.crashMusicSfx, true);
            Engine.TimeRate = 1f;
            level.Session.SetFlag(Flag);
            if (this.WasSkipped)
            {
                this.player.Sprite.Play("idle");
                CutsceneNode cutsceneNode = CutsceneNode.Find("player_skip");
                if (cutsceneNode != null)
                {
                    this.player.Position = ((Entity)cutsceneNode).Position.Floor();
                    level.Camera.Position = this.player.CameraTarget;
                }
                if (this.flingBirdmod != null)
                {
                    if (this.flingBirdmod.CrashSfxEmitter != null)
                        this.Scene.Remove(this.flingBirdmod.CrashSfxEmitter);
                    this.flingBirdmod.RemoveSelf();
                }
                if (this.bird != null)
                    this.bird.RemoveSelf();
                TriggerEnvironmentalEvents();
                StartMusic();
            }
            this.player.Speed = Vector2.Zero;
            this.player.DummyAutoAnimate = true;
            this.player.DummyFriction = true;
            this.player.DummyGravity = true;
            this.player.ForceCameraUpdate = false;
            this.player.StateMachine.State = 0;
        }
    }
}





