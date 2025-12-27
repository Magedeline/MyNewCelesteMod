function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("manifest") playSound("event:/els_final_intro") level.Shake(3.0) wait(3.0) helpers.sayExt("ELS_TRUEFINAL_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
