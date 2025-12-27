-- Francisca Mage-Sister Dream Friend entity for Loenn map editor
-- Ice attacks and platform creation

local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local francisca = {}

francisca.name = "Ingeste/FranciscaDreamFriend"
francisca.depth = -8500
francisca.justification = {0.5, 1.0}
francisca.texture = "characters/francisca/idle00"
francisca.nodeLimits = {0, 0}

francisca.placements = {
    {
        name = "Francisca Dream Friend",
        data = {
            index = 0,
            iceAxePower = 1.0,
            platformCount = 5
        }
    }
}

francisca.fieldInformation = {
    index = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 10
    },
    iceAxePower = {
        fieldType = "number",
        minimumValue = 0.5,
        maximumValue = 2.0
    },
    platformCount = {
        fieldType = "integer",
        minimumValue = 3,
        maximumValue = 8
    }
}

function francisca.sprite(room, entity)
    local texture = entity.texture or francisca.texture
    local sprite = drawableSprite.fromTexture(texture, entity)
    
    -- Ice-blue coloring
    sprite:setColor({0.7, 0.9, 1.0, 1.0})
    
    return sprite
end

function francisca.rectangle(room, entity)
    return utils.rectangle(entity.x - 6, entity.y - 12, 12, 12)
end

return francisca