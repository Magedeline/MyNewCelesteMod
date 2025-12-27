using System.Runtime.CompilerServices;

namespace DesoloZantas.Core.Core.NPCs;

public class NPC06_Magolor : NPC
{
    private bool started;

    [MethodImpl(MethodImplOptions.NoInlining)]
    public NPC06_Magolor(Vector2 position)
        : base(position)
    {
        Add(Sprite = GFX.SpriteBank.Create("magolor"));
        IdleAnim = "idle";
        MoveAnim = "walk";
        Visible = false;
        Maxspeed = 48f;
        SetupTheoSpriteSounds();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    public override void Update()
    {
        base.Update();
        if (!started)
        {
            Gondola gondola = base.Scene.Entities.FindFirst<Gondola>();
            CelestePlayer entity = base.Scene.Tracker.GetEntity<CelestePlayer>();
            if (gondola != null && entity != null && entity.X > gondola.Left - 16f)
            {
                started = true;
                var magolorGondolaCutscene = new MagolorGondolaCutscene(this, gondola, entity);
                base.Scene.Add(magolorGondolaCutscene);
            }
        }
    }
}




