-- Asriel Angel of Death Boss
local asrielAngelBoss = {}

asrielAngelBoss.name = "Ingeste/AsrielAngelOfDeathBoss"
asrielAngelBoss.depth = 0
asrielAngelBoss.texture = "characters/asriel/angel_idle00"
asrielAngelBoss.justification = {0.5, 1.0}

asrielAngelBoss.placements = {
    {
        name = "normal",
        data = {
            health = 2000,
            maxHealth = 2000,
            enableTransformation = true,
            enableEmotionalPhase = true,
            musicTrack = "els_09"
        }
    },
    {
        name = "hard_mode",
        data = {
            health = 2500,
            maxHealth = 2500,
            enableTransformation = true,
            enableEmotionalPhase = true,
            musicTrack = "els_09"
        }
    }
}

asrielAngelBoss.fieldInformation = {
    musicTrack = {
        fieldType = "string",
        editable = true
    }
}

return asrielAngelBoss
