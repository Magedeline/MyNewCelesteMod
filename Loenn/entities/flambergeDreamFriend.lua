-- Flamberge Mage-Sister Dream Friend entity for Loenn map editor
-- Fire attacks and flame trails

local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local flamberge = {}

flamberge.name = "Ingeste/FlambergeDreamFriend"
flamberge.depth = -8500
flamberge.justification = {0.5, 1.0}
flamberge.texture = "characters/flamberge/idle00"
flamberge.nodeLimits = {0, 0}

flamberge.placements = {
    {
        name = "Flamberge Dream Friend",
        data = {
            index = 0,
            firePower = 1.0,
            burnRadius = 25
        }
    }
}

flamberge.fieldInformation = {
    index = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 10
    },
    firePower = {
        fieldType = "number",
        minimumValue = 0.5,
        maximumValue = 2.5
    },
    burnRadius = {
        fieldType = "number",
        minimumValue = 15,
        maximumValue = 40
    }
}

function flamberge.sprite(room, entity)
    local texture = entity.texture or flamberge.texture
    local sprite = drawableSprite.fromTexture(texture, entity)
    
    -- Fire-orange coloring
    sprite:setColor({1.0, 0.6, 0.3, 1.0})
    
    return sprite
end

function flamberge.rectangle(room, entity)
    return utils.rectangle(entity.x - 6, entity.y - 12, 12, 12)
end

return flamberge