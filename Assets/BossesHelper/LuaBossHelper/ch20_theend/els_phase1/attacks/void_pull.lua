--[[
    Els Phase 1 Boss - Void Pull Attack
]]

function onBegin()
    helpers.playPuppetAnim("void_summon")
    playSound("event:/els_p1_void")
    wait(0.6)
    
    local playerPos = player.Position
    
    local voidPortal = helpers.getNewBasicAttackEntity(
        vector2(playerPos.X, playerPos.Y + 50),
        helpers.getCircle(35),
        "characters/els/void_portal",
        true
    )
    voidPortal:Add(helpers.getEntityTimer(3.0))
    helpers.storeObjectInBoss("voidPortal", voidPortal)
    helpers.addEntity(voidPortal)
    
    helpers.playPuppetAnim("void_control")
    
    -- Pull effect
    local duration = 2.5
    local startTime = helpers.engine.RawElapsedTime
    local pullStrength = 180
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local v = helpers.getStoredObjectFromBoss("voidPortal")
        if v and v.Scene then
            local pPos = player.Position
            local vPos = v.Position
            local dist = math.sqrt((pPos.X - vPos.X)^2 + (pPos.Y - vPos.Y)^2)
            
            if dist < 180 and dist > 15 then
                local dirX = (vPos.X - pPos.X) / dist
                local dirY = (vPos.Y - pPos.Y) / dist
                local force = (1 - dist / 180) * pullStrength
                player.Speed = vector2(player.Speed.X + dirX * force * 0.016, player.Speed.Y + dirY * force * 0.016)
            end
        end
        wait(0.016)
    end
    
    helpers.deleteStoredObjectFromBoss("voidPortal")
    helpers.playPuppetAnim("idle")
end
