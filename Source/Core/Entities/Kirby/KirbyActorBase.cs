namespace DesoloZantas.Core.Core.Entities.Kirby
{
    /// <summary>
    /// Base class for all Kirby-style actors (enemies, NPCs, mid bosses)
    /// Provides common functionality like sprite handling, health, power types, and interactions
    /// </summary>
    [Tracked]
    public abstract class KirbyActorBase : Actor
    {
        /// <summary>
        /// Power types that enemies can grant when inhaled
        /// </summary>
        public enum PowerType
        {
            None,
            Fire,
            Ice,
            Spark,
            Stone,
            Sword,
            Beam,
            Cutter,
            Bomb,
            Wheel,
            Needle,
            Mirror
        }

        /// <summary>
        /// Actor behavior states
        /// </summary>
        public enum ActorState
        {
            Idle,
            Moving,
            Attacking,
            Stunned,
            BeingInhaled,
            Defeated,
            Talking // For NPCs
        }

        // Core properties
        public PowerType CurrentPower { get; protected set; }
        public ActorState State { get; protected set; }
        public int Health { get; protected set; }
        public int MaxHealth { get; protected set; }
        public float MoveSpeed { get; protected set; }
        public bool CanBeInhaled { get; protected set; }
        public bool IsDefeated => State == ActorState.Defeated;
        public bool IsFriendly { get; protected set; }
        
        // Visual components
        protected Sprite sprite;
        protected VertexLight light;
        protected BloomPoint bloom;
        protected Wiggler hitWiggler;
        
        // Movement and AI
        protected Vector2 startPosition;
        protected bool facingRight = true;
        protected float stateTimer;
        protected global::Celeste.Player targetPlayer;
        protected float detectionRange = 100f;
        protected float attackRange = 30f;
        
        // Inhale mechanics
        protected float inhaleResistance = 1f;
        protected float inhaleProgress = 0f;
        private const float INHALE_THRESHOLD = 1f;

        protected KirbyActorBase(Vector2 position) : base(position)
        {
            startPosition = position;
            State = ActorState.Idle;
            CanBeInhaled = true;
            IsFriendly = false;
            
            // Default collision
            Collider = new Hitbox(16f, 16f, -8f, -16f);
            
            // Add visual effects
            Add(light = new VertexLight(Color.White, 0.5f, 16, 32));
            Add(bloom = new BloomPoint(0.3f, 8f));
            Add(hitWiggler = Wiggler.Create(0.5f, 4f, v => {
                if (sprite != null)
                    sprite.Scale = Vector2.One * (1f + v * 0.15f);
            }));
            
            // Add player collision
            Add(new PlayerCollider(OnPlayerCollision));
        }

        protected KirbyActorBase(EntityData data, Vector2 offset) 
            : this(data.Position + offset)
        {
            // Read common properties from entity data
            CurrentPower = (PowerType)data.Int("powerType", 0);
            MaxHealth = data.Int("maxHealth", 1);
            Health = MaxHealth;
            MoveSpeed = data.Float("moveSpeed", 50f);
            CanBeInhaled = data.Bool("canBeInhaled", true);
            detectionRange = data.Float("detectionRange", 100f);
            attackRange = data.Float("attackRange", 30f);
            facingRight = data.Bool("facingRight", true);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            SetupSprite();
            Depth = Depths.Enemy;
        }

        public override void Update()
        {
            base.Update();
            
            // Find player
            targetPlayer ??= Scene.Tracker.GetEntity<global::Celeste.Player>();
            
            // Update facing direction based on player position
            if (targetPlayer != null && State != ActorState.BeingInhaled && State != ActorState.Defeated)
            {
                UpdateFacing();
            }
            
            // Handle being inhaled
            if (State == ActorState.BeingInhaled)
            {
                UpdateInhaleState();
            }
            
            // Update state timer
            if (stateTimer > 0)
            {
                stateTimer -= Engine.DeltaTime;
            }
            
            // Apply sprite facing
            if (sprite != null)
            {
                sprite.Scale.X = Math.Abs(sprite.Scale.X) * (facingRight ? 1 : -1);
            }
        }

        /// <summary>
        /// Setup the sprite for this actor - override in derived classes
        /// </summary>
        protected abstract void SetupSprite();

        /// <summary>
        /// Called when this actor is hit
        /// </summary>
        public virtual void OnHit(int damage, Vector2 knockback)
        {
            if (IsDefeated || State == ActorState.BeingInhaled)
                return;

            Health -= damage;
            hitWiggler.Start();
            
            // Apply knockback
            if (knockback != Vector2.Zero)
            {
                MoveH(knockback.X);
                MoveV(knockback.Y);
            }
            
            // Play hit effect
            PlayHitEffect();
            
            if (Health <= 0)
            {
                OnDefeated();
            }
            else
            {
                State = ActorState.Stunned;
                stateTimer = 0.5f;
            }
        }

        /// <summary>
        /// Called when this actor is defeated
        /// </summary>
        protected virtual void OnDefeated()
        {
            State = ActorState.Defeated;
            Collidable = false;
            
            // Spawn defeat particles
            var level = Scene as Level;
            if (level != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    float angle = i * MathHelper.TwoPi / 8f;
                    Vector2 dir = Calc.AngleToVector(angle, 1f);
                    level.Particles.Emit(Refill.P_Shatter, Position, Color.White, angle);
                }
            }
            
            // Play defeat sound
            Audio.Play("event:/game/general/thing_destroyed", Position);
            
            // Drop food or star based on power type
            DropReward();
            
            // Remove after delay
            Add(new Coroutine(DefeatCoroutine()));
        }

        private IEnumerator DefeatCoroutine()
        {
            // Flash and fade out
            float flashTimer = 0.5f;
            while (flashTimer > 0)
            {
                flashTimer -= Engine.DeltaTime;
                Visible = (int)(flashTimer * 20) % 2 == 0;
                yield return null;
            }
            
            RemoveSelf();
        }

        /// <summary>
        /// Drop reward when defeated
        /// </summary>
        protected virtual void DropReward()
        {
            if (CurrentPower != PowerType.None && Calc.Random.Chance(0.3f))
            {
                // Drop a food item
                Scene.Add(new KirbyFood(Position, KirbyFood.FoodType.MaxTomato));
            }
        }

        /// <summary>
        /// Begin being inhaled by Kirby
        /// </summary>
        public virtual void BeginInhale()
        {
            if (!CanBeInhaled || IsDefeated)
                return;
                
            State = ActorState.BeingInhaled;
            inhaleProgress = 0f;
            
            // Play inhale start sound
            Audio.Play("event:/game/general/thing_booped", Position);
        }

        /// <summary>
        /// Update while being inhaled
        /// </summary>
        protected virtual void UpdateInhaleState()
        {
            var kirby = Scene.Tracker.GetEntity<KirbyPlayer>();
            if (kirby == null || !kirby.IsInhaling)
            {
                // Kirby stopped inhaling
                State = ActorState.Idle;
                inhaleProgress = 0f;
                return;
            }
            
            // Move toward Kirby
            Vector2 toKirby = kirby.Position - Position;
            float distance = toKirby.Length();
            
            if (distance > 5f)
            {
                Vector2 moveDir = toKirby.SafeNormalize();
                float pullSpeed = 200f / inhaleResistance;
                MoveH(moveDir.X * pullSpeed * Engine.DeltaTime);
                MoveV(moveDir.Y * pullSpeed * Engine.DeltaTime);
            }
            
            // Progress inhale
            inhaleProgress += Engine.DeltaTime / inhaleResistance;
            
            if (inhaleProgress >= INHALE_THRESHOLD || distance < 10f)
            {
                // Successfully inhaled
                OnInhaled(kirby);
            }
        }

        /// <summary>
        /// Called when successfully inhaled by Kirby
        /// </summary>
        protected virtual void OnInhaled(KirbyPlayer kirby)
        {
            Audio.Play("event:/game/general/thing_collected", Position);
            
            // Add to Kirby's inhaled entities
            kirby.InhaledEntities.Add(this);
            
            // Hide but don't remove yet (Kirby will handle that)
            Visible = false;
            Collidable = false;
        }

        /// <summary>
        /// Handle player collision
        /// </summary>
        protected virtual void OnPlayerCollision(global::Celeste.Player player)
        {
            if (IsDefeated || IsFriendly)
                return;
                
            // Damage player on contact
            if (State != ActorState.BeingInhaled && State != ActorState.Stunned)
            {
                player.Die((player.Position - Position).SafeNormalize());
            }
        }

        protected void UpdateFacing()
        {
            if (targetPlayer != null)
            {
                facingRight = targetPlayer.Position.X > Position.X;
            }
        }

        protected void PlayHitEffect()
        {
            var level = Scene as Level;
            level?.Displacement.AddBurst(Position, 0.3f, 8f, 24f, 0.3f);
        }

        /// <summary>
        /// Get color associated with power type
        /// </summary>
        protected Color GetPowerColor()
        {
            return CurrentPower switch
            {
                PowerType.Fire => Color.OrangeRed,
                PowerType.Ice => Color.LightBlue,
                PowerType.Spark => Color.Yellow,
                PowerType.Stone => Color.Gray,
                PowerType.Sword => Color.LightGreen,
                PowerType.Beam => Color.Gold,
                PowerType.Cutter => Color.Pink,
                PowerType.Bomb => Color.DarkOrange,
                PowerType.Wheel => Color.Red,
                PowerType.Needle => Color.DarkGreen,
                PowerType.Mirror => Color.Purple,
                _ => Color.White
            };
        }

        /// <summary>
        /// Get sprite path for power type
        /// </summary>
        protected string GetSpritePathForPower()
        {
            return CurrentPower switch
            {
                PowerType.Fire => "enemies/kirby/fire_enemy",
                PowerType.Ice => "enemies/kirby/ice_enemy",
                PowerType.Spark => "enemies/kirby/spark_enemy",
                PowerType.Stone => "enemies/kirby/stone_enemy",
                PowerType.Sword => "enemies/kirby/sword_enemy",
                PowerType.Beam => "enemies/kirby/beam_enemy",
                PowerType.Cutter => "enemies/kirby/cutter_enemy",
                PowerType.Bomb => "enemies/kirby/bomb_enemy",
                PowerType.Wheel => "enemies/kirby/wheel_enemy",
                PowerType.Needle => "enemies/kirby/needle_enemy",
                PowerType.Mirror => "enemies/kirby/mirror_enemy",
                _ => "enemies/kirby/waddle_dee"
            };
        }
    }
}
