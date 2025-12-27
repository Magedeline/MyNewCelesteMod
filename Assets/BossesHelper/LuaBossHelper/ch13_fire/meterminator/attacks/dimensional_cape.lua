--[[
    Meterminator Knight Boss - Dimensional Cape Attack
    Teleport behind player and strike
]]

function onBegin()
    helpers.playPuppetAnim("cape_wrap")
    playSound("event:/meterminator_cape")
    wait(0.2)
    
    -- Disappear
    helpers.disableCollisions()
    helpers.playPuppetAnim("cape_vanish")
    wait(0.3)
    
    -- Teleport behind player
    local playerPos = player.Position
    local playerFacing = player.Facing
    local behindOffset = playerFacing == 1 and -60 or 60
    
    puppet.Position = vector2(playerPos.X + behindOffset, playerPos.Y)
    
    -- Reappear and strike
    helpers.playPuppetAnim("cape_appear")
    helpers.enableCollisions()
    wait(0.1)
    
    helpers.playPuppetAnim("cape_slash")
    playSound("event:/meterminator_slash")
    
    local dirX = playerPos.X > puppet.Position.X and 1 or -1
    local slash = helpers.getNewBasicAttackEntity(
        puppet.Position,
        helpers.getHitbox(80, 50, dirX > 0 and 0 or -80, -25),
        "characters/meterminator/cape_slash",
        true
    )
    slash:Add(helpers.getEntityTimer(0.2))
    helpers.addEntity(slash)
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
