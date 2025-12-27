namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Giygas-style backdrop effect inspired by Earthbound's final boss
    /// Features unsettling swirling patterns, color distortion, and organic movement
    /// </summary>
    public class GiygasBackdrop : Backdrop
    {
        private struct WaveLayer
        {
            public float Phase;
            public float Speed;
            public float Amplitude;
            public float Frequency;
            public Color BaseColor;
            public float Alpha;
            public float VerticalOffset;
        }

        private struct DistortionRipple
        {
            public Vector2 Position;
            public float Radius;
            public float Strength;
            public float GrowthRate;
            public float Lifetime;
            public float MaxLifetime;
        }

        private const int WAVE_LAYERS = 8;
        private const int RIPPLE_COUNT = 12;
        private const int VERTICAL_SEGMENTS = 60;
        private const int HORIZONTAL_SEGMENTS = 80;

        private readonly WaveLayer[] waveLayers;
        private readonly DistortionRipple[] ripples;
        private VirtualRenderTarget renderTarget;
        private readonly VertexPositionColorTexture[] vertices;
        
        // Add background texture
        private readonly MTexture backgroundTexture;
        private readonly float backgroundAlpha = 0.5f;
        private Vector2 backgroundScroll;
        
        private float globalTime;
        private float colorShiftTime;
        private float intensityPulse;
        private Vector2 cameraOffset;
        
        // Configuration
        public float Intensity = 1f;
        public new float Speed = 1f;
        public float ColorShiftSpeed = 0.5f;
        public float DistortionStrength = 1f;
        public Color BaseColor1 = new(80, 0, 0);      // Dark red
        public Color BaseColor2 = new(120, 0, 60);    // Dark magenta
        public Color BaseColor3 = new(40, 0, 80);     // Dark purple
        public Color AccentColor = new(255, 100, 100); // Bright red accent

        public GiygasBackdrop()
        {
            // Initialize wave layers with varying properties for organic movement
            waveLayers = new WaveLayer[WAVE_LAYERS];
            for (int i = 0; i < WAVE_LAYERS; i++)
            {
                float layerFactor = (float)i / WAVE_LAYERS;
                waveLayers[i] = new WaveLayer
                {
                    Phase = Calc.Random.NextFloat() * MathHelper.TwoPi,
                    Speed = Calc.Random.Range(0.3f, 1.2f),
                    Amplitude = Calc.Random.Range(20f, 60f) * (1f + layerFactor),
                    Frequency = Calc.Random.Range(0.5f, 2.5f),
                    BaseColor = Color.Lerp(BaseColor1, BaseColor2, layerFactor),
                    Alpha = Calc.Random.Range(0.4f, 0.8f),
                    VerticalOffset = layerFactor * 180f
                };
            }

            // Initialize distortion ripples
            ripples = new DistortionRipple[RIPPLE_COUNT];
            for (int i = 0; i < RIPPLE_COUNT; i++)
            {
                ResetRipple(ref ripples[i]);
            }

            // Initialize vertex buffer for distorted mesh
            int vertexCount = (HORIZONTAL_SEGMENTS + 1) * (VERTICAL_SEGMENTS + 1) * 6;
            vertices = new VertexPositionColorTexture[vertexCount];
        }

        private static void ResetRipple(ref DistortionRipple ripple)
        {
            ripple.Position = new Vector2(
                Calc.Random.Range(-50f, 370f),
                Calc.Random.Range(-50f, 230f)
            );
            ripple.Radius = 0f;
            ripple.Strength = Calc.Random.Range(0.5f, 1.5f);
            ripple.GrowthRate = Calc.Random.Range(30f, 80f);
            ripple.Lifetime = 0f;
            ripple.MaxLifetime = Calc.Random.Range(3f, 8f);
        }

        public override void Update(Scene scene)
        {
            base.Update(scene);

            if (!Visible)
                return;

            // Update time variables
            globalTime += Engine.DeltaTime * Speed;
            colorShiftTime += Engine.DeltaTime * ColorShiftSpeed;
            intensityPulse = (float)Math.Sin(globalTime * 0.7f) * 0.3f + 0.7f;

            // Update wave layers
            for (int i = 0; i < WAVE_LAYERS; i++)
            {
                waveLayers[i].Phase += Engine.DeltaTime * waveLayers[i].Speed;
                
                // Organic color shifting
                float colorShift = (float)Math.Sin(colorShiftTime + i * 0.5f) * 0.5f + 0.5f;
                waveLayers[i].BaseColor = Color.Lerp(
                    Color.Lerp(BaseColor1, BaseColor2, (float)i / WAVE_LAYERS),
                    Color.Lerp(BaseColor2, BaseColor3, colorShift),
                    intensityPulse
                );
            }

            // Update distortion ripples
            for (int i = 0; i < RIPPLE_COUNT; i++)
            {
                ripples[i].Lifetime += Engine.DeltaTime;
                ripples[i].Radius += ripples[i].GrowthRate * Engine.DeltaTime;

                if (ripples[i].Lifetime >= ripples[i].MaxLifetime)
                {
                    ResetRipple(ref ripples[i]);
                }
            }

            // Camera tracking for parallax
            if (scene is Level level)
            {
                Vector2 targetOffset = level.Camera.Position * 0.1f;
                cameraOffset += (targetOffset - cameraOffset) * (1f - (float)Math.Pow(0.01, Engine.DeltaTime));
            }
        }

        public override void BeforeRender(Scene scene)
        {
            if (renderTarget == null || renderTarget.IsDisposed)
            {
                renderTarget = VirtualContent.CreateRenderTarget("Giygas Backdrop", 320, 180);
            }

            // Render to texture
            Engine.Graphics.GraphicsDevice.SetRenderTarget(renderTarget);
            Engine.Graphics.GraphicsDevice.Clear(BaseColor1 * 0.5f);

            // Draw background texture first if available
            if (backgroundTexture != null)
            {
                Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                
                // Apply scrolling effect based on time
                backgroundScroll = new Vector2(
                    (float)Math.Sin(globalTime * 0.1f) * 10f,
                    (float)Math.Cos(globalTime * 0.15f) * 10f
                );
                
                // Draw the background with distortion
                float scale = 1.0f + (float)Math.Sin(globalTime * 0.3f) * 0.1f;
                Vector2 origin = new Vector2(backgroundTexture.Width / 2f, backgroundTexture.Height / 2f);
                Vector2 position = new Vector2(160f, 90f) + backgroundScroll - cameraOffset * 0.2f;
                
                Draw.SpriteBatch.Draw(
                    backgroundTexture.Texture.Texture_Safe,
                    position,
                    backgroundTexture.ClipRect,
                    Color.White * backgroundAlpha * Intensity,
                    globalTime * 0.05f, // Slow rotation
                    origin,
                    scale * new Vector2(320f / backgroundTexture.Width, 180f / backgroundTexture.Height),
                    SpriteEffects.None,
                    0f
                );
                
                Draw.SpriteBatch.End();
            }

            // Build distorted mesh
            BuildDistortedMesh();

            // Render the mesh
            if (vertices.Length > 0)
            {
                GFX.DrawVertices(Matrix.Identity, vertices, vertices.Length, GFX.FxTexture);
            }

            // Add accent highlights
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            
            for (int i = 0; i < RIPPLE_COUNT; i++)
            {
                float rippleAlpha = 1f - (ripples[i].Lifetime / ripples[i].MaxLifetime);
                if (rippleAlpha > 0)
                {
                    float rippleSize = ripples[i].Radius * 0.5f;
                    Vector2 ripplePos = ripples[i].Position - cameraOffset * 0.3f;
                    
                    Color rippleColor = Color.Lerp(AccentColor, BaseColor3, 
                        ripples[i].Lifetime / ripples[i].MaxLifetime) * rippleAlpha * 0.3f;
                    
                    Draw.Rect(
                        ripplePos - new Vector2(rippleSize, rippleSize) / 2f,
                        rippleSize,
                        rippleSize,
                        rippleColor
                    );
                }
            }

            Draw.SpriteBatch.End();
        }

        private void BuildDistortedMesh()
        {
            int vertexIndex = 0;
            float segmentWidth = 320f / HORIZONTAL_SEGMENTS;
            float segmentHeight = 180f / VERTICAL_SEGMENTS;

            for (int y = 0; y < VERTICAL_SEGMENTS; y++)
            {
                for (int x = 0; x < HORIZONTAL_SEGMENTS; x++)
                {
                    // Calculate base positions
                    Vector2 topLeft = new Vector2(x * segmentWidth, y * segmentHeight);
                    Vector2 topRight = new Vector2((x + 1) * segmentWidth, y * segmentHeight);
                    Vector2 bottomLeft = new Vector2(x * segmentWidth, (y + 1) * segmentHeight);
                    Vector2 bottomRight = new Vector2((x + 1) * segmentWidth, (y + 1) * segmentHeight);

                    // Apply multi-layered distortion
                    topLeft = ApplyDistortion(topLeft);
                    topRight = ApplyDistortion(topRight);
                    bottomLeft = ApplyDistortion(bottomLeft);
                    bottomRight = ApplyDistortion(bottomRight);

                    // Calculate colors based on wave layers
                    Color colorTL = CalculateVertexColor(topLeft, y);
                    Color colorTR = CalculateVertexColor(topRight, y);
                    Color colorBL = CalculateVertexColor(bottomLeft, y + 1);
                    Color colorBR = CalculateVertexColor(bottomRight, y + 1);

                    // Create two triangles for the quad
                    // Triangle 1
                    vertices[vertexIndex++] = new VertexPositionColorTexture(
                        new Vector3(topLeft, 0), colorTL, Vector2.Zero);
                    vertices[vertexIndex++] = new VertexPositionColorTexture(
                        new Vector3(topRight, 0), colorTR, Vector2.Zero);
                    vertices[vertexIndex++] = new VertexPositionColorTexture(
                        new Vector3(bottomLeft, 0), colorBL, Vector2.Zero);

                    // Triangle 2
                    vertices[vertexIndex++] = new VertexPositionColorTexture(
                        new Vector3(topRight, 0), colorTR, Vector2.Zero);
                    vertices[vertexIndex++] = new VertexPositionColorTexture(
                        new Vector3(bottomRight, 0), colorBR, Vector2.Zero);
                    vertices[vertexIndex++] = new VertexPositionColorTexture(
                        new Vector3(bottomLeft, 0), colorBL, Vector2.Zero);
                }
            }
        }

        private Vector2 ApplyDistortion(Vector2 pos)
        {
            Vector2 distortedPos = pos;

            // Apply wave distortions from multiple layers
            float totalDistortionX = 0f;
            float totalDistortionY = 0f;

            for (int i = 0; i < WAVE_LAYERS; i++)
            {
                float waveX = (float)Math.Sin(
                    pos.Y * waveLayers[i].Frequency * 0.02f + waveLayers[i].Phase
                ) * waveLayers[i].Amplitude * 0.3f;

                float waveY = (float)Math.Cos(
                    pos.X * waveLayers[i].Frequency * 0.02f + waveLayers[i].Phase * 1.3f
                ) * waveLayers[i].Amplitude * 0.2f;

                totalDistortionX += waveX * waveLayers[i].Alpha;
                totalDistortionY += waveY * waveLayers[i].Alpha;
            }

            distortedPos.X += totalDistortionX * Intensity * DistortionStrength;
            distortedPos.Y += totalDistortionY * Intensity * DistortionStrength;

            // Apply ripple distortions
            for (int i = 0; i < RIPPLE_COUNT; i++)
            {
                Vector2 toRipple = pos - ripples[i].Position;
                float distance = toRipple.Length();
                
                if (distance < ripples[i].Radius && distance > 0)
                {
                    float rippleInfluence = 1f - (distance / ripples[i].Radius);
                    float rippleAlpha = 1f - (ripples[i].Lifetime / ripples[i].MaxLifetime);
                    
                    float distortAmount = (float)Math.Sin(distance * 0.1f - ripples[i].Lifetime * 3f) 
                        * ripples[i].Strength * rippleInfluence * rippleAlpha * 15f;
                    
                    Vector2 distortDir = Vector2.Normalize(toRipple);
                    distortedPos += distortDir * distortAmount * Intensity * DistortionStrength;
                }
            }

            // Add subtle organic movement
            float organicX = (float)Math.Sin(globalTime * 0.3f + pos.Y * 0.01f) * 5f;
            float organicY = (float)Math.Cos(globalTime * 0.4f + pos.X * 0.01f) * 5f;
            distortedPos += new Vector2(organicX, organicY) * Intensity;

            return distortedPos;
        }

        private Color CalculateVertexColor(Vector2 pos, int ySegment)
        {
            Vector3 resultColorVec = Vector3.Zero;
            float totalWeight = 0f;

            // Blend multiple wave layer colors
            for (int i = 0; i < WAVE_LAYERS; i++)
            {
                float influence = (float)Math.Sin(
                    pos.Y * 0.02f + waveLayers[i].Phase + pos.X * 0.01f
                ) * 0.5f + 0.5f;

                influence *= waveLayers[i].Alpha;
                
                // Add vertical gradient influence
                float verticalInfluence = (float)ySegment / VERTICAL_SEGMENTS;
                influence *= (1f - verticalInfluence * 0.5f);

                // Convert color to vector for math operations
                Vector3 colorVec = waveLayers[i].BaseColor.ToVector3();
                resultColorVec += colorVec * influence;
                totalWeight += influence;
            }

            Color resultColor;
            if (totalWeight > 0)
            {
                resultColorVec /= totalWeight;
                resultColor = new Color(resultColorVec);
            }
            else
            {
                resultColor = Color.Black;
            }

            // Add accent color based on intensity
            float accentInfluence = (float)Math.Sin(
                globalTime * 2f + pos.X * 0.03f + pos.Y * 0.02f
            ) * 0.5f + 0.5f;
            
            accentInfluence *= intensityPulse * 0.3f;
            resultColor = Color.Lerp(resultColor, AccentColor, accentInfluence);

            // Apply global intensity
            resultColor *= Intensity * FadeAlphaMultiplier;

            return resultColor;
        }

        public override void Render(Scene scene)
        {
            if (renderTarget != null && !renderTarget.IsDisposed && Visible)
            {
                Vector2 renderPos = new Vector2(160, 90) - cameraOffset;
                Vector2 origin = new Vector2(renderTarget.Width, renderTarget.Height) / 2f;
                
                Draw.SpriteBatch.Draw(
                    (RenderTarget2D)renderTarget,
                    renderPos,
                    renderTarget.Bounds,
                    Color.White * FadeAlphaMultiplier * Intensity,
                    0f,
                    origin,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }
        }

        public override void Ended(Scene scene)
        {
            base.Ended(scene);
            
            if (renderTarget != null)
            {
                renderTarget.Dispose();
                renderTarget = null;
            }
        }
    }
}




