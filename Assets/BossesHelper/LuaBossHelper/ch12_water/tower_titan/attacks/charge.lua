--[[
    Tower Titan Boss - Charge Attack
    Charges across the arena
]]

function onBegin()
    helpers.playPuppetAnim("charge_prepare")
    playSound("event:/tower_titan_charge_prepare")
    wait(0.6)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local chargeDir = playerPos.X > bossPos.X and 1 or -1
    
    helpers.playPuppetAnim("charge")
    playSound("event:/tower_titan_charge")
    
    helpers.setSpeed(chargeDir * 450, 0)
    helpers.keepSpeedDuring(0.8)
    
    wait(0.8)
    
    -- Impact if hit wall
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("charge_stop")
    playSound("event:/tower_titan_charge_stop")
    level.Shake(0.3)
    
    wait(0.4)
    helpers.playPuppetAnim("idle")
end
