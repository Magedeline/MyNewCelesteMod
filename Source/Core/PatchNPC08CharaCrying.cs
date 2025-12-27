#nullable disable
namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Modifies the method to remove the hardcoded FMOD event string used for playing the footstep sound effect.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    internal class PatchNpcSetupCharaSpriteSoundsAttribute : Attribute
    {
    }
}



