namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Hook manager for replacing vanilla entities with DesoloZantas variants.
    /// Call Initialize() from your module's Load method.
    /// </summary>
    public static class VanillaCoreHooks
    {
        private static bool _initialized = false;

        /// <summary>
        /// Initialize all vanilla core hooks
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;

            // Hook into entity loading to replace vanilla entities
            On.Celeste.Level.LoadLevel += OnLoadLevel;
            On.Celeste.LevelLoader.LoadingThread += OnLoadingThread;

            IngesteLogger.Info("VanillaCore hooks initialized");
        }

        /// <summary>
        /// Cleanup hooks when module unloads
        /// </summary>
        public static void Cleanup()
        {
            if (!_initialized)
                return;

            _initialized = false;

            On.Celeste.Level.LoadLevel -= OnLoadLevel;
            On.Celeste.LevelLoader.LoadingThread -= OnLoadingThread;

            IngesteLogger.Info("VanillaCore hooks cleaned up");
        }

        private static void OnLoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, 
            global::Celeste.Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(self, playerIntro, isFromLoader);

            // Post-load processing: replace/enhance vanilla entities
            PostProcessLevel(self);
        }

        private static void OnLoadingThread(On.Celeste.LevelLoader.orig_LoadingThread orig, LevelLoader self)
        {
            orig(self);

            // Can add pre-processing here if needed
        }

        private static void PostProcessLevel(Level level)
        {
            // Replace vanilla NPCs with mod characters based on current settings
            ReplaceVanillaNPCs(level);

            // Enhance triggers with mod-specific behavior
            EnhanceTriggers(level);

            // Setup character-specific features
            SetupCharacterFeatures(level);
        }

        private static void ReplaceVanillaNPCs(Level level)
        {
            // Check for Kirby mode
            bool isKirbyMode = level.Session.GetFlag("kirby_mode");

            // Find all vanilla NPCs and enhance/replace them
            foreach (var entity in level.Entities)
            {
                if (entity is global::Celeste.NPC vanillaNpc)
                {
                    // Determine which character this should be
                    var vanillaChar = IdentifyVanillaCharacter(vanillaNpc);
                    if (vanillaChar != null)
                    {
                        var modChar = CharacterMapping.GetCharacterForLevel(
                            level.Session.Level, 
                            vanillaChar.Value);

                        // Apply character customization
                        ApplyCharacterCustomization(vanillaNpc, modChar, level);
                    }
                }
            }
        }

        private static CharacterMapping.VanillaCharacter? IdentifyVanillaCharacter(global::Celeste.NPC npc)
        {
            // Identify NPC by sprite or type
            if (npc.Sprite != null)
            {
                string spriteId = npc.Sprite.Animations.Count > 0 ? 
                    npc.Sprite.CurrentAnimationID?.ToLower() : "";

                if (spriteId.Contains("theo"))
                    return CharacterMapping.VanillaCharacter.Theo;
                if (spriteId.Contains("granny"))
                    return CharacterMapping.VanillaCharacter.Granny;
                if (spriteId.Contains("oshiro"))
                    return CharacterMapping.VanillaCharacter.Oshiro;
            }

            return null;
        }

        private static void ApplyCharacterCustomization(global::Celeste.NPC npc, 
            CharacterMapping.ModCharacter modChar, Level level)
        {
            // Get sprite ID for mod character
            string spriteId = CharacterMapping.GetSpriteBankId(modChar);

            try
            {
                // Try to replace sprite
                var newSprite = GFX.SpriteBank.Create(spriteId);
                if (newSprite != null && npc.Sprite != null)
                {
                    npc.Remove(npc.Sprite);
                    npc.Sprite = newSprite;
                    npc.Add(npc.Sprite);
                }

                // Apply speed customization
                npc.Maxspeed = CharacterMapping.GetMaxSpeed(modChar);
            }
            catch
            {
                // Sprite not found, keep original
            }
        }

        private static void EnhanceTriggers(Level level)
        {
            // Enhance existing triggers with mod-specific behavior
            foreach (var entity in level.Entities)
            {
                if (entity is global::Celeste.Trigger trigger)
                {
                    // Add mod-specific components if needed
                }
            }
        }

        private static void SetupCharacterFeatures(Level level)
        {
            bool isKirbyMode = level.Session.GetFlag("kirby_mode");

            if (isKirbyMode)
            {
                // Setup Kirby-specific level features
                SetupKirbyModeFeatures(level);
            }
        }

        private static void SetupKirbyModeFeatures(Level level)
        {
            // Enable Kirby-only entities
            foreach (var entity in level.Entities)
            {
                // Check for Kirby-specific entity tags
                if (entity.TagCheck(Tags.Global) && entity is Booster booster)
                {
                    if (booster.KirbyModeOnly)
                    {
                        booster.Visible = true;
                        booster.Collidable = true;
                    }
                }
            }
        }

        private static bool ShouldReplaceCutscene(string cutsceneType, Level level)
        {
            // List of vanilla cutscenes to replace
            var replaceable = new HashSet<string>
            {
                "CS05_SaveTheo",
                "CS05_SeeTheo",
                "CS06_BossIntro",
                "CS06_Reflection",
                "CS07_Ending",
                "CS10_BadelineHelps",
                "CS10_Ending"
            };

            return replaceable.Contains(cutsceneType);
        }

        private static Entity GetReplacementCutscene(string cutsceneType, Level level)
        {
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player == null)
                return null;

            return cutsceneType switch
            {
                "CS05_SaveTheo" => new SaveCharacterFromCrystalCutscene(player),
                "CS05_SeeTheo" => new SeeCharacterInCrystalCutscene(player, 0),
                "CS06_BossIntro" => new CharaBossIntroCutscene(player),
                "CS10_BadelineHelps" => new CompanionHelpsCutscene(player),
                _ => null
            };
        }
    }

    /// <summary>
    /// Extension methods for vanilla Celeste types
    /// </summary>
    public static class VanillaCoreExtensions
    {
        /// <summary>
        /// Get the DesoloZantas character mapping for this NPC
        /// </summary>
        public static CharacterMapping.ModCharacter GetModCharacter(this global::Celeste.NPC npc, Level level)
        {
            // Default mapping based on NPC type name
            string typeName = npc.GetType().Name;
            if (typeName.Contains("Bird"))
                return CharacterMapping.ModCharacter.Bird;

            // Try to identify from sprite
            string spriteId = npc.Sprite?.CurrentAnimationID?.ToLower() ?? "";
            
            if (spriteId.Contains("theo"))
                return CharacterMapping.GetCharacterForLevel(level.Session.Level, 
                    CharacterMapping.VanillaCharacter.Theo);
            if (spriteId.Contains("granny"))
                return CharacterMapping.GetCharacterForLevel(level.Session.Level, 
                    CharacterMapping.VanillaCharacter.Granny);
            if (spriteId.Contains("badeline"))
                return CharacterMapping.GetCharacterForLevel(level.Session.Level, 
                    CharacterMapping.VanillaCharacter.Badeline);
            
            return CharacterMapping.ModCharacter.Kirby;
        }

        /// <summary>
        /// Check if player is in Kirby mode
        /// </summary>
        public static bool IsKirbyMode(this Level level)
        {
            return level.Session.GetFlag("kirby_mode");
        }

        /// <summary>
        /// Check if a specific character has been met
        /// </summary>
        public static bool HasMetCharacter(this Level level, CharacterMapping.ModCharacter character)
        {
            string flag = character switch
            {
                CharacterMapping.ModCharacter.Kirby => NPC.MetKirby,
                CharacterMapping.ModCharacter.Magolor => NPC.MetMagolor,
                CharacterMapping.ModCharacter.Toriel => NPC.MetToriel,
                CharacterMapping.ModCharacter.Ralsei => NPC.MetRalsei,
                CharacterMapping.ModCharacter.Chara => NPC.MetChara,
                _ => ""
            };

            return !string.IsNullOrEmpty(flag) && level.Session.GetFlag(flag);
        }

        /// <summary>
        /// Set that a character has been met
        /// </summary>
        public static void SetMetCharacter(this Level level, CharacterMapping.ModCharacter character)
        {
            string flag = character switch
            {
                CharacterMapping.ModCharacter.Kirby => NPC.MetKirby,
                CharacterMapping.ModCharacter.Magolor => NPC.MetMagolor,
                CharacterMapping.ModCharacter.Toriel => NPC.MetToriel,
                CharacterMapping.ModCharacter.Ralsei => NPC.MetRalsei,
                CharacterMapping.ModCharacter.Chara => NPC.MetChara,
                _ => ""
            };

            if (!string.IsNullOrEmpty(flag))
                level.Session.SetFlag(flag, true);
        }
    }
}
