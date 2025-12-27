--[[
    Marlet Possessed Bird Boss - Talon Swipe Attack
    Quick dash with talon attack
]]

function onBegin()
    -- Prepare animation
    helpers.playPuppetAnim("talon_prepare")
    wait(0.3)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dirX = playerPos.X > bossPos.X and 1 or -1
    
    -- Create talon hitbox that moves with boss
    local hitbox = helpers.getHitbox(40, 30, dirX > 0 and 0 or -40, -15)
    
    -- Quick dash
    helpers.playPuppetAnim("talon_swipe")
    playSound("event:/marlet_talon")
    helpers.setSpeed(dirX * 400, 0)
    helpers.keepSpeedDuring(0.4)
    
    wait(0.4)
    
    -- Stop and recover
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("recover")
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
