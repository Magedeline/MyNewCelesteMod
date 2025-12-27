namespace DesoloZantas.Core.Core.Content
{
    /// <summary>
    /// Asriel - Hope boosts with compassion protection aura
    /// </summary>
    public class AsrielAbility : CharacterAbility
    {
        private float hopeAuraRadius = 100f;
        private float compassionBoostPower = 1.3f;
        private float protectionTime = 0f;
        
        public AsrielAbility() 
            : base("asriel", "Hope Aura", "Hope boosts with compassion protection for safer navigation")
        {
        }
        
        protected override void OnActivate()
        {
            protectionTime = 0f;
        }
        
        protected override void OnDeactivate()
        {
        }
        
        protected override void OnUpdate()
        {
            if (player == null) return;
            
            // Update protection timer
            if (protectionTime > 0f)
            {
                protectionTime -= Engine.DeltaTime;
            }
            
            // Provide hope boost when player is struggling
            if (IsPlayerInDanger() && protectionTime <= 0f)
            {
                ActivateHopeBoost();
            }
        }
        
        private bool IsPlayerInDanger()
        {
            // Check if player is falling fast or near spikes
            return player.Speed.Y > 100f || level?.CollideCheck<Spikes>(player.Position + Vector2.UnitY * 20f) == true;
        }
        
        private void ActivateHopeBoost()
        {
            protectionTime = 2f; // 2-second protection cooldown
            
            // Reduce fall speed with hope
            if (player.Speed.Y > 0)
            {
                player.Speed.Y *= 0.6f;
            }
            
            // Add slight dash restore for hope
            if (player.Dashes < 1)
            {
                player.Dashes = 1;
            }
            
            PlaySound("event:/char/madeline/heartgem_red_get");
            CreateCompassionAura();
        }
        
        private void CreateCompassionAura()
        {
            // Create visual aura effect
            CreateParticles(ParticleTypes.Dust, 8, player.Position, Vector2.One * hopeAuraRadius);
            level?.Displacement?.AddBurst(player.Position, 0.3f, 12f, hopeAuraRadius, 0.4f, null, null);
        }
        
        public override void OnPlayerDash(Vector2 dashDirection)
        {
            // Hope-enhanced dashes
            if (IsActive)
            {
                player.Speed *= compassionBoostPower;
                CreateParticles(ParticleTypes.Dust, 4, player.Position, Vector2.One * 6f);
            }
        }
    }
    
    /// <summary>
    /// Frisk - Determination-powered movement boosts
    /// </summary>
    public class FriskAbility : MovementAssistAbility
    {
        private float determinationMeter = 100f;
        private float maxDetermination = 100f;
        private bool determinationActive = false;
        
        public FriskAbility() 
            : base("frisk", "Determination", "Determination-powered movement boosts and persistence", 1.25f, false, 0)
        {
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            determinationMeter = maxDetermination;
        }
        
        protected override void OnDeactivate()
        {
            determinationActive = false;
        }
        
        protected override void OnUpdate()
        {
            if (player == null) return;
            
            // Restore determination slowly when on safe ground
            if (player.OnGround() && determinationMeter < maxDetermination)
            {
                determinationMeter += Engine.DeltaTime * 20f;
            }
            
            // Activate determination when player faces adversity
            if (ShouldActivateDetermination() && determinationMeter > 30f)
            {
                ActivateDetermination();
            }
        }
        
        private bool ShouldActivateDetermination()
        {
            return !determinationActive && 
                   (player.Speed.Y > 120f || // Falling fast
                    player.Dashes == 0 || // No dashes
                    IsPlayerStuck()); // Stuck in difficult section
        }
        
        private bool IsPlayerStuck()
        {
            // Simple stuck detection
            return Math.Abs(player.Speed.X) < 5f && Math.Abs(player.Speed.Y) < 5f && !player.OnGround();
        }
        
        private void ActivateDetermination()
        {
            determinationActive = true;
            determinationMeter -= 30f;
            
            // Determination effects
            player.Dashes = Math.Max(player.Dashes, 1); // Always have at least one dash
            
            PlaySound("event:/char/madeline/jump");
            CreateDeterminationEffect();
            
            // Deactivate after short duration
            var entity = new Entity();
            entity.Add(new Coroutine(DeactivateDeterminationAfterDelay(1.5f)));
            level?.Add(entity);
        }
        
        private void CreateDeterminationEffect()
        {
            // Red determination particles
            CreateParticles(ParticleTypes.Dust, 10, player.Position, Vector2.One * 12f);
            level?.Displacement?.AddBurst(player.Position, 0.2f, 8f, 40f, 0.3f, null, null);
        }
        
        private IEnumerator DeactivateDeterminationAfterDelay(float delay)
        {
            yield return delay;
            determinationActive = false;
        }
        
        protected override void OnDashBoost(Vector2 dashDirection)
        {
            if (determinationActive)
            {
                // Extra determination dash power
                player.Speed += dashDirection * 30f;
                CreateParticles(ParticleTypes.Dust, 6, player.Position, Vector2.One * 8f);
            }
        }
    }
    
    /// <summary>
    /// Charlo - Celeste mod crossover abilities
    /// </summary>
    public class CharloAbility : TeleportationAbility
    {
        private float crossoverRange = 80f;
        private List<Vector2> checkpoints = new List<Vector2>();
        
        public CharloAbility() 
            : base("charlo", "Crossover Assist", "Celeste mod crossover abilities for navigation", 120f, 2.5f)
        {
        }
        
        protected override void OnActivate()
        {
            checkpoints.Clear();
            // Record safe positions as checkpoints
            if (player != null && player.OnGround())
            {
                checkpoints.Add(player.Position);
            }
        }
        
        protected override void OnDeactivate()
        {
            checkpoints.Clear();
        }
        
        protected override void OnUpdate()
        {
            if (player == null) return;
            
            // Record new checkpoint when player reaches safe ground
            if (player.OnGround() && IsPositionSafe(player.Position))
            {
                RecordCheckpoint(player.Position);
            }
            
            // Offer crossover assist when player is in trouble
            if (IsPlayerInTrouble() && Input.Grab.Pressed && checkpoints.Count > 0)
            {
                ReturnToLastCheckpoint();
            }
        }
        
        private bool IsPositionSafe(Vector2 position)
        {
            // Check if position is away from hazards
            return !level?.CollideCheck<Spikes>(position + Vector2.UnitY * 16f) == true;
        }
        
        private void RecordCheckpoint(Vector2 position)
        {
            // Only record if it's significantly different from last checkpoint
            if (checkpoints.Count == 0 || Vector2.Distance(checkpoints[checkpoints.Count - 1], position) > 50f)
            {
                checkpoints.Add(position);
                
                // Keep only last 3 checkpoints
                if (checkpoints.Count > 3)
                {
                    checkpoints.RemoveAt(0);
                }
                
                PlaySound("event:/ui/game/memorial_text_in");
            }
        }
        
        private bool IsPlayerInTrouble()
        {
            return player.Speed.Y > 100f || // Falling fast
                   !player.OnGround() && player.Dashes == 0; // No recovery options
        }
        
        private void ReturnToLastCheckpoint()
        {
            if (CanTeleport() && checkpoints.Count > 0)
            {
                Vector2 checkpoint = checkpoints[checkpoints.Count - 1];
                PerformTeleport(checkpoint);
                PlaySound("event:/char/badeline/disappear");
            }
        }
        
        protected override void OnTeleportComplete(Vector2 position)
        {
            base.OnTeleportComplete(position);
            // Crossover-specific effects
            level?.Displacement?.AddBurst(position, 0.3f, 10f, 50f, 0.4f, null, null);
        }
    }
    
    /// <summary>
    /// Clover - Justice-themed precision movement
    /// </summary>
    public class CloverAbility : MovementAssistAbility
    {
        private float precisionRadius = 40f;
        private bool justiceMode = false;
        private float justiceModeTime = 0f;
        
        public CloverAbility() 
            : base("clover", "Justice Precision", "Justice-themed precision movement and accurate dashing", 1.1f, false, 0)
        {
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            justiceMode = false;
        }
        
        protected override void OnDeactivate()
        {
            justiceMode = false;
        }
        
        protected override void OnUpdate()
        {
            if (player == null) return;
            
            // Update justice mode timer
            if (justiceMode && justiceModeTime > 0f)
            {
                justiceModeTime -= Engine.DeltaTime;
                if (justiceModeTime <= 0f)
                {
                    justiceMode = false;
                }
            }
            
            // Activate justice mode for precise movements
            if (!justiceMode && RequiresPrecision() && Input.Dash.Pressed)
            {
                ActivateJusticeMode();
            }
        }
        
        private bool RequiresPrecision()
        {
            // Check if there are narrow passages or precise jumps needed
            if (level == null) return false;
            
            // Look for tight spaces around player
            Vector2[] checkDirections = { Vector2.UnitX, -Vector2.UnitX, Vector2.UnitY, -Vector2.UnitY };
            
            int solidCount = 0;
            foreach (var dir in checkDirections)
            {
                if (level.CollideCheck<Solid>(player.Position + dir * precisionRadius))
                {
                    solidCount++;
                }
            }
            
            return solidCount >= 2; // Surrounded by walls, need precision
        }
        
        private void ActivateJusticeMode()
        {
            justiceMode = true;
            justiceModeTime = 3f; // 3-second precision mode
            
            PlaySound("event:/char/madeline/dash");
            CreateJusticeEffect();
        }
        
        private void CreateJusticeEffect()
        {
            // Yellow justice particles
            CreateParticles(ParticleTypes.Dust, 8, player.Position, Vector2.One * 10f);
            
            // Visual indicator for precision mode
            level?.Displacement?.AddBurst(player.Position, 0.2f, 6f, 30f, 0.2f, null, null);
        }
        
        protected override void OnDashBoost(Vector2 dashDirection)
        {
            if (justiceMode)
            {
                // Precise, controlled dash with less variance
                Vector2 preciseDirection = dashDirection.SafeNormalize();
                player.Speed = preciseDirection * player.Speed.Length(); // Normalize direction for precision
                
                CreateParticles(ParticleTypes.Dust, 4, player.Position, Vector2.One * 6f);
            }
        }
        
        public override void OnPlayerDash(Vector2 dashDirection)
        {
            base.OnPlayerDash(dashDirection);
            
            if (justiceMode)
            {
                // Justice dashes are more controlled
                player.Speed *= 0.9f; // Slightly slower but more precise
            }
        }
    }
    
    /// <summary>
    /// Ness - PSI teleportation and platform abilities
    /// </summary>
    public class NessAbility : TeleportationAbility
    {
        private float psiPower = 100f;
        private float maxPsiPower = 100f;
        private List<Entity> psiPlatforms = new List<Entity>();
        
        public NessAbility() 
            : base("ness", "PSI Powers", "PSI teleportation and platform creation abilities", 100f, 4.0f)
        {
        }
        
        protected override void OnActivate()
        {
            psiPower = maxPsiPower;
        }
        
        protected override void OnDeactivate()
        {
            // Clean up PSI platforms
            foreach (var platform in psiPlatforms)
            {
                platform?.RemoveSelf();
            }
            psiPlatforms.Clear();
        }
        
        protected override void OnUpdate()
        {
            if (player == null) return;
            
            // Restore PSI power slowly
            if (psiPower < maxPsiPower)
            {
                psiPower += Engine.DeltaTime * 15f;
            }
            
            // PSI Platform creation
            if (Input.Jump.Pressed && !player.OnGround() && psiPower >= 25f)
            {
                CreatePSIPlatform();
            }
            
            // PSI Teleport
            if (Input.Grab.Pressed && psiPower >= 40f && CanTeleport())
            {
                PerformPSITeleport();
            }
        }
        
        private void CreatePSIPlatform()
        {
            psiPower -= 25f;
            
            Vector2 platformPos = player.Position + Vector2.UnitY * 30f;
            var platform = new Solid(platformPos, 48f, 8f, false);
            platform.Depth = 1000;
            
            level?.Add(platform);
            psiPlatforms.Add(platform);
            
            PlaySound("event:/game/general/thing_booped");
            CreateParticles(ParticleTypes.Dust, 6, platformPos, Vector2.One * 8f);
            
            // Auto-remove PSI platform after 6 seconds
            var entity = new Entity();
            entity.Add(new Coroutine(RemovePSIPlatformAfterDelay(platform, 6f)));
            level?.Add(entity);
        }
        
        private void PerformPSITeleport()
        {
            psiPower -= 40f;
            
            // Find safe teleport location
            Vector2? teleportTarget = FindSafeTeleportLocation();
            
            if (teleportTarget.HasValue)
            {
                PerformTeleport(teleportTarget.Value);
                PlaySound("event:/char/badeline/disappear");
            }
        }
        
        private Vector2? FindSafeTeleportLocation()
        {
            // Look for safe ground positions
            for (int distance = 50; distance <= teleportRange; distance += 25)
            {
                for (int angle = 0; angle < 360; angle += 30)
                {
                    float rad = angle * (float)Math.PI / 180f;
                    Vector2 testPos = player.Position + new Vector2(
                        (float)Math.Cos(rad) * distance,
                        (float)Math.Sin(rad) * distance
                    );
                    
                    // Check if position is safe
                    if (!level?.CollideCheck<Solid>(testPos) == true &&
                        level?.CollideCheck<Solid>(testPos + Vector2.UnitY * 16f) == true)
                    {
                        return testPos;
                    }
                }
            }
            
            return null;
        }
        
        private IEnumerator RemovePSIPlatformAfterDelay(Entity platform, float delay)
        {
            yield return delay;
            platform?.RemoveSelf();
            psiPlatforms.Remove(platform);
        }
        
        protected override void OnTeleportComplete(Vector2 position)
        {
            base.OnTeleportComplete(position);
            // PSI-specific teleport effects
            CreateParticles(ParticleTypes.Dust, 12, position, Vector2.One * 16f);
        }
    }
}



