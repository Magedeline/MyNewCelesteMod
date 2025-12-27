namespace DesoloZantas.Core.Cutscenes
{
    /// <summary>
    /// Chapter 11 (Snow/Frozen Sanctuary) - Intro Cutscene
    /// </summary>
    public class CS_Chapter11_Intro : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_Intro(Player player) : base(false, true)
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

            // Play intro dialog
            yield return Textbox.Say("CH11_INTRO");

            level.Session.SetFlag("ch11_intro_complete");

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
    /// Chapter 11 - Marlet Introduction
    /// </summary>
    public class CS_Chapter11_IntroMarlet : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_IntroMarlet(Player player) : base(false, true)
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

            // Play Marlet intro dialog
            yield return Textbox.Say("CH11_INTRO_MARLET");

            level.Session.SetFlag("ch11_marlet_intro_complete");

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
    /// Chapter 11 - Wrong Volume Water Puzzle
    /// </summary>
    public class CS_Chapter11_WrongVolumeWater : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_WrongVolumeWater(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_WRONG_VOLUME_WATER");

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
    /// Chapter 11 - Gateway Lifted
    /// </summary>
    public class CS_Chapter11_GatewayLifted : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_GatewayLifted(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_GATEWAY_LIFTED");

            level.Session.SetFlag("ch11_gateway_lifted");

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
    /// Chapter 11 - Starlo and Marlet Meet
    /// </summary>
    public class CS_Chapter11_StarloMarlet : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_StarloMarlet(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_STARLO_AND_MARLET");

            level.Session.SetFlag("ch11_starlo_marlet_met");

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
    /// Chapter 11 - Bar Arrival
    /// </summary>
    public class CS_Chapter11_BarArrival : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_BarArrival(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_BAR_ARRIVIAL");

            level.Session.SetFlag("ch11_bar_arrived");

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
    /// Chapter 11 - Cowboy Bar Introduction
    /// </summary>
    public class CS_Chapter11_CowboyBarIntro : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_CowboyBarIntro(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_COWBOY_BAR_INTRO");

            level.Session.SetFlag("ch11_cowboy_intro_complete");

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
    /// Chapter 11 - Gun Practice Results
    /// </summary>
    public class CS_Chapter11_GunPractice : CutsceneEntity
    {
        private readonly Player player;
        private readonly string resultType; // "try_again_a", "try_again_b", "passed", "truly_passed"

        public CS_Chapter11_GunPractice(Player player, string result) : base(false, true)
        {
            this.player = player;
            this.resultType = result;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;

            string dialogKey = resultType switch
            {
                "try_again_a" => "CH11_COWBOY_GUN_PRACTICES_TRY_AGAIN_A",
                "try_again_b" => "CH11_COWBOY_GUN_PRACTICES_TRY_AGAIN_B",
                "passed" => "CH11_COWBOY_GUN_PRACTICES_PASSED",
                "truly_passed" => "CH11_COWBOY_GUN_PRACTICES_TRUELY_PASSED",
                _ => "CH11_COWBOY_GUN_PRACTICES_TRY_AGAIN_A"
            };

            yield return Textbox.Say(dialogKey);

            if (resultType == "passed" || resultType == "truly_passed")
            {
                level.Session.SetFlag("ch11_gun_practice_complete");
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

    /// <summary>
    /// Chapter 11 - Intro to Stages
    /// </summary>
    public class CS_Chapter11_IntroStages : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_IntroStages(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_INTRO_STAGES");

            level.Session.SetFlag("ch11_stages_intro_complete");

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
    /// Chapter 11 - Maggy/Magolor Encounter
    /// </summary>
    public class CS_Chapter11_Maggy : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_Maggy(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_MAGGY");

            level.Session.SetFlag("ch11_maggy_met");

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
    /// Chapter 11 - Cinematic Bar Scene (Mt. Celeste Eruption)
    /// </summary>
    public class CS_Chapter11_CinematicBar : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_CinematicBar(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_CINEMATIC_BAR");

            level.Session.SetFlag("ch11_cinematic_complete");

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
    /// Chapter 11 - Maggy End Scene
    /// </summary>
    public class CS_Chapter11_MaggyEnd : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_MaggyEnd(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_MAGGY_END");

            level.Session.SetFlag("ch11_maggy_end_complete");

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
    /// Chapter 11 - Mini Heart Collection Check
    /// </summary>
    public class CS_Chapter11_MiniHeartCheck : CutsceneEntity
    {
        private readonly Player player;
        private readonly bool hasEnough;

        public CS_Chapter11_MiniHeartCheck(Player player, bool enough) : base(false, true)
        {
            this.player = player;
            this.hasEnough = enough;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;

            string dialogKey = hasEnough ? "CH11_COLLECTING_MINIHEART_ENOUGH" : "CH11_COLLECTING_MINIHEART_NOT_ENOUGH";
            yield return Textbox.Say(dialogKey);

            if (hasEnough)
            {
                level.Session.SetFlag("ch11_minihearts_collected");
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

    /// <summary>
    /// Chapter 11 - Boss Introduction (Possessed Marlet)
    /// </summary>
    public class CS_Chapter11_BossIntro : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_BossIntro(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_BOSS_INTRO");

            level.Session.SetFlag("ch11_boss_intro_seen");

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
    /// Chapter 11 - Boss Warning (Don't Hurt Her)
    /// </summary>
    public class CS_Chapter11_BossWarning : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_BossWarning(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_BOSS_INTRO_DO_NOT_HURT_HER");

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
    /// Chapter 11 - Boss Mid-Fight (Marlet Breaking Free)
    /// </summary>
    public class CS_Chapter11_BossMid : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_BossMid(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_BOSS_MID");

            level.Session.SetFlag("ch11_boss_mid_seen");

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
    /// Chapter 11 - Boss Outro (Victory and Dark Matter Escape)
    /// </summary>
    public class CS_Chapter11_BossOutro : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter11_BossOutro(Player player) : base(false, true)
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

            yield return Textbox.Say("CH11_BOSS_OUTRO");

            level.Session.SetFlag("ch11_boss_complete");
            level.Session.SetFlag("ch11_complete");

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




