namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Manages external area data loading for the DesoloZantas campaign
    /// NOTE: This manager is DISABLED by default because Everest handles area loading 
    /// automatically via .meta.yaml files. Only enable if custom area registration is needed.
    /// </summary>
    public static class ExternalAreaDataManager
    {
        private static bool isLoaded = false;
        private static readonly List<AreaData> registeredAreas = new List<AreaData>();
        
        /// <summary>
        /// Set to true to enable manual area loading (not recommended when using .meta.yaml files)
        /// </summary>
        public static bool EnableManualLoading { get; set; } = false;

        /// <summary>
        /// Load and register all external areas for the DesoloZantas campaign
        /// </summary>
        public static void LoadExternalAreas()
        {
            // Skip loading - Everest handles this via .meta.yaml files
            if (!EnableManualLoading)
            {
                IngesteLogger.Debug("ExternalAreaDataManager: Skipping manual area loading (using Everest's .meta.yaml system)");
                return;
            }
            
            if (isLoaded)
            {
                IngesteLogger.Debug("External areas already loaded, skipping");
                return;
            }

            try
            {
                IngesteLogger.Info("Loading external area data for DesoloZantas campaign");

                // Clear any previously registered areas
                registeredAreas.Clear();

                // Load areas from areadata file
                string modRoot = IngesteModule.Instance.Metadata?.PathDirectory ?? ".";
                string areadataPath = Path.Combine(modRoot, "Maps", "areadata");
                if (File.Exists(areadataPath))
                {
                    LoadAreasFromFile(areadataPath);
                }
                else
                {
                    IngesteLogger.Warn($"Areadata file not found at: {areadataPath}");
                    LoadAreasManually();
                }

                isLoaded = true;
                IngesteLogger.Info($"Successfully loaded {registeredAreas.Count} external areas");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to load external areas");
            }
        }

        /// <summary>
        /// Load areas from the areadata file
        /// </summary>
        private static void LoadAreasFromFile(string areadataPath)
        {
            var lines = File.ReadAllLines(areadataPath);
            var currentArea = new Dictionary<int, string>(); // mode -> map path
            string currentChapterName = "";
            int currentChapterIndex = 0;

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                
                // Skip empty lines and comments
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                {
                    // Extract chapter name from comments
                    if (trimmedLine.StartsWith("# Chapter ") && trimmedLine.Contains(":"))
                    {
                        var parts = trimmedLine.Substring(2).Split(':');
                        if (parts.Length >= 2)
                        {
                            currentChapterName = parts[1].Trim();
                            var chapterPart = parts[0].Trim();
                            if (chapterPart.StartsWith("Chapter "))
                            {
                                if (int.TryParse(chapterPart.Substring(8), out var chapterNum))
                                {
                                    currentChapterIndex = chapterNum;
                                }
                            }
                        }
                    }
                    continue;
                }

                // Parse mode lines: "Mode 0 Maggy/Main/00Prologue_A"
                if (trimmedLine.StartsWith("Mode "))
                {
                    var parts = trimmedLine.Split(' ');
                    if (parts.Length >= 3)
                    {
                        if (int.TryParse(parts[1], out var mode))
                        {
                            var mapPath = string.Join(" ", parts, 2, parts.Length - 2);
                            
                            // If this is mode 0 and we have previous area data, create the area
                            if (mode == 0 && currentArea.Count > 0)
                            {
                                CreateAreaFromModes(currentArea, currentChapterName, currentChapterIndex);
                                currentArea.Clear();
                                currentChapterIndex++;
                            }

                            currentArea[mode] = mapPath;
                        }
                    }
                }
            }

            // Create the last area if there's remaining data
            if (currentArea.Count > 0)
            {
                CreateAreaFromModes(currentArea, currentChapterName, currentChapterIndex);
            }
        }

        /// <summary>
        /// Create an AreaData from the collected mode information
        /// </summary>
        private static void CreateAreaFromModes(Dictionary<int, string> modes, string chapterName, int chapterIndex)
        {
            if (modes == null || !modes.ContainsKey(0))
            {
                IngesteLogger.Warn($"No A-side found for chapter {chapterIndex}: {chapterName}");
                return;
            }

            // Safety check - don't add to AreaData.Areas if it's null
            if (AreaData.Areas == null)
            {
                IngesteLogger.Error($"AreaData.Areas is null - cannot register areas");
                return;
            }

            try
            {
                var aSidePath = modes[0];
                if (string.IsNullOrEmpty(aSidePath))
                {
                    IngesteLogger.Warn($"Empty A-side path for chapter {chapterIndex}: {chapterName}");
                    return;
                }
                
                var areaKey = GetAreaKeyFromPath(aSidePath);
                if (string.IsNullOrEmpty(areaKey))
                {
                    areaKey = $"Chapter{chapterIndex}";
                }
                
                IngesteLogger.Debug($"Creating area for {areaKey} ({chapterName})");

                // Create the base AreaData
                var areaData = new AreaData
                {
                    IntroType = global::Celeste.Player.IntroTypes.Jump,
                    Dreaming = false,
                    ID = registeredAreas.Count,
                    Name = areaKey,
                    Icon = $"areas/{areaKey.ToLower()}",
                    Interlude = false,
                    CanFullClear = true,
                    IsFinal = chapterIndex >= 20,
                    TitleBaseColor = Color.White,
                    TitleAccentColor = Color.Gray,
                    TitleTextColor = Color.White
                };

                // Add modes
                var modesList = new List<ModeProperties>();
                
                // Add A-side (always exists)
                modesList.Add(CreateModeProperties(modes[0], 0));

                // Add B-side if exists
                if (modes.ContainsKey(1))
                {
                    modesList.Add(CreateModeProperties(modes[1], 1));
                }

                // Add C-side if exists
                if (modes.ContainsKey(2))
                {
                    modesList.Add(CreateModeProperties(modes[2], 2));
                }

                // Add D-side if exists (custom for this mod)
                if (modes.ContainsKey(3))
                {
                    modesList.Add(CreateModeProperties(modes[3], 3));
                }

                areaData.Mode = modesList.ToArray();

                // Register the area
                AreaData.Areas.Add(areaData);
                registeredAreas.Add(areaData);

                IngesteLogger.Info($"Registered area: {areaKey} with {modesList.Count} modes");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, $"Failed to create area for chapter {chapterIndex}: {chapterName}");
            }
        }

        /// <summary>
        /// Create ModeProperties for a specific mode
        /// </summary>
        private static ModeProperties CreateModeProperties(string mapPath, int mode)
        {
            var modeProps = new ModeProperties
            {
                Path = mapPath,
                Inventory = PlayerInventory.Default,
                AudioState = new AudioState($"event:/Ingeste/music/lvl{mode}/intro", $"event:/Ingeste/env/{GetAreaKeyFromPath(mapPath).ToLower()}")
            };

            // Try to load additional metadata from .meta.yaml file
            try
            {
                string modRoot = IngesteModule.Instance.Metadata?.PathDirectory ?? ".";
                string metaPath = System.IO.Path.Combine(modRoot, "Maps", mapPath + ".meta.yaml");
                
                if (System.IO.File.Exists(metaPath))
                {
                    IngesteLogger.Debug($"Loading metadata from: {metaPath}");
                    // TODO: Parse YAML metadata to override defaults
                    // This would require a YAML parser or manual parsing
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Failed to load metadata for {mapPath}: {ex.Message}");
            }

            return modeProps;
        }

        /// <summary>
        /// Extract area key from map path (e.g., "Maggy/Main/00Prologue_A" -> "prologue")
        /// </summary>
        private static string GetAreaKeyFromPath(string mapPath)
        {
            var fileName = Path.GetFileNameWithoutExtension(mapPath);
            if (fileName.Contains("_"))
            {
                var parts = fileName.Split('_');
                if (parts.Length >= 2)
                {
                    // Remove numeric prefix (e.g., "00Prologue" -> "Prologue")
                    var name = parts[0];
                    var nameWithoutNumbers = "";
                    for (int i = 0; i < name.Length; i++)
                    {
                        if (!char.IsDigit(name[i]))
                        {
                            nameWithoutNumbers = name.Substring(i);
                            break;
                        }
                    }
                    return string.IsNullOrEmpty(nameWithoutNumbers) ? name : nameWithoutNumbers;
                }
            }
            return fileName;
        }

        /// <summary>
        /// Manually load areas if areadata file is not available
        /// </summary>
        private static void LoadAreasManually()
        {
            IngesteLogger.Info("Loading areas manually (fallback method)");

            var manualAreas = new[]
            {
                ("Prologue", "Maggy/Main/00Prologue_A"),
                ("City", "Maggy/Main/01City_A"),
                ("Nightmare", "Maggy/Main/02Nightmare_A"),
                ("Stars", "Maggy/Main/03Stars_A"),
                ("Legend", "Maggy/Main/04Legend_A"),
                ("Restore", "Maggy/Main/05Restore_A"),
                ("Stronghold", "Maggy/Main/06Stronghold_A"),
                ("Hell", "Maggy/Main/07Hell_A"),
                ("Truth", "Maggy/Main/08Truth_A"),
                ("Summit", "Maggy/Main/09Summit_A"),
                ("Ruins", "Maggy/Main/10Ruins_A"),
                ("Snow", "Maggy/Main/11Snow_A"),
                ("Water", "Maggy/Main/12Water_A"),
                ("Time", "Maggy/Main/13Time_A"),
                ("Digital", "Maggy/Main/14Digital_A"),
                ("Castle", "Maggy/Main/15Castle_A"),
                ("Corruption", "Maggy/Main/16Corruption_A"),
                ("Epilogue", "Maggy/Main/17Epilogue_A"),
                ("Heart", "Maggy/Main/18Heart_A"),
                ("Space", "Maggy/Main/19Space_A"),
                ("TheEnd", "Maggy/Main/20TheEnd_A"),
                ("Respite", "Maggy/Main/21Respite_A") // Placeholder
            };

            for (int i = 0; i < manualAreas.Length; i++)
            {
                var (name, path) = manualAreas[i];
                var modes = new Dictionary<int, string> { { 0, path } };
                CreateAreaFromModes(modes, name, i);
            }
        }

        /// <summary>
        /// Unload all registered external areas
        /// </summary>
        public static void UnloadExternalAreas()
        {
            if (!isLoaded)
                return;

            try
            {
                IngesteLogger.Info("Unloading external areas");

                // Remove registered areas from AreaData.Areas
                foreach (var area in registeredAreas)
                {
                    AreaData.Areas.Remove(area);
                }

                registeredAreas.Clear();
                isLoaded = false;

                IngesteLogger.Info("External areas unloaded successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to unload external areas");
            }
        }

        /// <summary>
        /// Get all registered external areas
        /// </summary>
        public static IReadOnlyList<AreaData> GetRegisteredAreas()
        {
            return registeredAreas.AsReadOnly();
        }

        /// <summary>
        /// Check if external areas are loaded
        /// </summary>
        public static bool IsLoaded => isLoaded;
    }
}



