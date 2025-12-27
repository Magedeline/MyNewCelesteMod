local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local windTrigger = {}

windTrigger.name = "Ingeste/WindTrigger"
windTrigger.depth = 0
windTrigger.canResize = {true, true}

windTrigger.placements = {
    {
        name = "right_wind",
        data = {
            direction = 0,
            strength = 100,
            affectPlayer = true,
            affectStylegrounds = true,
            triggerOnce = false,
            duration = 0,
            particleEffects = true,
            soundEffects = true,
            windColor = "87CEEB",
            windType = "Constant",
            gustFrequency = 2,
            gustIntensity = 1.5,
            width = 16,
            height = 16
        }
    },
    {
        name = "upward_wind",
        data = {
            direction = 90,
            strength = 150,
            affectPlayer = true,
            affectStylegrounds = true,
            triggerOnce = false,
            duration = 0,
            particleEffects = true,
            soundEffects = true,
            windColor = "87CEEB",
            windType = "Updraft",
            gustFrequency = 2,
            gustIntensity = 1.5,
            width = 24,
            height = 32
        }
    },
    {
        name = "left_wind",
        data = {
            direction = 180,
            strength = 100,
            affectPlayer = true,
            affectStylegrounds = true,
            triggerOnce = false,
            duration = 0,
            particleEffects = true,
            soundEffects = true,
            windColor = "87CEEB",
            windType = "Constant",
            gustFrequency = 2,
            gustIntensity = 1.5,
            width = 16,
            height = 16
        }
    },
    {
        name = "downward_wind",
        data = {
            direction = 270,
            strength = 120,
            affectPlayer = true,
            affectStylegrounds = true,
            triggerOnce = false,
            duration = 0,
            particleEffects = true,
            soundEffects = true,
            windColor = "87CEEB",
            windType = "Constant",
            gustFrequency = 2,
            gustIntensity = 1.5,
            width = 16,
            height = 16
        }
    },
    {
        name = "gusty_wind",
        data = {
            direction = 45,
            strength = 80,
            affectPlayer = true,
            affectStylegrounds = true,
            triggerOnce = false,
            duration = 0,
            particleEffects = true,
            soundEffects = true,
            windColor = "B0E0E6",
            windType = "Gusty",
            gustFrequency = 3,
            gustIntensity = 2.0,
            width = 20,
            height = 20
        }
    },
    {
        name = "swirling_wind",
        data = {
            direction = 0,
            strength = 90,
            affectPlayer = true,
            affectStylegrounds = true,
            triggerOnce = false,
            duration = 0,
            particleEffects = true,
            soundEffects = true,
            windColor = "98FB98",
            windType = "Swirling",
            gustFrequency = 2,
            gustIntensity = 1.3,
            width = 32,
            height = 32
        }
    },
    {
        name = "crosswind",
        data = {
            direction = 0,
            strength = 110,
            affectPlayer = true,
            affectStylegrounds = true,
            triggerOnce = false,
            duration = 5,
            particleEffects = true,
            soundEffects = true,
            windColor = "F0F8FF",
            windType = "Crosswind",
            gustFrequency = 1.5,
            gustIntensity = 1.8,
            width = 40,
            height = 16
        }
    }
}

windTrigger.fieldInformation = {
    direction = {
        fieldType = "number",
        minimumValue = 0,
        maximumValue = 360,
        editable = true
    },
    strength = {
        fieldType = "number",
        minimumValue = 0,
        maximumValue = 500,
        editable = true
    },
    affectPlayer = {
        fieldType = "boolean"
    },
    affectStylegrounds = {
        fieldType = "boolean"
    },
    triggerOnce = {
        fieldType = "boolean"
    },
    duration = {
        fieldType = "number",
        minimumValue = 0,
        maximumValue = 60,
        editable = true
    },
    particleEffects = {
        fieldType = "boolean"
    },
    soundEffects = {
        fieldType = "boolean"
    },
    windColor = {
        fieldType = "color",
        allowXNAColors = true
    },
    windType = {
        fieldType = "string",
        options = {
            "Constant",
            "Gusty", 
            "Swirling",
            "Updraft",
            "Crosswind"
        },
        editable = false
    },
    gustFrequency = {
        fieldType = "number",
        minimumValue = 0.1,
        maximumValue = 10,
        editable = true
    },
    gustIntensity = {
        fieldType = "number",
        minimumValue = 0.5,
        maximumValue = 5,
        editable = true
    }
}

windTrigger.fieldOrder = {
    "direction",
    "strength", 
    "windType",
    "affectPlayer",
    "affectStylegrounds",
    "triggerOnce",
    "duration",
    "particleEffects",
    "soundEffects",
    "windColor",
    "gustFrequency",
    "gustIntensity"
}

function windTrigger.sprite(room, entity)
    local sprites = {}
    
    -- Main trigger area with simple rectangle outline
    local width = entity.width or 16
    local height = entity.height or 16
    local x = entity.x - width / 2
    local y = entity.y - height / 2
    
    -- Wind color
    local windColor = entity.windColor or "87CEEB"
    local r, g, b = utils.parseHexColor(windColor)
    
    -- Create main background rectangle
    local bgSprite = drawableSprite.fromTexture("util/pixel", entity)
    bgSprite:setPosition(x, y)
    bgSprite:setScale(width, height)
    bgSprite:setColor({r/255, g/255, b/255, 0.3})
    table.insert(sprites, bgSprite)
    
    -- Wind direction indicator
    local direction = entity.direction or 0
    local directionRad = math.rad(direction)
    local arrowLength = math.min(width, height) / 3
    
    -- Calculate arrow end point
    local centerX = entity.x
    local centerY = entity.y
    local endX = centerX + math.cos(directionRad) * arrowLength
    local endY = centerY + math.sin(directionRad) * arrowLength
    
    -- Arrow line (simplified as a rectangle)
    local arrowSprite = drawableSprite.fromTexture("util/pixel", entity)
    arrowSprite:setPosition(centerX - 1, centerY - 1)
    arrowSprite:setScale(2, 2)
    arrowSprite:setColor({1.0, 1.0, 1.0, 0.9})
    table.insert(sprites, arrowSprite)
    
    -- Wind type text indicator
    local windType = entity.windType or "Constant"
    local typeText = string.upper(string.sub(windType, 1, 1))
    
    local textSprite = drawableSprite.fromText(typeText, entity.x - width/2 + 4, entity.y - height/2 + 4, "small")
    textSprite:setColor({1.0, 1.0, 1.0, 0.9})
    table.insert(sprites, textSprite)
    
    -- Strength indicators (dots)
    local strength = entity.strength or 100
    local strengthLevel = math.min(math.floor(strength / 100) + 1, 3)
    
    for i = 1, strengthLevel do
        local dotSprite = drawableSprite.fromTexture("util/pixel", entity)
        dotSprite:setPosition(entity.x + width/2 - 6, entity.y - height/2 + i * 3)
        dotSprite:setScale(2, 2)
        dotSprite:setColor({1.0, 1.0, 0.0, 0.8})
        table.insert(sprites, dotSprite)
    end
    
    return sprites
end

function windTrigger.selection(room, entity)
    local width = entity.width or 16
    local height = entity.height or 16
    return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height)
end

function windTrigger.minimumSize(room, entity)
    return 8, 8
end

function windTrigger.maximumSize(room, entity)
    return math.huge, math.huge
end

function windTrigger.resizeRectangle(room, entity)
    local width = entity.width or 16
    local height = entity.height or 16
    return utils.rectangle(entity.x - width / 2, entity.y - height / 2, width, height)
end

return windTrigger