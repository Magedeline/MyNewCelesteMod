--[[
    Tower Titan Boss - Setup and Collision Functions
]]

local MAX_HEALTH = 20
local PHASE2_THRESHOLD = 14
local PHASE3_THRESHOLD = 7

function setup()
    helpers.setHealth(MAX_HEALTH)
    helpers.setEffectiveGravityMult(1.5) -- Heavy boss
    helpers.setGroundFriction(300)
    helpers.setHitCooldown(1.2)
    helpers.storeObjectInBoss("currentPhase", 1)
end

function onHit()
    helpers.decreaseHealth(1)
    local currentHealth = helpers.getHealth()
    local currentPhase = helpers.getStoredObjectFromBoss("currentPhase")
    
    helpers.playPuppetAnim("hurt")
    playSound("event:/tower_titan_hurt")
    
    if currentHealth <= PHASE3_THRESHOLD and currentPhase < 3 then
        helpers.storeObjectInBoss("currentPhase", 3)
        helpers.interruptPattern()
        helpers.startAttackPattern(helpers.getPatternIndex("phase3_transition"))
    elseif currentHealth <= PHASE2_THRESHOLD and currentPhase < 2 then
        helpers.storeObjectInBoss("currentPhase", 2)
        helpers.interruptPattern()
        helpers.startAttackPattern(helpers.getPatternIndex("phase2_transition"))
    end
    
    if currentHealth <= 0 then
        helpers.interruptPattern()
        helpers.startAttackPattern(helpers.getPatternIndex("death"))
    end
end

function onDash()
    onHit()
end

function onBounce()
    -- Titan has hard armor, normal damage
    onHit()
end
