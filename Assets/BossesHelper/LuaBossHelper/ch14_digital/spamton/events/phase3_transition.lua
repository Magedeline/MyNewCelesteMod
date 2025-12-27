function onBegin() helpers.playPuppetAnim("neo_transform") playSound("event:/spamton_neo_transform") level.Shake(1.5) wait(1.5) helpers.sayExt("SPAMTON_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
