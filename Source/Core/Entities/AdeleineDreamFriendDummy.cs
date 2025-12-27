using DesoloZantas.Core.Core.Systems;

namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// Adeleine Dream Friend - Painting attacks and animal friend summoning
    /// </summary>
    [CustomEntity("Ingeste/AdeleineDreamFriend")]
    public class AdeleineDreamFriendDummy : Entity
    {
        public static ParticleType P_Paint = new ParticleType
        {
            Size = 1f,
            Color = Color.Pink,
            Color2 = Color.White,
            ColorMode = ParticleType.ColorModes.Fade,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.8f,
            LifeMax = 1.4f,
            SpeedMin = 20f,
            SpeedMax = 40f,
            DirectionRange = (float)Math.PI * 2f,
            Acceleration = new Vector2(0f, 30f)
        };

        public static ParticleType P_Magic = new ParticleType
        {
            Size = 1f,
            Color = Color.Gold,
            Color2 = Color.Yellow,
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 1.0f,
            LifeMax = 1.8f,
            SpeedMin = 15f,
            SpeedMax = 25f,
            DirectionRange = (float)Math.PI * 2f
        };

        // Visual components
        public PlayerSprite Sprite;
        public PlayerHair Hair;
        public BadelineAutoAnimator AutoAnimator;
        public SineWave Wave;
        public VertexLight Light;

        // Adeleine-specific properties
        private bool isPainting = false;
        private float paintTime = 0f;
        private const float MAX_PAINT_TIME = 2.0f;
        private ElementalType currentPaintElement = ElementalType.None;
        
        // Animal friend summoning
        private List<AnimalFriendType> availableAnimalFriends;
        private int currentAnimalIndex = 0;
        private float animalSummonCooldown = 0f;
        private const float ANIMAL_SUMMON_COOLDOWN = 3.0f;
        private List<Entity> summonedAnimals;
        
        // Paint abilities
        private float paintEnergy = 100f;
        private const float MAX_PAINT_ENERGY = 100f;
        private const float PAINT_COST = 25f;
        private const float ANIMAL_SUMMON_COST = 40f;
        
        // Following behavior
        protected global::Celeste.Player player;
        protected bool following = false;
        protected float followDistance = 45f;
        
        // Sound
        private SoundSource paintSound;

        public enum AnimalFriendType
        {
            Rick,   // Enhanced wall climbing
            Kine,   // Water currents
            Coo     // Wind currents and flying
        }

        public AdeleineDreamFriendDummy(Vector2 position, int index = 0) : base(position)
        {
        }

        // Standard Everest constructor for map loading
        public AdeleineDreamFriendDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Int("index", 0))
        {
            // Initialize animal friends
            availableAnimalFriends = new List<AnimalFriendType>
            {
                AnimalFriendType.Rick,
                AnimalFriendType.Kine,
                AnimalFriendType.Coo
            };
            
            summonedAnimals = new List<Entity>();
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            SetupVisuals();
            SetupMovement();
        }

        private void SetupVisuals()
        {
            // Create Adeleine sprite
            Sprite = new PlayerSprite(PlayerSpriteMode.Adeline);
            Sprite.Play("idle", false, false);
            Sprite.Scale.X = -1f; // Face left initially
            
            // Skip hair initialization - creating PlayerHair with null causes NullReferenceException
            Hair = null;
            Add(Sprite);
            
            // Create auto animator
            Add(AutoAnimator = new BadelineAutoAnimator());
            
            // Create floating wave effect
            Add(Wave = new SineWave(0.25f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                Sprite.Position = Vector2.UnitY * f * 2f;
            };
            
            // Create light with artistic colors
            Add(Light = new VertexLight(new Vector2(0f, -8f), Color.Pink, 1f, 22, 55));
            
            // Create paint sound
            paintSound = new SoundSource();
            Add(paintSound);
        }

        private void SetupMovement()
        {
            Add(new PlayerCollider(OnPlayer, null, null));
        }

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            if (player.StateMachine.State == global::Celeste.Player.StNormal)
            {
                // Painting attack - hold grab to paint, release to execute
                if (Input.Grab.Pressed && paintEnergy >= PAINT_COST && !isPainting)
                {
                    StartPainting();
                }
                else if (Input.Grab.Released && isPainting)
                {
                    ExecutePaintAttack(player);
                }
                
                // Animal friend summoning - press dash to cycle, hold to summon
                if (Input.Dash.Pressed && animalSummonCooldown <= 0f)
                {
                    if (paintEnergy >= ANIMAL_SUMMON_COST)
                    {
                        SummonAnimalFriend();
                    }
                    else
                    {
                        CycleAnimalFriend();
                    }
                }
                
                // Elemental paint switching - press jump to cycle elements
                if (Input.Jump.Pressed)
                {
                    CyclePaintElement();
                }
            }
        }

        private void StartPainting()
        {
            isPainting = true;
            paintTime = 0f;
            Sprite.Play("paint", false, false);
            paintSound.Play("event:/char/madeline/grab");
            
            Add(new Coroutine(PaintingRoutine()));
        }

        private IEnumerator PaintingRoutine()
        {
            while (isPainting && paintTime < MAX_PAINT_TIME)
            {
                paintTime += Engine.DeltaTime;
                
                // Emit painting particles
                Color paintColor = GetElementColor(currentPaintElement);
                SceneAs<Level>()?.ParticlesFG.Emit(CreatePaintParticles(paintColor), 3, 
                    Position + Vector2.UnitY * -8f, Vector2.One * 8f);
                
                // Increase light intensity while painting
                float paintRatio = paintTime / MAX_PAINT_TIME;
                Light.Alpha = 1f + paintRatio * 0.6f;
                Light.Color = Color.Lerp(Color.Pink, paintColor, paintRatio);
                
                yield return null;
            }
        }

        private void ExecutePaintAttack(global::Celeste.Player player)
        {
            isPainting = false;
            float paintRatio = Math.Min(paintTime / MAX_PAINT_TIME, 1f);
            
            // Consume energy
            paintEnergy -= PAINT_COST;
            
            // Calculate attack power
            float attackPower = 0.5f + paintRatio * 1.5f;
            Vector2 paintDirection = new Vector2(Sprite.Scale.X, 0f);
            
            // Execute elemental paint attack
            ExecuteElementalPaintAttack(player, paintDirection, attackPower);
            
            // Reset visual state
            paintTime = 0f;
            Light.Alpha = 1f;
            Light.Color = Color.Pink;
            
            Sprite.Play("paint_finish", false, false);
        }

        private void ExecuteElementalPaintAttack(global::Celeste.Player player, Vector2 direction, float power)
        {
            switch (currentPaintElement)
            {
                case ElementalType.Splash: // Cleaning (Water)
                    CreateCleaningWaterAttack(player, direction, power);
                    Audio.Play("event:/char/madeline/water_dash", Position);
                    break;
                    
                case ElementalType.Blizzard: // Blizzard (Ice)
                    CreateBlizzardIceAttack(player, direction, power);
                    Audio.Play("event:/char/madeline/dash_blue", Position);
                    break;
                    
                case ElementalType.Sizzle: // Fire painting
                    CreateFirePaintAttack(player, direction, power);
                    Audio.Play("event:/char/madeline/dash_red", Position);
                    break;
                    
                default: // Regular paint attack
                    CreateRegularPaintAttack(player, direction, power);
                    Audio.Play("event:/char/madeline/dash_pink", Position);
                    break;
            }
            
            // Create paint platforms for movement assistance
            CreatePaintPlatforms(direction, power);
        }

        private void CreateCleaningWaterAttack(global::Celeste.Player player, Vector2 direction, float power)
        {
            // Cleaning ability - removes corruption, dirt, and obstacles
            Level level = SceneAs<Level>();
            if (level == null) return;
            
            Add(new Coroutine(CleaningWaterRoutine(direction, power)));
            
            // Boost player with water current
            player.Speed.Y = Math.Min(player.Speed.Y, -120f * power);
            player.Speed.X += direction.X * 90f * power;
        }

        private IEnumerator CleaningWaterRoutine(Vector2 direction, float power)
        {
            for (float t = 0f; t < 2.5f; t += Engine.DeltaTime)
            {
                Vector2 cleaningPos = Position + direction * (t * 80f);
                
                // Emit cleaning water particles
                SceneAs<Level>()?.ParticlesFG.Emit(CreateWaterParticles(), 4,
                    cleaningPos, Vector2.One * 10f);
                
                // Clean nearby obstacles
                Level level = SceneAs<Level>();
                if (level != null)
                {
                    foreach (var entity in level.Entities)
                    {
                        if (Vector2.Distance(entity.Position, cleaningPos) < 32f)
                        {
                            // Remove dirt, corruption, and certain obstacles
                            if (entity.GetType().Name.Contains("Dirt") ||
                                entity.GetType().Name.Contains("Corruption") ||
                                entity.GetType().Name.Contains("Vine") ||
                                entity.GetType().Name.Contains("Weed"))
                            {
                                entity.RemoveSelf();
                                
                                // Cleaning effect
                                level.ParticlesFG.Emit(P_Magic, 6, entity.Position, Vector2.One * 8f);
                            }
                        }
                    }
                }
                
                yield return null;
            }
        }

        private void CreateBlizzardIceAttack(global::Celeste.Player player, Vector2 direction, float power)
        {
            // Blizzard ability - creates ice platforms and freeze effects
            Level level = SceneAs<Level>();
            if (level == null) return;
            
            // Boost player with ice dash
            player.Speed.Y = Math.Min(player.Speed.Y, -100f * power);
            player.Speed.X += direction.X * 70f * power;
            
            // Create ice platform trail
            Add(new Coroutine(BlizzardIceRoutine(direction, power)));
        }

        private IEnumerator BlizzardIceRoutine(Vector2 direction, float power)
        {
            for (int i = 1; i <= 4; i++)
            {
                Vector2 icePos = Position + direction * (i * 40f) + Vector2.UnitY * 12f;
                
                // Create temporary ice platform
                Entity icePlatform = new Entity(icePos);
                icePlatform.Collider = new Hitbox(32f, 8f, -16f, -4f);
                icePlatform.Add(new PlayerCollider(OnIcePlatform));
                
                SceneAs<Level>()?.Add(icePlatform);
                
                // Emit ice particles
                SceneAs<Level>()?.ParticlesFG.Emit(CreateIceParticles(), 6,
                    icePos, Vector2.One * 8f);
                
                // Remove platform after duration
                Add(new Coroutine(RemovePlatformAfterTime(icePlatform, 4f + i * 0.5f)));
                
                yield return 0.2f;
            }
        }

        private void CreateFirePaintAttack(global::Celeste.Player player, Vector2 direction, float power)
        {
            // Fire painting - burns obstacles and provides fire boost
            player.Speed.Y = Math.Min(player.Speed.Y, -140f * power);
            player.Speed.X += direction.X * 100f * power;
            
            Add(new Coroutine(FirePaintRoutine(direction, power)));
        }

        private IEnumerator FirePaintRoutine(Vector2 direction, float power)
        {
            for (float t = 0f; t < 2f; t += Engine.DeltaTime)
            {
                Vector2 firePos = Position + direction * (t * 90f);
                
                // Emit fire particles
                SceneAs<Level>()?.ParticlesFG.Emit(CreateFireParticles(), 5,
                    firePos, Vector2.One * 8f);
                
                // Burn obstacles
                Level level = SceneAs<Level>();
                if (level != null)
                {
                    foreach (var entity in level.Entities)
                    {
                        if (Vector2.Distance(entity.Position, firePos) < 28f)
                        {
                            if (entity.GetType().Name.Contains("IceBlock") ||
                                entity.GetType().Name.Contains("Grass") ||
                                entity.GetType().Name.Contains("Wood"))
                            {
                                entity.RemoveSelf();
                                level.ParticlesFG.Emit(CreateFireParticles(), 8, entity.Position, Vector2.One * 12f);
                            }
                        }
                    }
                }
                
                yield return null;
            }
        }

        private void CreateRegularPaintAttack(global::Celeste.Player player, Vector2 direction, float power)
        {
            // Regular paint attack - creates paint platforms
            player.Speed.Y = Math.Min(player.Speed.Y, -110f * power);
            player.Speed.X += direction.X * 80f * power;
            
            CreatePaintPlatforms(direction, power);
        }

        private void CreatePaintPlatforms(Vector2 direction, float power)
        {
            Level level = SceneAs<Level>();
            if (level == null) return;
            
            // Create paint platforms for movement assistance
            for (int i = 1; i <= 3; i++)
            {
                Vector2 platformPos = Position + direction * (i * 35f) + Vector2.UnitY * (8f + i * 4f);
                
                Entity paintPlatform = new Entity(platformPos);
                paintPlatform.Collider = new Hitbox(28f, 6f, -14f, -3f);
                paintPlatform.Add(new PlayerCollider(OnPaintPlatform));
                
                level.Add(paintPlatform);
                
                // Paint platform particles
                SceneAs<Level>()?.ParticlesFG.Emit(P_Paint, 4,
                    platformPos, Vector2.One * 6f);
                
                // Remove platform after duration
                Add(new Coroutine(RemovePlatformAfterTime(paintPlatform, 3f + i * 0.3f)));
            }
        }

        private void OnIcePlatform(global::Celeste.Player player)
        {
            // Ice platform provides enhanced grip
            if (player.Speed.Y > 0f)
            {
                player.Speed.Y *= 0.6f;
            }
        }

        private void OnPaintPlatform(global::Celeste.Player player)
        {
            // Paint platform provides slight bounce
            if (player.Speed.Y > 0f)
            {
                player.Speed.Y *= 0.8f;
            }
        }

        private IEnumerator RemovePlatformAfterTime(Entity platform, float duration)
        {
            yield return duration;
            platform?.RemoveSelf();
        }

        private void SummonAnimalFriend()
        {
            animalSummonCooldown = ANIMAL_SUMMON_COOLDOWN;
            paintEnergy -= ANIMAL_SUMMON_COST;
            
            AnimalFriendType animalType = availableAnimalFriends[currentAnimalIndex];
            Entity animalFriend = CreateAnimalFriend(animalType);
            
            if (animalFriend != null)
            {
                SceneAs<Level>()?.Add(animalFriend);
                summonedAnimals.Add(animalFriend);
                
                // Limit number of summoned animals
                if (summonedAnimals.Count > 2)
                {
                    Entity oldestAnimal = summonedAnimals[0];
                    oldestAnimal?.RemoveSelf();
                    summonedAnimals.RemoveAt(0);
                }
                
                // Summoning effect
                SceneAs<Level>()?.ParticlesFG.Emit(P_Magic, 12, 
                    Position + Vector2.UnitX * 20f, Vector2.One * 12f);
                Audio.Play("event:/char/madeline/crystalheart_pulse", Position);
            }
        }

        private Entity CreateAnimalFriend(AnimalFriendType animalType)
        {
            Vector2 summonPos = Position + Vector2.UnitX * (Sprite.Scale.X * 30f);
            
            switch (animalType)
            {
                case AnimalFriendType.Rick:
                    return new SummonedRick(summonPos);
                    
                case AnimalFriendType.Kine:
                    return new SummonedKine(summonPos);
                    
                case AnimalFriendType.Coo:
                    return new SummonedCoo(summonPos);
                    
                default:
                    return null;
            }
        }

        private void CycleAnimalFriend()
        {
            currentAnimalIndex = (currentAnimalIndex + 1) % availableAnimalFriends.Count;
            
            // Visual indication of animal switch
            Color animalColor = GetAnimalColor(availableAnimalFriends[currentAnimalIndex]);
            SceneAs<Level>()?.ParticlesFG.Emit(CreateAnimalParticles(animalColor), 6, 
                Position + Vector2.UnitY * -8f, Vector2.One * 6f);
            
            Audio.Play("event:/ui/main/button_select", Position);
        }

        private void CyclePaintElement()
        {
            // Cycle through paint elements
            switch (currentPaintElement)
            {
                case ElementalType.None:
                    currentPaintElement = ElementalType.Splash; // Cleaning
                    break;
                case ElementalType.Splash:
                    currentPaintElement = ElementalType.Blizzard; // Ice
                    break;
                case ElementalType.Blizzard:
                    currentPaintElement = ElementalType.Sizzle; // Fire
                    break;
                default:
                    currentPaintElement = ElementalType.None; // Regular
                    break;
            }
            
            // Visual indication
            Color elementColor = GetElementColor(currentPaintElement);
            Light.Color = Color.Lerp(Color.Pink, elementColor, 0.3f);
            
            Audio.Play("event:/ui/main/button_select", Position);
        }

        // Helper methods
        private Color GetElementColor(ElementalType elementType)
        {
            switch (elementType)
            {
                case ElementalType.Sizzle: return Color.OrangeRed;
                case ElementalType.Blizzard: return Color.LightBlue;
                case ElementalType.Splash: return Color.CornflowerBlue;
                default: return Color.Pink;
            }
        }

        private Color GetAnimalColor(AnimalFriendType animalType)
        {
            switch (animalType)
            {
                case AnimalFriendType.Rick: return Color.Brown;
                case AnimalFriendType.Kine: return Color.Blue;
                case AnimalFriendType.Coo: return Color.Purple;
                default: return Color.White;
            }
        }

        private ParticleType CreatePaintParticles(Color color)
        {
            return new ParticleType
            {
                Size = 1f,
                Color = color,
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Fade,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.8f,
                LifeMax = 1.4f,
                SpeedMin = 20f,
                SpeedMax = 40f,
                DirectionRange = (float)Math.PI * 2f,
                Acceleration = new Vector2(0f, 30f)
            };
        }

        private ParticleType CreateWaterParticles()
        {
            return new ParticleType
            {
                Size = 1f,
                Color = Color.CornflowerBlue,
                Color2 = Color.LightBlue,
                ColorMode = ParticleType.ColorModes.Fade,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.6f,
                LifeMax = 1.2f,
                SpeedMin = 15f,
                SpeedMax = 35f,
                DirectionRange = (float)Math.PI * 2f,
                Acceleration = new Vector2(0f, 50f)
            };
        }

        private ParticleType CreateIceParticles()
        {
            return new ParticleType
            {
                Size = 1f,
                Color = Color.LightBlue,
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 1.0f,
                LifeMax = 1.8f,
                SpeedMin = 10f,
                SpeedMax = 25f,
                DirectionRange = (float)Math.PI * 2f,
                Acceleration = new Vector2(0f, 20f)
            };
        }

        private ParticleType CreateFireParticles()
        {
            return new ParticleType
            {
                Size = 1f,
                Color = Color.OrangeRed,
                Color2 = Color.Yellow,
                ColorMode = ParticleType.ColorModes.Fade,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.8f,
                LifeMax = 1.4f,
                SpeedMin = 20f,
                SpeedMax = 40f,
                DirectionRange = (float)Math.PI * 2f,
                Acceleration = new Vector2(0f, -30f)
            };
        }

        private ParticleType CreateAnimalParticles(Color color)
        {
            return new ParticleType
            {
                Size = 1f,
                Color = color,
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.5f,
                LifeMax = 1.0f,
                SpeedMin = 10f,
                SpeedMax = 20f,
                DirectionRange = (float)Math.PI * 2f
            };
        }

        public override void Update()
        {
            base.Update();
            
            // Update cooldowns
            if (animalSummonCooldown > 0f)
            {
                animalSummonCooldown -= Engine.DeltaTime;
            }
            
            // Regenerate paint energy
            if (paintEnergy < MAX_PAINT_ENERGY)
            {
                paintEnergy += 20f * Engine.DeltaTime;
                paintEnergy = Math.Min(paintEnergy, MAX_PAINT_ENERGY);
            }
            
            // Update sprite facing
            if (player != null && following)
            {
                Sprite.Scale.X = player.Position.X > Position.X ? 1f : -1f;
                if (Hair != null) Hair.Facing = Sprite.Scale.X > 0 ? Facings.Right : Facings.Left;
                
                // Follow player
                UpdateFollowMovement();
            }
            
            // Clean up removed animals from list
            summonedAnimals.RemoveAll(animal => animal.Scene == null);
        }

        private void UpdateFollowMovement()
        {
            Vector2 targetPosition = player.Position + Vector2.UnitX * -followDistance;
            Vector2 direction = (targetPosition - Position).SafeNormalize();
            
            Position += direction * 75f * Engine.DeltaTime;
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
            
            // Render energy indicator
            RenderEnergyIndicator();
            
            // Render current paint element indicator
            RenderPaintElementIndicator();
            
            // Render current animal friend indicator
            RenderAnimalFriendIndicator();
        }

        private void RenderEnergyIndicator()
        {
            Vector2 barPos = Position + Vector2.UnitY * -25f;
            float barWidth = 32f;
            float barHeight = 3f;
            float energyRatio = paintEnergy / MAX_PAINT_ENERGY;
            
            // Background
            Draw.Rect(barPos.X - barWidth/2, barPos.Y, barWidth, barHeight, Color.DarkGray);
            
            // Energy fill
            if (energyRatio > 0f)
            {
                Draw.Rect(barPos.X - barWidth/2, barPos.Y, barWidth * energyRatio, barHeight, Color.Pink);
            }
        }

        private void RenderPaintElementIndicator()
        {
            Vector2 indicatorPos = Position + Vector2.UnitY * -30f;
            Color elementColor = GetElementColor(currentPaintElement);
            
            // Small colored circle to indicate current paint element
            Draw.Circle(indicatorPos, 3f, elementColor, 8);
        }

        private void RenderAnimalFriendIndicator()
        {
            Vector2 indicatorPos = Position + Vector2.UnitY * -35f;
            Color animalColor = GetAnimalColor(availableAnimalFriends[currentAnimalIndex]);
            
            // Small colored square to indicate current animal friend
            Draw.Rect(indicatorPos.X - 2f, indicatorPos.Y - 2f, 4f, 4f, animalColor);
        }
    }

    // Simple animal friend helper classes
    public class SummonedRick : Entity
    {
        public SummonedRick(Vector2 position) : base(position)
        {
            Collider = new Hitbox(8f, 8f, -4f, -4f);
            Add(new PlayerCollider(OnPlayerContact));
            
            // Temporary implementation - would have full Rick functionality
        }

        private void OnPlayerContact(global::Celeste.Player player)
        {
            // Enhanced wall climbing assistance
            if (player.StateMachine.State == global::Celeste.Player.StClimb)
            {
                player.Stamina += 2f * Engine.DeltaTime;
                player.Speed.Y = Math.Min(player.Speed.Y, -60f);
            }
        }

        public override void Update()
        {
            base.Update();
            // Follow behavior would be implemented here
        }
    }

    public class SummonedKine : Entity
    {
        public SummonedKine(Vector2 position) : base(position)
        {
            Collider = new Hitbox(8f, 8f, -4f, -4f);
            Add(new PlayerCollider(OnPlayerContact));
        }

        private void OnPlayerContact(global::Celeste.Player player)
        {
            // Water current assistance
            player.Speed += Vector2.UnitX * 40f * Engine.DeltaTime;
        }

        public override void Update()
        {
            base.Update();
            // Water current behavior would be implemented here
        }
    }

    public class SummonedCoo : Entity
    {
        public SummonedCoo(Vector2 position) : base(position)
        {
            Collider = new Hitbox(8f, 8f, -4f, -4f);
            Add(new PlayerCollider(OnPlayerContact));
        }

        private void OnPlayerContact(global::Celeste.Player player)
        {
            // Wind current and flying assistance
            player.Speed.Y = Math.Min(player.Speed.Y, -80f);
            player.Speed += Vector2.UnitY * -20f * Engine.DeltaTime;
        }

        public override void Update()
        {
            base.Update();
            // Flying and wind behavior would be implemented here
        }
    }
}



