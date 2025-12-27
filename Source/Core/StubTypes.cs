// Stub types for removed dependencies
// These are minimal type definitions to maintain compilation after cleanup

using Microsoft.Xna.Framework;
using Monocle;
using Celeste;
using MonoMod.Utils;
using DesoloZantas.Core.Core.Player;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Stub enum for playable character identification
    /// </summary>
    public enum PlayableCharacterId
    {
        Madeline = 0,
        Kirby = 1,
        Badeline = 2,
        MetaKnight = 3,
        KingDedede = 4,
        BandanaWaddleDee = 5,
        Custom = 99
    }
    
    /// <summary>
    /// Stub class for player inventory tracking
    /// </summary>
    public class IngestePlayerInventory
    {
        public int Dashes { get; set; } = 1;
        public bool HasKey { get; set; } = false;
        public bool NoRefills { get; set; } = false;
        
        public static IngestePlayerInventory Instance { get; } = new IngestePlayerInventory();
        public static IngestePlayerInventory Default { get; } = new IngestePlayerInventory();
        public static IngestePlayerInventory Heart { get; } = new IngestePlayerInventory { NoRefills = true };
    }
    
    /// <summary>
    /// Stub for character skin definitions
    /// </summary>
    public class CharacterSkinDefinition
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string SpritePath { get; set; } = "";
        public string SkinId { get; set; } = "";
        public PlayableCharacterId CharacterId { get; set; } = PlayableCharacterId.Madeline;
        public Color[] HairColorOverrides { get; set; } = Array.Empty<Color>();
        public bool IsSelfSkin { get; set; } = false;
        
        public CharacterSkinDefinition() { }
        
        public CharacterSkinDefinition(string id, string name)
        {
            Id = id;
            Name = name;
            SkinId = id;
        }
    }
}

namespace DesoloZantas.Core.Core.Extensions
{
    /// <summary>
    /// Player extension methods for Kirby mode support using DynamicData.
    /// These replace the removed PlayerExtensions.cs from the Player folder.
    /// </summary>
    public static class PlayerExtensions
    {
        private const string DD_IS_KIRBY_MODE = "DesoloZantas_IsKirbyMode";
        private const string DD_KIRBY_COMPONENT = "DesoloZantas_KirbyComponent";
        private const string DD_KIRBY_POWER_STATE = "DesoloZantas_KirbyPowerState";
        private const string DD_KIRBY_CANDY_INVINCIBLE = "DesoloZantas_KirbyCandyInvincible";
        private const string DD_KIRBY_CANDY_TIMER = "DesoloZantas_KirbyCandyTimer";
        private const string DD_KIRBY_INHALE_TIMER = "DesoloZantas_KirbyInhaleTimer";
        private const string DD_KIRBY_INHALING = "DesoloZantas_KirbyInhaling";
        private const string DD_KIRBY_DASH_TIMER = "DesoloZantas_KirbyDashTimer";
        private const string DD_KIRBY_DASH_DIR = "DesoloZantas_KirbyDashDirection";
        private const string DD_KIRBY_DASHING = "DesoloZantas_KirbyDashing";
        private const string DD_PLAYABLE_CHAR_ID = "DesoloZantas_PlayableCharacterId";
        private const string DD_CHAR_SKIN_ID = "DesoloZantas_CharacterSkinId";

        public static bool IsKirbyMode(this global::Celeste.Player player)
        {
            if (player == null) return false;
            return DynamicData.For(player).Get<bool>(DD_IS_KIRBY_MODE);
        }

        public static void EnableKirbyMode(this global::Celeste.Player player)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_IS_KIRBY_MODE, true);
        }

        public static void DisableKirbyMode(this global::Celeste.Player player)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_IS_KIRBY_MODE, false);
        }

        public static KirbyPlayerComponent GetKirbyComponent(this global::Celeste.Player player)
        {
            if (player == null) return null;
            return DynamicData.For(player).Get<KirbyPlayerComponent>(DD_KIRBY_COMPONENT);
        }

        public static void SetKirbyComponent(this global::Celeste.Player player, KirbyPlayerComponent component)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_COMPONENT, component);
        }

        public static int GetKirbyPowerState(this global::Celeste.Player player)
        {
            if (player == null) return 0;
            return DynamicData.For(player).Get<int>(DD_KIRBY_POWER_STATE);
        }

        public static void SetKirbyPowerState(this global::Celeste.Player player, int state)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_POWER_STATE, state);
        }

        public static bool IsKirbyCandyInvincible(this global::Celeste.Player player)
        {
            if (player == null) return false;
            return DynamicData.For(player).Get<bool>(DD_KIRBY_CANDY_INVINCIBLE);
        }

        public static void SetKirbyCandyInvincible(this global::Celeste.Player player, bool value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_CANDY_INVINCIBLE, value);
        }

        public static float GetKirbyCandyTimer(this global::Celeste.Player player)
        {
            if (player == null) return 0f;
            return DynamicData.For(player).Get<float>(DD_KIRBY_CANDY_TIMER);
        }

        public static void SetKirbyCandyTimer(this global::Celeste.Player player, float value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_CANDY_TIMER, value);
        }

        public static float GetKirbyInhaleTimer(this global::Celeste.Player player)
        {
            if (player == null) return 0f;
            return DynamicData.For(player).Get<float>(DD_KIRBY_INHALE_TIMER);
        }

        public static void SetKirbyInhaleTimer(this global::Celeste.Player player, float value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_INHALE_TIMER, value);
        }

        public static bool GetKirbyInhaling(this global::Celeste.Player player)
        {
            if (player == null) return false;
            return DynamicData.For(player).Get<bool>(DD_KIRBY_INHALING);
        }

        public static void SetKirbyInhaling(this global::Celeste.Player player, bool value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_INHALING, value);
        }

        public static float GetKirbyDashTimer(this global::Celeste.Player player)
        {
            if (player == null) return 0f;
            return DynamicData.For(player).Get<float>(DD_KIRBY_DASH_TIMER);
        }

        public static void SetKirbyDashTimer(this global::Celeste.Player player, float value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_DASH_TIMER, value);
        }

        public static Vector2 GetKirbyDashDirection(this global::Celeste.Player player)
        {
            if (player == null) return Vector2.Zero;
            return DynamicData.For(player).Get<Vector2>(DD_KIRBY_DASH_DIR);
        }

        public static void SetKirbyDashDirection(this global::Celeste.Player player, Vector2 dir)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_DASH_DIR, dir);
        }

        public static bool GetKirbyDashing(this global::Celeste.Player player)
        {
            if (player == null) return false;
            return DynamicData.For(player).Get<bool>(DD_KIRBY_DASHING);
        }

        public static void SetKirbyDashing(this global::Celeste.Player player, bool value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_DASHING, value);
        }

        public static PlayableCharacterId GetPlayableCharacterId(this global::Celeste.Player player)
        {
            if (player == null) return PlayableCharacterId.Madeline;
            return DynamicData.For(player).Get<PlayableCharacterId>(DD_PLAYABLE_CHAR_ID);
        }

        public static void SetPlayableCharacterId(this global::Celeste.Player player, PlayableCharacterId id)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_PLAYABLE_CHAR_ID, id);
        }

        public static string GetCharacterSkinId(this global::Celeste.Player player)
        {
            if (player == null) return "";
            return DynamicData.For(player).Get<string>(DD_CHAR_SKIN_ID) ?? "";
        }

        public static void SetCharacterSkinId(this global::Celeste.Player player, string skinId)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_CHAR_SKIN_ID, skinId);
        }
        
        // Additional Kirby extension methods for attack/parry
        private const string DD_KIRBY_ATTACK_TIMER = "DesoloZantas_KirbyAttackTimer";
        private const string DD_KIRBY_ATTACK_COMBO = "DesoloZantas_KirbyAttackCombo";
        private const string DD_KIRBY_PARRY_TIMER = "DesoloZantas_KirbyParryTimer";
        private const string DD_KIRBY_PARRYING = "DesoloZantas_KirbyParrying";
        private const string DD_KIRBY_PARRY_SUCCESSFUL = "DesoloZantas_KirbyParrySuccessful";
        
        public static float GetKirbyAttackTimer(this global::Celeste.Player player)
        {
            if (player == null) return 0f;
            return DynamicData.For(player).Get<float>(DD_KIRBY_ATTACK_TIMER);
        }

        public static void SetKirbyAttackTimer(this global::Celeste.Player player, float value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_ATTACK_TIMER, value);
        }
        
        public static int GetKirbyAttackCombo(this global::Celeste.Player player)
        {
            if (player == null) return 0;
            return DynamicData.For(player).Get<int>(DD_KIRBY_ATTACK_COMBO);
        }

        public static void SetKirbyAttackCombo(this global::Celeste.Player player, int value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_ATTACK_COMBO, value);
        }
        
        public static float GetKirbyParryTimer(this global::Celeste.Player player)
        {
            if (player == null) return 0f;
            return DynamicData.For(player).Get<float>(DD_KIRBY_PARRY_TIMER);
        }

        public static void SetKirbyParryTimer(this global::Celeste.Player player, float value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_PARRY_TIMER, value);
        }
        
        public static bool GetKirbyParrying(this global::Celeste.Player player)
        {
            if (player == null) return false;
            return DynamicData.For(player).Get<bool>(DD_KIRBY_PARRYING);
        }

        public static void SetKirbyParrying(this global::Celeste.Player player, bool value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_PARRYING, value);
        }
        
        public static bool GetKirbyParrySuccessful(this global::Celeste.Player player)
        {
            if (player == null) return false;
            return DynamicData.For(player).Get<bool>(DD_KIRBY_PARRY_SUCCESSFUL);
        }

        public static void SetKirbyParrySuccessful(this global::Celeste.Player player, bool value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_PARRY_SUCCESSFUL, value);
        }
        
        // Wall bounce extension methods
        private const string DD_KIRBY_WALL_BOUNCE_TIMER = "DesoloZantas_KirbyWallBounceTimer";
        
        public static float GetKirbyWallBounceTimer(this global::Celeste.Player player)
        {
            if (player == null) return 0f;
            return DynamicData.For(player).Get<float>(DD_KIRBY_WALL_BOUNCE_TIMER);
        }
        
        public static void SetKirbyWallBounceTimer(this global::Celeste.Player player, float value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_WALL_BOUNCE_TIMER, value);
        }
        
        // Wave dash extension methods
        private const string DD_KIRBY_WAVE_DASH_COUNT = "DesoloZantas_KirbyWaveDashCount";
        
        public static int GetKirbyWaveDashCount(this global::Celeste.Player player)
        {
            if (player == null) return 0;
            return DynamicData.For(player).Get<int>(DD_KIRBY_WAVE_DASH_COUNT);
        }
        
        public static void SetKirbyWaveDashCount(this global::Celeste.Player player, int value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_WAVE_DASH_COUNT, value);
        }
        
        // Charged dash extension methods
        private const string DD_KIRBY_CHARGE_TIME = "DesoloZantas_KirbyChargeTime";
        private const string DD_KIRBY_CHARGED_DASH_DIR = "DesoloZantas_KirbyChargedDashDir";
        
        public static float GetKirbyChargeTime(this global::Celeste.Player player)
        {
            if (player == null) return 0f;
            return DynamicData.For(player).Get<float>(DD_KIRBY_CHARGE_TIME);
        }
        
        public static void SetKirbyChargeTime(this global::Celeste.Player player, float value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_CHARGE_TIME, value);
        }
        
        public static Vector2 GetKirbyChargedDashDirection(this global::Celeste.Player player)
        {
            if (player == null) return Vector2.Zero;
            return DynamicData.For(player).Get<Vector2>(DD_KIRBY_CHARGED_DASH_DIR);
        }
        
        public static void SetKirbyChargedDashDirection(this global::Celeste.Player player, Vector2 dir)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_CHARGED_DASH_DIR, dir);
        }
        
        // Grab slam extension methods
        private const string DD_KIRBY_GRAB_SLAM_TIMER = "DesoloZantas_KirbyGrabSlamTimer";
        private const string DD_KIRBY_GRABBED_ENTITY = "DesoloZantas_KirbyGrabbedEntity";
        
        public static float GetKirbyGrabSlamTimer(this global::Celeste.Player player)
        {
            if (player == null) return 0f;
            return DynamicData.For(player).Get<float>(DD_KIRBY_GRAB_SLAM_TIMER);
        }
        
        public static void SetKirbyGrabSlamTimer(this global::Celeste.Player player, float value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_GRAB_SLAM_TIMER, value);
        }
        
        public static Entity GetKirbyGrabbedEntity(this global::Celeste.Player player)
        {
            if (player == null) return null;
            return DynamicData.For(player).Get<Entity>(DD_KIRBY_GRABBED_ENTITY);
        }
        
        public static void SetKirbyGrabbedEntity(this global::Celeste.Player player, Entity entity)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_GRABBED_ENTITY, entity);
        }
        
        // Star power charge extension methods
        private const string DD_KIRBY_STAR_POWER_CHARGE = "DesoloZantas_KirbyStarPowerCharge";
        
        public static float GetKirbyStarPowerCharge(this global::Celeste.Player player)
        {
            if (player == null) return 0f;
            return DynamicData.For(player).Get<float>(DD_KIRBY_STAR_POWER_CHARGE);
        }
        
        public static void SetKirbyStarPowerCharge(this global::Celeste.Player player, float value)
        {
            if (player == null) return;
            DynamicData.For(player).Set(DD_KIRBY_STAR_POWER_CHARGE, value);
        }
        
        // Initialize kirby data
        public static void InitializeKirbyData(this global::Celeste.Player player)
        {
            if (player == null) return;
            player.SetKirbyPowerState(0);
            player.SetKirbyCandyInvincible(false);
            player.SetKirbyCandyTimer(0f);
            player.SetKirbyInhaleTimer(0f);
            player.SetKirbyInhaling(false);
            player.SetKirbyDashTimer(0f);
            player.SetKirbyDashing(false);
            player.SetKirbyAttackTimer(0f);
            player.SetKirbyAttackCombo(0);
            player.SetKirbyParryTimer(0f);
            player.SetKirbyParrying(false);
            player.SetKirbyParrySuccessful(false);
            player.SetKirbyWallBounceTimer(0f);
            player.SetKirbyWaveDashCount(0);
            player.SetKirbyChargeTime(0f);
            player.SetKirbyGrabSlamTimer(0f);
            player.SetKirbyStarPowerCharge(0f);
        }
    }
}

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Stub enum for intro types used by save points
    /// </summary>
    public enum IntroTypes
    {
        None = 0,
        Respawn = 1,
        WalkInRight = 2,
        WalkInLeft = 3,
        Jump = 4,
        WakeUp = 5,
        Fall = 6,
        TempleMirrorVoid = 7,
        ThinkForABit = 8
    }
    
    /// <summary>
    /// Stub constants class with nested configuration classes
    /// </summary>
    public static class IngesteConstants
    {
        public const string ModName = "DesoloZantas";
        public const string ModVersion = "1.0.0";
        public const float DefaultDashSpeed = 240f;
        public const float DefaultJumpSpeed = -105f;
        
        public static class EntityNames
        {
            public const string AncientSwitch = "Ingeste/AncientSwitch";
            public const string SampleTrigger = "Ingeste/SampleTrigger";
            public const string ANCIENT_SWITCH = "Ingeste/AncientSwitch";
            public const string SAMPLE_TRIGGER = "Ingeste/SampleTrigger";
        }
        
        public static class Paths
        {
            public const string LOGS_DIRECTORY = "Logs";
            public const string JSON_EXPORT_DIRECTORY = "Exports";
        }
        
        public static class Debug
        {
            public const string LOG_TIMESTAMP_FORMAT = "yyyy-MM-dd_HH-mm-ss";
            public const string LOG_TIME_FORMAT = "HH:mm:ss.fff";
            public const string LOG_FILE_PREFIX = "ingeste";
            public const string LOG_FILE_EXTENSION = ".log";
            public const string LOGGER_NAME = "DesoloZantas";
        }
        
        public static class Console
        {
            public const int F1_KEY_INDEX = 0;
        }
    }
    
    /// <summary>
    /// Stub state machine wrapper
    /// </summary>
    public class IngesteStateMachine : Monocle.Component
    {
        public enum Facings { Left = -1, Right = 1 }
        
        public IngesteStateMachine() : base(true, false) { }
        
        public int State { get; set; } = 0;
        
        public void SetCallbacks(int state, Func<int> onUpdate, Func<IEnumerator> coroutine = null, Action begin = null, Action end = null)
        {
            // Stub implementation
        }
    }
    
    /// <summary>
    /// Stub audio helper - wraps Celeste.Audio for safe playback
    /// </summary>
    public static class AudioHelper
    {
        /// <summary>
        /// Safely play an audio event, with fallback to default if primary fails
        /// </summary>
        public static void PlaySafe(string primaryEvent, string fallbackEvent = null)
        {
            try
            {
                Audio.Play(primaryEvent);
            }
            catch
            {
                if (!string.IsNullOrEmpty(fallbackEvent))
                {
                    try { Audio.Play(fallbackEvent); } catch { }
                }
            }
        }
        
        public static void Play(string eventName) => Audio.Play(eventName);
        public static void Stop(string eventName) { /* Stub */ }
        
        public static bool SetMusicSafe(string eventName, bool startPlaying = true, bool allowFadeOut = false)
        {
            try
            {
                Audio.SetMusic(eventName, startPlaying);
                return true;
            }
            catch { return false; }
        }
    }
    
    /// <summary>
    /// Stub texture utility
    /// </summary>
    public static class TextureUtil
    {
        public static MTexture GetOrCreate(string path)
        {
            return GFX.Game.GetAtlasSubtexturesAt(path, 0) ?? GFX.Game["characters/player/idle00"];
        }
        
        public static MTexture[] GetAtlasSubtextures(string path)
        {
            return GFX.Game.GetAtlasSubtextures(path).ToArray();
        }
        
        public static MTexture TryGetOverworldTexture(string path)
        {
            try
            {
                return GFX.Gui[path];
            }
            catch
            {
                return null;
            }
        }
    }
    
    /// <summary>
    /// Stub IngesteConfig for cutscene configuration
    /// </summary>
    public static class IngesteConfig
    {
        public static string MusicEvent { get; set; } = "";
        public static string AmbienceEvent { get; set; } = "";
        public static float FadeTime { get; set; } = 1.0f;
        public const float FLOAT_SPEED = 50f;
    }
    
    /// <summary>
    /// Stub companion character manager
    /// </summary>
    public static class CompanionCharacterManager
    {
        public static void SpawnCompanion(Level level, string companionType, Vector2 position)
        {
            // Stub - companion spawning disabled
        }
        
        public static void SpawnCompanion(Level level, PlayerSpriteMode mode, Vector2 position)
        {
            // Stub - companion spawning disabled
        }
    }
    
    /// <summary>
    /// Stub Lua cutscene manager
    /// </summary>
    public static class LuaCutsceneManager
    {
        public static bool IsInitialized { get; set; } = false;
        public static bool HasCutscene(string name) => false;
        public static string[] GetCutsceneNames() => Array.Empty<string>();
        public static object LoadCutscene(string name) => null;
        public static void RegisterCutscene(string name, object cutscene) { }
        public static void Initialize() { IsInitialized = true; }
        public static object[] CallLuaFunction(string funcName, params object[] args) => Array.Empty<object>();
        
        /// <summary>
        /// Call a Lua function and return a single result as string
        /// </summary>
        public static string CallLuaFunctionString(string funcName, params object[] args)
        {
            var result = CallLuaFunction(funcName, args);
            if (result != null && result.Length > 0)
                return result[0]?.ToString() ?? string.Empty;
            return string.Empty;
        }
    }
    
    /// <summary>
    /// Stub player inventory manager
    /// </summary>
    public static class PlayerInventoryManager
    {
        public static void SetDashes(int count) { }
        public static void SetStamina(float stamina) { }
        public static void SetMaxHealth(int health) { }
        public static void SetCurrentHealth(int health) { }
        public static void SetInventoryItem(string item, bool has) { }
        public static void SetInventoryValue(string key, object value) { }
        public static void RestoreDefaultInventory() { }
        public static void EnableHeartPower(Level level = null) { }
        public static void EnableKirbyPlayer(Level level = null) { }
        public static void EnableSayGoodbye(Level level = null) { }
        public static void EnableTitanTowerClimbing(Level level = null) { }
        public static void EnableCorruption(Level level = null) { }
        public static void EnableTheEnd(Level level = null) { }
        public static void ResetToDefault(Level level = null) { }
        public static int GetMaxDashesForInventory(IngestePlayerInventory inventory = default) => 1;
    }
    
    /// <summary>
    /// Stub DeltaBerry entity reference
    /// </summary>
    public class DeltaBerry : Entity
    {
        public bool Collected { get; set; }
        public bool Golden { get; set; }
        
        public DeltaBerry(Vector2 position) : base(position) { }
        public DeltaBerry(EntityData data, Vector2 offset) : base(data.Position + offset) { }
        
        /// <summary>
        /// Trigger the collect behavior
        /// </summary>
        public void OnCollect()
        {
            Collected = true;
        }
        
        /// <summary>
        /// Trigger the collect behavior with player reference
        /// </summary>
        public void OnCollect(global::Celeste.Player player)
        {
            Collected = true;
        }
    }
    
    /// <summary>
    /// Stub WarperDashSimple class
    /// </summary>
    public static class WarperDashSimple
    {
        public static void ApplyWarp(global::Celeste.Player player) { }
        public static void EndWarp(global::Celeste.Player player) { }
        public static void PerformWarperDash(Vector2 position, Vector2 direction, Scene scene) { }
    }
    
    /// <summary>
    /// Stub external area data manager
    /// </summary>
    public static class ExternalAreaDataManager
    {
        public static void LoadExternalAreas()
        {
            // Stub - external areas loading disabled
        }
        
        public static void UnloadExternalAreas()
        {
            // Stub
        }
    }
    
    /// <summary>
    /// Stub cassette player system
    /// </summary>
    public static class CassettePlayerSystem
    {
        public static void RegisterHooks()
        {
            // Stub - cassette player hooks disabled
        }
        
        public static void UnregisterHooks()
        {
            // Stub
        }
        
        public static void Unload()
        {
            // Stub
        }
    }
    
    /// <summary>
    /// Stub metadata manager
    /// </summary>
    public static class MetadataManager
    {
        public static void Initialize(string modRoot)
        {
            // Stub - metadata manager disabled
        }
    }
    
    /// <summary>
    /// Stub fixed map data processor
    /// </summary>
    public class FixedMapDataProcessor
    {
        public void Process(MapData mapData)
        {
            // Stub - map data processing disabled
        }
    }
    
    /// <summary>
    /// Stub game over screen - redirects to Celeste's default
    /// </summary>
    public class GameOverScreen : Scene
    {
        private Level level;
        private Session session;
        
        public GameOverScreen(Level level, Session session)
        {
            this.level = level;
            this.session = session;
        }
        
        public override void Begin()
        {
            base.Begin();
            // Just restart the level
            Engine.Scene = new LevelLoader(session);
        }
    }
    
    /// <summary>
    /// Stub mountain manager namespace
    /// </summary>
    public static class Mountain
    {
        public static class DesoloMountainManager
        {
            public static void Initialize()
            {
                // Stub - mountain manager disabled
            }
            
            public static void Unload()
            {
                // Stub
            }
        }
    }
}

namespace DesoloZantas.Core.Core.Systems
{
    /// <summary>
    /// Stub enum for elemental types
    /// </summary>
    public enum ElementalType
    {
        None = 0,
        Fire = 1,
        Ice = 2,
        Electric = 3,
        Water = 4,
        Wind = 5,
        Earth = 6,
        Light = 7,
        Dark = 8,
        Splash = 9,
        Blizzard = 10,
        Sizzle = 11,
        Bluster = 12
    }
    
    /// <summary>
    /// Stub class for elemental abilities
    /// </summary>
    public class ElementalAbility
    {
        public ElementalType Type { get; set; } = ElementalType.None;
        public float Damage { get; set; } = 1f;
        public float Duration { get; set; } = 1f;
        
        protected float elementPower = 1f;
        protected float elementDuration = 1f;
        protected bool isActive = false;
        
        public virtual Color ElementColor => GetColorForType(Type);
        public virtual ParticleType ElementParticles => ParticleTypes.Dust;
        
        public ElementalAbility(ElementalType type)
        {
            Type = type;
        }
        
        private static Color GetColorForType(ElementalType type)
        {
            return type switch
            {
                ElementalType.Fire => Color.OrangeRed,
                ElementalType.Ice => Color.LightBlue,
                ElementalType.Electric => Color.Yellow,
                ElementalType.Water => Color.Blue,
                ElementalType.Wind => Color.LightGreen,
                ElementalType.Earth => Color.Brown,
                ElementalType.Light => Color.White,
                ElementalType.Dark => Color.Purple,
                ElementalType.Splash => Color.Cyan,
                ElementalType.Blizzard => Color.PowderBlue,
                ElementalType.Sizzle => Color.Orange,
                ElementalType.Bluster => Color.PaleGreen,
                _ => Color.White
            };
        }
        
        protected virtual void OnActivate(Vector2 position, Vector2 direction) { }
        protected virtual void OnDeactivate() { }
        protected virtual void OnUpdate(Vector2 position) { }
        
        public virtual void ApplyElementalEffect(Level level, Vector2 center, float radius, Entity source)
        {
            // Stub implementation
        }
    }
    
    /// <summary>
    /// Stub lives system
    /// </summary>
    public static class LivesSystem
    {
        public static int CurrentLives { get; set; } = 3;
        public static int MaxLives { get; set; } = 5;
    }
    
    /// <summary>
    /// Stub playable character system
    /// </summary>
    public static class PlayableCharacterSystem
    {
        public static PlayableCharacterId CurrentCharacter { get; set; } = PlayableCharacterId.Madeline;
        
        public static void Initialize()
        {
            // Stub - initialize playable character system
        }
        
        public static void SetCharacter(PlayableCharacterId id)
        {
            CurrentCharacter = id;
        }
        
        public static void SetCurrentCharacter(PlayableCharacterId id)
        {
            CurrentCharacter = id;
        }
        
        public static void ApplyToPlayer(global::Celeste.Player player)
        {
            // Stub - apply character settings to player
        }
    }
}

namespace DesoloZantas.Core.Core.Settings
{
    /// <summary>
    /// Stub Kirby settings class - replaced by IngesteModuleSettings
    /// </summary>
    public class KirbySettings
    {
        public bool KirbyModeEnabled { get; set; } = true;
        public Microsoft.Xna.Framework.Input.Keys InhaleKey { get; set; } = Microsoft.Xna.Framework.Input.Keys.H;
        public Microsoft.Xna.Framework.Input.Keys HoverKey { get; set; } = Microsoft.Xna.Framework.Input.Keys.C;
        public Microsoft.Xna.Framework.Input.Keys AttackKey { get; set; } = Microsoft.Xna.Framework.Input.Keys.S;
        public float HoverDuration { get; set; } = 3f;
        public int InhaleRange { get; set; } = 64;
        public int CombatDamage { get; set; } = 50;
        public float ParryWindow { get; set; } = 0.3f;
        
        // Additional properties required by KirbyStateExtensions
        public float MovementPrecision { get; set; } = 1.0f;
        public bool StarWarriorMode { get; set; } = false;
        
        public bool IsKeyPressed(string action) => false;
        public bool IsKeyCheck(string action) => false;
        public bool IsKeyReleased(string action) => false;
    }
}
