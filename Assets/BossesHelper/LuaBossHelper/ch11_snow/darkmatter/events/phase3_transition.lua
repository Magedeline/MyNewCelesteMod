function onBegin()
    helpers.playPuppetAnim("void_awakening")
    playSound("event:/darkmatter_void_awaken")
    level.Shake(1.0)
    wait(1.0)
    helpers.sayExt("DARKMATTER_PHASE3")
    helpers.playPuppetAnim("idle")
end

function onEnd(level, wasSkipped)
    if wasSkipped then helpers.playPuppetAnim("idle") end
end
