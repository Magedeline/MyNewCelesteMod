--[[
    Els Phase 1 Boss - Shadow Bolt Attack
]]

function onBegin()
    helpers.playPuppetAnim("bolt_charge")
    playSound("event:/els_p1_bolt")
    wait(0.4)
    
    helpers.playPuppetAnim("bolt_fire")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    for i = 1, 3 do
        local bolt = helpers.getNewBasicAttackEntity(
            bossPos,
            helpers.getCircle(14),
            "characters/els/shadow_bolt",
            true
        )
        local angle = math.atan2(playerPos.Y - bossPos.Y, playerPos.X - bossPos.X) + math.rad(-15 + i * 15)
        bolt.Speed = vector2(math.cos(angle) * 300, math.sin(angle) * 300)
        bolt:Add(helpers.getEntityTimer(2.0))
        helpers.addEntity(bolt)
        wait(0.15)
    end
    
    wait(0.2)
    helpers.playPuppetAnim("idle")
end
