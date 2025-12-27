using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Player
{
    internal class DashGroundPound
    {
        private const float ground_pound_speed = 300f;
        private global::Celeste.Player player;

        public DashGroundPound(global::Celeste.Player player)
        {
            this.player = player;
        }

        public DashGroundPound()
        {
            player = null;
        }

        public IEnumerator Begin()
        {
            player.Sprite.Play("DGPstart");
            Audio.Play("event:/char/kirby/dash_ground_pound", player.Position);
            yield return null;
        }

        public int Update()
        {
            if (player.OnGround())
            {
                return (int)IngesteStates.StNormal;
            }

            player.Speed.Y = ground_pound_speed;

            // Check for collision with StarBlock
            var starBlock = player.Scene.Tracker.GetEntity<StarBlock>();
            if (starBlock != null && starBlock.Collider.Collide(player.Collider))
            {
                starBlock.Break();
            }

            return (int)IngesteStates.StDashGroundPound;
        }

        public void End()
        {
            player.Sprite.Play("DGPend");
            Audio.Play("event:/char/kirby/dash_ground_pound_end", player.Position);
        }

        public IEnumerator CoroutineBegin()
        {
            player.Sprite.Play("DGPstart");

            while (!player.OnGround())
            {
                player.Speed.Y = ground_pound_speed;

                // Check for collision with StarBlock
                var starBlock = player.Scene.Tracker.GetEntity<StarBlock>();
                if (starBlock != null && starBlock.Collider.Collide(player.Collider))
                {
                    starBlock.Break();
                }

                yield return null;
            }

            player.StateMachine.State = (int)IngesteStates.StNormal;
        }
    }
}




