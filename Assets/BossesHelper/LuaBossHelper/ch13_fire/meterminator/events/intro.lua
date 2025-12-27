function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("cape_reveal") playSound("event:/meterminator_intro") wait(1.0) helpers.sayExt("METERMINATOR_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
