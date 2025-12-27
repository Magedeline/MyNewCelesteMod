namespace DesoloZantas.Core.Core.Content
{
    /// <summary>
    /// Base class for all character abilities in the Ingeste character system
    /// Ensures all abilities work within Celeste's dash-only movement limitations
    /// </summary>
    public abstract class CharacterAbility
    {
        public string CharacterId { get; protected set; }
        public string AbilityName { get; protected set; }
        public string Description { get; protected set; }
        public bool IsActive { get; protected set; }
        
        protected CelestePlayer player;
        protected Level level;
        
        public CharacterAbility(string characterId, string abilityName, string description)
        {
            CharacterId = characterId;
            AbilityName = abilityName;
            Description = description;
            IsActive = false;
        }
        
        /// <summary>
        /// Initialize the ability for the current player and level
        /// </summary>
        public virtual void Initialize(CelestePlayer player, Level level)
        {
            this.player = player;
            this.level = level;
        }
        
        /// <summary>
        /// Activate the character ability
        /// </summary>
        public virtual void Activate()
        {
            IsActive = true;
            OnActivate();
        }
        
        /// <summary>
        /// Deactivate the character ability
        /// </summary>
        public virtual void Deactivate()
        {
            IsActive = false;
            OnDeactivate();
        }
        
        /// <summary>
        /// Update the ability state - called every frame when active
        /// </summary>
        public virtual void Update()
        {
            if (IsActive && player != null)
            {
                OnUpdate();
            }
        }
        
        /// <summary>
        /// Called when the ability is activated
        /// </summary>
        protected abstract void OnActivate();
        
        /// <summary>
        /// Called when the ability is deactivated
        /// </summary>
        protected abstract void OnDeactivate();
        
        /// <summary>
        /// Called every frame when the ability is active
        /// </summary>
        protected abstract void OnUpdate();
        
        /// <summary>
        /// Called when the player dashes - allows abilities to enhance dash behavior
        /// </summary>
        public virtual void OnPlayerDash(Vector2 dashDirection) { }
        
        /// <summary>
        /// Called when the player lands on ground
        /// </summary>
        public virtual void OnPlayerLand() { }
        
        /// <summary>
        /// Called when the player grabs a wall
        /// </summary>
        public virtual void OnPlayerWallGrab() { }
        
        /// <summary>
        /// Helper method to safely play audio
        /// </summary>
        protected void PlaySound(string eventPath)
        {
            try
            {
                Audio.Play(eventPath);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "CharacterAbility", $"Failed to play sound {eventPath}: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Helper method to create particles safely
        /// </summary>
        protected void CreateParticles(ParticleType particleType, int count, Vector2 position, Vector2 range)
        {
            try
            {
                level?.ParticlesFG?.Emit(particleType, count, position, range);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "CharacterAbility", $"Failed to create particles: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Movement assistance ability that enhances player dash behavior
    /// </summary>
    public abstract class MovementAssistAbility : CharacterAbility
    {
        protected float dashBoostMultiplier;
        protected bool providesAirDash;
        protected int extraDashes;
        
        public MovementAssistAbility(string characterId, string abilityName, string description, 
                                   float dashBoostMultiplier = 1.0f, bool providesAirDash = false, int extraDashes = 0)
            : base(characterId, abilityName, description)
        {
            this.dashBoostMultiplier = dashBoostMultiplier;
            this.providesAirDash = providesAirDash;
            this.extraDashes = extraDashes;
        }
        
        protected override void OnActivate()
        {
            if (player != null && extraDashes > 0)
            {
                // Temporarily boost dash count for movement assistance
                player.Dashes = Math.Min(player.Dashes + extraDashes, 3); // Cap at 3 dashes max
            }
        }
        
        public override void OnPlayerDash(Vector2 dashDirection)
        {
            if (IsActive && dashBoostMultiplier > 1.0f)
            {
                // Enhance dash with movement boost
                player.Speed *= dashBoostMultiplier;
                OnDashBoost(dashDirection);
            }
        }
        
        /// <summary>
        /// Called when the dash boost is applied
        /// </summary>
        protected virtual void OnDashBoost(Vector2 dashDirection) { }
    }
    
    /// <summary>
    /// Platform creation ability for creating temporary platforms or assistance
    /// </summary>
    public abstract class PlatformCreationAbility : CharacterAbility
    {
        protected List<Entity> createdPlatforms;
        protected float platformDuration;
        
        public PlatformCreationAbility(string characterId, string abilityName, string description, float platformDuration = 5.0f)
            : base(characterId, abilityName, description)
        {
            this.platformDuration = platformDuration;
            createdPlatforms = new List<Entity>();
        }
        
        protected override void OnDeactivate()
        {
            // Clean up any created platforms
            foreach (var platform in createdPlatforms)
            {
                platform?.RemoveSelf();
            }
            createdPlatforms.Clear();
        }
        
        /// <summary>
        /// Create a temporary platform at the specified position
        /// </summary>
        protected virtual Entity CreateTemporaryPlatform(Vector2 position, Vector2 size)
        {
            // This will be implemented by specific character abilities
            return null;
        }
    }
    
    /// <summary>
    /// Teleportation ability for portal-based or instant movement
    /// </summary>
    public abstract class TeleportationAbility : CharacterAbility
    {
        protected float teleportRange;
        protected float cooldownTime;
        protected float lastTeleportTime;
        
        public TeleportationAbility(string characterId, string abilityName, string description, 
                                  float teleportRange = 100.0f, float cooldownTime = 2.0f)
            : base(characterId, abilityName, description)
        {
            this.teleportRange = teleportRange;
            this.cooldownTime = cooldownTime;
            lastTeleportTime = 0;
        }
        
        protected bool CanTeleport()
        {
            return IsActive && (Engine.Scene.TimeActive - lastTeleportTime) >= cooldownTime;
        }
        
        protected void PerformTeleport(Vector2 targetPosition)
        {
            if (!CanTeleport()) return;
            
            lastTeleportTime = Engine.Scene.TimeActive;
            player.Position = targetPosition;
            OnTeleportComplete(targetPosition);
        }
        
        /// <summary>
        /// Called after a successful teleport
        /// </summary>
        protected virtual void OnTeleportComplete(Vector2 position) 
        {
            PlaySound("event:/char/badeline/disappear");
            CreateParticles(ParticleTypes.Dust, 8, position, Vector2.One * 12f);
        }
    }
    
    /// <summary>
    /// Registry and manager for all character abilities
    /// </summary>
    public static class CharacterAbilityRegistry
    {
        private static Dictionary<string, CharacterData> characters = new Dictionary<string, CharacterData>();
        private static Dictionary<string, CharacterAbility> activeAbilities = new Dictionary<string, CharacterAbility>();
        
        public struct CharacterData
        {
            public string Id;
            public string Name;
            public string Series;
            public string Description;
            public List<CharacterAbility> Abilities;
            public string SpriteId;
            
            public CharacterData(string id, string name, string series, string description, string spriteId)
            {
                Id = id;
                Name = name;
                Series = series;
                Description = description;
                Abilities = new List<CharacterAbility>();
                SpriteId = spriteId;
            }
        }
        
        /// <summary>
        /// Register a character with their abilities
        /// </summary>
        public static void RegisterCharacter(string id, string name, string series, string description, 
                                           string spriteId, params CharacterAbility[] abilities)
        {
            var characterData = new CharacterData(id, name, series, description, spriteId);
            characterData.Abilities.AddRange(abilities);
            characters[id] = characterData;
            
            Logger.Log(LogLevel.Info, "CharacterAbility", $"Registered character: {name} ({id})");
        }
        
        /// <summary>
        /// Activate abilities for a specific character
        /// </summary>
        public static void ActivateCharacter(string characterId, CelestePlayer player, Level level)
        {
            DeactivateAllCharacters();
            
            if (characters.TryGetValue(characterId, out var characterData))
            {
                foreach (var ability in characterData.Abilities)
                {
                    ability.Initialize(player, level);
                    ability.Activate();
                    activeAbilities[ability.AbilityName] = ability;
                }
                
                Logger.Log(LogLevel.Info, "CharacterAbility", $"Activated character: {characterData.Name}");
            }
        }
        
        /// <summary>
        /// Deactivate all active character abilities
        /// </summary>
        public static void DeactivateAllCharacters()
        {
            foreach (var ability in activeAbilities.Values)
            {
                ability.Deactivate();
            }
            activeAbilities.Clear();
        }
        
        /// <summary>
        /// Update all active abilities
        /// </summary>
        public static void Update()
        {
            foreach (var ability in activeAbilities.Values)
            {
                ability.Update();
            }
        }
        
        /// <summary>
        /// Handle player dash events for all active abilities
        /// </summary>
        public static void OnPlayerDash(Vector2 dashDirection)
        {
            foreach (var ability in activeAbilities.Values)
            {
                ability.OnPlayerDash(dashDirection);
            }
        }
        
        /// <summary>
        /// Get all registered characters
        /// </summary>
        public static Dictionary<string, CharacterData> GetAllCharacters()
        {
            return new Dictionary<string, CharacterData>(characters);
        }
        
        /// <summary>
        /// Get character data by ID
        /// </summary>
        public static CharacterData? GetCharacter(string characterId)
        {
            return characters.TryGetValue(characterId, out var data) ? data : null;
        }
    }
}



