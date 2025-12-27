--[[
    King Titan Boss - Royal Decree Attack
]]

function onBegin()
    helpers.playPuppetAnim("decree_raise")
    playSound("event:/king_titan_decree")
    wait(0.6)
    
    helpers.playPuppetAnim("decree_cast")
    
    local bossPos = puppet.Position
    
    -- Royal energy beams from above
    for i = 1, 7 do
        local xPos = bossPos.X - 180 + i * 60
        local beam = helpers.getNewBasicAttackEntity(
            vector2(xPos, bossPos.Y - 300),
            helpers.getHitbox(40, 600, -20, 0),
            "characters/king_titan/royal_beam",
            true
        )
        beam.Speed = vector2(0, 400)
        beam:Add(helpers.getEntityTimer(2.0))
        helpers.addEntity(beam)
        wait(0.1)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
