using System;
using System.Collections;
using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.DesoloZatnas.Core.UI
{
    /// <summary>
    /// D-Side Postcard System - unlocks when player collects all A/B/C-Side hearts
    /// Rewards: Heart Gem + Pink Platinum Berry
    /// </summary>
    public class OuiDSidePostcard : Oui
    {
        private int selectedArea = -1;
        private List<PostcardInfo> availablePostcards = new List<PostcardInfo>();
        private float fadeAlpha = 1f;
        private int currentPostcard = 0;
        private float cardRotation = 0f;
        private bool showingReward = false;

        public override IEnumerator Enter(Oui from)
        {
            Visible = true;
            Focused = false;
            LoadAvailablePostcards();
            currentPostcard = 0;
            showingReward = false;
            
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

        private void LoadAvailablePostcards()
        {
            availablePostcards.Clear();

            SaveData saveData = SaveData.Instance;
            if (saveData == null)
                return;

            // Check areas 10-21 for D-Side unlocks
            for (int i = 10; i <= 21; i++)
            {
                if (i >= AreaData.Areas.Count)
                    continue;

                AreaData area = AreaData.Get(i);
                if (area == null)
                    continue;

                // Check if player has all three hearts (A, B, C sides)
                if (HasAllHearts(i))
                {
                    PostcardInfo postcard = new PostcardInfo
                    {
                        AreaID = i,
                        AreaName = Dialog.Clean(area.Name),
                        IsUnlocked = true,
                        IsCompleted = IsDSideCompleted(i),
                        HasCollectedRewards = HasCollectedDSideRewards(i)
                    };
                    availablePostcards.Add(postcard);
                }
            }
        }

        private bool HasAllHearts(int areaID)
        {
            SaveData saveData = SaveData.Instance;
            if (saveData == null || areaID >= saveData.Areas_Safe.Count)
                return false;

            AreaStats stats = saveData.Areas_Safe[areaID];
            if (stats?.Modes == null || stats.Modes.Length == 0)
                return false;
            
            bool hasASide = stats.Modes[0].HeartGem;
            bool hasBSide = stats.Modes.Length > 1 && stats.Modes[1].HeartGem;
            bool hasCSide = stats.Modes.Length > 2 && stats.Modes[2].HeartGem;

            return hasASide && hasBSide && hasCSide;
        }

        private bool IsDSideCompleted(int areaID)
        {
            // Check custom save data for D-Side completion
            // This would be stored in mod save data
            if (DesoloZantas.Core.Core.IngesteModule.SaveData != null)
            {
                return DesoloZantas.Core.Core.IngesteModule.SaveData.DSideCompleted.Contains(areaID);
            }
            return false;
        }

        private bool HasCollectedDSideRewards(int areaID)
        {
            if (DesoloZantas.Core.Core.IngesteModule.SaveData != null)
            {
                return DesoloZantas.Core.Core.IngesteModule.SaveData.DSideRewardsCollected.Contains(areaID);
            }
            return false;
        }

        public override void Update()
        {
            if (!Focused)
                return;
                
            base.Update();

            if (availablePostcards.Count == 0)
            {
                if (Input.MenuCancel.Pressed)
                {
                    Audio.Play("event:/ui/main/button_back");
                    Overworld.Goto<OuiMainMenuDesoloZatnas>();
                }
                return;
            }

            cardRotation += Engine.DeltaTime * 0.5f;

            if (!showingReward)
            {
                if (Input.MenuLeft.Pressed && currentPostcard > 0)
                {
                    currentPostcard--;
                    Audio.Play("event:/ui/main/rollover_up");
                }
                else if (Input.MenuRight.Pressed && currentPostcard < availablePostcards.Count - 1)
                {
                    currentPostcard++;
                    Audio.Play("event:/ui/main/rollover_down");
                }

                if (Input.MenuConfirm.Pressed)
                {
                    PostcardInfo postcard = availablePostcards[currentPostcard];
                    if (!postcard.IsCompleted)
                    {
                        // Launch D-Side
                        LaunchDSide(postcard.AreaID);
                    }
                    else if (!postcard.HasCollectedRewards)
                    {
                        // Show reward collection
                        showingReward = true;
                        Audio.Play("event:/ui/main/button_select");
                    }
                }

                if (Input.MenuCancel.Pressed)
                {
                    Audio.Play("event:/ui/main/button_back");
                    Overworld.Goto<OuiMainMenuDesoloZatnas>();
                }
            }
            else
            {
                if (Input.MenuConfirm.Pressed || Input.MenuCancel.Pressed)
                {
                    CollectRewards(availablePostcards[currentPostcard].AreaID);
                    showingReward = false;
                    LoadAvailablePostcards();
                    Audio.Play("event:/ui/main/button_select");
                }
            }
        }

        public override void Render()
        {
            base.Render();

            if (fadeAlpha <= 0f)
                return;

            // Background
            Draw.Rect(0, 0, 1920, 1080, Color.Black * 0.85f * fadeAlpha);

            // Title
            string title = "D-Side Postcards";
            ActiveFont.DrawOutline(title, new Vector2(960, 80), new Vector2(0.5f, 0.5f),
                Vector2.One * 2f, Color.HotPink * fadeAlpha, 3f, Color.Black * fadeAlpha);

            if (availablePostcards.Count == 0)
            {
                RenderNoPostcards();
            }
            else if (showingReward)
            {
                RenderRewardCollection();
            }
            else
            {
                RenderPostcardGallery();
            }
        }

        private void RenderNoPostcards()
        {
            string message = "Collect all A, B, and C-Side hearts\nto unlock D-Side postcards!";
            ActiveFont.DrawOutline(message, new Vector2(960, 400), new Vector2(0.5f, 0.5f),
                Vector2.One * 1.2f, Color.White * fadeAlpha, 2f, Color.Black * fadeAlpha);
        }

        private void RenderPostcardGallery()
        {
            if (currentPostcard < 0 || currentPostcard >= availablePostcards.Count)
                return;

            PostcardInfo postcard = availablePostcards[currentPostcard];

            // Postcard frame
            float cardX = 960f;
            float cardY = 400f;
            float cardWidth = 600f;
            float cardHeight = 400f;

            // Draw postcard background
            Color cardColor = new Color(255, 245, 230);
            Draw.Rect(cardX - cardWidth / 2, cardY - cardHeight / 2, cardWidth, cardHeight, 
                cardColor * fadeAlpha);
            
            // Border
            Draw.HollowRect(cardX - cardWidth / 2, cardY - cardHeight / 2, cardWidth, cardHeight,
                Color.HotPink * fadeAlpha);

            // Area name
            ActiveFont.DrawOutline(postcard.AreaName, new Vector2(cardX, cardY - 140),
                new Vector2(0.5f, 0.5f), Vector2.One * 1.5f, Color.DarkSlateGray * fadeAlpha,
                2f, Color.Black * fadeAlpha * 0.3f);

            // D-Side label
            ActiveFont.DrawOutline("D-SIDE", new Vector2(cardX, cardY - 80),
                new Vector2(0.5f, 0.5f), Vector2.One * 2f, Color.HotPink * fadeAlpha,
                3f, Color.Black * fadeAlpha * 0.5f);

            // Status
            string status = postcard.IsCompleted ? "COMPLETED ✓" : "NOT COMPLETED";
            Color statusColor = postcard.IsCompleted ? Color.LightGreen : Color.Orange;
            ActiveFont.DrawOutline(status, new Vector2(cardX, cardY + 40),
                new Vector2(0.5f, 0.5f), Vector2.One * 1f, statusColor * fadeAlpha,
                2f, Color.Black * fadeAlpha * 0.3f);

            // Rewards status
            if (postcard.IsCompleted && !postcard.HasCollectedRewards)
            {
                string rewardText = "Press [Confirm] to collect rewards!";
                ActiveFont.DrawOutline(rewardText, new Vector2(cardX, cardY + 100),
                    new Vector2(0.5f, 0.5f), Vector2.One * 0.9f, Color.Gold * fadeAlpha,
                    2f, Color.Black * fadeAlpha * 0.3f);
            }
            else if (postcard.HasCollectedRewards)
            {
                string rewardText = "Rewards Collected ✓";
                ActiveFont.DrawOutline(rewardText, new Vector2(cardX, cardY + 100),
                    new Vector2(0.5f, 0.5f), Vector2.One * 0.9f, Color.LightGreen * fadeAlpha,
                    2f, Color.Black * fadeAlpha * 0.3f);
            }

            // Navigation hints
            if (availablePostcards.Count > 1)
            {
                string navText = $"[{currentPostcard + 1}/{availablePostcards.Count}] ◄ ►";
                ActiveFont.DrawOutline(navText, new Vector2(cardX, cardY + 220),
                    new Vector2(0.5f, 0.5f), Vector2.One * 0.7f, Color.White * fadeAlpha,
                    1f, Color.Black * fadeAlpha * 0.3f);
            }
        }

        private void RenderRewardCollection()
        {
            PostcardInfo postcard = availablePostcards[currentPostcard];

            // Reward display
            float centerX = 960f;
            float centerY = 450f;

            ActiveFont.DrawOutline("D-Side Rewards!", new Vector2(centerX, 200),
                new Vector2(0.5f, 0.5f), Vector2.One * 2.5f, Color.Gold * fadeAlpha,
                3f, Color.Black * fadeAlpha);

            // Heart Gem icon
            float heartY = centerY - 80;
            float rotation = (float)Math.Sin(cardRotation * 2f) * 0.2f;
            
            ActiveFont.DrawOutline("💎", new Vector2(centerX - 200, heartY),
                new Vector2(0.5f, 0.5f), Vector2.One * 3f, Color.HotPink * fadeAlpha,
                3f, Color.Black * fadeAlpha);
            ActiveFont.DrawOutline("Heart Gem", new Vector2(centerX - 200, heartY + 80),
                new Vector2(0.5f, 0.5f), Vector2.One * 1f, Color.White * fadeAlpha,
                2f, Color.Black * fadeAlpha);

            // Pink Platinum Berry
            ActiveFont.DrawOutline("🍓", new Vector2(centerX + 200, heartY),
                new Vector2(0.5f, 0.5f), Vector2.One * 3f, Color.HotPink * fadeAlpha,
                3f, Color.Black * fadeAlpha);
            ActiveFont.DrawOutline("Pink Platinum Berry", new Vector2(centerX + 200, heartY + 80),
                new Vector2(0.5f, 0.5f), Vector2.One * 1f, Color.White * fadeAlpha,
                2f, Color.Black * fadeAlpha);

            // Collect prompt
            string prompt = "Press [Confirm] to collect";
            ActiveFont.DrawOutline(prompt, new Vector2(centerX, centerY + 150),
                new Vector2(0.5f, 0.5f), Vector2.One * 1.2f, Color.Yellow * fadeAlpha,
                2f, Color.Black * fadeAlpha);
        }

        private void LaunchDSide(int areaID)
        {
            // Launch the D-Side level
            SaveData saveData = SaveData.Instance;
            if (saveData != null)
            {
                AreaKey key = new AreaKey(areaID, (AreaMode)3); // D-Side is mode 3
                saveData.LastArea_Safe = key;
                Audio.Play("event:/ui/main/button_select");
                Overworld.Goto<OuiChapterPanel>();
            }
        }

        private void CollectRewards(int areaID)
        {
            // Mark rewards as collected
            if (DesoloZantas.Core.Core.IngesteModule.SaveData != null)
            {
                DesoloZantas.Core.Core.IngesteModule.SaveData.DSideRewardsCollected.Add(areaID);
                
                // Add to player inventory
                SaveData saveData = SaveData.Instance;
                if (saveData != null && areaID < saveData.Areas_Safe.Count)
                {
                    // Award heart gem and pink platinum berry
                    // This would be tracked in custom save data
                    Audio.Play("event:/game/general/diamond_touch");
                }
            }
        }

        private class PostcardInfo
        {
            public int AreaID { get; set; }
            public string AreaName { get; set; }
            public bool IsUnlocked { get; set; }
            public bool IsCompleted { get; set; }
            public bool HasCollectedRewards { get; set; }
        }
    }
}
