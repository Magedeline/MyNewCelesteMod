local tape = {}

tape.name = "Ingeste/Tape"
tape.depth = -1000000
tape.nodeLineRenderType = "line"
tape.nodeLimits = {0, 4}  -- Extended for C-side features

tape.placements = {
    {
        name = "Tape",
        data = {
            onCollect = "",
            customAudio = "",
            particleColor = "FF9CCF",
            glowStrength = 1.0,
            bloomStrength = 0.8,
            wiggleIntensity = 0.35,
            floatSpeed = 2.0,
            floatRange = 2.0,
            collectDelay = 0.3,
            persistent = true,
            -- C-side specific properties
            cSideUnlock = true,
            cSideID = "",
            unlockMessage = "C-Side Unlocked!",
            requiredCSides = 0,
            bonusCollectible = false,
            bonusType = "heart",
            bonusValue = 1
        }
    }
}

tape.fieldInformation = {
    onCollect = {
        fieldType = "string",
        options = {
            "",
            "unlock_door",
            "trigger_cutscene", 
            "activate_switch",
            "spawn_entity",
            "teleport_player",
            "change_music",
            "show_dialogue",
            "set_flag",
            "complete_level",
            "unlock_c_side",
            "custom_script"
        },
        editable = true
    },
    customAudio = {
        fieldType = "string"
    },
    particleColor = {
        fieldType = "string"
    },
    glowStrength = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 2.0
    },
    bloomStrength = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 2.0
    },
    wiggleIntensity = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 1.0
    },
    floatSpeed = {
        fieldType = "number",
        minimumValue = 0.1,
        maximumValue = 10.0
    },
    floatRange = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 10.0
    },
    collectDelay = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 2.0
    },
    persistent = {
        fieldType = "boolean"
    },
    -- C-side specific field information
    cSideUnlock = {
        fieldType = "boolean"
    },
    cSideID = {
        fieldType = "string"
    },
    unlockMessage = {
        fieldType = "string"
    },
    requiredCSides = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 50
    },
    bonusCollectible = {
        fieldType = "boolean"
    },
    bonusType = {
        fieldType = "string",
        options = {
            "heart",
            "strawberry",
            "key",
            "gem",
            "star",
            "crystal"
        },
        editable = true
    },
    bonusValue = {
        fieldType = "integer",
        minimumValue = 1,
        maximumValue = 10
    }
}

function tape.sprite(room, entity)
    local color = {1.0, 1.0, 1.0, 1.0}
    
    -- Parse hex color if provided
    if entity.particleColor and entity.particleColor ~= "" then
        local hex = entity.particleColor:gsub("#", "")
        if #hex == 6 then
            local r = tonumber(hex:sub(1, 2), 16) / 255
            local g = tonumber(hex:sub(3, 4), 16) / 255
            local b = tonumber(hex:sub(5, 6), 16) / 255
            color = {r, g, b, 1.0}
        end
    else
        -- Default tape glow color (FF9CCF - pinkish)
        color = {255/255, 156/255, 207/255, 1.0}
    end
    
    local sprites = {
        {
            texture = "collectables/tape/idle00",
            x = entity.x or 0,
            y = entity.y or 0,
            color = color
        }
    }
    
    -- Add C-side indicator if enabled
    if entity.cSideUnlock then
        table.insert(sprites, {
            texture = "util/pixel",
            x = (entity.x or 0) + 6,
            y = (entity.y or 0) - 6,
            width = 4,
            height = 4,
            color = {1.0, 1.0, 0.0, 0.8}  -- Yellow indicator for C-side
        })
    end
    
    -- Add bonus collectible indicator
    if entity.bonusCollectible then
        local bonusColor = {0.0, 1.0, 0.0, 0.6}  -- Green for bonus
        if entity.bonusType == "heart" then
            bonusColor = {1.0, 0.2, 0.2, 0.6}  -- Red for heart
        elseif entity.bonusType == "strawberry" then
            bonusColor = {1.0, 0.4, 0.4, 0.6}  -- Pink for strawberry
        elseif entity.bonusType == "key" then
            bonusColor = {1.0, 1.0, 0.0, 0.6}  -- Yellow for key
        elseif entity.bonusType == "gem" then
            bonusColor = {0.4, 0.4, 1.0, 0.6}  -- Blue for gem
        elseif entity.bonusType == "star" then
            bonusColor = {1.0, 1.0, 1.0, 0.6}  -- White for star
        elseif entity.bonusType == "crystal" then
            bonusColor = {0.8, 0.0, 1.0, 0.6}  -- Purple for crystal
        end
        
        table.insert(sprites, {
            texture = "util/pixel",
            x = (entity.x or 0) - 6,
            y = (entity.y or 0) - 6,
            width = 4,
            height = 4,
            color = bonusColor
        })
    end
    
    return sprites
end

function tape.nodeSprite(room, entity, node, nodeIndex)
    local x = node.x or 0
    local y = node.y or 0
    
    if nodeIndex == 1 then
        -- First node: target location for teleport or spawn
        return {
            {
                texture = "util/pixel",
                x = x - 4,
                y = y - 4,
                width = 8,
                height = 8,
                color = {1.0, 0.8, 0.2, 0.7}
            }
        }
    elseif nodeIndex == 2 then
        -- Second node: secondary target or activation point
        return {
            {
                texture = "util/pixel",
                x = x - 4,
                y = y - 4,
                width = 8,
                height = 8,
                color = {0.2, 0.8, 1.0, 0.7}
            }
        }
    elseif nodeIndex == 3 then
        -- Third node: tertiary target or effect area
        return {
            {
                texture = "util/pixel",
                x = x - 4,
                y = y - 4,
                width = 8,
                height = 8,
                color = {0.8, 0.2, 1.0, 0.7}
            }
        }
    elseif nodeIndex == 4 then
        -- Fourth node: C-side specific target or bonus collectible spawn
        return {
            {
                texture = "util/pixel",
                x = x - 4,
                y = y - 4,
                width = 8,
                height = 8,
                color = {1.0, 1.0, 0.0, 0.7}  -- Yellow for C-side related
            }
        }
    end
    
    return {}
end

-- Helper function to get action description for the editor
function tape.getActionDescription(action)
    local descriptions = {
        unlock_door = "Unlocks doors with matching ID",
        trigger_cutscene = "Starts a cutscene sequence",
        activate_switch = "Activates switches or mechanisms",
        spawn_entity = "Spawns a new entity at collection point",
        teleport_player = "Teleports player to specified location",
        change_music = "Changes background music",
        show_dialogue = "Displays dialogue text",
        set_flag = "Sets a session flag",
        complete_level = "Completes the current level",
        unlock_c_side = "Unlocks C-Side level progression",
        custom_script = "Runs custom Lua script"
    }
    return descriptions[action] or "Custom collection behavior"
end

function tape.selection(room, entity)
    local x = entity.x or 0
    local y = entity.y or 0
    return {x - 8, y - 8, 16, 16}
end

function tape.nodeSelection(room, entity, node)
    local x = node.x or 0
    local y = node.y or 0
    return {x - 8, y - 8, 16, 16}
end

return tape