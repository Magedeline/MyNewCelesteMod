--[[
    King Digital Dee Dee Dee Boss - Super Jump Attack
]]

function onBegin()
    helpers.playPuppetAnim("crouch")
    playSound("event:/king_ddd_charge")
    wait(0.5)
    
    helpers.playPuppetAnim("jump")
    playSound("event:/king_ddd_jump")
    helpers.setSpeed(0, -600)
    wait(0.8)
    
    -- Target player X position
    local targetX = player.Position.X
    puppet.Position = vector2(targetX, puppet.Position.Y)
    
    helpers.playPuppetAnim("drop")
    playSound("event:/king_ddd_drop")
    helpers.setSpeed(0, 700)
    helpers.keepSpeedDuring(0.6)
    wait(0.6)
    
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("land")
    playSound("event:/king_ddd_land")
    level.Shake(1.0)
    
    -- Impact shockwave
    local bossPos = puppet.Position
    for side = -1, 1, 2 do
        for i = 1, 6 do
            local wave = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + side * i * 30, bossPos.Y + 10),
                helpers.getHitbox(25, 40, -12, -20),
                "characters/king_ddd/impact_wave",
                true
            )
            wave.Speed = vector2(side * 180, 0)
            wave:Add(helpers.getEntityTimer(1.0))
            helpers.addEntity(wave)
        end
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
