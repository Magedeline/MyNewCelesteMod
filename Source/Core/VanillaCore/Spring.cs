namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Custom Spring implementation ported from vanilla Celeste.
    /// </summary>
    [CustomEntity("Ingeste/Spring")]
    public class Spring : Entity
    {
        public enum Orientations
        {
            Floor,
            WallLeft,
            WallRight
        }

        public static ParticleType P_Bounce;
        public static ParticleType P_BounceSpecial; // DesoloZantas custom

        private Sprite sprite;
        private Wiggler wiggler;
        private StaticMover staticMover;
        
        public Orientations Orientation;
        private bool playerCanUse;

        // DesoloZantas extensions
        public bool IsPowered { get; private set; }
        public float BounceMultiplier { get; private set; } = 1f;
        public bool KirbyModeBoost { get; private set; }
        public string CustomSound { get; private set; }

        static Spring()
        {
            P_BounceSpecial = new ParticleType
            {
                Source = GFX.Game["particles/blob"],
                Color = Color.Cyan,
                Color2 = Color.LightCyan,
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.3f,
                LifeMax = 0.6f,
                Size = 0.8f,
                SpeedMin = 40f,
                SpeedMax = 80f,
                Direction = -(float)Math.PI / 2f,
                DirectionRange = (float)Math.PI / 6f
            };
        }

        public Spring(Vector2 position, Orientations orientation, bool playerCanUse)
            : base(position)
        {
            Orientation = orientation;
            this.playerCanUse = playerCanUse;
            
            // Use SpriteBank for spring sprite
            try
            {
                sprite = GFX.SpriteBank.Create("spring");
            }
            catch
            {
                sprite = new Sprite(GFX.Game, "objects/spring/");
                sprite.AddLoop("idle", "00", 0f);
                sprite.AddLoop("bounce", "00", 0.07f);
                sprite.AddLoop("disabled", "00", 0f);
            }
            Add(sprite);
            sprite.Play("idle");
            sprite.Origin.X = sprite.Width / 2f;
            sprite.Origin.Y = sprite.Height;
            
            Depth = -8501;

            Add(wiggler = Wiggler.Create(1f, 4f, delegate(float v)
            {
                sprite.Scale.Y = 1f + v * 0.2f;
            }));

            switch (orientation)
            {
                case Orientations.Floor:
                    Collider = new Hitbox(16f, 6f, -8f, -6f);
                    Add(new PlayerCollider(OnCollide));
                    Add(new HoldableCollider(OnHoldable));
                    break;
                case Orientations.WallLeft:
                    Collider = new Hitbox(6f, 16f, 0f, -8f);
                    Add(new PlayerCollider(OnCollide));
                    Add(new HoldableCollider(OnHoldable));
                    sprite.Rotation = (float)Math.PI / 2f;
                    break;
                case Orientations.WallRight:
                    Collider = new Hitbox(6f, 16f, -6f, -8f);
                    Add(new PlayerCollider(OnCollide));
                    Add(new HoldableCollider(OnHoldable));
                    sprite.Rotation = -(float)Math.PI / 2f;
                    break;
            }

            staticMover = new StaticMover();
            staticMover.OnShake = OnShake;
            staticMover.SolidChecker = s => CollideCheck(s, Position - Vector2.UnitY);
            staticMover.JumpThruChecker = jt => CollideCheck(jt, Position - Vector2.UnitY);
            Add(staticMover);
        }

        public Spring(EntityData data, Vector2 offset)
            : this(data.Position + offset, 
                   data.Enum("orientation", Orientations.Floor), 
                   data.Bool("playerCanUse", true))
        {
            // DesoloZantas extensions
            IsPowered = data.Bool("powered", true);
            BounceMultiplier = data.Float("bounceMultiplier", 1f);
            KirbyModeBoost = data.Bool("kirbyModeBoost", false);
            CustomSound = data.Attr("customSound", "");

            if (!IsPowered)
            {
                sprite.Play("disabled");
                Collidable = false;
            }
        }

        private void OnShake(Vector2 amount)
        {
            sprite.Position += amount;
        }

        private void OnCollide(global::Celeste.Player player)
        {
            if (!playerCanUse || !IsPowered)
                return;

            bool isKirbyMode = (Scene as Level)?.Session.GetFlag("kirby_mode") ?? false;
            float multiplier = BounceMultiplier;
            
            // Apply Kirby mode boost if enabled
            if (KirbyModeBoost && isKirbyMode)
            {
                multiplier *= 1.5f;
            }

            switch (Orientation)
            {
                case Orientations.Floor:
                    if (player.Speed.Y >= 0f)
                    {
                        BounceAnimate();
                        // Apply bounce with multiplier
                        player.SuperBounce(Top);
                    }
                    break;
                case Orientations.WallLeft:
                    if (player.SideBounce(1, Right, CenterY))
                        BounceAnimate();
                    break;
                case Orientations.WallRight:
                    if (player.SideBounce(-1, Left, CenterY))
                        BounceAnimate();
                    break;
            }
        }

        private void OnHoldable(Holdable h)
        {
            if (!IsPowered)
                return;

            HitSpinner(h);
        }

        private void HitSpinner(Holdable h)
        {
            // Spring was hit by holdable - animate bounce
            BounceAnimate();
        }

        private void BounceAnimate()
        {
            string sound = !string.IsNullOrEmpty(CustomSound) ? 
                CustomSound : "event:/game/general/spring";
            Audio.Play(sound, BottomCenter);
            
            sprite.Play("bounce", true);
            wiggler.Start();
            
            // Emit particles
            Level level = Scene as Level;
            if (level != null)
            {
                ParticleType particleType = IsPowered && BounceMultiplier > 1f ? 
                    P_BounceSpecial : P_Bounce;
                
                Vector2 direction = Orientation switch
                {
                    Orientations.Floor => -Vector2.UnitY,
                    Orientations.WallLeft => Vector2.UnitX,
                    Orientations.WallRight => -Vector2.UnitX,
                    _ => -Vector2.UnitY
                };
                
                level.ParticlesFG.Emit(particleType ?? P_Bounce, 12, Center, 
                    new Vector2(4f, 4f), direction.Angle());
            }
        }

        /// <summary>
        /// Enable or disable the spring
        /// </summary>
        public void SetPowered(bool powered)
        {
            IsPowered = powered;
            Collidable = powered;
            sprite.Play(powered ? "idle" : "disabled");
        }

        public override void Render()
        {
            if (Collidable && IsPowered)
            {
                sprite.DrawOutline();
            }
            base.Render();
        }
    }
}
