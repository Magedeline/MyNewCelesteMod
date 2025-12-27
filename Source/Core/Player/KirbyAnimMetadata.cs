namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Metadata for Kirby animation frames including hair, hat, and carry positions
    /// </summary>
    public class KirbyAnimMetadata
    {
        /// <summary>
        /// Offset for hair rendering
        /// </summary>
        public Vector2 HairOffset;

        /// <summary>
        /// Offset for ability hat rendering
        /// </summary>
        public Vector2 HatOffset;

        /// <summary>
        /// Animation frame index
        /// </summary>
        public int Frame;

        /// <summary>
        /// Y offset for carrying objects
        /// </summary>
        public int CarryYOffset;

        /// <summary>
        /// Whether this frame should render hair
        /// </summary>
        public bool HasHair;

        /// <summary>
        /// Whether this frame should render an ability hat
        /// </summary>
        public bool HasHat;

        /// <summary>
        /// Optional: Shoe color override for special animations
        /// </summary>
        public Color? ShoeColorOverride;

        /// <summary>
        /// Optional: Scale multiplier for this frame
        /// </summary>
        public float ScaleMultiplier = 1f;

        /// <summary>
        /// Optional: Custom color tint for ability effects
        /// </summary>
        public Color? AbilityTint;

        /// <summary>
        /// Whether this frame is part of a combat animation
        /// </summary>
        public bool IsCombatFrame;

        /// <summary>
        /// Hit detection offset for combat frames
        /// </summary>
        public Vector2 HitboxOffset;

        /// <summary>
        /// Hit detection size for combat frames
        /// </summary>
        public Vector2 HitboxSize;

        public KirbyAnimMetadata()
        {
            HairOffset = Vector2.Zero;
            HatOffset = new Vector2(0f, -12f); // Default hat position above Kirby
            Frame = 0;
            CarryYOffset = 0;
            HasHair = true;
            HasHat = false;
            ShoeColorOverride = null;
            ScaleMultiplier = 1f;
            AbilityTint = null;
            IsCombatFrame = false;
            HitboxOffset = Vector2.Zero;
            HitboxSize = new Vector2(16f, 16f);
        }

        /// <summary>
        /// Create metadata with custom values
        /// </summary>
        public KirbyAnimMetadata(Vector2 hairOffset, Vector2 hatOffset, bool hasHair = true, bool hasHat = false)
        {
            HairOffset = hairOffset;
            HatOffset = hatOffset;
            Frame = 0;
            CarryYOffset = 0;
            HasHair = hasHair;
            HasHat = hasHat;
            ShoeColorOverride = null;
            ScaleMultiplier = 1f;
            AbilityTint = null;
        }

        /// <summary>
        /// Clone this metadata
        /// </summary>
        public KirbyAnimMetadata Clone()
        {
            return new KirbyAnimMetadata
            {
                HairOffset = this.HairOffset,
                HatOffset = this.HatOffset,
                Frame = this.Frame,
                CarryYOffset = this.CarryYOffset,
                HasHair = this.HasHair,
                HasHat = this.HasHat,
                ShoeColorOverride = this.ShoeColorOverride,
                ScaleMultiplier = this.ScaleMultiplier,
                AbilityTint = this.AbilityTint,
                IsCombatFrame = this.IsCombatFrame,
                HitboxOffset = this.HitboxOffset,
                HitboxSize = this.HitboxSize
            };
        }

        /// <summary>
        /// Set shoe color override
        /// </summary>
        public KirbyAnimMetadata WithShoeColor(Color color)
        {
            var clone = Clone();
            clone.ShoeColorOverride = color;
            return clone;
        }

        /// <summary>
        /// Set ability tint
        /// </summary>
        public KirbyAnimMetadata WithAbilityTint(Color color)
        {
            var clone = Clone();
            clone.AbilityTint = color;
            return clone;
        }

        /// <summary>
        /// Set scale multiplier
        /// </summary>
        public KirbyAnimMetadata WithScale(float scale)
        {
            var clone = Clone();
            clone.ScaleMultiplier = scale;
            return clone;
        }
    }
}




