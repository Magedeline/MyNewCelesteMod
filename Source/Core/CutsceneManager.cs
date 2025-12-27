namespace DesoloZantas.Core.Core
{
    public class CutsceneManager : Entity, IEnumerable<Component>, IEnumerable
    {
        private static Dictionary<string, Func<IEnumerator>> cutsceneRegistry;

        public static void RegisterCutscene(string id, Func<IEnumerator> cutscene)
        {
            cutsceneRegistry ??= new Dictionary<string, Func<IEnumerator>>();
            cutsceneRegistry[id] = cutscene;
        }

        public static IEnumerator PlayCutscene(string id, Level level)
        {
            if (cutsceneRegistry != null && cutsceneRegistry.TryGetValue(id, out var cutscene))
            {
                return cutscene();
            }
            return null;
        }

        internal static void Initialize()
        {
            cutsceneRegistry = new Dictionary<string, Func<IEnumerator>>();
        }

        // Fix for CS0117: Add the missing Cleanup method  
        public static void Cleanup()
        {
            cutsceneRegistry?.Clear();
        }
    }
}




