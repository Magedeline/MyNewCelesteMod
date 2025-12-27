--[[
    Marlet Possessed Bird - Phase 3 Transition
]]

function onBegin()
    helpers.playPuppetAnim("berserk")
    playSound("event:/marlet_berserk")
    level.Shake(1.0)
    wait(1.0)
    
    helpers.sayExt("MARLET_BIRD_PHASE3")
    helpers.playPuppetAnim("idle")
end

function onEnd(level, wasSkipped)
    if wasSkipped then helpers.playPuppetAnim("idle") end
end
