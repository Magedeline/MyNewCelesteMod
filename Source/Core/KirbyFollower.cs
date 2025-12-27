namespace DesoloZantas.Core.Core
{
    [Tracked(false)]
    public class KirbyFollower : Entity
    {
        public Entities.PlayerSprite Sprite;
        private int index;
        private global::Celeste.Player player;
        private bool following;
        private float followBehindTime;
        private float followBehindIndexDelay;
        public bool Hovering;

        public KirbyFollower(Vector2 position, int index) : base(position)
        {
            this.index = index;
            Depth = -1;
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Collidable = false;
            Sprite = new Entities.PlayerSprite(PlayerSpriteMode.Kirby); // Use Kirby sprite mode
            Sprite.Play("walk", true, false);
            Add(Sprite);
            Visible = false;
            followBehindTime = 1.55f;
            followBehindIndexDelay = 0.4f * index;
            Add(new PlayerCollider(OnPlayer, null, null));
        }

        public KirbyFollower(EntityData data, Vector2 offset, int index) : this(data.Position + offset, index) { }

        public KirbyFollower(Vector2 playerPosition)
        {
            Position = playerPosition;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Add(new Coroutine(StartChasingRoutine(scene as Level), true));
        }

        private IEnumerator StartChasingRoutine(Level level)
        {
            Hovering = true;
            while ((player = Scene.Tracker.GetEntity<global::Celeste.Player>()) == null || player.JustRespawned)
                yield return null;
            Vector2 to = player.Position;
            yield return followBehindIndexDelay;
            if (!Visible)
                PopIntoExistence(0.5f);
            Sprite.Play("walk", false, false);
            Hovering = false;
            yield return TweenToPlayer(to);
            Collidable = true;
            following = true;
            yield break;
        }

        private IEnumerator TweenToPlayer(Vector2 to)
        {
            Vector2 from = Position;
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, followBehindTime - 0.1f, true);
            tween.OnUpdate = t =>
            {
                Position = Vector2.Lerp(from, to, t.Eased);
                if (to.X != from.X)
                    Sprite.Scale.X = Math.Abs(Sprite.Scale.X) * Math.Sign(to.X - from.X);
            };
            Add(tween);
            yield return tween.Duration;
        }

        public override void Update()
        {
            if (player != null && following)
            {
                Position = Calc.Approach(Position, player.Position, 500f * Engine.DeltaTime);
                Sprite.Scale.X = Math.Sign(player.Position.X - Position.X);
            }
            base.Update();
        }

        private void OnPlayer(global::Celeste.Player player)
        {
            // Do nothing, or add a friendly interaction here
        }

        private void PopIntoExistence(float duration)
        {
            Visible = true;
            Sprite.Scale = Vector2.Zero;
            Sprite.Color = Color.Transparent;
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeIn, duration, true);
            tween.OnUpdate = t =>
            {
                Sprite.Scale = Vector2.One * t.Eased;
                Sprite.Color = Color.White * t.Eased;
            };
            Add(tween);
        }
    }
}



