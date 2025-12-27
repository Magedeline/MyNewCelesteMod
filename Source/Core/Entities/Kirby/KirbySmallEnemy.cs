namespace DesoloZantas.Core.Core.Entities.Kirby
{
    /// <summary>
    /// Small enemies in the Kirby style - can be inhaled to gain powers
    /// Examples: Waddle Dee, Waddle Doo, Poppy Bros, Hot Head, Chilly, etc.
    /// </summary>
    [CustomEntity("Ingeste/KirbySmallEnemy")]
    [Tracked]
    public class KirbySmallEnemy : KirbyActorBase
    {
        /// <summary>
        /// Specific enemy types with unique behaviors
        /// </summary>
        public enum EnemyVariant
        {
            WaddleDee,      // No power, walks back and forth
            WaddleDoo,      // Beam power, shoots beams
            HotHead,        // Fire power, breathes fire
            Chilly,         // Ice power, freezes on contact
            Sparky,         // Spark power, electric field
            Rocky,          // Stone power, heavy and slow
            SirKibble,      // Cutter power, throws boomerangs
            Poppy,          // Bomb power, throws bombs
            Wheelie,        // Wheel power, fast movement
            Needlous,       // Needle power, spiky
            Simirror,       // Mirror power, reflects attacks
            Scarfy,         // No power, transforms when attacked
            Gordo           // Cannot be inhaled, invincible
        }

        public EnemyVariant Variant { get; private set; }
        
        // AI properties
        private float patrolDistance;
        private Vector2 patrolStart;
        private bool patrollingRight = true;
        private float attackCooldown;
        private float attackTimer;
        private bool isAggro;
        
        // Variant-specific properties
        private float flyHeight;
        private bool canFly;
        private bool isTransformed; // For Scarfy
        
        // State machine
        private StateMachine stateMachine;
        
        // State indices
        private const int ST_IDLE = 0;
        private const int ST_PATROL = 1;
        private const int ST_CHASE = 2;
        private const int ST_ATTACK = 3;
        private const int ST_STUNNED = 4;
        private const int ST_INHALED = 5;

        public KirbySmallEnemy(Vector2 position, EnemyVariant variant) : base(position)
        {
            Variant = variant;
            SetupVariant();
        }

        public KirbySmallEnemy(EntityData data, Vector2 offset) 
            : base(data, offset)
        {
            Variant = (EnemyVariant)data.Int("variant", 0);
            patrolDistance = data.Float("patrolDistance", 64f);
            
            SetupVariant();
        }

        private void SetupVariant()
        {
            patrolStart = Position;
            
            // Set properties based on variant
            switch (Variant)
            {
                case EnemyVariant.WaddleDee:
                    CurrentPower = PowerType.None;
                    MaxHealth = 1;
                    MoveSpeed = 30f;
                    attackCooldown = 0f; // No attack
                    inhaleResistance = 0.5f;
                    break;

                case EnemyVariant.WaddleDoo:
                    CurrentPower = PowerType.Beam;
                    MaxHealth = 2;
                    MoveSpeed = 25f;
                    attackCooldown = 2f;
                    attackRange = 80f;
                    inhaleResistance = 1f;
                    break;

                case EnemyVariant.HotHead:
                    CurrentPower = PowerType.Fire;
                    MaxHealth = 2;
                    MoveSpeed = 35f;
                    attackCooldown = 1.5f;
                    attackRange = 60f;
                    inhaleResistance = 1f;
                    light.Color = Color.Orange;
                    break;

                case EnemyVariant.Chilly:
                    CurrentPower = PowerType.Ice;
                    MaxHealth = 2;
                    MoveSpeed = 20f;
                    attackCooldown = 3f;
                    attackRange = 40f;
                    inhaleResistance = 0.8f;
                    light.Color = Color.LightBlue;
                    break;

                case EnemyVariant.Sparky:
                    CurrentPower = PowerType.Spark;
                    MaxHealth = 2;
                    MoveSpeed = 40f;
                    attackCooldown = 2f;
                    attackRange = 50f;
                    inhaleResistance = 1.2f;
                    light.Color = Color.Yellow;
                    break;

                case EnemyVariant.Rocky:
                    CurrentPower = PowerType.Stone;
                    MaxHealth = 4;
                    MoveSpeed = 15f;
                    attackCooldown = 0f;
                    inhaleResistance = 3f; // Very hard to inhale
                    CanBeInhaled = true;
                    break;

                case EnemyVariant.SirKibble:
                    CurrentPower = PowerType.Cutter;
                    MaxHealth = 2;
                    MoveSpeed = 30f;
                    attackCooldown = 2.5f;
                    attackRange = 100f;
                    inhaleResistance = 1f;
                    break;

                case EnemyVariant.Poppy:
                    CurrentPower = PowerType.Bomb;
                    MaxHealth = 2;
                    MoveSpeed = 25f;
                    attackCooldown = 3f;
                    attackRange = 70f;
                    inhaleResistance = 1f;
                    break;

                case EnemyVariant.Wheelie:
                    CurrentPower = PowerType.Wheel;
                    MaxHealth = 3;
                    MoveSpeed = 80f;
                    attackCooldown = 1f;
                    attackRange = 30f;
                    inhaleResistance = 1.5f;
                    break;

                case EnemyVariant.Needlous:
                    CurrentPower = PowerType.Needle;
                    MaxHealth = 2;
                    MoveSpeed = 25f;
                    attackCooldown = 0f;
                    inhaleResistance = 1.2f;
                    break;

                case EnemyVariant.Simirror:
                    CurrentPower = PowerType.Mirror;
                    MaxHealth = 2;
                    MoveSpeed = 35f;
                    attackCooldown = 2f;
                    attackRange = 60f;
                    inhaleResistance = 1f;
                    light.Color = Color.Purple;
                    break;

                case EnemyVariant.Scarfy:
                    CurrentPower = PowerType.None;
                    MaxHealth = 1;
                    MoveSpeed = 30f;
                    canFly = true;
                    flyHeight = -40f;
                    inhaleResistance = 0.8f;
                    break;

                case EnemyVariant.Gordo:
                    CurrentPower = PowerType.None;
                    MaxHealth = 999; // Effectively invincible
                    MoveSpeed = 0f;
                    CanBeInhaled = false;
                    inhaleResistance = float.MaxValue;
                    break;
            }

            Health = MaxHealth;
            attackTimer = attackCooldown;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            // Setup state machine
            Add(stateMachine = new StateMachine());
            
            stateMachine.SetCallbacks(ST_IDLE, IdleUpdate, null, IdleBegin, null);
            stateMachine.SetCallbacks(ST_PATROL, PatrolUpdate, null, PatrolBegin, null);
            stateMachine.SetCallbacks(ST_CHASE, ChaseUpdate, null, ChaseBegin, null);
            stateMachine.SetCallbacks(ST_ATTACK, AttackUpdate, null, AttackBegin, AttackEnd);
            stateMachine.SetCallbacks(ST_STUNNED, StunnedUpdate, null, StunnedBegin, null);
            stateMachine.SetCallbacks(ST_INHALED, InhaledUpdate, null, InhaledBegin, null);
            
            stateMachine.State = ST_IDLE;
        }

        protected override void SetupSprite()
        {
            string spritePath = GetSpritePathForVariant();
            
            Add(sprite = new Sprite(GFX.Game, spritePath));
            sprite.AddLoop("idle", "", 0.15f);
            sprite.AddLoop("walk", "", 0.1f);
            sprite.AddLoop("attack", "", 0.08f);
            sprite.AddLoop("hurt", "", 0.1f);
            sprite.CenterOrigin();
            sprite.Play("idle");
            
            // Adjust collider based on variant
            if (Variant == EnemyVariant.Gordo)
            {
                Collider = new Hitbox(24f, 24f, -12f, -12f);
            }
        }

        private string GetSpritePathForVariant()
        {
            return Variant switch
            {
                EnemyVariant.WaddleDee => "enemies/kirby/waddle_dee/",
                EnemyVariant.WaddleDoo => "enemies/kirby/waddle_doo/",
                EnemyVariant.HotHead => "enemies/kirby/hot_head/",
                EnemyVariant.Chilly => "enemies/kirby/chilly/",
                EnemyVariant.Sparky => "enemies/kirby/sparky/",
                EnemyVariant.Rocky => "enemies/kirby/rocky/",
                EnemyVariant.SirKibble => "enemies/kirby/sir_kibble/",
                EnemyVariant.Poppy => "enemies/kirby/poppy/",
                EnemyVariant.Wheelie => "enemies/kirby/wheelie/",
                EnemyVariant.Needlous => "enemies/kirby/needlous/",
                EnemyVariant.Simirror => "enemies/kirby/simirror/",
                EnemyVariant.Scarfy => "enemies/kirby/scarfy/",
                EnemyVariant.Gordo => "enemies/kirby/gordo/",
                _ => "enemies/kirby/waddle_dee/"
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
            
            // Check for aggro
            if (targetPlayer != null && !isAggro)
            {
                float distance = Vector2.Distance(Position, targetPlayer.Position);
                if (distance < detectionRange)
                {
                    isAggro = true;
                }
            }
            
            // Handle Scarfy transformation
            if (Variant == EnemyVariant.Scarfy && !isTransformed)
            {
                HandleScarfyTransform();
            }
        }

        #region State Machine Callbacks

        // IDLE STATE
        private void IdleBegin()
        {
            sprite?.Play("idle");
            stateTimer = Calc.Random.Range(0.5f, 2f);
        }

        private int IdleUpdate()
        {
            if (State == ActorState.BeingInhaled) return ST_INHALED;
            if (State == ActorState.Stunned) return ST_STUNNED;
            
            if (stateTimer <= 0)
            {
                return ST_PATROL;
            }
            
            if (isAggro && targetPlayer != null)
            {
                float distance = Vector2.Distance(Position, targetPlayer.Position);
                if (distance < attackRange && attackTimer <= 0 && attackCooldown > 0)
                {
                    return ST_ATTACK;
                }
                return ST_CHASE;
            }
            
            return ST_IDLE;
        }

        // PATROL STATE
        private void PatrolBegin()
        {
            sprite?.Play("walk");
        }

        private int PatrolUpdate()
        {
            if (State == ActorState.BeingInhaled) return ST_INHALED;
            if (State == ActorState.Stunned) return ST_STUNNED;
            
            // Move in patrol direction
            float moveX = (patrollingRight ? 1 : -1) * MoveSpeed * Engine.DeltaTime;
            MoveH(moveX, OnCollideH);
            
            // Check patrol bounds
            float distFromStart = Position.X - patrolStart.X;
            if (Math.Abs(distFromStart) > patrolDistance)
            {
                patrollingRight = !patrollingRight;
                facingRight = patrollingRight;
            }
            
            // Check for aggro
            if (isAggro && targetPlayer != null)
            {
                float distance = Vector2.Distance(Position, targetPlayer.Position);
                if (distance < attackRange && attackTimer <= 0 && attackCooldown > 0)
                {
                    return ST_ATTACK;
                }
                return ST_CHASE;
            }
            
            return ST_PATROL;
        }

        // CHASE STATE
        private void ChaseBegin()
        {
            sprite?.Play("walk");
        }

        private int ChaseUpdate()
        {
            if (State == ActorState.BeingInhaled) return ST_INHALED;
            if (State == ActorState.Stunned) return ST_STUNNED;
            if (targetPlayer == null) return ST_IDLE;
            
            float distance = Vector2.Distance(Position, targetPlayer.Position);
            
            // Attack if in range
            if (distance < attackRange && attackTimer <= 0 && attackCooldown > 0)
            {
                return ST_ATTACK;
            }
            
            // Move toward player
            float direction = Math.Sign(targetPlayer.Position.X - Position.X);
            facingRight = direction > 0;
            
            float moveX = direction * MoveSpeed * Engine.DeltaTime;
            MoveH(moveX, OnCollideH);
            
            // Flying enemies also adjust Y
            if (canFly)
            {
                float targetY = targetPlayer.Position.Y + flyHeight;
                float moveY = Math.Sign(targetY - Position.Y) * MoveSpeed * 0.5f * Engine.DeltaTime;
                MoveV(moveY);
            }
            
            // Lose aggro if too far
            if (distance > detectionRange * 2)
            {
                isAggro = false;
                return ST_PATROL;
            }
            
            return ST_CHASE;
        }

        // ATTACK STATE
        private void AttackBegin()
        {
            sprite?.Play("attack");
            stateTimer = 0.5f;
            PerformAttack();
        }

        private int AttackUpdate()
        {
            if (State == ActorState.BeingInhaled) return ST_INHALED;
            if (State == ActorState.Stunned) return ST_STUNNED;
            
            if (stateTimer <= 0)
            {
                attackTimer = attackCooldown;
                return ST_CHASE;
            }
            
            return ST_ATTACK;
        }

        private void AttackEnd()
        {
            // Reset attack state
        }

        // STUNNED STATE
        private void StunnedBegin()
        {
            sprite?.Play("hurt");
            stateTimer = 0.5f;
        }

        private int StunnedUpdate()
        {
            if (State == ActorState.BeingInhaled) return ST_INHALED;
            
            if (stateTimer <= 0)
            {
                State = ActorState.Idle;
                return ST_IDLE;
            }
            
            return ST_STUNNED;
        }

        // INHALED STATE
        private void InhaledBegin()
        {
            // Handled by base class
        }

        private int InhaledUpdate()
        {
            if (State != ActorState.BeingInhaled)
            {
                return ST_IDLE;
            }
            return ST_INHALED;
        }

        #endregion

        #region Attack Methods

        private void PerformAttack()
        {
            if (targetPlayer == null) return;
            
            switch (Variant)
            {
                case EnemyVariant.WaddleDoo:
                    ShootBeam();
                    break;
                case EnemyVariant.HotHead:
                    BreatheFire();
                    break;
                case EnemyVariant.Chilly:
                    FreezeAura();
                    break;
                case EnemyVariant.Sparky:
                    ElectricField();
                    break;
                case EnemyVariant.SirKibble:
                    ThrowCutter();
                    break;
                case EnemyVariant.Poppy:
                    ThrowBomb();
                    break;
                case EnemyVariant.Wheelie:
                    ChargeAttack();
                    break;
                case EnemyVariant.Simirror:
                    MirrorShot();
                    break;
            }
            
            Audio.Play("event:/game/general/thing_booped", Position);
        }

        private void ShootBeam()
        {
            Vector2 direction = (targetPlayer.Position - Position).SafeNormalize();
            Scene.Add(new KirbyProjectile(Position, direction * 150f, KirbyProjectile.ProjectileType.Beam, this));
        }

        private void BreatheFire()
        {
            Vector2 direction = new Vector2(facingRight ? 1 : -1, 0);
            Scene.Add(new KirbyProjectile(Position + direction * 8f, direction * 100f, KirbyProjectile.ProjectileType.Fire, this));
        }

        private void FreezeAura()
        {
            // Create ice particles around self
            var level = Scene as Level;
            if (level != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    float angle = i * MathHelper.TwoPi / 8f;
                    level.Particles.Emit(Refill.P_Glow, Position + Calc.AngleToVector(angle, 24f), Color.LightBlue, angle);
                }
            }
        }

        private void ElectricField()
        {
            // Spark damage in area
            var level = Scene as Level;
            level?.Displacement.AddBurst(Position, 0.3f, 16f, 48f, 0.3f);
        }

        private void ThrowCutter()
        {
            Vector2 direction = (targetPlayer.Position - Position).SafeNormalize();
            Scene.Add(new KirbyProjectile(Position, direction * 120f, KirbyProjectile.ProjectileType.Cutter, this));
        }

        private void ThrowBomb()
        {
            Vector2 direction = (targetPlayer.Position - Position).SafeNormalize();
            Scene.Add(new KirbyProjectile(Position, direction * 80f + new Vector2(0, -50f), KirbyProjectile.ProjectileType.Bomb, this));
        }

        private void ChargeAttack()
        {
            // Dash forward
            float direction = facingRight ? 1 : -1;
            MoveH(direction * 100f * Engine.DeltaTime);
        }

        private void MirrorShot()
        {
            Vector2 direction = (targetPlayer.Position - Position).SafeNormalize();
            Scene.Add(new KirbyProjectile(Position, direction * 100f, KirbyProjectile.ProjectileType.Mirror, this));
        }

        #endregion

        private void HandleScarfyTransform()
        {
            var kirby = Scene.Tracker.GetEntity<KirbyPlayer>();
            if (kirby != null && kirby.IsInhaling)
            {
                float distance = Vector2.Distance(Position, kirby.Position);
                if (distance < 60f)
                {
                    // Transform into angry Scarfy
                    isTransformed = true;
                    CanBeInhaled = false;
                    MoveSpeed = 100f;
                    Health = 2;
                    MaxHealth = 2;
                    sprite?.Play("attack"); // Use angry animation
                    Audio.Play("event:/game/general/thing_booped", Position);
                }
            }
        }

        private void OnCollideH(CollisionData data)
        {
            // Turn around on collision
            patrollingRight = !patrollingRight;
            facingRight = patrollingRight;
        }

        public override void BeginInhale()
        {
            if (Variant == EnemyVariant.Gordo)
            {
                // Gordo can't be inhaled - bounce Kirby back
                Audio.Play("event:/game/general/wall_break_stone", Position);
                return;
            }
            
            base.BeginInhale();
            stateMachine.State = ST_INHALED;
        }
    }
}
