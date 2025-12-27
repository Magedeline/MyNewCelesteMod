namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// Francisca Mage-Sister - Ice-based attacks and ice platform creation
    /// </summary>
    [CustomEntity("Ingeste/FranciscaDreamFriend")]
    public class FranciscaDreamFriendDummy : Entity
    {
        public static ParticleType P_IceAxe = new ParticleType
        {
            Size = 1f,
            Color = Color.LightBlue,
            Color2 = Color.White,
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 1.0f,
            LifeMax = 1.8f,
            SpeedMin = 15f,
            SpeedMax = 30f,
            DirectionRange = (float)Math.PI * 2f,
            Acceleration = new Vector2(0f, 20f)
        };

        // Visual components
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;

        // Ice abilities
        private bool isChargingIce = false;
        private float iceChargeTime = 0f;
        private const float MAX_ICE_CHARGE = 2.0f;
        private float iceEnergy = 100f;
        private const float MAX_ICE_ENERGY = 100f;
        
        // Following behavior
        protected global::Celeste.Player player;
        protected bool following = false;
        protected float followDistance = 50f;
        
        // Sound
        private SoundSource iceSound;

        public FranciscaDreamFriendDummy(Vector2 position, int index = 0) : base(position)
        {
        }

        // Standard Everest constructor for map loading
        public FranciscaDreamFriendDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            SetupVisuals();
            SetupMovement();
        }

        private void SetupVisuals()
        {
            // Create Francisca sprite
            Sprite = new PlayerSprite(PlayerSpriteMode.MadelineNoBackpack);
            Sprite.Play("idle", false, false);
            Sprite.Scale.X = -1f;
            
            // Skip hair initialization - creating PlayerHair with null causes NullReferenceException
            Hair = null;
            Add(Sprite);
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            // Floating movement
            Add(Wave = new SineWave(0.3f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = Vector2.UnitY * f * 3f;
            };
            
            // Ice-blue light
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.LightBlue, 1f, 24, 60));
            
            iceSound = new SoundSource();
            Add(iceSound);
        }

        private void SetupMovement()
        {
            Add(new PlayerCollider(OnPlayer, null, null));
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            if (player.StateMachine.State == global::Celeste.Player.StNormal)
            {
                // Ice axe charging - hold grab
                if (Input.Grab.Check && iceEnergy >= 20f && !isChargingIce)
                {
                    StartIceCharge();
                }
                else if (Input.Grab.Released && isChargingIce)
                {
                    ExecuteIceAxeAttack(player);
                }
                
                // Ice platform creation - press dash
                if (Input.Dash.Pressed && iceEnergy >= 30f)
                {
                    CreateIcePlatformChain(player);
                }
                
                // Freeze aura - hold jump
                if (Input.Jump.Check && iceEnergy >= 15f)
                {
                    ActivateFreezeAura(player);
                }
            }
        }

        private void StartIceCharge()
        {
            isChargingIce = true;
            iceChargeTime = 0f;
            Sprite.Play("dash", true, false);
            iceSound.Play("event:/char/madeline/dash_blue");
            
            Add(new Coroutine(IceChargeRoutine()));
        }

        private IEnumerator IceChargeRoutine()
        {
            while (isChargingIce && iceChargeTime < MAX_ICE_CHARGE)
            {
                iceChargeTime += Engine.DeltaTime;
                
                // Emit ice charging particles
                SceneAs<Level>()?.ParticlesFG.Emit(P_IceAxe, 3, 
                    Position + Vector2.UnitY * -8f, Vector2.One * 8f);
                
                // Increase light intensity
                float chargeRatio = iceChargeTime / MAX_ICE_CHARGE;
                Light.Alpha = 1f + chargeRatio * 0.8f;
                
                yield return null;
            }
        }

        private void ExecuteIceAxeAttack(global::Celeste.Player player)
        {
            isChargingIce = false;
            float chargeRatio = Math.Min(iceChargeTime / MAX_ICE_CHARGE, 1f);
            
            // Energy cost based on charge
            float energyCost = 20f + (chargeRatio * 30f);
            iceEnergy -= energyCost;
            
            // Attack power and range based on charge
            float attackPower = 0.8f + chargeRatio * 1.7f;
            Vector2 iceDirection = new Vector2(Sprite.Scale.X, 0f);
            
            // Execute ice axe attack
            Add(new Coroutine(IceAxeAttackRoutine(player, iceDirection, attackPower, chargeRatio)));
            
            // Reset visual state
            Light.Alpha = 1f;
            Sprite.Play("idle", false, false);
            iceChargeTime = 0f;
            
            Audio.Play("event:/char/madeline/dash_blue", Position);
        }

        private IEnumerator IceAxeAttackRoutine(global::Celeste.Player player, Vector2 direction, float power, float charge)
        {
            // Boost player with ice power
            player.Speed.Y = Math.Min(player.Speed.Y, -120f * power);
            player.Speed.X += direction.X * 100f * power;
            
            // Create ice axe projectiles
            int projectileCount = 1 + (int)(charge * 3f);
            
            for (int i = 0; i < projectileCount; i++)
            {
                Vector2 projectileDir = direction.Rotate((i - projectileCount/2) * 0.3f);
                Vector2 startPos = Position + projectileDir * 20f;
                
                Add(new Coroutine(IceAxeProjectileRoutine(startPos, projectileDir, power)));
                
                yield return 0.1f;
            }
        }

        private IEnumerator IceAxeProjectileRoutine(Vector2 startPos, Vector2 direction, float power)
        {
            Vector2 currentPos = startPos;
            float distance = 0f;
            float maxDistance = 150f * power;
            
            while (distance < maxDistance)
            {
                distance += 4f;
                currentPos += direction * 4f;
                
                // Emit ice particles along path
                SceneAs<Level>()?.ParticlesFG.Emit(P_IceAxe, 2, currentPos, Vector2.One * 6f);
                
                // Check for obstacles to freeze
                Level level = SceneAs<Level>();
                if (level != null)
                {
                    foreach (var entity in level.Entities)
                    {
                        if (Vector2.Distance(entity.Position, currentPos) < 20f)
                        {
                            // Freeze certain entities
                            if (entity.GetType().Name.Contains("Spinner") ||
                                entity.GetType().Name.Contains("Lava") ||
                                entity.GetType().Name.Contains("Fire"))
                            {
                                // Apply freeze effect
                                ApplyFreezeEffect(entity);
                            }
                        }
                    }
                }
                
                yield return null;
            }
            
            // Create ice explosion at end
            SceneAs<Level>()?.ParticlesFG.Emit(P_IceAxe, 12, currentPos, Vector2.One * 15f);
        }

        private void CreateIcePlatformChain(global::Celeste.Player player)
        {
            iceEnergy -= 30f;
            
            Vector2 platformDirection = new Vector2(Sprite.Scale.X, 0f);
            
            // Boost player
            player.Speed.Y = Math.Min(player.Speed.Y, -100f);
            player.Speed.X += platformDirection.X * 80f;
            
            Add(new Coroutine(IcePlatformChainRoutine(platformDirection)));
            
            Audio.Play("event:/char/madeline/dash_blue", Position);
        }

        private IEnumerator IcePlatformChainRoutine(Vector2 direction)
        {
            Level level = SceneAs<Level>();
            if (level == null) yield break;
            
            for (int i = 1; i <= 5; i++)
            {
                Vector2 platformPos = Position + direction * (i * 35f) + Vector2.UnitY * (10f + i * 3f);
                
                // Create ice platform
                Entity icePlatform = new Entity(platformPos);
                icePlatform.Collider = new Hitbox(32f, 8f, -16f, -4f);
                icePlatform.Add(new PlayerCollider(OnIcePlatformContact));
                
                level.Add(icePlatform);
                
                // Ice platform particles
                SceneAs<Level>()?.ParticlesFG.Emit(P_IceAxe, 6, platformPos, Vector2.One * 8f);
                
                // Platform lasts longer than paint platforms
                Add(new Coroutine(RemoveIcePlatformAfterTime(icePlatform, 6f + i * 0.4f)));
                
                yield return 0.15f;
            }
        }

        private void OnIcePlatformContact(global::Celeste.Player player)
        {
            // Ice platforms provide enhanced grip and slip resistance
            if (player.Speed.Y > 0f)
            {
                player.Speed.Y *= 0.5f; // Strong grip
            }
            
            // Slight speed boost on ice
            if (Math.Abs(player.Speed.X) > 0f)
            {
                player.Speed.X *= 1.1f;
            }
        }

        private IEnumerator RemoveIcePlatformAfterTime(Entity platform, float duration)
        {
            yield return duration;
            
            // Ice shatter effect before removal
            if (platform.Scene != null)
            {
                SceneAs<Level>()?.ParticlesFG.Emit(P_IceAxe, 8, platform.Position, Vector2.One * 12f);
            }
            
            platform?.RemoveSelf();
        }

        private void ActivateFreezeAura(global::Celeste.Player player)
        {
            iceEnergy -= 15f * Engine.DeltaTime;
            
            // Create freezing aura around Francisca
            SceneAs<Level>()?.ParticlesFG.Emit(P_IceAxe, 2, Position, Vector2.One * 25f);
            
            // Slow down nearby entities
            Level level = SceneAs<Level>();
            if (level != null)
            {
                foreach (var entity in level.Entities)
                {
                    if (Vector2.Distance(entity.Position, Position) < 50f)
                    {
                        // Apply freeze slow effect
                        if (entity.GetType().Name.Contains("Spinner") ||
                            entity.GetType().Name.Contains("Enemy") ||
                            entity.GetType().Name.Contains("Hazard"))
                        {
                            ApplySlowEffect(entity);
                        }
                    }
                }
            }
        }

        private void ApplyFreezeEffect(Entity entity)
        {
            // Visual freeze effect
            SceneAs<Level>()?.ParticlesFG.Emit(P_IceAxe, 6, entity.Position, Vector2.One * 10f);
            
            // Could add temporary freeze component here
            // For now, just visual effect
        }

        private void ApplySlowEffect(Entity entity)
        {
            // Could implement slow effect component
            // For now, just visual effect
            SceneAs<Level>()?.ParticlesFG.Emit(P_IceAxe, 1, entity.Position, Vector2.One * 8f);
        }

        public override void Update()
        {
            base.Update();
            
            // Regenerate ice energy
            if (iceEnergy < MAX_ICE_ENERGY)
            {
                iceEnergy += 25f * Engine.DeltaTime;
                iceEnergy = Math.Min(iceEnergy, MAX_ICE_ENERGY);
            }
            
            // Update sprite facing and following
            if (player != null && following)
            {
                Sprite.Scale.X = player.Position.X > Position.X ? 1f : -1f;
                if (Hair != null) Hair.Facing = Sprite.Scale.X > 0 ? Facings.Right : Facings.Left;
                UpdateFollowMovement();
            }
        }

        private void UpdateFollowMovement()
        {
            Vector2 targetPosition = player.Position + Vector2.UnitX * -followDistance;
            Vector2 direction = (targetPosition - Position).SafeNormalize();
            Position += direction * 70f * Engine.DeltaTime;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            player = scene.Tracker.GetEntity<global::Celeste.Player>();
            following = true;
        }

        public override void Render()
        {
            base.Render();
            RenderIceEnergyIndicator();
        }

        private void RenderIceEnergyIndicator()
        {
            Vector2 barPos = Position + Vector2.UnitY * -25f;
            float barWidth = 32f;
            float barHeight = 3f;
            float energyRatio = iceEnergy / MAX_ICE_ENERGY;
            
            // Background
            Draw.Rect(barPos.X - barWidth/2, barPos.Y, barWidth, barHeight, Color.DarkGray);
            
            // Ice energy fill
            if (energyRatio > 0f)
            {
                Draw.Rect(barPos.X - barWidth/2, barPos.Y, barWidth * energyRatio, barHeight, Color.LightBlue);
            }
        }
    }

    /// <summary>
    /// Flamberge Mage-Sister - Fire-based attacks and flame trails
    /// </summary>
    [CustomEntity("Ingeste/FlambergeDreamFriend")]
    public class FlambergeDreamFriendDummy : Entity
    {
        public static ParticleType P_FireSword = new ParticleType
        {
            Size = 1f,
            Color = Color.OrangeRed,
            Color2 = Color.Yellow,
            ColorMode = ParticleType.ColorModes.Fade,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.8f,
            LifeMax = 1.5f,
            SpeedMin = 20f,
            SpeedMax = 40f,
            DirectionRange = (float)Math.PI * 2f,
            Acceleration = new Vector2(0f, -20f)
        };

        // Visual components
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;

        // Fire abilities
        private bool isChargingFire = false;
        private float fireChargeTime = 0f;
        private const float MAX_FIRE_CHARGE = 2.0f;
        private float fireEnergy = 100f;
        private const float MAX_FIRE_ENERGY = 100f;
        
        // Following behavior
        protected global::Celeste.Player player;
        protected bool following = false;
        protected float followDistance = 55f;
        
        // Sound
        private SoundSource fireSound;

        public FlambergeDreamFriendDummy(Vector2 position, int index = 0) : base(position)
        {
        }

        // Standard Everest constructor for map loading
        public FlambergeDreamFriendDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            SetupVisuals();
            SetupMovement();
        }

        private void SetupVisuals()
        {
            Sprite = new PlayerSprite(PlayerSpriteMode.MadelineNoBackpack);
            Sprite.Play("idle", false, false);
            Sprite.Scale.X = -1f;
            
            // Skip hair initialization - creating PlayerHair with null causes NullReferenceException
            Hair = null;
            Add(Sprite);
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            // Aggressive floating movement
            Add(Wave = new SineWave(0.4f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = Vector2.UnitY * f * 2.5f;
            };
            
            // Fire-orange light
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.OrangeRed, 1f, 26, 65));
            
            fireSound = new SoundSource();
            Add(fireSound);
        }

        private void SetupMovement()
        {
            Add(new PlayerCollider(OnPlayer, null, null));
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            if (player.StateMachine.State == global::Celeste.Player.StNormal)
            {
                // Fire sword charging - hold grab
                if (Input.Grab.Check && fireEnergy >= 25f && !isChargingFire)
                {
                    StartFireCharge();
                }
                else if (Input.Grab.Released && isChargingFire)
                {
                    ExecuteFireSwordAttack(player);
                }
                
                // Flame trail dash - press dash
                if (Input.Dash.Pressed && fireEnergy >= 35f)
                {
                    CreateFlameTrailDash(player);
                }
                
                // Fire barrier - hold jump
                if (Input.Jump.Check && fireEnergy >= 20f)
                {
                    ActivateFireBarrier(player);
                }
            }
        }

        private void StartFireCharge()
        {
            isChargingFire = true;
            fireChargeTime = 0f;
            Sprite.Play("dash", true, false);
            fireSound.Play("event:/char/madeline/dash_red");
            
            Add(new Coroutine(FireChargeRoutine()));
        }

        private IEnumerator FireChargeRoutine()
        {
            while (isChargingFire && fireChargeTime < MAX_FIRE_CHARGE)
            {
                fireChargeTime += Engine.DeltaTime;
                
                // Emit fire charging particles
                SceneAs<Level>()?.ParticlesFG.Emit(P_FireSword, 4, 
                    Position + Vector2.UnitY * -8f, Vector2.One * 10f);
                
                // Increase light intensity and flicker
                float chargeRatio = fireChargeTime / MAX_FIRE_CHARGE;
                Light.Alpha = 1f + chargeRatio * 1.0f + (float)Math.Sin(fireChargeTime * 10f) * 0.2f;
                
                yield return null;
            }
        }

        private void ExecuteFireSwordAttack(global::Celeste.Player player)
        {
            isChargingFire = false;
            float chargeRatio = Math.Min(fireChargeTime / MAX_FIRE_CHARGE, 1f);
            
            // Energy cost
            float energyCost = 25f + (chargeRatio * 35f);
            fireEnergy -= energyCost;
            
            // Attack power
            float attackPower = 1.0f + chargeRatio * 2.0f;
            Vector2 fireDirection = new Vector2(Sprite.Scale.X, 0f);
            
            // Execute fire sword attack
            Add(new Coroutine(FireSwordAttackRoutine(player, fireDirection, attackPower, chargeRatio)));
            
            // Reset visual state
            Light.Alpha = 1f;
            Sprite.Play("idle", false, false);
            fireChargeTime = 0f;
            
            Audio.Play("event:/char/madeline/dash_red", Position);
        }

        private IEnumerator FireSwordAttackRoutine(global::Celeste.Player player, Vector2 direction, float power, float charge)
        {
            // Boost player with fire power
            player.Speed.Y = Math.Min(player.Speed.Y, -140f * power);
            player.Speed.X += direction.X * 120f * power;
            
            // Create fire sword waves
            int waveCount = 2 + (int)(charge * 2f);
            
            for (int i = 0; i < waveCount; i++)
            {
                Vector2 waveDir = direction.Rotate((i - waveCount/2) * 0.4f);
                Vector2 startPos = Position + waveDir * 15f;
                
                Add(new Coroutine(FireWaveRoutine(startPos, waveDir, power)));
                
                yield return 0.08f;
            }
        }

        private IEnumerator FireWaveRoutine(Vector2 startPos, Vector2 direction, float power)
        {
            Vector2 currentPos = startPos;
            float distance = 0f;
            float maxDistance = 180f * power;
            
            while (distance < maxDistance)
            {
                distance += 5f;
                currentPos += direction * 5f;
                
                // Emit fire particles along path
                SceneAs<Level>()?.ParticlesFG.Emit(P_FireSword, 3, currentPos, Vector2.One * 8f);
                
                // Burn obstacles
                Level level = SceneAs<Level>();
                if (level != null)
                {
                    foreach (var entity in level.Entities)
                    {
                        if (Vector2.Distance(entity.Position, currentPos) < 25f)
                        {
                            // Burn certain entities
                            if (entity.GetType().Name.Contains("IceBlock") ||
                                entity.GetType().Name.Contains("Vine") ||
                                entity.GetType().Name.Contains("Wood") ||
                                entity.GetType().Name.Contains("Grass"))
                            {
                                ApplyBurnEffect(entity);
                            }
                        }
                    }
                }
                
                yield return null;
            }
            
            // Fire explosion at end
            SceneAs<Level>()?.ParticlesFG.Emit(P_FireSword, 15, currentPos, Vector2.One * 18f);
        }

        private void CreateFlameTrailDash(global::Celeste.Player player)
        {
            fireEnergy -= 35f;
            
            Vector2 dashDirection = new Vector2(Sprite.Scale.X, 0f);
            
            // Powerful fire boost
            player.Speed.Y = Math.Min(player.Speed.Y, -110f);
            player.Speed.X += dashDirection.X * 140f;
            
            Add(new Coroutine(FlameTrailRoutine(dashDirection)));
            
            Audio.Play("event:/char/madeline/dash_red", Position);
        }

        private IEnumerator FlameTrailRoutine(Vector2 direction)
        {
            for (float t = 0f; t < 1.5f; t += Engine.DeltaTime)
            {
                Vector2 trailPos = Position + direction * (t * 100f);
                
                // Create flame trail
                SceneAs<Level>()?.ParticlesFG.Emit(P_FireSword, 5, trailPos, Vector2.One * 12f);
                
                // Burn path
                Level level = SceneAs<Level>();
                if (level != null)
                {
                    foreach (var entity in level.Entities)
                    {
                        if (Vector2.Distance(entity.Position, trailPos) < 20f)
                        {
                            if (entity.GetType().Name.Contains("IceBlock") ||
                                entity.GetType().Name.Contains("Snow"))
                            {
                                ApplyBurnEffect(entity);
                            }
                        }
                    }
                }
                
                yield return null;
            }
        }

        private void ActivateFireBarrier(global::Celeste.Player player)
        {
            fireEnergy -= 20f * Engine.DeltaTime;
            
            // Create fire barrier around Flamberge
            SceneAs<Level>()?.ParticlesFG.Emit(P_FireSword, 3, Position, Vector2.One * 30f);
            
            // Burn nearby hazards
            Level level = SceneAs<Level>();
            if (level != null)
            {
                foreach (var entity in level.Entities)
                {
                    if (Vector2.Distance(entity.Position, Position) < 45f)
                    {
                        if (entity.GetType().Name.Contains("IceBlock") ||
                            entity.GetType().Name.Contains("Vine") ||
                            entity.GetType().Name.Contains("Corruption"))
                        {
                            ApplyBurnEffect(entity);
                        }
                    }
                }
            }
        }

        private void ApplyBurnEffect(Entity entity)
        {
            // Visual burn effect
            SceneAs<Level>()?.ParticlesFG.Emit(P_FireSword, 8, entity.Position, Vector2.One * 12f);
            
            // Remove certain burnable entities
            if (entity.GetType().Name.Contains("IceBlock") ||
                entity.GetType().Name.Contains("Vine") ||
                entity.GetType().Name.Contains("Wood"))
            {
                entity.RemoveSelf();
            }
        }

        public override void Update()
        {
            base.Update();
            
            // Regenerate fire energy
            if (fireEnergy < MAX_FIRE_ENERGY)
            {
                fireEnergy += 30f * Engine.DeltaTime;
                fireEnergy = Math.Min(fireEnergy, MAX_FIRE_ENERGY);
            }
            
            // Update sprite facing and following
            if (player != null && following)
            {
                Sprite.Scale.X = player.Position.X > Position.X ? 1f : -1f;
                if (Hair != null) Hair.Facing = Sprite.Scale.X > 0 ? Facings.Right : Facings.Left;
                UpdateFollowMovement();
            }
        }

        private void UpdateFollowMovement()
        {
            Vector2 targetPosition = player.Position + Vector2.UnitX * -followDistance;
            Vector2 direction = (targetPosition - Position).SafeNormalize();
            Position += direction * 80f * Engine.DeltaTime;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            player = scene.Tracker.GetEntity<global::Celeste.Player>();
            following = true;
        }

        public override void Render()
        {
            base.Render();
            RenderFireEnergyIndicator();
        }

        private void RenderFireEnergyIndicator()
        {
            Vector2 barPos = Position + Vector2.UnitY * -25f;
            float barWidth = 32f;
            float barHeight = 3f;
            float energyRatio = fireEnergy / MAX_FIRE_ENERGY;
            
            // Background
            Draw.Rect(barPos.X - barWidth/2, barPos.Y, barWidth, barHeight, Color.DarkGray);
            
            // Fire energy fill
            if (energyRatio > 0f)
            {
                Draw.Rect(barPos.X - barWidth/2, barPos.Y, barWidth * energyRatio, barHeight, Color.OrangeRed);
            }
        }
    }

    /// <summary>
    /// Zan Partizanne Mage-Sister - Electric attacks and lightning strikes
    /// </summary>
    [CustomEntity("Ingeste/ZanPartizanneDreamFriend")]
    public class ZanPartizanneDreamFriendDummy : Entity
    {
        public static ParticleType P_Lightning = new ParticleType
        {
            Size = 1f,
            Color = Color.Yellow,
            Color2 = Color.White,
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.6f,
            LifeMax = 1.2f,
            SpeedMin = 30f,
            SpeedMax = 60f,
            DirectionRange = (float)Math.PI * 2f,
            Acceleration = new Vector2(0f, 0f)
        };

        // Visual components
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;

        // Electric abilities
        private bool isChargingLightning = false;
        private float lightningChargeTime = 0f;
        private const float MAX_LIGHTNING_CHARGE = 2.0f;
        private float electricEnergy = 100f;
        private const float MAX_ELECTRIC_ENERGY = 100f;
        
        // Following behavior
        protected global::Celeste.Player player;
        protected bool following = false;
        protected float followDistance = 45f;
        
        // Sound
        private SoundSource electricSound;

        public ZanPartizanneDreamFriendDummy(Vector2 position, int index = 0) : base(position)
        {
        }

        // Standard Everest constructor for map loading
        public ZanPartizanneDreamFriendDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            SetupVisuals();
            SetupMovement();
        }

        private void SetupVisuals()
        {
            Sprite = new PlayerSprite(PlayerSpriteMode.MadelineNoBackpack);
            Sprite.Play("idle", false, false);
            Sprite.Scale.X = -1f;
            
            // Skip hair initialization - creating PlayerHair with null causes NullReferenceException
            Hair = null;
            Add(Sprite);
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            // Energetic floating movement
            Add(Wave = new SineWave(0.35f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = Vector2.UnitY * f * 3.5f;
            };
            
            // Electric-yellow light
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Yellow, 1f, 28, 70));
            
            electricSound = new SoundSource();
            Add(electricSound);
        }

        private void SetupMovement()
        {
            Add(new PlayerCollider(OnPlayer, null, null));
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            if (player.StateMachine.State == global::Celeste.Player.StNormal)
            {
                // Lightning charge - hold grab
                if (Input.Grab.Check && electricEnergy >= 30f && !isChargingLightning)
                {
                    StartLightningCharge();
                }
                else if (Input.Grab.Released && isChargingLightning)
                {
                    ExecuteLightningStrike(player);
                }
                
                // Electric dash - press dash
                if (Input.Dash.Pressed && electricEnergy >= 40f)
                {
                    CreateElectricDash(player);
                }
                
                // Thunder aura - hold jump
                if (Input.Jump.Check && electricEnergy >= 25f)
                {
                    ActivateThunderAura(player);
                }
            }
        }

        private void StartLightningCharge()
        {
            isChargingLightning = true;
            lightningChargeTime = 0f;
            Sprite.Play("dash", true, false);
            electricSound.Play("event:/char/madeline/dash_blue");
            
            Add(new Coroutine(LightningChargeRoutine()));
        }

        private IEnumerator LightningChargeRoutine()
        {
            while (isChargingLightning && lightningChargeTime < MAX_LIGHTNING_CHARGE)
            {
                lightningChargeTime += Engine.DeltaTime;
                
                // Emit lightning charging particles
                SceneAs<Level>()?.ParticlesFG.Emit(P_Lightning, 4, 
                    Position + Vector2.UnitY * -8f, Vector2.One * 12f);
                
                // Increase light intensity with electric flicker
                float chargeRatio = lightningChargeTime / MAX_LIGHTNING_CHARGE;
                Light.Alpha = 1f + chargeRatio * 1.2f + (float)Math.Sin(lightningChargeTime * 15f) * 0.3f;
                Light.Color = Color.Lerp(Color.Yellow, Color.White, chargeRatio);
                
                yield return null;
            }
        }

        private void ExecuteLightningStrike(global::Celeste.Player player)
        {
            isChargingLightning = false;
            float chargeRatio = Math.Min(lightningChargeTime / MAX_LIGHTNING_CHARGE, 1f);
            
            // Energy cost
            float energyCost = 30f + (chargeRatio * 40f);
            electricEnergy -= energyCost;
            
            // Attack power
            float attackPower = 1.2f + chargeRatio * 2.3f;
            
            // Execute lightning strike
            Add(new Coroutine(LightningStrikeRoutine(player, attackPower, chargeRatio)));
            
            // Reset visual state
            Light.Alpha = 1f;
            Light.Color = Color.Yellow;
            Sprite.Play("idle", false, false);
            lightningChargeTime = 0f;
            
            Audio.Play("event:/char/madeline/dash_blue", Position);
        }

        private IEnumerator LightningStrikeRoutine(global::Celeste.Player player, float power, float charge)
        {
            // Boost player with electric power
            player.Speed.Y = Math.Min(player.Speed.Y, -160f * power);
            player.Speed.X += (Sprite.Scale.X) * 110f * power;
            
            // Create lightning strikes
            int strikeCount = 3 + (int)(charge * 4f);
            
            for (int i = 0; i < strikeCount; i++)
            {
                Vector2 strikePos = Position + new Vector2(
                    (i - strikeCount/2) * 40f, 
                    -100f - i * 20f
                );
                
                Add(new Coroutine(LightningBoltRoutine(strikePos, power)));
                
                yield return 0.1f;
            }
        }

        private IEnumerator LightningBoltRoutine(Vector2 startPos, float power)
        {
            Vector2 currentPos = startPos;
            Vector2 endPos = startPos + Vector2.UnitY * 200f;
            float distance = 0f;
            float maxDistance = 200f;
            
            while (distance < maxDistance)
            {
                distance += 8f;
                currentPos = Vector2.Lerp(startPos, endPos, distance / maxDistance);
                
                // Add lightning zigzag
                currentPos.X += (float)Math.Sin(distance * 0.2f) * 10f;
                
                // Emit lightning particles
                SceneAs<Level>()?.ParticlesFG.Emit(P_Lightning, 3, currentPos, Vector2.One * 8f);
                
                // Electrify nearby entities
                Level level = SceneAs<Level>();
                if (level != null)
                {
                    foreach (var entity in level.Entities)
                    {
                        if (Vector2.Distance(entity.Position, currentPos) < 30f)
                        {
                            // Electrify certain entities
                            if (entity.GetType().Name.Contains("Spinner") ||
                                entity.GetType().Name.Contains("Machine") ||
                                entity.GetType().Name.Contains("Metal"))
                            {
                                ApplyElectricEffect(entity);
                            }
                        }
                    }
                }
                
                yield return null;
            }
            
            // Lightning explosion
            SceneAs<Level>()?.ParticlesFG.Emit(P_Lightning, 20, currentPos, Vector2.One * 25f);
        }

        private void CreateElectricDash(global::Celeste.Player player)
        {
            electricEnergy -= 40f;
            
            Vector2 dashDirection = new Vector2(Sprite.Scale.X, 0f);
            
            // Electric boost
            player.Speed.Y = Math.Min(player.Speed.Y, -120f);
            player.Speed.X += dashDirection.X * 150f;
            
            Add(new Coroutine(ElectricDashRoutine(dashDirection)));
            
            Audio.Play("event:/char/madeline/dash_blue", Position);
        }

        private IEnumerator ElectricDashRoutine(Vector2 direction)
        {
            for (float t = 0f; t < 1.2f; t += Engine.DeltaTime)
            {
                Vector2 dashPos = Position + direction * (t * 120f);
                
                // Create electric trail
                SceneAs<Level>()?.ParticlesFG.Emit(P_Lightning, 6, dashPos, Vector2.One * 15f);
                
                // Electrify path
                Level level = SceneAs<Level>();
                if (level != null)
                {
                    foreach (var entity in level.Entities)
                    {
                        if (Vector2.Distance(entity.Position, dashPos) < 25f)
                        {
                            if (entity.GetType().Name.Contains("Machine") ||
                                entity.GetType().Name.Contains("Electronic"))
                            {
                                ApplyElectricEffect(entity);
                            }
                        }
                    }
                }
                
                yield return null;
            }
        }

        private void ActivateThunderAura(global::Celeste.Player player)
        {
            electricEnergy -= 25f * Engine.DeltaTime;
            
            // Create thunder aura
            SceneAs<Level>()?.ParticlesFG.Emit(P_Lightning, 4, Position, Vector2.One * 35f);
            
            // Electrify nearby entities
            Level level = SceneAs<Level>();
            if (level != null)
            {
                foreach (var entity in level.Entities)
                {
                    if (Vector2.Distance(entity.Position, Position) < 50f)
                    {
                        if (entity.GetType().Name.Contains("Spinner") ||
                            entity.GetType().Name.Contains("Enemy") ||
                            entity.GetType().Name.Contains("Hazard"))
                        {
                            ApplyElectricEffect(entity);
                        }
                    }
                }
            }
        }

        private void ApplyElectricEffect(Entity entity)
        {
            // Visual electric effect
            SceneAs<Level>()?.ParticlesFG.Emit(P_Lightning, 8, entity.Position, Vector2.One * 12f);
            
            // Could add electric stun component here
        }

        public override void Update()
        {
            base.Update();
            
            // Regenerate electric energy
            if (electricEnergy < MAX_ELECTRIC_ENERGY)
            {
                electricEnergy += 35f * Engine.DeltaTime;
                electricEnergy = Math.Min(electricEnergy, MAX_ELECTRIC_ENERGY);
            }
            
            // Update sprite facing and following
            if (player != null && following)
            {
                Sprite.Scale.X = player.Position.X > Position.X ? 1f : -1f;
                if (Hair != null) Hair.Facing = Sprite.Scale.X > 0 ? Facings.Right : Facings.Left;
                UpdateFollowMovement();
            }
        }

        private void UpdateFollowMovement()
        {
            Vector2 targetPosition = player.Position + Vector2.UnitX * -followDistance;
            Vector2 direction = (targetPosition - Position).SafeNormalize();
            Position += direction * 85f * Engine.DeltaTime;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            player = scene.Tracker.GetEntity<global::Celeste.Player>();
            following = true;
        }

        public override void Render()
        {
            base.Render();
            RenderElectricEnergyIndicator();
        }

        private void RenderElectricEnergyIndicator()
        {
            Vector2 barPos = Position + Vector2.UnitY * -25f;
            float barWidth = 32f;
            float barHeight = 3f;
            float energyRatio = electricEnergy / MAX_ELECTRIC_ENERGY;
            
            // Background
            Draw.Rect(barPos.X - barWidth/2, barPos.Y, barWidth, barHeight, Color.DarkGray);
            
            // Electric energy fill
            if (energyRatio > 0f)
            {
                Draw.Rect(barPos.X - barWidth/2, barPos.Y, barWidth * energyRatio, barHeight, Color.Yellow);
            }
        }
    }
}



