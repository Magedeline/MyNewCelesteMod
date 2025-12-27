--[[
    Asriel Angel of Death Boss - Death Ray Attack
]]

function onBegin()
    helpers.playPuppetAnim("death_ray_charge")
    playSound("event:/asriel_aod_ray_charge")
    level.Shake(0.5)
    wait(0.6)
    
    helpers.playPuppetAnim("death_ray")
    playSound("event:/asriel_aod_ray")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dir = helpers.normalize(vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y))
    
    -- Continuous death ray
    local beamLength = 600
    for i = 1, 50 do
        local dist = (i / 50) * beamLength
        local ray = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + dir.X * dist, bossPos.Y + dir.Y * dist),
            helpers.getCircle(20),
            "characters/asriel_aod/death_ray",
            true
        )
        ray:Add(helpers.getEntityTimer(0.5))
        helpers.addEntity(ray)
    end
    
    wait(0.4)
    helpers.playPuppetAnim("idle")
end
