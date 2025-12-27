--[[
    Asriel Angel of Death Boss - Final Judgment Attack
]]

function onBegin()
    helpers.playPuppetAnim("judgment_charge")
    playSound("event:/asriel_aod_judgment_charge")
    level.Shake(2.0)
    wait(2.0)
    
    helpers.playPuppetAnim("judgment")
    playSound("event:/asriel_aod_judgment")
    level.Shake(3.0)
    
    local bossPos = puppet.Position
    
    -- Screen-filling attack
    -- Phase 1: Radial death rays
    for i = 1, 16 do
        local angle = (i / 16) * math.pi * 2
        local beamLength = 500
        for j = 1, 30 do
            local dist = (j / 30) * beamLength
            local ray = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + math.cos(angle) * dist, bossPos.Y + math.sin(angle) * dist),
                helpers.getCircle(18),
                "characters/asriel_aod/judgment_ray",
                true
            )
            ray:Add(helpers.getEntityTimer(0.6))
            helpers.addEntity(ray)
        end
    end
    
    wait(0.8)
    
    -- Phase 2: Feather apocalypse
    for wave = 1, 3 do
        for i = 1, 20 do
            local angle = (i / 20) * math.pi * 2 + wave * 0.5
            local feather = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getCircle(15),
                "characters/asriel_aod/judgment_feather",
                true
            )
            feather.Speed = vector2(math.cos(angle) * (250 + wave * 50), math.sin(angle) * (250 + wave * 50))
            feather:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(feather)
        end
        wait(0.3)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
