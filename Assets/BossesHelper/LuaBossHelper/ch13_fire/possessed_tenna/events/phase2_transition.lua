function onBegin() helpers.playPuppetAnim("power_surge") playSound("event:/possessed_tenna_surge") level.Shake(0.5) wait(0.8) helpers.sayExt("POSSESSED_TENNA_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
