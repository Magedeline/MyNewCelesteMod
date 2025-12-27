--[[
    ElsFlowey Boss - Soul Steal Attack
]]

function onBegin()
    helpers.playPuppetAnim("soul_charge")
    playSound("event:/elsflowey_soul_charge")
    wait(0.8)
    
    helpers.playPuppetAnim("soul_steal")
    playSound("event:/elsflowey_soul")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    -- Create soul extraction beam
    local beamLength = 250
    local dir = helpers.normalize(vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y))
    
    for i = 1, 20 do
        local dist = (i / 20) * beamLength
        local soul = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + dir.X * dist, bossPos.Y + dir.Y * dist),
            helpers.getCircle(18),
            "characters/elsflowey/soul_beam",
            true
        )
        soul:Add(helpers.getEntityTimer(0.6))
        helpers.addEntity(soul)
    end
    
    wait(0.5)
    
    -- Soul fragments fly toward boss
    for i = 1, 8 do
        local fragment = helpers.getNewBasicAttackEntity(
            vector2(playerPos.X + math.random(-50, 50), playerPos.Y + math.random(-50, 50)),
            helpers.getCircle(12),
            "characters/elsflowey/soul_fragment",
            true
        )
        local towardBoss = helpers.normalize(vector2(bossPos.X - fragment.Position.X, bossPos.Y - fragment.Position.Y))
        fragment.Speed = vector2(towardBoss.X * 250, towardBoss.Y * 250)
        fragment:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(fragment)
        wait(0.08)
    end
    
    helpers.playPuppetAnim("idle")
end
