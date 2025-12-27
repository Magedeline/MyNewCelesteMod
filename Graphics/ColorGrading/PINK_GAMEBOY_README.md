# Pink Game Boy Color Grade

## Overview
The Pink Game Boy Color Grade trigger applies a classic Game Boy-style pink/magenta tint to the game, reminiscent of the original Game Boy's pinkish monochrome display.

## Files Created
- **C# Trigger**: `Source/Core/Triggers/PinkGameboyColorGradeTrigger.cs`
- **Lönn Plugin**: `Loenn/triggers/pink_gameboy_colorgrade_trigger.lua`
- **Color Grade LUT**: `Graphics/ColorGrading/pinkgameboy.png` *(needs to be created)*

## How to Create the Color Grade

The `pinkgameboy.png` file is a **LUT (Look-Up Table)** texture that Celeste uses to remap colors. To create this file:

### Option 1: Use an Existing Tool
1. Use a photo editing tool that supports color grading
2. Apply a pink/magenta tint with reduced saturation to create a Game Boy look
3. Export as a 256x16 PNG LUT texture

### Option 2: Manual Creation
1. Start with an existing color grade (e.g., copy `golden.png` or `none.png`)
2. Open in an image editor (Photoshop, GIMP, etc.)
3. Apply the following adjustments:
   - Desaturate to near-monochrome
   - Apply a pink/magenta color overlay (RGB: ~255, 180, 200)
   - Reduce overall saturation to 20-30%
   - Adjust contrast for that classic LCD look
4. Save as `pinkgameboy.png` in `Graphics/ColorGrading/`

### Recommended Color Palette (Game Boy Pink)
- **Lightest**: #FFF0F8 (near white with pink tint)
- **Light**: #FFB0C8 (light pink)
- **Dark**: #C85A88 (magenta-pink)
- **Darkest**: #301020 (dark purple-pink)

## Trigger Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `flagToSet` | string | `pink_gameboy_activated` | The session flag to set when triggered |
| `colorGradeName` | string | `pinkgameboy` | Name of the color grade to apply |
| `triggerOnce` | bool | `true` | Whether the trigger activates only once |
| `transitionDuration` | float | `0.5` | Duration of the color grade transition in seconds |
| `playSound` | bool | `true` | Whether to play a sound effect on activation |

## Usage in Maps

1. Place the trigger in your map using Lönn
2. Configure the `flagToSet` property to match your flag logic
3. The trigger will:
   - Apply the pink Game Boy color grade
   - Set the specified flag
   - Play a subtle sound effect (optional)
   - Add a brief pink flash

## Flag Usage Example

```csharp
// Check if the pink Game Boy mode has been activated
if (level.Session.GetFlag("pink_gameboy_activated"))
{
    // Do something special when the classic mode is active
    // e.g., spawn retro enemies, change music, etc.
}
```

## Notes
- The color grade persists for the duration of the level session
- You can reset it with another color grade trigger or by using `level.NextColorGrade("none")`
- The flag is session-persistent (resets on level restart)
- For permanent flag storage, consider using SaveData flags instead
