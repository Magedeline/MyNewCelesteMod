using FMOD.Studio;

namespace DesoloZantas.Core.Core.VanillaCore;

/// <summary>
/// Event trigger ported from vanilla Celeste.
/// Maps vanilla events to DesoloZantas mod cutscenes and events.
/// </summary>
[CustomEntity("Ingeste/EventTrigger")]
public class EventTrigger : Trigger
{
    public string Event;
    public bool OnSpawnHack;
    private bool triggered;
    private EventInstance snapshot;

    public float Time { get; private set; }

    public EventTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        Event = data.Attr("event");
        OnSpawnHack = data.Bool("onSpawn");
    }

    public override void Awake(Scene scene)
    {
        base.Awake(scene);
        if (OnSpawnHack)
        {
            var player = CollideFirst<global::Celeste.Player>();
            if (player != null)
                OnEnter(player);
        }

        // Check for character-specific removal conditions
        CheckRemovalConditions();
    }

    private void CheckRemovalConditions()
    {
        Level level = Scene as Level;
        if (level == null) return;

        // Map vanilla character checks to mod characters
        switch (Event)
        {
            case "ch9_badeline_helps":
                // In DesoloZantas, this might be "chara_helps" instead
                var player = Scene.Tracker.GetEntity<global::Celeste.Player>();
                if (player != null && player.Left > Right)
                    RemoveSelf();
                break;
        }
    }

    public override void OnEnter(global::Celeste.Player player)
    {
        if (triggered)
            return;
            
        if (!ShouldActivate(player))
            return;

        triggered = true;
        Level level = Scene as Level;

        // Handle DesoloZantas custom events
        if (HandleCustomEvent(level, player))
            return;

        // Handle mapped vanilla events
        HandleMappedVanillaEvent(level, player);
    }

    private bool HandleCustomEvent(Level level, global::Celeste.Player player)
    {
        switch (Event)
        {
            // ===== KIRBY EVENTS =====
            case "kirby_intro":
                if (!level.Session.GetFlag("kirby_intro"))
                    Scene.Add(new KirbyIntroCutscene(player));
                return true;

            case "kirby_meets_madeline":
                if (!level.Session.GetFlag("kirby_meets_madeline"))
                    Scene.Add(new KirbyMeetsMadelineCutscene(player));
                return true;

            case "kirby_mode_unlock":
                level.Session.SetFlag("kirby_mode_unlocked", true);
                Audio.Play("event:/desolozantas/ui/kirby_unlock", Position);
                return true;

            // ===== MAGOLOR EVENTS =====
            case "magolor_intro":
                if (!level.Session.GetFlag("magolor_intro"))
                    Scene.Add(new MagolorIntroCutscene(player));
                return true;

            case "magolor_escape":
                Scene.Add(new MagolorEscapeCutscene(player));
                return true;

            // ===== UNDERTALE/DELTARUNE EVENTS =====
            case "toriel_intro":
                if (!level.Session.GetFlag("toriel_intro"))
                    Scene.Add(new TorielIntroCutscene(player));
                return true;

            case "chara_boss_intro":
                if (!level.Session.GetFlag("chara_boss_intro"))
                    Scene.Add(new CharaBossIntroCutscene(player));
                return true;

            case "ralsei_heals":
                if (!level.Session.GetFlag("ralsei_heals"))
                    Scene.Add(new RalseiHealsCutscene(player));
                return true;

            // ===== BOSS EVENTS =====
            case "flowey_intro":
                if (!level.Session.GetFlag("flowey_intro"))
                    Scene.Add(new FloweyIntroCutscene(player));
                return true;

            case "asriel_god_intro":
                if (!level.Session.GetFlag("asriel_god_intro"))
                    Scene.Add(new AsrielGodIntroCutscene(player));
                return true;

            // ===== AREA SPECIFIC =====
            case "ingeste_chapter_end":
                Scene.Add(new ChapterEndCutscene(player, new Vector2(Center.X, Bottom)));
                return true;

            case "delta_berry_unlock":
                level.Session.SetFlag("delta_berry_unlocked", true);
                Audio.Play("event:/desolozantas/collectibles/delta_berry", Position);
                return true;

            case "pink_plat_berry_collect":
                level.Session.SetFlag("pink_plat_berry_collected", true);
                return true;
        }

        return false;
    }

    private void HandleMappedVanillaEvent(Level level, global::Celeste.Player player)
    {
        switch (Event)
        {
            // Map vanilla Theo events to Kirby/custom character events
            case "ch5_found_theo":
                // Replaced with custom crystal rescue
                if (!level.Session.GetFlag("foundCharacterInCrystal"))
                    Scene.Add(new SaveCharacterFromCrystalCutscene(player));
                break;

            case "ch5_see_theo":
                if (!level.Session.GetFlag("seeCharacterInCrystal"))
                    Scene.Add(new SeeCharacterInCrystalCutscene(player, 0));
                break;

            // Map vanilla boss events
            case "ch6_boss_intro":
                if (!level.Session.GetFlag("boss_intro"))
                {
                    var boss = level.Entities.FindFirst<global::Celeste.FinalBoss>();
                    Scene.Add(new ModBossIntroCutscene(Center.X, player, boss));
                }
                break;

            case "ch6_reflect":
                if (!level.Session.GetFlag("reflection"))
                    Scene.Add(new ModReflectionCutscene(player, Center.X - 5f));
                break;

            // Chapter endings
            case "ch7_summit":
                Scene.Add(new ModSummitCutscene(player, new Vector2(Center.X, Bottom)));
                break;

            case "end_city":
                Scene.Add(new ModCityEndingCutscene(player));
                break;

            case "end_oldsite_awake":
                Scene.Add(new ModOldsiteEndingCutscene(player));
                break;

            // Farewell events - map Badeline to Chara/custom character
            case "ch9_badeline_helps":
                if (!level.Session.GetFlag("companion_helps"))
                    Scene.Add(new CompanionHelpsCutscene(player));
                break;

            case "ch9_ending":
                Scene.Add(new ModFarewellEndingCutscene(player));
                break;

            case "ch9_farewell":
                Scene.Add(new ModFarewellCutscene(player));
                break;

            // Brightness/lighting events
            case "cancel_ch5_see_theo":
                level.Session.SetFlag("ignore_darkness_" + level.Session.Level);
                Add(new Coroutine(Brighten()));
                break;

            // Snapshot events
            case "ch9_golden_snapshot":
                snapshot = Audio.CreateSnapshot("snapshot:/game_10_golden_room_flavour");
                level.SnapColorGrade("golden");
                break;

            default:
                // Log unknown events for debugging
                IngesteLogger.Warn($"Unknown event trigger: {Event}");
                break;
        }
    }

    public override void Removed(Scene scene)
    {
        base.Removed(scene);
        Audio.ReleaseSnapshot(snapshot);
    }

    public override void SceneEnd(Scene scene)
    {
        base.SceneEnd(scene);
        Audio.ReleaseSnapshot(snapshot);
    }

    private IEnumerator Brighten()
    {
        Level level = Scene as Level;
        float darkness = AreaData.Get(level).DarknessAlpha;
        while (level.Lighting.Alpha != darkness)
        {
            level.Lighting.Alpha = Calc.Approach(level.Lighting.Alpha, darkness, Engine.DeltaTime * 4f);
            yield return null;
        }
    }
}

// Placeholder cutscene classes - these should be implemented in the Cutscenes folder
// Kirby cutscenes
public class KirbyIntroCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public KirbyIntroCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("kirby_intro", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_KIRBY_INTRO"); EndCutscene(Level); }
}

public class KirbyMeetsMadelineCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public KirbyMeetsMadelineCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("kirby_meets_madeline", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_KIRBY_MEETS_MADELINE"); EndCutscene(Level); }
}

// Magolor cutscenes
public class MagolorIntroCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public MagolorIntroCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("magolor_intro", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_MAGOLOR_INTRO"); EndCutscene(Level); }
}

public class MagolorEscapeCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public MagolorEscapeCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("magolor_escape", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_MAGOLOR_ESCAPE"); EndCutscene(Level); }
}

// Undertale/Deltarune cutscenes
public class TorielIntroCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public TorielIntroCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("toriel_intro", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_TORIEL_INTRO"); EndCutscene(Level); }
}

public class CharaBossIntroCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public CharaBossIntroCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("chara_boss_intro", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_CHARA_BOSS_INTRO"); EndCutscene(Level); }
}

public class RalseiHealsCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public RalseiHealsCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("ralsei_heals", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_RALSEI_HEALS"); EndCutscene(Level); }
}

// Boss cutscenes
public class FloweyIntroCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public FloweyIntroCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("flowey_intro", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_FLOWEY_INTRO"); EndCutscene(Level); }
}

public class AsrielGodIntroCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public AsrielGodIntroCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("asriel_god_intro", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_ASRIEL_GOD_INTRO"); EndCutscene(Level); }
}

// Generic cutscenes for mapped vanilla events
public class SaveCharacterFromCrystalCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public SaveCharacterFromCrystalCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("foundCharacterInCrystal", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_SAVE_CRYSTAL"); EndCutscene(Level); }
}

public class SeeCharacterInCrystalCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    private int index;
    public SeeCharacterInCrystalCutscene(global::Celeste.Player player, int index) : base() { this.player = player; this.index = index; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("seeCharacterInCrystal", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_SEE_CRYSTAL"); EndCutscene(Level); }
}

public class ModBossIntroCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    private float centerX;
    private Entity boss;
    public ModBossIntroCutscene(float centerX, global::Celeste.Player player, Entity boss) : base() 
    { 
        this.centerX = centerX; this.player = player; this.boss = boss; 
    }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("boss_intro", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_BOSS_INTRO"); EndCutscene(Level); }
}

public class ModReflectionCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    private float x;
    public ModReflectionCutscene(global::Celeste.Player player, float x) : base() { this.player = player; this.x = x; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("reflection", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_REFLECTION"); EndCutscene(Level); }
}

public class ModSummitCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    private Vector2 position;
    public ModSummitCutscene(global::Celeste.Player player, Vector2 position) : base(endingChapterAfter: true) 
    { 
        this.player = player; this.position = position; 
    }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_SUMMIT"); EndCutscene(Level); }
}

public class ModCityEndingCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public ModCityEndingCutscene(global::Celeste.Player player) : base(endingChapterAfter: true) { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_CITY_END"); EndCutscene(Level); }
}

public class ModOldsiteEndingCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public ModOldsiteEndingCutscene(global::Celeste.Player player) : base(endingChapterAfter: true) { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_OLDSITE_END"); EndCutscene(Level); }
}

public class CompanionHelpsCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public CompanionHelpsCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("companion_helps", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_COMPANION_HELPS"); EndCutscene(Level); }
}

public class ModFarewellEndingCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public ModFarewellEndingCutscene(global::Celeste.Player player) : base(endingChapterAfter: true) { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_FAREWELL_END"); EndCutscene(Level); }
}

public class ModFarewellCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    public ModFarewellCutscene(global::Celeste.Player player) : base() { this.player = player; }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { level.Session.SetFlag("farewell_complete", true); }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_FAREWELL"); EndCutscene(Level); }
}

public class ChapterEndCutscene : CutsceneEntity
{
    private global::Celeste.Player player;
    private Vector2 position;
    public ChapterEndCutscene(global::Celeste.Player player, Vector2 position) : base(endingChapterAfter: true) 
    { 
        this.player = player; this.position = position; 
    }
    public override void OnBegin(Level level) { Add(new Coroutine(Routine())); }
    public override void OnEnd(Level level) { }
    private IEnumerator Routine() { yield return Textbox.Say("INGESTE_CHAPTER_END"); EndCutscene(Level); }
}