using System;
using System.Collections;
using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.DesoloZatnas.Core.UI
{
    /// <summary>
    /// Game Over screen with lives system
    /// Triggers when HP reaches 0 and player runs out of lives
    /// </summary>
    public class GameOverScreen : Scene
    {
        private float fadeAlpha = 0f;
        private float textAlpha = 0f;
        private float pulseTimer = 0f;
        private bool canContinue = false;
        private bool canRetry = true;
        private int selectedOption = 0;
        
        private string gameOverMusic = "event:/Ingeste/music/menu/gameover";
        private Session session;
        private Level level;
        
        // Encouraging dialog system
        private string encouragingMessage = "";
        private string encouragingCharacter = "";
        private Color characterColor = Color.White;

        // Stats
        private int deathCount;
        private string chapterName;
        private TimeSpan timeSpent;

        public GameOverScreen(Level level, Session session)
        {
            this.level = level;
            this.session = session;
            
            // Gather stats
            if (session != null)
            {
                deathCount = session.Deaths;
                AreaData area = AreaData.Get(session.Area);
                chapterName = area != null ? Dialog.Clean(area.Name) : "Unknown";
                timeSpent = TimeSpan.FromTicks(session.Time);
            }
            
            // Select a random encouraging message from a random character
            SelectEncouragingMessage();
        }

        public override void Begin()
        {
            base.Begin();
            
            // Start game over music
            Audio.SetMusic(gameOverMusic);
            Audio.SetAmbience(null);
        }

        public override void End()
        {
            base.End();
            Audio.SetMusic(null);
        }

        public override void Update()
        {
            base.Update();

            // Fade in animation
            if (fadeAlpha < 1f)
            {
                fadeAlpha = Calc.Approach(fadeAlpha, 1f, Engine.DeltaTime * 1.5f);
                textAlpha = Calc.Approach(textAlpha, 1f, Engine.DeltaTime * 0.8f);
            }
            else
            {
                canContinue = true;
            }

            pulseTimer += Engine.DeltaTime * 3f;

            if (canContinue)
            {
                // Menu navigation
                if (Input.MenuDown.Pressed)
                {
                    selectedOption = (selectedOption + 1) % 3;
                    Audio.Play("event:/ui/main/rollover_down");
                }
                else if (Input.MenuUp.Pressed)
                {
                    selectedOption = (selectedOption - 1 + 3) % 3;
                    Audio.Play("event:/ui/main/rollover_up");
                }

                if (Input.MenuConfirm.Pressed)
                {
                    HandleSelection();
                }
            }
        }

        private void HandleSelection()
        {
            Audio.Play("event:/ui/main/button_select");

            switch (selectedOption)
            {
                case 0: // Retry
                    RetryLevel();
                    break;
                case 1: // Return to Map
                    ReturnToMap();
                    break;
                case 2: // Quit to Menu
                    QuitToMenu();
                    break;
            }
        }

        private void RetryLevel()
        {
            if (session != null)
            {
                // Reset lives (this would use custom lives system)
                // For now, just restart the level
                Audio.SetMusic(null);
                Engine.Scene = new LevelLoader(session, session.RespawnPoint);
            }
        }

        private void ReturnToMap()
        {
            Audio.SetMusic(null);
            if (session != null)
            {
                Engine.Scene = new OverworldLoader(Overworld.StartMode.AreaComplete, snow: null);
            }
            else
            {
                Engine.Scene = new OverworldLoader(Overworld.StartMode.MainMenu);
            }
        }

        private void QuitToMenu()
        {
            Audio.SetMusic(null);
            SaveData.Instance?.BeforeSave();
            Engine.Scene = new OverworldLoader(Overworld.StartMode.MainMenu);
        }

        public override void Render()
        {
            base.Render();

            // Black background fade
            Draw.Rect(0, 0, 1920, 1080, Color.Black * fadeAlpha * 0.95f);

            if (textAlpha <= 0f)
                return;

            // "GAME OVER" title
            float titleY = 200f;
            float pulseScale = 1f + (float)Math.Sin(pulseTimer) * 0.05f;
            
            ActiveFont.DrawOutline("GAME OVER", new Vector2(960, titleY), new Vector2(0.5f, 0.5f),
                Vector2.One * 3f * pulseScale, Color.Red * textAlpha, 4f, Color.DarkRed * textAlpha);

            // Stats section
            float statsY = 350f;
            float lineSpacing = 50f;
            Color statsColor = Color.White * textAlpha * 0.8f;

            RenderStatLine("Chapter:", chapterName, statsY, statsColor);
            RenderStatLine("Deaths:", deathCount.ToString(), statsY + lineSpacing, statsColor);
            RenderStatLine("Time:", FormatTime(timeSpent), statsY + lineSpacing * 2, statsColor);

            // Menu options
            if (canContinue)
            {
                RenderMenu();
            }
            else
            {
                // "Please wait..." message
                float alpha = (float)Math.Sin(pulseTimer * 2f) * 0.3f + 0.7f;
                ActiveFont.DrawOutline("Please wait...", new Vector2(960, 700),
                    new Vector2(0.5f, 0.5f), Vector2.One * 1f, 
                    Color.Gray * textAlpha * alpha, 2f, Color.Black * textAlpha * alpha);
            }

            // Flavor text
            string[] quotes = new string[]
            {
                "Don't give up!",
                "You can do this!",
                "Try again!",
                "Every failure is a lesson.",
                "The mountain awaits...",
            };
            int quoteIndex = (int)(Engine.Scene.TimeActive * 0.1f) % quotes.Length;
            
            ActiveFont.DrawOutline(quotes[quoteIndex], new Vector2(960, 950),
                new Vector2(0.5f, 0.5f), Vector2.One * 0.9f,
                Color.Cyan * textAlpha * 0.6f, 2f, Color.Black * textAlpha * 0.3f);
            
            // Character encouraging message section
            if (!string.IsNullOrEmpty(encouragingMessage))
            {
                RenderEncouragingMessage();
            }
        }

        private void RenderStatLine(string label, string value, float y, Color color)
        {
            ActiveFont.DrawOutline(label, new Vector2(600, y), new Vector2(1f, 0.5f),
                Vector2.One * 1.1f, color, 2f, Color.Black * textAlpha * 0.5f);
            
            ActiveFont.DrawOutline(value, new Vector2(700, y), new Vector2(0f, 0.5f),
                Vector2.One * 1.1f, color, 2f, Color.Black * textAlpha * 0.5f);
        }

        private void RenderMenu()
        {
            string[] options = new string[]
            {
                "Retry Level",
                "Return to Map",
                "Quit to Menu"
            };

            float menuY = 600f;
            float spacing = 70f;

            for (int i = 0; i < options.Length; i++)
            {
                bool isSelected = i == selectedOption;
                Color color = isSelected ? Color.Yellow : Color.White;
                float scale = isSelected ? 1.3f : 1.1f;
                
                if (isSelected)
                {
                    float pulse = (float)Math.Sin(pulseTimer * 4f) * 0.1f + 0.9f;
                    color *= pulse;
                }

                string displayText = isSelected ? $"> {options[i]} <" : options[i];
                
                ActiveFont.DrawOutline(displayText, new Vector2(960, menuY + i * spacing),
                    new Vector2(0.5f, 0.5f), Vector2.One * scale,
                    color * textAlpha, 2f, Color.Black * textAlpha);
            }
        }

        private string FormatTime(TimeSpan time)
        {
            if (time.TotalHours >= 1)
                return $"{(int)time.TotalHours}:{time.Minutes:D2}:{time.Seconds:D2}";
            else
                return $"{time.Minutes}:{time.Seconds:D2}";
        }

        /// <summary>
        /// Selects a random encouraging message from a random character
        /// </summary>
        private void SelectEncouragingMessage()
        {
            // Define character categories with their message counts and colors
            var characters = new List<(string prefix, int count, string name, Color color)>
            {
                ("DESOLOZATNAS_GAMEOVER_TITANKING_", 8, "Titan King", new Color(180, 140, 60)),
                ("DESOLOZATNAS_GAMEOVER_ASRIEL_", 8, "Asriel", new Color(150, 255, 150)),
                ("DESOLOZATNAS_GAMEOVER_CHARA_", 9, "Chara", new Color(255, 220, 100)),
                ("DESOLOZATNAS_GAMEOVER_BADELINE_", 8, "Badeline", new Color(180, 100, 200)),
                ("DESOLOZATNAS_GAMEOVER_KIRBYPARENT_", 9, "Kirby's Family", new Color(255, 180, 200)),
                ("DESOLOZATNAS_GAMEOVER_GASTER_", 9, "W.D. Gaster", new Color(200, 200, 200)),
            };

            // Additional supporting characters for variety
            var supportingCharacters = new List<(string key, string name, Color color)>
            {
                ("DESOLOZATNAS_GAMEOVER_TORIEL_1", "Toriel", new Color(180, 150, 255)),
                ("DESOLOZATNAS_GAMEOVER_TORIEL_2", "Toriel", new Color(180, 150, 255)),
                ("DESOLOZATNAS_GAMEOVER_SANS_1", "Sans", new Color(120, 200, 255)),
                ("DESOLOZATNAS_GAMEOVER_SANS_2", "Sans", new Color(120, 200, 255)),
                ("DESOLOZATNAS_GAMEOVER_PAPYRUS_1", "Papyrus", new Color(255, 140, 50)),
                ("DESOLOZATNAS_GAMEOVER_PAPYRUS_2", "Papyrus", new Color(255, 140, 50)),
                ("DESOLOZATNAS_GAMEOVER_UNDYNE_1", "Undyne", new Color(100, 150, 255)),
                ("DESOLOZATNAS_GAMEOVER_UNDYNE_2", "Undyne", new Color(100, 150, 255)),
                ("DESOLOZATNAS_GAMEOVER_ALPHYS_1", "Alphys", new Color(255, 220, 100)),
                ("DESOLOZATNAS_GAMEOVER_ASGORE_1", "Asgore", new Color(180, 100, 200)),
                ("DESOLOZATNAS_GAMEOVER_THEO_1", "Theo", new Color(100, 200, 255)),
                ("DESOLOZATNAS_GAMEOVER_GRANNY_1", "Granny", new Color(255, 200, 200)),
                ("DESOLOZATNAS_GAMEOVER_MADELINE_1", "Madeline", new Color(255, 150, 150)),
                ("DESOLOZATNAS_GAMEOVER_MADELINE_2", "Madeline", new Color(255, 150, 150)),
            };

            Random random = new Random();
            
            // 70% chance for main characters, 30% for supporting
            if (random.NextDouble() < 0.7)
            {
                // Select from main encouraging characters
                var selectedCharacter = characters[random.Next(characters.Count)];
                int messageNum = random.Next(1, selectedCharacter.count + 1);
                string dialogKey = $"{selectedCharacter.prefix}{messageNum}";
                
                encouragingMessage = Dialog.Clean(dialogKey);
                encouragingCharacter = selectedCharacter.name;
                characterColor = selectedCharacter.color;
            }
            else
            {
                // Select from supporting characters
                var selectedCharacter = supportingCharacters[random.Next(supportingCharacters.Count)];
                
                encouragingMessage = Dialog.Clean(selectedCharacter.key);
                encouragingCharacter = selectedCharacter.name;
                characterColor = selectedCharacter.color;
            }
        }

        /// <summary>
        /// Renders the encouraging message box with character name
        /// </summary>
        private void RenderEncouragingMessage()
        {
            if (textAlpha <= 0f)
                return;

            float boxY = 520f;
            float boxWidth = 1400f;
            float boxHeight = 140f;
            float boxX = (1920f - boxWidth) / 2f;

            // Semi-transparent background box
            Draw.Rect(boxX, boxY, boxWidth, boxHeight, Color.Black * 0.7f * textAlpha);
            
            // Box border
            Draw.HollowRect(boxX, boxY, boxWidth, boxHeight, characterColor * textAlpha);
            Draw.HollowRect(boxX + 2, boxY + 2, boxWidth - 4, boxHeight - 4, characterColor * textAlpha * 0.5f);

            // Character name label
            float characterLabelY = boxY - 20f;
            string characterLabel = $"- {encouragingCharacter} -";
            
            ActiveFont.DrawOutline(characterLabel, new Vector2(960, characterLabelY),
                new Vector2(0.5f, 0.5f), Vector2.One * 0.9f,
                characterColor * textAlpha, 2f, Color.Black * textAlpha);

            // Encouraging message (word-wrapped)
            Vector2 messagePos = new Vector2(boxX + 40, boxY + 30);
            float maxWidth = boxWidth - 80f;
            
            ActiveFont.DrawOutline(encouragingMessage, messagePos,
                new Vector2(0f, 0f), Vector2.One * 0.75f,
                Color.White * textAlpha * 0.95f, 2f, Color.Black * textAlpha * 0.8f);

            // Small "Stay Determined!" footer
            float footerY = boxY + boxHeight + 25f;
            float footerPulse = (float)Math.Sin(pulseTimer * 2f) * 0.15f + 0.85f;
            
            ActiveFont.DrawOutline("★ Stay Determined! ★", new Vector2(960, footerY),
                new Vector2(0.5f, 0.5f), Vector2.One * 0.8f,
                Color.Yellow * textAlpha * footerPulse, 2f, Color.Red * textAlpha * footerPulse * 0.5f);
        }
    }

    /// <summary>
    /// Lives system manager for DesoloZatnas
    /// </summary>
    public static class LivesSystem
    {
        private const int DEFAULT_LIVES = 3;
        private const int MAX_LIVES = 9;

        public static int CurrentLives { get; private set; } = DEFAULT_LIVES;
        public static int MaxLives { get; private set; } = MAX_LIVES;

        public static void Initialize()
        {
            CurrentLives = DEFAULT_LIVES;
        }

        public static void AddLife()
        {
            if (CurrentLives < MaxLives)
            {
                CurrentLives++;
                Audio.Play("event:/game/general/1up_touch");
            }
        }

        public static bool LoseLife()
        {
            CurrentLives--;
            
            if (CurrentLives <= 0)
            {
                CurrentLives = 0;
                return true; // Game over
            }
            
            return false;
        }

        public static void Reset()
        {
            CurrentLives = DEFAULT_LIVES;
        }

        public static void SetLives(int lives)
        {
            CurrentLives = Math.Clamp(lives, 0, MaxLives);
        }
    }
}
