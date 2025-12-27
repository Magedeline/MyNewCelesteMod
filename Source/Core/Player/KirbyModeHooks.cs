using DesoloZantas.Core.Core.Extensions;
using DesoloZantas.Core.Core.Settings;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Mono.Cecil.Cil;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Centralized hook management for Kirby mode, inspired by Celeste-Aqua's architecture.
    /// Provides Initialize/Uninitialize pattern for clean mod loading/unloading.
    /// 
    /// When KirbyPlayerMode setting is enabled, Kirby automatically becomes the playable character.
    /// </summary>
    public static class KirbyModeHooks
    {
        private static bool _initialized = false;
        
        /// <summary>
        /// Returns true if Kirby should be the playable character based on settings
        /// </summary>
        public static bool IsKirbyPlayableCharacter => IngesteModule.Settings?.KirbyPlayerMode ?? false;
        
        // Kirby state indices (added to player's StateMachine)
        public const int ST_KIRBY_NORMAL = 27;
        public const int ST_KIRBY_INHALE = 28;
        public const int ST_KIRBY_DASH = 29;
        public const int ST_KIRBY_ATTACK = 30;
        public const int ST_KIRBY_PARRY = 31;
        public const int ST_KIRBY_WALL_BOUNCE = 32;
        public const int ST_KIRBY_ULTRA_WAVE_DASH = 33;
        public const int ST_KIRBY_CHARGED_DASH = 34;
        public const int ST_KIRBY_GRAB_SLAM = 35;
        public const int ST_KIRBY_MAX = 36;
        
        // Number of additional states we need
        private const int ADDITIONAL_STATES = ST_KIRBY_MAX - 26;
        
        /// <summary>
        /// Initialize all Kirby mode hooks. Call this during module Load().
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;
            
            try
            {
                // IL hooks to expand player's StateMachine
                IL.Celeste.Player.ctor += Player_ILConstruct;
                
                // On hooks for player lifecycle
                On.Celeste.Player.ctor += Player_Construct;
                On.Celeste.Player.Added += Player_Added;
                On.Celeste.Player.Removed += Player_Removed;
                On.Celeste.Player.Update += Player_Update;
                
                // State-specific hooks for Kirby mode integration
                On.Celeste.Player.NormalUpdate += Player_NormalUpdate;
                On.Celeste.Player.DashBegin += Player_DashBegin;
                On.Celeste.Player.DashUpdate += Player_DashUpdate;
                On.Celeste.Player.ClimbUpdate += Player_ClimbUpdate;
                
                // Hook level load to auto-enable Kirby when KirbyPlayerMode setting is enabled
                Everest.Events.Level.OnLoadLevel += Level_OnLoadLevel;
                
                _initialized = true;
                IngesteLogger.Info("KirbyModeHooks initialized successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Failed to initialize KirbyModeHooks: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Uninitialize all Kirby mode hooks. Call this during module Unload().
        /// </summary>
        public static void Uninitialize()
        {
            if (!_initialized) return;
            
            try
            {
                // Remove IL hooks
                IL.Celeste.Player.ctor -= Player_ILConstruct;
                
                // Remove On hooks
                On.Celeste.Player.ctor -= Player_Construct;
                On.Celeste.Player.Added -= Player_Added;
                On.Celeste.Player.Removed -= Player_Removed;
                On.Celeste.Player.Update -= Player_Update;
                
                On.Celeste.Player.NormalUpdate -= Player_NormalUpdate;
                On.Celeste.Player.DashBegin -= Player_DashBegin;
                On.Celeste.Player.DashUpdate -= Player_DashUpdate;
                On.Celeste.Player.ClimbUpdate -= Player_ClimbUpdate;
                
                // Remove level load hook
                Everest.Events.Level.OnLoadLevel -= Level_OnLoadLevel;
                
                _initialized = false;
                IngesteLogger.Info("KirbyModeHooks uninitialized successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Error during KirbyModeHooks uninitialize: {ex.Message}");
            }
        }
        
        #region IL Hooks
        
        /// <summary>
        /// IL hook to expand the player's StateMachine to accommodate Kirby states.
        /// Similar to Celeste-Aqua's approach.
        /// </summary>
        private static void Player_ILConstruct(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);
            
            // Find where StateMachine is created with 26 states (Celeste's default)
            // and add our additional Kirby states
            if (cursor.TryGotoNext(MoveType.After, ins => ins.MatchLdcI4(26)))
            {
                // Add additional states for Kirby mode
                cursor.Emit(OpCodes.Ldc_I4, ADDITIONAL_STATES);
                cursor.Emit(OpCodes.Add);
                
                IngesteLogger.Info($"Expanded player StateMachine by {ADDITIONAL_STATES} states for Kirby mode");
            }
            else
            {
                IngesteLogger.Warn("Could not find StateMachine size in Player constructor");
            }
        }
        
        #endregion
        
        #region Player Lifecycle Hooks
        
        /// <summary>
        /// Hook into Player constructor to register Kirby state callbacks
        /// </summary>
        private static void Player_Construct(On.Celeste.Player.orig_ctor orig, global::Celeste.Player self, Vector2 position, global::Celeste.PlayerSpriteMode spriteMode)
        {
            orig(self, position, spriteMode);
            
            // Initialize Kirby data storage via DynamicData
            self.InitializeKirbyData();
            
            // Register Kirby state callbacks on player's StateMachine
            RegisterKirbyStateCallbacks(self);
        }
        
        /// <summary>
        /// Register all Kirby state callbacks on the player's StateMachine
        /// </summary>
        private static void RegisterKirbyStateCallbacks(global::Celeste.Player player)
        {
            var sm = player.StateMachine;
            
            // Normal (Kirby-style movement)
            sm.SetCallbacks(ST_KIRBY_NORMAL, 
                () => player.KirbyNormalUpdate(), 
                null, 
                () => player.KirbyNormalBegin(), 
                () => player.KirbyNormalEnd());
            sm.SetStateName(ST_KIRBY_NORMAL, "KirbyNormal");
            
            // Inhale
            sm.SetCallbacks(ST_KIRBY_INHALE, 
                () => player.KirbyInhaleUpdate(), 
                null, 
                () => player.KirbyInhaleBegin(), 
                () => player.KirbyInhaleEnd());
            sm.SetStateName(ST_KIRBY_INHALE, "KirbyInhale");
            
            // Dash
            sm.SetCallbacks(ST_KIRBY_DASH, 
                () => player.KirbyDashUpdate(), 
                null, 
                () => player.KirbyDashBegin(), 
                () => player.KirbyDashEnd());
            sm.SetStateName(ST_KIRBY_DASH, "KirbyDash");
            
            // Attack
            sm.SetCallbacks(ST_KIRBY_ATTACK, 
                () => player.KirbyAttackUpdate(), 
                null, 
                () => player.KirbyAttackBegin(), 
                () => player.KirbyAttackEnd());
            sm.SetStateName(ST_KIRBY_ATTACK, "KirbyAttack");
            
            // Parry
            sm.SetCallbacks(ST_KIRBY_PARRY, 
                () => player.KirbyParryUpdate(), 
                null, 
                () => player.KirbyParryBegin(), 
                () => player.KirbyParryEnd());
            sm.SetStateName(ST_KIRBY_PARRY, "KirbyParry");
            
            // Wall Bounce
            sm.SetCallbacks(ST_KIRBY_WALL_BOUNCE, 
                () => player.KirbyWallBounceUpdate(), 
                null, 
                () => player.KirbyWallBounceBegin(), 
                () => player.KirbyWallBounceEnd());
            sm.SetStateName(ST_KIRBY_WALL_BOUNCE, "KirbyWallBounce");
            
            // Ultra Wave Dash
            sm.SetCallbacks(ST_KIRBY_ULTRA_WAVE_DASH, 
                () => player.KirbyUltraWaveDashUpdate(), 
                null, 
                () => player.KirbyUltraWaveDashBegin(), 
                () => player.KirbyUltraWaveDashEnd());
            sm.SetStateName(ST_KIRBY_ULTRA_WAVE_DASH, "KirbyUltraWaveDash");
            
            // Charged Dash
            sm.SetCallbacks(ST_KIRBY_CHARGED_DASH, 
                () => player.KirbyChargedDashUpdate(), 
                null, 
                () => player.KirbyChargedDashBegin(), 
                () => player.KirbyChargedDashEnd());
            sm.SetStateName(ST_KIRBY_CHARGED_DASH, "KirbyChargedDash");
            
            // Grab Slam
            sm.SetCallbacks(ST_KIRBY_GRAB_SLAM, 
                () => player.KirbyGrabSlamUpdate(), 
                null, 
                () => player.KirbyGrabSlamBegin(), 
                () => player.KirbyGrabSlamEnd());
            sm.SetStateName(ST_KIRBY_GRAB_SLAM, "KirbyGrabSlam");
        }
        
        /// <summary>
        /// Hook when level loads - auto-enable Kirby mode if KirbyPlayerMode setting is enabled
        /// </summary>
        private static void Level_OnLoadLevel(Level level, global::Celeste.Player.IntroTypes intro, bool isFromLoader)
        {
            // If KirbyPlayerMode setting is enabled, set the session flag so Kirby becomes the playable character
            if (IsKirbyPlayableCharacter)
            {
                level.Session.SetFlag("kirby_mode", true);
                IngesteLogger.Info("KirbyPlayerMode enabled - Kirby is the playable character");
            }
        }
        
        /// <summary>
        /// Hook when player is added to scene
        /// </summary>
        private static void Player_Added(On.Celeste.Player.orig_Added orig, global::Celeste.Player self, Scene scene)
        {
            orig(self, scene);
            
            // Check if Kirby mode should be enabled from session flag or setting
            if (scene is Level level)
            {
                // Auto-enable Kirby mode if KirbyPlayerMode setting is enabled
                if (IsKirbyPlayableCharacter && !level.Session.GetFlag("kirby_mode"))
                {
                    level.Session.SetFlag("kirby_mode", true);
                }
                
                // Enable Kirby mode if session flag is set
                if (level.Session.GetFlag("kirby_mode"))
                {
                    self.EnableKirbyMode();
                }
            }
        }
        
        /// <summary>
        /// Hook when player is removed from scene
        /// </summary>
        private static void Player_Removed(On.Celeste.Player.orig_Removed orig, global::Celeste.Player self, Scene scene)
        {
            // Clean up Kirby mode if active
            if (self.IsKirbyMode())
            {
                self.DisableKirbyMode();
            }
            
            orig(self, scene);
        }
        
        #endregion
        
        #region State Update Hooks
        
        /// <summary>
        /// Hook into Player.Update for Kirby-specific per-frame logic
        /// </summary>
        private static void Player_Update(On.Celeste.Player.orig_Update orig, global::Celeste.Player self)
        {
            // Pre-update Kirby logic
            if (self.IsKirbyMode())
            {
                self.PreKirbyUpdate();
            }
            
            orig(self);
            
            // Post-update Kirby logic
            if (self.IsKirbyMode())
            {
                self.PostKirbyUpdate();
            }
        }
        
        /// <summary>
        /// Hook NormalUpdate to redirect to Kirby normal when in Kirby mode
        /// </summary>
        private static int Player_NormalUpdate(On.Celeste.Player.orig_NormalUpdate orig, global::Celeste.Player self)
        {
            if (self.IsKirbyMode())
            {
                // Use Kirby's normal update instead
                return self.KirbyNormalUpdate();
            }
            
            return orig(self);
        }
        
        /// <summary>
        /// Hook DashBegin for Kirby-specific dash behavior
        /// </summary>
        private static void Player_DashBegin(On.Celeste.Player.orig_DashBegin orig, global::Celeste.Player self)
        {
            if (self.IsKirbyMode())
            {
                // Kirby has custom dash - transition to Kirby dash state
                self.StateMachine.State = ST_KIRBY_DASH;
                return;
            }
            
            orig(self);
        }
        
        /// <summary>
        /// Hook DashUpdate for Kirby mode
        /// </summary>
        private static int Player_DashUpdate(On.Celeste.Player.orig_DashUpdate orig, global::Celeste.Player self)
        {
            if (self.IsKirbyMode())
            {
                return self.KirbyDashUpdate();
            }
            
            return orig(self);
        }
        
        /// <summary>
        /// Hook ClimbUpdate for Kirby wall interactions
        /// </summary>
        private static int Player_ClimbUpdate(On.Celeste.Player.orig_ClimbUpdate orig, global::Celeste.Player self)
        {
            if (self.IsKirbyMode())
            {
                // Kirby doesn't climb - check for wall bounce instead
                var settings = IngesteModule.Settings?.KirbySettings;
                if (settings != null && settings.IsKeyPressed("Attack"))
                {
                    return ST_KIRBY_WALL_BOUNCE;
                }
                
                // Fall off wall
                return global::Celeste.Player.StNormal;
            }
            
            return orig(self);
        }
        
        #endregion
    }
}
