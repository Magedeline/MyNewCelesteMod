# Maggy Collectibles

## Overview

This folder contains sprite assets for custom collectibles in the DesoloZatnas mod, including berries, gems, and special items.

## Collectible Types

### Delta Berry (`deltaberry/`)

A special gold/yellow berry variant.

| Animation | Frames | Description |
|-----------|--------|-------------|
| `idle` | 8 | Floating idle |
| `collect` | 12 | Collection animation |
| `flash` | 4 | Respawn flash |

**Dimensions**: 16x16 pixels

### Ghost Delta Berry (`ghostdeltaberry/`)

Ghost/collected variant of the Delta Berry.

**Opacity**: 50% transparent
**Color**: Pale white with yellow tint

### Heart Gem (`heartgem/`)

Custom heart gem for chapter completion.

| Animation | Frames | Description |
|-----------|--------|-------------|
| `idle` | 6 | Pulsing glow |
| `spin` | 12 | Full rotation |
| `collect` | 20 | Dramatic collection |

**Dimensions**: 24x24 pixels

### Pink Platinum Berry (`pinkplatberry/`)

The ultimate collectible - awarded for D-Side completion.

| Animation | Frames | Description |
|-----------|--------|-------------|
| `idle` | 8 | Sparkle idle |
| `shimmer` | 16 | Rainbow shimmer |
| `collect` | 24 | Grand collection |

**Dimensions**: 20x20 pixels
**Special**: Rainbow particle effects

### Pop Star Berry (`popstarberry/`)

Kirby-themed star-shaped berry.

| Animation | Frames | Description |
|-----------|--------|-------------|
| `idle` | 6 | Star twinkle |
| `bounce` | 8 | Bouncy idle |
| `collect` | 16 | Star burst |

**Dimensions**: 16x16 pixels

### Tape (`tape/`)

Cassette tape collectible for bonus content.

| Animation | Frames | Description |
|-----------|--------|-------------|
| `idle` | 4 | Gentle float |
| `spin` | 8 | Tape spinning |
| `collect` | 12 | Tape ejection |

**Dimensions**: 24x16 pixels

### Void Star Berry (`voidstarberry/`)

Dark matter star berry for secret areas.

| Animation | Frames | Description |
|-----------|--------|-------------|
| `idle` | 8 | Void pulse |
| `absorb` | 12 | Dark absorption |
| `collect` | 16 | Void collapse |

**Dimensions**: 16x16 pixels
**Special**: Inverted color particles

## Design Guidelines

### Color Palette

| Collectible | Primary | Secondary | Glow |
|-------------|---------|-----------|------|
| Delta Berry | `#FFD700` | `#FFA500` | `#FFFF00` |
| Heart Gem | `#FF69B4` | `#FF1493` | `#FFB6C1` |
| Pink Platinum | `#FFB6C1` | `#FF69B4` | `#FFFFFF` |
| Pop Star Berry | `#FFD700` | `#FF6B6B` | `#FFFF88` |
| Void Star Berry | `#4B0082` | `#000000` | `#8B008B` |

### Animation Timing

- **Idle loops**: 0.1-0.15s per frame
- **Collection**: 0.05-0.08s per frame
- **Flash**: 0.04s per frame

### Particle Effects

Each collectible should have matching particles defined in the entity code:

```csharp
// Example particle color
Color particleColor = Calc.HexToColor("FFD700");
```

## XML Definition

Example from `Sprites.xml`:

```xml
<deltaberry path="collectables/maggy/deltaberry" start="idle">
  <Loop id="idle" path="" frames="0-7" delay="0.12"/>
  <Anim id="collect" path="collect" frames="0-11" delay="0.06"/>
  <Loop id="ghost" path="ghost" frames="0-3" delay="0.15"/>
</deltaberry>
```

## Entity Integration

See `Source/Core/Entities/` for collectible entity implementations.
