namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// Gooey Dream Friend - Enhanced tongue attacks, wall grabbing, and Gooey Stone
    /// </summary>
    [CustomEntity("Ingeste/GooeyDreamFriend")]
    public class GooeyDreamFriendDummy : Entity
    {
        public static ParticleType P_GooeySlime = new ParticleType
        {
            Size = 1f,
            Color = Color.LimeGreen,
            Color2 = Color.Yellow,
            ColorMode = ParticleType.ColorModes.Fade,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 1.0f,
            LifeMax = 1.8f,
            SpeedMin = 15f,
            SpeedMax = 30f,
            DirectionRange = (float)Math.PI * 2f,
            Acceleration = new Vector2(0f, 20f)
        };

        public static ParticleType P_GooeyStone = new ParticleType
        {
            Size = 2f,
            Color = Color.Purple,
            Color2 = Color.Magenta,
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.5f,
            LifeMax = 1.0f,
            SpeedMin = 40f,
            SpeedMax = 80f,
            DirectionRange = (float)Math.PI / 3f,
            Acceleration = new Vector2(0f, -50f)
        };

        // Visual components
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;

        // Gooey-specific properties
        private bool tongueExtended = false;
        private Vector2 tongueTarget = Vector2.Zero;
        private float tongueLength = 120f;
        private float tongueSpeed = 300f;
        private Entity tongueGrabbedEntity = null;
        
        // Gooey Stone attack
        private float gooeyStoneChargeTime = 0f;
        private const float MAX_GOOEY_STONE_CHARGE = 1.5f;
        private bool isChargingGooeyStone = false;
        
        // Abilities
        private float tongueCooldown = 0f;
        private const float TONGUE_COOLDOWN = 0.8f;
        private float gooeyStoneEnergy = 100f;
        private const float MAX_GOOEY_STONE_ENERGY = 100f;
        private const float GOOEY_STONE_COST = 50f;
        
        // Following behavior
        protected global::Celeste.Player player;
        protected bool following = false;
        protected float followDistance = 35f;
        
        // Sound
        private SoundSource gooeySound;

        public GooeyDreamFriendDummy(Vector2 position, int index = 0) : base(position)
        {
        }

        // Standard Everest constructor for map loading
        public GooeyDreamFriendDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            SetupVisuals();
            SetupMovement();
        }

        private void SetupVisuals()
        {
            // Create Gooey sprite
            Sprite = new PlayerSprite(PlayerSpriteMode.Gooey);
            Sprite.Play("idle", false, false);
            Sprite.Scale.X = -1f; // Face left initially
            
            // Skip hair initialization - creating PlayerHair with null causes NullReferenceException
            Hair = null;
            Add(Sprite);
            
            // Create auto animator
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            // Create floating wave effect (more bouncy for Gooey)
            Add(Wave = new SineWave(0.3f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = Vector2.UnitY * f * 3.5f;
            };
            
            // Create light with slime colors
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.LimeGreen, 1f, 20, 50));
            
            // Create gooey sound
            gooeySound = new SoundSource();
            Add(gooeySound);
        }

        private void SetupMovement()
        {
            Add(new PlayerCollider(OnPlayer, null, null));
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            if (player.StateMachine.State == global::Celeste.Player.StNormal)
            {
                // Tongue grab ability - hold grab to extend tongue
                if (Input.Grab.Pressed && tongueCooldown <= 0f && !tongueExtended)
                {
                    StartTongueAttack();
                }
                else if (Input.Grab.Released && tongueExtended)
                {
                    RetractTongue();
                }
                
                // Gooey Stone attack - hold dash to charge
                if (Input.Dash.Pressed && gooeyStoneEnergy >= GOOEY_STONE_COST && !isChargingGooeyStone)
                {
                    StartGooeyStoneCharge();
                }
                else if (Input.Dash.Released && isChargingGooeyStone)
                {
                    ExecuteGooeyStoneAttack(player);
                }
                
                // Tongue wall grab assist - automatic when near walls
                CheckForWallGrabAssist(player);
            }
        }

        private void StartTongueAttack()
        {
            tongueExtended = true;
            tongueCooldown = TONGUE_COOLDOWN;
            
            // Find target for tongue
            Vector2 playerDirection = player != null ? (player.Position - Position).SafeNormalize() : Vector2.UnitX;
            tongueTarget = Position + playerDirection * tongueLength;
            
            Sprite.Play("tongue_extend", false, false);
            gooeySound.Play("event:/char/madeline/grab");
            
            // Check for grabbable entities
            Add(new Coroutine(TongueAttackRoutine()));
        }

        private IEnumerator TongueAttackRoutine()
        {
            float tongueProgress = 0f;
            Vector2 startPos = Position;
            
            // Extend tongue
            while (tongueProgress < 1f && tongueExtended)
            {
                tongueProgress += Engine.DeltaTime * tongueSpeed / tongueLength;
                Vector2 currentTonguePos = Vector2.Lerp(startPos, tongueTarget, tongueProgress);
                
                // Check for grabbable entities
                CheckTongueGrab(currentTonguePos);
                
                // Emit slime particles along tongue
                SceneAs<Level>()?.ParticlesFG.Emit(P_GooeySlime, 2, currentTonguePos, Vector2.One * 4f);
                
                yield return null;
            }
            
            // Hold extended for a moment
            if (tongueExtended)
            {
                yield return 0.2f;
                RetractTongue();
            }
        }

        private void CheckTongueGrab(Vector2 tonguePos)
        {
            Level level = SceneAs<Level>();
            if (level == null) return;
            
            // Check for solid walls for wall grab assist
            foreach (var solid in level.Entities.FindAll<Solid>())
            {
                if (solid.CollidePoint(tonguePos))
                {
                    // Provide wall grab assist to player
                    if (player != null && player.StateMachine.State == global::Celeste.Player.StClimb)
                    {
                        // Pull player toward wall and provide grip assist
                        Vector2 pullDirection = (tonguePos - player.Position).SafeNormalize();
                        player.Speed += pullDirection * 60f * Engine.DeltaTime;
                        
                        // Enhanced grip (reduce stamina drain)
                        player.Stamina = Math.Max(player.Stamina, 20f);
                    }
                    break;
                }
            }
            
            // Check for grabbable entities (collectibles, blocks, etc.)
            foreach (var entity in level.Entities)
            {
                if (entity == this || entity == player) continue;
                
                if (Vector2.Distance(entity.Position, tonguePos) < 12f)
                {
                    // Grab entity
                    tongueGrabbedEntity = entity;
                    
                    // Pull small entities toward Gooey
                    if (entity.GetType().Name.Contains("Strawberry") ||
                        entity.GetType().Name.Contains("Key") ||
                        entity.GetType().Name.Contains("Crystal"))
                    {
                        entity.Position = Vector2.Lerp(entity.Position, Position, 0.1f);
                    }
                    break;
                }
            }
        }

        private void RetractTongue()
        {
            tongueExtended = false;
            tongueGrabbedEntity = null;
            Sprite.Play("tongue_retract", false, false);
            
            // If we grabbed something, apply tongue grab effects
            if (player != null)
            {
                ApplyTongueGrabEffects();
            }
        }

        private void ApplyTongueGrabEffects()
        {
            // Tongue grab provides movement assistance
            if (player.StateMachine.State == global::Celeste.Player.StClimb ||
                player.StateMachine.State == global::Celeste.Player.StNormal)
            {
                // Small upward boost from tongue assistance
                player.Speed.Y = Math.Min(player.Speed.Y, -80f);
                
                // Emit helpful slime particles
                SceneAs<Level>()?.ParticlesFG.Emit(P_GooeySlime, 6, 
                    Position, Vector2.One * 8f);
            }
        }

        private void CheckForWallGrabAssist(global::Celeste.Player player)
        {
            // Automatic wall grab assist when climbing
            if (player.StateMachine.State == global::Celeste.Player.StClimb)
            {
                Vector2 nearbyWallCheck = player.Position + Vector2.UnitX * (player.Facing == Facings.Left ? -16f : 16f);
                
                Level level = SceneAs<Level>();
                if (level != null)
                {
                    foreach (var solid in level.Entities.FindAll<Solid>())
                    {
                        if (solid.CollidePoint(nearbyWallCheck) && Vector2.Distance(Position, player.Position) < 40f)
                        {
                            // Provide ongoing wall grab assistance
                            player.Stamina += 0.5f * Engine.DeltaTime;
                            
                            // Emit assistance particles
                            if (Engine.Scene.OnInterval(0.1f))
                            {
                                SceneAs<Level>()?.ParticlesFG.Emit(P_GooeySlime, 1, 
                                    player.Position + Vector2.UnitX * (player.Facing == Facings.Left ? -8f : 8f), 
                                    Vector2.One * 3f);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void StartGooeyStoneCharge()
        {
            isChargingGooeyStone = true;
            gooeyStoneChargeTime = 0f;
            Sprite.Play("gooey_stone_charge", false, false);
            gooeySound.Play("event:/char/madeline/dash_red");
            
            Add(new Coroutine(GooeyStoneChargeRoutine()));
        }

        private IEnumerator GooeyStoneChargeRoutine()
        {
            while (isChargingGooeyStone && gooeyStoneChargeTime < MAX_GOOEY_STONE_CHARGE)
            {
                gooeyStoneChargeTime += Engine.DeltaTime;
                
                // Emit charging particles
                SceneAs<Level>()?.ParticlesFG.Emit(P_GooeyStone, 3, 
                    Position + Vector2.UnitY * -8f, Vector2.One * 6f);
                
                // Increase light intensity while charging
                float chargeRatio = gooeyStoneChargeTime / MAX_GOOEY_STONE_CHARGE;
                Light.Alpha = 1f + chargeRatio * 0.8f;
                Light.Color = Color.Lerp(Color.LimeGreen, Color.Purple, chargeRatio);
                
                // Screen effects at full charge
                if (chargeRatio >= 1f && gooeyStoneChargeTime % 0.05f < 0.025f)
                {
                    SceneAs<Level>()?.Shake(0.1f);
                }
                
                yield return null;
            }
        }

        private void ExecuteGooeyStoneAttack(global::Celeste.Player player)
        {
            isChargingGooeyStone = false;
            float chargeRatio = Math.Min(gooeyStoneChargeTime / MAX_GOOEY_STONE_CHARGE, 1f);
            
            // Consume energy
            gooeyStoneEnergy -= GOOEY_STONE_COST;
            
            // Calculate attack power
            float attackPower = 1f + chargeRatio * 2f;
            Vector2 attackDirection = player != null ? (player.Position - Position).SafeNormalize() : Vector2.UnitX;
            
            // Execute Gooey Stone attack
            CreateGooeyStoneProjectile(attackDirection, attackPower);
            
            // Provide massive boost to player
            if (player != null)
            {
                player.Speed.Y = Math.Min(player.Speed.Y, -200f * attackPower);
                player.Speed.X += attackDirection.X * 120f * attackPower;
            }
            
            // Reset visual state
            gooeyStoneChargeTime = 0f;
            Light.Alpha = 1f;
            Light.Color = Color.LimeGreen;
            
            Sprite.Play("gooey_stone_attack", false, false);
            Audio.Play("event:/char/badeline/boss_reappear", Position);
        }

        private void CreateGooeyStoneProjectile(Vector2 direction, float power)
        {
            Level level = SceneAs<Level>();
            if (level == null) return;
            
            // Create high-damage projectile
            Entity gooeyStone = new Entity(Position + direction * 16f);
            gooeyStone.Collider = new Hitbox(12f, 12f, -6f, -6f);
            
            // Add movement and effects
            Add(new Coroutine(GooeyStoneProjectileRoutine(gooeyStone, direction, power)));
            
            level.Add(gooeyStone);
        }

        private IEnumerator GooeyStoneProjectileRoutine(Entity projectile, Vector2 direction, float power)
        {
            float projectileSpeed = 250f * power;
            float lifetime = 2f;
            
            for (float t = 0f; t < lifetime; t += Engine.DeltaTime)
            {
                if (projectile.Scene == null) break;
                
                // Move projectile
                projectile.Position += direction * projectileSpeed * Engine.DeltaTime;
                
                // Emit particles
                SceneAs<Level>()?.ParticlesFG.Emit(P_GooeyStone, 4, 
                    projectile.Position, Vector2.One * 8f);
                
                // Check for collisions and apply high damage effects
                Level level = SceneAs<Level>();
                if (level != null)
                {
                    foreach (var solid in level.Entities.FindAll<Solid>())
                    {
                        if (solid.CollideCheck(projectile))
                        {
                            // High damage - destroy most breakable blocks
                            if (solid.GetType().Name.Contains("DashBlock") ||
                                solid.GetType().Name.Contains("CrumbleBlock") ||
                                solid.GetType().Name.Contains("FallingBlock"))
                            {
                                solid.RemoveSelf();
                                
                                // Create destruction effect
                                level.ParticlesFG.Emit(P_GooeyStone, 12, 
                                    projectile.Position, Vector2.One * 16f);
                                level.Shake(0.2f);
                            }
                            
                            projectile.RemoveSelf();
                            yield break;
                        }
                    }
                }
                
                yield return null;
            }
            
            // Remove projectile after lifetime
            projectile?.RemoveSelf();
        }

        public override void Update()
        {
            base.Update();
            
            // Update cooldowns
            if (tongueCooldown > 0f)
            {
                tongueCooldown -= Engine.DeltaTime;
            }
            
            // Regenerate Gooey Stone energy
            if (gooeyStoneEnergy < MAX_GOOEY_STONE_ENERGY)
            {
                gooeyStoneEnergy += 15f * Engine.DeltaTime;
                gooeyStoneEnergy = Math.Min(gooeyStoneEnergy, MAX_GOOEY_STONE_ENERGY);
            }
            
            // Update sprite facing based on player position
            if (player != null && following)
            {
                Sprite.Scale.X = player.Position.X > Position.X ? 1f : -1f;
                if (Hair != null) Hair.Facing = Sprite.Scale.X > 0 ? Facings.Right : Facings.Left;
                
                // Follow player with bouncy movement
                UpdateFollowMovement();
            }
            
            // Update Gooey Stone charging visuals
            if (isChargingGooeyStone)
            {
                UpdateGooeyStoneChargeVisuals();
            }
        }

        private void UpdateFollowMovement()
        {
            Vector2 targetPosition = player.Position + Vector2.UnitX * -followDistance;
            Vector2 direction = (targetPosition - Position).SafeNormalize();
            
            // Bouncy movement for Gooey
            Position += direction * 90f * Engine.DeltaTime;
            
            // Add slight bounce effect
            if (Vector2.Distance(Position, targetPosition) < 20f)
            {
                Wave.Rate = 0.4f; // Faster bounce when close
            }
            else
            {
                Wave.Rate = 0.3f; // Normal bounce rate
            }
        }

        private void UpdateGooeyStoneChargeVisuals()
        {
            float chargeRatio = Math.Min(gooeyStoneChargeTime / MAX_GOOEY_STONE_CHARGE, 1f);
            
            // Pulsing effect while charging
            Wave.Rate = 0.3f + chargeRatio * 0.4f;
            
            // Emit more particles as charge increases
            if (chargeRatio > 0.5f && Engine.Scene.OnInterval(0.05f))
            {
                SceneAs<Level>()?.ParticlesFG.Emit(P_GooeyStone, 2, 
                    Position + Vector2.UnitY * -8f, Vector2.One * 10f);
            }
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
            
            // Render tongue when extended
            if (tongueExtended)
            {
                RenderTongue();
            }
            
            // Render energy indicator
            RenderEnergyIndicator();
        }

        private void RenderTongue()
        {
            Vector2 tongueStart = Position + Vector2.UnitY * -4f;
            Vector2 tongueDirection = (tongueTarget - tongueStart).SafeNormalize();
            float tongueCurrentLength = Math.Min(Vector2.Distance(tongueStart, tongueTarget), tongueLength);
            
            // Simple line render for tongue
            Draw.Line(tongueStart, tongueStart + tongueDirection * tongueCurrentLength, Color.LimeGreen, 2f);
        }

        private void RenderEnergyIndicator()
        {
            // Render energy bar above Gooey
            Vector2 barPos = Position + Vector2.UnitY * -20f;
            float barWidth = 30f;
            float barHeight = 3f;
            float energyRatio = gooeyStoneEnergy / MAX_GOOEY_STONE_ENERGY;
            
            // Background
            Draw.Rect(barPos.X - barWidth/2, barPos.Y, barWidth, barHeight, Color.DarkGray);
            
            // Energy fill
            if (energyRatio > 0f)
            {
                Color energyColor = energyRatio >= (GOOEY_STONE_COST / MAX_GOOEY_STONE_ENERGY) ? Color.Purple : Color.DarkViolet;
                Draw.Rect(barPos.X - barWidth/2, barPos.Y, barWidth * energyRatio, barHeight, energyColor);
            }
        }
    }
}



