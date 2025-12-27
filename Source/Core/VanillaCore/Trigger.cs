namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Base Trigger class ported from vanilla Celeste.
    /// Extended with DesoloZantas-specific functionality.
    /// </summary>
    [Tracked(true)]
    public abstract class Trigger : Entity
    {
        public enum PositionModes
        {
            NoEffect,
            HorizontalCenter,
            VerticalCenter,
            TopToBottom,
            BottomToTop,
            LeftToRight,
            RightToLeft,
        }

        public bool Triggered;
        public bool PlayerIsInside { get; private set; }

        // DesoloZantas extensions
        public bool RequiresKirbyMode { get; protected set; }
        public bool DisabledInKirbyMode { get; protected set; }
        public string RequiredFlag { get; protected set; }

        public Trigger(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            Collider = new Hitbox(data.Width, data.Height);
            Visible = false;
            
            // Load DesoloZantas-specific properties if present
            RequiresKirbyMode = data.Bool("requiresKirbyMode", false);
            DisabledInKirbyMode = data.Bool("disabledInKirbyMode", false);
            RequiredFlag = data.Attr("requiredFlag", "");
        }

        /// <summary>
        /// Check if the trigger should be active based on DesoloZantas conditions
        /// </summary>
        protected virtual bool ShouldActivate(global::Celeste.Player player)
        {
            Level level = Scene as Level;
            if (level == null) return false;

            // Check required flag
            if (!string.IsNullOrEmpty(RequiredFlag) && !level.Session.GetFlag(RequiredFlag))
                return false;

            // Check Kirby mode requirements
            bool isKirbyMode = level.Session.GetFlag("kirby_mode");
            if (RequiresKirbyMode && !isKirbyMode)
                return false;
            if (DisabledInKirbyMode && isKirbyMode)
                return false;

            return true;
        }

        public virtual void OnEnter(global::Celeste.Player player)
        {
            PlayerIsInside = true;
        }

        public virtual void OnStay(global::Celeste.Player player)
        {
        }

        public virtual void OnLeave(global::Celeste.Player player)
        {
            PlayerIsInside = false;
        }

        protected float GetPositionLerp(global::Celeste.Player player, PositionModes mode)
        {
            return mode switch
            {
                PositionModes.HorizontalCenter => Math.Min(
                    Calc.ClampedMap(player.CenterX, Left, CenterX),
                    Calc.ClampedMap(player.CenterX, Right, CenterX)),
                PositionModes.VerticalCenter => Math.Min(
                    Calc.ClampedMap(player.CenterY, Top, CenterY),
                    Calc.ClampedMap(player.CenterY, Bottom, CenterY)),
                PositionModes.TopToBottom => Calc.ClampedMap(player.CenterY, Top, Bottom),
                PositionModes.BottomToTop => Calc.ClampedMap(player.CenterY, Bottom, Top),
                PositionModes.LeftToRight => Calc.ClampedMap(player.CenterX, Left, Right),
                PositionModes.RightToLeft => Calc.ClampedMap(player.CenterX, Right, Left),
                _ => 1f,
            };
        }

        /// <summary>
        /// Helper to get the percentage through the trigger horizontally
        /// </summary>
        protected float GetHorizontalProgress(global::Celeste.Player player)
        {
            return Calc.ClampedMap(player.CenterX, Left, Right, 0f, 1f);
        }

        /// <summary>
        /// Helper to get the percentage through the trigger vertically
        /// </summary>
        protected float GetVerticalProgress(global::Celeste.Player player)
        {
            return Calc.ClampedMap(player.CenterY, Top, Bottom, 0f, 1f);
        }
    }
}
