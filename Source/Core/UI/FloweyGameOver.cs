using System;
using System.Collections;
using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.DesoloZatnas.Core.UI
{
    /// <summary>
    /// Flowey's evil game over screen - appears randomly on death
    /// Gives rude/negative feedback and forcefully closes the game with evil laughter
    /// Inspired by Undertale's Flowey and other creepy meta moments
    /// </summary>
    public class FloweyGameOver : Scene
    {
        private float fadeAlpha = 0f;
        private float textAlpha = 0f;
        private float pulseTimer = 0f;
        private bool messageDone = false;
        private bool laughing = false;
        private float laughTimer = 0f;
        private float closeTimer = 0f;
        
        private string[] evilMessages;
        private int currentMessageIndex = 0;
        private float messageTimer = 0f;
        private const float MESSAGE_DISPLAY_TIME = 2.5f;
        
        private Session session;
        private Level level;
        
        // Flowey face sprite
        private Sprite floweySprite;
        private float floweyScale = 1f;
        private float floweyShake = 0f;
        
        // Evil laughter text
        private List<string> laughTexts = new List<string>
        {
            "Hee hee hee...",
            "HA HA HA HA HA!",
            "AHAHAHAHA!",
            "See you later... IDIOT!"
        };
        private int laughIndex = 0;
        
        public FloweyGameOver(Level level, Session session)
        {
            this.level = level;
            this.session = session;
            
            // Select evil messages
            SelectEvilMessages();
            
            // Initialize Flowey sprite (using placeholder - will need actual sprite)
            InitializeFloweySprite();
        }

        private void InitializeFloweySprite()
        {
            // Try to load Flowey sprite
            // Note: You'll need to add actual Flowey sprites to your Graphics/Atlases
            try
            {
                floweySprite = new Sprite(GFX.Game, "characters/flowey/");
                if (floweySprite != null)
                {
                    floweySprite.AddLoop("evil", "evil", 0.1f);
                    floweySprite.AddLoop("laugh", "laugh", 0.15f);
                    floweySprite.Play("evil");
                    floweySprite.Position = new Vector2(960, 400);
                    floweySprite.CenterOrigin();
                }
            }
            catch
            {
                // Fallback if sprite doesn't exist
                floweySprite = null;
            }
        }

        private void SelectEvilMessages()
        {
            // Get random evil messages from dialog
            List<string> allMessages = new List<string>();
            
            // Add the specific requested messages
            allMessages.Add(Dialog.Clean("DESOLOZATNAS_FLOWEY_GAMEOVER_NIGHTMARE"));
            allMessages.Add(Dialog.Clean("DESOLOZATNAS_FLOWEY_GAMEOVER_NEVER_WAKE"));
            
            // Add more evil messages
            for (int i = 1; i <= 15; i++)
            {
                string key = $"DESOLOZATNAS_FLOWEY_GAMEOVER_{i}";
                string message = Dialog.Clean(key);
                if (!string.IsNullOrEmpty(message) && message != key)
                {
                    allMessages.Add(message);
                }
            }
            
            // Shuffle and pick 3-4 messages
            Random random = new Random();
            List<string> selectedMessages = new List<string>();
            
            // Always include the two specific messages
            selectedMessages.Add(Dialog.Clean("DESOLOZATNAS_FLOWEY_GAMEOVER_NIGHTMARE"));
            selectedMessages.Add(Dialog.Clean("DESOLOZATNAS_FLOWEY_GAMEOVER_NEVER_WAKE"));
            
            // Add 1-2 more random messages
            int additionalCount = random.Next(1, 3);
            for (int i = 0; i < additionalCount && allMessages.Count > 2; i++)
            {
                int randomIndex = random.Next(allMessages.Count);
                if (!selectedMessages.Contains(allMessages[randomIndex]))
                {
                    selectedMessages.Add(allMessages[randomIndex]);
                }
            }
            
            evilMessages = selectedMessages.ToArray();
        }

        public override void Begin()
        {
            base.Begin();
            
            // Start creepy music/ambience
            Audio.SetMusic(null);
            Audio.SetAmbience("event:/env/amb/worldtower_platform_b");
            
            // Play Flowey's appearance sound (using Badeline reveal as placeholder)
            // TODO: Replace with custom Flowey appearance sound when available
            Audio.Play("event:/char/badeline/boss_prefight_reveal");
        }

        public override void End()
        {
            base.End();
            Audio.SetMusic(null);
            Audio.SetAmbience(null);
        }

        public override void Update()
        {
            base.Update();

            // Fade in
            if (fadeAlpha < 1f)
            {
                fadeAlpha = Calc.Approach(fadeAlpha, 1f, Engine.DeltaTime * 2f);
                textAlpha = Calc.Approach(textAlpha, 1f, Engine.DeltaTime * 1.2f);
            }

            pulseTimer += Engine.DeltaTime * 2f;
            
            // Update Flowey sprite
            if (floweySprite != null)
            {
                floweySprite.Update();
                
                if (laughing)
                {
                    floweyScale = 1f + (float)Math.Sin(pulseTimer * 8f) * 0.15f;
                    floweyShake = (float)Math.Sin(pulseTimer * 20f) * 5f;
                }
                else
                {
                    floweyScale = 1f + (float)Math.Sin(pulseTimer) * 0.05f;
                    floweyShake = 0f;
                }
            }

            // Message sequence
            if (!messageDone && currentMessageIndex < evilMessages.Length)
            {
                messageTimer += Engine.DeltaTime;
                
                if (messageTimer >= MESSAGE_DISPLAY_TIME)
                {
                    messageTimer = 0f;
                    currentMessageIndex++;
                    
                    // Play sound for each message (using Badeline bullet as placeholder)
                    // TODO: Replace with custom Flowey message sound when available
                    if (currentMessageIndex < evilMessages.Length)
                    {
                        Audio.Play("event:/char/badeline/boss_bullet");
                    }
                }
                
                if (currentMessageIndex >= evilMessages.Length)
                {
                    messageDone = true;
                    StartLaughing();
                }
            }
            else if (laughing)
            {
                laughTimer += Engine.DeltaTime;
                
                // Cycle through laugh texts
                if (laughTimer > 0.8f && laughIndex < laughTexts.Count - 1)
                {
                    laughIndex++;
                    laughTimer = 0f;
                    
                    // Play laugh sound each time (using Badeline laugh as placeholder)
                    // TODO: Add custom Flowey laugh variations
                    Audio.Play("event:/char/badeline/laugh_01");
                }
                
                // After final laugh, start close timer
                if (laughIndex >= laughTexts.Count - 1)
                {
                    closeTimer += Engine.DeltaTime;
                    
                    if (closeTimer >= 2f)
                    {
                        CloseGame();
                    }
                }
            }
            
            // Allow skip (but make it hard - hold Grab for 3 seconds)
            if (Input.Grab.Check)
            {
                closeTimer += Engine.DeltaTime * 2f;
                if (closeTimer >= 3f && !laughing)
                {
                    // Escape - go to normal game over
                    EscapeToNormalGameOver();
                }
            }
            else
            {
                if (!laughing)
                    closeTimer = 0f;
            }
        }

        private void StartLaughing()
        {
            laughing = true;
            laughTimer = 0f;
            laughIndex = 0;
            
            if (floweySprite != null)
            {
                floweySprite.Play("laugh");
            }
            
            // Screen shake
            Engine.TimeRate = 1f;
            
            // Play evil laugh (using Badeline laugh as placeholder)
            // TODO: Replace with custom Flowey evil laugh sound when available
            // Ideal sounds: "event:/Ingeste/flowey/evil_laugh" or similar
            Audio.Play("event:/char/badeline/laugh_01");
        }

        private void CloseGame()
        {
            // Save before closing (be nice at least)
            SaveData.Instance?.BeforeSave();
            
            // Close the game
            Engine.Instance.Exit();
        }

        private void EscapeToNormalGameOver()
        {
            // Player managed to escape - go to normal game over
            Audio.Play("event:/ui/main/button_back");
            
            if (session != null)
            {
                Engine.Scene = new GameOverScreen(level, session);
            }
            else
            {
                Engine.Scene = new OverworldLoader(Overworld.StartMode.MainMenu);
            }
        }

        public override void Render()
        {
            base.Render();

            // Black background
            Draw.Rect(0, 0, 1920, 1080, Color.Black * fadeAlpha);

            if (textAlpha <= 0f)
                return;

            // Draw Flowey sprite
            if (floweySprite != null)
            {
                float x = 960 + floweyShake;
                float y = 300;
                
                floweySprite.Scale = Vector2.One * floweyScale * 3f;
                floweySprite.Position = new Vector2(x, y);
                floweySprite.Render();
            }
            else
            {
                // Fallback - draw creepy smiley face using text
                string face = laughing ? "=)" : "=D";
                float faceScale = laughing ? 8f + (float)Math.Sin(pulseTimer * 8f) * 2f : 6f;
                Color faceColor = laughing ? Color.Red : Color.Yellow;
                
                ActiveFont.DrawOutline(face, new Vector2(960 + floweyShake, 300), 
                    new Vector2(0.5f, 0.5f), Vector2.One * faceScale,
                    faceColor * textAlpha, 8f, Color.Black * textAlpha);
            }

            // Draw current message
            if (!messageDone && currentMessageIndex < evilMessages.Length)
            {
                string currentMessage = evilMessages[currentMessageIndex];
                float messageY = 550f;
                
                // Pulse effect
                float messagePulse = 1f + (float)Math.Sin(pulseTimer * 3f) * 0.1f;
                
                ActiveFont.DrawOutline(currentMessage, new Vector2(960, messageY),
                    new Vector2(0.5f, 0.5f), Vector2.One * 1.2f * messagePulse,
                    Color.White * textAlpha, 3f, Color.DarkRed * textAlpha);
            }
            else if (laughing)
            {
                // Draw laugh text
                string laughText = laughTexts[laughIndex];
                float laughY = 550f;
                float laughScale = 1.5f + (float)Math.Sin(pulseTimer * 10f) * 0.3f;
                
                ActiveFont.DrawOutline(laughText, new Vector2(960, laughY),
                    new Vector2(0.5f, 0.5f), Vector2.One * laughScale,
                    Color.Red * textAlpha, 4f, Color.Black * textAlpha);
                
                // Closing message
                if (closeTimer > 0.5f)
                {
                    float closingAlpha = Calc.Clamp(closeTimer - 0.5f, 0f, 1f);
                    string closingMessage = "Game closing in " + (int)(2f - closeTimer + 1) + "...";
                    
                    ActiveFont.DrawOutline(closingMessage, new Vector2(960, 700),
                        new Vector2(0.5f, 0.5f), Vector2.One * 1f,
                        Color.White * textAlpha * closingAlpha, 2f, Color.Black * textAlpha * closingAlpha);
                }
            }
            
            // Progress indicator
            if (!messageDone)
            {
                string progress = $"{currentMessageIndex + 1}/{evilMessages.Length}";
                ActiveFont.DrawOutline(progress, new Vector2(960, 800),
                    new Vector2(0.5f, 0.5f), Vector2.One * 0.8f,
                    Color.Gray * textAlpha * 0.5f, 2f, Color.Black * textAlpha * 0.3f);
            }
            
            // Hint for escape (subtle)
            if (!laughing && closeTimer > 0f)
            {
                float hintAlpha = closeTimer / 3f;
                ActiveFont.DrawOutline("Hold to resist...", new Vector2(960, 950),
                    new Vector2(0.5f, 0.5f), Vector2.One * 0.6f,
                    Color.Cyan * textAlpha * hintAlpha * 0.5f, 1f, Color.Black * textAlpha * hintAlpha);
            }
        }
    }
    
    /// <summary>
    /// Helper to determine when to trigger Flowey game over
    /// </summary>
    public static class FloweyGameOverTrigger
    {
        private static Random random = new Random();
        private static int deathsSinceLastFlowey = 0;
        private const int MIN_DEATHS_BETWEEN_FLOWEY = 5;
        private const float BASE_CHANCE = 0.15f; // 15% base chance
        
        /// <summary>
        /// Check if Flowey should appear on this death
        /// </summary>
        public static bool ShouldTriggerFlowey(Session session)
        {
            deathsSinceLastFlowey++;
            
            // Don't trigger too frequently
            if (deathsSinceLastFlowey < MIN_DEATHS_BETWEEN_FLOWEY)
                return false;
            
            // Increase chance based on total deaths
            int totalDeaths = session?.Deaths ?? 0;
            float bonusChance = Math.Min(totalDeaths / 100f * 0.1f, 0.25f); // Up to +25% for every 100 deaths
            
            float totalChance = BASE_CHANCE + bonusChance;
            
            bool shouldTrigger = random.NextDouble() < totalChance;
            
            if (shouldTrigger)
            {
                deathsSinceLastFlowey = 0;
            }
            
            return shouldTrigger;
        }
        
        /// <summary>
        /// Force reset the counter (for testing or special conditions)
        /// </summary>
        public static void Reset()
        {
            deathsSinceLastFlowey = 0;
        }
    }
}
