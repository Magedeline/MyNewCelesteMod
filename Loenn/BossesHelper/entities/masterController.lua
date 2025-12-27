local bossMasterController = {}

local hurtModeOpts = {
    {"Player Contact", "playerContact"},
    {"Player Dash", "playerDash"},
    {"Head Bonk", "headBonk"},
    {"Sidekick Attack", "sidekickAttack"},
    {"Custom", "custom"}
}

bossMasterController.name = "DesoloZatnas/BossesHelper/BossController"
bossMasterController.depth = 0
bossMasterController.texture = "loenn/DesoloZatnas/BossesHelper/BossController"
--bossMasterController.nodeLineRenderType = "line"
bossMasterController.nodeLimits = {0, 0}
bossMasterController.fieldInformation = {
    bossHealthMax = {
        fieldType = "integer"
    },
    __Boss_pad = {
        fieldType = "spacer"
    },
    __Boss_pad_ = {
        fieldType = "spacer"
    },
    hurtMode = {
        options = hurtModeOpts,
        editable = false
    },
    bossHitCooldown = {
        fieldType = "number",
        minimumValue = 0
    },
    maxFall = {
        fieldType = "number"
    },
    baseGravityMultiplier = {
        fieldType = "number"
    },
    groundFriction = {
        fieldType = "number"
    },
    airFriction = {
        fieldType = "number"
    },
    sidekickCooldown = {
        fieldType = "number"
    },
    hitboxMetadataPath = {
        fieldType = "path",
        allowedExtensions = {"xml"},
        allowMissingPath = false
    },
    patternsPath = {
        fieldType = "path",
        allowedExtensions = {"xml"},
        allowMissingPath = false
    },
    functionsPath = {
        fieldType = "path",
        allowedExtensions = {"lua"},
        allowMissingPath = false
    },
    attacksPath = {
        fieldType = "path",
        allowFolders = true,
        allowFiles = false
    },
    eventsPath = {
        fieldType = "path",
        allowFolders = true,
        allowFiles = false
    }
}

function bossMasterController.fieldOrder(entity)
    local baseTable = {
        "x", "y",
        "bossID", "bossSprite",
        "hurtMode", "startingAnim",
        "bossHealthMax", "bossHitCooldown",
        "maxFall", "baseGravityMultiplier",
        "groundFriction", "airFriction",

        "hitboxMetadataPath", "patternsPath",
        "attacksPath", "eventsPath",
        "functionsPath", "__Boss_pad_",

        "dynamicFacing", "mirrorSprite", "killOnContact", "startAttackingImmediately",
        "startCollidable", "startSolidCollidable"
    }

    if entity.hurtMode == "sidekickAttack" then
        table.insert(baseTable, 13, "sidekickCooldown")
        table.insert(baseTable, 14, "__Boss_pad_")
        table.insert(baseTable, "sidekickFreeze")
    end

    return baseTable
end

bossMasterController.placements = {
    {
        name = "Boss Controller",
        data = {
            hitboxMetadataPath = "",
            attacksPath = "",
            eventsPath = "",
            functionsPath = "",
            patternsPath = "",
            bossID = "",
            bossSprite = "",
            startingAnim = "",
            bossHealthMax = 3,
            hurtMode = "playerContact",
            bossHitCooldown = "0.5",
            maxFall = "90",
            baseGravityMultiplier = "1.0",
            groundFriction = "0.0",
            airFriction = "0.0",
            dynamicFacing = true,
            mirrorSprite = false,
            killOnContact = false,
            startAttackingImmediately = false,
            startCollidable = true,
            startSolidCollidable = true
        }
    },
    {
        name = "Boss Controller (Sidekick Mode)",
        data = {
            hitboxMetadataPath = "",
            attacksPath = "",
            eventsPath = "",
            functionsPath = "",
            patternsPath = "",
            bossID = "",
            bossSprite = "",
            startingAnim = "",
            bossHealthMax = 3,
            hurtMode = "sidekickAttack",
            bossHitCooldown = "0.5",
            maxFall = "90",
            baseGravityMultiplier = "1.0",
            groundFriction = "0.0",
            airFriction = "0.0",
            sidekickCooldown = "5.0",
            dynamicFacing = true,
            mirrorSprite = false,
            killOnContact = false,
            startAttackingImmediately = false,
            startCollidable = true,
            startSolidCollidable = true,
            sidekickFreeze = false
        }
    }
}

return bossMasterController