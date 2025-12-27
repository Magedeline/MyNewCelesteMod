namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// Health bar component for KirbyPlayer
    /// </summary>
    public class KirbyHealthBar : Component
    {
        private KirbyPlayer kirbyPlayer;
        private MTexture healthTexture;
        private MTexture emptyHealthTexture;
        private Vector2 offset;
        private float displayTimer;
        private bool isVisible;
        
        private const float display_duration = 3.0f;
        private const float fade_duration = 0.5f;
        private const int heart_width = 12;
        private const int heart_height = 12;
        private const int hearts_per_row = 6;

        public KirbyHealthBar(KirbyPlayer player) : base(true, true)
        {
            kirbyPlayer = player;
            offset = new Vector2(-32f, -48f);
            
            // Load health textures with fallbacks
            try
            {
                healthTexture = GFX.Game["gui/kirby/heart_full"];
                emptyHealthTexture = GFX.Game["gui/kirby/heart_empty"];
            }
            catch
            {
                // Fallback to basic textures
                healthTexture = GFX.Game["collectables/heartGem/0/00"];
                emptyHealthTexture = GFX.Game["collectables/heartGem/0/00"];
            }
            
            isVisible = false;
            displayTimer = 0f;
        }

        public void UpdateHealth(int newHealth)
        {
            // Show health bar when health changes
            ShowHealthBar();
        }

        public void ShowHealthBar()
        {
            isVisible = true;
            displayTimer = display_duration;
        }

        public void HideHealthBar()
        {
            isVisible = false;
            displayTimer = 0f;
        }

        public override void Update()
        {
            base.Update();
            
            if (isVisible && displayTimer > 0)
            {
                displayTimer -= Engine.DeltaTime;
                if (displayTimer <= 0)
                {
                    isVisible = false;
                }
            }
        }

        public override void Render()
        {
            if (!isVisible || kirbyPlayer == null || healthTexture == null) 
                return;

            Vector2 position = kirbyPlayer.Position + offset;
            
            // Calculate alpha for fade effect
            float alpha = 1f;
            if (displayTimer < fade_duration)
            {
                alpha = displayTimer / fade_duration;
            }

            // Render health hearts
            renderHealthHearts(position, alpha);
        }

        private void renderHealthHearts(Vector2 position, float alpha)
        {
            int maxHealth = kirbyPlayer.MaxHealth;
            int currentHealth = kirbyPlayer.CurrentHealth;
            
            for (int i = 0; i < maxHealth; i++)
            {
                int row = i / hearts_per_row;
                int col = i % hearts_per_row;
                
                Vector2 heartPos = position + new Vector2(col * heart_width, row * heart_height);
                
                // Render empty heart first
                if (emptyHealthTexture != null)
                {
                    emptyHealthTexture.DrawCentered(heartPos, Color.White * alpha * 0.3f);
                }
                
                // Render filled heart if player has health
                if (i < currentHealth && healthTexture != null)
                {
                    Color heartColor = getHeartColor(currentHealth, maxHealth);
                    healthTexture.DrawCentered(heartPos, heartColor * alpha);
                }
            }
        }

        private Color getHeartColor(int currentHealth, int maxHealth)
        {
            float healthPercent = (float)currentHealth / maxHealth;
            
            if (healthPercent > 0.6f)
                return Color.LimeGreen; // High health - green
            else if (healthPercent > 0.3f)
                return Color.Yellow; // Medium health - yellow
            else
                return Color.Red; // Low health - red
        }
    }
}



