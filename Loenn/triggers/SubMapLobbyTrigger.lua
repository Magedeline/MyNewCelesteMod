local SubMapLobbyTrigger = {}

SubMapLobbyTrigger.name = "IngesteHelper/SubMapLobbyTrigger"
SubMapLobbyTrigger.placements = {
    {
        name = "submap_lobby_trigger",
        data = {
            chapterNumber = 10,
            requiredFlag = ""
        }
    }
}

SubMapLobbyTrigger.fieldInformation = {
    chapterNumber = {
        fieldType = "integer",
        minimumValue = 10,
        maximumValue = 14,
        description = "Chapter number for the submap lobby to enter (10-14)"
    },
    requiredFlag = {
        fieldType = "string",
        description = "Session flag required to access the lobby (optional)"
    }
}

SubMapLobbyTrigger.nodeLimits = {1, 1}
SubMapLobbyTrigger.nodeLineRenderType = "line"

return SubMapLobbyTrigger