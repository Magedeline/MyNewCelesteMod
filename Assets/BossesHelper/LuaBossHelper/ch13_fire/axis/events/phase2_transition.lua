function onBegin() helpers.playPuppetAnim("overcharge") playSound("event:/axis_overcharge") level.Shake(0.5) wait(0.8) helpers.sayExt("AXIS_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
