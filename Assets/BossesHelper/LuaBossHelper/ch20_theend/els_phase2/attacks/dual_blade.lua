--[[
    Els Phase 2 Boss - Dual Shadow Blade Attack
]]

function onBegin()
    helpers.playPuppetAnim("blade_summon")
    playSound("event:/els_p2_blade")
    wait(0.4)
    
    helpers.playPuppetAnim("blade_slash")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local facingDir = playerPos.X > bossPos.X and 1 or -1
    
    -- Two blade slashes
    for slash = 1, 2 do
        local blade = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + facingDir * 30, bossPos.Y + (slash == 1 and -20 or 20)),
            helpers.getHitbox(150, 40, facingDir > 0 and 0 or -150, -20),
            "characters/els/shadow_blade",
            true
        )
        blade.Speed = vector2(facingDir * 400, 0)
        blade:Add(helpers.getEntityTimer(1.0))
        helpers.addEntity(blade)
        playSound("event:/els_p2_slash")
        wait(0.2)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
