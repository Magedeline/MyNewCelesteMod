local PlatinumStrawberry = {}

PlatinumStrawberry.name = "Ingeste/PinkPlatinumStrawberry"
PlatinumStrawberry.depth = 8998
PlatinumStrawberry.texture = "collectables/maggy/pinkplatberry/idle00"

PlatinumStrawberry.fieldInformation = {
    collectSound = {
        editable = false,
        options = {
            ["Original"] = "Original",
            ["Elaborate"] = "Elaborate",
            ["Minimalist"] = "Minimalist",
            ["Custom"] = "Custom"
        }
    }
}

PlatinumStrawberry.placements = {
    name = "PinkPlatinumStrawberry",
    data = {
        collectSound = "Original",
        customCollectSound = ""
    }
}

return PlatinumStrawberry