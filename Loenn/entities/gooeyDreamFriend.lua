-- Gooey Dream Friend entity for Loenn map editor
-- Tongue mechanics and Gooey Stone attacks

local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local gooey = {}

gooey.name = "Ingeste/GooeyDreamFriend"
gooey.depth = -8500
gooey.justification = {0.5, 1.0}
gooey.texture = "characters/gooey/idle00"
gooey.nodeLimits = {0, 0}

gooey.placements = {
    {
        name = "Gooey Dream Friend",
        data = {
            index = 0,
            tongueLength = 80,
            wallGrabAssist = true
        }
    }
}

gooey.fieldInformation = {
    index = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 10
    },
    tongueLength = {
        fieldType = "number",
        minimumValue = 40,
        maximumValue = 120
    },
    wallGrabAssist = {
        fieldType = "boolean"
    }
}

function gooey.sprite(room, entity)
    local texture = entity.texture or gooey.texture
    local sprite = drawableSprite.fromTexture(texture, entity)
    
    -- Purple gooey coloring
    sprite:setColor({0.6, 0.3, 0.8, 1.0})
    
    return sprite
end

function gooey.rectangle(room, entity)
    return utils.rectangle(entity.x - 6, entity.y - 12, 12, 12)
end

return gooey