--[[
    Apex Predator Boss - Claw Swipe Attack
    Triple claw swipe combo
]]

function onBegin()
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dirX = playerPos.X > bossPos.X and 1 or -1
    
    for i = 1, 3 do
        helpers.playPuppetAnim("claw_" .. i)
        playSound("event:/apex_predator_claw")
        
        local clawHitbox = helpers.getHitbox(60, 40, dirX > 0 and 0 or -60, -20)
        local claw = helpers.getNewBasicAttackEntity(
            puppet.Position,
            clawHitbox,
            "characters/apex_predator/claw_slash_" .. i,
            true
        )
        claw:Add(helpers.getEntityTimer(0.15))
        helpers.addEntity(claw)
        
        -- Small forward movement
        helpers.setSpeed(dirX * 100, 0)
        wait(0.12)
        helpers.setSpeed(0, 0)
        wait(0.08)
    end
    
    helpers.playPuppetAnim("idle")
end
