--[[
    King Digital Dee Dee Dee Boss - Super Inhale Attack
]]

function onBegin()
    helpers.playPuppetAnim("inhale_charge")
    playSound("event:/king_ddd_inhale")
    wait(0.4)
    
    helpers.playPuppetAnim("inhale")
    
    local bossPos = puppet.Position
    local facingRight = puppet.Facing == 1
    local inhaleDir = facingRight and 1 or -1
    
    -- Pull effect toward boss
    local duration = 2.0
    local startTime = helpers.engine.RawElapsedTime
    local pullStrength = 250
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local pPos = player.Position
        local dist = math.abs(pPos.X - bossPos.X)
        
        if dist < 300 and dist > 30 then
            local isInFront = (pPos.X > bossPos.X and facingRight) or (pPos.X < bossPos.X and not facingRight)
            if isInFront then
                local force = (1 - dist / 300) * pullStrength
                player.Speed = vector2(player.Speed.X - inhaleDir * force * 0.016, player.Speed.Y)
            end
        end
        wait(0.016)
    end
    
    helpers.playPuppetAnim("idle")
end
