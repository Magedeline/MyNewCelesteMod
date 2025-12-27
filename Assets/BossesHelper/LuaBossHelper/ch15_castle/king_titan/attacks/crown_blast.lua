--[[
    King Titan Boss - Crown Blast Attack
]]

function onBegin()
    helpers.playPuppetAnim("crown_charge")
    playSound("event:/king_titan_crown_charge")
    level.Shake(0.5)
    wait(0.8)
    
    helpers.playPuppetAnim("crown_blast")
    playSound("event:/king_titan_crown_blast")
    
    local bossPos = puppet.Position
    
    -- Radial crown energy blast
    for ring = 1, 3 do
        local numProjectiles = 12 + ring * 4
        for i = 1, numProjectiles do
            local angle = (i / numProjectiles) * math.pi * 2
            local blast = helpers.getNewBasicAttackEntity(
                bossPos,
                helpers.getCircle(14),
                "characters/king_titan/crown_energy",
                true
            )
            blast.Speed = vector2(math.cos(angle) * (180 + ring * 40), math.sin(angle) * (180 + ring * 40))
            blast:Add(helpers.getEntityTimer(2.0))
            helpers.addEntity(blast)
        end
        wait(0.25)
    end
    
    wait(0.4)
    helpers.playPuppetAnim("idle")
end
