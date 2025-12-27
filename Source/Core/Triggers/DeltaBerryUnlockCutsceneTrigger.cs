using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Triggers {
    [CustomEntity("Ingeste/DeltaBerryUnlockCutsceneTrigger")]
    class DeltaBerryUnlockCutsceneTrigger : Trigger {
        private DeltaBerry berry;

        private readonly string levelSet;
        private readonly string maps;
        private Entities.DeltaBerry deltaberry;

        public DeltaBerryUnlockCutsceneTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            levelSet = data.Attr(nameof(levelSet));
            maps = data.Attr(nameof(maps));
        }

        public override void Awake(Scene scene) {
            base.Awake(scene);

            berry = Scene.Entities.OfType<DeltaBerry>()
                .Where(b => b.MatchesRainbowBerryTriggerWithSettings(levelSet, maps))
                .FirstOrDefault();
        }

        public override void OnEnter(global::Celeste.Player player)
        {
            base.OnEnter(player);

            if (berry != null && berry.HologramForCutscene is HoloDeltaBerry holoBerry && player != null)
            {
                // spawn the unlock cutscene.
                Scene.Add(new DeltaBerryUnlockCutscene(deltaberry, holoBerry, berry.TotalBerries));

                // clean up the reference to the holo rainbow berry.
                berry.HologramForCutscene = null;

                // save that the cutscene happened so that it doesn't happen again.
                ((dynamic)IngesteModule.SaveData).CombinedDeltaBerries = berry.GetCombinedRainbowId(Scene as Level);
            }

            // this trigger is one-use.
            RemoveSelf();
        }
    }
}




