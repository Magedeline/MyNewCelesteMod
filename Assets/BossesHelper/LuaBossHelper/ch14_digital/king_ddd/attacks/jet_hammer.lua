--[[
    King Digital Dee Dee Dee Boss - Jet Hammer Attack
]]

function onBegin()
    helpers.playPuppetAnim("jet_charge")
    playSound("event:/king_ddd_jet_charge")
    wait(0.8)
    
    helpers.playPuppetAnim("jet_spin")
    playSound("event:/king_ddd_jet")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dir = playerPos.X > bossPos.X and 1 or -1
    
    -- Jet propelled hammer spin
    local duration = 1.5
    local startTime = helpers.engine.RawElapsedTime
    
    helpers.setSpeed(dir * 350, 0)
    
    while helpers.engine.RawElapsedTime - startTime < duration do
        local currentPos = puppet.Position
        -- Create spinning hitbox trail
        local spin = helpers.getNewBasicAttackEntity(
            currentPos,
            helpers.getCircle(40),
            "characters/king_ddd/jet_spin",
            true
        )
        spin:Add(helpers.getEntityTimer(0.2))
        helpers.addEntity(spin)
        wait(0.1)
    end
    
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("jet_stop")
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
