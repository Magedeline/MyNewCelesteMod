local SubMapEnterTrigger = {}

SubMapEnterTrigger.name = "IngesteHelper/SubMapEnterTrigger"
SubMapEnterTrigger.placements = {
    {
        name = "submap_enter_ch10_s3",
        data = {
            chapterNumber = 10,
            submapNumber = 1,
            checkUnlocked = true
        }
    },
    {
        name = "submap_enter_ch11_s3",
        data = {
            chapterNumber = 11,
            submapNumber = 1,
            checkUnlocked = true
        }
    },
    {
        name = "submap_enter_ch12_s3",
        data = {
            chapterNumber = 12,
            submapNumber = 1,
            checkUnlocked = true
        }
    },
    {
        name = "submap_enter_ch13_s3",
        data = {
            chapterNumber = 13,
            submapNumber = 1,
            checkUnlocked = true
        }
    },
    {
        name = "submap_enter_ch14_s3",
        data = {
            chapterNumber = 14,
            submapNumber = 1,
            checkUnlocked = true
        }
    }
}

SubMapEnterTrigger.fieldInformation = {
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
    checkUnlocked = {
        fieldType = "boolean",
        description = "Whether to check if the submap is unlocked before entering"
    }
}

return SubMapEnterTrigger