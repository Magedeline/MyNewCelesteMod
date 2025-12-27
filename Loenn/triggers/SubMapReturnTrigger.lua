local SubMapReturnTrigger = {}

SubMapReturnTrigger.name = "IngesteHelper/SubMapReturnTrigger"
SubMapReturnTrigger.placements = {
    {
        name = "submap_return_trigger",
        data = {
            returnLevel = "",
            useManager = true
        }
    }
}

SubMapReturnTrigger.fieldInformation = {
    returnLevel = {
        fieldType = "string",
        description = "Specific level to return to (optional, uses SubMapManager if empty)"
    },
    useManager = {
        fieldType = "boolean",
        description = "Whether to use SubMapManager for automatic return handling"
    }
}

SubMapReturnTrigger.nodeLimits = {0, 1}
SubMapReturnTrigger.nodeLineRenderType = "line"

return SubMapReturnTrigger