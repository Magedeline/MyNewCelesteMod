function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("emerge") playSound("event:/giga_axis_emerge") level.Shake(1.0) wait(1.5) helpers.sayExt("GIGA_AXIS_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
