--[[
    Axis Boss - Charge Dash Attack
]]

function onBegin()
    helpers.playPuppetAnim("charge_prepare")
    playSound("event:/axis_charge_prepare")
    wait(0.4)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dir = helpers.normalize(vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y))
    
    helpers.playPuppetAnim("charge")
    playSound("event:/axis_charge")
    
    helpers.setSpeed(dir.X * 450, dir.Y * 450)
    helpers.keepSpeedDuring(0.6)
    
    wait(0.6)
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("charge_stop")
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
