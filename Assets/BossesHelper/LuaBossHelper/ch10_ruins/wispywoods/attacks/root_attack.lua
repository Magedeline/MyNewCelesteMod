--[[
    Wispy Woods Boss - Root Attack
    Sends roots bursting from the ground to attack the player
]]

function onBegin()
    -- Play charge animation
    helpers.playPuppetAnim("root_charge")
    wait(0.6)
    
    local playerPos = player.Position
    
    -- Spawn warning indicators first
    for i = -1, 1 do
        local offsetX = i * 60
        local warningPos = vector2(playerPos.X + offsetX, playerPos.Y + 8)
        
        -- Create ground crack warning sprite
        local warning = helpers.getNewBasicAttackEntity(
            warningPos,
            helpers.getHitbox(1, 1), -- No collision for warning
            "characters/wispywoods/root_warning",
            false -- Not collidable
        )
        warning:Add(helpers.getEntityTimer(0.5))
        helpers.addEntity(warning)
    end
    
    wait(0.5)
    playSound("event:/wispywoods_root_burst")
    
    -- Now spawn actual roots
    for i = -1, 1 do
        local offsetX = i * 60
        local rootPos = vector2(playerPos.X + offsetX, playerPos.Y)
        
        local hitbox = helpers.getHitbox(24, 64, -12, -64)
        local root = helpers.getNewBasicAttackEntity(
            rootPos,
            hitbox,
            "characters/wispywoods/root",
            true
        )
        
        -- Root stays for 1.5 seconds then retracts
        root:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(root)
    end
    
    helpers.playPuppetAnim("root_attack")
    wait(1.5)
    
    -- Return to idle
    helpers.playPuppetAnim("idle")
end
