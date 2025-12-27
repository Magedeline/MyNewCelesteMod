namespace DesoloZantas.Core.Core
{
    public abstract class WaveFazePage
    {
        public WaveFazePresentation Presentation;
        public Color ClearColor;
        public Transitions Transition;
        public bool AutoProgress;
        public bool WaitingForInput;

        public int Width => Presentation.ScreenWidth;

        public int Height => Presentation.ScreenHeight;

        public abstract IEnumerator Routine();

        public virtual void Added(WaveFazePresentation presentation) => Presentation = presentation;

        public abstract void Update();

        public virtual void Render()
        {
            // Clear the screen with the specified clear color
            Engine.Graphics.GraphicsDevice.Clear(ClearColor);

            // Begin the TheoSprite batch for rendering
            Draw.SpriteBatch.Begin();

            // Render any specific content for the current page
            if (Presentation != null)
                // Example rendering logic for a generic page
                ActiveFont.DrawOutline(Dialog.Clean("WAVEFAZE_PAGE_TITLE"), new Vector2(Width / 2f, Height / 4f),
                    new Vector2(0.5f, 0.5f), Vector2.One, Color.White, 2f, Color.Black);

            // End the TheoSprite batch
            Draw.SpriteBatch.End();
        }

        protected IEnumerator PressButton()
        {
            WaitingForInput = true;
            while (!Input.MenuConfirm.Pressed)
                yield return null;
            WaitingForInput = false;
            Audio.Play("event:/new_content/game/10_farewell/ppt_mouseclick");
        }

        public enum Transitions
        {
            ScaleIn,
            FadeIn,
            Rotate3D,
            Blocky,
            Spiral,
        }
    }
}




