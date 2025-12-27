--[[
    Wispy Woods Boss - Setup and Collision Functions
]]

-- Health configuration
local MAX_HEALTH = 12
local PHASE2_THRESHOLD = 8
local PHASE3_THRESHOLD = 4

function setup()
    -- Initialize boss health
    helpers.setHealth(MAX_HEALTH)
    
    -- Set gravity (Wispy Woods is stationary)
    helpers.setEffectiveGravityMult(0)
    
    -- Disable solid collisions (boss doesn't move)
    helpers.disableSolidCollisions()
    
    -- Set hit cooldown
    helpers.setHitCooldown(1.0)
    
    -- Store phase tracking
    helpers.storeObjectInBoss("currentPhase", 1)
end

function onHit()
    -- Decrease health
    helpers.decreaseHealth(1)
    
    local currentHealth = helpers.getHealth()
    local currentPhase = helpers.getStoredObjectFromBoss("currentPhase")
    
    -- Play hit animation
    helpers.playPuppetAnim("hurt")
    playSound("event:/wispywoods_hurt")
    
    -- Check for phase transitions
    if currentHealth <= PHASE3_THRESHOLD and currentPhase < 3 then
        helpers.storeObjectInBoss("currentPhase", 3)
        helpers.interruptPattern()
        helpers.startAttackPattern(helpers.getPatternIndex("phase3_transition"))
        
    elseif currentHealth <= PHASE2_THRESHOLD and currentPhase < 2 then
        helpers.storeObjectInBoss("currentPhase", 2)
        helpers.interruptPattern()
        helpers.startAttackPattern(helpers.getPatternIndex("phase2_transition"))
    end
    
    -- Check for death
    if currentHealth <= 0 then
        helpers.interruptPattern()
        helpers.startAttackPattern(helpers.getPatternIndex("death"))
    end
end

function onDash()
    onHit()
end

function onBounce()
    -- Bouncing on Wispy Woods deals double damage
    helpers.decreaseHealth(1) -- Extra damage (onHit adds 1 more)
    onHit()
end
