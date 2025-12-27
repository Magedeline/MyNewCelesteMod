--Lua Files obtained from Lua Cutscenes mod, reformatted to fit Bosses Helper

local cutsceneHelper = {}

---@module "Celeste.Mod"
local celesteMod = require("#celeste.mod")

--#region Coroutine

---@type ProxyResume
local function threadProxyResume(self, ...)
    if coroutine.status(self.value) == "dead" then
        return false, nil
    end

    local success, message = coroutine.resume(self.value)

    -- The error message should be returned as an exception and not a string
    if not success then
        return success, celesteMod.BossesHelper.Code.Helpers.Lua.LuaException(message)
    end

    return success, message
end

---@param func function
---@return LuaProxy?
function cutsceneHelper.getProxyTable(func)
    return func and {value = coroutine.create(func), resume = threadProxyResume}
end

--#endregion

--#region Lua Preparers

---@alias LuaPreparer fun(env: table, func: function): false|function, ...

---@type { [string]: LuaPreparer }
local luaPreparers = {}

function luaPreparers.getCutsceneData(env, func)
    local success, onBegin, onEnd = pcall(func)

    if success then
        onEnd = onEnd or env.onEnd
        onBegin = onBegin or env.onBegin

        return onBegin, onEnd
    end

    celesteMod.Logger.Error("Bosses Helper", "Failed to load cutscene in Lua: " .. onBegin)
    return success
end

function luaPreparers.getAttackData(env, func)
    local success, onBegin, onEnd, onComplete, onInterrupt, onDeath = pcall(func)

    if success then
        onBegin = onBegin or env.onBegin
        onEnd = onEnd or env.onEnd
        onComplete = onComplete or env.onComplete
        onInterrupt = onInterrupt or env.onInterrupt
        onDeath = onDeath or env.onDeath

        return onBegin, onEnd, onComplete, onInterrupt, onDeath
    end

    celesteMod.Logger.Error("Bosses Helper", "Failed to load attack in Lua: " .. onBegin)
    return success
end

function luaPreparers.getInterruptData(env, func)
    local success, onHit, onContact, onDash, onBounce, onLaser, setup = pcall(func)

    if success then
        onHit = onHit or env.onHit
        onContact = onContact or env.onContact
        onDash = onDash or env.onDash
        onBounce = onBounce or env.onBounce
        onLaser = onLaser or env.onLaser
        setup = setup or env.setup

        return setup, onHit, onContact, onDash, onBounce, onLaser
    end

    celesteMod.Logger.Error("Bosses Helper", "Failed to load interrupt data in Lua: " .. onHit)
    return success
end

function luaPreparers.getFunctionData(env, func)
    local success, onDamage, onRecover = pcall(func)

    if success then
        onDamage = onDamage or env.onDamage
        onRecover = onRecover or env.onRecover

        return onDamage, onRecover
    end

    celesteMod.Logger.Error("Bosses Helper", "Failed to load on damage function in Lua: " .. onDamage)
    return success
end

function luaPreparers.getSavePointData(env, func)
    local success, onTalk = pcall(func)

    if success then
        onTalk = onTalk or env.onTalk
        
        return onTalk
    end

    celesteMod.Logger.Error("Bosses Helper", "Failed to load on save point function in Lua: " .. onTalk)
    return success
end

--#endregion

--#region Lua Data Getters

local function addHelperFunctions(env)
    local helperContent = celesteMod.BossesHelper.Code.Helpers.Lua.LuaBossHelper.GetHelperFunctions()
    local helperFunctions = load(helperContent, nil, nil, env)()

    for k, v in pairs(helperFunctions) do
        env[k] = v
    end

    env.helpers = helperFunctions
end

local function getLuaEnv(data)
    local env = data or {}

    setmetatable(env, {__index = _G})

    return env
end

---@param content string
---@param data table
---@param preparer string
---@return table
---@return function|boolean ...
function cutsceneHelper.getLuaData(content, data, preparer)
    local preparationFunc = luaPreparers[preparer]
    if not preparationFunc then
        celesteMod.Logger.Error("Bosses Helper", "Preparation function not found given name: " .. preparer)
        preparationFunc = function() return false end
    end

    local env = getLuaEnv(data)

    addHelperFunctions(env)

    local func = load(content, nil, nil, env) --[[@as function]]

    return env, preparationFunc(env, func)
end

--#endregion

return cutsceneHelper
