--[[
    King Titan Boss - Scepter Strike Attack
]]

function onBegin()
    helpers.playPuppetAnim("scepter_raise")
    playSound("event:/king_titan_scepter_charge")
    wait(0.5)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dir = playerPos.X > bossPos.X and 1 or -1
    
    helpers.playPuppetAnim("scepter_strike")
    playSound("event:/king_titan_scepter_strike")
    
    -- Wide scepter swing
    local strike = helpers.getNewBasicAttackEntity(
        vector2(bossPos.X + dir * 60, bossPos.Y),
        helpers.getHitbox(120, 80, -60, -40),
        "characters/king_titan/scepter_swing",
        true
    )
    strike:Add(helpers.getEntityTimer(0.4))
    helpers.addEntity(strike)
    
    wait(0.3)
    
    -- Ground shockwave from impact
    for i = 1, 6 do
        local wave = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + dir * (60 + i * 50), bossPos.Y + 30),
            helpers.getHitbox(40, 60, -20, -30),
            "characters/king_titan/shockwave",
            true
        )
        wave.Speed = vector2(dir * 250, 0)
        wave:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(wave)
        wait(0.08)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
