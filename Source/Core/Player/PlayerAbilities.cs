namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Manages player abilities and power states for IngesteExtensionHelper
    /// </summary>
    public static class PlayerAbilities
    {
        /// <summary>
        /// Check if the player can use hover ability
        /// </summary>
        public static bool CanHover(global::Celeste.Player player)
        {
            if (player == null) return false;

            // Fix: Cast settings to correct type before accessing KirbyPlayerEnabled
            var settings = IngesteModule.Settings as IngesteModuleSettings;
            if (settings == null) return false;

            return settings.KirbyPlayerEnabled && !player.OnGround();
        }

        /// <summary>
        /// Check if the player can use inhale ability
        /// </summary>
        public static bool CanInhale(global::Celeste.Player player)
        {
            if (player == null) return false;

            var settings = IngesteModule.Settings as IngesteModuleSettings;
            if (settings == null) return false;

            return settings.KirbyPlayerEnabled;
        }

        /// <summary>
        /// Start hover ability
        /// </summary>
        public static bool TryStartHover(global::Celeste.Player player)
        {
            if (!CanHover(player)) return false;

            global::DesoloZantas.Core.Core.Player.PlayerExtensions.SetIngesteState(player, 27); // StHover
            return true;
        }

        /// <summary>
        /// Start inhale ability
        /// </summary>
        public static bool TryStartInhale(global::Celeste.Player player)
        {
            if (!CanInhale(player)) return false;

            global::DesoloZantas.Core.Core.Player.PlayerExtensions.SetIngesteState(player, 28); // StInhale
            return true;
        }

        /// <summary>
        /// Calculate hover fall speed based on settings
        /// </summary>
        public static float GetHoverFallSpeed()
        {
            var settings = IngesteModule.Settings as IngesteModuleSettings;
            return settings?.HoverFallSpeedFloat ?? 60f;
        }

        /// <summary>
        /// Get inhale range based on settings
        /// </summary>
        public static float GetInhaleRange()
        {
            var settings = IngesteModule.Settings as IngesteModuleSettings;
            return settings?.InhaleRangeFloat ?? 64f;
        }
    }
}



