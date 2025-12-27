namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/WarpStar")]
    [Tracked]
    public class WarpStar : Entity
    {
        public static ParticleType P_Collect;

        public static ParticleType P_Boost;

        public static ParticleType P_Flying;

        public static ParticleType P_Respawn;

        private const float RespawnTime = 3f;

        private Sprite sprite;

        private Image outline;

        private Wiggler wiggler;

        private BloomPoint bloom;

        private VertexLight light;

        private Level level;

        private SineWave sine;

        private bool shielded;

        private bool singleUse;

        private Wiggler shieldRadiusWiggle;

        private Wiggler moveWiggle;

        private Vector2 moveWiggleDir;

        private float respawnTimer;

        public WarpStar(Vector2 position, bool shielded, bool singleUse)
            : base(position)
        {
            this.shielded = shielded;
            this.singleUse = singleUse;
            base.Collider = new Hitbox(20f, 20f, -10f, -10f);
            Add(new PlayerCollider(OnPlayer));
            Add(sprite = GFX.SpriteBank.Create("warpstars"));
            Add(wiggler = Wiggler.Create(1f, 4f, (float v) =>
            {
                sprite.Scale = Vector2.One * (1f + v * 0.2f);
            }));
            Add(bloom = new BloomPoint(0.5f, 20f));
            Add(light = new VertexLight(Color.White, 1f, 16, 48));
            Add(sine = new SineWave(0.6f, 0f).Randomize());
            Add(outline = new Image(GFX.Game["objects/warpstars/outline"]));
            outline.CenterOrigin();
            outline.Visible = false;
            shieldRadiusWiggle = Wiggler.Create(0.5f, 4f);
            Add(shieldRadiusWiggle);
            moveWiggle = Wiggler.Create(0.8f, 2f);
            moveWiggle.StartZero = true;
            Add(moveWiggle);
            UpdateY();
        }
        public WarpStar(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Bool("shielded"), data.Bool("singleUse"))
        {
        }
        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
        }
        public override void Update()
        {
            base.Update();
            if (respawnTimer > 0f)
            {
                respawnTimer -= Engine.DeltaTime;
                if (respawnTimer <= 0f)
                {
                    Respawn();
                }
            }
            UpdateY();
            light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
            bloom.Alpha = light.Alpha * 0.8f;
        }
        public override void Render()
        {
            base.Render();
            if (shielded && sprite.Visible)
            {
                Draw.Circle(Position + sprite.Position, 10f - shieldRadiusWiggle.Value * 2f, Color.White, 3);
            }
        }

        private void Respawn()
        {
            if (!Collidable)
            {
                outline.Visible = false;
                Collidable = true;
                sprite.Visible = true;
                wiggler.Start();
                Audio.Play("event:/Ingeste/game/08_outrun/warpstar_reappear", Position);
                level.ParticlesFG.Emit(P_Respawn, 16, Position, Vector2.One * 2f);
            }
        }
        private void UpdateY()
        {
            sprite.X = 0f;
            Sprite obj = sprite;
            float y = (bloom.Y = sine.Value * 2f);
            obj.Y = y;
            sprite.Position += moveWiggleDir * moveWiggle.Value * -8f;
        }

        private void OnPlayer(global::Celeste.Player player)
        {
            Vector2 speed = player.Speed;
            if (shielded && !player.DashAttacking)
            {
                player.PointBounce(base.Center);
                moveWiggle.Start();
                shieldRadiusWiggle.Start();
                moveWiggleDir = (base.Center - player.Center).SafeNormalize(Vector2.UnitY);
                Audio.Play("event:/Ingeste/game/08_outrun/warpstar_bubble_bounce", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
                return;
            }
            bool flag = player.StateMachine.State == 19;
            if (player.StartStarFly())
            {
                if (!flag)
                {
                    Audio.Play(shielded ? "event:/Ingeste/game/08_outrun/warpstar_get" : "event:/Ingeste/game/08_outrun/warpstar_bubble_get", Position);
                }
                else
                {
                    Audio.Play(shielded ? "event:/Ingeste/game/08_outrun/warpstar_bubble_renew" : "event:/Ingeste/game/08_outrun/warpstar_renew", Position);
                }
                Collidable = false;
                Add(new Coroutine(CollectRoutine(player, speed)));
                if (!singleUse)
                {
                    outline.Visible = true;
                    respawnTimer = 3f;
                }
            }
        }
        private IEnumerator CollectRoutine(global::Celeste.Player player, Vector2 playerSpeed)
        {
            level.Shake();
            sprite.Visible = false;
            yield return 0.05f;
            float direction = ((!(playerSpeed != Vector2.Zero)) ? (Position - player.Center).Angle() : playerSpeed.Angle());
            level.ParticlesFG.Emit(P_Collect, 10, Position, Vector2.One * 6f);
            SlashFx.Burst(Position, direction);
        }
    }
}



