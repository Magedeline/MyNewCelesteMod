--[[
    King Titan Boss - Divine Judgment Attack (Ultimate)
]]

function onBegin()
    helpers.playPuppetAnim("judgment_prepare")
    playSound("event:/king_titan_judgment_charge")
    level.Shake(1.5)
    wait(1.5)
    
    helpers.playPuppetAnim("judgment")
    playSound("event:/king_titan_judgment")
    level.Shake(2.0)
    
    local bossPos = puppet.Position
    
    -- Screen-wide judgment beams
    for wave = 1, 5 do
        -- Horizontal beams
        for i = 1, 8 do
            local yPos = bossPos.Y - 200 + i * 50
            local hBeam = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X - 400, yPos),
                helpers.getHitbox(800, 20, 0, -10),
                "characters/king_titan/judgment_beam",
                true
            )
            hBeam:Add(helpers.getEntityTimer(0.6))
            helpers.addEntity(hBeam)
        end
        wait(0.8)
        
        -- Vertical beams
        for i = 1, 10 do
            local xPos = bossPos.X - 250 + i * 50
            local vBeam = helpers.getNewBasicAttackEntity(
                vector2(xPos, bossPos.Y - 300),
                helpers.getHitbox(20, 600, -10, 0),
                "characters/king_titan/judgment_beam",
                true
            )
            vBeam:Add(helpers.getEntityTimer(0.6))
            helpers.addEntity(vBeam)
        end
        wait(0.5)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
