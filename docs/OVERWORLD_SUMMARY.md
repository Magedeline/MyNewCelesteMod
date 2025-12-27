# DesoloZatnas - Overworld System File Inventory

## Overview

This document provides a complete inventory of all files that comprise the custom Overworld system for the DesoloZatnas mod.

## Core Overworld Files

### Integration Layer (`Source/Core/`)

| File | Lines | Description |
|------|-------|-------------|
| `OverworldMod.cs` | ~180 | Custom Overworld scene implementation; manages UI registration and music |
| `OverworldIntegration.cs` | ~250 | Hook installer and OUI registration; integrates custom screens into Celeste |
| `MountainConnector.cs` | ~120 | Connects custom chapters to mountain visualization |

### UI Components (`Source/Core/UI/`)

#### Main Menu Flow

| File | Lines | Description |
|------|-------|-------------|
| `OuiMainMenuDesoloZatnas.cs` | ~300 | Custom main menu with Chapter Select, Statistics, Credits, Extras |
| `OuiChapterSelectDesoloZatnas.cs` | ~480 | Chapter browser for areas 10-21 with A/B/C/D-Side selection |
| `OuiChapterSelectIconHooks.cs` | ~150 | Hooks for custom chapter icons on the chapter select screen |

#### Statistics & Progress

| File | Lines | Description |
|------|-------|-------------|
| `OuiStatisticsNotebook.cs` | ~620 | Multi-page statistics viewer (deaths, collections, speedruns) |
| `OuiDSidePostcard.cs` | ~400 | D-Side unlock postcards and reward collection |

#### Game Flow

| File | Lines | Description |
|------|-------|-------------|
| `GameOverScreen.cs` | ~280 | Custom game over with lives system |
| `FloweyGameOver.cs` | ~350 | Flowey-themed evil game over sequence |
| `OuiCreditsDesoloZatnas.cs` | ~330 | Auto-scrolling credits with custom sections |

#### Vessel Creation

| File | Lines | Description |
|------|-------|-------------|
| `VesselCreationScene.cs` | ~500 | Undertale-style player customization scene |

#### Helpers & Utilities

| File | Lines | Description |
|------|-------|-------------|
| `OuiHelpers.cs` | ~180 | Audio, rendering, navigation, and input helper functions |
| `CustomMountainRenderer.cs` | ~140 | Custom 3D mountain with chapter positioning |

### In-Game UI (`Source/Core/UI/InGame/`)

| File | Lines | Description |
|------|-------|-------------|
| `HUDOverlay.cs` | ~200 | Lives counter, health bar, collectibles display |
| `PauseMenuExtensions.cs` | ~150 | Custom pause menu additions |

## Mountain Assets (`Source/Core/Mountain/`)

| File | Description |
|------|-------------|
| `MountainModel.obj` | Custom 3D mountain mesh |
| `MountainTexture.png` | Mountain texture atlas |
| `ChapterPositions.json` | Chapter marker positions on mountain |

## Dialog Files (`Dialog/`)

| File | Description |
|------|-------------|
| `English.txt` | Main dialog file including UI strings |
| `English_DesoloZatnas_UI.txt` | UI-specific labels and messages |

## Audio Requirements (`Audio/`)

| Bank File | Description |
|-----------|-------------|
| `music_desolozantas.bank` | Menu and chapter music |
| `ui_desolozantas.bank` | UI sound effects |
| `sfx_desolozantas.bank` | General sound effects |

## Graphics Assets

### Mountain Graphics (`Graphics/Atlases/Overworld/`)

- Mountain backdrop layers
- Custom cursor sprites
- Chapter node icons

### GUI Elements (`Graphics/Atlases/Gui/DesoloZantas/`)

- Logo and title screen assets
- Menu button sprites
- Statistics notebook pages
- Postcard backgrounds

## Save Data (`Source/Core/`)

| File | Description |
|------|-------------|
| `IngesteModuleSaveData.cs` | D-Side completion, statistics, settings persistence |
| `IngesteModuleSession.cs` | Session-specific state (current chapter, lives) |

## Configuration

| File | Description |
|------|-------------|
| `IngesteModuleSettings.cs` | User preferences including `UseDesoloZantasBranding` |

## File Statistics

| Category | File Count | Total Lines |
|----------|------------|-------------|
| Core Integration | 3 | ~550 |
| UI Components | 11 | ~3,400 |
| Helpers | 2 | ~320 |
| Total C# Code | **16** | **~4,270** |

## Related Documentation

- [OUI_SYSTEM.md](OUI_SYSTEM.md) - Architecture and component details
- [OUI_IMPLEMENTATION_SUMMARY.md](OUI_IMPLEMENTATION_SUMMARY.md) - Feature status and integration
- [OUI_DEBUG_LOGGING.md](OUI_DEBUG_LOGGING.md) - Debug hooks and log patterns
- [QUICKSTART.md](../Source/Core/QUICKSTART.md) - Quick installation guide
