--[[
    Els True Final Boss - The End Attack (Ultimate)
]]

function onBegin()
    helpers.playPuppetAnim("end_charge")
    playSound("event:/els_final_end_charge")
    level.Shake(3.0)
    wait(2.5)
    
    helpers.playPuppetAnim("the_end")
    playSound("event:/els_final_the_end")
    level.Shake(5.0)
    
    local bossPos = puppet.Position
    
    -- Phase 1: Void walls from all sides
    for side = 1, 4 do
        local angle = (side / 4) * math.pi * 2
        local perpAngle = angle + math.pi / 2
        
        for i = 1, 12 do
            local wall = helpers.getNewBasicAttackEntity(
                vector2(
                    bossPos.X + math.cos(angle) * 500 + math.cos(perpAngle) * (i - 6) * 30,
                    bossPos.Y + math.sin(angle) * 500 + math.sin(perpAngle) * (i - 6) * 30
                ),
                helpers.getCircle(25),
                "characters/els_final/end_wall",
                true
            )
            wall.Speed = vector2(-math.cos(angle) * 300, -math.sin(angle) * 300)
            wall:Add(helpers.getEntityTimer(3.0))
            helpers.addEntity(wall)
        end
    end
    
    wait(0.8)
    
    -- Phase 2: Spiral of destruction
    local duration = 2.0
    local startTime = helpers.engine.RawElapsedTime
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local t = helpers.engine.RawElapsedTime - startTime
        for i = 1, 3 do
            local angle = t * 5 + i * math.pi * 2 / 3
            local proj = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getCircle(18),
                "characters/els_final/end_spiral",
                true
            )
            proj.Speed = vector2(math.cos(angle) * 280, math.sin(angle) * 280)
            proj:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(proj)
        end
        wait(0.08)
    end
    
    wait(0.5)
    
    -- Phase 3: Final extinction burst
    for ring = 1, 4 do
        for i = 1, 20 do
            local angle = (i / 20) * math.pi * 2 + ring * 0.3
            local burst = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getCircle(22),
                "characters/els_final/extinction_burst",
                true
            )
            burst.Speed = vector2(math.cos(angle) * (250 + ring * 60), math.sin(angle) * (250 + ring * 60))
            burst:Add(helpers.getEntityTimer(2.5))
            helpers.addEntity(burst)
        end
        wait(0.25)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
