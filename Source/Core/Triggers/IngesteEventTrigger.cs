using DesoloZantas.Core.Core.Cutscenes;
using DesoloZantas.Core.Core.Entities;
using DesoloZantas.Core.Core.NPCs;
using DesoloZantas.Core.Cutscenes;
using DesoloZantastras.Core.Cutscenes;
// Ensure global namespace is used to avoid conflicts

namespace DesoloZantas.Core.Core.Triggers;

[CustomEntity("Ingeste/EventTrigger")]
[Tracked]
public class IngesteEventTrigger : Trigger
{
    private readonly string eventName;
    private bool hasTriggered = false;

    public IngesteEventTrigger(EntityData data, Vector2 offset) : base(data, offset)
    {
        eventName = data.Attr("event", "");
        Logger.Log(LogLevel.Info, "IngesteEventTrigger", $"Created trigger for event: {eventName}");
    }

    internal static void Load()
    {
        On.Celeste.Level.LoadLevel += Level_LoadLevel;
        Logger.Log(LogLevel.Info, "IngesteEventTrigger", "Hooks registered");
    }

    internal static void Unload()
    {
        On.Celeste.Level.LoadLevel -= Level_LoadLevel;
        Logger.Log(LogLevel.Info, "IngesteEventTrigger", "Hooks unregistered");
    }

    private static void Level_LoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, global::Celeste.Player.IntroTypes playerIntro, bool isFromLoader)
    {
        orig(self, playerIntro, isFromLoader);
        Logger.Log(LogLevel.Debug, "IngesteEventTrigger", "Level loaded, triggers should be active");
    }

    private void TriggerOnce(Level level, string flag, Func<CutsceneEntity> cutsceneFactory)
    {
        Logger.Log(LogLevel.Info, "IngesteEventTrigger", $"TriggerOnce called for flag: {flag}");
        
        if (level.Session.GetFlag(flag))
        {
            Logger.Log(LogLevel.Info, "IngesteEventTrigger", $"Flag {flag} already set, skipping trigger");
            return;
        }

        level.Session.SetFlag(flag);
        
        try
        {
            var cs = cutsceneFactory();
            if (cs != null)
            {
                Logger.Log(LogLevel.Info, "IngesteEventTrigger", $"Adding cutscene {cs.GetType().Name} to scene");
                Scene.Add(cs);
            }
            else
            {
                Logger.Log(LogLevel.Warn, "IngesteEventTrigger", "Cutscene factory returned null");
            }
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, "IngesteEventTrigger", $"Error creating cutscene: {ex}");
        }
        
        RemoveSelf();
    }

    public override void OnEnter(global::Celeste.Player player)
    {
        base.OnEnter(player);
        
        if (hasTriggered)
        {
            Logger.Log(LogLevel.Debug, "IngesteEventTrigger", "Trigger already fired, ignoring");
            return;
        }

        Logger.Log(LogLevel.Info, "IngesteEventTrigger", $"Player entered trigger with event: {eventName}");
        
        if (Scene is not Level level)
        {
            Logger.Log(LogLevel.Warn, "IngesteEventTrigger", "Scene is not a Level, cannot trigger cutscene");
            return;
        }

        hasTriggered = true;

        // Handle all event types
        ProcessEvent(level, player, eventName);
    }

    private void ProcessEvent(Level level, global::Celeste.Player player, string eventName)
    {
        Logger.Log(LogLevel.Info, "IngesteEventTrigger", $"Processing event: {eventName}");

        switch (eventName)
        {
            // Legacy event names (keep for backwards compatibility)
            case "mod_city_end_1":
            case "ch1_mod_city_end":
                TriggerOnce(level, "ch1_mod_end_trigger", () => new Cs01ModEnding(player));
                break;
                
            case "mod_city_end_2":
            case "ch3_mod_city_end":
                TriggerOnce(level, "ch3_2nd_mod_end_trigger", () => new Cs03ModEnding(player));
                break;
                
            case "chara_trap":
            case "ch2_chara_trap":
                TriggerOnce(level, "chara_trap_trigger", () => new Cs02CharaTrap(player));
                break;
                
            case "call_kirby":
            case "ch2_call_kirby":
                TriggerOnce(level, "call_kirby_trigger", () => new Cs02CallKirby(player));
                break;
                
            case "found_maddy":
            case "ch7_found_maddy":
                TriggerOnce(level, "found_maddy_trigger", () => new Cs07SaveMaddy(player));
                break;
                
            case "ch8_starjump":
                // Find NPC for this cutscene
                var starJumpNPC = Scene.Entities.FindFirst<NPC>();
                TriggerOnce(level, "ch8_starjump_trigger", () => new CS08_StarJumpEnd(starJumpNPC, player));
                break;
            
            case "ch8_chara_boss_end":
                // Find Chara NPC for this cutscene
                var charaCrying = Scene.Entities.FindFirst<Npc08CharaCrying>();
                TriggerOnce(level, "ch8_chara_boss_end_trigger", () => new Cs08CharaBossEnd(player, charaCrying));
                break;
            
            case "ch8_endingmod":
                // Find NPCs for this cutscene
                var theoNMaddy = Scene.Entities.FindFirst<Npc08MaddyAndTheoEnding>();
                var magolor = Scene.Entities.FindFirst<Npc08MaggyEnding>();
                TriggerOnce(level, "ch8_endingmod_trigger", () => new Cs08EndingMod(player, theoNMaddy, magolor));
                break;
                
            case "ch7_intro":
                TriggerOnce(level, "ch7_intro_trigger", () => new Cs07Intro(player));
                break;

            case "ch19_loop":
                // Find CharaDummy entity required for this cutscene
                var charaDummy = Scene.Entities.FindFirst<CharaDummy>();
                var magolorLoop = Scene.Entities.FindFirst<Npc19MaggyLoop>();
                TriggerOnce(level, "ch19_loop_trigger", () => new Cs19TrapinLoop(player, charaDummy));
                break;
                
            case "ch19_chara_help":
                TriggerOnce(level, "ch19_chara_help_trigger", () => new CS19_CharaHelps(player));
                break;
                
            case "ch20_saved":
                TriggerOnce(level, "ch20_saved_trigger", () => new CS20_Saved(player));
                break;
                
            case "ch20_ending":
                TriggerOnce(level, "ch20_true_end_trigger", () => new CS20_Ending(player));
                break;
                
            case "ch16_Epilouge":
                TriggerOnce(level, "ch16_Epilouge_trigger", () => new Cs16WelcomeHome(player, targetX: 0f));
                break;
                
            case "ch19_another_dimension_intro":
                TriggerOnce(level, "ch19_another_dimension_intro_trigger", () => new Cs19AnotherDimensionIntro(player));
                break;
                
            case "ch19_big_final_room":
                TriggerOnce(level, "ch19_big_final_room_trigger", () => new Cs19BigFinalRoom(player, first: true));
                break;
                
            case "ch19_hub_second_intro":
            case "hub_second_intro":
                TriggerOnce(level, "hub_introsecondtime", () => new CS19_HubSecondIntro(Scene, player));
                break;
                
            case "ch9_fake_saved":
                TriggerOnce(level, "ch9_fake_saved_trigger", () => new CS09_FakeSavePoint(player, CS09_FakeSavePoint.GetCurrentStage(level)));
                break;
                
            case "ch9_credits":
                TriggerOnce(level, "ch9_credits_trigger", () => new CS09_Credits(player));
                break;
                
            case "ch9_message_end":
                TriggerOnce(level, "ch9_message_end_trigger", () => new CS09_MessageEnd(player));
                break;
                
            case "ch10_intro":
                // Special case - scene change instead of cutscene
                if (!level.Session.GetFlag("ch10_intro_trigger"))
                {
                    level.Session.SetFlag("ch10_intro_trigger");
                    Logger.Log(LogLevel.Info, "IngesteEventTrigger", "Changing scene to Cs10IntroVignetteAlt");
                    Engine.Scene = new Cs10IntroVignetteAlt(level.Session);
                    RemoveSelf();
                }
                break;
                
            case "ch19_goto_the_future":
            case "ch19_goto_the_past":
                level.OnEndOfFrame += () =>
                {
                    new Vector2(level.LevelOffset.X + (float)level.Bounds.Width - player.X, player.Y - level.LevelOffset.Y);
                    Vector2 levelOffset = level.LevelOffset;
                    Vector2 vector = player.Position - level.LevelOffset;
                    Vector2 vector2 = level.Camera.Position - level.LevelOffset;
                    Facings facing = player.Facing;
                    level.Remove(player);
                    level.UnloadLevel();
                    level.Session.Dreaming = true;
                    level.Session.Level = ((eventName == "ch19_goto_the_future") ? "intro-01-future-maggy" : "intro-00-past-maggy");
                    level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2(level.Bounds.Left, level.Bounds.Top));
                    level.Session.FirstLevel = false;
                    level.LoadLevel(global::Celeste.Player.IntroTypes.Transition);
                    level.Camera.Position = level.LevelOffset + vector2;
                    level.Session.Inventory.Dashes = 1;
                    player.Dashes = Math.Min(player.Dashes, 1);
                    level.Add(player);
                    player.Position = level.LevelOffset + vector;
                    player.Facing = facing;
                    player.Hair.MoveHairBy(level.LevelOffset - levelOffset);
                    if (level.Wipe != null)
                    {
                        level.Wipe.Cancel();
                    }
                    level.Flash(Color.White);
                    level.Shake();
                    level.Add(new LightningStrike(new Vector2(player.X + 60f, level.Bounds.Bottom - 180), 10, 200f));
                    level.Add(new LightningStrike(new Vector2(player.X + 220f, level.Bounds.Bottom - 180), 40, 200f, 0.25f));
                    Audio.Play("event:/new_content/game/10_farewell/lightning_strike");
                };
                RemoveSelf();
                break;
                
            default:
                Logger.Log(LogLevel.Warn, "IngesteEventTrigger", $"Unknown event: {eventName}");
                // Create a generic dialog cutscene as fallback
                TriggerOnce(level, $"generic_{eventName}_trigger", () => CreateGenericCutscene(player, eventName));
                break;
        }
    }

    private CutsceneEntity CreateGenericCutscene(global::Celeste.Player player, string eventName)
    {
        return new GenericCutscene(player, eventName);
    }

    // Generic cutscene for unknown events
    private class GenericCutscene : CutsceneEntity
    {
        private readonly global::Celeste.Player player;
        private readonly string eventName;

        public GenericCutscene(global::Celeste.Player player, string eventName)
        {
            this.player = player;
            this.eventName = eventName;
        }

        public override void OnBegin(Level level)
        {
            Add(new Coroutine(cutscene(level)));
        }

        private IEnumerator cutscene(Level level)
        {
            player.StateMachine.State = 11; // Dummy state
            yield return 0.5f;
            
            // Try to find a dialog key for this event
            string dialogKey = $"{eventName.ToUpper()}";
            yield return Textbox.Say(dialogKey);
            
            yield return 0.5f;
            EndCutscene(level);
        }

        public override void OnEnd(Level level)
        {
            if (player != null)
                player.StateMachine.State = 0; // Normal state
        }
    }
}
