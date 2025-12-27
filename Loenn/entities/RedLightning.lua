-- Loenn integration for Red Lightning entity
-- Creates dramatic red lightning visual effects

local redLightning = {}

redLightning.name = "Ingeste/RedLightning"
redLightning.depth = -10
redLightning.texture = "effects/lightning_red"

redLightning.fieldInformation = {
    duration = {
        fieldType = "number",
        minimumValue = 0.1,
        maximumValue = 10.0
    },
    intensity = {
        fieldType = "number", 
        minimumValue = 0.1,
        maximumValue = 2.0
    },
    color = {
        fieldType = "color",
        allowAlpha = true
    },
    followPlayer = {
        fieldType = "boolean"
    },
    soundEffect = {
        fieldType = "string",
        options = {
            "event:/game/general/lightning",
            "event:/char/badeline/disappear",
            "event:/game/general/thing_booped"
        },
        editable = true
    }
}

redLightning.placements = {
    {
        name = "dramatic_effect",
        data = {
            width = 32,
            height = 64,
            duration = 2.0,
            intensity = 1.0,
            color = "FF0000FF",
            followPlayer = false,
            soundEffect = "event:/game/general/lightning"
        }
    },
    {
        name = "chase_effect",
        data = {
            width = 24,
            height = 48,
            duration = 1.5,
            intensity = 1.5,
            color = "FF4444AA",
            followPlayer = true,
            soundEffect = "event:/char/badeline/disappear"
        }
    },
    {
        name = "ambient_effect",
        data = {
            width = 16,
            height = 32,
            duration = 0.5,
            intensity = 0.7,
            color = "DD0000BB",
            followPlayer = false,
            soundEffect = ""
        }
    }
}

function redLightning.sprite(room, entity)
    local sprite = {}
    
    sprite.texture = redLightning.texture
    sprite.x = entity.x
    sprite.y = entity.y
    sprite.scaleX = entity.width / 16
    sprite.scaleY = entity.height / 16
    
    return sprite
end

return redLightning