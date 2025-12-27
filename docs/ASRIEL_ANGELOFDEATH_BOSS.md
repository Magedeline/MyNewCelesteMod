# Asriel Angel of Death Boss

## Overview
A powerful 2-phase boss fight featuring Asriel in his Angel of Death form with rainbow cosmowing effects and multiple devastating attack patterns.

## Features

### Visual Effects
- **Cosmowing Rainbow Backgrounds**: Dynamic rainbow effects behind the boss that intensify in Phase 2
- **Multi-layered Sprites**: Separate sprites for stems, shoulders, and orb wings with synchronized animations
- **Tween Node System**: Phase 2 uses separate sprite nodes for head, torso, arms, wings, and halo with independent animations
- **Particle Effects**: Trail particles, explosion effects, and energy emanations

### Phase 1: Angel of Death
- **Health**: 100 HP
- **Attacks**:
  1. **Ultima Bullet**: Fires 8 homing projectiles in a circular pattern
  2. **Cross Shocker**: Creates cross-shaped lightning waves
  3. **Star Storm Ultra**: Rains 30 stars from above

### Phase 2: Transcendent Angel
- **Health**: 150 HP
- **Enhanced Abilities**: Faster attacks, more damage
- **New Visual**: Separated body parts with independent tween nodes
- **Additional Attacks**:
  4. **Shocker Breaker III**: Expanding circular shockwave rings
  5. **Final Beam**: Massive sustained laser beam (Ultimate attack)

## File Structure

### C# Implementation
- `Source/BossesHelper/Entities/AsrielAngelOfDeathBoss.cs` - Main boss entity class
- `Source/BossesHelper/Entities/AsrielAngelOfDeathAttacks.cs` - Attack projectile classes

### Lua Scripts
- `Assets/BossesHelper/LuaBossHelper/asriel_angelofdeath_attacks.lua` - Attack pattern definitions
- `Assets/BossesHelper/LuaBossHelper/asriel_angelofdeath_config.lua` - Boss configuration

### Lönn Editor Support
- `Loenn/BossesHelper/entities/asriel_angelofdeath.lua` - Entity definition for level editor

### Sprites (Required)
All sprites should be placed in `Graphics/Atlases/Gameplay/characters/asrielangelofdeathboss/`

#### Base Animations
- `idle/` - Idle animation
- `hover/` - Hovering animation
- `charge/` - Charging animation
- `attack/` - Basic attack animation
- `hurt/` - Hurt animation
- `defeat/` - Death animation

#### Background Effects
- `bg/cosmowing/` - Rainbow background effect (normal)
- `bg/cosmowing_intense/` - Intense rainbow effect
- `bg/cosmowing_phase2/` - Phase 2 background

#### Body Parts with Cosmowing
- `stems/idle/` - Stems idle animation
- `stems/cosmowing/` - Stems with rainbow effect
- `stems/cosmowing_phase2/` - Phase 2 stems effect
- `shoulder/idle/` - Shoulder idle
- `shoulder/cosmowing/` - Shoulder with rainbow effect
- `shoulder/cosmowing_phase2/` - Phase 2 shoulder effect
- `orbwings/idle/` - Orb wings idle
- `orbwings/cosmowing/` - Orb wings with rainbow
- `orbwings/spread/` - Wings spread animation
- `orbwings/cosmowing_phase2/` - Phase 2 wings effect

#### Phase 2 Animations
- `phase2/idle/` - Phase 2 idle
- `phase2/hover/` - Phase 2 hover
- `phase2/ultimate/` - Ultimate stance
- `phase2/hurt/` - Phase 2 hurt

#### Phase 2 Tween Nodes
- `phase2/parts/head/` - Head sprite
- `phase2/parts/torso/` - Torso sprite
- `phase2/parts/arms_left/` - Left arm
- `phase2/parts/arms_right/` - Right arm
- `phase2/parts/wings_left/` - Left wing
- `phase2/parts/wings_right/` - Right wing
- `phase2/parts/halo/` - Halo sprite

#### Transformation
- `transform/phase2_start/` - Transform start (0-30 frames)
- `transform/phase2_mid/` - Transform middle (loop)
- `transform/phase2_end/` - Transform end (0-25 frames)

#### Attack Animations
##### Ultima Bullet
- `attacks/ultimabullet/charge/` - Charge animation (0-15 frames)
- `attacks/ultimabullet/hold/` - Hold animation (loop, 0-10 frames)
- `attacks/ultimabullet/fire/` - Fire animation (0-20 frames)

##### Cross Shocker
- `attacks/crossshocker/start/` - Start (0-12 frames)
- `attacks/crossshocker/loop/` - Loop (0-8 frames)
- `attacks/crossshocker/end/` - End (0-10 frames)

##### Star Storm Ultra
- `attacks/starstormultra/start/` - Start (0-18 frames)
- `attacks/starstormultra/rain/` - Rain loop (0-15 frames)
- `attacks/starstormultra/end/` - End (0-12 frames)

##### Shocker Breaker III
- `attacks/shockerbreaker3/charge/` - Charge (0-20 frames)
- `attacks/shockerbreaker3/hold/` - Hold (0-12 frames)
- `attacks/shockerbreaker3/release/` - Release (0-30 frames)
- `attacks/shockerbreaker3/waves/` - Waves (0-18 frames)
- `attacks/shockerbreaker3/end/` - End (0-15 frames)

##### Final Beam
- `attacks/finalbeam/charge/` - Charge (0-40 frames)
- `attacks/finalbeam/focus/` - Focus (0-20 frames)
- `attacks/finalbeam/fire/` - Fire (0-60 frames)
- `attacks/finalbeam/hold/` - Hold beam (0-30 frames)
- `attacks/finalbeam/end/` - End (0-25 frames)

#### Projectile Sprites
- `projectiles/ultimabullet/` - Homing bullet sprite
- `projectiles/crossshocker/` - Lightning wave sprite
- `projectiles/starstorm/` - Falling star sprite
- `projectiles/finalbeam/` - Beam core sprite

## Attack Patterns

### Ultima Bullet
**Type**: Homing Projectiles  
**Pattern**: 8 bullets fired in a circle  
**Behavior**: Each bullet homes in on the player  
**Speed**: 180 units/s with 120 homing strength  
**Lifetime**: 5 seconds  
**Cooldown**: 2.0 seconds

### Cross Shocker
**Type**: Directional Waves  
**Pattern**: Cross formation (4 directions)  
**Waves**: 3 waves per attack  
**Speed**: 200 units/s  
**Lifetime**: 3 seconds  
**Cooldown**: 2.5 seconds

### Star Storm Ultra
**Type**: Rain Attack  
**Pattern**: 30 stars from top of screen  
**Behavior**: Falling with acceleration  
**Initial Speed**: 200 units/s  
**Acceleration**: 50 units/s²  
**Interval**: 0.15s between stars  
**Cooldown**: 3.0 seconds

### Shocker Breaker III (Phase 2 Only)
**Type**: Expanding Shockwaves  
**Pattern**: 5 expanding rings  
**Radius**: 80, 140, 200, 260, 320 units  
**Speed**: 200 units/s expansion  
**Thickness**: 20 units  
**Delay**: 0.2s per ring  
**Cooldown**: 4.0 seconds

### Final Beam (Phase 2 Only)
**Type**: Sustained Laser  
**Width**: 40 units  
**Length**: 1000 units  
**Duration**: 3 seconds  
**Charge Time**: 2.1 seconds  
**Effects**: Rainbow pulse, particles  
**Cooldown**: 6.0 seconds

## Cosmowing Rainbow Effect

The cosmowing effect creates a dynamic rainbow background that cycles through colors:
- Red → Orange → Yellow → Green → Blue → Indigo → Violet

### Intensity Levels
- **Normal**: Default rainbow effect (0.02 frame delay)
- **Intense**: Faster, brighter during special attacks (0.015 frame delay)

### Applied To
- Background layer (behind boss)
- Stems (lower body appendages)
- Shoulders (side elements)
- Orb Wings (wing structures)

## Phase Transition

### Trigger
When Phase 1 health reaches 0

### Sequence
1. **Transform Start** (30 frames, 0.9s)
   - Cosmowing intensifies
   - Boss begins glowing
   
2. **Transform Mid** (40 frames, loop until ready)
   - Energy builds up
   - Screen shake
   
3. **Transform End** (25 frames, 0.75s)
   - Phase 2 tween nodes initialize
   - New form revealed
   
4. **Phase 2 Start**
   - Health restored to 150
   - Enhanced attack patterns available

## Integration with BossesHelper

This boss uses the BossesHelper framework for:
- Health management
- Attack pattern coordination
- Projectile spawning
- Animation state management
- Phase transitions

### Custom Properties
```csharp
startPhase: 1 or 2 (default: 1)
enableCosmowing: true/false (default: true)
enablePhase2: true/false (default: true)
healthPhase1: integer (default: 100)
healthPhase2: integer (default: 150)
musicEvent: string (audio event path)
```

## Audio Requirements

### Music Events
- `event:/music/boss/asriel_angelofdeath_phase1`
- `event:/music/boss/asriel_angelofdeath_phase2`

### SFX Events
- `event:/sfx/boss/asriel_ultimabullet`
- `event:/sfx/boss/asriel_crossshocker`
- `event:/sfx/boss/asriel_starstorm`
- `event:/sfx/boss/asriel_shockerbreaker3`
- `event:/sfx/boss/asriel_finalbeam`
- `event:/sfx/boss/asriel_hurt`
- `event:/sfx/boss/asriel_death`
- `event:/sfx/boss/asriel_transform`

## Usage in Lönn

1. Open your map in Lönn
2. Add entity: `DesoloZatnas/AsrielAngelOfDeathBoss`
3. Configure properties as needed
4. Place in desired location

### Recommended Room Setup
- **Room Size**: At least 320x180 (20x10 tiles) for proper movement
- **Safe Zones**: Include cover areas for players
- **Height**: Sufficient vertical space for star storm attacks
- **Width**: Wide enough for cross shocker and beam attacks

## Combat Tips (for players)

### Phase 1
- Stay mobile during Ultima Bullet
- Use dash to dodge Cross Shocker waves
- Watch the sky during Star Storm
- Learn attack patterns and dodge windows

### Phase 2
- All Phase 1 attacks are faster
- Shocker Breaker III: Stay between the rings
- Final Beam: Use cover or constant movement perpendicular to beam
- Phase 2 is more aggressive with shorter cooldowns

## Development Notes

### Performance Considerations
- Multiple sprite layers may impact performance
- Particle effects are optimized with intervals
- Consider reducing particle count on lower-end systems

### Balancing
- Health values can be adjusted via entity properties
- Attack cooldowns are configurable in the Lua scripts
- Projectile speeds and behaviors can be tuned

### Future Enhancements
- Add difficulty modes (Easy/Normal/Hard)
- Additional attack patterns
- Special conditions for speedrun mode
- Achievement integration

## Credits
Created for DesoloZatnas mod  
BossesHelper framework integration  
Cosmowing effect inspired by rainbow aesthetics
