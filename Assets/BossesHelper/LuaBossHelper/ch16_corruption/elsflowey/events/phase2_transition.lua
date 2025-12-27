function onBegin() helpers.playPuppetAnim("transform") playSound("event:/elsflowey_transform") level.Shake(0.8) wait(1.0) helpers.sayExt("ELSFLOWEY_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
