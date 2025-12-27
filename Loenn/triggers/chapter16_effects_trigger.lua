-- Chapter 16 Special Effects Trigger
-- Handles screen effects, reality warping, and visual distortions for Chapter 16

local chapter16EffectsTrigger = {}

chapter16EffectsTrigger.name = "Ingeste/Chapter16EffectsTrigger"

chapter16EffectsTrigger.placements = {
    {
        name = "screen_shake",
        data = {
            width = 32,
            height = 32,
            effectType = "screen_shake",
            intensity = 5.0,
            duration = 2.0,
            triggerCommand = "dz_shake_screen 5.0 5.0",
            autoTrigger = false,
            triggerOnce = true,
            playerOnly = true,
            enableSound = true,
            soundEvent = "event:/char/desolozantas/screen_shake"
        }
    },
    {
        name = "flash_screen",
        data = {
            width = 24,
            height = 24,
            effectType = "flash_screen", 
            flashColor = "FFFFFF",
            intensity = 3.0,
            duration = 2.0,
            triggerCommand = "dz_flash_screen FFFFFF 2.0",
            autoTrigger = false,
            triggerOnce = true,
            playerOnly = true,
            enableSound = false,
            soundEvent = ""
        }
    },
    {
        name = "reality_glitch",
        data = {
            width = 48,
            height = 48,
            effectType = "reality_glitch",
            intensity = 8.0,
            duration = 5.0,
            triggerCommand = "dz_trigger 0 Flash glitched screen white, shake screen violently with karma multiple times, and teleport the windows all over the place",
            autoTrigger = false,
            triggerOnce = false,
            playerOnly = true,
            enableSound = true,
            soundEvent = "event:/char/desolozantas/reality_warp",
            enableWindowManipulation = true
        }
    },
    {
        name = "fade_to_black",
        data = {
            width = 40,
            height = 32,
            effectType = "fade_transition",
            fadeType = "to_black",
            intensity = 1.0,
            duration = 2.0,
            triggerCommand = "dz_trigger fade_to_black 2.0",
            autoTrigger = false,
            triggerOnce = true,
            playerOnly = true,
            enableSound = false,
            soundEvent = ""
        }
    },
    {
        name = "fade_from_black",
        data = {
            width = 40,
            height = 32,
            effectType = "fade_transition",
            fadeType = "from_black", 
            intensity = 1.0,
            duration = 3.0,
            triggerCommand = "dz_trigger fade_from_black 3.0",
            autoTrigger = false,
            triggerOnce = true,
            playerOnly = true,
            enableSound = false,
            soundEvent = ""
        }
    },
    {
        name = "camera_pan",
        data = {
            width = 32,
            height = 32,
            effectType = "camera_pan",
            panX = 0,
            panY = -200,
            intensity = 1.0,
            duration = 3.0,
            easing = "ease_out",
            triggerCommand = "dz_trigger camera_pan 0 -200 3.0 ease_out",
            autoTrigger = false,
            triggerOnce = true,
            playerOnly = true,
            enableSound = false,
            soundEvent = ""
        }
    },
    {
        name = "eight_souls_appear",
        data = {
            width = 64,
            height = 64,
            effectType = "soul_manifestation",
            intensity = 2.0,
            duration = 5.0,
            triggerCommand = "dz_flash_screen FFFFFF 2.0 eight soul appears around madeline and friends against els",
            autoTrigger = false,
            triggerOnce = true,
            playerOnly = true,
            enableSound = true,
            soundEvent = "event:/char/desolozantas/souls_awakening",
            soulCount = 8
        }
    },
    {
        name = "load_failed",
        data = {
            width = 48,
            height = 32,
            effectType = "system_message",
            messageText = "Load Failed",
            intensity = 1.0,
            duration = 3.0,
            triggerCommand = "dz_show_text load failed",
            autoTrigger = false,
            triggerOnce = true,
            playerOnly = true,
            enableSound = true,
            soundEvent = "event:/char/desolozantas/system_error"
        }
    },
    {
        name = "credits_roll",
        data = {
            width = 64,
            height = 32,
            effectType = "credits_sequence",
            intensity = 1.0,
            duration = 60.0,
            triggerCommand = "dz_trigger credits_roll",
            autoTrigger = false,
            triggerOnce = true,
            playerOnly = true,
            enableSound = true,
            soundEvent = "event:/music/desolozantas/ending_theme"
        }
    }
}

chapter16EffectsTrigger.fieldInformation = {
    effectType = {
        fieldType = "string",
        options = {
            "screen_shake",
            "flash_screen",
            "reality_glitch",
            "fade_transition",
            "camera_pan", 
            "soul_manifestation",
            "system_message",
            "credits_sequence",
            "window_manipulation",
            "karma_effect",
            "death_pellets",
            "barrier_break"
        }
    },
    flashColor = {
        fieldType = "string",
        options = {
            "FFFFFF", -- White
            "FF0000", -- Red
            "00FF00", -- Green
            "0000FF", -- Blue
            "FFFF00", -- Yellow
            "FF00FF", -- Magenta
            "00FFFF", -- Cyan
            "000000"  -- Black
        }
    },
    fadeType = {
        fieldType = "string",
        options = {
            "to_black",
            "from_black",
            "to_white",
            "from_white"
        }
    },
    panX = {
        fieldType = "number",
        minimumValue = -1000.0,
        maximumValue = 1000.0
    },
    panY = {
        fieldType = "number",
        minimumValue = -1000.0,
        maximumValue = 1000.0
    },
    easing = {
        fieldType = "string",
        options = {
            "linear",
            "ease_in",
            "ease_out",
            "ease_in_out",
            "bounce",
            "elastic"
        }
    },
    intensity = {
        fieldType = "number",
        minimumValue = 0.1,
        maximumValue = 10.0
    },
    duration = {
        fieldType = "number",
        minimumValue = 0.1,
        maximumValue = 60.0
    },
    triggerCommand = {
        fieldType = "string"
    },
    autoTrigger = {
        fieldType = "boolean"
    },
    triggerOnce = {
        fieldType = "boolean"
    },
    playerOnly = {
        fieldType = "boolean"
    },
    enableSound = {
        fieldType = "boolean"
    },
    soundEvent = {
        fieldType = "string"
    },
    enableWindowManipulation = {
        fieldType = "boolean"
    },
    soulCount = {
        fieldType = "integer",
        minimumValue = 1,
        maximumValue = 8
    },
    messageText = {
        fieldType = "string"
    }
}

function chapter16EffectsTrigger.sprite(room, entity)
    local effectType = entity.effectType or "screen_shake"
    
    local spriteBase = "objects/DesoloZantas/effects/"
    
    if effectType == "screen_shake" then
        return spriteBase .. "shake_trigger"
    elseif effectType == "flash_screen" then
        return spriteBase .. "flash_trigger"
    elseif effectType == "reality_glitch" then
        return spriteBase .. "glitch_trigger"
    elseif effectType == "fade_transition" then
        return spriteBase .. "fade_trigger"
    elseif effectType == "camera_pan" then
        return spriteBase .. "camera_trigger"
    elseif effectType == "soul_manifestation" then
        return spriteBase .. "soul_trigger"
    elseif effectType == "system_message" then
        return spriteBase .. "system_trigger"
    elseif effectType == "credits_sequence" then
        return spriteBase .. "credits_trigger"
    else
        return spriteBase .. "generic_trigger"
    end
end

function chapter16EffectsTrigger.selection(room, entity)
    return utils.rectangle(entity.x, entity.y, entity.width or 32, entity.height or 32)
end

-- Color coding for different effect types
local effectColors = {
    screen_shake = {0.8, 0.4, 0.2, 0.7},      -- Orange for shake
    flash_screen = {0.9, 0.9, 0.9, 0.8},      -- White for flash
    reality_glitch = {0.8, 0.2, 0.8, 0.8},    -- Purple for glitch
    fade_transition = {0.2, 0.2, 0.2, 0.7},   -- Dark for fade
    camera_pan = {0.2, 0.6, 0.8, 0.7},        -- Blue for camera
    soul_manifestation = {0.9, 0.9, 0.2, 0.8}, -- Yellow for souls
    system_message = {0.6, 0.6, 0.6, 0.7},    -- Gray for system
    credits_sequence = {0.2, 0.8, 0.2, 0.8}   -- Green for credits
}

function chapter16EffectsTrigger.rectangle(room, entity)
    local effectType = entity.effectType or "screen_shake"
    local intensity = entity.intensity or 1.0
    local width = entity.width or 32
    local height = entity.height or 32
    
    local color = effectColors[effectType] or effectColors.screen_shake
    
    -- Adjust alpha based on intensity
    local alpha = math.min(1.0, 0.5 + (intensity / 10.0) * 0.5)
    color = {color[1], color[2], color[3], alpha}
    
    return utils.rectangle(entity.x, entity.y, width, height), color
end

-- Node support for complex effects with multiple points
function chapter16EffectsTrigger.nodeSprite(room, entity, node, nodeIndex)
    local effectType = entity.effectType or "screen_shake"
    
    if effectType == "soul_manifestation" then
        return "objects/DesoloZantas/soul_spawn_point"
    elseif effectType == "camera_pan" then
        return "objects/DesoloZantas/camera_point"
    else
        return "objects/DesoloZantas/effect_point"
    end
end

function chapter16EffectsTrigger.nodeSelection(room, entity, node)
    return utils.rectangle(node.x - 4, node.y - 4, 8, 8)
end

function chapter16EffectsTrigger.nodeLimits()
    return {0, 8} -- Up to 8 effect points
end

return chapter16EffectsTrigger