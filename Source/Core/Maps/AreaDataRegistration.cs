using System;
using System.Collections.Generic;
using System.Linq;

namespace DesoloZantas.Core.Maps
{
    /// <summary>
    /// Handles registration and management of custom area data for DesoloZantas campaign.
    /// Maps chapters 00-21 in Maps/Maggy/DesoloZantas/Chapters/
    /// </summary>
    public static class AreaDataRegistration
    {
        private static readonly Dictionary<int, AreaRegistrationInfo> RegisteredAreas = new();
        private static bool _initialized = false;

        /// <summary>
        /// Chapter type classification.
        /// </summary>
        public enum ChapterType
        {
            Prologue,       // 00_Prologue
            Main,           // 01-16 (with A/B/C/D sides)
            Epilogue,       // 17_Epilogue
            DLC,            // 18-20
            PostEpilogue    // 21_PostEpilogue
        }

        /// <summary>
        /// Area registration information containing all necessary data for a custom area.
        /// </summary>
        public class AreaRegistrationInfo
        {
            public int ChapterId { get; set; }
            public string ChapterName { get; set; }
            public string MapPath { get; set; }
            public string DisplayName { get; set; }
            public string Icon { get; set; }
            public string TitleBase { get; set; }
            public string TitleAccentColor { get; set; }
            public string TitleTextColor { get; set; }
            public ChapterType Type { get; set; }
            public bool HasASide { get; set; }
            public bool HasBSide { get; set; }
            public bool HasCSide { get; set; }
            public bool HasDSide { get; set; }
            public bool IsInterlude { get; set; }
            public string[] Checkpoints { get; set; }
            public string IntroType { get; set; }
            public string ColorGrade { get; set; }
            public string Wipe { get; set; }
            public float DarknessAlpha { get; set; }
            public float BloomBase { get; set; }
            public float BloomStrength { get; set; }
            public string Music { get; set; }
            public string Ambience { get; set; }
            public string Description { get; set; }

            public AreaRegistrationInfo()
            {
                Checkpoints = Array.Empty<string>();
                IntroType = "WalkInRight";
                ColorGrade = "none";
                Wipe = "Curtain";
                DarknessAlpha = 0.05f;
                BloomBase = 0f;
                BloomStrength = 1f;
                HasASide = true;
            }

            /// <summary>
            /// Get the map file path for a specific side.
            /// </summary>
            public string GetMapFile(string side = "A")
            {
                if (Type == ChapterType.Prologue || Type == ChapterType.Epilogue || Type == ChapterType.PostEpilogue)
                {
                    return $"{MapPath}/{ChapterId:D2}_{ChapterName}.bin";
                }
                return $"{MapPath}/{ChapterId:D2}_{ChapterName}_{side}.bin";
            }

            /// <summary>
            /// Get available sides for this chapter.
            /// </summary>
            public string[] GetAvailableSides()
            {
                var sides = new List<string>();
                if (HasASide) sides.Add("A");
                if (HasBSide) sides.Add("B");
                if (HasCSide) sides.Add("C");
                if (HasDSide) sides.Add("D");
                return sides.ToArray();
            }
        }

        /// <summary>
        /// Initialize and register all DesoloZantas areas.
        /// Called during module load.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;

            RegisterPrologue();
            RegisterMainChapters();
            RegisterEpilogue();
            RegisterDLCChapters();
            RegisterPostEpilogue();

            _initialized = true;
            Logger.Log(LogLevel.Info, "DesoloZantas", $"Registered {RegisteredAreas.Count} custom areas (Chapters 00-21)");
        }

        /// <summary>
        /// Register Prologue (Chapter 00).
        /// </summary>
        private static void RegisterPrologue()
        {
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 0,
                ChapterName = "Prologue",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Prologue",
                Icon = "areas/desolozantas/prologue",
                TitleBase = "PROLOGUE",
                TitleAccentColor = "667da5",
                TitleTextColor = "ffffff",
                Type = ChapterType.Prologue,
                IsInterlude = true,
                HasASide = false,
                HasBSide = false,
                HasCSide = false,
                HasDSide = false,
                Checkpoints = new[] { "start", "intro", "tutorial", "end" },
                Music = "event:/Ingeste/music/lvl0/intro",
                Ambience = "event:/Ingeste/env/00_prologue",
                Description = "Introduction and tutorial"
            });
        }

        /// <summary>
        /// Register main campaign chapters (01-16).
        /// </summary>
        private static void RegisterMainChapters()
        {
            // Chapter 01 - City
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 1,
                ChapterName = "City",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "City",
                Icon = "areas/desolozantas/city",
                TitleBase = "CITY",
                TitleAccentColor = "4a7c9e",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "streets", "rooftops", "downtown", "end" },
                Music = "event:/Ingeste/music/lvl1/main",
                Ambience = "event:/Ingeste/env/01_main",
                ColorGrade = "city",
                Description = "Urban environment"
            });

            // Chapter 02 - Nightmare
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 2,
                ChapterName = "Nightmare",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Nightmare",
                Icon = "areas/desolozantas/nightmare",
                TitleBase = "NIGHTMARE",
                TitleAccentColor = "8b0000",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "descent", "shadow_realm", "terror", "awakening" },
                Music = "event:/Ingeste/music/lvl2/main",
                Ambience = "event:/Ingeste/env/02_nightmare",
                DarknessAlpha = 0.3f,
                ColorGrade = "templevoid",
                Description = "Dark dreamscape"
            });

            // Chapter 03 - Stars
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 3,
                ChapterName = "Stars",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Stars",
                Icon = "areas/desolozantas/stars",
                TitleBase = "STARS",
                TitleAccentColor = "ffd700",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "observatory", "cosmos", "constellation", "celestial" },
                Music = "event:/Ingeste/music/lvl3/intro",
                Ambience = "event:/Ingeste/env/03_main",
                BloomBase = 0.2f,
                BloomStrength = 1.5f,
                Description = "Celestial realm"
            });

            // Chapter 04 - Legend
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 4,
                ChapterName = "Legend",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Legend",
                Icon = "areas/desolozantas/legend",
                TitleBase = "LEGEND",
                TitleAccentColor = "cd853f",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "ancient_halls", "trials", "prophecy", "legend_end" },
                Music = "event:/Ingeste/music/lvl4/main",
                Ambience = "event:/Ingeste/env/04_dream",
                ColorGrade = "oldsite",
                Description = "Mythic journey"
            });

            // Chapter 05 - Restore
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 5,
                ChapterName = "Restore",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Restore",
                Icon = "areas/desolozantas/restore",
                TitleBase = "RESTORE",
                TitleAccentColor = "32cd32",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "healing_grove", "rebirth", "renewal", "restored" },
                Music = "event:/Ingeste/music/lvl5/intro",
                Ambience = "event:/Ingeste/env/03_main",
                ColorGrade = "feelingdown",
                Description = "Recovery chapter"
            });

            // Chapter 06 - Stronghold
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 6,
                ChapterName = "Stronghold",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Stronghold",
                Icon = "areas/desolozantas/stronghold",
                TitleBase = "STRONGHOLD",
                TitleAccentColor = "708090",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "outer_walls", "inner_keep", "throne_room", "fortress_peak" },
                Music = "event:/Ingeste/music/lvl6/main",
                Ambience = "event:/Ingeste/env/06_main",
                Description = "Fortress ascent"
            });

            // Chapter 07 - Hell
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 7,
                ChapterName = "Hell",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Hell",
                Icon = "areas/desolozantas/hell",
                TitleBase = "HELL",
                TitleAccentColor = "ff4500",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "inferno_gates", "lava_pits", "demon_lair", "abyss" },
                Music = "event:/Ingeste/music/lvl7/normal",
                Ambience = "event:/Ingeste/env/06_main",
                ColorGrade = "hot",
                DarknessAlpha = 0.1f,
                Description = "Infernal depths"
            });

            // Chapter 08 - Truth
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 8,
                ChapterName = "Truth",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Truth",
                Icon = "areas/desolozantas/truth",
                TitleBase = "TRUTH",
                TitleAccentColor = "ffffff",
                TitleTextColor = "000000",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "mirror_hall", "revelation", "acceptance", "truth_end" },
                Music = "event:/Ingeste/music/lvl8/main",
                Ambience = "event:/Ingeste/env/08_main",
                BloomBase = 0.3f,
                Description = "Revelation chapter"
            });

            // Chapter 09 - Summit
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 9,
                ChapterName = "Summit",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Summit",
                Icon = "areas/desolozantas/summit",
                TitleBase = "SUMMIT",
                TitleAccentColor = "87ceeb",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "base_camp", "cliffside", "ice_wall", "peak" },
                Music = "event:/Ingeste/music/lvl9/main",
                Ambience = "event:/Ingeste/env/06_main",
                ColorGrade = "cold",
                Description = "Mountain climb"
            });

            // Chapter 10 - Ruins
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 10,
                ChapterName = "Ruins",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Ruins",
                Icon = "areas/desolozantas/ruins",
                TitleBase = "RUINS",
                TitleAccentColor = "8b4513",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "ancient_entrance", "collapsed_halls", "buried_treasure", "ruins_heart" },
                Music = "event:/Ingeste/music/lvl10/fallen_girl_star",
                Ambience = "event:/Ingeste/env/10_ruins",
                ColorGrade = "oldsite",
                Description = "Ancient ruins"
            });

            // Chapter 11 - Snow
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 11,
                ChapterName = "Snow",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Snow",
                Icon = "areas/desolozantas/snow",
                TitleBase = "SNOW",
                TitleAccentColor = "b0e0e6",
                TitleTextColor = "000080",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "tundra", "blizzard", "ice_caves", "frozen_peak" },
                Music = "event:/Ingeste/music/lvl11/main",
                Ambience = "event:/Ingeste/env/11_snow_daytime",
                ColorGrade = "cold",
                Description = "Frozen wasteland"
            });

            // Chapter 12 - Water
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 12,
                ChapterName = "Water",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Water",
                Icon = "areas/desolozantas/water",
                TitleBase = "WATER",
                TitleAccentColor = "1e90ff",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "shallows", "deep_dive", "underwater_temple", "abyss_floor" },
                Music = "event:/Ingeste/music/lvl12/main",
                Ambience = "event:/Ingeste/env/12_river",
                ColorGrade = "reflection",
                Description = "Aquatic depths"
            });

            // Chapter 13 - Time
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 13,
                ChapterName = "Time",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Time",
                Icon = "areas/desolozantas/time",
                TitleBase = "TIME",
                TitleAccentColor = "daa520",
                TitleTextColor = "ffffff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "clock_tower", "temporal_rift", "paradox", "eternity" },
                Music = "event:/Ingeste/music/lvl13/main",
                Ambience = "event:/Ingeste/env/13_factory",
                Description = "Temporal puzzles"
            });

            // Chapter 14 - Digital
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 14,
                ChapterName = "Digital",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Digital",
                Icon = "areas/desolozantas/digital",
                TitleBase = "DIGITAL",
                TitleAccentColor = "00ff00",
                TitleTextColor = "000000",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "data_stream", "firewall", "core_access", "system_heart" },
                Music = "event:/Ingeste/music/lvl14/main",
                Ambience = "event:/Ingeste/env/14_digital",
                BloomBase = 0.15f,
                BloomStrength = 1.3f,
                Description = "Virtual world"
            });

            // Chapter 15 - Castle
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 15,
                ChapterName = "Castle",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Castle",
                Icon = "areas/desolozantas/castle",
                TitleBase = "CASTLE",
                TitleAccentColor = "4b0082",
                TitleTextColor = "ffd700",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "courtyard", "grand_hall", "tower_ascent", "throne" },
                Music = "event:/Ingeste/music/lvl15/main",
                Ambience = "event:/Ingeste/env/15_peak",
                Description = "Royal fortress"
            });

            // Chapter 16 - Corruption (A-Side only)
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 16,
                ChapterName = "Corruption",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Corruption",
                Icon = "areas/desolozantas/corruption",
                TitleBase = "CORRUPTION",
                TitleAccentColor = "800080",
                TitleTextColor = "ff00ff",
                Type = ChapterType.Main,
                HasASide = true, HasBSide = false, HasCSide = false, HasDSide = false,
                Checkpoints = new[] { "start", "infection", "spread", "core", "purification" },
                Music = "event:/Ingeste/music/lvl16/cinematic/intro01",
                Ambience = "event:/Ingeste/env/16_myworld",
                DarknessAlpha = 0.25f,
                ColorGrade = "templevoid",
                Description = "Dark corruption (A-Side only)"
            });
        }

        /// <summary>
        /// Register Epilogue (Chapter 17).
        /// </summary>
        private static void RegisterEpilogue()
        {
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 17,
                ChapterName = "Epilogue",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Epilogue",
                Icon = "areas/desolozantas/epilogue",
                TitleBase = "EPILOGUE",
                TitleAccentColor = "ffd700",
                TitleTextColor = "ffffff",
                Type = ChapterType.Epilogue,
                IsInterlude = true,
                HasASide = false,
                HasBSide = false,
                HasCSide = false,
                HasDSide = false,
                Checkpoints = new[] { "start", "reflection", "farewell", "end" },
                Music = "event:/Ingeste/music/lvl17/main",
                Ambience = "event:/Ingeste/env/00_prologue",
                Description = "Story epilogue"
            });
        }

        /// <summary>
        /// Register DLC chapters (18-20).
        /// </summary>
        private static void RegisterDLCChapters()
        {
            // Chapter 18 - Heart (All sides)
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 18,
                ChapterName = "Heart",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Heart",
                Icon = "areas/desolozantas/heart",
                TitleBase = "HEART",
                TitleAccentColor = "ff1493",
                TitleTextColor = "ffffff",
                Type = ChapterType.DLC,
                HasASide = true, HasBSide = true, HasCSide = true, HasDSide = true,
                Checkpoints = new[] { "start", "pulse", "core", "soul", "heart_end" },
                Music = "event:/Ingeste/music/lvl18/main",
                BloomBase = 0.2f,
                Description = "Heart chapter with all sides"
            });

            // Chapter 19 - Space (A-Side only)
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 19,
                ChapterName = "Space",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Space",
                Icon = "areas/desolozantas/space",
                TitleBase = "SPACE",
                TitleAccentColor = "000080",
                TitleTextColor = "ffffff",
                Type = ChapterType.DLC,
                HasASide = true, HasBSide = false, HasCSide = false, HasDSide = false,
                Checkpoints = new[] { "start", "launch", "orbit", "nebula", "cosmos" },
                Music = "null",
                Ambience = "null",
                DarknessAlpha = 0.4f,
                Description = "Space exploration (A-Side)"
            });

            // Chapter 20 - The End (A-Side only)
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 20,
                ChapterName = "TheEnd",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "The End",
                Icon = "areas/desolozantas/theend",
                TitleBase = "THE END",
                TitleAccentColor = "ff4500",
                TitleTextColor = "ffffff",
                Type = ChapterType.DLC,
                HasASide = true, HasBSide = false, HasCSide = false, HasDSide = false,
                Checkpoints = new[] { "start", "remix_gauntlet", "speedrun_arena", "expert_challenge", "finale" },
                Music = "null",
                Ambience = "null",
                IntroType = "Transition",
                ColorGrade = "hot",
                Description = "Advanced challenge maps featuring remixed mechanics from main chapters, speedrun-focused rooms, and expert-level platforming sequences with D-Side difficulty"
            });
        }

        /// <summary>
        /// Register Post-Epilogue (Chapter 21).
        /// </summary>
        private static void RegisterPostEpilogue()
        {
            RegisterArea(new AreaRegistrationInfo
            {
                ChapterId = 21,
                ChapterName = "PostEpilogue",
                MapPath = "Maggy/DesoloZantas/Chapters",
                DisplayName = "Post Epilogue",
                Icon = "areas/desolozantas/postepilogue",
                TitleBase = "???",
                TitleAccentColor = "000000",
                TitleTextColor = "ffffff",
                Type = ChapterType.PostEpilogue,
                IsInterlude = true,
                HasASide = false,
                HasBSide = false,
                HasCSide = false,
                HasDSide = false,
                Checkpoints = new[] { "start", "secret_path", "revelation", "true_ending" },
                Music = "event:/Ingeste/final_content/music/lvl21/cast",
                Ambience = "null",
                DarknessAlpha = 0.5f,
                Wipe = "Dream",
                Description = "Post-game story"
            });
        }

        /// <summary>
        /// Register a custom area with the given info.
        /// </summary>
        public static void RegisterArea(AreaRegistrationInfo info)
        {
            if (RegisteredAreas.ContainsKey(info.ChapterId))
            {
                Logger.Log(LogLevel.Warn, "DesoloZantas", $"Chapter {info.ChapterId} already registered, overwriting");
            }

            RegisteredAreas[info.ChapterId] = info;

            // Also register checkpoints
            if (info.Checkpoints?.Length > 0)
            {
                CheckpointRegistration.RegisterCheckpoints(info.ChapterId, info.ChapterName, info.Checkpoints);
            }
        }

        /// <summary>
        /// Get registration info for a specific chapter ID.
        /// </summary>
        public static AreaRegistrationInfo GetAreaInfo(int chapterId)
        {
            return RegisteredAreas.TryGetValue(chapterId, out var info) ? info : null;
        }

        /// <summary>
        /// Get all registered areas.
        /// </summary>
        public static IEnumerable<AreaRegistrationInfo> GetAllAreas()
        {
            return RegisteredAreas.Values.OrderBy(a => a.ChapterId);
        }

        /// <summary>
        /// Get all main campaign areas (01-16).
        /// </summary>
        public static IEnumerable<AreaRegistrationInfo> GetMainAreas()
        {
            return RegisteredAreas.Values
                .Where(a => a.Type == ChapterType.Main)
                .OrderBy(a => a.ChapterId);
        }

        /// <summary>
        /// Get all DLC areas (18-20).
        /// </summary>
        public static IEnumerable<AreaRegistrationInfo> GetDLCAreas()
        {
            return RegisteredAreas.Values
                .Where(a => a.Type == ChapterType.DLC)
                .OrderBy(a => a.ChapterId);
        }

        /// <summary>
        /// Get all story chapters (Prologue, Epilogue, PostEpilogue).
        /// </summary>
        public static IEnumerable<AreaRegistrationInfo> GetStoryChapters()
        {
            return RegisteredAreas.Values
                .Where(a => a.Type == ChapterType.Prologue || 
                           a.Type == ChapterType.Epilogue || 
                           a.Type == ChapterType.PostEpilogue)
                .OrderBy(a => a.ChapterId);
        }

        /// <summary>
        /// Check if a chapter ID belongs to DesoloZantas.
        /// </summary>
        public static bool IsDesoloZantasChapter(int chapterId)
        {
            return chapterId >= 0 && chapterId <= 21;
        }

        /// <summary>
        /// Get chapters with D-Side content.
        /// </summary>
        public static IEnumerable<AreaRegistrationInfo> GetChaptersWithDSide()
        {
            return RegisteredAreas.Values
                .Where(a => a.HasDSide)
                .OrderBy(a => a.ChapterId);
        }

        /// <summary>
        /// Cleanup and unregister all areas.
        /// </summary>
        public static void Cleanup()
        {
            RegisteredAreas.Clear();
            _initialized = false;
            Logger.Log(LogLevel.Info, "DesoloZantas", "Area registration cleaned up");
        }
    }
}
