using DesoloZantas.Core.Core.Effects;
using DesoloZantas.Core.Core.Player;
using DesoloZantas.Core.Core.Settings;
using FMOD.Studio;
using MonoMod.Utils;

namespace DesoloZantas.Core.Core.Extensions
{
    /// <summary>
    /// Extensions to add Kirby functionality to the standard Player entity.
    /// 
    /// REFACTORED: Now uses DynamicData pattern inspired by Celeste-Aqua.
    /// State management is now handled by KirbyModeHooks and KirbyStateExtensions.
    /// This file retains the audio constants and KirbyPlayerComponent for sprite/effect handling.
    /// 
    /// See: KirbyModeHooks.cs for hook registration
    /// See: KirbyStateExtensions.cs for state logic as extension methods
    /// See: PlayerExtensions.cs for DynamicData accessors
    /// </summary>
    public static class KirbyPlayerExtensions
    {
        // DEPRECATED: Use KirbyModeHooks.ST_KIRBY_* constants instead
        // These are kept for backwards compatibility
        [Obsolete("Use KirbyModeHooks.ST_KIRBY_NORMAL instead")]
        public const int ST_KIRBY_NORMAL = 27;
        [Obsolete("Use KirbyModeHooks.ST_KIRBY_INHALE instead")]
        public const int ST_KIRBY_INHALE = 28;
        [Obsolete("Use KirbyModeHooks.ST_KIRBY_DASH instead")]
        public const int ST_KIRBY_DASH = 29;
        [Obsolete("Use KirbyModeHooks.ST_KIRBY_ATTACK instead")]
        public const int ST_KIRBY_ATTACK = 30;
        [Obsolete("Use KirbyModeHooks.ST_KIRBY_PARRY instead")]
        public const int ST_KIRBY_PARRY = 31;
        [Obsolete("Use KirbyModeHooks.ST_KIRBY_WALL_BOUNCE instead")]
        public const int ST_KIRBY_WALL_BOUNCE = 32;
        [Obsolete("Use KirbyModeHooks.ST_KIRBY_ULTRA_WAVE_DASH instead")]
        public const int ST_KIRBY_ULTRA_WAVE_DASH = 33;
        [Obsolete("Use KirbyModeHooks.ST_KIRBY_CHARGED_DASH instead")]
        public const int ST_KIRBY_CHARGED_DASH = 34;
        [Obsolete("Use KirbyModeHooks.ST_KIRBY_GRAB_SLAM instead")]
        public const int ST_KIRBY_GRAB_SLAM = 35;
        
        // Kirby Audio Events
        public const string SFX_FOOTSTEP = "event:/char/kirby/footstep";
        public const string SFX_DASH_CHARGE_3D = "event:/char/kirby/dash_charge_3d";
        public const string SFX_SPIT = "event:/Ingeste/char/kirby/spit";
        public const string SFX_INHALE_START = "event:/Ingeste/char/kirby/inhale_start";
        public const string SFX_INHALE_LOOP = "event:/Ingeste/char/kirby/inhale_loop";
        public const string SFX_CHARGE_SNOWGRAVE = "event:/Ingeste/char/kirby/charge_snowgrave";
        public const string SFX_CHARGE_GUNSTAR = "event:/Ingeste/char/kirby/charge_gunstar";
        public const string SFX_CHARGE_DASH_ATTACK = "event:/Ingeste/char/kirby/charge_dash_attack";
        public const string SFX_CHARGE_BEAM = "event:/Ingeste/char/kirby/charge_beam";
        public const string SFX_CANDY_WARNING = "event:/Ingeste/char/kirby/candy_warning";
        public const string SFX_CANDY_START = "event:/Ingeste/char/kirby/candy_start";
        public const string SFX_CANDY_END = "event:/Ingeste/char/kirby/candy_end";
        public const string MUSIC_CANDY = "event:/Ingeste/music/menu/candy";
        
        // Settings instance
        private static KirbySettings Settings => IngesteModule.Settings?.KirbySettings ?? new KirbySettings();
        
        /// <summary>
        /// Enable Kirby mode for a player.
        /// REFACTORED: Now delegates to PlayerExtensions.EnableKirbyMode which uses DynamicData.
        /// </summary>
        [Obsolete("Use PlayerExtensions.EnableKirbyMode instead for new code")]
        public static void EnableKirbyModeLegacy(this global::Celeste.Player player)
        {
            // Delegate to new implementation
            PlayerExtensions.EnableKirbyMode(player);
        }
        
        /// <summary>
        /// Disable Kirby mode for a player.
        /// REFACTORED: Now delegates to PlayerExtensions.DisableKirbyMode which uses DynamicData.
        /// </summary>
        [Obsolete("Use PlayerExtensions.DisableKirbyMode instead for new code")]
        public static void DisableKirbyModeLegacy(this global::Celeste.Player player)
        {
            // Delegate to new implementation
            PlayerExtensions.DisableKirbyMode(player);
        }
        
        /// <summary>
        /// Check if player is in Kirby mode.
        /// REFACTORED: Now delegates to PlayerExtensions.IsKirbyMode which uses DynamicData.
        /// </summary>
        [Obsolete("Use PlayerExtensions.IsKirbyMode instead for new code")]
        public static bool IsKirbyModeLegacy(this global::Celeste.Player player)
        {
            return PlayerExtensions.IsKirbyMode(player);
        }
        
        /// <summary>
        /// Get Kirby component for player.
        /// REFACTORED: Now delegates to PlayerExtensions.GetKirbyComponent which uses DynamicData.
        /// </summary>
        [Obsolete("Use PlayerExtensions.GetKirbyComponent instead for new code")]
        public static KirbyPlayerComponent GetKirbyComponentLegacy(this global::Celeste.Player player)
        {
            return PlayerExtensions.GetKirbyComponent(player);
        }
        
        #region Legacy Kirby State Machine Callbacks
        // NOTE: These legacy callbacks are kept for backwards compatibility.
        // New code should use KirbyStateExtensions which implements states as extension methods on Player.
        // This eliminates the scene tracker lookup overhead and provides better performance.

        [Obsolete("Use KirbyStateExtensions.KirbyNormalUpdate extension method instead")]
        internal static int KirbyNormalUpdate()
        {
            try
            {
                var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
                if (player == null) return KirbyModeHooks.ST_KIRBY_NORMAL;
                
                // Delegate to new extension method implementation
                return player.KirbyNormalUpdate();
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Legacy KirbyNormalUpdate error: {ex.Message}");
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyNormalBegin extension method instead")]
        internal static void KirbyNormalBegin()
        {
            try
            {
                var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
                player?.KirbyNormalBegin();
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Legacy KirbyNormalBegin error: {ex.Message}");
            }
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyNormalEnd extension method instead")]
        internal static void KirbyNormalEnd()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyNormalEnd();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyInhaleUpdate extension method instead")]
        internal static int KirbyInhaleUpdate()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            return player?.KirbyInhaleUpdate() ?? KirbyModeHooks.ST_KIRBY_NORMAL;
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyInhaleBegin extension method instead")]
        internal static void KirbyInhaleBegin()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyInhaleBegin();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyInhaleEnd extension method instead")]
        internal static void KirbyInhaleEnd()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyInhaleEnd();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyDashUpdate extension method instead")]
        internal static int KirbyDashUpdate()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            return player?.KirbyDashUpdate() ?? KirbyModeHooks.ST_KIRBY_NORMAL;
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyDashBegin extension method instead")]
        internal static void KirbyDashBegin()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyDashBegin();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyDashEnd extension method instead")]
        internal static void KirbyDashEnd()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyDashEnd();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyAttackUpdate extension method instead")]
        internal static int KirbyAttackUpdate()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            return player?.KirbyAttackUpdate() ?? KirbyModeHooks.ST_KIRBY_NORMAL;
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyAttackBegin extension method instead")]
        internal static void KirbyAttackBegin()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyAttackBegin();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyAttackEnd extension method instead")]
        internal static void KirbyAttackEnd()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyAttackEnd();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyParryUpdate extension method instead")]
        internal static int KirbyParryUpdate()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            return player?.KirbyParryUpdate() ?? KirbyModeHooks.ST_KIRBY_NORMAL;
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyParryBegin extension method instead")]
        internal static void KirbyParryBegin()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyParryBegin();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyParryEnd extension method instead")]
        internal static void KirbyParryEnd()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyParryEnd();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyWallBounceUpdate extension method instead")]
        internal static int KirbyWallBounceUpdate()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            return player?.KirbyWallBounceUpdate() ?? KirbyModeHooks.ST_KIRBY_NORMAL;
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyWallBounceBegin extension method instead")]
        internal static void KirbyWallBounceBegin()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyWallBounceBegin();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyWallBounceEnd extension method instead")]
        internal static void KirbyWallBounceEnd()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyWallBounceEnd();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyUltraWaveDashUpdate extension method instead")]
        internal static int KirbyUltraWaveDashUpdate()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            return player?.KirbyUltraWaveDashUpdate() ?? KirbyModeHooks.ST_KIRBY_NORMAL;
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyUltraWaveDashBegin extension method instead")]
        internal static void KirbyUltraWaveDashBegin()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyUltraWaveDashBegin();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyUltraWaveDashEnd extension method instead")]
        internal static void KirbyUltraWaveDashEnd()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyUltraWaveDashEnd();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyChargedDashUpdate extension method instead")]
        internal static int KirbyChargedDashUpdate()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            return player?.KirbyChargedDashUpdate() ?? KirbyModeHooks.ST_KIRBY_NORMAL;
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyChargedDashBegin extension method instead")]
        internal static void KirbyChargedDashBegin()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyChargedDashBegin();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyChargedDashEnd extension method instead")]
        internal static void KirbyChargedDashEnd()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyChargedDashEnd();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyGrabSlamUpdate extension method instead")]
        internal static int KirbyGrabSlamUpdate()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            return player?.KirbyGrabSlamUpdate() ?? KirbyModeHooks.ST_KIRBY_NORMAL;
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyGrabSlamBegin extension method instead")]
        internal static void KirbyGrabSlamBegin()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyGrabSlamBegin();
        }
        
        [Obsolete("Use KirbyStateExtensions.KirbyGrabSlamEnd extension method instead")]
        internal static void KirbyGrabSlamEnd()
        {
            var player = Engine.Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            player?.KirbyGrabSlamEnd();
        }
        
        #endregion
    }
    
    #region KirbyPlayerComponent - Simplified for new architecture
    
    /// <summary>
    /// Component that handles Kirby sprite and visual effects.
    /// State management is now handled by DynamicData via PlayerExtensions accessors.
    /// </summary>
    public class KirbyPlayerComponent : Component
    {
        public enum PowerState
        {
            None,
            Fire,
            Ice,
            Spark,
            Stone,
            Sword
        }
        
        // Map power states to element types
        private static readonly Dictionary<PowerState, ElementType> PowerToElement = new Dictionary<PowerState, ElementType>
        {
            { PowerState.Fire, ElementType.Fire },
            { PowerState.Ice, ElementType.Ice },
            { PowerState.Spark, ElementType.Lightning },
            { PowerState.Stone, ElementType.Earth },
            { PowerState.Sword, ElementType.Light }
        };
        
        // Kirby-specific sprite
        public Sprite KirbySprite { get; private set; }
        private global::Celeste.Player player;
        
        // CurrentPower property for backwards compatibility - delegates to PlayerExtensions DynamicData
        public PowerState CurrentPower
        {
            get => player?.GetKirbyPowerState() ?? PowerState.None;
            set => player?.SetKirbyPowerState(value);
        }
        
        // Candy invincibility
        private const float CANDY_DURATION = 16f;
        private const float CANDY_WARNING_TIME = 2f;
        private EventInstance candyMusicInstance;
        
        public KirbyPlayerComponent() : base(true, false) { }
        
        public override void Added(Entity entity)
        {
            base.Added(entity);
            player = entity as global::Celeste.Player;
            
            if (player == null)
            {
                IngesteLogger.Error("KirbyPlayerComponent added to non-Player entity");
                return;
            }
            
            InitializeKirbySprite();
        }
        
        private void InitializeKirbySprite()
        {
            try
            {
                if (GFX.SpriteBank == null)
                {
                    IngesteLogger.Error("GFX.SpriteBank is null, cannot create Kirby sprite");
                    KirbySprite = player.Sprite;
                    return;
                }
                
                KirbySprite = GFX.SpriteBank.Create("kirby_player");
                
                if (KirbySprite == null)
                {
                    IngesteLogger.Error("Failed to create kirby_player sprite, using player sprite as fallback");
                    KirbySprite = player.Sprite;
                    return;
                }
                
                KirbySprite.Play("idle");
                
                if (player.Sprite != null)
                {
                    player.Sprite.Visible = false;
                }
                
                if (KirbySprite != player.Sprite)
                {
                    Entity.Add(KirbySprite);
                }
                
                IngesteLogger.Info("Kirby sprite initialized successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Failed to create Kirby sprite: {ex.Message}");
                KirbySprite = player?.Sprite;
                if (player?.Sprite != null)
                {
                    player.Sprite.Visible = true;
                }
            }
        }
        
        public override void Removed(Entity entity)
        {
            try
            {
                if (player?.Sprite != null)
                {
                    player.Sprite.Visible = true;
                }
                
                if (KirbySprite != null && KirbySprite != player?.Sprite && entity != null)
                {
                    entity.Remove(KirbySprite);
                }
                
                KirbySprite = null;
                player = null;
                
                base.Removed(entity);
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Error removing KirbyPlayerComponent: {ex.Message}");
                base.Removed(entity);
            }
        }
        
        public override void Update()
        {
            base.Update();
            UpdateSpriteFlip();
        }
        
        public void UpdateSpriteFlip()
        {
            if (KirbySprite != null && player != null)
            {
                KirbySprite.Scale.X = Math.Abs(KirbySprite.Scale.X) * (int)player.Facing;
            }
        }
        
        public void PlayAnimation(string animationName, bool restart = false)
        {
            try
            {
                if (KirbySprite != null && !string.IsNullOrEmpty(animationName))
                {
                    KirbySprite.Play(animationName, restart);
                    UpdateSpriteFlip();
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Failed to play animation '{animationName}': {ex.Message}");
                try
                {
                    KirbySprite?.Play("idle", restart);
                    UpdateSpriteFlip();
                }
                catch { }
            }
        }
        
        public string GetCurrentAnimation()
        {
            try
            {
                return KirbySprite?.CurrentAnimationID ?? "idle";
            }
            catch
            {
                return "idle";
            }
        }
        
        public void SetPowerState(PowerState newPower)
        {
            if (player == null) return;
            
            var currentPower = player.GetKirbyPowerState();
            if (currentPower == newPower) return;
            
            player.SetKirbyPowerState(newPower);
            
            string animationName = newPower switch
            {
                PowerState.Fire => "fire_idle",
                PowerState.Ice => "ice_idle",
                PowerState.Spark => "spark_idle",
                PowerState.Stone => "stone_idle",
                PowerState.Sword => "sword_idle",
                _ => "idle"
            };
            
            PlayAnimation(animationName, true);
            
            if (newPower != PowerState.None && PowerToElement.ContainsKey(newPower) && Scene is Level level)
            {
                ElementalEffectsManager.PlayElementalEffect(PowerToElement[newPower], level, player.Position);
            }
        }
        
        public bool ShouldParry()
        {
            if (!(Scene is Level level) || player == null) return false;
            
            var nearbyEnemies = level.Tracker.GetEntities<Actor>()
                .Where(e => e != player && Vector2.Distance(e.Position, player.Position) < 64f)
                .Any();
            
            float lastParryTime = player.GetKirbyParryTimer();
            bool recentThreat = (Engine.Scene.TimeActive - lastParryTime) > 0.5f;
            
            return nearbyEnemies && recentThreat;
        }
        
        public void PerformElementalAttack(global::Celeste.Player player)
        {
            var currentPower = player.GetKirbyPowerState();
            if (currentPower == PowerState.None || !(Scene is Level level)) return;
            
            Vector2 attackDirection = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
            if (attackDirection == Vector2.Zero)
                attackDirection = Vector2.UnitX;
            
            var parameters = EffectParams.WithDirection(attackDirection);
            
            switch (currentPower)
            {
                case PowerState.Fire:
                    Audio.Play(KirbyPlayerExtensions.SFX_CHARGE_GUNSTAR, player.Position);
                    ElementalEffectsManager.PlayEffect("fire_burst", level, player.Position, parameters);
                    break;
                case PowerState.Ice:
                    Audio.Play(KirbyPlayerExtensions.SFX_CHARGE_SNOWGRAVE, player.Position);
                    ElementalEffectsManager.PlayEffect("ice_burst", level, player.Position, parameters);
                    break;
                case PowerState.Spark:
                    Audio.Play(KirbyPlayerExtensions.SFX_CHARGE_BEAM, player.Position);
                    ElementalEffectsManager.PlayEffect("chain_lightning", level, player.Position, parameters);
                    break;
                case PowerState.Stone:
                    ElementalEffectsManager.PlayEffect("earth_spike", level, player.Position, parameters);
                    break;
                case PowerState.Sword:
                    Audio.Play(KirbyPlayerExtensions.SFX_CHARGE_BEAM, player.Position);
                    ElementalEffectsManager.PlayEffect("light_beam", level, player.Position,
                        EffectParams.WithEnd(player.Position + attackDirection * 48f));
                    break;
            }
        }
        
        public void UpdateStarWarriorAbilities(global::Celeste.Player player)
        {
            float charge = player.GetKirbyStarPowerCharge();
            charge = Math.Min(charge + 10f * Engine.DeltaTime, 100f);
            player.SetKirbyStarPowerCharge(charge);
            
            if (charge >= 100f && Input.Dash.Pressed && Input.Jump.Check)
            {
                PerformStarPowerAttack(player);
                player.SetKirbyStarPowerCharge(0f);
            }
        }
        
        private void PerformStarPowerAttack(global::Celeste.Player player)
        {
            if (!(Scene is Level level)) return;
            
            Vector2 attackDirection = new Vector2((int)player.Facing, 0);
            
            for (int i = -1; i <= 1; i++)
            {
                Vector2 starDirection = Vector2.Transform(attackDirection, Matrix.CreateRotationZ(i * 0.3f));
                Vector2 starPos = player.Position + starDirection * 16f;
                
                ElementalEffectsManager.PlayEffect("light_beam", level, starPos,
                    EffectParams.WithEnd(starPos + starDirection * 128f));
            }
        }
        
        public void StartCandyInvincibility()
        {
            if (player == null) return;
            if (player.IsKirbyCandyInvincible()) return;
            
            player.SetKirbyCandyInvincible(true);
            player.SetKirbyCandyTimer(CANDY_DURATION);
            
            Audio.Play(KirbyPlayerExtensions.SFX_CANDY_START, player.Position);
            candyMusicInstance = Audio.Play(KirbyPlayerExtensions.MUSIC_CANDY);
        }
        
        public void EndCandyInvincibility()
        {
            if (player == null) return;
            if (!player.IsKirbyCandyInvincible()) return;
            
            player.SetKirbyCandyInvincible(false);
            player.SetKirbyCandyTimer(0f);
            
            Audio.Play(KirbyPlayerExtensions.SFX_CANDY_END, player.Position);
            
            if (candyMusicInstance.isValid())
            {
                candyMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                candyMusicInstance.release();
            }
        }
    }
    
    #endregion
    
    #region Enhanced K_Player Extension with 3, 4, 5, 10 Dashes
    
    /// <summary>
    /// Enhanced Kirby Player Extension supporting extended dash counts (3, 4, 5, 10)
    /// and full character stats integration (HP/DEF, ATK, SPD)
    /// </summary>
    public static class KirbyPlayerEnhancedExtension
    {
        /// <summary>
        /// Initialize player with full Kirby character stats and dash tier
        /// </summary>
        public static void InitializeKirbyPlayer(this global::Celeste.Player player, 
            Player.KirbyDashExtensions.DashTier dashTier = Player.KirbyDashExtensions.DashTier.Triple)
        {
            if (player == null) return;
            
            // Initialize character stats
            var stats = Player.DashLevelStats.GetStatsForDashCount((int)dashTier);
            player.SetCharacterStats(stats);
            
            // Set dash tier
            player.SetDashTier(dashTier);
            
            // Enable Kirby mode
            player.EnableKirbyMode();
            
            // Initialize playable character system
            Player.PlayableCharacterSystem.Initialize();
            Player.PlayableCharacterSystem.SetCurrentCharacter(Player.PlayableCharacterId.Kirby);
            Player.PlayableCharacterSystem.ApplyToPlayer(player);
            
            IngesteLogger.Info($"K_Player initialized with {(int)dashTier} dashes");
        }
        
        /// <summary>
        /// Set player to specific dash count (3, 4, 5, or 10)
        /// </summary>
        public static void SetExtendedDashCount(this global::Celeste.Player player, int dashCount)
        {
            if (player == null) return;
            
            // Validate and clamp to supported values
            int validDashCount = dashCount switch
            {
                <= 1 => 1,
                2 => 2,
                3 => 3,
                4 => 4,
                5 => 5,
                >= 10 => 10,
                _ => dashCount
            };
            
            player.SetDashLevel(validDashCount);
            IngesteLogger.Info($"Player dash count set to {validDashCount}");
        }
        
        /// <summary>
        /// Set fighting stats (HP/Defense, ATK/Offense, SPD/Speed)
        /// </summary>
        public static void SetFightingStats(this global::Celeste.Player player, 
            int hp, float defense, float attack, float speed)
        {
            if (player == null) return;
            
            var stats = player.GetCharacterStats();
            stats.MaxHP = hp;
            stats.CurrentHP = hp;
            stats.Defense = defense;
            stats.Attack = attack;
            stats.Speed = speed;
            player.SetCharacterStats(stats);
            
            IngesteLogger.Info($"Fighting stats set: HP={hp}, DEF={defense}, ATK={attack}, SPD={speed}");
        }
        
        /// <summary>
        /// Get HP percentage for UI display
        /// </summary>
        public static float GetHPPercentage(this global::Celeste.Player player)
        {
            if (player == null) return 1f;
            var stats = player.GetCharacterStats();
            return (float)stats.CurrentHP / stats.MaxHP;
        }
        
        /// <summary>
        /// Get current defense rating
        /// </summary>
        public static float GetDefenseRating(this global::Celeste.Player player)
        {
            return player?.GetCharacterStats()?.Defense ?? 1f;
        }
        
        /// <summary>
        /// Get current attack rating
        /// </summary>
        public static float GetAttackRating(this global::Celeste.Player player)
        {
            return player?.GetCharacterStats()?.Attack ?? 1f;
        }
        
        /// <summary>
        /// Get current speed rating
        /// </summary>
        public static float GetSpeedRating(this global::Celeste.Player player)
        {
            return player?.GetCharacterStats()?.Speed ?? 1f;
        }
        
        /// <summary>
        /// Apply playable character configuration
        /// </summary>
        public static void SetPlayableCharacter(this global::Celeste.Player player, 
            Player.PlayableCharacterId characterId, string skinId = "default")
        {
            if (player == null) return;
            
            player.SetPlayableCharacterId(characterId);
            player.SetCharacterSkinId(skinId);
            
            IngesteLogger.Info($"Playable character set: {characterId} with skin {skinId}");
        }
        
        /// <summary>
        /// Apply "self" skin (uses player's configured skin settings)
        /// </summary>
        public static void ApplySelfSkin(this global::Celeste.Player player)
        {
            if (player == null) return;
            
            var currentCharacter = player.GetPlayableCharacterId();
            player.SetCharacterSkinId("self");
            
            IngesteLogger.Info($"Applied self skin for character {currentCharacter}");
        }
        
        /// <summary>
        /// Check if player is using extended dashes (3+)
        /// </summary>
        public static bool HasExtendedDashes(this global::Celeste.Player player)
        {
            if (player == null) return false;
            var stats = player.GetCharacterStats();
            return stats.MaxDashes >= 3;
        }
        
        /// <summary>
        /// Check if player is at full HP
        /// </summary>
        public static bool IsAtFullHP(this global::Celeste.Player player)
        {
            if (player == null) return true;
            var stats = player.GetCharacterStats();
            return stats.CurrentHP >= stats.MaxHP;
        }
        
        /// <summary>
        /// Deal combat damage using stats
        /// </summary>
        public static int DealDamage(this global::Celeste.Player player, int baseDamage)
        {
            if (player == null) return baseDamage;
            var stats = player.GetCharacterStats();
            return stats.CalculateDamageDealt(baseDamage);
        }
        
        /// <summary>
        /// Receive combat damage using stats
        /// </summary>
        public static bool ReceiveDamage(this global::Celeste.Player player, int baseDamage)
        {
            if (player == null) return true;
            return player.ApplyDamageWithStats(baseDamage);
        }
        
        /// <summary>
        /// Full character reset (stats, dashes, HP)
        /// </summary>
        public static void FullCharacterReset(this global::Celeste.Player player)
        {
            if (player == null) return;
            
            var stats = player.GetCharacterStats();
            stats.FullHeal();
            player.SetCharacterStats(stats);
            player.Dashes = stats.CurrentDashes;
            
            IngesteLogger.Info("Full character reset performed");
        }
    }
    
    #endregion
}
