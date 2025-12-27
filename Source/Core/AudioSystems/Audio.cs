using FMOD.Studio;
using CelesteAudio = global::Celeste.Audio;

namespace DesoloZantas.Core.Core.AudioSystems
{
    /// <summary>
    /// Proxy class that forwards all Audio calls to the actual Celeste.Audio class.
    /// This class exists to resolve namespace conflicts.
    /// All code should use this Audio class instead of Celeste.Audio directly.
    /// </summary>
    public static class Audio
    {
        // Forward all calls to the actual Celeste Audio class
        public static EventInstance Play(string path) => CelesteAudio.Play(path);
        public static EventInstance Play(string path, Vector2 position) => CelesteAudio.Play(path, position);
        public static EventInstance Play(string path, string param, float value) => CelesteAudio.Play(path, param, value);
        
        public static void SetMusic(string path, bool startPlaying = true, bool allowFadeOut = true) 
            => CelesteAudio.SetMusic(path, startPlaying, allowFadeOut);
        
        public static void SetMusicParam(string param, float value) => CelesteAudio.SetMusicParam(param, value);
        
        public static void SetParameter(EventInstance instance, string param, float value) 
            => CelesteAudio.SetParameter(instance, param, value);
        
        public static void Position(EventInstance instance, Vector2 position) 
            => CelesteAudio.Position(instance, position);
        
        public static void Stop(EventInstance instance, bool allowFadeout = true) 
            => CelesteAudio.Stop(instance, allowFadeout);
        
        public static EventInstance CurrentMusicEventInstance => CelesteAudio.CurrentMusicEventInstance;
        
        public static string CurrentMusic => CelesteAudio.CurrentMusic;
    }
}




