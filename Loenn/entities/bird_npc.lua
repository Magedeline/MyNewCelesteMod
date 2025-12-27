local birdNpcMod = {}

birdNpcMod.name = "Ingeste/BirdNPCMod"
birdNpcMod.depth = -1000000
birdNpcMod.nodeLineRenderType = "line"
birdNpcMod.justification = {0.5, 1.0}
birdNpcMod.texture = "characters/bird/crow00"
birdNpcMod.nodeLimits = {0, -1}

birdNpcMod.fieldInformation = {
    mode = {
        fieldType = "string",
        options = {
            "ClimbingTutorial",
            "DashingTutorial", 
            "DreamJumpTutorial",
            "SuperWallJumpTutorial",
            "HyperJumpTutorial",
            "FlyAway",
            "None",
            "Sleeping",
            "MoveToNodes",
            "WaitForLightningOff"
        },
        editable = true
    },
    autoFly = {
        fieldType = "boolean"
    },
    flyAwayUp = {
        fieldType = "boolean"
    },
    waitForLightningPostDelay = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 10.0
    },
    disableFlapSfx = {
        fieldType = "boolean"
    },
    onlyOnce = {
        fieldType = "boolean"
    },
    onlyIfPlayerLeft = {
        fieldType = "boolean"
    }
}

birdNpcMod.placements = {
    {
        name = "sleeping",
        data = {
            mode = "Sleeping",
            autoFly = false,
            flyAwayUp = true,
            waitForLightningPostDelay = 0.0,
            disableFlapSfx = false,
            onlyOnce = false,
            onlyIfPlayerLeft = false
        }
    },
    {
        name = "climbing_tutorial",
        data = {
            mode = "ClimbingTutorial",
            autoFly = false,
            flyAwayUp = true,
            waitForLightningPostDelay = 0.0,
            disableFlapSfx = false,
            onlyOnce = true,
            onlyIfPlayerLeft = false
        }
    },
    {
        name = "dashing_tutorial",
        data = {
            mode = "DashingTutorial",
            autoFly = false,
            flyAwayUp = true,
            waitForLightningPostDelay = 0.0,
            disableFlapSfx = false,
            onlyOnce = true,
            onlyIfPlayerLeft = false
        }
    },
    {
        name = "fly_away",
        data = {
            mode = "FlyAway",
            autoFly = true,
            flyAwayUp = true,
            waitForLightningPostDelay = 0.0,
            disableFlapSfx = false,
            onlyOnce = false,
            onlyIfPlayerLeft = false
        }
    }
}

local modeFacingScale = {
    climbingtutorial = -1,
    dashingtutorial = 1,
    dreamjumptutorial = 1,
    superwalljumptutorial = -1,
    hyperjumptutorial = -1,
    movetonodes = -1,
    waitforlightningoff = -1,
    flyaway = -1,
    sleeping = 1,
    none = -1
}

function birdNpcMod.scale(room, entity)
    local mode = string.lower(entity.mode or "sleeping")
    return modeFacingScale[mode] or -1, 1
end

return birdNpcMod