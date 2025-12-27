--[[
    King Digital Dee Dee Dee Boss - Digital Dededestruction Attack
]]

function onBegin()
    helpers.playPuppetAnim("ultimate_charge")
    playSound("event:/king_ddd_ultimate_charge")
    level.Shake(1.0)
    wait(1.2)
    
    helpers.playPuppetAnim("ultimate")
    playSound("event:/king_ddd_ultimate")
    level.Shake(1.5)
    
    local bossPos = puppet.Position
    
    -- Multiple hammer strikes from above
    for strike = 1, 5 do
        local xPos = bossPos.X - 150 + strike * 60
        
        -- Warning indicator
        wait(0.3)
        
        -- Hammer falls
        local hammer = helpers.getNewBasicAttackEntity(
            vector2(xPos, bossPos.Y - 300),
            helpers.getHitbox(60, 80, -30, -40),
            "characters/king_ddd/mega_hammer",
            true
        )
        hammer.Speed = vector2(0, 500)
        hammer:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(hammer)
        playSound("event:/king_ddd_hammer_drop")
    end
    
    wait(1.0)
    helpers.playPuppetAnim("idle")
end
