namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Represents the state of a chaser character for auto-animators.
    /// </summary>
    public class ChaserState
    {
        public Microsoft.Xna.Framework.Vector2 Position { get; set; }
        public int Facing { get; set; }
        public string Animation { get; set; }
        public bool OnGround { get; set; }

        public ChaserState()
        {
            Position = Microsoft.Xna.Framework.Vector2.Zero;
            Facing = 1;
            Animation = string.Empty;
            OnGround = false;
        }

        public ChaserState(Microsoft.Xna.Framework.Vector2 position, int facing, string animation, bool onGround)
        {
            Position = position;
            Facing = facing;
            Animation = animation;
            OnGround = onGround;
        }
    }
}



