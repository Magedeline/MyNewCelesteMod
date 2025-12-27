function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("appear") playSound("event:/els_p1_intro") wait(1.0) helpers.sayExt("ELS_PHASE1_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
