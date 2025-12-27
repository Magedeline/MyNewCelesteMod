using NLua;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Lua-enhanced cutscene manager that provides dynamic scripting capabilities for cutscenes
    /// Allows for complex behavior trees, dynamic dialogue selection, and scripted visual effects
    /// </summary>
    public static class LuaCutsceneManager
    {
        private static Lua luaState;
        private static Dictionary<string, LuaFunction> loadedScripts = new Dictionary<string, LuaFunction>();
        private static Dictionary<string, object> cutsceneContext = new Dictionary<string, object>();

        public static bool IsInitialized { get; private set; }

        public static void Initialize()
        {
            if (IsInitialized) return;

            IngesteLogger.Info("LuaCutsceneManager: Initializing Lua cutscene system...");

            try
            {
                luaState = new Lua();
                SetupLuaEnvironment();
                LoadCutsceneScripts();
                IsInitialized = true;
                IngesteLogger.Info("LuaCutsceneManager: Successfully initialized!");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"LuaCutsceneManager: Failed to initialize: {ex.Message}");
                IsInitialized = false;
            }
        }

        public static void Cleanup()
        {
            if (!IsInitialized) return;

            IngesteLogger.Info("LuaCutsceneManager: Cleaning up Lua cutscene system...");

            try
            {
                foreach (var script in loadedScripts.Values)
                {
                    script?.Dispose();
                }
                loadedScripts.Clear();
                cutsceneContext.Clear();
                luaState?.Dispose();
                luaState = null;
                IsInitialized = false;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"LuaCutsceneManager: Error during cleanup: {ex.Message}");
            }
        }

        private static void SetupLuaEnvironment()
        {
            // Register C# functions for Lua access
            luaState["log"] = (Action<string>)((message) => IngesteLogger.Info($"Lua: {message}"));
            luaState["logError"] = (Action<string>)((message) => IngesteLogger.Error($"Lua: {message}"));
            luaState["logWarn"] = (Action<string>)((message) => IngesteLogger.Warn($"Lua: {message}"));

            // Player state functions
            luaState["getPlayerDeaths"] = (Func<int>)(() => SaveData.Instance?.TotalDeaths ?? 0);
            luaState["getPlayerStrawberries"] = (Func<int>)(() => SaveData.Instance?.TotalStrawberries ?? 0);
            luaState["getPlayTime"] = (Func<long>)(() => SaveData.Instance?.Time ?? 0);
            luaState["isUsingAssist"] = (Func<bool>)IsUsingAssistMode;
            luaState["hasCompletedChapter"] = (Func<int, bool>)HasCompletedChapter;

            // Audio functions
            luaState["playSound"] = (Action<string>)((soundPath) => Audio.Play(soundPath));
            luaState["playMusic"] = (Action<string>)((musicPath) => Audio.SetMusic(musicPath));
            luaState["setMusicParam"] = (Action<string, float>)((param, value) => Audio.SetMusicParam(param, value));

            // Random functions
            luaState["randomFloat"] = (Func<float>)(() => Calc.Random.NextFloat());
            luaState["randomInt"] = (Func<int, int>)((max) => Calc.Random.Next(max));
            luaState["randomBool"] = (Func<bool>)(() => Calc.Random.NextFloat() < 0.5f);

            // Context functions
            luaState["setContext"] = (Action<string, object>)SetContext;
            luaState["getContext"] = (Func<string, object>)GetContext;
            luaState["hasContext"] = (Func<string, bool>)HasContext;

            // Cutscene control functions
            luaState["waitSeconds"] = (Func<float, IEnumerator>)WaitSeconds;
            luaState["fadeToBlack"] = (Func<float, IEnumerator>)FadeToBlack;
            luaState["fadeFromBlack"] = (Func<float, IEnumerator>)FadeFromBlack;

            // Load basic Lua libraries
            luaState.DoString(@"
                math.randomseed(os.time())
                
                -- Helper functions for cutscenes
                function lerp(a, b, t)
                    return a + (b - a) * t
                end
                
                function clamp(value, min, max)
                    return math.max(min, math.min(max, value))
                end
                
                function ease_in_out(t)
                    return t * t * (3.0 - 2.0 * t)
                end
                
                -- Behavior tree nodes
                BehaviorTree = {}
                BehaviorTree.__index = BehaviorTree
                
                function BehaviorTree:new()
                    local tree = {}
                    setmetatable(tree, BehaviorTree)
                    tree.root = nil
                    return tree
                end
                
                function BehaviorTree:run(context)
                    if self.root then
                        return self.root:execute(context)
                    end
                    return false
                end
            ");
        }

        private static void LoadCutsceneScripts()
        {
            try
            {
                // Load Flowey-specific scripts
                LoadScript("flowey_behavior", @"
-- Flowey Dynamic Behavior Script
local FloweyBehavior = {}

function FloweyBehavior.determineDialogue(playerData)
    log('Flowey: Determining dialogue for player with ' .. playerData.deaths .. ' deaths, ' .. playerData.strawberries .. ' strawberries')
    
    -- Behavior tree for dialogue selection
    local dialogueTree = BehaviorTree:new()
    
    -- Check for special conditions first
    if playerData.isUsingAssist then
        if playerData.hasBeenToHigherChapters then
            log('Flowey: Player using assist but has advanced - mocking tone')
            return 'CH10_FLOWEY_INTRO_ASSIST_ADVANCED'
        else
            log('Flowey: Player using assist - condescending tone')
            return 'CH10_FLOWEY_INTRO_ASSIST'
        end
    end
    
    -- Advanced player detection
    if playerData.hasBeenToHigherChapters then
        local mockingChance = 0.4
        
        -- Increase mocking chance based on death count
        if playerData.deaths > 1000 then
            mockingChance = 0.7
            log('Flowey: High death count detected - increased mocking')
        elseif playerData.deaths > 500 then
            mockingChance = 0.5
        end
        
        if randomFloat() < mockingChance then
            return FloweyBehavior.selectMockingDialogue(playerData)
        else
            return 'CH10_FLOWEY_INTRO_RETURNING'
        end
    end
    
    -- New player - check for potential
    if playerData.deaths < 50 and playerData.strawberries > 20 then
        log('Flowey: New player with skill detected')
        return 'CH10_FLOWEY_INTRO_IMPRESSED'
    elseif playerData.deaths > 100 and playerData.strawberries < 5 then
        log('Flowey: Struggling new player detected')
        return 'CH10_FLOWEY_INTRO_MOCKING_NEWBIE'
    end
    
    return 'CH10_FLOWEY_INTRO'
end

function FloweyBehavior.selectMockingDialogue(playerData)
    local mockingOptions = {}
    
    if playerData.deaths > 500 then
        table.insert(mockingOptions, 'CH10_FLOWEY_EASTER_DEATHS_HIGH')
    end
    
    if playerData.strawberries > 100 then
        table.insert(mockingOptions, 'CH10_FLOWEY_EASTER_STRAWBERRIES_OBSESSED')
    end
    
    if hasCompletedChapter(7) then
        table.insert(mockingOptions, 'CH10_FLOWEY_EASTER_CH7_SUMMIT_STRUGGLED')
    end
    
    if hasCompletedChapter(9) then
        table.insert(mockingOptions, 'CH10_FLOWEY_EASTER_CH9_FAREWELL_MASOCHIST')
    end
    
    -- Psychological manipulation options
    table.insert(mockingOptions, 'CH10_FLOWEY_EASTER_MADELINE_SELF_DOUBT')
    table.insert(mockingOptions, 'CH10_FLOWEY_EASTER_PLAYER_PERSISTENCE')
    table.insert(mockingOptions, 'CH10_FLOWEY_EASTER_WORTHLESS_ACHIEVEMENTS')
    
    if #mockingOptions > 0 then
        local selectedIndex = randomInt(#mockingOptions) + 1
        log('Flowey: Selected mocking dialogue: ' .. mockingOptions[selectedIndex])
        return mockingOptions[selectedIndex]
    end
    
    return 'CH10_FLOWEY_INTRO_RETURNING'
end

function FloweyBehavior.determineMusicVariant(playerData, dialogueKey)
    -- Dynamic music selection based on dialogue and player state
    if string.find(dialogueKey, 'ASSIST') then
        return 'event:/Ingeste/music/lvl10/flowey_condescending'
    elseif string.find(dialogueKey, 'MOCKING') or string.find(dialogueKey, 'EASTER') then
        return 'event:/Ingeste/music/lvl10/flowey_sinister'
    elseif playerData.shouldRemember then
        return 'event:/Ingeste/music/lvl10/flowey_remember'
    else
        return 'event:/Ingeste/music/lvl10/flowey'
    end
end

function FloweyBehavior.calculateAnimationTiming(playerData)
    -- Adjust animation timing based on player skill
    local baseSpeed = 1.0
    
    if playerData.deaths > 1000 then
        -- Slower, more mocking animations for high-death players
        baseSpeed = 0.7
    elseif playerData.deaths < 50 then
        -- Faster, more aggressive animations for skilled players
        baseSpeed = 1.3
    end
    
    return {
        emergeSpeed = baseSpeed,
        catchSpeed = baseSpeed * 0.8,
        healSpeed = baseSpeed * 1.2,
        attackSpeed = baseSpeed * 1.5
    }
end

return FloweyBehavior
                ");

                LoadScript("flowey_effects", @"
-- Flowey Visual Effects Script
local FloweyEffects = {}

function FloweyEffects.createParticleSystem(effectType, intensity)
    log('Creating ' .. effectType .. ' particles with intensity ' .. intensity)
    
    local particleData = {
        type = effectType,
        intensity = intensity,
        duration = 2.0,
        color = {1.0, 1.0, 0.5, 0.8}
    }
    
    if effectType == 'sinister' then
        particleData.color = {0.8, 0.1, 0.1, 0.9}
        particleData.duration = 3.0
    elseif effectType == 'healing' then
        particleData.color = {0.2, 0.8, 0.2, 0.7}
        particleData.duration = 1.5
    elseif effectType == 'mocking' then
        particleData.color = {0.9, 0.9, 0.2, 0.8}
        particleData.duration = 2.5
    end
    
    return particleData
end

function FloweyEffects.calculateScreenShake(intensity, playerDeaths)
    local baseShake = intensity
    
    -- More screen shake for players with high death counts (Flowey is more aggressive)
    if playerDeaths > 500 then
        baseShake = baseShake * 1.5
    elseif playerDeaths > 1000 then
        baseShake = baseShake * 2.0
    end
    
    return math.min(baseShake, 10.0) -- Cap at 10.0
end

return FloweyEffects
                ");

                IngesteLogger.Info("LuaCutsceneManager: Loaded cutscene scripts successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"LuaCutsceneManager: Failed to load scripts: {ex.Message}");
            }
        }

        private static void LoadScript(string scriptName, string scriptContent)
        {
            try
            {
                var result = luaState.DoString($"return ({scriptContent})");
                if (result?.Length > 0 && result[0] is LuaTable)
                {
                    IngesteLogger.Info($"LuaCutsceneManager: Loaded script '{scriptName}' successfully");
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"LuaCutsceneManager: Failed to load script '{scriptName}': {ex.Message}");
            }
        }

        // Public API for cutscenes
        public static string CallLuaFunction(string functionName, params object[] args)
        {
            if (!IsInitialized) return null;

            try
            {
                var result = luaState.DoString($"return {functionName}");
                if (result?.Length > 0 && result[0] is LuaFunction func)
                {
                    var output = func.Call(args);
                    return output?[0]?.ToString();
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"LuaCutsceneManager: Error calling Lua function '{functionName}': {ex.Message}");
            }

            return null;
        }

        public static T CallLuaFunction<T>(string functionName, params object[] args)
        {
            if (!IsInitialized) return default(T);

            try
            {
                var result = luaState.DoString($"return {functionName}");
                if (result?.Length > 0 && result[0] is LuaFunction func)
                {
                    var output = func.Call(args);
                    if (output?.Length > 0 && output[0] is T)
                    {
                        return (T)output[0];
                    }
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"LuaCutsceneManager: Error calling Lua function '{functionName}': {ex.Message}");
            }

            return default(T);
        }

        // Context management
        private static void SetContext(string key, object value)
        {
            cutsceneContext[key] = value;
        }

        private static object GetContext(string key)
        {
            return cutsceneContext.ContainsKey(key) ? cutsceneContext[key] : null;
        }

        private static bool HasContext(string key)
        {
            return cutsceneContext.ContainsKey(key);
        }

        // Helper coroutines for Lua
        private static IEnumerator WaitSeconds(float seconds)
        {
            yield return seconds;
        }

        private static IEnumerator FadeToBlack(float duration)
        {
            // Implement fade to black
            yield return duration;
        }

        private static IEnumerator FadeFromBlack(float duration)
        {
            // Implement fade from black
            yield return duration;
        }

        // Helper functions for Lua
        private static bool IsUsingAssistMode()
        {
            var saveData = SaveData.Instance;
            if (saveData?.Assists == null) return false;

            return saveData.Assists.DashAssist ||
                   saveData.Assists.InfiniteStamina ||
                   saveData.Assists.Invincible ||
                   saveData.Assists.MirrorMode ||
                   saveData.Assists.ThreeSixtyDashing ||
                   saveData.Assists.SuperDashing ||
                   saveData.Assists.NoGrabbing ||
                   saveData.Assists.GameSpeed != 10;
        }

        private static bool HasCompletedChapter(int chapterIndex)
        {
            var saveData = SaveData.Instance;
            if (saveData?.Areas == null || chapterIndex >= saveData.Areas.Count) return false;

            var areaStats = saveData.Areas[chapterIndex];
            return areaStats?.Modes != null && areaStats.Modes.Any(mode => mode?.Completed == true);
        }
    }
}



