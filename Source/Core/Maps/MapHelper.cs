using System;
using System.Collections.Generic;
using System.IO;
using Celeste;
using Monocle;

namespace DesoloZantas.Core.Maps
{
    /// <summary>
    /// Provides helper methods to work with map loading and area data
    /// using the new SID registry structure.
    /// </summary>
    public static class MapHelper
    {
        private static bool _initialized = false;
        
        /// <summary>
        /// Initializes the map helper and validates the map structure.
        /// Should be called during mod initialization.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;
            
            // Validate all SIDs are properly configured
            if (!MapSIDRegistry.ValidateAllSIDs())
            {
                Logger.Log(LogLevel.Warn, "DesoloZantas", "Map SID validation failed - some maps may not load correctly");
            }
            
            _initialized = true;
            Logger.Log(LogLevel.Info, "DesoloZantas", "MapHelper initialized successfully");
        }
        
        /// <summary>
        /// Gets the AreaKey for a chapter by its number and side.
        /// </summary>
        public static AreaKey? GetAreaKey(int chapterNumber, MapSIDRegistry.ChapterSide side = MapSIDRegistry.ChapterSide.ASide)
        {
            string sid = MapOrganizer.GetChapterSID(chapterNumber, side);
            return GetAreaKeyBySID(sid);
        }
        
        /// <summary>
        /// Gets the AreaKey for a map by its SID.
        /// </summary>
        public static AreaKey? GetAreaKeyBySID(string sid)
        {
            try
            {
                var areaData = AreaData.Get(sid);
                if (areaData != null)
                {
                    return areaData.ToKey();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "DesoloZantas", $"Failed to get AreaKey for SID '{sid}': {ex.Message}");
            }
            
            return null;
        }
        
        /// <summary>
        /// Checks if a map exists and is loaded.
        /// </summary>
        public static bool MapExists(string sid)
        {
            try
            {
                return AreaData.Get(sid) != null;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Gets the next chapter in story progression.
        /// </summary>
        public static MapSIDRegistry.MapInfo GetNextStoryChapter(int currentChapter)
        {
            int? nextChapter = MapOrganizer.GetNextChapter(currentChapter);
            if (nextChapter.HasValue)
            {
                return MapSIDRegistry.GetAllASides()
                    .Find(m => m.ChapterNumber == nextChapter.Value);
            }
            return null;
        }
        
        /// <summary>
        /// Gets the previous chapter in story progression.
        /// </summary>
        public static MapSIDRegistry.MapInfo GetPreviousStoryChapter(int currentChapter)
        {
            int? prevChapter = MapOrganizer.GetPreviousChapter(currentChapter);
            if (prevChapter.HasValue)
            {
                return MapSIDRegistry.GetAllASides()
                    .Find(m => m.ChapterNumber == prevChapter.Value);
            }
            return null;
        }
        
        /// <summary>
        /// Gets all available sides for a chapter.
        /// </summary>
        public static List<MapSIDRegistry.ChapterSide> GetAvailableSides(int chapterNumber)
        {
            var sides = new List<MapSIDRegistry.ChapterSide>();
            
            foreach (MapSIDRegistry.ChapterSide side in Enum.GetValues(typeof(MapSIDRegistry.ChapterSide)))
            {
                string sid = MapOrganizer.GetChapterSID(chapterNumber, side);
                if (MapExists(sid))
                {
                    sides.Add(side);
                }
            }
            
            return sides;
        }
        
        /// <summary>
        /// Gets all submaps for a chapter that are currently loaded.
        /// </summary>
        public static List<MapSIDRegistry.MapInfo> GetLoadedSubmaps(int chapterNumber)
        {
            var submaps = MapSIDRegistry.GetSubmapsForChapter(chapterNumber);
            return submaps.FindAll(s => MapExists(s.FullSID));
        }
        
        /// <summary>
        /// Transitions to the specified chapter.
        /// </summary>
        public static void TransitionToChapter(int chapterNumber, MapSIDRegistry.ChapterSide side = MapSIDRegistry.ChapterSide.ASide)
        {
            var areaKey = GetAreaKey(chapterNumber, side);
            if (areaKey.HasValue)
            {
                LevelEnter.Go(new Session(areaKey.Value), false);
            }
            else
            {
                Logger.Log(LogLevel.Error, "DesoloZantas", $"Cannot transition to Chapter {chapterNumber} {side} - map not found");
            }
        }
        
        /// <summary>
        /// Checks if the current chapter is a special chapter (prologue, epilogue, post-epilogue).
        /// </summary>
        public static bool IsSpecialChapter(int chapterNumber)
        {
            return chapterNumber == 0 || chapterNumber == 17 || chapterNumber == 21;
        }
        
        /// <summary>
        /// Gets the chapter type for a given chapter number.
        /// </summary>
        public static MapSIDRegistry.MapType GetChapterType(int chapterNumber)
        {
            return chapterNumber switch
            {
                0 => MapSIDRegistry.MapType.Prologue,
                17 => MapSIDRegistry.MapType.Epilogue,
                21 => MapSIDRegistry.MapType.PostEpilogue,
                _ => MapSIDRegistry.MapType.MainChapter
            };
        }
        
        /// <summary>
        /// Gets the display name for a chapter.
        /// </summary>
        public static string GetChapterDisplayName(int chapterNumber, MapSIDRegistry.ChapterSide side = MapSIDRegistry.ChapterSide.ASide)
        {
            string baseName = MapOrganizer.GetChapterName(chapterNumber);
            
            if (IsSpecialChapter(chapterNumber))
            {
                return baseName;
            }
            
            string sideStr = side switch
            {
                MapSIDRegistry.ChapterSide.ASide => "A-Side",
                MapSIDRegistry.ChapterSide.BSide => "B-Side",
                MapSIDRegistry.ChapterSide.CSide => "C-Side",
                MapSIDRegistry.ChapterSide.DSide => "D-Side",
                _ => ""
            };
            
            return $"Chapter {chapterNumber}: {baseName} - {sideStr}";
        }
        
        /// <summary>
        /// Gets the story progression percentage based on completed chapters.
        /// </summary>
        public static float GetStoryProgressPercentage(HashSet<int> completedChapters)
        {
            int totalChapters = MapOrganizer.StoryOrder.Length;
            int completed = 0;
            
            foreach (int chapter in MapOrganizer.StoryOrder)
            {
                if (completedChapters.Contains(chapter))
                {
                    completed++;
                }
            }
            
            return (float)completed / totalChapters * 100f;
        }
    }
}
