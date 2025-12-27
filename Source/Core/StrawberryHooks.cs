using System.Reflection;
using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// This class sets up some hooks for strawberry functionality.
    /// DeltaBerry has been removed - this now handles base strawberry hooks only.
    /// </summary>
    static class StrawberryHooks
    {
        internal static void Load()
        {
            On.Celeste.Strawberry.Added += modStrawberrySprite;
            On.Celeste.Strawberry.CollectRoutine += onStrawberryCollectRoutine;
            On.Celeste.Player.Added += Player_Added;
            On.Celeste.Level.End += onLevelEnd;
            Everest.Events.Level.OnCreatePauseMenuButtons += onCreatePauseMenuButtons;
        }

        private static void Player_Added(On.Celeste.Player.orig_Added orig, global::Celeste.Player self, Scene scene)
        {
            orig(self, scene);
            // DeltaBerry removed - no special handling needed
        }

        internal static void Unload()
        {
            On.Celeste.Strawberry.Added -= modStrawberrySprite;
            On.Celeste.Strawberry.CollectRoutine -= onStrawberryCollectRoutine;
            On.Celeste.Player.Added -= Player_Added;
            On.Celeste.Level.End -= onLevelEnd;
            Everest.Events.Level.OnCreatePauseMenuButtons -= onCreatePauseMenuButtons;
        }

        private static void onCreatePauseMenuButtons(Level level, TextMenu menu, bool minimal)
        {
            // DeltaBerry removed - no special pause menu buttons needed
        }

        private static void modStrawberrySprite(On.Celeste.Strawberry.orig_Added orig, CelesteStrawberry self, Scene scene)
        {
            orig(self, scene);
            // DeltaBerry removed - use default strawberry sprites
        }

        private static IEnumerator onStrawberryCollectRoutine(On.Celeste.Strawberry.orig_CollectRoutine orig, CelesteStrawberry self, int collectIndex)
        {
            IEnumerator origEnum = orig(self, collectIndex);
            while (origEnum.MoveNext())
            {
                yield return origEnum.Current;
            }
            // DeltaBerry removed - no special collect effects
        }

        private static void onLevelEnd(On.Celeste.Level.orig_End orig, Level self)
        {
            orig(self);
        }
    }

    // Stub for SaveDataExtensions
    public static class SaveDataExtensions
    {
        public static bool IsDeltaBerryCollected(string id) => false;
        public static void MarkDeltaBerryAsCollected(string id) { }
    }
}



