namespace DesoloZantas.Core.Core.Entities
{
    // Note: Abstract class - cannot have [CustomEntity] attribute as Everest cannot instantiate abstract classes
    [Tracked(true)]
    public abstract class StarBlock : Entity
    {
        private bool isBroken;
        private int width;
        private int height;
        private const int grid_size = 8;

        public StarBlock(Vector2 position, int width, int height) : base(position)
        {
            this.width = width;
            this.height = height;
            Collider = new Hitbox(width, height, -width / 2f, -height / 2f);
            Add(new PlayerCollider(OnPlayer));
        }

        public Vector2 Speed { get; set; }

        private void OnPlayer(global::Celeste.Player player)
        {
            if (!isBroken && Collider.Collide(player.Collider))
            {
                Audio.Play("event:/game/general/diamond_touch", Position);
                player.Bounce(1.5f);
                Break();
            }
        }

        public override void Update()
        {
            base.Update();
            var player = Scene.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null && player.StateMachine.State == (int)global::Celeste.Player.BoostTime && Collider.Collide(player.Collider))
            {
                Break();
            }
        }

        public void Break()
        {
            if (isBroken) return;
            isBroken = true;
            Audio.Play("event:/game/general/diamond_break", Position);
            for (int i = 0; i < 10; i++)
            {
                Scene.Add(new Particle(Position, Calc.Random.Choose(Color.Yellow, Color.Orange, Color.Red)));
            }
            RemoveSelf();
        }

        public override void Render()
        {
            base.Render();
            Draw.Rect(Collider.Bounds, Color.Yellow);
        }

        public void Resize(int newWidth, int newHeight)
        {
            width = snapToGrid(newWidth);
            height = snapToGrid(newHeight);
            Collider.Width = width;
            Collider.Height = height;
            Collider.Position = new Vector2(-width / 2f, -height / 2f);
        }

        private int snapToGrid(int value)
        {
            return (value / grid_size) * grid_size;
        }

        private class Particle : Entity
        {
            private Vector2 velocity;
            private Color color;
            private float timer;

            public Particle(Vector2 position, Color color) : base(position)
            {
                this.color = color;
                velocity = Calc.Random.Range(new Vector2(-50f, -50f), new Vector2(50f, 50f));
                timer = 0.5f;
            }

            public override void Update()
            {
                base.Update();
                Position += velocity * Engine.DeltaTime;
                velocity *= 0.9f;
                timer -= Engine.DeltaTime;
                if (timer <= 0) RemoveSelf();
            }

            public override void Render()
            {
                Draw.Rect(Position.X - 2, Position.Y - 2, 4, 4, color);
            }
        }
    }
}




