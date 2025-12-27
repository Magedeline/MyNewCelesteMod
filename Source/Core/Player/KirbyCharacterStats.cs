using System;
using System.Collections.Generic;
using MonoMod.Utils;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Character combat statistics for Kirby and other playable characters.
    /// Includes HP (Defense), ATK (Offense), and SPD (Speed) stats.
    /// </summary>
    public class KirbyCharacterStats
    {
        #region Base Stats
        
        /// <summary>
        /// Maximum health points (HP) - determines how much damage the character can take
        /// </summary>
        public int MaxHP { get; set; }
        
        /// <summary>
        /// Current health points
        /// </summary>
        public int CurrentHP { get; set; }
        
        /// <summary>
        /// Defense rating - reduces incoming damage
        /// Formula: ActualDamage = BaseDamage - (Defense * DefenseReduction)
        /// </summary>
        public float Defense { get; set; }
        
        /// <summary>
        /// Attack rating - increases outgoing damage
        /// Formula: ActualDamage = BaseDamage * (1 + Attack * AttackMultiplier)
        /// </summary>
        public float Attack { get; set; }
        
        /// <summary>
        /// Speed rating - affects movement speed and action cooldowns
        /// </summary>
        public float Speed { get; set; }
        
        /// <summary>
        /// Maximum number of dashes available
        /// Extended values: 1, 2, 3, 4, 5, or 10
        /// </summary>
        public int MaxDashes { get; set; }
        
        /// <summary>
        /// Current dash count
        /// </summary>
        public int CurrentDashes { get; set; }
        
        /// <summary>
        /// Stamina for climbing and special abilities
        /// </summary>
        public float MaxStamina { get; set; }
        
        /// <summary>
        /// Current stamina
        /// </summary>
        public float CurrentStamina { get; set; }
        
        #endregion
        
        #region Stat Modifiers
        
        /// <summary>
        /// Multiplier for defense calculations
        /// </summary>
        public const float DefenseReductionPerPoint = 0.05f;
        
        /// <summary>
        /// Multiplier for attack calculations
        /// </summary>
        public const float AttackMultiplierPerPoint = 0.1f;
        
        /// <summary>
        /// Multiplier for speed calculations
        /// </summary>
        public const float SpeedMultiplierPerPoint = 0.05f;
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// Create default stats
        /// </summary>
        public KirbyCharacterStats()
        {
            MaxHP = 100;
            CurrentHP = 100;
            Defense = 1.0f;
            Attack = 1.0f;
            Speed = 1.0f;
            MaxDashes = 1;
            CurrentDashes = 1;
            MaxStamina = 110f; // Celeste default
            CurrentStamina = 110f;
        }
        
        /// <summary>
        /// Create stats with custom values
        /// </summary>
        public KirbyCharacterStats(int maxHP, float defense, float attack, float speed, int maxDashes)
        {
            MaxHP = maxHP;
            CurrentHP = maxHP;
            Defense = defense;
            Attack = attack;
            Speed = speed;
            MaxDashes = maxDashes;
            CurrentDashes = maxDashes;
            MaxStamina = 110f;
            CurrentStamina = 110f;
        }
        
        /// <summary>
        /// Copy constructor
        /// </summary>
        public KirbyCharacterStats(KirbyCharacterStats other)
        {
            MaxHP = other.MaxHP;
            CurrentHP = other.CurrentHP;
            Defense = other.Defense;
            Attack = other.Attack;
            Speed = other.Speed;
            MaxDashes = other.MaxDashes;
            CurrentDashes = other.CurrentDashes;
            MaxStamina = other.MaxStamina;
            CurrentStamina = other.CurrentStamina;
        }
        
        #endregion
        
        #region Damage Calculations
        
        /// <summary>
        /// Calculate actual damage taken after defense reduction
        /// </summary>
        public int CalculateDamageTaken(int baseDamage)
        {
            float reduction = Defense * DefenseReductionPerPoint;
            float actualDamage = baseDamage * (1.0f - Math.Min(reduction, 0.9f)); // Cap at 90% reduction
            return Math.Max(1, (int)Math.Ceiling(actualDamage)); // Minimum 1 damage
        }
        
        /// <summary>
        /// Calculate actual damage dealt after attack bonus
        /// </summary>
        public int CalculateDamageDealt(int baseDamage)
        {
            float bonus = Attack * AttackMultiplierPerPoint;
            float actualDamage = baseDamage * (1.0f + bonus);
            return (int)Math.Floor(actualDamage);
        }
        
        /// <summary>
        /// Apply damage to character
        /// </summary>
        /// <returns>True if character is still alive</returns>
        public bool TakeDamage(int damage)
        {
            int actualDamage = CalculateDamageTaken(damage);
            CurrentHP = Math.Max(0, CurrentHP - actualDamage);
            return CurrentHP > 0;
        }
        
        /// <summary>
        /// Heal the character
        /// </summary>
        public void Heal(int amount)
        {
            CurrentHP = Math.Min(MaxHP, CurrentHP + amount);
        }
        
        /// <summary>
        /// Fully heal the character
        /// </summary>
        public void FullHeal()
        {
            CurrentHP = MaxHP;
            CurrentStamina = MaxStamina;
            CurrentDashes = MaxDashes;
        }
        
        #endregion
        
        #region Speed Calculations
        
        /// <summary>
        /// Get movement speed multiplier
        /// </summary>
        public float GetSpeedMultiplier()
        {
            return 1.0f + (Speed - 1.0f) * SpeedMultiplierPerPoint;
        }
        
        /// <summary>
        /// Get dash speed multiplier
        /// </summary>
        public float GetDashSpeedMultiplier()
        {
            return 1.0f + (Speed - 1.0f) * SpeedMultiplierPerPoint * 0.5f; // Half effect on dash
        }
        
        /// <summary>
        /// Get cooldown reduction (lower = faster cooldowns)
        /// </summary>
        public float GetCooldownMultiplier()
        {
            return 1.0f / (1.0f + (Speed - 1.0f) * SpeedMultiplierPerPoint);
        }
        
        #endregion
        
        #region Dash Management
        
        /// <summary>
        /// Use a dash if available
        /// </summary>
        /// <returns>True if dash was used</returns>
        public bool UseDash()
        {
            if (CurrentDashes > 0)
            {
                CurrentDashes--;
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Refill dashes (e.g., when touching ground)
        /// </summary>
        public void RefillDashes()
        {
            CurrentDashes = MaxDashes;
        }
        
        /// <summary>
        /// Add extra dashes
        /// </summary>
        public void AddExtraDashes(int count)
        {
            CurrentDashes = Math.Min(MaxDashes + count, CurrentDashes + count);
        }
        
        #endregion
        
        #region Stamina Management
        
        /// <summary>
        /// Use stamina for an action
        /// </summary>
        /// <returns>True if stamina was available</returns>
        public bool UseStamina(float amount)
        {
            if (CurrentStamina >= amount)
            {
                CurrentStamina -= amount;
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Recover stamina
        /// </summary>
        public void RecoverStamina(float amount)
        {
            CurrentStamina = Math.Min(MaxStamina, CurrentStamina + amount);
        }
        
        /// <summary>
        /// Check if tired (low stamina)
        /// </summary>
        public bool IsTired => CurrentStamina < 20f;
        
        /// <summary>
        /// Check if dead
        /// </summary>
        public bool IsDead => CurrentHP <= 0;
        
        #endregion
    }
    
    /// <summary>
    /// Predefined stat configurations for different dash levels
    /// </summary>
    public static class DashLevelStats
    {
        /// <summary>
        /// Standard 1 dash configuration (Celeste default)
        /// </summary>
        public static KirbyCharacterStats OneDash => new KirbyCharacterStats(100, 1.0f, 1.0f, 1.0f, 1);
        
        /// <summary>
        /// 2 dash configuration (standard power-up)
        /// </summary>
        public static KirbyCharacterStats TwoDash => new KirbyCharacterStats(100, 1.0f, 1.0f, 1.0f, 2);
        
        /// <summary>
        /// 3 dash configuration (enhanced mobility)
        /// </summary>
        public static KirbyCharacterStats ThreeDash => new KirbyCharacterStats(100, 0.9f, 1.1f, 1.2f, 3);
        
        /// <summary>
        /// 4 dash configuration (advanced mobility)
        /// </summary>
        public static KirbyCharacterStats FourDash => new KirbyCharacterStats(110, 0.85f, 1.2f, 1.3f, 4);
        
        /// <summary>
        /// 5 dash configuration (expert mobility)
        /// </summary>
        public static KirbyCharacterStats FiveDash => new KirbyCharacterStats(120, 0.8f, 1.3f, 1.4f, 5);
        
        /// <summary>
        /// 10 dash configuration (ultra mobility / Kirby Star Power)
        /// </summary>
        public static KirbyCharacterStats TenDash => new KirbyCharacterStats(150, 0.7f, 1.5f, 1.6f, 10);
        
        /// <summary>
        /// Get stats for a specific dash count
        /// </summary>
        public static KirbyCharacterStats GetStatsForDashCount(int dashCount)
        {
            return dashCount switch
            {
                1 => OneDash,
                2 => TwoDash,
                3 => ThreeDash,
                4 => FourDash,
                5 => FiveDash,
                10 => TenDash,
                _ => new KirbyCharacterStats(100, 1.0f, 1.0f, 1.0f, Math.Max(1, dashCount))
            };
        }
    }
    
    /// <summary>
    /// Extension methods for applying character stats to players
    /// </summary>
    public static class CharacterStatsExtensions
    {
        private const string STATS_KEY = "kirby_character_stats";
        
        /// <summary>
        /// Get or create character stats for a player
        /// </summary>
        public static KirbyCharacterStats GetCharacterStats(this global::Celeste.Player player)
        {
            if (player == null) return new KirbyCharacterStats();
            
            var data = DynamicData.For(player);
            var stats = data.Get<KirbyCharacterStats>(STATS_KEY);
            
            if (stats == null)
            {
                stats = new KirbyCharacterStats();
                data.Set(STATS_KEY, stats);
            }
            
            return stats;
        }
        
        /// <summary>
        /// Set character stats for a player
        /// </summary>
        public static void SetCharacterStats(this global::Celeste.Player player, KirbyCharacterStats stats)
        {
            if (player == null || stats == null) return;
            
            var data = DynamicData.For(player);
            data.Set(STATS_KEY, stats);
            
            // Apply dash count to player
            player.Dashes = stats.CurrentDashes;
        }
        
        /// <summary>
        /// Set dash level for player (auto-configures stats)
        /// </summary>
        public static void SetDashLevel(this global::Celeste.Player player, int dashCount)
        {
            if (player == null) return;
            
            var stats = DashLevelStats.GetStatsForDashCount(dashCount);
            player.SetCharacterStats(stats);
            
            IngesteLogger.Info($"Player dash level set to {dashCount}");
        }
        
        /// <summary>
        /// Apply stats modifiers to player movement
        /// </summary>
        public static float GetModifiedSpeed(this global::Celeste.Player player, float baseSpeed)
        {
            var stats = player.GetCharacterStats();
            return baseSpeed * stats.GetSpeedMultiplier();
        }
        
        /// <summary>
        /// Calculate modified dash speed
        /// </summary>
        public static float GetModifiedDashSpeed(this global::Celeste.Player player, float baseDashSpeed)
        {
            var stats = player.GetCharacterStats();
            return baseDashSpeed * stats.GetDashSpeedMultiplier();
        }
        
        /// <summary>
        /// Apply damage with stats calculation
        /// </summary>
        public static bool ApplyDamageWithStats(this global::Celeste.Player player, int baseDamage)
        {
            var stats = player.GetCharacterStats();
            return stats.TakeDamage(baseDamage);
        }
        
        /// <summary>
        /// Heal with stats
        /// </summary>
        public static void HealWithStats(this global::Celeste.Player player, int amount)
        {
            var stats = player.GetCharacterStats();
            stats.Heal(amount);
        }
        
        /// <summary>
        /// Refill dashes according to stats
        /// </summary>
        public static void RefillDashesWithStats(this global::Celeste.Player player)
        {
            var stats = player.GetCharacterStats();
            stats.RefillDashes();
            player.Dashes = stats.CurrentDashes;
        }
    }
}
