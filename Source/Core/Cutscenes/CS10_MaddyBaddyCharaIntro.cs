using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Cutscenes
{
    /// <summary>
    /// Cutscene for Chapter 10 spawn point featuring Madeline, Badeline, and Chara
    /// </summary>
    public class CS10_MaddyBaddyCharaIntro : CutsceneEntity
    {
        private global::Celeste.Player player;
        private BadelineDummy badelineDummy;
        private CharaDummy charaDummy;
        private string spawnRoomName;

        public CS10_MaddyBaddyCharaIntro(global::Celeste.Player player, string roomName) 
            : base(fadeInOnSkip: true)
        {
            this.player = player;
            this.spawnRoomName = roomName;
        }

        public override void OnBegin(Level level)
        {
            // Check if cutscene already played
            if (level.Session.GetFlag("ch10_maddy_baddy_chara_intro_played"))
            {
                RemoveSelf();
                return;
            }

            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            // Wait for player to be ready
            while (player == null || !player.OnGround())
            {
                yield return null;
            }

            // Lock player movement
            player.StateMachine.State = global::Celeste.Player.StDummy;

            // Spawn Badeline dummy to the right of player
            Vector2 badelinePos = player.Position + new Vector2(32f, 0f);
            badelineDummy = new BadelineDummy(badelinePos);
            badelineDummy.Add(new TalkComponent(
                new Rectangle(-24, -8, 48, 8),
                new Vector2(0f, -24f),
                (p) => OnTalkToBadeline(p)
            ));
            level.Add(badelineDummy);

            // Spawn Chara dummy to the right of Badeline
            Vector2 charaPos = badelinePos + new Vector2(32f, 0f);
            charaDummy = new CharaDummy(charaPos);
            charaDummy.Add(new TalkComponent(
                new Rectangle(-24, -8, 48, 8),
                new Vector2(0f, -24f),
                (p) => OnTalkToChara(p)
            ));
            level.Add(charaDummy);

            yield return 0.5f;

            // Start dialogue
            yield return Textbox.Say("CH10_MADDY_AND_BADDY_AND_CHARA");

            // Restore player control
            player.StateMachine.State = global::Celeste.Player.StNormal;

            // Mark cutscene as played and flag that NPCs are present
            level.Session.SetFlag("ch10_maddy_baddy_chara_intro_played");
            level.Session.SetFlag("ch10_badeline_chara_present");

            EndCutscene(level);
        }

        private void OnTalkToBadeline(global::Celeste.Player player)
        {
            Level level = Scene as Level;
            if (level == null) return;

            int talkCount = level.Session.GetCounter("ch10_badeline_talk_count");
            
            string dialogKey = talkCount switch
            {
                0 => "CH10_TALK_TO_BADDY_AND_CHARA1",
                1 => "CH10_TALK_TO_BADDY_AND_CHARA3",
                _ => "CH10_TALK_TO_BADDY_AND_CHARA1" // Loop back
            };

            level.Session.IncrementCounter("ch10_badeline_talk_count");
            
            Scene.Add(new CS_TalkToNPC(player, dialogKey));
        }

        private void OnTalkToChara(global::Celeste.Player player)
        {
            Level level = Scene as Level;
            if (level == null) return;

            int talkCount = level.Session.GetCounter("ch10_chara_talk_count");
            
            string dialogKey = talkCount switch
            {
                0 => "CH10_TALK_TO_BADDY_AND_CHARA0",
                1 => "CH10_TALK_TO_BADDY_AND_CHARA2",
                _ => "CH10_TALK_TO_BADDY_AND_CHARA0" // Loop back
            };

            level.Session.IncrementCounter("ch10_chara_talk_count");
            
            Scene.Add(new CS_TalkToNPC(player, dialogKey));
        }

        public override void OnEnd(Level level)
        {
            // Player movement will be restored by the cutscene system
            if (player != null && player.StateMachine.State == global::Celeste.Player.StDummy)
            {
                player.StateMachine.State = global::Celeste.Player.StNormal;
            }
        }

        /// <summary>
        /// Call this when player transitions to next room to remove NPCs
        /// </summary>
        public static void RemoveNPCsOnRoomTransition(Level level)
        {
            if (!level.Session.GetFlag("ch10_badeline_chara_present"))
                return;

            // Find and remove the dummy NPCs
            foreach (var entity in level.Entities)
            {
                if (entity is BadelineDummy || entity is CharaDummy)
                {
                    entity.RemoveSelf();
                }
            }

            level.Session.SetFlag("ch10_badeline_chara_present", false);
        }
    }

    /// <summary>
    /// Simple cutscene entity for NPC conversations
    /// </summary>
    public class CS_TalkToNPC : CutsceneEntity
    {
        private global::Celeste.Player player;
        private string dialogKey;

        public CS_TalkToNPC(global::Celeste.Player player, string dialogKey) 
            : base(fadeInOnSkip: false)
        {
            this.player = player;
            this.dialogKey = dialogKey;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(TalkSequence(level)));
        }

        private IEnumerator TalkSequence(Level level)
        {
            player.StateMachine.State = global::Celeste.Player.StDummy;
            
            yield return Textbox.Say(dialogKey);
            
            player.StateMachine.State = global::Celeste.Player.StNormal;
            
            EndCutscene(level);
        }

        public override void OnEnd(Level level)
        {
            // Cleanup if needed
        }
    }
}




