// This file ensures that the Ingeste.Player namespace is properly recognized
// and provides a central location for Player-related functionality

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Core player system for Ingeste mod
    /// </summary>
    public static class PlayerSystem
    {
        private static bool initialized = false;

        /// <summary>
        /// Initialize the Ingeste player system
        /// </summary>
        public static void Initialize()
        {
            if (initialized) return;
            
            initialized = true;
            Logger.Log(LogLevel.Info, "Ingeste.Player", "Player system initialized");
        }

        /// <summary>
        /// Cleanup the Ingeste player system
        /// </summary>
        public static void Cleanup()
        {
            if (!initialized) return;
            
            initialized = false;
            Logger.Log(LogLevel.Info, "Ingeste.Player", "Player system cleaned up");
        }

        /// <summary>
        /// Update player-specific mechanics
        /// </summary>
        public static void Update(global::Celeste.Player player)
        {
            if (!initialized || player == null) return;
            
            // Update player abilities and states
            updatePlayerAbilities(player);
        }

        private static void updatePlayerAbilities(global::Celeste.Player player)
        {
            // Cast IngesteModule.Settings to the correct type to access KirbyPlayerEnabled
            var settings = IngesteModule.Settings as IngesteModuleSettings;
            if (settings != null && settings.KirbyPlayerEnabled == true)
            {
                // Handle hover ability
                if (!player.OnGround() && Input.Jump.Pressed)
                {
                    global::DesoloZantas.Core.Core.Player.PlayerExtensions.SetIngesteState(player, 27); // StHover
                }

                // Handle inhale ability
                if (Input.Grab.Pressed)
                {
                    global::DesoloZantas.Core.Core.Player.PlayerExtensions.SetIngesteState(player, 28); // StInhale
                }
            }
        }
    }
}



