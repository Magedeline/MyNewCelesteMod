# HLSL Shader Effects System

This directory contains the HLSL shader effect system integrated from [CelesteEffects](https://github.com/NoelFB/CelesteEffects) by Noel Berry (MIT License).

## Directory Structure

```
Graphics/Effects/
├── Source/           # HLSL source files (.fx, .fxh)
│   ├── Common.fxh
│   ├── Border.fx
│   ├── ColorGrade.fx
│   ├── Dither.fx
│   ├── Distort.fx
│   ├── Dust.fx
│   ├── GaussianBlur.fx
│   ├── Glitch.fx
│   ├── Lighting.fx
│   ├── MagicGlow.fx
│   ├── Mirrors.fx
│   ├── MountainRender.fx
│   ├── Primitive.fx
│   └── Texture.fx
└── Compiled/         # Compiled shader bytecode (.xnb or .mgfx)
    └── (build artifacts go here)
```

## Available Effects

### 1. **Distortion Effect** (`Distort.fx`)
- **Purpose**: Water-like distortion, chromatic aberration, displacement
- **Parameters**:
  - `anxiety`: Chromatic aberration intensity (0.0-1.0)
  - `waterAlpha`: Water effect opacity (0.0-1.0)
  - `gamerate`: Animation speed multiplier (0.1-5.0)
- **Use Cases**: Underwater scenes, anxiety sequences, heat waves

### 2. **Glitch Effect** (`Glitch.fx`)
- **Purpose**: Digital corruption, glitch art, VHS effects
- **Parameters**:
  - `glitch`: Glitch intensity (0.0-1.0)
  - `amplitude`: Displacement amount (0.0-0.5)
  - `timer`: Time value for animation
  - `randomSeed`: Randomization seed
- **Use Cases**: Digital world corruption, boss transitions, cyber aesthetic

### 3. **Color Grading** (`ColorGrade.fx`)
- **Purpose**: LUT-based color correction and artistic color palettes
- **Parameters**:
  - `LutTexture`: 16x16x16 LUT texture
  - `intensity`: Effect strength (0.0-1.0)
  - `blend`: Blend between two LUTs (0.0-1.0)
- **Use Cases**: Chapter mood setting, dream sequences, time-of-day effects
- **LUT Presets**: cold, warm, sepia, noir, golden_hour

### 4. **Gaussian Blur** (`GaussianBlur.fx`)
- **Purpose**: Multi-tap blur for depth of field and background blur
- **Techniques**: GaussianBlur9, GaussianBlur5, GaussianBlur3
- **Parameters**:
  - `offset`: Blur direction and distance
  - `fade`: Blur intensity (0.0-1.0)
- **Use Cases**: Background blur, depth of field, focus effects

### 5. **Dither** (`Dither.fx`)
- **Purpose**: Retro-style 4x4 Bayer matrix dithering
- **Techniques**: Dither, InvertDither
- **Use Cases**: Retro aesthetic, transition effects, memory sequences

### 6. **Dust Particles** (`Dust.fx`)
- **Purpose**: Edge-detected noise-based particle effects
- **Parameters**:
  - `color1`, `color2`, `color3`: Three-color gradient
  - `timer`: Animation time
- **Use Cases**: Ancient ruins, magical atmospheres, floating particles

### 7. **Border/Outline** (`Border.fx`)
- **Purpose**: Pixel-perfect edge detection and outlining
- **Parameters**:
  - `thickness`: Outline width in pixels
  - `borderColor`: Outline color
- **Use Cases**: Character highlighting, UI emphasis, cartoon shading

### 8. **Light Gradient** (`Lighting.fx`)
- **Purpose**: Masked gradient lighting with vertex colors
- **Parameters**:
  - `lightDirection`: Direction of gradient
- **Use Cases**: Atmospheric lighting, temple illumination

### 9. **Magic Glow** (`MagicGlow.fx`)
- **Purpose**: Directional glow with noise distortion (16-tap)
- **Parameters**:
  - `direction`: Glow direction vector
  - `intensity`: Glow strength (0.0-2.0)
- **Use Cases**: Magic attacks, power-ups, Dream Friends abilities

### 10. **Mirror Reflection** (`Mirrors.fx`)
- **Purpose**: Reflection effect with customizable tint
- **Parameters**:
  - `offset`: Reflection offset
  - `tintColor`: Reflection color tint
- **Use Cases**: Mirror Temple integration, reflective surfaces

### 11-13. **Utility Shaders**
- **MountainRender.fx**: 3D mountain rendering with AO
- **Primitive.fx**: Basic vertex color shapes
- **Texture.fx**: Standard texture sampling

## Compilation

### Requirements
- **Windows**: DirectX SDK (fxc.exe) or Windows SDK
- **Cross-Platform**: MonoGame mgfxc compiler

### Manual Compilation

**Windows (fxc.exe):**
```powershell
# Shader Model 2.0 (maximum compatibility)
fxc.exe /T fx_2_0 /Fo Compiled/ColorGrade.fxo Source/ColorGrade.fx

# Shader Model 3.0 (better quality, requires GPU support)
fxc.exe /T fx_3_0 /Fo Compiled/Distort.fxo Source/Distort.fx
```

**MonoGame (mgfxc):**
```bash
mgfxc Source/Glitch.fx Compiled/Glitch.mgfx
```

### Docker Build Integration

Add to `Dockerfile.build`:
```dockerfile
# Install shader compiler (example for Wine + fxc)
RUN apt-get install -y wine fxc

# Copy shader sources
COPY Graphics/Effects/Source/*.fx /tmp/shaders/

# Compile all shaders
RUN for fx in /tmp/shaders/*.fx; do \
      fxc /T fx_2_0 /Fo /tmp/compiled/$(basename $fx .fx).fxo $fx; \
    done

# Copy compiled shaders to mod directory
COPY /tmp/compiled/*.fxo Graphics/Effects/Compiled/
```

## Usage in C#

### Basic Effect Application

```csharp
using Celeste.Mod.Ingeste.Core.Effects.ShaderEffects;

// In your Backdrop or Entity class
public class MyCustomEffect : Backdrop
{
    private Effect shader;
    private VirtualRenderTarget buffer;
    
    public MyCustomEffect() : base()
    {
        UseSpritebatch = false;
        shader = EffectManager.LoadEffect("Glitch");
        buffer = VirtualContent.CreateRenderTarget("my_buffer", 
            Engine.Width, Engine.Height);
    }
    
    public override void Render(Scene scene)
    {
        if (shader == null || buffer == null) return;
        
        // Set shader parameters
        shader.Parameters["glitch"]?.SetValue(0.5f);
        shader.Parameters["amplitude"]?.SetValue(0.1f);
        
        // Render with shader
        Draw.SpriteBatch.End();
        Engine.Graphics.GraphicsDevice.SetRenderTarget(buffer);
        
        Draw.SpriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            shader  // Apply shader here
        );
        
        base.Render(scene);
        Draw.SpriteBatch.End();
        
        // Draw result to screen
        Engine.Graphics.GraphicsDevice.SetRenderTarget(null);
        Draw.SpriteBatch.Begin();
        Draw.SpriteBatch.Draw(buffer, Vector2.Zero, Color.White);
        Draw.SpriteBatch.End();
        Draw.SpriteBatch.Begin();
    }
}
```

### Using Pre-Built Effect Classes

```csharp
// Add to map as backdrop
var glitchEffect = new GlitchEffect();
glitchEffect.GlitchAmount = 0.3f;
glitchEffect.Amplitude = 0.08f;

// Trigger dramatic glitch
glitchEffect.TriggerGlitch(duration: 1.0f, intensity: 0.9f);
```

## Usage in Lönn

Effects are automatically available in Lönn's effect panel after adding the `.lua` definitions.

### Distortion Effect
```lua
-- In Lönn, select "Ingeste/DistortionEffect"
-- Configure parameters:
anxiety = 0.5       -- Chromatic aberration
waterAlpha = 0.8    -- Water opacity
gamerate = 1.2      -- Animation speed
```

### Glitch Effect
```lua
-- In Lönn, select "Ingeste/GlitchEffect"
glitchAmount = 0.4  -- Glitch intensity
amplitude = 0.06    -- Displacement
```

### Color Grading
```lua
-- In Lönn, select "Ingeste/ColorGradeEffect"
lutTextureName = "sepia"  -- Preset: cold, warm, sepia, noir, golden_hour
intensity = 0.9           -- Effect strength
```

## Performance Guidelines

### Budget Limits
- **Background Effects**: Max 3-5 active shaders
- **Entity Effects**: Max 10-15 concurrent
- **Post-Processing Passes**: Max 2-3 per frame
- **GPU Memory**: <16MB total for all effects

### Optimization Tips
1. **RenderTarget Pooling**: Reuse buffers when possible
2. **LOD System**: Disable distant effects
3. **Effect Batching**: Group similar shader operations
4. **Shader Model Selection**: Use ps_2_0 for compatibility, ps_3_0 for quality
5. **Parameter Caching**: Avoid setting shader parameters every frame if unchanged

### Performance Profiling
```csharp
// Monitor effect count
int loadedEffects = EffectManager.LoadedEffectCount;
IngesteLogger.Info($"Active effects: {loadedEffects}");

// List loaded effects
foreach (var name in EffectManager.GetLoadedEffectNames())
{
    IngesteLogger.Debug($"Loaded: {name}");
}
```

## Troubleshooting

### Shader Not Loading
```csharp
// Check if effect loaded successfully
var effect = EffectManager.LoadEffect("MyEffect");
if (effect == null)
{
    IngesteLogger.Error("Effect failed to load - check file exists in Graphics/Effects/");
}
```

### Compilation Errors
- **Common.fxh not found**: Ensure `Common.fxh` is in the same directory as other `.fx` files
- **Syntax errors**: Verify shader model compatibility (ps_2_0 vs ps_3_0)
- **Platform-specific issues**: Test on both Windows and FNA builds

### Runtime Errors
- **Black screen**: Check RenderTarget creation and disposal
- **Performance drops**: Reduce active effect count or lower shader quality
- **Crashes**: Ensure proper SpriteBatch.Begin/End pairing

## Integration with Existing Systems

### Kirby Copy Abilities
```csharp
// Example: Glitch ability visual effect
public class GlitchAbility : CopyAbility
{
    private GlitchEffect glitchVisual;
    
    public override void OnActivate()
    {
        glitchVisual = Scene.Tracker.GetEntity<GlitchEffect>();
        if (glitchVisual != null)
        {
            glitchVisual.TriggerGlitch(0.5f, 0.7f);
        }
    }
}
```

### Boss Encounters
```csharp
// Example: Boss phase transition with effects
public class CharaBoss : Boss
{
    private DistortionEffect distortion;
    
    private void OnPhaseChange()
    {
        distortion.Anxiety = 0.8f;  // Intense chromatic aberration
        Tween.Set(this, Tween.TweenMode.Oneshot, 2f, Ease.CubeOut,
            t => distortion.Anxiety = 0f);
    }
}
```

### Chapter-Specific Color Grading
```yaml
# In chapter metadata (Loenn/metadata/chapter_data.yaml)
chapters:
  - id: "digital_world"
    color_grade: "cold"
    grade_intensity: 0.85
```

## Credits

- **Original Shaders**: Noel Berry (NoelFB) - [CelesteEffects](https://github.com/NoelFB/CelesteEffects)
- **License**: MIT License
- **Integration**: Desolo Zantas Development Team
- **Celeste**: Maddy Makes Games

## License

These shader effects are licensed under the MIT License from the original CelesteEffects repository:

```
MIT License

Copyright (c) Noel Berry

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
```

## See Also

- [TEAM_INSTRUCTIONS.md](../../TEAM_INSTRUCTIONS.md) - Project development guidelines
- [DOCUMENTATION_INDEX.md](../../DOCUMENTATION_INDEX.md) - Full documentation index
- [Celeste Mod Community Wiki](https://github.com/EverestAPI/Resources/wiki) - Modding resources
- [CelesteEffects GitHub](https://github.com/NoelFB/CelesteEffects) - Original shader source
