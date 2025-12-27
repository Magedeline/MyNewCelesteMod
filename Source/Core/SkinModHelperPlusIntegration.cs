namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Integration class for SkinModHelperPlus to register all Dream Friends characters
    /// as selectable player skins in the main mod.
    /// </summary>
    public static class SkinModHelperPlusIntegration
    {
        private static bool _initialized = false;
        private static readonly Dictionary<string, SkinDefinition> _registeredSkins = new();

        /// <summary>
        /// Initialize the SkinModHelperPlus integration
        /// Called during IngesteModule.Load()
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
                return;

            try
            {
                IngesteLogger.Info("Initializing SkinModHelperPlus integration for Dream Friends...");
                
                RegisterDreamFriendsSkins();
                SetupSkinConfiguration();
                
                _initialized = true;
                IngesteLogger.Info("SkinModHelperPlus integration completed successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to initialize SkinModHelperPlus integration");
            }
        }

        /// <summary>
        /// Register all Dream Friends as player skins
        /// </summary>
        private static void RegisterDreamFriendsSkins()
        {
            var availableSkins = new (string id, string displayName, string spritePath, Color hairColor)[]
            {
                ("kirby", "Kirby", "Characters/Kirby", new Color(255, 183, 197)),
                ("chara", "Chara", "Characters/Chara", new Color(139, 69, 19)),
                ("vessel", "Vessel", "Characters/Vessel", new Color(54, 69, 79)),
                ("ralsei", "Ralsei", "Characters/Ralsei", new Color(74, 74, 74)),
                ("ralsei_gl", "Ralsei (Glasses)", "Characters/RalseiGL", new Color(74, 74, 74)),
                ("ralsei_sil", "Ralsei (Silhouette)", "Characters/RalseiSil", new Color(169, 169, 169)),
                ("theo", "Theo", "Characters/Theo", new Color(139, 69, 19))
            };

            foreach (var (id, displayName, spritePath, hairColor) in availableSkins)
            {
                RegisterSkin(id, displayName, spritePath, hairColor);
            }

            IngesteLogger.Info($"Registered {_registeredSkins.Count} Dream Friends skins (available assets only)");
        }

        /// <summary>
        /// Register a single skin with SkinModHelperPlus
        /// </summary>
        private static void RegisterSkin(string id, string displayName, string spritePath, Color hairColor)
        {
            try
            {
                var skinDef = new SkinDefinition
                {
                    ID = id,
                    DisplayName = displayName,
                    SpritePath = spritePath,
                    HairColor = hairColor,
                    ModName = "Ingeste",
                    Description = $"Play as {displayName} from the Dream Friends collection!"
                };

                _registeredSkins[id] = skinDef;
                
                // Register with SkinModHelperPlus if available
                if (SkinModHelperPlusAPI.IsLoaded)
                {
                    SkinModHelperPlusAPI.RegisterPlayerSkin(skinDef);
                    IngesteLogger.Debug($"Registered skin: {displayName}");
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, $"Failed to register skin: {displayName}");
            }
        }

        /// <summary>
        /// Set up additional skin configuration
        /// </summary>
        private static void SetupSkinConfiguration()
        {
            // Configure skin groups for better organization
            var dreamFriendsGroup = new SkinGroup
            {
                ID = "dream_friends",
                DisplayName = "Dream Friends",
                Description = "Characters from the Ingeste adventure",
                ModName = "Ingeste"
            };

            var kirbyGroup = new SkinGroup
            {
                ID = "kirby_series",
                DisplayName = "Kirby Series",
                Description = "Characters from Planet Popstar",
                ModName = "Ingeste",
                ParentGroup = dreamFriendsGroup
            };

            var undertaleGroup = new SkinGroup
            {
                ID = "undertale_series",
                DisplayName = "Undertale/Deltarune",
                Description = "Monsters and humans from the Underground",
                ModName = "Ingeste",
                ParentGroup = dreamFriendsGroup
            };

            var justiceGroup = new SkinGroup
            {
                ID = "justice_squad",
                DisplayName = "Justice Squad",
                Description = "The seven human souls",
                ModName = "Ingeste",
                ParentGroup = dreamFriendsGroup
            };

            // Register groups with SkinModHelperPlus
            if (SkinModHelperPlusAPI.IsLoaded)
            {
                SkinModHelperPlusAPI.RegisterSkinGroup(dreamFriendsGroup);
                SkinModHelperPlusAPI.RegisterSkinGroup(kirbyGroup);
                SkinModHelperPlusAPI.RegisterSkinGroup(undertaleGroup);
                SkinModHelperPlusAPI.RegisterSkinGroup(justiceGroup);

                // Assign skins to groups
                AssignSkinsToGroups(dreamFriendsGroup, kirbyGroup, undertaleGroup, justiceGroup);
            }
        }

        /// <summary>
        /// Assign registered skins to their respective groups
        /// </summary>
        private static void AssignSkinsToGroups(SkinGroup dreamFriendsGroup, SkinGroup kirbyGroup, SkinGroup undertaleGroup, SkinGroup justiceGroup)
        {
            var kirbyCharacters = new[] { "kirby" };

            var undertaleCharacters = new[] { "chara", "ralsei", "ralsei_gl", "ralsei_sil" };

            var guestCharacters = new[] { "vessel", "theo" };

            foreach (var skinId in kirbyCharacters)
            {
                if (_registeredSkins.TryGetValue(skinId, out var skin))
                {
                    SkinModHelperPlusAPI.AssignSkinToGroup(skin, kirbyGroup);
                }
            }

            foreach (var skinId in undertaleCharacters)
            {
                if (_registeredSkins.TryGetValue(skinId, out var skin))
                {
                    SkinModHelperPlusAPI.AssignSkinToGroup(skin, undertaleGroup);
                }
            }

            foreach (var skinId in guestCharacters)
            {
                if (_registeredSkins.TryGetValue(skinId, out var skin))
                {
                    SkinModHelperPlusAPI.AssignSkinToGroup(skin, dreamFriendsGroup);
                }
            }
        }

        /// <summary>
        /// Get a registered skin by ID
        /// </summary>
        public static SkinDefinition GetSkin(string skinId)
        {
            return _registeredSkins.TryGetValue(skinId, out var skin) ? skin : null;
        }

        /// <summary>
        /// Get all registered skins
        /// </summary>
        public static IEnumerable<SkinDefinition> GetAllSkins()
        {
            return _registeredSkins.Values;
        }

        /// <summary>
        /// Check if SkinModHelperPlus is available
        /// </summary>
        public static bool IsAvailable => SkinModHelperPlusAPI.IsLoaded;

        /// <summary>
        /// Clean up the integration
        /// </summary>
        public static void Cleanup()
        {
            try
            {
                if (_initialized && SkinModHelperPlusAPI.IsLoaded)
                {
                    IngesteLogger.Info("Cleaning up SkinModHelperPlus integration");
                    
                    // Unregister all skins
                    foreach (var skin in _registeredSkins.Values)
                    {
                        SkinModHelperPlusAPI.UnregisterPlayerSkin(skin);
                    }
                    
                    _registeredSkins.Clear();
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Error during SkinModHelperPlus integration cleanup");
            }
            finally
            {
                _initialized = false;
            }
        }
    }

    /// <summary>
    /// Skin definition structure for SkinModHelperPlus compatibility
    /// </summary>
    public class SkinDefinition
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public string SpritePath { get; set; }
        public Color HairColor { get; set; }
        public string ModName { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Properties { get; set; } = [];
    }

    /// <summary>
    /// Skin group definition for organizing skins
    /// </summary>
    public class SkinGroup
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string ModName { get; set; }
        public SkinGroup ParentGroup { get; set; }
    }

    /// <summary>
    /// Mock API interface for SkinModHelperPlus compatibility
    /// This will be replaced by the actual SkinModHelperPlus API when available
    /// </summary>
    public static class SkinModHelperPlusAPI
    {
        public static bool IsLoaded => Everest.Modules.Any(m => 
            m.Metadata?.Name?.Contains("SkinModHelperPlus") == true);

        public static void RegisterPlayerSkin(SkinDefinition skin)
        {
            // This will call the actual SkinModHelperPlus API
            IngesteLogger.Debug($"Mock: Registered skin {skin.DisplayName}");
        }

        public static void UnregisterPlayerSkin(SkinDefinition skin)
        {
            // This will call the actual SkinModHelperPlus API
            IngesteLogger.Debug($"Mock: Unregistered skin {skin.DisplayName}");
        }

        public static void RegisterSkinGroup(SkinGroup group)
        {
            // This will call the actual SkinModHelperPlus API
            IngesteLogger.Debug($"Mock: Registered skin group {group.DisplayName}");
        }

        public static void AssignSkinToGroup(SkinDefinition skin, SkinGroup group)
        {
            // This will call the actual SkinModHelperPlus API
            IngesteLogger.Debug($"Mock: Assigned skin {skin.DisplayName} to group {group.DisplayName}");
        }
    }
}



