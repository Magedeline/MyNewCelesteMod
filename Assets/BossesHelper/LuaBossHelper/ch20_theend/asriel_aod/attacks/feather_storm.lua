--[[
    Asriel Angel of Death Boss - Feather Storm Attack
]]

function onBegin()
    helpers.playPuppetAnim("wings_spread")
    playSound("event:/asriel_aod_wings")
    wait(0.5)
    
    helpers.playPuppetAnim("feather_storm")
    
    local bossPos = puppet.Position
    
    -- Rain of death feathers
    for wave = 1, 5 do
        for i = 1, 10 do
            local xPos = bossPos.X - 250 + i * 50
            local feather = helpers.getNewBasicAttackEntity(
                vector2(xPos + math.random(-20, 20), bossPos.Y - 300),
                helpers.getCircle(12),
                "characters/asriel_aod/storm_feather",
                true
            )
            feather.Speed = vector2(math.random(-50, 50), 350)
            feather:Add(helpers.getEntityTimer(2.5))
            helpers.addEntity(feather)
        end
        playSound("event:/asriel_aod_feather_wave")
        wait(0.35)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
