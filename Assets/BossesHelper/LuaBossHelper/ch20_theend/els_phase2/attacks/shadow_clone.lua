--[[
    Els Phase 2 Boss - Shadow Clone Attack
]]

function onBegin()
    helpers.playPuppetAnim("clone_summon")
    playSound("event:/els_p2_clone")
    wait(0.5)
    
    local bossPos = puppet.Position
    
    -- Create shadow clones
    local clonePositions = {
        {-100, -50},
        {100, -50},
        {0, 100}
    }
    
    for i, pos in ipairs(clonePositions) do
        local clone = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + pos[1], bossPos.Y + pos[2]),
            helpers.getCircle(25),
            "characters/els/shadow_clone",
            true
        )
        clone:Add(helpers.getEntityTimer(4.0))
        helpers.storeObjectInBoss("clone_" .. i, clone)
        helpers.addEntity(clone)
    end
    
    helpers.playPuppetAnim("clone_control")
    
    -- Clones dash at player
    for wave = 1, 3 do
        for i = 1, 3 do
            local clone = helpers.getStoredObjectFromBoss("clone_" .. i)
            if clone and clone.Scene then
                local playerPos = player.Position
                local dir = helpers.normalize(vector2(playerPos.X - clone.Position.X, playerPos.Y - clone.Position.Y))
                clone.Speed = vector2(dir.X * 280, dir.Y * 280)
            end
        end
        wait(0.5)
        
        for i = 1, 3 do
            local clone = helpers.getStoredObjectFromBoss("clone_" .. i)
            if clone and clone.Scene then
                clone.Speed = vector2(0, 0)
            end
        end
        wait(0.3)
    end
    
    for i = 1, 3 do
        helpers.deleteStoredObjectFromBoss("clone_" .. i)
    end
    
    helpers.playPuppetAnim("idle")
end
