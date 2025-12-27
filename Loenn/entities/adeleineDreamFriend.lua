-- Adeleine Dream Friend entity for Loenn map editor
-- Painting attacks and animal friend summoning

local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local adeleine = {}

adeleine.name = "Ingeste/AdeleineDreamFriend"
adeleine.depth = -8500
adeleine.justification = {0.5, 1.0}
adeleine.texture = "characters/adeleine/idle00"
adeleine.nodeLimits = {0, 0}

adeleine.placements = {
    {
        name = "Adeleine Dream Friend",
        data = {
            index = 0,
            startingAnimalFriend = "Rick",
            paintElement = "None"
        }
    }
}

adeleine.fieldInformation = {
    index = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 10
    },
    startingAnimalFriend = {
        options = {"Rick", "Kine", "Coo"},
        editable = false
    },
    paintElement = {
        options = {"None", "Splash", "Blizzard", "Sizzle"},
        editable = false
    }
}

function adeleine.sprite(room, entity)
    local texture = entity.texture or adeleine.texture
    local sprite = drawableSprite.fromTexture(texture, entity)
    
    -- Pink artistic coloring
    sprite:setColor({1.0, 0.7, 0.8, 1.0})
    
    return sprite
end

function adeleine.rectangle(room, entity)
    return utils.rectangle(entity.x - 6, entity.y - 12, 12, 12)
end

return adeleine