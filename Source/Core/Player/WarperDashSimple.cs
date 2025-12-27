namespace DesoloZantas.Core.Core.Player
{
    // Simplified warper dash that avoids complex dependencies
    public static class WarperDashSimple
    {
        public static bool IsActive { get; private set; }
        
        public static void PerformWarperDash(Vector2 position, Vector2 direction, Scene scene)
        {
            IsActive = true;
            
            // Play audio effect
            Audio.Play("event:/warper_dash", position);
            
            // Simple dash effect - damage nearby enemies
            DamageNearbyEntities(scene, position, direction);
            
            IsActive = false;
        }
        
        private static void DamageNearbyEntities(Scene scene, Vector2 center, Vector2 direction)
        {
            if (scene == null) return;
            
            foreach (var entity in scene.Entities)
            {
                if (Vector2.Distance(entity.Position, center) <= 50f)
                {
                    // Check if it's a boss and damage it
                    if (entity.GetType().Name.Contains("Boss"))
                    {
                        // Use reflection to call TakeDamage if it exists
                        var method = entity.GetType().GetMethod("TakeDamage");
                        method?.Invoke(entity, new object[] { 25 });
                    }
                }
            }
        }
    }
}



