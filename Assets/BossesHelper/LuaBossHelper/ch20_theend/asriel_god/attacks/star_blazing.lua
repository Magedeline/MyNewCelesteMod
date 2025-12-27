--[[
    Asriel God Boss - Rainbow Star Blazing Attack
]]

function onBegin()
    helpers.playPuppetAnim("star_charge")
    playSound("event:/asriel_god_star")
    wait(0.5)
    
    helpers.playPuppetAnim("star_fire")
    
    local bossPos = puppet.Position
    
    -- Rainbow star projectiles
    for wave = 1, 3 do
        for i = 1, 10 do
            local angle = (i / 10) * math.pi * 2 + wave * 0.3
            local star = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getCircle(15),
                "characters/asriel_god/rainbow_star",
                true
            )
            star.Speed = vector2(math.cos(angle) * (200 + wave * 30), math.sin(angle) * (200 + wave * 30))
            star:Add(helpers.getEntityTimer(2.5))
            helpers.addEntity(star)
        end
        wait(0.25)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
