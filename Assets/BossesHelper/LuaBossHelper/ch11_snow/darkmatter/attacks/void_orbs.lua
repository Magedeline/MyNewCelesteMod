--[[
    Dark the Dark Matter Boss - Void Orbs Attack
    Summons orbs of void energy that home in on the player
]]

function onBegin()
    helpers.playPuppetAnim("orb_summon")
    playSound("event:/darkmatter_orbs")
    wait(0.5)
    
    local bossPos = puppet.Position
    local numOrbs = 5
    
    -- Spawn orbs in a circle
    for i = 1, numOrbs do
        local angle = (i / numOrbs) * math.pi * 2
        local orbPos = vector2(
            bossPos.X + math.cos(angle) * 50,
            bossPos.Y + math.sin(angle) * 50
        )
        
        local hitbox = helpers.getCircle(10)
        local orb = helpers.getNewBasicAttackEntity(
            orbPos,
            hitbox,
            "characters/darkmatter/void_orb",
            true
        )
        
        helpers.storeObjectInBoss("orb_" .. i, orb)
        orb:Add(helpers.getEntityTimer(5.0))
        helpers.addEntity(orb)
        wait(0.1)
    end
    
    helpers.playPuppetAnim("orb_control")
    
    -- Make orbs home in on player for 3 seconds
    local duration = 3.0
    local startTime = helpers.engine.RawElapsedTime
    local homingStrength = 100
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local playerPos = player.Position
        
        for i = 1, numOrbs do
            local orb = helpers.getStoredObjectFromBoss("orb_" .. i)
            if orb and orb.Scene then
                local dirX = playerPos.X - orb.Position.X
                local dirY = playerPos.Y - orb.Position.Y
                local length = math.sqrt(dirX * dirX + dirY * dirY)
                if length > 0 then
                    dirX = dirX / length
                    dirY = dirY / length
                    orb.Speed = vector2(
                        orb.Speed.X + dirX * homingStrength * 0.016,
                        orb.Speed.Y + dirY * homingStrength * 0.016
                    )
                    -- Cap speed
                    local speed = math.sqrt(orb.Speed.X^2 + orb.Speed.Y^2)
                    if speed > 180 then
                        orb.Speed = vector2(orb.Speed.X * 180/speed, orb.Speed.Y * 180/speed)
                    end
                end
            end
        end
        wait(0.016)
    end
    
    -- Cleanup
    for i = 1, numOrbs do
        helpers.deleteStoredObjectFromBoss("orb_" .. i)
    end
    
    helpers.playPuppetAnim("idle")
end
