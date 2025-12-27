# VanillaCore - Ported Celeste Core Classes

This folder contains ported vanilla Celeste core classes adapted for the DesoloZantas mod.

## Key Differences from Vanilla Celeste

1. **Namespace**: All classes use `DesoloZantas.Core.VanillaCore` namespace
2. **Custom NPCs**: Vanilla NPCs (Theo, Granny, Badeline, etc.) are mapped to mod-specific characters (Kirby, Toriel, Magolor, etc.)
3. **Custom Entities**: Vanilla entities reference mod-specific implementations
4. **Triggers**: All triggers are extended with mod-specific functionality

## Files

| File | Description |
|------|-------------|
| `NPC.cs` | Base NPC class with custom character flags (Kirby, Magolor, Toriel, etc.) |
| `Actor.cs` | Base physical actor class for collision and movement |
| `Trigger.cs` | Base trigger class with Kirby mode support |
| `CutsceneEntity.cs` | Base cutscene class with helper methods |
| `BaseBoss.cs` | Abstract boss template based on vanilla FinalBoss |
| `Booster.cs` | Custom booster with pink variant and Kirby mode boost |
| `Refill.cs` | Custom refill with pink variant and float ability grant |
| `Spring.cs` | Custom spring with power multiplier and Kirby mode boost |
| `CharacterMapping.cs` | Maps vanilla characters to mod characters |
| `EntityFactory.cs` | Factory for creating mod entities from vanilla types |
| `EventTrigger.cs` | Event trigger that maps vanilla events to mod cutscenes |
| `VanillaCoreHooks.cs` | Hook system for replacing vanilla entities at runtime |

## Character Mappings

| Vanilla Character | DesoloZantas Character |
|-------------------|------------------------|
| Theo              | Kirby/Magolor          |
| Granny            | Toriel                 |
| Badeline          | Chara                  |
| Oshiro            | Oshiro (kept)          |
| Bird              | Custom Bird NPCs       |

## Usage

### Import the namespace
```csharp
using DesoloZantas.Core.VanillaCore;
```

### Create a custom NPC with character mapping
```csharp
// Create an NPC that will be mapped based on level
var npc = EntityFactory.CreateNPC(position, CharacterMapping.VanillaCharacter.Theo, levelId);

// Or create a specific mod character
var kirbyNpc = EntityFactory.CreateModCharacterNPC(position, CharacterMapping.ModCharacter.Kirby);
```

### Create a custom cutscene
```csharp
public class MyCustomCutscene : CutsceneEntity
{
    public override void OnBegin(Level level)
    {
        Add(new Coroutine(MyCutsceneRoutine()));
    }

    public override void OnEnd(Level level)
    {
        level.Session.SetFlag("my_cutscene_complete", true);
    }

    private IEnumerator MyCutsceneRoutine()
    {
        yield return Textbox.Say("MY_DIALOG_KEY");
        EndCutscene(Level);
    }
}
```

### Use custom triggers
```csharp
[CustomEntity("Ingeste/MyTrigger")]
public class MyTrigger : Trigger
{
    public MyTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        RequiresKirbyMode = data.Bool("requiresKirbyMode", false);
    }

    public override void OnEnter(Player player)
    {
        if (!ShouldActivate(player)) return;
        // Your trigger logic
    }
}
```

### Add custom events
Add your event handling in `EventTrigger.cs` under `HandleCustomEvent()`:
```csharp
case "my_custom_event":
    if (!level.Session.GetFlag("my_event"))
        Scene.Add(new MyCustomCutscene(player));
    return true;
```

## Hooks Integration

The `VanillaCoreHooks` class is automatically initialized when the mod loads. It:
- Replaces vanilla NPCs with mod characters based on `CharacterMapping`
- Enhances triggers with Kirby mode support
- Sets up character-specific features per level

## Extension Methods

Use the extension methods in `VanillaCoreExtensions`:
```csharp
// Check if Kirby mode is active
if (level.IsKirbyMode())
{
    // Kirby-specific behavior
}

// Check if player has met a character
if (level.HasMetCharacter(CharacterMapping.ModCharacter.Toriel))
{
    // Character has been met
}
```
