namespace DesoloZantas.Core.Core.Systems
{
    /// <summary>
    /// Performance optimization system to reduce delays and improve gameplay responsiveness
    /// </summary>
    public static class PerformanceOptimizer
    {
        // Audio throttling
        private static Dictionary<string, float> audioThrottleCache = new Dictionary<string, float>();
        private static float lastGCTime = 0f;
        private const float GC_INTERVAL = 30f; // Force GC every 30 seconds to prevent stutters
        
        // Entity update optimization
        private static Queue<Entity> entityUpdateQueue = new Queue<Entity>();
        private static int maxEntitiesPerFrame = 50;
        
        /// <summary>
        /// Initialize performance optimization systems
        /// </summary>
        public static void Initialize()
        {
            Logger.Log(LogLevel.Info, "PerformanceOptimizer", "Initializing performance optimizations");
            
            // Reset caches
            audioThrottleCache.Clear();
            entityUpdateQueue.Clear();
            
            // Audio optimization setup
            Logger.Log(LogLevel.Info, "PerformanceOptimizer", "Audio optimization enabled");
        }
        
        /// <summary>
        /// Play audio with throttling to prevent overlap delays
        /// </summary>
        public static bool PlayThrottledAudio(string eventPath, Vector2 position, float throttleTime = 0.1f)
        {
            if (string.IsNullOrEmpty(eventPath))
                return false;
                
            float currentTime = Engine.Scene.TimeActive;
            
            if (audioThrottleCache.ContainsKey(eventPath))
            {
                if (currentTime - audioThrottleCache[eventPath] < throttleTime)
                    return false; // Too soon, skip this audio
            }
            
            audioThrottleCache[eventPath] = currentTime;
            Audio.Play(eventPath, position);
            return true;
        }
        
        /// <summary>
        /// Optimize entity updates by batching them
        /// </summary>
        public static void QueueEntityUpdate(Entity entity)
        {
            if (entity != null && !entityUpdateQueue.Contains(entity))
            {
                entityUpdateQueue.Enqueue(entity);
            }
        }
        
        /// <summary>
        /// Process queued entity updates (call from level update)
        /// </summary>
        public static void ProcessQueuedUpdates()
        {
            int processed = 0;
            while (entityUpdateQueue.Count > 0 && processed < maxEntitiesPerFrame)
            {
                Entity entity = entityUpdateQueue.Dequeue();
                if (entity?.Scene != null)
                {
                    // Custom entity update logic here
                    processed++;
                }
            }
        }
        
        /// <summary>
        /// Force garbage collection periodically to prevent stutters
        /// </summary>
        public static void ManageGarbageCollection()
        {
            float currentTime = Engine.Scene.TimeActive;
            if (currentTime - lastGCTime > GC_INTERVAL)
            {
                // Force GC during safe moments (not during intense gameplay)
                if (Engine.Scene is Level level)
                {
                    var player = level.Tracker.GetEntity<global::Celeste.Player>();
                    if (player == null || player.StateMachine.State == global::Celeste.Player.StNormal)
                    {
                        GC.Collect(0, GCCollectionMode.Optimized);
                        lastGCTime = currentTime;
                    }
                }
            }
        }
        
        /// <summary>
        /// Optimize particle emissions to reduce rendering load
        /// </summary>
        public static void OptimizeParticleEmission(ParticleSystem system, ParticleType particle, int count, Vector2 position, Vector2 range)
        {
            // Reduce particle count based on performance settings
            float performanceMultiplier = GetPerformanceMultiplier();
            int optimizedCount = Math.Max(1, (int)(count * performanceMultiplier));
            
            system.Emit(particle, optimizedCount, position, range);
        }
        
        /// <summary>
        /// Get performance multiplier based on current framerate
        /// </summary>
        private static float GetPerformanceMultiplier()
        {
            float fps = Engine.FPS;
            
            if (fps >= 60f)
                return 1.0f; // Full quality
            else if (fps >= 30f)
                return 0.7f; // Reduced quality
            else
                return 0.5f; // Minimal quality
        }
        
        /// <summary>
        /// Clean up optimization caches
        /// </summary>
        public static void Cleanup()
        {
            audioThrottleCache.Clear();
            entityUpdateQueue.Clear();
        }
        
        /// <summary>
        /// Log performance statistics
        /// </summary>
        public static void LogPerformanceStats()
        {
            Logger.Log(LogLevel.Debug, "PerformanceOptimizer", 
                $"Audio cache entries: {audioThrottleCache.Count}, " +
                $"Queued entities: {entityUpdateQueue.Count}, " +
                $"Current FPS: {Engine.FPS:F1}");
        }
    }
}



