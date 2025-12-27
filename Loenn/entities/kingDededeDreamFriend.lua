-- King Dedede Dream Friend entity for Loenn map editor
-- Hammer attacks and elemental infusion abilities

local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local kingDedede = {}

kingDedede.name = "Ingeste/KingDededeDreamFriend"
kingDedede.depth = -8500
kingDedede.justification = {0.5, 1.0}
kingDedede.texture = "characters/dedede/idle00"
kingDedede.nodeLimits = {0, 0}

kingDedede.placements = {
    {
        name = "King Dedede Dream Friend",
        data = {
            index = 0,
            elementalInfusion = "None"
        }
    }
}

kingDedede.fieldInformation = {
    index = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 10
    },
    elementalInfusion = {
        options = {"None", "Sizzle", "Blizzard", "Splash", "Zap", "Bluster"},
        editable = false
    }
}

function kingDedede.sprite(room, entity)
    local texture = entity.texture or kingDedede.texture
    local sprite = drawableSprite.fromTexture(texture, entity)
    
    -- Add elemental aura effect based on infusion
    local elementalColor = {1.0, 1.0, 1.0, 0.8}
    if entity.elementalInfusion == "Sizzle" then
        elementalColor = {1.0, 0.4, 0.2, 0.6}
    elseif entity.elementalInfusion == "Blizzard" then
        elementalColor = {0.4, 0.8, 1.0, 0.6}
    elseif entity.elementalInfusion == "Splash" then
        elementalColor = {0.2, 0.6, 1.0, 0.6}
    elseif entity.elementalInfusion == "Zap" then
        elementalColor = {1.0, 1.0, 0.2, 0.6}
    elseif entity.elementalInfusion == "Bluster" then
        elementalColor = {0.8, 1.0, 0.8, 0.6}
    end
    
    sprite:setColor(elementalColor)
    
    return sprite
end

function kingDedede.rectangle(room, entity)
    return utils.rectangle(entity.x - 6, entity.y - 12, 12, 12)
end

return kingDedede