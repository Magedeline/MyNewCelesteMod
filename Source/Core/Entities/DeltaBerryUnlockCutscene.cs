namespace DesoloZantas.Core.Core.Entities
{
    class DeltaBerryUnlockCutscene : CutsceneEntity
    {
        private DeltaBerry strawberry;
        private HoloDeltaBerry holoBerry;
        private int silverBerryCount;
        private ParticleSystem system;
        private SoundSource snapshot;
        private SoundSource sfx;

        public DeltaBerryUnlockCutscene(DeltaBerry strawberry, HoloDeltaBerry holoBerry, int silverBerryCount)
        {
            this.strawberry = strawberry ?? throw new ArgumentNullException(nameof(strawberry));
            this.holoBerry = holoBerry ?? throw new ArgumentNullException(nameof(holoBerry));
            this.silverBerryCount = silverBerryCount;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(cutscene(level)));
        }

        private IEnumerator cutscene(Level level)
        {
            var player = Scene.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null)
            {
                // Wait until the player is in control
                while (!player.InControl)
                {
                    yield return null;
                }
                player.StateMachine.State = global::Celeste.Player.StDummy; // Set player state to Dummy
                player.DummyAutoAnimate = true;
            }

            // Start animation and mute sound
            sfx = new SoundSource();
            Add(sfx);
            sfx.Play("event:/game/general/seed_complete_main");
            
            // Use Celeste's Audio system for snapshots
            // Note: Audio.SetSnapshot doesn't exist, use Audio.CreateSnapshot instead
            snapshot = new SoundSource();
            Add(snapshot);
            // Will be handled by Celeste's audio system automatically

            // Freeze the level after a short delay
            Depth = Depths.FormationSequences - 3;
            strawberry.Depth = Depths.FormationSequences - 2;
            holoBerry.Depth = -Depths.FormationSequences - 2;
            holoBerry.Particles.Depth = Depths.FormationSequences - 2;
            strawberry.AddTag(Tags.FrozenUpdate);
            yield return 0.35f;
            Tag = Tags.FrozenUpdate;
            level.Frozen = true;

            // Darken the level and pause SFX
            level.FormationBackdrop.Display = true;
            level.FormationBackdrop.Alpha = 0.5f;
            level.Displacement.Clear();
            level.Displacement.Enabled = false;
            pauseAudioBuses(true);
            yield return 0.1f;

            // Add particle system
            system = new ParticleSystem(Depths.FormationSequences - 2, 50)
            {
                Tag = Tags.FrozenUpdate
            };
            level.Add(system);

            // Focus camera on the rainbow berry
            Vector2 cameraTarget = strawberry.Position - new Vector2(160f, 90f);
            cameraTarget = cameraTarget.Clamp(level.Bounds.Left, level.Bounds.Top, level.Bounds.Right - 320, level.Bounds.Bottom - 180);
            yield return 0.1f;
            Add(new Coroutine(CameraTo(cameraTarget, 2f, Ease.CubeInOut)));
            yield return 3.9f;

            // Remove the silver berries and make the rainbow berry appear
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            holoBerry.RemoveSelf();
            strawberry.CollectedSeeds();
            yield return 0.5f;

            // Pan back to the player
            yield return CameraTo(player.CameraTarget, 1f, Ease.CubeOut);

            // End the cutscene
            level.EndCutscene();
            OnEnd(level);
        }

        public override void OnEnd(Level level)
        {
            if (WasSkipped)
            {
                sfx?.Stop();
            }
            var player = Scene.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null)
            {
                player.StateMachine.State = global::Celeste.Player.StNormal;
                player.DummyAutoAnimate = true;
            }
            level.OnEndOfFrame += () =>
            {
                if (WasSkipped)
                {
                    holoBerry.RemoveSelf();
                    strawberry.CollectedSeeds();
                    level.Camera.Position = player.CameraTarget;
                }
                strawberry.Depth = Depths.Pickups;
                strawberry.RemoveTag(Tags.FrozenUpdate);
                level.Frozen = false;
                level.FormationBackdrop.Display = false;
                level.Displacement.Enabled = true;
            };
            RemoveSelf();
        }

        private void endSfx()
        {
            pauseAudioBuses(false);
            // Release snapshot - handled automatically by Celeste's audio system
            snapshot?.Stop();
        }

        private void pauseAudioBuses(bool pause)
        {
            Audio.BusPaused(Buses.AMBIENCE, pause);
            Audio.BusPaused(Buses.CHAR, pause);
            Audio.BusPaused(Buses.YES_PAUSE, pause);
            Audio.BusPaused(Buses.CHAPTERS, pause);
        }

        public override void Removed(Scene scene)
        {
            endSfx();
            base.Removed(scene);
        }

        public override void SceneEnd(Scene scene)
        {
            endSfx();
            base.SceneEnd(scene);
        }
    }
}




