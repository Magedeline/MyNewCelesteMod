#nullable enable
using System.Text.Json;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Reads the launch mode configuration set by the CelesteDesoloZantas launcher.
    /// This allows the mod to behave differently based on how the game was launched.
    /// </summary>
    public static class LaunchModeReader
    {
        private const string LaunchModeFileName = ".launch_mode";
        private static LaunchModeData? _cachedData;
        private static bool _hasRead = false;

        /// <summary>
        /// Whether the game was launched in Desolo Zantas mode
        /// </summary>
        public static bool IsDesoloMode
        {
            get
            {
                EnsureLoaded();
                return _cachedData?.Mode == "DesoloZantas";
            }
        }

        /// <summary>
        /// Whether the custom title screen should be skipped
        /// </summary>
        public static bool ShouldSkipCustomTitleScreen
        {
            get
            {
                EnsureLoaded();
                return _cachedData?.SkipCustomTitleScreen ?? false;
            }
        }

        /// <summary>
        /// Whether the launcher was used (launch mode file exists)
        /// </summary>
        public static bool WasLaunchedViaLauncher
        {
            get
            {
                EnsureLoaded();
                return _cachedData != null;
            }
        }

        /// <summary>
        /// Get the launch time if available
        /// </summary>
        public static DateTime? LaunchTime
        {
            get
            {
                EnsureLoaded();
                if (_cachedData?.LaunchTime != null && DateTime.TryParse(_cachedData.LaunchTime, out var dt))
                {
                    return dt;
                }
                return null;
            }
        }

        /// <summary>
        /// Force reload the launch mode data
        /// </summary>
        public static void Reload()
        {
            _hasRead = false;
            _cachedData = null;
            EnsureLoaded();
        }

        /// <summary>
        /// Clear the launch mode file after reading (optional cleanup)
        /// </summary>
        public static void ClearLaunchModeFile()
        {
            try
            {
                string? modPath = GetModPath();
                if (modPath == null) return;

                string launchFilePath = Path.Combine(modPath, LaunchModeFileName);
                if (File.Exists(launchFilePath))
                {
                    File.Delete(launchFilePath);
                    IngesteLogger.Info("Launch mode file cleared");
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Failed to clear launch mode file: {ex.Message}");
            }
        }

        private static void EnsureLoaded()
        {
            if (_hasRead) return;
            _hasRead = true;

            try
            {
                string? modPath = GetModPath();
                if (modPath == null)
                {
                    IngesteLogger.Warn("Could not determine mod path for launch mode detection");
                    return;
                }

                string launchFilePath = Path.Combine(modPath, LaunchModeFileName);
                
                if (!File.Exists(launchFilePath))
                {
                    IngesteLogger.Info("No launch mode file found - game was launched directly");
                    return;
                }

                string json = File.ReadAllText(launchFilePath);
                _cachedData = JsonSerializer.Deserialize<LaunchModeData>(json);

                if (_cachedData != null)
                {
                    IngesteLogger.Info($"Launch mode detected: {_cachedData.Mode} (SkipCustomTitleScreen: {_cachedData.SkipCustomTitleScreen})");
                    
                    // Check if launch file is stale (more than 5 minutes old)
                    if (LaunchTime.HasValue)
                    {
                        var age = DateTime.UtcNow - LaunchTime.Value;
                        if (age.TotalMinutes > 5)
                        {
                            IngesteLogger.Warn($"Launch mode file is {age.TotalMinutes:F1} minutes old - treating as stale");
                            _cachedData = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Failed to read launch mode: {ex.Message}");
                _cachedData = null;
            }
        }

        private static string? GetModPath()
        {
            try
            {
                // Try to get path from assembly location
                string? assemblyPath = typeof(LaunchModeReader).Assembly.Location;
                if (!string.IsNullOrEmpty(assemblyPath))
                {
                    // Assembly is in Code/net8.0/, mod root is two directories up
                    string? codeDir = Path.GetDirectoryName(assemblyPath);
                    if (codeDir != null)
                    {
                        string? modRoot = Path.GetDirectoryName(Path.GetDirectoryName(codeDir));
                        if (modRoot != null && Directory.Exists(modRoot))
                        {
                            return modRoot;
                        }
                    }
                }

                // Fallback: try to get from Everest module metadata
                if (IngesteModule.Instance?.Metadata?.PathDirectory != null)
                {
                    return IngesteModule.Instance.Metadata.PathDirectory;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Internal class to deserialize the launch mode JSON
        /// </summary>
        private class LaunchModeData
        {
            public string? Mode { get; set; }
            public string? LaunchTime { get; set; }
            public bool SkipCustomTitleScreen { get; set; }
        }
    }
}
