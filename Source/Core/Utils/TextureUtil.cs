#nullable enable
namespace DesoloZantas.Core.Core.Utils
{
    // Renamed to avoid duplicate class definition
    public static class TextureUtil
    {
        public static Texture2D? TryGetOverworldTexture(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            // Attempt to load the texture
            return LoadTexture(name);

            Texture2D? LoadTexture(string textureName)
            {
                try
                {
                    // Assuming a method to load texture from the game's content pipeline
                    return Celeste.Celeste.Instance.Content.Load<Texture2D>(textureName);
                }
                catch
                {
                    // Log the error if necessary
                    return null;
                }
            }
        }
    }
}



