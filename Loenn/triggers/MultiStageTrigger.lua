-- Loenn integration for Multi-Stage Trigger
-- Allows complex multi-stage event sequences triggered by player

local multiStageTrigger = {}

multiStageTrigger.name = "Ingeste/MultiStageTrigger"
multiStageTrigger.depth = 0

multiStageTrigger.fieldInformation = {
    stages = {
        fieldType = "string",
        options = {},
        editable = true
    },
    resetOnLeave = {
        fieldType = "boolean"
    },
    showDebugInfo = {
        fieldType = "boolean"
    }
}

multiStageTrigger.placements = {
    {
        name = "dialog_sequence",
        data = {
            width = 32,
            height = 32,
            stages = "flag1:dialog:DIALOG_KEY_1;flag2:cutscene:CS_01;flag3:teleport:100,200",
            resetOnLeave = false,
            showDebugInfo = false
        }
    },
    {
        name = "progressive_unlock",
        data = {
            width = 48,
            height = 48,
            stages = ":unlock:door1;door1_unlocked:unlock:door2;door2_unlocked:spawn:enemy_boss",
            resetOnLeave = false,
            showDebugInfo = true
        }
    },
    {
        name = "checkpoint_sequence", 
        data = {
            width = 24,
            height = 24,
            stages = ":checkpoint:1;checkpoint_1:checkpoint:2;checkpoint_2:flag:sequence_complete",
            resetOnLeave = true,
            showDebugInfo = false
        }
    }
}

function multiStageTrigger.fieldOrder(entity)
    return {
        "x", "y", "width", "height",
        "stages",
        "resetOnLeave", 
        "showDebugInfo"
    }
end

return multiStageTrigger