using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Extended dash system for Kirby player with support for 3, 4, 5, and 10 dashes.
    /// Includes special dash types and visual effects for each tier.
    /// </summary>
    public static class KirbyDashExtensions
    {
        #region Dash Tier Definitions
        
        /// <summary>
        /// Available dash tiers
        /// </summary>
        public enum DashTier
        {
            /// <summary>Standard single dash</summary>
            Standard = 1,
            /// <summary>Double dash (Celeste crystal)</summary>
            Double = 2,
            /// <summary>Triple dash (enhanced mobility)</summary>
            Triple = 3,
            /// <summary>Quad dash (advanced mobility)</summary>
            Quad = 4,
            /// <summary>Penta dash (expert mobility)</summary>
            Penta = 5,
            /// <summary>Ultra dash (Kirby Star Power / 10 dashes)</summary>
            Ultra = 10
        }
        
        #endregion
        
        #region Hair Colors Per Dash Tier
        
        /// <summary>
        /// Hair colors for each dash count (extending Celeste's 2-dash system)
        /// </summary>
        private static readonly Dictionary<int, Color> DashTierHairColors = new Dictionary<int, Color>
        {
            { 0, Calc.HexToColor("44B7FF") },   // Used hair - Blue
            { 1, Calc.HexToColor("AC3232") },   // 1 dash - Red (default Celeste)
            { 2, Calc.HexToColor("FF6DEF") },   // 2 dashes - Pink (default Celeste)
            { 3, Calc.HexToColor("FFDE00") },   // 3 dashes - Gold
            { 4, Calc.HexToColor("00FFB0") },   // 4 dashes - Cyan/Teal
            { 5, Calc.HexToColor("FF8C00") },   // 5 dashes - Orange
            { 6, Calc.HexToColor("9B30FF") },   // 6 dashes - Purple
            { 7, Calc.HexToColor("00FF00") },   // 7 dashes - Bright Green
            { 8, Calc.HexToColor("FF1493") },   // 8 dashes - Deep Pink
            { 9, Calc.HexToColor("00BFFF") },   // 9 dashes - Deep Sky Blue
            { 10, Calc.HexToColor("FFD700") },  // 10 dashes - Rainbow/Gold Star
        };
        
        /// <summary>
        /// Get hair color for specific dash count
        /// </summary>
        public static Color GetDashTierHairColor(int dashCount)
        {
            if (DashTierHairColors.TryGetValue(dashCount, out var color))
                return color;
            
            // For counts beyond defined, use a gradient
            if (dashCount > 10)
                return Color.Lerp(DashTierHairColors[10], Color.White, 0.5f);
            
            return DashTierHairColors[1]; // Default to single dash color
        }
        
        #endregion
        
        #region Dash Speed Modifiers
        
        /// <summary>
        /// Base dash speed (from Celeste)
        /// </summary>
        public const float BaseDashSpeed = 240f;
        
        /// <summary>
        /// Speed multipliers for each dash tier
        /// </summary>
        private static readonly Dictionary<DashTier, float> DashTierSpeedMultipliers = new Dictionary<DashTier, float>
        {
            { DashTier.Standard, 1.0f },
            { DashTier.Double, 1.0f },
            { DashTier.Triple, 1.05f },
            { DashTier.Quad, 1.1f },
            { DashTier.Penta, 1.15f },
            { DashTier.Ultra, 1.25f }
        };
        
        /// <summary>
        /// Get dash speed for a specific tier
        /// </summary>
        public static float GetDashSpeed(DashTier tier)
        {
            if (DashTierSpeedMultipliers.TryGetValue(tier, out var mult))
                return BaseDashSpeed * mult;
            return BaseDashSpeed;
        }
        
        /// <summary>
        /// Get dash speed based on current dash count
        /// </summary>
        public static float GetDashSpeedForCount(int maxDashes)
        {
            var tier = GetTierFromCount(maxDashes);
            return GetDashSpeed(tier);
        }
        
        #endregion
        
        #region Dash Cooldown Modifiers
        
        /// <summary>
        /// Base dash cooldown (from Celeste)
        /// </summary>
        public const float BaseDashCooldown = 0.2f;
        
        /// <summary>
        /// Cooldown multipliers for each dash tier (lower = faster cooldown)
        /// </summary>
        private static readonly Dictionary<DashTier, float> DashTierCooldownMultipliers = new Dictionary<DashTier, float>
        {
            { DashTier.Standard, 1.0f },
            { DashTier.Double, 1.0f },
            { DashTier.Triple, 0.95f },
            { DashTier.Quad, 0.9f },
            { DashTier.Penta, 0.85f },
            { DashTier.Ultra, 0.75f }
        };
        
        /// <summary>
        /// Get dash cooldown for a specific tier
        /// </summary>
        public static float GetDashCooldown(DashTier tier)
        {
            if (DashTierCooldownMultipliers.TryGetValue(tier, out var mult))
                return BaseDashCooldown * mult;
            return BaseDashCooldown;
        }
        
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Convert dash count to tier
        /// </summary>
        public static DashTier GetTierFromCount(int dashCount)
        {
            return dashCount switch
            {
                <= 1 => DashTier.Standard,
                2 => DashTier.Double,
                3 => DashTier.Triple,
                4 => DashTier.Quad,
                5 => DashTier.Penta,
                >= 10 => DashTier.Ultra,
                _ => DashTier.Penta
            };
        }
        
        /// <summary>
        /// Check if player has ultra dash (10 dashes)
        /// </summary>
        public static bool HasUltraDash(this global::Celeste.Player player)
        {
            if (player == null) return false;
            var stats = player.GetCharacterStats();
            return stats.MaxDashes >= 10;
        }
        
        /// <summary>
        /// Check if player has penta dash (5 dashes)
        /// </summary>
        public static bool HasPentaDash(this global::Celeste.Player player)
        {
            if (player == null) return false;
            var stats = player.GetCharacterStats();
            return stats.MaxDashes >= 5;
        }
        
        /// <summary>
        /// Check if player has quad dash (4 dashes)
        /// </summary>
        public static bool HasQuadDash(this global::Celeste.Player player)
        {
            if (player == null) return false;
            var stats = player.GetCharacterStats();
            return stats.MaxDashes >= 4;
        }
        
        /// <summary>
        /// Check if player has triple dash (3 dashes)
        /// </summary>
        public static bool HasTripleDash(this global::Celeste.Player player)
        {
            if (player == null) return false;
            var stats = player.GetCharacterStats();
            return stats.MaxDashes >= 3;
        }
        
        #endregion
        
        #region Player Extension Methods
        
        /// <summary>
        /// Set player to specific dash tier
        /// </summary>
        public static void SetDashTier(this global::Celeste.Player player, DashTier tier)
        {
            if (player == null) return;
            
            var stats = player.GetCharacterStats();
            stats.MaxDashes = (int)tier;
            stats.CurrentDashes = (int)tier;
            player.SetCharacterStats(stats);
            
            // Update player dashes
            player.Dashes = stats.CurrentDashes;
            
            IngesteLogger.Info($"Player dash tier set to {tier} ({(int)tier} dashes)");
        }
        
        /// <summary>
        /// Upgrade player to next dash tier
        /// </summary>
        public static bool UpgradeDashTier(this global::Celeste.Player player)
        {
            if (player == null) return false;
            
            var stats = player.GetCharacterStats();
            int currentMax = stats.MaxDashes;
            
            // Determine next tier
            DashTier nextTier = currentMax switch
            {
                1 => DashTier.Double,
                2 => DashTier.Triple,
                3 => DashTier.Quad,
                4 => DashTier.Penta,
                5 => DashTier.Ultra,
                _ => DashTier.Ultra // Already at max
            };
            
            if ((int)nextTier > currentMax)
            {
                player.SetDashTier(nextTier);
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Get current dash tier hair color
        /// </summary>
        public static Color GetCurrentDashTierColor(this global::Celeste.Player player)
        {
            if (player == null) return GetDashTierHairColor(1);
            return GetDashTierHairColor(player.Dashes);
        }
        
        /// <summary>
        /// Perform enhanced dash with tier effects
        /// </summary>
        public static void PerformTieredDash(this global::Celeste.Player player, Vector2 direction)
        {
            if (player == null) return;
            
            var stats = player.GetCharacterStats();
            var tier = GetTierFromCount(stats.MaxDashes);
            
            // Apply dash speed based on tier
            float dashSpeed = GetDashSpeed(tier);
            
            // Create visual effects based on tier
            if (player.Scene is Level level)
            {
                CreateDashTierEffects(level, player.Position, direction, tier);
            }
            
            IngesteLogger.Debug($"Tiered dash performed: Tier={tier}, Speed={dashSpeed}");
        }
        
        /// <summary>
        /// Create visual effects for dash tier
        /// </summary>
        private static void CreateDashTierEffects(Level level, Vector2 position, Vector2 direction, DashTier tier)
        {
            if (level?.ParticlesFG == null) return;
            
            int particleCount = tier switch
            {
                DashTier.Standard => 2,
                DashTier.Double => 4,
                DashTier.Triple => 6,
                DashTier.Quad => 8,
                DashTier.Penta => 10,
                DashTier.Ultra => 15,
                _ => 4
            };
            
            Color particleColor = GetDashTierHairColor((int)tier);
            
            // Emit particles
            for (int i = 0; i < particleCount; i++)
            {
                float angle = direction.Angle() + (float)(Calc.Random.NextDouble() - 0.5) * 0.5f;
                Vector2 particleDir = Calc.AngleToVector(angle, 1f);
                
                level.ParticlesFG.Emit(
                    global::Celeste.Player.P_DashA,
                    position + particleDir * 8f,
                    particleColor,
                    angle
                );
            }
        }
        
        #endregion
        
        #region Dash Refill Logic
        
        /// <summary>
        /// Refill dashes based on tier rules
        /// </summary>
        public static void RefillDashesForTier(this global::Celeste.Player player, bool fullRefill = false)
        {
            if (player == null) return;
            
            var stats = player.GetCharacterStats();
            
            if (fullRefill)
            {
                stats.RefillDashes();
            }
            else
            {
                // Partial refill based on tier (higher tiers refill more slowly)
                var tier = GetTierFromCount(stats.MaxDashes);
                int refillAmount = tier switch
                {
                    DashTier.Ultra => 2,  // Ultra refills 2 at a time
                    DashTier.Penta => 2,  // Penta refills 2 at a time
                    _ => stats.MaxDashes  // Others get full refill
                };
                
                stats.CurrentDashes = Math.Min(stats.MaxDashes, stats.CurrentDashes + refillAmount);
            }
            
            player.Dashes = stats.CurrentDashes;
        }
        
        /// <summary>
        /// Check if player should get ground refill
        /// </summary>
        public static bool ShouldRefillOnGround(this global::Celeste.Player player)
        {
            if (player == null) return false;
            
            var stats = player.GetCharacterStats();
            return stats.CurrentDashes < stats.MaxDashes;
        }
        
        #endregion
        
        #region Special Dash Abilities
        
        /// <summary>
        /// Check if player can perform chain dash (3+ dashes)
        /// </summary>
        public static bool CanChainDash(this global::Celeste.Player player)
        {
            if (player == null) return false;
            var stats = player.GetCharacterStats();
            return stats.MaxDashes >= 3 && stats.CurrentDashes >= 2;
        }
        
        /// <summary>
        /// Check if player can perform ultra burst (10 dashes)
        /// </summary>
        public static bool CanUltraBurst(this global::Celeste.Player player)
        {
            if (player == null) return false;
            var stats = player.GetCharacterStats();
            return stats.MaxDashes >= 10 && stats.CurrentDashes >= 5;
        }
        
        /// <summary>
        /// Perform ultra burst (consumes 5 dashes for powerful effect)
        /// </summary>
        public static bool PerformUltraBurst(this global::Celeste.Player player, Vector2 direction)
        {
            if (!player.CanUltraBurst()) return false;
            
            var stats = player.GetCharacterStats();
            stats.CurrentDashes -= 5;
            player.Dashes = stats.CurrentDashes;
            
            // Create burst effect
            if (player.Scene is Level level)
            {
                CreateUltraBurstEffect(level, player.Position);
            }
            
            return true;
        }
        
        /// <summary>
        /// Create ultra burst visual effect
        /// </summary>
        private static void CreateUltraBurstEffect(Level level, Vector2 position)
        {
            if (level?.ParticlesFG == null) return;
            
            // Create radial burst of particles
            for (int i = 0; i < 24; i++)
            {
                float angle = (float)(i * Math.PI * 2 / 24);
                Vector2 dir = Calc.AngleToVector(angle, 1f);
                
                level.ParticlesFG.Emit(
                    global::Celeste.Player.P_DashB,
                    position,
                    GetDashTierHairColor(10),
                    angle
                );
            }
            
            // Screen shake
            level.Shake(0.3f);
            
            // Flash
            level.Flash(Color.Gold * 0.5f, false);
        }
        
        #endregion
    }
    
    /// <summary>
    /// Dash trail configuration for extended dash system
    /// </summary>
    public class DashTrailConfig
    {
        public Color TrailColor { get; set; }
        public float TrailDuration { get; set; }
        public int TrailCount { get; set; }
        public float TrailInterval { get; set; }
        
        public static DashTrailConfig GetForTier(KirbyDashExtensions.DashTier tier)
        {
            return tier switch
            {
                KirbyDashExtensions.DashTier.Standard => new DashTrailConfig
                {
                    TrailColor = KirbyDashExtensions.GetDashTierHairColor(1),
                    TrailDuration = 0.15f,
                    TrailCount = 3,
                    TrailInterval = 0.05f
                },
                KirbyDashExtensions.DashTier.Double => new DashTrailConfig
                {
                    TrailColor = KirbyDashExtensions.GetDashTierHairColor(2),
                    TrailDuration = 0.15f,
                    TrailCount = 4,
                    TrailInterval = 0.04f
                },
                KirbyDashExtensions.DashTier.Triple => new DashTrailConfig
                {
                    TrailColor = KirbyDashExtensions.GetDashTierHairColor(3),
                    TrailDuration = 0.18f,
                    TrailCount = 5,
                    TrailInterval = 0.035f
                },
                KirbyDashExtensions.DashTier.Quad => new DashTrailConfig
                {
                    TrailColor = KirbyDashExtensions.GetDashTierHairColor(4),
                    TrailDuration = 0.2f,
                    TrailCount = 6,
                    TrailInterval = 0.03f
                },
                KirbyDashExtensions.DashTier.Penta => new DashTrailConfig
                {
                    TrailColor = KirbyDashExtensions.GetDashTierHairColor(5),
                    TrailDuration = 0.22f,
                    TrailCount = 7,
                    TrailInterval = 0.025f
                },
                KirbyDashExtensions.DashTier.Ultra => new DashTrailConfig
                {
                    TrailColor = KirbyDashExtensions.GetDashTierHairColor(10),
                    TrailDuration = 0.3f,
                    TrailCount = 10,
                    TrailInterval = 0.02f
                },
                _ => new DashTrailConfig
                {
                    TrailColor = Color.White,
                    TrailDuration = 0.15f,
                    TrailCount = 3,
                    TrailInterval = 0.05f
                }
            };
        }
    }
}
