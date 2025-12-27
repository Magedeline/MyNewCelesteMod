--[[
    ElsFlowey Boss - Vine Lash Attack
]]

function onBegin()
    helpers.playPuppetAnim("vine_prepare")
    playSound("event:/elsflowey_vine")
    wait(0.3)
    
    helpers.playPuppetAnim("vine_lash")
    
    local bossPos = puppet.Position
    
    -- Vines emerge from ground
    for i = 1, 8 do
        local xPos = bossPos.X - 200 + i * 50
        
        -- Warning indicator
        wait(0.15)
        
        -- Vine emerges
        local vine = helpers.getNewBasicAttackEntity(
            vector2(xPos, bossPos.Y + 150),
            helpers.getHitbox(30, 200, -15, -200),
            "characters/elsflowey/vine",
            true
        )
        vine.Speed = vector2(0, -400)
        vine:Add(helpers.getEntityTimer(1.0))
        helpers.addEntity(vine)
        playSound("event:/elsflowey_vine_emerge")
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
