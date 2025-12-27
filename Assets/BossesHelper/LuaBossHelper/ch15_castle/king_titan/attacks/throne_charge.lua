--[[
    King Titan Boss - Throne Charge Attack
]]

function onBegin()
    helpers.playPuppetAnim("throne_rise")
    playSound("event:/king_titan_throne")
    wait(0.6)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dir = helpers.normalize(vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y))
    
    helpers.playPuppetAnim("throne_charge")
    playSound("event:/king_titan_charge")
    
    helpers.setSpeed(dir.X * 500, dir.Y * 500)
    
    -- Leave trail
    local duration = 0.8
    local startTime = helpers.engine.RawElapsedTime
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local trail = helpers.getNewBasicAttackEntity(
            puppet.Position,
            helpers.getCircle(30),
            "characters/king_titan/charge_trail",
            true
        )
        trail:Add(helpers.getEntityTimer(0.3))
        helpers.addEntity(trail)
        wait(0.05)
    end
    
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("throne_stop")
    level.Shake(0.5)
    wait(0.4)
    helpers.playPuppetAnim("idle")
end
