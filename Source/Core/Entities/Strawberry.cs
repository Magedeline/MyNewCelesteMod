namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("strawberry")]
    [Monocle.Tracked]
    public class Strawberry : Actor
    {
        public static ParticleType PGlow;
        public static ParticleType PGhostGlow;
        public static ParticleType PWingsBurst;
        public static ParticleType P_WingsBurst;
        public static ParticleType PCollectGlow;
        
        // Golden strawberry flag (instance property)
        public bool Golden { get; set; }

        private Monocle.Sprite sprite;
        private Wiggler wiggler;
        private BloomPoint bloom;
        private VertexLight light;
        private float wobble = 0f;
        private Vector2 start;
        private float collectTimer;
        private bool collected;
        private bool isGhostBerry = false;
        private EntityID id;

        public int CheckpointId { get; private set; } = -1;
        public int Order { get; private set; } = -1;

        public Strawberry(EntityData data, Vector2 offset, EntityID id)
            : base(data.Position + offset)
        {
            this.id = id;
            CheckpointId = data.Int("checkpointID", -1);
            Order = data.Int("order", -1);
            isGhostBerry = data.Bool("moon", false);

            Depth = -100;
            Collider = new Monocle.Hitbox(14f, 14f, -7f, -10f);
            start = Position;

            Add(sprite = GFX.SpriteBank.Create(isGhostBerry ? "moonghostberry" : "strawberry"));
            Add(wiggler = Wiggler.Create(0.4f, 4f, delegate(float v)
            {
                sprite.Scale = Vector2.One * (1f + v * 0.35f);
            }));
            Add(new PlayerCollider(OnPlayer));
            Add(light = new VertexLight(isGhostBerry ? Calc.HexToColor("99ffff") : Color.White, 1f, 16, 24));
            Add(bloom = new BloomPoint(isGhostBerry ? 0.5f : 1f, 12f));
            
            if (isGhostBerry)
            {
                sprite.Color = Color.White * 0.8f;
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            // Check if already collected
            var level = scene as Level;
            if (level != null)
            {
                var session = level.Session;
                if (session.Strawberries.Contains(id))
                {
                    RemoveSelf();
                    return;
                }
            }
        }

        public override void Update()
        {
            if (!collected)
            {
                wobble += Engine.DeltaTime * 4f;
                sprite.Y = (float)Math.Sin(wobble) * 2f;
            }

            if (collected)
            {
                collectTimer += Engine.DeltaTime;
                if (collectTimer > 0.3f)
                {
                    RemoveSelf();
                }
            }

            base.Update();
        }

        private void OnPlayer(global::Celeste.Player player)
        {
            if (collected)
                return;

            Audio.Play(isGhostBerry ? "event:/game/general/seed_poof" : "event:/game/general/strawberry_get", Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);

            collected = true;
            Collidable = false;
            
            // Add to session
            var level = Scene as Level;
            if (level != null)
            {
                level.Session.Strawberries.Add(id);
                level.Session.UpdateLevelStartDashes();
            }

            // Visual effects
            wiggler.Start();
            
            Add(new Coroutine(collectRoutine(player)));
        }

        private System.Collections.IEnumerator collectRoutine(global::Celeste.Player player)
        {
            var level = Scene as Level;
            
            // Ensure particles are initialized before using them
            if (PGlow == null || PGhostGlow == null || PCollectGlow == null)
            {
                LoadParticles();
            }
            
            // Particle effects
            level?.ParticlesFG?.Emit(isGhostBerry ? PGhostGlow : PGlow, 5, Position, Vector2.One * 8f);
            
            // Tween to player
            Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.25f, true);
            tween.OnUpdate = delegate(Tween t)
            {
                Position = Vector2.Lerp(start, player.Center, t.Eased);
            };
            
            Add(tween);
            
            yield return 0.25f;
            
            // Final collection effects
            level?.ParticlesFG?.Emit(PCollectGlow, 8, player.Center, Vector2.One * 4f);
        }

        public static void LoadParticles()
        {
            PGlow = new ParticleType
            {
                Size = 1f,
                Color = Calc.HexToColor("FC4A9B"),
                Color2 = Calc.HexToColor("FF6AC1"),
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.8f,
                LifeMax = 1.4f,
                SpeedMin = 8f,
                SpeedMax = 16f,
                DirectionRange = (float)Math.PI * 2f
            };
            
            PGhostGlow = new ParticleType
            {
                Size = 1f,
                Color = Calc.HexToColor("99ffff"),
                Color2 = Calc.HexToColor("ccffff"),
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.8f,
                LifeMax = 1.4f,
                SpeedMin = 8f,
                SpeedMax = 16f,
                DirectionRange = (float)Math.PI * 2f
            };
            
            PCollectGlow = new ParticleType(PGlow)
            {
                SpeedMin = 12f,
                SpeedMax = 24f
            };
        }

        internal void OnCollect()
        {
            // Find the player in the scene
            var player = Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            if (player != null && !collected)
            {
                OnPlayer(player);
            }
        }
    }
}



