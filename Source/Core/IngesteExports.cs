using System.Reflection;
using DesoloZantas.Core.BossesHelper.Components;
using DesoloZantas.Core.BossesHelper.Entities;
using DesoloZantas.Core.Core.Player;
using MonoMod.ModInterop;
using GlobalSavePointChanger = DesoloZantas.Core.BossesHelper.Helpers.Code.Components.GlobalSavePointChanger;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// MonoMod exports for FrostHelper integration
    /// Allows other mods (including FrostHelper) to interact with Ingeste features
    /// </summary>
    [ModExportName("IngesteExports")]
    public static class IngesteExports
    {
        /// <summary>
        /// Check if Ingeste mod is loaded and initialized
        /// </summary>
        /// <returns>True if Ingeste is available</returns>
        public static bool IsIngesteAvailable()
        {
            return IngesteModule.Instance != null;
        }

        /// <summary>
        /// Get the current Ingeste module instance
        /// </summary>
        /// <returns>IngesteModule instance or null if not loaded</returns>
        public static IngesteModule GetIngesteModule()
        {
            return IngesteModule.Instance;
        }

        /// <summary>
        /// Check if FrostHelper integration is active
        /// </summary>
        /// <returns>True if FrostHelper integration is working</returns>
        public static bool IsFrostHelperIntegrationActive()
        {
            return FrostHelperIntegration.IsFrostHelperAvailable;
        }

        /// <summary>
        /// Get Ingeste version information
        /// </summary>
        /// <returns>Version string</returns>
        public static string GetVersion()
        {
            return IngesteModule.Instance?.Metadata?.Version?.ToString() ?? "Unknown";
        }

        /// <summary>
        /// Check if a specific Ingeste feature is enabled
        /// </summary>
        /// <param name="featureName">Name of the feature to check</param>
        /// <returns>True if the feature is enabled</returns>
        public static bool IsFeatureEnabled(string featureName)
        {
            try
            {
                var settings = IngesteModule.Settings as IngesteModuleSettings;
                if (settings == null) return false;

                return featureName?.ToLowerInvariant() switch
                {
                    "debug" => settings.DebugMode,
                    "livereload" => settings.EnableLiveReload,
                    "showhitboxes" => settings.ShowHitboxes,
                    "verbose" => settings.LogVerbose,
                    _ => false
                };
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, $"Error checking feature state: {featureName}");
                return false;
            }
        }

        /// <summary>
        /// Get current save data for integration purposes
        /// </summary>
        /// <returns>Save data object or null</returns>
        public static object GetSaveData()
        {
            return IngesteModule.SaveData;
        }

        /// <summary>
        /// Get current session data for integration purposes
        /// </summary>
        /// <returns>Session data object or null</returns>
        public static object GetSessionData()
        {
                return IngesteModule.Session;
        }

        /// <summary>
        /// Log a message through Ingeste's logging system
        /// Useful for other mods that want to use Ingeste's enhanced logging
        /// </summary>
        /// <param name="level">Log level (Info, Warn, Error, Debug)</param>
        /// <param name="message">Message to log</param>
        public static void Log(string level, string message)
        {
            try
            {
                switch (level?.ToLowerInvariant())
                {
                    case "info":
                        IngesteLogger.Info($"[EXTERNAL] {message}");
                        break;
                    case "warn":
                    case "warning":
                        IngesteLogger.Warn($"[EXTERNAL] {message}");
                        break;
                    case "error":
                        IngesteLogger.Error($"[EXTERNAL] {message}");
                        break;
                    case "debug":
                        IngesteLogger.Debug($"[EXTERNAL] {message}");
                        break;
                    default:
                        IngesteLogger.Info($"[EXTERNAL] {message}");
                        break;
                }
            }
            catch (Exception ex)
            {
                // Fallback to basic logging if Ingeste logger fails
                Logger.Log(LogLevel.Warn, "IngesteExports", $"Failed to log external message: {ex.Message}");
            }
        }

        /// <summary>
        /// Register a callback for when Ingeste module is loaded
        /// Useful for mods that depend on Ingeste functionality
        /// </summary>
        /// <param name="callback">Action to call when loaded</param>
        public static void RegisterLoadCallback(Action callback)
        {
            try
            {
                if (IngesteModule.Instance != null)
                {
                    // Already loaded, call immediately
                    callback?.Invoke();
                }
                else
                {
                    // Store for later - would need event system for this
                    IngesteLogger.Warn("Callback registration not yet implemented - Ingeste should be loaded before dependent mods");
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Error registering load callback");
            }
        }

        // ===== BossesHelper Integration Exports =====

        public static Component GetGlobalSavePointChangerComponent(object levelNameSource, Vector2 spawnPoint, IntroTypes introType = IntroTypes.Respawn)
        {
            // Convert object to EntityID - assuming it's a string room name or ID
            EntityID id = levelNameSource is EntityID eid ? eid : new EntityID(levelNameSource.ToString(), 0);
            return new GlobalSavePointChanger(id, spawnPoint, introType);
        }

        public static void CreateGlobalSavePointOnEntityOnMethod<T>(T entity, Vector2 spawnPoint, string method,
            IntroTypes spawnType = IntroTypes.Respawn, BindingFlags flags = BindingFlags.Default,
            bool stateMethod = false) where T : Entity
        {
            // For entities, create an EntityID using a derived key
            EntityID id = new EntityID("savepoint_" + entity.GetHashCode(), 0);
            new GlobalSavePointChanger(id, spawnPoint, spawnType)
                .AddToEntityOnMethod(entity, method, flags, stateMethod);
        }

        public static Component GetEntityChainComponent(Entity entity, bool chain, bool remove = false)
        {
            return new EntityChain(entity, chain, remove);
        }

        public static Component GetEntityTimerComponent(float timer, Action<Entity> action)
        {
            return new EntityTimer(timer, action);
        }

        public static Component GetEntityFlaggerComponent(string flag, Action<Entity> action, bool stateNeeded = true, bool resetFlag = true)
        {
            return new EntityFlagger(flag, action, stateNeeded, resetFlag);
        }

        public static Component GetBossHealthTrackerComponent(Func<int> action)
        {
            return new BossHealthTracker(action);
        }

        public static int GetCurrentPlayerHealth()
        {
            if (Engine.Scene.Tracker.GetEntity<HealthSystemManager>() != null)
                return IngesteModule.Session.BossesHelper_CurrentPlayerHealth;
            return -1;
        }

        public static void RecoverPlayerHealth(int amount)
        {
            Engine.Scene.Tracker.GetEntity<HealthSystemManager>()?.RecoverHealth(amount);
        }

        public static void MakePlayerTakeDamage(Vector2 from = default, int amount = 1, bool silent = false, bool stagger = true, bool ignoreCooldown = false)
        {
            // TODO: Re-enable when BossesHelper integration is fixed
            // IngesteModule.BossesHelper_PlayerTakesDamage(from, amount, silent, stagger, ignoreCooldown);
            IngesteLogger.Warn("MakePlayerTakeDamage called but BossesHelper integration is disabled");
        }

        public static void UseFakeDeath()
        {
            IngesteModule.Session.BossesHelper_UseFakeDeath = true;
        }

        public static void ClearFakeDeath()
        {
            IngesteModule.Session.BossesHelper_UseFakeDeath = false;
        }
    }
}





