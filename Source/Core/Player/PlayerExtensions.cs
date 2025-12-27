using DesoloZantas.Core.Core.Extensions;
using DesoloZantas.Core.Core.Settings;
using MonoMod.Utils;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Extension methods for the Celeste Player class to add Ingeste-specific functionality.
    /// Uses DynamicData pattern inspired by Celeste-Aqua for clean state management.
    /// </summary>
    public static class PlayerExtensions
    {
        #region Basic Ingeste Extensions
        
        /// <summary>
        /// Get the player's current position with additional Ingeste context
        /// </summary>
        public static Vector2 GetIngestePosition(this global::Celeste.Player player)
        {
            return player?.Position ?? Vector2.Zero;
        }

        /// <summary>
        /// Check if the player has any Ingeste-specific states active
        /// </summary>
        public static bool HasIngesteStates(this global::Celeste.Player player)
        {
            if (player == null) return false;
            
            // Check for custom Kirby states
            return player.StateMachine.State >= KirbyModeHooks.ST_KIRBY_NORMAL;
        }

        /// <summary>
        /// Safely set player state with validation
        /// </summary>
        public static void SetIngesteState(this global::Celeste.Player player, int state)
        {
            if (player?.StateMachine == null) return;
            
            player.StateMachine.State = state;
        }

        /// <summary>
        /// Get current Ingeste state if applicable
        /// </summary>
        public static int? GetIngesteState(this global::Celeste.Player player)
        {
            if (player?.StateMachine == null) return null;
            
            int currentState = player.StateMachine.State;
            if (currentState >= KirbyModeHooks.ST_KIRBY_NORMAL && currentState < KirbyModeHooks.ST_KIRBY_MAX)
            {
                return currentState;
            }
            
            return null;
        }
        
        #endregion
        
        #region DynamicData-based Kirby State Management
        
        /// <summary>
        /// Initialize Kirby data storage on player using DynamicData
        /// </summary>
        public static void InitializeKirbyData(this global::Celeste.Player player)
        {
            var data = DynamicData.For(player);
            
            // Initialize all Kirby-related data fields
            data.Set("kirby_enabled", false);
            data.Set("kirby_component", null);
            data.Set("kirby_power_state", KirbyPlayerComponent.PowerState.None);
            data.Set("kirby_inhale_timer", 0f);
            data.Set("kirby_dash_timer", 0f);
            data.Set("kirby_attack_timer", 0f);
            data.Set("kirby_parry_timer", 0f);
            data.Set("kirby_dash_direction", Facings.Right);
            data.Set("kirby_is_inhaling", false);
            data.Set("kirby_is_dashing", false);
            data.Set("kirby_is_parrying", false);
            data.Set("kirby_wall_bounce_timer", 0f);
            data.Set("kirby_wave_dash_count", 0);
            data.Set("kirby_wave_dash_timer", 0f);
            data.Set("kirby_charge_time", 0f);
            data.Set("kirby_charged_dash_direction", Vector2.Zero);
            data.Set("kirby_grabbed_entity", null);
            data.Set("kirby_grab_slam_timer", 0f);
            data.Set("kirby_candy_invincible", false);
            data.Set("kirby_candy_timer", 0f);
            data.Set("kirby_star_power_charge", 0f);
            data.Set("kirby_last_attack_direction", Vector2.Zero);
            data.Set("kirby_parry_successful", false);
            data.Set("kirby_attack_combo", 0);
        }
        
        /// <summary>
        /// Check if player is in Kirby mode using DynamicData
        /// </summary>
        public static bool IsKirbyMode(this global::Celeste.Player player)
        {
            if (player == null) return false;
            return DynamicData.For(player).Get<bool>("kirby_enabled");
        }
        
        /// <summary>
        /// Enable Kirby mode for a player
        /// </summary>
        public static void EnableKirbyMode(this global::Celeste.Player player)
        {
            if (player == null) return;
            
            var data = DynamicData.For(player);
            if (data.Get<bool>("kirby_enabled")) return; // Already enabled
            
            data.Set("kirby_enabled", true);
            
            // Create and attach Kirby component
            var component = new KirbyPlayerComponent();
            data.Set("kirby_component", component);
            player.Add(component);
            
            // Set session flag for persistence across rooms
            if (player.Scene is Level level)
            {
                level.Session.SetFlag("kirby_mode", true);
            }
            
            IngesteLogger.Info("Kirby mode enabled for player");
        }
        
        /// <summary>
        /// Disable Kirby mode for a player
        /// </summary>
        public static void DisableKirbyMode(this global::Celeste.Player player)
        {
            if (player == null) return;
            
            var data = DynamicData.For(player);
            if (!data.Get<bool>("kirby_enabled")) return; // Already disabled
            
            // Remove Kirby component
            var component = data.Get<KirbyPlayerComponent>("kirby_component");
            if (component != null)
            {
                player.Remove(component);
            }
            
            data.Set("kirby_enabled", false);
            data.Set("kirby_component", null);
            
            // Return to normal state
            if (player.StateMachine.State >= KirbyModeHooks.ST_KIRBY_NORMAL)
            {
                player.StateMachine.State = global::Celeste.Player.StNormal;
            }
            
            // Clear session flag
            if (player.Scene is Level level)
            {
                level.Session.SetFlag("kirby_mode", false);
            }
            
            IngesteLogger.Info("Kirby mode disabled for player");
        }
        
        /// <summary>
        /// Get Kirby component for player using DynamicData
        /// </summary>
        public static KirbyPlayerComponent GetKirbyComponent(this global::Celeste.Player player)
        {
            if (player == null) return null;
            return DynamicData.For(player).Get<KirbyPlayerComponent>("kirby_component");
        }
        
        #endregion
        
        #region Kirby DynamicData Accessors
        
        // Power State
        public static KirbyPlayerComponent.PowerState GetKirbyPowerState(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<KirbyPlayerComponent.PowerState>("kirby_power_state");
        }
        
        public static void SetKirbyPowerState(this global::Celeste.Player player, KirbyPlayerComponent.PowerState state)
        {
            DynamicData.For(player).Set("kirby_power_state", state);
        }
        
        // Timers
        public static float GetKirbyInhaleTimer(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<float>("kirby_inhale_timer");
        }
        
        public static void SetKirbyInhaleTimer(this global::Celeste.Player player, float value)
        {
            DynamicData.For(player).Set("kirby_inhale_timer", value);
        }
        
        public static float GetKirbyDashTimer(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<float>("kirby_dash_timer");
        }
        
        public static void SetKirbyDashTimer(this global::Celeste.Player player, float value)
        {
            DynamicData.For(player).Set("kirby_dash_timer", value);
        }
        
        public static float GetKirbyAttackTimer(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<float>("kirby_attack_timer");
        }
        
        public static void SetKirbyAttackTimer(this global::Celeste.Player player, float value)
        {
            DynamicData.For(player).Set("kirby_attack_timer", value);
        }
        
        public static float GetKirbyParryTimer(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<float>("kirby_parry_timer");
        }
        
        public static void SetKirbyParryTimer(this global::Celeste.Player player, float value)
        {
            DynamicData.For(player).Set("kirby_parry_timer", value);
        }
        
        // Dash Direction
        public static Facings GetKirbyDashDirection(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<Facings>("kirby_dash_direction");
        }
        
        public static void SetKirbyDashDirection(this global::Celeste.Player player, Facings value)
        {
            DynamicData.For(player).Set("kirby_dash_direction", value);
        }
        
        // State Flags
        public static bool IsKirbyInhaling(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<bool>("kirby_is_inhaling");
        }
        
        public static void SetKirbyInhaling(this global::Celeste.Player player, bool value)
        {
            DynamicData.For(player).Set("kirby_is_inhaling", value);
        }
        
        public static bool IsKirbyDashing(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<bool>("kirby_is_dashing");
        }
        
        public static void SetKirbyDashing(this global::Celeste.Player player, bool value)
        {
            DynamicData.For(player).Set("kirby_is_dashing", value);
        }
        
        public static bool IsKirbyParrying(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<bool>("kirby_is_parrying");
        }
        
        public static void SetKirbyParrying(this global::Celeste.Player player, bool value)
        {
            DynamicData.For(player).Set("kirby_is_parrying", value);
        }
        
        // Advanced Movement
        public static float GetKirbyWallBounceTimer(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<float>("kirby_wall_bounce_timer");
        }
        
        public static void SetKirbyWallBounceTimer(this global::Celeste.Player player, float value)
        {
            DynamicData.For(player).Set("kirby_wall_bounce_timer", value);
        }
        
        public static int GetKirbyWaveDashCount(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<int>("kirby_wave_dash_count");
        }
        
        public static void SetKirbyWaveDashCount(this global::Celeste.Player player, int value)
        {
            DynamicData.For(player).Set("kirby_wave_dash_count", value);
        }
        
        public static float GetKirbyChargeTime(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<float>("kirby_charge_time");
        }
        
        public static void SetKirbyChargeTime(this global::Celeste.Player player, float value)
        {
            DynamicData.For(player).Set("kirby_charge_time", value);
        }
        
        public static Vector2 GetKirbyChargedDashDirection(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<Vector2>("kirby_charged_dash_direction");
        }
        
        public static void SetKirbyChargedDashDirection(this global::Celeste.Player player, Vector2 value)
        {
            DynamicData.For(player).Set("kirby_charged_dash_direction", value);
        }
        
        // Grab and Slam
        public static Entity GetKirbyGrabbedEntity(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<Entity>("kirby_grabbed_entity");
        }
        
        public static void SetKirbyGrabbedEntity(this global::Celeste.Player player, Entity value)
        {
            DynamicData.For(player).Set("kirby_grabbed_entity", value);
        }
        
        public static float GetKirbyGrabSlamTimer(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<float>("kirby_grab_slam_timer");
        }
        
        public static void SetKirbyGrabSlamTimer(this global::Celeste.Player player, float value)
        {
            DynamicData.For(player).Set("kirby_grab_slam_timer", value);
        }
        
        // Candy Invincibility
        public static bool IsKirbyCandyInvincible(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<bool>("kirby_candy_invincible");
        }
        
        public static void SetKirbyCandyInvincible(this global::Celeste.Player player, bool value)
        {
            DynamicData.For(player).Set("kirby_candy_invincible", value);
        }
        
        public static float GetKirbyCandyTimer(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<float>("kirby_candy_timer");
        }
        
        public static void SetKirbyCandyTimer(this global::Celeste.Player player, float value)
        {
            DynamicData.For(player).Set("kirby_candy_timer", value);
        }
        
        // Star Power
        public static float GetKirbyStarPowerCharge(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<float>("kirby_star_power_charge");
        }
        
        public static void SetKirbyStarPowerCharge(this global::Celeste.Player player, float value)
        {
            DynamicData.For(player).Set("kirby_star_power_charge", value);
        }
        
        // Combat
        public static Vector2 GetKirbyLastAttackDirection(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<Vector2>("kirby_last_attack_direction");
        }
        
        public static void SetKirbyLastAttackDirection(this global::Celeste.Player player, Vector2 value)
        {
            DynamicData.For(player).Set("kirby_last_attack_direction", value);
        }
        
        public static bool GetKirbyParrySuccessful(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<bool>("kirby_parry_successful");
        }
        
        public static void SetKirbyParrySuccessful(this global::Celeste.Player player, bool value)
        {
            DynamicData.For(player).Set("kirby_parry_successful", value);
        }
        
        public static int GetKirbyAttackCombo(this global::Celeste.Player player)
        {
            return DynamicData.For(player).Get<int>("kirby_attack_combo");
        }
        
        public static void SetKirbyAttackCombo(this global::Celeste.Player player, int value)
        {
            DynamicData.For(player).Set("kirby_attack_combo", value);
        }
        
        #endregion
    }
}



