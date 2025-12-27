# DesoloZatnas Core Module

## Overview

This folder contains the core C# source code for the DesoloZatnas Celeste mod. The code is organized into logical subsystems for maintainability.

## Directory Structure

```
Source/Core/
├── AudioSystems/        # Custom audio management
├── Content/             # Content loading and management
├── Cutscenes/           # Custom cutscene implementations
├── Effects/             # Visual effects and shaders
├── Entities/            # Custom game entities
├── Extensions/          # Extension methods and utilities
├── Integration/         # Third-party mod integration
├── Maps/                # Map processing and metadata
├── Mountain/            # Custom mountain renderer
├── NPCs/                # NPC character implementations
├── Others/              # Miscellaneous systems
├── Player/              # Player-related modifications
├── Portrait/            # Character portrait system
├── Properties/          # Assembly metadata
├── Settings/            # Mod settings and configuration
├── Systems/             # Core game systems
├── Triggers/            # Custom trigger implementations
├── UI/                  # User interface components
├── Utils/               # Utility classes
└── VanillaCore/         # Ported vanilla Celeste classes
```

## Key Files

### Module Entry Point

| File | Description |
|------|-------------|
| `IngesteModule.cs` | Main EverestModule class - entry point for the mod |
| `IngesteConfig.cs` | Configuration management |
| `IngesteConstants.cs` | Constant values and magic numbers |
| `IngesteLogger.cs` | Custom logging system |

### Hook Management

| File | Description |
|------|-------------|
| `HookManager.cs` | Central hook installation/removal |
| `IngesteModuleHook.cs` | Module-level hooks |
| `EverestLoaderPatch.cs` | Everest loader modifications |

### Save Data

| File | Description |
|------|-------------|
| `IngesteModuleSaveData.cs` | Persistent save data (D-Sides, stats) |
| `IngesteModuleSession.cs` | Session-specific data (lives, flags) |
| `IngesteModuleSettings.cs` | User preferences |

### Integration

| File | Description |
|------|-------------|
| `CollabUtils2Integration.cs` | Collab Utils 2 compatibility |
| `FrostHelperIntegration.cs` | Frost Helper compatibility |
| `SkinModHelperPlusIntegration.cs` | Skin Mod Helper+ compatibility |

## Core Systems

### Overworld System

See [OVERWORLD_SUMMARY.md](OVERWORLD_SUMMARY.md) for the complete file inventory.

- `OverworldMod.cs` - Custom overworld scene
- `OverworldIntegration.cs` - Hook installation
- `UI/` - All OUI components

### Character System

| File | Description |
|------|-------------|
| `CharaAutoAnimator.cs` | Chara sprite automation |
| `KirbyAutoAnimator.cs` | Kirby sprite automation |
| `MadelineAutoAnimator.cs` | Madeline sprite automation |
| `RalseiAutoAnimator.cs` | Ralsei sprite automation |
| `StarsiAutoAnimator.cs` | Starsi sprite automation |

### Boss System

| File | Description |
|------|-------------|
| `ElsFloweyBoss.cs` | Flowey boss fight logic |
| `ElsFloweyBossEntities.cs` | Flowey boss entities |
| `VanillaCore/BaseBoss.cs` | Abstract boss template |

### Custom Entities

Key custom entities include:

| File | Description |
|------|-------------|
| `Entities/CassetteTape.cs` | Collectible cassette tapes |
| `Entities/MusicCartridge.cs` | Music cartridge collectibles |
| `Entities/HeartStaff.cs` | Heart staff collectibles |
| `Entities/CharacterRefill.cs` | Character-themed refills |

## Usage

### Module Load

```csharp
public override void Load()
{
    // Install all hooks
    HookManager.InstallAll();
    
    // Register custom entities
    EntityFactory.RegisterAll();
    
    // Initialize subsystems
    AudioSystems.Initialize();
}
```

### Module Unload

```csharp
public override void Unload()
{
    // Remove all hooks
    HookManager.UninstallAll();
    
    // Cleanup subsystems
    AudioSystems.Cleanup();
}
```

## Building

```powershell
dotnet build Source/Desolo_Zantas.csproj
```

## Dependencies

- **Everest**: Celeste mod loader
- **MonoMod**: Runtime patching
- **FNA**: XNA reimplementation
- **Lua**: Scripting support (via MoonSharp)

## Related Documentation

- [VanillaCore/README.md](VanillaCore/README.md) - Ported vanilla classes
- [QUICKSTART.md](QUICKSTART.md) - Quick installation guide
- [OVERWORLD_SUMMARY.md](OVERWORLD_SUMMARY.md) - Overworld file inventory
- [docs/OUI_SYSTEM.md](../../docs/OUI_SYSTEM.md) - UI architecture
