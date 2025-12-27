#nullable enable

namespace DesoloZantas.Core.Core.Cutscenes
{
    class Cs10IntroVignetteAlt : Scene
    {
        private readonly Session session;

        private readonly string? areaMusic;

        private float fade = 0f;

        private TextMenu? menu;

        private float pauseFade = 0f;

        private readonly HudRenderer hud;

        private bool exiting;

        private Coroutine? textCoroutine;

        private float textAlpha = 0f;

        private readonly Textbox textbox;

        // Removed FMOD EventInstance - using Celeste's Audio system instead
        private bool ringtoneActive = false;

        public bool CanPause => menu == null;

        public Cs10IntroVignetteAlt(Session session, TextMenu? menu, bool playRingtone = false, HiresSnow? _ = null)
        {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.menu = menu;
            this.ringtoneActive = playRingtone;
            areaMusic = Audio.CurrentMusic;
            Audio.CurrentMusic = null;
            Add(hud = new HudRenderer());
            RendererList.UpdateLists();
            textbox = new Textbox("CH10_RUINS_INTRO");
            textCoroutine = new Coroutine(TextSequence());
        }

        public Cs10IntroVignetteAlt(Session session1) : this(session1, null, false, null)
        {
            Add(new HiresSnow());
            Add(new FadeWipe(this, true));
        }

        private IEnumerator TextSequence()
        {
            yield return 1f;
            // Use safe audio helper
            AudioHelper.PlaySafe("event:/Ingeste/music/lvl10/intro", "event:/music/cassette/01_forsaken_city");
            yield return 4f;
            // Stop ringtone using Celeste's audio system
            if (ringtoneActive)
            {
                // Let Celeste handle stopping the ringtone
                ringtoneActive = false;
            }
            Audio.SetMusicParam(nameof(fade), 1f);
            yield return 1f;
            yield return Say(textbox);
            yield return 0.5f;
            Audio.SetMusicParam("pitch", 1f);
            yield return 1f;
            StartGame();
        }

        private static IEnumerator Say(Textbox textbox)
        {
            Engine.Scene.Add(textbox);
            while (textbox.Opened)
            {
                yield return true;
            }
        }

        public override void Update()
        {
            if (menu == null)
            {
                base.Update();
                if (!exiting)
                {
                    textCoroutine?.Update();
                    if (Input.Pause.Pressed || Input.ESC.Pressed)
                    {
                        OpenMenu();
                    }
                }
            }
            else if (!exiting)
            {
                menu.Update();
            }
            pauseFade = Calc.Approach(pauseFade, menu != null ? 1 : 0, Engine.DeltaTime * 8f);
            hud.BackgroundFade = Calc.Approach(hud.BackgroundFade, menu != null ? 0.6f : 0f, Engine.DeltaTime * 3f);
            fade = Calc.Approach(fade, 0f, Engine.DeltaTime);
        }

        public void OpenMenu()
        {
            PauseSfx();
            Audio.Play("event:/ui/game/pause");
            Add(menu = new TextMenu());
            menu.Add(new TextMenu.Button(Dialog.Clean("intro_vignette_resume")).Pressed(CloseMenu));
            menu.Add(new TextMenu.Button(Dialog.Clean("intro_vignette_skip")).Pressed(StartGame));
            menu.OnCancel = menu.OnESC = menu.OnPause = CloseMenu;
        }

        private void CloseMenu()
        {
            ResumeSfx();
            Audio.Play("event:/ui/game/unpause");
            menu?.RemoveSelf();
            menu = null;
        }

        private void StartGame()
        {
            StopSfx();
            textCoroutine = null;
            Audio.CurrentMusic = areaMusic;
            if (menu != null)
            {
                menu.RemoveSelf();
                menu = null;
            }
            var fadeWipe = new FadeWipe(this, false, delegate
            {
                Engine.Scene = new LevelLoader(session);
            });
            fadeWipe.OnUpdate = delegate (float f)
            {
                textAlpha = Math.Min(textAlpha, 1f - f);
            };
            exiting = true;
        }

        public override void Render()
        {
            base.Render();
            if (fade > 0f || textAlpha > 0f)
            {
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, RasterizerState.CullNone, null, Engine.ScreenMatrix);
                if (fade > 0f)
                {
                    Draw.Rect(-1f, -1f, 1922f, 1082f, Color.Black * fade);
                }
                Draw.SpriteBatch.End();
            }
        }

        private void PauseSfx()
        {
            foreach (var component in Tracker.GetComponents<SoundSource>())
            {
                var sound = (SoundSource)component;
                sound.Pause();
            }
            // Removed direct FMOD setPaused call - Celeste handles this through SoundSource
        }

        private void ResumeSfx()
        {
            foreach (var component in Tracker.GetComponents<SoundSource>())
            {
                var sound = (SoundSource)component;
                sound.Resume();
            }
            // Removed direct FMOD setPaused call - Celeste handles this through SoundSource
        }

        private void StopSfx()
        {
            var components = new List<Component>();
            components.AddRange(Tracker.GetComponents<SoundSource>());
            foreach (var component in components)
            {
                var sound = (SoundSource)component;
                sound.RemoveSelf();
            }
            // Removed direct FMOD stop call - Celeste handles this through SoundSource
        }
    }
}



