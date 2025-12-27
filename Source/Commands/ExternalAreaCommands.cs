using DesoloZantas.Core.Core;

namespace DesoloZantas.Core.Commands
{
    /// <summary>
    /// Console commands for managing external area data
    /// </summary>
    public static class ExternalAreaCommands
    {
        [Command("dz_list_areas", "List all registered areas in the DesoloZantas campaign")]
        public static void ListAreas()
        {
            try
            {
                var allAreas = AreaData.Areas;
                var externalAreas = ExternalAreaDataManager.GetRegisteredAreas();

                Engine.Commands.Log($"Total Areas: {allAreas.Count}");
                Engine.Commands.Log($"DesoloZantas External Areas: {externalAreas.Count}");
                Engine.Commands.Log("");

                if (externalAreas.Count == 0)
                {
                    Engine.Commands.Log("No external areas loaded. Use 'dz_reload_areas' to load them.");
                    return;
                }

                Engine.Commands.Log("DesoloZantas Areas:");
                foreach (var area in externalAreas)
                {
                    var modes = area.Mode?.Length ?? 0;
                    Engine.Commands.Log($"  {area.ID}: {area.Name} ({modes} modes)");
                    
                    if (area.Mode != null)
                    {
                        for (int i = 0; i < area.Mode.Length; i++)
                        {
                            var mode = area.Mode[i];
                            var sideLabel = GetSideLabel(i);
                            Engine.Commands.Log($"    {sideLabel}: {mode.Path}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Engine.Commands.Log($"Error listing areas: {ex.Message}");
                IngesteLogger.Error(ex, "Failed to list areas");
            }
        }

        [Command("dz_reload_areas", "Reload external area data for the DesoloZantas campaign")]
        public static void ReloadAreas()
        {
            try
            {
                Engine.Commands.Log("Reloading DesoloZantas external areas...");
                
                ExternalAreaDataManager.UnloadExternalAreas();
                ExternalAreaDataManager.LoadExternalAreas();
                
                var loadedCount = ExternalAreaDataManager.GetRegisteredAreas().Count;
                Engine.Commands.Log($"Successfully reloaded {loadedCount} external areas");
            }
            catch (Exception ex)
            {
                Engine.Commands.Log($"Error reloading areas: {ex.Message}");
                IngesteLogger.Error(ex, "Failed to reload areas");
            }
        }

        [Command("dz_area_info", "Get detailed information about a specific area by ID or name")]
        public static void GetAreaInfo(int areaId)
        {
            try
            {
                var area = AreaData.Get(areaId);
                if (area == null)
                {
                    Engine.Commands.Log($"Area with ID {areaId} not found");
                    return;
                }

                DisplayAreaInfo(area);
            }
            catch (Exception ex)
            {
                Engine.Commands.Log($"Error getting area info: {ex.Message}");
                IngesteLogger.Error(ex, "Failed to get area info");
            }
        }

        [Command("dz_area_info", "Get detailed information about a specific area by name")]
        public static void GetAreaInfo(string areaName)
        {
            try
            {
                var area = AreaData.Areas.FirstOrDefault(a => 
                    string.Equals(a.Name, areaName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(a.LevelSet, areaName, StringComparison.OrdinalIgnoreCase));

                if (area == null)
                {
                    Engine.Commands.Log($"Area with name '{areaName}' not found");
                    return;
                }

                DisplayAreaInfo(area);
            }
            catch (Exception ex)
            {
                Engine.Commands.Log($"Error getting area info: {ex.Message}");
                IngesteLogger.Error(ex, "Failed to get area info");
            }
        }

        [Command("dz_check_areadata", "Check if the areadata file exists and show its path")]
        public static void CheckAreaDataFile()
        {
            try
            {
                string modRoot = IngesteModule.Instance.Metadata?.PathDirectory ?? ".";
                string areadataPath = System.IO.Path.Combine(modRoot, "Maps", "areadata");
                
                Engine.Commands.Log($"Areadata file path: {areadataPath}");
                Engine.Commands.Log($"File exists: {System.IO.File.Exists(areadataPath)}");
                
                if (System.IO.File.Exists(areadataPath))
                {
                    var lines = System.IO.File.ReadAllLines(areadataPath);
                    Engine.Commands.Log($"File contains {lines.Length} lines");
                    
                    int commentLines = lines.Count(line => line.Trim().StartsWith("#") || string.IsNullOrWhiteSpace(line));
                    int modeLines = lines.Count(line => line.Trim().StartsWith("Mode "));
                    
                    Engine.Commands.Log($"Comment/empty lines: {commentLines}");
                    Engine.Commands.Log($"Mode definition lines: {modeLines}");
                }
            }
            catch (Exception ex)
            {
                Engine.Commands.Log($"Error checking areadata file: {ex.Message}");
                IngesteLogger.Error(ex, "Failed to check areadata file");
            }
        }

        private static void DisplayAreaInfo(AreaData area)
        {
            Engine.Commands.Log($"=== Area Information ===");
            Engine.Commands.Log($"ID: {area.ID}");
            Engine.Commands.Log($"Name: {area.Name}");
            Engine.Commands.Log($"LevelSet: {area.LevelSet}");
            Engine.Commands.Log($"Icon: {area.Icon}");
            Engine.Commands.Log($"Interlude: {area.Interlude}");
            Engine.Commands.Log($"CanFullClear: {area.CanFullClear}");
            Engine.Commands.Log($"IsFinal: {area.IsFinal}");
            Engine.Commands.Log($"Dreaming: {area.Dreaming}");
            Engine.Commands.Log($"IntroType: {area.IntroType}");
            
            if (area.Mode != null)
            {
                Engine.Commands.Log($"Modes: {area.Mode.Length}");
                for (int i = 0; i < area.Mode.Length; i++)
                {
                    var mode = area.Mode[i];
                    var sideLabel = GetSideLabel(i);
                    Engine.Commands.Log($"  {sideLabel}:");
                    Engine.Commands.Log($"    Path: {mode.Path}");
                    Engine.Commands.Log($"    Inventory: {mode.Inventory}");
                    if (mode.AudioState != null)
                    {
                        Engine.Commands.Log($"    Music: {mode.AudioState.Music}");
                        Engine.Commands.Log($"    Ambience: {mode.AudioState.Ambience}");
                    }
                }
            }
            
            // Check if this is one of our external areas
            var externalAreas = ExternalAreaDataManager.GetRegisteredAreas();
            bool isExternal = externalAreas.Contains(area);
            Engine.Commands.Log($"External Area: {isExternal}");
        }

        private static string GetSideLabel(int modeIndex)
        {
            return modeIndex switch
            {
                0 => "A-Side",
                1 => "B-Side", 
                2 => "C-Side",
                3 => "D-Side",
                _ => $"Mode {modeIndex}"
            };
        }
    }
}



