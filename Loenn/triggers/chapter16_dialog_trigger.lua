-- Chapter 16 Dialog Trigger
-- Specialized trigger for Chapter 16 dialog sequences with character state management

local chapter16DialogTrigger = {}

chapter16DialogTrigger.name = "Ingeste/Chapter16DialogTrigger"

chapter16DialogTrigger.placements = {
    {
        name = "corrupted_intro",
        data = {
            width = 64,
            height = 32,
            dialogKey = "CH16_CORRUPTED_REALITY_INTRO",
            characterState = "madeline_distracted",
            enableCharacterStates = true,
            enablePortraitChanges = true,
            enableEffects = true,
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            madnessLevel = 3,
            enableTentacles = true
        }
    },
    {
        name = "first_battle",
        data = {
            width = 48,
            height = 32,
            dialogKey = "CH16_FIRST_PHASE_BATTLE_0",
            characterState = "flowey_laughing",
            enableCharacterStates = true,
            enablePortraitChanges = true,
            enableEffects = true,
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            madnessLevel = 4,
            enableTentacles = false
        }
    },
    {
        name = "lost_soul_1",
        data = {
            width = 32,
            height = 24,
            dialogKey = "CH16_LOST_SOULS_1",
            characterState = "lost_soul_none",
            enableCharacterStates = true,
            enablePortraitChanges = false,
            enableEffects = true,
            triggerOnce = false,
            playerOnly = true,
            autoStart = false,
            madnessLevel = 1,
            enableTentacles = false,
            soulType = "human",
            soulIndex = 1
        }
    },
    {
        name = "lost_soul_2",
        data = {
            width = 32,
            height = 24,
            dialogKey = "CH16_LOST_SOULS_2",
            characterState = "lost_soul_none",
            enableCharacterStates = true,
            enablePortraitChanges = false,
            enableEffects = true,
            triggerOnce = false,
            playerOnly = true,
            autoStart = false,
            madnessLevel = 1,
            enableTentacles = false,
            soulType = "patience",
            soulIndex = 2
        }
    },
    {
        name = "finale_sequence",
        data = {
            width = 56,
            height = 40,
            dialogKey = "CH16_MY_FINALE",
            characterState = "madeline_determined",
            enableCharacterStates = true,
            enablePortraitChanges = true,
            enableEffects = true,
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            madnessLevel = 5,
            enableTentacles = false,
            enableSoulPower = true
        }
    },
    {
        name = "take_the_l",
        data = {
            width = 64,
            height = 48,
            dialogKey = "CH16_TAKE_THE_L",
            characterState = "flowey_skibididoop",
            enableCharacterStates = true,
            enablePortraitChanges = true,
            enableEffects = true,
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            madnessLevel = 5,
            enableTentacles = false,
            enableRealityWarp = true
        }
    },
    {
        name = "call_for_help",
        data = {
            width = 48,
            height = 36,
            dialogKey = "CH16_YOU_CALL_HELP",
            characterState = "system_voice",
            enableCharacterStates = true,
            enablePortraitChanges = true,
            enableEffects = true,
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            madnessLevel = 4,
            enableTentacles = false,
            enableSoulRestore = true
        }
    },
    {
        name = "final_defeat",
        data = {
            width = 56,
            height = 32,
            dialogKey = "CH16_FINAL_DEFEAT",
            characterState = "flowey_freakout",
            enableCharacterStates = true,
            enablePortraitChanges = true,
            enableEffects = true,
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            madnessLevel = 5,
            enableTentacles = false
        }
    },
    {
        name = "return_home",
        data = {
            width = 64,
            height = 32,
            dialogKey = "CH16_RETURN_TO_CELESTE",
            characterState = "madeline_determined",
            enableCharacterStates = true,
            enablePortraitChanges = false,
            enableEffects = true,
            triggerOnce = true,
            playerOnly = true,
            autoStart = false,
            madnessLevel = 0,
            enableTentacles = false,
            peaceful = true
        }
    }
}

chapter16DialogTrigger.fieldInformation = {
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
    characterState = {
        fieldType = "string",
        options = {
            "madeline_distracted",
            "madeline_surprised", 
            "madeline_panic",
            "madeline_lunaticA",
            "madeline_lunaticB",
            "madeline_lunaticC",
            "madeline_determined",
            "madeline_normal",
            "badeline_worried",
            "badeline_concerned",
            "badeline_normal",
            "chara_terrified",
            "chara_crying",
            "flowey_evil",
            "flowey_laughing",
            "flowey_normal",
            "flowey_finalmoment",
            "flowey_skibididoop",
            "flowey_wtf",
            "flowey_wtfalt",
            "flowey_freakout",
            "lost_soul_none",
            "system_voice",
            "none"
        }
    },
    enableCharacterStates = {
        fieldType = "boolean"
    },
    enablePortraitChanges = {
        fieldType = "boolean"
    },
    enableEffects = {
        fieldType = "boolean"
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
    madnessLevel = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 5
    },
    enableTentacles = {
        fieldType = "boolean"
    },
    soulType = {
        fieldType = "string",
        options = {
            "human",
            "patience",
            "bravery",
            "integrity", 
            "perseverance",
            "kindness",
            "justice",
            "determination"
        }
    },
    soulIndex = {
        fieldType = "integer",
        minimumValue = 1,
        maximumValue = 8
    },
    enableSoulPower = {
        fieldType = "boolean"
    },
    enableRealityWarp = {
        fieldType = "boolean"
    },
    enableSoulRestore = {
        fieldType = "boolean"
    },
    peaceful = {
        fieldType = "boolean"
    }
}

function chapter16DialogTrigger.sprite(room, entity)
    local dialogKey = entity.dialogKey or "CH16_CORRUPTED_REALITY_INTRO"
    local characterState = entity.characterState or "madeline_distracted"
    
    local spriteBase = "objects/DesoloZantas/dialog/"
    
    if dialogKey:find("CORRUPTED") then
        return spriteBase .. "corrupted_dialog"
    elseif dialogKey:find("BATTLE") then
        return spriteBase .. "battle_dialog"
    elseif dialogKey:find("LOST_SOULS") then
        return spriteBase .. "soul_dialog"
    elseif dialogKey:find("FINALE") then
        return spriteBase .. "finale_dialog"
    elseif dialogKey:find("TAKE_THE_L") then
        return spriteBase .. "takethel_dialog"
    elseif dialogKey:find("HELP") then
        return spriteBase .. "help_dialog"
    elseif dialogKey:find("DEFEAT") then
        return spriteBase .. "defeat_dialog"
    elseif dialogKey:find("BARRIER") then
        return spriteBase .. "barrier_dialog"
    elseif dialogKey:find("RETURN") then
        return spriteBase .. "return_dialog"
    else
        return spriteBase .. "generic_dialog"
    end
end

function chapter16DialogTrigger.selection(room, entity)
    return utils.rectangle(entity.x, entity.y, entity.width or 32, entity.height or 32)
end

-- Dialog category colors for visual identification
local dialogColors = {
    corrupted = {0.8, 0.2, 0.8, 0.8},    -- Purple for corruption
    battle = {0.8, 0.2, 0.2, 0.8},       -- Red for battle
    soul = {0.9, 0.9, 0.2, 0.8},         -- Yellow for souls
    finale = {0.9, 0.6, 0.1, 0.8},       -- Orange for finale
    takethel = {0.8, 0.2, 0.9, 0.8},     -- Magenta for meme
    help = {0.2, 0.6, 0.9, 0.8},         -- Blue for help
    defeat = {0.6, 0.1, 0.1, 0.8},       -- Dark red for defeat
    barrier = {0.2, 0.9, 0.2, 0.8},      -- Green for liberation
    return = {0.4, 0.8, 0.4, 0.8},       -- Light green for peaceful
    generic = {0.5, 0.5, 0.8, 0.8}       -- Blue for generic
}

function chapter16DialogTrigger.rectangle(room, entity)
    local dialogKey = entity.dialogKey or "CH16_CORRUPTED_REALITY_INTRO"
    local madnessLevel = entity.madnessLevel or 0
    local width = entity.width or 32
    local height = entity.height or 32
    
    local colorType = "generic"
    if dialogKey:find("CORRUPTED") then
        colorType = "corrupted"
    elseif dialogKey:find("BATTLE") then
        colorType = "battle"
    elseif dialogKey:find("LOST_SOULS") then
        colorType = "soul"
    elseif dialogKey:find("FINALE") then
        colorType = "finale"
    elseif dialogKey:find("TAKE_THE_L") then
        colorType = "takethel"
    elseif dialogKey:find("HELP") then
        colorType = "help"
    elseif dialogKey:find("DEFEAT") then
        colorType = "defeat"
    elseif dialogKey:find("BARRIER") then
        colorType = "barrier"
    elseif dialogKey:find("RETURN") then
        colorType = "return"
    end
    
    local color = dialogColors[colorType]
    
    -- Adjust intensity based on madness level
    local intensity = 0.7 + (madnessLevel / 5.0) * 0.3
    color = {color[1] * intensity, color[2] * intensity, color[3] * intensity, color[4]}
    
    return utils.rectangle(entity.x, entity.y, width, height), color
end

-- Node support for character positioning and special effects
function chapter16DialogTrigger.nodeSprite(room, entity, node, nodeIndex)
    local characterState = entity.characterState or "madeline_distracted"
    
    if characterState:find("madeline") then
        return "objects/DesoloZantas/madeline_position"
    elseif characterState:find("flowey") then
        return "objects/DesoloZantas/flowey_position"
    elseif characterState:find("soul") then
        return "objects/DesoloZantas/soul_position"
    else
        return "objects/DesoloZantas/character_position"
    end
end

function chapter16DialogTrigger.nodeSelection(room, entity, node)
    return utils.rectangle(node.x - 8, node.y - 8, 16, 16)
end

function chapter16DialogTrigger.nodeLimits()
    return {0, 6} -- Up to 6 character positions
end

return chapter16DialogTrigger