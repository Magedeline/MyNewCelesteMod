--[[
    Marlet Possessed Bird - Death Event
]]

function onBegin()
    player.StateMachine.State = 11
    
    helpers.playPuppetAnim("defeat")
    playSound("event:/marlet_defeat")
    wait(1.0)
    
    level.Flash(helpers.celeste.Color.White, false)
    helpers.playPuppetAnim("purified")
    wait(0.5)
    
    helpers.sayExt("MARLET_BIRD_DEFEAT")
    
    helpers.removeBoss(true)
    setFlag("boss_marlet_bird_defeated", true)
    player.StateMachine.State = 0
end

function onEnd(level, wasSkipped)
    if wasSkipped then
        helpers.removeBoss(true)
        setFlag("boss_marlet_bird_defeated", true)
        player.StateMachine.State = 0
    end
end
