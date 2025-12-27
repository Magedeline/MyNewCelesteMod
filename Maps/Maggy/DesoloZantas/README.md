# DesoloZantas Map Organization

This document describes the new vanilla-like map organization structure for the DesoloZantas mod.

## Folder Structure

```
Maps/Maggy/DesoloZantas/
├── Chapters/          # Main story chapters (Prologue through Post-Epilogue)
│   ├── 00_Prologue.bin
│   ├── 01_City_A.bin
│   ├── 01_City_B.bin
│   ├── 01_City_C.bin
│   ├── 01_City_D.bin
│   ├── ...
│   ├── 17_Epilogue.bin
│   ├── 18_Heart_A.bin
│   ├── ...
│   ├── 20_TheEnd_A.bin
│   └── 21_PostEpilogue.bin
├── Submaps/           # Sub-areas within chapters
│   ├── 01_City_Sub1.bin
│   ├── 01_City_Sub2.bin
│   ├── ...
├── Lobbies/           # Chapter lobby areas
│   ├── 00_Prologue_Lobby.bin
│   ├── 01_City_Lobby.bin
│   ├── ...
├── Gyms/              # Tutorial/practice areas
│   ├── gym_combo.bin
│   ├── gym_wavedash.bin
│   ├── ...
└── WIP/               # Work-in-progress maps
    ├── kirbytest.bin
    └── test.bin
```

## Story Progression

The campaign follows this order:

1. **Prologue** (Chapter 0) - Awakening Dream
2. **Chapter 1** - City
3. **Chapter 2** - Nightmare
4. **Chapter 3** - Stars
5. **Chapter 4** - Legend
6. **Chapter 5** - Restore
7. **Chapter 6** - Stronghold
8. **Chapter 7** - Hell
9. **Chapter 8** - Truth
10. **Chapter 9** - Summit
11. **Chapter 10** - Ruins
12. **Chapter 11** - Snow
13. **Chapter 12** - Water
14. **Chapter 13** - Time
15. **Chapter 14** - Digital
16. **Chapter 15** - Castle
17. **Chapter 16** - Corruption
18. **Epilogue** (Chapter 17) - Final Resonance
19. **Chapter 18** - Heart
20. **Chapter 19** - Space
21. **Chapter 20** - The End
22. **Post-Epilogue** (Chapter 21) - Post Respite

## Naming Convention

### Main Chapters
- Format: `{ChapterNumber:D2}_{ChapterName}_{Side}.bin`
- Examples:
  - `00_Prologue.bin` (no side suffix for special chapters)
  - `01_City_A.bin` (A-Side)
  - `01_City_B.bin` (B-Side)
  - `01_City_C.bin` (C-Side)
  - `01_City_D.bin` (D-Side)

### Submaps
- Format: `{ChapterNumber:D2}_{ChapterName}_Sub{Index}.bin`
- Examples:
  - `01_City_Sub1.bin`
  - `05_Restore_Sub2.bin`

### Lobbies
- Format: `{ChapterNumber:D2}_{ChapterName}_Lobby.bin`
- Examples:
  - `00_Prologue_Lobby.bin`
  - `01_City_Lobby.bin`

### Gyms
- Format: `gym_{name}.bin`
- Examples:
  - `gym_wavedash.bin`
  - `gym_combo.bin`

## C# Integration

The map organization is managed through these classes:

### MapSIDRegistry
Located at: `Source/Core/Maps/MapSIDRegistry.cs`

Provides:
- All map SID constants organized by chapter
- `MapInfo` class with full metadata
- Methods to query maps by type, chapter, or side

Example usage:
```csharp
// Get a specific chapter's A-Side
var chapter5ASide = MapSIDRegistry.Chapter05.GetASide();

// Get all A-Sides in story order
var allASides = MapSIDRegistry.GetAllASides();

// Get submaps for a chapter
var submaps = MapSIDRegistry.GetSubmapsForChapter(1);
```

### MapOrganizer
Located at: `Source/Core/Maps/MapOrganizer.cs`

Provides:
- Path resolution utilities
- Migration mappings from old to new structure
- Story order constants

Example usage:
```csharp
// Get the path for a chapter
string path = MapOrganizer.GetChapterPath(5, MapSIDRegistry.ChapterSide.BSide);

// Get next chapter in story
int? nextChapter = MapOrganizer.GetNextChapter(16); // Returns 17 (Epilogue)
```

### MapHelper
Located at: `Source/Core/Maps/MapHelper.cs`

Provides:
- Runtime map loading helpers
- AreaKey resolution
- Chapter transition utilities

Example usage:
```csharp
// Initialize on mod load
MapHelper.Initialize();

// Transition to a chapter
MapHelper.TransitionToChapter(5, MapSIDRegistry.ChapterSide.ASide);

// Check if a chapter is special (prologue/epilogue/post-epilogue)
bool isSpecial = MapHelper.IsSpecialChapter(17); // true
```

## Migration from Old Structure

The old structure used separate folders:
- `Main/` - A-Sides (Alt1)
- `Main2/` - B-Sides (Alt2)
- `Main3/` - C-Sides (Alt3)
- `Main4/` - D-Sides (Alt4)
- `Submaps/` - Submaps
- `Lobbies/` - Lobbies
- `Gyms/` - Gyms
- `WIP/` - Work in progress

To migrate, run:
```powershell
.\Scripts\Migrate-Maps.ps1 -ModRootPath "path\to\mod"
```

Add `-DryRun` to preview without making changes.

## Meta Files

Each map can have associated metadata files:
- `{MapName}.meta.yaml` - Map configuration
- `{MapName}.altsideshelper.meta.yaml` - AltSidesHelper integration

These should be placed alongside the `.bin` files in the appropriate folder.
