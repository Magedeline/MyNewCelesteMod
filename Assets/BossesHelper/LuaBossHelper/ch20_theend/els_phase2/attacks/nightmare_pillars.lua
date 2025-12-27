--[[
    Els Phase 2 Boss - Nightmare Pillars Attack
]]

function onBegin()
    helpers.playPuppetAnim("pillar_summon")
    playSound("event:/els_p2_pillars")
    level.Shake(0.5)
    wait(0.6)
    
    helpers.playPuppetAnim("pillar_rise")
    
    local bossPos = puppet.Position
    
    -- Pillars rise from ground
    for i = 1, 6 do
        local xPos = bossPos.X - 150 + i * 50
        local pillar = helpers.getNewBasicAttackEntity(
            vector2(xPos, bossPos.Y + 150),
            helpers.getHitbox(40, 300, -20, -300),
            "characters/els/nightmare_pillar",
            true
        )
        pillar.Speed = vector2(0, -350)
        pillar:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(pillar)
        playSound("event:/els_p2_pillar_rise")
        wait(0.15)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
