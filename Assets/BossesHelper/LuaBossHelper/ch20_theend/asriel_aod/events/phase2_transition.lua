function onBegin() helpers.playPuppetAnim("rage") playSound("event:/asriel_aod_rage") level.Shake(1.2) wait(1.5) helpers.sayExt("ASRIEL_AOD_PHASE2") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
