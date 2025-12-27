--[[
    Spamton Boss - Big Shot Attack
]]

function onBegin()
    helpers.playPuppetAnim("big_shot_charge")
    playSound("event:/spamton_big_shot")
    wait(0.4)
    
    helpers.playPuppetAnim("big_shot")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    for i = 1, 5 do
        local shot = helpers.getNewBasicAttackEntity(
            bossPos,
            helpers.getCircle(16),
            "characters/spamton/big_shot",
            true
        )
        local angle = math.atan2(playerPos.Y - bossPos.Y, playerPos.X - bossPos.X) + math.rad(-20 + i * 10)
        shot.Speed = vector2(math.cos(angle) * 280, math.sin(angle) * 280)
        shot:Add(helpers.getEntityTimer(2.5))
        helpers.addEntity(shot)
        playSound("event:/spamton_shot")
        wait(0.1)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
