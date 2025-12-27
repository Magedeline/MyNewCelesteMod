#nullable enable

namespace DesoloZantas.Core.Core.Cutscenes {
    class Cs04LegendVignette : Scene {
        private Session session;

        private string? areaMusic;

        private float fade = 0f;

        private TextMenu? menu;

        private float pauseFade = 0f;

        private HudRenderer hud;

        private bool exiting;

        private Coroutine? textCoroutine;

        private float textAlpha = 0f;

        // Removed FMOD EventInstance - using Celeste's Audio system
        private bool legendMusicPlaying = false;

        private int currentLegendPart = 0;

        private readonly string[] legendParts = {
            "CH4_LEGEND_A",    // The void and broken star warrior
            "CH4_LEGEND_B",    // The FALL and El's emergence
            "CH4_LEGEND_C",    // Six heroes and star warrior
            "CH4_LEGEND_D",    // The Desolo Zantas
            "CH4_LEGEND_E",    // Banishment to the void
            "CH4_LEGEND_F"     // El's eventual return
        };

        public bool CanPause => menu == null;

        public Cs04LegendVignette(Session session, TextMenu? menu = null) {
            this.session = session ?? throw new ArgumentNullException(nameof(session));
            this.menu = menu;
            areaMusic = session.Audio.Music.Event;
            session.Audio.Music.Event = null;
            session.Audio.Apply(forceSixteenthNoteHack: false);
            Add(hud = new HudRenderer());
            RendererList.UpdateLists();
            textCoroutine = new Coroutine(legendSequence());
        }

        public Cs04LegendVignette(Session session1) : this(session1, null) {
            Add(new HiresSnow());
            Add(new FadeWipe(this, true));
        }

        private IEnumerator legendSequence() {
            yield return 1f;

            // Start the legend music using Celeste's Audio system
            Audio.Play("event:/Ingeste/music/lvl4/legend");
            legendMusicPlaying = true;
            yield return 2f;

            // Show each part of the legend
            for (currentLegendPart = 0; currentLegendPart < legendParts.Length; currentLegendPart++) {
                var textbox = new Textbox(legendParts[currentLegendPart]);
                yield return say(textbox);
                yield return 1f; // Brief pause between legend parts
            }

            yield return 2f;

            // Fade out legend music and transition to normal music
            if (legendMusicPlaying) {
                legendMusicPlaying = false;
                // Let Celeste's Audio system handle music transitions
            }
            Audio.SetMusicParam(nameof(fade), 1f);
            yield return 1f;
            Audio.SetMusicParam("pitch", 1f);
            yield return 1f;

            // Transition to LegendEnd cutscene instead of starting game directly
            transitionToLegendEnd();
        }

        private IEnumerator say(Textbox textbox) {
            Engine.Scene.Add(textbox);
            while (textbox.Opened) {
                yield return true;
            }
        }

        public override void Update() {
            if (menu == null) {
                base.Update();
                if (!exiting) {
                    textCoroutine?.Update();
                    if (Input.Pause.Pressed || Input.ESC.Pressed) {
                        OpenMenu();
                    }
                }
            } else if (!exiting) {
                menu.Update();
            }
            pauseFade = Calc.Approach(pauseFade, menu != null ? 1 : 0, Engine.DeltaTime * 8f);
            hud.BackgroundFade = Calc.Approach(hud.BackgroundFade, menu != null ? 0.6f : 0f, Engine.DeltaTime * 3f);
            fade = Calc.Approach(fade, 0f, Engine.DeltaTime);
        }

        public void OpenMenu() {
            pauseSfx();
            Audio.Play("event:/ui/game/pause");
            Add(menu = new TextMenu());
            menu.Add(new TextMenu.Button(Dialog.Clean("intro_vignette_resume")).Pressed(closeMenu));
            menu.Add(new TextMenu.Button(Dialog.Clean("intro_vignette_skip")).Pressed(startGame));
            menu.OnCancel = menu.OnESC = menu.OnPause = closeMenu;
        }

        private void closeMenu() {
            resumeSfx();
            Audio.Play("event:/ui/game/unpause");
            if (menu != null) {
                menu.RemoveSelf();
            }
            menu = null;
        }

        private void startGame() {
            stopSfx();
            textCoroutine = null;
            session.Audio.Music.Event = areaMusic;
            if (menu != null) {
                menu.RemoveSelf();
                menu = null;
            }
            var fadeWipe = new FadeWipe(this, false, delegate {
                Engine.Scene = new LevelLoader(session);
            });
            fadeWipe.OnUpdate = delegate (float f) {
                textAlpha = Math.Min(textAlpha, 1f - f);
            };
            exiting = true;
        }

        private void transitionToLegendEnd() {
            stopSfx();
            textCoroutine = null;
            if (menu != null) {
                menu.RemoveSelf();
                menu = null;
            }
            var fadeWipe = new FadeWipe(this, false, delegate {
                Engine.Scene = new Cs04LegendEnd(session);
            });
            fadeWipe.OnUpdate = delegate (float f) {
                textAlpha = Math.Min(textAlpha, 1f - f);
            };
            exiting = true;
        }

        public override void Render() {
            base.Render();
            if (fade > 0f || textAlpha > 0f) {
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, null, RasterizerState.CullNone, null, Engine.ScreenMatrix);
                if (fade > 0f) {
                    Draw.Rect(-1f, -1f, 1922f, 1082f, Color.Black * fade);
                }

                // Optional: Add visual effects or background imagery for the legend
                if (textAlpha > 0f) {
                    // You could add starfield, void effects, or legend imagery here
                    // Draw.Rect(0f, 0f, 1920f, 1080f, Color.DarkBlue * (textAlpha * 0.3f));
                }

                Draw.SpriteBatch.End();
            }
        }

        private void pauseSfx() {
            foreach (var component in Tracker.GetComponents<SoundSource>()) {
                var sound = (SoundSource)component;
                sound.Pause();
            }
            // Removed direct FMOD setPaused call - Celeste handles this through SoundSource
        }

        private void resumeSfx() {
            foreach (var component in Tracker.GetComponents<SoundSource>()) {
                var sound = (SoundSource)component;
                sound.Resume();
            }
            // Removed direct FMOD setPaused call - Celeste handles this through SoundSource
        }

        private void stopSfx() {
            var components = new List<Component>();
            components.AddRange(Tracker.GetComponents<SoundSource>());
            foreach (var component in components) {
                var sound = (SoundSource)component;
                sound.RemoveSelf();
            }
            // Removed direct FMOD stop call - Celeste handles this through SoundSource
            if (legendMusicPlaying) {
                legendMusicPlaying = false;
            }
        }
    }
}



