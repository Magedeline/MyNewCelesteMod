--[[
    Spamton Boss - Pipis Attack
]]

function onBegin()
    helpers.playPuppetAnim("pipis_summon")
    playSound("event:/spamton_pipis")
    wait(0.3)
    
    helpers.playPuppetAnim("pipis_throw")
    
    local bossPos = puppet.Position
    
    -- Throw pipis eggs that explode
    for i = 1, 4 do
        local pipis = helpers.getNewBasicAttackEntity(
            bossPos,
            helpers.getCircle(14),
            "characters/spamton/pipis",
            true
        )
        local angle = math.rad(-30 - i * 20)
        pipis.Speed = vector2(math.cos(angle) * 250, math.sin(angle) * 250)
        pipis:Add(helpers.getEntityTimer(3.0))
        helpers.storeObjectInBoss("pipis_" .. i, pipis)
        helpers.addEntity(pipis)
        wait(0.15)
    end
    
    -- After delay, pipis explode into smaller shots
    wait(1.0)
    
    for i = 1, 4 do
        local pipis = helpers.getStoredObjectFromBoss("pipis_" .. i)
        if pipis and pipis.Scene then
            local pPos = pipis.Position
            playSound("event:/spamton_pipis_burst")
            for j = 1, 6 do
                local fragment = helpers.getNewBasicAttackEntity(
                    pPos,
                    helpers.getCircle(8),
                    "characters/spamton/pipis_fragment",
                    true
                )
                local fragAngle = (j / 6) * math.pi * 2
                fragment.Speed = vector2(math.cos(fragAngle) * 150, math.sin(fragAngle) * 150)
                fragment:Add(helpers.getEntityTimer(1.5))
                helpers.addEntity(fragment)
            end
        end
        helpers.deleteStoredObjectFromBoss("pipis_" .. i)
    end
    
    helpers.playPuppetAnim("idle")
end
