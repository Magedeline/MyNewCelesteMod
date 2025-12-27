using DesoloZantas.Core.BossesHelper.Entities;

namespace DesoloZantas.Core.Core
{
    [Tracked]
    public class ElsFloweyBoss : BossActor
    {
        #region Boss States and Phases
        public enum BossPhase
        {
            Intro,
            OrganGarden,
            BoneCathedral, 
            FleshLabyrinth,
            WeaponArsenal,
            SoulCollection,
            FinalForm,
            Defeated,
            Redemption
        }

        public enum AttackType
        {
            OrganSlam,
            VineWhip,
            OrganProjectile,
            BoneSpear,
            SkullSwarm,
            FemurClub,
            MuscleTentacles,
            BloodGeyser,
            NerveShock,
            ChainFlail,
            BladeStorm,
            BoneMace,
            NightmareBeam,
            SoulCrush,
            RealityWarp,
            DespairAura
        }
        #endregion

        #region Boss Properties
        public BossPhase CurrentPhase { get; private set; }
        public bool IsVulnerable { get; private set; }
        public int SoulsCollected { get; private set; }

        private global::Celeste.Player player;
        private global::Celeste.Level level;
        private Camera camera;

        // Visual components
        // Sprite is inherited from BossEntity (readonly Sprite Sprite)
        private string currentTexture;
        private Vector2 basePosition;
        
        // Attack patterns
        private List<AttackType> currentAttackPattern;
        private int attackIndex;
        private float attackCooldown;
        
        // Organic elements
        private List<OrganicVine> vines;
        private List<BeatingHeart> hearts;
        private List<BoneSpike> boneSpikes;
        
        // Arena transformation
        private ArenaState currentArena;
        
        // Soul collection mechanics
        private Dictionary<string, Vector2> soulPositions;
        private Dictionary<string, bool> soulsCollected;
        private List<HumanSoul> activeSouls;
        
        // Audio
        private SoundSource organicSfx;
        private SoundSource boneSfx;
        private SoundSource fleshSfx;
        
        #endregion

        // BossActor requires these fields since it doesn't have Health/MaxHealth properties
        private float Health;
        private float MaxHealth;

        public ElsFloweyBoss(Vector2 position) 
            : base(position, 
                   spriteName: "characters/els_flowey_boss/flowey",
                   spriteScale: Vector2.One,
                   maxFall: 160f,
                   collidable: true,
                   solidCollidable: true,
                   gravityMult: 1.0f,
                   collider: new Hitbox(32, 48))
        {
            Initialize();
        }

        private void Initialize()
        {
            // Set up basic properties
            Health = MaxHealth = 1000f;
            CurrentPhase = BossPhase.Intro;
            IsVulnerable = false;
            SoulsCollected = 0;
            
            // Initialize position and configure sprite
            basePosition = Position;
            SetupSprite();
            
            // Initialize collections
            vines = new List<OrganicVine>();
            hearts = new List<BeatingHeart>();
            boneSpikes = new List<BoneSpike>();
            activeSouls = new List<HumanSoul>();
            
            // Setup soul positions for collection
            InitializeSoulPositions();
            
            // Collider already set in base constructor
            // Sprite already handled by BossEntity base class
            
            // Add components
            Add(new PlayerCollider(OnPlayerCollision));
            Add(new Coroutine(BossRoutine()));
        }

        private void SetupSprite()
        {
            // Sprite was created by BossEntity via sprite bank
            // If it's null or needs manual config, we set it up here
            if (Sprite != null)
            {
                // Configure sprite animations if they exist
                if (!Sprite.Has("idle"))
                {
                    Sprite.Add("idle", "idle", 0.1f);
                }
                if (!Sprite.Has("attacking"))
                {
                    Sprite.Add("attacking", "attack", 0.08f);
                }
                if (!Sprite.Has("transforming"))
                {
                    Sprite.Add("transforming", "transform", 0.12f);
                }
                if (!Sprite.Has("ultimate"))
                {
                    Sprite.Add("ultimate", "ultimate", 0.15f);
                }
                if (!Sprite.Has("vulnerable"))
                {
                    Sprite.Add("vulnerable", "vulnerable", 0.2f);
                }
                if (!Sprite.Has("dying"))
                {
                    Sprite.Add("dying", "dying", 0.3f);
                }
                
                Sprite.Play("idle");
                Sprite.CenterOrigin();
            }
            currentTexture = "idle";
        }

        private void InitializeSoulPositions()
        {
            soulPositions = new Dictionary<string, Vector2>
            {
                { "patience", new Vector2(100, 200) },     // Cyan - Organ Garden
                { "bravery", new Vector2(300, 150) },      // Orange - Bone Cathedral  
                { "integrity", new Vector2(500, 250) },    // Blue - Flesh Labyrinth
                { "perseverance", new Vector2(200, 400) }, // Purple - Weapon Chambers
                { "kindness", new Vector2(450, 350) },     // Green - Healing Springs
                { "justice", new Vector2(350, 100) }       // Yellow - Judgment Hall
            };
            
            soulsCollected = new Dictionary<string, bool>();
            foreach (string soul in soulPositions.Keys)
            {
                soulsCollected[soul] = false;
            }
        }

        #region Main Boss Routine
        private IEnumerator BossRoutine()
        {
            while (CurrentPhase != BossPhase.Defeated)
            {
                switch (CurrentPhase)
                {
                    case BossPhase.Intro:
                        yield return IntroPhase();
                        break;
                    case BossPhase.OrganGarden:
                        yield return OrganGardenPhase();
                        break;
                    case BossPhase.BoneCathedral:
                        yield return BoneCathedralPhase();
                        break;
                    case BossPhase.FleshLabyrinth:
                        yield return FleshLabyrinthPhase();
                        break;
                    case BossPhase.WeaponArsenal:
                        yield return WeaponArsenalPhase();
                        break;
                    case BossPhase.SoulCollection:
                        yield return SoulCollectionPhase();
                        break;
                    case BossPhase.FinalForm:
                        yield return FinalFormPhase();
                        break;
                    case BossPhase.Redemption:
                        yield return RedemptionPhase();
                        break;
                }
                
                yield return 0.1f;
            }
        }

        private IEnumerator IntroPhase()
        {
            // Trigger the corrupted reality intro dialog
            yield return Textbox.Say("CH16_CORRUPTED_REALITY_INTRO");
            
            // Wait for dialog to finish
            while (level.InCutscene)
                yield return null;
                
            // Start organic nightmare music
            Audio.SetMusic("event:/music/desolozantas/organic_nightmare");
            
            // Spawn initial organic elements
            SpawnOrganicElements();
            
            // Transition to first phase
            CurrentPhase = BossPhase.OrganGarden;
            yield return 2f;
        }

        private IEnumerator OrganGardenPhase()
        {
            TransformArena(ArenaState.OrganGarden);
            SetAttackPattern(new List<AttackType> 
            { 
                AttackType.OrganSlam, 
                AttackType.VineWhip, 
                AttackType.OrganProjectile 
            });
            
            yield return ExecuteAttackPhase(30f); // 30 seconds of attacks
            
            CurrentPhase = BossPhase.BoneCathedral;
        }

        private IEnumerator BoneCathedralPhase()
        {
            TransformArena(ArenaState.BoneCathedral);
            SetAttackPattern(new List<AttackType> 
            { 
                AttackType.BoneSpear, 
                AttackType.SkullSwarm, 
                AttackType.FemurClub 
            });
            
            yield return ExecuteAttackPhase(35f);
            
            CurrentPhase = BossPhase.FleshLabyrinth;
        }

        private IEnumerator FleshLabyrinthPhase()
        {
            TransformArena(ArenaState.FleshLabyrinth);
            SetAttackPattern(new List<AttackType> 
            { 
                AttackType.MuscleTentacles, 
                AttackType.BloodGeyser, 
                AttackType.NerveShock 
            });
            
            yield return ExecuteAttackPhase(40f);
            
            CurrentPhase = BossPhase.WeaponArsenal;
        }

        private IEnumerator WeaponArsenalPhase()
        {
            SetAttackPattern(new List<AttackType> 
            { 
                AttackType.ChainFlail, 
                AttackType.BladeStorm, 
                AttackType.BoneMace 
            });
            
            yield return ExecuteAttackPhase(45f);
            
            // Souls begin to appear
            SpawnHumanSouls();
            CurrentPhase = BossPhase.SoulCollection;
        }

        private IEnumerator SoulCollectionPhase()
        {
            // Become vulnerable as souls are collected
            IsVulnerable = true;
            Sprite.Play("vulnerable");
            
            while (SoulsCollected < 6)
            {
                // Check for soul collection
                CheckSoulCollection();
                
                // Become more desperate as souls are lost
                float desperation = (float)SoulsCollected / 6f;
                ModifyAttacksByDesperation(desperation);
                
                yield return 0.1f;
            }
            
            // All souls collected, transition to final form
            CurrentPhase = BossPhase.FinalForm;
        }

        private IEnumerator FinalFormPhase()
        {
            // Transform into ultimate form
            yield return TransformToFinalForm();
            
            // Trigger first phase battle dialog
            yield return Textbox.Say("CH16_FIRST_PHASE_BATTLE_0");
            
            SetAttackPattern(new List<AttackType> 
            { 
                AttackType.NightmareBeam, 
                AttackType.SoulCrush, 
                AttackType.RealityWarp, 
                AttackType.DespairAura 
            });

            // Final battle with empowered attacks
            int attackCount = 0;
            while (Health > 0)
            {
                yield return ExecuteUltimateAttack();
                attackCount++;
                
                // Trigger "call for help" sequence midway through battle
                if (attackCount == 3)
                {
                    yield return Textbox.Say("CH16_YOU_CALL_HELP");
                }
                
                // Check if player has defeated boss
                if (PlayerHasUltimateAttack())
                {
                    yield return PlayerFinalAttack();
                    break;
                }
                
                yield return 1f;
            }
            
            CurrentPhase = BossPhase.Redemption;
        }

        private IEnumerator RedemptionPhase()
        {
            // Vulnerable moment - show mercy or finish off
            Sprite.Play("dying");
            
            // Trigger final defeat dialog
            yield return Textbox.Say("CH16_FINAL_DEFEAT");
            
            // Wait for player choice (implemented via dialog system)
            yield return 3f;
            
            // Trigger barrier breaking sequence
            yield return Textbox.Say("CH16_BARRIER_BREAKS");
            
            // Trigger farewell titan king
            yield return Textbox.Say("CH16_FAREWELL_TITAN_KING");
            
            // Trigger return to celeste
            yield return Textbox.Say("CH16_RETURN_TO_CELESTE");
            
            // Assuming player shows mercy
            yield return RedemptionSequence();
            
            CurrentPhase = BossPhase.Defeated;
            RemoveSelf();
        }
        #endregion

        #region Attack Implementation
        private IEnumerator ExecuteAttackPhase(float duration)
        {
            float timer = 0;
            
            while (timer < duration && CurrentPhase == GetCurrentPhaseFromTimer(timer))
            {
                yield return ExecuteCurrentAttack();
                timer += Engine.DeltaTime;
                yield return attackCooldown;
            }
        }

        private IEnumerator ExecuteCurrentAttack()
        {
            if (currentAttackPattern == null || currentAttackPattern.Count == 0)
                yield break;
                
            AttackType attack = currentAttackPattern[attackIndex % currentAttackPattern.Count];
            attackIndex++;
            
            Sprite.Play("attacking");
            
            switch (attack)
            {
                case AttackType.OrganSlam:
                    yield return OrganSlamAttack();
                    break;
                case AttackType.VineWhip:
                    yield return VineWhipAttack();
                    break;
                case AttackType.OrganProjectile:
                    yield return OrganProjectileAttack();
                    break;
                case AttackType.BoneSpear:
                    yield return BoneSpearAttack();
                    break;
                case AttackType.SkullSwarm:
                    yield return SkullSwarmAttack();
                    break;
                case AttackType.FemurClub:
                    yield return FemurClubAttack();
                    break;
                case AttackType.MuscleTentacles:
                    yield return MuscleTentaclesAttack();
                    break;
                case AttackType.BloodGeyser:
                    yield return BloodGeyserAttack();
                    break;
                case AttackType.NerveShock:
                    yield return NerveShockAttack();
                    break;
                case AttackType.ChainFlail:
                    yield return ChainFlailAttack();
                    break;
                case AttackType.BladeStorm:
                    yield return BladeStormAttack();
                    break;
                case AttackType.BoneMace:
                    yield return BoneMaceAttack();
                    break;
                case AttackType.NightmareBeam:
                    yield return NightmareBeamAttack();
                    break;
                case AttackType.SoulCrush:
                    yield return SoulCrushAttack();
                    break;
                case AttackType.RealityWarp:
                    yield return RealityWarpAttack();
                    break;
                case AttackType.DespairAura:
                    yield return DespairAuraAttack();
                    break;
            }
            
            Sprite.Play("idle");
        }

        #region Specific Attack Methods
        private IEnumerator OrganSlamAttack()
        {
            // Create ground pound with gore effects
            Vector2 targetPos = player?.Position ?? Position;
            
            // Telegraph attack
            CreateTelegraphEffect(targetPos, Color.Red, 1.5f);
            yield return 1.5f;
            
            // Execute slam
            Audio.Play("event:/char/desolozantas/organic_slam", Position);
            level.DirectionalShake(Vector2.UnitY, 0.5f);
            
            // Spawn gore particles
            for (int i = 0; i < 20; i++)
            {
                level.ParticlesFG.Emit(ParticleTypes.Dust, 
                    targetPos + Calc.Random.Range(Vector2.One * -32, Vector2.One * 32));
            }
            
            // Create damage zone
            Scene.Add(new OrganicSlamZone(targetPos));
            
            yield return 0.5f;
        }

        private IEnumerator VineWhipAttack()
        {
            // Create thorny vine whips
            Vector2 playerPos = player?.Position ?? Position;
            Vector2 direction = (playerPos - Position).SafeNormalize();
            
            Audio.Play("event:/char/desolozantas/vine_whip", Position);
            
            for (int i = 0; i < 3; i++)
            {
                Scene.Add(new BloodyVine(Position, direction + Vector2.One.Rotate(i * 0.5f), 200f));
                yield return 0.2f;
            }
            
            yield return 1f;
        }

        private IEnumerator OrganProjectileAttack()
        {
            // Launch various organs as projectiles
            string[] organTypes = { "heart", "lung", "kidney", "liver", "brain" };
            
            for (int i = 0; i < 5; i++)
            {
                Vector2 targetPos = player?.Position ?? Position;
                Vector2 velocity = (targetPos - Position).SafeNormalize() * 150f;
                
                Scene.Add(new OrganProjectile(Position, velocity, organTypes[i % organTypes.Length]));
                
                Audio.Play("event:/char/desolozantas/organ_launch", Position);
                yield return 0.3f;
            }
        }

        private IEnumerator BoneSpearAttack()
        {
            // Rapid fire bone spears from the ground
            Vector2 playerPos = player?.Position ?? Position;
            
            for (int i = 0; i < 8; i++)
            {
                Vector2 spearPos = playerPos + Calc.Random.Range(Vector2.One * -64, Vector2.One * 64);
                Scene.Add(new BoneSpear(spearPos));
                
                Audio.Play("event:/char/desolozantas/bone_spike", spearPos);
                yield return 0.15f;
            }
        }

        private IEnumerator SkullSwarmAttack()
        {
            // Homing skull projectiles
            for (int i = 0; i < 6; i++)
            {
                Vector2 spawnPos = Position + Vector2.One.Rotate(i * MathHelper.Pi / 3) * 80f;
                if (player != null)
                {
                    Scene.Add(new HomingSkull(spawnPos, player));
                }
                
                Audio.Play("event:/char/desolozantas/skull_spawn", spawnPos);
                yield return 0.2f;
            }
        }

        private IEnumerator FemurClubAttack()
        {
            // Massive bone club swing
            Audio.Play("event:/char/desolozantas/bone_club", Position);
            
            // Create wide arc attack
            for (float angle = -MathHelper.Pi / 3; angle <= MathHelper.Pi / 3; angle += 0.2f)
            {
                Vector2 direction = Vector2.One.Rotate(angle);
                Scene.Add(new BoneClubSwing(Position, direction, 150f));
            }
            
            level.DirectionalShake(Vector2.UnitX, 0.8f);
            yield return 2f;
        }

        private IEnumerator MuscleTentaclesAttack()
        {
            // Grasping flesh tentacles
            Vector2 playerPos = player?.Position ?? Position;
            
            for (int i = 0; i < 4; i++)
            {
                Vector2 tentaclePos = playerPos + Calc.Random.Range(Vector2.One * -100, Vector2.One * 100);
                Scene.Add(new FleshTentacle(tentaclePos, playerPos));
                
                Audio.Play("event:/char/desolozantas/flesh_grab", tentaclePos);
                yield return 0.4f;
            }
        }

        private IEnumerator BloodGeyserAttack()
        {
            // Erupting blood geysers
            Vector2 playerPos = player?.Position ?? Position;
            
            // Create warning indicators
            for (int i = 0; i < 5; i++)
            {
                Vector2 geyserPos = playerPos + Calc.Random.Range(Vector2.One * -80, Vector2.One * 80);
                CreateTelegraphEffect(geyserPos, Color.DarkRed, 2f);
            }
            
            yield return 2f;
            
            // Erupt geysers
            for (int i = 0; i < 5; i++)
            {
                Vector2 geyserPos = playerPos + Calc.Random.Range(Vector2.One * -80, Vector2.One * 80);
                Scene.Add(new BloodGeyser(geyserPos));
                
                Audio.Play("event:/char/desolozantas/blood_geyser", geyserPos);
                yield return 0.3f;
            }
        }

        private IEnumerator NerveShockAttack()
        {
            // Electric nerve network
            Audio.Play("event:/char/desolozantas/nerve_shock", Position);
            
            // Create electric field
            Scene.Add(new NerveElectricField(Position, 200f));
            
            // Screen flash
            level.Flash(Color.Yellow, true);
            level.DirectionalShake(Vector2.One, 0.3f);
            
            yield return 1.5f;
        }

        private IEnumerator ChainFlailAttack()
        {
            // Swinging rusty chains
            Audio.Play("event:/char/desolozantas/chain_swing", Position);

            if (player != null)
            {
                Scene.Add(new RustyChainFlail(Position, player, 250f));
            }
            yield return 3f;
        }

        private IEnumerator BladeStormAttack()
        {
            // Spinning serrated blades
            for (int i = 0; i < 10; i++)
            {
                float angle = (i / 10f) * MathHelper.TwoPi;
                Vector2 direction = Vector2.One.Rotate(angle);
                Scene.Add(new SerratedBlade(Position, direction * 120f, 3f));
                
                Audio.Play("event:/char/desolozantas/blade_spin", Position);
                yield return 0.1f;
            }
            
            yield return 2f;
        }

        private IEnumerator BoneMaceAttack()
        {
            // Devastating bone mace slam
            Vector2 targetPos = player?.Position ?? Position;
            
            CreateTelegraphEffect(targetPos, Color.White, 2f);
            yield return 2f;
            
            Audio.Play("event:/char/desolozantas/bone_mace", Position);
            Scene.Add(new GiantBoneMace(Position, targetPos));
            
            level.DirectionalShake(Vector2.One, 1f);
            yield return 1f;
        }

        private IEnumerator NightmareBeamAttack()
        {
            // Ultimate devastating beam
            Audio.Play("event:/char/desolozantas/nightmare_beam", Position);
            Sprite.Play("ultimate");
            
            yield return 1f; // Charge up
            
            Vector2 targetPos = player?.Position ?? Position;
            Scene.Add(new NightmareBeam(Position, targetPos, 5f));
            
            yield return 3f;
        }

        private IEnumerator SoulCrushAttack()
        {
            // Attempt to crush player's soul
            if (player != null)
            {
                Audio.Play("event:/char/desolozantas/soul_crush", Position);
                Scene.Add(new SoulCrushEffect(player.Position));
            }
            
            yield return 2f;
        }

        private IEnumerator RealityWarpAttack()
        {
            // Distort the arena
            Audio.Play("event:/char/desolozantas/reality_warp", Position);
            
            level.Add(new RealityDistortionEffect());
            yield return 4f;
        }

        private IEnumerator DespairAuraAttack()
        {
            // Spreading aura of despair
            Audio.Play("event:/char/desolozantas/despair_aura", Position);
            
            Scene.Add(new DespairAura(Position, 300f));
            yield return 3f;
        }
        #endregion
        #endregion

        #region Arena and Visual Management
        public enum ArenaState
        {
            Normal,
            OrganGarden,
            BoneCathedral,
            FleshLabyrinth,
            WeaponArsenal,
            FinalChamber
        }

        private void TransformArena(ArenaState newState)
        {
            currentArena = newState;
            
            // Remove old arena elements
            ClearArenaElements();
            
            switch (newState)
            {
                case ArenaState.OrganGarden:
                    SetupOrganGarden();
                    break;
                case ArenaState.BoneCathedral:
                    SetupBoneCathedral();
                    break;
                case ArenaState.FleshLabyrinth:
                    SetupFleshLabyrinth();
                    break;
                case ArenaState.WeaponArsenal:
                    SetupWeaponArsenal();
                    break;
                case ArenaState.FinalChamber:
                    SetupFinalChamber();
                    break;
            }
        }

        private void SetupOrganGarden()
        {
            // Spawn hanging organs, beating hearts, pulsing vines
            for (int i = 0; i < 15; i++)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(50, level.Bounds.Width - 50),
                    Calc.Random.Range(50, level.Bounds.Height - 100)
                );
                
                Scene.Add(new HangingOrgan(pos, Calc.Random.Choose("heart", "lung", "kidney")));
            }
            
            // Add pulsing background elements
            level.Add(new OrganicBackground());
        }

        private void SetupBoneCathedral()
        {
            // Spawn bone pillars, skull decorations
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = new Vector2(
                    (i + 1) * (level.Bounds.Width / 9),
                    level.Bounds.Height - 150
                );
                
                Scene.Add(new BonePillar(pos));
            }
            
            level.Add(new BoneCathedralBackground());
        }

        private void SetupFleshLabyrinth()
        {
            // Create maze-like flesh walls
            for (int x = 0; x < level.Bounds.Width; x += 100)
            {
                for (int y = 0; y < level.Bounds.Height; y += 100)
                {
                    if (Calc.Random.Chance(0.3f))
                    {
                        Scene.Add(new FleshWall(new Vector2(x, y)));
                    }
                }
            }
            
            level.Add(new FleshLabyrinthBackground());
        }

        private void SetupWeaponArsenal()
        {
            // Display various torture weapons
            string[] weapons = { "chains", "blades", "maces", "spears" };
            
            foreach (string weapon in weapons)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(100, level.Bounds.Width - 100),
                    Calc.Random.Range(100, level.Bounds.Height - 100)
                );
                
                Scene.Add(new WeaponDisplay(pos, weapon));
            }
        }

        private void SetupFinalChamber()
        {
            // Ultimate nightmare arena
            level.Add(new NightmareBackground());
            Scene.Add(new UltimateArenaEffects());
        }

        private void ClearArenaElements()
        {
            // Remove all arena-specific entities
            foreach (Entity entity in Scene.Entities.OfType<ArenaElement>())
            {
                entity.RemoveSelf();
            }
        }
        #endregion

        #region Soul Collection System
        private void SpawnHumanSouls()
        {
            foreach (var soulData in soulPositions)
            {
                if (!soulsCollected[soulData.Key])
                {
                    Scene.Add(new HumanSoul(soulData.Value, soulData.Key));
                }
            }
        }

        private void CheckSoulCollection()
        {
            // Check if player is near any uncollected souls
            if (player == null) return;
            
            foreach (var soulData in soulPositions)
            {
                if (!soulsCollected[soulData.Key])
                {
                    float distance = Vector2.Distance(player.Position, soulData.Value);
                    if (distance < 32f)
                    {
                        CollectSoul(soulData.Key);
                    }
                }
            }
        }

        private void CollectSoul(string soulType)
        {
            soulsCollected[soulType] = true;
            SoulsCollected++;
            
            // Trigger dialog event
            TriggerSoulCollectionDialog(soulType);
            
            // Visual effects
            Audio.Play("event:/char/desolozantas/soul_collected", soulPositions[soulType]);
            level.Flash(GetSoulColor(soulType), true);
            
            // Make boss more desperate
            ModifyBossFromSoulLoss(soulType);
        }

        private Color GetSoulColor(string soulType)
        {
            return soulType switch
            {
                "patience" => Color.Cyan,
                "bravery" => Color.Orange,
                "integrity" => Color.Blue,
                "perseverance" => Color.Purple,
                "kindness" => Color.Green,
                "justice" => Color.Yellow,
                _ => Color.White
            };
        }

        private void TriggerSoulCollectionDialog(string soulType)
        {
            string dialogKey = $"CH16_LOST_SOULS_{SoulsCollected}";
            
            // Trigger the appropriate lost soul dialog
            Add(new Coroutine(SoulDialogSequence(dialogKey)));
            
            // Trigger custom dialog event through existing system
            level.Session.SetFlag($"soul_{soulType}_collected", true);
        }

        private IEnumerator SoulDialogSequence(string dialogKey)
        {
            yield return Textbox.Say(dialogKey);
        }

        private void ModifyBossFromSoulLoss(string soulType)
        {
            // Increase attack frequency and desperation
            attackCooldown = Math.Max(0.5f, attackCooldown - 0.2f);
            
            // Change behavior based on souls lost
            float desperation = (float)SoulsCollected / 6f;
            
            if (desperation >= 0.5f)
            {
                // Start using more dangerous attacks
                AddDesperationAttacks();
            }
            
            if (desperation >= 0.8f)
            {
                // Final desperate phase
                EnterDesperationMode();
            }
        }

        private void AddDesperationAttacks()
        {
            // Add more powerful attacks to the pattern
            if (currentAttackPattern != null)
            {
                currentAttackPattern.Add(AttackType.NightmareBeam);
                currentAttackPattern.Add(AttackType.RealityWarp);
            }
        }

        private void EnterDesperationMode()
        {
            // Boss becomes erratic and extremely dangerous
            Sprite.Play("vulnerable");
            attackCooldown = 0.3f;
            
            // Add screen distortion effects
            level.Add(new DesperationEffects());
        }
        #endregion

        #region Utility Methods
        private void SetAttackPattern(List<AttackType> pattern)
        {
            currentAttackPattern = new List<AttackType>(pattern);
            attackIndex = 0;
            attackCooldown = 2f;
        }

        private void SpawnOrganicElements()
        {
            // Spawn initial vines and hearts for the intro
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = basePosition + Calc.Random.Range(Vector2.One * -100, Vector2.One * 100);
                Scene.Add(new OrganicVine(pos));
            }
        }

        private void CreateTelegraphEffect(Vector2 position, Color color, float duration)
        {
            Scene.Add(new TelegraphWarning(position, color, duration));
        }

        private void ModifyAttacksByDesperation(float desperation)
        {
            // Modify attack patterns based on how many souls have been collected
            attackCooldown = Calc.LerpClamp(2f, 0.8f, desperation);
            
            // Add visual distress
            if (desperation > 0.3f)
            {
                Sprite.Color = Color.Lerp(Color.White, Color.Red, desperation);
            }
        }

        private IEnumerator TransformToFinalForm()
        {
            Sprite.Play("transforming");
            Audio.Play("event:/char/desolozantas/final_transform", Position);
            
            // Screen effects
            level.Flash(Color.Red, true);
            level.DirectionalShake(Vector2.One, 2f);
            
            yield return 3f;
            
            Sprite.Play("ultimate");
            TransformArena(ArenaState.FinalChamber);
        }

        private bool PlayerHasUltimateAttack()
        {
            // Check if all 6 souls have been collected
            return SoulsCollected >= 6;
        }

        private IEnumerator ExecuteUltimateAttack()
        {
            // Randomly choose from ultimate attacks
            AttackType[] ultimateAttacks = 
            {
                AttackType.NightmareBeam,
                AttackType.SoulCrush,
                AttackType.RealityWarp,
                AttackType.DespairAura
            };
            
            AttackType selectedAttack = Calc.Random.Choose(ultimateAttacks);
            
            switch (selectedAttack)
            {
                case AttackType.NightmareBeam:
                    yield return NightmareBeamAttack();
                    break;
                case AttackType.SoulCrush:
                    yield return SoulCrushAttack();
                    break;
                case AttackType.RealityWarp:
                    yield return RealityWarpAttack();
                    break;
                case AttackType.DespairAura:
                    yield return DespairAuraAttack();
                    break;
            }
        }

        private IEnumerator PlayerFinalAttack()
        {
            // Trigger finale dialog
            yield return Textbox.Say("CH16_MY_FINALE");
            
            // Show defense drop message
            yield return Textbox.Say("CH16_MY_FINALE_FLOWEY_DEFENSE_DROP_TO_ZERO");
            
            // Trigger final blow dialog
            yield return Textbox.Say("CH16_FINAL_BLOW");
            
            // The player's rainbow soul blast attack
            Audio.Play("event:/char/desolozantas/player_soul_blast", player.Position);
            
            // Create spectacular visual effect
            level.Flash(Color.White, true);
            
            // Damage the boss
            Health = 0;
            
            // Trigger take the L dialog
            yield return Textbox.Say("CH16_TAKE_THE_L");
            
            yield return 2f;
        }

        private IEnumerator RedemptionSequence()
        {
            // Peaceful resolution
            Sprite.Play("dying");
            Audio.SetMusic("event:/music/desolozantas/redemption");
            
            // Fade out organic elements
            foreach (var entity in Scene.Entities.OfType<ArenaElement>())
            {
                entity.Add(new Coroutine(FadeAndRemove(entity)));
            }
            
            yield return 5f;
        }

        private IEnumerator FadeAndRemove(Entity entity)
        {
            for (float t = 0; t < 1f; t += Engine.DeltaTime)
            {
                if (entity.Get<Sprite>() is Sprite sprite)
                {
                    Sprite.Color = Color.White * (1f - t);
                }
                yield return null;
            }
            
            entity.RemoveSelf();
        }

        private BossPhase GetCurrentPhaseFromTimer(float timer)
        {
            // This is a simplified version - in practice you'd want more sophisticated phase management
            return CurrentPhase;
        }

        private void OnPlayerCollision(Entity playerEntity)
        {
            // Handle collision with player - temporarily disabled due to type conflict
            var p = playerEntity as global::Celeste.Player;
            if (p != null && !IsVulnerable)
            {
                p.Die(Vector2.Zero, false, true);
            }
        }
        #endregion

        public override void Update()
        {
            base.Update();
            
            // Update references
            player = Scene?.Tracker?.GetEntity<global::Celeste.Player>();
            level = SceneAs<Level>();
            camera = level?.Camera;
        }

        public override void Render()
        {
            base.Render();
            
            // Render health bar if boss is active
            if (CurrentPhase != BossPhase.Intro && CurrentPhase != BossPhase.Defeated)
            {
                RenderHealthBar();
            }
        }

        private void RenderHealthBar()
        {
            Vector2 topLeft = camera.Position + new Vector2(20, 20);
            float barWidth = 300f;
            float barHeight = 20f;
            float healthPercent = Health / MaxHealth;
            
            // Background
            Draw.Rect(topLeft, barWidth, barHeight, Color.Black);
            
            // Health bar
            Draw.Rect(topLeft, barWidth * healthPercent, barHeight, Color.Red);
            
            // Border
            Draw.HollowRect(topLeft, barWidth, barHeight, Color.White);
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            
            // Cleanup
            organicSfx?.Stop();
            boneSfx?.Stop();
            fleshSfx?.Stop();
        }
    }
}




