function onBegin()
    player.StateMachine.State = 11
    helpers.playPuppetAnim("emerge")
    playSound("event:/apex_predator_roar")
    wait(1.0)
    helpers.sayExt("APEX_PREDATOR_INTRO")
    helpers.playPuppetAnim("idle")
    player.StateMachine.State = 0
end
function onEnd(level, wasSkipped) if wasSkipped then helpers.playPuppetAnim("idle") player.StateMachine.State = 0 end end
