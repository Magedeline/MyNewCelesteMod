namespace DesoloZantas.Core.Core.Utils {
    public class MyEntity : Entity {

        public override void Update() 
        {
            base.Update();
        }
        
        public override void Render() {
            base.Render();
            // Render logic here
        }
        
        // Make the entity interactive with the player
        public void MakeInteractive() {
            Add(new PlayerCollider(OnPlayerCollide));
        }
        
        // Handle player collisions - cast to Action<Player> to fix compile error
        private void OnPlayerCollide(global::Celeste.Player player) {
            // Default implementation does nothing
            // Derived classes should override this
        }
    }
}




