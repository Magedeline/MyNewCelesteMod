namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Enum representing different player inventory states
    /// </summary>
    public enum IngestePlayerInventory
    {
        Default,
        Heart,
        KirbyPlayer,
        SayGoodbye,
        TitanTowerClimbing,
        Corruption,
        TheEnd
    }

    /// <summary>
    /// Manages player inventory and ability states
    /// </summary>
    public static class PlayerInventoryManager
    {
        private static Dictionary<int, Action<global::Celeste.Player>> stateEffects;
        private static bool initialized = false;

        /// <summary>
        /// Initialize the player inventory system
        /// </summary>
        public static void Initialize()
        {
            if (initialized) return;

            stateEffects = new Dictionary<int, Action<global::Celeste.Player>>();
            RegisterStateEffects();
            initialized = true;
            
            IngesteLogger.Info("PlayerInventory system initialized");
        }

        /// <summary>
        /// Register state effects for different player states
        /// </summary>
        private static void RegisterStateEffects()
        {
            // Default state (normal gameplay)
            stateEffects[(int)IngesteStates.StNormal] = player => {
                // No special effects for default state
            };
            
            // Heart power state
            stateEffects[1] = player => {
                // Apply heart power effects
                player.Dashes = 2; // Double dash
            };
            
            // Kirby player state
            stateEffects[2] = player => {
                // Apply Kirby player effects
                var settings = IngesteModule.Settings as IngesteModuleSettings;
                if (settings != null) 
                    settings.KirbyPlayerEnabled = true;
            };
            
            // Say Goodbye state
            stateEffects[3] = player => {
                // Apply Say Goodbye effects
                player.Dashes = 1;
                // Additional effects specific to this state
            };
            
            // Titan Tower Climbing state
            stateEffects[4] = player => {
                // Apply Titan Tower Climbing effects
                player.Dashes = 2;
                // Additional climbing abilities
            };
            
            // Corruption state
            stateEffects[5] = player => {
                // Apply Corruption effects
                player.Dashes = 1;
                // Additional corruption-specific abilities
            };
            
            // The End state
            stateEffects[10] = player => {
                // Apply The End state effects
                player.Dashes = 3; // Triple dash
                // Additional end-game abilities
            };
        }

        /// <summary>
        /// Reset player to default state
        /// </summary>
        public static void ResetToDefault(Level level)
        {
            if (!initialized) Initialize();
            
            foreach (var entity in level.Tracker.GetEntities<global::Celeste.Player>())
            {
                // Cast to proper Player type
                var player = entity as global::Celeste.Player;
                if (player == null) continue;
                
                // Reset to normal state
                player.StateMachine.State = (int)IngesteStates.StNormal;
                
                // Disable Kirby abilities
                var settings = IngesteModule.Settings as IngesteModuleSettings;
                if (settings != null) 
                    settings.KirbyPlayerEnabled = false;
                
                // Apply default state effects
                if (stateEffects.TryGetValue((int)IngesteStates.StNormal, out var effect))
                    effect(player);
                
                // Reset dashes to default
                player.Dashes = 1;
            }
            
            IngesteLogger.Info("Reset player to default state");
        }

        /// <summary>
        /// Enable Heart Power
        /// </summary>
        public static void EnableHeartPower(Level level)
        {
            if (!initialized) Initialize();
            
            foreach (var entity in level.Tracker.GetEntities<global::Celeste.Player>())
            {
                // Cast to proper Player type
                var player = entity as global::Celeste.Player;
                if (player == null) continue;
                
                player.StateMachine.State = 1; // Heart Power state
                
                if (stateEffects.TryGetValue(1, out var effect))
                    effect(player);
            }
            
            IngesteLogger.Info("Enabled Heart Power");
        }

        /// <summary>
        /// Enable Kirby Player
        /// </summary>
        public static void EnableKirbyPlayer(Level level)
        {
            if (!initialized) Initialize();
            
            foreach (var entity in level.Tracker.GetEntities<global::Celeste.Player>())
            {
                // Cast to proper Player type
                var player = entity as global::Celeste.Player;
                if (player == null) continue;
                
                player.StateMachine.State = 2; // Kirby Player state
                
                if (stateEffects.TryGetValue(2, out var effect))
                    effect(player);
            }
            
            IngesteLogger.Info("Enabled Kirby Player");
        }

        /// <summary>
        /// Enable Say Goodbye
        /// </summary>
        public static void EnableSayGoodbye(Level level)
        {
            if (!initialized) Initialize();
            
            foreach (var entity in level.Tracker.GetEntities<global::Celeste.Player>())
            {
                // Cast to proper Player type
                var player = entity as global::Celeste.Player;
                if (player == null) continue;
                
                player.StateMachine.State = 3; // Say Goodbye state
                
                if (stateEffects.TryGetValue(3, out var effect))
                    effect(player);
            }
            
            IngesteLogger.Info("Enabled Say Goodbye");
        }

        /// <summary>
        /// Enable Titan Tower Climbing
        /// </summary>
        public static void EnableTitanTowerClimbing(Level level)
        {
            if (!initialized) Initialize();
            
            foreach (var entity in level.Tracker.GetEntities<global::Celeste.Player>())
            {
                // Cast to proper Player type
                var player = entity as global::Celeste.Player;
                if (player == null) continue;
                
                player.StateMachine.State = 4; // Titan Tower Climbing state
                
                if (stateEffects.TryGetValue(4, out var effect))
                    effect(player);
            }
            
            IngesteLogger.Info("Enabled Titan Tower Climbing");
        }

        /// <summary>
        /// Enable Corruption
        /// </summary>
        public static void EnableCorruption(Level level)
        {
            if (!initialized) Initialize();
            
            foreach (var entity in level.Tracker.GetEntities<global::Celeste.Player>())
            {
                // Cast to proper Player type
                var player = entity as global::Celeste.Player;
                if (player == null) continue;
                
                player.StateMachine.State = 5; // Corruption state
                
                if (stateEffects.TryGetValue(5, out var effect))
                    effect(player);
            }
            
            IngesteLogger.Info("Enabled Corruption");
        }

        /// <summary>
        /// Enable The End
        /// </summary>
        public static void EnableTheEnd(Level level)
        {
            if (!initialized) Initialize();
            
            foreach (var entity in level.Tracker.GetEntities<global::Celeste.Player>())
            {
                // Cast to proper Player type
                var player = entity as global::Celeste.Player;
                if (player == null) continue;
                
                player.StateMachine.State = 10; // The End state
                
                if (stateEffects.TryGetValue(10, out var effect))
                    effect(player);
            }
            
            IngesteLogger.Info("Enabled The End");
        }

        public static int GetMaxDashesForInventory(IngestePlayerInventory inventory)
        {
            return inventory switch
            {
                IngestePlayerInventory.Default => 1,
                IngestePlayerInventory.Heart => 2,
                IngestePlayerInventory.KirbyPlayer => 2,
                IngestePlayerInventory.SayGoodbye => 1,
                IngestePlayerInventory.TitanTowerClimbing => 2,
                IngestePlayerInventory.Corruption => 1,
                IngestePlayerInventory.TheEnd => 3,
                _ => 1
            };
        }
    }
}



