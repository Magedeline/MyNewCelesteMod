local blackholeAngelBoss = {}

blackholeAngelBoss.name = "Ingeste/BlackholeAngelBoss"
blackholeAngelBoss.depth = 0
blackholeAngelBoss.canResize = {false, false}
blackholeAngelBoss.placements = {
    {
        name = "blackhole_angel_boss",
        data = {
            bossType = "BlackholeAngel",
            tier = 6,
            gimmick = 6,
            health = 1500,
            speed = 120.0,
            arenaRadius = 300.0,
            attackCooldown = 1.0
        }
    }
}

blackholeAngelBoss.fieldInformation = {
    tier = {
        options = {
            ["Lowest"] = 1,
            ["Low"] = 2,
            ["Mid"] = 3,
            ["High"] = 4,
            ["Highest"] = 5,
            ["Final"] = 6
        },
        editable = false
    },
    gimmick = {
        options = {
            ["None"] = 0,
            ["Teleport"] = 1,
            ["Time Freeze"] = 2,
            ["Shield Breaker"] = 3,
            ["Elemental Fusion"] = 4,
            ["Gravity Control"] = 5,
            ["Dimension Rift"] = 6
        },
        editable = false
    }
}

blackholeAngelBoss.texture = "objects/boss/blackhole_angel"
blackholeAngelBoss.justification = {0.5, 1.0}

return blackholeAngelBoss