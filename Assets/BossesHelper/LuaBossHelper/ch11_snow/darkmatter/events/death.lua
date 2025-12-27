function onBegin()
    player.StateMachine.State = 11
    
    helpers.playPuppetAnim("defeat")
    playSound("event:/darkmatter_defeat")
    wait(1.0)
    
    level.Flash(helpers.celeste.Color.Black, false)
    helpers.playPuppetAnim("dissolve")
    wait(0.5)
    
    helpers.sayExt("DARKMATTER_DEFEAT")
    
    helpers.removeBoss(true)
    setFlag("boss_darkmatter_defeated", true)
    player.StateMachine.State = 0
end

function onEnd(level, wasSkipped)
    if wasSkipped then
        helpers.removeBoss(true)
        setFlag("boss_darkmatter_defeated", true)
        player.StateMachine.State = 0
    end
end
