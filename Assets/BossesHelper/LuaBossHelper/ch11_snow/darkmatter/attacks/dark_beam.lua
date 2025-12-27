--[[
    Dark the Dark Matter Boss - Dark Beam Attack
    Fires a concentrated beam of dark energy
]]

function onBegin()
    helpers.playPuppetAnim("beam_charge")
    playSound("event:/darkmatter_beam_charge")
    wait(0.8)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    -- Calculate direction
    local dirX = playerPos.X - bossPos.X
    local dirY = playerPos.Y - bossPos.Y
    local length = math.sqrt(dirX * dirX + dirY * dirY)
    dirX = dirX / length
    dirY = dirY / length
    
    helpers.playPuppetAnim("beam_fire")
    playSound("event:/darkmatter_beam_fire")
    
    -- Create beam segments along the path
    local beamLength = 400
    local segmentCount = 20
    for i = 1, segmentCount do
        local segmentDist = (i / segmentCount) * beamLength
        local segmentPos = vector2(
            bossPos.X + dirX * segmentDist,
            bossPos.Y + dirY * segmentDist
        )
        
        local hitbox = helpers.getCircle(12)
        local segment = helpers.getNewBasicAttackEntity(
            segmentPos,
            hitbox,
            "characters/darkmatter/beam_segment",
            true
        )
        segment:Add(helpers.getEntityTimer(0.5))
        helpers.addEntity(segment)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
