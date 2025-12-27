--[[
    Tower Titan Boss - Boulder Throw Attack
    Throws massive boulders that shatter on impact
]]

function onBegin()
    helpers.playPuppetAnim("grab_boulder")
    playSound("event:/tower_titan_grab")
    wait(0.6)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dirX = playerPos.X > bossPos.X and 1 or -1
    
    helpers.playPuppetAnim("throw_boulder")
    playSound("event:/tower_titan_throw")
    
    -- Create boulder
    local boulder = helpers.getNewBasicAttackActor(
        vector2(bossPos.X + dirX * 30, bossPos.Y - 60),
        helpers.getCircle(24),
        "characters/tower_titan/boulder",
        1.2, 200, true, true
    )
    
    -- Arc trajectory towards player
    local distX = playerPos.X - bossPos.X
    boulder.Speed = vector2(distX * 0.8, -300)
    
    -- On ground collision, spawn debris
    boulder:Add(helpers.getEntityChecker(
        function() return boulder.OnGround end,
        function(entity)
            playSound("event:/tower_titan_boulder_break")
            level.Shake(0.3)
            
            -- Spawn debris
            for i = 1, 5 do
                local debris = helpers.getNewBasicAttackActor(
                    entity.Position,
                    helpers.getCircle(8),
                    "characters/tower_titan/debris",
                    1.0, 150, true, true
                )
                debris.Speed = vector2(
                    (math.random() - 0.5) * 200,
                    -150 - math.random() * 100
                )
                debris:Add(helpers.getEntityTimer(2.0))
                helpers.addEntity(debris)
            end
            
            helpers.destroyEntity(entity)
        end,
        true, true
    ))
    
    helpers.addEntity(boulder)
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
