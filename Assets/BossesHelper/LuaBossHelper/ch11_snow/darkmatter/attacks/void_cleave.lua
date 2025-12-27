--[[
    Dark the Dark Matter Boss - Ultimate Katana: Void Cleave
    Devastating multi-slash attack with void energy
]]

function onBegin()
    helpers.playPuppetAnim("ultimate_charge")
    playSound("event:/darkmatter_ultimate_charge")
    wait(1.0)
    
    level.Shake(0.5)
    helpers.playPuppetAnim("ultimate_ready")
    wait(0.3)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    -- Teleport behind player
    local teleportOffset = playerPos.X > bossPos.X and -60 or 60
    helpers.playPuppetAnim("teleport_out")
    wait(0.1)
    
    puppet.Position = vector2(playerPos.X + teleportOffset, playerPos.Y)
    helpers.playPuppetAnim("teleport_in")
    wait(0.1)
    
    -- Triple slash combo
    for i = 1, 3 do
        local slashDir = player.Position.X > puppet.Position.X and 1 or -1
        
        helpers.playPuppetAnim("void_slash_" .. i)
        playSound("event:/darkmatter_void_slash")
        
        local slashHitbox = helpers.getHitbox(100, 60, slashDir > 0 and 0 or -100, -30)
        local slash = helpers.getNewBasicAttackEntity(
            puppet.Position,
            slashHitbox,
            "characters/darkmatter/void_slash_" .. i,
            true
        )
        slash:Add(helpers.getEntityTimer(0.2))
        helpers.addEntity(slash)
        
        -- Small dash with each slash
        helpers.setSpeed(slashDir * 200, 0)
        wait(0.15)
        helpers.setSpeed(0, 0)
        wait(0.1)
    end
    
    -- Final void explosion
    helpers.playPuppetAnim("void_explosion")
    playSound("event:/darkmatter_void_explosion")
    level.Shake(1.0)
    
    local explosion = helpers.getNewBasicAttackEntity(
        puppet.Position,
        helpers.getCircle(80),
        "characters/darkmatter/void_explosion",
        true
    )
    explosion:Add(helpers.getEntityTimer(0.5))
    helpers.addEntity(explosion)
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
