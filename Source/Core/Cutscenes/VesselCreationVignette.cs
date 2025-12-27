#nullable enable

namespace DesoloZantas.Core.Core.Cutscenes
{
    /// <summary>
    /// Vessel Creation Vignette - Interactive vessel creation cutscene 
    /// Based on Hollow Knight/Undertale inspired vessel creation sequence
    /// </summary>
    public class VesselCreationVignette : Scene
    {
        #region Constants
        private const float FADE_DURATION = 2f;
        private const float TEXT_FADE_SPEED = 3f;
        private const float CHOICE_EASE_SPEED = 4f;
        private const float VESSEL_DISPLAY_TIME = 3f;
        private const float WHITE_FADE_DURATION = 4f;
        
        // Audio events
        private const string CREATION_MUSIC_EVENT = "event:/Ingeste/music/vessel_creation";
        private const string CHOICE_APPEAR_EVENT = "event:/ui/game/chatoptions_appear";
        private const string CHOICE_SELECT_EVENT = "event:/ui/game/chatoptions_select";
        private const string CHOICE_MOVE_EVENT = "event:/ui/game/chatoptions_roll_down";
        private const string VESSEL_DISCARD_EVENT = "event:/Ingeste/sfx/vessel_discard";
        #endregion

        #region Vessel Creation Data
        private readonly string[] vesselLegOptions = { "Thin Legs", "Sturdy Legs", "Elegant Legs", "No Legs" };
        private readonly string[] vesselTorsoOptions = { "Slim Torso", "Broad Torso", "Armored Torso", "Ethereal Torso" };
        private readonly string[] vesselHeadOptions = { "Sharp Head", "Round Head", "Crowned Head", "Void Head" };
        
        private string selectedLeg = "";
        private string selectedTorso = "";
        private string selectedHead = "";
        private string vesselName = "";
        private string playerFeeling = "";
        private string creatorName = "";
        private bool isHonestAnswer = false;
        #endregion

        #region Fields
        private Session session;
        private string? areaMusic;
        private float backgroundFade = 1f;
        private float textAlpha = 0f;
        private float choiceEase = 0f;
        private bool exiting = false;
        
        // UI Components
        private TextMenu? currentMenu;
        private HudRenderer hud;
        private Coroutine? sequenceCoroutine;
        
        // Current state
        private CreationPhase currentPhase = CreationPhase.Introduction;
        private int currentChoiceIndex = 0;
        private List<string> currentChoices = new List<string>();
        
        // Audio handle not required; we use Celeste Audio static routing
        private bool creationMusicActive;
        
        public bool CanPause => currentMenu == null;
        #endregion

        #region Enums
        private enum CreationPhase
        {
            Introduction,
            LegSelection,
            TorsoSelection, 
            HeadSelection,
            VesselNaming,
            HonestQuestion,
            FeelingsQuestion,
            CreatorNaming,
            VesselDisplay,
            VesselDiscard,
            Transition
        }
        #endregion

        #region Constructor
        public VesselCreationVignette(Session session) : base()
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            
            // Store and clear current music
            areaMusic = session.Audio.Music.Event;
            session.Audio.Music.Event = null;
            session.Audio.Apply(forceSixteenthNoteHack: false);
            
            // Initialize UI
            Add(hud = new HudRenderer());
            RendererList.UpdateLists();
            
            // Start creation sequence
            sequenceCoroutine = new Coroutine(vesselCreationSequence());
            
            // Add fade-in effect
            Add(new FadeWipe(this, true));
        }
        #endregion

        #region Main Sequence
        private IEnumerator vesselCreationSequence()
        {
            // Phase 1: Introduction
            yield return introductionPhase();
            
            // Phase 2: Vessel Creation (Legs, Torso, Head)
            yield return legSelectionPhase();
            yield return torsoSelectionPhase();
            yield return headSelectionPhase();
            
            // Phase 3: Vessel Naming
            yield return vesselNamingPhase();
            
            // Phase 4: Truth Question
            yield return honestQuestionPhase();
            
            // Phase 5: Feelings Question
            yield return feelingsQuestionPhase();
            
            // Phase 6: Creator Naming
            yield return creatorNamingPhase();
            
            // Phase 7: Display Created Vessel
            yield return vesselDisplayPhase();
            
            // Phase 8: Vessel Discard
            yield return vesselDiscardPhase();
            
            // Phase 9: Transition to IntroVignette
            yield return transitionToIntroVignette();
        }
        #endregion

        #region Creation Phases
        private IEnumerator introductionPhase()
        {
            currentPhase = CreationPhase.Introduction;
            
            // Start creation music
            try
            {
                Audio.SetMusic(CREATION_MUSIC_EVENT);
                creationMusicActive = true;
            }
            catch
            {
                // Fallback: ignore if FMOD route unavailable in this context
                creationMusicActive = false;
            }
            
            yield return 1f;
            
            // Display introduction text
            yield return showText("VESSEL_CREATION_INTRO");
            
            yield return 2f;
        }

        private IEnumerator legSelectionPhase()
        {
            currentPhase = CreationPhase.LegSelection;
            
            yield return showText("VESSEL_CREATION_LEG_CHOICE");
            yield return showChoiceMenu(vesselLegOptions, (choice) => selectedLeg = choice);
            
            yield return 1f;
        }

        private IEnumerator torsoSelectionPhase()
        {
            currentPhase = CreationPhase.TorsoSelection;
            
            yield return showText("VESSEL_CREATION_TORSO_CHOICE");
            yield return showChoiceMenu(vesselTorsoOptions, (choice) => selectedTorso = choice);
            
            yield return 1f;
        }

        private IEnumerator headSelectionPhase()
        {
            currentPhase = CreationPhase.HeadSelection;
            
            yield return showText("VESSEL_CREATION_HEAD_CHOICE");
            yield return showChoiceMenu(vesselHeadOptions, (choice) => selectedHead = choice);
            
            yield return 1f;
        }

        private IEnumerator vesselNamingPhase()
        {
            currentPhase = CreationPhase.VesselNaming;
            
            yield return showText("VESSEL_CREATION_NAME_PROMPT");
            yield return showTextInput("Enter your vessel's name:", (name) => vesselName = name);
            
            yield return 1f;
        }

        private IEnumerator honestQuestionPhase()
        {
            currentPhase = CreationPhase.HonestQuestion;
            
            yield return showText("VESSEL_CREATION_HONEST_QUESTION");
            
            string[] honestChoices = { "Yes, I was honest", "No, I wasn't completely honest" };
            yield return showChoiceMenu(honestChoices, (choice) => 
            {
                isHonestAnswer = choice.StartsWith("Yes");
            });
            
            // Show response based on honesty
            string responseKey = isHonestAnswer ? "VESSEL_CREATION_HONEST_RESPONSE" : "VESSEL_CREATION_DISHONEST_RESPONSE";
            yield return showText(responseKey);
            
            yield return 2f;
        }

        private IEnumerator feelingsQuestionPhase()
        {
            currentPhase = CreationPhase.FeelingsQuestion;
            
            yield return showText("VESSEL_CREATION_FEELINGS_QUESTION");
            yield return showTextInput("How do you feel about this game's nature?", (feeling) => playerFeeling = feeling);
            
            yield return 1f;
        }

        private IEnumerator creatorNamingPhase()
        {
            currentPhase = CreationPhase.CreatorNaming;
            
            yield return showText("VESSEL_CREATION_CREATOR_PROMPT");
            yield return showTextInput("Name yourself as the creator:", (name) => creatorName = name);
            
            yield return 1f;
        }

        private IEnumerator vesselDisplayPhase()
        {
            currentPhase = CreationPhase.VesselDisplay;
            
            // Display the created vessel
            yield return showText("VESSEL_CREATION_DISPLAY");
            
            // Show vessel details
            string vesselDetails = $"Vessel Name: {vesselName}\n" +
                                   $"Legs: {selectedLeg}\n" +
                                   $"Torso: {selectedTorso}\n" +
                                   $"Head: {selectedHead}\n" +
                                   $"Creator: {creatorName}";
            
            yield return showCustomText(vesselDetails);
            yield return VESSEL_DISPLAY_TIME;
        }

        private IEnumerator vesselDiscardPhase()
        {
            currentPhase = CreationPhase.VesselDiscard;
            
            // Show discard message
            yield return showText("VESSEL_CREATION_DISCARD");
            
            // Play discard sound effect
            Audio.Play(VESSEL_DISCARD_EVENT);
            
            yield return 2f;
            
            // Show choice for fate
            string[] fateChoices = { "Accept the vessel's fate", "Try to save the vessel", "Question the process" };
            string selectedFate = "";
            yield return showChoiceMenu(fateChoices, (choice) => selectedFate = choice);
            
            // Show response based on choice
            string fateResponseKey = selectedFate switch
            {
                var s when s.Contains("Accept") => "VESSEL_CREATION_FATE_ACCEPT",
                var s when s.Contains("save") => "VESSEL_CREATION_FATE_SAVE",
                var s when s.Contains("Question") => "VESSEL_CREATION_FATE_QUESTION",
                _ => "VESSEL_CREATION_FATE_DEFAULT"
            };
            
            yield return showText(fateResponseKey);
            yield return 2f;
        }

        private IEnumerator transitionToIntroVignette()
        {
            currentPhase = CreationPhase.Transition;
            
            yield return showText("VESSEL_CREATION_TRANSITION");
            yield return 1f;
            
            // Stop creation music and restore area music
            try
            {
                if (creationMusicActive)
                {
                    Audio.SetMusic(null);
                    creationMusicActive = false;
                }
            }
            catch { }
            
            // Fade to white
            FadeWipe whiteFade = new FadeWipe(this, false, () =>
            {
                // Restore original music
                try
                {
                    if (!string.IsNullOrEmpty(areaMusic))
                    {
                        Audio.SetMusic(areaMusic);
                    }
                }
                catch { session.Audio.Music.Event = areaMusic; }
                
                // Transition to IntroVignette
                Engine.Scene = new Cs10IntroVignetteAlt(session);
            });
            
            whiteFade.Duration = WHITE_FADE_DURATION;
            whiteFade.OnUpdate = (f) =>
            {
                textAlpha = Math.Min(textAlpha, 1f - f);
                backgroundFade = 1f - f; // Fade to white instead of black
            };
            
            exiting = true;
            yield return null;
        }
        #endregion

        #region UI Helper Methods
        private IEnumerator showText(string dialogKey)
        {
            var textbox = new Textbox(dialogKey);
            return showTextbox(textbox);
        }

        private IEnumerator showCustomText(string text)
        {
            var textbox = new Textbox("temp", Dialog.Languages["english"]);
            // This would need custom implementation to show arbitrary text
            // For now, we'll use a placeholder approach
            return showTextbox(textbox);
        }

        private IEnumerator showTextbox(Textbox textbox)
        {
            Engine.Scene.Add(textbox);
            while (textbox.Opened)
            {
                yield return true;
            }
        }

        private IEnumerator showChoiceMenu(string[] choices, Action<string> onSelect)
        {
            currentChoices = new List<string>(choices);
            currentChoiceIndex = 0;
            
            // Create choice menu
            Audio.Play(CHOICE_APPEAR_EVENT);
            
            // Animate choices appearing
            while ((choiceEase += Engine.DeltaTime * CHOICE_EASE_SPEED) < 1.0)
                yield return null;
            
            choiceEase = 1f;
            yield return 0.25f;
            
            // Handle input
            while (!Input.MenuConfirm.Pressed)
            {
                if (Input.MenuUp.Pressed && currentChoiceIndex > 0)
                {
                    Audio.Play(CHOICE_MOVE_EVENT);
                    currentChoiceIndex--;
                }
                else if (Input.MenuDown.Pressed && currentChoiceIndex < currentChoices.Count - 1)
                {
                    Audio.Play(CHOICE_MOVE_EVENT);
                    currentChoiceIndex++;
                }
                yield return null;
            }
            
            // Selection made
            Audio.Play(CHOICE_SELECT_EVENT);
            
            // Animate choices disappearing
            while ((choiceEase -= Engine.DeltaTime * CHOICE_EASE_SPEED) > 0.0)
                yield return null;
            
            string selectedChoice = currentChoices[currentChoiceIndex];
            currentChoices.Clear();
            onSelect(selectedChoice);
        }

        private IEnumerator showTextInput(string prompt, Action<string> onComplete)
        {
            // This is a simplified text input system
            // In a real implementation, this would need a proper text input dialog
            yield return showCustomText(prompt);
            
            // For now, we'll simulate input with a basic string input system
            // This could be enhanced with a proper text input UI
            string input = "DefaultInput"; // Placeholder - could be enhanced with actual input
            onComplete(input);
            
            yield return 1f;
        }
        #endregion

        #region Scene Overrides
        public override void Update()
        {
            if (currentMenu == null && !exiting)
            {
                base.Update();
                
                // Update the sequence coroutine directly
                if (sequenceCoroutine != null && !sequenceCoroutine.Finished)
                {
                    sequenceCoroutine.Update();
                }
                
                if (Input.Pause.Pressed || Input.ESC.Pressed)
                {
                    // Handle pause if needed
                }
            }
            else if (currentMenu != null && !exiting)
            {
                currentMenu.Update();
            }
        }

        public override void Render()
        {
            base.Render();
            
            // Render background
            if (backgroundFade > 0f)
            {
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, RasterizerState.CullNone, null, Engine.ScreenMatrix);
                
                // Render background (black for most phases, white for transition)
                Color bgColor = currentPhase == CreationPhase.Transition ? Color.White : Color.Black;
                Draw.Rect(-1f, -1f, 1922f, 1082f, bgColor * backgroundFade);
                
                Draw.SpriteBatch.End();
            }
            
            // Render choice menu if active
            if (currentChoices.Count > 0 && choiceEase > 0f)
            {
                renderChoiceMenu();
            }
        }

        private void renderChoiceMenu()
        {
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, RasterizerState.CullNone, null, Engine.ScreenMatrix);
            
            Vector2 basePosition = new Vector2(Engine.Width / 2f, Engine.Height / 2f);
            float optionHeight = 60f;
            float totalHeight = currentChoices.Count * optionHeight;
            Vector2 startPosition = basePosition - new Vector2(0, totalHeight / 2f);
            
            for (int i = 0; i < currentChoices.Count; i++)
            {
                Vector2 position = startPosition + new Vector2(0, i * optionHeight);
                Color color = i == currentChoiceIndex ? Color.White : Color.Gray;
                color *= choiceEase;
                
                // Simple text rendering for choices
                ActiveFont.Draw(currentChoices[i], position, new Vector2(0.5f, 0.5f), Vector2.One * 0.8f, color);
            }
            
            Draw.SpriteBatch.End();
        }
        #endregion

        #region Cleanup
        public override void End()
        {
            // Ensure music is restored when vignette ends abruptly
            try
            {
                if (!string.IsNullOrEmpty(areaMusic))
                {
                    Audio.SetMusic(areaMusic);
                }
            }
            catch { }
            base.End();
        }
        #endregion
    }
}



