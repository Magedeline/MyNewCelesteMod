function onBegin() helpers.playPuppetAnim("glitch") playSound("event:/spamton_glitch") level.Shake(0.5) wait(0.6) helpers.sayExt("SPAMTON_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
