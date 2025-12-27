using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Identifiers for all playable characters in the mod
    /// </summary>
    public enum PlayableCharacterId
    {
        /// <summary>Default Celeste Madeline</summary>
        Madeline = 0,
        
        /// <summary>Kirby - main alternative character</summary>
        Kirby = 1,
        
        /// <summary>Badeline variant</summary>
        Badeline = 2,
        
        /// <summary>Meta Knight - Kirby character</summary>
        MetaKnight = 3,
        
        /// <summary>King Dedede - Kirby character</summary>
        KingDedede = 4,
        
        /// <summary>Bandana Waddle Dee - Kirby character</summary>
        BandanaWaddleDee = 5,
        
        /// <summary>Custom character slot 1</summary>
        Custom1 = 10,
        
        /// <summary>Custom character slot 2</summary>
        Custom2 = 11,
        
        /// <summary>Custom character slot 3</summary>
        Custom3 = 12,
        
        /// <summary>Self/Player skin (uses player's configured skin)</summary>
        CharacterSelf = 99
    }
    
    /// <summary>
    /// Definition of a playable character including stats, abilities, and visual configuration
    /// </summary>
    public class PlayableCharacterDefinition
    {
        #region Properties
        
        /// <summary>Character identifier</summary>
        public PlayableCharacterId Id { get; set; }
        
        /// <summary>Display name of the character</summary>
        public string DisplayName { get; set; }
        
        /// <summary>Character description</summary>
        public string Description { get; set; }
        
        /// <summary>Base statistics for this character</summary>
        public KirbyCharacterStats BaseStats { get; set; }
        
        /// <summary>Sprite bank path for this character</summary>
        public string SpriteBankPath { get; set; }
        
        /// <summary>Default sprite mode</summary>
        public string DefaultSpriteMode { get; set; }
        
        /// <summary>Hair color configuration</summary>
        public Color[] HairColors { get; set; }
        
        /// <summary>Trail color for dashing</summary>
        public Color TrailColor { get; set; }
        
        /// <summary>Whether this character can hover (Kirby-style)</summary>
        public bool CanHover { get; set; }
        
        /// <summary>Whether this character can inhale (Kirby-style)</summary>
        public bool CanInhale { get; set; }
        
        /// <summary>Whether this character uses Kirby copy abilities</summary>
        public bool UsesCopyAbilities { get; set; }
        
        /// <summary>List of available skins for this character</summary>
        public List<CharacterSkinDefinition> AvailableSkins { get; set; }
        
        /// <summary>Default dash tier for this character</summary>
        public KirbyDashExtensions.DashTier DefaultDashTier { get; set; }
        
        /// <summary>Special ability unique to this character</summary>
        public string SpecialAbilityId { get; set; }
        
        #endregion
        
        #region Constructors
        
        public PlayableCharacterDefinition()
        {
            BaseStats = new KirbyCharacterStats();
            HairColors = new Color[] { Calc.HexToColor("AC3232") };
            TrailColor = Color.White;
            AvailableSkins = new List<CharacterSkinDefinition>();
            DefaultDashTier = KirbyDashExtensions.DashTier.Standard;
        }
        
        public PlayableCharacterDefinition(PlayableCharacterId id, string name, string description) : this()
        {
            Id = id;
            DisplayName = name;
            Description = description;
        }
        
        #endregion
        
        #region Methods
        
        /// <summary>
        /// Create a copy of the base stats for a new instance
        /// </summary>
        public KirbyCharacterStats CreateStatsInstance()
        {
            return new KirbyCharacterStats(BaseStats);
        }
        
        /// <summary>
        /// Get default skin for this character
        /// </summary>
        public CharacterSkinDefinition GetDefaultSkin()
        {
            return AvailableSkins.Count > 0 ? AvailableSkins[0] : null;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Definition of a character skin
    /// </summary>
    public class CharacterSkinDefinition
    {
        /// <summary>Skin identifier</summary>
        public string SkinId { get; set; }
        
        /// <summary>Display name of the skin</summary>
        public string DisplayName { get; set; }
        
        /// <summary>Sprite path override</summary>
        public string SpritePath { get; set; }
        
        /// <summary>Hair color overrides</summary>
        public Color[] HairColorOverrides { get; set; }
        
        /// <summary>Trail color override</summary>
        public Color? TrailColorOverride { get; set; }
        
        /// <summary>Whether this is a "self" skin (uses player configuration)</summary>
        public bool IsSelfSkin { get; set; }
        
        /// <summary>Color grade effect for this skin</summary>
        public string ColorGradeEffect { get; set; }
        
        public CharacterSkinDefinition()
        {
            SkinId = "default";
            DisplayName = "Default";
        }
        
        public CharacterSkinDefinition(string id, string name, string spritePath = null)
        {
            SkinId = id;
            DisplayName = name;
            SpritePath = spritePath;
        }
    }
    
    /// <summary>
    /// Manages playable characters and their configurations
    /// </summary>
    public static class PlayableCharacterSystem
    {
        #region Fields
        
        private static Dictionary<PlayableCharacterId, PlayableCharacterDefinition> _characters;
        private static bool _initialized = false;
        private static PlayableCharacterId _currentCharacter = PlayableCharacterId.Madeline;
        private static string _currentSkinId = "default";
        
        #endregion
        
        #region Properties
        
        /// <summary>Current active character ID</summary>
        public static PlayableCharacterId CurrentCharacterId => _currentCharacter;
        
        /// <summary>Current active skin ID</summary>
        public static string CurrentSkinId => _currentSkinId;
        
        /// <summary>Get current character definition</summary>
        public static PlayableCharacterDefinition CurrentCharacter => GetCharacter(_currentCharacter);
        
        #endregion
        
        #region Initialization
        
        /// <summary>
        /// Initialize the playable character system
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;
            
            _characters = new Dictionary<PlayableCharacterId, PlayableCharacterDefinition>();
            RegisterBuiltInCharacters();
            _initialized = true;
            
            IngesteLogger.Info("PlayableCharacterSystem initialized");
        }
        
        /// <summary>
        /// Register all built-in characters
        /// </summary>
        private static void RegisterBuiltInCharacters()
        {
            // Madeline (default)
            RegisterCharacter(new PlayableCharacterDefinition
            {
                Id = PlayableCharacterId.Madeline,
                DisplayName = "Madeline",
                Description = "The determined mountain climber from Celeste.",
                BaseStats = new KirbyCharacterStats(100, 1.0f, 1.0f, 1.0f, 1),
                SpriteBankPath = "player",
                DefaultSpriteMode = "madeline",
                HairColors = new Color[] 
                { 
                    Calc.HexToColor("AC3232"), // 1 dash
                    Calc.HexToColor("FF6DEF")  // 2+ dashes
                },
                TrailColor = Calc.HexToColor("AC3232"),
                CanHover = false,
                CanInhale = false,
                UsesCopyAbilities = false,
                DefaultDashTier = KirbyDashExtensions.DashTier.Standard,
                AvailableSkins = new List<CharacterSkinDefinition>
                {
                    new CharacterSkinDefinition("default", "Default"),
                    new CharacterSkinDefinition("badeline", "Badeline", "badeline"),
                    new CharacterSkinDefinition("self", "Self") { IsSelfSkin = true }
                }
            });
            
            // Kirby
            RegisterCharacter(new PlayableCharacterDefinition
            {
                Id = PlayableCharacterId.Kirby,
                DisplayName = "Kirby",
                Description = "The pink puffball hero from Dream Land. Can hover and inhale enemies!",
                BaseStats = new KirbyCharacterStats(80, 0.8f, 1.2f, 1.3f, 3),
                SpriteBankPath = "kirby_player",
                DefaultSpriteMode = "kirby",
                HairColors = new Color[] 
                { 
                    Calc.HexToColor("FFB6C1"), // Pink
                    Calc.HexToColor("FFD700")  // Gold (star power)
                },
                TrailColor = Calc.HexToColor("FFB6C1"),
                CanHover = true,
                CanInhale = true,
                UsesCopyAbilities = true,
                DefaultDashTier = KirbyDashExtensions.DashTier.Triple,
                SpecialAbilityId = "kirby_inhale",
                AvailableSkins = new List<CharacterSkinDefinition>
                {
                    new CharacterSkinDefinition("default", "Pink Kirby"),
                    new CharacterSkinDefinition("yellow", "Yellow Kirby", "kirby_yellow"),
                    new CharacterSkinDefinition("blue", "Blue Kirby", "kirby_blue"),
                    new CharacterSkinDefinition("green", "Green Kirby", "kirby_green"),
                    new CharacterSkinDefinition("red", "Red Kirby", "kirby_red"),
                    new CharacterSkinDefinition("white", "White Kirby", "kirby_white"),
                    new CharacterSkinDefinition("shadow", "Shadow Kirby", "kirby_shadow"),
                    new CharacterSkinDefinition("self", "Self") { IsSelfSkin = true }
                }
            });
            
            // Badeline
            RegisterCharacter(new PlayableCharacterDefinition
            {
                Id = PlayableCharacterId.Badeline,
                DisplayName = "Badeline",
                Description = "Madeline's other self. Faster but more fragile.",
                BaseStats = new KirbyCharacterStats(70, 0.7f, 1.4f, 1.5f, 2),
                SpriteBankPath = "badeline",
                DefaultSpriteMode = "badeline",
                HairColors = new Color[] 
                { 
                    Calc.HexToColor("9B30FF"), // Purple
                    Calc.HexToColor("FF00FF")  // Magenta
                },
                TrailColor = Calc.HexToColor("9B30FF"),
                CanHover = false,
                CanInhale = false,
                UsesCopyAbilities = false,
                DefaultDashTier = KirbyDashExtensions.DashTier.Double,
                AvailableSkins = new List<CharacterSkinDefinition>
                {
                    new CharacterSkinDefinition("default", "Default"),
                    new CharacterSkinDefinition("corrupted", "Corrupted", "badeline_corrupted"),
                    new CharacterSkinDefinition("self", "Self") { IsSelfSkin = true }
                }
            });
            
            // Meta Knight
            RegisterCharacter(new PlayableCharacterDefinition
            {
                Id = PlayableCharacterId.MetaKnight,
                DisplayName = "Meta Knight",
                Description = "The masked swordsman. High attack, balanced defense.",
                BaseStats = new KirbyCharacterStats(90, 1.0f, 1.5f, 1.4f, 4),
                SpriteBankPath = "meta_knight",
                DefaultSpriteMode = "meta_knight",
                HairColors = new Color[] 
                { 
                    Calc.HexToColor("000080"), // Navy
                    Calc.HexToColor("FFD700")  // Gold
                },
                TrailColor = Calc.HexToColor("000080"),
                CanHover = true,
                CanInhale = false,
                UsesCopyAbilities = false,
                DefaultDashTier = KirbyDashExtensions.DashTier.Quad,
                SpecialAbilityId = "sword_beam",
                AvailableSkins = new List<CharacterSkinDefinition>
                {
                    new CharacterSkinDefinition("default", "Default"),
                    new CharacterSkinDefinition("galacta", "Galacta Knight", "galacta_knight"),
                    new CharacterSkinDefinition("dark", "Dark Meta Knight", "dark_meta_knight"),
                    new CharacterSkinDefinition("self", "Self") { IsSelfSkin = true }
                }
            });
            
            // King Dedede
            RegisterCharacter(new PlayableCharacterDefinition
            {
                Id = PlayableCharacterId.KingDedede,
                DisplayName = "King Dedede",
                Description = "The self-proclaimed king. High defense and attack, slower speed.",
                BaseStats = new KirbyCharacterStats(150, 1.5f, 1.3f, 0.8f, 2),
                SpriteBankPath = "king_dedede",
                DefaultSpriteMode = "dedede",
                HairColors = new Color[] 
                { 
                    Calc.HexToColor("0000CD"), // Medium Blue
                    Calc.HexToColor("FFD700")  // Gold crown
                },
                TrailColor = Calc.HexToColor("0000CD"),
                CanHover = true,
                CanInhale = true,
                UsesCopyAbilities = false,
                DefaultDashTier = KirbyDashExtensions.DashTier.Double,
                SpecialAbilityId = "hammer_slam",
                AvailableSkins = new List<CharacterSkinDefinition>
                {
                    new CharacterSkinDefinition("default", "Default"),
                    new CharacterSkinDefinition("masked", "Masked Dedede", "dedede_masked"),
                    new CharacterSkinDefinition("self", "Self") { IsSelfSkin = true }
                }
            });
            
            // Bandana Waddle Dee
            RegisterCharacter(new PlayableCharacterDefinition
            {
                Id = PlayableCharacterId.BandanaWaddleDee,
                DisplayName = "Bandana Waddle Dee",
                Description = "Kirby's loyal friend. Balanced stats with spear abilities.",
                BaseStats = new KirbyCharacterStats(85, 0.9f, 1.1f, 1.2f, 3),
                SpriteBankPath = "bandana_dee",
                DefaultSpriteMode = "bandana_dee",
                HairColors = new Color[] 
                { 
                    Calc.HexToColor("FFA500"), // Orange
                    Calc.HexToColor("0000FF")  // Blue bandana
                },
                TrailColor = Calc.HexToColor("FFA500"),
                CanHover = false,
                CanInhale = false,
                UsesCopyAbilities = false,
                DefaultDashTier = KirbyDashExtensions.DashTier.Triple,
                SpecialAbilityId = "spear_thrust",
                AvailableSkins = new List<CharacterSkinDefinition>
                {
                    new CharacterSkinDefinition("default", "Default"),
                    new CharacterSkinDefinition("sailor", "Sailor Dee", "sailor_dee"),
                    new CharacterSkinDefinition("self", "Self") { IsSelfSkin = true }
                }
            });
            
            // Character Self (uses player's configured skin)
            RegisterCharacter(new PlayableCharacterDefinition
            {
                Id = PlayableCharacterId.CharacterSelf,
                DisplayName = "Self",
                Description = "Uses your configured player skin and settings.",
                BaseStats = new KirbyCharacterStats(100, 1.0f, 1.0f, 1.0f, 1),
                SpriteBankPath = "player",
                DefaultSpriteMode = "madeline",
                HairColors = new Color[] 
                { 
                    Calc.HexToColor("AC3232"),
                    Calc.HexToColor("FF6DEF")
                },
                TrailColor = Calc.HexToColor("AC3232"),
                CanHover = false,
                CanInhale = false,
                UsesCopyAbilities = false,
                DefaultDashTier = KirbyDashExtensions.DashTier.Standard,
                AvailableSkins = new List<CharacterSkinDefinition>
                {
                    new CharacterSkinDefinition("self", "Self") { IsSelfSkin = true }
                }
            });
        }
        
        #endregion
        
        #region Character Management
        
        /// <summary>
        /// Register a new character definition
        /// </summary>
        public static void RegisterCharacter(PlayableCharacterDefinition character)
        {
            if (character == null) return;
            
            if (!_initialized)
            {
                _characters = new Dictionary<PlayableCharacterId, PlayableCharacterDefinition>();
                _initialized = true;
            }
            
            _characters[character.Id] = character;
            IngesteLogger.Info($"Registered character: {character.DisplayName} ({character.Id})");
        }
        
        /// <summary>
        /// Get a character definition by ID
        /// </summary>
        public static PlayableCharacterDefinition GetCharacter(PlayableCharacterId id)
        {
            if (!_initialized) Initialize();
            
            if (_characters.TryGetValue(id, out var character))
                return character;
            
            return _characters[PlayableCharacterId.Madeline]; // Default fallback
        }
        
        /// <summary>
        /// Get all registered characters
        /// </summary>
        public static IEnumerable<PlayableCharacterDefinition> GetAllCharacters()
        {
            if (!_initialized) Initialize();
            return _characters.Values;
        }
        
        /// <summary>
        /// Set the current active character
        /// </summary>
        public static void SetCurrentCharacter(PlayableCharacterId id)
        {
            if (!_initialized) Initialize();
            
            if (_characters.ContainsKey(id))
            {
                _currentCharacter = id;
                _currentSkinId = "default";
                IngesteLogger.Info($"Current character set to: {id}");
            }
        }
        
        /// <summary>
        /// Set the current skin for the active character
        /// </summary>
        public static void SetCurrentSkin(string skinId)
        {
            var character = CurrentCharacter;
            if (character == null) return;
            
            var skin = character.AvailableSkins.Find(s => s.SkinId == skinId);
            if (skin != null)
            {
                _currentSkinId = skinId;
                IngesteLogger.Info($"Current skin set to: {skinId}");
            }
        }
        
        #endregion
        
        #region Player Integration
        
        /// <summary>
        /// Apply current character configuration to a player
        /// </summary>
        public static void ApplyToPlayer(global::Celeste.Player player)
        {
            if (player == null) return;
            if (!_initialized) Initialize();
            
            var character = CurrentCharacter;
            if (character == null) return;
            
            // Apply stats
            var stats = character.CreateStatsInstance();
            player.SetCharacterStats(stats);
            
            // Apply dash tier
            player.SetDashTier(character.DefaultDashTier);
            
            // Enable Kirby mode if applicable
            if (character.CanHover || character.CanInhale || character.UsesCopyAbilities)
            {
                player.EnableKirbyMode();
            }
            
            IngesteLogger.Info($"Applied character {character.DisplayName} to player");
        }
        
        /// <summary>
        /// Get the current character's skin sprite path
        /// </summary>
        public static string GetCurrentSpritePath()
        {
            var character = CurrentCharacter;
            if (character == null) return "player";
            
            var skin = character.AvailableSkins.Find(s => s.SkinId == _currentSkinId);
            if (skin?.SpritePath != null)
                return skin.SpritePath;
            
            return character.SpriteBankPath;
        }
        
        /// <summary>
        /// Get hair color for current character at specific dash count
        /// </summary>
        public static Color GetHairColor(int dashCount)
        {
            var character = CurrentCharacter;
            if (character?.HairColors == null || character.HairColors.Length == 0)
                return KirbyDashExtensions.GetDashTierHairColor(dashCount);
            
            // Check for skin override
            var skin = character.AvailableSkins.Find(s => s.SkinId == _currentSkinId);
            if (skin?.HairColorOverrides != null && skin.HairColorOverrides.Length > 0)
            {
                int index = Math.Min(dashCount, skin.HairColorOverrides.Length - 1);
                return skin.HairColorOverrides[Math.Max(0, index)];
            }
            
            int hairIndex = Math.Min(dashCount, character.HairColors.Length - 1);
            return character.HairColors[Math.Max(0, hairIndex)];
        }
        
        /// <summary>
        /// Get trail color for current character
        /// </summary>
        public static Color GetTrailColor()
        {
            var character = CurrentCharacter;
            if (character == null) return Color.White;
            
            // Check for skin override
            var skin = character.AvailableSkins.Find(s => s.SkinId == _currentSkinId);
            if (skin?.TrailColorOverride.HasValue == true)
                return skin.TrailColorOverride.Value;
            
            return character.TrailColor;
        }
        
        #endregion
    }
    
    /// <summary>
    /// Extension methods for player character management
    /// </summary>
    public static class PlayableCharacterExtensions
    {
        private const string CHARACTER_KEY = "kirby_playable_character_id";
        private const string SKIN_KEY = "kirby_character_skin_id";
        
        /// <summary>
        /// Get the character ID assigned to a player
        /// </summary>
        public static PlayableCharacterId GetPlayableCharacterId(this global::Celeste.Player player)
        {
            if (player == null) return PlayableCharacterId.Madeline;
            
            var data = DynamicData.For(player);
            return data.Get<PlayableCharacterId?>(CHARACTER_KEY) ?? PlayableCharacterId.Madeline;
        }
        
        /// <summary>
        /// Set the character ID for a player
        /// </summary>
        public static void SetPlayableCharacterId(this global::Celeste.Player player, PlayableCharacterId id)
        {
            if (player == null) return;
            
            var data = DynamicData.For(player);
            data.Set(CHARACTER_KEY, id);
            
            // Apply character configuration
            PlayableCharacterSystem.SetCurrentCharacter(id);
            PlayableCharacterSystem.ApplyToPlayer(player);
        }
        
        /// <summary>
        /// Get the skin ID assigned to a player
        /// </summary>
        public static string GetCharacterSkinId(this global::Celeste.Player player)
        {
            if (player == null) return "default";
            
            var data = DynamicData.For(player);
            return data.Get<string>(SKIN_KEY) ?? "default";
        }
        
        /// <summary>
        /// Set the skin ID for a player
        /// </summary>
        public static void SetCharacterSkinId(this global::Celeste.Player player, string skinId)
        {
            if (player == null || string.IsNullOrEmpty(skinId)) return;
            
            var data = DynamicData.For(player);
            data.Set(SKIN_KEY, skinId);
            
            PlayableCharacterSystem.SetCurrentSkin(skinId);
        }
        
        /// <summary>
        /// Check if player is using a Kirby-family character
        /// </summary>
        public static bool IsKirbyFamilyCharacter(this global::Celeste.Player player)
        {
            var charId = player.GetPlayableCharacterId();
            return charId == PlayableCharacterId.Kirby ||
                   charId == PlayableCharacterId.MetaKnight ||
                   charId == PlayableCharacterId.KingDedede ||
                   charId == PlayableCharacterId.BandanaWaddleDee;
        }
        
        /// <summary>
        /// Check if player character can hover
        /// </summary>
        public static bool CharacterCanHover(this global::Celeste.Player player)
        {
            var character = PlayableCharacterSystem.GetCharacter(player.GetPlayableCharacterId());
            return character?.CanHover ?? false;
        }
        
        /// <summary>
        /// Check if player character can inhale
        /// </summary>
        public static bool CharacterCanInhale(this global::Celeste.Player player)
        {
            var character = PlayableCharacterSystem.GetCharacter(player.GetPlayableCharacterId());
            return character?.CanInhale ?? false;
        }
    }
}
