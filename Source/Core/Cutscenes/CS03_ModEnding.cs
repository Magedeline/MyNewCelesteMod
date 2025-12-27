#nullable enable
namespace DesoloZantas.Core.Core.Cutscenes
{
    class Cs03ModEnding : CutsceneEntity
    {
        private global::Celeste.Player player;
        private Bonfire? bonfire;

        public Cs03ModEnding(global::Celeste.Player player, Bonfire? bonfire)
            : base(false, true)
        {
            this.player = player;
            this.bonfire = bonfire;
        }

        // Implemented constructor
        public Cs03ModEnding(global::Celeste.Player player)
            : base(false, true)
        {
            this.player = player;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            // Attempt to get the Bonfire object from the Scene's tracker
            this.bonfire = Scene?.Tracker.GetEntity<Bonfire>();

            // Optional: Handle a case where the Bonfire isn't found
            if (this.bonfire == null)
            {
                throw new InvalidOperationException("Bonfire entity could not be found in the current scene.");
            }
        }

        public override void OnBegin(Level level)
        {
            level.RegisterAreaComplete();
            bonfire = Scene.Tracker.GetEntity<Bonfire>();
            Add(new Coroutine(cutscene(level)));
        }

        private IEnumerator cutscene(Level level)
        {
            Cs03ModEnding cs03ModEnding = this;
            cs03ModEnding.player.StateMachine.State = 11;
            cs03ModEnding.player.Dashes = 1;
            level.Session.Audio.Music.Layer(3, false);
            level.Session.Audio.Apply();
            yield return 0.5f;
            if (cs03ModEnding.bonfire != null)
                yield return cs03ModEnding.player.DummyWalkTo(cs03ModEnding.bonfire.X + 40f);
            yield return 1.5f;
            cs03ModEnding.player.Facing = Facings.Left;
            yield return 0.5f;
            yield return Textbox.Say("CH1_ENDKIRBY", modEndCityTrigger2);
            yield return 0.3f;
            endCutscene(new global::Celeste.Level { level });
        }

        private void endCutscene(Level level)
        {
            if (player != null)
            {
                player.StateMachine.Locked = false;
                player.StateMachine.State = 0;
            }
        }

        private IEnumerator modEndCityTrigger2()
        {
            Cs03ModEnding cs03ModEnding;
            cs03ModEnding = this;
            yield return 0.2f;
            if (cs03ModEnding.bonfire != null)
            {
                yield return cs03ModEnding.player.DummyWalkTo(cs03ModEnding.bonfire.X - 12f);
                yield return 0.2f;
                cs03ModEnding.player.Facing = Facings.Right;
                cs03ModEnding.player.DummyAutoAnimate = false;
                cs03ModEnding.player.Sprite.Play("duck");
                yield return 0.5f;
                cs03ModEnding.bonfire.SetMode(Bonfire.Mode.Lit);
                yield return 1f;
                cs03ModEnding.player.Sprite.Play("idle");
                yield return 0.4f;
                cs03ModEnding.player.DummyAutoAnimate = true;
                yield return cs03ModEnding.player.DummyWalkTo(cs03ModEnding.bonfire.X - 24f);
            }

            yield return 0.4f;
            cs03ModEnding.player.DummyAutoAnimate = false;
            cs03ModEnding.player.Facing = Facings.Right;
            cs03ModEnding.player.Sprite.Play("sleep");
            Audio.Play("event:/char/madeline/campfire_sit", cs03ModEnding.player.Position);
            yield return 4f;
            BirdNPC? bird = new BirdNPC(cs03ModEnding.player.Position + new Vector2(88f, -200f), BirdNPC.Modes.None);
            cs03ModEnding.Scene.Add(bird);
            FMOD.Studio.EventInstance? instance = Audio.Play("event:/game/general/bird_in", bird.Position);
            bird.Facing = Facings.Left;
            bird.Sprite.Play("fall");
            Vector2 from = bird.Position;
            Vector2 to = cs03ModEnding.player.Position + new Vector2(1f, -12f);
            float percent = 0.0f;
            while (percent < 1.0)
            {
                bird.Position = from + (to - from) * Ease.QuadOut(percent);
                Audio.Position(instance, bird.Position);
                if (percent > 0.5)
                    bird.Sprite.Play("fly");
                percent += Engine.DeltaTime * 0.5f;
                yield return null;
            }
            bird.Position = to;
            bird.Sprite.Play("idle");
            yield return 0.5f;
            bird.Sprite.Play("croak");
            yield return 0.6f;
            Audio.Play("event:/game/general/bird_squawk", bird.Position);
            yield return 0.9f;
            bird.Sprite.Play("sleep");
            yield return null;
            bird = null;
            instance = null;
            from = new Vector2();
            to = new Vector2();
            yield return 2f;
        }

        public override void OnEnd(Level level)
        {
            level.CompleteArea(true, false, false);
        }
    }
}




