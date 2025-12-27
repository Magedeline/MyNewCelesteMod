--[[
    Meterminator Knight Boss - Sword Beam Attack
    Fires energy beam from sword
]]

function onBegin()
    helpers.playPuppetAnim("beam_charge")
    playSound("event:/meterminator_beam_charge")
    wait(0.6)
    
    helpers.playPuppetAnim("beam_fire")
    playSound("event:/meterminator_beam")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dirX = playerPos.X > bossPos.X and 1 or -1
    
    -- Create beam projectile
    local beam = helpers.getNewBasicAttackEntity(
        vector2(bossPos.X + dirX * 30, bossPos.Y - 15),
        helpers.getHitbox(80, 20, 0, -10),
        "characters/meterminator/sword_beam",
        true
    )
    beam.Speed = vector2(dirX * 350, 0)
    beam:Add(helpers.getEntityTimer(2.0))
    helpers.addEntity(beam)
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
