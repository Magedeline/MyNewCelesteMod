--[[
    Els Phase 3 Boss - Void Cataclysm Attack
]]

function onBegin()
    helpers.playPuppetAnim("void_charge")
    playSound("event:/els_p3_void")
    level.Shake(0.8)
    wait(0.8)
    
    helpers.playPuppetAnim("void_release")
    
    local bossPos = puppet.Position
    
    -- Multiple void explosions
    for burst = 1, 4 do
        local burstPos = vector2(
            bossPos.X + math.random(-150, 150),
            bossPos.Y + math.random(-100, 100)
        )
        
        for i = 1, 12 do
            local angle = (i / 12) * math.pi * 2
            local void = helpers.getNewBasicAttackEntity(
                burstPos,
                helpers.getCircle(14),
                "characters/els/void_cataclysm",
                true
            )
            void.Speed = vector2(math.cos(angle) * 250, math.sin(angle) * 250)
            void:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(void)
        end
        playSound("event:/els_p3_burst")
        wait(0.25)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
