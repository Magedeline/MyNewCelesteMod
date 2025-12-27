--[[
    Giga Axis Boss - System Override Attack
]]

function onBegin()
    helpers.playPuppetAnim("override_charge")
    playSound("event:/giga_axis_override")
    level.Shake(0.8)
    wait(1.2)
    
    helpers.playPuppetAnim("override_pulse")
    playSound("event:/giga_axis_pulse")
    
    local bossPos = puppet.Position
    
    -- Expanding pulse rings
    for ring = 1, 4 do
        for i = 1, 16 do
            local angle = (i / 16) * math.pi * 2
            local pulse = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getCircle(15),
                "characters/giga_axis/override_pulse",
                true
            )
            pulse.Speed = vector2(math.cos(angle) * (150 + ring * 50), math.sin(angle) * (150 + ring * 50))
            pulse:Add(helpers.getEntityTimer(1.5))
            helpers.addEntity(pulse)
        end
        wait(0.4)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
