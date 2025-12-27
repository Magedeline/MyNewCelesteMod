function onBegin() helpers.playPuppetAnim("rage") playSound("event:/king_ddd_rage") level.Shake(0.5) wait(0.8) helpers.sayExt("KING_DDD_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
