namespace DesoloZantas.Core.Core.Systems
{
    /// <summary>
    /// Special abilities shared across all Dream Friend characters
    /// Slash, Launch, and Food Generation abilities
    /// </summary>
    public static class DreamFriendSpecialSkills
    {
        // Slash ability particles
        public static ParticleType P_Slash = new ParticleType
        {
            Size = 1f,
            Color = Color.White,
            Color2 = Color.Silver,
            ColorMode = ParticleType.ColorModes.Fade,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.5f,
            LifeMax = 1.0f,
            SpeedMin = 40f,
            SpeedMax = 80f,
            DirectionRange = (float)Math.PI / 4f,
            Acceleration = new Vector2(0f, 20f)
        };

        // Launch ability particles
        public static ParticleType P_Launch = new ParticleType
        {
            Size = 1f,
            Color = Color.Orange,
            Color2 = Color.Yellow,
            ColorMode = ParticleType.ColorModes.Blink,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.8f,
            LifeMax = 1.5f,
            SpeedMin = 30f,
            SpeedMax = 60f,
            DirectionRange = (float)Math.PI * 2f,
            Acceleration = new Vector2(0f, -10f)
        };

        // Food generation particles
        public static ParticleType P_Food = new ParticleType
        {
            Size = 1f,
            Color = Color.LightGreen,
            Color2 = Color.Green,
            ColorMode = ParticleType.ColorModes.Fade,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 1.0f,
            LifeMax = 1.8f,
            SpeedMin = 10f,
            SpeedMax = 25f,
            DirectionRange = (float)Math.PI * 2f,
            Acceleration = new Vector2(0f, 30f)
        };

        /// <summary>
        /// Slash ability - cuts through obstacles and barriers
        /// </summary>
        public static void ExecuteSlashAbility(Entity dreamFriend, global::Celeste.Player player, Vector2 direction, float power = 1.0f)
        {
            dreamFriend.Add(new Coroutine(SlashRoutine(dreamFriend, player, direction, power)));
        }

        private static IEnumerator SlashRoutine(Entity dreamFriend, global::Celeste.Player player, Vector2 direction, float power)
        {
            // Boost player with slash momentum
            player.Speed.Y = Math.Min(player.Speed.Y, -100f * power);
            player.Speed.X += direction.X * 90f * power;
            
            // Create slash effect
            Vector2 slashStart = dreamFriend.Position + direction * 10f;
            Vector2 slashEnd = slashStart + direction * (120f * power);
            
            // Create slash line with multiple segments
            int segments = (int)(8 * power);
            for (int i = 0; i <= segments; i++)
            {
                float t = (float)i / segments;
                Vector2 slashPos = Vector2.Lerp(slashStart, slashEnd, t);
                
                // Emit slash particles
                Level level = dreamFriend.SceneAs<Level>();
                level?.ParticlesFG.Emit(P_Slash, 3, slashPos, Vector2.One * 8f);
                
                // Check for cuttable obstacles
                if (level != null)
                {
                    foreach (var entity in level.Entities)
                    {
                        if (Vector2.Distance(entity.Position, slashPos) < 25f)
                        {
                            if (IsCuttableObstacle(entity))
                            {
                                CutObstacle(entity, level);
                            }
                        }
                    }
                }
                
                yield return 0.02f;
            }
            
            // Final slash explosion
            Level finalLevel = dreamFriend.SceneAs<Level>();
            finalLevel?.ParticlesFG.Emit(P_Slash, 12, slashEnd, Vector2.One * 15f);
            
            // Play slash sound
            Audio.Play("event:/char/madeline/dash_pink", slashEnd);
        }

        /// <summary>
        /// Launch ability - destroys blocks and barriers with explosive force
        /// </summary>
        public static void ExecuteLaunchAbility(Entity dreamFriend, global::Celeste.Player player, Vector2 direction, float power = 1.0f)
        {
            dreamFriend.Add(new Coroutine(LaunchRoutine(dreamFriend, player, direction, power)));
        }

        private static IEnumerator LaunchRoutine(Entity dreamFriend, global::Celeste.Player player, Vector2 direction, float power)
        {
            // Powerful launch boost
            player.Speed.Y = Math.Min(player.Speed.Y, -140f * power);
            player.Speed.X += direction.X * 120f * power;
            
            // Create launch projectile
            Vector2 launchPos = dreamFriend.Position + direction * 15f;
            float distance = 0f;
            float maxDistance = 150f * power;
            
            while (distance < maxDistance)
            {
                distance += 6f;
                launchPos += direction * 6f;
                
                // Emit launch particles
                Level level = dreamFriend.SceneAs<Level>();
                level?.ParticlesFG.Emit(P_Launch, 4, launchPos, Vector2.One * 10f);
                
                // Check for destructible blocks
                if (level != null)
                {
                    foreach (var entity in level.Entities)
                    {
                        if (Vector2.Distance(entity.Position, launchPos) < 30f)
                        {
                            if (IsDestructibleBlock(entity))
                            {
                                DestroyBlock(entity, level, power);
                            }
                        }
                    }
                }
                
                yield return null;
            }
            
            // Launch explosion
            Level finalLevel = dreamFriend.SceneAs<Level>();
            if (finalLevel != null)
            {
                finalLevel.ParticlesFG.Emit(P_Launch, 20, launchPos, Vector2.One * 25f);
                
                // Destroy blocks in explosion radius
                foreach (var entity in finalLevel.Entities)
                {
                    if (Vector2.Distance(entity.Position, launchPos) < 50f * power)
                    {
                        if (IsDestructibleBlock(entity))
                        {
                            DestroyBlock(entity, finalLevel, power);
                        }
                    }
                }
            }
            
            // Play launch explosion sound
            Audio.Play("event:/char/madeline/crystalheart_pulse", launchPos);
        }

        /// <summary>
        /// Food Generation ability - creates food items that restore health/energy
        /// </summary>
        public static void ExecuteFoodGeneration(Entity dreamFriend, global::Celeste.Player player, FoodType foodType = FoodType.Apple)
        {
            dreamFriend.Add(new Coroutine(FoodGenerationRoutine(dreamFriend, player, foodType)));
        }

        public enum FoodType
        {
            Apple,      // Small health restoration
            Cake,       // Medium health restoration
            Maxim,      // Full health restoration
            EnergyDrink // Energy/stamina boost
        }

        private static IEnumerator FoodGenerationRoutine(Entity dreamFriend, global::Celeste.Player player, FoodType foodType)
        {
            Vector2 foodPos = dreamFriend.Position + Vector2.UnitY * -20f;
            Level level = dreamFriend.SceneAs<Level>();
            
            if (level == null) yield break;
            
            // Create food generation effect
            for (float t = 0f; t < 1.0f; t += Engine.DeltaTime)
            {
                level.ParticlesFG.Emit(P_Food, 2, foodPos + Vector2.UnitY * (t * -20f), Vector2.One * 6f);
                yield return null;
            }
            
            // Create food item
            FoodItem food = new FoodItem(foodPos, foodType);
            level.Add(food);
            
            // Food creation effect
            level.ParticlesFG.Emit(P_Food, 15, foodPos, Vector2.One * 12f);
            Audio.Play("event:/char/madeline/crystalheart_pulse", foodPos);
        }

        // Helper methods for obstacle detection
        private static bool IsCuttableObstacle(Entity entity)
        {
            string entityName = entity.GetType().Name;
            return entityName.Contains("Vine") ||
                   entityName.Contains("Rope") ||
                   entityName.Contains("Chain") ||
                   entityName.Contains("Web") ||
                   entityName.Contains("Grass") ||
                   entityName.Contains("Barrier") ||
                   entityName.Contains("Curtain");
        }

        private static bool IsDestructibleBlock(Entity entity)
        {
            string entityName = entity.GetType().Name;
            return entityName.Contains("Block") ||
                   entityName.Contains("Wall") ||
                   entityName.Contains("Brick") ||
                   entityName.Contains("Stone") ||
                   entityName.Contains("Rock") ||
                   entityName.Contains("Crystal") ||
                   entityName.Contains("Ice") ||
                   entityName.Contains("Breakable");
        }

        private static void CutObstacle(Entity entity, Level level)
        {
            // Visual cut effect
            level.ParticlesFG.Emit(P_Slash, 8, entity.Position, Vector2.One * 12f);
            
            // Remove cuttable obstacles
            entity.RemoveSelf();
            
            // Cut sound
            Audio.Play("event:/char/madeline/grab", entity.Position);
        }

        private static void DestroyBlock(Entity entity, Level level, float power)
        {
            // Visual destruction effect
            Color blockColor = GetBlockColor(entity);
            ParticleType destroyParticles = CreateDestroyParticles(blockColor);
            
            level.ParticlesFG.Emit(destroyParticles, (int)(10 * power), entity.Position, Vector2.One * 15f);
            
            // Remove destructible blocks
            entity.RemoveSelf();
            
            // Destruction sound
            Audio.Play("event:/char/madeline/dash_red", entity.Position);
        }

        private static Color GetBlockColor(Entity entity)
        {
            string entityName = entity.GetType().Name.ToLower();
            
            if (entityName.Contains("ice")) return Color.LightBlue;
            if (entityName.Contains("fire") || entityName.Contains("lava")) return Color.OrangeRed;
            if (entityName.Contains("stone") || entityName.Contains("rock")) return Color.Gray;
            if (entityName.Contains("crystal")) return Color.Purple;
            if (entityName.Contains("wood")) return Color.Brown;
            
            return Color.White; // Default
        }

        private static ParticleType CreateDestroyParticles(Color color)
        {
            return new ParticleType
            {
                Size = 1f,
                Color = color,
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Fade,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.8f,
                LifeMax = 1.5f,
                SpeedMin = 30f,
                SpeedMax = 60f,
                DirectionRange = (float)Math.PI * 2f,
                Acceleration = new Vector2(0f, 50f)
            };
        }
    }

    /// <summary>
    /// Food item that restores health/energy when collected
    /// </summary>
    public class FoodItem : Entity
    {
        private DreamFriendSpecialSkills.FoodType foodType;
        private SineWave bobWave;
        private VertexLight light;
        private float lifetime = 15f; // Food expires after 15 seconds
        
        public FoodItem(Vector2 position, DreamFriendSpecialSkills.FoodType type) : base(position)
        {
            foodType = type;
            Collider = new Hitbox(8f, 8f, -4f, -4f);
            
            SetupVisuals();
            SetupBehavior();
        }

        private void SetupVisuals()
        {
            // Bobbing animation
            Add(bobWave = new SineWave(0.5f, 0f));
            bobWave.OnUpdate = delegate(float f)
            {
                Position = Position + Vector2.UnitY * f * 3f;
            };
            
            // Food glow
            Color foodColor = GetFoodColor();
            Add(light = new VertexLight(Vector2.Zero, foodColor, 0.8f, 16, 32));
        }

        private void SetupBehavior()
        {
            Add(new PlayerCollider(OnPlayerCollect));
        }

        private Color GetFoodColor()
        {
            switch (foodType)
            {
                case DreamFriendSpecialSkills.FoodType.Apple:
                    return Color.Red;
                case DreamFriendSpecialSkills.FoodType.Cake:
                    return Color.Pink;
                case DreamFriendSpecialSkills.FoodType.Maxim:
                    return Color.Gold;
                case DreamFriendSpecialSkills.FoodType.EnergyDrink:
                    return Color.Blue;
                default:
                    return Color.White;
            }
        }

        private void OnPlayerCollect(global::Celeste.Player player)
        {
            // Apply food effects
            ApplyFoodEffect(player);
            
            // Collection effect
            Color foodColor = GetFoodColor();
            SceneAs<Level>()?.ParticlesFG.Emit(DreamFriendSpecialSkills.P_Food, 8, Position, Vector2.One * 10f);
            
            // Collection sound
            Audio.Play("event:/char/madeline/crystalheart_pulse", Position);
            
            // Remove food item
            RemoveSelf();
        }

        private void ApplyFoodEffect(global::Celeste.Player player)
        {
            switch (foodType)
            {
                case DreamFriendSpecialSkills.FoodType.Apple:
                    // Small health restoration
                    RestorePlayerHealth(player, 25f);
                    break;
                    
                case DreamFriendSpecialSkills.FoodType.Cake:
                    // Medium health restoration
                    RestorePlayerHealth(player, 50f);
                    break;
                    
                case DreamFriendSpecialSkills.FoodType.Maxim:
                    // Full health restoration
                    RestorePlayerHealth(player, 100f);
                    break;
                    
                case DreamFriendSpecialSkills.FoodType.EnergyDrink:
                    // Energy/stamina boost
                    BoostPlayerEnergy(player);
                    break;
            }
        }

        private void RestorePlayerHealth(global::Celeste.Player player, float amount)
        {
            // In Celeste, we can't directly modify health since it's typically
            // binary (alive/dead), but we can provide other benefits
            
            // Restore stamina
            player.Stamina = Math.Min(player.Stamina + amount, 110f);
            
            // Give temporary invincibility frames
            if (player.StateMachine != null)
            {
                // Could add temporary protection effect here
            }
            
            // Visual health restoration effect
            SceneAs<Level>()?.ParticlesFG.Emit(CreateHealParticles(), 12, player.Position, Vector2.One * 15f);
        }

        private void BoostPlayerEnergy(global::Celeste.Player player)
        {
            // Full stamina restoration
            player.Stamina = 110f;
            
            // Speed boost effect
            player.Speed *= 1.2f;
            
            // Visual energy boost effect
            SceneAs<Level>()?.ParticlesFG.Emit(CreateEnergyParticles(), 15, player.Position, Vector2.One * 20f);
        }

        private ParticleType CreateHealParticles()
        {
            return new ParticleType
            {
                Size = 1f,
                Color = Color.LightGreen,
                Color2 = Color.Green,
                ColorMode = ParticleType.ColorModes.Fade,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 1.0f,
                LifeMax = 1.8f,
                SpeedMin = 20f,
                SpeedMax = 40f,
                DirectionRange = (float)Math.PI * 2f,
                Acceleration = new Vector2(0f, -30f)
            };
        }

        private ParticleType CreateEnergyParticles()
        {
            return new ParticleType
            {
                Size = 1f,
                Color = Color.Blue,
                Color2 = Color.LightBlue,
                ColorMode = ParticleType.ColorModes.Blink,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 1.2f,
                LifeMax = 2.0f,
                SpeedMin = 25f,
                SpeedMax = 50f,
                DirectionRange = (float)Math.PI * 2f,
                Acceleration = new Vector2(0f, -20f)
            };
        }

        public override void Update()
        {
            base.Update();
            
            // Expire food after lifetime
            lifetime -= Engine.DeltaTime;
            if (lifetime <= 0f)
            {
                // Fade out effect
                SceneAs<Level>()?.ParticlesFG.Emit(DreamFriendSpecialSkills.P_Food, 5, Position, Vector2.One * 8f);
                RemoveSelf();
            }
            
            // Fade light as food ages
            if (light != null)
            {
                light.Alpha = Math.Max(0.3f, lifetime / 15f);
            }
        }

        public override void Render()
        {
            base.Render();
            
            // Render food icon based on type
            RenderFoodIcon();
        }

        private void RenderFoodIcon()
        {
            Vector2 iconPos = Position;
            Color foodColor = GetFoodColor();
            
            switch (foodType)
            {
                case DreamFriendSpecialSkills.FoodType.Apple:
                    // Simple apple representation
                    Draw.Circle(iconPos, 4f, foodColor, 8);
                    Draw.Circle(iconPos + Vector2.UnitY * -2f, 2f, Color.Green, 6); // Leaf
                    break;
                    
                case DreamFriendSpecialSkills.FoodType.Cake:
                    // Simple cake representation
                    Draw.Rect(iconPos.X - 4f, iconPos.Y - 2f, 8f, 4f, foodColor);
                    Draw.Rect(iconPos.X - 3f, iconPos.Y - 4f, 6f, 2f, Color.White); // Frosting
                    break;
                    
                case DreamFriendSpecialSkills.FoodType.Maxim:
                    // Golden fruit representation
                    Draw.Circle(iconPos, 5f, foodColor, 10);
                    Draw.Circle(iconPos, 3f, Color.Yellow, 8);
                    break;
                    
                case DreamFriendSpecialSkills.FoodType.EnergyDrink:
                    // Energy drink representation
                    Draw.Rect(iconPos.X - 2f, iconPos.Y - 4f, 4f, 8f, foodColor);
                    Draw.Rect(iconPos.X - 1f, iconPos.Y - 3f, 2f, 6f, Color.LightBlue);
                    break;
            }
        }
    }

    /// <summary>
    /// Extension methods to add special skills to Dream Friend entities
    /// </summary>
    public static class DreamFriendExtensions
    {
        /// <summary>
        /// Add special skills input handling to a Dream Friend entity
        /// </summary>
        public static void AddSpecialSkillsHandling(this Entity dreamFriend, global::Celeste.Player player)
        {
            // This would be called in each Dream Friend's OnPlayer method
            // to add common special skill inputs
            
            // Special skill inputs (combinations)
            if (Input.Grab.Check && Input.Jump.Pressed)
            {
                // Slash ability
                Vector2 slashDirection = new Vector2(GetDreamFriendFacing(dreamFriend), 0f);
                DreamFriendSpecialSkills.ExecuteSlashAbility(dreamFriend, player, slashDirection);
            }
            else if (Input.Dash.Check && Input.Jump.Pressed)
            {
                // Launch ability
                Vector2 launchDirection = new Vector2(GetDreamFriendFacing(dreamFriend), 0f);
                DreamFriendSpecialSkills.ExecuteLaunchAbility(dreamFriend, player, launchDirection);
            }
            else if (Input.Grab.Check && Input.Dash.Check && Input.Jump.Check)
            {
                // Food generation ability
                DreamFriendSpecialSkills.FoodType foodType = GetRandomFoodType();
                DreamFriendSpecialSkills.ExecuteFoodGeneration(dreamFriend, player, foodType);
            }
        }

        private static float GetDreamFriendFacing(Entity dreamFriend)
        {
            // Try to get sprite scale or default to right-facing
            if (dreamFriend.Get<PlayerSprite>() is PlayerSprite sprite)
            {
                return sprite.Scale.X;
            }
            return 1f; // Default right-facing
        }

        private static DreamFriendSpecialSkills.FoodType GetRandomFoodType()
        {
            Random rand = new Random();
            Array foodTypes = Enum.GetValues(typeof(DreamFriendSpecialSkills.FoodType));
            return (DreamFriendSpecialSkills.FoodType)foodTypes.GetValue(rand.Next(foodTypes.Length));
        }
    }
}



