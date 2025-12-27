namespace DesoloZantas.Core.Core.Cutscenes
{
    /// <summary>
    /// Base class for Ingeste cutscenes with common functionality
    /// </summary>
    public abstract class IngesteCutsceneBase : CutsceneEntity
    {
        protected global::Celeste.Player Player;
        protected Vector2 PlayerStartPosition;

        public IngesteCutsceneBase(global::Celeste.Player player = null) : base(false, true)
        {
            Player = player;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);

            Player ??= Scene.Tracker.GetEntity<global::Celeste.Player>();

            if (Player != null)
                PlayerStartPosition = Player.Position;
        }

        public override void OnBegin(Level level)
        {
            level.RegisterAreaComplete();
            Add(new Coroutine(CutsceneSequence()));
        }

        protected abstract IEnumerator CutsceneSequence();

        protected virtual void SetupPlayer()
        {
            if (Player == null) return;

            Player.StateMachine.State = 11; // Dummy state
            Player.StateMachine.Locked = true;
        }

        protected virtual void RestorePlayer()
        {
            if (Player == null) return;

            Player.StateMachine.State = 0; // Normal state
            Player.StateMachine.Locked = false;
        }

        protected virtual void SetFlag(string flagName)
        {
            if (Level != null && !string.IsNullOrEmpty(flagName))
                Level.Session.SetFlag(flagName);
        }

        public override void OnEnd(Level level)
        {
            RestorePlayer();
            EndCutscene(level);
        }

        public override void Removed(Scene scene)
        {
            RestorePlayer();
            base.Removed(scene);
        }
    }
}




