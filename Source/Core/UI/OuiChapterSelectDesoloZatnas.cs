using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.DesoloZatnas.Core.UI
{
    /// <summary>
    /// Chapter selection UI for custom DesoloZatnas areas (10-18 + DLC)
    /// Supports submap/stage selection and lobby-style navigation
    /// Groups maps by 4 per group with combined time display
    /// </summary>
    public class OuiChapterSelectDesoloZatnas : Oui
    {
        private const int FIRST_CUSTOM_AREA = 0;
        private const int LAST_CUSTOM_AREA = 18;
        private const int DLC_AREA_START = 19;
        private const int DLC_AREA_COUNT = 3;
        private const int MAPS_PER_GROUP = 4;

        private List<AreaData> customAreas = new List<AreaData>();
        private List<AreaData> dlcAreas = new List<AreaData>();
        private List<MapGroup> mapGroups = new List<MapGroup>();
        private int selectedGroup = 0;
        private int selectedMapInGroup = 0;
        private int selectedArea = 0;
        private int selectedSubmap = 0;
        private bool inSubmapSelect = false;
        private bool inGroupSelect = true; // Start in group selection mode
        
        private float scrollPosition = 0f;
        private float targetScrollPosition = 0f;
        private float fadeAlpha = 1f;

        // Lobby mode
        private bool inLobbyMode = false;
        private List<StageInfo> lobbyStages = new List<StageInfo>();

        /// <summary>
        /// Represents a group of 4 maps with combined time tracking
        /// </summary>
        private class MapGroup
        {
            public string GroupName { get; set; }
            public List<AreaData> Maps { get; set; } = new List<AreaData>();
            public int GroupIndex { get; set; }

            /// <summary>
            /// Gets the total time spent on all maps in this group
            /// </summary>
            public long GetTotalTime()
            {
                long totalTicks = 0;
                SaveData saveData = SaveData.Instance;
                if (saveData == null) return 0;

                foreach (var map in Maps)
                {
                    if (map.ID < saveData.Areas_Safe.Count)
                    {
                        AreaStats stats = saveData.Areas_Safe[map.ID];
                        totalTicks += stats.TotalTimePlayed;
                    }
                }
                return totalTicks;
            }

            /// <summary>
            /// Formats the total time as a readable string (HH:MM:SS)
            /// </summary>
            public string GetFormattedTime()
            {
                long ticks = GetTotalTime();
                TimeSpan time = TimeSpan.FromTicks(ticks);
                if (time.TotalHours >= 1)
                    return string.Format("{0:D}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
                else
                    return string.Format("{0:D}:{1:D2}", time.Minutes, time.Seconds);
            }

            /// <summary>
            /// Gets the total strawberry count for the group
            /// </summary>
            public (int collected, int total) GetStrawberryCount()
            {
                int collected = 0;
                int total = 0;
                SaveData saveData = SaveData.Instance;

                foreach (var map in Maps)
                {
                    foreach (var mode in map.Mode)
                    {
                        if (mode != null)
                            total += mode.TotalStrawberries;
                    }
                    if (saveData != null && map.ID < saveData.Areas_Safe.Count)
                    {
                        collected += saveData.Areas_Safe[map.ID].TotalStrawberries;
                    }
                }
                return (collected, total);
            }

            /// <summary>
            /// Checks if all maps in the group are completed (A-Side)
            /// </summary>
            public bool IsCompleted()
            {
                SaveData saveData = SaveData.Instance;
                if (saveData == null) return false;

                foreach (var map in Maps)
                {
                    if (map.ID >= saveData.Areas_Safe.Count)
                        return false;
                    var areaStats = saveData.Areas_Safe[map.ID];
                    if (areaStats?.Modes == null || areaStats.Modes.Length == 0 || !areaStats.Modes[0].Completed)
                        return false;
                }
                return true;
            }
        }

        public override bool IsStart(Overworld overworld, Overworld.StartMode start)
        {
            // This chapter select should not auto-start on any startup mode
            // It will be shown via explicit Overworld.Goto<OuiChapterSelectDesoloZatnas>() calls
            return false;
        }

        public override IEnumerator Enter(Oui from)
        {
            Visible = true;
            Focused = false;
            LoadCustomAreas();
            BuildMapGroups();
            selectedGroup = 0;
            selectedMapInGroup = 0;
            selectedArea = 0;
            selectedSubmap = 0;
            inSubmapSelect = false;
            inLobbyMode = false;
            inGroupSelect = true;
            
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

        private void LoadCustomAreas()
        {
            customAreas.Clear();
            dlcAreas.Clear();

            // Load custom chapters 10-18
            for (int i = FIRST_CUSTOM_AREA; i <= LAST_CUSTOM_AREA; i++)
            {
                if (i < AreaData.Areas.Count)
                {
                    AreaData area = AreaData.Get(i);
                    if (area != null)
                    {
                        customAreas.Add(area);
                    }
                }
            }

            // Load DLC areas
            for (int i = 0; i < DLC_AREA_COUNT; i++)
            {
                int areaID = DLC_AREA_START + i;
                if (areaID < AreaData.Areas.Count)
                {
                    AreaData area = AreaData.Get(areaID);
                    if (area != null)
                    {
                        dlcAreas.Add(area);
                    }
                }
            }
        }

        /// <summary>
        /// Builds map groups by grouping 4 maps together
        /// </summary>
        private void BuildMapGroups()
        {
            mapGroups.Clear();
            
            List<AreaData> allMaps = new List<AreaData>(customAreas);
            // Optionally add DLC to groups or keep separate
            // allMaps.AddRange(dlcAreas);

            int groupIndex = 0;
            for (int i = 0; i < allMaps.Count; i += MAPS_PER_GROUP)
            {
                MapGroup group = new MapGroup
                {
                    GroupIndex = groupIndex,
                    GroupName = GetGroupName(groupIndex, allMaps, i)
                };

                // Add up to 4 maps to this group
                for (int j = 0; j < MAPS_PER_GROUP && (i + j) < allMaps.Count; j++)
                {
                    group.Maps.Add(allMaps[i + j]);
                }

                mapGroups.Add(group);
                groupIndex++;
            }

            // Add DLC as a separate group if there are any
            if (dlcAreas.Count > 0)
            {
                MapGroup dlcGroup = new MapGroup
                {
                    GroupIndex = groupIndex,
                    GroupName = "Final DLC Content"
                };
                dlcGroup.Maps.AddRange(dlcAreas);
                mapGroups.Add(dlcGroup);
            }
        }

        /// <summary>
        /// Gets a descriptive name for the group based on its first map
        /// </summary>
        private string GetGroupName(int groupIndex, List<AreaData> maps, int startIndex)
        {
            if (startIndex < maps.Count)
            {
                string firstName = Dialog.Clean(maps[startIndex].Name ?? $"Chapter {maps[startIndex].ID}");
                // Extract the chapter name without the level number suffix if present
                return $"Group {groupIndex + 1}: {firstName}";
            }
            return $"Group {groupIndex + 1}";
        }

        public override void Update()
        {
            if (!Focused)
                return;
                
            base.Update();

            // Smooth scrolling
            scrollPosition = Calc.Approach(scrollPosition, targetScrollPosition, Engine.DeltaTime * 800f);

            if (inLobbyMode)
            {
                UpdateLobbyMode();
            }
            else if (inSubmapSelect)
            {
                UpdateSubmapSelect();
            }
            else if (inGroupSelect)
            {
                UpdateGroupSelect();
            }
            else
            {
                UpdateMainSelect();
            }
        }

        /// <summary>
        /// Updates group selection mode - navigate between groups of 4 maps
        /// </summary>
        private void UpdateGroupSelect()
        {
            if (mapGroups.Count == 0) return;

            if (Input.MenuUp.Pressed && selectedGroup > 0)
            {
                selectedGroup--;
                targetScrollPosition = selectedGroup * 120f;
                Audio.Play("event:/ui/main/rollover_up");
            }
            else if (Input.MenuDown.Pressed && selectedGroup < mapGroups.Count - 1)
            {
                selectedGroup++;
                targetScrollPosition = selectedGroup * 120f;
                Audio.Play("event:/ui/main/rollover_down");
            }

            if (Input.MenuConfirm.Pressed)
            {
                // Enter the selected group to choose individual maps
                inGroupSelect = false;
                selectedMapInGroup = 0;
                Audio.Play("event:/ui/main/button_select");
            }

            if (Input.MenuCancel.Pressed)
            {
                Audio.Play("event:/ui/main/button_back");
                Overworld.Goto<OuiMainMenuDesoloZatnas>();
            }
        }

        private void UpdateMainSelect()
        {
            // Navigate within a group's maps
            MapGroup currentGroup = mapGroups[selectedGroup];
            
            if (Input.MenuUp.Pressed && selectedMapInGroup > 0)
            {
                selectedMapInGroup--;
                Audio.Play("event:/ui/main/rollover_up");
            }
            else if (Input.MenuDown.Pressed && selectedMapInGroup < currentGroup.Maps.Count - 1)
            {
                selectedMapInGroup++;
                Audio.Play("event:/ui/main/rollover_down");
            }

            if (Input.MenuConfirm.Pressed)
            {
                // Check if this area has submaps
                AreaData selectedAreaData = currentGroup.Maps[selectedMapInGroup];
                if (selectedAreaData != null)
                {
                    // Enter lobby/submap mode
                    inLobbyMode = true;
                    LoadLobbyStages(selectedAreaData);
                    Audio.Play("event:/ui/main/button_select");
                }
            }

            if (Input.MenuCancel.Pressed)
            {
                // Go back to group selection
                inGroupSelect = true;
                selectedMapInGroup = 0;
                Audio.Play("event:/ui/main/button_back");
            }
        }

        private void UpdateSubmapSelect()
        {
            // Navigate through submaps within a chapter
            if (Input.MenuUp.Pressed && selectedSubmap > 0)
            {
                selectedSubmap--;
                Audio.Play("event:/ui/main/rollover_up");
            }
            else if (Input.MenuDown.Pressed)
            {
                // Check if more submaps exist
                selectedSubmap++;
                Audio.Play("event:/ui/main/rollover_down");
            }

            if (Input.MenuConfirm.Pressed)
            {
                // Launch the selected submap
                LaunchSelectedStage();
            }

            if (Input.MenuCancel.Pressed)
            {
                inSubmapSelect = false;
                inLobbyMode = false;
                Audio.Play("event:/ui/main/button_back");
            }
        }

        private void UpdateLobbyMode()
        {
            if (lobbyStages.Count == 0)
            {
                inLobbyMode = false;
                return;
            }

            if (Input.MenuUp.Pressed && selectedSubmap > 0)
            {
                selectedSubmap--;
                Audio.Play("event:/ui/main/rollover_up");
            }
            else if (Input.MenuDown.Pressed && selectedSubmap < lobbyStages.Count - 1)
            {
                selectedSubmap++;
                Audio.Play("event:/ui/main/rollover_down");
            }

            if (Input.MenuConfirm.Pressed)
            {
                LaunchSelectedStage();
            }

            if (Input.MenuCancel.Pressed)
            {
                inLobbyMode = false;
                selectedSubmap = 0;
                Audio.Play("event:/ui/main/button_back");
            }
        }

        private AreaData GetSelectedAreaData()
        {
            if (mapGroups.Count == 0 || selectedGroup >= mapGroups.Count)
                return null;
            
            MapGroup group = mapGroups[selectedGroup];
            if (selectedMapInGroup < group.Maps.Count)
            {
                return group.Maps[selectedMapInGroup];
            }
            return null;
        }

        private void LoadLobbyStages(AreaData area)
        {
            lobbyStages.Clear();
            selectedSubmap = 0;

            // Create stage entries for each mode (A-Side, B-Side, C-Side, D-Side if unlocked)
            if (area.HasMode(AreaMode.Normal))
            {
                lobbyStages.Add(new StageInfo
                {
                    AreaKey = new AreaKey(area.ID, AreaMode.Normal),
                    Name = Dialog.Clean(area.Name) + " - A-Side",
                    Icon = "A"
                });
            }

            if (area.HasMode(AreaMode.BSide))
            {
                lobbyStages.Add(new StageInfo
                {
                    AreaKey = new AreaKey(area.ID, AreaMode.BSide),
                    Name = Dialog.Clean(area.Name) + " - B-Side",
                    Icon = "B"
                });
            }

            if (area.HasMode(AreaMode.CSide))
            {
                lobbyStages.Add(new StageInfo
                {
                    AreaKey = new AreaKey(area.ID, AreaMode.CSide),
                    Name = Dialog.Clean(area.Name) + " - C-Side",
                    Icon = "C"
                });
            }

            // Check for D-Side unlock (will be implemented with postcard system)
            if (IsDSideUnlocked(area.ID))
            {
                lobbyStages.Add(new StageInfo
                {
                    AreaKey = new AreaKey(area.ID, (AreaMode)3), // D-Side custom mode
                    Name = Dialog.Clean(area.Name) + " - D-Side",
                    Icon = "D",
                    IsSpecial = true
                });
            }
        }

        private bool IsDSideUnlocked(int areaID)
        {
            // Check if player has collected all A, B, C side hearts
            // This will be connected to the postcard unlock system
            SaveData saveData = SaveData.Instance;
            if (saveData == null) return false;

            AreaStats stats = saveData.Areas_Safe[areaID];
            if (stats?.Modes == null) return false;
            
            bool hasASideHeart = stats.Modes.Length > 0 && stats.Modes[0].HeartGem;
            bool hasBSideHeart = stats.Modes.Length > 1 && stats.Modes[1].HeartGem;
            bool hasCSideHeart = stats.Modes.Length > 2 && stats.Modes[2].HeartGem;

            return hasASideHeart && hasBSideHeart && hasCSideHeart;
        }

        private void LaunchSelectedStage()
        {
            if (selectedSubmap < 0 || selectedSubmap >= lobbyStages.Count)
                return;

            StageInfo stage = lobbyStages[selectedSubmap];
            Audio.Play("event:/ui/main/button_select");

            // Launch the level
            SaveData saveData = SaveData.Instance;
            if (saveData != null)
            {
                saveData.LastArea_Safe = stage.AreaKey;
                Overworld.Goto<OuiChapterPanel>();
            }
        }

        public override void Render()
        {
            base.Render();

            if (fadeAlpha <= 0f)
                return;

            // Background
            Draw.Rect(0, 0, 1920, 1080, Color.Black * 0.7f * fadeAlpha);

            // Title
            string title = inLobbyMode ? "Select Stage" : (inGroupSelect ? "Select Map Group" : "Select Map");
            ActiveFont.DrawOutline(title, new Vector2(960, 80), new Vector2(0.5f, 0.5f),
                Vector2.One * 2f, Color.White * fadeAlpha, 2f, Color.Black * fadeAlpha);

            if (inLobbyMode)
            {
                RenderLobbyMode();
            }
            else if (inGroupSelect)
            {
                RenderGroupList();
            }
            else
            {
                RenderMapList();
            }
        }

        /// <summary>
        /// Renders the list of map groups (4 maps per group) with combined times
        /// </summary>
        private void RenderGroupList()
        {
            float startY = 180f - scrollPosition;
            float spacing = 120f;

            for (int i = 0; i < mapGroups.Count; i++)
            {
                MapGroup group = mapGroups[i];
                bool isSelected = i == selectedGroup;
                float yPos = startY + i * spacing;

                if (yPos < 80 || yPos > 1000)
                    continue; // Off-screen culling

                RenderGroupEntry(group, yPos, isSelected);
            }
        }

        /// <summary>
        /// Renders a single group entry with time and strawberry info
        /// </summary>
        private void RenderGroupEntry(MapGroup group, float yPos, bool isSelected)
        {
            Vector2 position = new Vector2(960, yPos);
            Color color = isSelected ? Color.Yellow : Color.White;
            Color timeColor = isSelected ? Color.Cyan : Color.LightBlue;
            float scale = isSelected ? 1.3f : 1f;

            // Group name
            ActiveFont.DrawOutline(group.GroupName, position, new Vector2(0.5f, 0.5f),
                Vector2.One * scale, color * fadeAlpha, 2f, Color.Black * fadeAlpha);

            // Map count indicator
            string mapCount = $"({group.Maps.Count} maps)";
            ActiveFont.DrawOutline(mapCount, position + new Vector2(0, 35), new Vector2(0.5f, 0.5f),
                Vector2.One * 0.7f, Color.Gray * fadeAlpha, 1f, Color.Black * fadeAlpha);

            // Total time for group
            string timeDisplay = $"⏱ {group.GetFormattedTime()}";
            ActiveFont.DrawOutline(timeDisplay, position + new Vector2(380, 0), new Vector2(1f, 0.5f),
                Vector2.One * scale * 0.9f, timeColor * fadeAlpha, 2f, Color.Black * fadeAlpha);

            // Strawberry count for group
            var (collected, total) = group.GetStrawberryCount();
            string berryDisplay = $"{collected}/{total}🍓";
            ActiveFont.DrawOutline(berryDisplay, position + new Vector2(380, 35), new Vector2(1f, 0.5f),
                Vector2.One * 0.7f, Color.LightGreen * fadeAlpha, 1f, Color.Black * fadeAlpha);

            // Completion indicator
            if (group.IsCompleted())
            {
                ActiveFont.DrawOutline("✓", position - new Vector2(380, 0), new Vector2(0f, 0.5f),
                    Vector2.One * scale, Color.LightGreen * fadeAlpha, 2f, Color.Black * fadeAlpha);
            }
        }

        /// <summary>
        /// Renders the individual maps within the selected group
        /// </summary>
        private void RenderMapList()
        {
            if (mapGroups.Count == 0 || selectedGroup >= mapGroups.Count)
                return;

            MapGroup group = mapGroups[selectedGroup];
            
            // Show group header
            ActiveFont.DrawOutline(group.GroupName, new Vector2(960, 140), new Vector2(0.5f, 0.5f),
                Vector2.One * 1.2f, Color.Gold * fadeAlpha, 2f, Color.Black * fadeAlpha);

            // Show group total time
            string groupTime = $"Group Time: {group.GetFormattedTime()}";
            ActiveFont.DrawOutline(groupTime, new Vector2(960, 175), new Vector2(0.5f, 0.5f),
                Vector2.One * 0.8f, Color.Cyan * fadeAlpha, 1f, Color.Black * fadeAlpha);

            float startY = 230f;
            float spacing = 80f;

            for (int i = 0; i < group.Maps.Count; i++)
            {
                AreaData map = group.Maps[i];
                bool isSelected = i == selectedMapInGroup;
                float yPos = startY + i * spacing;

                RenderMapEntry(map, i, yPos, isSelected);
            }
        }

        /// <summary>
        /// Renders an individual map entry within a group
        /// </summary>
        private void RenderMapEntry(AreaData area, int index, float yPos, bool isSelected)
        {
            if (yPos < 100 || yPos > 1000)
                return; // Off-screen culling

            Vector2 position = new Vector2(960, yPos);
            Color color = isSelected ? Color.Yellow : Color.White;
            float scale = isSelected ? 1.3f : 1f;
            
            string areaName = Dialog.Clean(area.Name ?? $"Chapter {area.ID}");
            
            // Map number within group
            string mapLabel = $"[{index + 1}]";
            ActiveFont.DrawOutline(mapLabel, position - new Vector2(350, 0), 
                new Vector2(0f, 0.5f), Vector2.One * scale, 
                color * fadeAlpha, 2f, Color.Black * fadeAlpha);
            
            // Map name
            ActiveFont.DrawOutline(areaName, position, new Vector2(0.5f, 0.5f),
                Vector2.One * scale, color * fadeAlpha, 2f, Color.Black * fadeAlpha);

            // Individual map time
            SaveData saveData = SaveData.Instance;
            if (saveData != null && area.ID < saveData.Areas_Safe.Count)
            {
                AreaStats stats = saveData.Areas_Safe[area.ID];
                
                // Time played
                TimeSpan time = TimeSpan.FromTicks(stats.TotalTimePlayed);
                string timeStr;
                if (time.TotalHours >= 1)
                    timeStr = string.Format("{0:D}:{1:D2}:{2:D2}", (int)time.TotalHours, time.Minutes, time.Seconds);
                else
                    timeStr = string.Format("{0:D}:{1:D2}", time.Minutes, time.Seconds);
                
                ActiveFont.DrawOutline(timeStr, position + new Vector2(320, 0), 
                    new Vector2(1f, 0.5f), Vector2.One * 0.8f, 
                    Color.LightBlue * fadeAlpha, 1f, Color.Black * fadeAlpha);

                // Strawberry progress
                int totalStrawberries = 0;
                foreach (var mode in area.Mode)
                {
                    if (mode != null)
                        totalStrawberries += mode.TotalStrawberries;
                }
                string progress = $"{stats.TotalStrawberries}/{totalStrawberries}🍓";
                ActiveFont.DrawOutline(progress, position + new Vector2(320, 25), 
                    new Vector2(1f, 0.5f), Vector2.One * 0.6f, 
                    Color.LightGreen * fadeAlpha, 1f, Color.Black * fadeAlpha);

                // Completion checkmark
                if (stats?.Modes != null && stats.Modes.Length > 0 && stats.Modes[0].Completed)
                {
                    ActiveFont.DrawOutline("✓", position - new Vector2(400, 0),
                        new Vector2(0.5f, 0.5f), Vector2.One * scale,
                        Color.LightGreen * fadeAlpha, 2f, Color.Black * fadeAlpha);
                }
            }
        }

        private void RenderChapterEntry(AreaData area, int index, float yPos, bool isSelected)
        {
            if (yPos < 100 || yPos > 1000)
                return; // Off-screen culling

            Vector2 position = new Vector2(960, yPos);
            Color color = isSelected ? Color.Yellow : Color.White;
            float scale = isSelected ? 1.3f : 1f;
            
            string areaName = Dialog.Clean(area.Name ?? $"Chapter {area.ID}");
            
            // Chapter number/icon
            string chapterLabel = $"[{area.ID}]";
            ActiveFont.DrawOutline(chapterLabel, position - new Vector2(400, 0), 
                new Vector2(0f, 0.5f), Vector2.One * scale, 
                color * fadeAlpha, 2f, Color.Black * fadeAlpha);
            
            // Chapter name
            ActiveFont.DrawOutline(areaName, position, new Vector2(0.5f, 0.5f),
                Vector2.One * scale, color * fadeAlpha, 2f, Color.Black * fadeAlpha);

            // Progress indicator
            SaveData saveData = SaveData.Instance;
            if (saveData != null && area.ID < saveData.Areas_Safe.Count)
            {
                AreaStats stats = saveData.Areas_Safe[area.ID];
                int totalStrawberries = 0;
                foreach (var mode in area.Mode)
                {
                    totalStrawberries += mode.TotalStrawberries;
                }
                string progress = $"{stats.TotalStrawberries}/{totalStrawberries}🍓";
                ActiveFont.DrawOutline(progress, position + new Vector2(350, 0), 
                    new Vector2(1f, 0.5f), Vector2.One * 0.8f, 
                    Color.LightGreen * fadeAlpha, 2f, Color.Black * fadeAlpha);
            }
        }

        private void RenderLobbyMode()
        {
            float startY = 250f;
            float spacing = 100f;

            for (int i = 0; i < lobbyStages.Count; i++)
            {
                StageInfo stage = lobbyStages[i];
                bool isSelected = i == selectedSubmap;
                Vector2 position = new Vector2(960, startY + i * spacing);
                
                Color color = isSelected ? Color.Cyan : Color.White;
                if (stage.IsSpecial)
                    color = isSelected ? Color.HotPink : Color.Pink;
                
                float scale = isSelected ? 1.4f : 1.1f;

                // Stage icon
                ActiveFont.DrawOutline($"[{stage.Icon}]", position - new Vector2(250, 0),
                    new Vector2(0.5f, 0.5f), Vector2.One * scale * 1.5f,
                    color * fadeAlpha, 3f, Color.Black * fadeAlpha);

                // Stage name
                ActiveFont.DrawOutline(stage.Name, position, new Vector2(0.5f, 0.5f),
                    Vector2.One * scale, color * fadeAlpha, 2f, Color.Black * fadeAlpha);

                // Completion status
                if (IsStageCompleted(stage.AreaKey))
                {
                    ActiveFont.DrawOutline("✓", position + new Vector2(300, 0),
                        new Vector2(0.5f, 0.5f), Vector2.One * scale,
                        Color.LightGreen * fadeAlpha, 2f, Color.Black * fadeAlpha);
                }
            }
        }

        private bool IsStageCompleted(AreaKey key)
        {
            SaveData saveData = SaveData.Instance;
            if (saveData == null || key.ID >= saveData.Areas_Safe.Count)
                return false;

            AreaStats stats = saveData.Areas_Safe[key.ID];
            AreaModeStats modeStats = stats.Modes[(int)key.Mode];
            
            return modeStats.Completed;
        }

        private class StageInfo
        {
            public AreaKey AreaKey { get; set; }
            public string Name { get; set; }
            public string Icon { get; set; }
            public bool IsSpecial { get; set; }
        }
    }
}
