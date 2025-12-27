--[[
    Apex Predator Boss - Frenzy Attack
    Rapid assault of slashes and bites
]]

function onBegin()
    helpers.playPuppetAnim("frenzy_start")
    playSound("event:/apex_predator_frenzy")
    wait(0.3)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dirX = playerPos.X > bossPos.X and 1 or -1
    
    -- Rapid attacks
    for i = 1, 6 do
        local attackType = i % 3
        
        if attackType == 0 then
            helpers.playPuppetAnim("frenzy_claw")
            local claw = helpers.getNewBasicAttackEntity(
                puppet.Position,
                helpers.getHitbox(50, 30, dirX > 0 and 0 or -50, -15),
                "characters/apex_predator/frenzy_claw",
                true
            )
            claw:Add(helpers.getEntityTimer(0.1))
            helpers.addEntity(claw)
        elseif attackType == 1 then
            helpers.playPuppetAnim("frenzy_bite")
            local bite = helpers.getNewBasicAttackEntity(
                puppet.Position,
                helpers.getHitbox(40, 40, dirX > 0 and 10 or -50, -20),
                "characters/apex_predator/frenzy_bite",
                true
            )
            bite:Add(helpers.getEntityTimer(0.1))
            helpers.addEntity(bite)
        else
            helpers.playPuppetAnim("frenzy_tail")
            local tail = helpers.getNewBasicAttackEntity(
                puppet.Position,
                helpers.getCircle(50),
                "characters/apex_predator/frenzy_tail",
                true
            )
            tail:Add(helpers.getEntityTimer(0.1))
            helpers.addEntity(tail)
        end
        
        playSound("event:/apex_predator_frenzy_hit")
        helpers.setSpeed(dirX * 80, 0)
        wait(0.12)
        helpers.setSpeed(0, 0)
    end
    
    helpers.playPuppetAnim("frenzy_end")
    wait(0.4)
    helpers.playPuppetAnim("idle")
end
