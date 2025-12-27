using System;
using System.Collections.Generic;
using System.Linq;

namespace DesoloZantas.Core.Maps
{
    /// <summary>
    /// Centralized registry for all map SIDs (String IDs) in the DesoloZantas mod.
    /// Provides a vanilla-like organization structure from Prologue to Post-Epilogue.
    /// </summary>
    public static class MapSIDRegistry
    {
        #region Constants
        
        /// <summary>
        /// Base path for all maps in the mod.
        /// </summary>
        public const string MapBasePath = "Maps/Maggy/DesoloZantas";
        
        /// <summary>
        /// Mod name prefix for SIDs.
        /// </summary>
        public const string ModPrefix = "DesoloZantas";
        
        #endregion

        #region Map Types
        
        /// <summary>
        /// Defines the type of map for organizational purposes.
        /// </summary>
        public enum MapType
        {
            Prologue,
            MainChapter,
            Epilogue,
            PostEpilogue,
            Submap,
            Lobby,
            Gym,
            WIP
        }
        
        /// <summary>
        /// Defines the side of a chapter (A-Side, B-Side, C-Side, D-Side).
        /// </summary>
        public enum ChapterSide
        {
            ASide = 0,
            BSide = 1,
            CSide = 2,
            DSide = 3
        }
        
        #endregion

        #region Map Info Class
        
        /// <summary>
        /// Contains all metadata for a single map.
        /// </summary>
        public class MapInfo
        {
            public string SID { get; }
            public string DisplayName { get; }
            public int ChapterNumber { get; }
            public ChapterSide Side { get; }
            public MapType Type { get; }
            public string InternalName { get; }
            public string FilePath { get; }
            public string ParentChapterSID { get; }
            public int SubmapIndex { get; }
            
            public MapInfo(
                string sid,
                string displayName,
                int chapterNumber,
                ChapterSide side,
                MapType type,
                string internalName,
                string filePath,
                string parentChapterSID = null,
                int submapIndex = 0)
            {
                SID = sid;
                DisplayName = displayName;
                ChapterNumber = chapterNumber;
                Side = side;
                Type = type;
                InternalName = internalName;
                FilePath = filePath;
                ParentChapterSID = parentChapterSID;
                SubmapIndex = submapIndex;
            }
            
            /// <summary>
            /// Gets the full SID with mod prefix.
            /// </summary>
            public string FullSID => $"{ModPrefix}/{SID}";
            
            /// <summary>
            /// Gets the chapter display string (e.g., "Chapter 1 A-Side").
            /// </summary>
            public string ChapterDisplay
            {
                get
                {
                    string sideStr = Side switch
                    {
                        ChapterSide.ASide => "A-Side",
                        ChapterSide.BSide => "B-Side",
                        ChapterSide.CSide => "C-Side",
                        ChapterSide.DSide => "D-Side",
                        _ => ""
                    };
                    
                    return Type switch
                    {
                        MapType.Prologue => "Prologue",
                        MapType.Epilogue => "Epilogue",
                        MapType.PostEpilogue => "Post-Epilogue",
                        MapType.Submap => $"Chapter {ChapterNumber} Submap {SubmapIndex}",
                        MapType.Lobby => $"Chapter {ChapterNumber} Lobby",
                        MapType.Gym => $"Gym: {DisplayName}",
                        MapType.WIP => $"WIP: {DisplayName}",
                        _ => $"Chapter {ChapterNumber} {sideStr}"
                    };
                }
            }
        }
        
        #endregion

        #region Main Chapter SIDs
        
        // Prologue (Chapter 0)
        public static class Prologue
        {
            public const string SID = "00_Prologue";
            public const string DisplayName = "Awakening Dream";
            public const string FilePath = "Chapters/00_Prologue";
            
            public static MapInfo GetMapInfo() => new MapInfo(
                SID, DisplayName, 0, ChapterSide.ASide, MapType.Prologue, "Prologue", FilePath);
        }
        
        // Chapter 1 - City
        public static class Chapter01
        {
            public const string ChapterName = "City";
            public const int ChapterNumber = 1;
            
            public static class ASide { public const string SID = "01_City_A"; public const string FilePath = "Chapters/01_City_A"; }
            public static class BSide { public const string SID = "01_City_B"; public const string FilePath = "Chapters/01_City_B"; }
            public static class CSide { public const string SID = "01_City_C"; public const string FilePath = "Chapters/01_City_C"; }
            public static class DSide { public const string SID = "01_City_D"; public const string FilePath = "Chapters/01_City_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "City", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "City", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "City", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "City", DSide.FilePath);
        }
        
        // Chapter 2 - Nightmare
        public static class Chapter02
        {
            public const string ChapterName = "Nightmare";
            public const int ChapterNumber = 2;
            
            public static class ASide { public const string SID = "02_Nightmare_A"; public const string FilePath = "Chapters/02_Nightmare_A"; }
            public static class BSide { public const string SID = "02_Nightmare_B"; public const string FilePath = "Chapters/02_Nightmare_B"; }
            public static class CSide { public const string SID = "02_Nightmare_C"; public const string FilePath = "Chapters/02_Nightmare_C"; }
            public static class DSide { public const string SID = "02_Nightmare_D"; public const string FilePath = "Chapters/02_Nightmare_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Nightmare", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Nightmare", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Nightmare", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Nightmare", DSide.FilePath);
        }
        
        // Chapter 3 - Stars
        public static class Chapter03
        {
            public const string ChapterName = "Stars";
            public const int ChapterNumber = 3;
            
            public static class ASide { public const string SID = "03_Stars_A"; public const string FilePath = "Chapters/03_Stars_A"; }
            public static class BSide { public const string SID = "03_Stars_B"; public const string FilePath = "Chapters/03_Stars_B"; }
            public static class CSide { public const string SID = "03_Stars_C"; public const string FilePath = "Chapters/03_Stars_C"; }
            public static class DSide { public const string SID = "03_Stars_D"; public const string FilePath = "Chapters/03_Stars_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Stars", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Stars", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Stars", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Stars", DSide.FilePath);
        }
        
        // Chapter 4 - Legend
        public static class Chapter04
        {
            public const string ChapterName = "Legend";
            public const int ChapterNumber = 4;
            
            public static class ASide { public const string SID = "04_Legend_A"; public const string FilePath = "Chapters/04_Legend_A"; }
            public static class BSide { public const string SID = "04_Legend_B"; public const string FilePath = "Chapters/04_Legend_B"; }
            public static class CSide { public const string SID = "04_Legend_C"; public const string FilePath = "Chapters/04_Legend_C"; }
            public static class DSide { public const string SID = "04_Legend_D"; public const string FilePath = "Chapters/04_Legend_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Legend", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Legend", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Legend", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Legend", DSide.FilePath);
        }
        
        // Chapter 5 - Restore
        public static class Chapter05
        {
            public const string ChapterName = "Restore";
            public const int ChapterNumber = 5;
            
            public static class ASide { public const string SID = "05_Restore_A"; public const string FilePath = "Chapters/05_Restore_A"; }
            public static class BSide { public const string SID = "05_Restore_B"; public const string FilePath = "Chapters/05_Restore_B"; }
            public static class CSide { public const string SID = "05_Restore_C"; public const string FilePath = "Chapters/05_Restore_C"; }
            public static class DSide { public const string SID = "05_Restore_D"; public const string FilePath = "Chapters/05_Restore_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Restore", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Restore", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Restore", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Restore", DSide.FilePath);
        }
        
        // Chapter 6 - Stronghold
        public static class Chapter06
        {
            public const string ChapterName = "Stronghold";
            public const int ChapterNumber = 6;
            
            public static class ASide { public const string SID = "06_Stronghold_A"; public const string FilePath = "Chapters/06_Stronghold_A"; }
            public static class BSide { public const string SID = "06_Stronghold_B"; public const string FilePath = "Chapters/06_Stronghold_B"; }
            public static class CSide { public const string SID = "06_Stronghold_C"; public const string FilePath = "Chapters/06_Stronghold_C"; }
            public static class DSide { public const string SID = "06_Stronghold_D"; public const string FilePath = "Chapters/06_Stronghold_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Stronghold", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Stronghold", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Stronghold", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Stronghold", DSide.FilePath);
        }
        
        // Chapter 7 - Hell
        public static class Chapter07
        {
            public const string ChapterName = "Hell";
            public const int ChapterNumber = 7;
            
            public static class ASide { public const string SID = "07_Hell_A"; public const string FilePath = "Chapters/07_Hell_A"; }
            public static class BSide { public const string SID = "07_Hell_B"; public const string FilePath = "Chapters/07_Hell_B"; }
            public static class CSide { public const string SID = "07_Hell_C"; public const string FilePath = "Chapters/07_Hell_C"; }
            public static class DSide { public const string SID = "07_Hell_D"; public const string FilePath = "Chapters/07_Hell_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Hell", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Hell", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Hell", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Hell", DSide.FilePath);
        }
        
        // Chapter 8 - Truth
        public static class Chapter08
        {
            public const string ChapterName = "Truth";
            public const int ChapterNumber = 8;
            
            public static class ASide { public const string SID = "08_Truth_A"; public const string FilePath = "Chapters/08_Truth_A"; }
            public static class BSide { public const string SID = "08_Truth_B"; public const string FilePath = "Chapters/08_Truth_B"; }
            public static class CSide { public const string SID = "08_Truth_C"; public const string FilePath = "Chapters/08_Truth_C"; }
            public static class DSide { public const string SID = "08_Truth_D"; public const string FilePath = "Chapters/08_Truth_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Truth", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Truth", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Truth", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Truth", DSide.FilePath);
        }
        
        // Chapter 9 - Summit
        public static class Chapter09
        {
            public const string ChapterName = "Summit";
            public const int ChapterNumber = 9;
            
            public static class ASide { public const string SID = "09_Summit_A"; public const string FilePath = "Chapters/09_Summit_A"; }
            public static class BSide { public const string SID = "09_Summit_B"; public const string FilePath = "Chapters/09_Summit_B"; }
            public static class CSide { public const string SID = "09_Summit_C"; public const string FilePath = "Chapters/09_Summit_C"; }
            public static class DSide { public const string SID = "09_Summit_D"; public const string FilePath = "Chapters/09_Summit_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Summit", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Summit", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Summit", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Summit", DSide.FilePath);
        }
        
        // Chapter 10 - Ruins
        public static class Chapter10
        {
            public const string ChapterName = "Ruins";
            public const int ChapterNumber = 10;
            
            public static class ASide { public const string SID = "10_Ruins_A"; public const string FilePath = "Chapters/10_Ruins_A"; }
            public static class BSide { public const string SID = "10_Ruins_B"; public const string FilePath = "Chapters/10_Ruins_B"; }
            public static class CSide { public const string SID = "10_Ruins_C"; public const string FilePath = "Chapters/10_Ruins_C"; }
            public static class DSide { public const string SID = "10_Ruins_D"; public const string FilePath = "Chapters/10_Ruins_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Ruins", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Ruins", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Ruins", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Ruins", DSide.FilePath);
        }
        
        // Chapter 11 - Snow
        public static class Chapter11
        {
            public const string ChapterName = "Snow";
            public const int ChapterNumber = 11;
            
            public static class ASide { public const string SID = "11_Snow_A"; public const string FilePath = "Chapters/11_Snow_A"; }
            public static class BSide { public const string SID = "11_Snow_B"; public const string FilePath = "Chapters/11_Snow_B"; }
            public static class CSide { public const string SID = "11_Snow_C"; public const string FilePath = "Chapters/11_Snow_C"; }
            public static class DSide { public const string SID = "11_Snow_D"; public const string FilePath = "Chapters/11_Snow_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Snow", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Snow", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Snow", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Snow", DSide.FilePath);
        }
        
        // Chapter 12 - Water
        public static class Chapter12
        {
            public const string ChapterName = "Water";
            public const int ChapterNumber = 12;
            
            public static class ASide { public const string SID = "12_Water_A"; public const string FilePath = "Chapters/12_Water_A"; }
            public static class BSide { public const string SID = "12_Water_B"; public const string FilePath = "Chapters/12_Water_B"; }
            public static class CSide { public const string SID = "12_Water_C"; public const string FilePath = "Chapters/12_Water_C"; }
            public static class DSide { public const string SID = "12_Water_D"; public const string FilePath = "Chapters/12_Water_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Water", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Water", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Water", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Water", DSide.FilePath);
        }
        
        // Chapter 13 - Time
        public static class Chapter13
        {
            public const string ChapterName = "Time";
            public const int ChapterNumber = 13;
            
            public static class ASide { public const string SID = "13_Time_A"; public const string FilePath = "Chapters/13_Time_A"; }
            public static class BSide { public const string SID = "13_Time_B"; public const string FilePath = "Chapters/13_Time_B"; }
            public static class CSide { public const string SID = "13_Time_C"; public const string FilePath = "Chapters/13_Time_C"; }
            public static class DSide { public const string SID = "13_Time_D"; public const string FilePath = "Chapters/13_Time_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Time", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Time", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Time", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Time", DSide.FilePath);
        }
        
        // Chapter 14 - Digital
        public static class Chapter14
        {
            public const string ChapterName = "Digital";
            public const int ChapterNumber = 14;
            
            public static class ASide { public const string SID = "14_Digital_A"; public const string FilePath = "Chapters/14_Digital_A"; }
            public static class BSide { public const string SID = "14_Digital_B"; public const string FilePath = "Chapters/14_Digital_B"; }
            public static class CSide { public const string SID = "14_Digital_C"; public const string FilePath = "Chapters/14_Digital_C"; }
            public static class DSide { public const string SID = "14_Digital_D"; public const string FilePath = "Chapters/14_Digital_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Digital", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Digital", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Digital", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Digital", DSide.FilePath);
        }
        
        // Chapter 15 - Castle
        public static class Chapter15
        {
            public const string ChapterName = "Castle";
            public const int ChapterNumber = 15;
            
            public static class ASide { public const string SID = "15_Castle_A"; public const string FilePath = "Chapters/15_Castle_A"; }
            public static class BSide { public const string SID = "15_Castle_B"; public const string FilePath = "Chapters/15_Castle_B"; }
            public static class CSide { public const string SID = "15_Castle_C"; public const string FilePath = "Chapters/15_Castle_C"; }
            public static class DSide { public const string SID = "15_Castle_D"; public const string FilePath = "Chapters/15_Castle_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Castle", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Castle", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Castle", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Castle", DSide.FilePath);
        }
        
        // Chapter 16 - Corruption
        public static class Chapter16
        {
            public const string ChapterName = "Corruption";
            public const int ChapterNumber = 16;
            
            public static class ASide { public const string SID = "16_Corruption_A"; public const string FilePath = "Chapters/16_Corruption_A"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Corruption", ASide.FilePath);
        }
        
        // Chapter 17 - Epilogue
        public static class Epilogue
        {
            public const string SID = "17_Epilogue";
            public const string DisplayName = "Final Resonance";
            public const string FilePath = "Chapters/17_Epilogue";
            
            public static MapInfo GetMapInfo() => new MapInfo(
                SID, DisplayName, 17, ChapterSide.ASide, MapType.Epilogue, "Epilogue", FilePath);
        }
        
        // Chapter 18 - Heart
        public static class Chapter18
        {
            public const string ChapterName = "Heart";
            public const int ChapterNumber = 18;
            
            public static class ASide { public const string SID = "18_Heart_A"; public const string FilePath = "Chapters/18_Heart_A"; }
            public static class BSide { public const string SID = "18_Heart_B"; public const string FilePath = "Chapters/18_Heart_B"; }
            public static class CSide { public const string SID = "18_Heart_C"; public const string FilePath = "Chapters/18_Heart_C"; }
            public static class DSide { public const string SID = "18_Heart_D"; public const string FilePath = "Chapters/18_Heart_D"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Heart", ASide.FilePath);
            public static MapInfo GetBSide() => new MapInfo(BSide.SID, ChapterName, ChapterNumber, ChapterSide.BSide, MapType.MainChapter, "Heart", BSide.FilePath);
            public static MapInfo GetCSide() => new MapInfo(CSide.SID, ChapterName, ChapterNumber, ChapterSide.CSide, MapType.MainChapter, "Heart", CSide.FilePath);
            public static MapInfo GetDSide() => new MapInfo(DSide.SID, ChapterName, ChapterNumber, ChapterSide.DSide, MapType.MainChapter, "Heart", DSide.FilePath);
        }
        
        // Chapter 19 - Space
        public static class Chapter19
        {
            public const string ChapterName = "Space";
            public const int ChapterNumber = 19;
            
            public static class ASide { public const string SID = "19_Space_A"; public const string FilePath = "Chapters/19_Space_A"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "Space", ASide.FilePath);
        }
        
        // Chapter 20 - The End
        public static class Chapter20
        {
            public const string ChapterName = "The End";
            public const int ChapterNumber = 20;
            
            public static class ASide { public const string SID = "20_TheEnd_A"; public const string FilePath = "Chapters/20_TheEnd_A"; }
            
            public static MapInfo GetASide() => new MapInfo(ASide.SID, ChapterName, ChapterNumber, ChapterSide.ASide, MapType.MainChapter, "TheEnd", ASide.FilePath);
        }
        
        // Chapter 21 - Post-Epilogue
        public static class PostEpilogue
        {
            public const string SID = "21_PostEpilogue";
            public const string DisplayName = "Post Respite";
            public const string FilePath = "Chapters/21_PostEpilogue";
            
            public static MapInfo GetMapInfo() => new MapInfo(
                SID, DisplayName, 21, ChapterSide.ASide, MapType.PostEpilogue, "PostEpilogue", FilePath);
        }
        
        #endregion

        #region Submap SIDs
        
        /// <summary>
        /// Contains all submap SIDs organized by parent chapter.
        /// </summary>
        public static class Submaps
        {
            public const string BasePath = "Submaps";
            
            // Chapter 1 Submaps
            public static class Chapter01
            {
                public const string Submap1 = "Submaps/01_City_Sub1";
                public const string Submap2 = "Submaps/01_City_Sub2";
                public const string Submap3 = "Submaps/01_City_Sub3";
                
                public static MapInfo GetSubmap(int index) => new MapInfo(
                    $"01_City_Sub{index}", $"City Submap {index}", 1, ChapterSide.ASide, 
                    MapType.Submap, $"submap_01City_{index}", $"Submaps/01_City_Sub{index}",
                    MapSIDRegistry.Chapter01.ASide.SID, index);
            }
            
            // Chapter 2 Submaps
            public static class Chapter02
            {
                public const string Submap1 = "Submaps/02_Nightmare_Sub1";
                public const string Submap2 = "Submaps/02_Nightmare_Sub2";
                public const string Submap3 = "Submaps/02_Nightmare_Sub3";
                
                public static MapInfo GetSubmap(int index) => new MapInfo(
                    $"02_Nightmare_Sub{index}", $"Nightmare Submap {index}", 2, ChapterSide.ASide, 
                    MapType.Submap, $"submap_02Nightmare_{index}", $"Submaps/02_Nightmare_Sub{index}",
                    MapSIDRegistry.Chapter02.ASide.SID, index);
            }
            
            // Chapter 3 Submaps
            public static class Chapter03
            {
                public const string Submap1 = "Submaps/03_Stars_Sub1";
                
                public static MapInfo GetSubmap(int index) => new MapInfo(
                    $"03_Stars_Sub{index}", $"Stars Submap {index}", 3, ChapterSide.ASide, 
                    MapType.Submap, $"submap_03Stars_{index}", $"Submaps/03_Stars_Sub{index}",
                    MapSIDRegistry.Chapter03.ASide.SID, index);
            }
            
            // Chapter 5 Submaps
            public static class Chapter05
            {
                public const string Submap1 = "Submaps/05_Restore_Sub1";
                public const string Submap2 = "Submaps/05_Restore_Sub2";
                public const string Submap3 = "Submaps/05_Restore_Sub3";
                
                public static MapInfo GetSubmap(int index) => new MapInfo(
                    $"05_Restore_Sub{index}", $"Restore Submap {index}", 5, ChapterSide.ASide, 
                    MapType.Submap, $"submap_05Restore_{index}", $"Submaps/05_Restore_Sub{index}",
                    MapSIDRegistry.Chapter05.ASide.SID, index);
            }
            
            // Chapter 6 Submaps
            public static class Chapter06
            {
                public const string Submap1 = "Submaps/06_Stronghold_Sub1";
                public const string Submap2 = "Submaps/06_Stronghold_Sub2";
                public const string Submap3 = "Submaps/06_Stronghold_Sub3";
                
                public static MapInfo GetSubmap(int index) => new MapInfo(
                    $"06_Stronghold_Sub{index}", $"Stronghold Submap {index}", 6, ChapterSide.ASide, 
                    MapType.Submap, $"submap_06Stronghold_{index}", $"Submaps/06_Stronghold_Sub{index}",
                    MapSIDRegistry.Chapter06.ASide.SID, index);
            }
            
            // Chapter 7 Submaps
            public static class Chapter07
            {
                public const string Submap1 = "Submaps/07_Hell_Sub1";
                public const string Submap2 = "Submaps/07_Hell_Sub2";
                public const string Submap3 = "Submaps/07_Hell_Sub3";
                
                public static MapInfo GetSubmap(int index) => new MapInfo(
                    $"07_Hell_Sub{index}", $"Hell Submap {index}", 7, ChapterSide.ASide, 
                    MapType.Submap, $"submap_07Hell_{index}", $"Submaps/07_Hell_Sub{index}",
                    MapSIDRegistry.Chapter07.ASide.SID, index);
            }
            
            // Chapter 8 Submaps
            public static class Chapter08
            {
                public const string Submap1 = "Submaps/08_Truth_Sub1";
                public const string Submap2 = "Submaps/08_Truth_Sub2";
                public const string Submap3 = "Submaps/08_Truth_Sub3";
                
                public static MapInfo GetSubmap(int index) => new MapInfo(
                    $"08_Truth_Sub{index}", $"Truth Submap {index}", 8, ChapterSide.ASide, 
                    MapType.Submap, $"submap_08Truth_{index}", $"Submaps/08_Truth_Sub{index}",
                    MapSIDRegistry.Chapter08.ASide.SID, index);
            }
            
            // Chapter 9 Submaps
            public static class Chapter09
            {
                public const string Submap1 = "Submaps/09_Summit_Sub1";
                public const string Submap2 = "Submaps/09_Summit_Sub2";
                public const string Submap3 = "Submaps/09_Summit_Sub3";
                
                public static MapInfo GetSubmap(int index) => new MapInfo(
                    $"09_Summit_Sub{index}", $"Summit Submap {index}", 9, ChapterSide.ASide, 
                    MapType.Submap, $"submap_09Summit_{index}", $"Submaps/09_Summit_Sub{index}",
                    MapSIDRegistry.Chapter09.ASide.SID, index);
            }
            
            // Chapter 18 Submaps
            public static class Chapter18
            {
                public const string Submap1 = "Submaps/18_Heart_Sub1";
                public const string Submap2 = "Submaps/18_Heart_Sub2";
                public const string Submap3 = "Submaps/18_Heart_Sub3";
                
                public static MapInfo GetSubmap(int index) => new MapInfo(
                    $"18_Heart_Sub{index}", $"Heart Submap {index}", 18, ChapterSide.ASide, 
                    MapType.Submap, $"submap_18Heart_{index}", $"Submaps/18_Heart_Sub{index}",
                    MapSIDRegistry.Chapter18.ASide.SID, index);
            }
        }
        
        #endregion

        #region Lobby SIDs
        
        /// <summary>
        /// Contains all lobby map SIDs.
        /// </summary>
        public static class Lobbies
        {
            public const string BasePath = "Lobbies";
            
            public const string Prologue = "Lobbies/00_Prologue_Lobby";
            public const string Chapter01 = "Lobbies/01_City_Lobby";
            public const string Chapter02 = "Lobbies/02_Nightmare_Lobby";
            public const string Chapter04 = "Lobbies/04_Legend_Lobby";
            public const string Chapter05 = "Lobbies/05_Restore_Lobby";
            public const string Chapter06 = "Lobbies/06_Stronghold_Lobby";
            public const string Chapter07 = "Lobbies/07_Hell_Lobby";
            public const string Chapter08 = "Lobbies/08_Truth_Lobby";
            public const string Chapter09 = "Lobbies/09_Summit_Lobby";
            public const string Chapter10 = "Lobbies/10_Ruins_Lobby";
            public const string Chapter11 = "Lobbies/11_Snow_Lobby";
            public const string Chapter12 = "Lobbies/12_Water_Lobby";
            public const string Chapter13 = "Lobbies/13_Fire_Lobby";
            public const string Chapter14 = "Lobbies/14_Digital_Lobby";
            public const string Chapter15 = "Lobbies/15_Castle_Lobby";
            public const string Chapter16 = "Lobbies/16_Corruption_Lobby";
            public const string Epilogue = "Lobbies/17_Epilogue_Lobby";
            public const string Chapter18 = "Lobbies/18_Heart_Lobby";
            public const string Chapter19 = "Lobbies/19_Space_Lobby";
            public const string Chapter20 = "Lobbies/20_TheEnd_Lobby";
            
            public static MapInfo GetLobby(int chapterNumber, string chapterName) => new MapInfo(
                $"{chapterNumber:D2}_{chapterName}_Lobby", $"{chapterName} Lobby", chapterNumber, 
                ChapterSide.ASide, MapType.Lobby, $"lobby_{chapterNumber:D2}{chapterName}_A", 
                $"Lobbies/{chapterNumber:D2}_{chapterName}_Lobby");
        }
        
        #endregion

        #region Gym SIDs
        
        /// <summary>
        /// Contains all gym/tutorial map SIDs.
        /// </summary>
        public static class Gyms
        {
            public const string BasePath = "Gyms";
            
            public const string Combo = "Gyms/gym_combo";
            public const string SuperWallJump = "Gyms/gym_superwalljump";
            public const string SuperCoyoteJump = "Gyms/gym_super_coyote_jump";
            public const string TooFast = "Gyms/gym_too_fast";
            public const string TooLate = "Gyms/gym_too_late";
            public const string UltraDashfaze = "Gyms/gym_ultra_dashfaze";
            public const string Wavedash = "Gyms/gym_wavedash";
            public const string Wavefaze = "Gyms/gym_wavefaze";
            public const string WavefazingPPT = "Gyms/gym_wavefazingppt";
            
            public static MapInfo GetGym(string gymName) => new MapInfo(
                $"gym_{gymName}", gymName, 0, ChapterSide.ASide, MapType.Gym, 
                $"gym_{gymName}", $"Gyms/gym_{gymName}");
            
            public static readonly List<string> AllGyms = new List<string>
            {
                Combo, SuperWallJump, SuperCoyoteJump, TooFast, TooLate,
                UltraDashfaze, Wavedash, Wavefaze, WavefazingPPT
            };
        }
        
        #endregion

        #region WIP SIDs
        
        /// <summary>
        /// Contains work-in-progress map SIDs.
        /// </summary>
        public static class WIP
        {
            public const string BasePath = "WIP";
            
            public const string KirbyTest = "WIP/kirbytest";
            public const string Test = "WIP/test";
            
            public static MapInfo GetWIP(string name) => new MapInfo(
                $"wip_{name}", name, 0, ChapterSide.ASide, MapType.WIP, 
                name, $"WIP/{name}");
        }
        
        #endregion

        #region Registry Methods
        
        /// <summary>
        /// Gets all main chapter MapInfo objects in story order.
        /// </summary>
        public static List<MapInfo> GetAllMainChapters()
        {
            var chapters = new List<MapInfo>
            {
                Prologue.GetMapInfo(),
                Chapter01.GetASide(), Chapter01.GetBSide(), Chapter01.GetCSide(), Chapter01.GetDSide(),
                Chapter02.GetASide(), Chapter02.GetBSide(), Chapter02.GetCSide(), Chapter02.GetDSide(),
                Chapter03.GetASide(), Chapter03.GetBSide(), Chapter03.GetCSide(), Chapter03.GetDSide(),
                Chapter04.GetASide(), Chapter04.GetBSide(), Chapter04.GetCSide(), Chapter04.GetDSide(),
                Chapter05.GetASide(), Chapter05.GetBSide(), Chapter05.GetCSide(), Chapter05.GetDSide(),
                Chapter06.GetASide(), Chapter06.GetBSide(), Chapter06.GetCSide(), Chapter06.GetDSide(),
                Chapter07.GetASide(), Chapter07.GetBSide(), Chapter07.GetCSide(), Chapter07.GetDSide(),
                Chapter08.GetASide(), Chapter08.GetBSide(), Chapter08.GetCSide(), Chapter08.GetDSide(),
                Chapter09.GetASide(), Chapter09.GetBSide(), Chapter09.GetCSide(), Chapter09.GetDSide(),
                Chapter10.GetASide(), Chapter10.GetBSide(), Chapter10.GetCSide(), Chapter10.GetDSide(),
                Chapter11.GetASide(), Chapter11.GetBSide(), Chapter11.GetCSide(), Chapter11.GetDSide(),
                Chapter12.GetASide(), Chapter12.GetBSide(), Chapter12.GetCSide(), Chapter12.GetDSide(),
                Chapter13.GetASide(), Chapter13.GetBSide(), Chapter13.GetCSide(), Chapter13.GetDSide(),
                Chapter14.GetASide(), Chapter14.GetBSide(), Chapter14.GetCSide(), Chapter14.GetDSide(),
                Chapter15.GetASide(), Chapter15.GetBSide(), Chapter15.GetCSide(), Chapter15.GetDSide(),
                Chapter16.GetASide(),
                Epilogue.GetMapInfo(),
                Chapter18.GetASide(), Chapter18.GetBSide(), Chapter18.GetCSide(), Chapter18.GetDSide(),
                Chapter19.GetASide(),
                Chapter20.GetASide(),
                PostEpilogue.GetMapInfo()
            };
            
            return chapters;
        }
        
        /// <summary>
        /// Gets all A-Side chapters in story order.
        /// </summary>
        public static List<MapInfo> GetAllASides()
        {
            return GetAllMainChapters().Where(m => m.Side == ChapterSide.ASide).ToList();
        }
        
        /// <summary>
        /// Gets a map by its SID.
        /// </summary>
        public static MapInfo GetMapBySID(string sid)
        {
            return GetAllMainChapters().FirstOrDefault(m => m.SID == sid || m.FullSID == sid);
        }
        
        /// <summary>
        /// Gets all submaps for a specific chapter.
        /// </summary>
        public static List<MapInfo> GetSubmapsForChapter(int chapterNumber)
        {
            var submaps = new List<MapInfo>();
            
            switch (chapterNumber)
            {
                case 1:
                    for (int i = 1; i <= 3; i++) submaps.Add(Submaps.Chapter01.GetSubmap(i));
                    break;
                case 2:
                    for (int i = 1; i <= 3; i++) submaps.Add(Submaps.Chapter02.GetSubmap(i));
                    break;
                case 3:
                    submaps.Add(Submaps.Chapter03.GetSubmap(1));
                    break;
                case 5:
                    for (int i = 1; i <= 3; i++) submaps.Add(Submaps.Chapter05.GetSubmap(i));
                    break;
                case 6:
                    for (int i = 1; i <= 3; i++) submaps.Add(Submaps.Chapter06.GetSubmap(i));
                    break;
                case 7:
                    for (int i = 1; i <= 3; i++) submaps.Add(Submaps.Chapter07.GetSubmap(i));
                    break;
                case 8:
                    for (int i = 1; i <= 3; i++) submaps.Add(Submaps.Chapter08.GetSubmap(i));
                    break;
                case 9:
                    for (int i = 1; i <= 3; i++) submaps.Add(Submaps.Chapter09.GetSubmap(i));
                    break;
                case 18:
                    for (int i = 1; i <= 3; i++) submaps.Add(Submaps.Chapter18.GetSubmap(i));
                    break;
            }
            
            return submaps;
        }
        
        /// <summary>
        /// Gets all lobbies.
        /// </summary>
        public static List<MapInfo> GetAllLobbies()
        {
            return new List<MapInfo>
            {
                Lobbies.GetLobby(0, "Prologue"),
                Lobbies.GetLobby(1, "City"),
                Lobbies.GetLobby(2, "Nightmare"),
                Lobbies.GetLobby(4, "Legend"),
                Lobbies.GetLobby(5, "Restore"),
                Lobbies.GetLobby(6, "Stronghold"),
                Lobbies.GetLobby(7, "Hell"),
                Lobbies.GetLobby(8, "Truth"),
                Lobbies.GetLobby(9, "Summit"),
                Lobbies.GetLobby(10, "Ruins"),
                Lobbies.GetLobby(11, "Snow"),
                Lobbies.GetLobby(12, "Water"),
                Lobbies.GetLobby(13, "Fire"),
                Lobbies.GetLobby(14, "Digital"),
                Lobbies.GetLobby(15, "Castle"),
                Lobbies.GetLobby(16, "Corruption"),
                Lobbies.GetLobby(17, "Epilogue"),
                Lobbies.GetLobby(18, "Heart"),
                Lobbies.GetLobby(19, "Space"),
                Lobbies.GetLobby(20, "TheEnd")
            };
        }
        
        /// <summary>
        /// Validates all SIDs are properly formatted.
        /// </summary>
        public static bool ValidateAllSIDs()
        {
            var allMaps = GetAllMainChapters();
            foreach (var map in allMaps)
            {
                if (string.IsNullOrEmpty(map.SID) || string.IsNullOrEmpty(map.FilePath))
                    return false;
            }
            return true;
        }
        
        #endregion
    }
}
