namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Maps vanilla Celeste characters to DesoloZantas mod characters.
    /// Use this to replace vanilla NPCs with custom characters.
    /// </summary>
    public static class CharacterMapping
    {
        /// <summary>
        /// Vanilla character identifiers
        /// </summary>
        public enum VanillaCharacter
        {
            Madeline,
            Badeline,
            Theo,
            Granny,
            Oshiro,
            Bird,
            FinalBoss
        }

        /// <summary>
        /// DesoloZantas mod character identifiers
        /// </summary>
        public enum ModCharacter
        {
            // Kirby characters
            Kirby,
            KirbyClassic,
            MetaKnight,
            DarkMetaKnight,
            KingDedede,
            BandanaWadeeDee,
            Magolor,
            Taranza,
            Marx,
            
            // Undertale/Deltarune characters
            Chara,
            Toriel,
            Ralsei,
            Frisk,
            Asriel,
            
            // Original characters
            Els,
            Starsi,
            Maggy,
            Cody,
            
            // Kept vanilla
            Madeline,
            Badeline,
            Theo,
            Granny,
            Oshiro,
            Bird
        }

        private static Dictionary<VanillaCharacter, ModCharacter> _defaultMappings;
        private static Dictionary<string, ModCharacter> _levelMappings;

        static CharacterMapping()
        {
            _defaultMappings = new Dictionary<VanillaCharacter, ModCharacter>
            {
                { VanillaCharacter.Theo, ModCharacter.Kirby },
                { VanillaCharacter.Granny, ModCharacter.Toriel },
                { VanillaCharacter.Badeline, ModCharacter.Chara },
                { VanillaCharacter.Oshiro, ModCharacter.Oshiro },  // Kept
                { VanillaCharacter.Bird, ModCharacter.Bird },      // Kept
                { VanillaCharacter.Madeline, ModCharacter.Madeline }, // Kept
                { VanillaCharacter.FinalBoss, ModCharacter.Chara }
            };

            _levelMappings = new Dictionary<string, ModCharacter>();
        }

        /// <summary>
        /// Get the mod character that replaces a vanilla character
        /// </summary>
        public static ModCharacter GetModCharacter(VanillaCharacter vanilla)
        {
            return _defaultMappings.TryGetValue(vanilla, out var modChar) 
                ? modChar 
                : ModCharacter.Kirby;
        }

        /// <summary>
        /// Set a custom mapping for a specific level
        /// </summary>
        public static void SetLevelMapping(string levelId, VanillaCharacter vanilla, ModCharacter mod)
        {
            string key = $"{levelId}:{vanilla}";
            _levelMappings[key] = mod;
        }

        /// <summary>
        /// Get character for a specific level (falls back to default)
        /// </summary>
        public static ModCharacter GetCharacterForLevel(string levelId, VanillaCharacter vanilla)
        {
            string key = $"{levelId}:{vanilla}";
            if (_levelMappings.TryGetValue(key, out var modChar))
                return modChar;
            return GetModCharacter(vanilla);
        }

        /// <summary>
        /// Get the sprite bank ID for a mod character
        /// </summary>
        public static string GetSpriteBankId(ModCharacter character)
        {
            return character switch
            {
                ModCharacter.Kirby => "kirby",
                ModCharacter.KirbyClassic => "kirby_classic",
                ModCharacter.MetaKnight => "metaknight",
                ModCharacter.DarkMetaKnight => "dark_metaknight",
                ModCharacter.KingDedede => "kingdedede",
                ModCharacter.BandanaWadeeDee => "bandana_dee",
                ModCharacter.Magolor => "magolor",
                ModCharacter.Taranza => "taranza",
                ModCharacter.Marx => "marx",
                ModCharacter.Chara => "chara",
                ModCharacter.Toriel => "toriel",
                ModCharacter.Ralsei => "ralsei",
                ModCharacter.Frisk => "frisk",
                ModCharacter.Asriel => "asriel",
                ModCharacter.Els => "els",
                ModCharacter.Starsi => "starsi",
                ModCharacter.Maggy => "maggy",
                ModCharacter.Cody => "cody",
                ModCharacter.Madeline => "player",
                ModCharacter.Badeline => "badeline",
                ModCharacter.Theo => "theo",
                ModCharacter.Granny => "granny",
                ModCharacter.Oshiro => "oshiro",
                ModCharacter.Bird => "bird",
                _ => "kirby"
            };
        }

        /// <summary>
        /// Get the dialog prefix for a mod character
        /// </summary>
        public static string GetDialogPrefix(ModCharacter character)
        {
            return character switch
            {
                ModCharacter.Kirby => "INGESTE_KIRBY",
                ModCharacter.Magolor => "INGESTE_MAGOLOR",
                ModCharacter.Chara => "INGESTE_CHARA",
                ModCharacter.Toriel => "INGESTE_TORIEL",
                ModCharacter.Ralsei => "INGESTE_RALSEI",
                ModCharacter.Els => "INGESTE_ELS",
                ModCharacter.Starsi => "INGESTE_STARSI",
                ModCharacter.Maggy => "INGESTE_MAGGY",
                _ => "INGESTE"
            };
        }

        /// <summary>
        /// Get character max speed
        /// </summary>
        public static float GetMaxSpeed(ModCharacter character)
        {
            return character switch
            {
                ModCharacter.Kirby => 60f,
                ModCharacter.Magolor => 72f,
                ModCharacter.Toriel => 40f,
                ModCharacter.Ralsei => 55f,
                ModCharacter.MetaKnight => 80f,
                ModCharacter.Chara => 65f,
                _ => 48f
            };
        }

        /// <summary>
        /// Check if character can float
        /// </summary>
        public static bool CanFloat(ModCharacter character)
        {
            return character switch
            {
                ModCharacter.Kirby => true,
                ModCharacter.Magolor => true,
                ModCharacter.MetaKnight => true,
                _ => false
            };
        }
    }
}
