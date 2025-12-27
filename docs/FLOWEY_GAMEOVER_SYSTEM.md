# Flowey Game Over System

## Overview
The Flowey Game Over system is an evil, meta twist on the normal game over screen. Inspired by Undertale's Flowey, this system occasionally appears when the player (Madeline/Kirby) dies, delivering rude and negative feedback before forcefully closing the game with maniacal laughter.

## Features

### Random Appearance
- **15% base chance** of appearing on death
- Increases by up to 25% based on total death count (scaling with player failures)
- Won't appear more than once every 5 deaths minimum
- Smart trigger system prevents it from being too frequent or too rare

### Evil Dialog Sequence
The system displays a sequence of insulting messages, including the specifically requested ones:
1. "Holy moly that was a nightmare... I'm glad that was just a dream!"
2. "And you will NEVER EVER wake up... you IDIOT!"
3. Plus 13 additional rude messages randomly selected

### Visual Effects
- Creepy Flowey face (with fallback to text-based smiley if sprite unavailable)
- Pulsing and shaking animations during laugh sequence
- Screen wipe transitions
- Dark, atmospheric presentation

### Laugh Sequence
After displaying evil messages, Flowey:
1. Displays "Hee hee hee..."
2. Escalates to "HA HA HA HA HA!"
3. Then "AHAHAHAHA!"
4. Finally: "See you later... IDIOT!"
5. Shows countdown timer
6. **Closes the game** (saves progress first)

### Escape Mechanism
Players can escape by holding the Grab button for 3 seconds before the laugh sequence starts, returning to a normal Game Over screen.

## Implementation Files

### Core Files
- **FloweyGameOver.cs** - Main scene handling the evil game over
- **FloweyGameOverTrigger** - Static helper class managing when Flowey appears
- **PlayerDeadBody.cs** - Modified to check for Flowey trigger
- **KirbyDeadBody.cs** - Modified to check for Flowey trigger

### Dialog
All Flowey messages are defined in `Dialog/English.txt`:
- `DESOLOZATNAS_FLOWEY_GAMEOVER_NIGHTMARE` - First mandatory message
- `DESOLOZATNAS_FLOWEY_GAMEOVER_NEVER_WAKE` - Second mandatory message
- `DESOLOZATNAS_FLOWEY_GAMEOVER_1` through `DESOLOZATNAS_FLOWEY_GAMEOVER_15` - Additional evil messages

## Audio Requirements

### Current Placeholders
The system currently uses placeholder sounds from Badeline:
- Appearance: `event:/char/badeline/boss_prefight_reveal`
- Message transitions: `event:/char/badeline/boss_bullet`
- Evil laugh: `event:/char/badeline/laugh_01`

### Recommended Custom Sounds
For full effect, add these custom audio events:
- `event:/Ingeste/flowey/evil_appear` - Flowey's creepy appearance
- `event:/Ingeste/flowey/evil_laugh` - Maniacal laughter
- `event:/Ingeste/flowey/message_tick` - Sound for message transitions
- `event:/Ingeste/flowey/game_close` - Final sound before game closes

## Graphics Requirements

### Flowey Sprites
Place Flowey sprites in: `Graphics/Atlases/Gameplay/characters/flowey/`

Recommended animations:
- `evil` - Creepy smiling face (looping)
- `laugh` - Laughing animation (looping, more intense)

If sprites are not available, the system will use a text-based fallback (=D and =) faces).

## Technical Details

### Trigger Algorithm
```csharp
float baseChance = 0.15f; // 15%
float bonusChance = Math.Min(totalDeaths / 100f * 0.1f, 0.25f);
float totalChance = baseChance + bonusChance;

// Plus minimum 5 death cooldown between triggers
```

### Message Display
- Each message displays for 2.5 seconds
- 3-4 messages shown per sequence (always includes the 2 mandatory ones)
- Messages selected randomly from pool of 15+

### Game Closure
- Automatically saves player progress before closing
- 2-second countdown warning
- Clean exit using `Engine.Instance.Exit()`

## Usage Notes

### For Players
- This is a rare, special event - don't be alarmed!
- Your progress is saved before the game closes
- Hold Grab button to escape if you really need to
- Frequency increases slightly with death count (Flowey knows!)

### For Developers
- Test trigger manually by calling `FloweyGameOverTrigger.ShouldTriggerFlowey(session)` with a modified chance
- Adjust `BASE_CHANCE` and `MIN_DEATHS_BETWEEN_FLOWEY` constants for different feel
- Add more dialog entries to increase variety
- Replace placeholder sounds with custom Flowey audio for best effect

## Future Enhancements

Potential additions:
- Different Flowey expressions for different message types
- Screen glitching effects during laugh sequence
- Fake file deletion threat (visual only, harmless)
- Save file name reveal (meta horror)
- Integration with Chapter 9 meta-narrative moments
- Multiple laugh sound variations
- Custom particle effects

## Compatibility

Works with:
- Normal Madeline player
- Kirby player mode
- Any custom player skins
- Lives system (bypasses it)
- Boss Helper fake death system (Flowey won't appear during fake deaths)

## Credits

Inspired by:
- Undertale's Flowey (Toby Fox)
- Meta horror games (DDLC, OneShot, etc.)
- The original request for evil game over feedback

---

*"You IDIOT! >=)"* - Flowey
