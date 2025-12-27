namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Configuration constants for Ingeste Helper mod
    /// Provides shared constants for animations, movement, and effects
    /// </summary>
    public static class IngesteHelperConfig
    {
        // Float and movement speeds
        public const float FLOAT_SPEED = 120f;
        public const float FLOAT_ACCEL = 240f;
        public const float WALK_SPEED = 64f;
        public const float RUN_SPEED = 90f;
        
        // Animation timing
        public const float IDLE_TIMEOUT = 3f;
        public const float ANIMATION_TRANSITION_TIME = 0.1f;
        
        // Particle and effect settings
        public const int PARTICLE_COUNT_LOW = 8;
        public const int PARTICLE_COUNT_MEDIUM = 16;
        public const int PARTICLE_COUNT_HIGH = 32;
        
        // Distances and ranges
        public const float INTERACTION_RANGE = 32f;
        public const float FOLLOW_DISTANCE = 16f;
        public const float FLOAT_HEIGHT_VARIATION = 4f;
        
        // Audio settings
        public const float DEFAULT_VOLUME = 0.8f;
        public const float FOOTSTEP_VOLUME = 0.6f;
        
        // Kirby-specific settings
        public const float INHALE_RANGE = 64f;
        public const float HOVER_FALL_SPEED = 60f;
        public const int KIRBY_MAX_HEALTH = 6;
        
        // Boss and combat settings
        public const float BOSS_HEALTH_MULTIPLIER = 1.0f;
        public const float BOSS_DAMAGE_MULTIPLIER = 1.0f;
        
        // Timing constants
        public const float CUTSCENE_WAIT_SHORT = 0.5f;
        public const float CUTSCENE_WAIT_MEDIUM = 1.0f;
        public const float CUTSCENE_WAIT_LONG = 2.0f;
        
        // Visual effect constants
        public const float DISPLACEMENT_STRENGTH = 0.5f;
        public const float SCREEN_SHAKE_DURATION = 0.2f;
        public const float RUMBLE_STRENGTH = 0.8f;
        
        // Platform and collision constants
        public const float PLATFORM_SNAP_DISTANCE = 4f;
        public const float GROUND_CHECK_DISTANCE = 1f;
        
        // Special ability constants
        public const float POWER_DURATION = 60f; // seconds
        public const float INVINCIBILITY_TIME = 2f; // seconds
        public const float TRANSFORM_TIME = 0.5f; // seconds
    }
}



