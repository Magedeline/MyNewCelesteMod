function onBegin() helpers.playPuppetAnim("power_stance") playSound("event:/meterminator_power") level.Shake(0.5) wait(0.8) helpers.sayExt("METERMINATOR_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
