function onBegin()
    helpers.playPuppetAnim("power_up")
    playSound("event:/darkmatter_powerup")
    level.Shake(0.5)
    wait(0.8)
    helpers.sayExt("DARKMATTER_PHASE2")
    helpers.playPuppetAnim("idle")
end

function onEnd(level, wasSkipped)
    if wasSkipped then helpers.playPuppetAnim("idle") end
end
