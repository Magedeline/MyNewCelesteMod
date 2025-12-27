--[[
    King Digital Dee Dee Dee Boss - Gordo Throw Attack
]]

function onBegin()
    helpers.playPuppetAnim("gordo_summon")
    playSound("event:/king_ddd_gordo")
    wait(0.3)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    helpers.playPuppetAnim("gordo_throw")
    
    for i = 1, 3 do
        local gordo = helpers.getNewBasicAttackEntity(
            bossPos,
            helpers.getCircle(18),
            "characters/king_ddd/gordo",
            true
        )
        
        local angle = math.rad(-45 - i * 15)
        gordo.Speed = vector2(math.cos(angle) * 300 * (playerPos.X > bossPos.X and 1 or -1), math.sin(angle) * 300)
        gordo:Add(helpers.getEntityTimer(3.0))
        helpers.addEntity(gordo)
        
        playSound("event:/king_ddd_throw")
        wait(0.2)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
