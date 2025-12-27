namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Represents possible sprite-related errors that can occur during cutscenes and entity rendering
    /// </summary>
    public enum SpriteErrors
    {
        /// <summary>
        /// No error occurred
        /// </summary>
        None = 0,

        /// <summary>
        /// Scale value is invalid or missing
        /// </summary>
        Scale = 1,

        /// <summary>
        /// Sprite component is null or missing
        /// </summary>
        Sprite = 2,

        /// <summary>
        /// Failed to set sprite scale
        /// </summary>
        ScaleUpdate = 3,

        /// <summary>
        /// Invalid sprite transformation
        /// </summary>
        Transformation = 4,

        /// <summary>
        /// Sprite animation frame is missing
        /// </summary>
        AnimationFrame = 5,

        /// <summary>
        /// Error when attempting to flip sprite
        /// </summary>
        Flip = 6
    }
}



