--[[
    Asriel God Boss - Hyper Goner Attack
]]

function onBegin()
    helpers.playPuppetAnim("hyper_charge")
    playSound("event:/asriel_god_hyper_charge")
    level.Shake(1.5)
    wait(1.5)
    
    helpers.playPuppetAnim("hyper_goner")
    playSound("event:/asriel_god_hyper_goner")
    level.Shake(2.0)
    
    local bossPos = puppet.Position
    
    -- Create skull beam
    local skull = helpers.getNewBasicAttackEntity(
        vector2(bossPos.X, bossPos.Y + 300),
        helpers.getCircle(60),
        "characters/asriel_god/hyper_skull",
        true
    )
    skull:Add(helpers.getEntityTimer(4.0))
    helpers.storeObjectInBoss("hyperSkull", skull)
    helpers.addEntity(skull)
    
    -- Suction effect
    local duration = 3.5
    local startTime = helpers.engine.RawElapsedTime
    local pullStrength = 350
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local s = helpers.getStoredObjectFromBoss("hyperSkull")
        if s and s.Scene then
            local pPos = player.Position
            local sPos = s.Position
            local dist = math.sqrt((pPos.X - sPos.X)^2 + (pPos.Y - sPos.Y)^2)
            
            if dist < 400 and dist > 20 then
                local dirX = (sPos.X - pPos.X) / dist
                local dirY = (sPos.Y - pPos.Y) / dist
                local force = (1 - dist / 400) * pullStrength
                player.Speed = vector2(player.Speed.X + dirX * force * 0.016, player.Speed.Y + dirY * force * 0.016)
            end
        end
        wait(0.016)
    end
    
    helpers.deleteStoredObjectFromBoss("hyperSkull")
    helpers.playPuppetAnim("idle")
end
