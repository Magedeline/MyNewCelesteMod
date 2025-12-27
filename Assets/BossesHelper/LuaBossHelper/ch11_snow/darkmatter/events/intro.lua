--[[
    Dark the Dark Matter - Intro Event
]]

function onBegin()
    player.StateMachine.State = 11
    
    helpers.playPuppetAnim("materialize")
    playSound("event:/darkmatter_appear")
    wait(1.0)
    
    helpers.playPuppetAnim("draw_katana")
    playSound("event:/darkmatter_katana_draw")
    wait(0.5)
    
    helpers.sayExt("DARKMATTER_INTRO")
    
    helpers.playPuppetAnim("idle")
    player.StateMachine.State = 0
end

function onEnd(level, wasSkipped)
    if wasSkipped then
        helpers.playPuppetAnim("idle")
        player.StateMachine.State = 0
    end
end
