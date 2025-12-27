function onBegin()
    helpers.playPuppetAnim("rage")
    playSound("event:/tower_titan_rage")
    level.Shake(0.5)
    wait(0.8)
    helpers.sayExt("TOWER_TITAN_PHASE2")
    helpers.playPuppetAnim("idle")
end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
