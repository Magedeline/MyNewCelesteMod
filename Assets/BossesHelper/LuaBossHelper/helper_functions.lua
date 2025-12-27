--Lua Files obtained from Lua Cutscenes mod, reformatted to fit Bosses Helper
--Created by Cruor, modified and expanded by DavDualMain

local helpers = {}

--#region Mod Imports

---Mostly used for lua-language-server annotations and VS Code support

---@module "Monocle"
local _monocle = require("#monocle")

---@module "Celeste"
local _celeste = require("#celeste")

---@module "Microsoft.XNA.Framework.Vector2"
local _vector2 = require("#microsoft.xna.framework.vector2")

helpers.monocle = _monocle
helpers.engine = _monocle.Engine
helpers.celeste = _celeste

--#endregion

--#region Local Shortcuts

--- Locals to shortcut certain common accessed sub-tables.

local ease = _monocle.Ease
local celesteMod = _celeste.Mod
local engine = helpers.engine

local modName = modMetaData.Name
local bossesHelper = celesteMod[modName] --[[@as Celeste.Mod.BossesHelper]]
local classNamePrefix = "Celeste."

local luanet = _G.luanet

--#endregion



--#region Original Helper Functions

--- Helper functions that can be used in cutscenes.
-- When using from a cutscene "helpers." is not required.
-- For example "helpers.say" will be just "say".
-- Return values starting with # are from C#.
-- @module helper_functions

---Returns a new Vector2
---@param x float
---@param y float
---@return Vector2
---@overload fun(x: table): Vector2
---@overload fun(x: Vector2): Vector2
function helpers.vector2(x, y)
    if type(x) == "table" and not y then
        return _vector2(x[1], x[2])

    elseif type(x) == "userdata" and not y then
        return x --[[@as Vector2]]

    else
        return _vector2(x, y)
    end
end

local vector2 = helpers.vector2

local function simpleSplit(s, sep)
    local res = {}

    for part in s:gmatch("[^" .. sep .. "]+") do
        table.insert(res, part)
    end

    return res
end

local function getClassAndField(full)
    local parts = simpleSplit(full, "%.")
    local field = parts[#parts]

    table.remove(parts)

    local class = table.concat(parts, ".")

    return class, field
end

--- Set the prefix for getting Celeste classes.
--- By default this is "Celeste.".
---@param prefix string The new prefix.
function helpers.setClassNamePrefix(prefix)
	classNamePrefix = prefix
end

--- Get the prefix for getting Celeste classes.
--- By default this is "Celeste.".
---@return string classNamePrefix The class name prefix.
function helpers.getClassNamePrefix()
	return classNamePrefix
end

--- Get the content of a file from a Celeste asset.
---@param filename string Filename to load. Filename should not have a extention.
---@return string content The content of the file
function helpers.readCelesteAsset(filename)
    return bossesHelper.Code.Helpers.Lua.LuaBossHelper.GetFileContent(filename)
end

--- Loads and returns the result of a Lua asset.
---@param filename string Filename to load. Filename should not have a extention.
---@return table|nil result Tahble of loaded asset or nil
function helpers.loadCelesteAsset(filename)
    local content = helpers.readCelesteAsset(filename)

    if not content then
        celesteMod.Logger.Error("Bosses Helper", "Failed to require asset in Lua: file '" .. filename .. "' not found")

        return
    end

    local env = {}

    setmetatable(env, {__index = _ENV})

    local func = load(content, nil, nil, env)

    if func then
        local success, result = pcall(func)

        if success then
            return result
        end

        celesteMod.Logger.Error("Bosses Helper", "Failed to require asset in Lua: " .. result)
    end

    celesteMod.Logger.Error("Bosses Helper", "Failed to require asset in Lua: Not valid lua.")
end

---Load the file given by the path as a Lua table.
---@param path string The file path within the mod.
---@param assetMod? string The name of the mod to get the asset from. Defaults to BossesHelper
---@return table # The table loaded from the file given, if successful.
function helpers.getLuaAsset(path, assetMod)
    return celesteMod.BossesHelper.Code.Helpers.Lua.LuaBossHelper.GetLuaAsset(path, assetMod)
end

--- Put debug message in the Celeste console.
---@param message any The debug message.
---@param tag? string The tag in the console.
---@default "Bosses Helper"
function helpers.log(message, tag)
    celesteMod.Logger.Info(tag or "Bosses Helper", tostring(message))
end

---Gets enum value.
---@param enum string String name of enum.
---@param value int|string string name or enum value to get.
---@return Enum enumValue
function helpers.getEnum(enum, value)
    local enumValue = luanet.enum(luanet.import_type(enum), value)

    -- Check for enum on class
    if not enumValue then
        local class, field = getClassAndField(enum)

        enumValue = luanet.enum(luanet.import_type(class)[field], value)
    end

	return enumValue
end

--- Pause code exection for duration seconds.
---@param duration? float Duration to wait (in seconds).
---@return float?
---@overload fun(routine: IEnumerator): IEnumerator
function helpers.wait(duration)
    return coroutine.yield(duration)
end

local wait = helpers.wait

--- Gets the current room the player is in.
---@return Level level The current room.
function helpers.getRoom()
    return engine.Scene --[[@as Level]]
end

helpers.getLevel = helpers.getRoom

local getLevel = helpers.getLevel

--- Gets the current session.
---@return Session session The current session.
function helpers.getSession()
    return getLevel().Session
end

local getSession = helpers.getSession

--- Display textbox with dialog.
---@param dialog string Dialog ID used for the conversation.
---@return IEnumerator
function helpers.say(dialog)
    return wait(_celeste.Textbox.Say(tostring(dialog)))
end

--- Display minitextbox with dialog.
---@param dialog string Dialog ID used for the textbox.
function helpers.miniTextbox(dialog)
    engine.Scene:Add(_celeste.MiniTextbox(dialog))
end

--- Allow the user to select one of several minitextboxes, similar to intro cutscene of Reflection.
---@param ... string Dialog IDs for each of the textboxes as varargs. First argument can be a table of dialog ids instead.
---@return int index The index of the option the player selected.
---@overload fun(ids: string[]): int
---@overload fun(...: string): int
function helpers.choice(...)
    local choices = {...}

    local first = choices[1]
    if type(first) == "table" then
        choices = first
    end

    wait(celesteMod.LuaCutscenes.ChoicePrompt.Prompt(table.unpack(choices)))

    return celesteMod.LuaCutscenes.ChoicePrompt.Choice + 1
end

-- Used by helpers.choiceDialog to store the current dialog table.
-- When this gets set to null, the dialog ends.
local currentDialog

-- Used by helpers.choiceDialog to find an entry in the dialogTable based on its dialog key
local function findDialogByKey(dialogTable, key)
    for i, value in ipairs(dialogTable) do
        local dialogKey = value[1]

        if dialogKey == key then
            return value
        end
    end
end

-- Used by helpers.choiceDialog to check whether the requirements for a choice are met
local function requirementsMet(requires, ctx)
    if type(requires) ~= "table" then
        requires = { requires }
    end

    for _, required in ipairs(requires) do
        local t = type(required)

        if t == "function" then
            if not required(ctx) then
                return false
            end

        elseif t == "string" then
            if not ctx.usedDialogs[required] then
                return false
            end
        end
    end

    return true
end

local function prepareDialogTable(dialogTable)
    local dialogs = {}

    for _, dialog in ipairs(dialogTable) do
        if type(dialog) == "string" then
            table.insert(dialogs, {dialog, repeatable = true})

        else
            table.insert(dialogs, dialog)
        end
    end

    return dialogs
end

---Displays a choice dialog, similar to intro cutscene of Reflection.
---Unlike helpers.choice, this function also handles displaying dialogs and keeping track of which choices were already picked.
---Check the 'example_talker.lua' file for a usage example.
---@param dialogs table Table describing all choices and requirements, etc.
function helpers.choiceDialog(dialogs)
    local dialogTable = prepareDialogTable(dialogs)

    currentDialog = dialogTable

    local ctx = {
        usedDialogs = {}
    }

    while currentDialog do
        local currChoices = {}

        -- determine which choices are valid
        for _, value in ipairs(dialogTable) do
            local dialogKey = value[1]

            if not value.repeatable and ctx.usedDialogs[dialogKey] then
                goto next
            end

            if value.requires and not requirementsMet(value.requires, ctx) then
                goto next
            end

            table.insert(currChoices, dialogKey)
            ::next::
        end

        local chosenKey = currChoices[helpers.choice(currChoices)]
        local chosen = findDialogByKey(dialogTable, chosenKey)

        if chosen.onChosen then
            chosen.onChosen(ctx)

        else
            helpers.say(chosenKey .. "_SAY")
        end

        if chosen.onEnd then
            chosen.onEnd(ctx)
        end

        ctx.usedDialogs[chosenKey] = true
    end
end

---Closes the choice dialog previously opened by helpers.choiceDialog
function helpers.closeChoiceDialog()
    currentDialog = nil
end

--- Display postcard.
---@param dialog string Dialog ID or message to show in the postcard.
---@param sfxIn string|int effect when opening the postcard or area ID.
---@param sfxOut? string Sound effect when closing the postcard. If not used then second argument is assumed to be area ID.
---@default nil
function helpers.postcard(dialog, sfxIn, sfxOut)
    local message = _celeste.Dialog.Get(dialog) or dialog
    local postcard

    if sfxOut then ---@cast sfxIn string
        postcard = _celeste.Postcard(message, sfxIn, sfxOut)

    else ---@cast sfxIn int
        postcard = _celeste.Postcard(message, sfxIn)
    end

    getLevel():Add(postcard)
    postcard:BeforeRender()

    wait(postcard:DisplayRoutine())
end

--- Player walks to the given X coordinate. This is in pixels and uses map based coordinates.
---@param x float X coordinate to walk to.
---@param walkBackwards? boolean If the player should visually be walking backwards.
---@default false
---@param speedMultiplier? float How fast the player should move. Walking is considered a speed multiplier of 1.0.
---@default 1.0
---@param keepWalkingIntoWalls? boolean If the player should keep walking into walls.
---@default false
---@return IEnumerator
function helpers.walkTo(x, walkBackwards, speedMultiplier, keepWalkingIntoWalls)
    return wait(player:DummyWalkTo(x, walkBackwards or false, speedMultiplier or 1, keepWalkingIntoWalls or false))
end

--- Player walks x pixels from current position.
---@param x float X offset for where player should walk.
---@param walkBackwards? boolean If the player should visually be walking backwards.
---@default false
---@param speedMultiplier? float How fast the player should move. Walking is considered a speed multiplier of 1.0.
---@default 1.0
---@param keepWalkingIntoWalls? boolean If the player should keep walking into walls.
---@default false
---@return IEnumerator
function helpers.walk(x, walkBackwards, speedMultiplier, keepWalkingIntoWalls)
    return helpers.walkTo(player.Position.X + x, walkBackwards, speedMultiplier, keepWalkingIntoWalls)
end

--- Player runs to the given X coordinate. This is in pixels and uses map based coordinates.
---@param x float X coordinate to run to.
---@param fastAnimation? boolean Whether this should use the fast animation or not.
---@return IEnumerator
function helpers.runTo(x, fastAnimation)
    return wait(player:DummyRunTo(x, fastAnimation or false))
end

--- Player runs x pixels from current position.
---@param x float X offset for where player should run.
---@param fastAnimation? boolean Whether this should use the fast animation or not.
---@return IEnumerator
function helpers.run(x, fastAnimation)
    return helpers.runTo(player.Position.X + x, fastAnimation)
end

--- Kills the player.
---@param direction? table|Vector2 The direction the player dies from.
---@default {0, 0}
---@param evenIfInvincible? boolean If the player should die even if they are invincible (assist mode).
---@default false
---@param registerDeathInStats? boolean If it should count as a death in journal.
---@default true
function helpers.die(direction, evenIfInvincible, registerDeathInStats)
    if player and not player.Dead then
        player:Die(vector2(direction or {0, 0}), evenIfInvincible or false, registerDeathInStats or registerDeathInStats == nil)
    end
end

--- Sets the current player state.
---@param state string|int Name of the state or the state float.
---@param locked? boolean If this should prevent the player for changing state afterwards.
---@default false
function helpers.setPlayerState(state, locked)
    if type(state) == "string" then
        if not state:match("^St") then
            state = "St" .. state
        end

        player.StateMachine.State = player[state]

    else
        player.StateMachine.State = state
    end

    player.StateMachine.Locked = locked or false
end

--- Gets the current state of the player.
---@return int state The current Player's state
---@return boolean locked If the StateMachine is Locked
function helpers.getPlayerState()
    return player.StateMachine.State, player.StateMachine.Locked
end

--- Disable player movement.
function helpers.disableMovement()
    helpers.setPlayerState("Dummy", false)
end

--- Enable player movement.
function helpers.enableMovement()
    helpers.setPlayerState("Normal", false)
end

--- Make the player jump.
---@param duration? float How long the "jump button" would be held (in seconds).
---@default 2.0
function helpers.jump(duration)
    player:Jump(true, true)
    player.AutoJump = true
    player.AutoJumpTimer = duration or 2.0
end

--- Waits until the player is on the ground.
function helpers.waitUntilOnGround()
    while not player:OnGround(1) do
        wait()
    end
end

--- Changes the room the game thinks the player is in.
---@param name string Room name.
---@param spawnX? float X coordinate for new spawn point, by default it uses bottom left of room.
---@param spawnY? float Y coordinate for new spawn point, by default it uses bottom left of room.
function helpers.changeRoom(name, spawnX, spawnY)
    local level = getLevel()

    level.Session.Level = name
    level.Session.RespawnPoint = level:GetSpawnPoint(vector2(spawnX or level.Bounds.Left, spawnY or level.Bounds.Bottom))
    level.Session:UpdateLevelStartDashes()

    -- TODO - Test
    engine.Scene = _celeste.LevelLoader(level.Session, level.Session.RespawnPoint)
end

function helpers.getRoomPosition(name)
    -- TODO - Implement
    -- If name is absent use current room
end

--- Sets the player position to the absolute coordinates.
---@param x float Target x coordinate.
---@param y float Target y coordinate.
---@param room? string What room the game should attempt to load. If room is specified player will land at closest spawnpoint to target location.
---@param introType string|IntroTypes intro type to use, can be either a #IntroTypes enum or a string
---@overload fun(x: float, y: float, room?: string, introType?: string|IntroTypes)
---@overload fun(pos: Vector2, room?: string, introType?: string|IntroTypes)
function helpers.teleportTo(x, y, room, introType)
    if type(introType) == "string" then
        introType = helpers.getEnum("Celeste.Player.IntroTypes", introType) --[[@as IntroTypes]]
    end

    if room then
        local mapData = getSession().MapData
        local levelData = mapData:GetAt(vector2(x, y))

        -- TeleportTo adds the new room offset for spawnpoint check, we have to remove this
        local offsetX, offsetY = 0, 0

        if levelData then
            local bounds = levelData.Bounds

            offsetX, offsetY = bounds.X, bounds.Y
        end

        if x and y then
            bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.TeleportTo(getLevel(), player, room, introType or player.IntroType, vector2(x - offsetX, y - offsetY))
        else
            bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.TeleportTo(getLevel(), player, room, introType or player.IntroType)
        end

	else
		player.Position = vector2(x, y)
	end
end

--- Teleport the player to (x, y) pixels from current position.
---@param x float X offset on X axis.
---@param y float Y offset on Y axis.
---@param room? string What room the game should attempt to load. If room is specified player will land at closest spawnpoint to target location.
---@param introType? string|IntroTypes intro type to use, can be either a #IntroTypes enum or a string. Only applies if room is specified.
function helpers.teleport(x, y, room, introType)
    helpers.teleportTo(player.Position.X + x, player.Position.Y + y, room, introType)
end

--- Instantly teleport the player seamlessly.
--- Teleport player to (x, y) position, in pixels.
--- Room name as only argument will seamlessly teleport to that room at the same relative position.
---@param x float|string X offset on X axis if float. Target room if string.
---@param y? float Y offset on Y axis.
---@param room? string What room the game should attempt to load. By default same room.
---@overload fun(x: string)
function helpers.instantTeleportTo(x, y, room)
    if x and y then ---@cast x float
        -- Provide own position
        bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.InstantTeleport(getLevel(), player, room or "", false, x, y)

    else ---@cast x string
        -- Keep releative room position
        bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.InstantTeleport(getLevel(), player, x or "", true, 0.0, 0.0)
    end
end

--- Instantly teleport the player to the same coordinates in another room seamlessly.
--- Teleport player (x, y) pixels from current position.
--- Room name as only argument will seamlessly teleport to that room at the same relative position.
---@param x float X offset on X axis if float. Target room if string.
---@param y float Y offset on Y axis.
---@param room? string What room the game should attempt to load. By default same room.
---@overload fun(x: string)
function helpers.instantTeleport(x, y, room)
    if x and y then
        helpers.instantTeleportTo(player.Position.X + x, player.Position.Y + y, room)

    else
        helpers.instantTeleportTo(x, y, room)
    end
end

--- Completes the level and returns the player to the chapter screen.
---@param spotlightWipe? boolean Whether this should be a spotlight wipe or not.
---@default false
---@param skipScreenWipe? boolean Whether this wipe is skipped or not.
---@default false
---@param skipCompleteScreen? boolean Whether this skips the complete screen.
---@default false
function helpers.completeArea(spotlightWipe, skipScreenWipe, skipCompleteScreen)
    getLevel():CompleteArea(spotlightWipe or false, skipScreenWipe or false, skipCompleteScreen or false)
end

--- Plays a sound.
---@param name string Event for the song.
---@param position? Vector2 Where the sound is played from.
---@return EventInstance audio The audio instance of the sound.
function helpers.playSound(name, position)
    if position then
        return _celeste.Audio.Play(name, position)

    else
        return _celeste.Audio.Play(name)
    end
end

--- Gets all tracked entities by class name.
---@param name string Class name of the entity, relative to "Celeste." by default.
---@param prefix? string Overrides the global class name prefix.
---@return table entities Tracked entities of given class.
function helpers.getEntities(name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetEntities(name, prefix or classNamePrefix)
end

--- Gets the first tracked entity by class name.
---@param name string Class name of the entity, relative to "Celeste." by default.
---@param prefix? string Overrides the global class name prefix.
---@return any entity First tracked entity of given class.
function helpers.getEntity(name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetEntity(name, prefix or classNamePrefix)
end

--- Gets all entities by class name.
---@param name string Class name of the entity, relative to "Celeste." by default.
---@param prefix? string Overrides the global class name prefix.
---@return table entities All entities of given class.
function helpers.getAllEntities(name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetAllEntities(name, prefix or classNamePrefix)
end

--- Gets the first entity by class name.
---@param name string Class name of the entity, relative to "Celeste." by default.
---@param prefix? string Overrides the global class name prefix.
---@return any entity First entity of given class.
function helpers.getFirstEntity(name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetFirstEntity(name, prefix or classNamePrefix)
end

--- Puts player in feather state.
function helpers.giveFeather()
    player:StartStarFly()
end

--- Get amount of deaths in current room.
---@return int deaths Current deaths in room.
function helpers.deathsInCurrentRoom()
    return getSession().DeathsInCurrentLevel
end

--- Play and update the current music track.
---@param track string Name of song, same as in Ahorn's room window.
---@param progress? int Which progress level the music should be at. Leave empty for no change.
function helpers.playMusic(track, progress)
    getSession().Audio.Music.Event = _celeste.SFX.EventnameByHandle(track)

    if progress then
        getSession().Audio.Music.Progress = progress
    end

    getSession().Audio:Apply()
end

--- Get the current music track name.
---@return string track Music track name.
function helpers.getMusic()
    return _celeste.Audio.CurrentMusic
end

--- Sets music progression.
---@param progress float The new progress level.
function helpers.setMusicProgression(progress)
    getSession().Audio.Music.Progress = progress
    getSession().Audio:Apply()
end

--- Gets the current music progression.
---@return int progress Music progress level.
function helpers.getMusicProgression()
    return getSession().Audio.Music.Progress
end

--- Set music layer on/off.
---@param layer float[]|float float or table of floats to set.
---@param value boolean The state of the layer.
function helpers.setMusicLayer(layer, value)
    if type(layer) == "table" then
        for _, index in ipairs(layer) do
            getSession().Audio.Music:Layer(index, value)
        end

    else
        getSession().Audio.Music:Layer(layer, value)
    end

    getSession().Audio:Apply()
end

--- Attempt to set the player spawnpoint.
---@param target? table Where it should attempt to set the spawnpoint from.
---@default {0, 0}
---@param absolute? boolean If set uses absolute coordinates from target, otherwise it offsets from the center of the cutscene trigger.
---@default false
function helpers.setSpawnPoint(target, absolute)
    local session = getSession()

    target = target or {0, 0}
    target = absolute and target or vector2(player.Position.X + target[1], player.Position.Y + target[2])

    if session.RespawnPoint and (session.RespawnPoint.X ~= target.X or session.RespawnPoint.Y ~= target.Y) then
        session.HitCheckpoint = true
        session.RespawnPoint = target
        session:UpdateLevelStartDashes()
    end
end

--- Shakes the camera.
---@param direction Vector2|float Direction the screen should shake from.
---@param duration? float How long the screen should shake.
---@overload fun(direction: float)
function helpers.shake(direction, duration)
    if direction and duration then ---@cast direction Vector2
        getLevel():DirectionalShake(direction, duration)

    else ---@cast direction float
        getLevel():Shake(direction)
    end
end

--- Set player inventory
---@param inventory string|PlayerInventory Inventory to use. If name is string look it up in valid inventories, otherwise use the inventory.
function helpers.setInventory(inventory)
    if type(inventory) == "string" then
        getSession().Inventory = celeste.PlayerInventory[inventory]

    else
        getSession().Inventory = inventory
    end
end

--- Get player inventory
---@param inventory? string If name is given get inventory by name, otherwise the current player inventory
---@return PlayerInventory inventory
function helpers.getInventory(inventory)
    if inventory then
        return celeste.PlayerInventory[inventory]

    else
        return getSession().Inventory
    end
end

--- Offset the camera by x and y like in camera offset trigger.
---@param x float X coordinate or table of coordinates to offset by.
---@param y float Y coordinate to offset by.
---@overload fun(x: table)
function helpers.setCameraOffset(x, y)
    getLevel().CameraOffset = y and vector2(x * 48, y * 32) or x
end

--- Get the current offset struct.
---@return Vector2 offset The camera offset.
function helpers.getCameraOffset()
    return getLevel().CameraOffset
end

--- Get the current room coordinates.
---@return Vector2 offset The camera offset.
function helpers.getRoomCoordinates()
  return getLevel().LevelOffset
end

--- Get the current room coordinates offset by x and y.
---@param x float X coordinate or table of coordinates to offset by.
---@param y float Y coordinate to offset by.
---@return Vector2 offset The camera offset.
---@overload fun(offset: table): Vector2
function helpers.getRoomCoordinatesOffset(x, y)
    if type(x) == "float" then
        return getLevel().LevelOffset + vector2(x, y)

    else
        return getLevel().LevelOffset + vector2(x[1], x[2])
    end
end

--- Set session flag.
---@param flag string Flag to set.
---@param value? boolean State of flag.
function helpers.setFlag(flag, value)
    getSession():SetFlag(flag, value)
end

--- Get session flag.
---@param flag string Flag to get.
---@return boolean state The state of the flag.
function helpers.getFlag(flag)
    return getSession():GetFlag(flag)
end

-- TODO - Accept table?
-- TODO - Unhaunt, the index needs to be handled
---@param x float
---@param y float
---@param relativeToPlayer? boolean
---@return BadelineOldsite
function helpers.spawnBadeline(x, y, relativeToPlayer)
    local position = (relativeToPlayer or relativeToPlayer == nil) and vector2(player.Position.X + x, player.Position.Y + y) or vector2(x, y)
    local badeline = _celeste.BadelineOldsite(position, 1)

    engine.Scene:Add(badeline)

    return badeline
end

--- Ends the current cutscene.
function helpers.endCutscene()
    cutsceneEntity:EndCutscene(getLevel())
end

--- Sets the current bloom strength.
---@param amount float New bloom strength.
function helpers.setBloomStrength(amount)
    getLevel().Bloom.Strength = amount
end

--- Returns the current bloom strength.
---@return float strength Bloom strength.
function helpers.getBloomStrength()
    return getLevel().Bloom.Strength
end

helpers.setDarkness = helpers.setBloomStrength
helpers.getDarkness = helpers.getBloomStrength

--- Sets the current core mode.
---@param mode string|CoreModes String name for mode or Core Mode enum.
function helpers.setCoreMode(mode)
    if type(mode) == "string" then
        getLevel().CoreMode = celeste.Session.CoreModes[mode]

    else
        getLevel().CoreMode = mode
    end
end

--- Returns the current core mode.
---@return CoreModes
function helpers.getCoreMode()
    return getLevel().CoreMode
end

--- Changes the current colorgrade to the new one.
---@param colorGrade string Name of the color grade
---@param instant? boolean Wheter the color grade should instantly change or gradually change
---@default false
function helpers.setColorGrade(colorGrade, instant)
    if instant then
        getLevel():SnapColorGrade(colorGrade)

    else
        getLevel():NextColorGrade(colorGrade)
    end
end

--- Bubble flies (cassette collection) to the target. This is in pixels and uses map based coordinates.
---@param endX float X coordinate for end point.
---@param endY float Y coordinate for end point.
---@param controlX? float X coordinate for control point.
---@default endX
---@param controlY? float Y coordinate for control point.
---@default endY
function helpers.cassetteFlyTo(endX, endY, controlX, controlY)
    playSound("event:/game/general/cassette_bubblereturn", vector2(getLevel().Camera.Position.X + 160, getLevel().Camera.Position.Y + 90))

    if endX and endY and controlX and controlY then
        player:StartCassetteFly(vector2(endX, endY), vector2(controlX, controlY))

    else
        player:StartCassetteFly(vector2(endX, endY), vector2(endX, endY))
    end
end

--- Bubble flies (cassette collection) to the target relative to player. Values are in pixels and not tiles.
---@param endX float X offset for end point.
---@param endY float Y offset for end point.
---@param controlX? float X offset for control point.
---@default endX
---@param controlY? float Y offset for control point.
---@default endY
function helpers.cassetteFly(endX, endY, controlX, controlY)
    local playerX = player.Position.X
    local playerY = player.Position.Y

    controlX = controlX or endX
    controlY = controlY or endY

    helpers.cassetteFlyTo(endX + playerX, endY + playerY, controlX + playerX, controlY + playerY)
end

--- Set session level flag.
---@param flag string Flag to set.
---@param value boolean State of flag.
function helpers.setLevelFlag(flag, value)
    if value then
        getSession().LevelFlags:Add(flag)
    else
        getSession().LevelFlags:Remove(flag)
    end
end

--- Get session level flag.
---@param flag string Flag to get.
---@return boolean state The state of the flag.
function helpers.getLevelFlag(flag)
    return getSession():GetLevelFlag(flag)
end

--- Gives the player a key.
function helpers.giveKey()
    local level = getLevel()
    local key = _celeste.Key(player, _celeste.EntityID("unknown", 1073741823 + math.random(0, 10000)))

    level:Add(key)
    level.Session.Keys:Add(key.ID)
end

-- Test
function helpers.setWind(pattern)
    local windController = helpers.getFirstEntity("WindController")

    if type(pattern) == "string" then
        pattern = windController.Patterns[pattern]
    end

    if windController then
        windController:SetPattern(pattern)

    else
        windController = _celeste.WindController(pattern)
        engine.Scene:Add(windController)
    end
end

-- Requires reflection :(
function helpers.getWind()
    local windController = helpers.getFirstEntity("WindController")

    if windController then
        return windController.startPattern
    end
end

-- Requires Enums
function helpers.rumble(...)
    -- TODO - Implement
end

--- Disables skip cutscene from menu.
function helpers.makeUnskippable()
    getLevel():CancelCutscene()
end

--- Enables retrying from menu.
function helpers.enableRetry()
    getLevel().CanRetry = true
end

--- Disables retrying from menu.
function helpers.disableRetry()
    getLevel().CanRetry = false
end

--- Prevents the player from even accessing the pause menu.
function helpers.disablePause()
    getLevel().PauseLock = true
end

--- Reenables the player to pause the game.
function helpers.enablePause()
    getLevel().PauseLock = false
end

--#endregion



--#region Bosses Helper functions

--- Bosses Helper Attack specific functions and helper methods for the player

--#region Entity Adding

--- Adds the provided entity onto the scene, as well as into the Boss' tracked entities.
---@param entity Entity The entity to add
function helpers.addEntity(entity)
    boss:AddEntity(entity)
end

--- Calls RemoveSelf on the entity provided, as well as removing it from the tracked entities.
---@param entity Entity The entity to destroy.
function helpers.destroyEntity(entity)
    boss:DestroyEntity(entity)
end

--- Calls RemoveSelf on all active tracked entities.
function helpers.destroyAll()
    boss:DestroyAll()
end

--#endregion

--#region Fight Logic

---Plan an animation on the Boss's given sprite
---@param anim string The animation to play
function helpers.playPuppetAnim(anim)
    puppet:PlayAnim(anim)
end

---Play an animation on the Boss's given sprite and wait for it to complete one full cycle.
---@param anim string The animation to play
---@return IEnumerator
function helpers.playAndWaitPuppetAnim(anim)
    return wait(celesteMod.BossesHelper.Code.Helpers.BossesHelperUtils.PlayAnim(puppet.Sprite, anim))
end

---Get a random float based on the boss's random seed.
---@return int next A seeded-random int.
function helpers.seededRandom()
    return boss.Random:Next()
end

---Get the Boss' current health value
---@return int health The Boss's current health value
function helpers.getHealth()
    return boss.Health
end

---Set the Boss' health value to a new value.
---@param health int The value to set the health to.
function helpers.setHealth(health)
    boss.Health = health
end

---Decrease the Boss' health by the given value
---@param health? int The amount of health lost. Defaults to 1.
---@default 1
function helpers.decreaseHealth(health)
    boss.Health = boss.Health - (health or 1)
end

--- Wait for the current attack coroutine to end
function helpers.waitForAttackToEnd()
    while boss.IsActing do
        wait()
    end
end

---Interrupt the current boss action pattern
function helpers.interruptPattern()
    boss:InterruptPattern()
end

---Gets the currently set pattern index
---@return int ID The current pattern's index, base 0
function helpers.getCurrentPatternIndex()
    return boss.CurrentPatternIndex
end

helpers.getCurrentPatternID = helpers.getCurrentPatternIndex

---Gets the currently set pattern index
---@return string Name The current pattern's name, if any.
function helpers.getCurrentPatternName()
    return boss.CurrentPatternName
end

---Get the index of the Pattern that is identified by the given name, if any.
---@param name string The name of the Pattern to search for.
---@return int index The index of the pattern with the given name, or -1 if not found.
function helpers.getPatternIndex(name)
    return boss:GetPatternIndex(name)
end

---Start a new boss action pattern.
---@param goTo? int The pattern index to start executing. Defaults to -1, which will start the currently set pattern again.
---@default -1
function helpers.startAttackPattern(goTo)
    boss:StartAttackPattern(goTo or -1)
end

---Start the next action pattern in index order.
function helpers.startNextAttackPattern()
    helpers.startAttackPattern(helpers.getCurrentPatternID() + 1)
end

---Force the next attack to be the attack of the given index. Index is based off of position within the Pattern.
---Currently only supported in Random Patterns. The index is always ran past a modulo on the pattern attacks' count to avoid an out-of-bounds issue.
---@param index int The attack index to select next. Will only take effect once per call.
function helpers.forceNextAttackIndex(index)
    boss:ForceNextAttack(index)
end

---Saves certain values to the Mod's Session so they are stored on Retry and even on Save and Quit. These values will be fetched by the controler automatically when loaded back into the level.
---@param health int The health value to save and set back upon reload.
---@param index int The pattern index the boss should start with upon reload.
---@param startImmediately? boolean If the Boss should start the defined action pattern immediately instead of waiting for the player to move. Defaults to false.
---@default true
function helpers.savePhaseChangeToSession(health, index, startImmediately)
    boss:SavePhaseChangeToSession(health or helpers.getHealth(), index or helpers.getCurrentPatternID(), startImmediately or false)
end

---Removes the Boss from the scene, alongside its puppet and any Entities spawned by it.
---This function also Works in Cutscene files
---@param permanent boolean If the boss should not be loaded again. False will spawn the Boss every time the room is loaded.
function helpers.removeBoss(permanent)
    boss:RemoveBoss(permanent or false)
end

--#endregion

--#region Position and Movement

--- Set the gravity multiplier to the given value. Gravity constant is 900.
--- @param mult float The multiplier to apply to the Gravity constant which the Boss will use.
function helpers.setEffectiveGravityMult(mult)
    puppet.GravityMult = mult
end

---Set the Boss's horizontal ground friction deceleration rate.
---@param friction float The deceleration rate to set.
function helpers.setGroundFriction(friction)
    puppet.groundFriction = friction
end

---Set the Boss's horizontal air friction deceleration rate.
---@param friction float The deceleration rate to set.
function helpers.setAirFriction(friction)
    puppet.airFriction = friction
end

---Set the Boss' x speed to the given value
---@param value float The value to set the Boss' speed x component to.
function helpers.setXSpeed(value)
    puppet.Speed = vector2(value, puppet.Speed.Y)
end

---Set the Boss' y speed to the given value
---@param value float The value to set the Boss' speed y component to.
function helpers.setYSpeed(value)
    puppet.Speed = vector2(puppet.Speed.Y, value)
end

---Set the Boss' speed to the given values
---@param x float The value to set the Boss' speed x component to.
---@param y float The value to set the Boss' speed y component to.
function helpers.setSpeed(x, y)
    puppet.Speed = vector2(x, y)
end

---Set the Boss' x speed to the given value, kept constant during the given time.
---@param value float The value to set the Boss' speed x component to.
---@param time float The time to hold the value for.
---@return float time The time given from the Tween
function helpers.setXSpeedDuring(value, time)
    puppet:Set1DSpeedDuring(value, true, time)
    return time
end

---Set the Boss' y speed to the given value, kept constant during the given time.
---@param value float The value to set the Boss' speed y component to.
---@param time float The time to hold the value for.
---@return float time The time given from the Tween
function helpers.setYSpeedDuring(value, time)
    puppet:Set1DSpeedDuring(value, false, time)
    return time
end

---Set the Boss' speed to the given values, kept constant during the given time.
---@param x float The value to set the Boss' speed x component to.
---@param y float The value to set the Boss' speed y component to.
---@param time float The time to hold the values for.
---@return float time The time given from the Tween
function helpers.setSpeedDuring(x, y, time)
    puppet:Set1DSpeedDuring(x, true, time)
    puppet:Set1DSpeedDuring(y, false, time)
    return time
end

---Keep the Boss' current x speed constant during the given time.
---@param time float The time to hold the value for.
---@return float time The time given from the Tween
function helpers.keepXSpeedDuring(time)
    puppet:Set1DSpeedDuring(puppet.Speed.X, true, time)
    return time
end

---Keep the Boss' current y speed constant during the given time.
---@param time float The time to hold the value for.
---@return float time The time given from the Tween
function helpers.keepYSpeedDuring(time)
    puppet:Set1DSpeedDuring(puppet.Speed.Y, false, time)
    return time
end

---Keep the Boss' current speed constant during the given time.
---@param time float The time to hold the values for.
---@return float time The time given from the Tween
function helpers.keepSpeedDuring(time)
    puppet:Set1DSpeedDuring(puppet.Speed.X, true, time)
    puppet:Set1DSpeedDuring(puppet.Speed.Y, false, time)
    return time
end

local function getEaser(easer, invert)
    if type(easer) == "string" then
        return helpers.getEaserByName(easer, invert)
    elseif type(easer) == "userdata" then ---@cast easer Ease.Easer
        if invert then
            return ease.Invert(easer)
        end
        return easer
    end
    return nil
end

---Create a new Position Tween, which will slowly move the Boss to the target.
---@param target Vector2 The vector2 target position the Boss will move towards.
---@param time float The time the Boss will take to reach the target.
---@param treatNaive? bool Whether to ignore collisions when moving.
---@param stopOnCollide? bool Whether to stop the Tween on a collision.
---@param easer? string|Ease.Easer The easer to apply to the motion. If a string is provided, it will call helpers.getEaserByName. Defaults to nil.
---@default nil
---@param invert? boolean If the easer should be inverted. Defaults to false.
---@default false
---@return float time The time given from the Tween
function helpers.positionTween(target, time, treatNaive, stopOnCollide, easer, invert)
    celesteMod.BossesHelper.Code.Helpers.BossesHelperUtils.PositionTween(puppet, target, time, treatNaive, stopOnCollide, getEaser(easer, invert))
    return time
end

---Create a new Tween for the Boss' x speed.
---@param start float The initial value of the Tween, which the Boss' speed x component will set to at the start.
---@param target float The value the Boss' speed x component will slowly change to.
---@param time float The time the Boss will take to reach the target x speed.
---@param easer? string|Ease.Easer The easer to apply to the x speed value. If a string is provided, it will call helpers.getEaserByName. Defaults to nil.
---@default nil
---@param invert? boolean If the easer should be inverted. Defaults to false.
---@default false
---@return float time The time given from the Tween
function helpers.speedXTween(start, target, time, easer, invert)
    puppet:Speed1DTween(start, target, time, true, getEaser(easer, invert))
    return time
end

---Create a new Tween for the Boss' y speed.
---@param start float The initial value of the Tween, which the Boss' speed y component will set to at the start.
---@param target float The value the Boss' speed y component will slowly change to.
---@param time float The time the Boss will take to reach the target y speed.
---@param easer? string|Ease.Easer The easer to apply to the y speed value. If a string is provided, it will call helpers.getEaserByName. Defaults to nil.
---@default nil
---@param invert? boolean If the easer should be inverted. Defaults to false.
---@default false
---@return float time The time given from the Tween
function helpers.speedYTween(start, target, time, easer, invert)
    puppet:Speed1DTween(start, target, time, false, getEaser(easer, invert))
    return time
end

---Create a new Tween for the Boss' speed.
---@param xStart float The initial value of the Tween, which the Boss' speed x component will set to at the start.
---@param xTarget float The value the Boss' speed x component will slowly change to.
---@param yStart float The initial value of the Tween, which the Boss' speed y component will set to at the start.
---@param yTarget float The value the Boss' speed y component will slowly change to.
---@param time float The time the Boss will take to reach the target x speed.
---@param easer? string|Ease.Easer The easer to apply to the x speed value. If a string is provided, it will call helpers.getEaserByName. Defaults to nil.
---@default nil
---@param invert? boolean If the easer should be inverted. Defaults to false.
---@default false
---@return float time The time given from the Tween
function helpers.speedTween(xStart, yStart, xTarget, yTarget, time, easer, invert)
    helpers.speedXTween(xStart, xTarget, time, easer, invert)
    helpers.speedYTween(yStart, yTarget, time, easer, invert)
    return time
end

---Create a new Tween for the Boss' x speed from its current x speed value.
---@param target float The value the Boss' speed x component will slowly change to.
---@param time float The time the Boss will take to reach the target x speed.
---@param easer? string|Ease.Easer The easer to apply to the x speed value. If a string is provided, it will call helpers.getEaserByName. Defaults to nil.
---@default nil
---@param invert? boolean If the easer should be inverted. Defaults to false.
---@default false
---@return float time The time given from the Tween
function helpers.speedXTweenTo(target, time, easer, invert)
    return helpers.speedXTween(puppet.Speed.X, target, time, getEaser(easer, invert))
end

---Create a new Tween for the Boss' x speed from its current y speed value.
---@param target float The value the Boss' speed y component will slowly change to.
---@param time float The time the Boss will take to reach the target y speed.
---@param easer? string|Ease.Easer The easer to apply to the y speed value. If a string is provided, it will call helpers.getEaserByName. Defaults to nil.
---@default nil
---@param invert? boolean If the easer should be inverted. Defaults to false.
---@default false
---@return float time The time given from the Tween
function helpers.speedYTweenTo(target, time, easer, invert)
    return helpers.speedYTween(puppet.Speed.Y, target, time, getEaser(easer, invert))
end

---Create a new Tween for the Boss'  speed from its current x speed value.
---@param xTarget float The value the Boss' speed x component will slowly change to.
---@param yTarget float The value the Boss' speed y component will slowly change to.
---@param time float The time the Boss will take to reach the target x speed.
---@param easer? string|Ease.Easer The easer to apply to the x speed value. If a string is provided, it will call helpers.getEaserByName. Defaults to nil.
---@default nil
---@param invert? boolean If the easer should be inverted. Defaults to false.
---@default false
---@return float time The time given from the Tween
function helpers.speedTweenTo(xTarget, yTarget, time, easer, invert)
    return helpers.speedTween(puppet.Speed.X, puppet.Speed.Y, xTarget, yTarget, time, getEaser(easer, invert))
end

--#endregion

--#region Collisions and Colliders

--- Enable the Boss' Collision checks from other entities.
function helpers.enableCollisions()
    puppet.Collidable = true
end

--- Disable the Boss' Collision checks from other entities.
function helpers.disableCollisions()
    puppet.Collidable = false
end

--- Enable the Boss' Collision checks with solids.
function helpers.enableSolidCollisions()
    puppet.SolidCollidable = true
end

--- Disable the Boss' Collision checks with solids.
function helpers.disableSolidCollisions()
    puppet.SolidCollidable = false
end

---Set the Boss' hit cooldown to the given value
---@param value float The timer to set the cooldown to
function helpers.setHitCooldown(value)
    puppet.BossDamageCooldown.TimeLeft = value
end

---Set the Boss' hit cooldown back to the default value defined.
function helpers.resetHitCooldown()
    puppet.BossDamageCooldown:Reset()
end

---Change the Boss' hitboxes to those stored under the given tag.
---@param tag string The hitbox group tag to use.
function helpers.changeBaseHitboxTo(tag)
    puppet:ChangeHitboxOption(tag)
end

---Change the Boss' hurtboxes to those stored under the given tag.
---@param tag string The hurtbox group tag to use.
function helpers.changeHurtboxTo(tag)
    puppet:ChangeHurtboxOption(tag)
end

---Change the Boss' hurtboxes to those stored under the given tag.
---@param tag string The hurtbox group tag to use.
function helpers.changeKillColliderTo(tag)
    puppet:ChangeKillColliderOption(tag)
end

---Change the Boss' bouncebox to that stored under the given tag.
---@param tag string The bouncebox tag to use.
function helpers.changeBounceboxTo(tag)
    puppet:ChangeHitboxOption(tag)
end

---Change the Boss' Sidekick Target to that stored under the given tag.
---@param tag string The Sidekick Target tag to use.
function helpers.changeTargetTo(tag)
    puppet:ChangeHurtboxOption(tag)
end

---Create a new Rectangular Hitbox Collider
---@param width float The width of the collider.
---@param height float The height of the collider.
---@param x? float The x offset of the hitbox. Defaults to 0.
---@default 0
---@param y? float The y offest of the Hitbox. Defaults to 0.
---@default 0
---@return Hitbox hitbox The created Hitbox Collider
function helpers.getHitbox(width, height, x, y)
    return _monocle.Hitbox(width, height, x or 0, y or 0)
end

---Create a new Circle Collider
---@param radius float The radius of the collider.
---@param x? float The x offset of the hitbox. Defaults to 0.
---@default 0
---@param y? float The y offest of the Hitbox. Defaults to 0.
---@default 0
---@return Circle circle The created Hitbox Collider
function helpers.getCircle(radius, x, y)
    return _monocle.Circle(radius, x or 0, y or 0)
end

---Create a ColliderList object from the provided colliders.
---@param ... Collider All the colliders to combine into a ColliderList
---@return ColliderList colliderList The combined ColliderList object.
function helpers.getColliderList(...)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetColliderListFromLuaTable({...})
end

--#endregion

--#region Boss Components and Entities

---Add a component to the Boss.
---@param component Component The component to add.
function helpers.addComponentToBoss(component)
    puppet:Add(component)
end

---@param entity Entity
---@param player Player
local function killPlayer(entity, player)
    helpers.die(_monocle.Calc.SafeNormalize(player.Position - entity.Position))
end

---Returns an EntityChecker Component that will execute the second passed function when the first function's return value matches the state required.
---@param checker fun(): bool The function that will be called every frame to test its value.
---@param func? fun(entity: Entity) The function that will execute once the timer ends. Takes an entity parameter, which will be the Entity the component is added to. Defaults to the DestroyEntity function.
---@default helpers.destroyEntity
---@param state? boolean The state the checker function's return value must match. Defaults to true.
---@default true
---@param remove? boolean If the component should remove itself after it calls the func function. Defaults to true
---@default true
---@return EntityChecker checker The Entity Checker that can be added to any Entity.
function helpers.getEntityChecker(checker, func, state, remove)
    return bossesHelper.Code.Components.EntityChecker(checker, func or helpers.destroyEntity, state or state == nil, remove or remove == nil)
end

---Returns an EntityTimer Component that will execute the passed function when the timer ends.
---Can be added to any Entity.
---@param timer float The amount of time that must pass for the timer to execute.
---@param func? fun(entity: Entity) The function that will execute once the timer ends. Takes an entity parameter, which will be the Entity the component is added to. Defaults to the DestroyEntity function.
---@default helpers.destroyEntity
---@return EntityTimer timer The Entity Timer that can be added to any Entity.
function helpers.getEntityTimer(timer, func)
    return bossesHelper.Code.Components.EntityTimer(timer, func or helpers.destroyEntity)
end

---Returns an EntityFlagger Component that will execute the passed function when the given session flag's state matches the required state.
---Can be added to any Entity.
---@param flag string The session flag the entity will use to activate its function.
---@param func? fun(entity: Entity) The function that will execute once the session flag state is the same as the state parameter. Takes an entity parameter, which will the Entity the component is added to. Defaults to the destroyEntity function.
---@default helpers.destroyEntity
---@param state? boolean The state the flag must match to activate the passed function. Defaults to true.
---@default true
---@param resetFlag? boolean If the flag should return to its previous state once used by the Flagger. Defaults to true
---@default true
---@return EntityFlagger flagger The Entity Flagger that can be added to any Entity.
function helpers.getEntityFlagger(flag, func, state, resetFlag)
    return bossesHelper.Code.Components.EntityFlagger(flag, func or helpers.destroyEntity, state or state == nil, resetFlag or resetFlag == nil)
end

---Returns an EntityChain component that will keep another entity's position chained to the Entity this component is added to.
---@param entity Entity The entity to chain, whose position will change as the base Entity moves.
---@param startChained? boolean Whether the entity should start chained immediately. Defaults to true.
---@default true
---@param remove? boolean Whether the chained entity should be removed if the chain component is also removed.
---@default false
---@return EntityChain chain The Entity Chain component that can be added to any Entity.
function helpers.getEntityChain(entity, startChained, remove)
    return bossesHelper.Code.Components.EntityChain(entity, startChained or startChained == nil, remove or false)
end

---Take the function and create an IEnumerator that can execute it.
---@param func function
---@return IEnumerator
function helpers.funcToIEnumerator(func)
    return celesteMod.BossesHelper.Code.Helpers.Lua.LuaProxyCoroutine(func)
end

---Take the function and create a Coroutine Component that will run it.
---@param func function The function to create a coroutine of.
---@return Coroutine # The Coroutine Component
function helpers.getLuaCoroutine(func)
    return monocle.Coroutine(helpers.funcToIEnumerator(func))
end

---Creates a new EntityCollider component.
---@param entityType Entity|string|Type The type of Entity this component will try to collide with.
---@param func fun(entity: Entity) The function to call on collision.
---@param collider? Collider The collider to use. nil if it should use the entity the component is added to's collider.
---@return EntityCollider # The new EntityCollider component.
function helpers.getEntityCollider(entityType, func, collider)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetEntityCollider(entityType, func, collider)
end

---Creates a new EntityColliderByComponent component.
---@param entityType Component|string|Type The type of Component this component will try to collide with.
---@param func fun(entity: Component) The function to call on collision.
---@param collider? Collider The collider to use. nil if it should use the entity the component is added to's collider.
---@return EntityColliderByComponent # The new EntityColliderByComponent component.
function helpers.getEntityColliderByComponent(entityType, func, collider)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetEntityColliderByComponent(entityType, func, collider)
end

---Create and return a basic entity to use in attacks.
---@param position Vector2 The position the entity will be at.
---@param hitboxes Collider The collider the entity will use.
---@param spriteName? string The sprite the entity will use.
---@param startCollidable? boolean If the entity should spawn with collisions active. Defaults to true.
---@default true
---@param funcOnPlayer? fun(self, player) The function that will be called when the entity "self" collides with the Player. Defaults to killing the Player.
---@default killPlayer
---@param xScale? float The horizontal sprite scale. Defaults to 1.
---@default 1
---@param yScale? float The vertical sprite scale. Defaults to 1.
---@default 1
---@return AttackEntity
function helpers.getNewBasicAttackEntity(position, hitboxes, spriteName, startCollidable, funcOnPlayer, xScale, yScale)
    return celesteMod.BossesHelper.Code.Entities.AttackEntity(position, hitboxes, funcOnPlayer or killPlayer, startCollidable or startCollidable==nil, spriteName or '', xScale or 1, yScale or 1)
end

---Create and return a basic entity to use in attacks.
---@param position Vector2 The position the entity will be at.
---@param hitboxes Collider The collider the entity will use.
---@param spriteName? string The sprite the entity will use.
---@param gravMult? float The multiplier to the Gravity constant the Actor should use. Defaults to 1.
---@default 1
---@param maxFall? float The fastest the Boss will fall naturally due to gravity. Defaults to 90.
---@default 90
---@param startCollidable? boolean If the entity should spawn with collisions active. Defaults to true.
---@default true
---@param startSolidCollidable? boolean If the entity should spawn with solid collisions active. Defaults to true.
---@default true
---@param funcOnPlayer? fun(self, player) The function that will be called when the entity "self" collides with the Player. Defaults to killing the Player.
---@default killPlayer
---@param xScale? float The horizontal sprite scale. Defaults to 1.
---@default 1
---@param yScale? float The vertical sprite scale. Defaults to 1.
---@default 1
---@return AttackActor
function helpers.getNewBasicAttackActor(position, hitboxes, spriteName, gravMult, maxFall, startCollidable, startSolidCollidable, funcOnPlayer,  xScale, yScale)
    return celesteMod.BossesHelper.Code.Entities.AttackActor(position, hitboxes, funcOnPlayer or killPlayer, startCollidable or startCollidable==nil,
        startSolidCollidable or startSolidCollidable == nil, spriteName or '', gravMult or 1, maxFall or 90, xScale or 1, yScale or 1)
end

---Wrap a function in another function to call the inner one with parameters but the outer one without.
---@param func function The function to wrap.
---@param ... any The parameters to call func with.
---@return function function Function that will wrap the passed function with the arguements passed.
local function callFunc(func, ...)
    local args = {...}
    if select("#", ...) < 1 then
        return func
    end
    return function ()
        return func(table.unpack(args))
    end
end

---Add a function that will run in the background.
---@param func fun(...) The function that will run in the background. Will run to completion or loop as defined.
---@param ... any Parameters to pass to the wrapped function, if any
function helpers.addConstantBackgroundCoroutine(func, ...)
    puppet:Add(helpers.getLuaCoroutine(callFunc(func, {...})))
end

--#endregion

--#region Component Retreival

--- Gets all tracked components by class name.
--- @param name string Class name of the component, relative to "Celeste." by default.
--- @param prefix? string Overrides the global class name prefix.
--- @return Component[] components Tracked components of given class.
function helpers.getComponents(name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetComponents(name, prefix or classNamePrefix)
end

--- Gets the first tracked component by class name.
--- @param name string Class name of the component, relative to "Celeste." by default.
--- @param prefix? string Overrides the global class name prefix.
--- @return Component component First tracked component of given class.
function helpers.getComponent(name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetComponent(name, prefix or classNamePrefix)
end

--- Gets all components by class name.
--- @param name string Class name of the component, relative to "Celeste." by default.
--- @param prefix? string Overrides the global class name prefix.
--- @return Component[] components All components of given class on scene.
function helpers.getAllComponents(name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetAllComponents(name, prefix or classNamePrefix)
end

--- Gets the first component by class name.
--- @param name string Class name of the component, relative to "Celeste." by default.
--- @param prefix? string Overrides the global class name prefix.
--- @return Component component First component of given class.
function helpers.getFirstComponent(name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetFirstComponent(name, prefix or classNamePrefix)
end

--- Gets all components by class name added to an entity of given class name.
--- @param name string Class name of the component, relative to "Celeste." by default.
--- @param entity string Class name of the entity, relative to "Celeste." by default.
--- @param prefix? string Overrides the global class name prefix for the Component class.
--- @param entityPre? string Overrides the global class name prefix for the Entity class.
--- @return Component[] components All components of given class on scene attached to the entity type.
function helpers.getAllComponentsOnType(name, entity, prefix, entityPre)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetAllComponentsOnType(name, entity, prefix or classNamePrefix, entityPre or classNamePrefix)
end

--- Gets the first component by class name added to an entity of the given class name.
--- @param name string Class name of the component, relative to "Celeste." by default.
--- @param entity string Class name of the entity, relative to "Celeste." by default.
--- @param prefix? string Overrides the global class name prefix for the Component class.
--- @param entityPre? string Overrides the global class name prefix for the Entity class.
--- @return Component component First component of given class attached to the entity type.
function helpers.getFirstComponentOnType(name, entity, prefix, entityPre)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetFirstComponentOnType(name, entity, prefix or classNamePrefix, entityPre or classNamePrefix)
end

--- Returns all the components of the given class name from the entity given, if any.
--- @param entity Entity The entity to check.
--- @param name string Class name of the component, relative to "Celeste." by default.
--- @param prefix? string Overrides the global class name prefix.
--- @return Component[] components All components of given class on scene sored on the entity, if any.
function helpers.getComponentsFromEntity(entity, name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetComponentsFromEntity(entity, name, prefix or classNamePrefix)
end

--- Returns the component of the given class name from the entity given, if any.
--- @param entity Entity The entity to check.
--- @param name string Class name of the component, relative to "Celeste." by default.
--- @param prefix? string Overrides the global class name prefix.
--- @return Component component First component of given class stored on the entity, if any.
function helpers.getComponentFromEntity(entity, name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.GetComponentFromEntity(entity, name, prefix or classNamePrefix)
end

--- Checks if the entity given has a component of the given class name.
--- @param entity Entity The entity to check.
--- @param name string Class name of the component, relative to "Celeste." by default.
--- @param prefix? string Overrides the global class name prefix.
--- @return boolean componentFound If the Entity does have a Component of the type specified.
function helpers.entityHasComponent(entity, name, prefix)
    return bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.EntityHasComponent(entity, name, prefix or classNamePrefix)
end

--#endregion

--#region Health System

---Get the Player's current health value on the active Health System
---@return int health The player's health value, or -1 if there's no active Health System
function helpers.getPlayerHealth()
    return bossesHelper.BossesHelperModule.PlayerHealth
end

---Gives additional time where the player is invincible to taking damage.
---@param time float The time to add to the invincible timer.
function helpers.giveInvincibleFrames(time)
    bossesHelper.BossesHelperModule.GiveIFrames(time)
end

--#endregion

--#region Delegates

---These methods allow transforming a Lua function into C# delegates the API can then use correctly.

---Transform a Ease.Easer function into its C# counterpart.
---@param func L_Ease.Easer
---@return Ease.Easer
function helpers.funcToEaser(func)
    return bossesHelper.Code.Helpers.Lua.LuaDelegates.ToEaser(func)
end

---Transform a Ease.Easer function into its C# counterpart.
---@param func L_Collision
---@return Collision
function helpers.funcToCollision(func)
    return bossesHelper.Code.Helpers.Lua.LuaDelegates.ToCollision(func)
end

---Transform a Ease.Easer function into its C# counterpart.
---@param func L_DashCollision
---@return DashCollision
function helpers.funcToDashCollision(func)
    return bossesHelper.Code.Helpers.Lua.LuaDelegates.ToDashCollision(func)
end

--#endregion

--#region Misc. Functions

---Display textbox with dialog. Any provided functions will be passed as Triggers accessible to Dialog.txt triggers.
---@param dialog string Dialog ID used for the conversation.
---@param ... function Functions that will be called whenever a trigger is activated through dialogue.
---@return IEnumerator
function helpers.sayExt(dialog, ...)
    return wait(bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.Say(tostring(dialog), {...}))
end

---Creates a new SoundSource and adds it to the provided entity, starting the sound immediately
---@param event string The name of the Event to play.
---@param entity Entity The entity to add the SoundSource to.
---@return Component newSound The newly created SoundSource
function helpers.addSoundTo(event, entity)
    local newSound = _celeste.SoundSource()
    entity:Add(newSound:Play(event))
    return newSound
end

---Creates a new SoundSource and adds it to the Boss, starting the sound immediately
---@param event string The name of the Event to play.
---@return Component newSound The newly created SoundSource attached to the Boss
function helpers.addSoundToBoss(event)
    return helpers.addSoundTo(event, puppet)
end

---Normalizes the vector provided to the given length or 1.
---@param vector Vector2 The vector to normalize
---@param length? float The new length of the vector or 1
---@default 1
---@return Vector2 normal The normalized vector2
function helpers.normalize(vector, length)
    return _monocle.Calc.SafeNormalize(vector, length or 1)
end

---Get a new EntityData object
---@param position Vector2 The vector2 position the entityData will hold.
---@param width? float The width the EntityData will hold. Defaults to 0.
---@default 0
---@param height? float The height the EntityData will hold. Defaults to 0.
---@default 0
---@param id? int The id the EntityData will hold. Defaults to 1000.
---@default 1000
---@return EntityData entityData The formed EntityData object with the Values dictionary initialized empty.
function helpers.getNewEntityData(position, width, height, id)
    newData = celesteMod.BossesHelper.BossesHelperModule.MakeEntityData()
    newData.ID = id or 1000
    newData.Level = engine.Scene
    newData.Position = position or vector2(0,0)
    newData.Width = width or 0
    newData.Height = height or 0
    return newData
end

---Set a list of attributes to the provided EntityData object's Values dictionary.
---@param entityData EntityData The EntityData to update.
---@param attributes table<string,any> The list of attributes to add.
function helpers.setEntityDataAttributes(entityData, attributes)
    for k,v in pairs(attributes) do
        entityData.Values:Add(k,v)
    end
end

---Store any object within the Boss under a specific key, to be retreived later.
---@param key string The key to store the object with. Must be unique, or the object will not be stored.
---@param object any The object to store
function helpers.storeObjectInBoss(key, object)
    boss:StoreObject(key, object)
end

---Return an item that was stored within the Boss by key.
---@param key string The key the object is stored under.
---@return nil|any object The object stored, or nil if key is not found.
function helpers.getStoredObjectFromBoss(key)
    return boss:GetStoredObject(key)
end

---Remove the object stored under the specified key from the Boss' stored objects.
---@param key string The key the object is stored under.
function helpers.deleteStoredObjectFromBoss(key)
    boss:DeleteStoredObject(key)
end

---Set a method that will execute after a given delay.
---@param func fun() The function to execute. Takes no parameters.
---@param delay float The time in seconds the function will be called after.
function helpers.doMethodAfterDelay(func, delay)
    bossesHelper.Code.Helpers.Lua.LuaMethodWrappers.DoMethodAfterDelay(func, delay)
end

---Wait While the given predicate is true.
---@param predicate fun(): bool The predicate to evaluate each frame.
function helpers.waitWhile(predicate)
    while predicate() do
        wait()
    end
end

---A specific Easer can be obtained by calling "monocle.Ease.{name}" which returns the desired Easer.
---@param name? string The name of the Easer to get.
---@param invert? boolean If the easer returned should be inverted.
---@return nil|Ease.Easer easer The Easer found or nil if not found.
function helpers.getEaserByName(name, invert)
    local typ = type(name)
    if (typ ~= "string") then
        return nil
    end
    local value = string.lower(name)
    local choice = ease[value]
    if choice then
        if invert or false then
            return ease.Invert(choice)
        end
        return choice
    else
        return nil
    end
end

--#endregion

--#region Deprecated

---@deprecated Use Vector2:Length instead
---Get the length of the provided vector2
---@param vector Vector2 Vector to get length of
---@return float length The length of the vector2
function helpers.v2L(vector)
    return vector:Length()
end

--#endregion

--#endregion

return helpers
