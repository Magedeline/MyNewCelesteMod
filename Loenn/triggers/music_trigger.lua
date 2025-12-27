local musicTrigger = {}

musicTrigger.name = "Ingeste/MusicTrigger"
musicTrigger.placements = {
    {
        name = "normal",
        data = {
            width = 16,
            height = 16,
            musicTrack = "ingeste_theme",
            fadeTime = 1.0,
            loop = true,
            volume = 1.0
        }
    },
    {
        name = "ambient",
        data = {
            width = 32,
            height = 32,
            musicTrack = "ingeste_ambient",
            fadeTime = 2.0,
            loop = true,
            volume = 0.7
        }
    },
    {
        name = "stop_music",
        data = {
            width = 16,
            height = 16,
            musicTrack = "",
            fadeTime = 1.5,
            loop = false,
            volume = 0.0
        }
    }
}

musicTrigger.fieldInformation = {
    musicTrack = {
        fieldType = "string"
    },
    fadeTime = {
        fieldType = "number",
        minimumValue = 0.1,
        maximumValue = 10.0
    },
    loop = {
        fieldType = "boolean"
    },
    volume = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 1.0
    }
}

function musicTrigger.sprite(room, entity)
    local width = entity.width or 16
    local height = entity.height or 16
    local color = {0.2, 0.9, 0.3, 0.7}
    
    if entity.musicTrack == "" then
        color = {0.9, 0.3, 0.3, 0.7}
    end
    
    return {
        {
            texture = "ahorn/entityTrigger",
            x = entity.x,
            y = entity.y,
            scaleX = width / 8,
            scaleY = height / 8,
            color = color
        }
    }
end

function musicTrigger.selection(room, entity)
    local width = entity.width or 16
    local height = entity.height or 16
    return {entity.x, entity.y, width, height}
end

return musicTrigger
