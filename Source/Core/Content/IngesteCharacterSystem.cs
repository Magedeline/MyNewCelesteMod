namespace DesoloZantas.Core.Core.Content
{
    /// <summary>
    /// Master integration system for the Ingeste character system
    /// Handles initialization, updates, and integration with existing Celeste systems
    /// </summary>
    public static class IngesteCharacterSystem
    {
        private static bool initialized = false;
        private static CelestePlayer currentPlayer;
        private static Level currentLevel;
        
        /// <summary>
        /// Initialize the entire character system
        /// </summary>
        public static void Initialize()
        {
            if (initialized) return;
            
            try
            {
                // Initialize character registry
                IngesteCharacterRegistry.Initialize();
                
                // Set up hooks for player events
                SetupPlayerHooks();
                
                initialized = true;
                Logger.Log(LogLevel.Info, "IngesteCharacterSystem", "Character system initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteCharacterSystem", $"Failed to initialize character system: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Cleanup the character system
        /// </summary>
        public static void Cleanup()
        {
            if (!initialized) return;
            
            try
            {
                CharacterAbilityRegistry.DeactivateAllCharacters();
                CleanupPlayerHooks();
                
                initialized = false;
                Logger.Log(LogLevel.Info, "IngesteCharacterSystem", "Character system cleaned up");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteCharacterSystem", $"Error during cleanup: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Update the character system - should be called every frame
        /// </summary>
        public static void Update(CelestePlayer player, Level level)
        {
            if (!initialized) return;
            
            currentPlayer = player;
            currentLevel = level;
            
            try
            {
                // Update active character abilities
                CharacterAbilityRegistry.Update();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteCharacterSystem", $"Error during update: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Handle player dash events
        /// </summary>
        public static void OnPlayerDash(CelestePlayer player, Vector2 dashDirection)
        {
            if (!initialized || player == null) return;
            
            try
            {
                CharacterAbilityRegistry.OnPlayerDash(dashDirection);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteCharacterSystem", $"Error handling dash: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Handle player landing events
        /// </summary>
        public static void OnPlayerLand(CelestePlayer player)
        {
            if (!initialized || player == null) return;
            
            try
            {
                // Notify all active abilities about landing
                foreach (var ability in CharacterAbilityRegistry.GetAllCharacters().Values)
                {
                    foreach (var charAbility in ability.Abilities)
                    {
                        if (charAbility.IsActive)
                        {
                            charAbility.OnPlayerLand();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteCharacterSystem", $"Error handling landing: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Handle player wall grab events
        /// </summary>
        public static void OnPlayerWallGrab(CelestePlayer player)
        {
            if (!initialized || player == null) return;
            
            try
            {
                // Notify all active abilities about wall grab
                foreach (var ability in CharacterAbilityRegistry.GetAllCharacters().Values)
                {
                    foreach (var charAbility in ability.Abilities)
                    {
                        if (charAbility.IsActive)
                        {
                            charAbility.OnPlayerWallGrab();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "IngesteCharacterSystem", $"Error handling wall grab: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Get all available characters for UI display
        /// </summary>
        public static CharacterAbilityRegistry.CharacterData[] GetAvailableCharacters()
        {
            var characters = CharacterAbilityRegistry.GetAllCharacters();
            var result = new CharacterAbilityRegistry.CharacterData[characters.Count];
            int index = 0;
            
            foreach (var character in characters.Values)
            {
                result[index++] = character;
            }
            
            return result;
        }
        
        /// <summary>
        /// Activate a specific character by ID
        /// </summary>
        public static bool ActivateCharacter(string characterId)
        {
            if (!initialized || currentPlayer == null || currentLevel == null) return false;
            
            try
            {
                CharacterAbilityRegistry.ActivateCharacter(characterId, currentPlayer, currentLevel);
                Logger.Log(LogLevel.Info, "IngesteCharacterSystem", $"Activated character: {characterId}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteCharacterSystem", $"Failed to activate character {characterId}: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Deactivate all characters
        /// </summary>
        public static void DeactivateAllCharacters()
        {
            if (!initialized) return;
            
            try
            {
                CharacterAbilityRegistry.DeactivateAllCharacters();
                Logger.Log(LogLevel.Info, "IngesteCharacterSystem", "Deactivated all characters");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteCharacterSystem", $"Error deactivating characters: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Get character information by ID
        /// </summary>
        public static CharacterAbilityRegistry.CharacterData? GetCharacterInfo(string characterId)
        {
            return CharacterAbilityRegistry.GetCharacter(characterId);
        }
        
        private static void SetupPlayerHooks()
        {
            // This would hook into Celeste's player events
            // For now, it's a placeholder for future hook implementation
            Logger.Log(LogLevel.Debug, "IngesteCharacterSystem", "Player hooks setup (placeholder)");
        }
        
        private static void CleanupPlayerHooks()
        {
            // Cleanup hooks
            Logger.Log(LogLevel.Debug, "IngesteCharacterSystem", "Player hooks cleaned up (placeholder)");
        }
        
        /// <summary>
        /// Character system statistics for debugging
        /// </summary>
        public static class Statistics
        {
            public static int RegisteredCharacterCount => CharacterAbilityRegistry.GetAllCharacters().Count;
            public static bool IsInitialized => initialized;
            public static bool HasActivePlayer => currentPlayer != null;
            public static bool HasActiveLevel => currentLevel != null;
            
            public static void LogSystemStatus()
            {
                Logger.Log(LogLevel.Info, "IngesteCharacterSystem.Stats", 
                    $"Status: Initialized={IsInitialized}, Characters={RegisteredCharacterCount}, " +
                    $"Player={HasActivePlayer}, Level={HasActiveLevel}");
            }
        }
    }
    
    /// <summary>
    /// Extension methods for easier character system integration
    /// </summary>
    public static class CharacterSystemExtensions
    {
        /// <summary>
        /// Check if a player has an active character ability
        /// </summary>
        public static bool HasActiveCharacterAbility(this CelestePlayer player)
        {
            // This would check if the player currently has any active character abilities
            return IngesteCharacterSystem.Statistics.IsInitialized;
        }
        
        /// <summary>
        /// Get the current character ID for a player (if any)
        /// </summary>
        public static string GetCurrentCharacterId(this CelestePlayer player)
        {
            // This would return the current character ID
            // For now, return empty string as placeholder
            return "";
        }
    }
}



