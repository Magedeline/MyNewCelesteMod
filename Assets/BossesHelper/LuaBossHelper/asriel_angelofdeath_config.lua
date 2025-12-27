-- Asriel Angel of Death Boss Configuration
-- Defines boss behavior, phases, and integration with BossesHelper

return {
    -- Boss Identity
    entityName = "DesoloZatnas/AsrielAngelOfDeathBoss",
    displayName = "Asriel - Angel of Death",
    
    -- Boss Stats
    phases = {
        {
            id = 1,
            name = "Angel of Death",
            maxHealth = 100,
            damageMultiplier = 1.0,
            speedMultiplier = 1.0,
            
            -- Visual settings
            spritePrefix = "characters/asrielangelofdeathboss/",
            startAnimation = "idle",
            
            -- Cosmowing effects enabled
            effects = {
                cosmowing = {
                    enabled = true,
                    intensity = "normal",
                    parts = {"bg", "stems", "shoulder", "orbwings"}
                }
            },
            
            -- Available attacks
            attacks = {
                "ultimaBullet",
                "crossShocker",
                "starStormUltra"
            },
            
            -- Attack patterns
            attackPattern = "random", -- or "sequence", "weighted"
            attackDelay = {min = 0.3, max = 0.7},
            
            -- Music
            music = "event:/music/boss/asriel_angelofdeath_phase1"
        },
        
        {
            id = 2,
            name = "Transcendent Angel",
            maxHealth = 150,
            damageMultiplier = 1.5,
            speedMultiplier = 1.2,
            
            -- Visual settings
            spritePrefix = "characters/asrielangelofdeathboss/phase2/",
            startAnimation = "phase2_idle",
            
            -- Tween nodes enabled for multi-part boss
            tweenNodes = {
                enabled = true,
                parts = {
                    {name = "head", offset = {x = 0, y = -30}},
                    {name = "torso", offset = {x = 0, y = 0}},
                    {name = "arms_left", offset = {x = -25, y = 0}},
                    {name = "arms_right", offset = {x = 25, y = 0}},
                    {name = "wings_left", offset = {x = -40, y = 5}},
                    {name = "wings_right", offset = {x = 40, y = 5}},
                    {name = "halo", offset = {x = 0, y = -45}}
                }
            },
            
            -- Enhanced cosmowing effects
            effects = {
                cosmowing = {
                    enabled = true,
                    intensity = "intense",
                    parts = {"bg", "stems", "shoulder", "orbwings"},
                    rainbow = true,
                    speed = 1.5
                }
            },
            
            -- Available attacks (includes all Phase 1 attacks plus new ones)
            attacks = {
                "ultimaBullet",
                "crossShocker",
                "starStormUltra",
                "shockerBreaker3",
                "finalBeam"
            },
            
            -- Attack weights (for weighted random selection)
            attackWeights = {
                ultimaBullet = 15,
                crossShocker = 20,
                starStormUltra = 15,
                shockerBreaker3 = 25,
                finalBeam = 10 -- Rarer but more powerful
            },
            
            attackPattern = "weighted",
            attackDelay = {min = 0.2, max = 0.5},
            
            -- Music
            music = "event:/music/boss/asriel_angelofdeath_phase2"
        }
    },
    
    -- Phase Transition Settings
    transitions = {
        {
            fromPhase = 1,
            toPhase = 2,
            trigger = "health_depleted", -- or "time", "attack_count", etc.
            animation = {
                start = "phase2_transform_start",
                mid = "phase2_transform_mid",
                end = "phase2_transform_end"
            },
            duration = 3.0,
            invulnerable = true,
            effects = {
                screenShake = true,
                particleBurst = true,
                soundEvent = "event:/sfx/boss/asriel_transform"
            }
        }
    },
    
    -- Projectile Definitions
    projectiles = {
        UltimaBullet = {
            sprite = "characters/asrielangelofdeathboss/projectiles/ultimabullet",
            collider = {width = 8, height = 8},
            behavior = "homing",
            speed = 180,
            homingStrength = 120,
            lifetime = 5.0,
            damage = "instant_death",
            particles = {
                trail = true,
                color1 = {r = 0, g = 255, b = 255},
                color2 = {r = 255, g = 0, b = 255}
            }
        },
        
        CrossShockerWave = {
            sprite = "characters/asrielangelofdeathboss/projectiles/crossshocker",
            collider = {width = 16, height = 16},
            behavior = "linear",
            speed = 200,
            lifetime = 3.0,
            damage = "instant_death",
            effects = {
                lightning = true,
                pulse = true
            }
        },
        
        StarStorm = {
            sprite = "characters/asrielangelofdeathboss/projectiles/starstorm",
            collider = {width = 12, height = 12},
            behavior = "falling",
            fallSpeed = 200,
            acceleration = 50,
            damage = "instant_death",
            effects = {
                rotation = true,
                particles = true,
                explosion = {
                    onImpact = true,
                    radius = 20
                }
            }
        },
        
        ShockerBreaker3Wave = {
            type = "shockwave",
            behavior = "expanding_ring",
            expandSpeed = 200,
            thickness = 20,
            damage = "instant_death",
            visual = {
                color = {r = 76, g = 204, b = 255, a = 200},
                electricArcs = true,
                arcCount = 8
            }
        },
        
        FinalBeam = {
            type = "beam",
            width = 40,
            length = 1000,
            duration = 3.0,
            damage = "instant_death",
            visual = {
                layers = 3,
                color1 = {r = 255, g = 76, b = 204, a = 230},
                color2 = {r = 76, g = 204, b = 255, a = 230},
                core = {r = 255, g = 255, b = 255, a = 255},
                pulse = true,
                particles = true
            }
        }
    },
    
    -- Death Sequence
    death = {
        animation = "defeat",
        duration = 2.0,
        effects = {
            screenShake = true,
            particleExplosion = true,
            soundEvent = "event:/sfx/boss/asriel_death"
        },
        reward = {
            flag = "asriel_angelofdeath_defeated",
            strawberryUnlock = true,
            cutscene = "asriel_angelofdeath_aftermath"
        }
    },
    
    -- Special Mechanics
    mechanics = {
        -- Cosmowing rainbow background
        cosmowing = {
            enabled = true,
            blendMode = "additive",
            speed = 1.0,
            colors = {
                {r = 255, g = 0, b = 0},    -- Red
                {r = 255, g = 127, b = 0},  -- Orange
                {r = 255, g = 255, b = 0},  -- Yellow
                {r = 0, g = 255, b = 0},    -- Green
                {r = 0, g = 0, b = 255},    -- Blue
                {r = 75, g = 0, b = 130},   -- Indigo
                {r = 148, g = 0, b = 211}   -- Violet
            },
            transitionSpeed = 0.5
        },
        
        -- Tween nodes for phase 2
        tweenNodes = {
            interpolation = "smooth",
            bobbing = {
                enabled = true,
                amplitude = 2.0,
                frequency = 1.0,
                offset = true -- Each part has different phase
            },
            rotation = {
                enabled = true,
                parts = {"halo"},
                speed = 0.5
            }
        }
    },
    
    -- Audio
    audio = {
        attackSounds = {
            ultimaBullet = "event:/sfx/boss/asriel_ultimabullet",
            crossShocker = "event:/sfx/boss/asriel_crossshocker",
            starStormUltra = "event:/sfx/boss/asriel_starstorm",
            shockerBreaker3 = "event:/sfx/boss/asriel_shockerbreaker3",
            finalBeam = "event:/sfx/boss/asriel_finalbeam"
        },
        hurtSound = "event:/sfx/boss/asriel_hurt",
        deathSound = "event:/sfx/boss/asriel_death",
        transformSound = "event:/sfx/boss/asriel_transform"
    }
}
