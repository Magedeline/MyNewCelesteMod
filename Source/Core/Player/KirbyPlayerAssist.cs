using DesoloZantas.Core.Core.Extensions;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Kirby dash assist with enhanced visuals showing shoe color and ability hints
    /// </summary>
    [Tracked(false)]
    public class KirbyPlayerAssist : Entity
    {
        public float Direction;
        public float Scale;
        public Vector2 Offset;

        private List<MTexture> images;
        private List<MTexture> kirbyArrows;
        private float timer;
        private bool paused;
        private int lastIndex;
        private bool snapshotEnabled = false;

        // Visual enhancements
        private Color arrowColor = Color.White;
        private bool showShoeColorIndicator = true;
        private bool showAbilityHint = true;

        public KirbyPlayerAssist()
        {
            base.Tag = Tags.Global;
            base.Depth = -1000000;
            Visible = false;
            
            // Try to load Kirby-specific dash arrows, fallback to default
            if (GFX.Game.Has("util/kirby/dasharrow00"))
            {
                kirbyArrows = GFX.Game.GetAtlasSubtextures("util/kirby/dasharrow");
                images = kirbyArrows;
            }
            else
            {
                images = GFX.Game.GetAtlasSubtextures("util/dasharrow/dasharrow");
            }
        }

        public override void Update()
        {
            if (!Engine.DashAssistFreeze)
            {
                if (paused)
                {
                    if (!base.Scene.Paused)
                    {
                        Audio.PauseGameplaySfx = false;
                    }
                    DisableSnapshot();
                    timer = 0f;
                    paused = false;
                }
                return;
            }

            paused = true;
            Audio.PauseGameplaySfx = true;
            timer += Engine.RawDeltaTime;
            
            if (timer > 0.2f && !snapshotEnabled)
            {
                EnableSnapshot();
            }

            global::Celeste.Player entity = base.Scene.Tracker.GetEntity<global::Celeste.Player>();
            if (entity != null)
            {
                float num = Input.GetAimVector(entity.Facing).Angle();
                
                if (Calc.AbsAngleDiff(num, Direction) >= 1.5807964f)
                {
                    Direction = num;
                    Scale = 0f;
                }
                else
                {
                    Direction = Calc.AngleApproach(Direction, num, MathF.PI * 6f * Engine.RawDeltaTime);
                }

                Scale = Calc.Approach(Scale, 1f, Engine.DeltaTime * 4f);
                
                int num2 = 1 + (8 + (int)Math.Round(num / (MathF.PI / 4f))) % 8;
                
                if (lastIndex != 0 && lastIndex != num2)
                {
                    Audio.Play("event:/game/general/assist_dash_aim", entity.Center, "dash_direction", num2);
                }
                
                lastIndex = num2;

                // Update arrow color based on Kirby's state
                UpdateArrowColor(entity);
            }
        }

        /// <summary>
        /// Update arrow color based on Kirby's dash count and ability
        /// </summary>
        private void UpdateArrowColor(global::Celeste.Player player)
        {
            var kirbyComponent = player.Get<KirbyPlayerComponent>();
            
            if (kirbyComponent != null)
            {
                // Get shoe color from Kirby hair if available
                var kirbyHair = player.Get<KirbyHair>();
                if (kirbyHair != null)
                {
                    arrowColor = Color.Lerp(Color.White, kirbyHair.ShoeColor, 0.5f);
                }
                else
                {
                    arrowColor = GetDashCountColor(player.Dashes);
                }

                // Tint with ability color if active
                if (kirbyComponent.CurrentPower != KirbyPlayerComponent.PowerState.None)
                {
                    Color abilityColor = GetAbilityColor(kirbyComponent.CurrentPower);
                    arrowColor = Color.Lerp(arrowColor, abilityColor, 0.3f);
                }
            }
            else
            {
                arrowColor = GetDashCountColor(player.Dashes);
            }
        }

        /// <summary>
        /// Get color based on dash count (matching shoe colors)
        /// </summary>
        private Color GetDashCountColor(int dashCount)
        {
            return dashCount switch
            {
                0 => new Color(100, 100, 255),      // Blue
                1 => new Color(255, 180, 180),      // Pastel red
                2 => new Color(200, 100, 255),      // Purple
                3 => new Color(255, 180, 100),      // Orange
                4 => new Color(255, 255, 100),      // Yellow
                5 => new Color(100, 255, 100),      // Green
                >= 10 => new Color(255, 50, 50),    // Red
                _ => Color.Lerp(new Color(100, 255, 100), new Color(255, 50, 50), (dashCount - 5) / 5f)
            };
        }

        /// <summary>
        /// Get color for copy abilities
        /// </summary>
        private Color GetAbilityColor(KirbyPlayerComponent.PowerState ability)
        {
            return ability switch
            {
                KirbyPlayerComponent.PowerState.Fire => new Color(255, 100, 50),
                KirbyPlayerComponent.PowerState.Ice => new Color(100, 200, 255),
                KirbyPlayerComponent.PowerState.Spark => new Color(255, 255, 100),
                KirbyPlayerComponent.PowerState.Stone => new Color(150, 150, 150),
                KirbyPlayerComponent.PowerState.Sword => new Color(200, 200, 255),
                _ => Color.White
            };
        }

        public override void Render()
        {
            global::Celeste.Player entity = base.Scene.Tracker.GetEntity<global::Celeste.Player>();
            
            if (entity == null || !Engine.DashAssistFreeze)
            {
                return;
            }

            MTexture mTexture = null;
            float num = float.MaxValue;
            
            for (int i = 0; i < 8; i++)
            {
                float num2 = Calc.AngleDiff(MathF.PI * 2f * ((float)i / 8f), Direction);
                if (Math.Abs(num2) < Math.Abs(num))
                {
                    num = num2;
                    mTexture = images[i];
                }
            }

            if (mTexture != null)
            {
                if (Math.Abs(num) < 0.05f)
                {
                    num = 0f;
                }

                Vector2 arrowPosition = (entity.Center + Offset + Calc.AngleToVector(Direction, 20f)).Round();
                
                // Draw arrow with Kirby-themed color
                mTexture.DrawOutlineCentered(arrowPosition, arrowColor, Ease.BounceOut(Scale), num);

                // Draw shoe color indicator if enabled
                if (showShoeColorIndicator)
                {
                    RenderShoeColorIndicator(entity, arrowPosition);
                }

                // Draw ability hint if enabled and has ability
                if (showAbilityHint)
                {
                    RenderAbilityHint(entity, arrowPosition);
                }
            }
        }

        /// <summary>
        /// Render small shoe color indicator near the arrow
        /// </summary>
        private void RenderShoeColorIndicator(global::Celeste.Player player, Vector2 arrowPosition)
        {
            var kirbyHair = player.Get<KirbyHair>();
            if (kirbyHair == null)
                return;

            // Draw a small circle showing current shoe color
            Vector2 indicatorPos = arrowPosition + new Vector2(0f, 16f);
            float radius = 3f;
            
            Draw.Circle(indicatorPos, radius, kirbyHair.ShoeColor, 8);
            Draw.Circle(indicatorPos, radius - 1f, Color.Black, 8);
        }

        /// <summary>
        /// Render ability hint icon near the arrow
        /// </summary>
        private void RenderAbilityHint(global::Celeste.Player player, Vector2 arrowPosition)
        {
            var kirbyComponent = player.Get<KirbyPlayerComponent>();
            
            if (kirbyComponent == null || kirbyComponent.CurrentPower == KirbyPlayerComponent.PowerState.None)
                return;

            // Try to get ability icon
            string abilityIconPath = $"characters/kirby/ability_icons/{kirbyComponent.CurrentPower.ToString().ToLower()}";
            
            if (GFX.Game.Has(abilityIconPath))
            {
                MTexture abilityIcon = GFX.Game[abilityIconPath];
                Vector2 iconPos = arrowPosition + new Vector2(0f, -20f);
                
                abilityIcon.DrawCentered(iconPos, Color.White * 0.8f, 0.5f);
            }
            else
            {
                // Fallback: draw ability name
                Vector2 textPos = arrowPosition + new Vector2(0f, -20f);
                string abilityText = kirbyComponent.CurrentPower.ToString();
                
                ActiveFont.DrawOutline(abilityText, textPos, new Vector2(0.5f, 0.5f), 
                    Vector2.One * 0.4f, Color.White, 2f, Color.Black);
            }
        }

        private void EnableSnapshot()
        {
            if (!snapshotEnabled)
            {
                snapshotEnabled = true;
                // Audio snapshot would be enabled here in a real implementation
            }
        }

        private void DisableSnapshot()
        {
            if (snapshotEnabled)
            {
                snapshotEnabled = false;
                // Audio snapshot would be released here in a real implementation
            }
        }

        public override void Removed(Scene scene)
        {
            DisableSnapshot();
            base.Removed(scene);
        }

        public override void SceneEnd(Scene scene)
        {
            DisableSnapshot();
            base.SceneEnd(scene);
        }

        /// <summary>
        /// Toggle shoe color indicator visibility
        /// </summary>
        public void SetShoeColorIndicator(bool enabled)
        {
            showShoeColorIndicator = enabled;
        }

        /// <summary>
        /// Toggle ability hint visibility
        /// </summary>
        public void SetAbilityHint(bool enabled)
        {
            showAbilityHint = enabled;
        }
    }
}




