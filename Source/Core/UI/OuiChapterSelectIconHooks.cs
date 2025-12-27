using System;
using System.Collections.Generic;
using System.Reflection;
using Celeste;
using Celeste.Mod;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.DesoloZatnas.Core.UI
{
    /// <summary>
    /// Hooks for OuiChapterSelectIcon to reduce spacing between chapter icons
    /// This makes the icons "stick together" in the chapter select screen
    /// without triggering map selection on navigation
    /// </summary>
    public static class OuiChapterSelectIconHooks
    {
        private static bool hooksInstalled = false;
        
        // Reduced spacing factor - smaller = icons closer together
        // 0.5f means half the normal spacing
        private const float SPACING_MULTIPLIER = 0.50f;
        
        // Minimum distance between icon centers (prevents overlap)
        private const float MIN_ICON_SPACING = 85f;
        
        // Maximum distance between icon centers
        private const float MAX_ICON_SPACING = 120f;

        // Track if we've already adjusted this instance
        private static HashSet<int> adjustedInstances = new HashSet<int>();

        public static void Install()
        {
            if (hooksInstalled)
                return;

            try
            {
                Logger.Log(LogLevel.Info, "DesoloZatnas", "[OuiChapterSelectIconHooks] Installing chapter icon spacing hooks...");

                // Hook into OuiChapterSelect
                On.Celeste.OuiChapterSelect.Added += OuiChapterSelect_Added;
                On.Celeste.OuiChapterSelect.Update += OuiChapterSelect_Update;

                hooksInstalled = true;
                Logger.Log(LogLevel.Info, "DesoloZatnas", "[OuiChapterSelectIconHooks] Hooks installed successfully");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "DesoloZatnas", $"[OuiChapterSelectIconHooks] Failed to install hooks: {ex}");
            }
        }

        public static void Uninstall()
        {
            if (!hooksInstalled)
                return;

            try
            {
                Logger.Log(LogLevel.Info, "DesoloZatnas", "[OuiChapterSelectIconHooks] Uninstalling chapter icon spacing hooks...");

                On.Celeste.OuiChapterSelect.Added -= OuiChapterSelect_Added;
                On.Celeste.OuiChapterSelect.Update -= OuiChapterSelect_Update;
                
                adjustedInstances.Clear();

                hooksInstalled = false;
                Logger.Log(LogLevel.Info, "DesoloZatnas", "[OuiChapterSelectIconHooks] Hooks uninstalled successfully");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "DesoloZatnas", $"[OuiChapterSelectIconHooks] Failed to uninstall hooks: {ex}");
            }
        }

        private static void OuiChapterSelect_Added(On.Celeste.OuiChapterSelect.orig_Added orig, OuiChapterSelect self, Scene scene)
        {
            // Call original to create all icons first
            orig(self, scene);
            
            // After the original creates icons, reposition them with tighter spacing
            RepositionIconsWithTighterSpacing(self, true);
        }

        private static void OuiChapterSelect_Update(On.Celeste.OuiChapterSelect.orig_Update orig, OuiChapterSelect self)
        {
            orig(self);
            
            // Continuously reapply spacing adjustments to maintain tight positioning
            // This ensures icons don't drift back to vanilla positions during navigation
            RepositionIconsWithTighterSpacing(self, false);
        }

        /// <summary>
        /// Repositions all chapter select icons to be closer together horizontally
        /// </summary>
        private static void RepositionIconsWithTighterSpacing(OuiChapterSelect chapterSelect, bool initialSetup)
        {
            try
            {
                // Access the icons list via reflection
                var iconsField = typeof(OuiChapterSelect).GetField("icons", BindingFlags.NonPublic | BindingFlags.Instance);
                if (iconsField == null)
                    return;

                var icons = iconsField.GetValue(chapterSelect) as List<OuiChapterSelectIcon>;
                if (icons == null || icons.Count <= 1)
                    return;

                // Get the area (selected index) field
                var areaField = typeof(OuiChapterSelect).GetField("area", BindingFlags.NonPublic | BindingFlags.Instance);
                int selectedArea = areaField != null ? (int)areaField.GetValue(chapterSelect) : 0;

                if (initialSetup)
                {
                    Logger.Log(LogLevel.Info, "DesoloZatnas", $"[OuiChapterSelectIconHooks] Initial setup: Adjusting spacing for {icons.Count} chapter icons");
                }

                // Calculate the screen center and new tight spacing
                float screenCenterX = 960f;
                
                // Calculate new tighter spacing
                float newSpacing = Math.Max(MIN_ICON_SPACING, MAX_ICON_SPACING * SPACING_MULTIPLIER);
                
                // Calculate total width with new spacing
                float totalWidth = (icons.Count - 1) * newSpacing;
                float startX = screenCenterX - (totalWidth / 2f);

                // Offset based on selected area to keep selected icon centered
                float offsetToCenter = (selectedArea * newSpacing) - (totalWidth / 2f);
                startX = screenCenterX - offsetToCenter - (newSpacing * selectedArea);
                
                // Clamp start position to prevent icons going too far off screen
                float minStartX = -200f;
                float maxStartX = 1920f - totalWidth + 200f;
                startX = Math.Max(minStartX, Math.Min(maxStartX, screenCenterX - offsetToCenter));

                // Reposition each icon to be tightly packed
                for (int i = 0; i < icons.Count; i++)
                {
                    OuiChapterSelectIcon icon = icons[i];
                    if (icon == null) continue;

                    float newX = startX + (i * newSpacing);
                    
                    // Smoothly interpolate to new position during updates
                    if (!initialSetup)
                    {
                        float currentX = icon.Position.X;
                        newX = MathHelper.Lerp(currentX, newX, 0.15f); // Smooth transition
                    }
                    
                    float y = icon.Position.Y;
                    icon.Position = new Vector2(newX, y);

                    // Also update the internal position fields used for animation
                    UpdateIconInternalPositions(icon, newX);
                }

                if (initialSetup)
                {
                    Logger.Log(LogLevel.Info, "DesoloZatnas", $"[OuiChapterSelectIconHooks] Icons repositioned with spacing: {newSpacing:F0}px");
                }
            }
            catch (Exception ex)
            {
                if (initialSetup)
                {
                    Logger.Log(LogLevel.Error, "DesoloZatnas", $"[OuiChapterSelectIconHooks] Error repositioning icons: {ex}");
                }
            }
        }

        /// <summary>
        /// Updates the internal position fields of an icon to maintain consistent positioning
        /// </summary>
        private static void UpdateIconInternalPositions(OuiChapterSelectIcon icon, float newX)
        {
            try
            {
                // Try to update IdlePosition
                var idleField = typeof(OuiChapterSelectIcon).GetField("IdlePosition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (idleField != null)
                {
                    Vector2 idlePos = (Vector2)idleField.GetValue(icon);
                    idleField.SetValue(icon, new Vector2(newX, idlePos.Y));
                }

                // Try to update SelectPosition if it exists
                var selectField = typeof(OuiChapterSelectIcon).GetField("SelectPosition", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (selectField != null)
                {
                    Vector2 selectPos = (Vector2)selectField.GetValue(icon);
                    selectField.SetValue(icon, new Vector2(newX, selectPos.Y));
                }
            }
            catch
            {
                // Silently ignore - these fields may not exist in all versions
            }
        }
    }
}
