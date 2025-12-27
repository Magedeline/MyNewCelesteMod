#nullable enable
namespace DesoloZantas.Core.Core.Cutscenes
{
    public class Cs00EndingMod : CutsceneEntity
    {
        private readonly global::Celeste.Player? player;

        /// <summary>
        /// Flag to trigger title screen transition after ending
        /// </summary>
        public static bool ShowTitleScreenAfterEnding { get; set; }

        public Cs00EndingMod(global::Celeste.Player? player) : base(false, true)
        {
            this.player = player;
        }

        public override void OnBegin(Level level)
        {
            if (level != null) Add(new Coroutine(manageCutscene(level, player)));
        }

        private IEnumerator manageCutscene(Level level, global::Celeste.Player? player)
        {
            yield return manageTimeRateDuringCutscene();

            if (player != null)
            {
                player.StateMachine.State = 11; // Dummy state
                player.DummyAutoAnimate = false;
                player.Speed = Vector2.Zero;
                player.Position = new Vector2(level.Bounds.Center.X, level.Bounds.Bottom - 32);
            }

            yield return 2f;

            Audio.SetMusic("event:/music/lvl0/title_ping");

            yield return 2f;

            // Set flag to show custom title screen
            ShowTitleScreenAfterEnding = true;

            // Fade to black and transition to title screen
            level.Add(new FadeWipe(level, false, () =>
            {
                // Transition to overworld with title screen
                TransitionToTitleScreen(level.Session);
            }));

            yield return 1f;
        }

        private void TransitionToTitleScreen(Session session)
        {
            // Mark chapter as complete
            session.SetFlag("cs00_ending_complete");
            
            // Transition to overworld - the DesoloTitleScreen will be shown
            Engine.Scene = new OverworldLoader(Overworld.StartMode.Titlescreen, null);
        }

        private IEnumerator manageTimeRateDuringCutscene()
        {
            var originalTimeRate = Engine.TimeRate;
            Engine.TimeRate = 0.5f;

            yield return 5f;

            Engine.TimeRate = originalTimeRate;
        }

        public override void OnEnd(Level level)
        {
            if (level == null) return;
            Engine.TimeRate = 1f; // Reset time rate to normal after cutscene ends
        }
    }
}



