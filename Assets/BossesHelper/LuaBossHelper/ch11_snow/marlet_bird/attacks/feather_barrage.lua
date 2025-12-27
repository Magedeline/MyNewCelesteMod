--[[
    Marlet Possessed Bird Boss - Feather Barrage Attack
    Shoots feathers in a spread pattern
]]

function onBegin()
    helpers.playPuppetAnim("feather_charge")
    playSound("event:/marlet_feather_charge")
    wait(0.5)
    
    helpers.playPuppetAnim("feather_shoot")
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    
    -- Calculate base direction to player
    local baseDirX = playerPos.X - bossPos.X
    local baseDirY = playerPos.Y - bossPos.Y
    local baseAngle = math.atan2(baseDirY, baseDirX)
    
    -- Fire 7 feathers in a spread
    for i = -3, 3 do
        local spreadAngle = baseAngle + (i * math.rad(12))
        
        local hitbox = helpers.getHitbox(12, 6, -6, -3)
        local feather = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X, bossPos.Y),
            hitbox,
            "characters/marlet_bird/feather",
            true
        )
        
        feather.Speed = vector2(
            math.cos(spreadAngle) * 220,
            math.sin(spreadAngle) * 220
        )
        
        feather:Add(helpers.getEntityTimer(3.0))
        helpers.addEntity(feather)
        
        playSound("event:/marlet_feather_fire")
        wait(0.05)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
