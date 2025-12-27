--[[
    Dark the Dark Matter Boss - Katana Slash Attack
    Fast horizontal slash with the black katana
]]

function onBegin()
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dirX = playerPos.X > bossPos.X and 1 or -1
    
    -- Prepare stance
    helpers.playPuppetAnim("katana_ready")
    wait(0.3)
    
    -- Quick dash slash
    helpers.playPuppetAnim("katana_slash")
    playSound("event:/darkmatter_slash")
    
    -- Create slash hitbox
    local slashHitbox = helpers.getHitbox(80, 40, dirX > 0 and 0 or -80, -20)
    local slash = helpers.getNewBasicAttackEntity(
        bossPos,
        slashHitbox,
        "characters/darkmatter/slash_effect",
        true
    )
    slash:Add(helpers.getEntityTimer(0.3))
    helpers.addEntity(slash)
    
    -- Dash forward
    helpers.setSpeed(dirX * 400, 0)
    helpers.keepSpeedDuring(0.25)
    
    wait(0.25)
    helpers.setSpeed(0, 0)
    
    -- Recovery
    helpers.playPuppetAnim("katana_sheathe")
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
