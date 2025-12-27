--[[
    Asriel Angel of Death Boss - Soul Scythe Attack
]]

function onBegin()
    helpers.playPuppetAnim("scythe_summon")
    playSound("event:/asriel_aod_scythe")
    wait(0.4)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    helpers.playPuppetAnim("scythe_swing")
    
    for swing = 1, 3 do
        local scythe = helpers.getNewBasicAttackEntity(
            bossPos,
            helpers.getHitbox(200, 60, -100, -30),
            "characters/asriel_aod/soul_scythe",
            true
        )
        local dir = helpers.normalize(vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y))
        scythe.Speed = vector2(dir.X * 380, dir.Y * 380)
        scythe:Add(helpers.getEntityTimer(1.2))
        helpers.addEntity(scythe)
        playSound("event:/asriel_aod_swing")
        wait(0.35)
        playerPos = player.Position
    end
    
    helpers.playPuppetAnim("idle")
end
