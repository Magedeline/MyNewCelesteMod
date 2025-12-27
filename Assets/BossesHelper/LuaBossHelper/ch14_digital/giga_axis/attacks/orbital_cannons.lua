--[[
    Giga Axis Boss - Orbital Cannons Attack
]]

function onBegin()
    helpers.playPuppetAnim("summon_cannons")
    playSound("event:/giga_axis_cannons")
    wait(0.6)
    
    local bossPos = puppet.Position
    local numCannons = 4
    
    -- Summon orbital cannons
    for i = 1, numCannons do
        local angle = (i / numCannons) * math.pi * 2
        local cannon = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + math.cos(angle) * 120, bossPos.Y + math.sin(angle) * 120),
            helpers.getCircle(20),
            "characters/giga_axis/orbital_cannon",
            true
        )
        cannon:Add(helpers.getEntityTimer(5.0))
        helpers.storeObjectInBoss("cannon_" .. i, cannon)
        helpers.addEntity(cannon)
    end
    
    helpers.playPuppetAnim("cannon_control")
    
    -- Cannons fire at player sequentially
    for volley = 1, 3 do
        for i = 1, numCannons do
            local cannon = helpers.getStoredObjectFromBoss("cannon_" .. i)
            if cannon and cannon.Scene then
                local playerPos = player.Position
                local dir = helpers.normalize(vector2(playerPos.X - cannon.Position.X, playerPos.Y - cannon.Position.Y))
                local shot = helpers.getNewBasicAttackEntity(
                    cannon.Position,
                    helpers.getCircle(10),
                    "characters/giga_axis/cannon_shot",
                    true
                )
                shot.Speed = vector2(dir.X * 280, dir.Y * 280)
                shot:Add(helpers.getEntityTimer(2.0))
                helpers.addEntity(shot)
                playSound("event:/giga_axis_cannon_fire")
            end
            wait(0.2)
        end
        wait(0.5)
    end
    
    for i = 1, numCannons do
        helpers.deleteStoredObjectFromBoss("cannon_" .. i)
    end
    
    helpers.playPuppetAnim("idle")
end
