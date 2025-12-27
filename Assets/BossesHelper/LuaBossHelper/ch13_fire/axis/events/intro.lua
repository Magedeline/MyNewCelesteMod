function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("activate") playSound("event:/axis_activate") wait(1.0) helpers.sayExt("AXIS_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
