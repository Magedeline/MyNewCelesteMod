namespace DesoloZantas.Core.Core.Cutscenes
{
    [CustomEntity("DesoloZantas/MultiCharacterCutscene")]
    public class MultiCharacterCutscene : CutsceneEntity
    {
        private global::Celeste.Player player;
        private Dictionary<string, NPC> characters;
        private string cutsceneId;
        private bool autoStart;

        public MultiCharacterCutscene(EntityData data, Vector2 offset) : base()
        {
            Position = data.Position + offset;
            cutsceneId = data.Attr("cutsceneId", "CH0_MODINTRO");
            autoStart = data.Bool("autoStart", false);
            characters = new Dictionary<string, NPC>();
        }

        // Constructor for programmatic cutscene creation
        public MultiCharacterCutscene(global::Celeste.Player player, string cutsceneId) : base()
        {
            this.player = player;
            this.cutsceneId = cutsceneId;
            this.autoStart = false;
            this.characters = new Dictionary<string, NPC>();
        }

        public override void OnBegin(Level level)
        {
            player = Scene.Tracker.GetEntity<global::Celeste.Player>();
            
            // Find all NPCs in the scene
            foreach (Entity entity in Scene.Entities)
            {
                if (entity is NPC npc)
                {
                    string npcName = GetNPCName(npc);
                    if (!string.IsNullOrEmpty(npcName))
                    {
                        characters[npcName] = npc;
                    }
                }
            }
            
            Add(new Coroutine(CutsceneSequence()));
        }

        public override void OnEnd(Level level)
        {
            // Set completion flag
            level.Session.SetFlag($"cutscene_{cutsceneId}_complete");
        }

        private IEnumerator CutsceneSequence()
        {
            if (player != null)
            {
                player.StateMachine.State = global::Celeste.Player.StDummy;
            }

            Level level = Scene as Level;

            switch (cutsceneId)
            {
                case "CH0_MODINTRO":
                    yield return ModIntroSequence();
                    break;
                    
                case "CH0_END":
                    yield return Textbox.Say("CH0_END");
                    break;
                    
                case "CH1_ENDMADELINE":
                    yield return EndMadelineSequence();
                    break;
                    
                case "CH2_INTRO":
                    yield return NightmareIntroSequence();
                    break;
                    
                case "CH2_WAKEUP":
                    yield return WakeUpSequence();
                    break;
                    
                case "MEMORIAL_DARKSIDE":
                    yield return Textbox.Say("MEMORIAL_DARKSIDE");
                    break;
                    
                case "CH2_POEM":
                    yield return Textbox.Say("CH2_POEM");
                    break;
                    
                default:
                    yield return Textbox.Say(cutsceneId);
                    break;
            }

            EndCutscene(level);
        }

        private IEnumerator ModIntroSequence()
        {
            yield return Textbox.Say("CH0_MODINTRO");
            
            // Additional intro effects could go here
            if (Scene is Level level)
            {
                level.Session.SetFlag("mod_intro_seen");
            }
        }

        private IEnumerator EndMadelineSequence()
        {
            yield return Textbox.Say("CH1_ENDMADELINE");
            
            // Show bird landing on Madeline
            if (player != null)
            {
                // Create a simple bird effect
                yield return PlayerSitDown();
                yield return 1.0f; // Bird lands
            }
        }

        private IEnumerator NightmareIntroSequence()
        {
            yield return Textbox.Say("CH2_INTRO");
            
            // Dark effect
            yield return FadeEffect(Color.Black, 2.0f);
        }

        private IEnumerator WakeUpSequence()
        {
            yield return Textbox.Say("CH2_WAKEUP");
        }

        private IEnumerator PlayerSitDown()
        {
            if (player != null)
            {
                // Simple sitting animation - in a real implementation this would be more sophisticated
                yield return 0.5f;
            }
        }

        private IEnumerator FadeEffect(Color color, float duration)
        {
            // Simple fade effect - in a real implementation this would manipulate screen effects
            yield return duration;
        }

        private string GetNPCName(NPC npc)
        {
            // Try to determine NPC type from class name or other properties
            string typeName = npc.GetType().Name.ToLower();
            
            if (typeName.Contains("theo")) return "theo";
            if (typeName.Contains("magolor") || typeName.Contains("maggy")) return "magolor";
            if (typeName.Contains("badeline")) return "badeline";
            if (typeName.Contains("chara")) return "chara";
            if (typeName.Contains("kirby")) return "kirby";
            if (typeName.Contains("ralsei")) return "ralsei";
            
            return null;
        }

        private IEnumerator MoveCharacter(string characterName, Vector2 targetPosition, float duration = 1f)
        {
            if (characters.TryGetValue(characterName, out NPC character))
            {
                Vector2 startPosition = character.Position;
                float timer = 0f;

                while (timer < duration)
                {
                    timer += Engine.DeltaTime;
                    float progress = timer / duration;
                    character.Position = Vector2.Lerp(startPosition, targetPosition, Ease.CubeInOut(progress));
                    yield return null;
                }

                character.Position = targetPosition;
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            if (autoStart)
            {
                if (scene is Level level)
                {
                    level.StartCutscene(OnEnd);
                    OnBegin(level);
                }
            }
        }
    }
}



