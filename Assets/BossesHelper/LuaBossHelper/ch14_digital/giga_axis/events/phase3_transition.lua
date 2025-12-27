function onBegin() helpers.playPuppetAnim("final_form") playSound("event:/giga_axis_final") level.Shake(1.5) wait(1.5) helpers.sayExt("GIGA_AXIS_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
