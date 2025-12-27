--[[
    Dark the Dark Matter Boss - Shadow Clone Attack
    Creates shadow clones that attack simultaneously
]]

function onBegin()
    helpers.playPuppetAnim("clone_summon")
    playSound("event:/darkmatter_clone")
    wait(0.6)
    
    local bossPos = puppet.Position
    local clones = {}
    
    -- Spawn 2 shadow clones
    for i = 1, 2 do
        local offsetX = (i == 1) and -100 or 100
        local clonePos = vector2(bossPos.X + offsetX, bossPos.Y)
        
        local hitbox = helpers.getHitbox(24, 32, -12, -32)
        local clone = helpers.getNewBasicAttackEntity(
            clonePos,
            hitbox,
            "characters/darkmatter/shadow_clone",
            true
        )
        clone:Add(helpers.getEntityTimer(3.0))
        helpers.addEntity(clone)
        table.insert(clones, clone)
    end
    
    wait(0.3)
    
    -- All clones dash attack towards player
    helpers.playPuppetAnim("clone_attack")
    local playerPos = player.Position
    
    for _, clone in ipairs(clones) do
        if clone and clone.Scene then
            local dirX = playerPos.X > clone.Position.X and 1 or -1
            clone.Speed = vector2(dirX * 300, 0)
        end
    end
    
    -- Boss also attacks
    local bossDirX = playerPos.X > bossPos.X and 1 or -1
    helpers.setSpeed(bossDirX * 300, 0)
    helpers.keepSpeedDuring(0.4)
    
    wait(0.4)
    helpers.setSpeed(0, 0)
    
    wait(0.5)
    helpers.destroyAll()
    helpers.playPuppetAnim("idle")
end
