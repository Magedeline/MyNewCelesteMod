# OUI Debug Logging Guide

## Overview
Enhanced logging has been added to diagnose why the vanilla OUI (Overworld User Interface) keeps coming back instead of showing the custom Desolo Zatnas UI.

## What Was Added

### 1. OverworldIntegration.cs Logging

**`Overworld_ReloadMenus` hook:**
- Logs when ReloadMenus is called and with what StartMode
- Lists all existing UIs before custom registration
- Tracks total UI count

**`RegisterCustomUI` method:**
- Logs the value of `UseDesoloZantasBranding` setting
- Tracks when Desolo vs vanilla branding is selected
- Logs hook installation/removal

**`RegisterOuiIfNotExists` method:**
- Logs when a new OUI is registered
- Logs when an OUI already exists (skipped)

### 2. OuiTitleScreenMod.cs Logging

**`InstallHooks` method:**
- Logs when hooks are installed
- Logs if hooks are already installed (preventing duplicate installation)

**`RemoveHooks` method:**
- Logs when hooks are removed
- Logs if hooks weren't installed (preventing errors)

**`OnVanillaTitleScreen_IsStart` hook:**
- Logs when vanilla title screen tries to determine if it should start
- Shows the StartMode and UseDesoloBranding setting value
- Logs whether it's blocking or allowing the vanilla screen

**`OnVanillaTitleScreen_Render` hook:**
- Logs when vanilla title screen tries to render (verbose level)
- Shows whether rendering is blocked or allowed

**`IsStart` method:**
- Logs when OuiTitleScreenMod checks if it should start
- Shows the StartMode and decision

## How to Use These Logs

### Step 1: Reproduce the Issue
1. Launch the game with the mod
2. Navigate to where you see the vanilla OUI appearing instead of custom UI
3. Take note of when this happens (e.g., on startup, after returning from a level, etc.)

### Step 2: Find the Log File
The log file is typically located at:
- **Windows:** `%LOCALAPPDATA%\Celeste\log.txt`
- **Full path example:** `C:\Users\[YourName]\AppData\Local\Celeste\log.txt`

### Step 3: Search for Debug Messages
Look for lines containing `[OUI Debug]`. These will show you:

1. **When menus are reloaded:**
   ```
   [OUI Debug] Overworld_ReloadMenus called with StartMode: Titlescreen
   ```

2. **What UIs exist:**
   ```
   [OUI Debug] Existing UI: OuiTitleScreen
   [OUI Debug] Existing UI: OuiMainMenu
   ```

3. **Branding setting value:**
   ```
   [OUI Debug] RegisterCustomUI - UseDesoloZantasBranding: True
   ```

4. **Hook status:**
   ```
   [OUI Debug] Vanilla OuiTitleScreen hooks INSTALLED (Desolo branding active)
   ```
   or
   ```
   [OUI Debug] InstallHooks called but hooks already installed - skipping
   ```

5. **UI registration:**
   ```
   [OUI Debug] Registered NEW: OuiTitleScreenMod
   ```
   or
   ```
   [OUI Debug] Already exists (skipped): OuiTitleScreenMod
   ```

6. **IsStart checks:**
   ```
   [OUI Debug] OuiTitleScreenMod.IsStart - StartMode: Titlescreen, ShouldStart: True
   [OUI Debug] OnVanillaTitleScreen_IsStart called - StartMode: Titlescreen, UseDesoloBranding: True
   [OUI Debug] Blocking vanilla OuiTitleScreen.IsStart (returning false)
   ```

### Step 4: Common Issues to Look For

#### Issue 1: Settings Value Wrong
If you see:
```
[OUI Debug] RegisterCustomUI - UseDesoloZantasBranding: False
```
**Solution:** The setting is disabled. Enable it in mod settings.

#### Issue 2: Hooks Not Installing
If you don't see:
```
[OUI Debug] Vanilla OuiTitleScreen hooks INSTALLED
```
**Solution:** Check for errors during mod load. Hooks may have failed to install.

#### Issue 3: ReloadMenus Called Multiple Times
If you see many:
```
[OUI Debug] Overworld_ReloadMenus called with StartMode: [various modes]
```
**Solution:** This is normal, but check if hooks are being removed/reinstalled unnecessarily.

#### Issue 4: Vanilla UI Taking Priority
If you see:
```
[OUI Debug] OnVanillaTitleScreen_IsStart called - StartMode: Titlescreen, UseDesoloBranding: True
[OUI Debug] Allowing vanilla OuiTitleScreen.IsStart (returning true)
```
**Solution:** This indicates the hook logic isn't working correctly.

#### Issue 5: Custom OUI Not Starting
If you see:
```
[OUI Debug] OuiTitleScreenMod.IsStart - StartMode: Titlescreen, ShouldStart: False
```
**Solution:** Settings check or StartMode mismatch.

## Expected Flow (Normal Operation)

When everything works correctly, you should see:

```
[OUI Debug] Overworld_ReloadMenus called with StartMode: Titlescreen
[OUI Debug] After orig() - Total UI count: [X]
[OUI Debug] Existing UI: OuiTitleScreen
[OUI Debug] Existing UI: OuiMainMenu
[... other vanilla UIs ...]
[OUI Debug] RegisterCustomUI - UseDesoloZantasBranding: True
[OUI Debug] Registering Desolo Zatnas custom UI screens...
[OUI Debug] Registered NEW: OuiTitleScreenMod
[OUI Debug] Already exists (skipped): OuiMainMenuMod  (if reloading)
[... other registrations ...]
[OUI Debug] Installing OuiTitleScreenMod hooks...
[OUI Debug] Vanilla OuiTitleScreen hooks INSTALLED (Desolo branding active)
[OUI Debug] Hooks installed
[OUI Debug] OuiTitleScreenMod.IsStart - StartMode: Titlescreen, ShouldStart: True
[OUI Debug] OnVanillaTitleScreen_IsStart called - StartMode: Titlescreen, UseDesoloBranding: True
[OUI Debug] Blocking vanilla OuiTitleScreen.IsStart (returning false)
```

## Next Steps

1. **Collect logs** while reproducing the issue
2. **Share the relevant log section** (lines with `[OUI Debug]`) 
3. **Note the exact behavior** you're seeing vs. what you expect
4. **Check the pattern** - does it fail immediately, or after some action?

## Disabling Verbose Logging

The `Render` hook uses `LogLevel.Verbose` to avoid spam. If you need to see render logging:
1. Temporarily change `LogLevel.Verbose` to `LogLevel.Info` in `OnVanillaTitleScreen_Render`
2. Rebuild the mod
3. **Warning:** This will generate a LOT of log entries (every frame)

## Related Files

- `Source/Core/OverworldIntegration.cs` - Main overworld hook management
- `Source/Core/UI/OuiTitleScreenMod.cs` - Custom title screen and vanilla blocking
- `Source/Core/IngesteModule.cs` - Module initialization
- `Source/Core/IngesteModuleSettings.cs` - Settings including `UseDesoloZantasBranding`
