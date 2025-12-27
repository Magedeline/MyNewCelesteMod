-- Lönn integration for Kirby Mode Toggle Trigger
-- Comprehensive trigger for managing Kirby mode transformations with multiple activation modes

local kirbyModeToggleTrigger = {}

kirbyModeToggleTrigger.name = "Ingeste/Kirby_Mode_Toggle_Trigger"

kirbyModeToggleTrigger.fieldInformation = {
    -- Activation settings
    activationMode = {
        fieldType = "string",
        options = {
            "OnEnter",      -- Activate when player enters
            "OnExit",       -- Activate when player exits
            "Toggle",       -- Toggle mode on enter
            "OnStay",       -- Activate while staying inside
            "Persistent"    -- Enable until disabled
        },
        editable = false
    },
    
    transformEffect = {
        fieldType = "string",
        options = {
            "Instant",      -- Immediate with flash
            "Sparkle",      -- Sparkle particles
            "Flash",        -- Screen flash
            "Smooth",       -- Smooth transition
            "Custom"        -- Custom effect
        },
        editable = false
    },
    
    triggerState = {
        fieldType = "string",
        options = {
            "Enable",       -- Enable Kirby mode
            "Disable",      -- Disable Kirby mode
            "Toggle"        -- Toggle current state
        },
        editable = false
    },
    
    -- Boolean settings
    oneUse = {
        fieldType = "boolean"
    },
    
    respectSettings = {
        fieldType = "boolean"
    },
    
    silentMode = {
        fieldType = "boolean"
    },
    
    -- String settings
    flagRequired = {
        fieldType = "string"
    },
    
    flagToSet = {
        fieldType = "string"
    },
    
    transformSound = {
        fieldType = "string"
    },
    
    -- Numeric settings
    effectDuration = {
        fieldType = "number",
        minimumValue = 0.1,
        maximumValue = 5.0
    },
    
    particleCount = {
        fieldType = "integer",
        minimumValue = 0,
        maximumValue = 100
    },
    
    shakeIntensity = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 1.0
    },
    
    -- Color setting
    particleColor = {
        fieldType = "color"
    },
    
    -- Audio settings
    playSound = {
        fieldType = "boolean"
    },
    
    -- Visual settings
    screenShake = {
        fieldType = "boolean"
    }
}

kirbyModeToggleTrigger.placements = {
    -- Enable Kirby Mode (OnEnter)
    {
        name = "enable_kirby_on_enter",
        data = {
            width = 16,
            height = 16,
            activationMode = "OnEnter",
            transformEffect = "Sparkle",
            triggerState = "Enable",
            oneUse = false,
            respectSettings = true,
            flagRequired = "",
            flagToSet = "",
            silentMode = false,
            effectDuration = 1.0,
            particleColor = "FFC0CB",  -- Pink
            particleCount = 30,
            screenShake = true,
            shakeIntensity = 0.3,
            transformSound = "event:/Ingeste/kirby/transform",
            playSound = true
        }
    },
    
    -- Disable Kirby Mode (OnEnter)
    {
        name = "disable_kirby_on_enter",
        data = {
            width = 16,
            height = 16,
            activationMode = "OnEnter",
            transformEffect = "Sparkle",
            triggerState = "Disable",
            oneUse = false,
            respectSettings = true,
            flagRequired = "",
            flagToSet = "",
            silentMode = false,
            effectDuration = 1.0,
            particleColor = "FFB6C1",  -- Light pink
            particleCount = 30,
            screenShake = true,
            shakeIntensity = 0.3,
            transformSound = "event:/Ingeste/kirby/transform",
            playSound = true
        }
    },
    
    -- Toggle Kirby Mode
    {
        name = "toggle_kirby_mode",
        data = {
            width = 16,
            height = 16,
            activationMode = "Toggle",
            transformEffect = "Sparkle",
            triggerState = "Toggle",
            oneUse = false,
            respectSettings = true,
            flagRequired = "",
            flagToSet = "",
            silentMode = false,
            effectDuration = 1.0,
            particleColor = "FF69B4",  -- Hot pink
            particleCount = 30,
            screenShake = true,
            shakeIntensity = 0.3,
            transformSound = "event:/Ingeste/kirby/transform",
            playSound = true
        }
    },
    
    -- One-Use Enable
    {
        name = "one_use_enable",
        data = {
            width = 16,
            height = 16,
            activationMode = "OnEnter",
            transformEffect = "Flash",
            triggerState = "Enable",
            oneUse = true,
            respectSettings = true,
            flagRequired = "",
            flagToSet = "kirby_mode_activated",
            silentMode = false,
            effectDuration = 1.5,
            particleColor = "FFC0CB",
            particleCount = 50,
            screenShake = true,
            shakeIntensity = 0.5,
            transformSound = "event:/Ingeste/kirby/transform",
            playSound = true
        }
    },
    
    -- Persistent Enable
    {
        name = "persistent_enable",
        data = {
            width = 16,
            height = 16,
            activationMode = "Persistent",
            transformEffect = "Smooth",
            triggerState = "Enable",
            oneUse = false,
            respectSettings = true,
            flagRequired = "",
            flagToSet = "",
            silentMode = false,
            effectDuration = 2.0,
            particleColor = "FFC0CB",
            particleCount = 40,
            screenShake = false,
            shakeIntensity = 0.2,
            transformSound = "event:/Ingeste/kirby/transform",
            playSound = true
        }
    },
    
    -- Silent Toggle (No effects)
    {
        name = "silent_toggle",
        data = {
            width = 16,
            height = 16,
            activationMode = "OnEnter",
            transformEffect = "Instant",
            triggerState = "Toggle",
            oneUse = false,
            respectSettings = true,
            flagRequired = "",
            flagToSet = "",
            silentMode = true,
            effectDuration = 0.1,
            particleColor = "FFC0CB",
            particleCount = 0,
            screenShake = false,
            shakeIntensity = 0.0,
            transformSound = "",
            playSound = false
        }
    },
    
    -- Custom Effect Toggle
    {
        name = "custom_effect_toggle",
        data = {
            width = 16,
            height = 16,
            activationMode = "OnEnter",
            transformEffect = "Custom",
            triggerState = "Toggle",
            oneUse = false,
            respectSettings = true,
            flagRequired = "",
            flagToSet = "",
            silentMode = false,
            effectDuration = 1.2,
            particleColor = "FF1493",  -- Deep pink
            particleCount = 36,
            screenShake = true,
            shakeIntensity = 0.4,
            transformSound = "event:/Ingeste/kirby/transform",
            playSound = true
        }
    },
    
    -- Exit Trigger
    {
        name = "disable_on_exit",
        data = {
            width = 16,
            height = 16,
            activationMode = "OnExit",
            transformEffect = "Sparkle",
            triggerState = "Disable",
            oneUse = false,
            respectSettings = true,
            flagRequired = "",
            flagToSet = "",
            silentMode = false,
            effectDuration = 1.0,
            particleColor = "FFB6C1",
            particleCount = 25,
            screenShake = false,
            shakeIntensity = 0.2,
            transformSound = "event:/Ingeste/kirby/transform",
            playSound = true
        }
    }
}

return kirbyModeToggleTrigger
