namespace DesoloZantas.Core.Core;

public class BreathingRumbler : Entity
{
    private const float max_rumble = 0.25f;
    private float currentRumble;
    public float Strength = 0.2f;

    public BreathingRumbler(Vector2 playerPosition)
    {
        currentRumble = Strength;
    }

    public override void Update()
    {
        base.Update();
        currentRumble = Calc.Approach(currentRumble, Strength, 2f * Engine.DeltaTime);
        if (currentRumble <= 0.0)
            return;
        Input.RumbleSpecific(currentRumble * 0.25f, 0.05f);
    }

    public static explicit operator Monocle.Component(BreathingRumbler v)
    {
        throw new NotImplementedException();
    }
}



