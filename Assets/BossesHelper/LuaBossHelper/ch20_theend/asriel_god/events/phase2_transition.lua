function onBegin() helpers.playPuppetAnim("power_surge") playSound("event:/asriel_god_surge") level.Shake(1.0) wait(1.2) helpers.sayExt("ASRIEL_GOD_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
