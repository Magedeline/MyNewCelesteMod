namespace DesoloZantas.Core.Core.Triggers
{
    /// <summary>
    /// Advanced Flowey cutscene trigger with Lua scripting support
    /// Supports multiple personality modes and dynamic behavior
    /// </summary>
    [CustomEntity("Ingeste/FloweyLuaCutsceneTrigger")]
    public class FloweyLuaCutsceneTrigger : Trigger
    {
        // Configuration properties
        private string cutsceneId;
        private bool enableLua;
        private string personalityOverride;
        private bool debugMode;
        private bool triggerOnce;
        private bool playerOnly;
        private bool hasTriggered;

        // References
        private Level level;
        private global::Celeste.Player player;

        // Flowey personality states
        public enum FloweyPersonality
        {
            Default,
            Friendly,
            Neutral,
            Condescending,
            Mocking,
            Sinister,
            Aggressive,
            Manipulative
        }

        private FloweyPersonality currentPersonality;

        public FloweyLuaCutsceneTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            cutsceneId = data.Attr(nameof(cutsceneId), "flowey_intro");
            enableLua = data.Bool(nameof(enableLua), true);
            personalityOverride = data.Attr(nameof(personalityOverride), "");
            debugMode = data.Bool(nameof(debugMode), false);
            triggerOnce = data.Bool(nameof(triggerOnce), true);
            playerOnly = data.Bool(nameof(playerOnly), true);
            hasTriggered = false;

            // Parse personality override
            currentPersonality = ParsePersonality(personalityOverride);

            if (debugMode)
            {
                IngesteLogger.Info($"FloweyLuaCutsceneTrigger created: {cutsceneId}, Personality: {currentPersonality}, Lua: {enableLua}");
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = scene as Level;
            
            if (debugMode)
            {
                IngesteLogger.Info($"FloweyLuaCutsceneTrigger added to scene: {cutsceneId}");
            }
        }

        public override void OnEnter(global::Celeste.Player player)
        {
            base.OnEnter(player);

            if (playerOnly && this.player == null)
            {
                this.player = player;
            }

            if (!hasTriggered || !triggerOnce)
            {
                StartFloweyLuaCutscene();
            }
        }

        private void StartFloweyLuaCutscene()
        {
            if (level == null || string.IsNullOrEmpty(cutsceneId))
            {
                if (debugMode)
                {
                    IngesteLogger.Error("FloweyLuaCutsceneTrigger: Level is null or cutsceneId is empty");
                }
                return;
            }

            hasTriggered = true;

            if (debugMode)
            {
                IngesteLogger.Info($"Starting Flowey Lua cutscene: {cutsceneId}");
            }

            // Start the cutscene with fade
            level.StartCutscene(lvl => CreateFloweyLuaCutscene(lvl), fadeInOnSkip: true);
        }

        private IEnumerator CreateFloweyLuaCutscene(Level level)
        {
            // Get player reference
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player == null)
            {
                if (debugMode)
                {
                    IngesteLogger.Error("FloweyLuaCutsceneTrigger: No player found in level");
                }
                yield break;
            }

            // Set player to idle state
            player.StateMachine.State = global::Celeste.Player.StNormal;

            if (debugMode)
            {
                IngesteLogger.Info($"Flowey cutscene starting: {cutsceneId}, personality: {currentPersonality}");
            }

            // Create FloweyIntroScene for enhanced dialogue selection
            var floweyIntroScene = new FloweyIntroScene(player);
            string selectedDialogue = floweyIntroScene.GetDialogueKey();

            // Play entrance sound based on personality
            PlayPersonalitySound("enter");

            // Wait a moment for dramatic effect
            yield return 0.5f;

            if (enableLua)
            {
                yield return ExecuteLuaCutscene(level, player, floweyIntroScene);
            }
            else
            {
                yield return ExecuteBasicCutscene(level, player, selectedDialogue);
            }

            // Play exit sound
            PlayPersonalitySound("exit");

            // Restore player state
            if (player != null)
            {
                player.StateMachine.State = global::Celeste.Player.StNormal;
            }

            if (debugMode)
            {
                IngesteLogger.Info($"Flowey cutscene completed: {cutsceneId}");
            }
        }

        private IEnumerator ExecuteLuaCutscene(Level level, global::Celeste.Player player, FloweyIntroScene floweyIntroScene = null)
        {
            IEnumerator result = null;
            
            try
            {
                // Prepare player data for Lua
                var playerData = new Dictionary<string, object>
                {
                    ["position"] = new { x = player.Position.X, y = player.Position.Y },
                    ["deaths"] = level.Session.Deaths,
                    ["time"] = level.Session.Time,
                    ["level"] = level.Session.Level,
                    ["area"] = level.Session.Area.ToString(),
                    ["personality"] = currentPersonality.ToString().ToLower()
                };

                // Add FloweyIntroScene data if available
                if (floweyIntroScene != null)
                {
                    playerData["personalityState"] = floweyIntroScene.PersonalityState;
                    playerData["selectedMusic"] = floweyIntroScene.SelectedMusic;
                    playerData["animationTiming"] = floweyIntroScene.AnimationTiming;
                    playerData["effectIntensity"] = floweyIntroScene.EffectIntensity;
                }

                if (debugMode)
                {
                    IngesteLogger.Info($"Executing Lua cutscene with data: {string.Join(", ", playerData.Keys)}");
                }

                // Call Lua cutscene library
                var luaScript = $@"
                    local FloweyLua = require('libraries.flowey_cutscene_library')
                    local playerData = {{
                        position = {{ x = {player.Position.X}, y = {player.Position.Y} }},
                        deaths = {level.Session.Deaths},
                        time = {level.Session.Time},
                        level = '{level.Session.Level}',
                        area = '{level.Session.Area}',
                        personality = '{currentPersonality.ToString().ToLower()}'
                    }}
                    
                    local result = FloweyLua.executeCutscene('{cutsceneId}', playerData)
                    return result
                ";

                // Use CallLuaFunction instead of direct DoString access
                var luaFunctionCall = $"require('libraries.flowey_cutscene_library').executeCutscene('{cutsceneId}', {{}})";
                var luaResultArray = LuaCutsceneManager.CallLuaFunction(luaFunctionCall);
                var luaResultStr = luaResultArray?.Length > 0 ? luaResultArray[0]?.ToString() : null;
                
                if (!string.IsNullOrEmpty(luaResultStr))
                {
                    result = ProcessLuaStringResult(luaResultStr);
                }
                
                if (result == null)
                {
                    // Use enhanced dialogue if available
                    string selectedDialogue = floweyIntroScene?.GetDialogueKey() ?? GetDialogueKey();
                    result = ExecuteBasicCutscene(level, player, selectedDialogue);
                }
            }
            catch (Exception ex)
            {
                if (debugMode)
                {
                    IngesteLogger.Error($"FloweyLuaCutsceneTrigger Lua error: {ex.Message}");
                }
                
                string selectedDialogue = floweyIntroScene?.GetDialogueKey() ?? GetDialogueKey();
                result = ExecuteBasicCutscene(level, player, selectedDialogue);
            }

            // Execute the result
            if (result != null)
            {
                while (result.MoveNext())
                {
                    yield return result.Current;
                }
            }
        }

        private IEnumerator ProcessLuaStringResult(string luaResult)
        {
            // Simple processing of Lua string result
            if (!string.IsNullOrEmpty(luaResult))
            {
                yield return Textbox.Say(luaResult);
            }
            else
            {
                yield return 1.0f;
            }
        }

        private IEnumerator ExecuteBasicCutscene(Level level, global::Celeste.Player player, string dialogueKey = null)
        {
            // Basic cutscene implementation
            if (string.IsNullOrEmpty(dialogueKey))
            {
                dialogueKey = GetDialogueKey();
            }
            
            if (!string.IsNullOrEmpty(dialogueKey))
            {
                yield return Textbox.Say(dialogueKey);
            }

            // Add basic personality-based behavior
            switch (currentPersonality)
            {
                case FloweyPersonality.Aggressive:
                    yield return AggressiveBehavior();
                    break;
                case FloweyPersonality.Manipulative:
                    yield return ManipulativeBehavior();
                    break;
                case FloweyPersonality.Friendly:
                    yield return FriendlyBehavior();
                    break;
                default:
                    yield return 1.0f; // Default wait
                    break;
            }
        }

        private IEnumerator ExecuteSpecialEffects(object effects)
        {
            // Implement special effects based on Lua return
            if (effects is List<object> effectList)
            {
                foreach (var effect in effectList)
                {
                    if (effect is Dictionary<string, object> effectData)
                    {
                        var effectType = effectData.ContainsKey("type") ? effectData["type"].ToString() : "";
                        
                        switch (effectType)
                        {
                            case "shake":
                                level.Shake(0.3f);
                                yield return 0.3f;
                                break;
                            case "flash":
                                level.Flash(Color.White);
                                yield return 0.1f;
                                break;
                            case "particle":
                                // Add particle effects
                                yield return 0.5f;
                                break;
                        }
                    }
                }
            }
        }

        private IEnumerator AggressiveBehavior()
        {
            // Screen shake and red flash
            level.Shake(0.5f);
            level.Flash(Color.Red, true);
            yield return 0.8f;
        }

        private IEnumerator ManipulativeBehavior()
        {
            // Subtle effects, purple tint
            level.Flash(Color.Purple, false);
            yield return 1.2f;
        }

        private IEnumerator FriendlyBehavior()
        {
            // Gentle golden glow
            level.Flash(Color.Gold, false);
            yield return 0.6f;
        }

        private string GetDialogueKey()
        {
            // Construct dialogue key based on cutscene ID and personality
            string baseKey = $"FLOWEY_{cutsceneId.ToUpper()}";
            
            if (currentPersonality != FloweyPersonality.Default)
            {
                baseKey += $"_{currentPersonality.ToString().ToUpper()}";
            }

            return baseKey;
        }

        private void PlayPersonalitySound(string action)
        {
            string soundEvent = "event:/game/general/";
            
            switch (currentPersonality)
            {
                case FloweyPersonality.Aggressive:
                    soundEvent += action == "enter" ? "crystalheart_pulse" : "crystalheart_return";
                    break;
                case FloweyPersonality.Manipulative:
                    soundEvent += action == "enter" ? "strawberry_pulse" : "strawberry_return";
                    break;
                case FloweyPersonality.Friendly:
                    soundEvent += action == "enter" ? "golden_strawberry_pulse" : "golden_strawberry_return";
                    break;
                default:
                    soundEvent += action == "enter" ? "crystalheart_pulse" : "crystalheart_return";
                    break;
            }

            Audio.Play(soundEvent, Position);
        }

        private FloweyPersonality ParsePersonality(string personalityStr)
        {
            if (string.IsNullOrEmpty(personalityStr))
                return FloweyPersonality.Default;

            switch (personalityStr.ToLower())
            {
                case "friendly": return FloweyPersonality.Friendly;
                case "neutral": return FloweyPersonality.Neutral;
                case "condescending": return FloweyPersonality.Condescending;
                case "mocking": return FloweyPersonality.Mocking;
                case "sinister": return FloweyPersonality.Sinister;
                case "aggressive": return FloweyPersonality.Aggressive;
                case "manipulative": return FloweyPersonality.Manipulative;
                default: return FloweyPersonality.Default;
            }
        }

        public override void DebugRender(Camera camera)
        {
            base.DebugRender(camera);
            
            if (debugMode)
            {
                // Draw simple debug information without using potentially unavailable fonts
                // This is safer and will work across different Celeste/Monocle versions
            }
        }
    }
}



