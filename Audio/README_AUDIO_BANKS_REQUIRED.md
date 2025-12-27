# Audio Banks Required

## ?? IMPORTANT: Missing Audio Files

This mod requires FMOD audio bank files to play music and sound effects. Currently, these files are **missing**, which is why you cannot hear any audio.

## What You Need

Place the following files in this `Audio` directory:

### Required Bank Files:
1. **Master.bank** - Main FMOD master bank
2. **Master.strings.bank** - String lookup for audio events
3. **Ingeste_Bank_Music.bank** - Music tracks bank
4. **Ingeste_Bank_SFX.bank** - Sound effects bank

## How to Get These Files

### Option 1: Build from FMOD Studio (Recommended)
1. Install [FMOD Studio](https://www.fmod.com/download)
2. Create an FMOD project with events matching `GUIDs.txt`
3. Build the banks (File ? Build)
4. Copy the `.bank` files from the Build folder to this directory

### Option 2: Use Placeholder Banks (Temporary)
If you don't have the actual audio yet, you can:
1. Create empty/placeholder banks with the same event names
2. Use vanilla Celeste sounds as temporary replacements
3. The mod now has fallback code to prevent crashes when audio is missing

## Audio Events Reference

All audio events are defined in `GUIDs.txt` in this directory. Key events include:

### Music Events:
- `event:/Ingeste/music/lvl5/oshiro_theme`
- `event:/Ingeste/music/lvl2/evil_chara`
- `event:/Ingeste/music/lvl10/intro`

### SFX Events:
- `event:/char/kirby/*` - Kirby character sounds
- `event:/char/dialogue/*` - Character dialogue samples
- `event:/game/03_resort/*` - Resort-specific sounds

## Current Behavior

The mod has been updated with safe audio handling:
- Missing audio events will log warnings instead of crashing
- Fallback sounds from vanilla Celeste will play when available
- Music triggers will fail gracefully if banks are missing

## Testing

To verify your audio banks are working:
1. Place the `.bank` files in this directory
2. Launch the game
3. Check the Everest log for "DesoloZantas" audio warnings
4. Test in-game to confirm sounds play

## Need Help?

If you're having trouble:
- Check the Everest log file for specific error messages
- Verify all `.bank` files are in the correct format (FMOD 2.0+)
- Make sure GUIDs in the banks match those in `GUIDs.txt`
- Consult the [Celeste modding wiki](https://github.com/EverestAPI/Resources/wiki/Adding-Music) for audio setup guides
