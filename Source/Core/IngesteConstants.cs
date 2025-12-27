namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Centralized constants for the Ingeste mod to reduce magic strings throughout the codebase
    /// </summary>
    public static class IngesteConstants
    {
        public const string MOD_NAME = "DesoloZantas";
        public const string INTERNAL_NAME = "Ingeste";
        public const string ENTITY_PREFIX = "Ingeste/";
        
        /// <summary>
        /// Core entity and trigger names used throughout the mod
        /// </summary>
        public static class EntityNames
        {
            // Core gameplay entities
            public const string SAMPLE_ENTITY = "Ingeste/SampleEntity";
            public const string SAMPLE_TRIGGER = "Ingeste/SampleTrigger";
            public const string ANCIENT_SWITCH = "Ingeste/AncientSwitch";
            public const string CLUTTER_BLOCK = "Ingeste/ClutterBlock";
            public const string CLUTTER_SWITCH = "Ingeste/ClutterSwitch";
            public const string CUBE_DREAM_BLOCK = "Ingeste/CubeDreamBlock";
            public const string DELTA_BERRY = "Ingeste/DeltaBerry";
            public const string ENEMY = "Ingeste/Enemy";
            public const string FAKE_HEART_GEM = "Ingeste/FakeHeartGem";
            public const string HEART_GEM_MOD = "Ingeste/Heartgemmod";
            public const string HEART_STAFF = "Ingeste/HeartStaff";
            public const string HEART_STAFF_DOOR = "Ingeste/HeartStaffDoor";
            public const string ICE_BOUNCER = "Ingeste/IceBouncer";
            public const string GLITCH_GLIDER = "Ingeste/GlitchGlider";
            public const string MADDY_CRYSTAL = "Ingeste/Maddy_crystal";
            
            // Character dummy entities
            public const string ADELINE_DUMMY = "Ingeste/AdelineDummy";
            public const string ASRIEL_DUMMY = "Ingeste/AsrielDummy";
            public const string BATTY_DUMMY = "Ingeste/BattyDummy";
            public const string CHARLO_DUMMY = "Ingeste/CharloDummy";
            public const string CLOVER_DUMMY = "Ingeste/CloverDummy";
            public const string CODY_DUMMY = "Ingeste/CodyDummy";
            public const string DARK_META_KNIGHT_DUMMY = "Ingeste/DarkMetaKnightDummy";
            public const string EMILY_DUMMY = "Ingeste/EmilyDummy";
            public const string FLAMBERGE_ZALEA_DUMMY = "Ingeste/FlambergeZaleaDummy";
            public const string FRAN_ZALEA_DUMMY = "Ingeste/FranZaleaDummy";
            public const string HYNES_ZALEA_DUMMY = "Ingeste/HynesZaleaDummy";
            public const string KING_DDD_DUMMY = "Ingeste/KingDDDDummy";
            public const string KIRBY_CLASSIC_DUMMY = "Ingeste/KirbyClassicDummy";
            public const string MAGOLOR_DUMMY = "Ingeste/MagolorDummy";
            public const string MARX_DUMMY = "Ingeste/MarxDummy";
            public const string MELODY_DUMMY = "Ingeste/MelodyDummy";
            
            // Dream Friend entities
            public const string ADELINE_DREAM_FRIEND = "Ingeste/AdeleineDreamFriend";
            public const string GOOEY_DREAM_FRIEND = "Ingeste/GooeyDreamFriend";
            public const string KING_DEDEDE_DREAM_FRIEND = "Ingeste/KingDededeDreamFriend";
            public const string FRANCISCA_DREAM_FRIEND = "Ingeste/FranciscaDreamFriend";
            public const string FLAMBERGE_DREAM_FRIEND = "Ingeste/FlambergeDreamFriend";
            public const string ZAN_PARTIZANNE_DREAM_FRIEND = "Ingeste/ZanPartizanneDreamFriend";
            
            // Player system entities
            public const string KIRBY_PLAYER = "Ingeste/Kirby_Player";
            
            // NPCs
            public const string BIRD_NPC_MOD = "Ingeste/BirdNPCMod";
            public const string CUSTOM_NPC = "Ingeste/CustomNPC";
            public const string NPC_EVENT = "Ingeste/NPC_Event";
            public const string NPC05_OSHIRO_LOBBY = "Ingeste/NPC05_Oshiro_Lobby";
            public const string NPC07_BADELINE = "Ingeste/NPC07_Badeline";
            public const string NPC08_CHARA_CRYING = "Ingeste/NPC08_Chara_Crying";
            
            // Gameplay mechanics
            public const string CHARA_CHASER = "Ingeste/CharaChaser";
            public const string FLING_BIRD_INTRO_MOD = "Ingeste/FlingBirdIntroMod";
            public const string GONDOLA_MOD = "Ingeste/Gondola_Mod";
            
            // Cutscenes
            public const string CS12_TOWER_FOUNTAIN = "Ingeste/CS12_TowerFountain";
            
            // Triggers
            public const string EVENT_TRIGGER = "Ingeste/EventTrigger";
            public const string DIALOG_TRIGGER = "Ingeste/DialogTrigger";
            public const string DELTA_BERRY_UNLOCK_CUTSCENE_TRIGGER = "Ingeste/DeltaBerryUnlockCutsceneTrigger";
            public const string ENHANCED_MUSIC_TRIGGER = "Ingeste/MusicTrigger";
            public const string KIRBY_PLAYER_TRIGGER = "Ingeste/Kirby_Player_Trigger";
            public const string TOWER_FOUNTAIN_TRIGGER = "Ingeste/TowerFountainTrigger";
            public const string WIND_TRIGGER = "Ingeste/WindTrigger";
        }
        
        /// <summary>
        /// Directory and file path constants
        /// </summary>
        public static class Paths
        {
            public const string AUDIO_DIRECTORY = "Audio";
            public const string GRAPHICS_DIRECTORY = "Graphics";
            public const string DIALOG_DIRECTORY = "Dialog";
            public const string MAPS_DIRECTORY = "Maps";
            public const string LOGS_DIRECTORY = "Logs";
            public const string CODE_DIRECTORY = "Code";
            public const string TUTORIALS_DIRECTORY = "Tutorials";
            public const string JSON_EXPORT_DIRECTORY = "JSON_Export";
            
            // Asset paths
            public const string ATLASES_PATH = "Graphics/Atlases";
            public const string PORTRAITS_PATH = "Graphics/Portraits";
            public const string SPRITES_PATH = "Graphics/Sprites";
        }
        
        /// <summary>
        /// Debug and logging constants
        /// </summary>
        public static class Debug
        {
            public const string LOG_TAG = "IngesteModule";
            public const string LOGGER_NAME = nameof(DesoloZantas);
            public const string LOG_FILE_PREFIX = "Ingeste";
            public const string LOG_FILE_EXTENSION = ".log";
            public const string LOG_TIMESTAMP_FORMAT = "yyyyMMdd_HHmmss";
            public const string LOG_TIME_FORMAT = "HH:mm:ss.fff";
        }
        
        /// <summary>
        /// Player state and ability constants
        /// </summary>
        public static class Player
        {
            public const int NEXT_FREE_STATE_ID = 34;
            
            // State machine prefixes
            public const string STATE_PREFIX = "DesoloZantas_";
            public const string ABILITY_PREFIX = "Kirby_";
            
            // Common ability names
            public const string FIRE_ABILITY = "Fire";
            public const string ICE_ABILITY = "Ice";
            public const string SPARK_ABILITY = "Spark";
            public const string STONE_ABILITY = "Stone";
        }
        
        /// <summary>
        /// Integration constants for cross-mod compatibility
        /// </summary>
        public static class Integration
        {
            public const string FROST_HELPER_MOD_NAME = "FrostHelper";
            public const string SKIN_MOD_HELPER_PLUS_NAME = "SkinModHelperPlus";
            public const string ADVENTURE_HELPER_NAME = "AdventureHelper";
            
            // Export names for ModInterop
            public const string INGESTE_EXPORTS_NAME = "DesoloZantasExports";
        }
        
        /// <summary>
        /// Console commands and function key bindings
        /// </summary>
        public static class Console
        {
            public const string EXPORT_MAPS_COMMAND = "export_maps";
            public const string EXPORT_SINGLE_MAP_COMMAND = "export_single_map";
            
            // Function key indices (0-based for Engine.Commands.FunctionKeyActions)
            public const int F1_KEY_INDEX = 0;
            public const int F2_KEY_INDEX = 1;
            public const int F3_KEY_INDEX = 2;
        }
        
        /// <summary>
        /// Session and save data flag prefixes
        /// </summary>
        public static class Flags
        {
            public const string INTERACT_TRIGGER_PREFIX = "it_";
            public const string DREAM_FRIEND_PREFIX = "df_";
            public const string ABILITY_UNLOCK_PREFIX = "ability_";
            public const string CHARACTER_UNLOCK_PREFIX = "char_";
        }
    }
}



