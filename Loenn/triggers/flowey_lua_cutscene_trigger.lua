-- Flowey Lua Cutscene Trigger
-- Advanced Flowey cutscene trigger with Lua scripting support and dynamic personality system

local floweyLuaCutsceneTrigger = {}

floweyLuaCutsceneTrigger.name = "Ingeste/FloweyLuaCutsceneTrigger"

floweyLuaCutsceneTrigger.placements = {
    {
        name = "flowey_cutscene_intro",
        data = {
            width = 32,
            height = 32,
            cutsceneId = "flowey_intro",
            enableLua = true,
            personalityOverride = "",
            debugMode = false,
            triggerOnce = true,
            playerOnly = true
        }
    }
}

floweyLuaCutsceneTrigger.fieldInformation = {
    cutsceneId = {
        fieldType = "string",
        options = {
            "flowey_intro",
            "flowey_encounter", 
            "flowey_manipulation",
            "flowey_ending",
            "flowey_boss_pre",
            "flowey_boss_post",
            "flowey_final"
        }
    },
    enableLua = {
        fieldType = "boolean"
    },
    personalityOverride = {
        fieldType = "string",
        options = {
            "",
            "friendly",
            "neutral",
            "condescending", 
            "mocking",
            "sinister",
            "aggressive",
            "manipulative"
        }
    },
    debugMode = {
        fieldType = "boolean"
    },
    triggerOnce = {
        fieldType = "boolean"
    },
    playerOnly = {
        fieldType = "boolean"
    }
}

function floweyLuaCutsceneTrigger.sprite(room, entity)
    local width = entity.width or 32
    local height = entity.height or 32
    
    -- Different colors based on cutscene type and personality
    local color = {0.9, 0.8, 0.2, 0.8} -- Default golden Flowey color
    
    if entity.cutsceneId == "flowey_intro" then
        color = {0.9, 0.8, 0.2, 0.8} -- Golden
    elseif entity.cutsceneId == "flowey_encounter" then
        color = {0.8, 0.3, 0.3, 0.8} -- Red
    elseif entity.cutsceneId == "flowey_manipulation" then
        color = {0.6, 0.2, 0.8, 0.8} -- Purple
    elseif entity.cutsceneId == "flowey_ending" then
        color = {1.0, 0.5, 0.0, 0.8} -- Orange
    elseif entity.cutsceneId == "flowey_boss_pre" or entity.cutsceneId == "flowey_boss_post" then
        color = {0.9, 0.0, 0.0, 0.9} -- Intense red
    elseif entity.cutsceneId == "flowey_final" then
        color = {1.0, 1.0, 1.0, 0.9} -- White
    end
    
    -- Personality override colors
    if entity.personalityOverride == "aggressive" then
        color = {0.9, 0.0, 0.0, 0.9}
    elseif entity.personalityOverride == "manipulative" then
        color = {0.6, 0.2, 0.8, 0.8}
    elseif entity.personalityOverride == "sinister" then
        color = {0.3, 0.0, 0.3, 0.9}
    elseif entity.personalityOverride == "mocking" then
        color = {0.9, 0.6, 0.0, 0.8}
    elseif entity.personalityOverride == "condescending" then
        color = {0.7, 0.7, 0.2, 0.8}
    end
    
    -- Debug mode indicator
    if entity.debugMode then
        color[4] = 1.0 -- Full opacity for debug
    end
    
    return {
        {
            texture = "ahorn/entityTrigger",
            x = entity.x,
            y = entity.y,
            scaleX = width / 8,
            scaleY = height / 8,
            color = color
        }
    }
end

function floweyLuaCutsceneTrigger.selection(room, entity)
    local width = entity.width or 32
    local height = entity.height or 32
    return {entity.x, entity.y, width, height}
end

return floweyLuaCutsceneTrigger