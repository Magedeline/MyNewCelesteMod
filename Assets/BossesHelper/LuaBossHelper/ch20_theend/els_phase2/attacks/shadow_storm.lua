--[[
    Els Phase 2 Boss - Shadow Storm Attack
]]

function onBegin()
    helpers.playPuppetAnim("storm_charge")
    playSound("event:/els_p2_storm_charge")
    level.Shake(0.8)
    wait(0.8)
    
    helpers.playPuppetAnim("storm")
    playSound("event:/els_p2_storm")
    
    local bossPos = puppet.Position
    
    -- Storm of shadow projectiles
    for wave = 1, 4 do
        for i = 1, 8 do
            local proj = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + math.random(-150, 150), bossPos.Y - 200),
                helpers.getCircle(15),
                "characters/els/storm_shadow",
                true
            )
            proj.Speed = vector2(math.random(-100, 100), 300)
            proj:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(proj)
        end
        wait(0.4)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
