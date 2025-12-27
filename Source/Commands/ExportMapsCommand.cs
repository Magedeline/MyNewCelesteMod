using DesoloZantas.Core.Tools;

namespace DesoloZantas.Core.Commands
{
    /// <summary>
    /// Console command to export all map files to JSON
    /// </summary>
    public class ExportMapsCommand
    {
        [Command("export_maps", "Export all .bin map files to JSON format")]
        public static void ExportMaps(string outputPath = null)
        {
            try
            {
                Console.WriteLine("=== Ingeste Map Export Tool ===");
                Console.WriteLine($"Starting export process...");
                
                if (string.IsNullOrEmpty(outputPath))
                {
                    outputPath = Path.Combine(Directory.GetCurrentDirectory(), "Maps", "JSON_Export");
                    Console.WriteLine($"Using default output path: {outputPath}");
                }
                else
                {
                    Console.WriteLine($"Using custom output path: {outputPath}");
                }

                // Ensure we're in the right directory
                var currentDir = Directory.GetCurrentDirectory();
                Console.WriteLine($"Current directory: {currentDir}");

                // Check if Maps directory exists
                var mapsDir = Path.Combine(currentDir, "Maps");
                if (!Directory.Exists(mapsDir))
                {
                    Console.WriteLine($"ERROR: Maps directory not found at {mapsDir}");
                    Console.WriteLine("Please run this command from the mod root directory.");
                    return;
                }

                Console.WriteLine($"Maps directory found: {mapsDir}");
                
                // Count .bin files
                var binFiles = Directory.GetFiles(mapsDir, "*.bin", SearchOption.AllDirectories);
                Console.WriteLine($"Found {binFiles.Length} .bin files to export:");
                
                foreach (var file in binFiles)
                {
                    var relativePath = file.StartsWith(mapsDir)
    ? file.Substring(mapsDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
    : file;
                    Console.WriteLine($"  - {relativePath}");
                }

                Console.WriteLine("\nStarting export...");
                
                // Perform the export
                MapExporter.ExportAllMaps(outputPath);
                
                Console.WriteLine($"\n=== Export Complete ===");
                Console.WriteLine($"JSON files exported to: {outputPath}");
                Console.WriteLine("Check the _export_summary.json file for detailed results.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Export failed - {ex.Message}");
                Console.WriteLine($"Stack trace: {ex}");
            }
        }

        [Command("export_single_map", "Export a single .bin map file to JSON")]
        public static void ExportSingleMap(string mapPath, string outputPath = null)
        {
            try
            {
                if (string.IsNullOrEmpty(mapPath))
                {
                    Console.WriteLine("ERROR: Please specify a map file path");
                    Console.WriteLine("Usage: export_single_map <path_to_map.bin> [output_path]");
                    return;
                }

                if (!File.Exists(mapPath))
                {
                    Console.WriteLine($"ERROR: Map file not found: {mapPath}");
                    return;
                }

                if (string.IsNullOrEmpty(outputPath))
                {
                    outputPath = Path.ChangeExtension(mapPath, ".json");
                }

                Console.WriteLine($"Exporting single map: {mapPath}");
                Console.WriteLine($"Output: {outputPath}");

                // Create a temporary directory structure for the single file export
                var tempDir = Path.Combine(Path.GetTempPath(), "IngesteMapExport", Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                try
                {
                    // Use the main exporter but only for this file
                    var outputDir = Path.GetDirectoryName(outputPath);
                    if (!Directory.Exists(outputDir))
                    {
                        Directory.CreateDirectory(outputDir);
                    }

                    BinaryPacker.Element mapData = BinaryPacker.FromBinary(mapPath);

                    // Convert to JSON using the exporter's logic
                    var jsonData = ConvertSingleMapToJson(mapData, mapPath);
                    var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData, Newtonsoft.Json.Formatting.Indented);
                    
                    File.WriteAllText(outputPath, jsonString);
                    
                    Console.WriteLine("? Single map export complete!");
                }
                finally
                {
                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Single map export failed - {ex.Message}");
            }
        }

        private static object ConvertSingleMapToJson(BinaryPacker.Element element, string sourcePath)
        {
            var result = new System.Collections.Generic.Dictionary<string, object>
            {
                ["_metadata"] = new System.Collections.Generic.Dictionary<string, object>
                {
                    ["source_file"] = Path.GetFileName(sourcePath),
                    ["export_time"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ["exporter_version"] = "1.0.0"
                },
                ["name"] = element?.Name ?? "unknown"
            };

            if (element?.Attributes?.Count > 0)
            {
                result["attributes"] = element.Attributes;
            }

            if (element?.Children?.Count > 0)
            {
                result["children"] = element.Children;
            }

            return result;
        }

        [Command("list_maps", "List all .bin map files in the Maps directory")]
        public static void ListMaps()
        {
            try
            {
                var currentDir = Directory.GetCurrentDirectory();
                var mapsDir = Path.Combine(currentDir, "Maps");
                
                if (!Directory.Exists(mapsDir))
                {
                    Console.WriteLine($"ERROR: Maps directory not found at {mapsDir}");
                    return;
                }

                var binFiles = Directory.GetFiles(mapsDir, "*.bin", SearchOption.AllDirectories);
                
                Console.WriteLine($"=== Map Files Found ({binFiles.Length} total) ===");
                
                foreach (var file in binFiles)
                {
                    var relativePath = file.StartsWith(mapsDir)
    ? file.Substring(mapsDir.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
    : file;
                    var fileInfo = new FileInfo(file);
                    var sizeKB = Math.Round(fileInfo.Length / 1024.0, 2);
                    
                    Console.WriteLine($"{relativePath} ({sizeKB} KB)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: Failed to list maps - {ex.Message}");
            }
        }
    }
}



