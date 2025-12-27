function onBegin()
    helpers.playPuppetAnim("berserk")
    playSound("event:/tower_titan_berserk")
    level.Shake(1.0)
    wait(1.0)
    helpers.sayExt("TOWER_TITAN_PHASE3")
    helpers.playPuppetAnim("idle")
end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
