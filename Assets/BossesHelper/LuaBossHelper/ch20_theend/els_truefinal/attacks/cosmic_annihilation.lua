--[[
    Els True Final Boss - Cosmic Annihilation Attack
]]

function onBegin()
    helpers.playPuppetAnim("cosmic_charge")
    playSound("event:/els_final_cosmic_charge")
    level.Shake(1.2)
    wait(1.2)
    
    helpers.playPuppetAnim("cosmic")
    playSound("event:/els_final_cosmic")
    
    local bossPos = puppet.Position
    
    -- Cosmic explosions across arena
    local explosionPoints = {
        {0, 0},
        {-150, -100},
        {150, -100},
        {-100, 100},
        {100, 100},
        {0, -150},
        {0, 150}
    }
    
    for _, point in ipairs(explosionPoints) do
        local explodePos = vector2(bossPos.X + point[1], bossPos.Y + point[2])
        
        -- Cosmic explosion
        for i = 1, 10 do
            local angle = (i / 10) * math.pi * 2
            local cosmic = helpers.getNewBasicAttackEntity(
                explodePos,
                helpers.getCircle(20),
                "characters/els_final/cosmic",
                true
            )
            cosmic.Speed = vector2(math.cos(angle) * 220, math.sin(angle) * 220)
            cosmic:Add(helpers.getEntityTimer(1.8))
            helpers.addEntity(cosmic)
        end
        playSound("event:/els_final_explode")
        wait(0.15)
    end
    
    wait(0.4)
    helpers.playPuppetAnim("idle")
end
