namespace DesoloZantas.Core.Core.Systems
{
    /// <summary>
    /// Elemental types for Dream Friend abilities
    /// </summary>
    public enum ElementalType
    {
        None,
        Sizzle,     // Fire-based attacks
        Blizzard,   // Ice-based attacks  
        Splash,     // Water-based attacks
        Zap,        // Electricity-based attacks
        Bluster     // Wind-based attacks
    }

    /// <summary>
    /// Base class for elemental abilities shared across Dream Friends
    /// </summary>
    public abstract class ElementalAbility
    {
        public ElementalType ElementType { get; protected set; }
        public string ElementName { get; protected set; }
        public Color ElementColor { get; protected set; }
        public ParticleType ElementParticles { get; protected set; }
        public string ElementSound { get; protected set; }
        
        protected float elementPower = 1.0f;
        protected float elementDuration = 2.0f;
        protected bool isActive = false;

        protected ElementalAbility(ElementalType type)
        {
            ElementType = type;
            InitializeElementProperties();
        }

        private void InitializeElementProperties()
        {
            switch (ElementType)
            {
                case ElementalType.Sizzle:
                    ElementName = "Sizzle";
                    ElementColor = Color.OrangeRed;
                    ElementSound = "event:/char/madeline/dash_red";
                    ElementParticles = CreateFireParticles();
                    break;
                    
                case ElementalType.Blizzard:
                    ElementName = "Blizzard";
                    ElementColor = Color.LightBlue;
                    ElementSound = "event:/char/madeline/dash_blue";
                    ElementParticles = CreateIceParticles();
                    break;
                    
                case ElementalType.Splash:
                    ElementName = "Splash";
                    ElementColor = Color.CornflowerBlue;
                    ElementSound = "event:/char/madeline/water_dash";
                    ElementParticles = CreateWaterParticles();
                    break;
                    
                case ElementalType.Zap:
                    ElementName = "Zap";
                    ElementColor = Color.Yellow;
                    ElementSound = "event:/char/madeline/dash_yellow";
                    ElementParticles = CreateElectricParticles();
                    break;
                    
                case ElementalType.Bluster:
                    ElementName = "Bluster";
                    ElementColor = Color.LightGreen;
                    ElementSound = "event:/char/madeline/dash_green";
                    ElementParticles = CreateWindParticles();
                    break;
                    
                default:
                    ElementName = "None";
                    ElementColor = Color.White;
                    ElementSound = "";
                    ElementParticles = null;
                    break;
            }
        }

        public virtual void Activate(Vector2 position, Vector2 direction)
        {
            if (isActive) return;
            
            isActive = true;
            Audio.Play(ElementSound, position);
            OnActivate(position, direction);
        }

        public virtual void Deactivate()
        {
            if (!isActive) return;
            
            isActive = false;
            OnDeactivate();
        }

        protected abstract void OnActivate(Vector2 position, Vector2 direction);
        protected abstract void OnDeactivate();

        public virtual void Update(Vector2 position)
        {
            if (isActive)
            {
                OnUpdate(position);
            }
        }

        protected virtual void OnUpdate(Vector2 position) { }

        /// <summary>
        /// Apply elemental effect to nearby entities
        /// </summary>
        public virtual void ApplyElementalEffect(Level level, Vector2 center, float radius, Entity sourceEntity)
        {
            if (!isActive) return;

            foreach (var entity in level.Entities)
            {
                if (entity == sourceEntity) continue;
                
                float distance = Vector2.Distance(entity.Position, center);
                if (distance <= radius)
                {
                    ApplyElementalEffectToEntity(entity, center, distance);
                }
            }
        }

        protected virtual void ApplyElementalEffectToEntity(Entity entity, Vector2 center, float distance)
        {
            switch (ElementType)
            {
                case ElementalType.Sizzle:
                    ApplyFireEffect(entity, center, distance);
                    break;
                case ElementalType.Blizzard:
                    ApplyIceEffect(entity, center, distance);
                    break;
                case ElementalType.Splash:
                    ApplyWaterEffect(entity, center, distance);
                    break;
                case ElementalType.Zap:
                    ApplyElectricEffect(entity, center, distance);
                    break;
                case ElementalType.Bluster:
                    ApplyWindEffect(entity, center, distance);
                    break;
            }
        }

        protected virtual void ApplyFireEffect(Entity entity, Vector2 center, float distance)
        {
            // Fire melts ice blocks, destroys grass/vines
            if (entity.GetType().Name.Contains("IceBlock") || 
                entity.GetType().Name.Contains("Grass") ||
                entity.GetType().Name.Contains("Vine"))
            {
                entity.RemoveSelf();
            }
        }

        protected virtual void ApplyIceEffect(Entity entity, Vector2 center, float distance)
        {
            // Ice freezes water, creates temporary platforms
            if (entity is global::Celeste.Player player)
            {
                // Slight movement slowdown but enhanced grip
                player.Speed *= 0.9f;
            }
        }

        protected virtual void ApplyWaterEffect(Entity entity, Vector2 center, float distance)
        {
            // Water extinguishes fire, cleans dirt/corruption
            if (entity.GetType().Name.Contains("Fire") ||
                entity.GetType().Name.Contains("Dirt") ||
                entity.GetType().Name.Contains("Corruption"))
            {
                entity.RemoveSelf();
            }
        }

        protected virtual void ApplyElectricEffect(Entity entity, Vector2 center, float distance)
        {
            // Electric activates machines, destroys metal barriers
            if (entity.GetType().Name.Contains("Switch") ||
                entity.GetType().Name.Contains("PowerGenerator"))
            {
                // Activate electronic entities
            }
            else if (entity.GetType().Name.Contains("MetalBlock"))
            {
                entity.RemoveSelf();
            }
        }

        protected virtual void ApplyWindEffect(Entity entity, Vector2 center, float distance)
        {
            // Wind blows away light objects, activates windmills
            if (entity is global::Celeste.Player player)
            {
                Vector2 windDirection = (player.Position - center).SafeNormalize();
                player.Speed += windDirection * 50f * Engine.DeltaTime;
            }
        }

        // Particle system creation methods
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

        private ParticleType CreateElectricParticles()
        {
            return new ParticleType
            {
                Size = 1f,
                Color = Color.Yellow,
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.3f,
                LifeMax = 0.7f,
                SpeedMin = 30f,
                SpeedMax = 60f,
                DirectionRange = (float)Math.PI * 2f
            };
        }

        private ParticleType CreateWindParticles()
        {
            return new ParticleType
            {
                Size = 1f,
                Color = Color.LightGreen,
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Fade,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 1.2f,
                LifeMax = 2.0f,
                SpeedMin = 25f,
                SpeedMax = 45f,
                DirectionRange = (float)Math.PI / 4f, // More directional
                Acceleration = new Vector2(0f, -10f)
            };
        }
    }

    /// <summary>
    /// Manages elemental combinations and interactions
    /// </summary>
    public static class ElementalComboSystem
    {
        private static Dictionary<(ElementalType, ElementalType), ElementalType> combos = 
            new Dictionary<(ElementalType, ElementalType), ElementalType>
            {
                // Steam combinations
                { (ElementalType.Sizzle, ElementalType.Splash), ElementalType.Bluster },
                { (ElementalType.Splash, ElementalType.Sizzle), ElementalType.Bluster },
                
                // Ice storm combinations  
                { (ElementalType.Blizzard, ElementalType.Bluster), ElementalType.Zap },
                { (ElementalType.Bluster, ElementalType.Blizzard), ElementalType.Zap },
                
                // Electric water combinations
                { (ElementalType.Zap, ElementalType.Splash), ElementalType.Sizzle },
                { (ElementalType.Splash, ElementalType.Zap), ElementalType.Sizzle }
            };

        public static ElementalType GetComboResult(ElementalType element1, ElementalType element2)
        {
            if (combos.TryGetValue((element1, element2), out ElementalType result))
            {
                return result;
            }
            return ElementalType.None;
        }

        public static bool AreElementsOpposite(ElementalType element1, ElementalType element2)
        {
            return (element1 == ElementalType.Sizzle && element2 == ElementalType.Blizzard) ||
                   (element1 == ElementalType.Blizzard && element2 == ElementalType.Sizzle) ||
                   (element1 == ElementalType.Zap && element2 == ElementalType.Splash) ||
                   (element1 == ElementalType.Splash && element2 == ElementalType.Zap);
        }
    }

    /// <summary>
    /// Special skills that Dream Friends can possess
    /// </summary>
    public enum SpecialSkill
    {
        None,
        Slash,          // Cut through obstacles
        Launch,         // Destroy blocks by throwing
        FoodGeneration  // Generate healing items
    }

    /// <summary>
    /// Base class for Dream Friend entities with elemental abilities
    /// </summary>
    public abstract class DreamFriendDummy : Entity
    {
        protected ElementalAbility currentElement;
        protected List<ElementalType> availableElements;
        protected List<SpecialSkill> specialSkills;
        
        protected PlayerSprite Sprite;
        protected PlayerHair Hair;
        protected BadelineAutoAnimator AutoAnimator;
        protected SineWave Wave;
        protected VertexLight Light;
        
        protected global::Celeste.Player player;
        protected bool following = false;
        protected float followDistance = 40f;

        public DreamFriendDummy(Vector2 position) : base(position)
        {
            availableElements = new List<ElementalType>();
            specialSkills = new List<SpecialSkill>();
            
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            
            InitializeDreamFriend();
            SetupVisuals();
            SetupMovement();
        }

        protected abstract void InitializeDreamFriend();
        
        protected virtual void SetupVisuals()
        {
            Add(Wave = new SineWave(0.2f, 0f));
            Wave.OnUpdate = delegate(float f)
            {
                if (Sprite != null)
                    Sprite.Position = Vector2.UnitY * f * 2.5f;
            };
        }

        protected virtual void SetupMovement()
        {
            Add(new PlayerCollider(OnPlayer, null, null));
        }

        protected abstract void OnPlayer(global::Celeste.Player player);

        public virtual void SetElementalAbility(ElementalType elementType)
        {
            if (!availableElements.Contains(elementType)) return;
            
            currentElement?.Deactivate();
            currentElement = CreateElementalAbility(elementType);
        }

        protected abstract ElementalAbility CreateElementalAbility(ElementalType elementType);

        public virtual void ActivateElementalAbility(Vector2 direction)
        {
            currentElement?.Activate(Position, direction);
        }

        public override void Update()
        {
            base.Update();
            currentElement?.Update(Position);
            
            if (following && player != null)
            {
                UpdateFollowMovement();
            }
        }

        protected virtual void UpdateFollowMovement()
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
    }
}



