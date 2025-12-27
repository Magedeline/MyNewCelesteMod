namespace DesoloZantas.Core.Core.Cutscenes
{
    /// <summary>
    /// Custom Overworld Reflections Fall scene that returns to 08truthAlt1.bin room lvl_a-00
    /// Based on vanilla OverworldReflectionsFall but with custom return destination
    /// </summary>
    public class CS08_OverworldReflectionsFall : Scene
    {
        private Level returnTo;
        private Action returnCallback;
        private Maddy3D maddy;
        private MountainRenderer mountain;
        private MountainCamera startCamera = new MountainCamera(new Vector3(-8f, 12f, -0.4f), new Vector3(-2f, 9f, -0.5f));
        private MountainCamera fallCamera = new MountainCamera(new Vector3(-10f, 6f, -0.4f), new Vector3(-4.25f, 1.5f, -1.25f));

        public CS08_OverworldReflectionsFall(Level returnTo, Action returnCallback)
        {
            this.returnTo = returnTo;
            this.returnCallback = returnCallback;
            Add(mountain = new MountainRenderer());
            mountain.SnapCamera(-1, new MountainCamera(startCamera.Position + (startCamera.Target - startCamera.Position).SafeNormalize() * 2f, startCamera.Target));
            Add(new HiresSnow
            {
                ParticleAlpha = 0f
            });
            Add(new Snow3D(mountain.Model));
            Add(maddy = new Maddy3D(mountain));
            maddy.Falling();
            Add(new Entity
            {
                new Coroutine(Routine())
            });
        }

        private IEnumerator Routine()
        {
            mountain.EaseCamera(-1, startCamera, 0.4f, true);
            float duration = 4f;
            maddy.Position = startCamera.Target;
            for (int i = 0; i < 30; i++)
            {
                maddy.Position = startCamera.Target + new Vector3(Calc.Random.Range(-0.05f, 0.05f), Calc.Random.Range(-0.05f, 0.05f), Calc.Random.Range(-0.05f, 0.05f));
                yield return 0.01f;
            }
            yield return 0.1f;
            maddy.Add(new Coroutine(MaddyFall(duration + 0.1f)));
            yield return 0.1f;
            mountain.EaseCamera(-1, fallCamera, duration, true);
            mountain.ForceNearFog = true;
            yield return duration;
            yield return 0.25f;
            MountainCamera transform = new MountainCamera(fallCamera.Position + mountain.Model.Forward * 3f, fallCamera.Target);
            mountain.EaseCamera(-1, transform, 0.5f, true);
            Return();
        }

        private IEnumerator MaddyFall(float duration)
        {
            for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
            {
                maddy.Position = Vector3.Lerp(startCamera.Target, fallCamera.Target, p);
                yield return null;
            }
        }

        private void Return()
        {
            new FadeWipe(this, wipeIn: false, () =>
            {
                mountain.Dispose();
                if (returnTo != null)
                {
                    Engine.Scene = returnTo;
                }
                returnCallback();
            });
        }

        /// <summary>
        /// Creates a CS08_OverworldReflectionsFall scene that transitions to 08truthAlt1.bin room lvl_a-00
        /// </summary>
        public static CS08_OverworldReflectionsFall CreateFor08TruthAlt1(Level level)
        {
            return new CS08_OverworldReflectionsFall(level, delegate
            {
                Audio.SetAmbience(null, true);
                level.Session.Level = "lvl_a-00";
                level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2((float)level.Bounds.Center.X, (float)level.Bounds.Top));
                level.LoadLevel(global::Celeste.Player.IntroTypes.Fall, false);
                level.Add(new BackgroundFadeIn(Color.Black, 2f, 30f));
                level.Entities.UpdateLists();
                foreach (Entity entity in level.Tracker.GetEntities<CrystalStaticSpinner>())
                {
                    ((CrystalStaticSpinner)entity).ForceInstantiate();
                }
            });
        }
    }
}
