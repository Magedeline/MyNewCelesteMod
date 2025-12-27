namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Custom Booster implementation ported from vanilla Celeste.
    /// Extended with DesoloZantas-specific features.
    /// </summary>
    [CustomEntity("Ingeste/Booster")]
    public class Booster : Entity
    {
        private const float RespawnTime = 1f;
        
        public static ParticleType P_Burst;
        public static ParticleType P_BurstRed;
        public static ParticleType P_BurstPink; // DesoloZantas custom
        public static ParticleType P_Appear;
        public static ParticleType P_RedAppear;
        public static ParticleType P_PinkAppear; // DesoloZantas custom
        public static readonly Vector2 PlayerOffset = new Vector2(0f, -2f);

        private Sprite sprite;
        private Entity outline;
        private Wiggler wiggler;
        private BloomPoint bloom;
        private VertexLight light;
        private Coroutine dashRoutine;
        private DashListener dashListener;
        private ParticleType particleType;
        private float respawnTimer;
        private float cannotUseTimer;
        private bool red;
        private bool pink; // DesoloZantas: Pink variant for special boosts
        private SoundSource loopingSfx;
        
        // DesoloZantas extensions
        public bool KirbyModeOnly { get; private set; }
        public bool GrantsExtraDash { get; private set; }
        public string CustomSpriteId { get; private set; }
        public bool Ch9HubBooster;
        public bool Ch9HubTransition;

        public bool BoostingPlayer { get; private set; }

        static Booster()
        {
            // Initialize pink particle type for DesoloZantas
            P_BurstPink = new ParticleType
            {
                Source = GFX.Game["particles/bubble"],
                Color = Color.HotPink,
                Color2 = Color.LightPink,
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.5f,
                LifeMax = 1f,
                Size = 0.8f,
                SpeedMin = 20f,
                SpeedMax = 40f,
                Direction = -(float)Math.PI / 2f,
                DirectionRange = (float)Math.PI / 4f
            };
        }

        public Booster(Vector2 position, bool red, bool pink = false, bool kirbyModeOnly = false)
            : base(position)
        {
            Depth = -8500;
            Collider = new Circle(10f, y: 2f);
            this.red = red;
            this.pink = pink;
            KirbyModeOnly = kirbyModeOnly;

            // Choose sprite based on type
            string spriteId = pink ? "boosterPink" : (red ? "boosterRed" : "booster");
            
            // Try custom sprite first
            try
            {
                sprite = GFX.SpriteBank.Create(spriteId);
            }
            catch
            {
                // Fallback to vanilla
                sprite = GFX.SpriteBank.Create(red ? "boosterRed" : "booster");
            }
            Add(sprite);

            Add(new PlayerCollider(OnPlayer));
            Add(light = new VertexLight(Color.White, 1f, 16, 32));
            Add(bloom = new BloomPoint(0.1f, 16f));
            Add(wiggler = Wiggler.Create(0.5f, 4f, f => sprite.Scale = Vector2.One * (1f + f * 0.25f)));
            Add(dashRoutine = new Coroutine(false));
            Add(dashListener = new DashListener());
            Add(new MirrorReflection());
            Add(loopingSfx = new SoundSource());

            dashListener.OnDash = OnPlayerDashed;
            particleType = pink ? P_BurstPink : (red ? P_BurstRed : P_Burst);
        }

        public Booster(EntityData data, Vector2 offset)
            : this(data.Position + offset, 
                   data.Bool("red"), 
                   data.Bool("pink", false),
                   data.Bool("kirbyModeOnly", false))
        {
            Ch9HubBooster = data.Bool("ch9_hub_booster");
            GrantsExtraDash = data.Bool("grantsExtraDash", false);
            CustomSpriteId = data.Attr("customSpriteId", "");

            // Apply custom sprite if specified
            if (!string.IsNullOrEmpty(CustomSpriteId))
            {
                try
                {
                    Remove(sprite);
                    sprite = GFX.SpriteBank.Create(CustomSpriteId);
                    Add(sprite);
                }
                catch
                {
                    // Keep default
                }
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            // Create outline
            Image image = new Image(GFX.Game["objects/booster/outline"]);
            image.CenterOrigin();
            image.Color = Color.White * 0.75f;
            outline = new Entity(Position);
            outline.Depth = 8999;
            outline.Visible = false;
            outline.Add(image);
            outline.Add(new MirrorReflection());
            scene.Add(outline);
        }

        public override void Update()
        {
            base.Update();

            // Check Kirby mode requirement
            if (KirbyModeOnly)
            {
                Level level = Scene as Level;
                bool isKirbyMode = level?.Session.GetFlag("kirby_mode") ?? false;
                Collidable = isKirbyMode;
                Visible = isKirbyMode;
            }

            // Handle respawn timer
            if (respawnTimer > 0f)
            {
                respawnTimer -= Engine.DeltaTime;
                if (respawnTimer <= 0f)
                {
                    Appear();
                }
            }

            // Handle cannot use timer
            if (cannotUseTimer > 0f)
            {
                cannotUseTimer -= Engine.DeltaTime;
            }
        }

        public void Appear()
        {
            Audio.Play(red ? "event:/game/05_mirror_temple/redbooster_reappear" : 
                           "event:/game/04_cliffside/greenbooster_reappear", Position);
            sprite.Play("appear");
            wiggler.Start();
            Visible = true;
            AppearParticles();
        }

        private void AppearParticles()
        {
            ParticleSystem particlesBg = SceneAs<Level>().ParticlesBG;
            ParticleType appearType = pink ? P_PinkAppear : (red ? P_RedAppear : P_Appear);
            
            for (int i = 0; i < 360; i += 30)
            {
                particlesBg.Emit(appearType ?? P_Appear, 1, Center, Vector2.One * 2f, 
                    i * ((float)Math.PI / 180f));
            }
        }

        private void OnPlayer(global::Celeste.Player player)
        {
            if (respawnTimer > 0f || cannotUseTimer > 0f || BoostingPlayer)
                return;

            cannotUseTimer = 0.45f;
            
            // Grant extra dash if configured
            if (GrantsExtraDash)
            {
                player.Dashes = Math.Min(player.Dashes + 1, player.MaxDashes + 1);
            }

            // Note: Player expects vanilla Celeste.Booster type
            // This is a custom booster that provides similar functionality
            // but doesn't directly call player.Boost/RedBoost

            Audio.Play(red ? "event:/game/05_mirror_temple/redbooster_enter" : 
                           "event:/game/04_cliffside/greenbooster_enter", Position);
            wiggler.Start();
            sprite.Play("inside");
            sprite.FlipX = player.Facing == Facings.Left;
        }

        private void OnPlayerDashed(Vector2 dir)
        {
            if (!BoostingPlayer)
                return;
            BoostingPlayer = false;
        }

        public void PlayerBoosted(global::Celeste.Player player, Vector2 direction)
        {
            Audio.Play(red ? "event:/game/05_mirror_temple/redbooster_dash" : 
                           "event:/game/04_cliffside/greenbooster_dash", Position);
            
            if (red)
            {
                loopingSfx.Play("event:/game/05_mirror_temple/redbooster_move");
                loopingSfx.DisposeOnTransition = false;
            }

            BoostingPlayer = true;
            Tag = (int)Tags.Persistent | (int)Tags.TransitionUpdate;
            sprite.Play("spin");
            sprite.FlipX = player.Facing == Facings.Left;
            outline.Visible = true;
            wiggler.Start();
            dashRoutine.Replace(BoostRoutine(player, direction));
        }

        private IEnumerator BoostRoutine(global::Celeste.Player player, Vector2 dir)
        {
            float angle = (-dir).Angle();
            
            while ((player.StateMachine.State == 2 || player.StateMachine.State == 5) && BoostingPlayer)
            {
                sprite.RenderPosition = player.Center + PlayerOffset;
                loopingSfx.Position = sprite.Position;
                
                if (Scene.OnInterval(0.02f))
                {
                    SceneAs<Level>().ParticlesBG.Emit(particleType, 2, player.Center - dir * 3f + 
                        new Vector2(0f, -2f), new Vector2(3f, 3f), angle);
                }
                yield return null;
            }

            PlayerReleased();
            
            if (player.StateMachine.State == 4)
            {
                sprite.Visible = false;
            }
            
            while (SceneAs<Level>().Transitioning)
            {
                yield return null;
            }

            Tag = 0;
            respawnTimer = RespawnTime;
            outline.Visible = false;
            Visible = false;
        }

        public void PlayerReleased()
        {
            Audio.Play(red ? "event:/game/05_mirror_temple/redbooster_end" : 
                           "event:/game/04_cliffside/greenbooster_end", sprite.RenderPosition);
            sprite.Play("pop");
            loopingSfx.Stop();
            BoostingPlayer = false;
        }

        public void PlayerDied()
        {
            if (!BoostingPlayer)
                return;
            
            PlayerReleased();
            dashRoutine.Cancel();
            Tag = 0;
        }
    }
}
