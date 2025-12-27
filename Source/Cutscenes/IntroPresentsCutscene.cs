namespace DesoloZantas.Core.Cutscenes
{
    /// <summary>
    /// Intro "Presents" screen - Displays mod credits and disclaimer before gameplay.
    /// Similar to Celeste's "Matt Makes Games Presents" intro.
    /// </summary>
    public class CS_IntroPresents : CutsceneEntity
    {
        private readonly Player player;
        private bool skipRequested;
        private float alpha;
        private string currentText;
        private int textPhase;

        // Customizable text for each phase
        private static readonly string[] PresentTexts = new string[]
        {
            "MAGEDELINE",
            "PRESENTS",
            "DESOLO ZANTAS"
        };

        private static readonly string[] DisclaimerTexts = new string[]
        {
            "This mod contains flashing lights",
            "and content from various fandoms.",
            "",
            "Player discretion is advised.",
            "",
            "Press any button to continue..."
        };

        public CS_IntroPresents(Player player) : base(false, false)
        {
            this.player = player;
            this.alpha = 0f;
            this.textPhase = 0;
            this.currentText = "";
            Tag = Tags.HUD | Tags.PauseUpdate;
            Depth = -1000000; // Render on top
        }

        public override void OnBegin(Level level)
        {
            level.Session.Audio.Music.Event = SFX.EventnameByHandle("event:/music/menu/credits_intro");
            level.Session.Audio.Apply();
            
            Add(new Coroutine(IntroSequence(level)));
        }

        private IEnumerator IntroSequence(Level level)
        {
            // Freeze player during intro
            if (player != null)
            {
                player.StateMachine.State = Player.StDummy;
                player.StateMachine.Locked = true;
                player.Visible = false;
            }

            // Black screen start
            level.FormationBackdrop.Display = true;
            level.FormationBackdrop.Alpha = 1f;
            yield return 0.5f;

            // Phase 1: Creator name
            yield return ShowTextPhase(PresentTexts[0], 2.0f);

            // Phase 2: "Presents"
            yield return ShowTextPhase(PresentTexts[1], 1.5f);

            // Phase 3: Mod title
            yield return ShowTextPhase(PresentTexts[2], 2.5f);

            // Brief pause before disclaimer
            yield return 1.0f;

            // Phase 4: Disclaimer
            yield return ShowDisclaimerPhase(level);

            // Setup complete
            level.Session.SetFlag("intro_presents_complete");
            
            EndCutscene(level);
        }

        private IEnumerator ShowTextPhase(string text, float duration)
        {
            currentText = text;
            
            // Fade in
            for (float t = 0f; t < 0.5f; t += Engine.DeltaTime)
            {
                alpha = Ease.CubeOut(t / 0.5f);
                if (CheckSkip()) yield break;
                yield return null;
            }
            alpha = 1f;

            // Hold
            yield return duration;
            if (CheckSkip()) yield break;

            // Fade out
            for (float t = 0f; t < 0.5f; t += Engine.DeltaTime)
            {
                alpha = 1f - Ease.CubeIn(t / 0.5f);
                if (CheckSkip()) yield break;
                yield return null;
            }
            alpha = 0f;
            currentText = "";
        }

        private IEnumerator ShowDisclaimerPhase(Level level)
        {
            textPhase = 1; // Switch to disclaimer rendering mode
            
            // Fade in disclaimer
            for (float t = 0f; t < 0.8f; t += Engine.DeltaTime)
            {
                alpha = Ease.CubeOut(t / 0.8f);
                yield return null;
            }
            alpha = 1f;

            // Wait for input with timeout
            float timeout = 10f;
            bool inputReceived = false;
            
            while (!inputReceived && timeout > 0)
            {
                timeout -= Engine.DeltaTime;
                
                if (Input.Jump.Pressed || Input.Dash.Pressed || 
                    Input.MenuConfirm.Pressed || Input.Pause.Pressed ||
                    Input.ESC.Pressed)
                {
                    inputReceived = true;
                    Audio.Play(SFX.ui_main_button_select);
                }
                
                yield return null;
            }

            // Fade out disclaimer
            for (float t = 0f; t < 0.5f; t += Engine.DeltaTime)
            {
                alpha = 1f - Ease.CubeIn(t / 0.5f);
                yield return null;
            }
            alpha = 0f;

            // Transition to gameplay
            yield return 0.5f;
        }

        private bool CheckSkip()
        {
            if (Input.MenuConfirm.Pressed || Input.ESC.Pressed)
            {
                skipRequested = true;
                alpha = 0f;
                return true;
            }
            return false;
        }

        public override void Render()
        {
            base.Render();
            
            if (alpha <= 0f) return;

            // Draw black background
            Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black);

            if (textPhase == 0)
            {
                // Single centered text (Presents phase)
                RenderCenteredText(currentText, alpha);
            }
            else
            {
                // Disclaimer phase - multiple lines
                RenderDisclaimerText(alpha);
            }
        }

        private void RenderCenteredText(string text, float textAlpha)
        {
            if (string.IsNullOrEmpty(text)) return;

            Vector2 screenCenter = new Vector2(960f, 540f);
            float scale = text == PresentTexts[2] ? 2.5f : 1.5f; // Larger for title
            
            Color textColor = Color.White * textAlpha;
            
            ActiveFont.Draw(
                text,
                screenCenter,
                new Vector2(0.5f, 0.5f),
                Vector2.One * scale,
                textColor
            );
        }

        private void RenderDisclaimerText(float textAlpha)
        {
            Vector2 screenCenter = new Vector2(960f, 400f);
            float yOffset = 0f;
            float lineHeight = 50f;
            
            Color textColor = Color.White * textAlpha;
            Color headerColor = Color.Gold * textAlpha;

            // Header
            ActiveFont.Draw(
                "DISCLAIMER",
                new Vector2(screenCenter.X, screenCenter.Y - 60f),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 1.8f,
                headerColor
            );

            // Disclaimer lines
            foreach (string line in DisclaimerTexts)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    ActiveFont.Draw(
                        line,
                        new Vector2(screenCenter.X, screenCenter.Y + yOffset + 40f),
                        new Vector2(0.5f, 0.5f),
                        Vector2.One * 0.9f,
                        textColor
                    );
                }
                yOffset += lineHeight;
            }
        }

        public override void OnEnd(Level level)
        {
            level.FormationBackdrop.Display = false;
            
            if (player != null)
            {
                player.StateMachine.State = Player.StNormal;
                player.StateMachine.Locked = false;
                player.Visible = true;
            }
        }
    }

    /// <summary>
    /// Trigger entity that starts the intro presents cutscene
    /// </summary>
    [Tracked]
    [CustomEntity("DesoloZatnas/IntroPresentsTrigger")]
    public class IntroPresentsTrigger : Trigger
    {
        private bool triggered;

        public IntroPresentsTrigger(EntityData data, Vector2 offset) 
            : base(data, offset)
        {
            triggered = false;
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            
            if (triggered) return;
            
            Level level = Scene as Level;
            if (level == null) return;

            // Only trigger if intro hasn't been seen
            if (!level.Session.GetFlag("intro_presents_complete"))
            {
                triggered = true;
                level.Add(new CS_IntroPresents(player));
            }
        }
    }

    /// <summary>
    /// Standalone disclaimer screen that can be shown separately
    /// </summary>
    public class CS_DisclaimerScreen : CutsceneEntity
    {
        private readonly Player player;
        private float alpha;
        private bool acknowledged;

        // Configurable disclaimer content
        public string Header { get; set; } = "CONTENT WARNING";
        public string[] Lines { get; set; } = new string[]
        {
            "This mod contains:",
            "• Flashing lights and visual effects",
            "• Themes from various game franchises",
            "• Challenging platforming content",
            "• Some mature themes",
            "",
            "Press CONFIRM to acknowledge and continue"
        };

        public CS_DisclaimerScreen(Player player) : base(false, false)
        {
            this.player = player;
            this.alpha = 0f;
            this.acknowledged = false;
            Tag = Tags.HUD | Tags.PauseUpdate;
            Depth = -1000000;
        }

        public override void OnBegin(Level level)
        {
            if (player != null)
            {
                player.StateMachine.State = Player.StDummy;
                player.StateMachine.Locked = true;
            }

            Add(new Coroutine(DisclaimerSequence(level)));
        }

        private IEnumerator DisclaimerSequence(Level level)
        {
            // Fade in
            for (float t = 0f; t < 0.8f; t += Engine.DeltaTime)
            {
                alpha = Ease.CubeOut(t / 0.8f);
                yield return null;
            }
            alpha = 1f;

            // Wait for acknowledgment
            while (!acknowledged)
            {
                if (Input.MenuConfirm.Pressed || Input.Jump.Pressed)
                {
                    acknowledged = true;
                    Audio.Play(SFX.ui_main_button_select);
                }
                yield return null;
            }

            // Fade out
            for (float t = 0f; t < 0.5f; t += Engine.DeltaTime)
            {
                alpha = 1f - Ease.CubeIn(t / 0.5f);
                yield return null;
            }

            level.Session.SetFlag("disclaimer_acknowledged");
            EndCutscene(level);
        }

        public override void Render()
        {
            base.Render();
            
            if (alpha <= 0f) return;

            // Semi-transparent overlay
            Draw.Rect(-10f, -10f, 1940f, 1100f, Color.Black * alpha * 0.95f);

            Vector2 screenCenter = new Vector2(960f, 350f);
            float yOffset = 0f;
            float lineHeight = 45f;

            // Header
            ActiveFont.Draw(
                Header,
                new Vector2(screenCenter.X, screenCenter.Y - 80f),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 2.0f,
                Color.Red * alpha
            );

            // Content lines
            foreach (string line in Lines)
            {
                Color lineColor = Color.White * alpha;
                
                // Highlight bullet points differently
                if (line.StartsWith("•"))
                {
                    lineColor = Color.LightGray * alpha;
                }
                else if (line.Contains("Press"))
                {
                    lineColor = Color.Gold * alpha;
                }

                ActiveFont.Draw(
                    line,
                    new Vector2(screenCenter.X, screenCenter.Y + yOffset + 40f),
                    new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.85f,
                    lineColor
                );
                
                yOffset += lineHeight;
            }
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
    /// Trigger for showing the disclaimer screen
    /// </summary>
    [Tracked]
    [CustomEntity("DesoloZatnas/DisclaimerTrigger")]
    public class DisclaimerTrigger : Trigger
    {
        private bool triggered;
        private readonly string[] customLines;
        private readonly string customHeader;

        public DisclaimerTrigger(EntityData data, Vector2 offset) 
            : base(data, offset)
        {
            triggered = false;
            customHeader = data.Attr("header", "CONTENT WARNING");
            
            // Parse custom lines from data if provided
            string linesData = data.Attr("lines", "");
            if (!string.IsNullOrEmpty(linesData))
            {
                customLines = linesData.Split('|');
            }
            else
            {
                customLines = null;
            }
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            
            if (triggered) return;
            
            Level level = Scene as Level;
            if (level == null) return;

            if (!level.Session.GetFlag("disclaimer_acknowledged"))
            {
                triggered = true;
                var disclaimer = new CS_DisclaimerScreen(player);
                disclaimer.Header = customHeader;
                
                if (customLines != null)
                {
                    disclaimer.Lines = customLines;
                }
                
                level.Add(disclaimer);
            }
        }
    }
}
