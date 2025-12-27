using DesoloZantas.Core.Core.Extensions;
using DesoloZantas.Core.Core.Player;

namespace DesoloZantas.Core.Core.Triggers {
    [CustomEntity("Ingeste/Kirby_Player_Trigger")]
    [Tracked]
    public class KirbyPlayerTrigger : Trigger {
        public enum ActivationType {
            OnEnter,    // Activate when player enters
            OnExit,     // Activate when player exits
            Toggle      // Toggle between normal and Kirby mode
        }

        public enum TransformationType {
            Instant,    // Immediate transformation
            Animated,   // Play transformation animation
            Fade        // Fade out/in transformation
        }

        private ActivationType activationType;
        private TransformationType transformationType;
        private bool oneUse;
        private bool hasTriggered;
        private string transformAnimation;
        private float transformDuration;
        private bool preserveVelocity;
        private bool playerWasInside;

        // Visual effects
        private float particleTimer;
        private Tween.TweenMode Oneshot;

        public KirbyPlayerTrigger(EntityData data, Vector2 offset) : base(data, offset) {
            activationType = data.Enum<ActivationType>(nameof(activationType), ActivationType.OnEnter);
            transformationType = data.Enum<TransformationType>(nameof(transformationType), TransformationType.Animated);
            oneUse = data.Bool(nameof(oneUse), false);
            transformAnimation = data.Attr(nameof(transformAnimation), "transform");
            transformDuration = data.Float(nameof(transformDuration), 1.0f);
            preserveVelocity = data.Bool(nameof(preserveVelocity), true);
            hasTriggered = false;
            playerWasInside = false;
        }

        public override void Update() {
            base.Update();

            // Reset trigger for non-one-use triggers  
            if (!oneUse && activationType != ActivationType.Toggle) {
                var player = Scene.Tracker.GetEntity<global::Celeste.Player>();
                if (player != null && !CollideCheck(player)) {
                    hasTriggered = false;
                }
            }

            // Update particle effects
            particleTimer += Engine.DeltaTime;
            if (particleTimer > 0.3f) {
                particleTimer = 0f;
                // Add ambient sparkle effect around the trigger
                if (Calc.Random.Chance(0.3f)) {
                    Vector2 sparklePos = Position + new Vector2(
                        Calc.Random.Range(0, (int)Width),
                        Calc.Random.Range(0, (int)Height)
                    );

                    // Create simple sparkle visual
                    var sparkle = new Entity(sparklePos);
                    sparkle.Depth = -10000;
                    sparkle.Add(Tween.Create(Oneshot, null, 0.5f, true));
                    Scene.Add(sparkle);
                }
            }
        }

        public override void OnEnter(global::Celeste.Player player) {
            base.OnEnter(player);
            if (activationType == ActivationType.OnEnter) {
                triggerTransformation(player);
            }
        }

        public override void OnLeave(global::Celeste.Player player) {
            base.OnLeave(player);
            if (activationType == ActivationType.OnExit) {
                triggerTransformation(player);
            }
        }

        private void triggerTransformation(global::Celeste.Player player) {
            if (oneUse && hasTriggered) return;
            if (player == null) return;

            hasTriggered = true;

            // Check if player is in Kirby mode
            if (player.IsKirbyMode()) {
                // Transform back to normal player
                transformToNormalPlayer(player);
            } else {
                // Transform to Kirby player
                transformToKirbyPlayer(player);
            }
        }

        private void transformToNormalPlayer(global::Celeste.Player player) {
            if (player == null) return;

            // Play transformation effect
            playTransformationEffect(player.Position);

            // Disable Kirby mode
            player.DisableKirbyMode();
            
            IngesteLogger.Info("Player transformed back to normal");
        }

        private void transformToKirbyPlayer(global::Celeste.Player player) {
            if (player == null) return;
            
            // Play transformation effect
            playTransformationEffect(player.Position);

            // Enable Kirby mode
            player.EnableKirbyMode();
            
            IngesteLogger.Info("Player transformed to Kirby");
        }

        private void playTransformationEffect(Vector2 position) {
            // Create visual effect based on transformation type
            switch (transformationType) {
                case TransformationType.Instant:
                    createInstantEffect(position);
                    break;
                case TransformationType.Animated:
                    createAnimatedEffect(position);
                    break;
                case TransformationType.Fade:
                    createFadeEffect(position);
                    break;
            }
        }

        private void createInstantEffect(Vector2 position) {
            // Simple visual flash effect
            var flash = new Entity(position);
            flash.Depth = -10000;
            // Use Tween instead of Alarm
            flash.Add(Tween.Create(Oneshot, null, 0.2f));
            Scene.Add(flash);
        }

        private void createAnimatedEffect(Vector2 position) {
            // TODO: Implement animated transformation effect
            createInstantEffect(position); // Fallback for now
        }

        private void createFadeEffect(Vector2 position) {
            // TODO: Implement fade transformation effect
            createInstantEffect(position); // Fallback for now
        }

        public override void Render() {
            base.Render();

            // Debug rendering
            if (Engine.Commands.Open) {
                Draw.HollowRect(Collider, Color.Yellow);
                var center = Center;
                Draw.Line(center - Vector2.UnitX * 4, center + Vector2.UnitX * 4, Color.Yellow);
                Draw.Line(center - Vector2.UnitY * 4, center + Vector2.UnitY * 4, Color.Yellow);
            }
        }
    }
}



