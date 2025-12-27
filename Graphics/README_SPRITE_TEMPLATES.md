# Sprite Templates for Cassette Tape & Music Cartridge

## Overview
This document provides specifications for creating sprite assets for the CassetteTape and MusicCartridge entities.

---

## CassetteTape Sprites

### Directory
`Graphics/Atlases/Gameplay/collectibles/desolozantas/cassettetape/`

### Required Files

#### Idle Animation
- `idle00.png` through `idle05.png` (6 frames)
- **Dimensions:** 16x16 pixels
- **Frame Rate:** 10 FPS (0.1s per frame)
- **Loop:** Yes
- **Purpose:** Default floating animation

#### Shimmer Animation  
- `shimmer00.png` through `shimmer07.png` (8 frames)
- **Dimensions:** 16x16 pixels
- **Frame Rate:** 12.5 FPS (0.08s per frame)
- **Loop:** Yes
- **Purpose:** Plays when player is nearby and can interact

### Design Guidelines

**Visual Style:**
- Pixel art cassette tape design
- Clear, recognizable silhouette
- Transparent background (PNG alpha channel)
- Design centered in 16x16 canvas

**Color:**
- Base sprite should be white or light gray
- Color tint applied programmatically via entity `color` property
- Avoid dark colors in base sprite (allows better tinting)

**Animation Tips:**
- **Idle:** Subtle breathing/bobbing motion
- **Shimmer:** Gentle sparkle or highlight sweep
- Keep animations smooth and looping seamlessly

### Example Frame Breakdown

**Idle Animation:**
```
Frame 0: Base position
Frame 1: Slightly expanded
Frame 2: Peak expansion
Frame 3: Return to base
Frame 4: Slightly compressed
Frame 5: Return to base
```

**Shimmer Animation:**
```
Frame 0-1: Normal
Frame 2-3: Highlight starts (left side)
Frame 4-5: Highlight middle
Frame 6-7: Highlight ends (right side)
```

---

## MusicCartridge Sprites

### Directory
`Graphics/Atlases/Gameplay/collectibles/desolozantas/musiccartridge/`

### Required Files

#### Idle Animation
- `idle00.png` through `idle03.png` (4 frames)
- **Dimensions:** 20x20 pixels
- **Frame Rate:** 10 FPS (0.1s per frame)
- **Loop:** Yes
- **Purpose:** Constant floating animation

#### Pulse Animation
- `pulse00.png` through `pulse05.png` (6 frames)
- **Dimensions:** 20x20 pixels
- **Frame Rate:** 6.67 FPS (0.15s per frame)
- **Loop:** No (triggered every 3 seconds)
- **Purpose:** Periodic attention-grabbing pulse

### Design Guidelines

**Visual Style:**
- Retro game cartridge aesthetic
- Rectangular with rounded corners
- Label area at top for text overlay
- Transparent background

**Color:**
- Base sprite in light/neutral tones
- Entity applies color tint programmatically
- Label area should remain readable after tinting

**Label Area:**
- Reserve top 6-8 pixels for text
- Light background in this area for text contrast
- Text rendered separately by entity code

**Animation Tips:**
- **Idle:** Gentle rotation or sway
- **Pulse:** Scale up slightly then return
- Maintain label readability throughout animation

### Example Frame Breakdown

**Idle Animation:**
```
Frame 0: Straight
Frame 1: Rotated 2° clockwise
Frame 2: Straight
Frame 3: Rotated 2° counter-clockwise
```

**Pulse Animation:**
```
Frame 0: Normal size (100%)
Frame 1: Expand start (105%)
Frame 2: Peak expansion (115%)
Frame 3: Expand end (110%)
Frame 4: Returning (105%)
Frame 5: Back to normal (100%)
```

---

## Technical Specifications

### File Format
- **Type:** PNG (Portable Network Graphics)
- **Color Mode:** RGBA (with alpha transparency)
- **Bit Depth:** 32-bit (8-bit per channel)
- **Compression:** PNG compression (lossless)

### Canvas Setup
- **CassetteTape:** 16x16 pixels
- **MusicCartridge:** 20x20 pixels
- **Origin:** Center (sprite.CenterOrigin() applied in code)
- **Transparent Background:** Required

### Color Considerations
- Use light/neutral base colors (white, light gray)
- Avoid pure black (makes tinting difficult)
- Keep saturation moderate
- Ensure good contrast for visibility

### Export Settings
- No anti-aliasing on edges (pixel-perfect)
- No color profile embedding
- Nearest neighbor scaling if resizing
- Save for web/optimization acceptable

---

## Placeholder/Template Creation

If you don't have final sprites yet, create simple placeholders:

### CassetteTape Placeholder
1. 16x16 canvas
2. Draw white rectangle (12x8 pixels)
3. Add 2 small circles (tape reels)
4. Add horizontal line (tape)
5. Save as all required frame names (can be identical initially)

### MusicCartridge Placeholder
1. 20x20 canvas
2. Draw white rounded rectangle (16x18 pixels)
3. Add label area at top (16x6 pixels, light gray)
4. Add bottom notch detail
5. Save as all required frame names

### Quick Command (ImageMagick)
```bash
# Create simple white square placeholder
convert -size 16x16 xc:white -alpha set -channel A -evaluate set 100% idle00.png

# Duplicate for all frames
for i in {00..05}; do cp idle00.png idle$i.png; done
for i in {00..07}; do cp idle00.png shimmer$i.png; done
```

---

## Testing Sprites

### In-Game Testing
1. Place sprites in correct directory
2. Reload assets (or restart game)
3. Place entity in Loenn
4. Test in-game:
   - Check visibility
   - Verify animations play
   - Test color tinting
   - Check interaction states

### Loenn Preview
- Sprites appear in Loenn entity browser
- Only frame 00 shown in editor
- Full animation visible in-game only

### Common Issues
- **Sprite not appearing:** Check file path and naming
- **Wrong size:** Verify pixel dimensions exactly match spec
- **No transparency:** Ensure PNG has alpha channel
- **Color looks wrong:** Check base sprite isn't too dark

---

## Recommended Tools

### Free Tools
- **Aseprite** (Paid, but has free trial) - Best for pixel art
- **GIMP** - Free, supports all required features
- **Paint.NET** - Free, Windows, good for pixel art
- **LibreSprite** - Free fork of old Aseprite

### Online Tools
- **Piskel** - Browser-based pixel art tool
- **Pixilart** - Online pixel art editor

### Professional Tools
- **Photoshop** - Industry standard
- **Krita** - Free, professional-grade

---

## Animation Reference

### Timing
Both entities use Engine.DeltaTime for smooth animation regardless of framerate.

### Particle Effects
Sprites don't need to include particle effects - these are generated by entity code using:
- `ParticleType` definitions
- Programmatic particle emission
- Colored based on entity `color` property

### Lighting
Bloom and vertex lighting applied by entity code, not in sprites.

---

## File Organization

```
Graphics/Atlases/Gameplay/collectibles/desolozantas/
├── cassettetape/
│   ├── idle00.png
│   ├── idle01.png
│   ├── idle02.png
│   ├── idle03.png
│   ├── idle04.png
│   ├── idle05.png
│   ├── shimmer00.png
│   ├── shimmer01.png
│   ├── shimmer02.png
│   ├── shimmer03.png
│   ├── shimmer04.png
│   ├── shimmer05.png
│   ├── shimmer06.png
│   └── shimmer07.png
└── musiccartridge/
    ├── idle00.png
    ├── idle01.png
    ├── idle02.png
    ├── idle03.png
    ├── pulse00.png
    ├── pulse01.png
    ├── pulse02.png
    ├── pulse03.png
    ├── pulse04.png
    └── pulse05.png
```

---

## Next Steps

1. **Create sprites** using your preferred tool
2. **Export as PNG** with transparency
3. **Place in correct directories**
4. **Test in Loenn** to verify paths
5. **Test in-game** to verify animations
6. **Iterate** based on visual feedback

---

## Support

If sprites don't appear:
1. Check exact file paths and names
2. Verify PNG format with alpha channel
3. Confirm correct pixel dimensions
4. Reload game assets
5. Check mod log for errors

---

## Future Enhancements

Consider creating additional variants:
- Different tape/cartridge styles
- Alternative color palettes
- Special edition designs
- Animated labels
- Holographic effects

These can be added as separate entity types or as sprite variations selected via entity properties.
