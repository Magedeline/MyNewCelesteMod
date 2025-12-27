-- Asriel Angel of Death Boss - Lua Entity Definition for Lönn
local asrielAngelOfDeath = {}

asrielAngelOfDeath.name = "DesoloZatnas/AsrielAngelOfDeathBoss"
asrielAngelOfDeath.depth = 0
asrielAngelOfDeath.justification = {0.5, 0.5}
asrielAngelOfDeath.texture = "characters/asrielangelofdeathboss/idle00"

asrielAngelOfDeath.placements = {
    {
        name = "asriel_angelofdeath",
        data = {
            startPhase = 1,
            enableCosmowing = true,
            enablePhase2 = true,
            healthPhase1 = 100,
            healthPhase2 = 150,
            musicEvent = "event:/music/boss/asriel_angelofdeath"
        }
    }
}

asrielAngelOfDeath.fieldInformation = {
    startPhase = {
        fieldType = "integer",
        options = {1, 2},
        editable = false
    },
    enableCosmowing = {
        fieldType = "boolean"
    },
    enablePhase2 = {
        fieldType = "boolean"
    },
    healthPhase1 = {
        fieldType = "integer",
        minimumValue = 1
    },
    healthPhase2 = {
        fieldType = "integer",
        minimumValue = 1
    },
    musicEvent = {
        fieldType = "string"
    }
}

return asrielAngelOfDeath
