#nullable enable
namespace DesoloZantas.Core.Core.Mountain
{
    /// <summary>
    /// Manager for DesoloZantas custom mountain rendering
    /// Hooks into the vanilla MountainRenderer to provide custom visuals
    /// </summary>
    public static class DesoloMountainManager
    {
        private static bool _initialized = false;
        private static bool _customMountainActive = false;
        private static DesoloMountainResources? _resources;

        /// <summary>
        /// Whether the custom mountain is currently active
        /// </summary>
        public static bool IsCustomMountainActive => _customMountainActive;

        /// <summary>
        /// The custom mountain resources
        /// </summary>
        public static DesoloMountainResources? Resources => _resources;

        /// <summary>
        /// Initialize the mountain manager and hook into rendering
        /// </summary>
        public static void Initialize()
        {
            if (_initialized) return;

            try
            {
                // Create resources container
                _resources = new DesoloMountainResources();

                // Hook into mountain rendering
                On.Celeste.MountainRenderer.Update += OnMountainRendererUpdate;
                On.Celeste.MountainRenderer.BeforeRender += OnMountainRendererBeforeRender;
                On.Celeste.MountainModel.Update += OnMountainModelUpdate;

                _initialized = true;
                IngesteLogger.Info("DesoloMountainManager initialized");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to initialize DesoloMountainManager");
            }
        }

        /// <summary>
        /// Unload hooks and resources
        /// </summary>
        public static void Unload()
        {
            if (!_initialized) return;

            try
            {
                On.Celeste.MountainRenderer.Update -= OnMountainRendererUpdate;
                On.Celeste.MountainRenderer.BeforeRender -= OnMountainRendererBeforeRender;
                On.Celeste.MountainModel.Update -= OnMountainModelUpdate;

                _resources?.Unload();
                _resources = null;
                _customMountainActive = false;
                _initialized = false;

                IngesteLogger.Info("DesoloMountainManager unloaded");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to unload DesoloMountainManager");
            }
        }

        /// <summary>
        /// Activate custom mountain rendering
        /// </summary>
        public static void ActivateCustomMountain(string resourcePath = "DesoloZantas/Mountain")
        {
            if (_resources == null)
            {
                IngesteLogger.Warn("Cannot activate custom mountain - resources not initialized");
                return;
            }

            if (!_resources.IsLoaded)
            {
                _resources.Load(resourcePath);
            }

            _customMountainActive = true;
            Audio.Play("event:/Ingeste/ui/unlock_newmountian_icon");
            IngesteLogger.Info("Custom mountain activated");
        }

        /// <summary>
        /// Deactivate custom mountain rendering (return to vanilla)
        /// </summary>
        public static void DeactivateCustomMountain()
        {
            _customMountainActive = false;
            IngesteLogger.Info("Custom mountain deactivated");
        }

        /// <summary>
        /// Toggle custom mountain on/off
        /// </summary>
        public static void ToggleCustomMountain()
        {
            if (_customMountainActive)
            {
                DeactivateCustomMountain();
            }
            else
            {
                ActivateCustomMountain();
            }
        }

        private static void OnMountainRendererUpdate(On.Celeste.MountainRenderer.orig_Update orig, MountainRenderer self, Scene scene)
        {
            orig(self, scene);

            // Additional update logic for custom mountain
            if (_customMountainActive && _resources?.IsLoaded == true)
            {
                // Custom update behavior could go here
            }
        }

        private static void OnMountainRendererBeforeRender(On.Celeste.MountainRenderer.orig_BeforeRender orig, MountainRenderer self, Scene scene)
        {
            // Could modify rendering here if needed
            orig(self, scene);
        }

        private static void OnMountainModelUpdate(On.Celeste.MountainModel.orig_Update orig, MountainModel self)
        {
            orig(self);

            // Custom model update logic
            if (_customMountainActive && _resources?.IsLoaded == true)
            {
                // Could modify fog, stars, etc. here
            }
        }

        /// <summary>
        /// Set custom fog colors for the mountain
        /// </summary>
        public static void SetFogColors(MountainRenderer renderer, Color topColor, Color bottomColor)
        {
            if (renderer?.Model == null) return;

            try
            {
                // Access the fog through reflection or public fields if available
                IngesteLogger.Info($"Setting fog colors: top={topColor}, bottom={bottomColor}");
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"Failed to set fog colors: {ex.Message}");
            }
        }

        /// <summary>
        /// Ease the camera to a custom position
        /// </summary>
        public static float EaseCameraTo(MountainRenderer renderer, int area, MountainCamera target, float? duration = null)
        {
            if (renderer == null) return 0f;
            return renderer.EaseCamera(area, target, duration, true, false);
        }

        /// <summary>
        /// Snap the camera to a position instantly
        /// </summary>
        public static void SnapCameraTo(MountainRenderer renderer, int area, MountainCamera target)
        {
            renderer?.SnapCamera(area, target);
        }

        /// <summary>
        /// Get the current camera from the mountain renderer
        /// </summary>
        public static MountainCamera? GetCurrentCamera(MountainRenderer? renderer)
        {
            return renderer?.Camera;
        }

        /// <summary>
        /// Check if the mountain renderer is currently animating
        /// </summary>
        public static bool IsAnimating(MountainRenderer? renderer)
        {
            return renderer?.Animating ?? false;
        }

        /// <summary>
        /// Get the current area being displayed
        /// </summary>
        public static int GetCurrentArea(MountainRenderer? renderer)
        {
            return renderer?.Area ?? -1;
        }
    }
}
