namespace DesoloZantas.Core.Core.Triggers
{
    public class FloweyIntroScene
    {
        public FloweyIntroScene(global::Celeste.Player player)
        {
            Player = player;
            
            // Initialize Lua cutscene manager if not already done
            try
            {
                if (!LuaCutsceneManager.IsInitialized)
                {
                    LuaCutsceneManager.Initialize();
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"FloweyIntroScene: Failed to initialize LuaCutsceneManager: {ex.Message}");
            }
        }

        public global::Celeste.Player Player { get; }
        
        // Enhanced properties for Lua integration
        public string PersonalityState { get; private set; } = "neutral";
        public string SelectedMusic { get; private set; } = "event:/Ingeste/music/lvl10/flowey";
        public Dictionary<string, float> AnimationTiming { get; private set; } = new Dictionary<string, float>();
        public Dictionary<string, float> EffectIntensity { get; private set; } = new Dictionary<string, float>();
        
        /// <summary>
        /// Gets the appropriate dialogue key using Lua-enhanced behavior trees for dynamic selection
        /// </summary>
        public string GetDialogueKey()
        {
            try
            {
                // Try to use Lua system for enhanced dialogue selection
                if (LuaCutsceneManager.IsInitialized)
                {
                    var playerData = PreparePlayerData();
                    if (playerData != null)
                    {
                        var luaResult = CallLuaDialogueSelection(playerData);
                        if (luaResult.HasValue)
                        {
                            PersonalityState = luaResult.Value.personality ?? "neutral";
                            SelectedMusic = GetLuaMusic(PersonalityState);
                            AnimationTiming = GetLuaAnimationTiming(PersonalityState);
                            EffectIntensity = GetLuaEffectIntensity(PersonalityState);
                            return luaResult.Value.dialogue ?? GetSafeDialogueKey();
                        }
                    }
                }
                else
                {
                    IngesteLogger.Warn("FloweyIntroScene: LuaCutsceneManager not initialized, using fallback dialogue");
                }

                // Fallback to basic logic
                return GetSafeDialogueKey();
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Flowey: Error determining dialogue: {ex.Message}");
                return GetSafeDialogueKey();
            }
        }

        private Dictionary<string, object> PreparePlayerData()
        {
            try
            {
                var saveData = SaveData.Instance;
                return new Dictionary<string, object>
                {
                    ["deaths"] = saveData?.TotalDeaths ?? 0,
                    ["strawberries"] = saveData?.TotalStrawberries ?? 0,
                    ["playTime"] = saveData?.Time ?? 0,
                    ["isUsingAssist"] = IsUsingAssistMode(),
                    ["hasBeenToHigherChapters"] = HasBeenToHigherChapters()
                };
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"FloweyIntroScene: Error preparing player data: {ex.Message}");
                return new Dictionary<string, object>
                {
                    ["deaths"] = 0,
                    ["strawberries"] = 0,
                    ["playTime"] = 0,
                    ["isUsingAssist"] = false,
                    ["hasBeenToHigherChapters"] = false
                };
            }
        }

        private (string dialogue, string personality)? CallLuaDialogueSelection(Dictionary<string, object> playerData)
        {
            try
            {
                // Null check on LuaCutsceneManager before using
                if (!LuaCutsceneManager.IsInitialized)
                {
                    IngesteLogger.Warn("FloweyIntroScene: Lua system not initialized, returning null");
                    return null;
                }

                var luaScript = $@"
                    local FloweyLua = require('libraries.flowey_cutscene_library')
                    local playerData = {{
                        deaths = {playerData["deaths"]},
                        strawberries = {playerData["strawberries"]},
                        playTime = {playerData["playTime"]},
                        isUsingAssist = {playerData["isUsingAssist"].ToString().ToLower()},
                        hasBeenToHigherChapters = {playerData["hasBeenToHigherChapters"].ToString().ToLower()}
                    }}
                    local dialogue, personality = FloweyLua.selectDialogue(playerData)
                    return dialogue .. ',' .. personality
                ";

                var resultArray = LuaCutsceneManager.CallLuaFunction("dostring", luaScript);
                var result = resultArray?.Length > 0 ? resultArray[0]?.ToString() : null;
                if (!string.IsNullOrEmpty(result))
                {
                    var parts = result.Split(new[] { ',' }, StringSplitOptions.None);
                    if (parts.Length >= 2)
                    {
                        return (parts[0].Trim(), parts[1].Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Flowey: Lua dialogue error: {ex.Message}");
            }
            return null;
        }

        private string GetLuaMusic(string personalityState)
        {
            try
            {
                if (!LuaCutsceneManager.IsInitialized || string.IsNullOrEmpty(personalityState))
                {
                    return "event:/Ingeste/music/lvl10/flowey";
                }
                
                var resultArray = LuaCutsceneManager.CallLuaFunction($"require('libraries.flowey_cutscene_library').selectMusic('{personalityState}', nil)");
                var result = resultArray?.Length > 0 ? resultArray[0]?.ToString() : null;
                return result ?? "event:/Ingeste/music/lvl10/flowey";
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"FloweyIntroScene: Error getting Lua music for personality '{personalityState}': {ex.Message}");
                return "event:/Ingeste/music/lvl10/flowey";
            }
        }

        private Dictionary<string, float> GetLuaAnimationTiming(string personalityState)
        {
            // Return default timing - this can be enhanced later
            return new Dictionary<string, float>
            {
                ["emergeSpeed"] = 1.0f,
                ["catchSpeed"] = 1.0f,
                ["healSpeed"] = 1.0f
            };
        }

        private Dictionary<string, float> GetLuaEffectIntensity(string personalityState)
        {
            // Return default intensity - this can be enhanced later
            return new Dictionary<string, float>
            {
                ["screenShake"] = 1.0f,
                ["particles"] = 1.0f,
                ["vinesOpacity"] = 0.8f
            };
        }

        private bool IsUsingAssistMode()
        {
            var saveData = SaveData.Instance;
            if (saveData?.Assists == null) return false;
            return saveData.Assists.DashAssist || saveData.Assists.InfiniteStamina || saveData.Assists.Invincible;
        }

        private bool HasBeenToHigherChapters()
        {
            try
            {
                var saveData = SaveData.Instance;
                return saveData?.TotalDeaths > 200 || saveData?.TotalStrawberries > 50;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"FloweyIntroScene: Error checking chapter progress: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Safe fallback method that always returns a valid dialogue key
        /// </summary>
        private static string GetSafeDialogueKey()
        {
            return "CH10_FLOWEY_INTRO";
        }
    }

    public partial class Cs10FloweyIntro : CutsceneEntity
    {
        private const float music_slowdown_factor = 0.5f;
        private const float duration_flowey_emerges = 1.0f;
        private const float duration_omnius_zoom_in = 0.5f;
        private const float duration_flowey_catches_madeline = 0.5f;
        private const float duration_circle_madeline_with_seed_and_heal = 1.0f;
        private const float duration_star_bullet_hits_flowey = 0.5f;
        private const float duration_kirby_walks_in = 1.0f;
        private const float duration_theo_walks_in = 1.0f;
        private const float duration_everyone_poses = 1.0f;

        private readonly global::Celeste.Player player;
        private Level level;
        private NPC flowey;
        private NPC kirby;
        private NPC theo;

        private Monocle.SpriteBank spriteBank;

        public Cs10FloweyIntro(FloweyIntroScene floweyIntroScene) : base(false, true)
        {
            this.floweyIntroScene = floweyIntroScene;
            player = floweyIntroScene.Player;
        }

        private readonly FloweyIntroScene floweyIntroScene;

        public override void OnBegin(Level level)
        {
            this.level = level;
            
            // Ensure sprite bank is available
            try
            {
                spriteBank = GFX.SpriteBank;
                if (spriteBank == null)
                {
                    IngesteLogger.Error("FloweyIntroScene: SpriteBank is null, cutscene may not work properly");
                    EndCutscene(level);
                    return;
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"FloweyIntroScene: Error accessing SpriteBank: {ex.Message}");
                EndCutscene(level);
                return;
            }
            
            // Get dialogue key using Lua-enhanced behavior
            string dialogueKey = floweyIntroScene.GetDialogueKey();
            
            Add(new Coroutine(cutsceneRoutine(dialogueKey)));
        }

        private IEnumerator cutsceneRoutine(string dialogueKey)
        {
            player.StateMachine.State = global::Celeste.Player.StDummy;
            yield return 0.2f;
            
            // Validate dialogue key and play the Lua-selected dialogue
            if (!string.IsNullOrEmpty(dialogueKey))
            {
                yield return Textbox.Say(dialogueKey);
            }
            else
            {
                IngesteLogger.Warn("FloweyIntroScene: Dialogue key is null or empty, skipping dialogue");
            }
            
            yield return floweyEmerges();
            yield return omniusZoomIn();
            yield return floweyCatchesMadeline();
            yield return circleMadelineWithSeedAndHeal();
            yield return starBulletHitsFlowey();
            yield return kirbyWalksIn();
            yield return theoWalksIn();
            yield return everyonePoses();
            player.StateMachine.State = global::Celeste.Player.StNormal;
        }

        private IEnumerator floweyEmerges()
        {
            flowey = createNpc(Position + new Vector2(40, 0), nameof(flowey), "popin");
            level?.Add(flowey);
            
            // Play Lua-selected music based on personality
            Audio.Play(floweyIntroScene.SelectedMusic);
            IngesteLogger.Info($"Flowey: Playing {floweyIntroScene.SelectedMusic} for personality: {floweyIntroScene.PersonalityState}");
            
            yield return duration_flowey_emerges;
        }

        private IEnumerator omniusZoomIn()
        {
            if (flowey != null && level != null)
            {
                // Convert world position to screen-space position (relative to camera)
                Vector2 screenSpacePosition = flowey.Position - level.Camera.Position;
                yield return level.ZoomTo(screenSpacePosition, 2f, 1f);
            }
            else
            {
                yield return null;
            }
            yield return duration_omnius_zoom_in;
        }

        private IEnumerator floweyCatchesMadeline()
        {
            Audio.SetMusicParam("pitch", music_slowdown_factor);
            yield return moveNpc(flowey, flowey?.Position ?? Vector2.Zero, Position + new Vector2(-8, 0), duration_flowey_catches_madeline);
            addVinesEffect();
            yield return 1.0f;
            Audio.SetMusicParam("pitch", 1.0f);
        }

        private IEnumerator circleMadelineWithSeedAndHeal()
        {
            float startX = Position.X - 8;
            float startY = Position.Y - 8;
            const int rect_size = 16;
            Color yellow = Color.Yellow;
            float elapsed = 0f;
            float duration = duration_circle_madeline_with_seed_and_heal;
            while (elapsed < duration)
            {
                Draw.Rect(startX, startY, rect_size, rect_size, yellow);
                elapsed += Engine.DeltaTime;
                yield return null;
            }
        }

        private IEnumerator starBulletHitsFlowey()
        {
            if (flowey == null) yield break;
            Vector2 bulletStart = flowey.Position + new Vector2(-200, 0);
            Vector2 bulletEnd = flowey.Position;
            NPC starBullet = createNpc(bulletStart, "star_bullet", "fly");
            level?.Add(starBullet);
            yield return moveNpc(starBullet, bulletStart, bulletEnd, duration_star_bullet_hits_flowey);
            Audio.Play("event:/sfx/star_bullet_hit");
            var starBulletSprite = starBullet.Get<Sprite>();
            if (starBulletSprite != null)
                starBulletSprite.Play("knockout");
            var floweySprite = flowey.Get<Sprite>();
            if (floweySprite != null)
                floweySprite.Play("knockout");
            level?.Remove(starBullet);
            yield return 1.0f;
            level?.Remove(flowey);
        }

        private IEnumerator kirbyWalksIn()
        {
            kirby = createNpc(Position + new Vector2(-100, 0), nameof(kirby), "walk");
            level?.Add(kirby);
            yield return moveNpc(kirby, kirby.Position, Position + new Vector2(-32, 0), duration_kirby_walks_in);
        }

        private IEnumerator theoWalksIn()
        {
            theo = createNpc(Position + new Vector2(100, 0), nameof(theo), "walk");
            level?.Add(theo);
            yield return moveNpc(theo, theo.Position, Position + new Vector2(32, 0), duration_theo_walks_in);
        }

        private IEnumerator everyonePoses()
        {
            playVictoryAnimation(kirby);
            playVictoryAnimation(theo);
            playVictoryAnimation(player);
            yield return duration_everyone_poses;
        }

        public override void OnEnd(Level activeLevel)
        {
            removeNpc(activeLevel, flowey);
            removeNpc(activeLevel, kirby);
            removeNpc(activeLevel, theo);
        }

        private NPC createNpc(Vector2 position, string spriteId, string animation)
        {
            try
            {
                var npc = new NPC(position);
                if (!string.IsNullOrEmpty(spriteId) && spriteBank != null)
                {
                    var sprite = spriteBank.Create(spriteId);
                    if (sprite != null)
                    {
                        if (!string.IsNullOrEmpty(animation))
                        {
                            try
                            {
                                sprite.Play(animation);
                            }
                            catch (Exception ex)
                            {
                                IngesteLogger.Warn($"FloweyIntroScene: Animation '{animation}' not found for sprite '{spriteId}': {ex.Message}");
                            }
                        }
                        npc.Add(sprite);
                    }
                    else
                    {
                        IngesteLogger.Warn($"FloweyIntroScene: Failed to create sprite '{spriteId}'");
                    }
                }
                else
                {
                    IngesteLogger.Warn($"FloweyIntroScene: Invalid sprite ID '{spriteId}' or null spriteBank");
                }
                return npc;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"FloweyIntroScene: Error creating NPC with sprite '{spriteId}': {ex.Message}");
                return new NPC(position); // Return basic NPC without sprite
            }
        }

        private IEnumerator moveNpc(Entity entity, Vector2 start, Vector2 end, float duration)
        {
            if (entity == null) yield break;
            float inverseDuration = 1f / duration;
            float t = 0f;
            while (t < duration)
            {
                entity.Position = Vector2.Lerp(start, end, t * inverseDuration);
                t += Engine.DeltaTime;
                yield return null;
            }
            entity.Position = end;
        }

        private void addVinesEffect()
        {
            try
            {
                if (spriteBank != null && level != null)
                {
                    var vines = new Entity(Position);
                    var vineSprite = spriteBank.Create("vines");
                    if (vineSprite != null)
                    {
                        vineSprite.Play("grab");
                        vines.Add(vineSprite);
                        level.Add(vines);
                        vineSprite.Play("letgo");
                    }
                    else
                    {
                        IngesteLogger.Warn("FloweyIntroScene: Failed to create vines sprite");
                    }
                }
                else
                {
                    IngesteLogger.Warn("FloweyIntroScene: Cannot add vines effect - spriteBank or level is null");
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"FloweyIntroScene: Error adding vines effect: {ex.Message}");
            }
        }

        private void playVictoryAnimation(Entity entity)
        {
            var sprite = entity?.Get<Sprite>();
            if (sprite != null)
                sprite.Play("victory");
        }

        private void removeNpc(Level level, NPC npc)
        {
            if (npc != null && level != null)
            {
                level.Remove(npc);
            }
        }
    }
}



