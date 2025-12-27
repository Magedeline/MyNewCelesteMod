--[[
    Els Phase 3 Boss - Reality Collapse Attack
]]

function onBegin()
    helpers.playPuppetAnim("collapse_charge")
    playSound("event:/els_p3_collapse_charge")
    level.Shake(2.0)
    wait(1.5)
    
    helpers.playPuppetAnim("collapse")
    playSound("event:/els_p3_collapse")
    level.Shake(3.0)
    
    local bossPos = puppet.Position
    
    -- Reality shattering screen-wide attack
    -- Phase 1: Void walls closing in
    for i = 1, 8 do
        -- Left wall
        local leftWall = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X - 400 + i * 50, bossPos.Y),
            helpers.getHitbox(30, 600, -15, -300),
            "characters/els/void_wall",
            true
        )
        leftWall.Speed = vector2(200, 0)
        leftWall:Add(helpers.getEntityTimer(2.0))
        helpers.addEntity(leftWall)
        
        -- Right wall
        local rightWall = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + 400 - i * 50, bossPos.Y),
            helpers.getHitbox(30, 600, -15, -300),
            "characters/els/void_wall",
            true
        )
        rightWall.Speed = vector2(-200, 0)
        rightWall:Add(helpers.getEntityTimer(2.0))
        helpers.addEntity(rightWall)
        
        wait(0.1)
    end
    
    wait(0.5)
    
    -- Phase 2: Chaos rain
    for wave = 1, 4 do
        for i = 1, 12 do
            local xPos = bossPos.X - 300 + i * 50
            local chaos = helpers.getNewBasicAttackEntity(
                vector2(xPos, bossPos.Y - 400),
                helpers.getCircle(18),
                "characters/els/chaos_drop",
                true
            )
            chaos.Speed = vector2(0, 400)
            chaos:Add(helpers.getEntityTimer(2.5))
            helpers.addEntity(chaos)
        end
        wait(0.3)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
