--[[
    Wispy Woods Boss - Inhale Attack
    Sucks in the player towards the boss
]]

function onBegin()
    -- Play inhale start animation
    helpers.playPuppetAnim("inhale_start")
    wait(0.4)
    
    helpers.playPuppetAnim("inhale_loop")
    playSound("event:/wispywoods_inhale")
    
    local bossPos = puppet.Position
    local duration = 2.5
    local startTime = helpers.engine.RawElapsedTime
    local pullStrength = 120
    
    -- Create suction hitbox (doesn't kill, just pulls)
    local suctionHitbox = helpers.getHitbox(200, 100, -200, -50)
    local suctionZone = helpers.getNewBasicAttackEntity(
        vector2(bossPos.X, bossPos.Y - 30),
        suctionHitbox,
        nil, -- No sprite
        false -- Not immediately collidable (we handle pull manually)
    )
    suctionZone:Add(helpers.getEntityTimer(duration))
    helpers.addEntity(suctionZone)
    
    -- Apply pull effect to player
    while helpers.engine.RawElapsedTime - startTime < duration do
        local playerPos = player.Position
        local dirX = bossPos.X - playerPos.X
        local distance = math.abs(dirX)
        
        if distance < 200 and distance > 30 then
            local normalizedDir = dirX > 0 and 1 or -1
            local pullForce = (1 - distance / 200) * pullStrength
            
            -- Apply horizontal pull (player will resist somewhat)
            player.Speed = vector2(
                player.Speed.X + normalizedDir * pullForce * 0.016,
                player.Speed.Y
            )
        end
        
        wait(0.016)
    end
    
    helpers.playPuppetAnim("inhale_end")
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
