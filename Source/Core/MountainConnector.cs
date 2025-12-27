#nullable enable
using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Rewritten connector which safely manages a primary overworld mountain renderer
    /// and a lazily-created secondary renderer (e.g. unlocked after progression) without
    /// assuming presence of a custom Model object (some installs were crashing when the
    /// provided model asset failed to load). It defers secondary creation until needed
    /// and wraps all MountainRenderer calls in try/catch so a bad mountain never hard-crashes.
    /// </summary>
    public class MountainConnector : Entity {
        private readonly MountainRenderer primary;
        private MountainRenderer? secondary; // created on-demand
        private bool secondaryVisible;
        private bool triedCreateSecondary;

        public readonly Vector3 ConnectionPoint;
        public readonly Vector3 SecondaryStartPoint;

        /// <summary>
        /// Custom Maggy3D marker for the mountain (replaces Maddy3D when enabled)
        /// </summary>
        public Maggy3D? MaggyMarker { get; private set; }
        
        /// <summary>
        /// Whether to use Maggy3D instead of Maddy3D as the mountain marker
        /// </summary>
        public bool UseMaggyMarker { get; set; }
        
        /// <summary>
        /// Stored position for the marker
        /// </summary>
        private Vector3 markerPosition = Vector3.Zero;

        public MountainConnector(MountainRenderer primaryMountain, Vector3 connectionPoint, Vector3 secondaryStartPoint) {
            primary = primaryMountain ?? throw new ArgumentNullException(nameof(primaryMountain));
            ConnectionPoint = connectionPoint;
            SecondaryStartPoint = secondaryStartPoint;
            Depth = -10000; // render behind Oui / UI
            Add(new Coroutine(deferredUnlockCheck()));
        }

        private IEnumerator deferredUnlockCheck() {
            // Wait one frame so SaveData.Instance is definitely populated.
            yield return null;
            tryAutoUnlock();
            
            // Initialize Maggy marker if enabled
            if (UseMaggyMarker && MaggyMarker == null) {
                InitializeMaggyMarker();
            }
        }

        private void InitializeMaggyMarker() {
            try {
                MaggyMarker = new Maggy3D(primary);
                Scene?.Add(MaggyMarker);
                IngesteLogger.Info("Maggy3D marker initialized on mountain");
            } catch (Exception ex) {
                IngesteLogger.Warn($"Failed to initialize Maggy3D marker: {ex.Message}");
                MaggyMarker = null;
            }
        }

        /// <summary>
        /// Enable and show the Maggy3D marker, optionally with a custom texture path
        /// </summary>
        public void EnableMaggyMarker(string? customMarkerPath = null) {
            UseMaggyMarker = true;
            
            if (MaggyMarker == null) {
                InitializeMaggyMarker();
            }
            
            if (MaggyMarker != null) {
                if (customMarkerPath != null) {
                    MaggyMarker.SetCustomMarker(customMarkerPath);
                }
                MaggyMarker.Show = true;
            }
        }

        /// <summary>
        /// Disable and hide the Maggy3D marker
        /// </summary>
        public void DisableMaggyMarker() {
            UseMaggyMarker = false;
            if (MaggyMarker != null) {
                MaggyMarker.Show = false;
            }
        }
        
        /// <summary>
        /// Set the position for the Maggy marker
        /// </summary>
        public void SetMarkerPosition(Vector3 position) {
            markerPosition = position;
            if (MaggyMarker != null) {
                MaggyMarker.Position = position;
            }
        }

        private void tryAutoUnlock() {
            if (secondaryVisible || triedCreateSecondary)
                return;
            if (SaveData.Instance != null && SaveData.Instance.UnlockedAreas >= 2) {
                UnlockSecondary();
            }
        }

        public void UnlockSecondary() {
            if (secondaryVisible)
                return;
            ensureSecondary();
            if (secondary == null)
                return; // creation failed gracefully
            secondaryVisible = true;
            secondary.Visible = true;
            Audio.Play("event:/Ingeste/ui/unlock_newmountian_icon");
            Add(new Coroutine(unlockAnimation()));
        }

        private void ensureSecondary() {
            if (secondary != null || triedCreateSecondary)
                return;
            triedCreateSecondary = true;
            try {
                secondary = new MountainRenderer {
                    Visible = false
                };
                Scene?.Add(secondary);
            } catch (Exception ex) {
                Logger.Log(LogLevel.Warn, nameof(DesoloZantas), $"Secondary MountainRenderer creation failed: {ex.Message}");
                secondary = null;
            }
        }

        private IEnumerator unlockAnimation() {
            const float duration = 1.8f;
            float t = 0f;
            while (t < duration) {
                t += Engine.DeltaTime;
                // In the future we could move/scale the secondary towards ConnectionPoint.
                yield return null;
            }
            
            // Trigger Maggy wiggle if active
            MaggyMarker?.TriggerWiggle();
        }

        public override void Update() {
            base.Update();
            tryAutoUnlock();

            safeUpdate(primary);
            if (secondaryVisible)
                safeUpdate(secondary);
            
            // Update Maggy marker position
            if (UseMaggyMarker && MaggyMarker != null) {
                MaggyMarker.Position = markerPosition;
            }
        }

        private void safeUpdate(MountainRenderer? renderer) {
            if (renderer == null) return;
            try { renderer.Update(null); } catch (Exception ex) {
                Logger.Log(LogLevel.Warn, nameof(DesoloZantas), $"MountainRenderer.Update failed: {ex.Message}");
            }
        }

        public override void Render() {
            base.Render();
            safeRender(primary);
            if (secondaryVisible)
                safeRender(secondary);
        }

        private void safeRender(MountainRenderer? renderer) {
            if (renderer == null) return;
            try { renderer.Render(null); } catch (Exception ex) {
                Logger.Log(LogLevel.Warn, nameof(DesoloZantas), $"MountainRenderer.Render failed: {ex.Message}");
            }
        }
    }
}




