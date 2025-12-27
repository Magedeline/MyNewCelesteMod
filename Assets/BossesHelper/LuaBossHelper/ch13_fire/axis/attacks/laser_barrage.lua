--[[
    Axis Boss - Laser Barrage Attack
]]

function onBegin()
    helpers.playPuppetAnim("laser_charge")
    playSound("event:/axis_laser_charge")
    wait(0.5)
    
    helpers.playPuppetAnim("laser_fire")
    playSound("event:/axis_laser")
    
    local bossPos = puppet.Position
    
    for i = 1, 5 do
        local angle = math.rad(-60 + i * 24)
        local laser = helpers.getNewBasicAttackEntity(
            bossPos,
            helpers.getHitbox(200, 8, 0, -4),
            "characters/axis/laser",
            true
        )
        laser.Speed = vector2(math.cos(angle) * 300, math.sin(angle) * 300)
        laser:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(laser)
        wait(0.1)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
