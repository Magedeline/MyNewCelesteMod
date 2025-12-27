--[[
    Asriel God Boss - Chaos Saber Attack
]]

function onBegin()
    helpers.playPuppetAnim("saber_summon")
    playSound("event:/asriel_god_saber")
    wait(0.4)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    for slash = 1, 4 do
        helpers.playPuppetAnim("saber_slash")
        playSound("event:/asriel_god_slash")
        
        local dir = helpers.normalize(vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y))
        
        -- Wide saber slash
        local saber = helpers.getNewBasicAttackEntity(
            bossPos,
            helpers.getHitbox(180, 50, -90, -25),
            "characters/asriel_god/chaos_saber",
            true
        )
        saber.Speed = vector2(dir.X * 350, dir.Y * 350)
        saber:Add(helpers.getEntityTimer(1.0))
        helpers.addEntity(saber)
        
        wait(0.3)
        playerPos = player.Position
    end
    
    helpers.playPuppetAnim("idle")
end
