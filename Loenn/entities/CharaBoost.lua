return {
    name = "Ingeste/CharaBoost",
    depth = -1000000,
    nodeLineRenderType = "line",
    texture = "objects/charaboost/idle00",
    nodeLimits = {1, -1}, -- Requires at least one node for movement
    fieldInformation = {
        finalCh9Dialog = {
            fieldType = "string",
        }
    },
    fieldOrder = {
        "x", "y",
        "lockCamera",
        "canSkip",
        "finalCh19Boost",
        "finalCh19GoldenBoost",
        "finalCh19Dialog"
    },
    placements = {
        name = "Chara Boost",
        data = {
            lockCamera = true,
            canSkip = false,
            finalCh19Boost = false,
            finalCh19GoldenBoost = false,
            finalCh19Dialog = "CH19_CHARA_LAST_BOOST"
        }
    }
}
