using System;
using System.Collections;
using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.DesoloZatnas.Core.UI
{
    /// <summary>
    /// Credits screen for DesoloZatnas mod
    /// Shows mod creators, helpers, and contributors
    /// </summary>
    public class OuiCreditsDesoloZatnas : Oui
    {
        private float fadeAlpha = 1f;
        private float scrollPosition = 0f;
        private float scrollSpeed = 30f;
        private bool autoScroll = true;
        private List<CreditEntry> credits = new List<CreditEntry>();

        public override IEnumerator Enter(Oui from)
        {
            Visible = true;
            Focused = false;
            scrollPosition = 0f;
            autoScroll = true;
            InitializeCredits();
            
            // Fade in
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
            
            // Fade out
            for (float t = 0f; t < 1f; t += Engine.DeltaTime * 3f)
            {
                fadeAlpha = 1f - Ease.CubeIn(t);
                yield return null;
            }
            fadeAlpha = 0f;
        }

        private void InitializeCredits()
        {
            credits.Clear();

            // Title
            credits.Add(new CreditEntry
            {
                Type = CreditType.Title,
                Text = "DesoloZatnas",
                Color = Color.Gold
            });

            credits.Add(new CreditEntry { Type = CreditType.Spacer });

            // Main Developer
            credits.Add(new CreditEntry
            {
                Type = CreditType.Header,
                Text = "Created By",
                Color = Color.Cyan
            });
            credits.Add(new CreditEntry
            {
                Type = CreditType.Name,
                Text = "[Your Name Here]",
                Color = Color.White
            });

            credits.Add(new CreditEntry { Type = CreditType.Spacer });

            // Celeste Mod Helpers
            credits.Add(new CreditEntry
            {
                Type = CreditType.Header,
                Text = "Powered By",
                Color = Color.LightBlue
            });
            credits.Add(new CreditEntry
            {
                Type = CreditType.Name,
                Text = "Everest Mod Loader",
                Color = Color.White
            });
            credits.Add(new CreditEntry
            {
                Type = CreditType.Name,
                Text = "Celeste Mod Helpers Community",
                Color = Color.White
            });

            credits.Add(new CreditEntry { Type = CreditType.Spacer });

            // Helper Mods
            credits.Add(new CreditEntry
            {
                Type = CreditType.Header,
                Text = "Helper Mods",
                Color = Color.LightGreen
            });
            
            string[] helperMods = new string[]
            {
                "AdventureHelper",
                "AltSidesHelper",
                "BossesHelper",
                "CommunalHelper",
                "ExtendedVariantMode",
                "FrostHelper",
                "SkinModHelperPlus",
                "And many more..."
            };

            foreach (string mod in helperMods)
            {
                credits.Add(new CreditEntry
                {
                    Type = CreditType.Name,
                    Text = mod,
                    Color = Color.LightGray
                });
            }

            credits.Add(new CreditEntry { Type = CreditType.Spacer });

            // Inspiration
            credits.Add(new CreditEntry
            {
                Type = CreditType.Header,
                Text = "Inspired By",
                Color = Color.Pink
            });
            credits.Add(new CreditEntry
            {
                Type = CreditType.Name,
                Text = "Snowberry - by catapillie",
                Color = Color.White
            });
            credits.Add(new CreditEntry
            {
                Type = CreditType.Name,
                Text = "Randomizer - Celeste Community",
                Color = Color.White
            });
            credits.Add(new CreditEntry
            {
                Type = CreditType.Name,
                Text = "PICO-8 - Creative UI Design",
                Color = Color.White
            });

            credits.Add(new CreditEntry { Type = CreditType.Spacer });

            // Special Thanks
            credits.Add(new CreditEntry
            {
                Type = CreditType.Header,
                Text = "Special Thanks",
                Color = Color.Yellow
            });
            credits.Add(new CreditEntry
            {
                Type = CreditType.Name,
                Text = "Maddy Makes Games (Celeste)",
                Color = Color.White
            });
            credits.Add(new CreditEntry
            {
                Type = CreditType.Name,
                Text = "Everest Development Team",
                Color = Color.White
            });
            credits.Add(new CreditEntry
            {
                Type = CreditType.Name,
                Text = "Celeste Modding Community",
                Color = Color.White
            });
            credits.Add(new CreditEntry
            {
                Type = CreditType.Name,
                Text = "All Playtesters",
                Color = Color.White
            });

            credits.Add(new CreditEntry { Type = CreditType.Spacer });
            credits.Add(new CreditEntry { Type = CreditType.Spacer });

            // Closing message
            credits.Add(new CreditEntry
            {
                Type = CreditType.Header,
                Text = "Thank You for Playing!",
                Color = Color.Gold
            });

            credits.Add(new CreditEntry { Type = CreditType.Spacer });
            credits.Add(new CreditEntry { Type = CreditType.Spacer });
            credits.Add(new CreditEntry { Type = CreditType.Spacer });
        }

        public override void Update()
        {
            if (!Focused)
                return;
                
            base.Update();

            if (autoScroll)
            {
                scrollPosition += scrollSpeed * Engine.DeltaTime;
            }

            // Manual scroll control
            if (Input.MenuDown.Check)
            {
                autoScroll = false;
                scrollPosition += scrollSpeed * 2f * Engine.DeltaTime;
            }
            else if (Input.MenuUp.Check)
            {
                autoScroll = false;
                scrollPosition -= scrollSpeed * 2f * Engine.DeltaTime;
            }

            scrollPosition = Math.Max(0, scrollPosition);

            // Reset autoscroll if no input
            if (!Input.MenuDown.Check && !Input.MenuUp.Check && Input.MenuConfirm.Pressed)
            {
                autoScroll = true;
            }

            if (Input.MenuCancel.Pressed)
            {
                Audio.Play("event:/ui/main/button_back");
                Overworld.Goto<OuiMainMenuDesoloZatnas>();
            }
        }

        public override void Render()
        {
            base.Render();

            if (fadeAlpha <= 0f)
                return;

            // Background
            Draw.Rect(0, 0, 1920, 1080, Color.Black * 0.95f * fadeAlpha);

            // Render credits
            float startY = 900f - scrollPosition;
            float currentY = startY;

            foreach (CreditEntry entry in credits)
            {
                if (currentY > -100 && currentY < 1200)
                {
                    RenderCreditEntry(entry, currentY);
                }

                currentY += GetEntryHeight(entry);
            }

            // Gradient overlay at top and bottom
            DrawGradientOverlay();
        }

        private void RenderCreditEntry(CreditEntry entry, float y)
        {
            Vector2 position = new Vector2(960, y);
            float alpha = fadeAlpha;

            // Fade in/out at edges
            if (y < 200)
                alpha *= y / 200f;
            else if (y > 880)
                alpha *= (1080 - y) / 200f;

            alpha = Math.Max(0, Math.Min(1, alpha));

            switch (entry.Type)
            {
                case CreditType.Title:
                    ActiveFont.DrawOutline(entry.Text, position, new Vector2(0.5f, 0.5f),
                        Vector2.One * 3f, entry.Color * alpha, 4f, Color.Black * alpha);
                    break;

                case CreditType.Header:
                    ActiveFont.DrawOutline(entry.Text, position, new Vector2(0.5f, 0.5f),
                        Vector2.One * 1.5f, entry.Color * alpha, 3f, Color.Black * alpha * 0.7f);
                    break;

                case CreditType.Name:
                    ActiveFont.DrawOutline(entry.Text, position, new Vector2(0.5f, 0.5f),
                        Vector2.One * 1.1f, entry.Color * alpha, 2f, Color.Black * alpha * 0.5f);
                    break;

                case CreditType.Spacer:
                    // Just spacing, no rendering
                    break;
            }
        }

        private float GetEntryHeight(CreditEntry entry)
        {
            switch (entry.Type)
            {
                case CreditType.Title:
                    return 120f;
                case CreditType.Header:
                    return 80f;
                case CreditType.Name:
                    return 50f;
                case CreditType.Spacer:
                    return 40f;
                default:
                    return 50f;
            }
        }

        private void DrawGradientOverlay()
        {
            // Top gradient
            for (int i = 0; i < 200; i++)
            {
                float alpha = (float)i / 200f;
                Draw.Rect(0, i, 1920, 1, Color.Black * (1 - alpha) * fadeAlpha * 0.8f);
            }

            // Bottom gradient
            for (int i = 0; i < 200; i++)
            {
                float alpha = (float)i / 200f;
                Draw.Rect(0, 880 + i, 1920, 1, Color.Black * alpha * fadeAlpha * 0.8f);
            }
        }

        private enum CreditType
        {
            Title,
            Header,
            Name,
            Spacer
        }

        private class CreditEntry
        {
            public CreditType Type { get; set; }
            public string Text { get; set; }
            public Color Color { get; set; } = Color.White;
        }
    }
}
