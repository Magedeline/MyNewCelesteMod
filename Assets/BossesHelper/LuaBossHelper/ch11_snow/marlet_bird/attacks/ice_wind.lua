--[[
    Marlet Possessed Bird Boss - Ice Wind Attack
    Creates a freezing wind that pushes player and spawns ice crystals
]]

function onBegin()
    helpers.playPuppetAnim("wind_charge")
    wait(0.6)
    
    helpers.playPuppetAnim("wind_blast")
    playSound("event:/marlet_ice_wind")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local windDir = playerPos.X > bossPos.X and 1 or -1
    
    -- Create ice wind effect for 2 seconds
    local duration = 2.0
    local startTime = helpers.engine.RawElapsedTime
    local crystalSpawnTimer = 0
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        -- Push player
        player.Speed = vector2(
            player.Speed.X + windDir * 200 * 0.016,
            player.Speed.Y
        )
        
        -- Spawn ice crystals periodically
        crystalSpawnTimer = crystalSpawnTimer + 0.016
        if crystalSpawnTimer >= 0.3 then
            crystalSpawnTimer = 0
            
            local crystalY = bossPos.Y + (math.random() - 0.5) * 80
            local hitbox = helpers.getCircle(8)
            local crystal = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + windDir * 30, crystalY),
                hitbox,
                "characters/marlet_bird/ice_crystal",
                true
            )
            
            crystal.Speed = vector2(windDir * 180, 0)
            crystal:Add(helpers.getEntityTimer(2.5))
            helpers.addEntity(crystal)
        end
        
        wait(0.016)
    end
    
    helpers.playPuppetAnim("idle")
end
