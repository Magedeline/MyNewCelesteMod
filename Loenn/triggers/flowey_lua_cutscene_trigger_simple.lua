local floweyLuaCutsceneTriggerSimple = {}

floweyLuaCutsceneTriggerSimple.name = "Ingeste/FloweyLuaCutsceneTriggerSimple"

floweyLuaCutsceneTriggerSimple.placements = {
    {
        name = "flowey_cutscene",
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

floweyLuaCutsceneTriggerSimple.fieldInformation = {
    cutsceneId = {
        fieldType = "string"
    },
    enableLua = {
        fieldType = "boolean"
    },
    personalityOverride = {
        fieldType = "string"
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

function floweyLuaCutsceneTriggerSimple.sprite(room, entity)
    local width = entity.width or 32
    local height = entity.height or 32
    
    return {
        {
            texture = "ahorn/entityTrigger",
            x = entity.x,
            y = entity.y,
            scaleX = width / 8,
            scaleY = height / 8,
            color = {0.9, 0.8, 0.2, 0.8}
        }
    }
end

function floweyLuaCutsceneTriggerSimple.selection(room, entity)
    local width = entity.width or 32
    local height = entity.height or 32
    return {entity.x, entity.y, width, height}
end

return floweyLuaCutsceneTriggerSimple