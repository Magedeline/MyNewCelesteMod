--[[
    Tower Titan Boss - Tidal Wave Attack
    Summons a massive wave of water across the arena
]]

function onBegin()
    helpers.playPuppetAnim("wave_summon")
    playSound("event:/tower_titan_wave_charge")
    wait(1.0)
    
    helpers.playPuppetAnim("wave_release")
    playSound("event:/tower_titan_wave")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local waveDir = playerPos.X > bossPos.X and 1 or -1
    
    -- Create massive wave
    local wave = helpers.getNewBasicAttackEntity(
        vector2(bossPos.X + waveDir * 50, bossPos.Y - 40),
        helpers.getHitbox(80, 100, -40, -100),
        "characters/tower_titan/tidal_wave",
        true
    )
    wave.Speed = vector2(waveDir * 200, 0)
    wave:Add(helpers.getEntityTimer(3.0))
    helpers.addEntity(wave)
    
    -- Smaller waves follow
    for i = 1, 3 do
        wait(0.3)
        local smallWave = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + waveDir * 50, bossPos.Y - 20),
            helpers.getHitbox(40, 50, -20, -50),
            "characters/tower_titan/small_wave",
            true
        )
        smallWave.Speed = vector2(waveDir * 220, 0)
        smallWave:Add(helpers.getEntityTimer(2.5))
        helpers.addEntity(smallWave)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
