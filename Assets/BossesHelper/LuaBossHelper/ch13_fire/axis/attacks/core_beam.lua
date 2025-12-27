--[[
    Axis Boss - Core Beam Attack
]]

function onBegin()
    helpers.playPuppetAnim("core_charge")
    playSound("event:/axis_core_charge")
    level.Shake(0.5)
    wait(1.0)
    
    helpers.playPuppetAnim("core_fire")
    playSound("event:/axis_core_beam")
    level.Shake(0.8)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dir = helpers.normalize(vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y))
    
    -- Create continuous beam
    local beamLength = 400
    for i = 1, 30 do
        local segmentDist = (i / 30) * beamLength
        local segment = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + dir.X * segmentDist, bossPos.Y + dir.Y * segmentDist),
            helpers.getCircle(15),
            "characters/axis/core_beam_segment",
            true
        )
        segment:Add(helpers.getEntityTimer(0.8))
        helpers.addEntity(segment)
    end
    
    wait(0.8)
    helpers.playPuppetAnim("idle")
end
