using System.Collections.ObjectModel;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Helper class to determine if we're running in Everest/modded environment
    /// and provide safe fallbacks for initialization
    /// </summary>
    internal static class EverestHelper
    {
        /// <summary>
        /// Check if we're running in a modded environment with Everest
        /// </summary>
        public static bool IsEverestLoaded
        {
            get
            {
                try
                {
                    return typeof(Everest.Loader) != null;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Safe access to Everest modules
        /// </summary>
        public static bool TryGetEverestModules(out ReadOnlyCollection<EverestModule> modules)
        {
            modules = null;
            try
            {
                if (IsEverestLoaded)
                {
                    modules = Everest.Modules;
                    return true;
                }
            }
            catch
            {
                // Ignore exceptions
            }
            return false;
        }
    }
}



