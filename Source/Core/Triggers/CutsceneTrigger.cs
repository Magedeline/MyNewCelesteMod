namespace DesoloZantas.Core.Core.Triggers
{
    /// <summary>
    /// Trigger that starts cutscenes with various options
    /// </summary>
    [CustomEntity("Ingeste/CutsceneTrigger")]
    public class CutsceneTrigger : Trigger
    {
        private string cutsceneId;
        private bool triggerOnce;
        private bool playerOnly;
        private bool autoStart;
        private bool hasTriggered;
        private Level level;

        public CutsceneTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            cutsceneId = data.Attr(nameof(cutsceneId), "desolo_intro");
            triggerOnce = data.Bool(nameof(triggerOnce), true);
            playerOnly = data.Bool(nameof(playerOnly), true);
            autoStart = data.Bool(nameof(autoStart), false);
            hasTriggered = false;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;

            // Auto-start cutscene on level load if enabled
            if (autoStart && !hasTriggered)
            {
                startCutscene();
            }
        }

        public override void OnEnter(global::Celeste.Player player)
        {
            base.OnEnter(player);

            if (!autoStart && (!triggerOnce || !hasTriggered))
            {
                startCutscene();
            }
        }

        public override void OnStay(global::Celeste.Player player)
        {
            base.OnStay(player);

            // Allow continuous triggering if not set to trigger once
            if (!triggerOnce && !autoStart)
            {
                startCutscene();
            }
        }

        private void startCutscene()
        {
            if (level == null || string.IsNullOrEmpty(cutsceneId))
                return;

            hasTriggered = true;
            {
                // Fix: Use a lambda that returns IEnumerator to match the expected signature
                level.StartCutscene(lvl => createCutscene(lvl), fadeInOnSkip: true);
            }
        }

        private IEnumerator createCutscene(Level level) {
            // Retrieve the player entity
            var player = level.Tracker.GetEntity<global::Celeste.Player>();

            // Set the player's state to idle
            player?.StateMachine.SetStateName(global::Celeste.Player.StNormal, "idle");

            // Play a sound effect
            Audio.Play("event:/game/general/crystalheart_pulse", Position);

            // Wait for a short duration
            yield return 1f;

            // Check if the cutscene ID corresponds to a dialog
            if (cutsceneId.StartsWith("dialog_")) {
                var dialogKey = cutsceneId.Substring("dialog_".Length); // Extract dialog key
                yield return Textbox.Say(dialogKey);
            } else {
                // Default cutscene behavior
                yield return Textbox.Say("CUTSCENE_DEFAULT");
            }

            // Restore the player's state to normal
            if (player != null) player.StateMachine.State = global::Celeste.Player.StNormal;
        }
    }
}




