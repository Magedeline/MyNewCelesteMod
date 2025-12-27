-- Els True Final Boss - Doppia Elillca / Penumbra Phastasm
local elsTrueFinalBoss = {}

elsTrueFinalBoss.name = "Ingeste/ElsTrueFinalBoss"
elsTrueFinalBoss.depth = 0
elsTrueFinalBoss.texture = "characters/els/doppia_idle00"
elsTrueFinalBoss.justification = {0.5, 1.0}

elsTrueFinalBoss.placements = {
    {
        name = "els_true_final_boss",
        data = {
            health = 3000,
            maxHealth = 3000,
            startPhase = 1,
            enableDefense = true,
            enableHealing = true,
            enableDimensionRift = true,
            defenseDuration = 5.0,
            healCooldown = 15.0,
            defenseReduction = 0.75
        }
    },
    {
        name = "phase_2_only",
        data = {
            health = 1500,
            maxHealth = 3000,
            startPhase = 2,
            enableDefense = true,
            enableHealing = true,
            enableDimensionRift = true,
            defenseDuration = 6.0,
            healCooldown = 15.0,
            defenseReduction = 0.75
        }
    },
    {
        name = "nightmare_mode",
        data = {
            health = 5000,
            maxHealth = 5000,
            startPhase = 1,
            enableDefense = true,
            enableHealing = true,
            enableDimensionRift = true,
            defenseDuration = 8.0,
            healCooldown = 10.0,
            defenseReduction = 0.85
        }
    }
}

elsTrueFinalBoss.fieldInformation = {
    startPhase = {
        options = {1, 2},
        editable = false
    },
    defenseDuration = {
        fieldType = "number",
        minimumValue = 0.0,
        editable = true
    },
    healCooldown = {
        fieldType = "number",
        minimumValue = 0.0,
        editable = true
    },
    defenseReduction = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 1.0,
        editable = true
    }
}

return elsTrueFinalBoss
