# NPC Character Sprites

## Overview

This folder contains sprite sheets and individual frames for NPC characters that appear throughout DesoloZantas.

## Available Sprite Files

| File | Description |
|------|-------------|
| `characterssprites.ase` | Aseprite source file with all NPC animations |
| `characterssprites.png` | Exported sprite sheet |
| `enemiessprites.png` | Enemy character sprites |

## Character Sprite Folders

Located in `../` (characters folder):

### Crossover Characters

| Folder | Character | Origin |
|--------|-----------|--------|
| `kirby/` | Kirby | Kirby series |
| `magolor/` | Magolor | Kirby series |
| `meta_knight/` | Meta Knight | Kirby series |
| `dedede/` | King Dedede | Kirby series |
| `toriel/` | Toriel | Undertale |
| `sans/` | Sans | Undertale |
| `chara/` | Chara | Undertale |
| `flowey/` | Flowey | Undertale |
| `ralsei/` | Ralsei | Deltarune |
| `susie/` | Susie | Deltarune |
| `asriel/` | Asriel | Undertale |

### Celeste Characters

| Folder | Character |
|--------|-----------|
| `madeline/` | Madeline (protagonist) |
| `badeline/` | Badeline (Part of Me) |
| `theo/` | Theo |
| `oshiro/` | Mr. Oshiro |
| `oldlady/` | Granny |
| `bird/` | Memorial Bird |

## Magolor NPC Sprites

Location: `magolor/` in characters folder

### Required Animations

| Animation | Frames | Description |
|-----------|--------|-------------|
| `idle` | 4 | Standing idle loop |
| `walk` | 8 | Walking cycle |
| `talk` | 6 | Talking mouth movement |
| `happy` | 4 | Excited/happy expression |
| `surprised` | 2 | Surprised reaction |
| `wave` | 6 | Waving greeting |
| `magic` | 8 | Casting magic effects |
| `float` | 4 | Floating hover animation |

### Sprite Specifications

- **Dimensions**: 32x32 pixels per frame
- **Origin**: Center-bottom
- **Facing**: Right by default (flip for left)
- **Frame Rate**: 10 FPS default

### Color Palette

| Element | Color | Hex |
|---------|-------|-----|
| Body | Blue | `#4080FF` |
| Cape | White | `#FFFFFF` |
| Hands | Tan | `#FFE0B0` |
| Eyes | Yellow | `#FFD700` |
| Accents | Gold | `#DAA520` |

## Sprite Sheet Layout

The `characterssprites.png` file uses a grid layout:

```
Row 1: Idle animations (all characters)
Row 2: Walk cycles
Row 3: Talk animations
Row 4: Special animations
...
```

## Animation Definitions

Define in `Sprites.xml`:

```xml
<magolor_npc path="characters/magolor" start="idle">
  <Loop id="idle" path="idle" frames="0-3" delay="0.15"/>
  <Loop id="walk" path="walk" frames="0-7" delay="0.08"/>
  <Loop id="talk" path="talk" frames="0-5" delay="0.1"/>
  <Anim id="wave" path="wave" frames="0-5" delay="0.12" goto="idle"/>
  <Loop id="float" path="float" frames="0-3" delay="0.2"/>
</magolor_npc>
```

## NPC Entity Usage

```csharp
// Create Magolor NPC
[CustomEntity("DesoloZantas/MagolorNPC")]
public class MagolorNPC : NPC
{
    public MagolorNPC(EntityData data, Vector2 offset)
        : base(data, offset)
    {
        Add(Sprite = GFX.SpriteBank.Create("magolor_npc"));
        Sprite.Play("idle");
    }
}
```

## Adding New NPC Sprites

1. Create sprite sheet with animations
2. Add to `characterssprites.ase` (source)
3. Export to PNG
4. Add XML definition to `Sprites.xml`
5. Create entity class in `Source/Core/NPCs/`
6. Register in `EntityFactory.cs`

## Tips

- Keep pixel density consistent across all NPCs
- Use sub-pixel animation for smooth movement
- Match Celeste's general art style
- Test animations at different game speeds
