using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Identifiers for all Kirby copy abilities supported by the Ingeste mod.
    /// </summary>
    public enum KirbyCopyAbilityId
    {
        None = 0,
        Sword,
        Stone,
        Spark,
        Fire,
        Ice,
        Archer,
        Leaf,
        Water,
        Esp,
        Hammer,
        Ranger,
        Mike,
        Crash,
        Bomb,
        Cutter,
        Painter,
        Cook,
        Bell,
        Light,
        Drill,
        Beam,
        Wheel,
        Phase,
        TripleSwap,
        TimeCrash,
        Umbrella,
        Mirror,
        Recycler,
        Mini,
        InfernoSuper,
        GrandHammerSuper,
        MechanizeRangerSuper,
        FrostMindSuper,
        UltraSwordSuper,
        KnightSuper
    }

    /// <summary>
    /// Definition data for a Kirby copy ability.
    /// </summary>
    internal sealed class KirbyCopyAbilityDefinition
    {
        public KirbyCopyAbilityDefinition(
            KirbyCopyAbilityId id,
            string displayName,
            string description,
            float attackRating,
            float defenseRating,
            Func<KirbyAbilityBehavior> behaviorFactory,
            bool isSuperAbility = false)
        {
            Id = id;
            DisplayName = displayName;
            Description = description;
            AttackRating = attackRating;
            DefenseRating = defenseRating;
            BehaviorFactory = behaviorFactory;
            IsSuperAbility = isSuperAbility;
        }

        public KirbyCopyAbilityId Id { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public float AttackRating { get; }
        public float DefenseRating { get; }
        public bool IsSuperAbility { get; }
        internal Func<KirbyAbilityBehavior> BehaviorFactory { get; }

        public KirbyAbilityBehavior CreateBehavior() => BehaviorFactory();
    }

    /// <summary>
    /// Runtime behavior contract for Kirby copy abilities.
    /// </summary>
    internal abstract class KirbyAbilityBehavior
    {
        internal KirbyCopyAbilityRuntime Runtime { get; private set; }

        internal void AttachRuntime(KirbyCopyAbilityRuntime runtime)
        {
            Runtime = runtime;
        }

        protected KirbyCopyAbilityDefinition Definition => Runtime.Definition;

        public virtual void OnEnter(KirbyAbilityContext context) { }
        public virtual void OnExit(KirbyAbilityContext context) { }
        public virtual void OnUpdate(KirbyAbilityContext context, float deltaTime) { }
        public virtual bool HandleDash(KirbyAbilityContext context, Vector2 dashDirection) => false;
        public virtual bool HandleSpecial(KirbyAbilityContext context) => false;
        public virtual int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage) => attemptedDamage;
        public virtual int ModifyDamageDealt(KirbyAbilityContext context, int attemptedDamage) => attemptedDamage;
        public virtual void OnEnemyHit(KirbyAbilityContext context, Entity enemy, ref int appliedDamage) { }
        public virtual void OnPlayerLanded(KirbyAbilityContext context) { }
    }

    /// <summary>
    /// Convenience wrapper passed to ability behaviors.
    /// </summary>
    internal readonly struct KirbyAbilityContext
    {
        public KirbyAbilityContext(KirbyCopyAbilityRuntime runtime)
        {
            Runtime = runtime;
            Player = runtime.Player;
        }

    internal KirbyCopyAbilityRuntime Runtime { get; }
    public KirbyPlayer Player { get; }
    public Scene Scene => Player != null ? Player.Scene : null;
    public Level Level => Scene as Level;
    public float AttackMultiplier => Runtime.AttackMultiplier;
    public float DefenseMultiplier => Runtime.DefenseMultiplier;

        public ParticleSystem Particles => Level?.ParticlesFG;

        public void SpawnParticles(ParticleType type, int count, Vector2? range = null)
        {
            if (Particles == null || Player == null)
                return;

            Vector2 particleRange = range ?? new Vector2(12f, 12f);
            Particles.Emit(type, count, Player.Position, particleRange);
        }

        public void SpawnParticlesAt(Vector2 position, ParticleType type, int count, Vector2? range = null)
        {
            if (Particles == null)
                return;

            Vector2 particleRange = range ?? new Vector2(12f, 12f);
            Particles.Emit(type, count, position, particleRange);
        }

        public void PlaySound(string eventPath)
        {
            if (string.IsNullOrWhiteSpace(eventPath))
                return;

            AudioHelper.PlaySafe(eventPath, "event:/char/madeline/jump"); // Fallback to a vanilla Celeste sound
        }

        public int DealAreaDamage(float radius, int baseDamage)
            => KirbyCopyAbilityManager.DealAreaDamage(this, radius, baseDamage);

        public int DealLineDamage(Vector2 direction, float length, int baseDamage, float width = 16f)
            => KirbyCopyAbilityManager.DealLineDamage(this, direction, length, baseDamage, width);

        public void Heal(int amount)
            => KirbyCopyAbilityManager.Heal(Player, amount);

        public void Schedule(float delaySeconds, Action action)
            => KirbyCopyAbilityManager.Schedule(Scene, delaySeconds, action);

        public void ShakeLevel(float amount)
        {
            if (Level != null)
            {
                Level.Shake(amount);
            }
        }
    }

    internal sealed class KirbyCopyAbilityRuntime
    {
        public KirbyCopyAbilityRuntime(KirbyPlayer player, KirbyCopyAbilityDefinition definition)
        {
            Player = player;
            Definition = definition;
            Behavior = definition.CreateBehavior() ?? new NullAbilityBehavior();
            Behavior.AttachRuntime(this);
        }

        public KirbyPlayer Player { get; }
        public KirbyCopyAbilityDefinition Definition { get; }
        public KirbyAbilityBehavior Behavior { get; }

        public float AttackMultiplier => 1f + Definition.AttackRating * 0.25f;
        public float DefenseMultiplier => 1f + Definition.DefenseRating * 0.25f;
    }

    internal sealed class NullAbilityBehavior : KirbyAbilityBehavior { }

    /// <summary>
    /// Core manager that tracks active Kirby copy abilities per player instance.
    /// </summary>
    public static class KirbyCopyAbilityManager
    {
        private static readonly Dictionary<KirbyCopyAbilityId, KirbyCopyAbilityDefinition> Definitions = new();
        private static readonly Dictionary<KirbyPlayer, KirbyCopyAbilityRuntime> ActiveRuntimes = new();
        private static bool initialized;

        public static void Initialize()
        {
            if (initialized)
                return;

            initialized = true;
            RegisterDefaultAbilities();
        }

        public static void EnsureInitialized()
        {
            if (!initialized)
            {
                Initialize();
            }
        }

        internal static KirbyCopyAbilityDefinition GetDefinition(KirbyCopyAbilityId id)
        {
            EnsureInitialized();
            return Definitions.TryGetValue(id, out var def) ? def : null;
        }

        public static KirbyCopyAbilityId GetCurrentAbilityId(KirbyPlayer player)
        {
            if (player == null) return KirbyCopyAbilityId.None;
            if (ActiveRuntimes.TryGetValue(player, out var runtime))
                return runtime.Definition.Id;
            return KirbyCopyAbilityId.None;
        }

        public static void SwitchAbility(KirbyPlayer player, KirbyCopyAbilityId abilityId)
        {
            EnsureInitialized();
            if (player == null)
                return;

            if (ActiveRuntimes.TryGetValue(player, out var existing))
            {
                var exitContext = new KirbyAbilityContext(existing);
                try
                {
                    existing.Behavior.OnExit(exitContext);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Warn, "KirbyAbilities", $"OnExit error for {existing.Definition.Id}: {ex}");
                }
                ActiveRuntimes.Remove(player);
            }

            if (abilityId == KirbyCopyAbilityId.None || !Definitions.TryGetValue(abilityId, out var definition))
            {
                return;
            }

            var runtime = new KirbyCopyAbilityRuntime(player, definition);
            ActiveRuntimes[player] = runtime;

            var enterContext = new KirbyAbilityContext(runtime);
            try
            {
                runtime.Behavior.OnEnter(enterContext);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "KirbyAbilities", $"OnEnter error for {definition.Id}: {ex}");
            }
        }

        public static void ClearAbility(KirbyPlayer player)
        {
            if (player == null)
                return;

            if (ActiveRuntimes.TryGetValue(player, out var runtime))
            {
                try
                {
                    runtime.Behavior.OnExit(new KirbyAbilityContext(runtime));
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Warn, "KirbyAbilities", $"OnExit error during clear for {runtime.Definition.Id}: {ex}");
                }
                ActiveRuntimes.Remove(player);
            }
        }

        public static void Update(KirbyPlayer player, float deltaTime)
        {
            if (player == null)
                return;

            if (!ActiveRuntimes.TryGetValue(player, out var runtime))
                return;

            try
            {
                runtime.Behavior.OnUpdate(new KirbyAbilityContext(runtime), deltaTime);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "KirbyAbilities", $"Update error for {runtime.Definition.Id}: {ex}");
            }
        }

        public static bool HandleDash(KirbyPlayer player, Vector2 dashDirection)
        {
            if (player == null)
                return false;

            if (!ActiveRuntimes.TryGetValue(player, out var runtime))
                return false;

            try
            {
                return runtime.Behavior.HandleDash(new KirbyAbilityContext(runtime), dashDirection);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "KirbyAbilities", $"HandleDash error for {runtime.Definition.Id}: {ex}");
                return false;
            }
        }

        public static bool HandleSpecial(KirbyPlayer player)
        {
            if (player == null)
                return false;

            if (!ActiveRuntimes.TryGetValue(player, out var runtime))
                return false;

            try
            {
                return runtime.Behavior.HandleSpecial(new KirbyAbilityContext(runtime));
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "KirbyAbilities", $"HandleSpecial error for {runtime.Definition.Id}: {ex}");
                return false;
            }
        }

        public static void NotifyPlayerLanded(KirbyPlayer player)
        {
            if (player == null)
                return;

            if (!ActiveRuntimes.TryGetValue(player, out var runtime))
                return;

            try
            {
                runtime.Behavior.OnPlayerLanded(new KirbyAbilityContext(runtime));
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "KirbyAbilities", $"OnPlayerLanded error for {runtime.Definition.Id}: {ex}");
            }
        }

        public static int ModifyIncomingDamage(KirbyPlayer player, int damage)
        {
            if (player == null || damage <= 0)
                return 0;

            if (!ActiveRuntimes.TryGetValue(player, out var runtime))
                return Math.Max(0, damage);

            var context = new KirbyAbilityContext(runtime);
            float scaled = damage / runtime.DefenseMultiplier;
            int adjusted = Math.Max(1, (int)Math.Round(scaled));

            try
            {
                adjusted = Math.Max(0, runtime.Behavior.ModifyDamageTaken(context, adjusted));
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "KirbyAbilities", $"ModifyIncomingDamage error for {runtime.Definition.Id}: {ex}");
            }

            return adjusted;
        }

        public static int ModifyOutgoingDamage(KirbyPlayer player, int damage)
        {
            if (player == null || damage <= 0)
                return 0;

            if (!ActiveRuntimes.TryGetValue(player, out var runtime))
                return Math.Max(1, damage);

            var context = new KirbyAbilityContext(runtime);
            float scaled = damage * runtime.AttackMultiplier;
            int adjusted = Math.Max(1, (int)Math.Round(scaled));

            try
            {
                adjusted = Math.Max(1, runtime.Behavior.ModifyDamageDealt(context, adjusted));
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "KirbyAbilities", $"ModifyOutgoingDamage error for {runtime.Definition.Id}: {ex}");
            }

            return adjusted;
        }

        internal static int DealAreaDamage(KirbyAbilityContext context, float radius, int baseDamage)
        {
            if (context.Scene == null)
                return 0;

            int applied = 0;
            int damage = ModifyOutgoingDamage(context.Player, baseDamage);
            Vector2 origin = context.Player.Position;

            foreach (Entity entity in context.Scene.Entities)
            {
                if (entity == null || entity == context.Player)
                    continue;

                if (!entity.Collidable)
                    continue;

                if (Vector2.Distance(entity.Position, origin) > radius)
                    continue;

                if (TryDealDamage(context, entity, damage))
                {
                    applied++;
                }
            }

            return applied;
        }

        internal static int DealLineDamage(KirbyAbilityContext context, Vector2 direction, float length, int baseDamage, float width)
        {
            if (context.Scene == null || direction == Vector2.Zero)
                return 0;

            direction.Normalize();
            int applied = 0;
            int damage = ModifyOutgoingDamage(context.Player, baseDamage);
            Vector2 origin = context.Player.Position;
            Vector2 end = origin + direction * length;

            foreach (Entity entity in context.Scene.Entities)
            {
                if (entity == null || entity == context.Player)
                    continue;

                if (!entity.Collidable)
                    continue;

                float distance = DistanceFromSegment(entity.Position, origin, end);
                if (distance > width)
                    continue;

                if (TryDealDamage(context, entity, damage))
                {
                    applied++;
                }
            }

            return applied;
        }

        internal static int DealAreaDamageAt(KirbyAbilityContext context, Vector2 position, float radius, int baseDamage)
        {
            if (context.Scene == null)
                return 0;

            int damage = ModifyOutgoingDamage(context.Player, baseDamage);
            int applied = 0;

            foreach (Entity entity in context.Scene.Entities)
            {
                if (entity == null || entity == context.Player)
                    continue;

                if (!entity.Collidable)
                    continue;

                if (Vector2.Distance(entity.Position, position) > radius)
                    continue;

                if (TryDealDamage(context, entity, damage))
                {
                    applied++;
                }
            }

            return applied;
        }

        public static void Heal(KirbyPlayer player, int amount)
        {
            player?.Heal(amount);
        }

        public static void Schedule(Scene scene, float delaySeconds, Action action)
        {
            if (action == null)
                return;

            if (scene == null || delaySeconds <= 0f)
            {
                action();
                return;
            }

            scene.Add(new KirbyAbilityDelayedAction(delaySeconds, action));
        }

        private static bool TryDealDamage(KirbyAbilityContext context, Entity entity, int damage)
        {
            try
            {
                var type = entity.GetType();
                var damageMethod = type.GetMethod("TakeDamage", new[] { typeof(int) });
                if (damageMethod != null)
                {
                    damageMethod.Invoke(entity, new object[] { damage });
                    var applied = damage;
                    context.Runtime.Behavior.OnEnemyHit(context, entity, ref applied);
                    return true;
                }

                var damageFloat = type.GetMethod("TakeDamage", new[] { typeof(float) });
                if (damageFloat != null)
                {
                    damageFloat.Invoke(entity, new object[] { (float)damage });
                    var applied = damage;
                    context.Runtime.Behavior.OnEnemyHit(context, entity, ref applied);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "KirbyAbilities", $"Failed to apply damage to {entity?.GetType().Name}: {ex.Message}");
            }

            return false;
        }

        private static float DistanceFromSegment(Vector2 point, Vector2 start, Vector2 end)
        {
            Vector2 seg = end - start;
            float lenSq = seg.LengthSquared();
            if (lenSq <= float.Epsilon)
                return Vector2.Distance(point, start);

            float t = Vector2.Dot(point - start, seg) / lenSq;
            t = MathHelper.Clamp(t, 0f, 1f);
            Vector2 projection = start + seg * t;
            return Vector2.Distance(point, projection);
        }

        private static void RegisterDefaultAbilities()
        {
            Definitions.Clear();

            // Ability definitions will be populated below. Implementations are provided later in the file.
            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Sword,
                "Sword Kirby",
                "Balanced melee slashes with quick burst damage.",
                2.8f,
                1.6f,
                () => new SwordAbility()));

            // Additional abilities are registered in RegisterAdditionalAbilities.
            RegisterAdditionalAbilities();
        }

        private static void RegisterAbility(KirbyCopyAbilityDefinition definition)
        {
            if (definition == null)
                return;

            Definitions[definition.Id] = definition;
        }

        private static void RegisterAdditionalAbilities()
        {
            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Stone,
                "Stone Kirby",
                "Fortify into an immovable fortress with crushing counter attacks.",
                1.2f,
                3.8f,
                () => new StoneAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Spark,
                "Spark Kirby",
                "Charge electricity and discharge shocking barrier blasts.",
                3.2f,
                2.0f,
                () => new SparkAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Fire,
                "Fire Kirby",
                "Dash with blazing bursts that ignite nearby threats.",
                3.0f,
                1.4f,
                () => new FireAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Ice,
                "Ice Kirby",
                "Freeze foes and surf frigid gusts for safe landings.",
                2.4f,
                2.6f,
                () => new IceAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Archer,
                "Archer Kirby",
                "Loose piercing arrows with precision charge shots.",
                2.7f,
                1.5f,
                () => new ArcherAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Leaf,
                "Leaf Kirby",
                "Summon leaf barriers that shred projectiles and soften blows.",
                1.8f,
                3.1f,
                () => new LeafAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Water,
                "Water Kirby",
                "Ride fountain jets while cleansing allies with restorative mist.",
                2.2f,
                2.0f,
                () => new WaterAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Esp,
                "ESP Kirby",
                "Teleport and hurl psychic waves to bend battlefields.",
                2.9f,
                2.3f,
                () => new EspAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Hammer,
                "Hammer Kirby",
                "Deliver seismic hammer slams that crush clustered foes.",
                3.4f,
                2.1f,
                () => new AreaBurstAbility(88f, 28, 0.9f, "event:/kirby/hammer_slam", new Vector2(24f, 16f), 30f, -140f, ctx => ctx.ShakeLevel(0.6f))));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Ranger,
                "Ranger Kirby",
                "Aim long range star shots with ricochet potential.",
                2.8f,
                1.8f,
                () => new RangerAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Mike,
                "Mike Kirby",
                "Unleash three ear-splitting performances that wipe screens.",
                4.0f,
                1.2f,
                () => new MikeAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Crash,
                "Crash Kirby",
                "Trigger a singular nova that obliterates nearby foes.",
                4.6f,
                0.8f,
                () => new CrashAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Bomb,
                "Bomb Kirby",
                "Plant timed star bombs and juggle explosive combos.",
                3.1f,
                1.7f,
                () => new BombAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Cutter,
                "Cutter Kirby",
                "Toss boomerang blades that strike on both departure and return.",
                2.6f,
                1.9f,
                () => new CutterAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Painter,
                "Painter Kirby",
                "Paint platforms and palette shields to reshape terrain.",
                2.0f,
                2.8f,
                () => new PainterAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Cook,
                "Cook Kirby",
                "Simmer inhaled foes into hearty healing dishes.",
                1.7f,
                2.4f,
                () => new CookAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Bell,
                "Bell Kirby",
                "Ring resonant bells that stun and deflect attacks.",
                2.1f,
                3.0f,
                () => new BellAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Light,
                "Light Kirby",
                "Illuminate darkness and phase through hazards with radiance.",
                1.6f,
                2.7f,
                () => new LightAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Drill,
                "Drill Kirby",
                "Burrow with unstoppable corkscrew dives and aerial launchers.",
                3.3f,
                2.0f,
                () => new DrillAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Beam,
                "Beam Kirby",
                "Sweep sweeping energy whips with combo potential.",
                3.0f,
                1.6f,
                () => new BeamAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Wheel,
                "Wheel Kirby",
                "Transform into a tire and blitz across hazards.",
                2.9f,
                1.9f,
                () => new WheelAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Phase,
                "Matrix Kirby",
                "Slip through non-dream solids while retaliating on re-materialize.",
                2.5f,
                3.2f,
                () => new PhaseAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.TripleSwap,
                "Triple Swap Kirby",
                "Queue three random abilities and cycle on command.",
                3.0f,
                2.5f,
                () => new TripleSwapAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.TimeCrash,
                "Time Crash Kirby",
                "Bend time while maintaining Kirby's nimble pace.",
                3.8f,
                2.2f,
                () => new TimeCrashAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Umbrella,
                "Umbrella Kirby",
                "Parasol glide with gentle counters and weather wards.",
                1.5f,
                3.5f,
                () => new UmbrellaAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Mirror,
                "Mirror Kirby",
                "Reflect projectiles and mirror incoming forces.",
                2.0f,
                3.4f,
                () => new MirrorAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Recycler,
                "Garbage Machine Kirby",
                "Process debris into empowered junk barrages.",
                2.3f,
                2.6f,
                () => new RecyclerAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.Mini,
                "Mini Kirby",
                "Shrink down to nimble size with evasive power.",
                1.3f,
                2.9f,
                () => new MiniAbility()));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.InfernoSuper,
                "Inferno Light Kirby",
                "Super ability: ignite entire arenas with blazing light.",
                4.8f,
                2.5f,
                () => new AreaBurstAbility(120f, 42, 1.8f, "event:/kirby/super_inferno", new Vector2(40f, 40f), 0f, -220f, ctx => ctx.ShakeLevel(0.9f)),
                true));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.GrandHammerSuper,
                "Grand Smacker Hammer Kirby",
                "Super ability: colossal hammer smashes that quake landscapes.",
                4.7f,
                2.7f,
                () => new AreaBurstAbility(140f, 48, 2.0f, "event:/kirby/super_grandhammer", new Vector2(48f, 24f), 0f, -260f, ctx => ctx.ShakeLevel(1.0f)),
                true));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.MechanizeRangerSuper,
                "Mechanize Ranger Kirby",
                "Super ability: mechanized barrage of precision star shots.",
                4.5f,
                2.3f,
                () => new MechanizeRangerSuperAbility(),
                true));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.FrostMindSuper,
                "Frosted Mind Kirby",
                "Super ability: mind-freezing icicle storms that halt time.",
                4.2f,
                3.4f,
                () => new FrostMindSuperAbility(),
                true));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.UltraSwordSuper,
                "Ultra Sword Kirby",
                "Super ability: legendary blade arcs that cleave realities.",
                5.0f,
                2.8f,
                () => new AreaBurstAbility(150f, 56, 1.6f, "event:/kirby/super_ultrasword", new Vector2(52f, 30f), 60f, -200f, ctx => ctx.DealLineDamage(Vector2.UnitX, 160f, 40, 24f)),
                true));

            RegisterAbility(new KirbyCopyAbilityDefinition(
                KirbyCopyAbilityId.KnightSuper,
                "Kirby Knight Super",
                "Super ability: channel the knight form with empowered strikes.",
                4.4f,
                3.0f,
                () => new KnightSuperAbility(),
                true));
        }

        private sealed class KirbyAbilityDelayedAction : Entity
        {
            private float timer;
            private readonly Action callback;

            public KirbyAbilityDelayedAction(float delaySeconds, Action callback)
            {
                this.timer = Math.Max(0f, delaySeconds);
                this.callback = callback;
                Tag = Tags.Global;
            }

            public override void Update()
            {
                base.Update();
                timer -= Engine.DeltaTime;
                if (timer <= 0f)
                {
                    callback?.Invoke();
                    RemoveSelf();
                }
            }
        }
    }

    #region Ability Implementations

    internal sealed class SwordAbility : KirbyAbilityBehavior
    {
        private float cooldown;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (cooldown > 0f)
            {
                cooldown -= deltaTime;
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (cooldown > 0f)
                return false;

            cooldown = 0.45f;
            context.PlaySound("event:/kirby/sword_slash");
            context.SpawnParticles(ParticleTypes.Dust, 6, new Vector2(18f, 12f));
            context.DealAreaDamage(64f, 18);
            var player = context.Player;
            if (player != null)
            {
                float horizontal = Input.MoveX.Value;
                if (Math.Abs(horizontal) < 0.1f)
                {
                    horizontal = player.Speed.X >= 0f ? 1f : -1f;
                }

                player.Speed += new Vector2(120f * Math.Sign(horizontal), -40f);
            }
            return true;
        }
    }

    internal sealed class StoneAbility : KirbyAbilityBehavior
    {
        private bool stoneActive;
        private float stoneTimer;

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (!stoneActive)
            {
                stoneActive = true;
                stoneTimer = 3.5f;
                context.PlaySound("event:/kirby/stone_activate");
                context.SpawnParticles(ParticleTypes.Dust, 12, new Vector2(18f, 18f));
                context.Player.Speed = Vector2.Zero;
                return true;
            }

            stoneActive = false;
            context.PlaySound("event:/kirby/stone_cancel");
            context.SpawnParticles(ParticleTypes.Dust, 8, new Vector2(12f, 12f));
            return true;
        }

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (!stoneActive)
                return;

            stoneTimer -= deltaTime;
            context.Player.Speed = Vector2.Zero;

            if (stoneTimer <= 0f)
            {
                stoneActive = false;
                context.PlaySound("event:/kirby/stone_cancel");
            }
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (!stoneActive)
                return attemptedDamage;

            // Stone Kirby shrugs off most damage while active.
            return Math.Max(0, attemptedDamage - 10);
        }

        public override void OnPlayerLanded(KirbyAbilityContext context)
        {
            if (!stoneActive)
                return;

            context.PlaySound("event:/kirby/stone_land");
            context.SpawnParticles(ParticleTypes.Dust, 10, new Vector2(24f, 12f));
            context.DealAreaDamage(72f, 24);
        }
    }

    internal sealed class SparkAbility : KirbyAbilityBehavior
    {
        private float chargeTimer;
        private float chargeDuration = 0.8f;
        private bool charging;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (!charging)
                return;

            chargeTimer += deltaTime;
            context.SpawnParticles(ParticleTypes.Dust, 2, new Vector2(10f, 10f));

            if (chargeTimer >= chargeDuration)
            {
                releaseShockwave(context);
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (!charging)
            {
                charging = true;
                chargeTimer = 0f;
                context.PlaySound("event:/kirby/spark_charge_start");
                return true;
            }

            // Manual release
            releaseShockwave(context);
            return true;
        }

        private void releaseShockwave(KirbyAbilityContext context)
        {
            charging = false;
            chargeTimer = 0f;
            context.PlaySound("event:/kirby/spark_release");
            context.SpawnParticles(ParticleTypes.Dust, 18, new Vector2(20f, 20f));
            context.DealAreaDamage(96f, 22);
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (!charging)
                return attemptedDamage;

            // While charging spark, Kirby gains minor damage resistance.
            return Math.Max(0, attemptedDamage - 4);
        }
    }

    internal sealed class FireAbility : KirbyAbilityBehavior
    {
        private float dashBoostTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (dashBoostTimer > 0f)
            {
                dashBoostTimer -= deltaTime;
                context.SpawnParticles(ParticleTypes.Dust, 2, new Vector2(16f, 10f));
            }
        }

        public override bool HandleDash(KirbyAbilityContext context, Vector2 dashDirection)
        {
            dashBoostTimer = 0.5f;
            context.PlaySound("event:/kirby/fire_dash");
            context.DealAreaDamage(56f, 16);
            Vector2 dir = dashDirection;
            if (dir == Vector2.Zero)
            {
                dir = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
            }
            if (dir == Vector2.Zero)
            {
                dir = Vector2.UnitX;
            }
            dir.Normalize();
            Vector2 speed = context.Player.Speed;
            speed += dir * 140f;
            speed.Y -= 20f;
            context.Player.Speed = speed;
            return true;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            context.PlaySound("event:/kirby/fire_pillar");
            context.SpawnParticles(ParticleTypes.Dust, 12, new Vector2(14f, 28f));
            context.DealAreaDamage(70f, 20);
            return true;
        }
    }

    internal sealed class IceAbility : KirbyAbilityBehavior
    {
        private float glideTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (glideTimer > 0f)
            {
                glideTimer -= deltaTime;
                if (context.Player.Speed.Y > 20f)
                {
                    context.Player.Speed = new Vector2(context.Player.Speed.X, 20f);
                }
                context.SpawnParticles(ParticleTypes.Dust, 2, new Vector2(14f, 18f));
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            context.PlaySound("event:/kirby/ice_wave");
            context.SpawnParticles(ParticleTypes.Dust, 14, new Vector2(22f, 14f));
            context.DealAreaDamage(68f, 18);
            glideTimer = 1.4f;
            return true;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (glideTimer <= 0f)
                return attemptedDamage;

            // While gliding, damage is reduced.
            return Math.Max(0, attemptedDamage - 5);
        }
    }

    internal sealed class AreaBurstAbility : KirbyAbilityBehavior
    {
        private readonly float radius;
        private readonly int damage;
        private readonly float cooldown;
        private readonly string soundEvent;
        private readonly Vector2 particleRange;
        private readonly float horizontalImpulse;
        private readonly float verticalImpulse;
    private readonly Action<KirbyAbilityContext> extraAction;
        private float cooldownTimer;

        public AreaBurstAbility(
            float radius,
            int damage,
            float cooldown,
            string soundEvent,
            Vector2 particleRange,
            float horizontalImpulse = 0f,
            float verticalImpulse = 0f,
            Action<KirbyAbilityContext> extraAction = null)
        {
            this.radius = radius;
            this.damage = damage;
            this.cooldown = Math.Max(0.05f, cooldown);
            this.soundEvent = soundEvent;
            this.particleRange = particleRange;
            this.horizontalImpulse = horizontalImpulse;
            this.verticalImpulse = verticalImpulse;
            this.extraAction = extraAction;
        }

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (cooldownTimer > 0f)
            {
                cooldownTimer -= deltaTime;
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (cooldownTimer > 0f)
                return false;

            cooldownTimer = cooldown;
            if (!string.IsNullOrEmpty(soundEvent))
            {
                context.PlaySound(soundEvent);
            }
            context.SpawnParticles(ParticleTypes.Dust, 12, particleRange);
            context.DealAreaDamage(radius, damage);

            if (context.Player != null)
            {
                Vector2 dir = Input.MoveX.Value >= 0 ? Vector2.UnitX : -Vector2.UnitX;
                context.Player.Speed += new Vector2(horizontalImpulse * dir.X, verticalImpulse);
            }

            if (extraAction != null)
            {
                extraAction(context);
            }
            return true;
        }
    }

    internal sealed class ArcherAbility : KirbyAbilityBehavior
    {
        private bool charging;
        private float chargeTimer;
        private const float maxCharge = 1.4f;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (!charging)
                return;

            chargeTimer += deltaTime;
            context.SpawnParticles(ParticleTypes.Dust, 1, new Vector2(8f, 8f));

            if (chargeTimer >= maxCharge)
            {
                releaseArrow(context);
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (!charging)
            {
                charging = true;
                chargeTimer = 0f;
                context.PlaySound("event:/kirby/archer_draw");
                return true;
            }

            releaseArrow(context);
            return true;
        }

        private void releaseArrow(KirbyAbilityContext context)
        {
            int damage = 12 + (int)(chargeTimer * 20f);
            chargeTimer = 0f;
            charging = false;
            context.PlaySound("event:/kirby/archer_shot");

            Vector2 aim = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
            if (aim == Vector2.Zero)
                aim = Vector2.UnitX;
            aim.Normalize();

            context.DealLineDamage(aim, 220f, damage, 20f);
        }
    }

    internal sealed class LeafAbility : KirbyAbilityBehavior
    {
        private float shieldTimer;
        private float pulseTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (shieldTimer <= 0f)
                return;

            shieldTimer -= deltaTime;
            pulseTimer -= deltaTime;

            if (pulseTimer <= 0f)
            {
                pulseTimer = 0.6f;
                context.SpawnParticles(ParticleTypes.Dust, 10, new Vector2(26f, 26f));
                context.DealAreaDamage(52f, 10);
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            shieldTimer = 5f;
            pulseTimer = 0.2f;
            context.PlaySound("event:/kirby/leaf_shield");
            return true;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (shieldTimer <= 0f)
                return attemptedDamage;

            return Math.Max(0, attemptedDamage - 6);
        }
    }

    internal sealed class WaterAbility : KirbyAbilityBehavior
    {
        private float jetTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (jetTimer > 0f)
            {
                jetTimer -= deltaTime;
                context.SpawnParticles(ParticleTypes.Dust, 3, new Vector2(10f, 26f));
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            jetTimer = 0.8f;
            context.PlaySound("event:/kirby/water_jet");
            context.DealAreaDamage(54f, 14);
            if (context.Player != null)
            {
                context.Player.Speed = new Vector2(context.Player.Speed.X, -220f);
            }
            context.Heal(1);
            return true;
        }
    }

    internal sealed class EspAbility : KirbyAbilityBehavior
    {
        private float cooldown;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (cooldown > 0f)
                cooldown -= deltaTime;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (cooldown > 0f)
                return false;

            cooldown = 0.6f;
            Vector2 direction = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
            if (direction == Vector2.Zero)
                direction = Vector2.UnitX;
            direction.Normalize();

            context.PlaySound("event:/kirby/esp_warp");
            context.SpawnParticles(ParticleTypes.Dust, 12, new Vector2(24f, 24f));

            if (context.Player != null)
            {
                context.Player.Position += direction * 96f;
                context.DealAreaDamage(60f, 18);
            }
            return true;
        }
    }

    internal sealed class RangerAbility : KirbyAbilityBehavior
    {
        private float cooldown;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (cooldown > 0f)
                cooldown -= deltaTime;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (cooldown > 0f)
                return false;

            cooldown = 0.7f;
            Vector2 direction = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
            if (direction == Vector2.Zero)
                direction = Vector2.UnitX;
            direction.Normalize();

            context.PlaySound("event:/kirby/ranger_shot");
            context.DealLineDamage(direction, 240f, 24, 18f);
            context.Schedule(0.12f, delegate { context.DealLineDamage(direction, 240f, 12, 24f); });
            return true;
        }
    }

    internal sealed class MikeAbility : KirbyAbilityBehavior
    {
        private int charges = 3;
        private float rechargeTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (charges >= 3)
                return;

            rechargeTimer += deltaTime;
            if (rechargeTimer >= 8f)
            {
                rechargeTimer = 0f;
                charges++;
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (charges <= 0)
                return false;

            charges--;
            rechargeTimer = 0f;
            context.PlaySound("event:/kirby/mike_shout");
            context.SpawnParticles(ParticleTypes.Dust, 32, new Vector2(40f, 40f));
            context.DealAreaDamage(120f, 32);
            context.ShakeLevel(0.8f);
            return true;
        }
    }

    internal sealed class CrashAbility : KirbyAbilityBehavior
    {
        private bool used;

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (used)
                return false;

            used = true;
            context.PlaySound("event:/kirby/crash_blast");
            context.DealAreaDamage(160f, 48);
            context.ShakeLevel(1.0f);
            KirbyCopyAbilityManager.SwitchAbility(context.Player, KirbyCopyAbilityId.None);
            return true;
        }
    }

    internal sealed class BombAbility : KirbyAbilityBehavior
    {
        private float cooldown;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (cooldown > 0f)
                cooldown -= deltaTime;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (cooldown > 0f)
                return false;

            cooldown = 0.4f;
            Vector2 bombPosition = context.Player != null ? context.Player.Position : Vector2.Zero;

            context.PlaySound("event:/kirby/bomb_throw");
            context.Schedule(1.1f, delegate
            {
                KirbyAbilityContext delayedContext = new KirbyAbilityContext(context.Runtime);
                delayedContext.PlaySound("event:/kirby/bomb_explode");
                KirbyCopyAbilityManager.DealAreaDamageAt(delayedContext, bombPosition, 96f, 26);
                delayedContext.SpawnParticlesAt(bombPosition, ParticleTypes.Dust, 20, new Vector2(30f, 30f));
            });

            return true;
        }
    }

    internal sealed class CutterAbility : KirbyAbilityBehavior
    {
        private float cooldown;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (cooldown > 0f)
                cooldown -= deltaTime;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (cooldown > 0f)
                return false;

            cooldown = 0.65f;
            context.PlaySound("event:/kirby/cutter_throw");
            context.DealAreaDamage(62f, 16);
            context.Schedule(0.35f, delegate
            {
                KirbyAbilityContext returnContext = new KirbyAbilityContext(context.Runtime);
                returnContext.PlaySound("event:/kirby/cutter_return");
                returnContext.DealAreaDamage(62f, 14);
            });
            return true;
        }
    }

    internal sealed class PainterAbility : KirbyAbilityBehavior
    {
        private float paletteTimer;
        private float pulseTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (paletteTimer <= 0f)
                return;

            paletteTimer -= deltaTime;
            pulseTimer -= deltaTime;

            if (pulseTimer <= 0f)
            {
                pulseTimer = 1f;
                context.SpawnParticles(ParticleTypes.Dust, 10, new Vector2(24f, 24f));
                context.DealAreaDamage(52f, 14);
                context.Heal(1);
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            paletteTimer = 5f;
            pulseTimer = 0.2f;
            context.PlaySound("event:/kirby/painter_wave");
            return true;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (paletteTimer <= 0f)
                return attemptedDamage;

            return Math.Max(0, attemptedDamage - 3);
        }
    }

    internal sealed class CookAbility : KirbyAbilityBehavior
    {
        private float cooldown;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (cooldown > 0f)
                cooldown -= deltaTime;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (cooldown > 0f)
                return false;

            cooldown = 2.0f;
            int count = context.Player != null ? context.Player.ConsumeInhaledEntities() : 0;
            context.PlaySound("event:/kirby/cook_pot");
            context.DealAreaDamage(80f, 12 + count * 4);
            context.Heal(Math.Max(1, count));
            return true;
        }
    }

    internal sealed class BellAbility : KirbyAbilityBehavior
    {
        private float resonanceTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (resonanceTimer > 0f)
            {
                resonanceTimer -= deltaTime;
                context.SpawnParticles(ParticleTypes.Dust, 4, new Vector2(28f, 28f));
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            resonanceTimer = 3f;
            context.PlaySound("event:/kirby/bell_ring");
            context.DealAreaDamage(84f, 18);
            context.ShakeLevel(0.4f);
            return true;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (resonanceTimer <= 0f)
                return attemptedDamage;

            return Math.Max(0, attemptedDamage - 5);
        }
    }

    internal sealed class LightAbility : KirbyAbilityBehavior
    {
        private VertexLight light;
        private float auraTimer;

        public override void OnEnter(KirbyAbilityContext context)
        {
            if (context.Player != null && light == null)
            {
                light = new VertexLight(Color.White, 1f, 96, 160);
                context.Player.Add(light);
            }
        }

        public override void OnExit(KirbyAbilityContext context)
        {
            if (light != null)
            {
                light.RemoveSelf();
                light = null;
            }
        }

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (auraTimer > 0f)
                auraTimer -= deltaTime;

            if (light != null)
            {
                light.Alpha = auraTimer > 0f ? 1.2f : 0.8f;
                light.Visible = true;
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            auraTimer = 4f;
            context.PlaySound("event:/kirby/light_flash");
            context.DealAreaDamage(60f, 16);
            return true;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (auraTimer <= 0f)
                return attemptedDamage;
            return Math.Max(0, attemptedDamage - 4);
        }
    }

    internal sealed class DrillAbility : KirbyAbilityBehavior
    {
        private float drillTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (drillTimer <= 0f)
                return;

            drillTimer -= deltaTime;
            Vector2 speed = context.Player.Speed;
            speed.Y = Math.Min(speed.Y + 600f * deltaTime, 260f);
            context.Player.Speed = speed;
            context.SpawnParticles(ParticleTypes.Dust, 3, new Vector2(12f, 30f));
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            drillTimer = 0.9f;
            context.PlaySound("event:/kirby/drill_start");
            return true;
        }

        public override void OnPlayerLanded(KirbyAbilityContext context)
        {
            if (drillTimer <= 0f)
                return;

            drillTimer = 0f;
            context.PlaySound("event:/kirby/drill_land");
            context.DealAreaDamage(78f, 24);
        }
    }

    internal sealed class BeamAbility : KirbyAbilityBehavior
    {
        private float cooldown;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (cooldown > 0f)
                cooldown -= deltaTime;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (cooldown > 0f)
                return false;

            cooldown = 0.6f;
            Vector2 direction = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
            if (direction == Vector2.Zero)
                direction = Vector2.UnitX;
            direction.Normalize();

            context.PlaySound("event:/kirby/beam_whip");
            for (int i = 0; i < 3; i++)
            {
                float delay = 0.12f * i;
                int damage = 16 + i * 4;
                context.Schedule(delay, delegate
                {
                    KirbyAbilityContext burstContext = new KirbyAbilityContext(context.Runtime);
                    burstContext.DealLineDamage(direction, 200f, damage, 28f - i * 4f);
                });
            }

            return true;
        }
    }

    internal sealed class WheelAbility : KirbyAbilityBehavior
    {
        private float wheelTimer;
        private Vector2 wheelDirection = Vector2.UnitX;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (wheelTimer <= 0f)
                return;

            wheelTimer -= deltaTime;
            Vector2 speed = wheelDirection * 260f;
            speed.Y = Math.Min(context.Player.Speed.Y, 30f);
            context.Player.Speed = speed;
            context.SpawnParticles(ParticleTypes.Dust, 4, new Vector2(30f, 16f));
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            wheelTimer = 1.4f;
            Vector2 direction = new Vector2(Input.MoveX.Value, 0f);
            if (direction == Vector2.Zero)
                direction = Vector2.UnitX;
            direction.Normalize();
            wheelDirection = direction;
            context.PlaySound("event:/kirby/wheel_roll");
            return true;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (wheelTimer <= 0f)
                return attemptedDamage;

            return Math.Max(0, attemptedDamage - 4);
        }
    }

    internal sealed class PhaseAbility : KirbyAbilityBehavior
    {
        private float phaseTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (phaseTimer > 0f)
            {
                phaseTimer -= deltaTime;
                if (context.Player != null)
                    context.Player.Collidable = false;
            }
            else if (context.Player != null)
            {
                context.Player.Collidable = true;
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            phaseTimer = 2.2f;
            context.PlaySound("event:/kirby/phase_shift");
            return true;
        }

        public override void OnExit(KirbyAbilityContext context)
        {
            if (context.Player != null)
                context.Player.Collidable = true;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (phaseTimer > 0f)
            {
                context.DealAreaDamage(58f, attemptedDamage);
                return 0;
            }

            return attemptedDamage;
        }
    }

    internal sealed class TripleSwapAbility : KirbyAbilityBehavior
    {
        private int stance = -1;
        private float stanceTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (stanceTimer > 0f)
            {
                stanceTimer -= deltaTime;

                if (stance == 1 && context.Player != null)
                {
                    Vector2 speed = context.Player.Speed;
                    speed.X += 40f * deltaTime * Math.Sign(Input.MoveX.Value == 0 ? speed.X : Input.MoveX.Value);
                    context.Player.Speed = speed;
                }
            }
            else
            {
                stance = -1;
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            stance = (stance + 1) % 3;
            stanceTimer = 6f;
            context.PlaySound("event:/kirby/triple_swap");
            return true;
        }

        public override int ModifyDamageDealt(KirbyAbilityContext context, int attemptedDamage)
        {
            if (stance == 0 && stanceTimer > 0f)
                return attemptedDamage + 6;
            return attemptedDamage;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (stance == 2 && stanceTimer > 0f)
                return Math.Max(0, attemptedDamage - 6);
            return attemptedDamage;
        }
    }

    internal sealed class TimeCrashAbility : KirbyAbilityBehavior
    {
        private float timeTimer;
        private float originalTimeRate = 1f;
        private bool slowed;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (!slowed)
                return;

            timeTimer -= deltaTime;
            if (timeTimer <= 0f)
            {
                Engine.TimeRate = originalTimeRate;
                slowed = false;
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            originalTimeRate = Engine.TimeRate;
            Engine.TimeRate = 0.35f;
            timeTimer = 2f;
            slowed = true;
            context.PlaySound("event:/kirby/time_crash");
            context.DealAreaDamage(96f, 26);
            return true;
        }

        public override void OnExit(KirbyAbilityContext context)
        {
            if (slowed)
            {
                Engine.TimeRate = originalTimeRate;
                slowed = false;
            }
        }
    }

    internal sealed class UmbrellaAbility : KirbyAbilityBehavior
    {
        private float umbrellaTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (umbrellaTimer <= 0f)
                return;

            umbrellaTimer -= deltaTime;
            Vector2 speed = context.Player.Speed;
            if (speed.Y > -40f)
            {
                speed.Y = -40f;
                context.Player.Speed = speed;
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            umbrellaTimer = 4f;
            context.PlaySound("event:/kirby/umbrella_open");
            return true;
        }

        public override void OnPlayerLanded(KirbyAbilityContext context)
        {
            if (umbrellaTimer > 0f)
            {
                context.DealAreaDamage(48f, 12);
            }
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (umbrellaTimer <= 0f)
                return attemptedDamage;
            return Math.Max(0, attemptedDamage - 4);
        }
    }

    internal sealed class MirrorAbility : KirbyAbilityBehavior
    {
        private float mirrorTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (mirrorTimer > 0f)
                mirrorTimer -= deltaTime;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            mirrorTimer = 2.4f;
            context.PlaySound("event:/kirby/mirror_flash");
            return true;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (mirrorTimer > 0f)
            {
                context.DealAreaDamage(54f, attemptedDamage);
                return 0;
            }
            return attemptedDamage;
        }
    }

    internal sealed class RecyclerAbility : KirbyAbilityBehavior
    {
        private float cooldown;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (cooldown > 0f)
                cooldown -= deltaTime;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (cooldown > 0f)
                return false;

            cooldown = 1.2f;
            int count = context.Player != null ? context.Player.ConsumeInhaledEntities() : 0;
            context.PlaySound("event:/kirby/recycle_burst");
            context.DealAreaDamage(72f, 14 + count * 6);
            return true;
        }
    }

    internal sealed class MiniAbility : KirbyAbilityBehavior
    {
        private float dashCooldown;

        public override void OnEnter(KirbyAbilityContext context)
        {
            if (context.Player?.Sprite != null)
            {
                context.Player.Sprite.Scale = new Vector2(0.7f, 0.7f);
            }
        }

        public override void OnExit(KirbyAbilityContext context)
        {
            if (context.Player?.Sprite != null)
            {
                context.Player.Sprite.Scale = Vector2.One;
            }
        }

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (dashCooldown > 0f)
                dashCooldown -= deltaTime;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (dashCooldown > 0f)
                return false;

            dashCooldown = 0.6f;
            Vector2 direction = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
            if (direction == Vector2.Zero)
                direction = Vector2.UnitX;
            direction.Normalize();
            Vector2 speed = context.Player.Speed;
            speed += direction * 200f;
            context.Player.Speed = speed;
            context.PlaySound("event:/kirby/mini_dash");
            return true;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            return Math.Max(0, attemptedDamage - 3);
        }
    }

    internal sealed class MechanizeRangerSuperAbility : KirbyAbilityBehavior
    {
        private float cooldown;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (cooldown > 0f)
                cooldown -= deltaTime;
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (cooldown > 0f)
                return false;

            cooldown = 1.6f;
            context.PlaySound("event:/kirby/mecha_burst");
            Vector2 baseDir = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
            if (baseDir == Vector2.Zero)
                baseDir = Vector2.UnitX;
            baseDir.Normalize();

            for (int i = -1; i <= 1; i++)
            {
                float angle = 0.25f * i;
                Vector2 dir = new Vector2(
                    (float)(baseDir.X * Math.Cos(angle) - baseDir.Y * Math.Sin(angle)),
                    (float)(baseDir.X * Math.Sin(angle) + baseDir.Y * Math.Cos(angle)));
                context.Schedule(0.08f * (i + 1), delegate
                {
                    KirbyAbilityContext shotContext = new KirbyAbilityContext(context.Runtime);
                    shotContext.DealLineDamage(dir, 280f, 32, 26f);
                });
            }

            return true;
        }
    }

    internal sealed class FrostMindSuperAbility : KirbyAbilityBehavior
    {
        private float frostTimer;
        private float pulseTimer;

        public override void OnUpdate(KirbyAbilityContext context, float deltaTime)
        {
            if (frostTimer <= 0f)
                return;

            frostTimer -= deltaTime;
            pulseTimer -= deltaTime;

            if (pulseTimer <= 0f)
            {
                pulseTimer = 0.5f;
                context.DealAreaDamage(110f, 26);
                context.SpawnParticles(ParticleTypes.Dust, 16, new Vector2(40f, 40f));
            }
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            frostTimer = 2.6f;
            pulseTimer = 0.1f;
            context.PlaySound("event:/kirby/frostmind_burst");
            return true;
        }

        public override int ModifyDamageTaken(KirbyAbilityContext context, int attemptedDamage)
        {
            if (frostTimer > 0f)
                return Math.Max(0, attemptedDamage - 8);
            return attemptedDamage;
        }
    }

    internal sealed class KnightSuperAbility : KirbyAbilityBehavior
    {
        public override void OnEnter(KirbyAbilityContext context)
        {
            KirbyKnightSimple.TryTransformToKnight(context.Scene, true);
        }

        public override void OnExit(KirbyAbilityContext context)
        {
            KirbyKnightSimple.ResetKnightForm();
        }

        public override bool HandleSpecial(KirbyAbilityContext context)
        {
            if (context.Player == null)
                return false;

            KirbyKnightSimple.PerformKnightAttack(context.Player.Position, context.Scene);
            context.DealAreaDamage(88f, 34);
            context.PlaySound("event:/kirby/knight_super_slash");
            return true;
        }
    }

    #endregion
}




