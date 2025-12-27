--[[
    Els Phase 3 Boss - Soul Shatter Attack
]]

function onBegin()
    helpers.playPuppetAnim("shatter_charge")
    playSound("event:/els_p3_shatter_charge")
    level.Shake(0.5)
    wait(0.6)
    
    helpers.playPuppetAnim("shatter")
    playSound("event:/els_p3_shatter")
    
    local playerPos = player.Position
    
    -- Soul shattering effect at player position
    for ring = 1, 3 do
        local radius = ring * 60
        local numFragments = 8 + ring * 4
        
        for i = 1, numFragments do
            local angle = (i / numFragments) * math.pi * 2
            local fragment = helpers.getNewBasicAttackEntity(
                vector2(playerPos.X + math.cos(angle) * radius, playerPos.Y + math.sin(angle) * radius),
                helpers.getCircle(12),
                "characters/els/soul_fragment",
                true
            )
            fragment.Speed = vector2(math.cos(angle) * 150, math.sin(angle) * 150)
            fragment:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(fragment)
        end
        wait(0.2)
    end
    
    helpers.playPuppetAnim("idle")
end
