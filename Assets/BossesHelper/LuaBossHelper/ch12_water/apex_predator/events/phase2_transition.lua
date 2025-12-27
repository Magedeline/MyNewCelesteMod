function onBegin() helpers.playPuppetAnim("enrage") playSound("event:/apex_predator_enrage") level.Shake(0.5) wait(0.8) helpers.sayExt("APEX_PREDATOR_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
