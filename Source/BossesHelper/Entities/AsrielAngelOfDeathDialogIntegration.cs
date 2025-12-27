using Celeste;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using DesoloZantas.Core.Core.VanillaCore;

namespace Celeste.Mod.DesoloZatnas.BossesHelper.Entities
{
    /// <summary>
    /// Dialog and cutscene integration for Asriel Angel of Death boss fight
    /// Handles soul rescue sequences, memory restoration, and emotional dialog triggers
    /// </summary>
    public partial class AsrielAngelOfDeathBoss
    {
        // Soul rescue system - tracks which souls have been rescued
        private static readonly string[] SoulNames = new string[]
        {
            "magolor", "chara", "theo", "oshiro",
            "toriel", "asgore", "alphys", "papyrus", "sans", "undyne",
            "suzy", "noelle", "berdly", "ralsei", "starsei"
        };
        
        /// <summary>
        /// Main soul rescue sequence - player saves lost souls to restore Asriel's memories
        /// </summary>
        private IEnumerator SoulRescueSequence()
        {
            // Trigger 0: Void Power appears
            yield return PlayDialog("CH20_ASRIEL_HEART_LOST_SOUL");
            
            // Pause attacks while souls are being rescued
            isInMemorySequence = true;
            
            // Rescue each soul group
            yield return RescueSoul("magolor", "MAGOLOR");
            yield return RescueSoul("chara", "CHARA");
            yield return RescueSoul("theo", "THEO");
            yield return RescueSoul("oshiro", "OSHIRO");
            
            // Monster kind souls
            yield return RescueSoul("toriel", "TORIEL");
            yield return RescueSoul("asgore", "ASGORE");
            yield return RescueSoul("alphys", "ALPHYS");
            yield return RescueSoul("papyrus", "PAPYRUS");
            yield return RescueSoul("sans", "SANS");
            yield return RescueSoul("undyne", "UNDYNE");
            
            // Deltarune crew
            yield return RescueSoul("suzy", "SUZY");
            yield return RescueSoul("noelle", "NOELLE");
            yield return RescueSoul("berdly", "BERDLY");
            yield return RescueSoul("ralsei", "RALSEI");
            yield return RescueSoul("starsei", "STARSEI");
            
            isInMemorySequence = false;
            
            // All souls rescued - begin memory restoration
            yield return MemoryRestorationSequence();
        }
        
        /// <summary>
        /// Rescue a single soul
        /// </summary>
        private IEnumerator RescueSoul(string soulId, string soulName)
        {
            if (rescuedSouls.Contains(soulId))
                yield break;
            
            // Visual effect for soul appearing
            yield return SpawnSoulEffect(soulName);
            
            // Brief pause for effect
            yield return 0.5f;
            
            // Mark as rescued
            rescuedSouls.Add(soulId);
            
            // Visual feedback
            yield return FlashScreen("FFFFFF", 0.1f);
        }
        
        /// <summary>
        /// Memory restoration sequence - Asriel remembers who he really is
        /// </summary>
        private IEnumerator MemoryRestorationSequence()
        {
            // Asriel begins to remember
            yield return PlayDialog("CH20_ASRIEL_REMEMBER_A");
            
            memoryRestorationStage = 1;
            yield return ShakeScreen(1.0f, 2.0f);
            
            // Asriel fights back against memories
            yield return PlayDialog("CH20_ASRIEL_REMEMBER_B");
            
            memoryRestorationStage = 2;
            yield return FlashScreen("990000", 0.3f);
            
            // Asriel reveals his true motivations
            yield return PlayDialog("CH20_ASRIEL_REMEMBER_C");
            
            memoryRestorationStage = 3;
            yield return CameraZoom(2.5f, 1.5f, "ease_out");
            
            // Deeper emotional revelation
            yield return PlayDialog("CH20_ASRIEL_REMEMBER_D");
            
            memoryRestorationStage = 4;
            
            // Asriel's fear of leaving
            yield return PlayDialog("CH20_ASRIEL_REMEMBER_E");
            
            memoryRestorationStage = 5;
            
            // Final beam attack sequence with breakdown
            yield return FinalBeamMemorySequence();
            
            // Asriel breaks down and remembers
            yield return PlayDialog("CH20_ASRIEL_REMEMBER_F");
            
            memoryRestorationStage = 6;
            yield return CameraZoomBack(2.0f);
        }
        
        /// <summary>
        /// Final beam attack with emotional breakdown
        /// </summary>
        private IEnumerator FinalBeamMemorySequence()
        {
            isInFinalBeamSequence = true;
            
            // Dialog during charging
            yield return PlayDialog("CH20_ASRIEL_REMEMBER_FINAL");
            
            // Trigger 1: Asriel's form destabilizes
            bodySprite.Play("attack_finalbeam_charge");
            
            for (int i = 0; i < 3; i++)
            {
                yield return ShakeScreen(0.5f + i * 0.3f, 1.5f);
                yield return FlashScreen(i % 2 == 0 ? "FF0000" : "000000", 0.2f);
            }
            
            // Hold positions during emotional breakdown
            bodySprite.Play("attack_finalbeam_hold_A");
            yield return 1.5f;
            
            bodySprite.Play("attack_finalbeam_hold_B");
            yield return 1.5f;
            
            // Beam dissipates as Asriel loses control
            yield return SpawnParticles("light", Position, 50);
            
            isInFinalBeamSequence = false;
        }
        
        /// <summary>
        /// Final defeat sequence after Asriel remembers
        /// </summary>
        private IEnumerator FinalDefeatSequence()
        {
            // Asriel has fully remembered and accepts defeat
            bodySprite.Play("defeat");
            
            // Emotional fade out
            yield return CameraZoom(1.5f, 3.0f, "ease_in");
            yield return FlashScreen("FFFFFF", 2.0f);
            
            yield return 2f;
            
            // Victory - battle ends
            RemoveSelf();
        }
        
        // ===== HELPER METHODS FOR CUTSCENE EFFECTS =====
        
        /// <summary>
        /// Play a dialog cutscene
        /// </summary>
        private IEnumerator PlayDialog(string dialogKey)
        {
            if (level == null)
                yield break;
            
            // Pause player during dialog
            if (player != null)
                player.StateMachine.State = Player.StDummy;
            
            // Trigger the dialog through Celeste's textbox system
            yield return Textbox.Say(dialogKey);
            
            // Resume player control
            if (player != null && player.StateMachine.State == Player.StDummy)
                player.StateMachine.State = Player.StNormal;
        }
        
        /// <summary>
        /// Camera zoom effect
        /// </summary>
        private IEnumerator CameraZoom(float targetZoom, float duration, string easingType = "linear")
        {
            if (level == null)
                yield break;
            
            float startZoom = level.Zoom;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Engine.DeltaTime;
                float progress = Math.Min(elapsed / duration, 1f);
                
                // Apply easing
                float easedProgress = ApplyEasing(progress, easingType);
                level.Zoom = Calc.LerpClamp(startZoom, targetZoom, easedProgress);
                
                yield return null;
            }
            
            level.Zoom = targetZoom;
        }
        
        /// <summary>
        /// Camera zoom back to normal
        /// </summary>
        private IEnumerator CameraZoomBack(float duration)
        {
            yield return CameraZoom(1.0f, duration, "ease_out");
        }
        
        /// <summary>
        /// Screen shake effect
        /// </summary>
        private IEnumerator ShakeScreen(float intensity, float duration)
        {
            if (level == null)
                yield break;
            
            level.Shake(duration);
            yield return duration;
        }
        
        /// <summary>
        /// Flash screen with color
        /// </summary>
        private IEnumerator FlashScreen(string colorHex, float duration)
        {
            if (level == null)
                yield break;
            
            Color flashColor = Calc.HexToColor(colorHex);
            
            // Create flash overlay
            Entity flash = new Entity(Position);
            Image flashImg = new Image(GFX.Misc["whiteCube"]);
            flashImg.Color = flashColor;
            flashImg.Scale = new Vector2(320f, 180f); // Full screen coverage
            flash.Add(flashImg);
            
            level.Add(flash);
            
            // Fade out flash
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Engine.DeltaTime;
                flashImg.Color = flashColor * (1f - elapsed / duration);
                yield return null;
            }
            
            flash.RemoveSelf();
        }
        
        /// <summary>
        /// Spawn particle effects
        /// </summary>
        private IEnumerator SpawnParticles(string particleType, Vector2 position, int count)
        {
            if (level == null)
                yield break;
            
            for (int i = 0; i < count; i++)
            {
                Vector2 offset = new Vector2(
                    (float)(random.NextDouble() * 40 - 20),
                    (float)(random.NextDouble() * 40 - 20)
                );
                
                switch (particleType.ToLower())
                {
                    case "dust":
                        level.Particles.Emit(Player.P_DashA, position + offset);
                        break;
                    case "light":
                        level.ParticlesFG.Emit(Strawberry.P_Glow, position + offset);
                        break;
                    case "sparkle":
                        level.ParticlesFG.Emit(Strawberry.P_Glow, position + offset);
                        break;
                }
            }
            
            yield return 0.1f;
        }
        
        /// <summary>
        /// Spawn visual effect for a soul appearing
        /// </summary>
        private IEnumerator SpawnSoulEffect(string soulName)
        {
            if (level == null)
                yield break;
            
            // Create soul visual
            Vector2 soulPosition = Position + new Vector2(0, -60);
            
            yield return SpawnParticles("light", soulPosition, 20);
            yield return FlashScreen("AAFFFF", 0.2f);
            
            // Play sound effect
            Audio.Play("event:/char/badeline/boss_bullet", soulPosition);
        }
        
        /// <summary>
        /// Apply easing function to progress value
        /// </summary>
        private float ApplyEasing(float progress, string easingType)
        {
            switch (easingType.ToLower())
            {
                case "ease_in":
                    return progress * progress;
                case "ease_out":
                    return 1f - (1f - progress) * (1f - progress);
                case "ease_in_out":
                    return progress < 0.5f 
                        ? 2f * progress * progress 
                        : 1f - 2f * (1f - progress) * (1f - progress);
                case "cube":
                    return progress * progress * progress;
                default: // linear
                    return progress;
            }
        }
    }
}
