using MonoMod.RuntimeDetour;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Centralized hook manager that ensures all hooks are properly scoped to this mod only.
    /// Prevents interference with vanilla Celeste and other mods.
    /// </summary>
    public static class HookManager
    {
        private static readonly List<IDetour> ActiveHooks = new List<IDetour>();
        private static bool hooksRegistered = false;

        // Mod identification constant - all mod content should use this prefix
        private const string MOD_SID_PREFIX = "DesoloZantas/";

        /// <summary>
        /// Checks if the current session is in a DesoloZantas map
        /// </summary>
        public static bool IsModMap(Level level)
        {
            if (level == null || level.Session == null)
                return false;

            string sid = level.Session.Area.GetSID();
            return sid != null && sid.StartsWith(MOD_SID_PREFIX, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Checks if an area SID belongs to this mod
        /// </summary>
        public static bool IsModArea(string sid)
        {
            return sid != null && sid.StartsWith(MOD_SID_PREFIX, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Register all mod hooks. This method is idempotent.
        /// </summary>
        public static void RegisterHooks()
        {
            if (hooksRegistered)
            {
                IngesteLogger.Warn("Hooks already registered, skipping duplicate registration");
                return;
            }

            try
            {
                IngesteLogger.Info("Registering mod-scoped hooks");

                // Register Level hooks with mod-scoping
                On.Celeste.Level.LoadLevel += ModScoped_Level_LoadLevel;
                On.Celeste.Level.Begin += ModScoped_Level_Begin;
                On.Celeste.Level.End += ModScoped_Level_End;
                On.Celeste.Level.TransitionTo += ModScoped_Level_TransitionTo;
                On.Celeste.Level.Update += ModScoped_Level_Update;

                // Register MapData hooks for custom backdrops (only affects parsing our maps)
                On.Celeste.MapData.ParseBackdrop += ModScoped_MapData_ParseBackdrop;

                // Hook vanilla cutscenes to prevent them from running in mod maps
                On.Celeste.CS00_Ending.OnBegin += ModScoped_CS00_Ending_OnBegin;

                hooksRegistered = true;
                IngesteLogger.Info("All mod-scoped hooks registered successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to register hooks");
                throw;
            }
        }

        /// <summary>
        /// Unregister all mod hooks. This method is idempotent.
        /// </summary>
        public static void UnregisterHooks()
        {
            if (!hooksRegistered)
            {
                IngesteLogger.Info("Hooks not registered, skipping unregistration");
                return;
            }

            try
            {
                IngesteLogger.Info("Unregistering mod-scoped hooks");

                // Unregister Level hooks
                On.Celeste.Level.LoadLevel -= ModScoped_Level_LoadLevel;
                On.Celeste.Level.Begin -= ModScoped_Level_Begin;
                On.Celeste.Level.End -= ModScoped_Level_End;
                On.Celeste.Level.TransitionTo -= ModScoped_Level_TransitionTo;
                On.Celeste.Level.Update -= ModScoped_Level_Update;

                // Unregister MapData hooks
                On.Celeste.MapData.ParseBackdrop -= ModScoped_MapData_ParseBackdrop;

                // Unregister vanilla cutscene hooks
                On.Celeste.CS00_Ending.OnBegin -= ModScoped_CS00_Ending_OnBegin;

                // Dispose any manual detours
                foreach (var hook in ActiveHooks)
                {
                    try
                    {
                        hook?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        IngesteLogger.Warn($"Failed to dispose hook: {ex.Message}");
                    }
                }
                ActiveHooks.Clear();

                hooksRegistered = false;
                IngesteLogger.Info("All mod-scoped hooks unregistered successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to unregister hooks");
            }
        }

        #region Mod-Scoped Hook Implementations

        private static void ModScoped_Level_LoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, global::Celeste.Player.IntroTypes playerIntro, bool isFromLoader)
        {
            // Only apply mod logic if we're in a mod map
            if (!IsModMap(self))
            {
                orig(self, playerIntro, isFromLoader);
                return;
            }

            try
            {
                // For mod maps, check if we need to load a vignette BEFORE calling orig
                // This allows vignettes to replace the level scene before it fully loads
                string sid = self.Session.Area.GetSID();
                string roomName = self.Session.Level;

                // Handle Maggy/Main map vignettes
                if (sid == "DesoloZantas/Maps/Maggy/Main")
                {
                    bool isSpawnRoom = roomName.Contains("start") || roomName.Contains("spawn") || roomName == "lvl-0";
                    
                    if (isSpawnRoom)
                    {
                        // First priority: Vessel Creation
                        if (!self.Session.GetFlag("maggy_vessel_created"))
                        {
                            IngesteLogger.Info("Maggy/Main: Starting Vessel Creation Vignette before level load");
                            self.Session.SetFlag("maggy_vessel_created", true);
                            Engine.Scene = new Cutscenes.VesselCreationVignette(self.Session);
                            return; // Skip orig - vignette replaces the scene
                        }

                        // Second priority: Intro Vignette (plays after vessel creation)
                        if (!self.Session.GetFlag("maggy_intro_complete"))
                        {
                            IngesteLogger.Info("Maggy/Main: Starting Intro Vignette before level load");
                            self.Session.SetFlag("maggy_intro_complete", true);
                            Engine.Scene = new Cutscenes.Cs00IntroVignette(self.Session);
                            return; // Skip orig - vignette replaces the scene
                        }
                    }
                }

                // Call original - level will load normally
                orig(self, playerIntro, isFromLoader);
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Error in mod-scoped Level.LoadLevel hook");
                // On error, still call original to prevent game breaking
                orig(self, playerIntro, isFromLoader);
            }
        }

        private static void ModScoped_Level_Begin(On.Celeste.Level.orig_Begin orig, Level self)
        {
            // Always call original first
            orig(self);

            // Only apply mod logic if we're in a mod map
            if (!IsModMap(self))
                return;

            // Mod-specific logic can be added here if needed in the future
        }

        private static void ModScoped_Level_End(On.Celeste.Level.orig_End orig, Level self)
        {
            // Only apply mod logic if we're in a mod map
            if (IsModMap(self))
            {
                // Mod-specific cleanup logic can be added here if needed in the future
            }

            // Always call original
            orig(self);
        }

        private static void ModScoped_Level_TransitionTo(On.Celeste.Level.orig_TransitionTo orig, Level self, LevelData next, Vector2 direction)
        {
            // Only apply mod logic if we're in a mod map
            if (IsModMap(self))
            {
                // Mod-specific transition logic can be added here if needed in the future
            }

            // Always call original
            orig(self, next, direction);
        }

        private static void ModScoped_Level_Update(On.Celeste.Level.orig_Update orig, Level self)
        {
            // Always call original first
            orig(self);

            // Only apply mod logic if we're in a mod map
            if (!IsModMap(self))
                return;

            // Mod-specific update logic can be added here if needed in the future
        }

        private static Backdrop ModScoped_MapData_ParseBackdrop(On.Celeste.MapData.orig_ParseBackdrop orig, MapData self, BinaryPacker.Element child, BinaryPacker.Element above)
        {
            // Only intercept if this is our mod's map
            if (!IsModArea(self.Area.GetSID()))
            {
                return orig(self, child, above);
            }

            // Custom backdrop parsing logic can be added here if needed in the future
            // For now, just fall back to original
            return orig(self, child, above);
        }

        /// <summary>
        /// Prevents vanilla CS00_Ending from running in DesoloZantas maps.
        /// This avoids conflicts with the mod's custom ending cutscenes.
        /// </summary>
        private static void ModScoped_CS00_Ending_OnBegin(On.Celeste.CS00_Ending.orig_OnBegin orig, global::Celeste.CS00_Ending self, Level level)
        {
            // Check if we're in a DesoloZantas map
            if (IsModMap(level))
            {
                IngesteLogger.Info("Blocking vanilla CS00_Ending in mod map - using custom ending instead");
                // Don't call orig - skip the vanilla cutscene entirely
                // The cutscene entity will be removed after this, which is safe
                self.RemoveSelf();
                return;
            }

            // For non-mod maps, run the original cutscene
            orig(self, level);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Registers a custom IL hook and tracks it for proper disposal
        /// </summary>
        public static void RegisterILHook(IDetour hook)
        {
            if (hook != null)
            {
                ActiveHooks.Add(hook);
            }
        }

        /// <summary>
        /// Safely checks if a level session has a specific flag set
        /// </summary>
        public static bool GetModFlag(Level level, string flag)
        {
            if (!IsModMap(level))
                return false;

            return level?.Session?.GetFlag(flag) ?? false;
        }

        /// <summary>
        /// Safely sets a flag in the level session, only if in a mod map
        /// </summary>
        public static void SetModFlag(Level level, string flag, bool value = true)
        {
            if (!IsModMap(level))
            {
                IngesteLogger.Warn($"Attempted to set mod flag '{flag}' outside of mod map");
                return;
            }

            level?.Session?.SetFlag(flag, value);
        }

        #endregion
    }
}




