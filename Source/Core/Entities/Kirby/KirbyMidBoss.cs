namespace DesoloZantas.Core.Core.Entities.Kirby
{
    /// <summary>
    /// Mid-tier bosses in Kirby style - stronger than regular enemies, have multiple attack phases
    /// Examples: Whispy Woods, Kracko, Mr. Frosty, Bonkers, Bugzzy, etc.
    /// </summary>
    [CustomEntity("Ingeste/KirbyMidBoss")]
    [Tracked]
    public class KirbyMidBoss : KirbyActorBase
    {
        /// <summary>
        /// Mid boss types with unique attack patterns
        /// </summary>
        public enum MidBossType
        {
            WhispyWoods,    // Tree boss - drops apples, blows air
            Kracko,         // Cloud boss - shoots lightning, drops Waddle Doos
            MrFrosty,       // Ice boss - throws ice blocks, slides
            Bonkers,        // Hammer boss - throws coconuts, hammer attacks
            Bugzzy,         // Bug boss - suplex attacks
            FireLion,       // Fire boss - charges, breathes fire
            IronMam,        // Metal boss - body slam, rolling attack
            GrandWheely,    // Wheel boss - fast charges
            BoxBoxer,       // Boxing boss - punch combos
            MasterHand,     // Hand boss - various hand attacks
            Custom
        }

        /// <summary>
        /// Boss phase for multi-stage fights
        /// </summary>
        public enum BossPhase
        {
            Intro,
            Phase1,
            Phase2,
            Enraged,
            Defeated
        }

        public MidBossType BossType { get; private set; }
        public BossPhase CurrentPhase { get; private set; }
        
        // Boss-specific properties
        private float phaseTransitionHealth;
        private float enrageHealthPercent = 0.25f;
        private bool hasPlayedIntro;
        private float attackCooldown;
        private float attackTimer;
        private int attackPattern;
        private float invulnerableTimer;
        
        // Visual components
        private Sprite bossSprite;
        private ParticleEmitter particleEmitter;
        private float flashTimer;
        
        // State machine
        private StateMachine stateMachine;
        
        // State indices
        private const int ST_INTRO = 0;
        private const int ST_IDLE = 1;
        private const int ST_ATTACK = 2;
        private const int ST_TRANSITION = 3;
        private const int ST_STUNNED = 4;
        private const int ST_DEFEATED = 5;

        // Arena boundaries
        private Rectangle arenaBounds;

        public KirbyMidBoss(Vector2 position, MidBossType bossType) : base(position)
        {
            BossType = bossType;
            IsFriendly = false;
            
            SetupBossType();
        }

        public KirbyMidBoss(EntityData data, Vector2 offset) 
            : base(data, offset)
        {
            BossType = (MidBossType)data.Int("bossType", 0);
            
            // Arena bounds
            float arenaWidth = data.Float("arenaWidth", 320f);
            float arenaHeight = data.Float("arenaHeight", 180f);
            arenaBounds = new Rectangle(
                (int)(Position.X - arenaWidth / 2),
                (int)(Position.Y - arenaHeight),
                (int)arenaWidth,
                (int)arenaHeight
            );
            
            IsFriendly = false;
            SetupBossType();
        }

        private void SetupBossType()
        {
            // Set properties based on boss type
            switch (BossType)
            {
                case MidBossType.WhispyWoods:
                    MaxHealth = 100;
                    MoveSpeed = 0f; // Stationary
                    attackCooldown = 2f;
                    CurrentPower = PowerType.None;
                    CanBeInhaled = false;
                    Collider = new Hitbox(64f, 96f, -32f, -96f);
                    detectionRange = 200f;
                    break;

                case MidBossType.Kracko:
                    MaxHealth = 80;
                    MoveSpeed = 60f;
                    attackCooldown = 1.5f;
                    CurrentPower = PowerType.Spark;
                    CanBeInhaled = false;
                    Collider = new Hitbox(48f, 32f, -24f, -16f);
                    detectionRange = 250f;
                    break;

                case MidBossType.MrFrosty:
                    MaxHealth = 60;
                    MoveSpeed = 50f;
                    attackCooldown = 2f;
                    CurrentPower = PowerType.Ice;
                    CanBeInhaled = true;
                    inhaleResistance = 5f; // Very hard to inhale
                    Collider = new Hitbox(32f, 32f, -16f, -32f);
                    detectionRange = 150f;
                    break;

                case MidBossType.Bonkers:
                    MaxHealth = 80;
                    MoveSpeed = 40f;
                    attackCooldown = 2.5f;
                    CurrentPower = PowerType.None; // Hammer power
                    CanBeInhaled = true;
                    inhaleResistance = 5f;
                    Collider = new Hitbox(32f, 40f, -16f, -40f);
                    detectionRange = 120f;
                    break;

                case MidBossType.Bugzzy:
                    MaxHealth = 70;
                    MoveSpeed = 55f;
                    attackCooldown = 1.8f;
                    CurrentPower = PowerType.None; // Suplex power
                    CanBeInhaled = true;
                    inhaleResistance = 4f;
                    Collider = new Hitbox(32f, 24f, -16f, -24f);
                    detectionRange = 100f;
                    break;

                case MidBossType.FireLion:
                    MaxHealth = 75;
                    MoveSpeed = 70f;
                    attackCooldown = 1.5f;
                    CurrentPower = PowerType.Fire;
                    CanBeInhaled = true;
                    inhaleResistance = 4f;
                    Collider = new Hitbox(40f, 24f, -20f, -24f);
                    detectionRange = 140f;
                    break;

                case MidBossType.IronMam:
                    MaxHealth = 100;
                    MoveSpeed = 30f;
                    attackCooldown = 3f;
                    CurrentPower = PowerType.Stone;
                    CanBeInhaled = false;
                    Collider = new Hitbox(48f, 48f, -24f, -48f);
                    detectionRange = 100f;
                    break;

                case MidBossType.GrandWheely:
                    MaxHealth = 65;
                    MoveSpeed = 120f;
                    attackCooldown = 1f;
                    CurrentPower = PowerType.Wheel;
                    CanBeInhaled = true;
                    inhaleResistance = 4f;
                    Collider = new Hitbox(32f, 32f, -16f, -32f);
                    detectionRange = 180f;
                    break;

                case MidBossType.BoxBoxer:
                    MaxHealth = 70;
                    MoveSpeed = 50f;
                    attackCooldown = 1.2f;
                    CurrentPower = PowerType.None; // Fighter power
                    CanBeInhaled = true;
                    inhaleResistance = 4f;
                    Collider = new Hitbox(24f, 32f, -12f, -32f);
                    detectionRange = 80f;
                    break;

                case MidBossType.MasterHand:
                    MaxHealth = 120;
                    MoveSpeed = 80f;
                    attackCooldown = 2f;
                    CurrentPower = PowerType.None;
                    CanBeInhaled = false;
                    Collider = new Hitbox(48f, 32f, -24f, -16f);
                    detectionRange = 300f;
                    break;

                default:
                    MaxHealth = 50;
                    MoveSpeed = 40f;
                    attackCooldown = 2f;
                    CurrentPower = PowerType.None;
                    CanBeInhaled = true;
                    inhaleResistance = 3f;
                    break;
            }

            Health = MaxHealth;
            phaseTransitionHealth = MaxHealth / 2;
            CurrentPhase = BossPhase.Intro;
            attackTimer = attackCooldown;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            // Setup state machine
            Add(stateMachine = new StateMachine());
            
            stateMachine.SetCallbacks(ST_INTRO, IntroUpdate, IntroCoroutine, IntroBegin, null);
            stateMachine.SetCallbacks(ST_IDLE, IdleUpdate, null, IdleBegin, null);
            stateMachine.SetCallbacks(ST_ATTACK, AttackUpdate, null, AttackBegin, AttackEnd);
            stateMachine.SetCallbacks(ST_TRANSITION, TransitionUpdate, TransitionCoroutine, TransitionBegin, null);
            stateMachine.SetCallbacks(ST_STUNNED, StunnedUpdate, null, StunnedBegin, null);
            stateMachine.SetCallbacks(ST_DEFEATED, DefeatedUpdate, DefeatedCoroutine, DefeatedBegin, null);
            
            stateMachine.State = ST_INTRO;
            
            Depth = Depths.Enemy - 1; // Render above regular enemies
        }

        protected override void SetupSprite()
        {
            string spritePath = GetSpritePathForBoss();
            
            Add(sprite = new Sprite(GFX.Game, spritePath));
            sprite.AddLoop("idle", "", 0.15f);
            sprite.AddLoop("attack", "", 0.1f);
            sprite.AddLoop("hurt", "", 0.1f);
            sprite.AddLoop("defeated", "", 0.2f);
            sprite.CenterOrigin();
            sprite.Play("idle");
            
            // Boss glow
            light.Color = GetBossColor();
            light.StartRadius = 32;
            light.EndRadius = 64;
            bloom.Alpha = 0.5f;
            bloom.Radius = 16f;
        }

        private string GetSpritePathForBoss()
        {
            return BossType switch
            {
                MidBossType.WhispyWoods => "bosses/kirby/whispy_woods/",
                MidBossType.Kracko => "bosses/kirby/kracko/",
                MidBossType.MrFrosty => "bosses/kirby/mr_frosty/",
                MidBossType.Bonkers => "bosses/kirby/bonkers/",
                MidBossType.Bugzzy => "bosses/kirby/bugzzy/",
                MidBossType.FireLion => "bosses/kirby/fire_lion/",
                MidBossType.IronMam => "bosses/kirby/iron_mam/",
                MidBossType.GrandWheely => "bosses/kirby/grand_wheely/",
                MidBossType.BoxBoxer => "bosses/kirby/box_boxer/",
                MidBossType.MasterHand => "bosses/kirby/master_hand/",
                _ => "bosses/kirby/generic/"
            };
        }

        private Color GetBossColor()
        {
            return BossType switch
            {
                MidBossType.WhispyWoods => Color.Green,
                MidBossType.Kracko => Color.Purple,
                MidBossType.MrFrosty => Color.LightBlue,
                MidBossType.Bonkers => Color.Orange,
                MidBossType.Bugzzy => Color.YellowGreen,
                MidBossType.FireLion => Color.OrangeRed,
                MidBossType.IronMam => Color.Gray,
                MidBossType.GrandWheely => Color.Red,
                MidBossType.BoxBoxer => Color.Yellow,
                MidBossType.MasterHand => Color.White,
                _ => Color.White
            };
        }

        public override void Update()
        {
            base.Update();
            
            // Update attack cooldown
            if (attackTimer > 0)
            {
                attackTimer -= Engine.DeltaTime;
            }
            
            // Update invulnerability
            if (invulnerableTimer > 0)
            {
                invulnerableTimer -= Engine.DeltaTime;
                
                // Flash while invulnerable
                flashTimer += Engine.DeltaTime;
                sprite.Color = (int)(flashTimer * 10) % 2 == 0 ? Color.White : Color.Red;
            }
            else
            {
                sprite.Color = Color.White;
            }
            
            // Check for phase transitions
            CheckPhaseTransition();
        }

        private void CheckPhaseTransition()
        {
            if (CurrentPhase == BossPhase.Defeated) return;
            
            float healthPercent = (float)Health / MaxHealth;
            
            if (healthPercent <= 0)
            {
                CurrentPhase = BossPhase.Defeated;
                stateMachine.State = ST_DEFEATED;
            }
            else if (healthPercent <= enrageHealthPercent && CurrentPhase != BossPhase.Enraged)
            {
                CurrentPhase = BossPhase.Enraged;
                stateMachine.State = ST_TRANSITION;
            }
            else if (Health <= phaseTransitionHealth && CurrentPhase == BossPhase.Phase1)
            {
                CurrentPhase = BossPhase.Phase2;
                stateMachine.State = ST_TRANSITION;
            }
        }

        public override void OnHit(int damage, Vector2 knockback)
        {
            if (invulnerableTimer > 0 || IsDefeated)
                return;
            
            base.OnHit(damage, knockback);
            
            // Brief invulnerability after hit
            invulnerableTimer = 0.5f;
            
            // Interrupt current attack on hit
            if (stateMachine.State == ST_ATTACK)
            {
                stateMachine.State = ST_STUNNED;
            }
            
            // Screen shake
            var level = Scene as Level;
            level?.DirectionalShake(knockback.SafeNormalize(), 0.1f);
        }

        #region State Machine Callbacks

        // INTRO STATE
        private void IntroBegin()
        {
            sprite?.Play("idle");
            Collidable = false;
        }

        private int IntroUpdate()
        {
            return ST_INTRO; // Coroutine handles transition
        }

        private IEnumerator IntroCoroutine()
        {
            yield return 0.5f;
            
            // Boss entrance effects
            var level = Scene as Level;
            
            Audio.Play("event:/game/general/thing_booped", Position);
            level?.Displacement.AddBurst(Position, 0.5f, 32f, 128f, 0.5f);
            
            // Flash screen
            level?.Flash(GetBossColor() * 0.3f, true);
            
            yield return 1f;
            
            hasPlayedIntro = true;
            Collidable = true;
            CurrentPhase = BossPhase.Phase1;
            stateMachine.State = ST_IDLE;
        }

        // IDLE STATE
        private void IdleBegin()
        {
            sprite?.Play("idle");
            stateTimer = Calc.Random.Range(0.5f, 1.5f);
        }

        private int IdleUpdate()
        {
            if (targetPlayer == null) return ST_IDLE;
            
            // Move toward player (if mobile boss)
            if (MoveSpeed > 0 && BossType != MidBossType.WhispyWoods)
            {
                Vector2 toPlayer = targetPlayer.Position - Position;
                
                // Stay within arena
                if (arenaBounds.Width > 0)
                {
                    Vector2 targetPos = Position + toPlayer.SafeNormalize() * MoveSpeed * Engine.DeltaTime;
                    if (arenaBounds.Contains((int)targetPos.X, (int)targetPos.Y))
                    {
                        MoveH(toPlayer.X > 0 ? MoveSpeed * Engine.DeltaTime : -MoveSpeed * Engine.DeltaTime);
                    }
                }
                else
                {
                    MoveH(toPlayer.X > 0 ? MoveSpeed * Engine.DeltaTime : -MoveSpeed * Engine.DeltaTime);
                }
                
                facingRight = toPlayer.X > 0;
            }
            
            // Attack when ready
            if (attackTimer <= 0)
            {
                return ST_ATTACK;
            }
            
            return ST_IDLE;
        }

        // ATTACK STATE
        private void AttackBegin()
        {
            sprite?.Play("attack");
            stateTimer = 1f;
            
            // Select and perform attack
            SelectAttackPattern();
            PerformAttack();
        }

        private int AttackUpdate()
        {
            if (stateTimer <= 0)
            {
                attackTimer = attackCooldown * (CurrentPhase == BossPhase.Enraged ? 0.5f : 1f);
                return ST_IDLE;
            }
            
            return ST_ATTACK;
        }

        private void AttackEnd()
        {
            sprite?.Play("idle");
        }

        // TRANSITION STATE
        private void TransitionBegin()
        {
            sprite?.Play("hurt");
            Collidable = false;
            invulnerableTimer = 2f;
        }

        private int TransitionUpdate()
        {
            return ST_TRANSITION; // Coroutine handles transition
        }

        private IEnumerator TransitionCoroutine()
        {
            var level = Scene as Level;
            
            // Phase transition effects
            Audio.Play("event:/game/general/thing_booped", Position);
            level?.Shake(0.3f);
            
            yield return 0.5f;
            
            // Visual effect
            for (int i = 0; i < 12; i++)
            {
                float angle = i * MathHelper.TwoPi / 12f;
                level?.Particles.Emit(Refill.P_Shatter, Position, GetBossColor(), angle);
            }
            
            yield return 1f;
            
            // Speed up attacks in later phases
            if (CurrentPhase == BossPhase.Enraged)
            {
                attackCooldown *= 0.6f;
                MoveSpeed *= 1.3f;
            }
            
            Collidable = true;
            stateMachine.State = ST_IDLE;
        }

        // STUNNED STATE
        private void StunnedBegin()
        {
            sprite?.Play("hurt");
            stateTimer = 1f;
        }

        private int StunnedUpdate()
        {
            if (stateTimer <= 0)
            {
                return ST_IDLE;
            }
            return ST_STUNNED;
        }

        // DEFEATED STATE
        private void DefeatedBegin()
        {
            sprite?.Play("defeated");
            Collidable = false;
            State = ActorState.Defeated;
        }

        private int DefeatedUpdate()
        {
            return ST_DEFEATED; // Coroutine handles removal
        }

        private IEnumerator DefeatedCoroutine()
        {
            var level = Scene as Level;
            
            // Defeat effects
            Audio.Play("event:/game/general/spring", Position);
            level?.Shake(0.5f);
            
            // Explosion particles
            for (float t = 0f; t < 2f; t += 0.1f)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offset = Calc.Random.Range(-32f, 32f) * Vector2.One;
                    level?.Particles.Emit(Refill.P_Shatter, Position + offset, GetBossColor());
                }
                yield return 0.1f;
            }
            
            // Drop reward
            DropBossReward();
            
            // Final explosion
            level?.Flash(Color.White, true);
            Audio.Play("event:/game/general/wall_break_stone", Position);
            
            yield return 0.5f;
            
            RemoveSelf();
        }

        #endregion

        #region Attack Methods

        private void SelectAttackPattern()
        {
            int maxPatterns = BossType switch
            {
                MidBossType.WhispyWoods => 3,
                MidBossType.Kracko => 4,
                MidBossType.MrFrosty => 3,
                MidBossType.Bonkers => 3,
                MidBossType.Bugzzy => 2,
                MidBossType.FireLion => 3,
                MidBossType.IronMam => 2,
                MidBossType.GrandWheely => 2,
                MidBossType.BoxBoxer => 4,
                MidBossType.MasterHand => 5,
                _ => 2
            };
            
            attackPattern = Calc.Random.Next(0, maxPatterns);
        }

        private void PerformAttack()
        {
            switch (BossType)
            {
                case MidBossType.WhispyWoods:
                    WhispyAttack();
                    break;
                case MidBossType.Kracko:
                    KrackoAttack();
                    break;
                case MidBossType.MrFrosty:
                    MrFrostyAttack();
                    break;
                case MidBossType.Bonkers:
                    BonkersAttack();
                    break;
                case MidBossType.Bugzzy:
                    BugzzyAttack();
                    break;
                case MidBossType.FireLion:
                    FireLionAttack();
                    break;
                case MidBossType.IronMam:
                    IronMamAttack();
                    break;
                case MidBossType.GrandWheely:
                    GrandWheelyAttack();
                    break;
                case MidBossType.BoxBoxer:
                    BoxBoxerAttack();
                    break;
                case MidBossType.MasterHand:
                    MasterHandAttack();
                    break;
            }
        }

        private void WhispyAttack()
        {
            switch (attackPattern)
            {
                case 0: // Drop apples
                    Add(new Coroutine(DropApplesRoutine()));
                    break;
                case 1: // Blow air
                    Add(new Coroutine(BlowAirRoutine()));
                    break;
                case 2: // Root attack
                    Add(new Coroutine(RootAttackRoutine()));
                    break;
            }
        }

        private IEnumerator DropApplesRoutine()
        {
            Audio.Play("event:/game/general/thing_booped", Position);
            
            int appleCount = CurrentPhase == BossPhase.Enraged ? 5 : 3;
            for (int i = 0; i < appleCount; i++)
            {
                Vector2 spawnPos = Position + new Vector2(Calc.Random.Range(-48f, 48f), -80f);
                Scene.Add(new KirbyFood(spawnPos, KirbyFood.FoodType.Apple, true)); // Damaging apple
                yield return 0.3f;
            }
        }

        private IEnumerator BlowAirRoutine()
        {
            Audio.Play("event:/game/general/spring", Position);
            
            var level = Scene as Level;
            float pushForce = CurrentPhase == BossPhase.Enraged ? 200f : 100f;
            
            for (float t = 0f; t < 1.5f; t += Engine.DeltaTime)
            {
                if (targetPlayer != null)
                {
                    // Push player away
                    Vector2 pushDir = (targetPlayer.Position - Position).SafeNormalize();
                    targetPlayer.MoveH(pushDir.X * pushForce * Engine.DeltaTime);
                }
                
                // Visual effect
                level?.Displacement.AddBurst(Position + new Vector2(32f, 0f), 0.1f, 16f, 64f, 0.2f);
                yield return null;
            }
        }

        private IEnumerator RootAttackRoutine()
        {
            if (targetPlayer == null) yield break;
            
            Audio.Play("event:/game/general/thing_booped", Position);
            
            // Spawn root projectiles from ground
            Vector2 targetPos = targetPlayer.Position;
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawnPos = new Vector2(targetPos.X + (i - 1) * 32f, Position.Y + 32f);
                Scene.Add(new KirbyProjectile(spawnPos, new Vector2(0, -100f), KirbyProjectile.ProjectileType.Root, this));
                yield return 0.2f;
            }
        }

        private void KrackoAttack()
        {
            switch (attackPattern)
            {
                case 0: // Lightning bolt
                    ShootLightning();
                    break;
                case 1: // Summon Waddle Doos
                    Add(new Coroutine(SummonWaddleDoosRoutine()));
                    break;
                case 2: // Rain attack
                    Add(new Coroutine(RainAttackRoutine()));
                    break;
                case 3: // Charge attack
                    Add(new Coroutine(KrackoChargeRoutine()));
                    break;
            }
        }

        private void ShootLightning()
        {
            if (targetPlayer == null) return;
            
            Audio.Play("event:/game/general/thing_booped", Position);
            Scene.Add(new KirbyProjectile(Position, 
                (targetPlayer.Position - Position).SafeNormalize() * 200f, 
                KirbyProjectile.ProjectileType.Lightning, this));
        }

        private IEnumerator SummonWaddleDoosRoutine()
        {
            Audio.Play("event:/game/general/thing_booped", Position);
            
            int count = CurrentPhase == BossPhase.Enraged ? 3 : 2;
            for (int i = 0; i < count; i++)
            {
                Vector2 spawnPos = Position + new Vector2(Calc.Random.Range(-32f, 32f), 16f);
                Scene.Add(new KirbySmallEnemy(spawnPos, KirbySmallEnemy.EnemyVariant.WaddleDoo));
                yield return 0.3f;
            }
        }

        private IEnumerator RainAttackRoutine()
        {
            Audio.Play("event:/game/general/thing_booped", Position);
            
            for (int i = 0; i < 10; i++)
            {
                Vector2 spawnPos = Position + new Vector2(Calc.Random.Range(-80f, 80f), 0f);
                Scene.Add(new KirbyProjectile(spawnPos, new Vector2(0, 150f), KirbyProjectile.ProjectileType.Rain, this));
                yield return 0.15f;
            }
        }

        private IEnumerator KrackoChargeRoutine()
        {
            if (targetPlayer == null) yield break;
            
            Audio.Play("event:/game/general/spring", Position);
            
            Vector2 direction = (targetPlayer.Position - Position).SafeNormalize();
            float chargeSpeed = 200f;
            
            for (float t = 0f; t < 0.8f; t += Engine.DeltaTime)
            {
                MoveH(direction.X * chargeSpeed * Engine.DeltaTime);
                MoveV(direction.Y * chargeSpeed * Engine.DeltaTime);
                yield return null;
            }
        }

        private void MrFrostyAttack()
        {
            switch (attackPattern)
            {
                case 0: // Throw ice block
                    ThrowIceBlock();
                    break;
                case 1: // Slide attack
                    Add(new Coroutine(SlideAttackRoutine()));
                    break;
                case 2: // Ice breath
                    Add(new Coroutine(IceBreathRoutine()));
                    break;
            }
        }

        private void ThrowIceBlock()
        {
            if (targetPlayer == null) return;
            
            Audio.Play("event:/game/general/thing_booped", Position);
            Vector2 direction = (targetPlayer.Position - Position).SafeNormalize();
            Scene.Add(new KirbyProjectile(Position + new Vector2(0, -16f), direction * 120f, KirbyProjectile.ProjectileType.Ice, this));
        }

        private IEnumerator SlideAttackRoutine()
        {
            Audio.Play("event:/game/general/spring", Position);
            
            float slideDirection = facingRight ? 1 : -1;
            float slideSpeed = 150f;
            
            for (float t = 0f; t < 1f; t += Engine.DeltaTime)
            {
                MoveH(slideDirection * slideSpeed * Engine.DeltaTime);
                yield return null;
            }
        }

        private IEnumerator IceBreathRoutine()
        {
            Audio.Play("event:/game/general/thing_booped", Position);
            
            float direction = facingRight ? 1 : -1;
            for (int i = 0; i < 5; i++)
            {
                Vector2 spawnPos = Position + new Vector2(direction * 16f, -8f);
                Scene.Add(new KirbyProjectile(spawnPos, new Vector2(direction * 80f, Calc.Random.Range(-20f, 20f)), KirbyProjectile.ProjectileType.Ice, this));
                yield return 0.1f;
            }
        }

        // Simplified attack implementations for other bosses
        private void BonkersAttack() => PerformGenericAttack();
        private void BugzzyAttack() => PerformGenericAttack();
        private void FireLionAttack() => PerformGenericAttack();
        private void IronMamAttack() => PerformGenericAttack();
        private void GrandWheelyAttack() => PerformGenericAttack();
        private void BoxBoxerAttack() => PerformGenericAttack();
        private void MasterHandAttack() => PerformGenericAttack();

        private void PerformGenericAttack()
        {
            if (targetPlayer == null) return;
            
            Audio.Play("event:/game/general/thing_booped", Position);
            
            // Basic projectile attack
            Vector2 direction = (targetPlayer.Position - Position).SafeNormalize();
            Scene.Add(new KirbyProjectile(Position, direction * 100f, KirbyProjectile.ProjectileType.Star, this));
        }

        #endregion

        private void DropBossReward()
        {
            // Drop max tomato
            Scene.Add(new KirbyFood(Position, KirbyFood.FoodType.MaxTomato));
            
            // Chance to drop extra reward
            if (Calc.Random.Chance(0.5f))
            {
                Scene.Add(new KirbyFood(Position + new Vector2(24f, 0f), KirbyFood.FoodType.CherryBunch));
            }
        }

        protected override void OnPlayerCollision(global::Celeste.Player player)
        {
            if (IsDefeated || invulnerableTimer > 0) return;
            
            // Boss contact damage
            player.Die((player.Position - Position).SafeNormalize());
        }
    }
}
