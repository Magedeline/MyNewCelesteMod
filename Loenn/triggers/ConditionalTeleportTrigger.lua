-- Loenn integration for Conditional Teleport Trigger
-- Allows teleportation with requirements (flags, collectibles, etc.)

local conditionalTeleportTrigger = {}

conditionalTeleportTrigger.name = "Ingeste/ConditionalTeleportTrigger"
conditionalTeleportTrigger.depth = 0

conditionalTeleportTrigger.fieldInformation = {
    targetLevel = {
        fieldType = "string",
        options = {},
        editable = true
    },
    spawnPoint = {
        fieldType = "string", 
        options = {},
        editable = true
    },
    requiredFlag = {
        fieldType = "string",
        options = {},
        editable = true
    },
    completionFlag = {
        fieldType = "string",
        options = {},
        editable = true
    },
    requiredStrawberries = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 999
    },
    checkInventory = {
        fieldType = "boolean"
    }
}

conditionalTeleportTrigger.placements = {
    {
        name = "flag_required",
        data = {
            width = 32,
            height = 32,
            targetLevel = "1-A",
            spawnPoint = "main",
            requiredFlag = "key_obtained",
            completionFlag = "level_unlocked",
            checkInventory = false,
            requiredStrawberries = 0
        }
    },
    {
        name = "strawberry_gate",
        data = {
            width = 32,
            height = 32,
            targetLevel = "bonus_level",
            spawnPoint = "start",
            requiredFlag = "",
            completionFlag = "bonus_accessed",
            checkInventory = true,
            requiredStrawberries = 10
        }
    },
    {
        name = "simple_teleport",
        data = {
            width = 24,
            height = 24,
            targetLevel = "next_area",
            spawnPoint = "beginning",
            requiredFlag = "",
            completionFlag = "",
            checkInventory = false,
            requiredStrawberries = 0
        }
    }
}

function conditionalTeleportTrigger.fieldOrder(entity)
    return {
        "x", "y", "width", "height",
        "targetLevel", "spawnPoint",
        "requiredFlag", "completionFlag", 
        "checkInventory", "requiredStrawberries"
    }
end

return conditionalTeleportTrigger