local MAX_HEALTH = 20

function setup()
    helpers.setHealth(MAX_HEALTH)
    helpers.setEffectiveGravityMult(0)
    helpers.setHitCooldown(0.7)
end

function onHit()
    helpers.decreaseHealth(1)
    local currentHealth = helpers.getHealth()
    helpers.playPuppetAnim("hurt")
    playSound("event:/els_p3_hurt")
    if currentHealth <= 0 then helpers.interruptPattern() helpers.startAttackPattern(helpers.getPatternIndex("death")) end
end

function onDash() onHit() end
function onBounce() onHit() end
