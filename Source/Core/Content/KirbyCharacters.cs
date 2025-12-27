namespace DesoloZantas.Core.Core.Content
{
    /// <summary>
    /// King DDD - Powerful hammer boosts and ground shockwaves
    /// </summary>
    public class KingDDDAbility : MovementAssistAbility
    {
        private float shockwaveRadius = 80f;
        
        public KingDDDAbility() 
            : base("king_ddd", "Hammer Boost", "Powerful hammer boosts and ground shockwaves for enhanced movement", 1.3f, false, 0)
        {
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
        }
        
        protected override void OnDeactivate()
        {
        }
        
        protected override void OnUpdate()
        {
        }
        
        public override void OnPlayerLand()
        {
            if (IsActive && player != null)
            {
                // Create ground shockwave for movement assistance
                CreateShockwave();
                PlaySound("event:/char/badeline/boss_reappear");
            }
        }
        
        protected override void OnDashBoost(Vector2 dashDirection)
        {
            // Add hammer trail effect
            CreateParticles(ParticleTypes.Dust, 6, player.Position, Vector2.One * 8f);
        }
        
        private void CreateShockwave()
        {
            if (level == null) return;
            
            // Create visual shockwave effect
            level.Displacement?.AddBurst(player.Position, 0.4f, 16f, shockwaveRadius, 0.5f, null, null);
            
            // Destroy nearby dash blocks to assist movement
            foreach (var entity in level.Entities.FindAll<Solid>())
            {
                if (Vector2.Distance(entity.Position, player.Position) <= shockwaveRadius)
                {
                    // Check if it's a breakable block and break it
                    if (entity.GetType().Name.Contains("DashBlock"))
                    {
                        entity.RemoveSelf();
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Meta Knight - Enhanced sword dashes with aerial assistance
    /// </summary>
    public class MetaKnightAbility : MovementAssistAbility
    {
        private float swordDashRange = 120f;
        private int airDashCount = 0;
        
        public MetaKnightAbility() 
            : base("meta_knight", "Sword Dash", "Enhanced sword dashes with aerial movement assistance", 1.4f, true, 1)
        {
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            airDashCount = 0;
        }
        
        protected override void OnDeactivate()
        {
        }
        
        protected override void OnUpdate()
        {
            // Reset air dash count when on ground
            if (player != null && player.OnGround())
            {
                airDashCount = 0;
            }
        }
        
        protected override void OnDashBoost(Vector2 dashDirection)
        {
            // Sword dash with extended range
            if (!player.OnGround() && airDashCount < 2)
            {
                airDashCount++;
                player.Speed += dashDirection * 50f; // Extra aerial momentum
                CreateSwordTrail(dashDirection);
            }
        }
        
        private void CreateSwordTrail(Vector2 direction)
        {
            // Create sword trail particles
            for (int i = 0; i < 5; i++)
            {
                Vector2 trailPos = player.Position - direction * (i * 8f);
                CreateParticles(ParticleTypes.Dust, 2, trailPos, Vector2.One * 4f);
            }
        }
    }
    
    /// <summary>
    /// Bandana Waddle Dee - Spear platforms for enhanced jumping
    /// </summary>
    public class BandanaWaddleDeeAbility : PlatformCreationAbility
    {
        private float spearPlatformWidth = 32f;
        private float spearPlatformHeight = 8f;
        
        public BandanaWaddleDeeAbility() 
            : base("bandana_waddle_dee", "Spear Platform", "Creates temporary spear platforms for enhanced jumping", 8.0f)
        {
        }
        
        protected override void OnActivate()
        {
        }
        
        protected override void OnDeactivate()
        {
            base.OnDeactivate();
        }
        
        protected override void OnUpdate()
        {
            // Create spear platform when player is falling and needs assistance
            if (player != null && player.Speed.Y > 50f && !player.OnGround() && Input.Jump.Pressed)
            {
                CreateSpearPlatform();
            }
        }
        
        private void CreateSpearPlatform()
        {
            Vector2 platformPos = player.Position + Vector2.UnitY * 40f; // Below player
            var platform = CreateTemporaryPlatform(platformPos, new Vector2(spearPlatformWidth, spearPlatformHeight));
            
            if (platform != null)
            {
                createdPlatforms.Add(platform);
                PlaySound("event:/game/general/thing_booped");

                // Auto-remove platform after duration
                platform.Add(new Coroutine(RemovePlatformAfterDelay(platform, platformDuration)));
            }
        }
        
        protected override Entity CreateTemporaryPlatform(Vector2 position, Vector2 size)
        {
            // Create a simple solid platform
            var platform = new Solid(position, size.X, size.Y, false);
            platform.Depth = 1000;
            level?.Add(platform);
            return platform;
        }
        
        private IEnumerator RemovePlatformAfterDelay(Entity platform, float delay)
        {
            yield return delay;
            platform?.RemoveSelf();
            createdPlatforms.Remove(platform);
        }
    }
    
    /// <summary>
    /// Magolor - Portal-based movement assistance
    /// </summary>
    public class MagolorAbility : TeleportationAbility
    {
        private Vector2? portalTarget;
        private float portalDetectionRange = 100f;
        
        public MagolorAbility() 
            : base("magolor", "Portal Movement", "Portal-based movement assistance for navigation", 150f, 3.0f)
        {
        }
        
        protected override void OnActivate()
        {
            portalTarget = null;
        }
        
        protected override void OnDeactivate()
        {
            portalTarget = null;
        }
        
        protected override void OnUpdate()
        {
            // Look for teleport opportunities when player is stuck or falling
            if (player != null && (player.Speed.Y > 80f || IsPlayerStuck()))
            {
                FindPortalTarget();
                
                if (portalTarget.HasValue && Input.Dash.Pressed)
                {
                    PerformTeleport(portalTarget.Value);
                    portalTarget = null;
                }
            }
        }
        
        private bool IsPlayerStuck()
        {
            // Check if player has been in same general area for too long
            return Math.Abs(player.Speed.X) < 10f && Math.Abs(player.Speed.Y) < 10f;
        }
        
        private void FindPortalTarget()
        {
            if (level == null) return;
            
            // Find safe ground position within teleport range
            for (int angle = 0; angle < 360; angle += 45)
            {
                float rad = angle * (float)Math.PI / 180f;
                Vector2 testPos = player.Position + new Vector2(
                    (float)Math.Cos(rad) * teleportRange,
                    (float)Math.Sin(rad) * teleportRange
                );
                
                // Check if position is safe (not in solid, has ground below)
                if (!level.CollideCheck<Solid>(testPos) && 
                    level.CollideCheck<Solid>(testPos + Vector2.UnitY * 16f))
                {
                    portalTarget = testPos;
                    break;
                }
            }
        }
        
        protected override void OnTeleportComplete(Vector2 position)
        {
            base.OnTeleportComplete(position);
            // Add portal-specific effects
            CreateParticles(ParticleTypes.Dust, 12, position, Vector2.One * 16f);
        }
    }
    
    /// <summary>
    /// Kirby Classic - Original Game Boy puff abilities with fall assistance
    /// </summary>
    public class KirbyClassicAbility : MovementAssistAbility
    {
        private float puffPower = 1.2f;
        private int maxPuffs = 6;
        private int currentPuffs = 0;
        
        public KirbyClassicAbility() 
            : base("kirby_classic", "Puff Assist", "Classic puff abilities with fall damage protection", 1.1f, false, 0)
        {
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            currentPuffs = 0;
        }
        
        protected override void OnDeactivate()
        {
        }
        
        protected override void OnUpdate()
        {
            // Reset puffs when on ground
            if (player != null && player.OnGround())
            {
                currentPuffs = 0;
            }
            
            // Provide puff assistance when falling
            if (player != null && player.Speed.Y > 0 && !player.OnGround() && 
                Input.Jump.Pressed && currentPuffs < maxPuffs)
            {
                PerformPuff();
            }
        }
        
        private void PerformPuff()
        {
            currentPuffs++;
            
            // Reduce fall speed
            player.Speed.Y *= 0.7f;
            
            // Add slight upward boost
            player.Speed.Y -= 20f;
            
            PlaySound("event:/char/madeline/jump");
            CreateParticles(ParticleTypes.Dust, 4, player.Position, Vector2.One * 8f);
        }
        
        public override void OnPlayerLand()
        {
            // Reduce fall damage effect
            if (currentPuffs > 0)
            {
                // Player used puffs, so landing should be softer
                CreateParticles(ParticleTypes.Dust, 3, player.Position, Vector2.One * 6f);
            }
        }
    }
    
    /// <summary>
    /// Gooey - Enhanced with tongue wall grab mechanics
    /// </summary>
    public class GooeyAbility : CharacterAbility
    {
        private bool tongueGrabActive = false;
        private float tongueRange = 60f;
        private Vector2 tongueTarget;
        
        public GooeyAbility() 
            : base("gooey", "Tongue Grab", "Enhanced wall grab mechanics with tongue assistance")
        {
        }
        
        protected override void OnActivate()
        {
        }
        
        protected override void OnDeactivate()
        {
            tongueGrabActive = false;
        }
        
        protected override void OnUpdate()
        {
            if (player == null) return;
            
            // Enhanced wall grab detection
            if (Input.Grab.Check && !player.OnGround())
            {
                CheckTongueGrab();
            }
            else
            {
                tongueGrabActive = false;
            }
        }
        
        private void CheckTongueGrab()
        {
            // Check for walls within tongue range
            for (int angle = -90; angle <= 90; angle += 15)
            {
                float rad = angle * (float)Math.PI / 180f;
                Vector2 checkPos = player.Position + new Vector2(
                    (float)Math.Cos(rad) * tongueRange,
                    (float)Math.Sin(rad) * tongueRange
                );
                
                if (level?.CollideCheck<Solid>(checkPos) == true)
                {
                    // Found wall within tongue range
                    if (!tongueGrabActive)
                    {
                        tongueGrabActive = true;
                        tongueTarget = checkPos;
                        PlaySound("event:/char/madeline/grabwall");
                        
                        // Reduce fall speed when tongue grabs
                        player.Speed.Y *= 0.5f;
                    }
                    break;
                }
            }
        }
        
        public override void OnPlayerWallGrab()
        {
            if (tongueGrabActive)
            {
                // Enhanced wall grab with tongue
                player.Speed.Y = Math.Min(player.Speed.Y, 20f); // Slower wall slide
                CreateParticles(ParticleTypes.Dust, 2, player.Position, Vector2.One * 4f);
            }
        }
    }
}



