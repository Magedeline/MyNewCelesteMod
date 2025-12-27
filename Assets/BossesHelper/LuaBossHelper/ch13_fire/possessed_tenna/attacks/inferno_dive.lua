--[[
    Possessed Tenna Boss - Inferno Dive Attack
]]

function onBegin()
    helpers.playPuppetAnim("rise")
    helpers.setSpeed(0, -250)
    wait(0.5)
    
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("inferno_charge")
    playSound("event:/possessed_tenna_inferno")
    wait(0.4)
    
    local playerPos = player.Position
    local bossPos = puppet.Position
    
    helpers.playPuppetAnim("inferno_dive")
    local dir = helpers.normalize(vector2(playerPos.X - bossPos.X, playerPos.Y - bossPos.Y + 50))
    helpers.setSpeed(dir.X * 400, dir.Y * 400)
    
    -- Create fire trail
    helpers.addConstantBackgroundCoroutine(function()
        for i = 1, 15 do
            local trail = helpers.getNewBasicAttackEntity(
                puppet.Position,
                helpers.getCircle(12),
                "characters/possessed_tenna/fire_trail",
                true
            )
            trail:Add(helpers.getEntityTimer(1.0))
            helpers.addEntity(trail)
            wait(0.05)
        end
    end)
    
    wait(0.8)
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("idle")
end
