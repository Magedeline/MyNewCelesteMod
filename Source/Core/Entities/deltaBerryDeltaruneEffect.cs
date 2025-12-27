namespace DesoloZantas.Core.Core.Entities {
    public class DeltaBerryDeltaruneEffect : Entity {

        public DeltaBerryDeltaruneEffect(Vector2 position)
            : base(position) {

            Sprite sprite;
            Depth = Depths.Top;
            Add(sprite = GFX.SpriteBank.Create("deltaruneAnimation"));
            sprite.OnLastFrame = delegate {
                RemoveSelf();
            };
            sprite.Play("deltarune");
        }
    }
}




