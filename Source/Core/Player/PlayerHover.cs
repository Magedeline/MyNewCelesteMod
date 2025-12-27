namespace DesoloZantas.Core.Core.Player;
internal class PlayerHover
{
    // Remove initialization from here, as Player.Speed is not static.
    private Vector2 speed;
    private float hover_speed;

    public void Begin(global::Celeste.Player player)
    {
        // Initialize speed using the instance of player.
        speed = player.Speed;
        player.Speed = Vector2.Zero; // FIX: Use the instance, not the type.
        player.Sprite.Play("hover_start");
    }

    public int Update(global::Celeste.Player player)
    {
        if (Input.Jump.Check)
        {
            speed.Y = hover_speed;
            return (int)IngesteStates.StHover;
        }
        return (int)IngesteStates.StNormal;
    }

    public void End(global::Celeste.Player player)
    {
        player.Sprite.Play("hover_end");
    }
}




