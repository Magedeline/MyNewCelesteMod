using System;
using System.Collections.Generic;
using System.Linq;

namespace DesoloZantas.Core.Maps
{
    /// <summary>
    /// Handles registration and management of checkpoints for DesoloZantas chapters.
    /// Provides checkpoint data for map navigation and save/load functionality.
    /// Maps: Maps/Maggy/DesoloZantas/Chapters/XX_ChapterName_Side.bin
    /// </summary>
    public static class CheckpointRegistration
    {
        /// <summary>
        /// Checkpoint information with position and metadata.
        /// </summary>
        public class CheckpointInfo
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public int Order { get; set; }
            public string RoomName { get; set; }
            public Vector2? SpawnPosition { get; set; }
            public bool IsStart { get; set; }
            public bool IsEnd { get; set; }
            public string RequiredFlag { get; set; }
            public string[] UnlocksFlags { get; set; }

            public CheckpointInfo()
            {
                UnlocksFlags = Array.Empty<string>();
            }

            public CheckpointInfo(string name, int order, bool isStart = false, bool isEnd = false)
            {
                Name = name;
                DisplayName = FormatDisplayName(name);
                Order = order;
                IsStart = isStart;
                IsEnd = isEnd;
                UnlocksFlags = Array.Empty<string>();
            }

            private static string FormatDisplayName(string name)
            {
                // Convert snake_case to Title Case
                if (string.IsNullOrEmpty(name)) return name;
                return string.Join(" ", name.Split('_')
                    .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
            }
        }

        /// <summary>
        /// Chapter checkpoint collection with side support.
        /// </summary>
        public class ChapterCheckpoints
        {
            public int ChapterId { get; set; }
            public string ChapterName { get; set; }
            public Dictionary<string, List<CheckpointInfo>> SideCheckpoints { get; set; }

            public ChapterCheckpoints(int chapterId, string chapterName)
            {
                ChapterId = chapterId;
                ChapterName = chapterName;
                SideCheckpoints = new Dictionary<string, List<CheckpointInfo>>
                {
                    { "A", new List<CheckpointInfo>() },
                    { "B", new List<CheckpointInfo>() },
                    { "C", new List<CheckpointInfo>() },
                    { "D", new List<CheckpointInfo>() }
                };
            }

            public List<CheckpointInfo> GetCheckpoints(string side = "A")
            {
                return SideCheckpoints.TryGetValue(side, out var checkpoints)
                    ? checkpoints
                    : new List<CheckpointInfo>();
            }

            /// <summary>
            /// Get the map file path for this chapter and side.
            /// </summary>
            public string GetMapPath(string side = "A")
            {
                // Story chapters don't have sides
                if (ChapterId == 0 || ChapterId == 17 || ChapterId == 19 || ChapterId == 20 || ChapterId == 21)
                {
                    return $"Maggy/DesoloZantas/Chapters/{ChapterId:D2}_{ChapterName}";
                }
                return $"Maggy/DesoloZantas/Chapters/{ChapterId:D2}_{ChapterName}_{side}";
            }
        }

        private static readonly Dictionary<int, ChapterCheckpoints> RegisteredCheckpoints = new();

        /// <summary>
        /// Register checkpoints for a chapter (A-Side by default).
        /// </summary>
        public static void RegisterCheckpoints(int chapterId, string chapterName, string[] checkpointNames)
        {
            RegisterCheckpoints(chapterId, chapterName, "A", checkpointNames);
        }

        /// <summary>
        /// Register checkpoints for a specific side of a chapter.
        /// </summary>
        public static void RegisterCheckpoints(int chapterId, string chapterName, string side, string[] checkpointNames)
        {
            if (!RegisteredCheckpoints.TryGetValue(chapterId, out var chapter))
            {
                chapter = new ChapterCheckpoints(chapterId, chapterName);
                RegisteredCheckpoints[chapterId] = chapter;
            }

            var checkpoints = new List<CheckpointInfo>();
            for (int i = 0; i < checkpointNames.Length; i++)
            {
                checkpoints.Add(new CheckpointInfo(
                    checkpointNames[i],
                    i,
                    isStart: i == 0,
                    isEnd: i == checkpointNames.Length - 1
                ));
            }

            chapter.SideCheckpoints[side] = checkpoints;

            Logger.Log(LogLevel.Verbose, "DesoloZantas",
                $"Registered {checkpoints.Count} checkpoints for Chapter {chapterId:D2}_{chapterName} ({side}-Side)");
        }

        /// <summary>
        /// Register checkpoints with detailed info.
        /// </summary>
        public static void RegisterCheckpoints(int chapterId, string chapterName, string side, List<CheckpointInfo> checkpoints)
        {
            if (!RegisteredCheckpoints.TryGetValue(chapterId, out var chapter))
            {
                chapter = new ChapterCheckpoints(chapterId, chapterName);
                RegisteredCheckpoints[chapterId] = chapter;
            }

            chapter.SideCheckpoints[side] = checkpoints;

            Logger.Log(LogLevel.Verbose, "DesoloZantas",
                $"Registered {checkpoints.Count} detailed checkpoints for Chapter {chapterId:D2}_{chapterName} ({side}-Side)");
        }

        /// <summary>
        /// Get all checkpoints for a chapter and side.
        /// </summary>
        public static List<CheckpointInfo> GetCheckpoints(int chapterId, string side = "A")
        {
            if (RegisteredCheckpoints.TryGetValue(chapterId, out var chapter))
            {
                return chapter.GetCheckpoints(side);
            }
            return new List<CheckpointInfo>();
        }

        /// <summary>
        /// Get checkpoint by name.
        /// </summary>
        public static CheckpointInfo GetCheckpoint(int chapterId, string checkpointName, string side = "A")
        {
            var checkpoints = GetCheckpoints(chapterId, side);
            return checkpoints.FirstOrDefault(c =>
                c.Name.Equals(checkpointName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get the starting checkpoint for a chapter.
        /// </summary>
        public static CheckpointInfo GetStartCheckpoint(int chapterId, string side = "A")
        {
            var checkpoints = GetCheckpoints(chapterId, side);
            return checkpoints.FirstOrDefault(c => c.IsStart) ?? checkpoints.FirstOrDefault();
        }

        /// <summary>
        /// Get the ending checkpoint for a chapter.
        /// </summary>
        public static CheckpointInfo GetEndCheckpoint(int chapterId, string side = "A")
        {
            var checkpoints = GetCheckpoints(chapterId, side);
            return checkpoints.FirstOrDefault(c => c.IsEnd) ?? checkpoints.LastOrDefault();
        }

        /// <summary>
        /// Get next checkpoint after the given one.
        /// </summary>
        public static CheckpointInfo GetNextCheckpoint(int chapterId, string currentCheckpoint, string side = "A")
        {
            var checkpoints = GetCheckpoints(chapterId, side);
            var current = checkpoints.FindIndex(c =>
                c.Name.Equals(currentCheckpoint, StringComparison.OrdinalIgnoreCase));

            if (current >= 0 && current < checkpoints.Count - 1)
            {
                return checkpoints[current + 1];
            }
            return null;
        }

        /// <summary>
        /// Get previous checkpoint before the given one.
        /// </summary>
        public static CheckpointInfo GetPreviousCheckpoint(int chapterId, string currentCheckpoint, string side = "A")
        {
            var checkpoints = GetCheckpoints(chapterId, side);
            var current = checkpoints.FindIndex(c =>
                c.Name.Equals(currentCheckpoint, StringComparison.OrdinalIgnoreCase));

            if (current > 0)
            {
                return checkpoints[current - 1];
            }
            return null;
        }

        /// <summary>
        /// Check if a checkpoint exists.
        /// </summary>
        public static bool HasCheckpoint(int chapterId, string checkpointName, string side = "A")
        {
            return GetCheckpoint(chapterId, checkpointName, side) != null;
        }

        /// <summary>
        /// Get total checkpoint count for a chapter side.
        /// </summary>
        public static int GetCheckpointCount(int chapterId, string side = "A")
        {
            return GetCheckpoints(chapterId, side).Count;
        }

        /// <summary>
        /// Get the chapter checkpoints container.
        /// </summary>
        public static ChapterCheckpoints GetChapterCheckpoints(int chapterId)
        {
            return RegisteredCheckpoints.TryGetValue(chapterId, out var chapter) ? chapter : null;
        }

        /// <summary>
        /// Register all default DesoloZantas chapter checkpoints.
        /// This is called during initialization if not loading from YAML.
        /// </summary>
        public static void RegisterDefaultCheckpoints()
        {
            // Chapter 00 - Prologue
            RegisterCheckpoints(0, "Prologue", new[] { "start", "intro", "tutorial", "end" });

            // Chapter 01 - City
            RegisterCheckpoints(1, "City", new[] { "start", "streets", "rooftops", "downtown", "end" });

            // Chapter 02 - Nightmare
            RegisterCheckpoints(2, "Nightmare", new[] { "start", "descent", "shadow_realm", "terror", "awakening" });

            // Chapter 03 - Stars
            RegisterCheckpoints(3, "Stars", new[] { "start", "observatory", "cosmos", "constellation", "celestial" });

            // Chapter 04 - Legend
            RegisterCheckpoints(4, "Legend", new[] { "start", "ancient_halls", "trials", "prophecy", "legend_end" });

            // Chapter 05 - Restore
            RegisterCheckpoints(5, "Restore", new[] { "start", "healing_grove", "rebirth", "renewal", "restored" });

            // Chapter 06 - Stronghold
            RegisterCheckpoints(6, "Stronghold", new[] { "start", "outer_walls", "inner_keep", "throne_room", "fortress_peak" });

            // Chapter 07 - Hell
            RegisterCheckpoints(7, "Hell", new[] { "start", "inferno_gates", "lava_pits", "demon_lair", "abyss" });

            // Chapter 08 - Truth
            RegisterCheckpoints(8, "Truth", new[] { "start", "mirror_hall", "revelation", "acceptance", "truth_end" });

            // Chapter 09 - Summit
            RegisterCheckpoints(9, "Summit", new[] { "start", "base_camp", "cliffside", "ice_wall", "peak" });

            // Chapter 10 - Ruins
            RegisterCheckpoints(10, "Ruins", new[] { "start", "ancient_entrance", "collapsed_halls", "buried_treasure", "ruins_heart" });

            // Chapter 11 - Snow
            RegisterCheckpoints(11, "Snow", new[] { "start", "tundra", "blizzard", "ice_caves", "frozen_peak" });

            // Chapter 12 - Water
            RegisterCheckpoints(12, "Water", new[] { "start", "shallows", "deep_dive", "underwater_temple", "abyss_floor" });

            // Chapter 13 - Time
            RegisterCheckpoints(13, "Time", new[] { "start", "clock_tower", "temporal_rift", "paradox", "eternity" });

            // Chapter 14 - Digital
            RegisterCheckpoints(14, "Digital", new[] { "start", "data_stream", "firewall", "core_access", "system_heart" });

            // Chapter 15 - Castle
            RegisterCheckpoints(15, "Castle", new[] { "start", "courtyard", "grand_hall", "tower_ascent", "throne" });

            // Chapter 16 - Corruption
            RegisterCheckpoints(16, "Corruption", new[] { "start", "infection", "spread", "core", "purification" });

            // Chapter 17 - Epilogue
            RegisterCheckpoints(17, "Epilogue", new[] { "start", "reflection", "farewell", "end" });

            // Chapter 18 - Heart (DLC)
            RegisterCheckpoints(18, "Heart", new[] { "start", "pulse", "core", "soul", "heart_end" });

            // Chapter 19 - Space (DLC)
            RegisterCheckpoints(19, "Space", new[] { "start", "launch", "orbit", "nebula", "cosmos" });

            // Chapter 20 - The End (DLC)
            RegisterCheckpoints(20, "TheEnd", new[] { "start", "remix_gauntlet", "speedrun_arena", "expert_challenge", "finale" });

            // Chapter 21 - Post Epilogue
            RegisterCheckpoints(21, "PostEpilogue", new[] { "start", "secret_path", "revelation", "true_ending" });

            Logger.Log(LogLevel.Info, "DesoloZantas", "Registered default checkpoints for all 22 chapters (00-21)");
        }

        /// <summary>
        /// Clear all registered checkpoints.
        /// </summary>
        public static void Clear()
        {
            RegisteredCheckpoints.Clear();
        }
    }
}
