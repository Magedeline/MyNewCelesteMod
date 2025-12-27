using System;
using System.Collections;
using System.Collections.Generic;
using Celeste;
using Monocle;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.DesoloZatnas.Core.UI
{
    /// <summary>
    /// Music and audio integration for custom OUI system
    /// </summary>
    public static class CustomOuiAudio
    {
        private static string currentMenuMusic = null;
        private static bool musicEnabled = true;

        // Music tracks
        public const string MENU_MAIN = "event:/Ingeste/music/menu/level_select";
        public const string MENU_STATISTICS = "event:/Ingeste/music/menu/statistics";
        public const string MENU_CREDITS = "event:/Ingeste/music/menu/credits";
        public const string GAME_OVER = "event:/Ingeste/music/gameover";
        public const string POSTCARD_UNLOCK = "event:/Ingeste/music/postcard";

        public static void PlayMenuMusic(string track)
        {
            if (!musicEnabled || string.IsNullOrEmpty(track))
                return;

            if (currentMenuMusic != track)
            {
                currentMenuMusic = track;
                Audio.SetMusic(track);
            }
        }

        public static void StopMenuMusic()
        {
            currentMenuMusic = null;
            Audio.SetMusic(null);
        }

        public static void PlaySound(string sfx)
        {
            if (!string.IsNullOrEmpty(sfx))
            {
                Audio.Play(sfx);
            }
        }

        public static void SetMusicEnabled(bool enabled)
        {
            musicEnabled = enabled;
            if (!enabled)
            {
                StopMenuMusic();
            }
        }
    }

    /// <summary>
    /// Helper class for common UI rendering operations
    /// </summary>
    public static class OuiRenderHelper
    {
        public static void DrawTextWithOutline(string text, Vector2 position, Vector2 justify, 
            float scale, Color color, float outlineThickness, Color outlineColor)
        {
            ActiveFont.DrawOutline(text, position, justify, Vector2.One * scale, 
                color, outlineThickness, outlineColor);
        }

        public static void DrawCenteredText(string text, float y, float scale, Color color)
        {
            DrawTextWithOutline(text, new Vector2(960, y), new Vector2(0.5f, 0.5f), 
                scale, color, 2f, Color.Black * 0.7f);
        }

        public static void DrawPanel(float x, float y, float width, float height, Color color)
        {
            Draw.Rect(x, y, width, height, color);
        }

        public static void DrawPanelWithBorder(float x, float y, float width, float height, 
            Color bgColor, Color borderColor, float borderWidth = 2f)
        {
            Draw.Rect(x, y, width, height, bgColor);
            Draw.HollowRect(x, y, width, height, borderColor);
        }

        public static void DrawFadeGradient(float y, float height, bool topToBottom, float alpha)
        {
            for (int i = 0; i < height; i++)
            {
                float gradientAlpha = topToBottom ? (float)i / height : 1f - ((float)i / height);
                Draw.Rect(0, y + i, 1920, 1, Color.Black * gradientAlpha * alpha);
            }
        }
    }

    /// <summary>
    /// Navigation helper for OUI screens
    /// </summary>
    public static class OuiNavigationHelper
    {
        public static void GoToMainMenu(Overworld overworld)
        {
            overworld.Goto<OuiMainMenuDesoloZatnas>();
        }

        public static void GoToChapterSelect(Overworld overworld)
        {
            overworld.Goto<OuiChapterSelectDesoloZatnas>();
        }

        public static void GoToStatistics(Overworld overworld)
        {
            overworld.Goto<OuiStatisticsNotebook>();
        }

        public static void GoToPostcards(Overworld overworld)
        {
            overworld.Goto<OuiDSidePostcard>();
        }

        public static void GoToCredits(Overworld overworld)
        {
            overworld.Goto<OuiCreditsDesoloZatnas>();
        }

        public static void GoToFileSelect(Overworld overworld)
        {
            overworld.Goto<OuiFileSelect>();
        }
    }

    /// <summary>
    /// Animation helper for smooth transitions
    /// </summary>
    public class OuiAnimationHelper
    {
        private float currentValue = 0f;
        private float targetValue = 0f;
        private float speed = 4f;

        public float Value => currentValue;

        public OuiAnimationHelper(float initialValue = 0f, float speed = 4f)
        {
            this.currentValue = initialValue;
            this.targetValue = initialValue;
            this.speed = speed;
        }

        public void SetTarget(float target)
        {
            targetValue = target;
        }

        public void Update()
        {
            currentValue = Calc.Approach(currentValue, targetValue, Engine.DeltaTime * speed);
        }

        public void SetImmediate(float value)
        {
            currentValue = value;
            targetValue = value;
        }

        public bool IsComplete => Math.Abs(currentValue - targetValue) < 0.01f;
    }

    /// <summary>
    /// Input helper for consistent button handling across OUI screens
    /// </summary>
    public static class OuiInputHelper
    {
        public static bool ConfirmPressed => Input.MenuConfirm.Pressed;
        public static bool CancelPressed => Input.MenuCancel.Pressed;
        public static bool UpPressed => Input.MenuUp.Pressed;
        public static bool DownPressed => Input.MenuDown.Pressed;
        public static bool LeftPressed => Input.MenuLeft.Pressed;
        public static bool RightPressed => Input.MenuRight.Pressed;

        public static bool UpCheck => Input.MenuUp.Check;
        public static bool DownCheck => Input.MenuDown.Check;
        public static bool LeftCheck => Input.MenuLeft.Check;
        public static bool RightCheck => Input.MenuRight.Check;

        public static void PlaySelectSound()
        {
            Audio.Play("event:/ui/main/button_select");
        }

        public static void PlayBackSound()
        {
            Audio.Play("event:/ui/main/button_back");
        }

        public static void PlayNavigateSound()
        {
            Audio.Play("event:/ui/main/rollover_up");
        }
    }
}
