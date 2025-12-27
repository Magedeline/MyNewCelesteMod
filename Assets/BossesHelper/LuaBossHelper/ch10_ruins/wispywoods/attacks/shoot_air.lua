--[[
    Wispy Woods Boss - Shoot Air Attack
    Fires air puffs that push the player back
]]

function onBegin()
    -- Play wind-up animation
    helpers.playPuppetAnim("wind_charge")
    wait(0.5)
    
    -- Get player position for targeting
    local playerPos = player.Position
    local bossPos = puppet.Position
    
    -- Calculate direction towards player
    local dirX = playerPos.X > bossPos.X and 1 or -1
    
    -- Fire 3 air puffs in a spread pattern
    for i = -1, 1 do
        local angle = i * 15 -- degrees spread
        local radians = math.rad(angle)
        
        local hitbox = helpers.getCircle(16)
        local airPuff = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + (dirX * 40), bossPos.Y - 20 + (i * 20)),
            hitbox,
            "characters/wispywoods/air_puff",
            true
        )
        
        -- Add timer to destroy after 3 seconds
        airPuff:Add(helpers.getEntityTimer(3.0))
        
        -- Set velocity
        airPuff.Speed = vector2(dirX * 180 * math.cos(radians), 50 * math.sin(radians))
        
        helpers.addEntity(airPuff)
        wait(0.1)
    end
    
    -- Play attack animation
    helpers.playPuppetAnim("shoot")
    wait(0.3)
    
    -- Return to idle
    helpers.playPuppetAnim("idle")
end
