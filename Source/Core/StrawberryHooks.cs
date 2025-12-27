using System.Reflection;
using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// This class sets up some hooks that will be useful for rainbow berries.
    /// They mod the following things:
    /// - strawberry sprite for rainbows
    /// - death sounds for rainbows
    /// - collect sounds for rainbows
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
            var deltaBerry = scene.Tracker.GetEntity<DeltaBerry>();
            if (deltaBerry != null && deltaBerry.Follower != null)
                deltaBerry.Follower.Leader = self.Leader;
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
            DeltaBerry berry = level.Tracker.GetEntity<DeltaBerry>();
            if (berry != null && berry.Follower != null && DeltaBerry.HasLeader && !minimal)
            {
                TextMenu.Button item = new TextMenu.Button(Dialog.Clean("restartdeltaberry"))
                {
                    OnPressed = () =>
                    {
                        level.Paused = false;
                        level.PauseMainMenuOpen = false;
                        menu.RemoveSelf();
                        berry.TimeRanOut = true;
                    }
                };
                menu.Add(item);
            }
        }

        private static void modStrawberrySprite(On.Celeste.Strawberry.orig_Added orig, CelesteStrawberry self, Scene scene)
        {
            orig(self, scene);
            DeltaBerry deltaBerry = scene.Tracker.GetEntity<DeltaBerry>();
            if (deltaBerry != null)
            {
                var spriteField = typeof(CelesteStrawberry).GetField("sprite", BindingFlags.NonPublic | BindingFlags.Instance);
                if (spriteField != null)
                {
                    if (SaveDataExtensions.IsDeltaBerryCollected(deltaBerry.ID.ToString()))
                        spriteField.SetValue(self, GFX.SpriteBank.Create("ghostDeltaBerry"));
                    else
                        spriteField.SetValue(self, GFX.SpriteBank.Create("DeltaBerry"));
                }
            }
        }

        private static IEnumerator onStrawberryCollectRoutine(On.Celeste.Strawberry.orig_CollectRoutine orig, CelesteStrawberry self, int collectIndex)
        {
            Scene scene = self.Scene;
            IEnumerator origEnum = orig(self, collectIndex);
            while (origEnum.MoveNext())
            {
                yield return origEnum.Current;
            }
            DeltaBerry deltaBerry = null;
            if (deltaBerry != null)
            {
                SaveDataExtensions.MarkDeltaBerryAsCollected(deltaBerry.ID.ToString());
                StrawberryPoints points = scene.Entities.ToAdd.OfType<StrawberryPoints>().FirstOrDefault();
                if (points != null) scene.Entities.ToAdd.Remove(points);
                scene.Add(new DeltaBerryDeltaruneEffect(self.Position));
            }
        }

        private static void onLevelEnd(On.Celeste.Level.orig_End orig, Level self)
        {
            orig(self);
        }
    }

// Stub for SaveDataExtensions
    public static class SaveDataExtensions
    {
        public static bool IsDeltaBerryCollected(string id)
        {
            // TODO: Implement actual logic
            return false;
        }
        public static void MarkDeltaBerryAsCollected(string id)
        {
            // TODO: Implement actual logic
        }
    }
}



