# Character Refill Sprite Requirements

This document describes the sprite assets needed for the Character Refill entity system.

## Overview

Character Refills are themed dash crystals that match various characters from Celeste, Undertale, Deltarune, and Kirby. Each character has their own colored crystal with unique particles and special abilities.

## Directory Structure

Each character requires sprites in their own folder under:
`Graphics/Atlases/Gameplay/objects/characterRefill/<character>/`

## Required Sprites Per Character

Each character folder needs the following sprite files:

### Idle Animation (Required)
- `idle00.png` through `idle05.png` (6 frames recommended)
- Size: 24x24 pixels (centered)
- The main floating crystal animation

### Flash Animation (Required)  
- `flash00.png` through `flash05.png` (6 frames)
- Size: 24x24 pixels
- Bright flash effect when respawning

### Optional Special Animations (Character-Specific)

Some characters have additional animations defined in Sprites.xml:

| Character    | Extra Animations |
|-------------|------------------|
| Oshiro      | `ghostly00-07`   |
| Chara       | `pulse00-03`     |
| Ralsei      | `heal00-06`      |
| Asriel      | `rainbow00-11`   |
| Meta Knight | `slash00-04`     |
| Magolor     | `warp00-05`      |
| Mage Kirby  | `magic00-07`     |

## Character Colors

Reference colors for creating sprites:

| Character    | Primary Color    | Hex Code  |
|-------------|------------------|-----------|
| Kirby       | Hot Pink         | #FF69B4   |
| Madeline    | Auburn Red       | #E66464   |
| Badeline    | Purple           | #8B45FF   |
| Theo        | Dodger Blue      | #1E90FF   |
| Granny      | Silver           | #C0C0C0   |
| Oshiro      | Teal             | #64C8B4   |
| Chara       | Red              | #FF0000   |
| Frisk       | Gold             | #FFD700   |
| Ralsei      | Lime Green       | #32CD32   |
| Asriel      | White/Rainbow    | #FFFFFF   |
| Meta Knight | Dark Blue        | #00008B   |
| King Dedede | Royal Blue       | #4169E1   |
| Magolor     | Purple/Orange    | #643296   |
| Mage Kirby  | Cyan             | #00FFFF   |

## Design Guidelines

1. **Base Shape**: Start with the standard Celeste refill crystal shape
2. **Character Elements**: Add subtle character-themed elements:
   - Kirby: Star-shaped or pink puff
   - Madeline: Hair/feather motifs
   - Badeline: Darker purple with dream wisps
   - Chara: Heart soul shape in red
   - Frisk: Heart soul shape in yellow
   - Ralsei: Heart soul with green healing glow
   - Meta Knight: Sword/mask elements
   - etc.

3. **Animation Style**: 
   - Smooth floating motion
   - Gentle rotation or pulse
   - Sparkle effects matching character color

4. **Size**: All sprites should be 24x24 pixels to match vanilla refills

## Fallback Behavior

Until custom sprites are created, the system will:
1. Use vanilla refill sprites as fallbacks
2. Apply character-specific particle colors
3. Still grant character-specific abilities

## Integration with Loenn

The Loenn entity definition (`character_refill.lua`) shows each character's refill in the editor with:
- Appropriate color tinting
- Fallback to vanilla refill sprites
- All configurable properties

## Testing

After adding sprites:
1. Rebuild the mod
2. Open Loenn and place Character Refills
3. Test in-game to verify:
   - Sprite displays correctly
   - Particles match character color
   - Special abilities trigger properly
   - Sound effects play correctly
