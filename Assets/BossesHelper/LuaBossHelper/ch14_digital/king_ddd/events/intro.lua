function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("entrance") playSound("event:/king_ddd_entrance") wait(1.2) helpers.sayExt("KING_DDD_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
