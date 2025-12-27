--[[
    Els True Final Boss - Existence Erasure Attack
]]

function onBegin()
    helpers.playPuppetAnim("erasure_charge")
    playSound("event:/els_final_erasure_charge")
    level.Shake(0.8)
    wait(0.8)
    
    helpers.playPuppetAnim("erasure")
    playSound("event:/els_final_erasure")
    
    local bossPos = puppet.Position
    
    -- Erasure beams that track and sweep
    for beam = 1, 4 do
        local startAngle = (beam / 4) * math.pi * 2
        local beamLength = 500
        
        -- Sweeping beam
        for sweep = 1, 15 do
            local angle = startAngle + sweep * 0.15
            for i = 1, 35 do
                local dist = (i / 35) * beamLength
                local erase = helpers.getNewBasicAttackEntity(
                    vector2(bossPos.X + math.cos(angle) * dist, bossPos.Y + math.sin(angle) * dist),
                    helpers.getCircle(16),
                    "characters/els_final/erasure",
                    true
                )
                erase:Add(helpers.getEntityTimer(0.15))
                helpers.addEntity(erase)
            end
            wait(0.08)
        end
    end
    
    helpers.playPuppetAnim("idle")
end
