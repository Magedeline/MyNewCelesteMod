--[[
    Spamton Boss - Neo Transformation Attack (Ultimate)
]]

function onBegin()
    helpers.playPuppetAnim("neo_charge")
    playSound("event:/spamton_neo_charge")
    level.Shake(1.0)
    wait(1.5)
    
    helpers.playPuppetAnim("neo_attack")
    playSound("event:/spamton_neo")
    level.Shake(1.5)
    
    local bossPos = puppet.Position
    
    -- Chaotic multi-directional beam storm
    for burst = 1, 4 do
        for i = 1, 12 do
            local angle = (i / 12) * math.pi * 2 + burst * 0.3
            local beam = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getHitbox(150, 15, 0, -7),
                "characters/spamton/neo_beam",
                true
            )
            beam.Speed = vector2(math.cos(angle) * 350, math.sin(angle) * 350)
            beam:Add(helpers.getEntityTimer(1.5))
            helpers.addEntity(beam)
        end
        playSound("event:/spamton_burst")
        wait(0.4)
    end
    
    -- Final big shot barrage
    local playerPos = player.Position
    for i = 1, 8 do
        local bigShot = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + math.random(-100, 100), bossPos.Y),
            helpers.getCircle(25),
            "characters/spamton/neo_big_shot",
            true
        )
        local dir = helpers.normalize(vector2(playerPos.X - bigShot.Position.X, playerPos.Y - bigShot.Position.Y))
        bigShot.Speed = vector2(dir.X * 300, dir.Y * 300)
        bigShot:Add(helpers.getEntityTimer(2.0))
        helpers.addEntity(bigShot)
        wait(0.1)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
