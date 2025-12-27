using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DesoloZantas.Core.Maps
{
    /// <summary>
    /// Handles map file organization and path resolution for the DesoloZantas mod.
    /// Provides utilities to migrate from the old structure (Main, Main2, Main3, Main4, etc.)
    /// to the new vanilla-like structure.
    /// </summary>
    public static class MapOrganizer
    {
        #region Path Constants
        
        /// <summary>
        /// Root path for all maps relative to the mod folder.
        /// </summary>
        public const string MapsRoot = "Maps/Maggy/DesoloZantas";
        
        /// <summary>
        /// Path for main chapter maps.
        /// </summary>
        public const string ChaptersPath = "Maps/Maggy/DesoloZantas/Chapters";
        
        /// <summary>
        /// Path for submap files.
        /// </summary>
        public const string SubmapsPath = "Maps/Maggy/DesoloZantas/Submaps";
        
        /// <summary>
        /// Path for lobby maps.
        /// </summary>
        public const string LobbiesPath = "Maps/Maggy/DesoloZantas/Lobbies";
        
        /// <summary>
        /// Path for gym/tutorial maps.
        /// </summary>
        public const string GymsPath = "Maps/Maggy/DesoloZantas/Gyms";
        
        /// <summary>
        /// Path for work-in-progress maps.
        /// </summary>
        public const string WIPPath = "Maps/Maggy/DesoloZantas/WIP";
        
        #endregion

        #region Migration Mappings
        
        /// <summary>
        /// Defines the mapping from old file names to new SID-based file names.
        /// </summary>
        public static class OldToNewMapping
        {
            /// <summary>
            /// Main folder (A-Side) mappings.
            /// </summary>
            public static readonly Dictionary<string, string> MainMappings = new Dictionary<string, string>
            {
                // Prologue
                { "00Prologue", "00_Prologue" },
                
                // Chapters 1-16 A-Side
                { "01CityAlt1", "01_City_A" },
                { "02NightmareAlt1", "02_Nightmare_A" },
                { "03StarsAlt1", "03_Stars_A" },
                { "04LegendAlt1", "04_Legend_A" },
                { "05RestoreAlt1", "05_Restore_A" },
                { "06StrongholdAlt1", "06_Stronghold_A" },
                { "07HellAlt1", "07_Hell_A" },
                { "08TruthAlt1", "08_Truth_A" },
                { "09SummitAlt1", "09_Summit_A" },
                { "10RuinsAlt1", "10_Ruins_A" },
                { "11SnowAlt1", "11_Snow_A" },
                { "12WaterAlt1", "12_Water_A" },
                { "13TimeAlt1", "13_Time_A" },
                { "14DigitalAlt1", "14_Digital_A" },
                { "15CastleAlt1", "15_Castle_A" },
                { "16Corruption", "16_Corruption_A" },
                
                // Epilogue and beyond
                { "17Epilogue", "17_Epilogue" },
                { "18HeartAlt1", "18_Heart_A" },
                { "19Space", "19_Space_A" },
                { "20TheEnd", "20_TheEnd_A" }
            };
            
            /// <summary>
            /// Main2 folder (B-Side) mappings.
            /// </summary>
            public static readonly Dictionary<string, string> Main2Mappings = new Dictionary<string, string>
            {
                { "01CityAlt2", "01_City_B" },
                { "02NightmareAlt2", "02_Nightmare_B" },
                { "03StarsAlt2", "03_Stars_B" },
                { "04LegendAlt2", "04_Legend_B" },
                { "05RestoreAlt2", "05_Restore_B" },
                { "06StrongholdAlt2", "06_Stronghold_B" },
                { "07HellAlt2", "07_Hell_B" },
                { "08TruthAlt2", "08_Truth_B" },
                { "09SummitAlt2", "09_Summit_B" },
                { "10RuinsAlt2", "10_Ruins_B" },
                { "11SnowAlt2", "11_Snow_B" },
                { "12WaterAlt2", "12_Water_B" },
                { "13TimeAlt2", "13_Time_B" },
                { "14DigitalAlt2", "14_Digital_B" },
                { "15CastleAlt2", "15_Castle_B" },
                { "18HeartAlt2", "18_Heart_B" }
            };
            
            /// <summary>
            /// Main3 folder (C-Side) mappings.
            /// </summary>
            public static readonly Dictionary<string, string> Main3Mappings = new Dictionary<string, string>
            {
                { "01CityAlt3", "01_City_C" },
                { "02NightmareAlt3", "02_Nightmare_C" },
                { "03StarsAlt3", "03_Stars_C" },
                { "04LegendAlt3", "04_Legend_C" },
                { "05RestoreAlt3", "05_Restore_C" },
                { "06StrongholdAlt3", "06_Stronghold_C" },
                { "07HellAlt3", "07_Hell_C" },
                { "08TruthAlt3", "08_Truth_C" },
                { "09SummitAlt3", "09_Summit_C" },
                { "10RuinsAlt3", "10_Ruins_C" },
                { "11SnowAlt3", "11_Snow_C" },
                { "12WaterAlt3", "12_Water_C" },
                { "13TimeAlt3", "13_Time_C" },
                { "14DigitalAlt3", "14_Digital_C" },
                { "15CastleAlt3", "15_Castle_C" },
                { "18HeartAlt3", "18_Heart_C" }
            };
            
            /// <summary>
            /// Main4 folder (D-Side) mappings.
            /// </summary>
            public static readonly Dictionary<string, string> Main4Mappings = new Dictionary<string, string>
            {
                { "01CityAlt4", "01_City_D" },
                { "02NightmareAlt4", "02_Nightmare_D" },
                { "03StarsAlt4", "03_Stars_D" },
                { "04LegendAlt4", "04_Legend_D" },
                { "05RestoreAlt4", "05_Restore_D" },
                { "06StrongholdAlt4", "06_Stronghold_D" },
                { "07HellAlt4", "07_Hell_D" },
                { "08TruthAlt4", "08_Truth_D" },
                { "09SummitAlt4", "09_Summit_D" },
                { "10RuinsAlt4", "10_Ruins_D" },
                { "11SnowAlt4", "11_Snow_D" },
                { "12WaterAlt4", "12_Water_D" },
                { "13TimeAlt4", "13_Time_D" },
                { "14DigitalAlt4", "14_Digital_D" },
                { "15CastleAlt4", "15_Castle_D" },
                { "18HeartAlt4", "18_Heart_D" }
            };
            
            /// <summary>
            /// Submap folder mappings.
            /// </summary>
            public static readonly Dictionary<string, string> SubmapMappings = new Dictionary<string, string>
            {
                { "submap_01City_1", "01_City_Sub1" },
                { "submap_01City_2", "01_City_Sub2" },
                { "submap_01City_3", "01_City_Sub3" },
                { "submap_02Nightmare_1", "02_Nightmare_Sub1" },
                { "submap_02Nightmare_2", "02_Nightmare_Sub2" },
                { "submap_02Nightmare_3", "02_Nightmare_Sub3" },
                { "submap_03Stars_1", "03_Stars_Sub1" },
                { "submap_05Restore_1", "05_Restore_Sub1" },
                { "submap_05Restore_2", "05_Restore_Sub2" },
                { "submap_05Restore_3", "05_Restore_Sub3" },
                { "submap_06Stronghold_1", "06_Stronghold_Sub1" },
                { "submap_06Stronghold_2", "06_Stronghold_Sub2" },
                { "submap_06Stronghold_3", "06_Stronghold_Sub3" },
                { "submap_07Hell_1", "07_Hell_Sub1" },
                { "submap_07Hell_2", "07_Hell_Sub2" },
                { "submap_07Hell_3", "07_Hell_Sub3" },
                { "submap_08Truth_1", "08_Truth_Sub1" },
                { "submap_08Truth_2", "08_Truth_Sub2" },
                { "submap_08Truth_3", "08_Truth_Sub3" },
                { "submap_09Summit_1", "09_Summit_Sub1" },
                { "submap_09Summit_2", "09_Summit_Sub2" },
                { "submap_09Summit_3", "09_Summit_Sub3" },
                { "submap_18Heart_1", "18_Heart_Sub1" },
                { "submap_18Heart_2", "18_Heart_Sub2" },
                { "submap_18Heart_3", "18_Heart_Sub3" }
            };
            
            /// <summary>
            /// Lobby folder mappings.
            /// </summary>
            public static readonly Dictionary<string, string> LobbyMappings = new Dictionary<string, string>
            {
                { "lobby_00Prologue_A", "00_Prologue_Lobby" },
                { "lobby_01City_A", "01_City_Lobby" },
                { "lobby_02Nightmare_A", "02_Nightmare_Lobby" },
                { "lobby_04Legend_A", "04_Legend_Lobby" },
                { "lobby_05Restore_A", "05_Restore_Lobby" },
                { "lobby_06Stronghold_A", "06_Stronghold_Lobby" },
                { "lobby_07Hell_A", "07_Hell_Lobby" },
                { "lobby_08Truth_A", "08_Truth_Lobby" },
                { "lobby_09Summit_A", "09_Summit_Lobby" },
                { "lobby_10Ruins_A", "10_Ruins_Lobby" },
                { "lobby_11Snow_A", "11_Snow_Lobby" },
                { "lobby_12Water_A", "12_Water_Lobby" },
                { "lobby_13Fire_A", "13_Fire_Lobby" },
                { "lobby_14Digital_A", "14_Digital_Lobby" },
                { "lobby_15Castle_A", "15_Castle_Lobby" },
                { "lobby_16Corruption_A", "16_Corruption_Lobby" },
                { "lobby_17Epilogue_A", "17_Epilogue_Lobby" },
                { "lobby_18Heart_A", "18_Heart_Lobby" },
                { "lobby_19Space_A", "19_Space_Lobby" },
                { "lobby_20TheEnd_A", "20_TheEnd_Lobby" }
            };
            
            /// <summary>
            /// Gym folder mappings.
            /// </summary>
            public static readonly Dictionary<string, string> GymMappings = new Dictionary<string, string>
            {
                { "gym_combo", "gym_combo" },
                { "gym_superwalljump", "gym_superwalljump" },
                { "gym_super_coyote_jump", "gym_super_coyote_jump" },
                { "gym_too_fast", "gym_too_fast" },
                { "gym_too_late", "gym_too_late" },
                { "gym_ultra_dashfaze", "gym_ultra_dashfaze" },
                { "gym_wavedash", "gym_wavedash" },
                { "gym_wavefaze", "gym_wavefaze" },
                { "gym_wavefazingppt", "gym_wavefazingppt" }
            };
        }
        
        #endregion

        #region Path Resolution
        
        /// <summary>
        /// Resolves the full path for a chapter given its number and side.
        /// </summary>
        public static string GetChapterPath(int chapterNumber, MapSIDRegistry.ChapterSide side = MapSIDRegistry.ChapterSide.ASide)
        {
            string sideSuffix = side switch
            {
                MapSIDRegistry.ChapterSide.ASide => "_A",
                MapSIDRegistry.ChapterSide.BSide => "_B",
                MapSIDRegistry.ChapterSide.CSide => "_C",
                MapSIDRegistry.ChapterSide.DSide => "_D",
                _ => "_A"
            };
            
            string chapterName = GetChapterName(chapterNumber);
            
            // Special cases without side suffix
            if (chapterNumber == 0 || chapterNumber == 17 || chapterNumber == 21)
            {
                return $"{ChaptersPath}/{chapterNumber:D2}_{chapterName}";
            }
            
            return $"{ChaptersPath}/{chapterNumber:D2}_{chapterName}{sideSuffix}";
        }
        
        /// <summary>
        /// Gets the chapter name by number.
        /// </summary>
        public static string GetChapterName(int chapterNumber)
        {
            return chapterNumber switch
            {
                0 => "Prologue",
                1 => "City",
                2 => "Nightmare",
                3 => "Stars",
                4 => "Legend",
                5 => "Restore",
                6 => "Stronghold",
                7 => "Hell",
                8 => "Truth",
                9 => "Summit",
                10 => "Ruins",
                11 => "Snow",
                12 => "Water",
                13 => "Time",
                14 => "Digital",
                15 => "Castle",
                16 => "Corruption",
                17 => "Epilogue",
                18 => "Heart",
                19 => "Space",
                20 => "TheEnd",
                21 => "PostEpilogue",
                _ => $"Unknown{chapterNumber}"
            };
        }
        
        /// <summary>
        /// Gets the full SID for a chapter.
        /// </summary>
        public static string GetChapterSID(int chapterNumber, MapSIDRegistry.ChapterSide side = MapSIDRegistry.ChapterSide.ASide)
        {
            string path = GetChapterPath(chapterNumber, side);
            return $"{MapSIDRegistry.ModPrefix}/{path.Replace(MapsRoot + "/", "")}";
        }
        
        /// <summary>
        /// Resolves the path for a submap.
        /// </summary>
        public static string GetSubmapPath(int chapterNumber, int submapIndex)
        {
            string chapterName = GetChapterName(chapterNumber);
            return $"{SubmapsPath}/{chapterNumber:D2}_{chapterName}_Sub{submapIndex}";
        }
        
        /// <summary>
        /// Resolves the path for a lobby.
        /// </summary>
        public static string GetLobbyPath(int chapterNumber)
        {
            string chapterName = GetChapterName(chapterNumber);
            return $"{LobbiesPath}/{chapterNumber:D2}_{chapterName}_Lobby";
        }
        
        /// <summary>
        /// Resolves the path for a gym.
        /// </summary>
        public static string GetGymPath(string gymName)
        {
            return $"{GymsPath}/{gymName}";
        }
        
        #endregion

        #region Migration Support
        
        /// <summary>
        /// Migration result for a single file.
        /// </summary>
        public class MigrationResult
        {
            public string OldPath { get; set; }
            public string NewPath { get; set; }
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
        
        /// <summary>
        /// Generates the migration plan without executing it.
        /// </summary>
        public static List<MigrationResult> GenerateMigrationPlan(string modRootPath)
        {
            var plan = new List<MigrationResult>();
            string mapsPath = Path.Combine(modRootPath, "Maps", "Maggy");
            
            // Main folder
            AddMigrationPlan(plan, mapsPath, "Main", OldToNewMapping.MainMappings, ChaptersPath);
            
            // Main2 folder (B-Sides)
            AddMigrationPlan(plan, mapsPath, "Main2", OldToNewMapping.Main2Mappings, ChaptersPath);
            
            // Main3 folder (C-Sides)
            AddMigrationPlan(plan, mapsPath, "Main3", OldToNewMapping.Main3Mappings, ChaptersPath);
            
            // Main4 folder (D-Sides)
            AddMigrationPlan(plan, mapsPath, "Main4", OldToNewMapping.Main4Mappings, ChaptersPath);
            
            // Submaps
            AddMigrationPlan(plan, mapsPath, "Submaps", OldToNewMapping.SubmapMappings, SubmapsPath);
            
            // Lobbies
            AddMigrationPlan(plan, mapsPath, "Lobbies", OldToNewMapping.LobbyMappings, LobbiesPath);
            
            // Gyms
            AddMigrationPlan(plan, mapsPath, "Gyms", OldToNewMapping.GymMappings, GymsPath);
            
            return plan;
        }
        
        private static void AddMigrationPlan(
            List<MigrationResult> plan,
            string mapsPath,
            string sourceFolder,
            Dictionary<string, string> mappings,
            string targetPath)
        {
            string sourceFolderPath = Path.Combine(mapsPath, sourceFolder);
            
            if (!Directory.Exists(sourceFolderPath))
                return;
            
            foreach (var mapping in mappings)
            {
                // Check for .bin file
                string binSource = Path.Combine(sourceFolderPath, $"{mapping.Key}.bin");
                if (File.Exists(binSource))
                {
                    plan.Add(new MigrationResult
                    {
                        OldPath = binSource,
                        NewPath = Path.Combine(mapsPath, "DesoloZantas", targetPath.Replace(MapsRoot + "/", ""), $"{mapping.Value}.bin"),
                        Success = true
                    });
                }
                
                // Check for .meta.yaml file
                string metaSource = Path.Combine(sourceFolderPath, $"{mapping.Key}.meta.yaml");
                if (File.Exists(metaSource))
                {
                    plan.Add(new MigrationResult
                    {
                        OldPath = metaSource,
                        NewPath = Path.Combine(mapsPath, "DesoloZantas", targetPath.Replace(MapsRoot + "/", ""), $"{mapping.Value}.meta.yaml"),
                        Success = true
                    });
                }
                
                // Check for .altsideshelper.meta.yaml file
                string altSideSource = Path.Combine(sourceFolderPath, $"{mapping.Key}.altsideshelper.meta.yaml");
                if (File.Exists(altSideSource))
                {
                    plan.Add(new MigrationResult
                    {
                        OldPath = altSideSource,
                        NewPath = Path.Combine(mapsPath, "DesoloZantas", targetPath.Replace(MapsRoot + "/", ""), $"{mapping.Value}.altsideshelper.meta.yaml"),
                        Success = true
                    });
                }
            }
        }
        
        #endregion

        #region Story Order
        
        /// <summary>
        /// Defines the canonical story order for the DesoloZantas campaign.
        /// </summary>
        public static readonly int[] StoryOrder = new int[]
        {
            0,   // Prologue
            1,   // Chapter 1 - City
            2,   // Chapter 2 - Nightmare
            3,   // Chapter 3 - Stars
            4,   // Chapter 4 - Legend
            5,   // Chapter 5 - Restore
            6,   // Chapter 6 - Stronghold
            7,   // Chapter 7 - Hell
            8,   // Chapter 8 - Truth
            9,   // Chapter 9 - Summit
            10,  // Chapter 10 - Ruins
            11,  // Chapter 11 - Snow
            12,  // Chapter 12 - Water
            13,  // Chapter 13 - Time
            14,  // Chapter 14 - Digital
            15,  // Chapter 15 - Castle
            16,  // Chapter 16 - Corruption
            17,  // Epilogue
            18,  // Chapter 18 - Heart
            19,  // Chapter 19 - Space (Note: No chapter between 17-19, jumps to 19)
            20,  // Chapter 20 - The End
            21   // Post-Epilogue
        };
        
        /// <summary>
        /// Gets the next chapter in the story after the given chapter.
        /// </summary>
        public static int? GetNextChapter(int currentChapter)
        {
            int currentIndex = Array.IndexOf(StoryOrder, currentChapter);
            if (currentIndex < 0 || currentIndex >= StoryOrder.Length - 1)
                return null;
            return StoryOrder[currentIndex + 1];
        }
        
        /// <summary>
        /// Gets the previous chapter in the story before the given chapter.
        /// </summary>
        public static int? GetPreviousChapter(int currentChapter)
        {
            int currentIndex = Array.IndexOf(StoryOrder, currentChapter);
            if (currentIndex <= 0)
                return null;
            return StoryOrder[currentIndex - 1];
        }
        
        /// <summary>
        /// Checks if a chapter is part of the main story.
        /// </summary>
        public static bool IsMainStoryChapter(int chapterNumber)
        {
            return Array.IndexOf(StoryOrder, chapterNumber) >= 0;
        }
        
        /// <summary>
        /// Gets the story index for a chapter (0-based).
        /// </summary>
        public static int GetStoryIndex(int chapterNumber)
        {
            return Array.IndexOf(StoryOrder, chapterNumber);
        }
        
        #endregion
    }
}
