namespace DesoloZantas.Core.Cutscenes
{
    /// <summary>
    /// Chapter 10 (Ruins) - Intro Cutscene
    /// Flowey's introduction to Madeline with different variants
    /// </summary>
    public class CS_Chapter10_Intro : CutsceneEntity
    {
        private readonly Player player;
        private readonly bool isReturning;
        private readonly bool isAssistMode;

        public CS_Chapter10_Intro(Player player, bool returning = false, bool assist = false) : base(false, true)
        {
            this.player = player;
            this.isReturning = returning;
            this.isAssistMode = assist;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;

            // Determine which dialog to use based on conditions
            string dialogKey;
            if (isAssistMode)
            {
                dialogKey = "CH10_FLOWEY_INTRO_ASSIST";
            }
            else if (isReturning)
            {
                dialogKey = "CH10_FLOWEY_INTRO_RETURNING";
            }
            else
            {
                dialogKey = "CH10_FLOWEY_INTRO";
            }

            // Play the appropriate dialog
            yield return Textbox.Say(dialogKey);

            // Set completion flag
            level.Session.SetFlag("ch10_intro_complete");

            EndCutscene(level);
        }

        public override void OnEnd(Level level)
        {
            if (player != null)
            {
                player.StateMachine.State = Player.StNormal;
                player.StateMachine.Locked = false;
            }
        }
    }

    /// <summary>
    /// Chapter 10 - Asriel Introduction
    /// </summary>
    public class CS_Chapter10_AsrielIntro : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter10_AsrielIntro(Player player) : base(false, true)
        {
            this.player = player;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;

            // Play Asriel intro dialog
            yield return Textbox.Say("CH10_RUINS_INTRO");

            level.Session.SetFlag("ch10_asriel_intro_complete");

            EndCutscene(level);
        }

        public override void OnEnd(Level level)
        {
            if (player != null)
            {
                player.StateMachine.State = Player.StNormal;
                player.StateMachine.Locked = false;
            }
        }
    }

    /// <summary>
    /// Chapter 10 - Madeline, Badeline, and Chara conversation
    /// </summary>
    public class CS_Chapter10_MaddyBaddyChara : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter10_MaddyBaddyChara(Player player) : base(false, true)
        {
            this.player = player;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;

            // Play dialog
            yield return Textbox.Say("CH10_MADDY_AND_BADDY_AND_CHARA");

            level.Session.SetFlag("ch10_trio_talk_complete");

            EndCutscene(level);
        }

        public override void OnEnd(Level level)
        {
            if (player != null)
            {
                player.StateMachine.State = Player.StNormal;
                player.StateMachine.Locked = false;
            }
        }
    }

    /// <summary>
    /// Chapter 10 - Talking to Badeline and Chara (multiple variants)
    /// </summary>
    public class CS_Chapter10_TalkToBaddyChara : CutsceneEntity
    {
        private readonly Player player;
        private readonly int variant; // 0-3 for different dialog options

        public CS_Chapter10_TalkToBaddyChara(Player player, int variant = 0) : base(false, true)
        {
            this.player = player;
            this.variant = Calc.Clamp(variant, 0, 3);
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;

            // Play the appropriate variant dialog
            string dialogKey = $"CH10_TALK_TO_BADDY_AND_CHARA{variant}";
            yield return Textbox.Say(dialogKey);

            level.Session.SetFlag($"ch10_talk_variant_{variant}_complete");

            EndCutscene(level);
        }

        public override void OnEnd(Level level)
        {
            if (player != null)
            {
                player.StateMachine.State = Player.StNormal;
                player.StateMachine.Locked = false;
            }
        }
    }

    /// <summary>
    /// Chapter 10 - Flowey Easter Eggs (Chapter-specific memories)
    /// Triggered when returning from higher chapters
    /// </summary>
    public class CS_Chapter10_FloweyEasterEgg : CutsceneEntity
    {
        private readonly Player player;
        private readonly int chapterMemory; // 1-7 for chapters

        public CS_Chapter10_FloweyEasterEgg(Player player, int chapter) : base(false, true)
        {
            this.player = player;
            this.chapterMemory = chapter;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;

            // Randomly trigger easter egg (30% chance)
            if (Calc.Random.NextFloat() < 0.3f)
            {
                string dialogKey = chapterMemory switch
                {
                    1 => "CH10_FLOWEY_EASTER_CH1_FORSAKEN_CITY",
                    2 => "CH10_FLOWEY_EASTER_CH2_OLD_SITE",
                    3 => "CH10_FLOWEY_EASTER_CH3_CELESTIAL_RESORT",
                    4 => "CH10_FLOWEY_EASTER_CH4_GOLDEN_RIDGE",
                    5 => "CH10_FLOWEY_EASTER_CH5_MIRROR_TEMPLE",
                    6 => "CH10_FLOWEY_EASTER_CH6_REFLECTION",
                    7 => "CH10_FLOWEY_EASTER_CH7_SUMMIT",
                    _ => null
                };

                if (dialogKey != null)
                {
                    yield return Textbox.Say(dialogKey);
                    level.Session.SetFlag($"ch10_flowey_easter_{chapterMemory}_seen");
                }
            }

            EndCutscene(level);
        }

        public override void OnEnd(Level level)
        {
            if (player != null)
            {
                player.StateMachine.State = Player.StNormal;
                player.StateMachine.Locked = false;
            }
        }
    }
}




