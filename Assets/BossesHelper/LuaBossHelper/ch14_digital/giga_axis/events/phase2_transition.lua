function onBegin() helpers.playPuppetAnim("transform") playSound("event:/giga_axis_transform") level.Shake(0.8) wait(1.2) helpers.sayExt("GIGA_AXIS_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
