--[[
    Possessed Tenna Boss - Flame Pillar Attack
]]

function onBegin()
    helpers.playPuppetAnim("pillar_summon")
    playSound("event:/possessed_tenna_pillar")
    wait(0.6)
    
    local playerPos = player.Position
    
    -- Warning markers
    for i = -1, 1 do
        local pos = vector2(playerPos.X + i * 60, playerPos.Y + 8)
        local warning = helpers.getNewBasicAttackEntity(pos, helpers.getHitbox(1,1), "characters/possessed_tenna/pillar_warning", false)
        warning:Add(helpers.getEntityTimer(0.5))
        helpers.addEntity(warning)
    end
    
    wait(0.5)
    playSound("event:/possessed_tenna_pillar_burst")
    
    for i = -1, 1 do
        local pos = vector2(playerPos.X + i * 60, playerPos.Y)
        local pillar = helpers.getNewBasicAttackEntity(pos, helpers.getHitbox(30, 100, -15, -100), "characters/possessed_tenna/flame_pillar", true)
        pillar:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(pillar)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
