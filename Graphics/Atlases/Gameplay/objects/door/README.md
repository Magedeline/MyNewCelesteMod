# Door Sprites

## Overview

This folder contains door animation sprites for various door types used throughout DesoloZantas.

## Door Types

### Standard Door (`door00-04`)

| Frame | Description |
|-------|-------------|
| `door00.png` | Fully closed |
| `door01.png` | Opening 25% |
| `door02.png` | Opening 50% |
| `door03.png` | Opening 75% |
| `door04.png` | Fully open |

**Dimensions**: 16x32 pixels
**Animation**: 0.1s per frame

### Lock Door (`lockdoor00-18`)

Locked door that requires a key to open.

| Frame Range | Description |
|-------------|-------------|
| `lockdoor00-04` | Closed with lock |
| `lockdoor05-09` | Lock unlocking |
| `lockdoor10-18` | Door opening |

**Dimensions**: 16x32 pixels

### Temple Door Variants

#### Temple Door A (`lockdoorTempleA00-18`)
Stone temple door with ancient markings.

#### Temple Door B (`lockdoorTempleB00-18`)
Dark temple door with void energy effects.

#### Temple Door Standard (`TempleDoor00-14`)
Standard temple entry door.

#### Temple Door B (`TempleDoorB00-14`)
Temple side door variant.

#### Temple Door C (`TempleDoorC00-14`)
Temple secret door variant.

### Metal Door (`metaldoor00-04`)

Industrial metal door for mechanical areas.

**Dimensions**: 16x32 pixels
**Animation**: 0.08s per frame (faster mechanical motion)

### Moon Door (`moonDoor00-43`)

Elaborate moon-themed door for Chapter 18.

| Frame Range | Description |
|-------------|-------------|
| `moonDoor00-10` | Idle lunar glow |
| `moonDoor11-25` | Unlocking sequence |
| `moonDoor26-43` | Opening with particle effects |

**Dimensions**: 32x48 pixels
**Animation**: Variable timing for dramatic effect

### Ghost Door (`ghost_door00-28`)

Ethereal door that fades between dimensions.

| Frame Range | Description |
|-------------|-------------|
| `ghost_door00-07` | Idle shimmer |
| `ghost_door08-18` | Materializing |
| `ghost_door19-28` | Opening/phasing |

**Dimensions**: 16x32 pixels
**Special**: Semi-transparent frames

### Trap Door (`trap00-07`)

Floor trap door that opens downward.

| Frame | Description |
|-------|-------------|
| `trap00-02` | Closed |
| `trap03-05` | Opening |
| `trap06-07` | Fully open |

**Dimensions**: 32x8 pixels
**Orientation**: Horizontal

## Sprite Specifications

### Standard Format

- **Format**: PNG with alpha channel
- **Color Depth**: 32-bit RGBA
- **Origin**: Center-bottom for vertical doors
- **Naming**: `[type][frame_number].png` (2-digit padding)

### Animation Timing

| Door Type | Open Speed | Close Speed |
|-----------|------------|-------------|
| Standard | 0.5s | 0.3s |
| Lock | 1.2s | N/A |
| Temple | 0.8s | 0.5s |
| Metal | 0.3s | 0.2s |
| Moon | 2.0s | 1.0s |
| Ghost | 0.6s | 0.4s |
| Trap | 0.2s | 0.15s |

## Usage in Code

```csharp
// Reference door sprite
var doorSprite = GFX.Game["objects/door/door"];
doorSprite.Play("open");

// Custom temple door
var templeDoor = GFX.Game["objects/door/TempleDoorA"];
templeDoor.Play("unlock");
```

## Adding New Door Types

1. Create frames following naming convention
2. Add entry to `Sprites.xml`:
```xml
<door path="objects/door/newdoor" start="idle">
  <Loop id="idle" path="" frames="0-4" delay="0.1"/>
  <Anim id="open" path="" frames="0-8" delay="0.08" goto="open_idle"/>
  <Loop id="open_idle" path="" frames="8"/>
</door>
```
3. Register entity in `EntityFactory.cs`
