local bossHealthBar = {}

local barTypeOpts = {
    {"Icons", "icons"},
    {"Bar (Left)", "barLeft"},
    {"Bar (Right)", "barRight"},
    {"Bar (Center)", "barCentered"},
    {"Countdown", "countdown"}
}

bossHealthBar.name = "DesoloZatnas/BossesHelper/BossHealthBar"
bossHealthBar.depth = 0
bossHealthBar.texture = "loenn/DesoloZatnas/BossesHelper/HealthBar"
bossHealthBar.nodeLimits = {1, 1}
bossHealthBar.nodeLineRenderType = "line"
bossHealthBar.fieldInformation = {
    healthIcons = {
        fieldType = "list",
        minimumElements = 1,
        elementOptions = {
            fieldType = "string"
        }
    },
    healthIconsCreateAnim = {
        fieldType = "list",
        minimumElements = 1,
        elementOptions = {
            fieldType = "string"
        }
    },
    healthIconsRemoveAnim = {
        fieldType = "list",
        minimumElements = 1,
        elementOptions = {
            fieldType = "string"
        }
    },
    healthIconsSeparation = {
        fieldType = "list",
        minimumElements = 1,
        elementDefault = "0.0",
        elementOptions = {
            validator = function (string)
                res = tonumber(string)
                return res == nil or res >= 0
            end
        }
    },
    baseColor = {
        fieldType = "color"
    },
    barType = {
        options = barTypeOpts,
        editable = false
    },
    healthBarX = {
        fieldType = "number",
        minimumValue = 0,
        maximumValue = 1920
    },
    healthBarY = {
        fieldType = "number",
        minimumValue = 0,
        maximumValue = 1080
    },
    __bar_pad = {
        fieldType = "spacer"
    }
}

function bossHealthBar.fieldOrder(entity)
    if entity.barType == "icons" then
        return {
            "healthBarX", "healthBarY",
            "barType", "healthIcons",
            "healthIconsCreateAnim", "healthIconsRemoveAnim",
            "healthScaleX", "healthScaleY",
            "healthIconsSeparation", "__bar_pad",
            "startVisible"
        }
    end
    if entity.barType == "countdown" then
        return {
            "healthBarX", "healthBarY",
            "barType", "baseColor",
            "healthScaleX", "healthScaleY",
            "startVisible"
        }
    end
    return {
        "healthBarX", "healthBarY",
        "barType", "baseColor",
        "healthScaleX", "healthScaleY",
        "startVisible"
    }
end

function bossHealthBar.ignoredFields(entity)
    local ignored = {
        "_name", "_id", "originX", "originY", "x", "y",
        "healthIcons", "healthIconsCreateAnim", "healthIconsRemoveAnim", "healthIconsSeparation",
        "baseColor"
    }
    local function doNotIgnore(value)
        for i = #ignored, 1, -1 do
            if ignored[i] == value then
                table.remove(ignored, i)
                return
            end
        end
    end

    if entity.barType == "icons" then
        doNotIgnore("healthIcons")
        doNotIgnore("healthIconsCreateAnim")
        doNotIgnore("healthIconsRemoveAnim")
        doNotIgnore("healthIconsSeparation")
    elseif entity.barType == "countdown" then
        doNotIgnore("baseColor")
    else
        doNotIgnore("baseColor")
    end
    return ignored
end

bossHealthBar.placements = {
    {
        name = "Boss Health Bar (Icons)",
        data = {
            barType = "icons",
            healthIcons = "",
            healthIconsCreateAnim = "",
            healthIconsRemoveAnim = "",
            healthIconsSeparation = "0",
            healthBarX = 0,
            healthBarY = 0,
            healthScaleX = 1,
            healthScaleY = 1,
            startVisible = true
        }
    },
    {
        name = "Boss Health Bar (Countdown)",
        data = {
            barType = "countdown",
            baseColor = "#FFFFFF",
            healthBarX = 0,
            healthBarY = 0,
            healthScaleX = 1,
            healthScaleY = 1,
            startVisible = true
        }
    },
    {
        name = "Boss Health Bar (Bar, Left)",
        data = {
            barType = "barLeft",
            baseColor = "#FF0000",
            healthBarX = 0,
            healthBarY = 0,
            healthScaleX = 1,
            healthScaleY = 1,
            startVisible = true
        }
    },
    {
        name = "Boss Health Bar (Bar, Right)",
        data = {
            barType = "barRight",
            baseColor = "#FF0000",
            healthBarX = 0,
            healthBarY = 0,
            healthScaleX = 1,
            healthScaleY = 1,
            startVisible = true
        }
    },
    {
        name = "Boss Health Bar (Bar, Center)",
        data = {
            barType = "barCentered",
            baseColor = "#FF0000",
            healthBarX = 0,
            healthBarY = 0,
            healthScaleX = 1,
            healthScaleY = 1,
            startVisible = true
        }
    }
}

return bossHealthBar