function onBegin() helpers.playPuppetAnim("omega_transform") playSound("event:/elsflowey_omega_transform") level.Shake(2.0) wait(2.0) helpers.sayExt("ELSFLOWEY_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
