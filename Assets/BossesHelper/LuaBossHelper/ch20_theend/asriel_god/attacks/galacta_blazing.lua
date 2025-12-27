--[[
    Asriel God Boss - Galacta Blazing Attack
]]

function onBegin()
    helpers.playPuppetAnim("galacta_charge")
    playSound("event:/asriel_god_galacta")
    level.Shake(0.8)
    wait(0.8)
    
    helpers.playPuppetAnim("galacta_fire")
    
    local bossPos = puppet.Position
    
    -- Galaxy beam
    local beamLength = 500
    local playerPos = player.Position
    local dir = helpers.normalize(vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y))
    
    for i = 1, 40 do
        local dist = (i / 40) * beamLength
        local beam = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + dir.X * dist, bossPos.Y + dir.Y * dist),
            helpers.getCircle(25),
            "characters/asriel_god/galacta_beam",
            true
        )
        beam:Add(helpers.getEntityTimer(0.8))
        helpers.addEntity(beam)
    end
    
    playSound("event:/asriel_god_beam")
    wait(0.7)
    helpers.playPuppetAnim("idle")
end
