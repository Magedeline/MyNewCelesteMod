--[[
    Asriel Angel of Death Boss - Angelic Descent Attack
]]

function onBegin()
    helpers.playPuppetAnim("ascend")
    playSound("event:/asriel_aod_ascend")
    helpers.setSpeed(0, -400)
    wait(0.5)
    helpers.setSpeed(0, 0)
    
    -- Target player position
    local targetX = player.Position.X
    puppet.Position = vector2(targetX, puppet.Position.Y)
    
    helpers.playPuppetAnim("descend")
    playSound("event:/asriel_aod_descend")
    helpers.setSpeed(0, 600)
    
    -- Leave death feathers trail
    local duration = 0.6
    local startTime = helpers.engine.RawElapsedTime
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local feather = helpers.getNewBasicAttackEntity(
            puppet.Position,
            helpers.getCircle(15),
            "characters/asriel_aod/death_feather",
            true
        )
        feather.Speed = vector2(math.random(-100, 100), 50)
        feather:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(feather)
        wait(0.05)
    end
    
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("land")
    level.Shake(1.0)
    playSound("event:/asriel_aod_land")
    
    -- Impact shockwave
    local bossPos = puppet.Position
    for side = -1, 1, 2 do
        for i = 1, 8 do
            local wave = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + side * i * 40, bossPos.Y),
                helpers.getHitbox(35, 60, -17, -30),
                "characters/asriel_aod/impact_wave",
                true
            )
            wave.Speed = vector2(side * 300, 0)
            wave:Add(helpers.getEntityTimer(1.0))
            helpers.addEntity(wave)
        end
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
