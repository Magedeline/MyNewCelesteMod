--[[
    Spamton Boss - Phone Hands Attack
]]

function onBegin()
    helpers.playPuppetAnim("phone_summon")
    playSound("event:/spamton_phone")
    wait(0.4)
    
    helpers.playPuppetAnim("phone_attack")
    
    local bossPos = puppet.Position
    
    -- Giant phone hands slam from sides
    for wave = 1, 3 do
        -- Left hand
        local leftHand = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X - 250, bossPos.Y + wave * 50),
            helpers.getHitbox(100, 60, -50, -30),
            "characters/spamton/phone_hand",
            true
        )
        leftHand.Speed = vector2(400, 0)
        leftHand:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(leftHand)
        
        -- Right hand
        local rightHand = helpers.getNewBasicAttackEntity(
            vector2(bossPos.X + 250, bossPos.Y + wave * 50),
            helpers.getHitbox(100, 60, -50, -30),
            "characters/spamton/phone_hand",
            true
        )
        rightHand.Speed = vector2(-400, 0)
        rightHand:Add(helpers.getEntityTimer(1.5))
        helpers.addEntity(rightHand)
        
        playSound("event:/spamton_slam")
        wait(0.4)
    end
    
    wait(0.3)
    helpers.playPuppetAnim("idle")
end
