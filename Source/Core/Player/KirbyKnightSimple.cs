namespace DesoloZantas.Core.Core.Player
{
    // Simplified Kirby Knight transformation for chapters 19-20
    public static class KirbyKnightSimple
    {
        public static bool IsKnightActive { get; private set; }
        
        public static void TryTransformToKnight(Scene scene, bool hasLostFight)
        {
            if (!hasLostFight || IsKnightActive) return;
            
            // Check if we're in chapters 19 or 20
            var level = scene as Level;
            if (level?.Session?.Area == null) return;
            
            string areaKey = level.Session.Area.GetSID();
            if (!areaKey.Contains("ch19") && !areaKey.Contains("ch20")) return;
            
            // Transform to knight
            IsKnightActive = true;
            Audio.Play("event:/kirby_knight_transform");
        }
        
        public static void ResetKnightForm()
        {
            IsKnightActive = false;
        }
        
        public static void PerformKnightAttack(Vector2 position, Scene scene)
        {
            if (!IsKnightActive) return;
            
            Audio.Play("event:/kirby_knight_attack", position);
            
            // Enhanced damage to nearby enemies
            foreach (var entity in scene.Entities)
            {
                if (Vector2.Distance(entity.Position, position) <= 60f)
                {
                    if (entity.GetType().Name.Contains("Boss"))
                    {
                        var method = entity.GetType().GetMethod("TakeDamage");
                        method?.Invoke(entity, new object[] { 50 }); // Double damage in knight form
                    }
                }
            }
        }
    }
}



