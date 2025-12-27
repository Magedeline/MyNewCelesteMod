using DesoloZantas.Core.Core.AudioSystems;
using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Cutscenes
{
    /// <summary>
    /// Chapter 5 Master Suite cutscene - Oshiro's haunted mirror encounter
    /// Features Chara emerging from the mirror while Badeline and Ralsei react
    /// </summary>
    public class CS05_OshiroMasterSuite(NPC oshiro) : CutsceneEntity
    {
        public const string Flag = "oshiro_resort_suite";

        // Audio events
        private const string MusicOshiroTheme = "event:/Ingeste/music/lvl5/oshiro_theme";
        private const string MusicEvilChara = "event:/Ingeste/music/lvl2/evil_chara";
        private const string SfxCharaIntro = "event:/Ingeste/game/05_restore/suite_chara_intro";
        private const string SfxMirrorBreak = "event:/Ingeste/game/05_restore/suite_chara_mirrorbreak";
        private const string SfxBadMoveLeft = "event:/game/03_resort/suite_bad_movestageleft";
        private const string SfxCeilingBreak = "event:/game/03_resort/suite_bad_ceilingbreak";
        private const string SfxBadExit = "event:/game/03_resort/suite_bad_exittop";
        private const string SfxOshiroCollapse = "event:/char/oshiro/chat_collapse";

        // Character offsets
        private static readonly Vector2 BadelineOffset = new(-24f, -16f);
        private static readonly Vector2 RalseiOffset = new(-48f, -16f);

        // References
        private readonly NPC oshiro = oshiro;
        private global::Celeste.Player player;
        private CharaDummy chara;
        private BadelineDummy badeline;
        private RalseiDummy ralsei;
        private Entities.ResortMirror mirror;

        public override void OnBegin(Level level)
        {
            mirror = Scene.Entities.FindFirst<Entities.ResortMirror>();
            Add(new Coroutine(Cutscene(level)));
        }

        private IEnumerator Cutscene(Level level)
        {
            // Wait for player to be available
            while ((player = Scene.Tracker.GetEntity<global::Celeste.Player>()) == null)
                yield return null;

            // Setup cutscene state
            Audio.SetMusic(null);
            yield return 0.4f;

            LockPlayer();
            InitializeOshiro();

            // Walk player to Oshiro
            Add(new Coroutine(player.DummyWalkTo(oshiro.X + 32f)));
            yield return 1f;

            // Spawn companions
            SpawnCompanions();
            AudioHelper.SetMusicSafe(MusicOshiroTheme);

            // Main dialogue with event triggers
            yield return Textbox.Say("CH5_OSHIRO_SUITE",
                ShadowAppear,
                ShadowDisrupt,
                ShadowCeiling,
                Wander,
                Console,
                JumpBack,
                Collapse,
                AwkwardPause);

            // Chara exits
            if (chara != null)
            {
                chara.Add(new SoundSource(Vector2.Zero, SfxBadExit));
                Scene.Remove(chara);
            }

            // Restore lighting
            yield return RestoreLighting(level);

            EndCutscene(level);
        }

        #region Setup Methods

        private void LockPlayer()
        {
            player.StateMachine.State = 11;
            player.StateMachine.Locked = true;
        }

        private void UnlockPlayer()
        {
            if (player == null) return;
            player.StateMachine.Locked = false;
            player.StateMachine.State = 0;
        }

        private void InitializeOshiro()
        {
            oshiro.Sprite.Visible = true;
            oshiro.Sprite.Play("idle");
        }

        private void SpawnCompanions()
        {
            badeline = new BadelineDummy(player.Position + BadelineOffset);
            ralsei = new RalseiDummy(player.Position + RalseiOffset);

            Scene.Add(badeline);
            Scene.Add(ralsei);
        }

        #endregion

        #region Dialogue Event Handlers

        private IEnumerator ShadowAppear()
        {
            if (mirror == null) yield break;

            mirror.EvilAppear();
            SetEvilMusic();
            Audio.Play(SfxCharaIntro, mirror.Position);

            // Smooth zoom transition to mirror
            yield return SmoothZoomTo(new Vector2(216f, 110f), 2f);
        }

        private IEnumerator ShadowDisrupt()
        {
            if (mirror == null) yield break;

            Audio.Play(SfxMirrorBreak, mirror.Position);
            yield return mirror.SmashRoutine();

            // Spawn Chara from broken mirror
            chara = new CharaDummy(mirror.Position + new Vector2(0f, -8f));
            Scene.Add(chara);

            yield return 1.2f;
            oshiro.Sprite.Scale.X = 1f;
        }

        private IEnumerator ShadowCeiling()
        {
            yield return Level.ZoomBack(0.5f);

            chara.Add(new SoundSource(Vector2.Zero, SfxBadMoveLeft));
            player.Facing = Facings.Left;
            yield return 0.25f;

            // Dramatic ceiling break
            chara.Add(new SoundSource(Vector2.Zero, SfxCeilingBreak));
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            Level.DirectionalShake(-Vector2.UnitY);

            yield return chara.CollidePoint(chara.Position + new Vector2(0f, -32f));
            yield return 0.8f;
        }

        private IEnumerator Wander()
        {
            yield return 0.5f;

            // Badeline explores the room
            yield return badeline.FloatTo(new Vector2(oshiro.X + 64f, badeline.Y), null, false);
            yield return 1f;

            yield return badeline.FloatTo(new Vector2(oshiro.X - 48f, badeline.Y), null, false);
            yield return 0.1f;

            oshiro.Sprite.Scale.X = -1f;
            yield return 0.2f;

            // Badeline looks up curiously
            badeline.Sprite.Play("lookUp");
            yield return 1f;
            badeline.Sprite.Play("idle");
            yield return 0.4f;

            // Return to player
            yield return badeline.FloatTo(new Vector2(oshiro.X - 40f, badeline.Y), null, false);
            yield return 0.5f;

            yield return Level.ZoomTo(new Vector2(190f, 110f), 2f, 0.5f);
        }

        private IEnumerator Console()
        {
            yield return player.DummyWalkToExact((int)oshiro.X - 16);
        }

        private IEnumerator JumpBack()
        {
            // All companions retreat in alarm
            Add(new Coroutine(badeline.FloatTo(badeline.Position + new Vector2(-32f, 0f), null, false)));
            Add(new Coroutine(ralsei.FloatTo(ralsei.Position + new Vector2(-32f, 0f), null, false)));

            yield return player.DummyWalkToExact((int)oshiro.X - 32, walkBackwards: true);
            yield return 0.8f;
        }

        private IEnumerator Collapse()
        {
            oshiro.Sprite.Play("fall");
            Audio.Play(SfxOshiroCollapse, oshiro.Position);
            yield return null;
        }

        private IEnumerator AwkwardPause()
        {
            yield return 2f;
        }

        #endregion

        #region Utility Methods

        private IEnumerator SmoothZoomTo(Vector2 target, float speed)
        {
            Vector2 from = Level.ZoomFocusPoint;
            for (float t = 0f; t < 1f; t += Engine.DeltaTime * speed)
            {
                Level.ZoomFocusPoint = Vector2.Lerp(from, target, Ease.SineInOut(t));
                yield return null;
            }
            Level.ZoomFocusPoint = target;
        }

        private IEnumerator RestoreLighting(Level level)
        {
            while (level.Lighting.Alpha != level.BaseLightingAlpha)
            {
                level.Lighting.Alpha = Calc.Approach(
                    level.Lighting.Alpha,
                    level.BaseLightingAlpha,
                    Engine.DeltaTime * 0.5f);
                yield return null;
            }
        }

        private void SetEvilMusic()
        {
            if (Level.Session.Area.Mode != AreaMode.Normal) return;

            Level.Session.Audio.Music.Event = MusicEvilChara;
            Level.Session.Audio.Apply();
        }

        #endregion

        #region Cleanup

        public override void OnEnd(Level level)
        {
            if (WasSkipped)
            {
                CleanupOnSkip();
            }

            oshiro.Talker.Enabled = true;
            UnlockPlayer();

            level.Lighting.Alpha = level.BaseLightingAlpha;
            level.Session.SetFlag(Flag);
            SetEvilMusic();
        }

        private void CleanupOnSkip()
        {
            if (chara != null) Scene.Remove(chara);
            if (badeline != null) Scene.Remove(badeline);
            if (ralsei != null) Scene.Remove(ralsei);

            mirror?.Broken();
            Scene.Entities.FindFirst<CelesteDashBlock>()?.RemoveAndFlagAsGone();
            oshiro.Sprite.Play("idle_ground");
        }

        #endregion
    }
}




