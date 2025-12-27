--[[
    Marlet Possessed Bird Boss - Dive Attack
    Aerial dive bomb towards the player
]]

function onBegin()
    -- Rise up animation
    helpers.playPuppetAnim("rise")
    helpers.setSpeed(0, -200)
    wait(0.6)
    
    -- Hover at the top
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("hover")
    wait(0.3)
    
    -- Target player position
    local playerPos = player.Position
    local bossPos = puppet.Position
    
    -- Calculate dive direction
    local dirX = playerPos.X - bossPos.X
    local dirY = playerPos.Y - bossPos.Y
    local length = math.sqrt(dirX * dirX + dirY * dirY)
    dirX = dirX / length
    dirY = dirY / length
    
    -- Dive attack
    helpers.playPuppetAnim("dive")
    playSound("event:/marlet_dive")
    helpers.setSpeed(dirX * 350, dirY * 350)
    helpers.keepSpeedDuring(0.8)
    
    wait(0.8)
    
    -- Recovery
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("recover")
    wait(0.4)
    helpers.playPuppetAnim("idle")
end
