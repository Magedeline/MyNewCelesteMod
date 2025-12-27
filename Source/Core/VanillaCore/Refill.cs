namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Custom Refill implementation ported from vanilla Celeste.
    /// Extended with DesoloZantas-specific features like pink refills.
    /// </summary>
    [CustomEntity("Ingeste/Refill")]
    public class Refill : Entity
    {
        public static ParticleType P_Shatter;
        public static ParticleType P_Regen;
        public static ParticleType P_Glow;
        public static ParticleType P_ShatterTwo;
        public static ParticleType P_RegenTwo;
        public static ParticleType P_GlowTwo;
        
        // DesoloZantas custom particles
        public static ParticleType P_ShatterPink;
        public static ParticleType P_RegenPink;
        public static ParticleType P_GlowPink;

        private Sprite sprite;
        private Sprite flash;
        private Image outline;
        private Wiggler wiggler;
        private BloomPoint bloom;
        private VertexLight light;
        private Level level;
        private SineWave sine;
        private bool twoDashes;
        private bool oneUse;
        private float respawnTimer;

        // DesoloZantas extensions
        public bool IsPinkRefill { get; private set; }
        public bool KirbyModeOnly { get; private set; }
        public bool GrantsFloatAbility { get; private set; }
        public int CustomDashCount { get; private set; }

        static Refill()
        {
            // Initialize pink particles
            P_ShatterPink = new ParticleType
            {
                Source = GFX.Game["particles/triangle"],
                Color = Color.HotPink,
                Color2 = Color.LightPink,
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.5f,
                LifeMax = 1f,
                Size = 1f,
                SpeedMin = 100f,
                SpeedMax = 200f,
                Direction = -(float)Math.PI / 2f,
                DirectionRange = (float)Math.PI
            };
            
            P_RegenPink = new ParticleType
            {
                Source = GFX.Game["particles/blob"],
                Color = Color.HotPink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.5f,
                LifeMax = 1f,
                Size = 0.7f,
                SpeedMin = 10f,
                SpeedMax = 20f,
                SpeedMultiplier = 0.1f,
                Direction = -(float)Math.PI / 2f,
                DirectionRange = (float)Math.PI / 4f
            };

            P_GlowPink = new ParticleType
            {
                Source = GFX.Game["particles/blob"],
                Color = Color.HotPink,
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.InAndOut,
                LifeMin = 0.6f,
                LifeMax = 1f,
                Size = 0.5f,
                SpeedMin = 4f,
                SpeedMax = 8f,
                DirectionRange = (float)Math.PI * 2f
            };
        }

        public Refill(Vector2 position, bool twoDashes, bool oneUse)
            : base(position)
        {
            Collider = new Hitbox(16f, 16f, -8f, -8f);
            this.twoDashes = twoDashes;
            this.oneUse = oneUse;
            
            string spriteId = twoDashes ? "refillTwo" : "refill";
            Add(sprite = GFX.SpriteBank.Create(spriteId));
            sprite.Play("idle");
            
            Add(flash = new Sprite(GFX.Game, "objects/refill/flash"));
            flash.Add("flash", "", 0.05f);
            flash.OnFinish = delegate { flash.Visible = false; };
            flash.CenterOrigin();
            flash.Visible = false;
            
            Add(wiggler = Wiggler.Create(1f, 4f, delegate(float v) 
            { 
                sprite.Scale = Vector2.One * (1f + v * 0.2f); 
            }));
            
            Add(new MirrorReflection());
            Add(bloom = new BloomPoint(0.8f, 16f));
            Add(light = new VertexLight(Color.White, 1f, 16, 48));
            Add(sine = new SineWave(0.6f, 0f));
            sine.Randomize();
            
            UpdateY();
            Depth = -100;
        }

        public Refill(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Bool("twoDash"), data.Bool("oneUse"))
        {
            // DesoloZantas extensions
            IsPinkRefill = data.Bool("pink", false);
            KirbyModeOnly = data.Bool("kirbyModeOnly", false);
            GrantsFloatAbility = data.Bool("grantsFloat", false);
            CustomDashCount = data.Int("customDashCount", twoDashes ? 2 : 1);

            // Apply pink variant if specified
            if (IsPinkRefill)
            {
                try
                {
                    Remove(sprite);
                    sprite = GFX.SpriteBank.Create("refillPink");
                    sprite.Play("idle");
                    Add(sprite);
                    // Note: BloomPoint doesn't have Color property, only Alpha
                    light.Color = Color.HotPink;
                }
                catch
                {
                    // Keep default sprite if pink not found
                }
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
        }

        public override void Update()
        {
            base.Update();
            
            // Check Kirby mode requirement
            if (KirbyModeOnly)
            {
                bool isKirbyMode = level?.Session.GetFlag("kirby_mode") ?? false;
                Collidable = isKirbyMode && respawnTimer <= 0f;
                Visible = isKirbyMode;
            }
            else
            {
                Collidable = respawnTimer <= 0f;
            }
            
            if (respawnTimer > 0f)
            {
                respawnTimer -= Engine.DeltaTime;
                if (respawnTimer <= 0f)
                {
                    Respawn();
                }
            }
            else if (Scene.OnInterval(0.1f))
            {
                var particleType = IsPinkRefill ? P_GlowPink : 
                                   (twoDashes ? P_GlowTwo : P_Glow);
                level.ParticlesFG.Emit(particleType ?? P_Glow, 1, Position, 
                    Vector2.One * 5f);
            }
            
            UpdateY();
            light.Alpha = Calc.Approach(light.Alpha, sprite.Visible ? 1f : 0f, 4f * Engine.DeltaTime);
            bloom.Alpha = light.Alpha * 0.8f;
            
            if (Scene.OnInterval(2f) && sprite.Visible)
            {
                flash.Play("flash", true);
                flash.Visible = true;
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
                
                var particleType = IsPinkRefill ? P_RegenPink : 
                                   (twoDashes ? P_RegenTwo : P_Regen);
                Audio.Play(twoDashes ? "event:/game/general/diamond_return" : 
                                       "event:/game/general/diamond_return", Position);
                level.ParticlesFG.Emit(particleType ?? P_Regen, 16, Position, Vector2.One * 2f);
            }
        }

        private void UpdateY()
        {
            sprite.Y = bloom.Y = sine.Value * 2f;
            flash.Y = sprite.Y;
        }

        public override void Render()
        {
            if (sprite.Visible)
            {
                sprite.DrawOutline();
            }
            base.Render();
        }

        private void OnPlayer(global::Celeste.Player player)
        {
            // Apply custom dash count if set
            int dashesToGrant = CustomDashCount > 0 ? CustomDashCount : (twoDashes ? 2 : 1);
            player.RefillDash();
            
            if (dashesToGrant > 1)
            {
                for (int i = 1; i < dashesToGrant && player.Dashes < player.MaxDashes + dashesToGrant - 1; i++)
                {
                    player.Dashes++;
                }
            }
            
            player.RefillStamina();
            
            // Grant float ability if configured
            if (GrantsFloatAbility)
            {
                level.Session.SetFlag("has_float_ability", true);
            }
            
            Audio.Play(twoDashes ? "event:/game/general/diamond_touch" : 
                                   "event:/game/general/diamond_touch", Position);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            Collidable = false;
            
            Add(new Coroutine(RefillRoutine(player)));
            respawnTimer = 2.5f;
        }

        private IEnumerator RefillRoutine(global::Celeste.Player player)
        {
            Celeste.Celeste.Freeze(0.05f);
            yield return null;
            
            level.Shake();
            sprite.Visible = false;
            
            if (!oneUse)
            {
                outline.Visible = true;
            }
            
            Depth = 8999;
            
            var particleType = IsPinkRefill ? P_ShatterPink : 
                               (twoDashes ? P_ShatterTwo : P_Shatter);
            level.ParticlesFG.Emit(particleType ?? P_Shatter, 5, Position, Vector2.One * 4f, 
                -(float)Math.PI / 2f);
            level.ParticlesFG.Emit(particleType ?? P_Shatter, 5, Position, Vector2.One * 4f, 
                (float)Math.PI / 2f);
            
            SlashFx.Burst(Position, 0f);
            
            if (oneUse)
            {
                RemoveSelf();
            }
        }
    }
}
