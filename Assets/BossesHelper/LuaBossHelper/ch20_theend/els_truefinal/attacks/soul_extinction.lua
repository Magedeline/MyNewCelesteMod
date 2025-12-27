--[[
    Els True Final Boss - Soul Extinction Attack
]]

function onBegin()
    helpers.playPuppetAnim("extinction_channel")
    playSound("event:/els_final_extinction")
    level.Shake(1.0)
    wait(0.8)
    
    helpers.playPuppetAnim("extinction")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    -- Soul extraction chains
    for i = 1, 6 do
        local angle = (i / 6) * math.pi * 2
        local chainStart = vector2(bossPos.X + math.cos(angle) * 80, bossPos.Y + math.sin(angle) * 80)
        
        -- Chain toward player
        for seg = 1, 12 do
            local t = seg / 12
            local chain = helpers.getNewBasicAttackEntity(
                vector2(
                    chainStart.X + (playerPos.X - chainStart.X) * t,
                    chainStart.Y + (playerPos.Y - chainStart.Y) * t
                ),
                helpers.getCircle(14),
                "characters/els_final/extinction_chain",
                true
            )
            chain:Add(helpers.getEntityTimer(1.0))
            helpers.addEntity(chain)
        end
        wait(0.1)
    end
    
    wait(0.5)
    
    -- Soul fragments fly outward from player
    for i = 1, 12 do
        local angle = (i / 12) * math.pi * 2
        local soul = helpers.getNewBasicAttackEntity(
            playerPos,
            helpers.getCircle(15),
            "characters/els_final/extinct_soul",
            true
        )
        soul.Speed = vector2(math.cos(angle) * 200, math.sin(angle) * 200)
        soul:Add(helpers.getEntityTimer(2.0))
        helpers.addEntity(soul)
    end
    
    helpers.playPuppetAnim("idle")
end
