--[[
    Axis Boss - Energy Orbs Attack
]]

function onBegin()
    helpers.playPuppetAnim("orb_summon")
    playSound("event:/axis_orbs")
    wait(0.4)
    
    local bossPos = puppet.Position
    local numOrbs = 6
    
    for i = 1, numOrbs do
        local angle = (i / numOrbs) * math.pi * 2
        local orb = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + math.cos(angle) * 60, bossPos.Y + math.sin(angle) * 60),
            helpers.getCircle(12),
            "characters/axis/energy_orb",
            true
        )
        orb:Add(helpers.getEntityTimer(4.0))
        helpers.storeObjectInBoss("orb_" .. i, orb)
        helpers.addEntity(orb)
    end
    
    helpers.playPuppetAnim("orb_control")
    
    -- Make orbs spiral outward then home
    local duration = 2.5
    local startTime = helpers.engine.RawElapsedTime
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local t = helpers.engine.RawElapsedTime - startTime
        for i = 1, numOrbs do
            local orb = helpers.getStoredObjectFromBoss("orb_" .. i)
            if orb and orb.Scene then
                local baseAngle = (i / numOrbs) * math.pi * 2
                local currentAngle = baseAngle + t * 3
                local radius = 60 + t * 80
                orb.Position = vector2(
                    bossPos.X + math.cos(currentAngle) * radius,
                    bossPos.Y + math.sin(currentAngle) * radius
                )
            end
        end
        wait(0.016)
    end
    
    -- Fire at player
    local playerPos = player.Position
    for i = 1, numOrbs do
        local orb = helpers.getStoredObjectFromBoss("orb_" .. i)
        if orb and orb.Scene then
            local dir = helpers.normalize(vector2(playerPos.X - orb.Position.X, playerPos.Y - orb.Position.Y))
            orb.Speed = vector2(dir.X * 200, dir.Y * 200)
        end
        helpers.deleteStoredObjectFromBoss("orb_" .. i)
    end
    
    helpers.playPuppetAnim("idle")
end
