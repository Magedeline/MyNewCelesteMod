
namespace DesoloZantas.Core.Core
{
    public class TowerObstacle {
        private Vector2 vector2;
        private ObstacleType noneob;
        private MovementPattern nonemp;
        private Vector2 obstaclePosition;

        public TowerObstacle(Vector2 vector2, ObstacleType noneob, MovementPattern nonemp) {
            this.vector2 = vector2;
            this.noneob = noneob;
            this.nonemp = nonemp;
        }

        public TowerObstacle(Vector2 obstaclePosition) {
            this.obstaclePosition = obstaclePosition;
        }

        public enum ObstacleType {
            Spikes,
            MovingPlatform,
            None
        }
        public enum MovementPattern {
            Circular,
            Static,
            None
        }
        public ObstacleType Type { get; set; }
        public MovementPattern Pattern { get; set; }
        }
    }




