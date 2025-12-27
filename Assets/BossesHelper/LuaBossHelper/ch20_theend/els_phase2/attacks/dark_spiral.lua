--[[
    Els Phase 2 Boss - Dark Spiral Attack
]]

function onBegin()
    helpers.playPuppetAnim("spiral_charge")
    playSound("event:/els_p2_spiral")
    wait(0.5)
    
    helpers.playPuppetAnim("spiral")
    
    local bossPos = puppet.Position
    
    -- Spiral projectile pattern
    local duration = 2.0
    local startTime = helpers.engine.RawElapsedTime
    local projectileCount = 0
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local t = helpers.engine.RawElapsedTime - startTime
        projectileCount = projectileCount + 1
        
        local angle = t * 6 + projectileCount * 0.3
        local proj = helpers.getNewBasicAttackEntity(
            bossPos,
            helpers.getCircle(10),
            "characters/els/dark_spiral",
            true
        )
        proj.Speed = vector2(math.cos(angle) * 220, math.sin(angle) * 220)
        proj:Add(helpers.getEntityTimer(2.5))
        helpers.addEntity(proj)
        wait(0.08)
    end
    
    helpers.playPuppetAnim("idle")
end
