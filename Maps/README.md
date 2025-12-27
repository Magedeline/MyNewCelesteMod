# DesoloZatnas Maps

## Chapter Structure

All maps are located in `Maps/Maggy/DesoloZantas/Chapters/`

### Main Chapters (00-16)

| Chapter | Chapter Name | Map Files | Description |
|---------|--------------|-----------|-------------|
| 00 | Prologue | `00_Prologue.bin` | Introduction and tutorial |
| 01 | City | `01_City_[A/B/C/D].bin` | Urban environment |
| 02 | Nightmare | `02_Nightmare_[A/B/C/D].bin` | Dark dreamscape |
| 03 | Stars | `03_Stars_[A/B/C/D].bin` | Celestial realm |
| 04 | Legend | `04_Legend_[A/B/C/D].bin` | Mythic journey |
| 05 | Restore | `05_Restore_[A/B/C/D].bin` | Recovery chapter |
| 06 | Stronghold | `06_Stronghold_[A/B/C/D].bin` | Fortress ascent |
| 07 | Hell | `07_Hell_[A/B/C/D].bin` | Infernal depths |
| 08 | Truth | `08_Truth_[A/B/C/D].bin` | Revelation chapter |
| 09 | Summit | `09_Summit_[A/B/C/D].bin` | Mountain climb |
| 10 | Ruins | `10_Ruins_[A/B/C/D].bin` | Ancient ruins |
| 11 | Snow | `11_Snow_[A/B/C/D].bin` | Frozen wasteland |
| 12 | Water | `12_Water_[A/B/C/D].bin` | Aquatic depths |
| 13 | Time | `13_Time_[A/B/C/D].bin` | Temporal puzzles |
| 14 | Digital | `14_Digital_[A/B/C/D].bin` | Virtual world |
| 15 | Castle | `15_Castle_[A/B/C/D].bin` | Royal fortress |
| 16 | Corruption | `16_Corruption_A.bin` | Dark corruption (A-Side only) |

### Story Chapters (17, 21)

| Chapter | Chapter Name | Map Files | Description |
|---------|--------------|-----------|-------------|
| 17 | Epilogue | `17_Epilogue.bin` | Story epilogue |
| 21 | Post Epilogue | `21_PostEpilogue.bin` | Post-game story |

### DLC Chapters (18-20)

| Chapter | Chapter Name | Map Files | Description |
|---------|--------------|-----------|-------------|
| 18 | Heart | `18_Heart_[A/B/C/D].bin` | Heart chapter with all sides |
| 19 | Space | `19_Space_A.bin` | Space exploration (A-Side) |
| 20 | The End | `20_TheEnd_A.bin` | Advanced challenge maps featuring remixed mechanics from main chapters, speedrun-focused rooms, and expert-level platforming sequences with D-Side difficulty |

## Naming Conventions

### Map Files

```
Maps/Maggy/DesoloZantas/Chapters/XX_ChapterName_Side.bin

Format: [ChapterNumber]_[ChapterName]_[Side].bin

Examples:
00_Prologue.bin           # Prologue (no sides)
01_City_A.bin             # Chapter 1 City A-Side
01_City_B.bin             # Chapter 1 City B-Side
02_Nightmare_C.bin        # Chapter 2 Nightmare C-Side
03_Stars_D.bin            # Chapter 3 Stars D-Side
17_Epilogue.bin           # Epilogue (no sides)
20_TheEnd_A.bin           # Chapter 20 The End A-Side
```

### Associated Metadata Files

Each map can have associated metadata files:
- `XX_ChapterName_Side.meta.yaml` - Chapter metadata (music, visuals, etc.)
- `XX_ChapterName_Side.altsideshelper.meta.yaml` - AltSidesHelper integration

### Side Prefixes

| Prefix | Side | Difficulty |
|--------|------|------------|
| `A-` | A-Side | Normal |
| `B-` | B-Side | Hard |
| `C-` | C-Side | Expert |
| `D-` | D-Side | Master |

## C# Helper Classes

The map system is implemented in `Source/Core/Maps/` with three main classes:

### Area Data Registration

Located in `Source/Core/Maps/AreaDataRegistration.cs`

```csharp
using DesoloZantas.Core.Maps;

// Initialize all areas during module load
AreaDataRegistration.Initialize();

// Register a custom area
AreaDataRegistration.RegisterArea(new AreaDataRegistration.AreaRegistrationInfo
{
    AreaId = 20,
    MapPath = "Maggy/DesoloZantas/DLC/2-Bonus",
    Name = "Bonus Stage 2",
    Icon = "areas/desolozantas/dlc/bonus2",
    TitleBase = "CHALLENGE ARENA",
    TitleAccentColor = "ff4500",
    TitleTextColor = "ffffff",
    HasDSide = false,
    IsDLC = true,
    Checkpoints = new[] { "start", "remix_forsaken", "expert_finale" },
    Music = "event:/music/desolozantas/dlc/bonus2"
});

// Query areas
var areaInfo = AreaDataRegistration.GetAreaInfo(20);
var allAreas = AreaDataRegistration.GetAllAreas();
var dlcAreas = AreaDataRegistration.GetDLCAreas();
bool isOurs = AreaDataRegistration.IsDesoloZantasArea(areaId);
```

### Custom Chapter Metadata

Located in `Source/Core/Maps/CustomChapterMetadataProcessor.cs`

```csharp
using DesoloZantas.Core.Maps;

// The processor automatically applies metadata during map loading
// Access chapter metadata:
var meta = CustomChapterMetadataProcessor.GetChapterMeta(areaKey);
if (meta != null)
{
    bool isDesoloZantas = meta.IsDesoloZantasChapter;
    bool hasDSide = meta.HasDSide;
    string bossType = meta.BossType;
    string[] requirements = meta.UnlockRequirements;
}

// Check D-Side availability
bool hasDSide = CustomChapterMetadataProcessor.HasDSide(areaId);
int sideCount = CustomChapterMetadataProcessor.GetSideCount(areaId);
```

### Checkpoint Registration

Located in `Source/Core/Maps/CheckpointRegistration.cs`

```csharp
using DesoloZantas.Core.Maps;

// Register checkpoints for a chapter (A-Side)
CheckpointRegistration.RegisterCheckpoints("Maggy/DesoloZantas/1-ForsakenPath", new[] {
    "start",
    "ruins_entrance",
    "forest_clearing",
    "temple_gate",
    "boss_arena"
});

// Register checkpoints for specific side (B-Side)
CheckpointRegistration.RegisterCheckpoints(
    "Maggy/DesoloZantas/1-ForsakenPath",
    AreaMode.BSide,
    new[] { "start", "hard_section", "boss" }
);

// Query checkpoints
var checkpoints = CheckpointRegistration.GetCheckpoints(mapPath);
var start = CheckpointRegistration.GetStartCheckpoint(mapPath);
var next = CheckpointRegistration.GetNextCheckpoint(mapPath, "ruins_entrance");
int count = CheckpointRegistration.GetCheckpointCount(mapPath);

// Register all default checkpoints
CheckpointRegistration.RegisterDefaultCheckpoints();
```

## Loenn Integration

Custom Loenn plugins for map editing are in:

- `Loenn/entities/` - Custom entity definitions
- `Loenn/triggers/` - Custom trigger definitions
- `Loenn/effects/` - Custom styleground effects
- `Loenn/metadata/` - Chapter metadata helpers

## Building Maps

1. Create `.bin` files using Loenn
2. Place in appropriate chapter folder
3. Register in `Maps/Maggy/DesoloZantas/meta.yaml`
4. Build mod: `dotnet build`

## Testing

```powershell
# Test specific chapter (A-Side)
celeste.exe --debug --map "Maggy/DesoloZantas/Chapters/01_City_A"

# Test B-Side
celeste.exe --debug --map "Maggy/DesoloZantas/Chapters/01_City_B"

# Test Prologue
celeste.exe --debug --map "Maggy/DesoloZantas/Chapters/00_Prologue"

# Test with debug mode
celeste.exe --debug --console
```

## Additional Folders

| Folder | Purpose |
|--------|---------|
| `Chapters/` | Main campaign maps |
| `Gyms/` | Practice/training maps |
| `Lobbies/` | Hub/lobby maps |
| `Submaps/` | Sub-area maps |
| `WIP/` | Work-in-progress maps |
