--[[
    King Titan Boss - Summon Guards Attack
]]

function onBegin()
    helpers.playPuppetAnim("summon")
    playSound("event:/king_titan_summon")
    wait(0.8)
    
    local bossPos = puppet.Position
    
    -- Summon knight guardians
    local guardPositions = {
        {-150, 50},
        {150, 50},
        {-100, -100},
        {100, -100}
    }
    
    for i, pos in ipairs(guardPositions) do
        local guard = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + pos[1], bossPos.Y + pos[2]),
            helpers.getCircle(24),
            "characters/king_titan/knight_guard",
            true
        )
        guard:Add(helpers.getEntityTimer(5.0))
        helpers.storeObjectInBoss("guard_" .. i, guard)
        helpers.addEntity(guard)
        playSound("event:/king_titan_guard_spawn")
    end
    
    helpers.playPuppetAnim("command")
    
    -- Guards attack in sequence
    for wave = 1, 3 do
        for i, pos in ipairs(guardPositions) do
            local guard = helpers.getStoredObjectFromBoss("guard_" .. i)
            if guard and guard.Scene then
                local playerPos = player.Position
                local dir = helpers.normalize(vector2(playerPos.X - guard.Position.X, playerPos.Y - guard.Position.Y))
                guard.Speed = vector2(dir.X * 200, dir.Y * 200)
            end
        end
        wait(0.6)
        
        -- Stop guards
        for i = 1, 4 do
            local guard = helpers.getStoredObjectFromBoss("guard_" .. i)
            if guard and guard.Scene then
                guard.Speed = vector2(0, 0)
            end
        end
        wait(0.3)
    end
    
    for i = 1, 4 do
        helpers.deleteStoredObjectFromBoss("guard_" .. i)
    end
    
    helpers.playPuppetAnim("idle")
end
