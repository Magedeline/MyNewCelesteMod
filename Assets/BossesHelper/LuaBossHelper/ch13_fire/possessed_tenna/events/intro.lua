function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("possessed_awaken") playSound("event:/possessed_tenna_awaken") wait(1.0) helpers.sayExt("POSSESSED_TENNA_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
