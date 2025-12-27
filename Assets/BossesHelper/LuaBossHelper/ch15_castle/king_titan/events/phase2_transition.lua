function onBegin() helpers.playPuppetAnim("enraged") playSound("event:/king_titan_rage") level.Shake(0.8) wait(1.0) helpers.sayExt("KING_TITAN_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
