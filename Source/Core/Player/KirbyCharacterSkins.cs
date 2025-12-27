using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Comprehensive skin definitions for all playable characters
    /// including color variants, special skins, and self-skin support
    /// </summary>
    public static class KirbyCharacterSkins
    {
        #region Skin Categories
        
        /// <summary>
        /// Categories for organizing skins
        /// </summary>
        public enum SkinCategory
        {
            Default,
            Color,
            Special,
            Crossover,
            Self,
            Custom
        }
        
        #endregion
        
        #region Extended Skin Definition
        
        /// <summary>
        /// Extended skin definition with more customization options
        /// </summary>
        public class ExtendedSkinDefinition : CharacterSkinDefinition
        {
            /// <summary>Skin category</summary>
            public SkinCategory Category { get; set; }
            
            /// <summary>Primary body color</summary>
            public Color PrimaryColor { get; set; }
            
            /// <summary>Secondary/accent color</summary>
            public Color SecondaryColor { get; set; }
            
            /// <summary>Eye color</summary>
            public Color EyeColor { get; set; }
            
            /// <summary>Blush/cheek color</summary>
            public Color BlushColor { get; set; }
            
            /// <summary>Particle effects color</summary>
            public Color ParticleColor { get; set; }
            
            /// <summary>Whether skin has special animations</summary>
            public bool HasSpecialAnimations { get; set; }
            
            /// <summary>Special effect identifier</summary>
            public string SpecialEffectId { get; set; }
            
            /// <summary>Unlock requirement description</summary>
            public string UnlockRequirement { get; set; }
            
            /// <summary>Whether skin is unlocked by default</summary>
            public bool UnlockedByDefault { get; set; }
            
            public ExtendedSkinDefinition() : base()
            {
                Category = SkinCategory.Default;
                PrimaryColor = Color.White;
                SecondaryColor = Color.White;
                EyeColor = Color.Blue;
                BlushColor = Calc.HexToColor("FF6B8A");
                ParticleColor = Color.White;
                HasSpecialAnimations = false;
                UnlockedByDefault = true;
            }
            
            public ExtendedSkinDefinition(string id, string name, SkinCategory category) : base(id, name)
            {
                Category = category;
                PrimaryColor = Color.White;
                SecondaryColor = Color.White;
                EyeColor = Color.Blue;
                BlushColor = Calc.HexToColor("FF6B8A");
                ParticleColor = Color.White;
                HasSpecialAnimations = false;
                UnlockedByDefault = category != SkinCategory.Special;
            }
        }
        
        #endregion
        
        #region Kirby Skins
        
        /// <summary>
        /// Get all Kirby skins
        /// </summary>
        public static List<ExtendedSkinDefinition> GetKirbySkins()
        {
            return new List<ExtendedSkinDefinition>
            {
                // Default Pink Kirby
                new ExtendedSkinDefinition("default", "Pink Kirby", SkinCategory.Default)
                {
                    SpritePath = "kirby_player",
                    PrimaryColor = Calc.HexToColor("FFB6C1"),
                    SecondaryColor = Calc.HexToColor("FF69B4"),
                    BlushColor = Calc.HexToColor("FF6B8A"),
                    ParticleColor = Calc.HexToColor("FFB6C1"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("FFB6C1"), Calc.HexToColor("FFD700") }
                },
                
                // Yellow Kirby (Player 2 classic)
                new ExtendedSkinDefinition("yellow", "Yellow Kirby", SkinCategory.Color)
                {
                    SpritePath = "kirby_yellow",
                    PrimaryColor = Calc.HexToColor("FFFF00"),
                    SecondaryColor = Calc.HexToColor("FFD700"),
                    BlushColor = Calc.HexToColor("FFA500"),
                    ParticleColor = Calc.HexToColor("FFFF00"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("FFFF00"), Calc.HexToColor("FFD700") }
                },
                
                // Blue Kirby (Player 3 classic)
                new ExtendedSkinDefinition("blue", "Blue Kirby", SkinCategory.Color)
                {
                    SpritePath = "kirby_blue",
                    PrimaryColor = Calc.HexToColor("00BFFF"),
                    SecondaryColor = Calc.HexToColor("1E90FF"),
                    BlushColor = Calc.HexToColor("87CEEB"),
                    ParticleColor = Calc.HexToColor("00BFFF"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("00BFFF"), Calc.HexToColor("1E90FF") }
                },
                
                // Green Kirby (Player 4 classic)
                new ExtendedSkinDefinition("green", "Green Kirby", SkinCategory.Color)
                {
                    SpritePath = "kirby_green",
                    PrimaryColor = Calc.HexToColor("32CD32"),
                    SecondaryColor = Calc.HexToColor("228B22"),
                    BlushColor = Calc.HexToColor("90EE90"),
                    ParticleColor = Calc.HexToColor("32CD32"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("32CD32"), Calc.HexToColor("FFD700") }
                },
                
                // Red Kirby
                new ExtendedSkinDefinition("red", "Red Kirby", SkinCategory.Color)
                {
                    SpritePath = "kirby_red",
                    PrimaryColor = Calc.HexToColor("FF4444"),
                    SecondaryColor = Calc.HexToColor("CC0000"),
                    BlushColor = Calc.HexToColor("FF6666"),
                    ParticleColor = Calc.HexToColor("FF4444"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("FF4444"), Calc.HexToColor("FFD700") }
                },
                
                // White Kirby
                new ExtendedSkinDefinition("white", "White Kirby", SkinCategory.Color)
                {
                    SpritePath = "kirby_white",
                    PrimaryColor = Color.White,
                    SecondaryColor = Calc.HexToColor("E8E8E8"),
                    BlushColor = Calc.HexToColor("FFB6C1"),
                    ParticleColor = Color.White,
                    HairColorOverrides = new Color[] { Color.White, Calc.HexToColor("FFD700") }
                },
                
                // Orange Kirby
                new ExtendedSkinDefinition("orange", "Orange Kirby", SkinCategory.Color)
                {
                    SpritePath = "kirby_orange",
                    PrimaryColor = Calc.HexToColor("FFA500"),
                    SecondaryColor = Calc.HexToColor("FF8C00"),
                    BlushColor = Calc.HexToColor("FFCC99"),
                    ParticleColor = Calc.HexToColor("FFA500"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("FFA500"), Calc.HexToColor("FFD700") }
                },
                
                // Purple Kirby
                new ExtendedSkinDefinition("purple", "Purple Kirby", SkinCategory.Color)
                {
                    SpritePath = "kirby_purple",
                    PrimaryColor = Calc.HexToColor("9B30FF"),
                    SecondaryColor = Calc.HexToColor("8B008B"),
                    BlushColor = Calc.HexToColor("DDA0DD"),
                    ParticleColor = Calc.HexToColor("9B30FF"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("9B30FF"), Calc.HexToColor("FFD700") }
                },
                
                // Shadow Kirby (special)
                new ExtendedSkinDefinition("shadow", "Shadow Kirby", SkinCategory.Special)
                {
                    SpritePath = "kirby_shadow",
                    PrimaryColor = Calc.HexToColor("404040"),
                    SecondaryColor = Calc.HexToColor("202020"),
                    EyeColor = Calc.HexToColor("FFD700"),
                    BlushColor = Calc.HexToColor("606060"),
                    ParticleColor = Calc.HexToColor("404040"),
                    HasSpecialAnimations = true,
                    SpecialEffectId = "shadow_trail",
                    HairColorOverrides = new Color[] { Calc.HexToColor("404040"), Calc.HexToColor("FFD700") },
                    UnlockRequirement = "Complete Amazing Mirror reference"
                },
                
                // Keeby (Kirby's Dream Course Yellow)
                new ExtendedSkinDefinition("keeby", "Keeby", SkinCategory.Special)
                {
                    SpritePath = "kirby_keeby",
                    PrimaryColor = Calc.HexToColor("FFFF00"),
                    SecondaryColor = Calc.HexToColor("FFD700"),
                    BlushColor = Calc.HexToColor("FF6B8A"),
                    ParticleColor = Calc.HexToColor("FFFF00"),
                    HasSpecialAnimations = false,
                    HairColorOverrides = new Color[] { Calc.HexToColor("FFFF00"), Calc.HexToColor("00FF00") }
                },
                
                // Ghost Kirby
                new ExtendedSkinDefinition("ghost", "Ghost Kirby", SkinCategory.Special)
                {
                    SpritePath = "kirby_ghost",
                    PrimaryColor = Color.White * 0.7f,
                    SecondaryColor = Calc.HexToColor("E8E8FF") * 0.7f,
                    BlushColor = Calc.HexToColor("CCCCFF"),
                    ParticleColor = Color.White * 0.5f,
                    HasSpecialAnimations = true,
                    SpecialEffectId = "ghost_fade",
                    HairColorOverrides = new Color[] { Color.White * 0.7f, Calc.HexToColor("87CEEB") },
                    UnlockRequirement = "Complete Ghost Gordo challenge"
                },
                
                // Ice Kirby (Frozen Friends reference)
                new ExtendedSkinDefinition("ice", "Ice Kirby", SkinCategory.Special)
                {
                    SpritePath = "kirby_ice",
                    PrimaryColor = Calc.HexToColor("ADD8E6"),
                    SecondaryColor = Calc.HexToColor("87CEEB"),
                    BlushColor = Calc.HexToColor("B0E0E6"),
                    ParticleColor = Calc.HexToColor("00CED1"),
                    HasSpecialAnimations = true,
                    SpecialEffectId = "ice_sparkle",
                    HairColorOverrides = new Color[] { Calc.HexToColor("ADD8E6"), Calc.HexToColor("00FFFF") }
                },
                
                // Fire Kirby
                new ExtendedSkinDefinition("fire", "Fire Kirby", SkinCategory.Special)
                {
                    SpritePath = "kirby_fire",
                    PrimaryColor = Calc.HexToColor("FF6600"),
                    SecondaryColor = Calc.HexToColor("FF4400"),
                    BlushColor = Calc.HexToColor("FFCC00"),
                    ParticleColor = Calc.HexToColor("FF3300"),
                    HasSpecialAnimations = true,
                    SpecialEffectId = "fire_ember",
                    HairColorOverrides = new Color[] { Calc.HexToColor("FF6600"), Calc.HexToColor("FFFF00") }
                },
                
                // Star Rod Kirby
                new ExtendedSkinDefinition("star_rod", "Star Rod Kirby", SkinCategory.Special)
                {
                    SpritePath = "kirby_star_rod",
                    PrimaryColor = Calc.HexToColor("FFD700"),
                    SecondaryColor = Calc.HexToColor("FFA500"),
                    BlushColor = Calc.HexToColor("FFEC8B"),
                    ParticleColor = Calc.HexToColor("FFD700"),
                    HasSpecialAnimations = true,
                    SpecialEffectId = "star_sparkle",
                    HairColorOverrides = new Color[] { Calc.HexToColor("FFD700"), Color.White },
                    UnlockRequirement = "Collect all Star Fragments"
                },
                
                // Celeste Madeline Kirby (Crossover)
                new ExtendedSkinDefinition("madeline", "Madeline Kirby", SkinCategory.Crossover)
                {
                    SpritePath = "kirby_madeline",
                    PrimaryColor = Calc.HexToColor("AC3232"),
                    SecondaryColor = Calc.HexToColor("FF6DEF"),
                    BlushColor = Calc.HexToColor("FFB6C1"),
                    ParticleColor = Calc.HexToColor("AC3232"),
                    HasSpecialAnimations = true,
                    SpecialEffectId = "madeline_hair",
                    HairColorOverrides = new Color[] { Calc.HexToColor("AC3232"), Calc.HexToColor("FF6DEF") }
                },
                
                // Character Self (uses player configuration)
                new ExtendedSkinDefinition("self", "Self", SkinCategory.Self)
                {
                    IsSelfSkin = true,
                    UnlockedByDefault = true
                }
            };
        }
        
        #endregion
        
        #region Meta Knight Skins
        
        /// <summary>
        /// Get all Meta Knight skins
        /// </summary>
        public static List<ExtendedSkinDefinition> GetMetaKnightSkins()
        {
            return new List<ExtendedSkinDefinition>
            {
                new ExtendedSkinDefinition("default", "Meta Knight", SkinCategory.Default)
                {
                    SpritePath = "meta_knight",
                    PrimaryColor = Calc.HexToColor("000080"),
                    SecondaryColor = Calc.HexToColor("C0C0C0"),
                    EyeColor = Calc.HexToColor("FFFF00"),
                    ParticleColor = Calc.HexToColor("000080"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("000080"), Calc.HexToColor("FFD700") }
                },
                
                new ExtendedSkinDefinition("galacta", "Galacta Knight", SkinCategory.Special)
                {
                    SpritePath = "galacta_knight",
                    PrimaryColor = Calc.HexToColor("FF69B4"),
                    SecondaryColor = Calc.HexToColor("FFD700"),
                    EyeColor = Calc.HexToColor("FF1493"),
                    ParticleColor = Calc.HexToColor("FF69B4"),
                    HasSpecialAnimations = true,
                    SpecialEffectId = "galacta_wings",
                    HairColorOverrides = new Color[] { Calc.HexToColor("FF69B4"), Color.White },
                    UnlockRequirement = "Defeat all bosses"
                },
                
                new ExtendedSkinDefinition("dark", "Dark Meta Knight", SkinCategory.Special)
                {
                    SpritePath = "dark_meta_knight",
                    PrimaryColor = Calc.HexToColor("404040"),
                    SecondaryColor = Calc.HexToColor("800080"),
                    EyeColor = Calc.HexToColor("FF0000"),
                    ParticleColor = Calc.HexToColor("800080"),
                    HasSpecialAnimations = true,
                    SpecialEffectId = "mirror_shatter",
                    HairColorOverrides = new Color[] { Calc.HexToColor("404040"), Calc.HexToColor("FF0000") },
                    UnlockRequirement = "Complete Mirror World"
                },
                
                new ExtendedSkinDefinition("morpho", "Morpho Knight", SkinCategory.Special)
                {
                    SpritePath = "morpho_knight",
                    PrimaryColor = Calc.HexToColor("FF6600"),
                    SecondaryColor = Calc.HexToColor("FFD700"),
                    EyeColor = Calc.HexToColor("00FFFF"),
                    ParticleColor = Calc.HexToColor("FF6600"),
                    HasSpecialAnimations = true,
                    SpecialEffectId = "butterfly_soul",
                    HairColorOverrides = new Color[] { Calc.HexToColor("FF6600"), Calc.HexToColor("00FFFF") },
                    UnlockRequirement = "True ending unlock"
                },
                
                new ExtendedSkinDefinition("self", "Self", SkinCategory.Self)
                {
                    IsSelfSkin = true
                }
            };
        }
        
        #endregion
        
        #region King Dedede Skins
        
        /// <summary>
        /// Get all King Dedede skins
        /// </summary>
        public static List<ExtendedSkinDefinition> GetKingDededeSkins()
        {
            return new List<ExtendedSkinDefinition>
            {
                new ExtendedSkinDefinition("default", "King Dedede", SkinCategory.Default)
                {
                    SpritePath = "king_dedede",
                    PrimaryColor = Calc.HexToColor("0000CD"),
                    SecondaryColor = Calc.HexToColor("FFD700"),
                    EyeColor = Calc.HexToColor("000000"),
                    BlushColor = Calc.HexToColor("FFA500"),
                    ParticleColor = Calc.HexToColor("0000CD"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("0000CD"), Calc.HexToColor("FFD700") }
                },
                
                new ExtendedSkinDefinition("masked", "Masked Dedede", SkinCategory.Special)
                {
                    SpritePath = "dedede_masked",
                    PrimaryColor = Calc.HexToColor("0000CD"),
                    SecondaryColor = Calc.HexToColor("808080"),
                    EyeColor = Calc.HexToColor("FF0000"),
                    ParticleColor = Calc.HexToColor("FF4500"),
                    HasSpecialAnimations = true,
                    SpecialEffectId = "rage_aura",
                    HairColorOverrides = new Color[] { Calc.HexToColor("0000CD"), Calc.HexToColor("FF0000") },
                    UnlockRequirement = "Complete Revenge of the King"
                },
                
                new ExtendedSkinDefinition("shadow", "Shadow Dedede", SkinCategory.Special)
                {
                    SpritePath = "dedede_shadow",
                    PrimaryColor = Calc.HexToColor("404040"),
                    SecondaryColor = Calc.HexToColor("FFD700"),
                    EyeColor = Calc.HexToColor("800080"),
                    ParticleColor = Calc.HexToColor("800080"),
                    HasSpecialAnimations = true,
                    SpecialEffectId = "shadow_hammer",
                    HairColorOverrides = new Color[] { Calc.HexToColor("404040"), Calc.HexToColor("800080") }
                },
                
                new ExtendedSkinDefinition("self", "Self", SkinCategory.Self)
                {
                    IsSelfSkin = true
                }
            };
        }
        
        #endregion
        
        #region Bandana Waddle Dee Skins
        
        /// <summary>
        /// Get all Bandana Waddle Dee skins
        /// </summary>
        public static List<ExtendedSkinDefinition> GetBandanaDeeSkins()
        {
            return new List<ExtendedSkinDefinition>
            {
                new ExtendedSkinDefinition("default", "Bandana Waddle Dee", SkinCategory.Default)
                {
                    SpritePath = "bandana_dee",
                    PrimaryColor = Calc.HexToColor("FFA500"),
                    SecondaryColor = Calc.HexToColor("0000FF"),
                    EyeColor = Calc.HexToColor("000000"),
                    BlushColor = Calc.HexToColor("FF6B8A"),
                    ParticleColor = Calc.HexToColor("FFA500"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("FFA500"), Calc.HexToColor("0000FF") }
                },
                
                new ExtendedSkinDefinition("sailor", "Sailor Dee", SkinCategory.Special)
                {
                    SpritePath = "sailor_dee",
                    PrimaryColor = Calc.HexToColor("FFA500"),
                    SecondaryColor = Color.White,
                    ParticleColor = Calc.HexToColor("00BFFF"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("FFA500"), Color.White }
                },
                
                new ExtendedSkinDefinition("parasol", "Parasol Dee", SkinCategory.Special)
                {
                    SpritePath = "parasol_dee",
                    PrimaryColor = Calc.HexToColor("FFA500"),
                    SecondaryColor = Calc.HexToColor("FF69B4"),
                    ParticleColor = Calc.HexToColor("FF69B4"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("FFA500"), Calc.HexToColor("FF69B4") }
                },
                
                new ExtendedSkinDefinition("self", "Self", SkinCategory.Self)
                {
                    IsSelfSkin = true
                }
            };
        }
        
        #endregion
        
        #region Madeline Skins
        
        /// <summary>
        /// Get all Madeline skins
        /// </summary>
        public static List<ExtendedSkinDefinition> GetMadelineSkins()
        {
            return new List<ExtendedSkinDefinition>
            {
                new ExtendedSkinDefinition("default", "Madeline", SkinCategory.Default)
                {
                    SpritePath = "player",
                    PrimaryColor = Calc.HexToColor("AC3232"),
                    SecondaryColor = Calc.HexToColor("44B7FF"),
                    ParticleColor = Calc.HexToColor("AC3232"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("AC3232"), Calc.HexToColor("FF6DEF") }
                },
                
                new ExtendedSkinDefinition("badeline", "Badeline", SkinCategory.Special)
                {
                    SpritePath = "badeline",
                    PrimaryColor = Calc.HexToColor("9B30FF"),
                    SecondaryColor = Calc.HexToColor("FF00FF"),
                    ParticleColor = Calc.HexToColor("9B30FF"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("9B30FF"), Calc.HexToColor("FF00FF") }
                },
                
                new ExtendedSkinDefinition("granny", "Granny", SkinCategory.Special)
                {
                    SpritePath = "granny",
                    PrimaryColor = Calc.HexToColor("C0C0C0"),
                    SecondaryColor = Calc.HexToColor("808080"),
                    ParticleColor = Calc.HexToColor("C0C0C0"),
                    HairColorOverrides = new Color[] { Calc.HexToColor("C0C0C0"), Calc.HexToColor("FFD700") }
                },
                
                new ExtendedSkinDefinition("self", "Self", SkinCategory.Self)
                {
                    IsSelfSkin = true
                }
            };
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Get all skins for a specific character
        /// </summary>
        public static List<ExtendedSkinDefinition> GetSkinsForCharacter(PlayableCharacterId characterId)
        {
            return characterId switch
            {
                PlayableCharacterId.Kirby => GetKirbySkins(),
                PlayableCharacterId.MetaKnight => GetMetaKnightSkins(),
                PlayableCharacterId.KingDedede => GetKingDededeSkins(),
                PlayableCharacterId.BandanaWaddleDee => GetBandanaDeeSkins(),
                PlayableCharacterId.Madeline => GetMadelineSkins(),
                PlayableCharacterId.Badeline => GetMadelineSkins(), // Shares with Madeline
                _ => new List<ExtendedSkinDefinition>
                {
                    new ExtendedSkinDefinition("default", "Default", SkinCategory.Default),
                    new ExtendedSkinDefinition("self", "Self", SkinCategory.Self) { IsSelfSkin = true }
                }
            };
        }
        
        /// <summary>
        /// Get a specific skin by ID for a character
        /// </summary>
        public static ExtendedSkinDefinition GetSkin(PlayableCharacterId characterId, string skinId)
        {
            var skins = GetSkinsForCharacter(characterId);
            return skins.Find(s => s.SkinId == skinId) ?? skins[0];
        }
        
        /// <summary>
        /// Check if a skin is unlocked (placeholder for save data integration)
        /// </summary>
        public static bool IsSkinUnlocked(PlayableCharacterId characterId, string skinId)
        {
            var skin = GetSkin(characterId, skinId);
            if (skin == null) return false;
            
            // TODO: Check save data for unlock status
            return skin.UnlockedByDefault;
        }
        
        /// <summary>
        /// Get all unlocked skins for a character
        /// </summary>
        public static List<ExtendedSkinDefinition> GetUnlockedSkins(PlayableCharacterId characterId)
        {
            var skins = GetSkinsForCharacter(characterId);
            return skins.FindAll(s => IsSkinUnlocked(characterId, s.SkinId));
        }
        
        #endregion
    }
}
