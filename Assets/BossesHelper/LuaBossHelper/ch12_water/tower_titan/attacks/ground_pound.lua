--[[
    Tower Titan Boss - Ground Pound Attack
    Massive ground slam creating shockwaves
]]

function onBegin()
    -- Jump up
    helpers.playPuppetAnim("jump_up")
    helpers.setSpeed(0, -350)
    wait(0.5)
    
    -- Hover momentarily
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("pound_charge")
    wait(0.3)
    
    -- Slam down
    helpers.playPuppetAnim("pound_fall")
    helpers.setSpeed(0, 500)
    helpers.enableSolidCollisions()
    
    -- Wait until ground
    helpers.waitWhile(function() return not puppet.OnGround end)
    
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("pound_impact")
    playSound("event:/tower_titan_slam")
    level.Shake(1.0)
    
    -- Create shockwaves going left and right
    local bossPos = puppet.Position
    for dir = -1, 1, 2 do
        local shockwave = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + dir * 20, bossPos.Y),
            helpers.getHitbox(40, 20, -20, -20),
            "characters/tower_titan/shockwave",
            true
        )
        shockwave.Speed = vector2(dir * 250, 0)
        shockwave:Add(helpers.getEntityTimer(2.0))
        helpers.addEntity(shockwave)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
