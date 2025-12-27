-- Blackhole Blocker Trigger - stops player movement when entering
local blackholeBlockerTrigger = {}

blackholeBlockerTrigger.name = "Ingeste/BlackholeBlockerTrigger"
blackholeBlockerTrigger.placements = {
    {
        name = "normal",
        data = {
            width = 16,
            height = 16,
            stopDuration = 1.5,
            oneUse = false,
            pullStrength = 200.0,
            pullDirection = "Center",
            killPlayer = false,
            visualEffect = true,
            releaseFlag = ""
        }
    },
    {
        name = "deadly",
        data = {
            width = 32,
            height = 32,
            stopDuration = 2.0,
            oneUse = false,
            pullStrength = 300.0,
            pullDirection = "Center",
            killPlayer = true,
            visualEffect = true,
            releaseFlag = ""
        }
    },
    {
        name = "pull_up",
        data = {
            width = 64,
            height = 16,
            stopDuration = 1.0,
            oneUse = false,
            pullStrength = 150.0,
            pullDirection = "Up",
            killPlayer = false,
            visualEffect = true,
            releaseFlag = ""
        }
    }
}

blackholeBlockerTrigger.fieldInformation = {
    stopDuration = {
        fieldType = "number",
        minimumValue = 0.1,
        maximumValue = 10.0
    },
    oneUse = {
        fieldType = "boolean"
    },
    pullStrength = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 1000.0
    },
    pullDirection = {
        options = { "Center", "Up", "Down", "Left", "Right" },
        editable = false
    },
    killPlayer = {
        fieldType = "boolean"
    },
    visualEffect = {
        fieldType = "boolean"
    },
    releaseFlag = {
        fieldType = "string"
    }
}

function blackholeBlockerTrigger.sprite(room, entity)
    -- Visual representation of the trigger
    local sprites = {}
    
    -- Background
    table.insert(sprites, {
        texture = "objects/IngesteHelper/blackhole_blocker",
        x = 0,
        y = 0,
        justificationX = 0.0,
        justificationY = 0.0,
        scaleX = entity.width / 8,
        scaleY = entity.height / 8,
        color = entity.killPlayer and {0.8, 0.0, 0.0, 0.4} or {0.5, 0.0, 0.5, 0.4}
    })
    
    -- Direction indicator
    local arrowX = entity.width / 2
    local arrowY = entity.height / 2
    local arrowColor = {1.0, 1.0, 1.0, 0.8}
    
    if entity.pullDirection == "Up" then
        arrowY = 4
    elseif entity.pullDirection == "Down" then
        arrowY = entity.height - 4
    elseif entity.pullDirection == "Left" then
        arrowX = 4
    elseif entity.pullDirection == "Right" then
        arrowX = entity.width - 4
    end
    
    table.insert(sprites, {
        texture = "objects/IngesteHelper/blackhole_arrow",
        x = arrowX,
        y = arrowY,
        justificationX = 0.5,
        justificationY = 0.5,
        color = arrowColor
    })
    
    return sprites
end

function blackholeBlockerTrigger.rectangle(room, entity)
    return utils.rectangle(entity.x, entity.y, entity.width, entity.height)
end

return blackholeBlockerTrigger
