--[[
    Marlet Possessed Bird Boss - Blizzard Attack
    Creates a localized blizzard that obscures vision and damages
]]

function onBegin()
    helpers.playPuppetAnim("blizzard_summon")
    playSound("event:/marlet_blizzard_start")
    wait(0.8)
    
    helpers.playPuppetAnim("blizzard_maintain")
    
    local bossPos = puppet.Position
    local duration = 4.0
    local startTime = helpers.engine.RawElapsedTime
    
    -- Spawn multiple blizzard hazard zones
    local blizzardZones = {}
    for i = 1, 3 do
        local offsetX = (i - 2) * 80
        local zonePos = vector2(bossPos.X + offsetX, bossPos.Y + 50)
        
        local hitbox = helpers.getHitbox(60, 120, -30, -60)
        local zone = helpers.getNewBasicAttackEntity(
            zonePos,
            hitbox,
            "characters/marlet_bird/blizzard_zone",
            true
        )
        zone:Add(helpers.getEntityTimer(duration))
        helpers.addEntity(zone)
        table.insert(blizzardZones, zone)
    end
    
    -- Spawn snow particles throughout
    while helpers.engine.RawElapsedTime - startTime < duration do
        for _, zone in ipairs(blizzardZones) do
            if zone and zone.Scene then
                local particlePos = vector2(
                    zone.Position.X + (math.random() - 0.5) * 60,
                    zone.Position.Y - 60
                )
                
                local snowflake = helpers.getNewBasicAttackEntity(
                    particlePos,
                    helpers.getCircle(4),
                    "characters/marlet_bird/snowflake",
                    true
                )
                snowflake.Speed = vector2((math.random() - 0.5) * 40, 100)
                snowflake:Add(helpers.getEntityTimer(1.5))
                helpers.addEntity(snowflake)
            end
        end
        wait(0.15)
    end
    
    helpers.playPuppetAnim("idle")
end
