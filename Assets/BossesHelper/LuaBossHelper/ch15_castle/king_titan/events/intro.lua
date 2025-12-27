function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("throne_appear") playSound("event:/king_titan_entrance") wait(1.5) helpers.sayExt("KING_TITAN_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
