--[[
    Els True Final Boss - Dimension Shatter Attack
]]

function onBegin()
    helpers.playPuppetAnim("shatter_charge")
    playSound("event:/els_final_shatter_charge")
    level.Shake(1.5)
    wait(1.0)
    
    helpers.playPuppetAnim("shatter")
    playSound("event:/els_final_shatter")
    level.Shake(2.0)
    
    local bossPos = puppet.Position
    
    -- Dimension cracks spread across screen
    for crack = 1, 8 do
        local angle = (crack / 8) * math.pi * 2
        local crackLength = 400
        
        for i = 1, 25 do
            local dist = (i / 25) * crackLength
            local shard = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + math.cos(angle) * dist, bossPos.Y + math.sin(angle) * dist),
                helpers.getHitbox(30, 30, -15, -15),
                "characters/els_final/dimension_shard",
                true
            )
            shard:Add(helpers.getEntityTimer(1.5))
            helpers.addEntity(shard)
        end
        wait(0.1)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
