# DesoloZatnas Maps

## Chapter Structure

### Main Chapters (Areas 10-18)

| Area ID | Chapter Name | Folder | Description |
|---------|--------------|--------|-------------|
| 10 | Prologue II | `Maggy/DesoloZantas/0-Prologue/` | Introduction and tutorial |
| 11 | Forsaken Path | `Maggy/DesoloZantas/1-ForsakenPath/` | Forest and ruins |
| 12 | Crystal Caverns | `Maggy/DesoloZantas/2-CrystalCaverns/` | Underground crystals |
| 13 | Starlight Spire | `Maggy/DesoloZantas/3-StarlightSpire/` | Tower ascent |
| 14 | Void Sanctum | `Maggy/DesoloZantas/4-VoidSanctum/` | Dark temple |
| 15 | Dream Gardens | `Maggy/DesoloZantas/5-DreamGardens/` | Surreal landscapes |
| 16 | Mirror Depths | `Maggy/DesoloZantas/6-MirrorDepths/` | Reflection puzzles |
| 17 | Summit Redux | `Maggy/DesoloZantas/7-SummitRedux/` | Mountain climb |
| 18 | Final Ascent | `Maggy/DesoloZantas/8-FinalAscent/` | Endgame |

### DLC Chapters (Areas 19-21)

| Area ID | Chapter Name | Folder | Description |
|---------|--------------|--------|-------------|
| 19 | Bonus Stage 1 | `Maggy/DesoloZantas/DLC/1-Bonus/` | Extra content |
| 20 | Bonus Stage 2 | `Maggy/DesoloZantas/DLC/2-Bonus/` | Challenge maps |
| 21 | Secret Finale | `Maggy/DesoloZantas/DLC/3-Secret/` | Hidden ending |

## Naming Conventions

### Map Files

```
[ChapterNumber]-[ChapterName]/[SidePrefix]-[RoomName].bin

Examples:
0-Prologue/A-Intro.bin        # Prologue A-Side intro room
1-ForsakenPath/B-01.bin       # Chapter 1 B-Side room 1
2-CrystalCaverns/C-Boss.bin   # Chapter 2 C-Side boss room
3-StarlightSpire/D-Final.bin  # Chapter 3 D-Side final room
```

### Side Prefixes

| Prefix | Side | Difficulty |
|--------|------|------------|
| `A-` | A-Side | Normal |
| `B-` | B-Side | Hard |
| `C-` | C-Side | Expert |
| `D-` | D-Side | Master |

## C# Helper Classes

### Area Data Registration

```csharp
using Celeste.Mod.DesoloZatnas.Core;

// Register custom area data
MapAreaExpansion.RegisterArea(10, "DesoloZantas/0-Prologue", new AreaData {
    Name = "Prologue II",
    Icon = "areas/desolozantas/prologue",
    TitleBase = "PROLOGUE",
    // ... more properties
});
```

### Custom Chapter Metadata

```csharp
// In ConcreteMapDataProcessor.cs
public override void Process(AreaKey area, MapData data)
{
    // Add custom metadata to chapters
    if (area.ID >= 10 && area.ID <= 21)
    {
        data.Meta.DesoloZantasChapter = true;
        data.Meta.HasDSide = area.ID >= 10 && area.ID <= 18;
    }
}
```

### Checkpoint Registration

```csharp
// Register checkpoints for chapter
CheckpointData.Register("DesoloZantas/1-ForsakenPath", new[] {
    "start",
    "ruins_entrance",
    "forest_clearing",
    "temple_gate",
    "boss_arena"
});
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
# Test specific chapter
celeste.exe --debug --map "Maggy/DesoloZantas/1-ForsakenPath/A-01"

# Test with debug mode
celeste.exe --debug --console
```
