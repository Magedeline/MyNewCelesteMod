function onBegin() helpers.playPuppetAnim("final_form") playSound("event:/asriel_god_final") level.Shake(2.0) wait(2.0) helpers.sayExt("ASRIEL_GOD_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
