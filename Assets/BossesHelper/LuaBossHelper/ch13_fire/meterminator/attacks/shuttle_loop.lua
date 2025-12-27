--[[
    Meterminator Knight Boss - Shuttle Loop Attack
    Aerial loop attack
]]

function onBegin()
    helpers.playPuppetAnim("shuttle_start")
    playSound("event:/meterminator_shuttle")
    wait(0.2)
    
    local bossPos = puppet.Position
    local playerPos = player.Position
    local dirX = playerPos.X > bossPos.X and 1 or -1
    
    helpers.playPuppetAnim("shuttle_loop")
    
    -- Upward arc
    helpers.setSpeed(dirX * 200, -350)
    wait(0.3)
    
    -- Create sword hitbox during loop
    local swordHitbox = helpers.getHitbox(60, 60, -30, -30)
    local sword = helpers.getNewBasicAttackEntity(
        puppet.Position,
        swordHitbox,
        "characters/meterminator/shuttle_sword",
        true
    )
    helpers.storeObjectInBoss("shuttleSword", sword)
    
    -- Loop motion
    helpers.setSpeed(dirX * 350, 100)
    wait(0.2)
    helpers.setSpeed(dirX * 350, 300)
    wait(0.3)
    
    helpers.destroyAll()
    helpers.deleteStoredObjectFromBoss("shuttleSword")
    
    helpers.setSpeed(0, 0)
    helpers.playPuppetAnim("shuttle_land")
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
