--[[
    Els Phase 3 Boss - Chaos Chains Attack
]]

function onBegin()
    helpers.playPuppetAnim("chain_summon")
    playSound("event:/els_p3_chains")
    wait(0.5)
    
    helpers.playPuppetAnim("chain_attack")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    -- Chains that snake toward player
    for chain = 1, 4 do
        local startAngle = (chain / 4) * math.pi * 2
        local chainX = bossPos.X + math.cos(startAngle) * 80
        local chainY = bossPos.Y + math.sin(startAngle) * 80
        
        for segment = 1, 8 do
            local seg = helpers.getNewBasicAttackEntity(
                vector2(chainX, chainY),
                helpers.getCircle(12),
                "characters/els/chaos_chain",
                true
            )
            local dir = helpers.normalize(vector2(playerPos.X - chainX, playerPos.Y - chainY))
            seg.Speed = vector2(dir.X * (180 + segment * 20), dir.Y * (180 + segment * 20))
            seg:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(seg)
            chainX = chainX + dir.X * 25
            chainY = chainY + dir.Y * 25
            wait(0.05)
        end
        wait(0.2)
    end
    
    helpers.playPuppetAnim("idle")
end
