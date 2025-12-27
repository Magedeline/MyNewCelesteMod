# DesoloZatnas Quick Start Guide

## Installation

### Step 1: Install Hooks in Your Module

Add to your `EverestModule` class:

```csharp
using Celeste.Mod.DesoloZatnas.Core;

public class YourModule : EverestModule
{
    public override void Load()
    {
        // Install custom overworld hooks
        OverworldIntegration.InstallHooks();
        
        // Install other hooks as needed
        VanillaCoreHooks.Install();
    }

    public override void Unload()
    {
        // Clean up hooks
        OverworldIntegration.UninstallHooks();
        VanillaCoreHooks.Uninstall();
    }
}
```

### Step 2: Add Required Dialog Strings

Add these to `Dialog/English.txt`:

```
# === MAIN MENU ===
UI_MAIN_TITLE=DESOLO ZATNAS
UI_MAIN_CONTINUE=Continue
UI_MAIN_NEWGAME=New Game
UI_MAIN_CHAPTERS=Chapter Select
UI_MAIN_STATISTICS=Statistics
UI_MAIN_CREDITS=Credits
UI_MAIN_OPTIONS=Options
UI_MAIN_EXIT=Exit

# === FILE SELECT ===
UI_FILE_TITLE=Select Save File
UI_FILE_SLOT=File {0}
UI_FILE_EMPTY=Empty
UI_FILE_DELETE=Delete
UI_FILE_DELETE_CONFIRM=Delete this save file?

# === CHAPTER SELECT ===
UI_CHAPTER_TITLE=Chapter Select
UI_CHAPTER_ASIDE=A-Side
UI_CHAPTER_BSIDE=B-Side
UI_CHAPTER_CSIDE=C-Side
UI_CHAPTER_DSIDE=D-Side
UI_CHAPTER_LOCKED=Locked
UI_CHAPTER_STRAWBERRIES=Strawberries: {0}/{1}
UI_CHAPTER_DEATHS=Deaths: {0}
UI_CHAPTER_TIME=Time: {0}

# === STATISTICS ===
UI_STATS_TITLE=Statistics
UI_STATS_OVERVIEW=Overview
UI_STATS_DEATHS=Deaths
UI_STATS_COLLECTIONS=Collections
UI_STATS_ENEMIES=Enemies Defeated
UI_STATS_DASHES=Dashes
UI_STATS_SPEEDRUN=Speedrun Times
UI_STATS_TOTAL_TIME=Total Playtime: {0}
UI_STATS_TOTAL_DEATHS=Total Deaths: {0}
UI_STATS_COMPLETION=Completion: {0}%

# === GAME OVER ===
UI_GAMEOVER_TITLE=GAME OVER
UI_GAMEOVER_RETRY=Retry
UI_GAMEOVER_MAP=Return to Map
UI_GAMEOVER_QUIT=Quit
UI_GAMEOVER_DEATHS=Deaths: {0}
UI_GAMEOVER_TIME=Time: {0}

# === CREDITS ===
UI_CREDITS_TITLE=Credits
UI_CREDITS_CREATEDBY=Created By
UI_CREDITS_POWEREDBY=Powered By
UI_CREDITS_HELPERS=Helper Mods
UI_CREDITS_THANKS=Special Thanks

# === OPTIONS ===
UI_OPTIONS_TITLE=Options
UI_OPTIONS_AUDIO=Audio
UI_OPTIONS_VIDEO=Video
UI_OPTIONS_CONTROLS=Controls
UI_OPTIONS_GAMEPLAY=Gameplay

# === LIVES SYSTEM ===
UI_LIVES_REMAINING=Lives: {0}
UI_LIVES_GAINED=+1 Life!
UI_LIVES_LOST=Life Lost
```

### Step 3: Build and Test

```powershell
dotnet build
```

Launch Celeste and verify:
1. Custom main menu appears
2. All text displays correctly
3. Navigation works smoothly

## Quick Customization

### Change Branding

In your module settings:

```csharp
public class YourModuleSettings : EverestModuleSettings
{
    [SettingName("Use Custom Branding")]
    public bool UseDesoloZantasBranding { get; set; } = true;
}
```

### Change Title Text

Edit `OuiMainMenuDesoloZatnas.cs`:

```csharp
private const string TitleText = "YOUR MOD NAME";
```

### Change Colors

```csharp
// Title color
Color titleColor = Calc.HexToColor("FFD700"); // Gold

// Menu highlight
Color highlightColor = Calc.HexToColor("00FFFF"); // Cyan

// Background
Color bgColor = Color.Black * 0.85f;
```

### Add Menu Items

```csharp
menuItems.Add(new MenuItem("MY_OPTION", MyOptionAction));
```

## Troubleshooting

| Problem | Solution |
|---------|----------|
| UI not showing | Verify `InstallHooks()` called in `Load()` |
| Missing text | Add dialog strings to `English.txt` |
| Crashes on startup | Check `ErrorLog.txt` for missing dependencies |
| Vanilla UI appears | Check `UseDesoloZantasBranding` setting is `true` |
| Audio not playing | Verify `.bank` files exist in `Audio/` folder |

## Debug Mode

Enable verbose logging in your module:

```csharp
Logger.SetLogLevel("DesoloZatnas", LogLevel.Verbose);
```

Check logs for `[OUI Debug]` messages to trace UI registration and hook execution.

## Next Steps

- Read [OUI_SYSTEM.md](OUI_SYSTEM.md) for architecture details
- See [OUI_IMPLEMENTATION_SUMMARY.md](OUI_IMPLEMENTATION_SUMMARY.md) for feature list
- Check [OUI_DEBUG_LOGGING.md](OUI_DEBUG_LOGGING.md) for debugging
