function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("true_form") playSound("event:/els_p3_intro") level.Shake(1.5) wait(2.0) helpers.sayExt("ELS_PHASE3_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
