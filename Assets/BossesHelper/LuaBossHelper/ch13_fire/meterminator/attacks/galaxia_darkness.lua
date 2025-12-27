--[[
    Meterminator Knight Boss - Galaxia Darkness Ultimate
]]

function onBegin()
    helpers.playPuppetAnim("galaxia_start")
    playSound("event:/meterminator_galaxia_charge")
    wait(0.8)
    
    -- Screen darkens
    level.Flash(helpers.celeste.Color.Black, false)
    helpers.playPuppetAnim("galaxia_prepare")
    wait(0.5)
    
    local playerPos = player.Position
    
    -- Multiple slashes in darkness
    for i = 1, 5 do
        local offsetX = (math.random() - 0.5) * 100
        local offsetY = (math.random() - 0.5) * 60
        
        puppet.Position = vector2(playerPos.X + offsetX, playerPos.Y + offsetY)
        helpers.playPuppetAnim("galaxia_slash")
        playSound("event:/meterminator_galaxia_slash")
        
        local slash = helpers.getNewBasicAttackEntity(
            puppet.Position,
            helpers.getCircle(60),
            "characters/meterminator/galaxia_slash",
            true
        )
        slash:Add(helpers.getEntityTimer(0.15))
        helpers.addEntity(slash)
        
        wait(0.15)
    end
    
    -- Final strike
    helpers.playPuppetAnim("galaxia_final")
    playSound("event:/meterminator_galaxia_final")
    level.Shake(1.0)
    
    local finalSlash = helpers.getNewBasicAttackEntity(
        puppet.Position,
        helpers.getCircle(100),
        "characters/meterminator/galaxia_final",
        true
    )
    finalSlash:Add(helpers.getEntityTimer(0.3))
    helpers.addEntity(finalSlash)
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
