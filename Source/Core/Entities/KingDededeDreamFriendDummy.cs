namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// King Dedede Dream Friend - Hammer with elemental infusion abilities
    /// </summary>
    [CustomEntity("Ingeste/KingDededeDreamFriend")]
    public class KingDededeDreamFriendDummy : Entity
    {
        public static ParticleType P_HammerSmash = new ParticleType
        {
            Size = 1f,
            Color = Color.Gold,
            Color2 = Color.Orange,
            ColorMode = ParticleType.ColorModes.Fade,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.8f,
            LifeMax = 1.4f,
            SpeedMin = 20f,
            SpeedMax = 40f,
            DirectionRange = (float)Math.PI * 2f,
            Acceleration = new Vector2(0f, -20f)
        };

        // Visual components
        public PlayerSprite Sprite;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;

        // Hammer-specific properties
        private float hammerChargeTime = 0f;
        private const float MAX_HAMMER_CHARGE = 2.0f;
        private bool isChargingHammer = false;
        private SoundSource hammerSound;
        private float shockwaveRadius = 80f;
        
    // Elemental system
    private ElementalType currentElement = ElementalType.Sizzle;
        private int currentElementIndex = 0;
        private float elementSwitchCooldown = 0f;
        private const float ELEMENT_SWITCH_COOLDOWN = 0.5f;
        private List<ElementalType> availableElements;
        
        // Following behavior
        protected global::Celeste.Player player;
        protected bool following = false;
        protected float followDistance = 50f;

        public KingDededeDreamFriendDummy(Vector2 position, int index = 0) : base(position)
        {
        }

        // Standard Everest constructor for map loading
        public KingDededeDreamFriendDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            // Initialize elemental abilities
            availableElements = new List<ElementalType>
            {
                ElementalType.Sizzle,    // Fire Hammer
                ElementalType.Blizzard,  // Ice Hammer  
                ElementalType.Splash,    // Water Hammer
                ElementalType.Bluster    // Wind Hammer
            };
            
            SetElementalAbility(currentElement);

            Collider = new Hitbox(6f, 6f, -3f, -7f);
            SetupVisuals();
            SetupMovement();
        }

        private void SetupVisuals()
        {
            // Create King Dedede sprite
            Sprite = new PlayerSprite(PlayerSpriteMode.KingDDD);
            Sprite.Play("idle", false, false);
            Sprite.Scale.X = -1f; // Face left initially
            
            Add(Sprite);
            
            // Create auto animator
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            // Create floating wave effect
            Add(Wave = new SineWave(0.2f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = Vector2.UnitY * f * 2.5f;
            };
            
            // Create light with royal colors
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Gold, 1f, 24, 64));
            
            // Create hammer sound
            hammerSound = new SoundSource();
            Add(hammerSound);
        }

        private void SetupMovement()
        {
            Add(new PlayerCollider(OnPlayer, null, null));
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            if (player.StateMachine.State == global::Celeste.Player.StNormal)
            {
                // Hammer boost ability - hold grab to charge, release to attack
                if (Input.Grab.Pressed && !isChargingHammer)
                {
                    StartHammerCharge();
                }
                else if (Input.Grab.Released && isChargingHammer)
                {
                    ExecuteHammerAttack(player);
                }
                
                // Element switching - press jump to cycle elements
                if (Input.Jump.Pressed && elementSwitchCooldown <= 0f)
                {
                    CycleElementalHammer();
                }
            }
        }

        private void StartHammerCharge()
        {
            isChargingHammer = true;
            hammerChargeTime = 0f;
            Sprite.Play("charge", false, false);
            hammerSound.Play("event:/char/madeline/grab");
            
            // Visual charging effect
            Add(new Coroutine(HammerChargeRoutine()));
        }

        private IEnumerator HammerChargeRoutine()
        {
            while (isChargingHammer && hammerChargeTime < MAX_HAMMER_CHARGE)
            {
                hammerChargeTime += Engine.DeltaTime;
                
                // Emit charging particles based on current element
                Color elementColor = GetElementColor(currentElement);
                ParticleType particles = GetElementParticles(currentElement);
                if (particles != null)
                {
                    SceneAs<Level>()?.ParticlesFG.Emit(particles, 2,
                        Position + Vector2.UnitY * -8f, Vector2.One * 6f);
                }
                
                // Increase light intensity while charging
                Light.Alpha = 1f + (hammerChargeTime / MAX_HAMMER_CHARGE) * 0.5f;
                Light.Color = Color.Lerp(Color.Gold, elementColor, hammerChargeTime / MAX_HAMMER_CHARGE);
                
                yield return null;
            }
        }

        private void ExecuteHammerAttack(global::Celeste.Player player)
        {
            isChargingHammer = false;
            float chargeRatio = Math.Min(hammerChargeTime / MAX_HAMMER_CHARGE, 1f);
            
            // Play hammer smash animation
            Sprite.Play("attack", false, false);
            
            // Calculate hammer power based on charge
            float hammerPower = 1f + chargeRatio * 1.5f;
            Vector2 hammerDirection = new Vector2(Sprite.Scale.X, 0f);
            
            // Execute elemental hammer attack
            ExecuteElementalHammerAttack(player, hammerDirection, hammerPower);
            
            // Create shockwave
            CreateHammerShockwave(chargeRatio);
            
            // Reset charging state
            hammerChargeTime = 0f;
            Light.Alpha = 1f;
            Light.Color = Color.Gold;
        }

        private void ExecuteElementalHammerAttack(global::Celeste.Player player, Vector2 direction, float power)
        {
            // Apply elemental boost to player based on current element
            switch (currentElement)
            {
                case ElementalType.Sizzle:
                    // Fire hammer - upward boost with burn effect
                    player.Speed.Y = Math.Min(player.Speed.Y, -150f * power);
                    player.Speed.X += direction.X * 80f * power;
                    CreateFireTrail(player);
                    Audio.Play("event:/char/madeline/dash_red", Position);
                    break;
                    
                case ElementalType.Blizzard:
                    // Ice hammer - creates ice platforms 
                    player.Speed.Y = Math.Min(player.Speed.Y, -120f * power);
                    CreateIcePlatforms(direction, power);
                    Audio.Play("event:/char/madeline/dash_blue", Position);
                    break;
                    
                case ElementalType.Splash:
                    // Water hammer - cleaning effect and water dash
                    player.Speed.Y = Math.Min(player.Speed.Y, -130f * power);
                    CreateWaterCurrent(direction, power);
                    Audio.Play("event:/char/madeline/water_dash", Position);
                    break;
                    
                case ElementalType.Bluster:
                    // Wind hammer - air dash and wind boost
                    player.Speed.Y = Math.Min(player.Speed.Y, -180f * power);
                    CreateWindBoost(player, direction, power);
                    Audio.Play("event:/char/madeline/dash_green", Position);
                    break;
            }
        }

        private void CreateHammerShockwave(float chargeRatio)
        {
            Level level = SceneAs<Level>();
            if (level == null) return;
            
            float actualRadius = shockwaveRadius * (0.5f + chargeRatio * 0.5f);
            
            // Visual shockwave
            level.Displacement?.AddBurst(Position, 0.4f, 16f, actualRadius, 0.6f, null, null);
            level.Shake(0.3f);
            
            // Emit hammer smash particles
            level.ParticlesFG.Emit(P_HammerSmash, 12, Position, Vector2.One * 12f);
            
            // Apply elemental effects in radius
            ApplyElementalEffect(level, Position, actualRadius);
            
            // Destroy breakable blocks in radius
            foreach (var entity in level.Entities.FindAll<Solid>())
            {
                if (Vector2.Distance(entity.Position, Position) <= actualRadius)
                {
                    if (entity.GetType().Name.Contains("DashBlock") ||
                        entity.GetType().Name.Contains("CrumbleBlock"))
                    {
                        entity.RemoveSelf();
                    }
                }
            }
            
            Audio.Play("event:/char/badeline/boss_reappear", Position);
        }

        private void CycleElementalHammer()
        {
            currentElementIndex = (currentElementIndex + 1) % availableElements.Count;
            SetElementalAbility(availableElements[currentElementIndex]);
            elementSwitchCooldown = ELEMENT_SWITCH_COOLDOWN;
            
            // Visual element switch effect
            ParticleType particles = GetElementParticles(currentElement);
            if (particles != null)
            {
                SceneAs<Level>()?.ParticlesFG.Emit(particles, 8,
                    Position + Vector2.UnitY * -8f, Vector2.One * 8f);
            }
            
            Audio.Play("event:/ui/main/button_select", Position);
        }

        private void SetElementalAbility(ElementalType elementType)
        {
            currentElement = elementType;
        }

        private HammerElementalAbility GetCurrentAbility()
        {
            return CreateElementalAbility(currentElement);
        }

        private Color GetElementColor(ElementalType elementType)
        {
            return CreateElementalAbility(elementType).ElementColor;
        }

        private ParticleType GetElementParticles(ElementalType elementType)
        {
            return CreateElementalAbility(elementType).ElementParticles;
        }

        private void ApplyElementalEffect(Level level, Vector2 center, float radius)
        {
            if (level == null)
                return;

            var ability = CreateElementalAbility(currentElement);
            ability?.ApplyEffect(level, center, radius, this);
        }

        private void CreateFireTrail(global::Celeste.Player player)
        {
            Add(new Coroutine(FireTrailRoutine(player)));
        }

        private IEnumerator FireTrailRoutine(global::Celeste.Player player)
        {
            for (float t = 0f; t < 1.5f; t += Engine.DeltaTime)
            {
                if (player != null)
                {
                    ParticleType particles = GetElementParticles(currentElement);
                    if (particles != null)
                    {
                        SceneAs<Level>()?.ParticlesBG.Emit(particles, 3,
                            player.Position, Vector2.One * 6f);
                    }
                }
                yield return null;
            }
        }

        private void CreateIcePlatforms(Vector2 direction, float power)
        {
            Level level = SceneAs<Level>();
            if (level == null) return;
            
            // Create temporary ice platforms
            for (int i = 1; i <= 3; i++)
            {
                Vector2 platformPos = Position + direction * (i * 32f) + Vector2.UnitY * 16f;
                
                Entity icePlatform = new Entity(platformPos);
                icePlatform.Collider = new Hitbox(24f, 8f, -12f, -4f);
                icePlatform.Add(new PlayerCollider(OnIcePlatform));
                
                level.Add(icePlatform);
                
                // Remove ice platform after duration
                Add(new Coroutine(RemoveIcePlatformAfterTime(icePlatform, 3f + i * 0.5f)));
            }
        }

        private void OnIcePlatform(global::Celeste.Player player)
        {
            // Ice platform provides grip and slight upward boost
            if (player.Speed.Y > 0f)
            {
                player.Speed.Y *= 0.7f; // Reduce fall speed
            }
        }

        private IEnumerator RemoveIcePlatformAfterTime(Entity platform, float duration)
        {
            yield return duration;
            platform?.RemoveSelf();
        }

        private void CreateWaterCurrent(Vector2 direction, float power)
        {
            Add(new Coroutine(WaterCurrentRoutine(direction, power)));
        }

        private IEnumerator WaterCurrentRoutine(Vector2 direction, float power)
        {
            for (float t = 0f; t < 2f; t += Engine.DeltaTime)
            {
                Vector2 currentPos = Position + direction * (t * 60f);
                
                // Emit water particles
                ParticleType particles = GetElementParticles(currentElement);
                if (particles != null)
                {
                    SceneAs<Level>()?.ParticlesFG.Emit(particles, 4,
                        currentPos, Vector2.One * 8f);
                }
                
                // Apply water effect to nearby player
                if (player != null && Vector2.Distance(player.Position, currentPos) < 24f)
                {
                    player.Speed += direction * 30f * Engine.DeltaTime;
                }
                
                yield return null;
            }
        }

        private void CreateWindBoost(global::Celeste.Player player, Vector2 direction, float power)
        {
            Add(new Coroutine(WindBoostRoutine(player, direction, power)));
        }

        private IEnumerator WindBoostRoutine(global::Celeste.Player player, Vector2 direction, float power)
        {
            for (float t = 0f; t < 1.2f; t += Engine.DeltaTime)
            {
                if (player != null)
                {
                    // Continuous wind boost
                    player.Speed += new Vector2(direction.X * 40f, -30f) * Engine.DeltaTime * power;
                    
                    // Wind particles around player
                    ParticleType particles = GetElementParticles(currentElement);
                    if (particles != null)
                    {
                        SceneAs<Level>()?.ParticlesFG.Emit(particles, 3,
                            player.Position, Vector2.One * 10f);
                    }
                }
                yield return null;
            }
        }

        public override void Update()
        {
            base.Update();
            
            // Update element switch cooldown
            if (elementSwitchCooldown > 0f)
            {
                elementSwitchCooldown -= Engine.DeltaTime;
            }
            
            // Update sprite facing based on player position
            if (player != null && following)
            {
                Sprite.Scale.X = player.Position.X > Position.X ? 1f : -1f;
            }
            
            // Update hammer charging visuals
            if (isChargingHammer)
            {
                UpdateHammerChargeVisuals();
            }
        }

        private void UpdateHammerChargeVisuals()
        {
            float chargeRatio = Math.Min(hammerChargeTime / MAX_HAMMER_CHARGE, 1f);
            
            // Pulsing effect while charging
            Wave.Rate = 0.2f + chargeRatio * 0.3f;
            
            // Screen shake at full charge
            if (chargeRatio >= 1f && hammerChargeTime % 0.1f < 0.05f)
            {
                SceneAs<Level>()?.Shake(0.1f);
            }
        }

        private HammerElementalAbility CreateElementalAbility(ElementalType elementType)
        {
            return new HammerElementalAbility(elementType);
        }

        /// <summary>
        /// Specialized elemental ability for King Dedede's hammer
        /// </summary>
        private class HammerElementalAbility : ElementalAbility
        {
            public HammerElementalAbility(ElementalType type) : base(type)
            {
                elementPower = 1.5f; // Hammers are powerful
                elementDuration = 3.0f; // Longer duration for hammer effects
            }

            protected override void OnActivate(Vector2 position, Vector2 direction)
            {
                // Hammer-specific activation effects
            }

            protected override void OnDeactivate()
            {
                // Hammer-specific deactivation effects
            }

            protected override void OnUpdate(Vector2 position)
            {
                // Hammer-specific update logic
            }

            public void ApplyEffect(Level level, Vector2 center, float radius, Entity source)
            {
                isActive = true;
                ApplyElementalEffect(level, center, radius, source);
                isActive = false;
            }
        }
    }
}



