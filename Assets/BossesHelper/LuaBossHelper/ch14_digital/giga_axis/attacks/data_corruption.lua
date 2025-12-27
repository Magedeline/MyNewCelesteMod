--[[
    Giga Axis Boss - Data Corruption Attack
]]

function onBegin()
    helpers.playPuppetAnim("corrupt_charge")
    playSound("event:/giga_axis_corrupt")
    wait(0.5)
    
    helpers.playPuppetAnim("corrupt_spread")
    
    -- Create corrupted zones across the arena
    local arenaWidth = 400
    local numZones = 6
    
    for i = 1, numZones do
        local xPos = puppet.Position.X - arenaWidth/2 + (i-1) * (arenaWidth / (numZones-1))
        local zone = helpers.getNewBasicAttackEntity(
            vector2(xPos, puppet.Position.Y + 100),
            helpers.getHitbox(50, 200, -25, -100),
            "characters/giga_axis/corruption_zone",
            true
        )
        zone:Add(helpers.getEntityTimer(3.0))
        helpers.addEntity(zone)
        wait(0.15)
    end
    
    wait(0.5)
    helpers.playPuppetAnim("idle")
end
