-- Color Grade Effect for Lönn
local colorgradeEffect = {}

colorgradeEffect.name = "Ingeste/ColorGradeEffect"
colorgradeEffect.canForeground = true
colorgradeEffect.canBackground = true

colorgradeEffect.fieldInformation = {
    lutTextureName = {
        fieldType = "string",
        options = {
            "default",
            "cold",
            "warm",
            "sepia",
            "noir",
            "golden_hour"
        }
    },
    intensity = {
        fieldType = "number",
        minimumValue = 0.0,
        maximumValue = 1.0
    }
}

colorgradeEffect.placements = {
    {
        name = "colorgrade",
        data = {
            lutTextureName = "default",
            intensity = 1.0
        }
    }
}

return colorgradeEffect
