local rainbow_blackhole_bg = {}

rainbow_blackhole_bg.name = "Ingeste/RainbowBlackholeBg"
rainbow_blackhole_bg.canForeground = false
rainbow_blackhole_bg.canBackground = true

rainbow_blackhole_bg.fieldInformation = {
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

rainbow_blackhole_bg.placements = {
    {
        name = "rainbow_blackhole_bg",
        data = {
            strength = "Medium",
            scrollX = 1.0,
            scrollY = 1.0,
            speedX = 0.0,
            speedY = 0.0,
            fadeX = "",
            fadeY = "",
            color = "FFFFFF",
            alpha = 1.0,
            flipX = false,
            flipY = false,
            loopX = true,
            loopY = true,
            instantIn = false,
            instantOut = false,
            fadeIn = false,
            fadeOut = false,
            tag = "",
            flag = "",
            notFlag = "",
            always = "",
            dreaming = "",
            exclude = ""
        }
    }
}

return rainbow_blackhole_bg
