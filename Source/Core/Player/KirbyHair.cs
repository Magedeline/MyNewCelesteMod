namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Kirby hair component with customizable color system based on dash count
    /// Includes shoe color changes for visual feedback
    /// </summary>
    [Tracked(false)]
    public class KirbyHair : Component
    {
        public const string Hair = "characters/kirby/hair00";

        public Color Color = global::Celeste.Player.NormalHairColor;
        public Color Border = Color.Black;
        public Color ShoeColor = new Color(100, 100, 255); // Default blue shoes
        public float Alpha = 1f;
        public Facings Facing;
        public bool DrawPlayerSpriteOutline;
        public bool SimulateMotion = true;

        public Vector2 StepPerSegment = new Vector2(0f, 2f);
        public float StepInFacingPerSegment = 0.5f;
        public float StepApproach = 64f;
        public float StepYSinePerSegment;

        public Sprite Sprite;
        public int HairCount { get; set; } = 4;
        public List<Vector2> Nodes = new List<Vector2>();

        private List<MTexture> bangs = GFX.Game.GetAtlasSubtextures("characters/kirby/bangs");
        private float wave;

        // Dash-based shoe colors
        private static readonly Dictionary<int, Color> DashToShoeColor = new Dictionary<int, Color>
        {
            { 0, new Color(100, 100, 255) },      // Blue (0 dash)
            { 1, new Color(255, 180, 180) },      // Pastel red (1 dash)
            { 2, new Color(200, 100, 255) },      // Purple (2 dashes)
            { 3, new Color(255, 180, 100) },      // Orange (3 dashes)
            { 4, new Color(255, 255, 100) },      // Yellow (4 dashes)
            { 5, new Color(100, 255, 100) },      // Green (5 dashes)
            { 10, new Color(255, 50, 50) }        // Red (10 dashes)
        };

        public float Wave => wave;

        public KirbyHair(Sprite sprite)
            : base(active: true, visible: true)
        {
            Sprite = sprite;
            // Initialize nodes based on hair count
            for (int i = 0; i < HairCount; i++)
            {
                Nodes.Add(Vector2.Zero);
            }
        }

        public void Start()
        {
            Vector2 value = base.Entity.Position + new Vector2((0 - Facing) * 200, 200f);
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i] = value;
            }
        }

        public void AfterUpdate()
        {
            Vector2 hairOffset = GetSpriteHairOffset();
            Vector2 vector = hairOffset * new Vector2((float)Facing, 1f);
            Nodes[0] = Sprite.RenderPosition + new Vector2(0f, -9f * Sprite.Scale.Y) + vector;
            Vector2 target = Nodes[0] + new Vector2((float)(0 - Facing) * StepInFacingPerSegment * 2f, (float)Math.Sin(wave) * StepYSinePerSegment) + StepPerSegment;
            Vector2 vector2 = Nodes[0];
            float num = 3f;

            for (int i = 1; i < HairCount; i++)
            {
                if (i >= Nodes.Count)
                {
                    Nodes.Add(Nodes[i - 1]);
                }

                if (SimulateMotion)
                {
                    float num2 = (1f - (float)i / (float)HairCount * 0.5f) * StepApproach;
                    Nodes[i] = Calc.Approach(Nodes[i], target, num2 * Engine.DeltaTime);
                }

                if ((Nodes[i] - vector2).Length() > num)
                {
                    Nodes[i] = vector2 + (Nodes[i] - vector2).SafeNormalize() * num;
                }

                target = Nodes[i] + new Vector2((float)(0 - Facing) * StepInFacingPerSegment, (float)Math.Sin(wave + (float)i * 0.8f) * StepYSinePerSegment) + StepPerSegment;
                vector2 = Nodes[i];
            }
        }

        public override void Update()
        {
            wave += Engine.DeltaTime * 4f;
            
            // Update shoe color based on dash count
            if (Entity is global::Celeste.Player player)
            {
                UpdateShoeColor(player.Dashes);
            }
            
            base.Update();
        }

        /// <summary>
        /// Update shoe color based on dash count
        /// </summary>
        private void UpdateShoeColor(int dashCount)
        {
            if (DashToShoeColor.TryGetValue(dashCount, out Color color))
            {
                ShoeColor = color;
            }
            else if (dashCount > 10)
            {
                // For more than 10 dashes, use red
                ShoeColor = DashToShoeColor[10];
            }
            else if (dashCount > 5)
            {
                // Interpolate between green (5) and red (10)
                float t = (dashCount - 5) / 5f;
                ShoeColor = Color.Lerp(DashToShoeColor[5], DashToShoeColor[10], t);
            }
        }

        public void MoveHairBy(Vector2 amount)
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                Nodes[i] += amount;
            }
        }

        public override void Render()
        {
            if (!HasHair())
            {
                return;
            }

            Vector2 origin = new Vector2(5f, 5f);
            Color color = Border * Alpha;

            if (DrawPlayerSpriteOutline && Sprite != null)
            {
                Color color2 = Sprite.Color;
                Vector2 position = Sprite.Position;
                Sprite.Color = color;
                Sprite.Position = position + new Vector2(0f, -1f);
                Sprite.Render();
                Sprite.Position = position + new Vector2(0f, 1f);
                Sprite.Render();
                Sprite.Position = position + new Vector2(-1f, 0f);
                Sprite.Render();
                Sprite.Position = position + new Vector2(1f, 0f);
                Sprite.Render();
                Sprite.Color = color2;
                Sprite.Position = position;
            }

            Nodes[0] = Nodes[0].Floor();
            
            // Render hair outline
            if (color.A > 0)
            {
                for (int i = 0; i < HairCount; i++)
                {
                    MTexture hairTexture = GetHairTexture(i);
                    Vector2 hairScale = GetHairScale(i);
                    hairTexture.Draw(Nodes[i] + new Vector2(-1f, 0f), origin, color, hairScale);
                    hairTexture.Draw(Nodes[i] + new Vector2(1f, 0f), origin, color, hairScale);
                    hairTexture.Draw(Nodes[i] + new Vector2(0f, -1f), origin, color, hairScale);
                    hairTexture.Draw(Nodes[i] + new Vector2(0f, 1f), origin, color, hairScale);
                }
            }

            // Render hair
            for (int num = HairCount - 1; num >= 0; num--)
            {
                GetHairTexture(num).Draw(Nodes[num], origin, GetHairColor(num), GetHairScale(num));
            }
            
            // Render Kirby's shoes with dash-based color
            RenderShoes();
        }

        /// <summary>
        /// Render Kirby's shoes with color based on dash count
        /// </summary>
        private void RenderShoes()
        {
            if (Entity == null || Sprite == null)
                return;

            // Get shoe texture (assumes shoes are part of Kirby sprite)
            if (GFX.Game.Has("characters/kirby/shoes"))
            {
                MTexture shoeTexture = GFX.Game["characters/kirby/shoes"];
                Vector2 shoePosition = Sprite.RenderPosition + new Vector2(0f, 6f);
                shoeTexture.Draw(shoePosition, Vector2.Zero, ShoeColor * Alpha, Sprite.Scale);
            }
        }

        private Vector2 GetHairScale(int index)
        {
            float num = 0.25f + (1f - (float)index / (float)HairCount) * 0.75f;
            return new Vector2(((index == 0) ? ((float)Facing) : num) * Math.Abs(Sprite.Scale.X), num);
        }

        public MTexture GetHairTexture(int index)
        {
            if (index == 0 && bangs.Count > 0)
            {
                int hairFrame = GetSpriteHairFrame();
                if (hairFrame < bangs.Count)
                {
                    return bangs[hairFrame];
                }
                return bangs[0];
            }
            return GFX.Game["characters/kirby/hair00"];
        }

        public Vector2 PublicGetHairScale(int index)
        {
            return GetHairScale(index);
        }

        public Color GetHairColor(int index)
        {
            return Color * Alpha;
        }

        /// <summary>
        /// Get the current shoe color description for UI/tips
        /// </summary>
        public string GetShoeColorDescription()
        {
            if (Entity is global::Celeste.Player player)
            {
                return player.Dashes switch
                {
                    0 => "Blue (No dashes)",
                    1 => "Pastel Red (1 dash)",
                    2 => "Purple (2 dashes)",
                    3 => "Orange (3 dashes)",
                    4 => "Yellow (4 dashes)",
                    5 => "Green (5 dashes)",
                    >= 10 => "Red (Max dashes)",
                    _ => $"Custom ({player.Dashes} dashes)"
                };
            }
            return "Unknown";
        }

        /// <summary>
        /// Check if the sprite has hair (safely handles different sprite types)
        /// </summary>
        private bool HasHair()
        {
            if (Sprite == null)
                return false;

            // Check if it's a KirbyPlayerSprite
            if (Sprite is KirbyPlayerSprite kirbySprite)
            {
                return kirbySprite.HasHair;
            }

            // For regular sprites, assume they have hair
            return true;
        }

        /// <summary>
        /// Get hair offset from sprite (safely handles different sprite types)
        /// </summary>
        private Vector2 GetSpriteHairOffset()
        {
            if (Sprite is KirbyPlayerSprite kirbySprite)
            {
                return kirbySprite.HairOffset;
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Get hair frame from sprite (safely handles different sprite types)
        /// </summary>
        private int GetSpriteHairFrame()
        {
            if (Sprite is KirbyPlayerSprite kirbySprite)
            {
                return kirbySprite.HairFrame;
            }
            return 0;
        }
    }
}




