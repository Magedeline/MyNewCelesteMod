--[[
    Possessed Tenna Boss - Possession Wave Attack
]]

function onBegin()
    helpers.playPuppetAnim("wave_charge")
    playSound("event:/possessed_tenna_wave_charge")
    wait(0.8)
    
    helpers.playPuppetAnim("wave_release")
    playSound("event:/possessed_tenna_wave")
    level.Shake(0.5)
    
    local bossPos = puppet.Position
    
    -- Create expanding ring
    for dir = -1, 1, 2 do
        local wave = helpers.getNewBasicAttackEntity(
            bossPos,
            helpers.getHitbox(50, 60, dir > 0 and 0 or -50, -30),
            "characters/possessed_tenna/possession_wave",
            true
        )
        wave.Speed = vector2(dir * 180, 0)
        wave:Add(helpers.getEntityTimer(2.5))
        helpers.addEntity(wave)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
