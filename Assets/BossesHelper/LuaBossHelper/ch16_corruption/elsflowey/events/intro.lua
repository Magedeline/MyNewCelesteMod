function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("emerge") playSound("event:/elsflowey_emerge") wait(1.0) helpers.sayExt("ELSFLOWEY_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
