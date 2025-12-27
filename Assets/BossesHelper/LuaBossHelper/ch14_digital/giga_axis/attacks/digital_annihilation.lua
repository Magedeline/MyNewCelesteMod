--[[
    Giga Axis Boss - Digital Annihilation Attack
]]

function onBegin()
    helpers.playPuppetAnim("annihilation_charge")
    playSound("event:/giga_axis_annihilation_charge")
    level.Shake(1.5)
    wait(1.5)
    
    helpers.playPuppetAnim("annihilation")
    playSound("event:/giga_axis_annihilation")
    level.Shake(2.0)
    
    local bossPos = puppet.Position
    
    -- Massive beam downward
    for i = 1, 50 do
        local yOffset = i * 15
        local beam = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X, bossPos.Y + yOffset),
            helpers.getHitbox(80, 15, -40, -7),
            "characters/giga_axis/annihilation_beam",
            true
        )
        beam:Add(helpers.getEntityTimer(1.2))
        helpers.addEntity(beam)
    end
    
    wait(0.3)
    
    -- Side sweeping beams
    for side = -1, 1, 2 do
        for i = 1, 20 do
            local xOffset = side * i * 20
            local beam = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + xOffset, bossPos.Y + 200),
                helpers.getHitbox(20, 100, -10, -50),
                "characters/giga_axis/sweep_beam",
                true
            )
            beam:Add(helpers.getEntityTimer(1.0))
            helpers.addEntity(beam)
        end
    end
    
    wait(1.0)
    helpers.playPuppetAnim("idle")
end
