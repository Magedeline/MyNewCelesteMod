local MAX_HEALTH = 14
local PHASE2_THRESHOLD = 9
local PHASE3_THRESHOLD = 4

function setup()
    helpers.setHealth(MAX_HEALTH)
    helpers.setEffectiveGravityMult(0.5)
    helpers.setHitCooldown(0.8)
    helpers.storeObjectInBoss("currentPhase", 1)
end

function onHit()
    helpers.decreaseHealth(1)
    local currentHealth = helpers.getHealth()
    local currentPhase = helpers.getStoredObjectFromBoss("currentPhase")
    helpers.playPuppetAnim("hurt")
    playSound("event:/possessed_tenna_hurt")
    
    if currentHealth <= PHASE3_THRESHOLD and currentPhase < 3 then
        helpers.storeObjectInBoss("currentPhase", 3)
        helpers.interruptPattern()
        helpers.startAttackPattern(helpers.getPatternIndex("phase3_transition"))
    elseif currentHealth <= PHASE2_THRESHOLD and currentPhase < 2 then
        helpers.storeObjectInBoss("currentPhase", 2)
        helpers.interruptPattern()
        helpers.startAttackPattern(helpers.getPatternIndex("phase2_transition"))
    end
    if currentHealth <= 0 then helpers.interruptPattern() helpers.startAttackPattern(helpers.getPatternIndex("death")) end
end

function onDash() onHit() end
function onBounce() onHit() end
