--[[
    Marlet Possessed Bird - Phase 2 Transition
]]

function onBegin()
    helpers.playPuppetAnim("enrage")
    playSound("event:/marlet_enrage")
    level.Shake(0.5)
    wait(0.8)
    
    helpers.sayExt("MARLET_BIRD_PHASE2")
    helpers.playPuppetAnim("idle")
end

function onEnd(level, wasSkipped)
    if wasSkipped then helpers.playPuppetAnim("idle") end
end
