#nullable enable
namespace DesoloZantas.Core.Core.Mountain
{
    /// <summary>
    /// Custom mountain state data for DesoloZantas overworld
    /// Extends the vanilla MountainState system with custom properties
    /// </summary>
    public class DesoloMountainState
    {
        public MTexture? TerrainTexture;
        public MTexture? BuildingsTexture;
        public MTexture? SkyboxTexture;
        public Color FogColor;
        public Color? StarFogColor;
        public Color[]? StarStreamColors;
        public Color[]? StarBeltColors1;
        public Color[]? StarBeltColors2;

        public DesoloMountainState(
            MTexture? terrain,
            MTexture? buildings,
            MTexture? skybox,
            Color fogColor)
        {
            TerrainTexture = terrain;
            BuildingsTexture = buildings;
            SkyboxTexture = skybox;
            FogColor = fogColor;
        }

        /// <summary>
        /// Create a DesoloMountainState from hex color strings
        /// </summary>
        public static DesoloMountainState FromHexColors(
            MTexture? terrain,
            MTexture? buildings,
            MTexture? skybox,
            string fogColorHex,
            string? starFogColorHex = null,
            string[]? starStreamColorsHex = null)
        {
            var state = new DesoloMountainState(terrain, buildings, skybox, Calc.HexToColor(fogColorHex));
            
            if (!string.IsNullOrEmpty(starFogColorHex))
            {
                state.StarFogColor = Calc.HexToColor(starFogColorHex);
            }

            if (starStreamColorsHex != null && starStreamColorsHex.Length >= 3)
            {
                state.StarStreamColors = new Color[3];
                for (int i = 0; i < 3; i++)
                {
                    state.StarStreamColors[i] = Calc.HexToColor(starStreamColorsHex[i]);
                }
            }

            return state;
        }
    }

    /// <summary>
    /// Custom mountain resources for DesoloZantas
    /// Manages textures, models, and states for a custom mountain appearance
    /// </summary>
    public class DesoloMountainResources
    {
        public ObjModel? CustomTerrain;
        public ObjModel? CustomBuildings;
        public ObjModel? CustomCoreWall;
        public ObjModel? CustomMoon;
        public ObjModel? CustomBird;

        public List<ObjModel> ExtraModels = new List<ObjModel>();
        
        public MTexture?[]? TerrainTextures;
        public MTexture?[]? BuildingTextures;
        public MTexture?[]? SkyboxTextures;
        public List<MTexture?[]> ExtraModelTextures = new List<MTexture?[]>();

        public MTexture? MoonTexture;
        public MTexture? FogTexture;
        public MTexture? SpaceTexture;
        public MTexture? SpaceStarsTexture;
        public MTexture? StarStreamTexture;

        public DesoloMountainState[]? States;

        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Load custom mountain resources from the mod's Graphics directory
        /// </summary>
        public void Load(string basePath = "DesoloZantas/Mountain")
        {
            try
            {
                IngesteLogger.Info($"Loading custom mountain resources from {basePath}");

                // Try to load textures
                TerrainTextures = new MTexture?[4];
                BuildingTextures = new MTexture?[4];
                SkyboxTextures = new MTexture?[4];

                for (int i = 0; i < 4; i++)
                {
                    string statePath = $"{basePath}/state{i}";
                    
                    if (MTN.Mountain.Has($"{statePath}/terrain"))
                        TerrainTextures[i] = MTN.Mountain[$"{statePath}/terrain"];
                    
                    if (MTN.Mountain.Has($"{statePath}/buildings"))
                        BuildingTextures[i] = MTN.Mountain[$"{statePath}/buildings"];
                    
                    if (MTN.Mountain.Has($"{statePath}/skybox"))
                        SkyboxTextures[i] = MTN.Mountain[$"{statePath}/skybox"];
                }

                // Load shared textures
                if (MTN.Mountain.Has($"{basePath}/moon"))
                    MoonTexture = MTN.Mountain[$"{basePath}/moon"];
                
                if (MTN.Mountain.Has($"{basePath}/fog"))
                    FogTexture = MTN.Mountain[$"{basePath}/fog"];
                
                if (MTN.Mountain.Has($"{basePath}/space"))
                    SpaceTexture = MTN.Mountain[$"{basePath}/space"];
                
                if (MTN.Mountain.Has($"{basePath}/stars"))
                    SpaceStarsTexture = MTN.Mountain[$"{basePath}/stars"];
                
                if (MTN.Mountain.Has($"{basePath}/starstream"))
                    StarStreamTexture = MTN.Mountain[$"{basePath}/starstream"];

                // Create default states with custom fog colors
                States = new DesoloMountainState[4];
                States[0] = DesoloMountainState.FromHexColors(
                    TerrainTextures?[0], BuildingTextures?[0], SkyboxTextures?[0],
                    "0a0515"); // Dark purple night
                States[1] = DesoloMountainState.FromHexColors(
                    TerrainTextures?[1], BuildingTextures?[1], SkyboxTextures?[1],
                    "1a1535"); // Purple twilight
                States[2] = DesoloMountainState.FromHexColors(
                    TerrainTextures?[2], BuildingTextures?[2], SkyboxTextures?[2],
                    "2a1a40"); // Deep purple
                States[3] = DesoloMountainState.FromHexColors(
                    TerrainTextures?[3], BuildingTextures?[3], SkyboxTextures?[3],
                    "0a0515", // Core state
                    "050210", // Star fog
                    new[] { "000000", "6020a0", "2080ff" }); // Star stream colors

                IsLoaded = true;
                IngesteLogger.Info("Custom mountain resources loaded successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Failed to load custom mountain resources: {ex.Message}");
                IsLoaded = false;
            }
        }

        /// <summary>
        /// Unload and dispose of custom mountain resources
        /// </summary>
        public void Unload()
        {
            TerrainTextures = null;
            BuildingTextures = null;
            SkyboxTextures = null;
            MoonTexture = null;
            FogTexture = null;
            SpaceTexture = null;
            SpaceStarsTexture = null;
            StarStreamTexture = null;
            States = null;
            ExtraModels.Clear();
            ExtraModelTextures.Clear();
            IsLoaded = false;
            
            IngesteLogger.Info("Custom mountain resources unloaded");
        }
    }
}
