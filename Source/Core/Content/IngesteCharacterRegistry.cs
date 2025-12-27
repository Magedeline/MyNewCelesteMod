using DesoloZantas.Core.Core.Content.Characters;

namespace DesoloZantas.Core.Core.Content
{
    /// <summary>
    /// Registry and initialization for all 23+ characters in the Ingeste character system
    /// </summary>
    public static class IngesteCharacterRegistry
    {
        private static bool initialized = false;
        
        /// <summary>
        /// Initialize all characters and their abilities
        /// </summary>
        public static void Initialize()
        {
            if (initialized) return;
            
            try
            {
                RegisterKirbySeriesCharacters();
                RegisterUndertaleSeriesCharacters();
                RegisterUndertaleYellowCharacters();
                RegisterOtherSeriesCharacters();
                
                initialized = true;
                Logger.Log(LogLevel.Info, "IngesteCharacterRegistry", "Successfully registered all 23+ characters");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteCharacterRegistry", $"Failed to initialize characters: {ex.Message}");
            }
        }
        
        private static void RegisterKirbySeriesCharacters()
        {
            // King DDD - Powerful hammer boosts and ground shockwaves
            CharacterAbilityRegistry.RegisterCharacter(
                "king_ddd", "King DDD", "Kirby Series",
                "Powerful hammer boosts and ground shockwaves for enhanced movement",
                "king_ddd_sprite",
                new KingDDDAbility()
            );
            
            // Meta Knight - Enhanced sword dashes (existing, enhanced)
            CharacterAbilityRegistry.RegisterCharacter(
                "meta_knight", "Meta Knight", "Kirby Series",
                "Enhanced sword dashes with aerial movement assistance",
                "meta_knight_sprite",
                new MetaKnightAbility()
            );
            
            // Bandana Waddle Dee - Spear platforms for enhanced jumping (existing, enhanced)
            CharacterAbilityRegistry.RegisterCharacter(
                "bandana_waddle_dee", "Bandana Waddle Dee", "Kirby Series",
                "Creates temporary spear platforms for enhanced jumping",
                "bandana_waddle_dee_sprite",
                new BandanaWaddleDeeAbility()
            );
            
            // Magolor - Portal-based movement assistance
            CharacterAbilityRegistry.RegisterCharacter(
                "magolor", "Magolor", "Kirby Series",
                "Portal-based movement assistance for difficult navigation",
                "magolor_sprite",
                new MagolorAbility()
            );
            
            // Susie Haltmann - Mechanical boost systems
            CharacterAbilityRegistry.RegisterCharacter(
                "susie_haltmann", "Susie Haltmann", "Kirby Series",
                "Mechanical boost systems with dash enhancement technology",
                "susie_sprite",
                new SusieAbility()
            );
            
            // Taranza - Web-based climbing assistance
            CharacterAbilityRegistry.RegisterCharacter(
                "taranza", "Taranza", "Kirby Series",
                "Web-based climbing assistance and aerial navigation",
                "taranza_sprite",
                new TaranzaAbility()
            );
            
            // Squeaker - Treasure detection and navigation help
            CharacterAbilityRegistry.RegisterCharacter(
                "squeaker", "Squeaker", "Kirby Series",
                "Treasure detection and hidden path navigation assistance",
                "squeaker_sprite",
                new SqueakerAbility()
            );
            
            // Dark Meta Knight - Shadow dash abilities
            CharacterAbilityRegistry.RegisterCharacter(
                "dark_meta_knight", "Dark Meta Knight", "Kirby Series",
                "Shadow dash abilities with phase-through mechanics",
                "dark_meta_knight_sprite",
                new DarkMetaKnightAbility()
            );
            
            // Marx - Chaotic teleportation and trickery
            CharacterAbilityRegistry.RegisterCharacter(
                "marx", "Marx", "Kirby Series",
                "Chaotic teleportation and unpredictable movement assistance",
                "marx_sprite",
                new MarxAbility()
            );
            
            // Three Mage Sisters - Elemental movement assistance
            CharacterAbilityRegistry.RegisterCharacter(
                "francisca", "Francisca", "Kirby Series",
                "Ice-based movement assistance with frozen platforms",
                "francisca_sprite",
                new FranciscaAbility()
            );
            
            CharacterAbilityRegistry.RegisterCharacter(
                "flamberge", "Flamberge", "Kirby Series",
                "Fire-based movement assistance with thermal updrafts",
                "flamberge_sprite",
                new FlambergeAbility()
            );
            
            CharacterAbilityRegistry.RegisterCharacter(
                "zan_partizanne", "Zan Partizanne", "Kirby Series",
                "Electric-based movement assistance with lightning dashes",
                "zan_partizanne_sprite",
                new ZanPartizanneAbility()
            );
            
            // Kirby Classic - Original Game Boy puff abilities
            CharacterAbilityRegistry.RegisterCharacter(
                "kirby_classic", "Kirby Classic", "Kirby Series",
                "Original Game Boy puff abilities with fall damage protection",
                "kirby_classic_sprite",
                new KirbyClassicAbility()
            );
            
            // Gooey - Enhanced with tongue wall grab mechanics
            CharacterAbilityRegistry.RegisterCharacter(
                "gooey", "Gooey", "Kirby Series",
                "Enhanced wall grab mechanics with tongue assistance",
                "gooey_sprite",
                new GooeyAbility()
            );
        }
        
        private static void RegisterUndertaleSeriesCharacters()
        {
            // Asriel - Hope boosts with compassion protection aura
            CharacterAbilityRegistry.RegisterCharacter(
                "asriel", "Asriel", "Undertale",
                "Hope boosts with compassion protection for safer navigation",
                "asriel_sprite",
                new AsrielAbility()
            );
            
            // Frisk - Determination-powered movement boosts (existing, enhanced)
            CharacterAbilityRegistry.RegisterCharacter(
                "frisk", "Frisk", "Undertale",
                "Determination-powered movement boosts and persistence",
                "frisk_sprite",
                new FriskAbility()
            );
            
            // Charlo - Celeste mod crossover abilities
            CharacterAbilityRegistry.RegisterCharacter(
                "charlo", "Charlo", "Deltarune",
                "Celeste mod crossover abilities for navigation",
                "charlo_sprite",
                new CharloAbility()
            );
        }
        
        private static void RegisterUndertaleYellowCharacters()
        {
            // Clover - Justice-themed precision movement
            CharacterAbilityRegistry.RegisterCharacter(
                "clover", "Clover", "Undertale Yellow",
                "Justice-themed precision movement and accurate dashing",
                "clover_sprite",
                new CloverAbility()
            );
            
            // Melody & Batty - Musical rhythm-based abilities
            CharacterAbilityRegistry.RegisterCharacter(
                "melody", "Melody", "Fan Games",
                "Musical rhythm-based movement with beat synchronization",
                "melody_sprite",
                new MelodyAbility()
            );
            
            CharacterAbilityRegistry.RegisterCharacter(
                "batty", "Batty", "Fan Games",
                "Aerial navigation assistance with echolocation",
                "batty_sprite",
                new BattyAbility()
            );
            
            // Emily, Cody & Odin - Rainbow-themed elemental assists
            CharacterAbilityRegistry.RegisterCharacter(
                "emily", "Emily", "Fan Games",
                "Rainbow dash trails with chromatic movement boosts",
                "emily_sprite",
                new EmilyAbility()
            );
            
            CharacterAbilityRegistry.RegisterCharacter(
                "cody", "Cody", "Fan Games",
                "Code-based puzzle solving and path optimization",
                "cody_sprite",
                new CodyAbility()
            );
            
            CharacterAbilityRegistry.RegisterCharacter(
                "odin", "Odin", "Fan Games",
                "Wisdom-based navigation with foresight abilities",
                "odin_sprite",
                new OdinAbility()
            );
        }
        
        private static void RegisterOtherSeriesCharacters()
        {
            // Adeline/Ado - Paint-based platform creation
            CharacterAbilityRegistry.RegisterCharacter(
                "adeleine", "Adeleine", "Kirby Series",
                "Paint-based platform creation and artistic assistance",
                "adeleine_sprite",
                new AdelineAbility()
            );
            
            // Ness - PSI teleportation and platform abilities (existing, enhanced)
            CharacterAbilityRegistry.RegisterCharacter(
                "ness", "Ness", "EarthBound",
                "PSI teleportation and platform creation abilities",
                "ness_sprite",
                new NessAbility()
            );
        }
    }
    
    /// <summary>
    /// Additional character abilities for the extended roster
    /// </summary>
    namespace Characters
    {
        // Susie Haltmann - Mechanical boost systems
        public class SusieAbility : MovementAssistAbility
        {
            private float mechanicalBoostTimer = 0f;
            
            public SusieAbility() 
                : base("susie_haltmann", "Mech Boost", "Mechanical boost systems with dash enhancement", 1.35f, false, 1)
            {
            }
            
            protected override void OnActivate() 
            { 
                base.OnActivate();
                mechanicalBoostTimer = 0f;
            }
            
            protected override void OnDeactivate() { }
            
            protected override void OnUpdate()
            {
                mechanicalBoostTimer += Engine.DeltaTime;
                
                // Provide periodic mechanical assistance
                if (mechanicalBoostTimer >= 3f && player != null && !player.OnGround())
                {
                    ProvideMechanicalAssist();
                    mechanicalBoostTimer = 0f;
                }
            }
            
            private void ProvideMechanicalAssist()
            {
                // Mechanical dash restore
                if (player.Dashes < 1)
                {
                    player.Dashes = 1;
                    PlaySound("event:/char/madeline/jump");
                    CreateParticles(ParticleTypes.Dust, 6, player.Position, Vector2.One * 10f);
                }
            }
            
            protected override void OnDashBoost(Vector2 dashDirection)
            {
                // Mechanical enhancement effects
                CreateParticles(ParticleTypes.Dust, 4, player.Position, Vector2.One * 8f);
            }
        }
        
        // Taranza - Web-based climbing assistance
        public class TaranzaAbility : CharacterAbility
        {
            private float webRange = 80f;
            private bool webActive = false;
            
            public TaranzaAbility() 
                : base("taranza", "Web Assist", "Web-based climbing assistance and aerial navigation")
            {
            }
            
            protected override void OnActivate() 
            { 
                webActive = false;
            }
            
            protected override void OnDeactivate() 
            { 
                webActive = false;
            }
            
            protected override void OnUpdate()
            {
                if (player == null) return;
                
                // Activate web assistance when falling or climbing
                if (Input.Grab.Check && !player.OnGround())
                {
                    CheckWebAssist();
                }
            }
            
            private void CheckWebAssist()
            {
                // Look for surfaces within web range
                Vector2[] directions = { Vector2.UnitX, -Vector2.UnitX, Vector2.UnitY, -Vector2.UnitY };
                
                foreach (var dir in directions)
                {
                    Vector2 checkPos = player.Position + dir * webRange;
                    if (level?.CollideCheck<Solid>(checkPos) == true)
                    {
                        // Create web assistance
                        if (!webActive)
                        {
                            webActive = true;
                            player.Speed.Y *= 0.3f; // Slow fall
                            PlaySound("event:/char/madeline/grabwall");
                            CreateParticles(ParticleTypes.Dust, 4, player.Position, Vector2.One * 6f);
                        }
                        return;
                    }
                }
                
                webActive = false;
            }
        }
        
        // Additional character abilities continue...
        public class SqueakerAbility : CharacterAbility
        {
            public SqueakerAbility() : base("squeaker", "Treasure Sense", "Navigation and hidden path detection") { }
            protected override void OnActivate() { }
            protected override void OnDeactivate() { }
            protected override void OnUpdate() { }
        }
        
        public class DarkMetaKnightAbility : MovementAssistAbility
        {
            public DarkMetaKnightAbility() : base("dark_meta_knight", "Shadow Dash", "Phase-through abilities", 1.3f) { }
            protected override void OnActivate() { base.OnActivate(); }
            protected override void OnDeactivate() { }
            protected override void OnUpdate() { }
        }
        
        public class MarxAbility : TeleportationAbility
        {
            public MarxAbility() : base("marx", "Chaos Teleport", "Chaotic movement assistance", 120f, 2f) { }
            protected override void OnActivate() { }
            protected override void OnDeactivate() { }
            protected override void OnUpdate() { }
        }
        
        public class FranciscaAbility : PlatformCreationAbility
        {
            public FranciscaAbility() : base("francisca", "Ice Platform", "Ice platform creation", 6f) { }
            protected override void OnActivate() { }
            protected override void OnDeactivate() { base.OnDeactivate(); }
            protected override void OnUpdate() { }
        }
        
        public class FlambergeAbility : MovementAssistAbility
        {
            public FlambergeAbility() : base("flamberge", "Fire Boost", "Thermal updraft assistance", 1.25f) { }
            protected override void OnActivate() { base.OnActivate(); }
            protected override void OnDeactivate() { }
            protected override void OnUpdate() { }
        }
        
        public class ZanPartizanneAbility : MovementAssistAbility
        {
            public ZanPartizanneAbility() : base("zan_partizanne", "Lightning Dash", "Electric dash enhancement", 1.4f) { }
            protected override void OnActivate() { base.OnActivate(); }
            protected override void OnDeactivate() { }
            protected override void OnUpdate() { }
        }
        
        public class MelodyAbility : CharacterAbility
        {
            public MelodyAbility() : base("melody", "Rhythm Assist", "Musical rhythm-based movement") { }
            protected override void OnActivate() { }
            protected override void OnDeactivate() { }
            protected override void OnUpdate() { }
        }
        
        public class BattyAbility : CharacterAbility
        {
            public BattyAbility() : base("batty", "Echo Navigate", "Aerial navigation with echolocation") { }
            protected override void OnActivate() { }
            protected override void OnDeactivate() { }
            protected override void OnUpdate() { }
        }
        
        public class EmilyAbility : MovementAssistAbility
        {
            public EmilyAbility() : base("emily", "Rainbow Dash", "Chromatic movement boosts", 1.2f) { }
            protected override void OnActivate() { base.OnActivate(); }
            protected override void OnDeactivate() { }
            protected override void OnUpdate() { }
        }
        
        public class CodyAbility : CharacterAbility
        {
            public CodyAbility() : base("cody", "Code Optimize", "Path optimization assistance") { }
            protected override void OnActivate() { }
            protected override void OnDeactivate() { }
            protected override void OnUpdate() { }
        }
        
        public class OdinAbility : CharacterAbility
        {
            public OdinAbility() : base("odin", "Foresight", "Wisdom-based navigation") { }
            protected override void OnActivate() { }
            protected override void OnDeactivate() { }
            protected override void OnUpdate() { }
        }
        
        public class AdelineAbility : PlatformCreationAbility
        {
            public AdelineAbility() : base("adeleine", "Paint Platform", "Artistic platform creation", 8f) { }
            protected override void OnActivate() { }
            protected override void OnDeactivate() { base.OnDeactivate(); }
            protected override void OnUpdate() { }
        }
    }
}



