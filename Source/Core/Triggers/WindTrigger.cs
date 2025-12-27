namespace DesoloZantas.Core.Core.Triggers
{
    /// <summary>
    /// Custom wind trigger that affects both the player (including Kirby) and stylegrounds with wind effects
    /// Provides configurable wind direction, strength, and visual effects
    /// </summary>
    [CustomEntity("Ingeste/WindTrigger")]
    public class WindTrigger : Trigger
    {
        #region Fields
        private Vector2 windDirection;
        private float windStrength;
        private bool affectPlayer;
        private bool affectStylegrounds;
        private bool triggerOnce;
        private bool hasTriggered;
        private float windDuration;
        private bool particleEffects;
        private bool soundEffects;
        private Color windColor;
        private WindType windType;
        private float gustFrequency;
        private float gustIntensity;
        
        // Runtime state
        private Level level;
        private bool isActive;
        private float activeTime;
        private Coroutine windCoroutine;
        private SoundSource windSound;
        private readonly ParticleType windParticles;
        private float nextGustTime;
        #endregion

        #region Enums
        public enum WindType
        {
            Constant,      // Steady wind force
            Gusty,         // Variable wind with gusts
            Swirling,      // Rotating/circular wind patterns
            Updraft,       // Strong upward wind current
            Crosswind      // Horizontal sweeping wind
        }
        #endregion

        #region Constructor
        public WindTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            // Read wind direction (degrees, 0 = right, 90 = up, 180 = left, 270 = down)
            float directionDegrees = data.Float("direction", 0f);
            windDirection = Calc.AngleToVector(MathHelper.ToRadians(directionDegrees), 1f);
            
            // Wind parameters
            windStrength = data.Float("strength", 100f);
            affectPlayer = data.Bool("affectPlayer", true);
            affectStylegrounds = data.Bool("affectStylegrounds", true);
            triggerOnce = data.Bool("triggerOnce", false);
            windDuration = data.Float("duration", 0f); // 0 = infinite while in trigger
            particleEffects = data.Bool("particleEffects", true);
            soundEffects = data.Bool("soundEffects", true);
            
            // Wind appearance
            windColor = data.HexColor("windColor", Color.LightBlue);
            string windTypeStr = data.Attr("windType", "Constant");
            if (!Enum.TryParse(windTypeStr, true, out windType))
            {
                windType = WindType.Constant;
            }
            
            // Gust parameters
            gustFrequency = data.Float("gustFrequency", 2f); // gusts per second
            gustIntensity = data.Float("gustIntensity", 1.5f); // multiplier for gust strength
            
            // Initialize particle system
            windParticles = CreateWindParticleType();
            
            // Initialize audio
            if (soundEffects)
            {
                windSound = new SoundSource();
                Add(windSound);
            }
            
            IngesteLogger.Info($"WindTrigger created: direction={directionDegrees}�, strength={windStrength}, type={windType}");
        }
        #endregion

        #region Particle System
        private ParticleType CreateWindParticleType()
        {
            return new ParticleType
            {
                Size = 1f,
                Color = windColor,
                Color2 = Color.Lerp(windColor, Color.White, 0.3f),
                ColorMode = ParticleType.ColorModes.Fade,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 1.0f,
                LifeMax = 2.5f,
                SpeedMin = windStrength * 0.3f,
                SpeedMax = windStrength * 0.8f,
                DirectionRange = MathHelper.ToRadians(30f), // Cone of wind direction
                Direction = windDirection.Angle(),
                Acceleration = windDirection * (windStrength * 0.1f),
                SpinMin = -1f,
                SpinMax = 1f
            };
        }
        #endregion

        #region Overrides
        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
        }

        public override void OnEnter(global::Celeste.Player player)
        {
            base.OnEnter(player);
            
            if (!triggerOnce || !hasTriggered)
            {
                StartWind();
                hasTriggered = true;
            }
        }

        public override void OnLeave(global::Celeste.Player player)
        {
            base.OnLeave(player);
            
            // Stop wind when leaving trigger (unless it has a duration)
            if (windDuration <= 0f)
            {
                StopWind();
            }
        }

        public override void Update()
        {
            base.Update();
            
            if (isActive)
            {
                activeTime += Engine.DeltaTime;
                
                // Check if wind should stop due to duration
                if (windDuration > 0f && activeTime >= windDuration)
                {
                    StopWind();
                    return;
                }
                
                UpdateWindEffects();
                
                // Handle gusts for gusty wind type
                if (windType == WindType.Gusty && activeTime >= nextGustTime)
                {
                    TriggerGust();
                    nextGustTime = activeTime + (1f / gustFrequency) + Calc.Random.Range(-0.5f, 0.5f);
                }
            }
        }
        #endregion

        #region Wind Control
        private void StartWind()
        {
            if (isActive) return;
            
            isActive = true;
            activeTime = 0f;
            nextGustTime = 1f / gustFrequency;
            
            IngesteLogger.Debug($"Starting wind effect: type={windType}, strength={windStrength}");
            
            // Start wind coroutine
            windCoroutine = new Coroutine(WindEffectCoroutine());
            Add(windCoroutine);
            
            // Start wind sound
            if (soundEffects && windSound != null)
            {
                string soundEvent = windType switch
                {
                    WindType.Updraft => "event:/env/local/09_core/screenchange_impact",
                    WindType.Gusty => "event:/env/amb/09_core/conveyor_idle",
                    WindType.Swirling => "event:/env/amb/04_main/conveyor_idle",
                    _ => "event:/env/amb/03_resort/wind_deep"
                };
                
                windSound.Play(soundEvent);
            }
            
            // Apply immediate effects
            if (affectPlayer)
            {
                ApplyPlayerEffects();
            }
            
            if (affectStylegrounds)
            {
                ApplyStylegroundEffects();
            }
        }

        private void StopWind()
        {
            if (!isActive) return;
            
            isActive = false;
            IngesteLogger.Debug("Stopping wind effect");
            
            // Stop coroutine
            if (windCoroutine != null)
            {
                windCoroutine.Cancel();
                windCoroutine = null;
            }
            
            // Stop sound
            if (windSound != null)
            {
                windSound.Stop();
            }
            
            // Stop styleground effects
            StopStylegroundEffects();
        }

        private IEnumerator WindEffectCoroutine()
        {
            while (isActive)
            {
                // Emit wind particles
                if (particleEffects && level != null)
                {
                    EmitWindParticles();
                }
                
                // Apply continuous effects
                if (affectPlayer)
                {
                    ApplyPlayerEffects();
                }
                
                yield return null;
            }
        }
        #endregion

        #region Player Effects
        private void ApplyPlayerEffects()
        {
            if (level == null) return;
            
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player == null) return;
            
            // Check if player is still in trigger area
            if (!CollideCheck(player)) return;
            
            Vector2 windForce = CalculateWindForce();
            
            // Apply wind force based on wind type
            switch (windType)
            {
                case WindType.Constant:
                    ApplyConstantWind(player, windForce);
                    break;
                    
                case WindType.Gusty:
                    ApplyGustyWind(player, windForce);
                    break;
                    
                case WindType.Swirling:
                    ApplySwirlingWind(player, windForce);
                    break;
                    
                case WindType.Updraft:
                    ApplyUpdraftWind(player, windForce);
                    break;
                    
                case WindType.Crosswind:
                    ApplyCrosswind(player, windForce);
                    break;
            }
            
            // Special handling for Kirby player (if available)
            ApplyKirbySpecificEffects(player, windForce);
        }

        private Vector2 CalculateWindForce()
        {
            float currentStrength = windStrength;
            
            // Modify strength based on wind type
            switch (windType)
            {
                case WindType.Gusty:
                    // Add some randomness for gusty winds
                    currentStrength *= (0.7f + Calc.Random.NextFloat() * 0.6f);
                    break;
                    
                case WindType.Swirling:
                    // Rotating wind pattern
                    float angle = windDirection.Angle() + (float)Math.Sin(activeTime * 2f) * MathHelper.PiOver2;
                    return Calc.AngleToVector(angle, currentStrength);
                    
                case WindType.Updraft:
                    // Stronger upward component
                    currentStrength *= 1.3f;
                    break;
            }
            
            return windDirection * currentStrength;
        }

        private void ApplyConstantWind(global::Celeste.Player player, Vector2 windForce)
        {
            // Apply steady wind force
            player.Speed += windForce * Engine.DeltaTime;
            
            // Limit maximum wind-induced speed
            float maxWindSpeed = windStrength * 1.5f;
            if (player.Speed.Length() > maxWindSpeed)
            {
                player.Speed = player.Speed.SafeNormalize() * maxWindSpeed;
            }
        }

        private void ApplyGustyWind(global::Celeste.Player player, Vector2 windForce)
        {
            // Similar to constant but with more variation
            ApplyConstantWind(player, windForce);
        }

        private void ApplySwirlingWind(global::Celeste.Player player, Vector2 windForce)
        {
            // Circular wind pattern around trigger center
            Vector2 toCenter = Center - player.Position;
            Vector2 tangent = new Vector2(-toCenter.Y, toCenter.X).SafeNormalize();
            Vector2 swirlingForce = tangent * windStrength + windForce * 0.3f;
            
            player.Speed += swirlingForce * Engine.DeltaTime;
        }

        private void ApplyUpdraftWind(global::Celeste.Player player, Vector2 windForce)
        {
            // Strong upward force, helpful for reaching high places
            Vector2 updraftForce = windForce;
            if (windDirection.Y < 0) // If wind goes up
            {
                updraftForce.Y *= 1.5f; // Boost upward component
            }
            
            player.Speed += updraftForce * Engine.DeltaTime;
            
            // Give player extra air control during updraft
            if (player.Speed.Y < 0 && Input.MoveY.Value < 0)
            {
                player.Speed.Y *= 0.95f; // Slight resistance when trying to go up
            }
        }

        private void ApplyCrosswind(global::Celeste.Player player, Vector2 windForce)
        {
            // Horizontal wind that can push player sideways
            Vector2 horizontalForce = new Vector2(windForce.X, windForce.Y * 0.3f);
            player.Speed += horizontalForce * Engine.DeltaTime;
        }

        private void ApplyKirbySpecificEffects(global::Celeste.Player player, Vector2 windForce)
        {
            // Check if this is a Kirby player (using the mod's entity system)
            // For now, we'll check if there are any Kirby-related entities in the scene
            bool hasKirbyEntities = Scene.Entities.Any(e => e.GetType().Name.Contains("Kirby"));
            
            if (hasKirbyEntities)
            {
                // Kirby gets enhanced air control in wind
                if (Math.Abs(player.Speed.Y) < 50f)
                {
                    // Enhanced floating ability in wind
                    if (Input.Jump.Check)
                    {
                        player.Speed.Y -= 80f * Engine.DeltaTime;
                    }
                }
                
                // Wind enhances Kirby's copy abilities
                ApplyKirbyAbilityEnhancements(windForce);
            }
        }

        private void ApplyKirbyAbilityEnhancements(Vector2 windForce)
        {
            // This would integrate with the existing Kirby ability system
            // For now, we'll add some basic enhancements
            
            if (level?.Tracker.GetEntity<global::Celeste.Player>() is var player && player != null)
            {
                // Wind boosts dash distance
                if (player.StateMachine.State == global::Celeste.Player.StDash)
                {
                    player.Speed += windForce * 0.5f * Engine.DeltaTime;
                }
            }
        }

        private void TriggerGust()
        {
            if (level == null) return;
            
            // Create a strong gust effect
            level.Shake(0.2f);
            
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null && CollideCheck(player))
            {
                Vector2 gustForce = windDirection * (windStrength * gustIntensity);
                player.Speed += gustForce * 0.3f;
            }
            
            // Emit extra particles for gust
            if (particleEffects)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 particlePos = Position + new Vector2(
                        Calc.Random.Range(-Width / 2, Width / 2),
                        Calc.Random.Range(-Height / 2, Height / 2)
                    );
                    level.ParticlesFG.Emit(windParticles, 1, particlePos, Vector2.One * 4f);
                }
            }
            
            // Gust sound effect
            if (soundEffects)
            {
                Audio.Play("event:/env/local/09_core/screenchange_impact", Position);
            }
        }
        #endregion

        #region Styleground Effects
        private void ApplyStylegroundEffects()
        {
            if (level == null) return;
            
            // Find and affect stylegrounds with wind effects
            var stylegrounds = level.Background.Backdrops;
            
            foreach (var backdrop in stylegrounds)
            {
                ApplyWindToStyleground(backdrop);
            }
            
            // Also affect foreground stylegrounds
            var foregrounds = level.Foreground.Backdrops;
            foreach (var backdrop in foregrounds)
            {
                ApplyWindToStyleground(backdrop);
            }
        }

        private void ApplyWindToStyleground(Backdrop backdrop)
        {
            if (backdrop == null) return;
            
            // Apply wind effects based on backdrop type
            string backdropTypeName = backdrop.GetType().Name.ToLower();
            
            if (backdropTypeName.Contains("cloud") || backdropTypeName.Contains("mist"))
            {
                ApplyCloudWindEffect(backdrop);
            }
            else if (backdropTypeName.Contains("particle") || backdropTypeName.Contains("snow"))
            {
                ApplyParticleWindEffect(backdrop);
            }
            else if (backdropTypeName.Contains("parallax") || backdropTypeName.Contains("tower"))
            {
                ApplyParallaxWindEffect(backdrop);
            }
        }

        private void ApplyCloudWindEffect(Backdrop backdrop)
        {
            // Affect cloud movement and opacity
            if (backdrop is Parallax parallax)
            {
                // Modify parallax speed based on wind
                float windInfluence = windStrength * 0.01f;
                // This would need to modify the parallax's internal speed
                // The exact implementation depends on the Parallax class structure
            }
        }

        private void ApplyParticleWindEffect(Backdrop backdrop)
        {
            // Affect particle-based stylegrounds
            // This would influence particle direction and intensity
        }

        private void ApplyParallaxWindEffect(Backdrop backdrop)
        {
            // Apply subtle movement to parallax backgrounds
            // Since we can't easily cast to TowerBackgroundStyleground without knowing if it inherits from Backdrop,
            // we'll apply generic wind effects to parallax backgrounds
            if (backdrop is Parallax parallax)
            {
                // Apply wind-based modifications to parallax backgrounds
                // This is a placeholder - actual implementation would depend on Parallax internals
            }
        }

        private void StopStylegroundEffects()
        {
            // Reset any styleground modifications
            if (level == null) return;
            
            var allBackdrops = level.Background.Backdrops.ToArray()
                .Concat(level.Foreground.Backdrops.ToArray());
                
            foreach (var backdrop in allBackdrops)
            {
                ResetStylegroundWind(backdrop);
            }
        }

        private void ResetStylegroundWind(Backdrop backdrop)
        {
            // Reset backdrop to normal state
            // Generic reset for all backdrop types
            if (backdrop is Parallax parallax)
            {
                // Reset parallax-specific wind effects
                // This is a placeholder - actual implementation would depend on Parallax internals
            }
        }
        #endregion

        #region Particle Effects
        private void EmitWindParticles()
        {
            if (level == null || !particleEffects) return;
            
            // Emit particles based on wind type and strength
            int particleCount = windType switch
            {
                WindType.Gusty => 3,
                WindType.Swirling => 4,
                WindType.Updraft => 5,
                _ => 2
            };
            
            for (int i = 0; i < particleCount; i++)
            {
                Vector2 particlePos = Position + new Vector2(
                    Calc.Random.Range(-Width / 2, Width / 2),
                    Calc.Random.Range(-Height / 2, Height / 2)
                );
                
                // Emit particles in appropriate layer
                if (windType == WindType.Updraft)
                {
                    level.ParticlesFG.Emit(windParticles, 1, particlePos, Vector2.One * 6f);
                }
                else
                {
                    level.ParticlesBG.Emit(windParticles, 1, particlePos, Vector2.One * 4f);
                }
            }
        }

        private void UpdateWindEffects()
        {
            // Update any ongoing wind visual effects
            if (windType == WindType.Swirling && particleEffects && level != null && Scene.OnInterval(0.1f))
            {
                // Create swirling particle pattern
                float angle = activeTime * 3f;
                Vector2 spiralPos = Center + Calc.AngleToVector(angle, 20f + (float)Math.Sin(activeTime * 2f) * 10f);
                level.ParticlesFG.Emit(windParticles, 1, spiralPos, Vector2.One * 2f);
            }
        }
        #endregion

        #region Public API
        /// <summary>
        /// Manually trigger the wind effect (for use by other entities)
        /// </summary>
        public void ActivateWind()
        {
            if (!hasTriggered || !triggerOnce)
            {
                StartWind();
                hasTriggered = true;
            }
        }

        /// <summary>
        /// Manually stop the wind effect
        /// </summary>
        public void DeactivateWind()
        {
            StopWind();
        }

        /// <summary>
        /// Check if wind is currently active
        /// </summary>
        public bool IsWindActive => isActive;

        /// <summary>
        /// Get current wind force at player position
        /// </summary>
        public Vector2 GetWindForceAtPosition(Vector2 position)
        {
            if (!isActive || !CollidePoint(position)) return Vector2.Zero;
            return CalculateWindForce();
        }
        #endregion
    }
}



