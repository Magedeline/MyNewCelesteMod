--[[
    Wispy Woods Boss - Leaf Storm Attack
    Creates a storm of leaves that swirl around the arena
]]

function onBegin()
    -- Play storm charge animation
    helpers.playPuppetAnim("storm_charge")
    wait(0.8)
    
    helpers.playPuppetAnim("storm_active")
    playSound("event:/wispywoods_leaf_storm")
    
    local bossPos = puppet.Position
    local numLeaves = 12
    
    -- Spawn leaves in a circle pattern
    for i = 1, numLeaves do
        local angle = (i / numLeaves) * math.pi * 2
        local radius = 80
        
        local leafPos = vector2(
            bossPos.X + math.cos(angle) * radius,
            bossPos.Y - 40 + math.sin(angle) * radius
        )
        
        local hitbox = helpers.getCircle(10)
        local leaf = helpers.getNewBasicAttackEntity(
            leafPos,
            hitbox,
            "characters/wispywoods/leaf",
            true
        )
        
        -- Store initial angle for rotation
        helpers.storeObjectInBoss("leaf_" .. i .. "_angle", angle)
        helpers.storeObjectInBoss("leaf_" .. i, leaf)
        
        leaf:Add(helpers.getEntityTimer(4.0))
        helpers.addEntity(leaf)
    end
    
    -- Animate leaves spinning for 3 seconds
    local startTime = helpers.engine.RawElapsedTime
    local duration = 3.0
    local spinSpeed = 2.0 -- radians per second
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        for i = 1, numLeaves do
            local leaf = helpers.getStoredObjectFromBoss("leaf_" .. i)
            if leaf and leaf.Scene then
                local baseAngle = helpers.getStoredObjectFromBoss("leaf_" .. i .. "_angle")
                local currentAngle = baseAngle + (helpers.engine.RawElapsedTime - startTime) * spinSpeed
                local radius = 80 + math.sin(currentAngle * 3) * 20 -- Pulsing radius
                
                leaf.Position = vector2(
                    bossPos.X + math.cos(currentAngle) * radius,
                    bossPos.Y - 40 + math.sin(currentAngle) * radius
                )
            end
        end
        wait(0.016) -- ~60fps update
    end
    
    -- Clean up stored objects
    for i = 1, numLeaves do
        helpers.deleteStoredObjectFromBoss("leaf_" .. i .. "_angle")
        helpers.deleteStoredObjectFromBoss("leaf_" .. i)
    end
    
    helpers.playPuppetAnim("idle")
end
