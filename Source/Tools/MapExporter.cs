using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DesoloZantas.Core.Tools
{
    /// <summary>
    /// Utility class to export all .bin map files to JSON format
    /// </summary>
    public static class MapExporter
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
            Converters = new List<JsonConverter>
            {
                new Vector2Converter(),
                new ColorConverter(),
                new RectangleConverter()
            }
        };

        /// <summary>
        /// Export all .bin files from the Maps directory to JSON
        /// </summary>
        /// <param name="outputDirectory">Directory to save JSON files (default: Maps/JSON_Export)</param>
        public static void ExportAllMaps(string outputDirectory = null)
        {
            try
            {
                outputDirectory ??= Path.Combine("Maps", "JSON_Export");
                
                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                }

                Logger.Log(LogLevel.Info, "MapExporter", $"Starting map export to: {outputDirectory}");

                var mapsDirectory = "Maps";
                if (!Directory.Exists(mapsDirectory))
                {
                    Logger.Log(LogLevel.Error, "MapExporter", "Maps directory not found!");
                    return;
                }

                var binFiles = Directory.GetFiles(mapsDirectory, "*.bin", SearchOption.AllDirectories);
                Logger.Log(LogLevel.Info, "MapExporter", $"Found {binFiles.Length} .bin files to export");

                int successCount = 0;
                int errorCount = 0;

                foreach (string binFile in binFiles)
                {
                    try
                    {
                        ExportSingleMap(binFile, outputDirectory);
                        successCount++;
                        Logger.Log(LogLevel.Info, "MapExporter", $"? Exported: {Path.GetFileName(binFile)}");
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        Logger.Log(LogLevel.Error, "MapExporter", $"? Failed to export {Path.GetFileName(binFile)}: {ex.Message}");
                    }
                }

                Logger.Log(LogLevel.Info, "MapExporter", $"Export complete! Success: {successCount}, Errors: {errorCount}");
                
                // Create summary file
                CreateExportSummary(outputDirectory, successCount, errorCount, binFiles.Length);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "MapExporter", $"Export failed: {ex}");
            }
        }

        /// <summary>
        /// Export a single .bin map file to JSON
        /// </summary>
        private static void ExportSingleMap(string binFilePath, string outputDirectory)
        {
            if (!File.Exists(binFilePath))
            {
                throw new FileNotFoundException($"Map file not found: {binFilePath}");
            }

            // Load map data from binary
            BinaryPacker.Element mapData;
            mapData = BinaryPacker.FromBinary(binFilePath);

            // Convert to exportable format
            var jsonData = ConvertMapDataToJson(mapData, binFilePath);

            // Determine output file path
            var mapsDirectory = "Maps";
            var relativePath = binFilePath.StartsWith(mapsDirectory + Path.DirectorySeparatorChar)
                ? binFilePath.Substring(mapsDirectory.Length + 1)
                : Path.GetFileName(binFilePath);
            var outputFileName = Path.ChangeExtension(relativePath, ".json");
            var outputPath = Path.Combine(outputDirectory, outputFileName);

            // Ensure output directory exists
            var outputDir = Path.GetDirectoryName(outputPath);
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }

            // Write JSON file
            var jsonString = JsonConvert.SerializeObject(jsonData, JsonSettings);
            File.WriteAllText(outputPath, jsonString);
        }

        /// <summary>
        /// Convert BinaryPacker.Element to JSON-serializable format
        /// </summary>
        private static object ConvertMapDataToJson(BinaryPacker.Element element, string sourcePath)
        {
            var result = new Dictionary<string, object>
            {
                ["_metadata"] = new Dictionary<string, object>
                {
                    ["source_file"] = Path.GetFileName(sourcePath),
                    ["export_time"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ["exporter_version"] = "1.0.0"
                }
            };

            if (element == null)
            {
                result["error"] = "Failed to parse binary data";
                return result;
            }

            // Add basic element info
            result["name"] = element.Name;
            
            // Convert attributes
            if (element.Attributes?.Count > 0)
            {
                result["attributes"] = ConvertAttributes(element.Attributes);
            }

            // Convert children
            if (element.Children?.Count > 0)
            {
                result["children"] = element.Children.Select(child => ConvertElementToJson(child)).ToList();
            }

            // Extract specific map information if this is a map root
            if (element.Name == "Map" || element.Name == "map")
            {
                ExtractMapMetadata(element, result);
            }

            return result;
        }

        /// <summary>
        /// Convert a BinaryPacker.Element to JSON recursively
        /// </summary>
        private static object ConvertElementToJson(BinaryPacker.Element element)
        {
            if (element == null) return null;

            var result = new Dictionary<string, object>
            {
                ["name"] = element.Name
            };

            // Convert attributes
            if (element.Attributes?.Count > 0)
            {
                result["attributes"] = ConvertAttributes(element.Attributes);
            }

            // Convert children
            if (element.Children?.Count > 0)
            {
                result["children"] = element.Children.Select(child => ConvertElementToJson(child)).ToList();
            }

            return result;
        }

        /// <summary>
        /// Convert attributes dictionary to JSON-friendly format
        /// </summary>
        private static Dictionary<string, object> ConvertAttributes(Dictionary<string, object> attributes)
        {
            var result = new Dictionary<string, object>();

            foreach (var kvp in attributes)
            {
                var value = kvp.Value;
                
                // Handle special types
                if (value is Vector2 vector)
                {
                    result[kvp.Key] = new { x = vector.X, y = vector.Y };
                }
                else if (value is Color color)
                {
                    result[kvp.Key] = new { r = color.R, g = color.G, b = color.B, a = color.A };
                }
                else if (value is Rectangle rect)
                {
                    result[kvp.Key] = new { x = rect.X, y = rect.Y, width = rect.Width, height = rect.Height };
                }
                else if (value is float[] floatArray)
                {
                    result[kvp.Key] = floatArray;
                }
                else if (value is int[] intArray)
                {
                    result[kvp.Key] = intArray;
                }
                else if (value is byte[] byteArray)
                {
                    // Convert byte arrays to base64 for JSON
                    result[kvp.Key] = new { type = "bytes", data = Convert.ToBase64String(byteArray) };
                }
                else
                {
                    result[kvp.Key] = value;
                }
            }

            return result;
        }

        /// <summary>
        /// Extract specific map metadata for easier analysis
        /// </summary>
        private static void ExtractMapMetadata(BinaryPacker.Element mapElement, Dictionary<string, object> result)
        {
            var metadata = new Dictionary<string, object>();

            // Extract level count and info
            var levels = mapElement.Children?.Where(c => c.Name == "levels" || c.Name == "level").ToList();
            if (levels?.Count > 0)
            {
                metadata["level_count"] = levels.Count;
                metadata["levels"] = levels.Select(level => ExtractLevelInfo(level)).ToList();
            }

            // Extract filler rectangles
            var fillers = mapElement.Children?.Where(c => c.Name == "Filler").ToList();
            if (fillers?.Count > 0)
            {
                metadata["filler_count"] = fillers.Count;
            }

            // Extract style info if present
            var style = mapElement.Children?.FirstOrDefault(c => c.Name == "Style");
            if (style != null)
            {
                metadata["style"] = ConvertElementToJson(style);
            }

            if (metadata.Count > 0)
            {
                result["map_metadata"] = metadata;
            }
        }

        /// <summary>
        /// Extract level information for metadata
        /// </summary>
        private static object ExtractLevelInfo(BinaryPacker.Element levelElement)
        {
            var info = new Dictionary<string, object>();

            if (levelElement.Attributes != null)
            {
                // Extract key level properties
                if (levelElement.Attributes.TryGetValue("name", out var name))
                    info["name"] = name;
                if (levelElement.Attributes.TryGetValue("x", out var x))
                    info["x"] = x;
                if (levelElement.Attributes.TryGetValue("y", out var y))
                    info["y"] = y;
                if (levelElement.Attributes.TryGetValue("width", out var width))
                    info["width"] = width;
                if (levelElement.Attributes.TryGetValue("height", out var height))
                    info["height"] = height;
            }

            // Count entities and other elements
            var entities = levelElement.Children?.Where(c => c.Name == "entities").SelectMany(e => e.Children ?? new List<BinaryPacker.Element>()).ToList();
            if (entities?.Count > 0)
            {
                info["entity_count"] = entities.Count;
                info["entity_types"] = entities.GroupBy(e => e.Name).ToDictionary(g => g.Key, g => g.Count());
            }

            var triggers = levelElement.Children?.Where(c => c.Name == "triggers").SelectMany(t => t.Children ?? new List<BinaryPacker.Element>()).ToList();
            if (triggers?.Count > 0)
            {
                info["trigger_count"] = triggers.Count;
                info["trigger_types"] = triggers.GroupBy(t => t.Name).ToDictionary(g => g.Key, g => g.Count());
            }

            return info;
        }

        /// <summary>
        /// Create a summary file of the export process
        /// </summary>
        private static void CreateExportSummary(string outputDirectory, int successCount, int errorCount, int totalCount)
        {
            var summary = new Dictionary<string, object>
            {
                ["export_summary"] = new Dictionary<string, object>
                {
                    ["total_files"] = totalCount,
                    ["successful_exports"] = successCount,
                    ["failed_exports"] = errorCount,
                    ["success_rate"] = totalCount > 0 ? (double)successCount / totalCount * 100 : 0,
                    ["export_time"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    ["output_directory"] = outputDirectory
                }
            };

            var summaryPath = Path.Combine(outputDirectory, "_export_summary.json");
            var summaryJson = JsonConvert.SerializeObject(summary, JsonSettings);
            File.WriteAllText(summaryPath, summaryJson);
        }

        #region JSON Converters

        private class Vector2Converter : JsonConverter<Vector2>
        {
            public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(value.X);
                writer.WritePropertyName("y");
                writer.WriteValue(value.Y);
                writer.WriteEndObject();
            }

            public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var obj = JObject.Load(reader);
                return new Vector2(obj["x"]?.Value<float>() ?? 0, obj["y"]?.Value<float>() ?? 0);
            }
        }

        private class ColorConverter : JsonConverter<Color>
        {
            public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("r");
                writer.WriteValue(value.R);
                writer.WritePropertyName("g");
                writer.WriteValue(value.G);
                writer.WritePropertyName("b");
                writer.WriteValue(value.B);
                writer.WritePropertyName("a");
                writer.WriteValue(value.A);
                writer.WriteEndObject();
            }

            public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var obj = JObject.Load(reader);
                return new Color(
                    obj["r"]?.Value<int>() ?? 255,
                    obj["g"]?.Value<int>() ?? 255,
                    obj["b"]?.Value<int>() ?? 255,
                    obj["a"]?.Value<int>() ?? 255
                );
            }
        }

        private class RectangleConverter : JsonConverter<Rectangle>
        {
            public override void WriteJson(JsonWriter writer, Rectangle value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(value.X);
                writer.WritePropertyName("y");
                writer.WriteValue(value.Y);
                writer.WritePropertyName("width");
                writer.WriteValue(value.Width);
                writer.WritePropertyName("height");
                writer.WriteValue(value.Height);
                writer.WriteEndObject();
            }

            public override Rectangle ReadJson(JsonReader reader, Type objectType, Rectangle existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var obj = JObject.Load(reader);
                return new Rectangle(
                    obj["x"]?.Value<int>() ?? 0,
                    obj["y"]?.Value<int>() ?? 0,
                    obj["width"]?.Value<int>() ?? 0,
                    obj["height"]?.Value<int>() ?? 0
                );
            }
        }

        #endregion
    }
}



