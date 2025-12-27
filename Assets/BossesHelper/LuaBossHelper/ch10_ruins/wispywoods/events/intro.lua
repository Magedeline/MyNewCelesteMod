--[[
    Wispy Woods Boss - Intro Cutscene Event
]]

function onBegin()
    -- Disable player control
    player.StateMachine.State = 11 -- StDummy state
    
    -- Camera focus on boss
    local bossPos = puppet.Position
    
    -- Boss awakening animation
    helpers.playPuppetAnim("sleep")
    wait(0.5)
    
    playSound("event:/wispywoods_awaken")
    helpers.playPuppetAnim("wake")
    wait(1.0)
    
    -- Dialog
    helpers.sayExt("WISPYWOODS_INTRO")
    
    -- Boss ready animation
    helpers.playPuppetAnim("ready")
    wait(0.5)
    helpers.playPuppetAnim("idle")
    
    -- Return player control
    player.StateMachine.State = 0 -- StNormal state
end

function onEnd(level, wasSkipped)
    if wasSkipped then
        helpers.playPuppetAnim("idle")
        player.StateMachine.State = 0
    end
end
