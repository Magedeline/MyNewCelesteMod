# Quick Start Guide - Custom Overworld Integration

## Step 1: Add to Your Mod Module

Add this to your main mod class (e.g., `IngesteModule.cs` or similar):

```csharp
using Celeste.Mod.DesoloZatnas.Core;

public class DesoloZantasModule : EverestModule
{
    public override void Load()
    {
        // Install custom overworld hooks
        OverworldIntegration.InstallHooks();
    }

    public override void Unload()
    {
        // Clean up overworld hooks
        OverworldIntegration.UninstallHooks();
    }
}
```

## Step 2: Add Dialog Strings

Create or append to `Dialog/English.txt`:

```
# Copy the dialog strings from Source/Core/UI/README.md
# Section: "Dialog Strings"
```

Minimum required strings:
- UI_TITLE_PRESSANYBUTTON
- UI_MAIN_* (menu items)
- UI_FILE_* (file selection)
- UI_CHAPTER_* (chapter selection)
- UI_PANEL_* (chapter panel)
- UI_JOURNAL_* (journal pages)
- UI_OPTIONS_* (options menu)
- UI_ASSIST_* (assist mode)

## Step 3: Build Your Mod

```powershell
dotnet build
```

## Step 4: Test

1. Launch Celeste with Everest
2. Start your mod
3. Navigate through the custom menus
4. Verify all screens appear correctly

## Quick Customization

### Change Title Text
Edit `OuiTitleScreenMod.cs`:
```csharp
private string customTitleText = "YOUR MOD NAME";
```

### Change Menu Colors
Find color definitions like:
```csharp
Color textColor = Calc.HexToColor("FFD700"); // Gold
```

Replace with your colors.

### Change Credits
Edit `OuiCreditMod.cs`:
```csharp
creditLines = new string[]
{
    "YOUR MOD NAME",
    "",
    "Created by YOUR NAME",
    // ... add your credits
};
```

## Troubleshooting

**Problem: UI not showing**
- Check that hooks are installed in Load()
- Verify all files are in correct directories
- Check build output for errors

**Problem: Missing text**
- Add dialog strings to English.txt
- Rebuild the mod
- Check dialog file is being loaded

**Problem: Crashes on startup**
- Check error logs in Celeste/ErrorLog.txt
- Verify all dependencies are present
- Make sure namespace matches your mod structure

## Full Documentation

See `Source/Core/UI/README.md` for complete documentation.
