--[[
    Els Phase 1 Boss - Dark Wave Attack
]]

function onBegin()
    helpers.playPuppetAnim("wave_charge")
    playSound("event:/els_p1_wave")
    wait(0.5)
    
    helpers.playPuppetAnim("wave_release")
    
    local bossPos = puppet.Position
    
    for ring = 1, 3 do
        local numProjectiles = 12
        for i = 1, numProjectiles do
            local angle = (i / numProjectiles) * math.pi * 2
            local wave = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getCircle(12),
                "characters/els/dark_wave",
                true
            )
            wave.Speed = vector2(math.cos(angle) * (150 + ring * 40), math.sin(angle) * (150 + ring * 40))
            wave:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(wave)
        end
        wait(0.3)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
