namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Configuration constants for Ingeste mod
    /// </summary>
    public static class IngesteConfig
    {
        // Default float values
        public const float DEFAULT_FLOATNESS = 1.0f;
        public const float FLOAT_SPEED = 40f;
        
        // Hitbox dimensions
        public const float HITBOX_WIDTH = 8f;
        public const float HITBOX_HEIGHT = 11f;
        public const float HITBOX_OFFSET_X = -4f;
        public const float HITBOX_OFFSET_Y = -11f;
        
        // Light settings
        public const float DEFAULT_LIGHT_ALPHA = 1f;
        public const int DEFAULT_LIGHT_START_RADIUS = 32;
        public const int DEFAULT_LIGHT_END_RADIUS = 64;
        
        // Animation frame constants
        public const int WALK_FOOTSTEP_FRAME1 = 0;
        public const int WALK_FOOTSTEP_FRAME2 = 6;
        internal const float DEFAULT_WALK_SPEED = 7;

        /// <summary>
        /// Animation names
        /// </summary>
        public static class Animations
        {
            public const string FALL_SLOW = "fallSlow";
            public const string WALK = "walk";
            public const string RUN_SLOW = "runSlow"; 
            public const string RUN_FAST = "runFast";
        }
        
        /// <summary>
        /// Audio event names
        /// </summary>
        public static class AudioEvents
        {
            public const string FOOTSTEP_SOUND = "event:/char/madeline/footstep_wood";
        }
    }
}



