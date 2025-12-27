-- Chapter 16 Cutscene Trigger
-- Handles all Chapter 16 dialog sequences and boss cutscenes

local chapter16CutsceneTrigger = {}

chapter16CutsceneTrigger.name = "Ingeste/Chapter16CutsceneTrigger"

chapter16CutsceneTrigger.placements = {
    {
        name = "corrupted_intro",
        data = {
            width = 64,
            height = 32,
            cutsceneId = "ch16_corrupted_intro",
            dialogKey = "CH16_CORRUPTED_REALITY_INTRO",
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            enableEffects = true,
            corruptionLevel = 5.0,
            enableTentacles = true,
            madnessLevel = 3
        }
    },
    {
        name = "first_phase_battle",
        data = {
            width = 32,
            height = 32,
            cutsceneId = "ch16_first_phase_battle",
            dialogKey = "CH16_FIRST_PHASE_BATTLE_0",
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            enableEffects = true,
            corruptionLevel = 7.0,
            enableTentacles = false,
            madnessLevel = 4
        }
    },
    {
        name = "lost_souls",
        data = {
            width = 32,
            height = 32,
            cutsceneId = "ch16_lost_souls",
            dialogKey = "CH16_LOST_SOULS_1",
            triggerOnce = false,
            playerOnly = true,
            autoStart = false,
            enableEffects = true,
            corruptionLevel = 3.0,
            enableTentacles = false,
            madnessLevel = 1,
            soulIndex = 1
        }
    },
    {
        name = "finale_sequence",
        data = {
            width = 48,
            height = 48,
            cutsceneId = "ch16_finale",
            dialogKey = "CH16_MY_FINALE",
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            enableEffects = true,
            corruptionLevel = 10.0,
            enableTentacles = true,
            madnessLevel = 5
        }
    },
    {
        name = "call_for_help",
        data = {
            width = 40,
            height = 32,
            cutsceneId = "ch16_call_help",
            dialogKey = "CH16_YOU_CALL_HELP",
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            enableEffects = true,
            corruptionLevel = 8.0,
            enableTentacles = false,
            madnessLevel = 4,
            enableSoulRestore = true
        }
    },
    {
        name = "final_defeat",
        data = {
            width = 64,
            height = 32,
            cutsceneId = "ch16_final_defeat",
            dialogKey = "CH16_FINAL_DEFEAT",
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            enableEffects = true,
            corruptionLevel = 9.0,
            enableTentacles = false,
            madnessLevel = 5
        }
    },
    {
        name = "barrier_breaks",
        data = {
            width = 80,
            height = 32,
            cutsceneId = "ch16_barrier_breaks",
            dialogKey = "CH16_BARRIER_BREAKS",
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            enableEffects = true,
            corruptionLevel = 1.0,
            enableTentacles = false,
            madnessLevel = 0,
            enableHealing = true
        }
    },
    {
        name = "return_home",
        data = {
            width = 64,
            height = 32,
            cutsceneId = "ch16_return_home",
            dialogKey = "CH16_RETURN_TO_CELESTE",
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            enableEffects = true,
            corruptionLevel = 0.0,
            enableTentacles = false,
            madnessLevel = 0,
            enableHealing = true,
            peaceful = true
        }
    }
}

chapter16CutsceneTrigger.fieldInformation = {
    cutsceneId = {
        fieldType = "string",
        options = {
            "ch16_corrupted_intro",
            "ch16_flowey_transform", 
            "ch16_organic_garden",
            "ch16_bone_cathedral",
            "ch16_flesh_labyrinth",
            "ch16_weapon_arsenal",
            "ch16_spawn_souls",
            "ch16_first_phase_battle",
            "ch16_call_help",
            "ch16_finale",
            "ch16_final_defeat",
            "ch16_barrier_breaks",
            "ch16_farewell_titan",
            "ch16_return_home",
            "ch16_lost_souls",
            "ch16_soul_patience",
            "ch16_soul_bravery", 
            "ch16_soul_integrity",
            "ch16_soul_perseverance",
            "ch16_soul_kindness",
            "ch16_soul_justice",
            "ch16_reality_distort",
            "ch16_nightmare_aura",
            "ch16_soul_liberation",
            "ch16_asriel_reveal"
        }
    },
    dialogKey = {
        fieldType = "string",
        options = {
            "CH16_CORRUPTED_REALITY_INTRO",
            "CH16_FIRST_PHASE_BATTLE_0",
            "CH16_FIRST_PHASE_BATTLE_1",
            "CH16_LOST_SOULS_1",
            "CH16_LOST_SOULS_2",
            "CH16_LOST_SOULS_3",
            "CH16_LOST_SOULS_4",
            "CH16_LOST_SOULS_5",
            "CH16_LOST_SOULS_6",
            "CH16_LOST_SOULS_7",
            "CH16_LOST_SOULS_8",
            "CH16_MY_FINALE",
            "CH16_MY_FINALE_FLOWEY_DEFENSE_DROP_TO_ZERO",
            "CH16_FINAL_BLOW",
            "CH16_TAKE_THE_L",
            "CH16_YOU_CALL_HELP",
            "CH16_FINAL_DEFEAT",
            "CH16_BARRIER_BREAKS",
            "CH16_FAREWELL_TITAN_KING",
            "CH16_RETURN_TO_CELESTE"
        }
    },
    triggerOnce = {
        fieldType = "boolean"
    },
    playerOnly = {
        fieldType = "boolean"
    },
    autoStart = {
        fieldType = "boolean"
    },
    enableEffects = {
        fieldType = "boolean"
    },
    corruptionLevel = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 10.0
    },
    enableTentacles = {
        fieldType = "boolean"
    },
    madnessLevel = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 5
    },
    soulIndex = {
        fieldType = "integer",
        minimumValue = 1,
        maximumValue = 8
    },
    enableSoulRestore = {
        fieldType = "boolean"
    },
    enableHealing = {
        fieldType = "boolean"
    },
    peaceful = {
        fieldType = "boolean"
    }
}

function chapter16CutsceneTrigger.sprite(room, entity)
    local cutsceneId = entity.cutsceneId or "ch16_corrupted_intro"
    local corruptionLevel = entity.corruptionLevel or 5.0
    
    -- Different sprites based on cutscene type
    if cutsceneId:find("corrupted") then
        return "objects/DesoloZantas/cutscene_trigger_corrupted"
    elseif cutsceneId:find("soul") or cutsceneId:find("finale") then
        return "objects/DesoloZantas/cutscene_trigger_soul"
    elseif cutsceneId:find("battle") or cutsceneId:find("defeat") then
        return "objects/DesoloZantas/cutscene_trigger_battle"
    elseif cutsceneId:find("barrier") or cutsceneId:find("return") then
        return "objects/DesoloZantas/cutscene_trigger_peaceful"
    else
        return "objects/DesoloZantas/cutscene_trigger_default"
    end
end

function chapter16CutsceneTrigger.selection(room, entity)
    return utils.rectangle(entity.x, entity.y, entity.width or 32, entity.height or 32)
end

-- Color coding for different types of cutscenes
local cutsceneColors = {
    corrupted = {0.8, 0.2, 0.8, 0.7},  -- Purple for corruption
    battle = {0.8, 0.2, 0.2, 0.7},     -- Red for battle
    soul = {0.9, 0.9, 0.2, 0.7},       -- Yellow for souls
    peaceful = {0.2, 0.8, 0.2, 0.7},   -- Green for peaceful
    default = {0.5, 0.5, 0.8, 0.7}     -- Blue for default
}

function chapter16CutsceneTrigger.rectangle(room, entity)
    local cutsceneId = entity.cutsceneId or "ch16_corrupted_intro"
    local width = entity.width or 32
    local height = entity.height or 32
    
    local colorType = "default"
    if cutsceneId:find("corrupted") then
        colorType = "corrupted"
    elseif cutsceneId:find("soul") or cutsceneId:find("finale") then
        colorType = "soul"
    elseif cutsceneId:find("battle") or cutsceneId:find("defeat") then
        colorType = "battle"
    elseif cutsceneId:find("barrier") or cutsceneId:find("return") then
        colorType = "peaceful"
    end
    
    local color = cutsceneColors[colorType]
    
    -- Add visual intensity based on corruption level
    local corruptionLevel = entity.corruptionLevel or 5.0
    local intensity = corruptionLevel / 10.0
    color = {color[1] * intensity, color[2] * intensity, color[3] * intensity, color[4]}
    
    return utils.rectangle(entity.x, entity.y, width, height), color
end

-- Node support for complex cutscenes with multiple points of interest
function chapter16CutsceneTrigger.nodeSprite(room, entity, node, nodeIndex)
    local cutsceneId = entity.cutsceneId or "ch16_corrupted_intro"
    
    if cutsceneId:find("soul") then
        return "objects/DesoloZantas/soul_point"
    elseif cutsceneId:find("tentacle") or entity.enableTentacles then
        return "objects/DesoloZantas/tentacle_point"
    else
        return "objects/DesoloZantas/cutscene_point"
    end
end

function chapter16CutsceneTrigger.nodeSelection(room, entity, node)
    return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

function chapter16CutsceneTrigger.nodeLimits()
    return {0, 10} -- Up to 10 special points per cutscene
end

return chapter16CutsceneTrigger