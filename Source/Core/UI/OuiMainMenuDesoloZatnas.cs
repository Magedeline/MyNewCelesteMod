using System;
using System.Collections;
using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod;

namespace Celeste.Mod.DesoloZatnas.Core.UI
{
    /// <summary>
    /// Custom Main Menu for DesoloZatnas mod - inspired by Snowberry
    /// Provides external-style menu interface for the mod
    /// </summary>
    public class OuiMainMenuDesoloZatnas : Oui
    {
        private enum MenuState
        {
            Start,
            ChapterSelect,
            Statistics,
            Credits,
            Extras,
            Exiting
        }

        private MenuState currentState = MenuState.Start;
        private MenuState previousState = MenuState.Start;
        private float[] stateLerp = new float[6];
        private float fadeAlpha = 0f;

        // UI Elements
        private List<MenuButton> mainButtons = new List<MenuButton>();
        private TextMenu menu;
        private Vector2 titlePosition;
        private string titleText = "DesoloZatnas";
        
        // Custom music
        private string menuMusicEvent = "event:/Ingeste/music/menu/level_select";
        private bool musicStarted = false;

        public override bool IsStart(Overworld overworld, Overworld.StartMode start)
        {
            // This menu should not auto-start on any startup mode
            // It will be shown via explicit Overworld.Goto<OuiMainMenuDesoloZatnas>() calls
            return false;
        }

        public override IEnumerator Enter(Oui from)
        {
            Visible = true;
            Focused = false;
            currentState = MenuState.Start;
            fadeAlpha = 0f;
            
            // Start custom music
            if (!musicStarted)
            {
                Audio.SetMusic(menuMusicEvent);
                musicStarted = true;
            }

            // Initialize UI elements
            InitializeButtons();
            
            // Fade in animation
            for (float t = 0f; t < 1f; t += Engine.DeltaTime * 2f)
            {
                fadeAlpha = Ease.CubeOut(t);
                yield return null;
            }
            fadeAlpha = 1f;
            Focused = true;
        }

        public override IEnumerator Leave(Oui next)
        {
            Focused = false;
            Visible = false;
            
            // Fade out animation
            for (float t = 0f; t < 1f; t += Engine.DeltaTime * 3f)
            {
                fadeAlpha = 1f - Ease.CubeIn(t);
                yield return null;
            }
            
            fadeAlpha = 0f;
            musicStarted = false;
        }

        private void InitializeButtons()
        {
            mainButtons.Clear();
            
            // Create main menu buttons
            mainButtons.Add(new MenuButton
            {
                Label = Dialog.Clean("DESOLOZATNAS_MENU_CHAPTER_SELECT"),
                OnPress = () => OnChapterSelect()
            });
            
            mainButtons.Add(new MenuButton
            {
                Label = Dialog.Clean("DESOLOZATNAS_MENU_STATISTICS"),
                OnPress = () => OnStatistics()
            });
            
            mainButtons.Add(new MenuButton
            {
                Label = Dialog.Clean("DESOLOZATNAS_MENU_EXTRAS"),
                OnPress = () => OnExtras()
            });
            
            mainButtons.Add(new MenuButton
            {
                Label = Dialog.Clean("DESOLOZATNAS_MENU_CREDITS"),
                OnPress = () => OnCredits()
            });
            
            mainButtons.Add(new MenuButton
            {
                Label = Dialog.Clean("DESOLOZATNAS_MENU_EXIT"),
                OnPress = () => OnExit()
            });
        }

        public override void Update()
        {
            if (!Focused)
                return;
                
            base.Update();
            
            // Update state transitions
            for (int i = 0; i < stateLerp.Length; i++)
            {
                float target = ((int)currentState == i) ? 1f : 0f;
                stateLerp[i] = Calc.Approach(stateLerp[i], target, Engine.DeltaTime * 4f);
            }
            
            // Handle input
            if (Input.MenuCancel.Pressed && currentState != MenuState.Start)
            {
                if (currentState == MenuState.ChapterSelect || 
                    currentState == MenuState.Statistics || 
                    currentState == MenuState.Credits ||
                    currentState == MenuState.Extras)
                {
                    currentState = MenuState.Start;
                    Audio.Play("event:/ui/main/button_back");
                }
            }
        }

        public override void Render()
        {
            base.Render();
            
            if (fadeAlpha <= 0f)
                return;

            // Background
            Draw.Rect(0, 0, 1920, 1080, Color.Black * 0.8f * fadeAlpha);
            
            // Title
            Vector2 titlePos = new Vector2(960, 100);
            float titleScale = 2.5f;
            Vector2 titleSize = ActiveFont.Measure(titleText) * titleScale;
            ActiveFont.DrawOutline(titleText, titlePos, new Vector2(0.5f, 0.5f), 
                Vector2.One * titleScale, Color.White * fadeAlpha, 2f, Color.Black * fadeAlpha);
            
            // Render based on state
            switch (currentState)
            {
                case MenuState.Start:
                    RenderMainMenu();
                    break;
                case MenuState.ChapterSelect:
                    RenderChapterSelect();
                    break;
                case MenuState.Statistics:
                    RenderStatistics();
                    break;
                case MenuState.Credits:
                    RenderCredits();
                    break;
                case MenuState.Extras:
                    RenderExtras();
                    break;
            }
        }

        private void RenderMainMenu()
        {
            float startY = 300f;
            float spacing = 60f;
            int selected = 0;
            
            for (int i = 0; i < mainButtons.Count; i++)
            {
                Vector2 pos = new Vector2(960, startY + i * spacing);
                Color color = Color.White;
                float scale = 1f;
                
                // Hover effect would go here
                ActiveFont.DrawOutline(mainButtons[i].Label, pos, new Vector2(0.5f, 0.5f),
                    Vector2.One * scale, color * fadeAlpha, 2f, Color.Black * fadeAlpha);
            }
        }

        private void RenderChapterSelect()
        {
            ActiveFont.DrawOutline("Chapter Select", new Vector2(960, 200), new Vector2(0.5f, 0.5f),
                Vector2.One * 1.5f, Color.White * fadeAlpha, 2f, Color.Black * fadeAlpha);
        }

        private void RenderStatistics()
        {
            ActiveFont.DrawOutline("Statistics Notebook", new Vector2(960, 200), new Vector2(0.5f, 0.5f),
                Vector2.One * 1.5f, Color.White * fadeAlpha, 2f, Color.Black * fadeAlpha);
        }

        private void RenderCredits()
        {
            ActiveFont.DrawOutline("Credits", new Vector2(960, 200), new Vector2(0.5f, 0.5f),
                Vector2.One * 1.5f, Color.White * fadeAlpha, 2f, Color.Black * fadeAlpha);
        }

        private void RenderExtras()
        {
            ActiveFont.DrawOutline("Extras", new Vector2(960, 200), new Vector2(0.5f, 0.5f),
                Vector2.One * 1.5f, Color.White * fadeAlpha, 2f, Color.Black * fadeAlpha);
        }

        // Button callbacks
        private void OnChapterSelect()
        {
            currentState = MenuState.ChapterSelect;
            Audio.Play("event:/ui/main/button_select");
        }

        private void OnStatistics()
        {
            currentState = MenuState.Statistics;
            Audio.Play("event:/ui/main/button_select");
        }

        private void OnExtras()
        {
            currentState = MenuState.Extras;
            Audio.Play("event:/ui/main/button_select");
        }

        private void OnCredits()
        {
            currentState = MenuState.Credits;
            Audio.Play("event:/ui/main/button_select");
        }

        private void OnExit()
        {
            currentState = MenuState.Exiting;
            Audio.Play("event:/ui/main/button_back");
            Overworld.Goto<OuiFileSelect>();
        }

        private class MenuButton
        {
            public string Label { get; set; }
            public Action OnPress { get; set; }
            public bool Enabled { get; set; } = true;
        }
    }
}
