namespace DesoloZantas.Core.Core
{
    // Auto-animator for Kirby: no laughing, angry, or floating animations
    public class KirbyAutoAnimator : Component
    {
        private string lastAnimation;
        private bool wasSyncingSprite;
        private Wiggler pop;

        public KirbyAutoAnimator(bool wasSyncingSprite) : base(true, false)
        {
            this.wasSyncingSprite = wasSyncingSprite;
            lastAnimation = "idle";
        }

        public override void Added(Entity entity)
        {
            base.Added(entity);
            entity.Add(pop = Wiggler.Create(0.5f, 4f, f =>
            {
                Sprite sprite = Entity.Get<Sprite>();
                if (sprite == null)
                    return;
                sprite.Scale = new Microsoft.Xna.Framework.Vector2(Math.Sign(sprite.Scale.X), 1f) * (float)(1.0 + 0.25 * f);
            }));
        }

        public override void Removed(Entity entity)
        {
            entity.Remove(pop);
            base.Removed(entity);
        }

        public void SetReturnToAnimation(string anim) => lastAnimation = anim;

        public override void Update()
        {
            Sprite sprite = Entity.Get<Sprite>();
            if (Scene == null || sprite == null)
                return;

            // No laughing, angry, or floating logic for Kirby
            // Only handle basic animation fallback

            if (string.IsNullOrEmpty(lastAnimation) || lastAnimation == "spin")
                lastAnimation = "idle";
            pop.Start();
            sprite.Play(lastAnimation);
        }
    }
}



