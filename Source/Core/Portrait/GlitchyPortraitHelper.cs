namespace DesoloZantas.Core.Core.Portrait
{
    /// <summary>
    /// Helper class for managing glitchy portrait effects in dialog and cutscenes
    /// Provides easy-to-use methods for triggering glitchy Chara portraits
    /// </summary>
    public static class GlitchyPortraitHelper
    {
        // Mapping of dialog expressions to glitch levels
        private static readonly Dictionary<string, Effects.GlitchyPortraitEffect.GlitchLevel> ExpressionToGlitchLevel = 
            new Dictionary<string, Effects.GlitchyPortraitEffect.GlitchLevel>(StringComparer.OrdinalIgnoreCase)
        {
            { "glitchy", Effects.GlitchyPortraitEffect.GlitchLevel.Subtle },
            { "glitchycreepy", Effects.GlitchyPortraitEffect.GlitchLevel.High },
            { "glitchyangry", Effects.GlitchyPortraitEffect.GlitchLevel.High },
            { "glitchysmirk", Effects.GlitchyPortraitEffect.GlitchLevel.Moderate },
            { "glitchyfreak", Effects.GlitchyPortraitEffect.GlitchLevel.Chaos },
            { "glitchyrevenge", Effects.GlitchyPortraitEffect.GlitchLevel.High },
            { "glitchypanic", Effects.GlitchyPortraitEffect.GlitchLevel.Extreme }
        };

        /// <summary>
        /// Gets the glitch level for a given portrait expression
        /// </summary>
        public static Effects.GlitchyPortraitEffect.GlitchLevel GetGlitchLevelForExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return Effects.GlitchyPortraitEffect.GlitchLevel.None;

            if (ExpressionToGlitchLevel.TryGetValue(expression, out var level))
                return level;

            // Check if it contains "glitchy" as a fallback
            if (expression.IndexOf("glitchy", StringComparison.OrdinalIgnoreCase) >= 0)
                return Effects.GlitchyPortraitEffect.GlitchLevel.Moderate;

            return Effects.GlitchyPortraitEffect.GlitchLevel.None;
        }

        /// <summary>
        /// Checks if an expression is a glitchy variant
        /// </summary>
        public static bool IsGlitchyExpression(string expression)
        {
            return GetGlitchLevelForExpression(expression) != Effects.GlitchyPortraitEffect.GlitchLevel.None;
        }

        /// <summary>
        /// Creates a formatted dialog tag for glitchy Chara portraits
        /// </summary>
        /// <param name="baseExpression">Base expression (e.g., "creepy", "angry")</param>
        /// <param name="makeGlitchy">Whether to make it glitchy</param>
        /// <returns>Dialog tag string like "portrait chara glitchycreepy"</returns>
        public static string CreateCharaPortraitTag(string baseExpression, bool makeGlitchy = false)
        {
            string expression = makeGlitchy ? $"glitchy{baseExpression}" : baseExpression;
            return $"portrait chara {expression}";
        }

        /// <summary>
        /// Creates a list of common glitchy Chara expressions for reference
        /// </summary>
        public static List<string> GetAvailableGlitchyExpressions()
        {
            return new List<string>
            {
                "glitchy",          // Subtle distortion
                "glitchycreepy",    // Intense creepy glitch
                "glitchyangry",     // Unstable rage
                "glitchysmirk",     // Menacing glitch
                "glitchyfreak",     // Maximum chaos
                "glitchyrevenge",   // Corrupted determination
                "glitchypanic"      // Reality breaking
            };
        }

        /// <summary>
        /// Gets a description of glitch intensity for designers
        /// </summary>
        public static string GetGlitchDescription(string expression)
        {
            var level = GetGlitchLevelForExpression(expression);
            
            switch (level)
            {
                case Effects.GlitchyPortraitEffect.GlitchLevel.None:
                    return "No glitch effect";
                case Effects.GlitchyPortraitEffect.GlitchLevel.Subtle:
                    return "Subtle distortion - barely noticeable, eerie";
                case Effects.GlitchyPortraitEffect.GlitchLevel.Moderate:
                    return "Moderate glitching - clearly visible, unsettling";
                case Effects.GlitchyPortraitEffect.GlitchLevel.High:
                    return "High intensity - strong visual corruption";
                case Effects.GlitchyPortraitEffect.GlitchLevel.Extreme:
                    return "Extreme glitching - reality breaking apart";
                case Effects.GlitchyPortraitEffect.GlitchLevel.Chaos:
                    return "Maximum chaos - complete visual breakdown";
                default:
                    return "Unknown glitch level";
            }
        }

        /// <summary>
        /// Example usage documentation for dialog authors
        /// </summary>
        public static string GetUsageExample()
        {
            return @"
// In dialog files (e.g., English.txt), use glitchy portrait tags like:

DIALOG_EXAMPLE_1
    {portrait chara glitchy} # Subtle glitch
    Greetings.
    
DIALOG_EXAMPLE_2
    {portrait chara glitchycreepy} # Intense creepy glitch
    Do you know who I am?
    
DIALOG_EXAMPLE_3
    {portrait chara glitchyfreak} # Maximum chaos
    IT'S KILL OR BE KILLED!

// Available glitchy expressions:
// - glitchy (subtle)
// - glitchycreepy (high intensity creepy)
// - glitchyangry (high intensity angry)
// - glitchysmirk (moderate menacing)
// - glitchyfreak (maximum chaos)
// - glitchyrevenge (high intensity determined)
// - glitchypanic (extreme reality breaking)
";
        }
    }
}




