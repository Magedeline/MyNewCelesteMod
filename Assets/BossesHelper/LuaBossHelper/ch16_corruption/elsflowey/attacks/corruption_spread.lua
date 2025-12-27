--[[
    ElsFlowey Boss - Corruption Spread Attack
]]

function onBegin()
    helpers.playPuppetAnim("corrupt_channel")
    playSound("event:/elsflowey_corrupt")
    level.Shake(0.5)
    wait(0.6)
    
    helpers.playPuppetAnim("corrupt_spread")
    
    local bossPos = puppet.Position
    
    -- Dark corruption spreads across arena
    for wave = 1, 4 do
        local radius = wave * 80
        local numPatches = 8 + wave * 2
        
        for i = 1, numPatches do
            local angle = (i / numPatches) * math.pi * 2
            local patch = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + math.cos(angle) * radius, bossPos.Y + math.sin(angle) * radius),
                helpers.getCircle(25),
                "characters/elsflowey/corruption_patch",
                true
            )
            patch:Add(helpers.getEntityTimer(2.5))
            helpers.addEntity(patch)
        end
        playSound("event:/elsflowey_corrupt_wave")
        wait(0.3)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
