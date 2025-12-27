--[[
    Apex Predator Boss - Tail Whip Attack
    360 degree tail sweep
]]

function onBegin()
    helpers.playPuppetAnim("tail_windup")
    wait(0.3)
    
    helpers.playPuppetAnim("tail_spin")
    playSound("event:/apex_predator_tail")
    
    local bossPos = puppet.Position
    
    -- Create circular hitbox that spins
    local tailHitbox = helpers.getCircle(70)
    local tailAttack = helpers.getNewBasicAttackEntity(
        bossPos,
        tailHitbox,
        "characters/apex_predator/tail_sweep",
        true
    )
    tailAttack:Add(helpers.getEntityTimer(0.4))
    helpers.addEntity(tailAttack)
    
    wait(0.4)
    helpers.playPuppetAnim("idle")
end
