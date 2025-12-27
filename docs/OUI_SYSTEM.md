# DesoloZatnas Custom OUI System

## Overview

This document describes the custom Overworld User Interface (OUI) system for the DesoloZatnas Celeste mod. The system is inspired by Snowberry and provides an external-style main menu experience.

## Critical Understanding: Two Separate UI Systems

Celeste has **two completely separate UI contexts** that require different implementations:

### 1. Overworld UI (Menu Context)
- **Scene Type**: `Overworld`
- **Base Class**: `Oui` (Overworld User Interface)
- **When Active**: Main menu, chapter select, file select, options
- **Input Pattern**: Focus-based menu navigation
- **Lifecycle**: `Enter()` → `Update()` → `Leave()`
- **Examples**: Main menu, chapter selection, statistics screens

### 2. In-Game UI (Gameplay Context)
- **Scene Type**: `Level`
- **Base Class**: `Entity` with `Tags.HUD`
- **When Active**: During active gameplay
- **Input Pattern**: Continuous gameplay, pause menus
- **Lifecycle**: `Added()` → `Update()` → `Render()` → `Removed()`
- **Examples**: Lives counter, HUD overlays, custom pause menus

**Your current system only implements Overworld UI (#1). You need to add In-Game UI (#2) for lives counter, D-Side progress displays, and custom pause menu enhancements.**

## Architecture

### Core Components

#### 1. **OuiMainMenuDesoloZatnas** (`OuiMainMenuDesoloZatnas.cs`)
The main entry point for the custom UI system.

**Features:**
- Custom title screen with DesoloZatnas branding
- Navigation to Chapter Select, Statistics, Extras, and Credits
- Custom music playback
- Smooth transitions and animations

**States:**
- `Start`: Main menu
- `ChapterSelect`: Chapter selection
- `Statistics`: Statistics notebook
- `Credits`: Credits roll
- `Extras`: Extra content
- `Exiting`: Exit transition

#### 2. **OuiChapterSelectDesoloZatnas** (`OuiChapterSelectDesoloZatnas.cs`)
Chapter selection interface for custom areas 10-18 and DLC content.

**Features:**
- Scrollable chapter list
- Support for 9 custom chapters (10-18)
- 3 DLC chapters (19-21)
- Lobby/stage selection mode
- A/B/C/D-Side selection per chapter
- Progress tracking and completion indicators

**Navigation:**
- Up/Down: Navigate chapters
- Confirm: Enter lobby mode
- Cancel: Return to main menu

#### 3. **OuiStatisticsNotebook** (`OuiStatisticsNotebook.cs`)
Statistics tracking and display system.

**Pages:**
1. **Overview**: Total playtime, deaths, completion percentage
2. **Deaths**: Deaths by chapter breakdown
3. **Collections**: Strawberries, hearts, golden berries, pink platinum berries
4. **Enemy Kills**: Total and by enemy type
5. **Dashes**: Ground, air, wavedashes, hyperdashes, super dashes
6. **Speedruns**: Best times by chapter

**Navigation:**
- Left/Right: Switch pages
- Up/Down: Scroll content
- Cancel: Return to main menu

#### 4. **OuiDSidePostcard** (`OuiDSidePostcard.cs`)
D-Side unlock and reward system.

**Unlock Requirement:**
- Collect all A-Side, B-Side, and C-Side heart gems for a chapter

**Rewards:**
- Heart Gem
- Pink Platinum Berry

**Features:**
- Postcard gallery view
- Left/Right navigation between unlocked postcards
- Reward collection screen
- Completion tracking

#### 5. **GameOverScreen** (`GameOverScreen.cs`)
Custom game over screen with lives system.

**Features:**
- Displays chapter name, death count, and time spent
- Menu options: Retry, Return to Map, Quit
- Custom game over music
- Motivational quotes

**Lives System:**
- Default: 3 lives
- Maximum: 9 lives
- Game over triggers when lives reach 0

#### 6. **OuiCreditsDesoloZatnas** (`OuiCreditsDesoloZatnas.cs`)
Credits screen with auto-scrolling.

**Sections:**
- Created By
- Powered By (Everest, Celeste Mod Helpers)
- Helper Mods
- Inspired By (Snowberry, Randomizer, PICO-8)
- Special Thanks

**Controls:**
- Auto-scroll by default
- Up/Down: Manual scroll
- Confirm: Resume auto-scroll
- Cancel: Return to main menu

#### 7. **CustomMountainRenderer** (`CustomMountainRenderer.cs`)
Custom mountain visualization for chapters 10-21.

**Features:**
- Custom 3D model integration
- Chapter positioning on mountain
- Spiral layout for chapters 10-18
- Peak locations for DLC chapters

## Integration

### OverworldIntegration.cs

The `OverworldIntegration` class hooks into Celeste's `Overworld.ReloadMenus` to register custom OUI screens:

```csharp
public static void InstallHooks()
{
    On.Celeste.Overworld.ReloadMenus += Overworld_ReloadMenus;
}

private static void Overworld_ReloadMenus(On.Celeste.Overworld.orig_ReloadMenus orig, Overworld self, Overworld.StartMode startMode)
{
    orig(self, startMode);
    
    if (useCustomUI)
    {
        RegisterOuiIfNotExists<OuiMainMenuDesoloZatnas>(self);
        RegisterOuiIfNotExists<OuiChapterSelectDesoloZatnas>(self);
        RegisterOuiIfNotExists<OuiStatisticsNotebook>(self);
        RegisterOuiIfNotExists<OuiDSidePostcard>(self);
        RegisterOuiIfNotExists<OuiCreditsDesoloZatnas>(self);
    }
}
```

### Save Data Integration

Custom save data in `IngesteModuleSaveData.cs`:

```csharp
public class IngesteModuleSaveData : EverestModuleSaveData
{
    // D-Side System
    public HashSet<int> DSideCompleted { get; set; }
    public HashSet<int> DSideRewardsCollected { get; set; }
    
    // Statistics
    public DesoloZantasStatsData Statistics { get; set; }
}
```

## Audio System

### Music Tracks

Custom music events (defined in `OuiHelpers.cs`):

- `MENU_MAIN`: Main menu music
- `MENU_STATISTICS`: Statistics notebook music
- `MENU_CREDITS`: Credits music
- `GAME_OVER`: Game over screen music
- `POSTCARD_UNLOCK`: D-Side postcard unlock music

### Audio Banks

Required audio banks (in `Audio/` folder):
- `music_desolozatnas.bank`
- `sfx_desolozatnas.bank`
- `ui_desolozatnas.bank`

## Dialog System

All dialog entries are in `Dialog/English_DesoloZatnas_UI.txt`.

Example entries:
```
DESOLOZATNAS_MENU_CHAPTER_SELECT= Chapter Select
DESOLOZATNAS_STATS_OVERVIEW= Overview
DESOLOZATNAS_DSIDE_TITLE= D-Side Postcards
```

## Helper Classes

### OuiHelpers.cs

**CustomOuiAudio:**
- `PlayMenuMusic(string track)`: Play menu music
- `StopMenuMusic()`: Stop menu music
- `PlaySound(string sfx)`: Play sound effect

**OuiRenderHelper:**
- `DrawTextWithOutline()`: Draw text with outline
- `DrawCenteredText()`: Draw centered text
- `DrawPanel()`: Draw colored panel
- `DrawPanelWithBorder()`: Draw panel with border

**OuiNavigationHelper:**
- `GoToMainMenu()`: Navigate to main menu
- `GoToChapterSelect()`: Navigate to chapter select
- `GoToStatistics()`: Navigate to statistics

**OuiInputHelper:**
- Button state properties
- Sound effect helpers

## Usage Examples

### Navigating to Custom Menu

```csharp
// From anywhere in the Overworld
Overworld.Goto<OuiMainMenuDesoloZatnas>();
```

### Tracking Statistics

```csharp
// Increment death counter
IngesteModule.SaveData.Statistics.IncrementDeaths(areaID);

// Record enemy kill
IngesteModule.SaveData.Statistics.IncrementEnemyKill("Boss");

// Record speedrun time
IngesteModule.SaveData.Statistics.RecordChapterTime(areaID, timeInTicks);
```

### Checking D-Side Unlock

```csharp
bool hasAllHearts = HasAllHearts(areaID); // A, B, C-Side hearts
if (hasAllHearts)
{
    // D-Side is unlocked
}
```

### Triggering Game Over

```csharp
if (LivesSystem.LoseLife())
{
    // Out of lives - show game over
    Engine.Scene = new GameOverScreen(level, session);
}
```

## Customization

### Adding New Menu Options

1. Add new menu button in `OuiMainMenuDesoloZatnas.InitializeButtons()`
2. Create corresponding OUI screen class
3. Register in `OverworldIntegration.Overworld_ReloadMenus()`
4. Add dialog entries

### Adding New Statistics

1. Add properties to `DesoloZantasStatsData`
2. Add increment/tracking methods
3. Update `OuiStatisticsNotebook` rendering
4. Hook into relevant game events

### Customizing Visual Style

Colors, fonts, and layout can be adjusted in each OUI class:

```csharp
// Example: Change menu title color
Color titleColor = Color.Gold;
ActiveFont.DrawOutline(titleText, position, justify, scale, titleColor, thickness, outlineColor);
```

## File Structure

```
Source/Core/UI/
├── OuiMainMenuDesoloZatnas.cs          # Main menu
├── OuiChapterSelectDesoloZatnas.cs     # Chapter selection
├── OuiStatisticsNotebook.cs            # Statistics notebook
├── OuiDSidePostcard.cs                 # D-Side postcard system
├── GameOverScreen.cs                    # Game over screen
├── OuiCreditsDesoloZatnas.cs           # Credits
├── CustomMountainRenderer.cs            # Mountain visualization
└── OuiHelpers.cs                        # Helper utilities

Dialog/
└── English_DesoloZatnas_UI.txt         # UI dialog entries

Audio/
├── music_desolozatnas.bank
├── sfx_desolozatnas.bank
└── ui_desolozatnas.bank
```

## Planned Features

- [ ] Art gallery in Extras menu
- [ ] Music player in Extras menu
- [ ] Achievement system
- [ ] Animated backgrounds
- [ ] Custom save file visuals
- [ ] Postcard illustrations
- [ ] 3D mountain model enhancements

## Credits

Inspired by:
- **Snowberry** by catapillie - In-game map editor with custom UI
- **Randomizer** - External menu approach
- **PICO-8** - Visual style and aesthetic

## License

Part of the DesoloZatnas Celeste mod project.
