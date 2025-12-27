function onBegin() helpers.playPuppetAnim("core_reveal") playSound("event:/axis_core") level.Shake(1.0) wait(1.0) helpers.sayExt("AXIS_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
