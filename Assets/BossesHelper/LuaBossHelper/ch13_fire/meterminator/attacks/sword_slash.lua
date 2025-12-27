--[[
    Meterminator Knight Boss - Sword Slash Attack
    Quick three-hit sword combo
]]

function onBegin()
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dirX = playerPos.X > bossPos.X and 1 or -1
    
    for i = 1, 3 do
        helpers.playPuppetAnim("slash_" .. i)
        playSound("event:/meterminator_slash")
        
        local slashHitbox = helpers.getHitbox(70, 50, dirX > 0 and 0 or -70, -25)
        local slash = helpers.getNewBasicAttackEntity(
            puppet.Position,
            slashHitbox,
            "characters/meterminator/sword_slash_" .. i,
            true
        )
        slash:Add(helpers.getEntityTimer(0.15))
        helpers.addEntity(slash)
        
        helpers.setSpeed(dirX * 120, 0)
        wait(0.15)
        helpers.setSpeed(0, 0)
        wait(0.1)
    end
    
    helpers.playPuppetAnim("idle")
end
