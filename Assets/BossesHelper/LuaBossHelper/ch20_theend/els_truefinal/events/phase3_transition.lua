function onBegin() helpers.playPuppetAnim("absolute_form") playSound("event:/els_final_phase3") level.Shake(4.0) wait(3.0) helpers.sayExt("ELS_TRUEFINAL_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
