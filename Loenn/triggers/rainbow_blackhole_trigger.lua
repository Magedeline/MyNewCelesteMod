local rainbowBlackholeTrigger = {}

rainbowBlackholeTrigger.name = "Ingeste/RainbowBlackholeTrigger"
rainbowBlackholeTrigger.displayName = "Rainbow Blackhole Trigger"
rainbowBlackholeTrigger.nodeLimits = {0, 0}

rainbowBlackholeTrigger.fieldInformation = {
    action = {
        options = {
            "Enable",
            "Disable",
            "ChangeStrength",
            "SetAlpha",
            "SetScale",
            "SetDirection",
            "Toggle"
        },
        editable = false
    },
    strength = {
        options = {
            "Mild",
            "Medium",
            "High",
            "Wild",
            "Insane",
            "RainbowChaos",
            "Cosmic"
        },
        editable = false
    }
}

rainbowBlackholeTrigger.placements = {
    {
        name = "enable",
        data = {
            width = 16,
            height = 16,
            action = "Enable",
            strength = "Medium",
            alpha = 1.0,
            scale = 1.0,
            direction = 1.0,
            triggerOnce = false,
            fadeTime = 1.0,
            flag = "",
            onlyIfFlag = false
        }
    },
    {
        name = "disable",
        data = {
            width = 16,
            height = 16,
            action = "Disable",
            strength = "Medium",
            alpha = 0.0,
            scale = 1.0,
            direction = 1.0,
            triggerOnce = false,
            fadeTime = 1.0,
            flag = "",
            onlyIfFlag = false
        }
    },
    {
        name = "change_strength",
        data = {
            width = 16,
            height = 16,
            action = "ChangeStrength",
            strength = "High",
            alpha = 1.0,
            scale = 1.0,
            direction = 1.0,
            triggerOnce = false,
            fadeTime = 0.0,
            flag = "",
            onlyIfFlag = false
        }
    },
    {
        name = "set_alpha",
        data = {
            width = 16,
            height = 16,
            action = "SetAlpha",
            strength = "Medium",
            alpha = 0.5,
            scale = 1.0,
            direction = 1.0,
            triggerOnce = false,
            fadeTime = 2.0,
            flag = "",
            onlyIfFlag = false
        }
    },
    {
        name = "toggle",
        data = {
            width = 16,
            height = 16,
            action = "Toggle",
            strength = "Medium",
            alpha = 1.0,
            scale = 1.0,
            direction = 1.0,
            triggerOnce = true,
            fadeTime = 0.0,
            flag = "",
            onlyIfFlag = false
        }
    }
}

return rainbowBlackholeTrigger