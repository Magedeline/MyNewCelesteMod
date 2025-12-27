--[[
    Wispy Woods Boss - Drop Apples Attack
    Drops poisonous apples from above
]]

function onBegin()
    -- Play shake animation
    helpers.playPuppetAnim("shake")
    wait(0.3)
    
    local bossPos = puppet.Position
    
    -- Drop 5 apples at random positions above the arena
    for i = 1, 5 do
        local offsetX = (math.random() - 0.5) * 200
        local spawnPos = vector2(bossPos.X + offsetX, bossPos.Y - 100)
        
        local hitbox = helpers.getHitbox(16, 16, -8, -8)
        local apple = helpers.getNewBasicAttackActor(
            spawnPos,
            hitbox,
            "characters/wispywoods/apple",
            1.0,  -- gravity multiplier
            160,  -- max fall speed
            true, -- start collidable
            true  -- solid collidable
        )
        
        -- Apple destroys on ground collision
        apple:Add(helpers.getEntityChecker(
            function() return apple.OnGround end,
            function(entity)
                -- Spawn splatter effect
                playSound("event:/wispywoods_apple_splat")
                helpers.destroyEntity(entity)
            end,
            true,
            true
        ))
        
        helpers.addEntity(apple)
        wait(0.2)
    end
    
    -- Return to idle
    helpers.playPuppetAnim("idle")
end
