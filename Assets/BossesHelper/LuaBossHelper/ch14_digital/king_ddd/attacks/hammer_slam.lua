--[[
    King Digital Dee Dee Dee Boss - Hammer Slam Attack
]]

function onBegin()
    helpers.playPuppetAnim("hammer_raise")
    playSound("event:/king_ddd_hammer_raise")
    wait(0.5)
    
    helpers.playPuppetAnim("hammer_slam")
    playSound("event:/king_ddd_slam")
    level.Shake(0.8)
    
    local bossPos = puppet.Position
    
    -- Ground shockwave
    for side = -1, 1, 2 do
        for i = 1, 8 do
            local wave = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + side * i * 40, bossPos.Y + 20),
                helpers.getHitbox(35, 50, -17, -25),
                "characters/king_ddd/shockwave",
                true
            )
            wave.Speed = vector2(side * 200, 0)
            wave:Add(helpers.getEntityTimer(1.5))
            helpers.addEntity(wave)
            wait(0.05)
        end
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
