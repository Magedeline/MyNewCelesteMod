using System;
using System.Collections;
using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.DesoloZatnas.Core.UI
{
    /// <summary>
    /// Statistics Notebook UI - tracks deaths, collections, enemy kills, dashes, and speedrun times
    /// </summary>
    public class OuiStatisticsNotebook : Oui
    {
        private enum NotebookPage
        {
            Overview,
            Deaths,
            Collections,
            EnemyKills,
            Dashes,
            Speedruns
        }

        private NotebookPage currentPage = NotebookPage.Overview;
        private int selectedItem = 0;
        private float fadeAlpha = 1f;
        private float pageTransitionAlpha = 1f;
        private int scrollOffset = 0;

        // Statistics data
        private DesoloZantasStatistics stats;

        public override IEnumerator Enter(Oui from)
        {
            Visible = true;
            Focused = false;
            currentPage = NotebookPage.Overview;
            selectedItem = 0;
            scrollOffset = 0;
            
            // Load statistics
            LoadStatistics();
            
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

        private void LoadStatistics()
        {
            // Get or create statistics from save data
            stats = DesoloZantasStatistics.GetOrCreate();
        }

        public override void Update()
        {
            if (!Focused)
                return;
                
            base.Update();

            // Tab navigation
            if (Input.MenuLeft.Pressed)
            {
                int newPage = (int)currentPage - 1;
                if (newPage < 0)
                    newPage = 5;
                currentPage = (NotebookPage)newPage;
                selectedItem = 0;
                scrollOffset = 0;
                Audio.Play("event:/ui/main/rollover_up");
            }
            else if (Input.MenuRight.Pressed)
            {
                int newPage = (int)currentPage + 1;
                if (newPage > 5)
                    newPage = 0;
                currentPage = (NotebookPage)newPage;
                selectedItem = 0;
                scrollOffset = 0;
                Audio.Play("event:/ui/main/rollover_down");
            }

            // List navigation
            if (Input.MenuUp.Pressed && selectedItem > 0)
            {
                selectedItem--;
                Audio.Play("event:/ui/main/rollover_up");
            }
            else if (Input.MenuDown.Pressed)
            {
                selectedItem++;
                Audio.Play("event:/ui/main/rollover_down");
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

            if (fadeAlpha <= 0f || stats == null)
                return;

            // Background - notebook style
            Draw.Rect(0, 0, 1920, 1080, Color.Black * 0.8f * fadeAlpha);
            
            // Notebook frame
            float notebookX = 200f;
            float notebookY = 100f;
            float notebookWidth = 1520f;
            float notebookHeight = 880f;
            
            Draw.Rect(notebookX, notebookY, notebookWidth, notebookHeight, 
                new Color(245, 235, 220) * fadeAlpha); // Parchment color
            
            // Notebook border
            Draw.HollowRect(notebookX, notebookY, notebookWidth, notebookHeight, 
                Color.SaddleBrown * fadeAlpha);

            // Title
            string title = "DesoloZatnas Statistics";
            ActiveFont.DrawOutline(title, new Vector2(960, notebookY + 40), new Vector2(0.5f, 0.5f),
                Vector2.One * 1.8f, Color.DarkSlateGray * fadeAlpha, 2f, Color.Black * fadeAlpha * 0.3f);

            // Tab navigation
            RenderTabs(notebookX, notebookY + 100, notebookWidth);

            // Content area
            float contentY = notebookY + 180;
            float contentHeight = notebookHeight - 180;

            switch (currentPage)
            {
                case NotebookPage.Overview:
                    RenderOverview(notebookX, contentY, notebookWidth, contentHeight);
                    break;
                case NotebookPage.Deaths:
                    RenderDeaths(notebookX, contentY, notebookWidth, contentHeight);
                    break;
                case NotebookPage.Collections:
                    RenderCollections(notebookX, contentY, notebookWidth, contentHeight);
                    break;
                case NotebookPage.EnemyKills:
                    RenderEnemyKills(notebookX, contentY, notebookWidth, contentHeight);
                    break;
                case NotebookPage.Dashes:
                    RenderDashes(notebookX, contentY, notebookWidth, contentHeight);
                    break;
                case NotebookPage.Speedruns:
                    RenderSpeedruns(notebookX, contentY, notebookWidth, contentHeight);
                    break;
            }
        }

        private void RenderTabs(float x, float y, float width)
        {
            string[] tabNames = new string[]
            {
                "Overview", "Deaths", "Collections", "Enemies", "Dashes", "Speedruns"
            };

            float tabWidth = width / tabNames.Length;
            
            for (int i = 0; i < tabNames.Length; i++)
            {
                float tabX = x + i * tabWidth;
                bool isSelected = (int)currentPage == i;
                
                Color bgColor = isSelected ? new Color(220, 200, 160) : new Color(200, 180, 140);
                Color textColor = isSelected ? Color.DarkRed : Color.DarkSlateGray;
                
                Draw.Rect(tabX + 5, y, tabWidth - 10, 50, bgColor * fadeAlpha);
                
                Vector2 textPos = new Vector2(tabX + tabWidth / 2, y + 25);
                ActiveFont.DrawOutline(tabNames[i], textPos, new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.8f, textColor * fadeAlpha, 1f, Color.Black * fadeAlpha * 0.2f);
            }
        }

        private void RenderOverview(float x, float y, float width, float height)
        {
            float lineY = y + 40;
            float lineSpacing = 50f;
            Color textColor = Color.DarkSlateGray;

            // Total playtime
            string playtime = FormatTime(stats.TotalPlaytimeSeconds);
            RenderStatLine("Total Playtime:", playtime, x + 100, lineY, textColor);
            lineY += lineSpacing;

            // Total deaths
            RenderStatLine("Total Deaths:", stats.TotalDeaths.ToString(), x + 100, lineY, textColor);
            lineY += lineSpacing;

            // Total strawberries
            RenderStatLine("Strawberries Collected:", 
                $"{stats.TotalStrawberries}/{stats.TotalStrawberriesAvailable}", 
                x + 100, lineY, textColor);
            lineY += lineSpacing;

            // Total enemy kills
            RenderStatLine("Enemies Defeated:", stats.TotalEnemyKills.ToString(), x + 100, lineY, textColor);
            lineY += lineSpacing;

            // Total dashes
            RenderStatLine("Total Dashes:", stats.TotalDashes.ToString(), x + 100, lineY, textColor);
            lineY += lineSpacing;

            // Hearts collected
            RenderStatLine("Heart Gems:", $"{stats.HeartsCollected}/{stats.TotalHeartsAvailable}", 
                x + 100, lineY, textColor);
            lineY += lineSpacing;

            // Completion percentage
            float completion = stats.CalculateCompletionPercentage();
            RenderStatLine("Completion:", $"{completion:F1}%", x + 100, lineY, Color.Gold);
        }

        private void RenderDeaths(float x, float y, float width, float height)
        {
            float lineY = y + 40;
            float lineSpacing = 45f;
            Color textColor = Color.DarkSlateGray;

            RenderStatLine("Total Deaths:", stats.TotalDeaths.ToString(), x + 100, lineY, Color.DarkRed);
            lineY += lineSpacing + 20;

            // Deaths by chapter
            if (stats.DeathsByChapter != null && stats.DeathsByChapter.Count > 0)
            {
                ActiveFont.DrawOutline("Deaths by Chapter:", new Vector2(x + 100, lineY),
                    Vector2.Zero, Vector2.One * 0.9f, textColor * fadeAlpha, 1f, Color.Black * fadeAlpha * 0.2f);
                lineY += lineSpacing;

                int index = 0;
                foreach (var kvp in stats.DeathsByChapter)
                {
                    if (index < scrollOffset)
                    {
                        index++;
                        continue;
                    }
                    
                    if (lineY > y + height - 50)
                        break;

                    string chapterName = GetChapterName(kvp.Key);
                    RenderStatLine($"  {chapterName}:", kvp.Value.ToString(), x + 120, lineY, textColor);
                    lineY += lineSpacing;
                    index++;
                }
            }
        }

        private void RenderCollections(float x, float y, float width, float height)
        {
            float lineY = y + 40;
            float lineSpacing = 45f;
            Color textColor = Color.DarkSlateGray;

            // Strawberries
            RenderStatLine("Strawberries:", 
                $"{stats.TotalStrawberries}/{stats.TotalStrawberriesAvailable}", 
                x + 100, lineY, Color.Red);
            lineY += lineSpacing;

            // Heart Gems
            RenderStatLine("Heart Gems:", 
                $"{stats.HeartsCollected}/{stats.TotalHeartsAvailable}", 
                x + 100, lineY, Color.Pink);
            lineY += lineSpacing;

            // Crystal Hearts
            RenderStatLine("Crystal Hearts:", stats.CrystalHeartsCollected.ToString(), 
                x + 100, lineY, Color.Cyan);
            lineY += lineSpacing;

            // Golden Strawberries
            RenderStatLine("Golden Strawberries:", stats.GoldenStrawberries.ToString(), 
                x + 100, lineY, Color.Gold);
            lineY += lineSpacing;

            // Pink Platinum Berries (D-Side reward)
            RenderStatLine("Pink Platinum Berries:", stats.PinkPlatinumBerries.ToString(), 
                x + 100, lineY, Color.HotPink);
            lineY += lineSpacing;

            // Cassettes
            RenderStatLine("Cassettes:", stats.CassettesCollected.ToString(), 
                x + 100, lineY, Color.Purple);
        }

        private void RenderEnemyKills(float x, float y, float width, float height)
        {
            float lineY = y + 40;
            float lineSpacing = 45f;
            Color textColor = Color.DarkSlateGray;

            RenderStatLine("Total Enemies Defeated:", stats.TotalEnemyKills.ToString(), 
                x + 100, lineY, Color.DarkRed);
            lineY += lineSpacing + 20;

            // Enemy kills by type
            if (stats.EnemyKillsByType != null && stats.EnemyKillsByType.Count > 0)
            {
                ActiveFont.DrawOutline("Enemies by Type:", new Vector2(x + 100, lineY),
                    Vector2.Zero, Vector2.One * 0.9f, textColor * fadeAlpha, 1f, Color.Black * fadeAlpha * 0.2f);
                lineY += lineSpacing;

                int index = 0;
                foreach (var kvp in stats.EnemyKillsByType)
                {
                    if (index < scrollOffset)
                    {
                        index++;
                        continue;
                    }
                    
                    if (lineY > y + height - 50)
                        break;

                    RenderStatLine($"  {kvp.Key}:", kvp.Value.ToString(), x + 120, lineY, textColor);
                    lineY += lineSpacing;
                    index++;
                }
            }
        }

        private void RenderDashes(float x, float y, float width, float height)
        {
            float lineY = y + 40;
            float lineSpacing = 45f;
            Color textColor = Color.DarkSlateGray;

            RenderStatLine("Total Dashes:", stats.TotalDashes.ToString(), x + 100, lineY, Color.Cyan);
            lineY += lineSpacing;

            RenderStatLine("Ground Dashes:", stats.GroundDashes.ToString(), x + 100, lineY, textColor);
            lineY += lineSpacing;

            RenderStatLine("Air Dashes:", stats.AirDashes.ToString(), x + 100, lineY, textColor);
            lineY += lineSpacing;

            RenderStatLine("Wavedashes:", stats.Wavedashes.ToString(), x + 100, lineY, Color.Orange);
            lineY += lineSpacing;

            RenderStatLine("Hyperdashes:", stats.Hyperdashes.ToString(), x + 100, lineY, Color.Red);
            lineY += lineSpacing;

            RenderStatLine("Super Dashes:", stats.SuperDashes.ToString(), x + 100, lineY, Color.Purple);
        }

        private void RenderSpeedruns(float x, float y, float width, float height)
        {
            float lineY = y + 40;
            float lineSpacing = 45f;
            Color textColor = Color.DarkSlateGray;

            ActiveFont.DrawOutline("Best Times:", new Vector2(x + 100, lineY),
                Vector2.Zero, Vector2.One * 1.1f, textColor * fadeAlpha, 1f, Color.Black * fadeAlpha * 0.2f);
            lineY += lineSpacing + 10;

            if (stats.BestTimesByChapter != null && stats.BestTimesByChapter.Count > 0)
            {
                int index = 0;
                foreach (var kvp in stats.BestTimesByChapter)
                {
                    if (index < scrollOffset)
                    {
                        index++;
                        continue;
                    }
                    
                    if (lineY > y + height - 50)
                        break;

                    string chapterName = GetChapterName(kvp.Key);
                    string timeStr = FormatTime(kvp.Value);
                    RenderStatLine($"{chapterName}:", timeStr, x + 120, lineY, textColor);
                    lineY += lineSpacing;
                    index++;
                }
            }
            else
            {
                ActiveFont.DrawOutline("No speedrun data yet", new Vector2(x + 120, lineY),
                    Vector2.Zero, Vector2.One * 0.8f, Color.Gray * fadeAlpha, 1f, Color.Black * fadeAlpha * 0.2f);
            }
        }

        private void RenderStatLine(string label, string value, float x, float y, Color color)
        {
            ActiveFont.DrawOutline(label, new Vector2(x, y), Vector2.Zero, Vector2.One * 0.8f,
                color * fadeAlpha, 1f, Color.Black * fadeAlpha * 0.2f);
            
            ActiveFont.DrawOutline(value, new Vector2(x + 500, y), Vector2.Zero, Vector2.One * 0.8f,
                color * fadeAlpha, 1f, Color.Black * fadeAlpha * 0.2f);
        }

        private string GetChapterName(int areaID)
        {
            if (areaID < AreaData.Areas.Count)
            {
                AreaData area = AreaData.Get(areaID);
                if (area != null)
                {
                    return Dialog.Clean(area.Name ?? $"Chapter {areaID}");
                }
            }
            return $"Chapter {areaID}";
        }

        private string FormatTime(long seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            if (time.TotalHours >= 1)
                return $"{(int)time.TotalHours}h {time.Minutes}m {time.Seconds}s";
            else if (time.TotalMinutes >= 1)
                return $"{time.Minutes}m {time.Seconds}s";
            else
                return $"{time.Seconds}s";
        }
    }

    /// <summary>
    /// Statistics data structure for DesoloZatnas
    /// </summary>
    public class DesoloZantasStatistics
    {
        public long TotalPlaytimeSeconds { get; set; }
        public int TotalDeaths { get; set; }
        public Dictionary<int, int> DeathsByChapter { get; set; } = new Dictionary<int, int>();
        
        public int TotalStrawberries { get; set; }
        public int TotalStrawberriesAvailable { get; set; }
        public int HeartsCollected { get; set; }
        public int TotalHeartsAvailable { get; set; }
        public int CrystalHeartsCollected { get; set; }
        public int GoldenStrawberries { get; set; }
        public int PinkPlatinumBerries { get; set; }
        public int CassettesCollected { get; set; }

        public int TotalEnemyKills { get; set; }
        public Dictionary<string, int> EnemyKillsByType { get; set; } = new Dictionary<string, int>();

        public int TotalDashes { get; set; }
        public int GroundDashes { get; set; }
        public int AirDashes { get; set; }
        public int Wavedashes { get; set; }
        public int Hyperdashes { get; set; }
        public int SuperDashes { get; set; }

        public Dictionary<int, long> BestTimesByChapter { get; set; } = new Dictionary<int, long>();

        public static DesoloZantasStatistics GetOrCreate()
        {
            // This would normally load from save data
            // For now, create a sample instance
            return new DesoloZantasStatistics
            {
                TotalPlaytimeSeconds = 3600,
                TotalDeaths = 42,
                TotalStrawberries = 15,
                TotalStrawberriesAvailable = 200,
                HeartsCollected = 3,
                TotalHeartsAvailable = 21,
                TotalEnemyKills = 50,
                TotalDashes = 1000,
                GroundDashes = 600,
                AirDashes = 400
            };
        }

        public float CalculateCompletionPercentage()
        {
            float total = 0f;
            float current = 0f;

            if (TotalStrawberriesAvailable > 0)
            {
                total += TotalStrawberriesAvailable;
                current += TotalStrawberries;
            }

            if (TotalHeartsAvailable > 0)
            {
                total += TotalHeartsAvailable;
                current += HeartsCollected;
            }

            if (total == 0)
                return 0f;

            return (current / total) * 100f;
        }
    }
}
