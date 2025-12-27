function onBegin() helpers.playPuppetAnim("galaxia_reveal") playSound("event:/meterminator_galaxia") level.Shake(1.0) wait(1.0) helpers.sayExt("METERMINATOR_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
