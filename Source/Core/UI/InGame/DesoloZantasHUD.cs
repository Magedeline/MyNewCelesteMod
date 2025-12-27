using System;
using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.DesoloZatnas.Core.UI.InGame
{
    /// <summary>
    /// Custom HUD overlay for DesoloZatnas chapters during gameplay
    /// Displays lives counter and D-Side progress
    /// </summary>
    public class DesoloZantasHUD : Entity
    {
        private Level level;
        private float alpha = 1f;
        private bool visible = true;
        private const float FADE_SPEED = 4f;
        
        // Lives display
        private const float HEART_SPACING = 24f;
        private const float HEART_SCALE = 0.8f;
        private Vector2 livesPosition = new Vector2(32f, 32f);
        
        // D-Side progress display
        private Vector2 progressPosition = new Vector2(32f, 80f);
        private string currentObjective = "";

        public DesoloZantasHUD(Level level)
        {
            this.level = level;
            Tag = Tags.HUD;
            Depth = -100; // Render on top of everything
            
            UpdateDSideObjective();
        }

        public override void Update()
        {
            base.Update();
            
            // Hide during cutscenes, transitions, or when paused
            visible = !level.Transitioning && 
                     !level.InCutscene && 
                     !level.Paused &&
                     !level.SkippingCutscene;
            
            // Smooth fade in/out
            if (visible)
                alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime * FADE_SPEED);
            else
                alpha = Calc.Approach(alpha, 0f, Engine.DeltaTime * FADE_SPEED * 2f);
        }

        public override void Render()
        {
            if (alpha <= 0f)
                return;
            
            RenderLivesCounter();
            
            if (IsInDSideChapter())
                RenderDSideProgress();
        }

        private void RenderLivesCounter()
        {
            int lives = LivesSystem.CurrentLives;
            int maxLives = LivesSystem.MaxLives;
            
            if (lives <= 0)
                return;

            // Draw heart icons
            for (int i = 0; i < Math.Min(lives, maxLives); i++)
            {
                Vector2 heartPos = livesPosition + new Vector2(i * HEART_SPACING, 0f);
                
                // Use different color based on remaining lives
                Color heartColor = Color.White;
                if (lives <= 1)
                    heartColor = Color.Red; // Critical
                else if (lives <= 3)
                    heartColor = Color.Yellow; // Warning
                
                // Draw heart outline
                DrawHeart(heartPos, heartColor * alpha, HEART_SCALE);
            }
            
            // Draw lives count text
            Vector2 textPos = livesPosition + new Vector2(maxLives * HEART_SPACING + 10f, 0f);
            string livesText = $"x{lives}";
            
            ActiveFont.DrawOutline(
                livesText,
                textPos,
                new Vector2(0f, 0.5f),
                Vector2.One * 0.7f,
                Color.White * alpha,
                2f,
                Color.Black * alpha * 0.7f
            );
        }

        private void DrawHeart(Vector2 position, Color color, float scale)
        {
            // Simple heart representation using text (you can replace with sprite later)
            // For now, use a filled circle as placeholder
            const float heartSize = 16f;
            
            // Draw filled heart shape (using Draw.Circle as placeholder)
            for (int i = 0; i < 3; i++)
            {
                float size = heartSize * scale - i * 2;
                Draw.Circle(position + new Vector2(heartSize/2, heartSize/2) * scale, size / 2f, color, (int)size);
            }
            
            // You can replace this with:
            // MTexture heartSprite = GFX.Gui["heartFull"];
            // heartSprite.DrawCentered(position, color, scale);
        }

        private void RenderDSideProgress()
        {
            if (string.IsNullOrEmpty(currentObjective))
                return;

            // Background panel
            Vector2 textSize = ActiveFont.Measure(currentObjective) * 0.6f;
            Vector2 panelSize = textSize + new Vector2(20f, 10f);
            
            Draw.Rect(
                progressPosition.X - 5f,
                progressPosition.Y - 5f,
                panelSize.X,
                panelSize.Y,
                Color.Black * 0.7f * alpha
            );
            
            // Objective text
            ActiveFont.DrawOutline(
                currentObjective,
                progressPosition,
                new Vector2(0f, 0f),
                Vector2.One * 0.6f,
                Color.Cyan * alpha,
                2f,
                Color.Black * alpha * 0.7f
            );
        }

        private void UpdateDSideObjective()
        {
            if (!IsInDSideChapter())
            {
                currentObjective = "";
                return;
            }

            AreaKey area = level.Session.Area;
            
            // Get D-Side specific objectives based on chapter
            if (area.Mode == (AreaMode)3) // D-Side
            {
                currentObjective = GetDSideObjective(area.ID);
            }
            else
            {
                currentObjective = "";
            }
        }

        private string GetDSideObjective(int areaID)
        {
            // Define objectives for each D-Side chapter
            switch (areaID)
            {
                case 10:
                    return "D-Side: Collect all Pink Platinum Berries";
                case 11:
                    return "D-Side: Complete without dying";
                case 12:
                    return "D-Side: Master the advanced mechanics";
                case 13:
                    return "D-Side: Speedrun challenge";
                case 14:
                    return "D-Side: Hidden secrets await";
                case 15:
                    return "D-Side: Ultimate test of skill";
                case 16:
                    return "D-Side: Precision platforming";
                case 17:
                    return "D-Side: Overcome the gauntlet";
                case 18:
                    return "D-Side: Final challenge";
                case 19:
                case 20:
                case 21:
                    return "DLC D-Side: Extreme difficulty";
                default:
                    return "D-Side Challenge";
            }
        }

        private bool IsInDSideChapter()
        {
            AreaKey area = level.Session.Area;
            
            // Check if in chapters 10-21 and in D-Side mode
            return area.ID >= 10 && area.ID <= 21 && area.Mode == (AreaMode)3; // D-Side
        }
    }
}
