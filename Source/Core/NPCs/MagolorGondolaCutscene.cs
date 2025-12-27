namespace DesoloZantas.Core.Core.NPCs;

public class MagolorGondolaCutscene : CutsceneEntity
{
    private readonly NPC06_Magolor magolor;
    private readonly Gondola gondola;
    private readonly CelestePlayer player;

    public MagolorGondolaCutscene(NPC06_Magolor magolor, Gondola gondola, CelestePlayer player)
    {
        this.magolor = magolor;
        this.gondola = gondola;
        this.player = player;
    }

    public override void OnBegin(Level level)
    {
        Add(new Coroutine(Cutscene()));
    }

    public override void OnEnd(Level level)
    {
        // Cleanup logic when cutscene ends
        // Reset any state changes made during the cutscene
        if (magolor != null)
        {
            // Reset Magolor's state if needed
        }
    }

    private IEnumerator Cutscene()
    {
        // Make Magolor visible and perform cutscene logic
        magolor.Visible = true;
        
        // Add your cutscene sequence here
        // For example:
        // yield return magolor.MoveTo(somePosition);
        // yield return Textbox.Say("magolor_intro");
        
        yield return null;
        EndCutscene(Level);
    }
}




