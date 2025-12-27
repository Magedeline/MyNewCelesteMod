function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("power_up") playSound("event:/els_p2_intro") level.Shake(0.5) wait(1.2) helpers.sayExt("ELS_PHASE2_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
