-- Zan Partizanne Mage-Sister Dream Friend entity for Loenn map editor
-- Electric attacks and lightning strikes

local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local zanPartizanne = {}

zanPartizanne.name = "Ingeste/ZanPartizanneDreamFriend"
zanPartizanne.depth = -8500
zanPartizanne.justification = {0.5, 1.0}
zanPartizanne.texture = "characters/zanpartizanne/idle00"
zanPartizanne.nodeLimits = {0, 0}

zanPartizanne.placements = {
    {
        name = "Zan Partizanne Dream Friend",
        data = {
            index = 0,
            lightningPower = 1.0,
            strikeCount = 3
        }
    }
}

zanPartizanne.fieldInformation = {
    index = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 10
    },
    lightningPower = {
        fieldType = "number",
        minimumValue = 0.5,
        maximumValue = 3.0
    },
    strikeCount = {
        fieldType = "integer",
        minimumValue = 2,
        maximumValue = 8
    }
}

function zanPartizanne.sprite(room, entity)
    local texture = entity.texture or zanPartizanne.texture
    local sprite = drawableSprite.fromTexture(texture, entity)
    
    -- Electric-yellow coloring
    sprite:setColor({1.0, 1.0, 0.4, 1.0})
    
    return sprite
end

function zanPartizanne.rectangle(room, entity)
    return utils.rectangle(entity.x - 6, entity.y - 12, 12, 12)
end

return zanPartizanne