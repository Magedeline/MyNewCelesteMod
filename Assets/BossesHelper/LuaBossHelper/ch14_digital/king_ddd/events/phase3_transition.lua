function onBegin() helpers.playPuppetAnim("enrage") playSound("event:/king_ddd_enrage") level.Shake(1.0) wait(1.0) helpers.sayExt("KING_DDD_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
