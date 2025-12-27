function onBegin()
    player.StateMachine.State = 11
    helpers.playPuppetAnim("defeat")
    playSound("event:/tower_titan_defeat")
    level.Shake(1.5)
    wait(1.5)
    helpers.sayExt("TOWER_TITAN_DEFEAT")
    helpers.removeBoss(true)
    setFlag("boss_tower_titan_defeated", true)
    player.StateMachine.State = 0
end
function onEnd(level, wasSkipped)
    if wasSkipped then
        helpers.removeBoss(true)
        setFlag("boss_tower_titan_defeated", true)
        player.StateMachine.State = 0
    end
end
