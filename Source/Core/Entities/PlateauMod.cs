namespace DesoloZantas.Core.Core.Entities;
[CustomEntity("Ingeste/PlateauMod")]
public class PlateauMod : Solid
{
    private Image sprite;
    public readonly LightOcclude Occluder;

    public static void Load()
    {
        // no-op: reserved for future hooks
    }

    public PlateauMod(Vector2 playerPosition) : base(Vector2.One, 104f, 4f, true)
    {
        Collider.Left += 8f;
        Add(sprite = new Image(GFX.Game["scenery/fallplateau"]));
        Add(Occluder = new LightOcclude());
        SurfaceSoundIndex = 23;
        EnableAssistModeChecks = true;
    }
}




