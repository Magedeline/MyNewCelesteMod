local void_gate = {}

void_gate.name = "DesoloZantas/VoidGate"
void_gate.depth = 0
void_gate.canResize = {true, true}
void_gate.placements = {
    {
        name = "Void Gate",
        data = {
            width = 16,
            height = 48,
            requiredDefeats = 1,
            gateId = ""
        }
    },
    {
        name = "Void Gate (Large)",
        data = {
            width = 32,
            height = 64,
            requiredDefeats = 3,
            gateId = ""
        }
    }
}

void_gate.fieldInformation = {
    requiredDefeats = {
        fieldType = "integer",
        minimumValue = 1,
        maximumValue = 10
    }
}

function void_gate.sprite(room, entity)
    local sprites = {}
    local width = entity.width or 16
    local height = entity.height or 48
    
    local drawableRectangle = require("structs.drawable_rectangle")
    
    -- Dark void fill
    local fillRect = drawableRectangle.fromRectangle("fill", entity.x, entity.y, width, height, {0.2, 0.0, 0.2, 0.9})
    for _, sprite in ipairs(fillRect:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end
    
    -- Border
    local borderRect = drawableRectangle.fromRectangle("line", entity.x, entity.y, width, height, {0.6, 0.0, 0.6, 1.0})
    for _, sprite in ipairs(borderRect:getDrawableSprite()) do
        table.insert(sprites, sprite)
    end
    
    return sprites
end

function void_gate.rectangle(room, entity)
    return {
        x = entity.x,
        y = entity.y,
        width = entity.width or 16,
        height = entity.height or 48
    }
end

return void_gate
