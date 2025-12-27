namespace DesoloZantas.Core.Core.Triggers
{
    /// <summary>
    /// Trigger that starts the Vessel Creation Vignette
    /// This should be placed before the IntroVignette in the game flow
    /// </summary>
    [CustomEntity("Ingeste/VesselCreationTrigger")]
    public class VesselCreationTrigger : Trigger
    {
        private bool hasTriggered = false;
        private readonly bool triggerOnce;
        private readonly bool autoStart;

        public VesselCreationTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            triggerOnce = data.Bool("triggerOnce", true);
            autoStart = data.Bool("autoStart", false);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            // Auto-start if enabled
            if (autoStart && !hasTriggered)
            {
                startVesselCreation();
            }
        }

        public override void OnEnter(global::Celeste.Player player)
        {
            base.OnEnter(player);

            if (!autoStart && (!triggerOnce || !hasTriggered))
            {
                startVesselCreation();
            }
        }

        private void startVesselCreation()
        {
            if (hasTriggered && triggerOnce)
                return;

            hasTriggered = true;

            if (Scene is Level level)
            {
                // Get the current session
                Session session = level.Session;
                
                // Create and transition to the VesselCreationVignette
                Engine.Scene = new Cutscenes.VesselCreationVignette(session);
            }
        }
    }
}



