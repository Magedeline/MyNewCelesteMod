--[[
    ElsFlowey Boss - Fire Ring Attack
]]

function onBegin()
    helpers.playPuppetAnim("fire_charge")
    playSound("event:/elsflowey_fire")
    wait(0.5)
    
    helpers.playPuppetAnim("fire_ring")
    
    local bossPos = puppet.Position
    
    -- Flamethrower style spinning attack
    local duration = 2.0
    local startTime = helpers.engine.RawElapsedTime
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local t = helpers.engine.RawElapsedTime - startTime
        for i = 1, 4 do
            local angle = (i / 4) * math.pi * 2 + t * 4
            local flame = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + math.cos(angle) * 100, bossPos.Y + math.sin(angle) * 100),
                helpers.getCircle(20),
                "characters/elsflowey/flame",
                true
            )
            flame.Speed = vector2(math.cos(angle) * 200, math.sin(angle) * 200)
            flame:Add(helpers.getEntityTimer(0.8))
            helpers.addEntity(flame)
        end
        wait(0.08)
    end
    
    helpers.playPuppetAnim("idle")
end
