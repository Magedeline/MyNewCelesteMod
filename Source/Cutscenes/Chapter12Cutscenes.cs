namespace DesoloZantas.Core.Cutscenes
{
    /// <summary>
    /// Chapter 12 (Water/Cascading Depths) - Intro Cutscene
    /// </summary>
    public class CS_Chapter12_Intro : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter12_Intro(Player player) : base(false, true)
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
            yield return Textbox.Say("CH12_INTRO");

            level.Session.SetFlag("ch12_intro_complete");

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
    /// Chapter 12 - Darkness Meetup (Dark Matter's Fate)
    /// </summary>
    public class CS_Chapter12_DarknessMeetup : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter12_DarknessMeetup(Player player) : base(false, true)
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

            yield return Textbox.Say("CH12_DARKNESS_MEETUP");

            level.Session.SetFlag("ch12_darkness_meetup_complete");

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
    /// Chapter 12 - Darkness Meetup End
    /// </summary>
    public class CS_Chapter12_DarknessMeetupEnd : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter12_DarknessMeetupEnd(Player player) : base(false, true)
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

            yield return Textbox.Say("CH12_DARKNESS_MEETUP_END");

            level.Session.SetFlag("ch12_darkness_end_complete");

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
    /// Chapter 12 - Pre-Intro to Starsi
    /// </summary>
    public class CS_Chapter12_PreIntroStarsi : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter12_PreIntroStarsi(Player player) : base(false, true)
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

            yield return Textbox.Say("CH12_PRE_INTRO_STARSI");

            level.Session.SetFlag("ch12_pre_intro_starsi_complete");

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
    /// Chapter 12 - Starsi Introduction
    /// </summary>
    public class CS_Chapter12_IntroStarsi : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter12_IntroStarsi(Player player) : base(false, true)
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

            yield return Textbox.Say("CH12_INTRO_STARSI");

            level.Session.SetFlag("ch12_intro_starsi_complete");
            level.Session.SetFlag("ch12_starsi_joined_party");

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
    /// Chapter 12 - Untold Legend Segments (A-Z)
    /// Starsi tells the story of previous chapters
    /// </summary>
    public class CS_Chapter12_UntoldLegend : CutsceneEntity
    {
        private readonly Player player;
        private readonly string segment; // A through Z

        public CS_Chapter12_UntoldLegend(Player player, string segment) : base(false, true)
        {
            this.player = player;
            this.segment = segment.ToUpper();
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;

            // Valid segments: A-P, Q, X, Y, Z, END
            string dialogKey = $"CH12_UNTOLD_LEGEND_{segment}";
            yield return Textbox.Say(dialogKey);

            level.Session.SetFlag($"ch12_legend_{segment.ToLower()}_told");

            // If this is the END segment, mark the full legend as complete
            if (segment == "END")
            {
                level.Session.SetFlag("ch12_full_legend_complete");
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
    /// Chapter 12 - Mini Heart Collection Check
    /// </summary>
    public class CS_Chapter12_MiniHeartCheck : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter12_MiniHeartCheck(Player player) : base(false, true)
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

            yield return Textbox.Say("CH12_COLLECTING_MINIHEART_NOT_ENOUGH");

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
    /// Chapter 12 - Temmie Room Encounter
    /// </summary>
    public class CS_Chapter12_TemmieRoom : CutsceneEntity
    {
        private readonly Player player;
        private readonly string variant; // "", "A", "B", or "BOB"

        public CS_Chapter12_TemmieRoom(Player player, string variant = "") : base(false, true)
        {
            this.player = player;
            this.variant = variant;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;

            string dialogKey = string.IsNullOrEmpty(variant) ? "CH12_TEMMIE_ROOM" : $"CH12_TEMMIE_ROOM_{variant}";
            yield return Textbox.Say(dialogKey);

            level.Session.SetFlag($"ch12_temmie_room_{variant.ToLower()}_seen");

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
    /// Chapter 12 - Complete Untold Legend Sequence
    /// Plays all legend segments in order
    /// </summary>
    public class CS_Chapter12_CompleteUntoldLegend : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter12_CompleteUntoldLegend(Player player) : base(false, true)
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

            // Play all legend segments in order
            string[] segments = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P" };
            
            foreach (string segment in segments)
            {
                yield return Textbox.Say($"CH12_UNTOLD_LEGEND_{segment}");
                level.Session.SetFlag($"ch12_legend_{segment.ToLower()}_told");
                yield return 0.3f; // Small pause between segments
            }

            // Play the ending
            yield return Textbox.Say("CH12_UNTOLD_LEGEND_END");
            
            level.Session.SetFlag("ch12_full_legend_complete");

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
    /// Chapter 12 - Future Chapters Preview (Q, X, Y, Z)
    /// Special legend segments about future chapters
    /// </summary>
    public class CS_Chapter12_FutureChaptersPreview : CutsceneEntity
    {
        private readonly Player player;

        public CS_Chapter12_FutureChaptersPreview(Player player) : base(false, true)
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

            // Only play if main legend is complete
            if (level.Session.GetFlag("ch12_full_legend_complete"))
            {
                // Chapter 16 preview
                yield return Textbox.Say("CH12_UNTOLD_LEGEND_Q");
                yield return 0.3f;

                // Chapter 18 preview
                yield return Textbox.Say("CH12_UNTOLD_LEGEND_X");
                yield return 0.3f;

                // Chapter 19 preview
                yield return Textbox.Say("CH12_UNTOLD_LEGEND_Y");
                yield return 0.3f;

                // Chapter 20 preview
                yield return Textbox.Say("CH12_UNTOLD_LEGEND_Z");

                level.Session.SetFlag("ch12_future_chapters_previewed");
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
    /// Chapter 12 - Vingteenairee's Dramatic Explanation
    /// Cinematic cutscene revealing the truth about the child's soul and determination
    /// </summary>
    public class CS_Chapter12_VingteenaireeExplanation : CutsceneEntity
    {
        private readonly Player player;
        private float screenDarken = 0f;
        private bool isFlashing = false;
        private float flashAlpha = 0f;
        private Color flashColor = Color.White;

        public CS_Chapter12_VingteenaireeExplanation(Player player) : base(false, true)
        {
            this.player = player;
            Tag = Tags.HUD; // Render above most entities
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(CutsceneSequence(level)));
        }

        private IEnumerator CutsceneSequence(Level level)
        {
            // Lock player movement
            player.StateMachine.State = Player.StDummy;
            player.StateMachine.Locked = true;
            player.ForceCameraUpdate = true;

            // Store original lighting
            float originalBloom = level.Bloom.Base;
            
            // Dramatic opening - screen shake and darken
            yield return 0.5f;
            level.Shake(0.3f);
            Audio.Play("event:/game/general/thing_booped");
            
            // Darken the screen gradually
            Add(new Coroutine(DarkenScreen(0.4f, 1.5f)));
            yield return 1.5f;

            // First segment - The soul revelation
            yield return 0.3f;
            Add(new Coroutine(FlashEffect(Color.Cyan, 0.3f, 0.5f)));
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART1");
            
            // Dramatic pause with pulse effect
            level.Flash(Color.White * 0.2f, true);
            yield return 0.8f;

            // Second segment - The child's power
            Add(new Coroutine(FlashEffect(Color.Red, 0.4f, 0.7f)));
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART2");
            level.Shake(0.2f);
            yield return 0.5f;

            // Third segment - The choice
            Add(new Coroutine(FlashEffect(Color.Yellow, 0.3f, 0.6f)));
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART3");
            yield return 0.4f;

            // Fourth segment - The tragedy
            level.Shake(0.5f);
            Add(new Coroutine(FlashEffect(Color.Purple, 0.5f, 1.0f)));
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART4");
            
            // Heavy dramatic pause
            level.Flash(Color.Black * 0.5f, true);
            yield return 1.0f;

            // Fifth segment - The army
            Add(new Coroutine(FlashEffect(Color.DarkRed, 0.4f, 0.8f)));
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART5");
            level.Shake(0.3f);
            yield return 0.5f;

            // Sixth segment - The suited man's power
            Add(new Coroutine(FlashEffect(Color.Gray, 0.6f, 1.2f)));
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART6");
            yield return 0.4f;

            // Seventh segment - No choices
            level.Shake(0.6f);
            Add(new Coroutine(FlashEffect(Color.DarkGray, 0.5f, 1.0f)));
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART7");
            yield return 0.6f;

            // Eighth segment - Kill or be killed
            Add(new Coroutine(FlashEffect(Color.Crimson, 0.7f, 1.5f)));
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART8");
            level.Shake(0.7f);
            yield return 0.8f;

            // Ninth segment - Eternal suffering
            level.Flash(Color.Black * 0.7f, true);
            yield return 0.5f;
            Add(new Coroutine(FlashEffect(Color.DarkBlue, 0.4f, 1.0f)));
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART9");
            yield return 1.0f;

            // Tenth segment - The loss
            Add(new Coroutine(FlashEffect(Color.Navy, 0.3f, 0.8f)));
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART10");
            yield return 0.6f;

            // Final segment - The blame
            level.Shake(0.4f);
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_PART11");
            
            // Climactic finish
            level.Flash(Color.White * 0.5f, true);
            yield return 0.5f;

            // Restore lighting gradually
            Add(new Coroutine(DarkenScreen(0f, 2.0f)));
            yield return 2.0f;

            // Play the ending dialog
            yield return 0.5f;
            yield return Textbox.Say("CH12_VINGTEENAIREE_EXPLANATION_END");

            // Set completion flag
            level.Session.SetFlag("ch12_vingteenairee_explanation_complete");

            // Restore bloom
            level.Bloom.Base = originalBloom;

            EndCutscene(level);
        }

        private IEnumerator DarkenScreen(float targetDarkness, float duration)
        {
            float startDarkness = screenDarken;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Engine.DeltaTime;
                screenDarken = Calc.LerpClamp(startDarkness, targetDarkness, elapsed / duration);
                yield return null;
            }

            screenDarken = targetDarkness;
        }

        private IEnumerator FlashEffect(Color color, float intensity, float duration)
        {
            isFlashing = true;
            flashColor = color;
            flashAlpha = 0f;

            // Fade in
            float fadeInDuration = duration * 0.2f;
            float elapsed = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed += Engine.DeltaTime;
                flashAlpha = Calc.LerpClamp(0f, intensity, elapsed / fadeInDuration);
                yield return null;
            }

            // Hold
            flashAlpha = intensity;
            yield return duration * 0.6f;

            // Fade out
            float fadeOutDuration = duration * 0.2f;
            elapsed = 0f;

            while (elapsed < fadeOutDuration)
            {
                elapsed += Engine.DeltaTime;
                flashAlpha = Calc.LerpClamp(intensity, 0f, elapsed / fadeOutDuration);
                yield return null;
            }

            flashAlpha = 0f;
            isFlashing = false;
        }

        public override void Render()
        {
            base.Render();

            if (Scene is Level level)
            {
                // Render screen darkening overlay
                if (screenDarken > 0f)
                {
                    Draw.Rect(level.Camera.X, level.Camera.Y, 320, 180, Color.Black * screenDarken);
                }

                // Render flash effect
                if (isFlashing && flashAlpha > 0f)
                {
                    Draw.Rect(level.Camera.X, level.Camera.Y, 320, 180, flashColor * flashAlpha);
                }
            }
        }

        public override void OnEnd(Level level)
        {
            if (player != null)
            {
                player.StateMachine.State = Player.StNormal;
                player.StateMachine.Locked = false;
                player.ForceCameraUpdate = false;
            }

            // Ensure visual effects are cleared
            screenDarken = 0f;
            isFlashing = false;
            flashAlpha = 0f;
        }
    }
}




