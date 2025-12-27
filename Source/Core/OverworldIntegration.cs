using System;
using Celeste.Mod;
using MonoMod.ModInterop;

namespace Celeste.Mod.DesoloZatnas
{
    /// <summary>
    /// Overworld integration - Custom OUI system for DesoloZatnas
    /// Note: OUI screens have been removed - this is a stub implementation
    /// </summary>
    public class OverworldIntegration
    {
        private static bool hooksInstalled = false;

        /// <summary>
        /// Call this from your EverestModule.Load() method
        /// </summary>
        public static void InstallHooks()
        {
            if (hooksInstalled)
                return;

            Logger.Log(LogLevel.Info, "DesoloZatnas", "[OverworldIntegration] Installing custom OUI system");

            On.Celeste.Overworld.ReloadMenus += Overworld_ReloadMenus;
            
            // OUI screens have been removed

            hooksInstalled = true;
        }

        /// <summary>
        /// Call this from your EverestModule.Unload() method
        /// </summary>
        public static void UninstallHooks()
        {
            if (!hooksInstalled)
                return;

            Logger.Log(LogLevel.Info, "DesoloZatnas", "[OverworldIntegration] Uninstalling custom OUI system");

            On.Celeste.Overworld.ReloadMenus -= Overworld_ReloadMenus;
            
            // OUI screens have been removed

            hooksInstalled = false;
        }

        private static void Overworld_ReloadMenus(On.Celeste.Overworld.orig_ReloadMenus orig, Overworld self, Overworld.StartMode startMode)
        {
            // Call original to register vanilla UI first
            orig(self, startMode);

            Logger.Log(LogLevel.Info, "DesoloZatnas", $"[OverworldIntegration] StartMode={startMode}");
            
            // OUI screens have been removed - using vanilla screens
        }
    }
}
