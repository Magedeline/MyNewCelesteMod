--[[
    Tower Titan Boss - Pillar Stomp Attack
    Creates stone pillars from the ground
]]

function onBegin()
    helpers.playPuppetAnim("stomp_charge")
    playSound("event:/tower_titan_stomp_charge")
    wait(0.5)
    
    helpers.playPuppetAnim("stomp")
    playSound("event:/tower_titan_stomp")
    level.Shake(0.5)
    
    local playerPos = player.Position
    
    -- Create warning indicators
    local pillarPositions = {}
    for i = -2, 2 do
        local pos = vector2(playerPos.X + i * 50, playerPos.Y + 8)
        table.insert(pillarPositions, pos)
        
        local warning = helpers.getNewBasicAttackEntity(
            pos,
            helpers.getHitbox(1, 1),
            "characters/tower_titan/pillar_warning",
            false
        )
        warning:Add(helpers.getEntityTimer(0.5))
        helpers.addEntity(warning)
    end
    
    wait(0.5)
    playSound("event:/tower_titan_pillars")
    
    -- Spawn pillars
    for _, pos in ipairs(pillarPositions) do
        local pillar = helpers.getNewBasicAttackEntity(
            pos,
            helpers.getHitbox(32, 80, -16, -80),
            "characters/tower_titan/stone_pillar",
            true
        )
        pillar:Add(helpers.getEntityTimer(2.0))
        helpers.addEntity(pillar)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
