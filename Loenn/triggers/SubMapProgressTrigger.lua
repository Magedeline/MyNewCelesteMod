local SubMapProgressTrigger = {}

SubMapProgressTrigger.name = "IngesteHelper/SubMapProgressTrigger"
SubMapProgressTrigger.placements = {
    {
        name = "submap_progress_trigger",
        data = {
            chapterNumber = 10,
            submapNumber = 1,
            showGemsCollected = true,
            resetDelay = 2.0
        }
    }
}

SubMapProgressTrigger.fieldInformation = {
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
    showGemsCollected = {
        fieldType = "boolean",
        description = "Whether to show gem collection progress in the message"
    },
    resetDelay = {
        fieldType = "number",
        minimumValue = 0.5,
        maximumValue = 10.0,
        description = "Time in seconds before the trigger can fire again"
    }
}

return SubMapProgressTrigger