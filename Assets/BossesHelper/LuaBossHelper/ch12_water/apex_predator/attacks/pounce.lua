--[[
    Apex Predator Boss - Pounce Attack
    Quick leap attack towards player
]]

function onBegin()
    helpers.playPuppetAnim("crouch")
    playSound("event:/apex_predator_growl")
    wait(0.4)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    -- Calculate pounce trajectory
    local dirX = playerPos.X - bossPos.X
    local dirY = playerPos.Y - bossPos.Y - 50 -- Aim above player
    local length = math.sqrt(dirX * dirX + dirY * dirY)
    
    helpers.playPuppetAnim("pounce")
    playSound("event:/apex_predator_pounce")
    
    helpers.setSpeed(dirX * 1.5, math.min(dirY * 1.5, -200))
    helpers.setEffectiveGravityMult(1.0)
    
    wait(0.6)
    
    -- Landing
    if puppet.OnGround then
        helpers.playPuppetAnim("land")
        playSound("event:/apex_predator_land")
    end
    
    helpers.setSpeed(0, 0)
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
