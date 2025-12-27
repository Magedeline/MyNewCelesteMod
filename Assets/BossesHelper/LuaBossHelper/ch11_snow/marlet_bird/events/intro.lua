--[[
    Marlet Possessed Bird - Intro Event
]]

function onBegin()
    player.StateMachine.State = 11
    
    helpers.playPuppetAnim("possessed_idle")
    wait(0.5)
    
    playSound("event:/marlet_screech")
    helpers.playPuppetAnim("screech")
    wait(1.0)
    
    helpers.sayExt("MARLET_BIRD_INTRO")
    
    helpers.playPuppetAnim("idle")
    player.StateMachine.State = 0
end

function onEnd(level, wasSkipped)
    if wasSkipped then
        helpers.playPuppetAnim("idle")
        player.StateMachine.State = 0
    end
end
