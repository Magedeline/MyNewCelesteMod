-- Disclaimer Trigger for Loenn
-- Shows a content warning/disclaimer screen that requires acknowledgment

local disclaimerTrigger = {}

disclaimerTrigger.name = "DesoloZatnas/DisclaimerTrigger"
disclaimerTrigger.placements = {
    {
        name = "default",
        data = {
            width = 32,
            height = 32,
            header = "CONTENT WARNING",
            lines = ""
        }
    },
    {
        name = "flashing_lights",
        data = {
            width = 32,
            height = 32,
            header = "PHOTOSENSITIVITY WARNING",
            lines = "This section contains flashing lights|and rapid visual effects.|Player discretion is advised.|Press CONFIRM to continue..."
        }
    },
    {
        name = "mature_content",
        data = {
            width = 32,
            height = 32,
            header = "CONTENT ADVISORY",
            lines = "This section contains mature themes|and references to various media.|Press CONFIRM to continue..."
        }
    }
}

disclaimerTrigger.fieldInformation = {
    header = {
        fieldType = "string"
    },
    lines = {
        fieldType = "string"
    }
}

disclaimerTrigger.fieldOrder = {
    "x", "y", "width", "height", "header", "lines"
}

function disclaimerTrigger.sprite(room, entity)
    local width = entity.width or 32
    local height = entity.height or 32
    
    -- Red/orange color to indicate warning trigger
    local rectangles = {
        {
            x = 0,
            y = 0,
            width = width,
            height = height,
            color = {1.0, 0.4, 0.2, 0.5}  -- Orange-red
        }
    }
    
    return rectangles
end

function disclaimerTrigger.selection(room, entity)
    return {
        x = entity.x,
        y = entity.y,
        width = entity.width or 32,
        height = entity.height or 32
    }
end

return disclaimerTrigger
