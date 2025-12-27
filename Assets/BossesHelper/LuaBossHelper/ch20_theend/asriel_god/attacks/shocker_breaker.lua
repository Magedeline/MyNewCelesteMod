--[[
    Asriel God Boss - Shocker Breaker Attack
]]

function onBegin()
    helpers.playPuppetAnim("lightning_charge")
    playSound("event:/asriel_god_lightning_charge")
    level.Shake(0.5)
    wait(0.6)
    
    helpers.playPuppetAnim("lightning_strike")
    playSound("event:/asriel_god_lightning")
    level.Shake(0.8)
    
    local bossPos = puppet.Position
    
    -- Lightning bolts from above
    for i = 1, 8 do
        local xPos = bossPos.X - 200 + i * 50
        local lightning = helpers.getNewBasicAttackEntity(
            vector2(xPos, bossPos.Y - 400),
            helpers.getHitbox(40, 800, -20, 0),
            "characters/asriel_god/lightning",
            true
        )
        lightning.Speed = vector2(0, 600)
        lightning:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(lightning)
        wait(0.08)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
