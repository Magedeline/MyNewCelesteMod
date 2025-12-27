using DesoloZantas.Core.Core.AudioSystems;

namespace DesoloZantas.Core.Core.Triggers
{
    /// <summary>
    /// Enhanced music trigger with fade effects and volume control
    /// </summary>
    [CustomEntity("Ingeste/MusicTrigger")]
    public class EnhancedMusicTrigger : Trigger
    {
        private string musicTrack;
        private float fadeTime;
        private bool loop;
        private float volume;
        private bool hasTriggered;

        public EnhancedMusicTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            musicTrack = data.Attr(nameof(musicTrack), "desolo_theme");
            fadeTime = data.Float(nameof(fadeTime), 1.0f);
            loop = data.Bool(nameof(loop), true);
            volume = data.Float(nameof(volume), 1.0f);
            hasTriggered = false;
        }

        public override void OnEnter(global::Celeste.Player player)
        {
            base.OnEnter(player);

            if (!hasTriggered)
            {
                hasTriggered = true;
                changeMusic();
            }
        }

        public override void OnLeave(global::Celeste.Player player)
        {
            base.OnLeave(player);

            // Allow retriggering when leaving and entering again
            hasTriggered = false;
        }

        private void changeMusic()
        {
            if (string.IsNullOrEmpty(musicTrack))
            {
                // Stop music
                Audio.SetMusic(null);
                return;
            }

            try
            {
                // Set the new music track using safe helper
                if (!AudioHelper.SetMusicSafe(musicTrack, startPlaying: true, allowFadeOut: true))
                {
                    // If it fails, log a helpful message
                    Logger.Log(LogLevel.Warn, nameof(DesoloZantas), 
                        $"Could not play music '{musicTrack}'. Check that audio banks are installed.");
                }

                // Set volume if specified
                if (volume != 1.0f)
                {
                    Audio.SetMusicParam(nameof(volume), volume);
                }
            }
            catch (System.Exception ex)
            {
                Logger.Log(LogLevel.Warn, nameof(DesoloZantas), $"Failed to set music '{musicTrack}': {ex.Message}");
            }
        }
    }
}




