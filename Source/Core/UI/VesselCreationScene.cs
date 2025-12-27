using System;
using System.Collections;
using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.DesoloZatnas.Core.UI
{
    /// <summary>
    /// Complete Vessel Creation System - Deltarune-style introduction
    /// Creates a vessel through player choices before discarding it
    /// </summary>
    public class VesselCreationScene : Scene
    {
        // Vessel parts
        private string selectedLegs = "normal";
        private string selectedTorso = "normal";
        private string selectedHead = "round";
        private string selectedColor = "red";
        private string selectedFood = "strawberry";
        private string selectedGift = "heart";
        private string vesselName = "VESSEL";
        private string creatorName = "CREATOR";
        private bool wasHonest = true;
        private string finalChoice = "friends";
        
        private int currentPhase = 0;
        private int currentSelection = 0;
        private float timer = 0f;
        private float fadeAlpha = 0f;
        private bool transitioning = false;
        
        // Choice data
        private List<string[]> phaseChoices;
        private string[] phaseDialogs;
        
        // Vessel preview
        private float vesselBob = 0f;
        private float glitchIntensity = 0f;
        
        // Text input mode
        private bool textInputMode = false;
        private string currentTextInput = "";
        private int maxTextLength = 12;
        
        public VesselCreationScene()
        {
            InitializeChoices();
            InitializeDialogs();
        }
        
        private void InitializeChoices()
        {
            phaseChoices = new List<string[]>
            {
                // Phase 0: Legs
                new string[] { "No legs", "Short legs", "Normal legs", "Long legs" },
                // Phase 1: Torso
                new string[] { "Slim torso", "Normal torso", "Wide torso" },
                // Phase 2: Head
                new string[] { "Round head", "Square head", "Pointed head" },
                // Phase 3: Color (skip for text input phases)
                new string[] { "Red", "Orange", "Yellow", "Green", "Cyan", "Blue", "Purple", "Pink" },
                // Phase 4: Food
                new string[] { "Snail Pie", "Spaghetti", "Hot Dog...?", "Butterscotch Pie", "Star Candy", "Maximum Tomato", "Strawberry" },
                // Phase 5: Gift
                new string[] { "A piece of your heart", "A cherished memory", "A spark of hope", "A fragment of power", "Nothing" },
                // Phase 6: Name (text input)
                null,
                // Phase 7: Honest question
                new string[] { "Yes", "No" },
                // Phase 8: Creator name (text input)
                null,
                // Phase 9: Final question
                new string[] { "My friends", "Myself", "The truth", "Nothing matters" },
                // Phase 10: Vessel display
                null,
                // Phase 11: Discard revelation
                null,
                // Phase 12: Chara reveal
                null
            };
        }
        
        private void InitializeDialogs()
        {
            phaseDialogs = new string[]
            {
                Dialog.Clean("VESSEL_CREATION_LEG_CHOICE"),
                Dialog.Clean("VESSEL_CREATION_TORSO_CHOICE"),
                Dialog.Clean("VESSEL_CREATION_HEAD_CHOICE"),
                Dialog.Clean("VESSEL_FAVORITE_COLOR"),
                Dialog.Clean("VESSEL_FAVORITE_FOOD"),
                Dialog.Clean("VESSEL_GIFT"),
                Dialog.Clean("VESSEL_CREATION_NAME_PROMPT"),
                Dialog.Clean("VESSEL_CREATION_HONEST_QUESTION"),
                Dialog.Clean("VESSEL_CREATION_CREATOR_PROMPT"),
                Dialog.Clean("VESSEL_FINAL_WHAT_MATTER"),
                Dialog.Clean("VESSEL_CREATION_DISPLAY"),
                Dialog.Clean("VESSEL_CREATION_DISCARD"),
                Dialog.Clean("VESSEL_CHARA_REVEAL")
            };
        }
        
        public override void Begin()
        {
            base.Begin();
            Audio.SetMusic(null);
            Audio.SetAmbience("event:/env/amb/worldtower_platform_b");
        }
        
        public override void Update()
        {
            base.Update();
            
            timer += Engine.DeltaTime;
            vesselBob += Engine.DeltaTime * 2f;
            
            if (transitioning)
            {
                fadeAlpha = Calc.Approach(fadeAlpha, 1f, Engine.DeltaTime * 2f);
                if (fadeAlpha >= 1f)
                {
                    transitioning = false;
                    fadeAlpha = 0f;
                }
                return;
            }
            
            fadeAlpha = Calc.Approach(fadeAlpha, 0f, Engine.DeltaTime * 2f);
            
            if (textInputMode)
            {
                HandleTextInput();
            }
            else if (phaseChoices[currentPhase] != null)
            {
                HandleChoiceInput();
            }
            else
            {
                HandleNarrativeInput();
            }
        }
        
        private void HandleChoiceInput()
        {
            string[] choices = phaseChoices[currentPhase];
            if (choices == null) return;
            
            if (Input.MenuUp.Pressed)
            {
                currentSelection = (currentSelection - 1 + choices.Length) % choices.Length;
                Audio.Play("event:/ui/main/rollover_up");
            }
            if (Input.MenuDown.Pressed)
            {
                currentSelection = (currentSelection + 1) % choices.Length;
                Audio.Play("event:/ui/main/rollover_down");
            }
            if (Input.MenuConfirm.Pressed)
            {
                SelectChoice(currentSelection);
                Audio.Play("event:/ui/main/button_select");
            }
        }
        
        private void HandleTextInput()
        {
            // Simplified text input - in full implementation, use TextInput class
            foreach (char c in MInput.Keyboard.CurrentState.GetPressedKeys()
                .Select(k => k.ToString())
                .Where(s => s.Length == 1 && char.IsLetterOrDigit(s[0]))
                .Select(s => s[0]))
            {
                if (currentTextInput.Length < maxTextLength)
                {
                    currentTextInput += c;
                }
            }
            
            if (Input.MenuCancel.Pressed && currentTextInput.Length > 0)
            {
                currentTextInput = currentTextInput.Substring(0, currentTextInput.Length - 1);
            }
            
            if (Input.MenuConfirm.Pressed && currentTextInput.Length > 0)
            {
                ConfirmTextInput();
                Audio.Play("event:/ui/main/button_select");
            }
        }
        
        private void HandleNarrativeInput()
        {
            if (Input.MenuConfirm.Pressed)
            {
                AdvancePhase();
                Audio.Play("event:/ui/main/button_select");
            }
        }
        
        private void SelectChoice(int choice)
        {
            switch (currentPhase)
            {
                case 0: // Legs
                    selectedLegs = new[] { "none", "short", "normal", "long" }[choice];
                    break;
                case 1: // Torso
                    selectedTorso = new[] { "slim", "normal", "wide" }[choice];
                    break;
                case 2: // Head
                    selectedHead = new[] { "round", "square", "pointed" }[choice];
                    break;
                case 3: // Color
                    selectedColor = new[] { "red", "orange", "yellow", "green", "cyan", "blue", "purple", "pink" }[choice];
                    break;
                case 4: // Food
                    selectedFood = new[] { "snails", "spaghetti", "hotdog", "butterscotch", "stars", "tomato", "strawberry" }[choice];
                    break;
                case 5: // Gift
                    selectedGift = new[] { "heart", "memory", "hope", "power", "nothing" }[choice];
                    break;
                case 7: // Honest
                    wasHonest = choice == 0;
                    break;
                case 9: // Final choice
                    finalChoice = new[] { "friends", "self", "truth", "nothing" }[choice];
                    break;
            }
            
            AdvancePhase();
        }
        
        private void ConfirmTextInput()
        {
            if (currentPhase == 6)
            {
                vesselName = currentTextInput.ToUpper();
            }
            else if (currentPhase == 8)
            {
                creatorName = currentTextInput.ToUpper();
            }
            
            currentTextInput = "";
            textInputMode = false;
            AdvancePhase();
        }
        
        private void AdvancePhase()
        {
            transitioning = true;
            currentPhase++;
            currentSelection = 0;
            
            // Check for text input phases
            if (currentPhase == 6 || currentPhase == 8)
            {
                textInputMode = true;
            }
            
            // End of vessel creation
            if (currentPhase >= 13)
            {
                TransitionToGame();
            }
            
            // Increase glitch as we approach the discard
            if (currentPhase >= 10)
            {
                glitchIntensity = (currentPhase - 10) * 0.2f;
            }
        }
        
        private void TransitionToGame()
        {
            // Save vessel data to session
            // Transition to the actual game
            Audio.Play("event:/game/general/assist_screenshake");
            
            // After Chara reveal, start the game proper
            Engine.Scene = new OverworldLoader(Overworld.StartMode.MainMenu);
        }
        
        public override void Render()
        {
            base.Render();
            
            // Dark background
            Draw.Rect(0, 0, 1920, 1080, Color.Black);
            
            // Render based on current phase
            if (currentPhase < 10)
            {
                RenderCreationPhase();
            }
            else if (currentPhase == 10)
            {
                RenderVesselDisplay();
            }
            else if (currentPhase == 11)
            {
                RenderDiscard();
            }
            else
            {
                RenderCharaReveal();
            }
            
            // Fade overlay
            if (fadeAlpha > 0f)
            {
                Draw.Rect(0, 0, 1920, 1080, Color.Black * fadeAlpha);
            }
            
            // Glitch effect
            if (glitchIntensity > 0f)
            {
                RenderGlitchEffect();
            }
        }
        
        private void RenderCreationPhase()
        {
            // Draw dialog
            string dialog = phaseDialogs[currentPhase];
            ActiveFont.DrawOutline(
                dialog,
                new Vector2(960, 200),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 1.2f,
                Color.White,
                2f,
                Color.Black
            );
            
            // Draw choices or text input
            if (textInputMode)
            {
                RenderTextInput();
            }
            else if (phaseChoices[currentPhase] != null)
            {
                RenderChoices();
            }
            
            // Draw vessel preview
            RenderVesselPreview(new Vector2(1400, 600));
        }
        
        private void RenderChoices()
        {
            string[] choices = phaseChoices[currentPhase];
            float startY = 400f;
            float spacing = 50f;
            
            for (int i = 0; i < choices.Length; i++)
            {
                Color color = i == currentSelection ? Color.Yellow : Color.White;
                string prefix = i == currentSelection ? "> " : "  ";
                
                ActiveFont.DrawOutline(
                    prefix + choices[i],
                    new Vector2(200, startY + i * spacing),
                    Vector2.Zero,
                    Vector2.One,
                    color,
                    2f,
                    Color.Black
                );
            }
        }
        
        private void RenderTextInput()
        {
            string display = currentTextInput + "_";
            
            ActiveFont.DrawOutline(
                display,
                new Vector2(960, 500),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 2f,
                Color.Yellow,
                3f,
                Color.Black
            );
            
            ActiveFont.DrawOutline(
                "Press CONFIRM when done",
                new Vector2(960, 600),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 0.8f,
                Color.Gray,
                2f,
                Color.Black
            );
        }
        
        private void RenderVesselPreview(Vector2 pos)
        {
            float bob = (float)Math.Sin(vesselBob) * 8f;
            pos.Y += bob;
            
            // Simple vessel representation based on choices
            Color vesselColor = GetColorFromSelection();
            
            // Head
            float headSize = selectedHead == "round" ? 40f : (selectedHead == "square" ? 35f : 30f);
            Draw.Circle(pos + new Vector2(0, -60), headSize, vesselColor, 16);
            
            // Torso
            float torsoWidth = selectedTorso == "slim" ? 30f : (selectedTorso == "normal" ? 45f : 60f);
            Draw.Rect(pos.X - torsoWidth / 2, pos.Y - 20, torsoWidth, 60f, vesselColor);
            
            // Legs
            if (selectedLegs != "none")
            {
                float legHeight = selectedLegs == "short" ? 30f : (selectedLegs == "normal" ? 50f : 70f);
                Draw.Rect(pos.X - 20, pos.Y + 40, 15f, legHeight, vesselColor);
                Draw.Rect(pos.X + 5, pos.Y + 40, 15f, legHeight, vesselColor);
            }
            
            // Eyes (white dots)
            Draw.Circle(pos + new Vector2(-10, -65), 5f, Color.White, 8);
            Draw.Circle(pos + new Vector2(10, -65), 5f, Color.White, 8);
        }
        
        private Color GetColorFromSelection()
        {
            return selectedColor switch
            {
                "red" => Color.Red,
                "orange" => Color.Orange,
                "yellow" => Color.Yellow,
                "green" => Color.Green,
                "cyan" => Color.Cyan,
                "blue" => Color.Blue,
                "purple" => new Color(128, 0, 128),
                "pink" => Color.HotPink,
                _ => Color.White
            };
        }
        
        private void RenderVesselDisplay()
        {
            RenderVesselPreview(new Vector2(960, 400));
            
            ActiveFont.DrawOutline(
                vesselName,
                new Vector2(960, 700),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 2f,
                Color.White,
                3f,
                Color.Black
            );
            
            ActiveFont.DrawOutline(
                "Created by: " + creatorName,
                new Vector2(960, 800),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 1f,
                Color.Gray,
                2f,
                Color.Black
            );
        }
        
        private void RenderDiscard()
        {
            // Vessel fading/distorting
            float fade = 1f - glitchIntensity;
            
            // Show discard text
            ActiveFont.DrawOutline(
                Dialog.Clean("VESSEL_CREATION_DISCARD"),
                new Vector2(960, 300),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 1.2f,
                Color.Red * fade,
                2f,
                Color.Black
            );
        }
        
        private void RenderCharaReveal()
        {
            // Chara appearing
            float pulse = 0.7f + (float)Math.Sin(timer * 3f) * 0.3f;
            
            ActiveFont.DrawOutline(
                "CHARA",
                new Vector2(960, 400),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 3f * pulse,
                Color.Red,
                4f,
                Color.Black
            );
            
            ActiveFont.DrawOutline(
                "Now then...",
                new Vector2(960, 600),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 1.5f,
                Color.White,
                2f,
                Color.Black
            );
            
            ActiveFont.DrawOutline(
                "Let's climb a mountain.",
                new Vector2(960, 700),
                new Vector2(0.5f, 0.5f),
                Vector2.One * 1.2f,
                Color.White,
                2f,
                Color.Black
            );
        }
        
        private void RenderGlitchEffect()
        {
            // Simple glitch bands
            Random rand = new Random((int)(timer * 10));
            for (int i = 0; i < 5; i++)
            {
                float y = rand.Next(0, 1080);
                float height = rand.Next(2, 20);
                float offset = (float)(rand.NextDouble() - 0.5) * glitchIntensity * 100f;
                
                Draw.Rect(offset, y, 1920, height, Color.Red * glitchIntensity * 0.5f);
            }
        }
    }
}
