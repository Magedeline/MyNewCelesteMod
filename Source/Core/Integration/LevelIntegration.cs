using System;
using Celeste;
using Celeste.Mod;
using Celeste.Mod.DesoloZatnas.Core.UI.InGame;
using Celeste.Mod.DesoloZatnas.Core.UI;
using Monocle;

namespace Celeste.Mod.DesoloZatnas.Core.Integration
{
    /// <summary>
    /// Level integration for DesoloZatnas - handles in-game UI and custom pause menu
    /// </summary>
    public static class LevelIntegration
    {
        private static bool hooksInstalled = false;

        /// <summary>
        /// Install hooks for in-game systems
        /// </summary>
        public static void InstallHooks()
        {
            if (hooksInstalled)
                return;

            Logger.Log(LogLevel.Info, "DesoloZatnas", "[LevelIntegration] Installing in-game UI hooks");

            // Hook Level.LoadLevel to inject custom HUD
            On.Celeste.Level.LoadLevel += Level_LoadLevel;
            
            // Hook pause menu creation to add custom options
            Everest.Events.Level.OnCreatePauseMenuButtons += Level_OnCreatePauseMenuButtons;

            hooksInstalled = true;
        }

        /// <summary>
        /// Uninstall hooks for in-game systems
        /// </summary>
        public static void UninstallHooks()
        {
            if (!hooksInstalled)
                return;

            Logger.Log(LogLevel.Info, "DesoloZatnas", "[LevelIntegration] Uninstalling in-game UI hooks");

            On.Celeste.Level.LoadLevel -= Level_LoadLevel;
            Everest.Events.Level.OnCreatePauseMenuButtons -= Level_OnCreatePauseMenuButtons;

            hooksInstalled = false;
        }

        private static void Level_LoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader)
        {
            // Call original first
            orig(self, playerIntro, isFromLoader);

            // Check if we're in a DesoloZatnas chapter (10-21)
            if (IsDesoloZantasChapter(self.Session.Area))
            {
                // Add custom HUD to the level
                DesoloZantasHUD hud = new DesoloZantasHUD(self);
                self.Add(hud);
                
                Logger.Log(LogLevel.Verbose, "DesoloZatnas", 
                    $"[LevelIntegration] Added DesoloZantasHUD to chapter {self.Session.Area.ID}");
            }
        }

        private static void Level_OnCreatePauseMenuButtons(Level level, TextMenu menu, bool minimal)
        {
            // Only add custom options for DesoloZatnas chapters
            if (!IsDesoloZantasChapter(level.Session.Area))
                return;

            // Don't add custom options in minimal mode
            if (minimal)
                return;

            Logger.Log(LogLevel.Verbose, "DesoloZatnas", "[LevelIntegration] Adding custom pause menu options");

            // Add separator before custom options
            menu.Add(new TextMenu.SubHeader("DesoloZatnas Options"));

            // Quick Stats option
            TextMenu.Button quickStatsButton = new TextMenu.Button(Dialog.Clean("DESOLOZATNAS_PAUSE_QUICK_STATS"));
            quickStatsButton.Pressed(() => ShowQuickStats(level));
            menu.Add(quickStatsButton);

            // D-Side Progress option (only for D-Side mode)
            if (level.Session.Area.Mode == (AreaMode)3) // D-Side
            {
                TextMenu.Button dsideProgressButton = new TextMenu.Button(Dialog.Clean("DESOLOZATNAS_PAUSE_DSIDE_PROGRESS"));
                dsideProgressButton.Pressed(() => ShowDSideProgress(level));
                menu.Add(dsideProgressButton);
            }

            // Lives Info option
            TextMenu.Button livesInfoButton = new TextMenu.Button(Dialog.Clean("DESOLOZATNAS_PAUSE_LIVES_INFO"));
            livesInfoButton.Pressed(() => ShowLivesInfo(level));
            menu.Add(livesInfoButton);
        }

        private static void ShowQuickStats(Level level)
        {
            // Show a quick stats overlay
            level.Paused = false; // Unpause
            
            // Create and add quick stats entity
            Entity statsOverlay = new QuickStatsOverlay(level);
            level.Add(statsOverlay);
            
            Audio.Play("event:/ui/main/button_select");
        }

        private static void ShowDSideProgress(Level level)
        {
            level.Paused = false;
            
            // Create and add D-Side progress overlay
            Entity progressOverlay = new DSideProgressOverlay(level);
            level.Add(progressOverlay);
            
            Audio.Play("event:/ui/main/button_select");
        }

        private static void ShowLivesInfo(Level level)
        {
            level.Paused = false;
            
            // Create and add lives info overlay
            Entity livesOverlay = new LivesInfoOverlay(level);
            level.Add(livesOverlay);
            
            Audio.Play("event:/ui/main/button_select");
        }

        private static bool IsDesoloZantasChapter(AreaKey area)
        {
            // Chapters 0-21 are DesoloZatnas chapters
            return area.ID >= 0 && area.ID <= 21;
        }

        #region Overlay Entities

        /// <summary>
        /// Quick stats overlay showing deaths, time, and collectibles
        /// </summary>
        private class QuickStatsOverlay : Entity
        {
            private Level level;
            private float alpha = 0f;
            private bool closing = false;

            public QuickStatsOverlay(Level level)
            {
                this.level = level;
                Tag = Tags.HUD | Tags.PauseUpdate;
                Depth = -10000;
            }

            public override void Update()
            {
                base.Update();

                if (closing)
                {
                    alpha = Calc.Approach(alpha, 0f, Engine.DeltaTime * 4f);
                    if (alpha <= 0f)
                        RemoveSelf();
                }
                else
                {
                    alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime * 4f);
                }

                if (Input.MenuConfirm.Pressed || Input.MenuCancel.Pressed)
                {
                    closing = true;
                    Audio.Play("event:/ui/main/button_back");
                }
            }

            public override void Render()
            {
                base.Render();

                // Background
                Draw.Rect(0, 0, 1920, 1080, Color.Black * 0.7f * alpha);

                if (alpha <= 0f)
                    return;

                // Title
                Vector2 titlePos = new Vector2(960, 100);
                ActiveFont.DrawOutline("Quick Stats", titlePos, new Vector2(0.5f, 0.5f),
                    Vector2.One * 1.5f, Color.Yellow * alpha, 2f, Color.Black * alpha);

                // Stats
                float y = 250f;
                float spacing = 60f;

                DrawStat("Deaths:", level.Session.Deaths.ToString(), y, alpha);
                y += spacing;

                DrawStat("Time:", FormatTime(level.Session.Time), y, alpha);
                y += spacing;

                DrawStat("Strawberries:", level.Session.Strawberries.Count.ToString(), y, alpha);
                y += spacing;

                DrawStat("Lives:", LivesSystem.CurrentLives.ToString(), y, alpha);

                // Hint
                Vector2 hintPos = new Vector2(960, 900);
                ActiveFont.DrawOutline("Press any button to close", hintPos, new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.8f, Color.Gray * alpha, 2f, Color.Black * alpha);
            }

            private void DrawStat(string label, string value, float y, float alpha)
            {
                ActiveFont.DrawOutline(label, new Vector2(600, y), new Vector2(1f, 0.5f),
                    Vector2.One * 1f, Color.White * alpha, 2f, Color.Black * alpha);

                ActiveFont.DrawOutline(value, new Vector2(700, y), new Vector2(0f, 0.5f),
                    Vector2.One * 1f, Color.Cyan * alpha, 2f, Color.Black * alpha);
            }

            private string FormatTime(long ticks)
            {
                TimeSpan time = TimeSpan.FromTicks(ticks);
                return $"{time.Minutes}:{time.Seconds:D2}.{time.Milliseconds:D3}";
            }
        }

        /// <summary>
        /// D-Side progress overlay showing objectives and completion
        /// </summary>
        private class DSideProgressOverlay : Entity
        {
            private Level level;
            private float alpha = 0f;
            private bool closing = false;

            public DSideProgressOverlay(Level level)
            {
                this.level = level;
                Tag = Tags.HUD | Tags.PauseUpdate;
                Depth = -10000;
            }

            public override void Update()
            {
                base.Update();

                if (closing)
                {
                    alpha = Calc.Approach(alpha, 0f, Engine.DeltaTime * 4f);
                    if (alpha <= 0f)
                        RemoveSelf();
                }
                else
                {
                    alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime * 4f);
                }

                if (Input.MenuConfirm.Pressed || Input.MenuCancel.Pressed)
                {
                    closing = true;
                    Audio.Play("event:/ui/main/button_back");
                }
            }

            public override void Render()
            {
                base.Render();

                Draw.Rect(0, 0, 1920, 1080, Color.Black * 0.7f * alpha);

                if (alpha <= 0f)
                    return;

                Vector2 titlePos = new Vector2(960, 100);
                ActiveFont.DrawOutline("D-Side Progress", titlePos, new Vector2(0.5f, 0.5f),
                    Vector2.One * 1.5f, Color.Magenta * alpha, 2f, Color.Black * alpha);

                float y = 250f;
                string objective = GetObjective();
                
                ActiveFont.DrawOutline(objective, new Vector2(960, y), new Vector2(0.5f, 0f),
                    Vector2.One * 0.9f, Color.White * alpha, 2f, Color.Black * alpha);

                Vector2 hintPos = new Vector2(960, 900);
                ActiveFont.DrawOutline("Press any button to close", hintPos, new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.8f, Color.Gray * alpha, 2f, Color.Black * alpha);
            }

            private string GetObjective()
            {
                return "Complete all D-Side challenges\nCollect Pink Platinum Berries\nMaster advanced techniques";
            }
        }

        /// <summary>
        /// Lives info overlay explaining the lives system
        /// </summary>
        private class LivesInfoOverlay : Entity
        {
            private Level level;
            private float alpha = 0f;
            private bool closing = false;

            public LivesInfoOverlay(Level level)
            {
                this.level = level;
                Tag = Tags.HUD | Tags.PauseUpdate;
                Depth = -10000;
            }

            public override void Update()
            {
                base.Update();

                if (closing)
                {
                    alpha = Calc.Approach(alpha, 0f, Engine.DeltaTime * 4f);
                    if (alpha <= 0f)
                        RemoveSelf();
                }
                else
                {
                    alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime * 4f);
                }

                if (Input.MenuConfirm.Pressed || Input.MenuCancel.Pressed)
                {
                    closing = true;
                    Audio.Play("event:/ui/main/button_back");
                }
            }

            public override void Render()
            {
                base.Render();

                Draw.Rect(0, 0, 1920, 1080, Color.Black * 0.7f * alpha);

                if (alpha <= 0f)
                    return;

                Vector2 titlePos = new Vector2(960, 100);
                ActiveFont.DrawOutline("Lives System", titlePos, new Vector2(0.5f, 0.5f),
                    Vector2.One * 1.5f, Color.Red * alpha, 2f, Color.Black * alpha);

                float y = 250f;
                float spacing = 50f;

                int currentLives = LivesSystem.CurrentLives;
                int maxLives = LivesSystem.MaxLives;

                string[] info = new string[]
                {
                    $"Current Lives: {currentLives} / {maxLives}",
                    "",
                    "You start with 3 lives",
                    "Lose a life when you die",
                    "Game Over when you run out of lives",
                    "",
                    "Collect hearts to gain extra lives",
                    $"Maximum lives: {maxLives}",
                    "",
                    "Good luck!"
                };

                foreach (string line in info)
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        y += spacing / 2;
                        continue;
                    }

                    ActiveFont.DrawOutline(line, new Vector2(960, y), new Vector2(0.5f, 0f),
                        Vector2.One * 0.8f, Color.White * alpha, 2f, Color.Black * alpha);
                    y += spacing;
                }

                Vector2 hintPos = new Vector2(960, 900);
                ActiveFont.DrawOutline("Press any button to close", hintPos, new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.8f, Color.Gray * alpha, 2f, Color.Black * alpha);
            }
        }

        #endregion
    }
}
