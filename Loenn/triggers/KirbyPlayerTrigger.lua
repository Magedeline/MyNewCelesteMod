-- Loenn integration for Kirby Player Trigger
-- Allows transformation between normal and Kirby player modes

local kirbyPlayerTrigger = {}

kirbyPlayerTrigger.name = "Ingeste/Kirby_Player_Trigger"

kirbyPlayerTrigger.fieldInformation = {
    activationType = {
        options = {
            "OnEnter",
            "OnExit", 
            "Toggle"
        },
        editable = false
    },
    transformationType = {
        options = {
            "Instant",
            "Animated",
            "Fade"
        },
        editable = false
    },
    oneUse = {
        fieldType = "boolean"
    },
    transformAnimation = {
        fieldType = "string"
    },
    transformDuration = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 10.0
    },
    preserveVelocity = {
        fieldType = "boolean"
    }
}

kirbyPlayerTrigger.placements = {
    {
        name = "on_enter",
        data = {
            width = 16,
            height = 16,
            activationType = "OnEnter",
            transformationType = "Animated",
            oneUse = false,
            transformAnimation = "transform_to_kirby",
            transformDuration = 1.0,
            preserveVelocity = true
        }
    },
    {
        name = "on_exit", 
        data = {
            width = 16,
            height = 16,
            activationType = "OnExit",
            transformationType = "Fade",
            oneUse = false,
            transformAnimation = "transform_to_normal",
            transformDuration = 0.5,
            preserveVelocity = true
        }
    },
    {
        name = "toggle",
        data = {
            width = 16,
            height = 16,
            activationType = "Toggle",
            transformationType = "Instant",
            oneUse = true,
            transformAnimation = "transform",
            transformDuration = 0.0,
            preserveVelocity = false
        }
    }
}

return kirbyPlayerTrigger