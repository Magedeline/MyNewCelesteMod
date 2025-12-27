--[[
    Els Phase 1 Boss - Teleport Strike Attack
]]

function onBegin()
    helpers.playPuppetAnim("vanish")
    playSound("event:/els_p1_vanish")
    wait(0.3)
    
    local playerPos = player.Position
    
    -- Teleport behind player
    puppet.Position = vector2(playerPos.X + (math.random() > 0.5 and 80 or -80), playerPos.Y)
    
    helpers.playPuppetAnim("appear")
    playSound("event:/els_p1_appear")
    wait(0.2)
    
    helpers.playPuppetAnim("strike")
    playSound("event:/els_p1_strike")
    
    local strike = helpers.getNewBasicAttackEntity(
        puppet.Position,
        helpers.getHitbox(100, 60, -50, -30),
        "characters/els/shadow_strike",
        true
    )
    strike:Add(helpers.getEntityTimer(0.3))
    helpers.addEntity(strike)
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
