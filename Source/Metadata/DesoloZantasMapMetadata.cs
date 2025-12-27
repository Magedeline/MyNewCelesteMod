using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DesoloZantas.Core.Metadata
{
    public class DesoloZantasMapMetadata
    {
        public string Icon { get; set; }
        public bool Interlude { get; set; }
        public string TitleBaseColor { get; set; }
        public string TitleAccentColor { get; set; }
        public string TitleTextColor { get; set; }
        public string IntroType { get; set; }
        public bool Dreaming { get; set; }
        public string ColorGrade { get; set; }
        public string Wipe { get; set; }
        public float DarknessAlpha { get; set; }
        public float BloomBase { get; set; }
        public float BloomStrength { get; set; }
        public string Jumpthru { get; set; }
        public string CassetteNoteColor { get; set; }
        public string CassetteSong { get; set; }
        public List<ModeMetadata> Modes { get; set; }
        public MountainMetadata Mountain { get; set; }
        public CompleteScreenMetadata CompleteScreen { get; set; }

        public DesoloZantasMapMetadata()
        {
            Modes = new List<ModeMetadata>();
        }

        public static DesoloZantasMapMetadata Load(string modRoot)
        {
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            string path = Path.Combine(modRoot, "metadata", "Maps", "Maggy", "DesoloZantas", "metadata.yaml");

            if (File.Exists(path))
            {
                string yaml = File.ReadAllText(path);
                return deserializer.Deserialize<DesoloZantasMapMetadata>(yaml);
            }

            return null;
        }
    }

    public class ModeMetadata
    {
        public AudioStateMetadata AudioState { get; set; }
        public string Inventory { get; set; }
        public bool SeekerSlowdown { get; set; }
        public bool HeartIsEnd { get; set; }
    }

    public class AudioStateMetadata
    {
        public string Music { get; set; }
        public string Ambience { get; set; }
    }

    public class MountainMetadata
    {
        public CameraPosition Idle { get; set; }
        public CameraPosition Select { get; set; }
        public CameraPosition Zoom { get; set; }
        public List<float> Cursor { get; set; }
        public int State { get; set; }
    }

    public class CameraPosition
    {
        public List<float> Position { get; set; }
        public List<float> Target { get; set; }
    }

    public class CompleteScreenMetadata
    {
        public string Atlas { get; set; }
        public int Scale { get; set; }
        public List<float> Start { get; set; }
        public List<float> Center { get; set; }
        public TitleMetadata Title { get; set; }
        public List<LayerMetadata> Layers { get; set; }
    }

    public class TitleMetadata
    {
        public string ASide { get; set; }
        public string BSide { get; set; }
        public string CSide { get; set; }
        public string DSide { get; set; }
        public string FullClear { get; set; }
    }

    public class LayerMetadata
    {
        public string Type { get; set; }
        public List<string> Images { get; set; }
        public List<float> Position { get; set; }
        public List<float> Scroll { get; set; }
        public int Scale { get; set; }
    }
}




