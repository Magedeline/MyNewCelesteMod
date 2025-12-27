using System;
using Celeste.Mod;
using Celeste.Mod.DesoloZatnas.Core;
using Celeste.Mod.DesoloZatnas.Core.UI;
using Celeste.Mod.DesoloZatnas.Core.Integration;
using MonoMod.ModInterop;

namespace Celeste.Mod.DesoloZatnas
{
    /// <summary>
    /// Overworld integration - Custom OUI system for DesoloZatnas
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
            
            // Install in-game level integration hooks
            LevelIntegration.InstallHooks();
            
            // Install chapter select icon spacing hooks
            OuiChapterSelectIconHooks.Install();

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
            
            // Uninstall in-game level integration hooks
            LevelIntegration.UninstallHooks();
            
            // Uninstall chapter select icon spacing hooks
            OuiChapterSelectIconHooks.Uninstall();

            hooksInstalled = false;
        }

        private static void Overworld_ReloadMenus(On.Celeste.Overworld.orig_ReloadMenus orig, Overworld self, Overworld.StartMode startMode)
        {
            // Call original to register vanilla UI first
            orig(self, startMode);

            Logger.Log(LogLevel.Info, "DesoloZatnas", $"[OverworldIntegration] Registering custom OUI screens, StartMode={startMode}");
            
            // Register custom OUI screens
            RegisterOuiIfNotExists<OuiMainMenuDesoloZatnas>(self);
            RegisterOuiIfNotExists<OuiChapterSelectDesoloZatnas>(self);
            RegisterOuiIfNotExists<OuiStatisticsNotebook>(self);
            RegisterOuiIfNotExists<OuiDSidePostcard>(self);
            RegisterOuiIfNotExists<OuiCreditsDesoloZatnas>(self);
            
            Logger.Log(LogLevel.Info, "DesoloZatnas", "[OverworldIntegration] Custom OUI screens registered");
        }

        private static void RegisterOuiIfNotExists<T>(Overworld overworld) where T : Oui
        {
            // Check if this OUI type already exists
            foreach (Oui ui in overworld.UIs)
            {
                if (ui is T)
                {
                    Logger.Log(LogLevel.Verbose, "DesoloZatnas", $"[OverworldIntegration] {typeof(T).Name} already exists");
                    return;
                }
            }

            // Register new OUI
            overworld.RegisterOui(typeof(T));
            Logger.Log(LogLevel.Info, "DesoloZatnas", $"[OverworldIntegration] Registered {typeof(T).Name}");
        }
    }
}
