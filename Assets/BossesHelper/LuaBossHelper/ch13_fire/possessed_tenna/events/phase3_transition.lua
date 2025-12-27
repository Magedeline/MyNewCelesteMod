function onBegin() helpers.playPuppetAnim("full_possession") playSound("event:/possessed_tenna_full") level.Shake(1.0) wait(1.0) helpers.sayExt("POSSESSED_TENNA_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
