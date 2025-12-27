#nullable disable
using MonoMod.ModInterop;

namespace DesoloZantas.Core.Core
{
    public static class ModExports
    {
        [ModExportName("Ingeste.Setting")]
        public static class Settings
        {
            public static bool RespawnBehavior()
            {
                // Implement the desired respawn behavior logic here
                // Example: Return true to indicate a specific respawn behavior
                return true;
            }
        }
    }
}



