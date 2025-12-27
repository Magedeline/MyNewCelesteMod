local SubMapCompletionTrigger = {}

SubMapCompletionTrigger.name = "IngesteHelper/SubMapCompletionTrigger"
SubMapCompletionTrigger.placements = {
    {
        name = "submap_completion_trigger",
        data = {
            chapterNumber = 10,
            submapNumber = 1,
            requireAllGems = true,
            playCompletionCutscene = true
        }
    }
}

SubMapCompletionTrigger.fieldInformation = {
    chapterNumber = {
        fieldType = "integer",
        minimumValue = 10,
        maximumValue = 14,
        description = "Chapter number (10-14)"
    },
    submapNumber = {
        fieldType = "integer",
    minimumValue = 1,
        maximumValue = 6,
    description = "Submap number (1-6)"
    },
    requireAllGems = {
        fieldType = "boolean",
        description = "Whether all gems must be collected to complete the submap"
    },
    playCompletionCutscene = {
        fieldType = "boolean",
        description = "Whether to play a completion cutscene when triggered"
    }
}

return SubMapCompletionTrigger