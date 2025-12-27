--[[
    Els True Final Boss - Absolute Void Attack
]]

function onBegin()
    helpers.playPuppetAnim("absolute_charge")
    playSound("event:/els_final_void_charge")
    level.Shake(1.0)
    wait(1.0)
    
    helpers.playPuppetAnim("absolute_void")
    playSound("event:/els_final_void")
    level.Shake(1.5)
    
    local bossPos = puppet.Position
    
    -- Massive void projectile storm
    for wave = 1, 5 do
        for i = 1, 16 do
            local angle = (i / 16) * math.pi * 2 + wave * 0.4
            local void = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getCircle(18),
                "characters/els_final/absolute_void",
                true
            )
            void.Speed = vector2(math.cos(angle) * (200 + wave * 40), math.sin(angle) * (200 + wave * 40))
            void:Add(helpers.getEntityTimer(2.5))
            helpers.addEntity(void)
        end
        wait(0.3)
    end
    
    wait(0.4)
    helpers.playPuppetAnim("idle")
end
