--[[
    Dark the Dark Matter Boss - Dimensional Rift Attack
    Opens rifts that spawn dark projectiles
]]

function onBegin()
    helpers.playPuppetAnim("rift_open")
    playSound("event:/darkmatter_rift")
    wait(0.6)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    -- Create 3 rifts around the arena
    local riftPositions = {
        vector2(bossPos.X - 120, bossPos.Y - 80),
        vector2(bossPos.X + 120, bossPos.Y - 80),
        vector2(playerPos.X, bossPos.Y - 150)
    }
    
    for i, riftPos in ipairs(riftPositions) do
        local rift = helpers.getNewBasicAttackEntity(
            riftPos,
            helpers.getHitbox(1, 1), -- No collision
            "characters/darkmatter/rift",
            false
        )
        helpers.storeObjectInBoss("rift_" .. i, rift)
        rift:Add(helpers.getEntityTimer(4.0))
        helpers.addEntity(rift)
    end
    
    helpers.playPuppetAnim("rift_maintain")
    
    -- Spawn projectiles from rifts
    local duration = 3.0
    local startTime = helpers.engine.RawElapsedTime
    local spawnTimer = 0
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        spawnTimer = spawnTimer + 0.016
        if spawnTimer >= 0.4 then
            spawnTimer = 0
            
            -- Random rift spawns projectile
            local riftIndex = math.random(1, 3)
            local rift = helpers.getStoredObjectFromBoss("rift_" .. riftIndex)
            if rift and rift.Scene then
                local proj = helpers.getNewBasicAttackActor(
                    rift.Position,
                    helpers.getCircle(8),
                    "characters/darkmatter/dark_projectile",
                    0.5, 120, true, false
                )
                
                -- Aim at player
                local dirX = player.Position.X - rift.Position.X
                local dirY = player.Position.Y - rift.Position.Y
                local length = math.sqrt(dirX * dirX + dirY * dirY)
                proj.Speed = vector2(dirX/length * 150, dirY/length * 150)
                
                proj:Add(helpers.getEntityTimer(3.0))
                helpers.addEntity(proj)
                playSound("event:/darkmatter_projectile")
            end
        end
        wait(0.016)
    end
    
    -- Cleanup
    for i = 1, 3 do
        helpers.deleteStoredObjectFromBoss("rift_" .. i)
    end
    
    helpers.playPuppetAnim("idle")
end
