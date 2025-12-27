namespace DesoloZantas.Core.Core.Entities.Kirby
{
    /// <summary>
    /// Projectiles used by Kirby enemies and bosses
    /// </summary>
    [Tracked]
    public class KirbyProjectile : Entity
    {
        public enum ProjectileType
        {
            Star,       // Generic star projectile
            Beam,       // Waddle Doo beam
            Fire,       // Fire breath/ball
            Ice,        // Ice block/breath
            Lightning,  // Spark/electric
            Cutter,     // Boomerang cutter
            Bomb,       // Explosive bomb
            Mirror,     // Reflective mirror shot
            Root,       // Ground root attack
            Rain,       // Rain drop
            Apple,      // Falling apple
            Custom
        }

        public ProjectileType Type { get; private set; }
        public Entity Owner { get; private set; }
        public int Damage { get; set; } = 1;
        public bool CanBeReflected { get; set; } = true;
        public bool DestroyOnCollision { get; set; } = true;
        
        private Sprite sprite;
        private Vector2 velocity;
        private float lifetime;
        private float maxLifetime = 5f;
        private float gravity;
        private bool isReflected;
        
        // Explosion properties for bombs
        private float explosionRadius = 32f;
        private float explosionTimer;
        private bool hasExploded;

        public KirbyProjectile(Vector2 position, Vector2 velocity, ProjectileType type, Entity owner = null) 
            : base(position)
        {
            Type = type;
            Owner = owner;
            this.velocity = velocity;
            
            SetupProjectile();
        }

        private void SetupProjectile()
        {
            Collider = new Hitbox(8f, 8f, -4f, -4f);
            
            switch (Type)
            {
                case ProjectileType.Star:
                    maxLifetime = 3f;
                    Damage = 1;
                    break;
                    
                case ProjectileType.Beam:
                    maxLifetime = 2f;
                    Damage = 1;
                    Collider = new Hitbox(16f, 4f, -8f, -2f);
                    break;
                    
                case ProjectileType.Fire:
                    maxLifetime = 1.5f;
                    Damage = 2;
                    gravity = 50f;
                    break;
                    
                case ProjectileType.Ice:
                    maxLifetime = 2f;
                    Damage = 2;
                    Collider = new Hitbox(12f, 12f, -6f, -6f);
                    break;
                    
                case ProjectileType.Lightning:
                    maxLifetime = 0.5f;
                    Damage = 3;
                    Collider = new Hitbox(4f, 32f, -2f, -16f);
                    CanBeReflected = false;
                    break;
                    
                case ProjectileType.Cutter:
                    maxLifetime = 3f;
                    Damage = 2;
                    DestroyOnCollision = false; // Boomerangs pass through
                    break;
                    
                case ProjectileType.Bomb:
                    maxLifetime = 3f;
                    Damage = 3;
                    gravity = 150f;
                    explosionTimer = 2f;
                    break;
                    
                case ProjectileType.Mirror:
                    maxLifetime = 2f;
                    Damage = 1;
                    CanBeReflected = false; // Mirror shots don't reflect
                    break;
                    
                case ProjectileType.Root:
                    maxLifetime = 1f;
                    Damage = 2;
                    CanBeReflected = false;
                    break;
                    
                case ProjectileType.Rain:
                    maxLifetime = 3f;
                    Damage = 1;
                    Collider = new Hitbox(4f, 8f, -2f, -4f);
                    break;
                    
                case ProjectileType.Apple:
                    maxLifetime = 5f;
                    Damage = 1;
                    gravity = 200f;
                    break;
            }
            
            Depth = Depths.Enemy - 10; // Render in front of enemies
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            SetupSprite();
            
            // Add player collision
            Add(new PlayerCollider(OnPlayerCollision));
        }

        private void SetupSprite()
        {
            string spritePath = GetSpritePathForType();
            
            Add(sprite = new Sprite(GFX.Game, spritePath));
            sprite.AddLoop("idle", "", 0.1f);
            sprite.CenterOrigin();
            sprite.Play("idle");
            
            // Set sprite color based on type
            sprite.Color = GetProjectileColor();
        }

        private string GetSpritePathForType()
        {
            return Type switch
            {
                ProjectileType.Star => "projectiles/kirby/star/",
                ProjectileType.Beam => "projectiles/kirby/beam/",
                ProjectileType.Fire => "projectiles/kirby/fire/",
                ProjectileType.Ice => "projectiles/kirby/ice/",
                ProjectileType.Lightning => "projectiles/kirby/lightning/",
                ProjectileType.Cutter => "projectiles/kirby/cutter/",
                ProjectileType.Bomb => "projectiles/kirby/bomb/",
                ProjectileType.Mirror => "projectiles/kirby/mirror/",
                ProjectileType.Root => "projectiles/kirby/root/",
                ProjectileType.Rain => "projectiles/kirby/rain/",
                ProjectileType.Apple => "projectiles/kirby/apple/",
                _ => "projectiles/kirby/star/"
            };
        }

        private Color GetProjectileColor()
        {
            return Type switch
            {
                ProjectileType.Star => Color.Yellow,
                ProjectileType.Beam => Color.Gold,
                ProjectileType.Fire => Color.OrangeRed,
                ProjectileType.Ice => Color.LightBlue,
                ProjectileType.Lightning => Color.Yellow,
                ProjectileType.Cutter => Color.Silver,
                ProjectileType.Bomb => Color.DarkOrange,
                ProjectileType.Mirror => Color.Purple,
                ProjectileType.Root => Color.Brown,
                ProjectileType.Rain => Color.Blue,
                ProjectileType.Apple => Color.Red,
                _ => Color.White
            };
        }

        public override void Update()
        {
            base.Update();
            
            // Apply gravity
            if (gravity > 0)
            {
                velocity.Y += gravity * Engine.DeltaTime;
            }
            
            // Move
            Position += velocity * Engine.DeltaTime;
            
            // Rotate based on movement for some types
            if (Type == ProjectileType.Star || Type == ProjectileType.Cutter)
            {
                sprite.Rotation += Engine.DeltaTime * 10f;
            }
            else if (velocity != Vector2.Zero)
            {
                sprite.Rotation = (float)Math.Atan2(velocity.Y, velocity.X);
            }
            
            // Update lifetime
            lifetime += Engine.DeltaTime;
            if (lifetime >= maxLifetime)
            {
                Destroy();
                return;
            }
            
            // Bomb timer
            if (Type == ProjectileType.Bomb)
            {
                explosionTimer -= Engine.DeltaTime;
                if (explosionTimer <= 0 && !hasExploded)
                {
                    Explode();
                }
            }
            
            // Boomerang return for cutter
            if (Type == ProjectileType.Cutter && lifetime > maxLifetime / 2)
            {
                // Return to owner
                if (Owner != null && Owner.Scene != null)
                {
                    Vector2 toOwner = (Owner.Position - Position).SafeNormalize();
                    velocity = Vector2.Lerp(velocity, toOwner * velocity.Length(), Engine.DeltaTime * 3f);
                    
                    // Check if returned
                    if (Vector2.Distance(Position, Owner.Position) < 16f)
                    {
                        Destroy();
                    }
                }
            }
            
            // Check for wall collision
            if (CollideCheck<Solid>())
            {
                OnWallCollision();
            }
        }

        private void OnPlayerCollision(global::Celeste.Player player)
        {
            // Don't hurt player if reflected
            if (isReflected && Owner is KirbyActorBase)
            {
                // Damage the owner instead
                (Owner as KirbyActorBase)?.OnHit(Damage, (Owner.Position - Position).SafeNormalize() * 50f);
                Destroy();
                return;
            }
            
            // Damage player
            player.Die((player.Position - Position).SafeNormalize());
            
            if (DestroyOnCollision)
            {
                Destroy();
            }
        }

        private void OnWallCollision()
        {
            if (Type == ProjectileType.Bomb && !hasExploded)
            {
                Explode();
                return;
            }
            
            if (DestroyOnCollision)
            {
                Destroy();
            }
            else if (Type == ProjectileType.Cutter)
            {
                // Reflect cutter
                velocity.X *= -1;
            }
        }

        private void Explode()
        {
            if (hasExploded) return;
            hasExploded = true;
            
            var level = Scene as Level;
            if (level != null)
            {
                // Explosion visual
                level.Displacement.AddBurst(Position, 0.4f, 16f, explosionRadius, 0.4f);
                
                for (int i = 0; i < 12; i++)
                {
                    float angle = i * MathHelper.TwoPi / 12f;
                    level.Particles.Emit(Refill.P_Shatter, Position, Color.Orange, angle);
                }
            }
            
            Audio.Play("event:/game/general/wall_break_stone", Position);
            
            // Damage player if in radius
            var player = Scene.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null && Vector2.Distance(Position, player.Position) < explosionRadius)
            {
                player.Die((player.Position - Position).SafeNormalize());
            }
            
            // Damage enemies if reflected
            if (isReflected)
            {
                foreach (var enemy in Scene.Tracker.GetEntities<KirbyActorBase>())
                {
                    if (Vector2.Distance(Position, enemy.Position) < explosionRadius)
                    {
                        (enemy as KirbyActorBase)?.OnHit(Damage * 2, (enemy.Position - Position).SafeNormalize() * 100f);
                    }
                }
            }
            
            Destroy();
        }

        /// <summary>
        /// Reflect this projectile back
        /// </summary>
        public void Reflect(Vector2 direction)
        {
            if (!CanBeReflected) return;
            
            isReflected = true;
            velocity = direction.SafeNormalize() * velocity.Length() * 1.5f;
            sprite.Color = Color.White; // Change color to indicate reflected
            
            Audio.Play("event:/game/general/thing_booped", Position);
        }

        private void Destroy()
        {
            // Destruction particles
            var level = Scene as Level;
            if (level != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    level.Particles.Emit(Refill.P_Shatter, Position, GetProjectileColor());
                }
            }
            
            RemoveSelf();
        }
    }
}
