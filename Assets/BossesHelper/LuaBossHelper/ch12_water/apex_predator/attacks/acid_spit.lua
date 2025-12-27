--[[
    Apex Predator Boss - Acid Spit Attack
    Spits corrosive acid projectiles
]]

function onBegin()
    helpers.playPuppetAnim("acid_charge")
    wait(0.4)
    
    helpers.playPuppetAnim("acid_spit")
    playSound("event:/apex_predator_spit")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    -- Spit 3 acid globs in a spread
    for i = -1, 1 do
        local baseDir = vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y)
        local length = math.sqrt(baseDir.X^2 + baseDir.Y^2)
        baseDir = vector2(baseDir.X/length, baseDir.Y/length)
        
        -- Add spread
        local spreadAngle = i * math.rad(15)
        local cosA, sinA = math.cos(spreadAngle), math.sin(spreadAngle)
        local dir = vector2(
            baseDir.X * cosA - baseDir.Y * sinA,
            baseDir.X * sinA + baseDir.Y * cosA
        )
        
        local acid = helpers.getNewBasicAttackActor(
            vector2(bossPos.X, bossPos.Y - 20),
            helpers.getCircle(12),
            "characters/apex_predator/acid_glob",
            0.5, 150, true, true
        )
        
        acid.Speed = vector2(dir.X * 200, dir.Y * 200)
        
        -- Leave acid pool on ground
        acid:Add(helpers.getEntityChecker(
            function() return acid.OnGround end,
            function(entity)
                local pool = helpers.getNewBasicAttackEntity(
                    entity.Position,
                    helpers.getHitbox(30, 10, -15, -5),
                    "characters/apex_predator/acid_pool",
                    true
                )
                pool:Add(helpers.getEntityTimer(3.0))
                helpers.addEntity(pool)
                helpers.destroyEntity(entity)
            end,
            true, true
        ))
        
        acid:Add(helpers.getEntityTimer(4.0))
        helpers.addEntity(acid)
        wait(0.1)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
