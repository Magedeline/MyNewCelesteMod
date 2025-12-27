function onBegin() player.StateMachine.State = 11 helpers.playPuppetAnim("ascend") playSound("event:/asriel_god_intro") level.Shake(1.0) wait(2.0) helpers.sayExt("ASRIEL_GOD_INTRO") helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
