using FMOD.Studio;
using CelesteAudio = global::Celeste.Audio;

namespace DesoloZantas.Core.Core.AudioSystems
{
    /// <summary>
    /// Helper class to safely handle audio when banks might be missing.
    /// </summary>
    public static class AudioHelper
    {
        /// <summary>
        /// Safely plays an audio event, falling back to a default sound if the event doesn't exist
        /// </summary>
        public static EventInstance PlaySafe(string eventPath, string fallbackEvent = null)
        {
            try
            {
                // Try to play the requested event
                var instance = CelesteAudio.Play(eventPath);
                
                // Check if the event actually loaded
                if (instance.isValid())
                {
                    return instance;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "DesoloZantas", $"Failed to play audio event '{eventPath}': {ex.Message}");
            }
            
            // Try fallback event if provided
            if (!string.IsNullOrEmpty(fallbackEvent))
            {
                try
                {
                    return CelesteAudio.Play(fallbackEvent);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Warn, "DesoloZantas", $"Fallback audio event '{fallbackEvent}' also failed: {ex.Message}");
                }
            }
            
            return default(EventInstance);
        }
        
        /// <summary>
        /// Safely sets music, falling back to silence if the track doesn't exist
        /// </summary>
        public static bool SetMusicSafe(string musicEvent, bool startPlaying = true, bool allowFadeOut = true)
        {
            if (string.IsNullOrEmpty(musicEvent))
            {
                CelesteAudio.SetMusic(null);
                return true;
            }
            
            try
            {
                CelesteAudio.SetMusic(musicEvent, startPlaying, allowFadeOut);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warn, "DesoloZantas", $"Failed to set music '{musicEvent}': {ex.Message}");
                Logger.Log(LogLevel.Info, "DesoloZantas", "Make sure you have the required .bank files in the Audio folder");
                return false;
            }
        }
    }
}




