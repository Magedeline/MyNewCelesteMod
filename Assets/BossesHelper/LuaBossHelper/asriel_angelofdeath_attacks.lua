-- Asriel Angel of Death Boss - Attack Pattern Lua Scripts
-- These scripts work with BossesHelper to define attack behaviors

local attacks = {}

-- Attack 1: Ultima Bullet
attacks.ultimaBullet = {
    name = "UltimaBullet",
    cooldown = 2.0,
    phases = {1, 2},
    
    execute = function(boss, player)
        -- Charge animation
        boss:PlayAnimation("attack_ultimabullet_start")
        boss:EnableCosmowing("stems", true)
        boss:Wait(0.5)
        
        boss:PlayAnimation("attack_ultimabullet_hold")
        
        -- Fire 8 homing bullets in a circle
        for i = 0, 7 do
            local angle = (i * math.pi / 4)
            local direction = {
                x = math.cos(angle),
                y = math.sin(angle)
            }
            
            boss:SpawnProjectile("UltimaBullet", {
                position = boss.position,
                direction = direction,
                speed = 180,
                homingStrength = 120,
                target = player,
                lifetime = 5.0
            })
            
            boss:Wait(0.1)
        end
        
        boss:PlayAnimation("attack_ultimabullet_fire")
        boss:Wait(0.3)
        boss:EnableCosmowing("stems", false)
    end
}

-- Attack 2: Cross Shocker
attacks.crossShocker = {
    name = "CrossShocker",
    cooldown = 2.5,
    phases = {1, 2},
    
    execute = function(boss, player)
        boss:PlayAnimation("attack_crossshocker_start")
        boss:EnableCosmowing("shoulder", true)
        boss:PlayAnimation("orbwings", "spread")
        boss:Wait(0.4)
        
        boss:PlayAnimation("attack_crossshocker_loop")
        
        -- Fire 3 waves of cross-shaped lightning
        for wave = 1, 3 do
            -- Vertical beams
            boss:SpawnProjectile("CrossShockerWave", {
                position = boss.position,
                direction = {x = 0, y = 1},
                speed = 200,
                lifetime = 3.0
            })
            
            boss:SpawnProjectile("CrossShockerWave", {
                position = boss.position,
                direction = {x = 0, y = -1},
                speed = 200,
                lifetime = 3.0
            })
            
            -- Horizontal beams
            boss:SpawnProjectile("CrossShockerWave", {
                position = boss.position,
                direction = {x = 1, y = 0},
                speed = 200,
                lifetime = 3.0
            })
            
            boss:SpawnProjectile("CrossShockerWave", {
                position = boss.position,
                direction = {x = -1, y = 0},
                speed = 200,
                lifetime = 3.0
            })
            
            boss:Wait(0.4)
        end
        
        boss:PlayAnimation("attack_crossshocker_end")
        boss:Wait(0.3)
        boss:EnableCosmowing("shoulder", false)
        boss:PlayAnimation("orbwings", "idle")
    end
}

-- Attack 3: Star Storm Ultra
attacks.starStormUltra = {
    name = "StarStormUltra",
    cooldown = 3.0,
    phases = {1, 2},
    
    execute = function(boss, player)
        boss:PlayAnimation("attack_starstormultra_start")
        boss:EnableCosmowing("bg", "intense")
        boss:Wait(0.6)
        
        boss:PlayAnimation("attack_starstormultra_rain")
        
        -- Rain 30 stars from the top of the screen
        local cameraLeft = boss:GetCameraBounds().left
        local cameraRight = boss:GetCameraBounds().right
        local cameraTop = boss:GetCameraBounds().top
        
        for i = 1, 30 do
            local randomX = math.random(cameraLeft, cameraRight)
            
            boss:SpawnProjectile("StarStorm", {
                position = {x = randomX, y = cameraTop - 20},
                fallSpeed = 200,
                acceleration = 50
            })
            
            boss:Wait(0.15)
        end
        
        boss:PlayAnimation("attack_starstormultra_end")
        boss:Wait(0.4)
        boss:EnableCosmowing("bg", "normal")
    end
}

-- Attack 4: Shocker Breaker III (Phase 2 only)
attacks.shockerBreaker3 = {
    name = "ShockerBreakerIII",
    cooldown = 4.0,
    phases = {2},
    
    execute = function(boss, player)
        boss:PlayAnimation("attack_shockerbreaker3_start")
        
        -- Scale up head and halo for dramatic effect
        boss:SetSpriteScale("phase2_head", 1.2)
        boss:SetSpriteScale("phase2_halo", 1.5)
        boss:Wait(0.7)
        
        boss:PlayAnimation("attack_shockerbreaker3_hold")
        boss:Wait(0.3)
        
        boss:PlayAnimation("attack_shockerbreaker3_release")
        
        -- Create 5 expanding shockwave rings
        for ring = 0, 4 do
            boss:SpawnProjectile("ShockerBreaker3Wave", {
                position = boss.position,
                radius = ring * 60 + 80,
                expandSpeed = 200,
                thickness = 20,
                delay = ring * 0.2
            })
            
            boss:Wait(0.3)
        end
        
        boss:PlayAnimation("attack_shockerbreaker3_waves")
        boss:Wait(1.0)
        
        boss:PlayAnimation("attack_shockerbreaker3_end")
        boss:SetSpriteScale("phase2_head", 1.0)
        boss:SetSpriteScale("phase2_halo", 1.0)
        boss:Wait(0.4)
    end
}

-- Attack 5: Final Beam (Phase 2 only - Ultimate attack)
attacks.finalBeam = {
    name = "FinalBeam",
    cooldown = 6.0,
    phases = {2},
    
    execute = function(boss, player)
        boss:PlayAnimation("attack_finalbeam_charge")
        
        -- Activate all cosmowing effects
        boss:EnableCosmowing("bg", "intense")
        boss:EnableCosmowing("stems", true)
        boss:EnableCosmowing("shoulder", true)
        boss:EnableCosmowing("orbwings", true)
        
        -- Charge animation - wings spread out
        for i = 0, 30 do
            local offset = i * 0.5
            boss:SetSpritePosition("phase2_wings_left", {x = -40 - offset, y = 5})
            boss:SetSpritePosition("phase2_wings_right", {x = 40 + offset, y = 5})
            boss:Wait(0.05)
        end
        
        boss:PlayAnimation("attack_finalbeam_focus")
        boss:Wait(0.6)
        
        boss:PlayAnimation("attack_finalbeam_fire")
        
        -- Calculate beam direction toward player
        local directionToPlayer = boss:GetDirectionToPlayer(player)
        
        -- Spawn the massive beam
        boss:SpawnProjectile("FinalBeam", {
            position = boss.position,
            direction = directionToPlayer,
            width = 40,
            length = 1000,
            duration = 3.0,
            damage = "instant_death"
        })
        
        boss:PlayAnimation("attack_finalbeam_hold")
        boss:Wait(3.0)
        
        boss:PlayAnimation("attack_finalbeam_end")
        
        -- Reset all effects
        boss:EnableCosmowing("stems", false)
        boss:EnableCosmowing("shoulder", false)
        boss:EnableCosmowing("orbwings", false)
        boss:EnableCosmowing("bg", "normal")
        boss:SetSpritePosition("phase2_wings_left", {x = -40, y = 5})
        boss:SetSpritePosition("phase2_wings_right", {x = 40, y = 5})
        
        boss:Wait(0.5)
    end
}

-- Boss AI routine
attacks.aiRoutine = function(boss)
    -- Phase 1 attacks
    local phase1Attacks = {
        attacks.ultimaBullet,
        attacks.crossShocker,
        attacks.starStormUltra
    }
    
    -- Phase 2 attacks (includes all Phase 1 attacks plus new ones)
    local phase2Attacks = {
        attacks.ultimaBullet,
        attacks.crossShocker,
        attacks.starStormUltra,
        attacks.shockerBreaker3,
        attacks.finalBeam
    }
    
    while boss:IsAlive() do
        local currentPhase = boss:GetPhase()
        local availableAttacks = currentPhase == 1 and phase1Attacks or phase2Attacks
        
        -- Select random attack
        local selectedAttack = availableAttacks[math.random(1, #availableAttacks)]
        
        -- Execute attack if cooldown is ready
        if boss:IsAttackReady(selectedAttack.name) then
            local player = boss:GetTargetPlayer()
            if player then
                selectedAttack.execute(boss, player)
                boss:SetAttackCooldown(selectedAttack.name, selectedAttack.cooldown)
            end
        end
        
        -- Wait before next attack
        local waitTime = currentPhase == 1 and 0.5 or 0.3
        boss:Wait(waitTime)
    end
end

-- Phase transition handler
attacks.onPhaseTransition = function(boss, fromPhase, toPhase)
    if fromPhase == 1 and toPhase == 2 then
        boss:PlayAnimation("phase2_transform_start")
        boss:EnableCosmowing("bg", "intense")
        boss:Wait(boss:GetAnimationDuration("phase2_transform_start"))
        
        boss:PlayAnimation("phase2_transform_mid")
        boss:Wait(boss:GetAnimationDuration("phase2_transform_mid"))
        
        -- Initialize phase 2 sprite nodes
        boss:InitializePhase2Nodes()
        
        boss:PlayAnimation("phase2_transform_end")
        boss:Wait(boss:GetAnimationDuration("phase2_transform_end"))
        
        boss:PlayAnimation("phase2_idle")
        boss:SetHealth(150) -- Phase 2 health
    end
end

-- Death sequence
attacks.onDeath = function(boss)
    boss:PlayAnimation("defeat")
    boss:Wait(2.0)
    boss:Remove()
end

return attacks
