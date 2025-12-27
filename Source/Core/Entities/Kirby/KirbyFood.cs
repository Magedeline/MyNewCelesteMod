namespace DesoloZantas.Core.Core.Entities.Kirby
{
    /// <summary>
    /// Healable food items in Kirby style - restore health when collected
    /// Examples: Apple, Maxim Tomato, Cherry, Meat, Cake, etc.
    /// </summary>
    [CustomEntity("Ingeste/KirbyFood")]
    [Tracked]
    public class KirbyFood : Actor
    {
        /// <summary>
        /// Food types with different healing values and effects
        /// </summary>
        public enum FoodType
        {
            // Basic Foods (1-2 health)
            Apple,
            Orange,
            Cherry,
            Banana,
            Watermelon,
            Pineapple,
            Grapes,
            Strawberry,
            
            // Medium Foods (3-5 health)
            Meat,
            Fish,
            IceCream,
            Cake,
            Hamburger,
            HotDog,
            Pizza,
            Sandwich,
            CherryBunch,
            
            // Large Foods (Full heal)
            MaxTomato,
            
            // Special Items
            InvincibilityStar,  // Temporary invincibility
            OneUp,              // Extra life
            PointStar,          // Score points
            
            // Custom
            Custom
        }

        public FoodType Type { get; private set; }
        public int HealAmount { get; private set; }
        public bool IsDamaging { get; private set; } // For boss-dropped items
        public bool IsSpecial => Type == FoodType.InvincibilityStar || Type == FoodType.OneUp;
        
        // Components
        private Sprite sprite;
        private VertexLight light;
        private BloomPoint bloom;
        private Wiggler collectWiggler;
        private SineWave sine;
        
        // Physics
        private Vector2 velocity;
        private float gravity = 200f;
        private float friction = 100f;
        private float bounce = 0.3f;
        private bool isGrounded;
        private bool isFalling = true;
        
        // State
        private float despawnTimer = 10f; // Despawn after 10 seconds
        private float flashTimer;
        private bool collected;

        public KirbyFood(Vector2 position, FoodType type, bool isDamaging = false) : base(position)
        {
            Type = type;
            IsDamaging = isDamaging;
            
            SetupFoodType();
            Initialize();
        }

        public KirbyFood(EntityData data, Vector2 offset) 
            : base(data.Position + offset)
        {
            Type = (FoodType)data.Int("foodType", 0);
            HealAmount = data.Int("healAmount", 0);
            IsDamaging = data.Bool("isDamaging", false);
            despawnTimer = data.Float("despawnTime", 10f);
            isFalling = data.Bool("isFalling", false);
            
            if (HealAmount == 0)
            {
                SetupFoodType();
            }
            
            Initialize();
        }

        private void SetupFoodType()
        {
            HealAmount = Type switch
            {
                // Basic Foods
                FoodType.Apple => 1,
                FoodType.Orange => 1,
                FoodType.Cherry => 1,
                FoodType.Banana => 1,
                FoodType.Watermelon => 2,
                FoodType.Pineapple => 2,
                FoodType.Grapes => 2,
                FoodType.Strawberry => 1,
                
                // Medium Foods
                FoodType.Meat => 4,
                FoodType.Fish => 3,
                FoodType.IceCream => 3,
                FoodType.Cake => 5,
                FoodType.Hamburger => 4,
                FoodType.HotDog => 3,
                FoodType.Pizza => 4,
                FoodType.Sandwich => 3,
                FoodType.CherryBunch => 5,
                
                // Large Foods
                FoodType.MaxTomato => 100, // Full heal
                
                // Special Items
                FoodType.InvincibilityStar => 0,
                FoodType.OneUp => 0,
                FoodType.PointStar => 0,
                
                _ => 1
            };
        }

        private void Initialize()
        {
            Collider = new Hitbox(16f, 16f, -8f, -8f);
            
            // Setup visual components
            Add(light = new VertexLight(GetFoodColor(), 0.6f, 8, 24));
            Add(bloom = new BloomPoint(0.4f, 8f));
            Add(collectWiggler = Wiggler.Create(0.4f, 4f, v => {
                if (sprite != null)
                    sprite.Scale = Vector2.One * (1f + v * 0.2f);
            }));
            Add(sine = new SineWave(0.4f));
            
            // Add player collision
            Add(new PlayerCollider(OnPlayerCollision));
            
            // Initial velocity for dropped items
            if (isFalling)
            {
                velocity = new Vector2(Calc.Random.Range(-50f, 50f), -100f);
            }
            
            Depth = Depths.Pickups;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            SetupSprite();
            collectWiggler.Start();
        }

        private void SetupSprite()
        {
            string spritePath = GetSpritePathForType();
            
            Add(sprite = new Sprite(GFX.Game, spritePath));
            sprite.AddLoop("idle", "", 0.15f);
            sprite.AddLoop("shine", "", 0.1f);
            sprite.CenterOrigin();
            sprite.Play("idle");
            
            // Special items have shine animation
            if (IsSpecial)
            {
                sprite.Play("shine");
            }
        }

        private string GetSpritePathForType()
        {
            return Type switch
            {
                FoodType.Apple => "items/kirby/food/apple/",
                FoodType.Orange => "items/kirby/food/orange/",
                FoodType.Cherry => "items/kirby/food/cherry/",
                FoodType.Banana => "items/kirby/food/banana/",
                FoodType.Watermelon => "items/kirby/food/watermelon/",
                FoodType.Pineapple => "items/kirby/food/pineapple/",
                FoodType.Grapes => "items/kirby/food/grapes/",
                FoodType.Strawberry => "items/kirby/food/strawberry/",
                FoodType.Meat => "items/kirby/food/meat/",
                FoodType.Fish => "items/kirby/food/fish/",
                FoodType.IceCream => "items/kirby/food/icecream/",
                FoodType.Cake => "items/kirby/food/cake/",
                FoodType.Hamburger => "items/kirby/food/hamburger/",
                FoodType.HotDog => "items/kirby/food/hotdog/",
                FoodType.Pizza => "items/kirby/food/pizza/",
                FoodType.Sandwich => "items/kirby/food/sandwich/",
                FoodType.CherryBunch => "items/kirby/food/cherrybunch/",
                FoodType.MaxTomato => "items/kirby/food/maxtomato/",
                FoodType.InvincibilityStar => "items/kirby/star/",
                FoodType.OneUp => "items/kirby/oneup/",
                FoodType.PointStar => "items/kirby/pointstar/",
                _ => "items/kirby/food/apple/"
            };
        }

        private Color GetFoodColor()
        {
            return Type switch
            {
                FoodType.Apple => Color.Red,
                FoodType.Orange => Color.Orange,
                FoodType.Cherry => Color.DarkRed,
                FoodType.Banana => Color.Yellow,
                FoodType.Watermelon => Color.Green,
                FoodType.Pineapple => Color.Gold,
                FoodType.Grapes => Color.Purple,
                FoodType.Strawberry => Color.Pink,
                FoodType.Meat => Color.Brown,
                FoodType.Fish => Color.LightBlue,
                FoodType.IceCream => Color.Pink,
                FoodType.Cake => Color.White,
                FoodType.Hamburger => Color.SandyBrown,
                FoodType.HotDog => Color.Tan,
                FoodType.Pizza => Color.Orange,
                FoodType.Sandwich => Color.Wheat,
                FoodType.CherryBunch => Color.DarkRed,
                FoodType.MaxTomato => Color.Red,
                FoodType.InvincibilityStar => Color.Gold,
                FoodType.OneUp => Color.LimeGreen,
                FoodType.PointStar => Color.Yellow,
                _ => Color.White
            };
        }

        public override void Update()
        {
            base.Update();
            
            if (collected) return;
            
            // Physics update
            if (isFalling)
            {
                UpdatePhysics();
            }
            else
            {
                // Floating animation for placed items
                Position.Y += sine.Value * Engine.DeltaTime * 2f;
            }
            
            // Despawn timer
            despawnTimer -= Engine.DeltaTime;
            
            if (despawnTimer <= 3f)
            {
                // Flash when about to despawn
                flashTimer += Engine.DeltaTime;
                Visible = (int)(flashTimer * 8) % 2 == 0;
            }
            
            if (despawnTimer <= 0)
            {
                RemoveSelf();
            }
            
            // Rotate special items
            if (IsSpecial)
            {
                sprite.Rotation += Engine.DeltaTime * 2f;
            }
        }

        private void UpdatePhysics()
        {
            // Apply gravity
            velocity.Y += gravity * Engine.DeltaTime;
            
            // Apply friction when grounded
            if (isGrounded)
            {
                velocity.X = Calc.Approach(velocity.X, 0f, friction * Engine.DeltaTime);
            }
            
            // Move horizontally
            float moveX = velocity.X * Engine.DeltaTime;
            bool collidedH = MoveH(moveX, OnCollideH);
            if (collidedH)
            {
                velocity.X *= -bounce;
            }
            
            // Move vertically
            float moveY = velocity.Y * Engine.DeltaTime;
            bool collidedV = MoveV(moveY, OnCollideV);
            
            if (collidedV && velocity.Y > 0)
            {
                isGrounded = true;
                velocity.Y *= -bounce;
                
                // Stop bouncing if velocity is low enough
                if (Math.Abs(velocity.Y) < 20f)
                {
                    velocity.Y = 0f;
                    isFalling = false;
                }
            }
            else
            {
                isGrounded = false;
            }
        }

        private void OnCollideH(CollisionData data)
        {
            velocity.X *= -bounce;
        }

        private void OnCollideV(CollisionData data)
        {
            velocity.Y *= -bounce;
        }

        private void OnPlayerCollision(global::Celeste.Player player)
        {
            if (collected) return;
            
            // Damaging food hurts the player
            if (IsDamaging)
            {
                player.Die((player.Position - Position).SafeNormalize());
                collected = true;
                RemoveSelf();
                return;
            }
            
            Collect(player);
        }

        private void Collect(global::Celeste.Player player)
        {
            collected = true;
            Collidable = false;
            
            // Apply effect based on type
            if (Type == FoodType.InvincibilityStar)
            {
                ApplyInvincibility(player);
            }
            else if (Type == FoodType.OneUp)
            {
                ApplyOneUp(player);
            }
            else if (Type == FoodType.PointStar)
            {
                // Just score points
                Audio.Play("event:/game/general/diamond_touch", Position);
            }
            else
            {
                // Heal Kirby player
                HealPlayer(player);
            }
            
            // Visual effects
            PlayCollectEffect();
            
            // Remove
            Add(new Coroutine(CollectRoutine()));
        }

        private void HealPlayer(global::Celeste.Player player)
        {
            // Try to find KirbyPlayer for healing
            var kirbyPlayer = Scene.Tracker.GetEntity<KirbyPlayer>();
            
            if (kirbyPlayer != null)
            {
                int healAmount = HealAmount;
                
                // Max tomato heals to full
                if (Type == FoodType.MaxTomato)
                {
                    healAmount = kirbyPlayer.MaxHealth;
                }
                
                kirbyPlayer.Heal(healAmount);
                
                // Show heal number
                ShowHealNumber(healAmount);
            }
            
            // Play appropriate sound
            if (Type == FoodType.MaxTomato)
            {
                Audio.Play("event:/game/general/cassette_obtain", Position);
            }
            else
            {
                Audio.Play("event:/game/general/diamond_touch", Position);
            }
        }

        private void ShowHealNumber(int amount)
        {
            // Spawn floating number (simplified - could be expanded)
            var level = Scene as Level;
            if (level != null)
            {
                // Particles as visual feedback
                for (int i = 0; i < amount && i < 5; i++)
                {
                    level.Particles.Emit(Refill.P_Glow, Position + new Vector2(0, -8f * i), Color.Lime);
                }
            }
        }

        private void ApplyInvincibility(global::Celeste.Player player)
        {
            Audio.Play("event:/game/general/cassette_obtain", Position);
            
            // Apply invincibility effect through KirbyPlayer
            var kirbyPlayer = Scene.Tracker.GetEntity<KirbyPlayer>();
            if (kirbyPlayer != null)
            {
                // TODO: Implement invincibility state in KirbyPlayer
            }
            
            // Visual effect
            var level = Scene as Level;
            level?.Flash(Color.Gold * 0.5f, true);
        }

        private void ApplyOneUp(global::Celeste.Player player)
        {
            Audio.Play("event:/game/general/heart_collect", Position);
            
            // Add extra life through session
            var level = Scene as Level;
            if (level != null)
            {
                // Could track lives in session data
            }
            
            // Visual effect
            level?.Flash(Color.LimeGreen * 0.3f, true);
        }

        private void PlayCollectEffect()
        {
            var level = Scene as Level;
            if (level == null) return;
            
            // Burst particles
            Color particleColor = GetFoodColor();
            for (int i = 0; i < 8; i++)
            {
                float angle = i * MathHelper.TwoPi / 8f;
                level.Particles.Emit(Refill.P_Glow, Position, particleColor, angle);
            }
            
            // Screen effects for special items
            if (Type == FoodType.MaxTomato)
            {
                level.Displacement.AddBurst(Position, 0.4f, 16f, 64f, 0.4f);
            }
        }

        private IEnumerator CollectRoutine()
        {
            // Scale up and fade out
            float duration = 0.3f;
            float timer = 0f;
            
            while (timer < duration)
            {
                timer += Engine.DeltaTime;
                float progress = timer / duration;
                
                sprite.Scale = Vector2.One * (1f + progress);
                sprite.Color = Color.White * (1f - progress);
                Position.Y -= 30f * Engine.DeltaTime;
                
                yield return null;
            }
            
            RemoveSelf();
        }

        /// <summary>
        /// Spawn food at position with random type
        /// </summary>
        public static KirbyFood SpawnRandomFood(Vector2 position, Scene scene, bool isFalling = true)
        {
            // Weight towards basic foods
            FoodType type;
            float roll = Calc.Random.NextFloat();
            
            if (roll < 0.6f)
            {
                // Basic food (60%)
                type = (FoodType)Calc.Random.Next(0, 8);
            }
            else if (roll < 0.9f)
            {
                // Medium food (30%)
                type = (FoodType)Calc.Random.Next(8, 17);
            }
            else
            {
                // Max tomato (10%)
                type = FoodType.MaxTomato;
            }
            
            var food = new KirbyFood(position, type);
            food.isFalling = isFalling;
            
            if (isFalling)
            {
                food.velocity = new Vector2(Calc.Random.Range(-50f, 50f), Calc.Random.Range(-150f, -50f));
            }
            
            scene.Add(food);
            return food;
        }

        /// <summary>
        /// Spawn multiple food items in burst pattern
        /// </summary>
        public static void SpawnFoodBurst(Vector2 position, Scene scene, int count = 5)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = i * MathHelper.TwoPi / count;
                Vector2 offset = Calc.AngleToVector(angle, 8f);
                
                var food = SpawnRandomFood(position + offset, scene, true);
                food.velocity = Calc.AngleToVector(angle, 100f) + new Vector2(0, -80f);
            }
        }
    }
}
