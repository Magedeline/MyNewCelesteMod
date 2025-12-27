--[[
    Els Phase 3 Boss - Dimension Tear Attack
]]

function onBegin()
    helpers.playPuppetAnim("tear_open")
    playSound("event:/els_p3_tear")
    level.Shake(1.0)
    wait(0.7)
    
    helpers.playPuppetAnim("dimension_attack")
    
    local bossPos = puppet.Position
    
    -- Create dimension tears that spawn projectiles
    local tearPositions = {
        {-120, -80},
        {120, -80},
        {0, 100},
        {-80, 0},
        {80, 0}
    }
    
    for _, pos in ipairs(tearPositions) do
        local tear = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + pos[1], bossPos.Y + pos[2]),
            helpers.getCircle(30),
            "characters/els/dimension_tear",
            true
        )
        tear:Add(helpers.getEntityTimer(3.0))
        helpers.addEntity(tear)
    end
    
    -- Tears shoot projectiles
    for volley = 1, 4 do
        for _, pos in ipairs(tearPositions) do
            local tearPos = vector2(bossPos.X + pos[1], bossPos.Y + pos[2])
            local playerPos = player.Position
            local dir = helpers.normalize(vector2(playerPos.X - tearPos.X, playerPos.Y - tearPos.Y))
            
            local proj = helpers.getNewBasicAttackEntity(
                tearPos,
                helpers.getCircle(10),
                "characters/els/tear_projectile",
                true
            )
            proj.Speed = vector2(dir.X * 280, dir.Y * 280)
            proj:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(proj)
        end
        playSound("event:/els_p3_tear_shot")
        wait(0.4)
    end
    
    helpers.playPuppetAnim("idle")
end
