namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Extension class for AreaData that provides additional functionality for tape management
    /// and enhanced area lookup capabilities
    /// </summary>
    public static class AreaDataExt
    {
        private static Dictionary<string, global::Celeste.AreaData> areaCache = new Dictionary<string, global::Celeste.AreaData>();
        private static bool cacheInitialized = false;

        /// <summary>
        /// Initializes the area cache for faster lookups
        /// </summary>
        public static void Initialize()
        {
            if (cacheInitialized) return;

            Logger.Log(LogLevel.Info, nameof(AreaDataExt), "Initializing AreaData cache...");

            areaCache.Clear();

            foreach (var area in global::Celeste.AreaData.Areas)
            {
                if (area?.SID != null)
                {
                    areaCache[area.SID] = area;
                }
            }

            cacheInitialized = true;
            Logger.Log(LogLevel.Info, nameof(AreaDataExt),
                $"AreaData cache initialized with {areaCache.Count} areas");
        }

        /// <summary>
        /// Clears the area cache and forces reinitialization on next access
        /// </summary>
        public static void ClearCache()
        {
            areaCache.Clear();
            cacheInitialized = false;
        }

        /// <summary>
        /// Gets an AreaData by its SID (String ID)
        /// </summary>
        /// <param name="sid">The String ID of the area</param>
        /// <param name="areaData">The found AreaData, or null if not found</param>
        /// <returns>True if the area was found, false otherwise</returns>
        public static bool Get(string sid, out global::Celeste.AreaData areaData)
        {
            if (!cacheInitialized)
            {
                Initialize();
            }

            if (string.IsNullOrEmpty(sid))
            {
                areaData = null;
                return false;
            }

            if (areaCache.TryGetValue(sid, out areaData))
            {
                return true;
            }

            // Fallback: search through AreaData.Areas directly
            areaData = global::Celeste.AreaData.Areas.FirstOrDefault(area => area?.SID == sid);

            // Cache the result for future lookups
            if (areaData != null)
            {
                areaCache[sid] = areaData;
            }

            return areaData != null;
        }

        /// <summary>
        /// Gets an AreaData by its SID (String ID) - alternative method signature
        /// </summary>
        /// <param name="sid">The String ID of the area</param>
        /// <returns>The AreaData if found, null otherwise</returns>
        public static global::Celeste.AreaData Get(string sid)
        {
            Get(sid, out var areaData);
            return areaData;
        }

        /// <summary>
        /// Gets an AreaData by its numeric ID
        /// </summary>
        /// <param name="id">The numeric ID of the area</param>
        /// <param name="areaData">The found AreaData, or null if not found</param>
        /// <returns>True if the area was found, false otherwise</returns>
        public static bool Get(int id, out global::Celeste.AreaData areaData)
        {
            if (id >= 0 && id < global::Celeste.AreaData.Areas.Count)
            {
                areaData = global::Celeste.AreaData.Areas[id];
                return areaData != null;
            }

            areaData = null;
            return false;
        }

        /// <summary>
        /// Checks if an area exists by its SID
        /// </summary>
        /// <param name="sid">The String ID of the area</param>
        /// <returns>True if the area exists, false otherwise</returns>
        public static bool Exists(string sid)
        {
            return Get(sid, out _);
        }

        /// <summary>
        /// Checks if an area exists by its numeric ID
        /// </summary>
        /// <param name="id">The numeric ID of the area</param>
        /// <returns>True if the area exists, false otherwise</returns>
        public static bool Exists(int id)
        {
            return Get(id, out _);
        }

        /// <summary>
        /// Gets all areas that match a specific pattern in their SID
        /// </summary>
        /// <param name="pattern">The pattern to match (supports wildcards)</param>
        /// <returns>List of matching AreaData objects</returns>
        public static List<global::Celeste.AreaData> GetByPattern(string pattern)
        {
            if (!cacheInitialized)
            {
                Initialize();
            }

            var results = new List<global::Celeste.AreaData>();

            if (string.IsNullOrEmpty(pattern))
            {
                return results;
            }

            // Simple wildcard matching
            bool isWildcard = pattern.Contains('*');

            foreach (var kvp in areaCache)
            {
                if (isWildcard)
                {
                    if (MatchesWildcard(kvp.Key, pattern))
                    {
                        results.Add(kvp.Value);
                    }
                }
                else
                {
                    if (kvp.Key.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        results.Add(kvp.Value);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets all areas from a specific mod
        /// </summary>
        /// <param name="modName">The name of the mod</param>
        /// <returns>List of AreaData objects from the specified mod</returns>
        public static List<global::Celeste.AreaData> GetByMod(string modName)
        {
            if (!cacheInitialized)
            {
                Initialize();
            }

            var results = new List<global::Celeste.AreaData>();

            if (string.IsNullOrEmpty(modName))
            {
                return results;
            }

            foreach (var area in areaCache.Values)
            {
                // Check if the area SID starts with the mod name
                if (area.SID?.StartsWith(modName, StringComparison.OrdinalIgnoreCase) == true)
                {
                    results.Add(area);
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the SID from an AreaKey
        /// </summary>
        /// <param name="areaKey">The AreaKey to get the SID from</param>
        /// <returns>The SID of the area, or null if not found</returns>
        public static string GetSid(AreaKey areaKey)
        {
            if (Get(areaKey.ID, out var areaData))
            {
                return areaData.SID;
            }

            return null;
        }

        /// <summary>
        /// Creates an AreaKey from a SID and mode
        /// </summary>
        /// <param name="sid">The String ID of the area</param>
        /// <param name="mode">The area mode (optional, defaults to Normal)</param>
        /// <returns>The AreaKey if the area exists, null otherwise</returns>
        public static AreaKey? CreateAreaKey(string sid, AreaMode mode = AreaMode.Normal)
        {
            if (Get(sid, out var areaData))
            {
                return new AreaKey(areaData.ID, mode);
            }

            return null;
        }

        /// <summary>
        /// Checks if an area supports a specific mode
        /// </summary>
        /// <param name="sid">The String ID of the area</param>
        /// <param name="mode">The mode to check</param>
        /// <returns>True if the area supports the mode, false otherwise</returns>
        public static bool SupportsMode(string sid, AreaMode mode)
        {
            if (Get(sid, out var areaData))
            {
                return areaData.HasMode(mode);
            }

            return false;
        }

        /// <summary>
        /// Gets the display name of an area by its SID
        /// </summary>
        /// <param name="sid">The String ID of the area</param>
        /// <returns>The display name of the area, or the SID if not found</returns>
        public static string GetDisplayName(string sid)
        {
            if (Get(sid, out var areaData))
            {
                return areaData.Name;
            }

            return sid;
        }

        /// <summary>
        /// Simple wildcard matching helper
        /// </summary>
        private static bool MatchesWildcard(string text, string pattern)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern))
                return false;

            // Convert to regex-like pattern
            string regexPattern = "^" + pattern.Replace("*", ".*").Replace("?", ".") + "$";

            try
            {
                return System.Text.RegularExpressions.Regex.IsMatch(text, regexPattern,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }
            catch
            {
                // Fallback to simple contains check
                return text.IndexOf(pattern.Replace("*", ""), StringComparison.OrdinalIgnoreCase) >= 0;
            }
        }

        /// <summary>
        /// Refreshes the cache with current AreaData information
        /// </summary>
        public static void RefreshCache()
        {
            ClearCache();
            Initialize();
        }

        /// <summary>
        /// Gets all cached area SIDs
        /// </summary>
        /// <returns>Collection of all cached area SIDs</returns>
        public static IEnumerable<string> GetAllSiDs()
        {
            if (!cacheInitialized)
            {
                Initialize();
            }

            return areaCache.Keys;
        }

        /// <summary>
        /// Gets all cached areas
        /// </summary>
        /// <returns>Collection of all cached AreaData objects</returns>
        public static IEnumerable<global::Celeste.AreaData> GetAllAreas()
        {
            if (!cacheInitialized)
            {
                Initialize();
            }

            return areaCache.Values;
        }
    }
}



