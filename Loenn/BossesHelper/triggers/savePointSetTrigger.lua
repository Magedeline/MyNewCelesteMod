local savePointSetTrigger = {}

local respawnOpts = {
    "Transition",
    "Respawn",
    "WalkInRight",
    "WalkInLeft",
    "Jump",
    "WakeUp",
    "Fall",
    "TempleMirrorVoid",
    "None",
    "ThinkForABit"
}

savePointSetTrigger.name = "DesoloZatnas/BossesHelper/SavePointSetTrigger"
savePointSetTrigger.depth = 0
savePointSetTrigger.nodeLimits = {0, 1}
savePointSetTrigger.nodeLineRenderType = "line"
savePointSetTrigger.fieldInformation = {
    respawnType = {
        options = respawnOpts,
        editable = false
    }
}
savePointSetTrigger.placements = {
    name = "Save Point Set Trigger",
    data = {
        respawnType = "Respawn",
        flagTrigger = "",
        onlyOnce = true,
        invertFlag = false
    }
}

return savePointSetTrigger