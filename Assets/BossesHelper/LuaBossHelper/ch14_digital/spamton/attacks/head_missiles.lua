--[[
    Spamton Boss - Head Missiles Attack
]]

function onBegin()
    helpers.playPuppetAnim("head_detach")
    playSound("event:/spamton_head")
    wait(0.5)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    helpers.playPuppetAnim("head_attack")
    
    -- Launch homing heads
    for i = 1, 3 do
        local head = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + (i-2) * 30, bossPos.Y - 20),
            helpers.getCircle(20),
            "characters/spamton/head_missile",
            true
        )
        head:Add(helpers.getEntityTimer(4.0))
        helpers.storeObjectInBoss("head_" .. i, head)
        helpers.addEntity(head)
    end
    
    -- Homing behavior
    local duration = 3.0
    local startTime = helpers.engine.RawElapsedTime
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local pPos = player.Position
        for i = 1, 3 do
            local head = helpers.getStoredObjectFromBoss("head_" .. i)
            if head and head.Scene then
                local dir = helpers.normalize(vector2(pPos.X - head.Position.X, pPos.Y - head.Position.Y))
                head.Speed = vector2(dir.X * 180, dir.Y * 180)
            end
        end
        wait(0.05)
    end
    
    for i = 1, 3 do
        helpers.deleteStoredObjectFromBoss("head_" .. i)
    end
    
    helpers.playPuppetAnim("idle")
end
