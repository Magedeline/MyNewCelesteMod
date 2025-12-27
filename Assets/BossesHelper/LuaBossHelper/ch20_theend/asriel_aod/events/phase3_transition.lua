function onBegin() helpers.playPuppetAnim("true_form") playSound("event:/asriel_aod_true") level.Shake(2.5) wait(2.5) helpers.sayExt("ASRIEL_AOD_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
