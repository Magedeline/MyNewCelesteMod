# Heart Staff Sprites

This folder contains sprites for the Heart Staff collectibles inspired by Kirby Star Allies Friend Hearts.

## Required Sprite Folders

Each color needs its own folder with animation frames:

```
collectables/heartstaff/
├── red/          (Blazing Heart Staff - Fire/Power)
│   ├── 00.png    (Frame 0 - idle/spin)
│   ├── 01.png    (Frame 1)
│   ├── ...
│   └── 11.png    (Frame 11)
├── blue/         (Glacial Heart Staff - Water/Ice)
│   └── (same frame structure)
├── yellow/       (Spark Heart Staff - Electric)
│   └── (same frame structure)
├── green/        (Nature Heart Staff - Leaf/Nature)
│   └── (same frame structure)
├── purple/       (Void Heart Staff - Dark/Poison)
│   └── (same frame structure)
├── orange/       (Radiant Heart Staff - Beam/Light)
│   └── (same frame structure)
├── pink/         (Love Heart Staff - Heart/Final)
│   └── (same frame structure)
└── ghost/        (Ghost version for collected staffs)
    └── (same frame structure, semi-transparent)
```

## Animation Specifications

- **idle**: Frames 0-5, 0.12s delay, gentle floating pulse
- **spin**: Frames 0-11, 0.08s delay, full rotation
- **collect**: Frames 0-11 then hold frame 11, 0.05s delay, fast spin on collection

## Design Guidelines

Based on Kirby Star Allies Friend Hearts and Star Rods:

1. **Shape**: Heart-tipped staff/wand design
2. **Size**: Approximately 32x48 pixels (centered origin)
3. **Colors**: 
   - Red (#ff4444) - Warm, fiery glow
   - Blue (#4488ff) - Cool, icy shimmer
   - Yellow (#ffdd44) - Electric sparkle
   - Green (#44ff88) - Natural, leafy aura
   - Purple (#aa44ff) - Mysterious, dark energy
   - Orange (#ff8844) - Radiant, light beams
   - Pink (#ff88cc) - Love hearts, friendship sparkles

4. **Effects**: Each staff should have:
   - Particle/sparkle effects around it
   - Soft glow matching its color
   - Subtle rotation animation

## Fallback Behavior

If custom sprites are not provided, the game will use the standard Heart Gem sprites 
with color tinting applied in code.

## Collection Animation

When collected:
1. Staff rises up and spins faster
2. Burst of colored particles
3. Flash to white then fade
4. Display collection message

## Door Integration

All 7 Heart Staffs unlock the final chapter door (HeartStaffDoor entity).
The door displays icons for each staff showing collected status.
