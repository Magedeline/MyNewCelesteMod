--[[
    ElsFlowey Boss - Omega Form Attack (Ultimate)
]]

function onBegin()
    helpers.playPuppetAnim("omega_charge")
    playSound("event:/elsflowey_omega_charge")
    level.Shake(2.0)
    wait(2.0)
    
    helpers.playPuppetAnim("omega")
    playSound("event:/elsflowey_omega")
    level.Shake(3.0)
    
    local bossPos = puppet.Position
    
    -- Massive multi-pattern attack
    -- Wave 1: Surrounding pellet storm
    for ring = 1, 3 do
        for i = 1, 20 do
            local angle = (i / 20) * math.pi * 2 + ring * 0.5
            local pellet = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getCircle(12),
                "characters/elsflowey/omega_pellet",
                true
            )
            pellet.Speed = vector2(math.cos(angle) * (200 + ring * 50), math.sin(angle) * (200 + ring * 50))
            pellet:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(pellet)
        end
        wait(0.3)
    end
    
    wait(0.5)
    
    -- Wave 2: Vine barrage
    for i = 1, 12 do
        local xPos = bossPos.X - 300 + i * 50
        local vine = helpers.getNewBasicAttackEntity(
            vector2(xPos, bossPos.Y + 200),
            helpers.getHitbox(35, 300, -17, -300),
            "characters/elsflowey/omega_vine",
            true
        )
        vine.Speed = vector2(0, -500)
        vine:Add(helpers.getEntityTimer(1.2))
        helpers.addEntity(vine)
    end
    
    wait(0.8)
    
    -- Wave 3: Fire rain
    for i = 1, 15 do
        local xPos = bossPos.X - 350 + i * 50
        local fire = helpers.getNewBasicAttackEntity(
            vector2(xPos, bossPos.Y - 300),
            helpers.getCircle(20),
            "characters/elsflowey/omega_fire",
            true
        )
        fire.Speed = vector2(0, 400)
        fire:Add(helpers.getEntityTimer(2.0))
        helpers.addEntity(fire)
        wait(0.05)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
