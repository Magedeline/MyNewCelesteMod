--[[
    Possessed Tenna Boss - Fire Blast Attack
]]

function onBegin()
    helpers.playPuppetAnim("fire_charge")
    playSound("event:/possessed_tenna_charge")
    wait(0.5)
    
    helpers.playPuppetAnim("fire_blast")
    playSound("event:/possessed_tenna_fire")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dir = helpers.normalize(vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y))
    
    -- Fire main fireball
    local fireball = helpers.getNewBasicAttackEntity(
        vector2(bossPos.X, bossPos.Y - 20),
        helpers.getCircle(20),
        "characters/possessed_tenna/fireball",
        true
    )
    fireball.Speed = vector2(dir.X * 250, dir.Y * 250)
    fireball:Add(helpers.getEntityTimer(3.0))
    helpers.addEntity(fireball)
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
