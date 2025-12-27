function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("appear") playSound("event:/spamton_appear") wait(0.8) helpers.sayExt("SPAMTON_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
