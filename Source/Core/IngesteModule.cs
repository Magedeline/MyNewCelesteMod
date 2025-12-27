using System.Reflection;
using Celeste.Mod.DesoloZatnas;
using DesoloZantas.Core.Core.Effects.ShaderEffects;
using DesoloZantas.Core.Core.Entities;
using DesoloZantas.Core.Core.Extensions;
using DesoloZantas.Core.Core.Player;
using DesoloZantas.Core.Cutscenes;
using DesoloZantas.Core.Metadata;
using DesoloZantas.Core.Tools;
using MonoMod.ModInterop;
using MonoMod.Utils;

namespace DesoloZantas.Core.Core
{
    public class IngesteModule : EverestModule
    {
        public static bool LaunchPart1Credits;

        public static IngesteModule Instance { get; private set; }

        public override Type SettingsType => typeof(IngesteModuleSettings);
        public override Type SaveDataType => typeof(IngesteModuleSaveData);
        public override Type SessionType => typeof(IngesteModuleSession);

        public static IngesteModuleSettings Settings => (IngesteModuleSettings)Instance?._Settings;
        public static IngesteModuleSaveData SaveData => (IngesteModuleSaveData)Instance?._SaveData;
        public static IngesteModuleSession Session => (IngesteModuleSession)Instance?._Session;

        private readonly HashSet<Type> initializedEntityTypes = new HashSet<Type>();
        private readonly HashSet<Type> initializedParticleProviders = new HashSet<Type>();
        private bool backdropsRegistered = false;

        // Core heart sprite bank for custom heart graphics
        public static SpriteBank HeartSpriteBank;
        
        // Star projectile effects
        public static ParticleType P_StarExplosion;
        public static SpriteBank SpriteBank;

        private static readonly string[] ForcedEntityTypeNames = new[]
        {
            // Core entities
            "DesoloZantas.Core.Entities.SampleEntity",
            "DesoloZantas.Core.Entities.SampleSolid",
            "DesoloZantas.Core.Entities.AscendManager",
            "DesoloZantas.Core.Entities.NpcEventInteract",
            "DesoloZantas.Core.Entities.Bridge",

            // NPCs
            "DesoloZantas.Core.NPCs.Npc06Theo",
            "DesoloZantas.Core.NPCs.Npc03Theo",
            "DesoloZantas.Core.NPCs.Npc16Theo",

            // Character dummies
            "DesoloZantas.Core.Entities.AdelineDummy",
            "DesoloZantas.Core.Entities.AsrielDummy",
            "DesoloZantas.Core.Entities.BandanaWaddleDeeDummy",
            "DesoloZantas.Core.Entities.BattyDummy",
            "DesoloZantas.Core.Entities.CharaDummy",
            "DesoloZantas.Core.Entities.CharloDummy",
            "DesoloZantas.Core.Entities.CloverDummy",
            "DesoloZantas.Core.Entities.CodyDummy",
            "DesoloZantas.Core.Entities.CooDummy",
            "DesoloZantas.Core.Entities.DarkMetaKnightDummy",
            "DesoloZantas.Core.Entities.EmilyDummy",
            "DesoloZantas.Core.Entities.FlambergeZaleaDummy",
            "DesoloZantas.Core.Entities.FranZaleaDummy",
            "DesoloZantas.Core.Entities.FriskDummy",
            "DesoloZantas.Core.Entities.GooeySiDummy",
            "DesoloZantas.Core.Entities.HynesZaleaDummy",
            "DesoloZantas.Core.Entities.KineDummy",
            "DesoloZantas.Core.Entities.KingDDDDummy",
            "DesoloZantas.Core.Entities.KirbyClassicDummy",
            "DesoloZantas.Core.Entities.KirbyDummy",
            "DesoloZantas.Core.Entities.MagolorDummy",
            "DesoloZantas.Core.Entities.MarxDummy",
            "DesoloZantas.Core.Entities.MelodyDummy",
            "DesoloZantas.Core.Entities.MetaKnightDummy",
            "DesoloZantas.Core.Entities.NessDummy",
            "DesoloZantas.Core.Entities.OdinDummy",
            "DesoloZantas.Core.Entities.RalseiDummy",
            "DesoloZantas.Core.Entities.RickDummy",
            "DesoloZantas.Core.Entities.SqueakerDummy",
            "DesoloZantas.Core.Entities.StarsieDummy",
            "DesoloZantas.Core.Entities.SusieHaltmannDummy",
            "DesoloZantas.Core.Entities.TaranzaDummy",

            // Dream Friend entities (Kirby series)
            "DesoloZantas.Core.Entities.KingDededeDreamFriendDummy",
            "DesoloZantas.Core.Entities.GooeyDreamFriendDummy",
            "DesoloZantas.Core.Entities.AdeleineDreamFriendDummy",
            "DesoloZantas.Core.Entities.FranciscaDreamFriendDummy",
            "DesoloZantas.Core.Entities.FlambergeDreamFriendDummy",
            "DesoloZantas.Core.Entities.ZanPartizanneDreamFriendDummy",

            // Game mechanics entities
            "DesoloZantas.Core.Entities.AncientSwitch",
            "DesoloZantas.Core.Entities.BirdNPC",
            "DesoloZantas.Core.Entities.CharaChaser",
            "DesoloZantas.Core.Entities.ClutterBlock",
            "DesoloZantas.Core.Entities.ClutterSwitch",
            "DesoloZantas.Core.Entities.CubeDreamBlock",
            "DesoloZantas.Core.Entities.CustomNPCs",
            "DesoloZantas.Core.Entities.DeltaBerry",
            "DesoloZantas.Core.Entities.Enemy",
            "DesoloZantas.Core.Entities.FakeHeartGem",
            "DesoloZantas.Core.Entities.FlingBirdIntro",
            "DesoloZantas.Core.Entities.GlitchGlider",
            "DesoloZantas.Core.Entities.HeartGem",
            "DesoloZantas.Core.Entities.HeartStaff",
            "DesoloZantas.Core.Entities.HeartStaffDoor",
            "DesoloZantas.Core.Entities.IceBouncer",
            "DesoloZantas.Core.Entities.KirbyPlayer",
            "DesoloZantas.Core.Entities.MaddyCrystal",
            "DesoloZantas.Core.Entities.NPCEvent",
            "DesoloZantas.Core.Entities.OshiroLobbyBell",
            "DesoloZantas.Core.Entities.PlayerInventoryTrigger",
            "DesoloZantas.Core.Entities.PlateauMod",
            "DesoloZantas.Core.Entities.Refill",
            "DesoloZantas.Core.Entities.CharacterRefill",
            "DesoloZantas.Core.Entities.StarJumpCutsceneControl",
            "DesoloZantas.Core.Entities.SuperCoreBlock",
            "DesoloZantas.Core.Entities.TeleportPipe",
            "DesoloZantas.Core.Entities.TesseractMirrorPortal",
            "DesoloZantas.Core.Entities.TesseractSwitch",
            "DesoloZantas.Core.Entities.Tower3D",
            "DesoloZantas.Core.Entities.TowerBackgroundStylegroundExtensions",
            "DesoloZantas.Core.Entities.WarpStar"
        };

        private static readonly string[] ParticleProviderTypeNames = new[]
        {
            "DesoloZantas.Core.Entities.WarpStar",
            "DesoloZantas.Core.Entities.AdvancedRefill",
            "DesoloZantas.Core.Entities.CharacterRefill",
            "DesoloZantas.Core.DummyBase",
            "DesoloZantas.Core.Entities.NessDummy",
            "DesoloZantas.Core.Entities.HoloDeltaBerry",
            "DesoloZantas.Core.Effects.MagicalAura",
            "DesoloZantas.Core.Effects.AncientRunes",
            "DesoloZantas.Core.Entities.Strawberry",
            "DesoloZantas.Core.Entities.Cassette",
            "DesoloZantas.Core.Entities.Tape",
            "DesoloZantas.Core.Entities.CharaChaser"
        };

        private static IngesteModuleSession.HealthSystemData HealthData => Session.healthData;

        public int TASSeed;

        [Command("set_boss_seed", "Set the seed Bosses will use for their RNG, added to a deterministic per-entity value. Value 0 makes the seed based on the Active Timer")]
        public static void SetBossSeed(int value)
        {
            Instance.TASSeed = value >= 0 ? value : Instance.TASSeed;
        }

        public IngesteModule()
        {
            Instance = this;
        }

        public override void DeserializeSaveData(int index, byte[] data)
        {
            base.DeserializeSaveData(index, data);
            if (SaveData == null)
                Instance._SaveData = new IngesteModuleSaveData();
            if (SaveData.UnlockedBSideIDs == null)
                SaveData.UnlockedBSideIDs = new HashSet<string>();
        }

        public override void Load()
        {
            try
            {
                IngesteLogger.Info("Starting module load");

                // Execute initialization phases in order
                var phases = Enum.GetValues(typeof(InitializationPhase))
                    .Cast<InitializationPhase>()
                    .OrderBy(p => (int)p);
                
                foreach (var phase in phases)
                {
                    try
                    {
                        IngesteLogger.Info($"Starting {phase.GetDescription()}");
                        ExecuteInitializationPhase(phase);
                        IngesteLogger.Info($"Completed {phase.GetDescription()}");
                    }
                    catch (Exception phaseEx)
                    {
                        string errorMsg = $"Failed during {phase.GetDescription()}: {phaseEx.Message}";
                        
                        if (phase.IsCritical())
                        {
                            IngesteLogger.Error(phaseEx, $"Critical phase {phase} failed");
                            throw new InvalidOperationException(errorMsg, phaseEx);
                        }
                        else
                        {
                            IngesteLogger.Warn($"Non-critical phase {phase} failed: {phaseEx.Message}");
                        }
                    }
                }

                // Load external area data
                ExternalAreaDataManager.LoadExternalAreas();

                // Register BossesHelper hooks
                RegisterBossesHelperHooks();

                IngesteLogger.Info("Module loaded successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Error during module load");
                throw; // Re-throw to ensure Everest knows about the failure
            }
        }

        /// <summary>
        /// Executes a specific initialization phase by calling the corresponding method
        /// </summary>
        /// <param name="phase">The initialization phase to execute</param>
        private void ExecuteInitializationPhase(InitializationPhase phase)
        {
            switch (phase)
            {
                case InitializationPhase.AudioDirectory:
                    InitializeAudioDirectory();
                    break;
                case InitializationPhase.CoreIntegrations:
                    InitializeCoreIntegrations();
                    break;
                case InitializationPhase.CustomCutscenes:
                    InitializeCustomCutscenes();
                    break;
                case InitializationPhase.MetadataRegistries:
                    InitializeMetadataRegistries();
                    break;
                case InitializationPhase.EntityRegistration:
                    RegisterCustomEntitiesAndTriggers();
                    break;
                case InitializationPhase.ParticleSystems:
                    InitializeParticles();
                    break;
                case InitializationPhase.HookRegistration:
                    RegisterHooks();
                    RegisterCustomBackdrops();
                    break;
                case InitializationPhase.NPCSystems:
                    InitializeNPCSystems();
                    break;
                case InitializationPhase.CutsceneManager:
                    InitializeCutsceneManager();
                    break;
                case InitializationPhase.ShaderEffects:
                    InitializeShaderEffects();
                    break;
                case InitializationPhase.CompanionManager:
                    InitializeCompanionManager();
                    break;
                case InitializationPhase.PlayerSystems:
                    InitializePlayerSystems();
                    break;
                case InitializationPhase.StrawberryHooks:
                    InitializeStrawberryHooks();
                    break;
                case InitializationPhase.ConsoleCommands:
                    RegisterConsoleCommands();
                    break;
                default:
                    throw new ArgumentException($"Unknown initialization phase: {phase}");
            }
        }

        /// <summary>
        /// Initialize core integrations with other mods
        /// </summary>
        private void InitializeCoreIntegrations()
        {
            // Check launch mode from launcher (if used)
            try
            {
                LaunchModeReader.Reload();
                if (LaunchModeReader.WasLaunchedViaLauncher)
                {
                    IngesteLogger.Info($"Launched via CelesteDesoloZantas launcher - Mode: {(LaunchModeReader.IsDesoloMode ? "Desolo Zantas" : "Vanilla")}");
                }
                else
                {
                    IngesteLogger.Info("Launched directly (no launcher detected)");
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Failed to read launch mode: {ex.Message}");
            }

            InitializeFrostHelperIntegration();
            InitializeMonoModExports();
  
            // Initialize CollabUtils2 integration for sub maps
            try
            {
                IngesteLogger.Info("Initializing CollabUtils2 integration");
                CollabUtils2Integration.Initialize();
                IngesteLogger.Info("CollabUtils2 integration completed");
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"CollabUtils2 integration failed (continuing without it): {ex.Message}");
            }

            // Initialize Cassette Player FMOD plugin system
            try
            {
                IngesteLogger.Info("Initializing Cassette Player audio system");
                CassettePlayerSystem.RegisterHooks();
                IngesteLogger.Info("Cassette Player audio system initialized");
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Cassette Player integration failed (continuing without it): {ex.Message}");
            }

            // Initialize new simplified title screen
            // TODO: OuiDesoloTitleScreenHooks class not yet implemented
            // try
            // {
            //     IngesteLogger.Info("Initializing OuiDesoloTitleScreen hooks");
            //     UI.OuiDesoloTitleScreenHooks.Initialize();
            //     IngesteLogger.Info("OuiDesoloTitleScreen hooks initialized");
            // }
            // catch (Exception ex)
            // {
            //     IngesteLogger.Warn($"OuiDesoloTitleScreen hooks failed (continuing without it): {ex.Message}");
            // }

            // Initialize DesoloZantas Main Menu extensions (keeping for menu buttons)
            // TODO: DesoloMainMenuExt.Initialize() method not yet implemented
            // try
            // {
            //     IngesteLogger.Info("Initializing DesoloZantas Main Menu extensions");
            //     UI.DesoloMainMenuExt.Initialize();
            //     IngesteLogger.Info("DesoloZantas Main Menu extensions initialized");
            // }
            // catch (Exception ex)
            // {
            //     IngesteLogger.Warn($"Main Menu extensions failed (continuing without it): {ex.Message}");
            // }

            // OLD: DesoloZantas Title Logo Manager - disabled, using OuiDesoloTitleScreen instead
            // try
            // {
            //     IngesteLogger.Info("Initializing DesoloZantas Title Logo Manager");
            //     UI.DesoloTitleLogoManager.Initialize();
            //     IngesteLogger.Info("DesoloZantas Title Logo Manager initialized");
            // }
            // catch (Exception ex)
            // {
            //     IngesteLogger.Warn($"Title Logo Manager failed (continuing without it): {ex.Message}");
            // }

            // Initialize DesoloZantas Chapter Select hooks (for chapters 18, 19, 20)
            // TODO: DesoloChapterSelectHooks class not yet implemented
            // try
            // {
            //     IngesteLogger.Info("Initializing DesoloZantas Chapter Select hooks");
            //     UI.DesoloChapterSelectHooks.Initialize();
            //     IngesteLogger.Info("DesoloZantas Chapter Select hooks initialized");
            // }
            // catch (Exception ex)
            // {
            //     IngesteLogger.Warn($"Chapter Select hooks failed (continuing without it): {ex.Message}");
            // }

            // Initialize DesoloZantas Mountain Manager
            try
            {
                IngesteLogger.Info("Initializing DesoloZantas Mountain Manager");
                Mountain.DesoloMountainManager.Initialize();
                IngesteLogger.Info("DesoloZantas Mountain Manager initialized");
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Mountain Manager failed (continuing without it): {ex.Message}");
            }

            // OLD: Intro Presents Manager - disabled, using OuiDesoloTitleScreen instead
            // try
            // {
            //     IngesteLogger.Info("Initializing Intro Presents Manager");
            //     UI.IntroPresentsManager.Initialize();
            //     IngesteLogger.Info("Intro Presents Manager initialized");
            // }
            // catch (Exception ex)
            // {
            //     IngesteLogger.Warn($"Intro Presents Manager failed (continuing without it): {ex.Message}");
            // }

            // Initialize DesoloZantas Overworld Loader Manager (for custom postcards and transitions)
            // TODO: DesoloOverworldLoaderManager class not yet implemented
            // try
            // {
            //     IngesteLogger.Info("Initializing DesoloZantas Overworld Loader Manager");
            //     UI.DesoloOverworldLoaderManager.Initialize();
            //     IngesteLogger.Info("DesoloZantas Overworld Loader Manager initialized");
            // }
            // catch (Exception ex)
            // {
            //     IngesteLogger.Warn($"DesoloZantas Overworld Loader Manager failed (continuing without it): {ex.Message}");
            // }
        }

        /// <summary>
        /// Initialize metadata registries for all YAML metadata types
        /// </summary>
        private void InitializeMetadataRegistries()
        {
            try
            {
                IngesteLogger.Info("Initializing metadata registries");

                // Get the mod root directory - use assembly location as fallback
                string modRoot;
                if (Metadata?.PathDirectory != null)
                {
                    modRoot = Metadata.PathDirectory;
                }
                else
                {
                    // Fallback to assembly location if Metadata is not yet initialized
                    modRoot = Path.GetDirectoryName(typeof(IngesteModule).Assembly.Location) ?? ".";
                    IngesteLogger.Warn($"Metadata.PathDirectory is null, using assembly location: {modRoot}");
                }

                // Initialize the central metadata manager
                MetadataManager.Initialize(modRoot);

                // Load DesoloZatnas map metadata
                var mapMetadata = DesoloZantasMapMetadata.Load(modRoot);
                if (mapMetadata != null)
                {
                    IngesteLogger.Info($"Loaded DesoloZatnas map metadata: Icon={mapMetadata.Icon}, Interlude={mapMetadata.Interlude}");
                }
                else
                {
                    IngesteLogger.Warn("Failed to load DesoloZatnas map metadata");
                }

                IngesteLogger.Info("Metadata registries initialized successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to initialize metadata registries");
                // Don't rethrow since metadata is not critical for basic functionality
            }
        }

        /// <summary>
        /// Register console commands for the mod
        /// </summary>
        private void RegisterConsoleCommands()
        {
            try
            {
                IngesteLogger.Info("Registering console commands");

                RefreshConsoleCommandList();

                // Register map export commands
                if (Engine.Commands != null && Engine.Commands.FunctionKeyActions.Length > IngesteConstants.Console.F1_KEY_INDEX)
                {
                    Engine.Commands.FunctionKeyActions[IngesteConstants.Console.F1_KEY_INDEX] = () =>
                    {
                        IngesteLogger.Info("F1 pressed - Starting map export");
                        try
                        {
                            MapExporter.ExportAllMaps();
                            Engine.Commands.Log($"Map export completed! Check the {IngesteConstants.Paths.JSON_EXPORT_DIRECTORY} folder.");
                        }
                        catch (Exception ex)
                        {
                            Engine.Commands.Log($"Map export failed: {ex.Message}");
                            IngesteLogger.Error(ex, "Map export error");
                        }
                    };
                }
                else
                {
                    IngesteLogger.Warn("Engine.Commands unavailable; skipping F1 binding");
                }

                IngesteLogger.Info("Console commands registered (F1 = Export Maps, commands exported)");
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Console commands registration failed: {ex.Message}");
            }
        }

        public override void LoadContent(bool firstLoad)
        {
            // Register custom backdrop types
            RegisterCustomBackdrops();

            // Register the fixed map data processor
            // Everest.Content does not have OnProcessMapData, so use OnProcessMapData event from Everest.Events if available.
            // If you want to process map data, use Everest.Events.Level.OnLoadLevel or Everest.Events.Content.OnProcessLoad if appropriate.
            // Example using OnLoadLevel:
            Everest.Events.Level.OnLoadLevel += (level, introType, isFromLoader) => {
                var processor = new FixedMapDataProcessor();
                // You may need to access mapData differently, depending on your processor's requirements.
                // This is a placeholder for your actual logic.
                // processor.Process(level.Session.MapData);
            };
        }

        /// <summary>
        /// Register all custom backdrop/styleground types with Celeste
        /// </summary>
        private void RegisterCustomBackdrops()
        {
            if (backdropsRegistered)
            {
                IngesteLogger.Debug("Custom backdrops already registered, skipping");
                return;
            }

            try
            {
                IngesteLogger.Info("Registering custom backdrops");

                // Hook MapData.ParseBackdrop to register custom backdrop types
                On.Celeste.MapData.ParseBackdrop += MapData_ParseBackdrop;

                backdropsRegistered = true;
                IngesteLogger.Info("Custom backdrops registered successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to register custom backdrops");
            }
        }

        private Backdrop MapData_ParseBackdrop(On.Celeste.MapData.orig_ParseBackdrop orig, MapData self, BinaryPacker.Element child, BinaryPacker.Element above)
        {
            // Check if this is one of our custom backdrop types
            // The backdrop type is stored in child.Name, not in an attribute
            string backdropType = child.Name;
            
            // Debug log to see what backdrop type we're trying to parse
            if (backdropType != null && backdropType.Contains("Ingeste"))
            {
                IngesteLogger.Debug($"Parsing Ingeste backdrop: {backdropType}");
            }
            
            switch (backdropType)
            {
                case "Ingeste/RainbowBlackholeBg":
                    IngesteLogger.Debug("Creating RainbowBlackholeBg instance");
                    return new RainbowBlackholeBg();
                
                case "Ingeste/GiygasBackdrop":
                    IngesteLogger.Debug("Creating GiygasBackdrop instance");
                    var giygasBackdrop = new GiygasBackdrop();
                    
                    // Parse configuration parameters
                    giygasBackdrop.Intensity = child.AttrFloat("intensity", 1.0f);
                    giygasBackdrop.Speed = child.AttrFloat("speed", 1.0f);
                    giygasBackdrop.ColorShiftSpeed = child.AttrFloat("colorShiftSpeed", 0.5f);
                    giygasBackdrop.DistortionStrength = child.AttrFloat("distortionStrength", 1.0f);
                    
                    // Parse colors
                    giygasBackdrop.BaseColor1 = new Color(
                        child.AttrInt("baseColor1R", 80),
                        child.AttrInt("baseColor1G", 0),
                        child.AttrInt("baseColor1B", 0)
                    );
                    giygasBackdrop.BaseColor2 = new Color(
                        child.AttrInt("baseColor2R", 120),
                        child.AttrInt("baseColor2G", 0),
                        child.AttrInt("baseColor2B", 60)
                    );
                    giygasBackdrop.BaseColor3 = new Color(
                        child.AttrInt("baseColor3R", 40),
                        child.AttrInt("baseColor3G", 0),
                        child.AttrInt("baseColor3B", 80)
                    );
                    giygasBackdrop.AccentColor = new Color(
                        child.AttrInt("accentColorR", 255),
                        child.AttrInt("accentColorG", 100),
                        child.AttrInt("accentColorB", 100)
                    );
                    
                    return giygasBackdrop;
                
                case "Ingeste/AncientRunes":
                    // Parse parameters from element for AncientRunes
                    string runeType = child.Attr("runeType", "ancient");
                    float glowIntensity = child.AttrFloat("glowIntensity", 1.0f);
                    float animationSpeed = child.AttrFloat("animationSpeed", 1.0f);
                    float fadeDistance = child.AttrFloat("fadeDistance", 200f);
                    return new Effects.AncientRunes(runeType, glowIntensity, animationSpeed, fadeDistance);
                
                case "Ingeste/MagicalAura":
                    // Parse parameters from element for MagicalAura
                    string colorType = child.Attr("colorType", "purple");
                    float intensity = child.AttrFloat("intensity", 1.0f);
                    float speed = child.AttrFloat("speed", 1.0f);
                    int particleCount = child.AttrInt("particleCount", 50);
                    return new Effects.MagicalAura(colorType, intensity, speed, particleCount);
                
                default:
                    // Not our custom backdrop, let the game handle it
                    return orig(self, child, above);
            }
        }

        private void RegisterHooks()
        {
            try
            {
                // OUI/Overworld integration removed - will be re-added later

                // Add Level hooks
                global::On.Celeste.Level.LoadLevel += Level_LoadLevel;
                global::On.Celeste.Level.Begin += Level_Begin;
                global::On.Celeste.Level.End += Level_End;
                global::On.Celeste.Level.TransitionTo += Level_TransitionTo;
                global::On.Celeste.Level.Update += Level_Update;

                // Initialize VanillaCore hooks for vanilla entity replacement
                try
                {
                    VanillaCore.VanillaCoreHooks.Initialize();
                    IngesteLogger.Info("VanillaCore hooks registered");
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"VanillaCore hooks registration failed: {ex.Message}");
                }

                // Initialize trigger hooks
                try
                {
                    var triggerType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.Triggers.IngesteEventTrigger");
                    if (triggerType != null)
                    {
                        var loadMethod = triggerType.GetMethod("Load", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        loadMethod?.Invoke(null, null);
                        IngesteLogger.Info("IngesteEventTrigger hooks registered");
                    }
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"IngesteEventTrigger hooks registration failed: {ex.Message}");
                }

                // Add Player hooks (if they exist)
                try
                {
                    var playerHooksType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.Player.PlayerSystem");
                    if (playerHooksType != null)
                    {
                        var hookMethod = playerHooksType.GetMethod("RegisterHooks", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        hookMethod?.Invoke(null, null);
                    }
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"Player hooks registration failed: {ex.Message}");
                }

                // Add NPC hooks
                try
                {
                    var npcHooksType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.CustomTalkComponent");
                    if (npcHooksType != null)
                    {
                        var hookMethod = npcHooksType.GetMethod("RegisterHooks", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        hookMethod?.Invoke(null, null);
                    }
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"NPC hooks registration failed: {ex.Message}");
                }

                IngesteLogger.Info("Hooks registered successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Error registering hooks");
            }
        }

        private void Level_LoadLevel(global::On.Celeste.Level.orig_LoadLevel orig, Level self, global::Celeste.Player.IntroTypes introTypes, bool isFromLoader)
        {
            IngesteLogger.Info("Level load initiated");
            orig(self, introTypes, isFromLoader);
            
            // Check if KirbyPlayer replacement is enabled
            if (Settings?.KirbyPlayerEnabled == true)
            {
                try
                {
                    IngesteLogger.Info("KirbyPlayer enabled - checking for player");
                    
                    // Find the Player entity in the level
                    var player = self.Tracker.GetEntity<global::Celeste.Player>();
                    if (player != null)
                    {
                        // Check if any KirbyPlayer spawn markers exist in the level
                        var kirbyPlayers = self.Tracker.GetEntities<KirbyPlayer>();
                        if (kirbyPlayers.Count > 0)
                        {
                            var firstKirbyPlayer = kirbyPlayers[0] as KirbyPlayer;
                            if (firstKirbyPlayer != null)
                            {
                                IngesteLogger.Info($"Moving Player to KirbyPlayer spawn marker at {firstKirbyPlayer.Position}");
                                
                                // Move the Player to the KirbyPlayer's position
                                player.Position = firstKirbyPlayer.Position;
                                
                                // Remove the spawn marker
                                firstKirbyPlayer.RemoveSelf();
                            }
                        }
                        
                        // Enable Kirby mode for the player
                        player.EnableKirbyMode();
                        IngesteLogger.Info("Kirby mode enabled for player");
                    }
                    else
                    {
                        IngesteLogger.Warn("No Player entity found to enable Kirby mode");
                    }
                }
                catch (Exception ex)
                {
                    IngesteLogger.Error(ex, "Error during KirbyPlayer setup logic");
                }
            }
            
            // Chapter 10 specific logic
            HandleChapter10Logic(self, isFromLoader);
            
            IngesteLogger.Info("Level load completed");
        }

        private void HandleChapter10Logic(Level level, bool isFromLoader)
        {
            try
            {
                // Check if we're in Chapter 10
                if (level.Session.Area.ID != 10)
                    return;

                var player = level.Tracker.GetEntity<global::Celeste.Player>();
                if (player == null)
                    return;

                // Check if this is the spawn room (first room)
                string currentRoom = level.Session.Level;
                bool isSpawnRoom = currentRoom.Contains("spawn") || currentRoom.Contains("start") || currentRoom.Contains("start");

                if (isSpawnRoom && !level.Session.GetFlag("ch10_maddy_baddy_chara_intro_played"))
                {
                    // Trigger the cutscene
                    IngesteLogger.Info("Triggering Chapter 10 intro cutscene");
                    level.Add(new Cutscenes.CS10_MaddyBaddyCharaIntro(player, currentRoom));
                }
                else if (!isSpawnRoom && level.Session.GetFlag("ch10_badeline_chara_present"))
                {
                    // Player has left the spawn room, remove NPCs
                    IngesteLogger.Info("Player left spawn room, removing CH10 NPCs");
                    Cutscenes.CS10_MaddyBaddyCharaIntro.RemoveNPCsOnRoomTransition(level);
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Error handling Chapter 10 logic");
            }
        }

        private void Level_Begin(global::On.Celeste.Level.orig_Begin orig, Level self)
        {
            IngesteLogger.Info("Level begin");
            orig(self);
            
            // Reset zoom on level begin to prevent leftover zoom from cutscenes or other mods
            try
            {
                if (self.Zoom != 1f || self.ZoomTarget != 1f)
                {
                    self.ResetZoom();
                    IngesteLogger.Info("Reset camera zoom on level begin");
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Failed to reset zoom on level begin: {ex.Message}");
            }
        }

        private void Level_End(global::On.Celeste.Level.orig_End orig, Level self)
        {
            IngesteLogger.Info("Level end");
            orig(self);
        }

        private void Level_TransitionTo(global::On.Celeste.Level.orig_TransitionTo orig, Level self, LevelData next, Vector2 direction)
        {
            IngesteLogger.Info("Level transition");
            
            // Reset zoom before room transition to prevent zoom from carrying over
            try
            {
                if (self.Zoom != 1f || self.ZoomTarget != 1f)
                {
                    self.ResetZoom();
                    IngesteLogger.Info("Reset camera zoom on room transition");
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Failed to reset zoom on transition: {ex.Message}");
            }
            
            orig(self, next, direction);
        }

        private void Level_Update(global::On.Celeste.Level.orig_Update orig, Level self)
        {
            orig(self);
        }

        public override void Unload()
        {
            try
            {
                IngesteLogger.Info("Starting module unload");

                // OUI/Overworld integration removed - will be re-added later

                // Unload shader effects
                try
                {
                    EffectManager.UnloadAll();
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"Shader effects cleanup failed: {ex.Message}");
                }

                // Unhook backdrop parsing
                On.Celeste.MapData.ParseBackdrop -= MapData_ParseBackdrop;

                // Unload external areas
                ExternalAreaDataManager.UnloadExternalAreas();

                // Clean up Cassette Player system
                try
                {
                    CassettePlayerSystem.UnregisterHooks();
                    CassettePlayerSystem.Unload();
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"Cassette Player cleanup failed: {ex.Message}");
                }

                // Clean up trigger hooks
                try
                {
                    var triggerType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.Triggers.IngesteEventTrigger");
                    if (triggerType != null)
                    {
                        var unloadMethod = triggerType.GetMethod("Unload", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        unloadMethod?.Invoke(null, null);
                        IngesteLogger.Info("IngesteEventTrigger hooks unregistered");
                    }
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"IngesteEventTrigger hooks cleanup failed: {ex.Message}");
                }

                // Clean up VanillaCore hooks
                try
                {
                    VanillaCore.VanillaCoreHooks.Cleanup();
                    IngesteLogger.Info("VanillaCore hooks unregistered");
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"VanillaCore hooks cleanup failed: {ex.Message}");
                }

                // Clean up FrostHelper integration
                FrostHelperIntegration.Cleanup();

                // Clean up NPC systems
                try
                {
                    CleanupNPCSystems();
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"NPC systems cleanup failed: {ex.Message}");
                }

                // Clean up player system hooks
                try
                {
                    var playerHooksType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.Player.PlayerSystem");
                    if (playerHooksType != null)
                    {
                        var cleanupMethod = playerHooksType.GetMethod("UnregisterHooks", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        cleanupMethod?.Invoke(null, null);
                    }
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"Player hooks cleanup failed: {ex.Message}");
                }

                // Clean up KirbyMode hooks (Aqua-style centralized hooks)
                try
                {
                    var kirbyHooksType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.Player.KirbyModeHooks");
                    if (kirbyHooksType != null)
                    {
                        var uninitMethod = kirbyHooksType.GetMethod("Uninitialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        uninitMethod?.Invoke(null, null);
                        IngesteLogger.Info("KirbyModeHooks uninitialized");
                    }
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"KirbyModeHooks cleanup failed: {ex.Message}");
                }

                // Clean up NPC hooks
                try
                {
                    var npcHooksType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.CustomTalkComponent");
                    if (npcHooksType != null)
                    {
                        var cleanupMethod = npcHooksType.GetMethod("UnregisterHooks", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        cleanupMethod?.Invoke(null, null);
                    }
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"NPC hooks cleanup failed: {ex.Message}");
                }

                // Clean up OuiDesoloTitleScreen hooks
                // TODO: OuiDesoloTitleScreenHooks class not yet implemented
                // try
                // {
                //     UI.OuiDesoloTitleScreenHooks.Unload();
                //     IngesteLogger.Info("OuiDesoloTitleScreen hooks unloaded");
                // }
                // catch (Exception ex)
                // {
                //     IngesteLogger.Warn($"OuiDesoloTitleScreen hooks cleanup failed: {ex.Message}");
                // }

                // Clean up DesoloZantas Main Menu extensions
                // TODO: DesoloMainMenuExt.Unload() method not yet implemented
                // try
                // {
                //     UI.DesoloMainMenuExt.Unload();
                //     IngesteLogger.Info("DesoloZantas Main Menu extensions unloaded");
                // }
                // catch (Exception ex)
                // {
                //     IngesteLogger.Warn($"Main Menu extensions cleanup failed: {ex.Message}");
                // }

                // OLD: DesoloZantas Title Logo Manager unload - disabled
                // try
                // {
                //     UI.DesoloTitleLogoManager.Unload();
                //     IngesteLogger.Info("DesoloZantas Title Logo Manager unloaded");
                // }
                // catch (Exception ex)
                // {
                //     IngesteLogger.Warn($"Title Logo Manager cleanup failed: {ex.Message}");
                // }

                // Clean up DesoloZantas Chapter Select hooks
                // TODO: DesoloChapterSelectHooks class not yet implemented
                // try
                // {
                //     UI.DesoloChapterSelectHooks.Unload();
                //     IngesteLogger.Info("DesoloZantas Chapter Select hooks unloaded");
                // }
                // catch (Exception ex)
                // {
                //     IngesteLogger.Warn($"Chapter Select hooks cleanup failed: {ex.Message}");
                // }

                // Clean up DesoloZantas Mountain Manager
                try
                {
                    Mountain.DesoloMountainManager.Unload();
                    IngesteLogger.Info("DesoloZantas Mountain Manager unloaded");
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"Mountain Manager cleanup failed: {ex.Message}");
                }

                // OLD: Intro Presents Manager unload - disabled
                // try
                // {
                //     UI.IntroPresentsManager.Unload();
                //     IngesteLogger.Info("Intro Presents Manager unloaded");
                // }
                // catch (Exception ex)
                // {
                //     IngesteLogger.Warn($"Intro Presents Manager cleanup failed: {ex.Message}");
                // }

                // Clean up DesoloZantas Overworld Loader Manager
                // TODO: DesoloOverworldLoaderManager class not yet implemented
                // try
                // {
                //     UI.DesoloOverworldLoaderManager.Unload();
                //     IngesteLogger.Info("DesoloZantas Overworld Loader Manager unloaded");
                // }
                // catch (Exception ex)
                // {
                //     IngesteLogger.Warn($"DesoloZantas Overworld Loader Manager cleanup failed: {ex.Message}");
                // }

                // Clean up strawberry hooks
                try
                {
                    var strawberryHooksType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.StrawberryHooks");
                    if (strawberryHooksType != null)
                    {
                        var cleanupMethod = strawberryHooksType.GetMethod("Cleanup", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        cleanupMethod?.Invoke(null, null);
                    }
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"Strawberry hooks cleanup failed: {ex.Message}");
                }

                // Unregister BossesHelper hooks
                UnregisterBossesHelperHooks();

                CleanupParticles();
                CleanupEntityRegistration();

                // Clean up core hooks
                global::On.Celeste.Level.LoadLevel -= Level_LoadLevel;
                global::On.Celeste.Level.Begin -= Level_Begin;
                global::On.Celeste.Level.End -= Level_End;
                global::On.Celeste.Level.TransitionTo -= Level_TransitionTo;
                global::On.Celeste.Level.Update -= Level_Update;

                // Clean up console commands
                try
                {
                    // FunctionKeyActions is an Action[] (not a dictionary), so we can't use ContainsKey or Remove.
                    // Instead, set the F1 slot to null to "unregister" the action.
                    if (Engine.Commands != null && Engine.Commands.FunctionKeyActions.Length > IngesteConstants.Console.F1_KEY_INDEX)
                    {
                        Engine.Commands.FunctionKeyActions[IngesteConstants.Console.F1_KEY_INDEX] = null;
                    }
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"Console commands cleanup failed: {ex.Message}");
                }

                try
                {
                    RefreshConsoleCommandList();
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"Console commands deregistration failed: {ex.Message}");
                }

                IngesteLogger.Info("Module unloaded successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Error during unload");
            }
        }

        public static void SetPersistentSetting(string s, bool b)
        {
            if (Settings != null)
            {
                DynamicData.For(Settings).Set(s, b);
            }
        }

        private static void RefreshConsoleCommandList()
        {
            try
            {
                if (Engine.Commands != null)
                {
                    Engine.Commands.ReloadCommandsList();
                    Logger.Log(LogLevel.Info, "IngesteModule", "Console command list reloaded");
                }
                else
                {
                    Logger.Log(LogLevel.Verbose, "IngesteModule", "Engine.Commands not ready; command list reload skipped");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteModule", $"Failed to refresh console commands: {ex.Message}");
            }
        }

        /// <summary>
        /// Register all custom entities and triggers with Everest
        /// </summary>
        private void RegisterCustomEntitiesAndTriggers()
        {
            try
            {
                IngesteLogger.Info("Registering custom entities and triggers");

                var assembly = typeof(IngesteModule).Assembly;
                int entityCount = 0;
                int triggerCount = 0;

                foreach (var type in assembly.GetTypes())
                {
                    try
                    {
                        var customEntityAttrs = type.GetCustomAttributes(typeof(CustomEntityAttribute), false);
                        if (customEntityAttrs.Length > 0)
                        {
                            foreach (CustomEntityAttribute attr in customEntityAttrs)
                            {
                                string entityName = attr.IDs?.FirstOrDefault() ?? type.Name;
                                IngesteLogger.Debug($"Found custom entity: {entityName} -> {type.FullName}");
                                entityCount++;
                            }
                        }

                        if (typeof(Trigger).IsAssignableFrom(type) && customEntityAttrs.Length > 0)
                        {
                            foreach (CustomEntityAttribute attr in customEntityAttrs)
                            {
                                string triggerName = attr.IDs?.FirstOrDefault() ?? type.Name;
                                IngesteLogger.Debug($"Found custom trigger: {triggerName} -> {type.FullName}");
                                triggerCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        IngesteLogger.Warn($"Error checking type {type.Name}: {ex.Message}");
                    }
                }

                IngesteLogger.Info($"Entity/Trigger registration completed: {entityCount} entities, {triggerCount} triggers");

                // Force static constructors for known entity types
                EnsureEntityRegistration();

                IngesteLogger.Info("Custom entities and triggers registered successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Error registering custom entities and triggers");
            }
        }

        private void EnsureEntityRegistration()
        {
            try
            {
                // ? Use reflection to safely trigger static constructors for all entities
                foreach (var typeName in ForcedEntityTypeNames)
                {
                    try
                    {
                        var type = typeof(IngesteModule).Assembly.GetType(typeName);
                        if (type != null)
                        {
                            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                            initializedEntityTypes.Add(type);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Warn, "IngesteModule", $"Failed to initialize {typeName}: {ex.Message}");
                    }
                }

                Logger.Log(LogLevel.Debug, "IngesteModule", "Key entity static constructors triggered");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteModule", $"Error running static constructors: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize audio directory structure and load FMOD banks
        /// </summary>
        private void InitializeAudioDirectory()
        {
            try
            {
                string modRoot = Path.GetDirectoryName(typeof(IngesteModule).Assembly.Location);
                if (!string.IsNullOrEmpty(modRoot))
                {
                    string audioPath = Path.Combine(modRoot, "Audio");

                    if (Directory.Exists(audioPath))
                    {
                        Logger.Log(LogLevel.Info, "IngesteModule", $"Audio directory found at: {audioPath}");

                        var bankFiles = Directory.GetFiles(audioPath, "*.bank")
                            .Where(f => !string.IsNullOrWhiteSpace(Path.GetFileNameWithoutExtension(f)))
                            .Where(f => !Path.GetFileName(f).Contains(" "))
                            .ToArray();

                        if (bankFiles.Length > 0)
                        {
                            Logger.Log(LogLevel.Info, "IngesteModule", $"Found {bankFiles.Length} valid audio bank files - Everest will auto-load these");
                            
                            // Log each bank file
                            foreach (var bankFile in bankFiles)
                            {
                                string bankName = Path.GetFileName(bankFile);
                                Logger.Log(LogLevel.Info, "IngesteModule", $"  ✓ {bankName}");
                            }
                            
                            // Log bank naming conventions for verification
                            Logger.Log(LogLevel.Info, "IngesteModule", "Audio banks should be accessible via event paths like:");
                            Logger.Log(LogLevel.Info, "IngesteModule", "  event:/music/desolozantas/your_music_event");
                            Logger.Log(LogLevel.Info, "IngesteModule", "  event:/sfx/desolozantas/your_sfx_event");
                        }
                        else
                        {
                            Logger.Log(LogLevel.Warn, "IngesteModule", "No valid FMOD bank files found - mod will use vanilla audio only");
                        }
                    }
                    else
                    {
                        Logger.Log(LogLevel.Warn, "IngesteModule", "No audio directory found - using vanilla audio only");
                    }
                }

                Logger.Log(LogLevel.Info, "IngesteModule", "Audio directory initialization completed");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteModule", $"Error initializing audio directory: {ex}");
            }
        }

        /// <summary>
        /// Initialize particle systems for custom entities
        /// </summary>
        private void InitializeParticles()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing particle systems");

                // ? Check if types exist before calling LoadParticles
                foreach (var typeName in ParticleProviderTypeNames)
                {
                    try 
                    { 
                        var type = typeof(IngesteModule).Assembly.GetType(typeName);
                        if (type != null)
                        {
                            var loadMethod = type.GetMethod("LoadParticles", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            if (loadMethod != null)
                            {
                                loadMethod.Invoke(null, null);
                                initializedParticleProviders.Add(type);
                            }
                        }
                    }
                    catch (Exception ex) 
                    { 
                        Logger.Log(LogLevel.Warn, "IngesteModule", $"Failed to load {typeName} particles: {ex.Message}"); 
                    }
                }

                Logger.Log(LogLevel.Info, "IngesteModule", "Particle systems initialized");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteModule", $"Error initializing particles: {ex}");
            }
        }

        /// <summary>
        /// Initialize MonoMod exports for cross-mod compatibility
        /// </summary>
        private void InitializeMonoModExports()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing MonoMod exports");
                // Only call if IngesteExports exists
                var exportsType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.IngesteExports");
                if (exportsType != null)
                {
                    exportsType.ModInterop();
                    Logger.Log(LogLevel.Info, "IngesteModule", "MonoMod exports initialized");
                }
                else
                {
                    Logger.Log(LogLevel.Info, "IngesteModule", "No MonoMod exports found - skipping");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteModule", $"Error initializing MonoMod exports: {ex}");
            }
        }

        /// <summary>
        /// Initialize FrostHelper integration
        /// </summary>
        private void InitializeFrostHelperIntegration()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing FrostHelper integration");
                FrostHelperIntegration.Initialize();
                Logger.Log(LogLevel.Info, "IngesteModule", "FrostHelper integration initialized");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteModule", $"FrostHelper integration failed (continuing without it): {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize cutscene manager
        /// </summary>
        private void InitializeCutsceneManager()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing cutscene manager");
                var cutsceneManagerType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.CutsceneManager");
                if (cutsceneManagerType != null)
                {
                    var initMethod = cutsceneManagerType.GetMethod("Initialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    initMethod?.Invoke(null, null);
                }
                Logger.Log(LogLevel.Info, "IngesteModule", "Cutscene manager initialized");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteModule", $"Cutscene manager initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize HLSL shader effect system
        /// </summary>
        private void InitializeShaderEffects()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing HLSL shader effects");
                EffectManager.Initialize();
                Logger.Log(LogLevel.Info, "IngesteModule", "Shader effects initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteModule", $"Shader effects initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize companion character manager
        /// </summary>
        private void InitializeCompanionManager()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing companion character manager");
                var companionManagerType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.CompanionCharacterManager");
                if (companionManagerType != null)
                {
                    var initMethod = companionManagerType.GetMethod("Initialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    initMethod?.Invoke(null, null);
                }
                Logger.Log(LogLevel.Info, "IngesteModule", "Companion character manager initialized");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteModule", $"Companion character manager initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize player systems (states, abilities, etc.)
        /// </summary>
        private void InitializePlayerSystems()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing player systems");

                var playerSystemTypes = new[]
                {
                    "DesoloZantas.Core.Player.PlayerSystem",
                    "DesoloZantas.Core.Player.PlayerKirbyStates",
                    "DesoloZantas.Core.Player.PlayerAbilities",
                    "DesoloZantas.Core.PlayerDashAssist",
                    "DesoloZantas.Core.PlayerSprite",
                    "DesoloZantas.Core.Player.KirbyModeHooks"  // Aqua-style centralized hooks
                };

                foreach (var typeName in playerSystemTypes)
                {
                    try
                    {
                        var type = typeof(IngesteModule).Assembly.GetType(typeName);
                        if (type != null)
                        {
                            var initMethod = type.GetMethod("Initialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            initMethod?.Invoke(null, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Warn, "IngesteModule", $"Failed to initialize {typeName}: {ex.Message}");
                    }
                }

                Logger.Log(LogLevel.Info, "IngesteModule", "Player systems initialized");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteModule", $"Player systems initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize NPC systems
        /// </summary>
        private void InitializeNPCSystems()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing NPC systems");

                var npcTypes = new[]
                {
                    "DesoloZantas.Core.NPCs.Npc06Theo",
                    "DesoloZantas.Core.NPCs.Npc03Theo",
                    "DesoloZantas.Core.NPCs.Npc16Theo"
                };

                foreach (var typeName in npcTypes)
                {
                    try
                    {
                        var type = typeof(IngesteModule).Assembly.GetType(typeName);
                        if (type != null)
                        {
                            var loadMethod = type.GetMethod("Load", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            loadMethod?.Invoke(null, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Warn, "IngesteModule", $"Failed to load NPC {typeName}: {ex.Message}");
                    }
                }

                Logger.Log(LogLevel.Info, "IngesteModule", "NPC systems initialized");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteModule", $"NPC systems initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Initialize custom cutscene systems with PrismaticHelper integration
        /// </summary>
        private void InitializeCustomCutscenes()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing custom cutscene systems");

                // Load custom cutscene triggers with PrismaticHelper
                CustomCutsceneTriggers.Load();

                Logger.Log(LogLevel.Info, "IngesteModule", "Custom cutscene systems initialized");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteModule", $"Custom cutscene initialization failed: {ex.Message}");
                // This is non-critical, so we don't rethrow
            }
        }

        /// <summary>
        /// Initialize strawberry collection hooks
        /// </summary>
        private void InitializeStrawberryHooks()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing strawberry hooks");

                var strawberryHooksType = typeof(IngesteModule).Assembly.GetType("DesoloZantas.Core.StrawberryHooks");
                if (strawberryHooksType != null)
                {
                    var initMethod = strawberryHooksType.GetMethod("Initialize", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                    initMethod?.Invoke(null, null);
                    
                    Logger.Log(LogLevel.Info, "IngesteModule", "Strawberry hooks initialized");
                }
                else
                {
                    Logger.Log(LogLevel.Info, "IngesteModule", "No strawberry hooks found - using default behavior");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteModule", $"Strawberry hooks initialization failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Clean up NPC systems
        /// </summary>
        private void CleanupNPCSystems()
        {
            try
            {
                IngesteLogger.Info("Cleaning up NPC systems");

                var npcTypes = new[]
                {
                    "DesoloZantas.Core.NPCs.Npc06Theo",
                    "DesoloZantas.Core.NPCs.Npc03Theo",
                    "DesoloZantas.Core.NPCs.Npc16Theo"
                };

                foreach (var typeName in npcTypes)
                {
                    try
                    {
                        var type = typeof(IngesteModule).Assembly.GetType(typeName);
                        if (type != null)
                        {
                            var unloadMethod = type.GetMethod("Unload", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            unloadMethod?.Invoke(null, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        IngesteLogger.Warn($"Failed to unload NPC {typeName}: {ex.Message}");
                    }
                }

                IngesteLogger.Info("NPC systems cleaned up");
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"NPC systems cleanup failed: {ex.Message}");
            }
        }

        private void CleanupEntityRegistration()
        {
            foreach (var type in initializedEntityTypes)
            {
                try
                {
                    InvokeOptionalStaticCleanup(type, "Unload", "Cleanup", "Dispose", "UnloadContent");
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"Entity cleanup failed for {type.FullName}: {ex.Message}");
                }
            }

            initializedEntityTypes.Clear();
        }

        private void CleanupParticles()
        {
            foreach (var type in initializedParticleProviders)
            {
                try
                {
                    InvokeOptionalStaticCleanup(type, "UnloadParticles", "CleanupParticles");
                }
                catch (Exception ex)
                {
                    IngesteLogger.Warn($"Particle cleanup failed for {type.FullName}: {ex.Message}");
                }
            }

            initializedParticleProviders.Clear();
        }

        private static void InvokeOptionalStaticCleanup(Type type, params String[] methodNames)
        {
            foreach (var methodName in methodNames)
            {
                var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (method != null && method.GetParameters().Length == 0)
                {
                    method.Invoke(null, null);
                    IngesteLogger.Debug($"Invoked {methodName} on {type.FullName}");
                    break;
                }
            }
        }

        #region BossesHelper Integration
        // TODO: BossesHelper integration has API incompatibilities that need to be resolved:
        // 1. Player namespace vs Celeste.Player type conflicts
        // 2. Missing Scene extension methods (GetPlayer, GetEntity)
        // 3. Private member access (PlayerDeadBody.bounce, scale, etc.)
        // 4. Missing DetourConfigContext type
        // 5. Hook signature mismatches (Level.hook_LoadLevel, Player.IntroTypes)
        // The original code has been commented out below.
        
        private void RegisterBossesHelperHooks()
        {
            IngesteLogger.Info("BossesHelper hooks registration disabled - API compatibility issues need to be resolved");
        }

        private void UnregisterBossesHelperHooks()
        {
            // No-op since hooks weren't registered
        }
        
        /* ORIGINAL BOSSESHELPER INTEGRATION CODE - NEEDS API FIXES
        private void RegisterBossesHelperHooks_DISABLED()
        {
            try
            {
                IngesteLogger.Info("Registering BossesHelper hooks");
                
                typeof(IngesteExports).ModInterop();
                
                using (new DetourConfigContext(new DetourConfig("BossesHelperEnforceBounds", 0, after: ["*"])).Use())
                {
                    On.Celeste.Level.EnforceBounds += BossesHelper_PlayerDiedWhileEnforceBounds;
                }
                On.Celeste.Level.LoadLevel += BossesHelper_SetStartingHealth;
                On.Celeste.Player.Update += BossesHelper_UpdatePlayerLastSafe;
                IL.Celeste.Player.OnSquish += BossesHelper_ILOnSquish;
                On.Celeste.Player.Die += BossesHelper_OnPlayerDie;
                
                IngesteLogger.Info("BossesHelper hooks registered successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to register BossesHelper hooks");
            }
        }

        private void UnregisterBossesHelperHooks()
        {
            try
            {
                On.Celeste.Level.EnforceBounds -= BossesHelper_PlayerDiedWhileEnforceBounds;
                On.Celeste.Level.LoadLevel -= BossesHelper_SetStartingHealth;
                On.Celeste.Player.Update -= BossesHelper_UpdatePlayerLastSafe;
                IL.Celeste.Player.OnSquish -= BossesHelper_ILOnSquish;
                On.Celeste.Player.Die -= BossesHelper_OnPlayerDie;
                DesoloZantas.BossesHelper.Code.Helpers.ILHookHelper.DisposeAll();
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Failed to unregister BossesHelper hooks: {ex.Message}");
            }
        }

        private static void BossesHelper_PlayerDiedWhileEnforceBounds(On.Celeste.Level.orig_EnforceBounds orig, Level self, global::Celeste.Player player)
        {
            Session.BossesHelper_WasOffscreen = true;
            orig(self, player);
            Session.BossesHelper_WasOffscreen = false;
        }

        public static void BossesHelper_SetStartingHealth(On.Celeste.Level.orig_LoadLevel orig, Level self, DesoloZantas.Core.Player.IntroTypes intro, bool fromLoader = false)
        {
            if (fromLoader)
            {
                if (Session.BossesHelper_HealthData.isCreated)
                    self.Add(new BHEntities.HealthSystemManager());
                if (Session.BossesHelper_SafeGroundBlockerCreated)
                    self.Add(new BHEntities.UpdateSafeBlocker());
            }
            orig(self, intro, fromLoader);
            if (Engine.Scene.GetPlayer() is Player entity)
            {
                entity.Sprite.Visible = true;
                entity.Hair.Visible = true;
                Session.BossesHelper_SafeSpawn = entity.SceneAs<Level>().Session.RespawnPoint ?? entity.Position;
                if (Session.BossesHelper_SavePointSet && !Session.BossesHelper_TravelingToSavePoint && intro == DesoloZantas.Core.Player.IntroTypes.Respawn)
                {
                    Session.BossesHelper_TravelingToSavePoint = true;
                    self.TeleportTo(entity, Session.BossesHelper_SavePointLevel, Session.BossesHelper_SavePointSpawnType, Session.BossesHelper_SavePointSpawn);
                    Session.BossesHelper_TravelingToSavePoint = false;
                }
                entity.AddIFramesWatch(true);
            }
            if (Engine.Scene.GetEntity<BHEntities.HealthSystemManager>() is not { } manager || !BHEntities.HealthSystemManager.IsEnabled)
                return;
            if (Session.BossesHelper_HealthData.globalController &&
                (intro == DesoloZantas.Core.Player.IntroTypes.Transition && !Session.BossesHelper_HealthData.globalHealth ||
                intro == DesoloZantas.Core.Player.IntroTypes.Respawn && !fromLoader && !Session.BossesHelper_FakeDeathRespawn))
            {
                manager.RefillHealth();
            }
            Session.BossesHelper_FakeDeathRespawn = false;
        }

        public static void BossesHelper_UpdatePlayerLastSafe(On.Celeste.Player.orig_Update orig, global::Celeste.Player self)
        {
            orig(self);
            if (self.OnSafeGround && self.Scene.GetEntity<BHEntities.UpdateSafeBlocker>() == null)
                Session.BossesHelper_LastSafePosition = self.Position.NearestWhole();
            if (self.StateMachine.State != Player.StCassetteFly)
                Session.BossesHelper_AlreadyFlying = false;
            if (self.SceneAs<Level>().Session.RespawnPoint is Vector2 spawn && Session.BossesHelper_LastSpawnPoint != spawn)
                Session.BossesHelper_SafeSpawn = spawn;
        }

        public static void BossesHelper_ILOnSquish(ILContext il)
        {
            try
            {
                var cursor = new ILCursor(il);
                if (cursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("Die")))
                {
                    cursor.Emit(OpCodes.Ldarg_0);
                    cursor.Emit(OpCodes.Ldarg_1);
                    cursor.EmitDelegate<Action<Player, CollisionData>>((player, data) =>
                    {
                        BossesHelper_KillOnCrush(player, data, true);
                    });
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to IL hook Player.OnSquish");
            }
        }

        public static global::Celeste.PlayerDeadBody BossesHelper_OnPlayerDie(On.Celeste.Player.orig_Die orig, CelestePlayer self, Vector2 dir, bool always, bool register)
        {
            bool damageTracked = self.Scene.Tracker.GetEntity<BHEntities.HealthSystemManager>() != null && BHEntities.HealthSystemManager.IsEnabled;
            if (always)
            {
                if (damageTracked)
                    BossesHelper_PlayerTakesDamage(amount: Session.BossesHelper_CurrentPlayerHealth, evenIfInvincible: true);
                return orig(self, dir, always, register);
            }
            if (damageTracked && Session.BossesHelper_CurrentPlayerHealth <= 0)
                return orig(self, dir, always, register);
            if (Session.BossesHelper_UseFakeDeath)
                return BossesHelper_FakeDie(self, dir);
            if (self.Get<BHComponents.Stopwatch>() is BHComponents.Stopwatch watch && !watch.Finished)
                return null;
            if (!damageTracked)
                return orig(self, dir, always, register);

            if (!BossesHelper_KillOffscreen(self))
                BossesHelper_PlayerTakesDamage(dir);
            return null;
        }

        public static void BossesHelper_KillOnCrush(global::Celeste.Player player, CollisionData data, bool evenIfInvincible)
        {
            if (player.Scene.GetEntity<BHEntities.HealthSystemManager>() == null || !BHEntities.HealthSystemManager.IsEnabled)
                return;
            switch (Session.BossesHelper_HealthData.playerOnCrush)
            {
                case BHEntities.HealthSystemManager.CrushEffect.PushOut:
                    BossesHelper_PlayerTakesDamage();
                    if (!player.TrySquishWiggle(data, (int)data.Pusher.Width, (int)data.Pusher.Height))
                        player.TrySquishWiggle(data, player.level.Bounds.Width, player.level.Bounds.Height);
                    break;
                case BHEntities.HealthSystemManager.CrushEffect.InvincibleSolid:
                    if (evenIfInvincible) break;
                    BossesHelper_PlayerTakesDamage();
                    data.Pusher.Add(new BHComponents.SolidOnInvinciblePlayer());
                    break;
                default:
                    BossesHelper_SharedDeath(player, Session.BossesHelper_HealthData.playerOnCrush == BHEntities.HealthSystemManager.CrushEffect.FakeDeath);
                    break;
            }
        }

        private static bool BossesHelper_KillOffscreen(global::Celeste.Player player)
        {
            float? offscreemAtY = BossesHelper_GetFromY(player);
            if (offscreemAtY is not float atY)
                return false;
            switch (Session.BossesHelper_HealthData.playerOffscreen)
            {
                case BHEntities.HealthSystemManager.OffscreenEffect.BounceUp:
                    BossesHelper_PlayerTakesDamage(stagger: false);
                    player.Play("event:/game/general/assist_screenbottom");
                    player.Bounce(atY);
                    break;
                case BHEntities.HealthSystemManager.OffscreenEffect.BubbleBack:
                    BossesHelper_PlayerTakesDamage(stagger: false);
                    if (!Session.BossesHelper_AlreadyFlying)
                        BossesHelper_PlayerFlyBack(player).AsCoroutine(player);
                    break;
                default:
                    BossesHelper_SharedDeath(player, Session.BossesHelper_HealthData.playerOffscreen == BHEntities.HealthSystemManager.OffscreenEffect.FakeDeath);
                    break;
            }
            return true;
        }

        private static void BossesHelper_SharedDeath(global::Celeste.Player player, bool fakeDeath)
        {
            if (fakeDeath)
            {
                BossesHelper_PlayerTakesDamage(stagger: false, evenIfInvincible: true);
                BossesHelper_FakeDie(player);
                return;
            }
            BossesHelper_PlayerTakesDamage(amount: Session.BossesHelper_CurrentPlayerHealth, evenIfInvincible: true);
        }

        private static global::Celeste.PlayerDeadBody BossesHelper_FakeDie(global::Celeste.Player self, Vector2? dir = null)
        {
            Level level = self.SceneAs<Level>();

            if (!self.Dead && self.StateMachine.State != Player.StReflectionFall)
            {
                Session.BossesHelper_FakeDeathRespawn = true;
                self.Stop(self.wallSlideSfx);
                self.Depth = -1000000;
                self.Speed = Vector2.Zero;
                self.StateMachine.Locked = true;
                self.Collidable = false;
                self.Drop();
                self.LastBooster?.PlayerDied();
                self.level.InCutscene = false;
                self.level.Shake();
                Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
                PlayerDeadBody fakeDeadBody = new(self, dir ?? Vector2.UnitY * -1)
                {
                    DeathAction = () =>
                    {
                        self.Position = Session.BossesHelper_LastSafePosition;
                        level.DoScreenWipe(true);
                    }
                };
                fakeDeadBody.Get<Coroutine>().Replace(BossesHelper_FakeDeathRoutine(fakeDeadBody));
                level.Add(fakeDeadBody);
                level.Remove(self);
                level.Tracker.GetEntity<Lookout>()?.StopInteracting();
            }
            return null;
        }

        private static IEnumerator BossesHelper_FakeDeathRoutine(PlayerDeadBody self)
        {
            Level level = self.SceneAs<Level>();
            if (self.bounce != Vector2.Zero)
            {
                Audio.Play("event:/char/madeline/predeath", self.Position);
                self.scale = 1.5f;
                Celeste.Celeste.Freeze(0.05f);
                yield return null;
                Vector2 from = self.Position;
                Vector2 to = from + self.bounce * 24f;
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, start: true);
                self.Add(tween);
                tween.OnUpdate = t =>
                {
                    self.Position = from + (to - from) * t.Eased;
                    self.scale = 1.5f - t.Eased * 0.5f;
                    self.sprite.Rotation = (float)(Math.Floor(t.Eased * 4f) * 6.2831854820251465);
                };
                yield return tween.Duration * 0.75f;
                tween.Stop();
            }
            self.Position += Vector2.UnitY * -5f;
            level.Shake();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            self.End();
        }

        private static IEnumerator BossesHelper_PlayerFlyBack(global::Celeste.Player player)
        {
            Session.BossesHelper_AlreadyFlying = true;
            yield return 0.3f;
            Audio.Play("event:/game/general/cassette_bubblereturn", player.SceneAs<Level>().Camera.Position + new Vector2(160f, 90f));
            Vector2 middle = new(player.X + (Session.BossesHelper_LastSafePosition.X - player.X) / 2, player.Y + (Session.BossesHelper_LastSafePosition.Y - player.Y) / 2);
            player.StartCassetteFly(Session.BossesHelper_LastSafePosition, middle - Vector2.UnitY * 8);
        }

        public static void BossesHelper_PlayerTakesDamage(Vector2 origin = default, int amount = 1, bool silent = false, bool stagger = true, bool evenIfInvincible = false)
        {
            Engine.Scene.GetEntity<BHEntities.HealthSystemManager>()?.TakeDamage(origin, amount, silent, stagger, evenIfInvincible);
        }

        private static float? BossesHelper_GetFromY(global::Celeste.Player player)
        {
            Level level = player.SceneAs<Level>();
            if (!Session.BossesHelper_WasOffscreen)
                return null;
            Rectangle camera = new((int)level.Camera.Left, (int)level.Camera.Top, 320, 180);
            if (level.CameraLockMode != Level.CameraLockModes.None)
            {
                if (camera.Bottom < level.Bounds.Bottom - 4 && player.Top > camera.Bottom)
                    return camera.Top;
                if (camera.Top > level.Bounds.Top + 4 && player.Bottom < level.Bounds.Top)
                    return level.Bounds.Top;
            }
            if (player.Bottom < level.Bounds.Top)
                return level.Bounds.Top;
            if (player.Top > level.Bounds.Bottom)
                return level.Bounds.Bottom;
            return null;
        }

        public static void BossesHelper_GiveIFrames(float time)
        {
            Player player = Engine.Scene.GetPlayer();
            player.AddIFramesWatch(true);
            player.Get<BHComponents.Stopwatch>().TimeLeft += time;
        }
        END OF ORIGINAL BOSSESHELPER CODE */

        #endregion
    }
}








