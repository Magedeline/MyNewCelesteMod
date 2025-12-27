using DesoloZantas.Core.Core.Extensions;
using DesoloZantas.Core.Core.Player;

namespace DesoloZantas.Core.Core.Triggers
{
    /// <summary>
  /// Trigger to enable or disable Kirby mode for the player
    /// Useful for switching between normal and Kirby player modes in CollabUtils2 submaps
    /// </summary>
    [CustomEntity("DesoloZantas/KirbyModeTrigger")]
    [Tracked(false)]
    public class KirbyModeTrigger : Trigger
    {
      private readonly bool enableKirby;
      private readonly bool oneUse;
        private bool triggered;

   public KirbyModeTrigger(EntityData data, Vector2 offset) 
            : base(data, offset)
    {
            enableKirby = data.Bool("enableKirby", true);
          oneUse = data.Bool("oneUse", false);
        }

     public override void OnEnter(global::Celeste.Player player)
        {
        base.OnEnter(player);

    if (oneUse && triggered) return;

            triggered = true;

            try
            {
         Level level = SceneAs<Level>();
        if (level == null) return;

   if (enableKirby)
      {
          if (!player.IsKirbyMode())
                {
         player.EnableKirbyMode();
            level.Session.SetFlag("player_mode_kirby", true);
           level.Session.SetFlag("player_mode_normal", false);
         
          IngesteLogger.Info("Kirby mode enabled via trigger");
    
              // Visual/audio feedback
        Audio.Play("event:/game/general/crystalheart_blue_get", player.Position);
        level.ParticlesFG.Emit(ParticleTypes.Dust, 10, player.Position, Vector2.One * 16f);
         }
     }
   else
         {
    if (player.IsKirbyMode())
        {
       player.DisableKirbyMode();
 level.Session.SetFlag("player_mode_kirby", false);
         level.Session.SetFlag("player_mode_normal", true);
           
        IngesteLogger.Info("Kirby mode disabled via trigger");
       
   // Visual/audio feedback
              Audio.Play("event:/game/general/seed_touch", player.Position);
 level.ParticlesFG.Emit(ParticleTypes.Dust, 10, player.Position, Vector2.One * 16f);
   }
        }
         }
      catch (System.Exception ex)
            {
       IngesteLogger.Error($"Error in KirbyModeTrigger: {ex.Message}");
            }
        }

        public override void OnStay(global::Celeste.Player player)
        {
            // Reset triggered flag if not one-use
  if (!oneUse)
         {
      triggered = false;
   }
    }
    }
}




