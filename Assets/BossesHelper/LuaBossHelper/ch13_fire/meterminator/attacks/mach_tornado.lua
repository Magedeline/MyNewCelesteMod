--[[
    Meterminator Knight Boss - Mach Tornado Attack
    Spinning tornado attack
]]

function onBegin()
    helpers.playPuppetAnim("tornado_start")
    playSound("event:/meterminator_tornado")
    wait(0.3)
    
    helpers.playPuppetAnim("tornado_spin")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dirX = playerPos.X > bossPos.X and 1 or -1
    
    -- Create spinning hitbox
    local tornado = helpers.getNewBasicAttackEntity(
        bossPos,
        helpers.getCircle(50),
        "characters/meterminator/tornado",
        true
    )
    helpers.storeObjectInBoss("tornado", tornado)
    
    -- Move while spinning
    helpers.setSpeed(dirX * 300, 0)
    
    local duration = 1.5
    local startTime = helpers.engine.RawElapsedTime
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local t = helpers.getStoredObjectFromBoss("tornado")
        if t and t.Scene then
            t.Position = puppet.Position
        end
        wait(0.016)
    end
    
    helpers.setSpeed(0, 0)
    helpers.destroyAll()
    helpers.deleteStoredObjectFromBoss("tornado")
    
    helpers.playPuppetAnim("tornado_end")
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
