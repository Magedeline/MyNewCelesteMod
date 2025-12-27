using System.Runtime.Serialization;
using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Extension for AreaMode that provides additional side modes beyond A, B, and C-sides.
    /// Extends the standard Celeste AreaMode enum with D-side and beyond.
    /// Supports IO operations including serialization, deserialization, and file persistence.
    /// </summary>
    public static class AreaModeExt
    {
        /// <summary>
        /// D-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode DSide = (AreaMode)3;

        /// <summary>
        /// E-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode ESide = (AreaMode)4;

        /// <summary>
        /// F-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode FSide = (AreaMode)5;

        /// <summary>
        /// G-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode GSide = (AreaMode)6;

        /// <summary>
        /// H-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode HSide = (AreaMode)7;

        /// <summary>
        /// I-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode ISide = (AreaMode)8;

        /// <summary>
        /// J-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode JSide = (AreaMode)9;

        /// <summary>
        /// K-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode KSide = (AreaMode)10;

        /// <summary>
        /// L-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode LSide = (AreaMode)11;

        /// <summary>
        /// M-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode MSide = (AreaMode)12;

        /// <summary>
        /// N-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode NSide = (AreaMode)13;

        /// <summary>
        /// O-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode OSide = (AreaMode)14;

        /// <summary>
        /// P-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode PSide = (AreaMode)15;

        /// <summary>
        /// Q-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode QSide = (AreaMode)16;

        /// <summary>
        /// R-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode RSide = (AreaMode)17;

        /// <summary>
        /// S-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode SSide = (AreaMode)18;

        /// <summary>
        /// T-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode TSide = (AreaMode)19;

        /// <summary>
        /// U-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode USide = (AreaMode)20;

        /// <summary>
        /// V-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode VSide = (AreaMode)21;

        /// <summary>
        /// W-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode WSide = (AreaMode)22;

        /// <summary>
        /// X-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode XSide = (AreaMode)23;

        /// <summary>
        /// Y-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode YSide = (AreaMode)24;

        /// <summary>
        /// Z-Side mode (extends AreaMode enum)
        /// </summary>
        public static readonly AreaMode ZSide = (AreaMode)25;

        /// <summary>
        /// Converts an AreaMode to its display name (e.g., "A-Side", "B-Side", "D-Side")
        /// </summary>
        /// <param name="mode">The AreaMode to convert</param>
        /// <returns>The display name of the side</returns>
        public static string ToDisplayName(this AreaMode mode)
        {
            switch ((int)mode)
            {
                case 0: return "A-Side";
                case 1: return "B-Side";
                case 2: return "C-Side";
                case 3: return "D-Side";
                case 4: return "E-Side";
                case 5: return "F-Side";
                case 6: return "G-Side";
                case 7: return "H-Side";
                case 8: return "I-Side";
                case 9: return "J-Side";
                case 10: return "K-Side";
                case 11: return "L-Side";
                case 12: return "M-Side";
                case 13: return "N-Side";
                case 14: return "O-Side";
                case 15: return "P-Side";
                case 16: return "Q-Side";
                case 17: return "R-Side";
                case 18: return "S-Side";
                case 19: return "T-Side";
                case 20: return "U-Side";
                case 21: return "V-Side";
                case 22: return "W-Side";
                case 23: return "X-Side";
                case 24: return "Y-Side";
                case 25: return "Z-Side";
                default: return $"Side-{(int)mode}";
            }
        }

        /// <summary>
        /// Converts an AreaMode to a single character (e.g., 'A', 'B', 'D')
        /// </summary>
        /// <param name="mode">The AreaMode to convert</param>
        /// <returns>The character representing the side</returns>
        public static char ToChar(this AreaMode mode)
        {
            int modeInt = (int)mode;
            if (modeInt >= 0 && modeInt <= 25)
            {
                return (char)('A' + modeInt);
            }
            return '?';
        }

        /// <summary>
        /// Converts a character to an AreaMode (e.g., 'D' -> DSide)
        /// </summary>
        /// <param name="sideChar">The character representing the side (A-Z)</param>
        /// <returns>The corresponding AreaMode</returns>
        public static AreaMode FromChar(char sideChar)
        {
            char upper = char.ToUpper(sideChar);
            if (upper >= 'A' && upper <= 'Z')
            {
                return (AreaMode)(upper - 'A');
            }
            return AreaMode.Normal; // Default to A-Side
        }

        /// <summary>
        /// Checks if an AreaMode represents an extended side (D-Side or beyond)
        /// </summary>
        /// <param name="mode">The AreaMode to check</param>
        /// <returns>True if the mode is an extended side (>= D-Side)</returns>
        public static bool IsExtendedSide(this AreaMode mode)
        {
            return (int)mode >= 3;
        }

        /// <summary>
        /// Checks if an AreaMode is valid (within A-Z range)
        /// </summary>
        /// <param name="mode">The AreaMode to check</param>
        /// <returns>True if the mode is valid</returns>
        public static bool IsValid(this AreaMode mode)
        {
            int modeInt = (int)mode;
            return modeInt >= 0 && modeInt <= 25;
        }

        /// <summary>
        /// Gets the next AreaMode (e.g., A -> B, B -> C, C -> D)
        /// </summary>
        /// <param name="mode">The current AreaMode</param>
        /// <returns>The next AreaMode, or null if at the maximum</returns>
        public static AreaMode? GetNext(this AreaMode mode)
        {
            int nextMode = (int)mode + 1;
            if (nextMode <= 25)
            {
                return (AreaMode)nextMode;
            }
            return null;
        }

        /// <summary>
        /// Gets the previous AreaMode (e.g., D -> C, C -> B, B -> A)
        /// </summary>
        /// <param name="mode">The current AreaMode</param>
        /// <returns>The previous AreaMode, or null if at A-Side</returns>
        public static AreaMode? GetPrevious(this AreaMode mode)
        {
            int prevMode = (int)mode - 1;
            if (prevMode >= 0)
            {
                return (AreaMode)prevMode;
            }
            return null;
        }

        /// <summary>
        /// Converts a string to an AreaMode (e.g., "D-Side" -> DSide, "D" -> DSide)
        /// </summary>
        /// <param name="sideString">The string representing the side</param>
        /// <returns>The corresponding AreaMode, or null if invalid</returns>
        public static AreaMode? FromString(string sideString)
        {
            if (string.IsNullOrEmpty(sideString))
                return null;

            // Remove "-Side" suffix if present
            string normalized = sideString.Replace("-Side", "").Replace("Side", "").Trim().ToUpper();

            if (normalized.Length == 1)
            {
                return FromChar(normalized[0]);
            }

            // Try to parse as a number
            if (int.TryParse(normalized, out int modeInt) && modeInt >= 0 && modeInt <= 25)
            {
                return (AreaMode)modeInt;
            }

            return null;
        }

        #region IO Integration

        /// <summary>
        /// Serializable wrapper for AreaMode that supports various serialization formats
        /// </summary>
        [Serializable]
        [DataContract]
        public class AreaModeData
        {
            [DataMember]
            [JsonProperty("mode")]
            [YamlMember(Alias = "mode")]
            public int Mode { get; set; }

            [DataMember]
            [JsonProperty("displayName")]
            [YamlMember(Alias = "displayName")]
            public string DisplayName { get; set; }

            [DataMember]
            [JsonProperty("character")]
            [YamlMember(Alias = "character")]
            public char Character { get; set; }

            public AreaModeData() { }

            public AreaModeData(AreaMode mode)
            {
                Mode = (int)mode;
                DisplayName = mode.ToDisplayName();
                Character = mode.ToChar();
            }

            public AreaMode ToAreaMode()
            {
                return (AreaMode)Mode;
            }
        }

        /// <summary>
        /// Writes an AreaMode to a BinaryWriter (for save files and binary formats)
        /// </summary>
        public static void WriteTo(this AreaMode mode, BinaryWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            writer.Write((int)mode);
            writer.Write(mode.ToChar());
            writer.Write(mode.ToDisplayName());
        }

        /// <summary>
        /// Reads an AreaMode from a BinaryReader (for save files and binary formats)
        /// </summary>
        public static AreaMode ReadFrom(BinaryReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            int modeInt = reader.ReadInt32();
            reader.ReadChar(); // Read but don't use (for compatibility)
            reader.ReadString(); // Read but don't use (for compatibility)
            
            return (AreaMode)modeInt;
        }

        /// <summary>
        /// Serializes an AreaMode to JSON format
        /// </summary>
        public static string ToJson(this AreaMode mode)
        {
            var data = new AreaModeData(mode);
            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        /// <summary>
        /// Deserializes an AreaMode from JSON format
        /// </summary>
        public static AreaMode? FromJson(string json)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<AreaModeData>(json);
                return data?.ToAreaMode();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Serializes an AreaMode to YAML format
        /// </summary>
        public static string ToYaml(this AreaMode mode)
        {
            var serializer = new SerializerBuilder().Build();
            var data = new AreaModeData(mode);
            return serializer.Serialize(data);
        }

        /// <summary>
        /// Deserializes an AreaMode from YAML format
        /// </summary>
        public static AreaMode? FromYaml(string yaml)
        {
            try
            {
                var deserializer = new DeserializerBuilder().Build();
                var data = deserializer.Deserialize<AreaModeData>(yaml);
                return data?.ToAreaMode();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Writes an AreaMode to a text file
        /// </summary>
        public static void WriteToFile(this AreaMode mode, string filePath, FileFormat format = FileFormat.Text)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            switch (format)
            {
                case FileFormat.Text:
                    File.WriteAllText(filePath, mode.ToDisplayName());
                    break;
                case FileFormat.Json:
                    File.WriteAllText(filePath, mode.ToJson());
                    break;
                case FileFormat.Yaml:
                    File.WriteAllText(filePath, mode.ToYaml());
                    break;
                case FileFormat.Binary:
                    using (var fs = File.Create(filePath))
                    using (var writer = new BinaryWriter(fs))
                    {
                        mode.WriteTo(writer);
                    }
                    break;
                case FileFormat.Character:
                    File.WriteAllText(filePath, mode.ToChar().ToString());
                    break;
            }
        }

        /// <summary>
        /// Reads an AreaMode from a file
        /// </summary>
        public static AreaMode? ReadFromFile(string filePath, FileFormat format = FileFormat.Text)
        {
            if (!File.Exists(filePath))
                return null;

            try
            {
                switch (format)
                {
                    case FileFormat.Text:
                        return FromString(File.ReadAllText(filePath));
                    case FileFormat.Json:
                        return FromJson(File.ReadAllText(filePath));
                    case FileFormat.Yaml:
                        return FromYaml(File.ReadAllText(filePath));
                    case FileFormat.Binary:
                        using (var fs = File.OpenRead(filePath))
                        using (var reader = new BinaryReader(fs))
                        {
                            return ReadFrom(reader);
                        }
                    case FileFormat.Character:
                        string charStr = File.ReadAllText(filePath).Trim();
                        return charStr.Length > 0 ? FromChar(charStr[0]) : null;
                    default:
                        return null;
                }
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Appends AreaMode information to a log file
        /// </summary>
        public static void AppendToLog(this AreaMode mode, string logFilePath, string message = null)
        {
            if (string.IsNullOrEmpty(logFilePath))
                throw new ArgumentNullException(nameof(logFilePath));

            Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));

            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] AreaMode: {mode.ToDisplayName()} ({mode.ToChar()})";
            if (!string.IsNullOrEmpty(message))
                logEntry += $" - {message}";

            File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
        }

        /// <summary>
        /// Saves a collection of AreaModes to a file
        /// </summary>
        public static void SaveCollection(IEnumerable<AreaMode> modes, string filePath, FileFormat format = FileFormat.Json)
        {
            if (modes == null)
                throw new ArgumentNullException(nameof(modes));
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var dataList = new List<AreaModeData>();
            foreach (var mode in modes)
            {
                dataList.Add(new AreaModeData(mode));
            }

            switch (format)
            {
                case FileFormat.Json:
                    File.WriteAllText(filePath, JsonConvert.SerializeObject(dataList, Formatting.Indented));
                    break;
                case FileFormat.Yaml:
                    var serializer = new SerializerBuilder().Build();
                    File.WriteAllText(filePath, serializer.Serialize(dataList));
                    break;
                case FileFormat.Binary:
                    using (var fs = File.Create(filePath))
                    using (var writer = new BinaryWriter(fs))
                    {
                        writer.Write(dataList.Count);
                        foreach (var data in dataList)
                        {
                            data.ToAreaMode().WriteTo(writer);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Loads a collection of AreaModes from a file
        /// </summary>
        public static List<AreaMode> LoadCollection(string filePath, FileFormat format = FileFormat.Json)
        {
            if (!File.Exists(filePath))
                return new List<AreaMode>();

            try
            {
                switch (format)
                {
                    case FileFormat.Json:
                        var jsonData = JsonConvert.DeserializeObject<List<AreaModeData>>(File.ReadAllText(filePath));
                        return jsonData?.ConvertAll(d => d.ToAreaMode()) ?? new List<AreaMode>();
                    
                    case FileFormat.Yaml:
                        var deserializer = new DeserializerBuilder().Build();
                        var yamlData = deserializer.Deserialize<List<AreaModeData>>(File.ReadAllText(filePath));
                        return yamlData?.ConvertAll(d => d.ToAreaMode()) ?? new List<AreaMode>();
                    
                    case FileFormat.Binary:
                        var modes = new List<AreaMode>();
                        using (var fs = File.OpenRead(filePath))
                        using (var reader = new BinaryReader(fs))
                        {
                            int count = reader.ReadInt32();
                            for (int i = 0; i < count; i++)
                            {
                                modes.Add(ReadFrom(reader));
                            }
                        }
                        return modes;
                    
                    default:
                        return new List<AreaMode>();
                }
            }
            catch
            {
                return new List<AreaMode>();
            }
        }

        /// <summary>
        /// Exports AreaMode configuration to a CSV file
        /// </summary>
        public static void ExportToCSV(IEnumerable<AreaMode> modes, string filePath)
        {
            if (modes == null)
                throw new ArgumentNullException(nameof(modes));
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Index,Character,DisplayName,IsExtended");
                foreach (var mode in modes)
                {
                    writer.WriteLine($"{(int)mode},{mode.ToChar()},{mode.ToDisplayName()},{mode.IsExtendedSide()}");
                }
            }
        }

        /// <summary>
        /// Imports AreaModes from a CSV file
        /// </summary>
        public static List<AreaMode> ImportFromCSV(string filePath)
        {
            var modes = new List<AreaMode>();
            
            if (!File.Exists(filePath))
                return modes;

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    reader.ReadLine(); // Skip header
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split(',');
                        if (parts.Length > 0 && int.TryParse(parts[0], out int modeInt))
                        {
                            modes.Add((AreaMode)modeInt);
                        }
                    }
                }
            }
            catch
            {
                // Return empty list on error
            }

            return modes;
        }

        #endregion

        /// <summary>
        /// File format options for AreaMode serialization
        /// </summary>
        public enum FileFormat
        {
            /// <summary>Plain text format (human-readable)</summary>
            Text,
            /// <summary>JSON format</summary>
            Json,
            /// <summary>YAML format</summary>
            Yaml,
            /// <summary>Binary format (compact)</summary>
            Binary,
            /// <summary>Single character format</summary>
            Character
        }
    }
}




