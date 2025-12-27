using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Player;
internal class InhaleSpit
{
    private const float inhale_range = 50f;
    private const float spit_speed_x = 200f;
    private const float spit_speed_y = -50f;
    private StarBlock inhaledObject;

    public void Begin(global::Celeste.Player player)
    {
        player.Sprite.Play("inhale_start");
        Audio.Play("event:/char/kirby/inhale", player.Position);
    }

    public int Update(global::Celeste.Player player)
    {
        foreach (var entity in player.Scene.Tracker.GetEntities<StarBlock>())
        {
            if (Vector2.Distance(player.Center, entity.Center) < inhale_range)
            {
                player.Scene.Remove(entity);
                inhaledObject = (StarBlock)entity;
                return (int)IngesteStates.StSpit;
            }
        }
        return (int)IngesteStates.StInhale;
    }

    public void End(global::Celeste.Player player)
    {
        player.Sprite.Play("inhale_end");
    }

    public void SpitBegin(global::Celeste.Player player)
    {
        player.Sprite.Play("spit_start");
        Audio.Play("event:/char/kirby/spit", player.Position);
    }

    public int SpitUpdate(global::Celeste.Player player)
    {
        if (inhaledObject == null)
            return (int)IngesteStates.StNormal;

        inhaledObject.Position = player.Center;
        inhaledObject.Speed = new Vector2((int)player.Facing * spit_speed_x, spit_speed_y);
        player.Scene.Add(inhaledObject);
        inhaledObject = null;
        return (int)IngesteStates.StNormal;
    }

    public void SpitEnd(global::Celeste.Player player)
    {
        player.Sprite.Play("spit_end");
    }
}




