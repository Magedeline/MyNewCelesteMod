namespace DesoloZantas.Core.Core.Cutscenes
{
    [CustomEntity("DesoloZantas/PayphoneCutscene")]
    public class PayphoneCutscene : CutsceneEntity
    {
        private global::Celeste.Player player;
        private bool isAwake;
        private string cutsceneType;

        public PayphoneCutscene(EntityData data, Vector2 offset) : base()
        {
            Position = data.Position + offset;
            cutsceneType = data.Attr("cutsceneType", "dream");
            isAwake = cutsceneType == "awake";
        }

        public override void OnBegin(Level level)
        {
            player = Scene.Tracker.GetEntity<global::Celeste.Player>();
            
            Add(new Coroutine(PayphoneSequence()));
        }

        public override void OnEnd(Level level)
        {
            // Set completion flag
            level.Session.SetFlag(isAwake ? "payphone_awake_complete" : "payphone_dream_complete");
        }

        private IEnumerator PayphoneSequence()
        {
            if (player != null)
            {
                player.StateMachine.State = global::Celeste.Player.StDummy;
            }

            Level level = Scene as Level;

            if (isAwake)
            {
                yield return Textbox.Say("KIRBY_PAYPHONE_AWAKE_END");
            }
            else
            {
                yield return Textbox.Say("KIRBY_PAYPHONE_DREAM_END");
            }

            EndCutscene(level);
        }
    }
}



