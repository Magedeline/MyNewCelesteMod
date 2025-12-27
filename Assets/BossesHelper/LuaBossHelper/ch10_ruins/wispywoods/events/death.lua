--[[
    Wispy Woods Boss - Death Event
]]

function onBegin()
    -- Disable player control briefly
    player.StateMachine.State = 11
    
    -- Death animation sequence
    helpers.playPuppetAnim("death_start")
    playSound("event:/wispywoods_death")
    wait(0.5)
    
    -- Shake and flash
    level.Shake(1.5)
    level.Flash(helpers.celeste.Color.White, false)
    
    helpers.playPuppetAnim("death_fall")
    wait(1.0)
    
    -- Dialog
    helpers.sayExt("WISPYWOODS_DEFEAT")
    
    -- Remove boss permanently
    helpers.removeBoss(true)
    
    -- Set completion flag
    setFlag("boss_wispywoods_defeated", true)
    
    -- Return player control
    player.StateMachine.State = 0
end

function onEnd(level, wasSkipped)
    if wasSkipped then
        helpers.removeBoss(true)
        setFlag("boss_wispywoods_defeated", true)
        player.StateMachine.State = 0
    end
end
