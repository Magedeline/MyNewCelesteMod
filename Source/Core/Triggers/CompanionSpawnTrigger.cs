namespace DesoloZantas.Core.Core.Triggers
{
    [CustomEntity("Ingeste/CompanionSpawnTrigger")]
    public class CompanionSpawnTrigger : Trigger
    {
        private readonly PlayerSpriteMode companionType;
        private readonly bool oneUse;
        private readonly string sessionFlag;
        private bool triggered = false;

        public CompanionSpawnTrigger(EntityData data, Vector2 offset) 
            : base(data, offset)
        {
            // Parse companion type from data
            string companionName = data.Attr("companionType", "Gooey");
            if (Enum.TryParse<PlayerSpriteMode>(companionName, out var parsedType))
            {
                companionType = parsedType;
            }
            else
            {
                companionType = PlayerSpriteMode.Gooey; // Default fallback
            }
            
            oneUse = data.Bool("oneUse", true);
            sessionFlag = data.Attr("sessionFlag", "");
        }

        public override void OnEnter(global::Celeste.Player player)
        {
            if (triggered && oneUse)
                return;

            // Check session flag if specified
            if (!string.IsNullOrEmpty(sessionFlag))
            {
                var session = SceneAs<Level>().Session;
                if (session.GetFlag(sessionFlag))
                    return;
                
                session.SetFlag(sessionFlag, true);
            }

            // Spawn the companion
            Vector2 spawnPosition = player.Position + Vector2.UnitY * -16f;
            CompanionCharacterManager.SpawnCompanion(SceneAs<Level>(), companionType, spawnPosition);
            
            triggered = true;

            // Visual and audio feedback
            Audio.Play("event:/char/companion/spawn", Position);
            SceneAs<Level>().Displacement.AddBurst(spawnPosition, 0.4f, 16f, 64f, 0.3f, null, null);
            SceneAs<Level>().Particles.Emit(Strawberry.P_Glow, 15, spawnPosition, Vector2.One * 12f);
        }
    }
}



