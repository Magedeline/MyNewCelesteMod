--[[
    Chara Boss Entity (Ingeste/CharaBoss)
    
    Attack Patterns:
    - Pattern 0: Intro/Sitting - Boss sits and plays pretend dead animation until triggered
    - Pattern 1: Shoot Only - Rapid projectile attacks with charging animation
    - Pattern 2: Beam + Shoot - Alternates between laser beams and projectile shots
    - Pattern 21: Bigger Beam + Triple Shoot - Larger laser beam followed by 3 consecutive shots
    
    Custom Attack Sequence Format:
    "action arg delay, action arg delay, ..."
    
    Available Actions:
    1. shoot [angleOffset] [delay]
       - Fires a projectile at the player
       - angleOffset: Angle offset in degrees (optional, default 0)
       - delay: Wait time after attack in seconds (optional, default 0.3)
       - Examples: "shoot 0.5", "shoot 15 0.3", "shoot -30 0.2"
    
    2. beam [delay]
       - Fires a laser beam at the player
       - delay: Wait time after attack in seconds (optional, default 0.3)
       - Examples: "beam 1.0", "beam 0.5"
    
    3. biggerbeam [delay]
       - Fires a larger, more powerful laser beam
       - delay: Wait time after attack in seconds (optional, default 0.3)
       - Examples: "biggerbeam 1.5", "biggerbeam 2.0"
    
    Attack Sequence Examples:
    - "shoot 0.5, shoot 15 0.5, shoot -15 0.5" (3 shots at different angles)
    - "beam 1.0, shoot 0.3, shoot 0.3" (beam followed by 2 quick shots)
    - "biggerbeam 2.0, shoot 0 0.2, shoot 15 0.2, shoot -15 0.2" (big beam + 3 angled shots)
    - "shoot 0.2, beam 0.8, shoot 0.2, biggerbeam 1.5" (mixed attack pattern)
    
    Notes:
    - Boss requires at least 1 node (up to 20) for movement path
    - Boss moves to next node when hit by player
    - Camera locks at x=140 past player, Y can be locked or unlocked
    - Dialog shows mini-textbox on hits: ch8_charaboss_tired_a/b/c
    - If attackSequence is empty, uses patternIndex for built-in patterns
--]]

local charaBoss = {}

charaBoss.name = "Ingeste/CharaBoss"
charaBoss.depth = -8500
charaBoss.texture = "characters/charaBoss/boss00"
charaBoss.justification = {0.5, 1.0}

charaBoss.nodeLimits = {1, 20}

charaBoss.placements = {
    {
        name = "chara_boss",
        data = {
            patternIndex = 0,
            cameraPastY = 120.0,
            dialog = false,
            startHit = false,
            cameraLockY = true,
        }
    }
}

charaBoss.fieldOrder = {
    "x", "y",
    "patternIndex",
    "attackSequence",
    "cameraPastY",
    "dialog",
    "startHit",
    "cameraLockY"
}

charaBoss.fieldInformation = {
    patternIndex = {
        fieldType = "integer",
        options = {0, 1, 2, 21},
        editable = true
    },
    cameraPastY = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 500.0
    },
    dialog = {
        fieldType = "boolean"
    },
    startHit = {
        fieldType = "boolean"
    },
    cameraLockY = {
        fieldType = "boolean"
    },
    attackSequence = {
        fieldType = "string"
    }
}

return charaBoss