using DesoloZantas.Core.Core;

namespace DesoloZantas.Core.Commands
{
    /// <summary>
    /// Console commands for managing metadata registries
    /// </summary>
    public class MetadataCommands
    {
        [Command("reload_metadata", "Reload all metadata registries from YAML files")]
        public static void ReloadMetadata()
        {
            try
            {
                Console.WriteLine("Reloading all metadata registries...");
                MetadataManager.ReloadAll();
                Console.WriteLine("Metadata registries reloaded successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to reload metadata: {ex.Message}");
                IngesteLogger.Error(ex, "Console command: reload_metadata failed");
            }
        }

        [Command("metadata_stats", "Show statistics for all loaded metadata")]
        public static void ShowMetadataStats()
        {
            try
            {
                Console.WriteLine("=== Metadata Statistics ===");
                var stats = MetadataManager.GetMetadataStatistics();
                
                Console.WriteLine($"Areas: {stats.Areas}");
                Console.WriteLine($"Alt Sides: {stats.AltSides}");
                Console.WriteLine($"Plugins: {stats.Plugins}");
                Console.WriteLine($"Submaps: {stats.Submaps}");
                Console.WriteLine($"Cutscenes: {stats.Cutscenes}");
                Console.WriteLine($"Models: {stats.Models}");
                Console.WriteLine($"Inventory Profiles: {stats.InventoryProfiles}");
                Console.WriteLine($"Audio Events: {stats.AudioEvents}");
                Console.WriteLine($"Particle Effects: {stats.ParticleEffects}");
                Console.WriteLine($"UI Themes: {stats.UIThemes}");
                Console.WriteLine($"---");
                Console.WriteLine($"Total: {stats.Total} metadata entries");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get metadata statistics: {ex.Message}");
                IngesteLogger.Error(ex, "Console command: metadata_stats failed");
            }
        }

        [Command("list_submaps", "List all loaded submap metadata")]
        public static void ListSubmaps(string parentArea = null)
        {
            try
            {
                Console.WriteLine("=== Submap Metadata ===");
                var submaps = SubmapMetadataRegistry.Submaps.Values;
                
                if (!string.IsNullOrEmpty(parentArea))
                {
                    submaps = submaps.Where(s => s.ParentArea?.ToLower() == parentArea.ToLower()).ToList();
                    Console.WriteLine($"Filtering by parent area: {parentArea}");
                }
                
                if (!submaps.Any())
                {
                    Console.WriteLine("No submaps found.");
                    return;
                }
                
                foreach (var submap in submaps.OrderBy(s => s.ParentArea).ThenBy(s => s.Name))
                {
                    Console.WriteLine($"  {submap.Id}:");
                    Console.WriteLine($"    Name: {submap.Name}");
                    Console.WriteLine($"    Parent Area: {submap.ParentArea}");
                    Console.WriteLine($"    Difficulty: {submap.Difficulty}");
                    Console.WriteLine($"    Is Secret: {submap.IsSecret}");
                    if (!string.IsNullOrEmpty(submap.Description))
                        Console.WriteLine($"    Description: {submap.Description}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to list submaps: {ex.Message}");
                IngesteLogger.Error(ex, "Console command: list_submaps failed");
            }
        }

        [Command("list_cutscenes", "List all loaded cutscene metadata")]
        public static void ListCutscenes(string type = null)
        {
            try
            {
                Console.WriteLine("=== Cutscene Metadata ===");
                var cutscenes = CutsceneMetadataRegistry.Cutscenes.Values;
                
                if (!string.IsNullOrEmpty(type))
                {
                    cutscenes = cutscenes.Where(c => c.Type?.ToLower() == type.ToLower()).ToList();
                    Console.WriteLine($"Filtering by type: {type}");
                }
                
                if (!cutscenes.Any())
                {
                    Console.WriteLine("No cutscenes found.");
                    return;
                }
                
                foreach (var cutscene in cutscenes.OrderBy(c => c.Type).ThenBy(c => c.Id))
                {
                    Console.WriteLine($"  {cutscene.Id}:");
                    Console.WriteLine($"    Type: {cutscene.Type}");
                    Console.WriteLine($"    Duration: {cutscene.Duration}s");
                    Console.WriteLine($"    Skippable: {cutscene.Skippable}");
                    Console.WriteLine($"    Auto Start: {cutscene.AutoStart}");
                    if (cutscene.Characters?.Any() == true)
                        Console.WriteLine($"    Characters: {string.Join(", ", cutscene.Characters)}");
                    if (!string.IsNullOrEmpty(cutscene.Description))
                        Console.WriteLine($"    Description: {cutscene.Description}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to list cutscenes: {ex.Message}");
                IngesteLogger.Error(ex, "Console command: list_cutscenes failed");
            }
        }

        [Command("list_models", "List all loaded 3D model metadata")]
        public static void ListModels(string category = null)
        {
            try
            {
                Console.WriteLine("=== 3D Model Metadata ===");
                var models = ModelMetadataRegistry.Models.Values;
                
                if (!string.IsNullOrEmpty(category))
                {
                    models = models.Where(m => m.Category?.ToLower() == category.ToLower()).ToList();
                    Console.WriteLine($"Filtering by category: {category}");
                }
                
                if (!models.Any())
                {
                    Console.WriteLine("No models found.");
                    return;
                }
                
                foreach (var model in models.OrderBy(m => m.Category).ThenBy(m => m.Id))
                {
                    Console.WriteLine($"  {model.Id}:");
                    Console.WriteLine($"    Category: {model.Category}");
                    Console.WriteLine($"    Model Path: {model.ModelPath}");
                    Console.WriteLine($"    Is Static: {model.IsStatic}");
                    Console.WriteLine($"    Cast Shadows: {model.CastShadows}");
                    Console.WriteLine($"    Render Distance: {model.RenderDistance}");
                    if (model.Animations?.Any() == true)
                        Console.WriteLine($"    Animations: {string.Join(", ", model.Animations)}");
                    if (!string.IsNullOrEmpty(model.Description))
                        Console.WriteLine($"    Description: {model.Description}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to list models: {ex.Message}");
                IngesteLogger.Error(ex, "Console command: list_models failed");
            }
        }

        [Command("list_inventory_profiles", "List all loaded inventory profiles")]
        public static void ListInventoryProfiles()
        {
            try
            {
                Console.WriteLine("=== Inventory Profile Metadata ===");
                var profiles = InventoryMetadataRegistry.Profiles.Values;
                
                if (!profiles.Any())
                {
                    Console.WriteLine("No inventory profiles found.");
                    return;
                }
                
                foreach (var profile in profiles.OrderBy(p => p.Name))
                {
                    Console.WriteLine($"  {profile.ProfileId}:");
                    Console.WriteLine($"    Name: {profile.Name}");
                    Console.WriteLine($"    Health: {profile.StartingHealth}/{profile.MaxHealth}");
                    Console.WriteLine($"    Stamina: {profile.StartingStamina}/{profile.MaxStamina}");
                    Console.WriteLine($"    Dashes: {profile.StartingDashes}");
                    Console.WriteLine($"    Wall Jump: {profile.HasWallJump}");
                    Console.WriteLine($"    Climbing: {profile.HasClimbing}");
                    Console.WriteLine($"    Double Jump: {profile.HasDoubleJump}");
                    if (profile.Items?.Any() == true)
                        Console.WriteLine($"    Starting Items: {string.Join(", ", profile.Items)}");
                    if (profile.Abilities?.Any() == true)
                        Console.WriteLine($"    Starting Abilities: {string.Join(", ", profile.Abilities)}");
                    if (!string.IsNullOrEmpty(profile.Description))
                        Console.WriteLine($"    Description: {profile.Description}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to list inventory profiles: {ex.Message}");
                IngesteLogger.Error(ex, "Console command: list_inventory_profiles failed");
            }
        }

        [Command("list_audio_events", "List all loaded audio event metadata")]
        public static void ListAudioEvents(string category = null)
        {
            try
            {
                Console.WriteLine("=== Audio Event Metadata ===");
                var audioEvents = AudioMetadataRegistry.Audio.Values;
                if (!string.IsNullOrEmpty(category))
                {
                    audioEvents = audioEvents.Where(a => a.Category?.ToLower() == category.ToLower()).ToList();
                    Console.WriteLine($"Filtering by category: {category}");
                }
                
                if (!audioEvents.Any())
                {
                    Console.WriteLine("No audio events found.");
                    return;
                }
                
                foreach (var audio in audioEvents.OrderBy(a => a.Category).ThenBy(a => a.Id))
                {
                    Console.WriteLine($"  {audio.Id}:");
                    Console.WriteLine($"    Category: {audio.Category}");
                    Console.WriteLine($"    Event Path: {audio.EventPath}");
                    Console.WriteLine($"    Volume: {audio.Volume}");
                    Console.WriteLine($"    Looping: {audio.Looping}");
                    Console.WriteLine($"    Priority: {audio.Priority}");
                    if (audio.Tags?.Any() == true)
                        Console.WriteLine($"    Tags: {string.Join(", ", audio.Tags)}");
                    if (!string.IsNullOrEmpty(audio.Description))
                        Console.WriteLine($"    Description: {audio.Description}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to list audio events: {ex.Message}");
                IngesteLogger.Error(ex, "Console command: list_audio_events failed");
            }
        }

        [Command("validate_metadata", "Validate all loaded metadata entries")]
        public static void ValidateMetadata()
        {
            try
            {
                Console.WriteLine("=== Metadata Validation ===");
                int totalErrors = 0;
                int totalWarnings = 0;

                // Validate submaps
                Console.WriteLine("Validating submaps...");
                foreach (var submap in SubmapMetadataRegistry.Submaps.Values)
                {
                    if (!SubmapMetadataRegistry.ValidateSubmap(submap, out var error))
                    {
                        Console.WriteLine($"  ERROR: Submap '{submap.Id}': {error}");
                        totalErrors++;
                    }
                }

                // Validate cutscenes
                Console.WriteLine("Validating cutscenes...");
                foreach (var cutscene in CutsceneMetadataRegistry.Cutscenes.Values)
                {
                    if (!CutsceneMetadataRegistry.ValidateCutscene(cutscene, out var error))
                    {
                        Console.WriteLine($"  ERROR: Cutscene '{cutscene.Id}': {error}");
                        totalErrors++;
                    }
                }

                // Validate models
                Console.WriteLine("Validating models...");
                foreach (var model in ModelMetadataRegistry.Models.Values)
                {
                    if (!ModelMetadataRegistry.ValidateModel(model, out var error))
                    {
                        Console.WriteLine($"  ERROR: Model '{model.Id}': {error}");
                        totalErrors++;
                    }
                }

                // Validate inventory profiles
                Console.WriteLine("Validating inventory profiles...");
                foreach (var profile in InventoryMetadataRegistry.Profiles.Values)
                {
                    if (!InventoryMetadataRegistry.ValidateInventoryProfile(profile, out var error))
                    {
                        Console.WriteLine($"  ERROR: Inventory Profile '{profile.ProfileId}': {error}");
                        totalErrors++;
                    }
                }

                Console.WriteLine($"---");
                Console.WriteLine($"Validation complete: {totalErrors} errors, {totalWarnings} warnings");

                if (totalErrors == 0)
                {
                    Console.WriteLine("All metadata entries are valid!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to validate metadata: {ex.Message}");
                IngesteLogger.Error(ex, "Console command: validate_metadata failed");
            }
        }
    }
}



