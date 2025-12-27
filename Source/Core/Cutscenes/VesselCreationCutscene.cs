namespace DesoloZantas.Core.Core.Cutscenes
{
    [CustomEntity("DesoloZantas/VesselCreationCutscene")]
    public class VesselCreationCutscene : CutsceneEntity
    {
        private global::Celeste.Player player;
        private bool completed = false;
        private Vector2 originalCameraPosition;
        private VesselCreationUI vesselUI;

        public VesselCreationCutscene(EntityData data, Vector2 offset) : base()
        {
            Position = data.Position + offset;
        }

        public override void OnBegin(Level level)
        {
            player = Scene.Tracker.GetEntity<global::Celeste.Player>();
            originalCameraPosition = level.Camera.Position;
            
            Add(new Coroutine(VesselCreationSequence()));
        }

        public override void OnEnd(Level level)
        {
            if (vesselUI != null)
            {
                Scene.Remove(vesselUI);
                vesselUI = null;
            }

            // Restore camera position
            level.Camera.Position = originalCameraPosition;
            
            // Set completion flag
            level.Session.SetFlag("vessel_creation_complete");
        }

        private IEnumerator VesselCreationSequence()
        {
            if (player != null)
            {
                player.StateMachine.State = global::Celeste.Player.StDummy;
            }

            Level level = Scene as Level;

            // Introduction
            yield return Textbox.Say("VESSEL_CREATION_INTRO");
            
            // Dark screen effect
            yield return FadeToBlack(1.0f);
            
            // Leg choice
            yield return Textbox.Say("VESSEL_CREATION_LEG_CHOICE");
            yield return ShowVesselChoice("legs");
            
            // Torso choice
            yield return Textbox.Say("VESSEL_CREATION_TORSO_CHOICE");
            yield return ShowVesselChoice("torso");
            
            // Head choice
            yield return Textbox.Say("VESSEL_CREATION_HEAD_CHOICE");
            yield return ShowVesselChoice("head");
            
            // Name prompt
            yield return Textbox.Say("VESSEL_CREATION_NAME_PROMPT");
            yield return ShowNameInput();
            
            // Honesty question
            yield return Textbox.Say("VESSEL_CREATION_HONEST_QUESTION");
            yield return ShowBinaryChoice("Yes", "No");
            
            // For now, assume honest choice
            yield return Textbox.Say("VESSEL_CREATION_HONEST_RESPONSE");
            
            // Feelings question
            yield return Textbox.Say("VESSEL_CREATION_FEELINGS_QUESTION");
            yield return ShowTextInput();
            
            // Creator prompt
            yield return Textbox.Say("VESSEL_CREATION_CREATOR_PROMPT");
            yield return ShowNameInput();
            
            // Display vessel
            yield return Textbox.Say("VESSEL_CREATION_DISPLAY");
            yield return ShowVesselDisplay();
            
            // Discard vessel
            yield return Textbox.Say("VESSEL_CREATION_DISCARD");
            
            // Fate choice - simplified for now
            yield return ShowMultipleChoice(new string[] { "Accept", "Try to Save", "Question", "..." });
            
            // For now, show accept response
            yield return Textbox.Say("VESSEL_CREATION_FATE_ACCEPT");
            
            // Transition
            yield return Textbox.Say("VESSEL_CREATION_TRANSITION");
            
            yield return FadeFromBlack(1.0f);
            
            completed = true;
            EndCutscene(level);
        }

        private IEnumerator FadeToBlack(float duration)
        {
            // Simple fade implementation
            float timer = 0f;
            while (timer < duration)
            {
                timer += Engine.DeltaTime;
                yield return null;
            }
        }

        private IEnumerator FadeFromBlack(float duration)
        {
            // Simple fade implementation
            float timer = 0f;
            while (timer < duration)
            {
                timer += Engine.DeltaTime;
                yield return null;
            }
        }

        private IEnumerator ShowVesselChoice(string partType)
        {
            // Show vessel customization UI
            vesselUI = new VesselCreationUI(partType);
            Scene.Add(vesselUI);
            
            // Wait for player choice
            while (!vesselUI.ChoiceMade)
            {
                yield return null;
            }
            
            Scene.Remove(vesselUI);
            vesselUI = null;
        }

        private IEnumerator ShowNameInput()
        {
            // Show text input for naming
            yield return 1.0f; // Placeholder
        }

        private IEnumerator ShowTextInput()
        {
            // Show text input for feelings
            yield return 1.0f; // Placeholder
        }

        private IEnumerator ShowBinaryChoice(string option1, string option2)
        {
            // Show binary choice UI - simplified for now
            yield return 1.0f; // Placeholder
        }

        private IEnumerator ShowMultipleChoice(string[] options)
        {
            // Show multiple choice UI - simplified for now
            yield return 1.0f; // Placeholder
        }

        private IEnumerator ShowVesselDisplay()
        {
            // Show the completed vessel
            yield return 2.0f; // Display for 2 seconds
        }
    }

    // Simple UI class for vessel creation
    public class VesselCreationUI : Entity
    {
        public bool ChoiceMade { get; private set; } = false;
        private string partType;

        public VesselCreationUI(string partType)
        {
            this.partType = partType;
            Depth = -1000; // Render on top
        }

        public override void Update()
        {
            base.Update();
            
            // Simple input handling - in a real implementation this would be more sophisticated
            if (Input.MenuConfirm.Pressed)
            {
                ChoiceMade = true;
            }
        }

        public override void Render()
        {
            base.Render();
            
            // Render vessel creation UI elements
            // This would contain actual UI rendering in a complete implementation
        }
    }
}



