--[[
    Giga Axis Boss - Mega Laser Array Attack
]]

function onBegin()
    helpers.playPuppetAnim("array_charge")
    playSound("event:/giga_axis_array_charge")
    level.Shake(0.3)
    wait(0.8)
    
    helpers.playPuppetAnim("array_fire")
    playSound("event:/giga_axis_array")
    level.Shake(0.5)
    
    local bossPos = puppet.Position
    
    -- Fire lasers in multiple directions
    for wave = 1, 3 do
        for i = 1, 8 do
            local angle = math.rad(i * 45 + wave * 15)
            local laser = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getHitbox(300, 12, 0, -6),
                "characters/giga_axis/mega_laser",
                true
            )
            laser.Speed = vector2(math.cos(angle) * 350, math.sin(angle) * 350)
            laser:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(laser)
        end
        wait(0.3)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
