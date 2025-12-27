# Gameplay Assets

## Overview

This folder contains all gameplay sprite atlases including characters, objects, effects, and environment elements.

## Directory Structure

```
Gameplay/
├── animatedtiles/    # Animated tileset decorations
├── bgs/              # Background layers
├── characters/       # Player and NPC sprites
├── collectables/     # Berries, gems, tapes
├── cutscenes/        # Cutscene-specific sprites
├── danger/           # Hazards and spikes
├── debris/           # Particle and debris sprites
├── decals/           # Decorative decals
├── effects/          # Visual effects
├── mirrormasks/      # Mirror reflection masks
├── objects/          # Interactive objects
├── particles/        # Particle sprites
├── pico8/            # PICO-8 style assets
├── scenery/          # Environmental scenery
├── star/             # Star-related sprites
├── Systems/          # System sprites
├── tilesets/         # Tile graphics
└── util/             # Utility sprites
```

## Key Asset Categories

### Characters (`characters/`)

Player characters, NPCs, bosses, and enemies.

| Subfolder | Contents |
|-----------|----------|
| `player/` | Madeline sprites |
| `kirby/` | Kirby sprites |
| `ralsei/` | Ralsei sprites |
| `asriel/` | Asriel sprites |
| `flowey/` | Flowey sprites |
| `chara/` | Chara sprites |
| `magolor/` | Magolor sprites |
| `Bosses/` | Boss-specific sprites |
| `npcs/` | NPC sprite sheets |

### Collectables (`collectables/`)

All collectible items.

| Subfolder | Contents |
|-----------|----------|
| `strawberry/` | Standard strawberries |
| `goldberry/` | Golden strawberries |
| `heartGem/` | Crystal hearts |
| `heartstaff/` | Heart staff collectibles |
| `maggy/` | Custom mod collectibles |
| `cassette/` | Cassette tapes |

### Objects (`objects/`)

Interactive game objects.

| Subfolder | Contents |
|-----------|----------|
| `door/` | All door types |
| `refill/` | Dash refills |
| `spring/` | Springs and launchers |
| `booster/` | Boosters |
| `characterRefill/` | Character-themed refills |
| `checkpoint/` | Checkpoint flags |

### Danger (`danger/`)

Hazardous elements.

| Subfolder | Contents |
|-----------|----------|
| `spikes/` | All spike variants |
| `spike*` | Directional spikes |
| `killbox/` | Kill zones |

## Sprite Specifications

### Standard Sizes

| Asset Type | Typical Size |
|------------|--------------|
| Player | 32x32 |
| NPCs | 24-48 px |
| Collectibles | 16-24 px |
| Objects | Variable |
| Tiles | 8x8 |

### Animation Standards

- **Frame Rate**: 10 FPS default
- **Format**: PNG with alpha
- **Naming**: `animation_frame.png` (e.g., `idle00.png`)

## Adding New Assets

1. Create sprites matching existing style
2. Place in appropriate subfolder
3. Add XML definition to `Sprites.xml`
4. Reference in entity code

## Related Documentation

- [README_CHARACTER_REFILLS.md](objects/characterRefill/README_CHARACTER_REFILLS.md)
- [README.md (doors)](objects/door/README.md)
- [README.md (NPCs)](characters/npcs/README.md)
- [README.md (Maggy collectibles)](collectables/maggy/README.md)
