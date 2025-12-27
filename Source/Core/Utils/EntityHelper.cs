namespace DesoloZantas.Core.Core.Utils {
    /// <summary>
    /// Small helpers related to entities. Some parts of the project expect this file to exist.
    /// Keep the API minimal to avoid extra dependencies.
    /// </summary>
    public static class EntityHelper {
        /// <summary>
        /// Adds a simple PlayerCollider to the provided entity that invokes the specified callback
        /// when the player collides with the entity.
        /// </summary>
        public static void MakeInteractive(this Entity entity, Action<global::Celeste.Player> onPlayerCollide) {
            if (entity == null || onPlayerCollide == null) return;
            entity.Add(new PlayerCollider(onPlayerCollide));
        }
    }
}



