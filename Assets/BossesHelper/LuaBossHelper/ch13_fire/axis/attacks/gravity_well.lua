--[[
    Axis Boss - Gravity Well Attack
]]

function onBegin()
    helpers.playPuppetAnim("gravity_charge")
    playSound("event:/axis_gravity")
    wait(0.6)
    
    helpers.playPuppetAnim("gravity_create")
    
    local playerPos = player.Position
    
    -- Create gravity well at player position
    local well = helpers.getNewBasicAttackEntity(
        playerPos,
        helpers.getCircle(30),
        "characters/axis/gravity_well",
        true
    )
    well:Add(helpers.getEntityTimer(3.0))
    helpers.addEntity(well)
    helpers.storeObjectInBoss("gravityWell", well)
    
    -- Apply pull effect
    local duration = 2.5
    local startTime = helpers.engine.RawElapsedTime
    local pullStrength = 150
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local w = helpers.getStoredObjectFromBoss("gravityWell")
        if w and w.Scene then
            local pPos = player.Position
            local wPos = w.Position
            local dist = math.sqrt((pPos.X - wPos.X)^2 + (pPos.Y - wPos.Y)^2)
            
            if dist < 150 and dist > 10 then
                local dirX = (wPos.X - pPos.X) / dist
                local dirY = (wPos.Y - pPos.Y) / dist
                local force = (1 - dist / 150) * pullStrength
                player.Speed = vector2(
                    player.Speed.X + dirX * force * 0.016,
                    player.Speed.Y + dirY * force * 0.016
                )
            end
        end
        wait(0.016)
    end
    
    helpers.deleteStoredObjectFromBoss("gravityWell")
    helpers.playPuppetAnim("idle")
end
