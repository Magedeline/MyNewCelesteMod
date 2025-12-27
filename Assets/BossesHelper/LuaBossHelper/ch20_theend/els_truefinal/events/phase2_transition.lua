function onBegin() helpers.playPuppetAnim("ascend_form") playSound("event:/els_final_phase2") level.Shake(2.0) wait(2.0) helpers.sayExt("ELS_TRUEFINAL_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
