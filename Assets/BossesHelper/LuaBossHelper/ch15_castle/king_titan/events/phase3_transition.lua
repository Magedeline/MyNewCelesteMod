function onBegin() helpers.playPuppetAnim("divine_form") playSound("event:/king_titan_divine") level.Shake(1.5) wait(1.5) helpers.sayExt("KING_TITAN_PHASE3") helpers.playPuppetAnim("idle") end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") end end
