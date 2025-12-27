--[[
    Wispy Woods Boss - Phase 2 Transition Event
]]

function onBegin()
    -- Play angry/hurt animation
    helpers.playPuppetAnim("angry")
    playSound("event:/wispywoods_angry")
    wait(0.8)
    
    -- Screen shake
    level.Shake(0.5)
    
    -- Dialog
    helpers.sayExt("WISPYWOODS_PHASE2")
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end

function onEnd(level, wasSkipped)
    if wasSkipped then
        helpers.playPuppetAnim("idle")
    end
end
