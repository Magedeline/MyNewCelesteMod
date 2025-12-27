using DesoloZantas.Core.Core.Effects;
using DesoloZantas.Core.Core.Player;

namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/Kirby_Player")]
    [Monocle.Tracked]
    public class KirbyPlayer : Actor
    {
    public enum PowerState
    {
        None,
        Fire,
        Ice,
        Spark,
        Stone,
        Sword
    }

    // Map power states to element types
    private static readonly Dictionary<PowerState, ElementType> PowerToElement = new Dictionary<PowerState, ElementType>
    {
        { PowerState.Fire, ElementType.Fire },
        { PowerState.Ice, ElementType.Ice },
        { PowerState.Spark, ElementType.Lightning },
        { PowerState.Stone, ElementType.Earth },
        { PowerState.Sword, ElementType.Light }
    };        public PowerState CurrentPower { get; private set; }
        public bool IsInhaling { get; internal set; }
        public bool IsHovering { get; internal set; }
        public bool IsActive { get; set; } = true;

    private Sprite sprite;
    private StateMachine stateMachine;
        private float inhaleTimer = 0f;
    internal List<Entity> InhaledEntities = new List<Entity>();

        // Add a Speed property to fix the CS0103 errors
        internal int CurrentHealth = 100; // Default health value
        internal int MaxHealth = 100; // Default max health value

        public Vector2 Speed { get; set; } = Vector2.Zero;

        // Add constructor that matches the expected signature
        public KirbyPlayer(Vector2 position) : base(position)
        {
            initializeKirbyPlayer();
        }

        // Add constructor with EntityData for CustomEntity support
        public KirbyPlayer(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            initializeKirbyPlayer();
        }

        public Sprite Sprite => sprite;

        public void Heal(int amount)
        {
            if (amount <= 0)
                return;

            CurrentHealth = Calc.Clamp(CurrentHealth + amount, 0, MaxHealth);
        }

        public int ConsumeInhaledEntities()
        {
            int count = InhaledEntities.Count;
            if (count == 0)
                return 0;

            foreach (Entity entity in InhaledEntities)
            {
                entity?.RemoveSelf();
            }

            InhaledEntities.Clear();
            return count;
        }

        private void initializeKirbyPlayer()
        {
            Collider = new Monocle.Hitbox(16f, 16f, -8f, -8f);

            // Safely create sprite
            try
            {
                Add(sprite = GFX.SpriteBank.Create("kirby_player"));
            }
            catch
            {
                // Fallback if sprite doesn't exist - create a basic sprite
                sprite = new Sprite(GFX.Game, "characters/player/");
                sprite.AddLoop("idle", "idle", 0.1f, 0);
                sprite.Play("idle");
                Add(sprite);
            }

            Add(stateMachine = new Monocle.StateMachine());

            stateMachine.SetCallbacks(0, stateNormal, null, beginNormal, null);
            stateMachine.SetCallbacks(1, startInhale, null, beginInhale, endInhale);
            stateMachine.SetCallbacks(2, stateHover, null, beginHover, endHover);
        }

        private int stateNormal()
        {
            // Handle normal movement
            Vector2 moveH = new Vector2(Input.MoveX.Value, Input.MoveY.Value);

            if (Input.Jump.Pressed && OnGround())
            {
                jump();
            }

            // Safe access to settings
            var settings = IngesteModule.Settings;
            bool inhaleCheck = false;
            bool hoverCheck = false;

            if (settings != null)
            {
                inhaleCheck = settings.InhaleHoldMode ? Input.Grab.Check : Input.Grab.Pressed;
            }
            else
            {
                // Fallback behavior
                inhaleCheck = Input.Grab.Pressed;
                hoverCheck = Input.Jump.Pressed;
            }

            if (inhaleCheck)
            {
                return 1; // Switch to inhale state
            }

            if (!OnGround() && hoverCheck)
            {
                return 2; // Switch to hover state
            }

            // Check for warper dash activation
            if (Input.Dash.Pressed && CurrentPower != PowerState.None)
            {
                WarperDashSimple.PerformWarperDash(Position, new Vector2(Input.MoveX.Value, Input.MoveY.Value), Scene);
                
                // Add elemental effect to dash
                if (PowerToElement.ContainsKey(CurrentPower))
                {
                    ElementalEffectsManager.PlayElementalEffect(PowerToElement[CurrentPower], Scene as Level, Position);
                }
                
                return 0; // Stay in normal state
            }

            // Check for elemental attack
            if (Input.Grab.Pressed && CurrentPower != PowerState.None)
            {
                PerformElementalAttack();
                return 0; // Stay in normal state
            }

            // Check for Kirby Knight transformation in chapters 19-20
            if (Input.CrouchDash.Pressed)
            {
                KirbyKnightSimple.TryTransformToKnight(Scene, CurrentHealth <= 10);
                return 0; // Stay in normal state
            }

            return 0;
        }

        private void beginNormal()
        {
            sprite?.Play("idle");
        }

        private int startInhale()
        {
            IsInhaling = true;
            inhaleTimer += Monocle.Engine.DeltaTime;
            Speed = new Vector2(Speed.X, Math.Min(Speed.Y + 80f * Monocle.Engine.DeltaTime, 60f));

            if (inhaleTimer >= 0.5f)
            {
                endInhale();
                return 0; // Return to normal state after inhaling
            }

            return 1;
        }

        private void beginInhale()
        {
            sprite?.Play("inhale");
            inhaleTimer = 0f;
        }

        private void endInhale()
        {
            IsInhaling = false;
            sprite?.Play("idle");
            // Process inhaled entities

            InhaledEntities.Clear();
        }

        private int stateHover()
        {
            IsHovering = true;
            Speed = new Vector2(Speed.X, Math.Max(Speed.Y - 120f * Monocle.Engine.DeltaTime, -60f));

            if (OnGround() || !Input.Jump.Check)
            {
                return 0;
            }

            return 2;
        }

        private void beginHover()
        {
            sprite?.Play("hover");
        }

        private void endHover()
        {
            IsHovering = false;
        }

        private void jump()
        {
            Speed = new Vector2(Speed.X, -120f); // Adjust the jump speed as needed
        }

        /// <summary>
        /// Perform an elemental attack based on current power state
        /// </summary>
        private void PerformElementalAttack()
        {
            if (CurrentPower == PowerState.None || !(Scene is Level level)) return;

            Vector2 attackDirection = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
            if (attackDirection == Vector2.Zero)
                attackDirection = Vector2.UnitX; // Default to right

            var parameters = EffectParams.WithDirection(attackDirection);

            switch (CurrentPower)
            {
                case PowerState.Fire:
                    ElementalEffectsManager.PlayEffect("fire_burst", level, Position, parameters);
                    break;
                    
                case PowerState.Ice:
                    ElementalEffectsManager.PlayEffect("ice_burst", level, Position, parameters);
                    break;
                    
                case PowerState.Spark:
                    ElementalEffectsManager.PlayEffect("chain_lightning", level, Position, parameters);
                    break;
                    
                case PowerState.Stone:
                    ElementalEffectsManager.PlayEffect("earth_spike", level, Position, parameters);
                    break;
                    
                case PowerState.Sword:
                    ElementalEffectsManager.PlayEffect("light_beam", level, Position, 
                        EffectParams.WithEnd(Position + attackDirection * 48f));
                    break;
            }
        }

        /// <summary>
        /// Set the current power state and update visual effects
        /// </summary>
        public void SetPowerState(PowerState newPower)
        {
            if (CurrentPower == newPower) return;

            CurrentPower = newPower;
            
            // Update sprite based on power state
            string animationName = newPower switch
            {
                PowerState.Fire => "fire_idle",
                PowerState.Ice => "ice_idle", 
                PowerState.Spark => "spark_idle",
                PowerState.Stone => "stone_idle",
                PowerState.Sword => "sword_idle",
                _ => "idle"
            };

            sprite?.Play(animationName, restart: true);

            // Add power acquisition effect
            if (newPower != PowerState.None && PowerToElement.ContainsKey(newPower) && Scene is Level level)
            {
                ElementalEffectsManager.PlayElementalEffect(PowerToElement[newPower], level, Position);
            }
        }

        /// <summary>
        /// Get the current elemental type based on power state
        /// </summary>
        public ElementType GetCurrentElementType()
        {
            return PowerToElement.ContainsKey(CurrentPower) ? PowerToElement[CurrentPower] : ElementType.Fire;
        }

        public override void Update()
        {
            base.Update();

            // Apply speed to movement
            if (Speed != Vector2.Zero)
            {
                MoveH(Speed.X * Engine.DeltaTime);
                MoveV(Speed.Y * Engine.DeltaTime);

                // Apply basic physics
                if (OnGround())
                {
                    Speed = new Vector2(Speed.X * 0.8f, 0f); // Ground friction
                }
                else
                {
                    Speed = new Vector2(Speed.X, Speed.Y + 900f * Engine.DeltaTime); // Gravity
                }
            }
            
            // Check for fight loss and knight transformation in chapters 19-20
            if (CurrentHealth <= 0)
            {
                KirbyKnightSimple.TryTransformToKnight(Scene, true);
                if (KirbyKnightSimple.IsKnightActive)
                {
                    // Restore some health when transforming to knight
                    CurrentHealth = MaxHealth / 2;
                }
            }
            
            // Knight attack if in knight form
            if (KirbyKnightSimple.IsKnightActive && Input.Grab.Pressed)
            {
                KirbyKnightSimple.PerformKnightAttack(Position, Scene);
            }
        }
    }
}



