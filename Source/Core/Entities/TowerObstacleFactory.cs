namespace DesoloZantas.Core.Core.Entities
{
    public static class TowerObstacleFactory<TOwerObstacle, TOwer3D> where TOwerObstacle : new()
    {
        public enum BackgroundStyle
        {
            Default,
            Mystical,
            Dark,
            Crystal
        }
        public enum ObstacleSetType
        {
            Beginner,
            Intermediate,
            Advanced,
            Expert
        }
        public static TowerBackgroundStyleground CreateTowerBackground(Vector2 position, BackgroundStyle style)
        {
            var background = new TowerBackgroundStyleground(position);
            background.SetTintColor(style switch
            {
                BackgroundStyle.Mystical => new Color(0.6f, 0.4f, 0.8f),
                BackgroundStyle.Dark => new Color(0.3f, 0.3f, 0.4f),
                BackgroundStyle.Crystal => new Color(0.4f, 0.6f, 0.8f),
                _ => Color.White
            });
            return background;
        }
        public static List<TOwerObstacle> CreateObstacleSet(TitanTower3D tower, ObstacleSetType setType)
        {
            var obstacles = new List<TOwerObstacle>();
            int count = setType switch
            {
                ObstacleSetType.Beginner => 5,
                ObstacleSetType.Intermediate => 8,
                _ => 0
            };
            for (int i = 0; i < count; i++)
            {
                float height = setType == ObstacleSetType.Beginner ? 100 + i * 150 : 100 + i * 120;
                bool slot = tower.Equals(height);
                if (setType == ObstacleSetType.Intermediate)
                {
                    var type = i % 2 == 0 ? TowerObstacle.ObstacleType.Spikes : TowerObstacle.ObstacleType.MovingPlatform;
                    var pattern = i % 3 == 0 ? TowerObstacle.MovementPattern.Circular : TowerObstacle.MovementPattern.Static;
                }
                var obstacle = new TOwerObstacle();
                obstacles.Add(obstacle);
            }
            return obstacles;
        }
    }
}



