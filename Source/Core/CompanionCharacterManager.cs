using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core
{
    public static class CompanionCharacterManager
    {
        // Registry of all available companion characters
        private static readonly Dictionary<PlayerSpriteMode, CompanionCharacterInfo> companionRegistry = new()
        {
            { PlayerSpriteMode.Gooey, new CompanionCharacterInfo("Gooey", "Tongue grab walls for assistance", () => new GooeySiDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.MetaKnight, new CompanionCharacterInfo("Meta Knight", "Enhanced sword dashes", () => new MetaKnightDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.BandanaWaddleDee, new CompanionCharacterInfo("Bandana Waddle Dee", "Spear platforms for jumping", () => new BandanaWaddleDeeDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Frisk, new CompanionCharacterInfo("Frisk", "Determination boosts", () => new FriskDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Rick, new CompanionCharacterInfo("Rick", "Wall climbing assistance", () => new RickDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Kine, new CompanionCharacterInfo("Kine", "Water currents for movement", () => new KineDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Coo, new CompanionCharacterInfo("Coo", "Wind currents and flying carry", () => new CooDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Ness, new CompanionCharacterInfo("Ness", "PSI platforms and teleportation", () => new NessDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.KingDDD, new CompanionCharacterInfo("King DDD", "Powerful hammer boosts and shockwaves", () => new KingDDDDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Asriel, new CompanionCharacterInfo("Asriel", "Hope boosts and compassion protection", () => new AsrielDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Marx, new CompanionCharacterInfo("Marx", "Chaotic teleportation and trickery", () => new MarxDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.KirbyClassic, new CompanionCharacterInfo("Kirby Classic", "Classic puff abilities and fall assistance", () => new KirbyClassicDummy(Vector2.Zero, 1)) },
            // Missing characters now added
            { PlayerSpriteMode.Adeline, new CompanionCharacterInfo("Adeline", "Paint platforms for jumping assistance", () => new AdelineDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Clover, new CompanionCharacterInfo("Clover", "Justice boosts and star trail guidance", () => new CloverDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Melody, new CompanionCharacterInfo("Melody", "Musical rhythm assistance", () => new MelodyDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Batty, new CompanionCharacterInfo("Batty", "Flight assistance and echo navigation", () => new BattyDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Emily, new CompanionCharacterInfo("Emily", "Bravery boosts and energy assistance", () => new EmilyDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Cody, new CompanionCharacterInfo("Cody", "Patience and steady movement", () => new CodyDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Odin, new CompanionCharacterInfo("Odin", "Perseverance and endurance", () => new OdinDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Charlo, new CompanionCharacterInfo("Charlo", "Determination and focus", () => new CharloDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Magolor, new CompanionCharacterInfo("Magolor", "Magic boosts and portal assistance", () => new MagolorDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.SusieHaltmann, new CompanionCharacterInfo("Susie Haltmann", "Technology and mechanical assistance", () => new SusieHaltmannDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Taranza, new CompanionCharacterInfo("Taranza", "Web platforms and spider assistance", () => new TaranzaDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.Squeaker, new CompanionCharacterInfo("Squeaker", "Squeak sound navigation", () => new SqueakerDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.DarkMetaKnight, new CompanionCharacterInfo("Dark Meta Knight", "Shadow dash enhancement", () => new DarkMetaKnightDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.FranZalea, new CompanionCharacterInfo("Fran Zalea", "Frost platforms and ice assistance", () => new FranZaleaDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.FlambergeZalea, new CompanionCharacterInfo("Flamberge Zalea", "Flame boost and fire assistance", () => new FlambergeZaleaDummy(Vector2.Zero, 1)) },
            { PlayerSpriteMode.HynesZalea, new CompanionCharacterInfo("Hynes Zalea", "Electric boost and lightning assistance", () => new HynesZaleaDummy(Vector2.Zero, 1)) }
        };

        // Currently active companions
        private static readonly List<Entity> activeCompanions = new();
        
        // Maximum number of active companions
        private const int MAX_COMPANIONS = 3;

        public struct CompanionCharacterInfo
        {
            public string Name { get; }
            public string Description { get; }
            public Func<Entity> CreateCompanion { get; }

            public CompanionCharacterInfo(string name, string description, Func<Entity> createCompanion)
            {
                Name = name;
                Description = description;
                CreateCompanion = createCompanion;
            }
        }

        public static IEnumerable<PlayerSpriteMode> GetAvailableCompanions()
        {
            return companionRegistry.Keys;
        }

        public static CompanionCharacterInfo GetCompanionInfo(PlayerSpriteMode spriteMode)
        {
            return companionRegistry.TryGetValue(spriteMode, out var info) ? info : default;
        }

        public static void SpawnCompanion(Level level, PlayerSpriteMode companionType, Vector2 position)
        {
            // Remove excess companions if at max capacity
            while (activeCompanions.Count >= MAX_COMPANIONS)
            {
                var oldCompanion = activeCompanions[0];
                activeCompanions.RemoveAt(0);
                oldCompanion?.RemoveSelf();
            }

            // Spawn new companion
            if (companionRegistry.TryGetValue(companionType, out var companionInfo))
            {
                var companion = companionInfo.CreateCompanion();
                companion.Position = position;
                level.Add(companion);
                activeCompanions.Add(companion);

                // Play spawn sound
                Audio.Play("event:/char/companion/spawn", position);
            }
        }

        public static void SpawnRandomCompanions(Level level, Vector2 playerPosition, int count = 1)
        {
            var availableCompanions = new List<PlayerSpriteMode>(companionRegistry.Keys);
            
            for (int i = 0; i < Math.Min(count, MAX_COMPANIONS); i++)
            {
                if (availableCompanions.Count == 0) break;
                
                int randomIndex = Calc.Random.Next(availableCompanions.Count);
                var selectedCompanion = availableCompanions[randomIndex];
                availableCompanions.RemoveAt(randomIndex);
                
                Vector2 spawnPos = playerPosition + new Vector2(
                    (i - count / 2f) * 20f,
                    -10f
                );
                
                SpawnCompanion(level, selectedCompanion, spawnPos);
            }
        }

        public static void ClearAllCompanions()
        {
            foreach (var companion in activeCompanions)
            {
                companion?.RemoveSelf();
            }
            activeCompanions.Clear();
        }

        public static void UpdateCompanionPositions(Vector2 playerPosition)
        {
            // Clean up removed companions
            activeCompanions.RemoveAll(c => c == null || c.Scene == null);
            
            // Update companion follow positions to spread them out nicely
            for (int i = 0; i < activeCompanions.Count; i++)
            {
                var companion = activeCompanions[i];
                if (companion != null)
                {
                    // Spread companions in a formation around the player
                    float angle = (float)i / activeCompanions.Count * MathHelper.TwoPi;
                    Vector2 offset = new Vector2(
                        (float)Math.Cos(angle) * 30f,
                        (float)Math.Sin(angle) * 15f
                    );
                    
                    // Note: Actual position updating is handled by individual companion Update() methods
                    // This is just for formation calculation if needed
                }
            }
        }

        public static int GetActiveCompanionCount()
        {
            activeCompanions.RemoveAll(c => c == null || c.Scene == null);
            return activeCompanions.Count;
        }

        public static List<string> GetActiveCompanionNames()
        {
            activeCompanions.RemoveAll(c => c == null || c.Scene == null);
            var names = new List<string>();
            
            foreach (var companion in activeCompanions)
            {
                // Try to determine companion type from the companion entity
                foreach (var kvp in companionRegistry)
                {
                    if (companion.GetType().Name.Contains(kvp.Value.Name.Replace(" ", "")))
                    {
                        names.Add(kvp.Value.Name);
                        break;
                    }
                }
            }
            
            return names;
        }

        // Hook into level loading to spawn initial companions
        public static void OnLevelStart(Level level)
        {
            // Clear any existing companions when starting a new level
            ClearAllCompanions();
            
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null)
            {
                // Spawn default companions or based on save data
                SpawnRandomCompanions(level, player.Position, 2);
            }
        }

        // Hook into level transition
        public static void OnLevelTransition(Level level)
        {
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null)
            {
                UpdateCompanionPositions(player.Position);
            }
        }
    }
}



