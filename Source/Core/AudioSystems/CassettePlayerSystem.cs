using System.Runtime.InteropServices;
using FMOD;
using INITFLAGS = FMOD.Studio.INITFLAGS;

namespace DesoloZantas.Core.Core.AudioSystems
{
    /// <summary>
    /// Manages the Cassette Player FMOD DSP plugin integration.
    /// This system loads platform-specific native FMOD plugins that provide
    /// cassette tape audio effects (wow/flutter, tape hiss, etc.).
    /// </summary>
    public static class CassettePlayerSystem
    {
        private static bool isLoaded = false;
        private static uint pluginHandle = 0;

        /// <summary>
        /// Hook for FMOD Studio System initialization to load the cassette player plugin.
        /// This should be added in the module's Load() method.
        /// </summary>
        public static RESULT OnFmodStudioSystemInit(
            On.FMOD.Studio.System.orig_initialize orig,
            FMOD.Studio.System self,
            int maxchannels,
            INITFLAGS studioFlags,
            FMOD.INITFLAGS flags,
            IntPtr extradriverdata)
        {
            var result = orig(self, maxchannels, studioFlags, flags, extradriverdata);

            if (result == RESULT.OK)
            {
                try
                {
                    LoadCassettePlayerPlugin(self);
                }
                catch (Exception ex)
                {
                    IngesteLogger.Error(ex, "Failed to load cassette player FMOD plugin");
                    // Don't fail the entire initialization, just log the error
                }
            }

            return result;
        }

        /// <summary>
        /// Loads the platform-specific cassette player FMOD DSP plugin.
        /// </summary>
        private static void LoadCassettePlayerPlugin(FMOD.Studio.System studioSystem)
        {
            if (isLoaded)
            {
                IngesteLogger.Info("Cassette player plugin already loaded");
                return;
            }

            // Get the low-level FMOD system
            var result = studioSystem.getLowLevelSystem(out var llSys);
            if (result != RESULT.OK)
            {
                IngesteLogger.Error($"Failed to get low-level FMOD system: {result}");
                return;
            }

            // Determine the correct plugin filename based on platform and architecture
            string pluginFileName = GetPlatformPluginFileName();
            
            if (string.IsNullOrEmpty(pluginFileName))
            {
                IngesteLogger.Warn("Cassette player plugin not supported on this platform");
                return;
            }

            // Get the path to the plugin using Everest's content system
            string pluginPath = Everest.Content.Get($"Mods/DesoloZatnas/Libs/{pluginFileName}")?.GetCachedPath();
            
            if (string.IsNullOrEmpty(pluginPath))
            {
                IngesteLogger.Warn($"Cassette player plugin not found at Libs/{pluginFileName}");
                return;
            }

            // Load the plugin into FMOD
            var loadResult = llSys.loadPlugin(pluginPath, out pluginHandle);
            
            if (loadResult == RESULT.OK)
            {
                isLoaded = true;
                IngesteLogger.Info($"Loaded cassette player FMOD DSP plugin: {pluginFileName}");
            }
            else
            {
                IngesteLogger.Error($"Failed to load cassette player plugin from {pluginPath}: {loadResult}");
            }
        }

        /// <summary>
        /// Determines the correct plugin filename based on the current platform and architecture.
        /// </summary>
        private static string GetPlatformPluginFileName()
        {
            // Determine architecture
            string arch = RuntimeInformation.OSArchitecture is Architecture.X86 or Architecture.X64
                ? "x86"
                : "arm";

            // Determine OS and return appropriate filename
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "cassette_player.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "libcassette_player.so";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return $"libcassette_player.{arch}.dylib";
            }
            
            // Unsupported platform
            return null;
        }

        /// <summary>
        /// Unloads the cassette player plugin.
        /// This should be called in the module's Unload() method.
        /// </summary>
        public static void Unload()
        {
            if (!isLoaded)
            {
                return;
            }

            try
            {
                // Note: FMOD plugins are automatically unloaded when the FMOD system shuts down
                // We just need to reset our tracking state
                isLoaded = false;
                pluginHandle = 0;
                IngesteLogger.Info("Cassette player plugin unloaded");
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Error during cassette player plugin unload: {ex.Message}");
            }
        }

        /// <summary>
        /// Registers hooks for the cassette player system.
        /// Call this from IngesteModule.Load().
        /// </summary>
        public static void RegisterHooks()
        {
            On.FMOD.Studio.System.initialize += OnFmodStudioSystemInit;
            IngesteLogger.Info("Cassette player hooks registered");
        }

        /// <summary>
        /// Unregisters hooks for the cassette player system.
        /// Call this from IngesteModule.Unload().
        /// </summary>
        public static void UnregisterHooks()
        {
            On.FMOD.Studio.System.initialize -= OnFmodStudioSystemInit;
            IngesteLogger.Info("Cassette player hooks unregistered");
        }
    }
}




