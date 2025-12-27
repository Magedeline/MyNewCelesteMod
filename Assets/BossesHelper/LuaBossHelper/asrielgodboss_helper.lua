--[[
    Asriel God Boss Helper Functions
    Provides Lua helper functions for the Asriel God of Hyperdeath boss fight
    
    Sprite path: characters/asrielgodboss/
    Available animations: idle, boss, shrug, appear, disappear, fadein, fadeout,
                          castsp, mch, at_mch, at_sw, sum_mch, sum_sw,
                          beamStart, beam, lightning_aim, lightning_strike,
                          bigger_lightning_aim, bigger_lightning_strike,
                          star, starb, starc, star_impact, shockwave,
                          airwave, airwavedisappear, groundwave, groundwavedisappear,
                          hg_debris, hg_debrisB, hg_debrisC, blackhole, trails
]]

local asrielGodBoss = {}

-- Import helper functions
local helpers = require("helper_functions")
local vector2 = helpers.vector2

--#region Sprite Constants

asrielGodBoss.SPRITE_PATH = "characters/asrielgodboss/"

-- Animation IDs
asrielGodBoss.Animations = {
    -- Base states
    IDLE = "idle",
    BOSS = "boss",
    SHRUG = "shrug",
    
    -- Appearance
    APPEAR = "appear",
    DISAPPEAR = "disappear",
    FADEIN = "fadein",
    FADEOUT = "fadeout",
    
    -- Attack animations
    CASTSP = "castsp",
    MCH = "mch",
    AT_MCH = "at_mch",
    AT_SW = "at_sw",
    SUM_MCH = "sum_mch",
    SUM_SW = "sum_sw",
    
    -- Beam attack
    BEAM_START = "beamStart",
    BEAM = "beam",
    
    -- Lightning attacks
    LIGHTNING_AIM = "lightning_aim",
    LIGHTNING_STRIKE = "lightning_strike",
    BIGGER_LIGHTNING_AIM = "bigger_lightning_aim",
    BIGGER_LIGHTNING_STRIKE = "bigger_lightning_strike",
    
    -- Projectiles
    STAR = "star",
    STARB = "starb",
    STARC = "starc",
    STAR_IMPACT = "star_impact",
    
    -- Effects
    SHOCKWAVE = "shockwave",
    AIRWAVE = "airwave",
    AIRWAVE_DISAPPEAR = "airwavedisappear",
    GROUNDWAVE = "groundwave",
    GROUNDWAVE_DISAPPEAR = "groundwavedisappear",
    
    -- Hyper Goner
    HG_DEBRIS = "hg_debris",
    HG_DEBRIS_B = "hg_debrisB",
    HG_DEBRIS_C = "hg_debrisC",
    
    -- Special
    BLACKHOLE = "blackhole",
    TRAILS = "trails",
    
    -- Sword (for tween attacks)
    SWORD = "sword"
}

-- Separate sword sprite animations (for tweening entity)
asrielGodBoss.SwordAnimations = {
    IDLE = "idle",
    SPIN = "spin",
    SLASH = "slash"
}

--#endregion

--#region Attack Types

asrielGodBoss.AttackTypes = {
    SHOOT = "Shoot",
    BEAM = "Beam",
    BIGGER_BEAM = "BiggerBeam",
    BIG_BEAM_BALL = "BigBeamBall",
    RAINBOW_BLACKHOLE = "RainbowBlackhole",
    BLADE_THROWER = "BladeThrower",
    FIRE_SHOCKWAVE = "FireShockwave",
    STARS_METEORITE = "StarsMeteorite",
    CHAOS_BLASTER = "ChaosBlaster",
    HYPER_GONER = "HyperGoner",
    GALACTIC_SABER = "GalacticSaber",
    STARSTORM_RAIN = "StarstormRain",
    LIGHTNING_STORM = "LightningStorm",
    DIMENSIONAL_RIFT = "DimensionalRift",
    RAINBOW_INFERNO = "RainbowInferno",
    CELESTIAL_SPEARS = "CelestialSpears",
    TIMEWARP_VORTEX = "TimewarpVortex",
    PRISM_BURST = "PrismBurst",
    SOUL_RESONANCE = "SoulResonance",
    ETERNAL_CHAOS = "EternalChaos"
}

--#endregion

--#region Attack Library
--[[
    ============================================================================
    ASRIEL GOD BOSS ATTACK LIBRARY
    ============================================================================
    Complete reference for all Asriel God of Hyperdeath boss attacks.
    Use this library to configure attack sequences and patterns.
    
    USAGE:
    local attack = asrielGodBoss.AttackLibrary.SHOOT
    -- attack.name, attack.damage, attack.animation, etc.
]]

asrielGodBoss.AttackLibrary = {
    --==========================================================================
    -- BASIC ATTACKS (Phase 1)
    --==========================================================================
    
    SHOOT = {
        name = "Shoot",
        id = "Shoot",
        description = "Fires a single star projectile towards the player",
        animation = "castsp",
        projectileAnim = "star",
        damage = 10,
        cooldown = 0.8,
        phase = 1,
        difficulty = "Easy",
        params = {
            speed = 200,
            tracking = false,
            count = 1
        }
    },
    
    BEAM = {
        name = "Beam",
        id = "Beam",
        description = "Fires a concentrated energy beam that sweeps across the arena",
        animation = "beamStart",
        loopAnimation = "beam",
        damage = 15,
        cooldown = 3.0,
        phase = 1,
        difficulty = "Medium",
        params = {
            duration = 2.5,
            sweepSpeed = 45,  -- degrees per second
            width = 24
        }
    },
    
    BIGGER_BEAM = {
        name = "Bigger Beam",
        id = "BiggerBeam",
        description = "An enhanced beam attack with wider coverage and longer duration",
        animation = "beamStart",
        loopAnimation = "beam",
        damage = 20,
        cooldown = 4.0,
        phase = 2,
        difficulty = "Hard",
        params = {
            duration = 4.0,
            sweepSpeed = 60,
            width = 48,
            multiBeam = false
        }
    },
    
    BIG_BEAM_BALL = {
        name = "Big Beam Ball",
        id = "BigBeamBall",
        description = "Charges and releases a massive energy sphere that explodes on impact",
        animation = "castsp",
        projectileAnim = "starc",
        damage = 25,
        cooldown = 5.0,
        phase = 2,
        difficulty = "Hard",
        params = {
            chargeTime = 1.5,
            speed = 150,
            explosionRadius = 80,
            explosionDamage = 15
        }
    },
    
    --==========================================================================
    -- PROJECTILE ATTACKS
    --==========================================================================
    
    BLADE_THROWER = {
        name = "Blade Thrower",
        id = "BladeThrower",
        description = "Hurls multiple sword-shaped projectiles in a fan pattern",
        animation = "sum_sw",
        projectileAnim = "sword",
        damage = 12,
        cooldown = 2.0,
        phase = 1,
        difficulty = "Medium",
        params = {
            bladeCount = 5,
            spreadAngle = 60,
            speed = 180,
            spinSpeed = 360
        }
    },
    
    STARS_METEORITE = {
        name = "Stars Meteorite",
        id = "StarsMeteorite",
        description = "Summons a shower of star projectiles from above",
        animation = "castsp",
        projectileAnim = "star",
        impactAnim = "star_impact",
        damage = 8,
        cooldown = 3.5,
        phase = 1,
        difficulty = "Medium",
        params = {
            starCount = 12,
            fallSpeed = 300,
            spawnDelay = 0.15,
            spreadWidth = 200
        }
    },
    
    STARSTORM_RAIN = {
        name = "Starstorm Rain",
        id = "StarstormRain",
        description = "Creates an intense barrage of stars raining from multiple directions",
        animation = "castsp",
        projectileAnim = "starb",
        impactAnim = "star_impact",
        damage = 10,
        cooldown = 4.0,
        phase = 2,
        difficulty = "Hard",
        params = {
            starCount = 24,
            fallSpeed = 350,
            spawnDelay = 0.08,
            spreadWidth = 320,
            randomOffset = 40
        }
    },
    
    CELESTIAL_SPEARS = {
        name = "Celestial Spears",
        id = "CelestialSpears",
        description = "Materializes ethereal spears that pierce through the arena",
        animation = "at_mch",
        damage = 18,
        cooldown = 3.0,
        phase = 2,
        difficulty = "Hard",
        params = {
            spearCount = 8,
            telegraphTime = 0.6,
            speed = 400,
            pattern = "radial"  -- radial, horizontal, vertical, diagonal
        }
    },
    
    PRISM_BURST = {
        name = "Prism Burst",
        id = "PrismBurst",
        description = "Releases a burst of prismatic projectiles that split on travel",
        animation = "castsp",
        projectileAnim = "starc",
        damage = 12,
        cooldown = 3.5,
        phase = 2,
        difficulty = "Hard",
        params = {
            initialCount = 4,
            splitCount = 3,
            splitDistance = 100,
            speed = 160
        }
    },
    
    --==========================================================================
    -- AREA ATTACKS
    --==========================================================================
    
    FIRE_SHOCKWAVE = {
        name = "Fire Shockwave",
        id = "FireShockwave",
        description = "Slams the ground creating an expanding wave of fire",
        animation = "at_sw",
        effectAnim = "groundwave",
        disappearAnim = "groundwavedisappear",
        damage = 20,
        cooldown = 4.0,
        phase = 1,
        difficulty = "Medium",
        params = {
            waveSpeed = 250,
            waveHeight = 32,
            waveCount = 1,
            groundOnly = true
        }
    },
    
    RAINBOW_BLACKHOLE = {
        name = "Rainbow Blackhole",
        id = "RainbowBlackhole",
        description = "Creates a gravitational vortex that pulls the player towards it",
        animation = "castsp",
        effectAnim = "blackhole",
        damage = 30,
        cooldown = 8.0,
        phase = 2,
        difficulty = "Very Hard",
        params = {
            duration = 4.0,
            pullStrength = 120,
            damageRadius = 60,
            pullRadius = 200
        }
    },
    
    DIMENSIONAL_RIFT = {
        name = "Dimensional Rift",
        id = "DimensionalRift",
        description = "Tears open rifts in space that spawn additional hazards",
        animation = "castsp",
        damage = 15,
        cooldown = 6.0,
        phase = 3,
        difficulty = "Very Hard",
        params = {
            riftCount = 3,
            riftDuration = 5.0,
            spawnInterval = 0.8,
            hazardTypes = {"star", "blade"}
        }
    },
    
    TIMEWARP_VORTEX = {
        name = "Timewarp Vortex",
        id = "TimewarpVortex",
        description = "Distorts time around the player, creating afterimage hazards",
        animation = "castsp",
        effectAnim = "trails",
        damage = 12,
        cooldown = 7.0,
        phase = 3,
        difficulty = "Very Hard",
        params = {
            duration = 4.0,
            afterimageCount = 5,
            delayBetween = 0.3,
            slowdownFactor = 0.5
        }
    },
    
    --==========================================================================
    -- LIGHTNING ATTACKS
    --==========================================================================
    
    LIGHTNING_STORM = {
        name = "Lightning Storm",
        id = "LightningStorm",
        description = "Calls down multiple lightning strikes across the arena",
        animation = "lightning_aim",
        strikeAnim = "lightning_strike",
        damage = 25,
        cooldown = 4.5,
        phase = 2,
        difficulty = "Hard",
        params = {
            strikeCount = 6,
            telegraphTime = 0.8,
            delayBetween = 0.3,
            strikeWidth = 32,
            trackPlayer = true
        }
    },
    
    BIGGER_LIGHTNING = {
        name = "Bigger Lightning",
        id = "BiggerLightning",
        description = "An enhanced lightning attack with wider and more devastating strikes",
        animation = "bigger_lightning_aim",
        strikeAnim = "bigger_lightning_strike",
        damage = 35,
        cooldown = 6.0,
        phase = 3,
        difficulty = "Very Hard",
        params = {
            strikeCount = 4,
            telegraphTime = 1.0,
            delayBetween = 0.5,
            strikeWidth = 64,
            trackPlayer = true
        }
    },
    
    --==========================================================================
    -- SPECIAL ATTACKS (Phase 3+)
    --==========================================================================
    
    CHAOS_BLASTER = {
        name = "Chaos Blaster",
        id = "ChaosBlaster",
        description = "Rapid-fire chaos energy projectiles with randomized patterns",
        animation = "mch",
        projectileAnim = "starc",
        damage = 8,
        cooldown = 0.5,
        phase = 2,
        difficulty = "Hard",
        params = {
            burstCount = 12,
            burstDelay = 0.1,
            speed = 220,
            randomAngle = 30,
            autoAim = true
        }
    },
    
    GALACTIC_SABER = {
        name = "Galactic Saber",
        id = "GalacticSaber",
        description = "Summons a giant ethereal sword for devastating slash attacks",
        animation = "sum_sw",
        swordAnim = "sword",
        damage = 30,
        cooldown = 5.0,
        phase = 2,
        difficulty = "Hard",
        params = {
            slashCount = 3,
            slashDelay = 0.8,
            slashSpeed = 500,
            slashWidth = 200,
            pattern = "cross"  -- cross, vertical, horizontal, spiral
        }
    },
    
    RAINBOW_INFERNO = {
        name = "Rainbow Inferno",
        id = "RainbowInferno",
        description = "Engulfs the arena in rainbow flames from all directions",
        animation = "castsp",
        effectAnim = "airwave",
        disappearAnim = "airwavedisappear",
        damage = 15,
        cooldown = 6.0,
        phase = 3,
        difficulty = "Very Hard",
        params = {
            waveCount = 4,
            waveDelay = 0.5,
            coveragePercent = 75,
            duration = 3.0,
            safeZones = 2
        }
    },
    
    SOUL_RESONANCE = {
        name = "Soul Resonance",
        id = "SoulResonance",
        description = "Resonates with the player's soul, creating synchronized attacks",
        animation = "castsp",
        damage = 20,
        cooldown = 7.0,
        phase = 3,
        difficulty = "Very Hard",
        params = {
            mirrorDelay = 0.5,
            echoCount = 3,
            echoStrength = 0.8,
            trackSoul = true
        }
    },
    
    --==========================================================================
    -- ULTIMATE ATTACKS
    --==========================================================================
    
    HYPER_GONER = {
        name = "Hyper Goner",
        id = "HyperGoner",
        description = "Ultimate attack - Summons the Hyper Goner skull for massive devastation",
        animation = "castsp",
        effectAnims = {"hg_debris", "hg_debrisB", "hg_debrisC", "blackhole"},
        damage = 50,
        cooldown = 15.0,
        phase = 3,
        difficulty = "Extreme",
        isUltimate = true,
        params = {
            chargeTime = 2.0,
            duration = 6.0,
            pullStrength = 200,
            debrisCount = 20,
            debrisSpeed = 300,
            coreRadius = 100
        }
    },
    
    ETERNAL_CHAOS = {
        name = "Eternal Chaos",
        id = "EternalChaos",
        description = "Final form attack - Unleashes the full power of absolute chaos",
        animation = "boss",
        damage = 100,
        cooldown = 20.0,
        phase = 4,
        difficulty = "Extreme",
        isUltimate = true,
        params = {
            phaseCount = 5,
            phaseDuration = 2.0,
            attacksPerPhase = 3,
            intensity = 1.0,
            noSafeZone = true
        }
    }
}

--#endregion

--#region Attack Sequence Builder
--[[
    Helper functions for building attack sequences and patterns.
]]

--- Get all attacks for a specific phase
---@param phase number The phase number (1-4)
---@return table List of attacks for that phase
function asrielGodBoss.getAttacksForPhase(phase)
    local attacks = {}
    for name, attack in pairs(asrielGodBoss.AttackLibrary) do
        if attack.phase <= phase then
            table.insert(attacks, attack)
        end
    end
    return attacks
end

--- Get attacks by difficulty
---@param difficulty string Difficulty level ("Easy", "Medium", "Hard", "Very Hard", "Extreme")
---@return table List of attacks with that difficulty
function asrielGodBoss.getAttacksByDifficulty(difficulty)
    local attacks = {}
    for name, attack in pairs(asrielGodBoss.AttackLibrary) do
        if attack.difficulty == difficulty then
            table.insert(attacks, attack)
        end
    end
    return attacks
end

--- Get attack by ID
---@param attackId string The attack ID
---@return table|nil The attack data or nil if not found
function asrielGodBoss.getAttackById(attackId)
    for name, attack in pairs(asrielGodBoss.AttackLibrary) do
        if attack.id == attackId then
            return attack
        end
    end
    return nil
end

--- Create an attack sequence
---@param attackIds table List of attack IDs to sequence
---@return table Attack sequence data
function asrielGodBoss.createAttackSequence(attackIds)
    local sequence = {
        attacks = {},
        currentIndex = 1,
        looping = false
    }
    
    for _, id in ipairs(attackIds) do
        local attack = asrielGodBoss.getAttackById(id)
        if attack then
            table.insert(sequence.attacks, attack)
        end
    end
    
    return sequence
end

--- Get next attack in sequence
---@param sequence table The attack sequence
---@return table|nil The next attack or nil if sequence ended
function asrielGodBoss.getNextAttack(sequence)
    if sequence.currentIndex > #sequence.attacks then
        if sequence.looping then
            sequence.currentIndex = 1
        else
            return nil
        end
    end
    
    local attack = sequence.attacks[sequence.currentIndex]
    sequence.currentIndex = sequence.currentIndex + 1
    return attack
end

--#endregion

--#region Helper Functions

--- Get the full texture path for an asrielgodboss animation frame
---@param animName string The animation name
---@param frameNum number The frame number (default 0)
---@return string The full texture path
function asrielGodBoss.getTexturePath(animName, frameNum)
    frameNum = frameNum or 0
    return string.format("%s%s%02d", asrielGodBoss.SPRITE_PATH, animName, frameNum)
end

--- Play an animation on the boss sprite
---@param boss userdata The boss entity
---@param animId string The animation ID to play
function asrielGodBoss.playAnimation(boss, animId)
    if boss and boss.Sprite then
        boss.Sprite:Play(animId)
    end
end

--- Create a star projectile at the given position
---@param scene userdata The current scene
---@param position table|Vector2 The spawn position {x, y} or Vector2
---@param velocity table|Vector2 The projectile velocity {x, y} or Vector2
---@param starType string The star type: "star", "starb", or "starc" (default "star")
function asrielGodBoss.createStarProjectile(scene, position, velocity, starType)
    starType = starType or "star"
    local pos = vector2(position)
    local vel = vector2(velocity)
    
    -- Implementation depends on the StarProjectile entity in the C# codebase
    -- This is a placeholder for the Lua integration
    helpers.log("Creating star projectile at " .. tostring(pos.X) .. ", " .. tostring(pos.Y))
end

--- Trigger a beam attack sequence
---@param boss userdata The boss entity
---@param targetPosition table|Vector2 The target position to aim at
---@param duration number The duration of the beam in seconds (default 3.0)
function asrielGodBoss.triggerBeamAttack(boss, targetPosition, duration)
    duration = duration or 3.0
    local targetPos = vector2(targetPosition)
    
    asrielGodBoss.playAnimation(boss, asrielGodBoss.Animations.BEAM_START)
    -- The beam animation will auto-transition to BEAM loop
end

--- Trigger a lightning attack
---@param boss userdata The boss entity  
---@param targetPosition table|Vector2 The target position
---@param isBigger boolean Whether to use the bigger lightning variant
function asrielGodBoss.triggerLightningAttack(boss, targetPosition, isBigger)
    local aimAnim = isBigger and asrielGodBoss.Animations.BIGGER_LIGHTNING_AIM or asrielGodBoss.Animations.LIGHTNING_AIM
    asrielGodBoss.playAnimation(boss, aimAnim)
end

--- Make the boss appear with animation
---@param boss userdata The boss entity
---@param useFade boolean Whether to use fade-in instead of appear animation
function asrielGodBoss.appear(boss, useFade)
    local anim = useFade and asrielGodBoss.Animations.FADEIN or asrielGodBoss.Animations.APPEAR
    asrielGodBoss.playAnimation(boss, anim)
end

--- Make the boss disappear with animation
---@param boss userdata The boss entity
---@param useFade boolean Whether to use fade-out instead of disappear animation
function asrielGodBoss.disappear(boss, useFade)
    local anim = useFade and asrielGodBoss.Animations.FADEOUT or asrielGodBoss.Animations.DISAPPEAR
    asrielGodBoss.playAnimation(boss, anim)
end

--- Create a shockwave effect
---@param scene userdata The current scene
---@param position table|Vector2 The origin position
---@param isGround boolean Whether this is a ground wave or air wave
function asrielGodBoss.createShockwave(scene, position, isGround)
    local pos = vector2(position)
    local waveType = isGround and "groundwave" or "airwave"
    helpers.log("Creating " .. waveType .. " at " .. tostring(pos.X) .. ", " .. tostring(pos.Y))
end

--- Get the current boss state
---@param boss userdata The boss entity
---@return string The current state name
function asrielGodBoss.getCurrentState(boss)
    if boss and boss.currentState then
        local states = {"Idle", "Attacking", "Hurt", "Stunned", "Transitioning", "Defeated"}
        return states[boss.currentState + 1] or "Unknown"
    end
    return "Unknown"
end

--#endregion

--#region Sword Tween Functions

--- Tween easing functions
asrielGodBoss.Easing = {
    -- Linear interpolation
    linear = function(t) return t end,
    
    -- Ease out cubic (smooth deceleration)
    easeOutCubic = function(t) return 1 - math.pow(1 - t, 3) end,
    
    -- Ease in cubic (smooth acceleration)
    easeInCubic = function(t) return t * t * t end,
    
    -- Ease in out cubic
    easeInOutCubic = function(t)
        if t < 0.5 then
            return 4 * t * t * t
        else
            return 1 - math.pow(-2 * t + 2, 3) / 2
        end
    end,
    
    -- Ease out back (overshoot)
    easeOutBack = function(t)
        local c1 = 1.70158
        local c3 = c1 + 1
        return 1 + c3 * math.pow(t - 1, 3) + c1 * math.pow(t - 1, 2)
    end,
    
    -- Ease out elastic
    easeOutElastic = function(t)
        if t == 0 then return 0 end
        if t == 1 then return 1 end
        local c4 = (2 * math.pi) / 3
        return math.pow(2, -10 * t) * math.sin((t * 10 - 0.75) * c4) + 1
    end
}

--- Sword tween configuration
asrielGodBoss.SwordConfig = {
    defaultDuration = 0.3,
    defaultRotationSpeed = 720, -- degrees per second for spin
    slashCount = 3,
    delayBetweenSlashes = 0.5
}

--- Create a sword tween data structure
---@param startPos table|Vector2 Starting position
---@param endPos table|Vector2 Target position
---@param duration number Tween duration in seconds
---@param easing function Easing function to use
---@return table Tween data structure
function asrielGodBoss.createSwordTween(startPos, endPos, duration, easing)
    return {
        startPosition = vector2(startPos),
        endPosition = vector2(endPos),
        duration = duration or asrielGodBoss.SwordConfig.defaultDuration,
        easing = easing or asrielGodBoss.Easing.easeOutCubic,
        elapsed = 0,
        active = true,
        rotation = 0,
        targetRotation = math.atan2(
            endPos[2] - startPos[2],
            endPos[1] - startPos[1]
        )
    }
end

--- Update a sword tween
---@param tween table The tween data structure
---@param deltaTime number Time elapsed since last update
---@return table Updated position {x, y}
---@return boolean Whether the tween is still active
function asrielGodBoss.updateSwordTween(tween, deltaTime)
    if not tween.active then
        return {tween.endPosition.X, tween.endPosition.Y}, false
    end
    
    tween.elapsed = tween.elapsed + deltaTime
    local t = math.min(tween.elapsed / tween.duration, 1)
    local easedT = tween.easing(t)
    
    local currentX = tween.startPosition.X + (tween.endPosition.X - tween.startPosition.X) * easedT
    local currentY = tween.startPosition.Y + (tween.endPosition.Y - tween.startPosition.Y) * easedT
    
    -- Interpolate rotation
    tween.rotation = tween.rotation + (tween.targetRotation - tween.rotation) * easedT
    
    if t >= 1 then
        tween.active = false
    end
    
    return {currentX, currentY}, tween.active
end

--- Execute a sword slash attack towards player
---@param boss userdata The boss entity
---@param player userdata The player entity
---@param duration number Optional tween duration
function asrielGodBoss.executeSwordSlash(boss, player, duration)
    if not boss or not player then return nil end
    
    local startPos = {boss.Position.X, boss.Position.Y - 20}
    local endPos = {player.Position.X, player.Position.Y}
    
    local tween = asrielGodBoss.createSwordTween(startPos, endPos, duration)
    
    -- Play sword animation
    if boss.swordSprite then
        boss.swordSprite:Play("slash")
        boss.swordSprite.Visible = true
    end
    
    helpers.log("Executing sword slash from (" .. startPos[1] .. ", " .. startPos[2] .. 
                ") to (" .. endPos[1] .. ", " .. endPos[2] .. ")")
    
    return tween
end

--- Execute a spinning sword attack
---@param boss userdata The boss entity
---@param centerPos table|Vector2 Center position for the spin
---@param radius number Radius of the spin
---@param duration number Duration of the spin
---@param rotations number Number of full rotations
function asrielGodBoss.executeSwordSpin(boss, centerPos, radius, duration, rotations)
    radius = radius or 60
    duration = duration or 1.5
    rotations = rotations or 2
    
    local center = vector2(centerPos or {boss.Position.X, boss.Position.Y})
    
    return {
        center = center,
        radius = radius,
        duration = duration,
        totalRotation = rotations * math.pi * 2,
        elapsed = 0,
        active = true
    }
end

--- Update a spinning sword attack
---@param spinData table The spin data structure
---@param deltaTime number Time elapsed since last update
---@return table Current sword position {x, y}
---@return number Current rotation angle
---@return boolean Whether the spin is still active
function asrielGodBoss.updateSwordSpin(spinData, deltaTime)
    if not spinData.active then
        return {spinData.center.X, spinData.center.Y}, 0, false
    end
    
    spinData.elapsed = spinData.elapsed + deltaTime
    local t = math.min(spinData.elapsed / spinData.duration, 1)
    
    local angle = spinData.totalRotation * t
    local x = spinData.center.X + math.cos(angle) * spinData.radius
    local y = spinData.center.Y + math.sin(angle) * spinData.radius
    
    if t >= 1 then
        spinData.active = false
    end
    
    return {x, y}, angle, spinData.active
end

--#endregion

--#region Music Helpers

--- Music tracks for different boss phases
asrielGodBoss.MusicTracks = {
    PHASE_1 = "event:/Ingeste/final_content/music/lvl20/els_08",
    PHASE_2 = "event:/Ingeste/final_content/music/lvl20/els_08",
    PHASE_3 = "event:/Ingeste/final_content/music/lvl20/els_08",
    ASRIEL_REMEMBER_1 = "event:/Ingeste/final_content/music/lvl20/els_10",
    ASRIEL_REMEMBER_2 = "event:/Ingeste/final_content/music/lvl20/els_11"
}

--- Set the boss music phase
---@param level userdata The current level
---@param phase number The music phase (1-4, or use string from MusicTracks)
function asrielGodBoss.setMusicPhase(level, phase)
    local track = asrielGodBoss.MusicTracks["PHASE_" .. tostring(phase)]
    if track and level and level.Session and level.Session.Audio then
        level.Session.Audio.Music.Event = track
        level.Session.Audio:Apply()
    end
end

--#endregion

return asrielGodBoss
