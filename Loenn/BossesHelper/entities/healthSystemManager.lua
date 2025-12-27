local healthSystemManager = {}

local crushModeOpts = {
    {"Push Out", "pushOut"},
    {"Solid On Invincible Player", "invincibleSolid"},
    {"Fake Death", "fakeDeath"},
    {"Instant Death", "instantDeath"},
    {"Use Old Value", ""}
}

local offscreenModeOpts = {
    {"Bounce Up", "bounceUp"},
    {"Bubble Back", "bubbleBack"},
    {"Fake Death", "fakeDeath"},
    {"Instant Death", "instantDeath"},
    {"Use Old Value", ""}
}

healthSystemManager.name = "DesoloZatnas/BossesHelper/HealthSystemManager"
healthSystemManager.depth = 0
healthSystemManager.texture = "loenn/DesoloZatnas/BossesHelper/HealthController"
healthSystemManager.nodeLimits = {0, 0}
healthSystemManager.fieldInformation = {
    healthIcons = {
        fieldType = "list",
        elementOptions = {
            fieldType = "string"
        }
    },
    healthIconsCreateAnim = {
        fieldType = "list",
        elementOptions = {
            fieldType = "string"
        }
    },
    healthIconsRemoveAnim = {
        fieldType = "list",
        elementOptions = {
            fieldType = "string"
        }
    },
    healthIconsSeparation = {
        fieldType = "list",
        elementDefault = "0.0",
        elementOptions = {
            validator = function (string)
                res = tonumber(string)
                return res ~= nil and res >= 0
            end
        }
    },
    playerHealth = {
        validator = function (string)
            res = tonumber(string)
            return res == nil or res > 1 and string:find("%.") == nil
        end
    },
    damageCooldown = {
        validator = function (string)
            res = tonumber(string)
            return res == nil or res >= 0
        end
    },
    healthIconScreenX = {
        validator = function (string)
            res = tonumber(string)
            return res == nil or (res >= 0 and res <= 1920)
        end
    },
    healthIconScreenY = {
        validator = function (string)
            res = tonumber(string)
            return res == nil or (res >= 0 and res <= 1080)
        end
    },
    crushEffect = {
        options = crushModeOpts,
        editable = false
    },
    offscreenEffect = {
        options = offscreenModeOpts,
        editable = false
    },
    onDamageFunction = {
        fieldType = "path",
        allowedExtensions = {"lua"},
        allowMissingPath = false
    },
    __Boss_pad = {
        fieldType = "spacer"
    }
}

healthSystemManager.fieldOrder = {
    "x", "y",
    "playerHealth", "damageCooldown",
    "crushEffect", "offscreenEffect",
    "activationFlag", "onDamageFunction",
    "frameSprite", "__Boss_pad",
    "healthIcons", "healthIconsCreateAnim",
    "healthIconsSeparation", "healthIconsRemoveAnim",
    "healthIconsScreenX", "healthIconsScreenY",
    "healthIconsScaleX", "healthIconsScaleY",
    "isGlobal", "globalHealth", "applySystemInstantly", "startVisible",
    "playerBlink", "playerStagger"
}

healthSystemManager.placements = {
    name = "Health System Manager",
    data = {
        activationFlag = "",
        healthIcons = "",
        frameSprite = "",
        healthIconsCreateAnim = "",
        healthIconsRemoveAnim = "",
        healthIconsScreenX = "160",
        healthIconsScreenY = "950",
        healthIconsScaleX = "1",
        healthIconsScaleY = "1",
        healthIconsSeparation = "20.0",
        playerHealth = "3",
        damageCooldown = "1",
        crushEffect = "instantDeath",
        offscreenEffect = "instantDeath",
        onDamageFunction = "",
        isGlobal = true,
        globalHealth = false,
        applySystemInstantly = true,
        startVisible = true,
        playerStagger = true,
        playerBlink = true,
        removeOnDamage = true
    }
}

return healthSystemManager