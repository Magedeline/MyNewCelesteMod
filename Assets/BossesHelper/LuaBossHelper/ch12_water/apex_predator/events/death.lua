function onBegin()
    player.StateMachine.State = 11
    helpers.playPuppetAnim("defeat")
    playSound("event:/apex_predator_defeat")
    wait(1.0)
    helpers.sayExt("APEX_PREDATOR_DEFEAT")
    helpers.removeBoss(true)
    setFlag("boss_apex_predator_defeated", true)
    player.StateMachine.State = 0
end
function onEnd(level, wasSkipped) if wasSkipped then helpers.removeBoss(true) setFlag("boss_apex_predator_defeated", true) player.StateMachine.State = 0 end end
