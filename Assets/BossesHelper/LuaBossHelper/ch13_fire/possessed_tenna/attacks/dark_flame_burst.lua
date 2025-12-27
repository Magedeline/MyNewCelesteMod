--[[
    Possessed Tenna Boss - Dark Flame Burst Attack
]]

function onBegin()
    helpers.playPuppetAnim("dark_charge")
    playSound("event:/possessed_tenna_dark_charge")
    wait(1.0)
    
    helpers.playPuppetAnim("dark_burst")
    playSound("event:/possessed_tenna_dark_burst")
    level.Shake(0.8)
    
    local bossPos = puppet.Position
    
    -- 8 directional dark flames
    for i = 0, 7 do
        local angle = (i / 8) * math.pi * 2
        local flame = helpers.getNewBasicAttackEntity(
            bossPos,
            helpers.getCircle(15),
            "characters/possessed_tenna/dark_flame",
            true
        )
        flame.Speed = vector2(math.cos(angle) * 200, math.sin(angle) * 200)
        flame:Add(helpers.getEntityTimer(2.0))
        helpers.addEntity(flame)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
