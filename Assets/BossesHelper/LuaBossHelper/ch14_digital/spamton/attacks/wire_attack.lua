--[[
    Spamton Boss - Wire Attack
]]

function onBegin()
    helpers.playPuppetAnim("wire_extend")
    playSound("event:/spamton_wire")
    wait(0.3)
    
    local bossPos = puppet.Position
    
    -- Extend wires in cardinal directions
    local directions = {
        {0, -1},  -- up
        {1, 0},   -- right
        {0, 1},   -- down
        {-1, 0}   -- left
    }
    
    for _, dir in ipairs(directions) do
        for i = 1, 10 do
            local wire = helpers.getNewBasicAttackEntity(
                vector2(bossPos.X + dir[1] * i * 30, bossPos.Y + dir[2] * i * 30),
                helpers.getHitbox(25, 25, -12, -12),
                "characters/spamton/wire",
                true
            )
            wire:Add(helpers.getEntityTimer(1.0))
            helpers.addEntity(wire)
        end
    end
    
    wait(0.8)
    helpers.playPuppetAnim("idle")
end
