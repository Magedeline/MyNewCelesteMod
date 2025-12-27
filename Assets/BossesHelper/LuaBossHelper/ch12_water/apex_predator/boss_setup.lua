--[[
    Apex Predator Boss - Setup
]]

local MAX_HEALTH = 16
local PHASE2_THRESHOLD = 11
local PHASE3_THRESHOLD = 5

function setup()
    helpers.setHealth(MAX_HEALTH)
    helpers.setEffectiveGravityMult(1.0)
    helpers.setGroundFriction(400)
    helpers.setAirFriction(50)
    helpers.setHitCooldown(0.5)
    helpers.storeObjectInBoss("currentPhase", 1)
end

function onHit()
    helpers.decreaseHealth(1)
    local currentHealth = helpers.getHealth()
    local currentPhase = helpers.getStoredObjectFromBoss("currentPhase")
    
    helpers.playPuppetAnim("hurt")
    playSound("event:/apex_predator_hurt")
    
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

function onDash() onHit() end
function onBounce() helpers.decreaseHealth(1) onHit() end
