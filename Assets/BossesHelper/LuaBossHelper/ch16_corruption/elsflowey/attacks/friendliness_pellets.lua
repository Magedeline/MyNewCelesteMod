--[[
    ElsFlowey Boss - Friendliness Pellets Attack
]]

function onBegin()
    helpers.playPuppetAnim("pellet_summon")
    playSound("event:/elsflowey_pellets")
    wait(0.4)
    
    helpers.playPuppetAnim("pellet_attack")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    -- Ring of "friendliness pellets" that closes in
    local numPellets = 16
    local radius = 200
    
    for i = 1, numPellets do
        local angle = (i / numPellets) * math.pi * 2
        local pellet = helpers.getNewBasicAttackEntity(
            vector2(playerPos.X + math.cos(angle) * radius, playerPos.Y + math.sin(angle) * radius),
            helpers.getCircle(10),
            "characters/elsflowey/pellet",
            true
        )
        pellet:Add(helpers.getEntityTimer(4.0))
        helpers.storeObjectInBoss("pellet_" .. i, pellet)
        helpers.addEntity(pellet)
    end
    
    -- Pellets close in on player
    local duration = 2.5
    local startTime = helpers.engine.RawElapsedTime
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local t = (helpers.engine.RawElapsedTime - startTime) / duration
        local currentRadius = radius * (1 - t * 0.85)
        local pPos = player.Position
        
        for i = 1, numPellets do
            local pellet = helpers.getStoredObjectFromBoss("pellet_" .. i)
            if pellet and pellet.Scene then
                local angle = (i / numPellets) * math.pi * 2 + t * 2
                pellet.Position = vector2(
                    pPos.X + math.cos(angle) * currentRadius,
                    pPos.Y + math.sin(angle) * currentRadius
                )
            end
        end
        wait(0.016)
    end
    
    for i = 1, numPellets do
        helpers.deleteStoredObjectFromBoss("pellet_" .. i)
    end
    
    helpers.playPuppetAnim("idle")
end
