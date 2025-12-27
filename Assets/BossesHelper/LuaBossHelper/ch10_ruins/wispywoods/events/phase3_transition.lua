--[[
    Wispy Woods Boss - Phase 3 Transition Event
]]

function onBegin()
    -- Play desperate animation
    helpers.playPuppetAnim("desperate")
    playSound("event:/wispywoods_desperate")
    wait(1.0)
    
    -- Heavy screen shake
    level.Shake(1.0)
    
    -- Dialog
    helpers.sayExt("WISPYWOODS_PHASE3")
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end

function onEnd(level, wasSkipped)
    if wasSkipped then
        helpers.playPuppetAnim("idle")
    end
end
