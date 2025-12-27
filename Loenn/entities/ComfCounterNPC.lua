local comfCounterNPC = {}

comfCounterNPC.name = "Ingeste/ComfCounterNPC"
comfCounterNPC.depth = 100
comfCounterNPC.justification = {0.5, 1.0}
comfCounterNPC.texture = "characters/theo/idle00"

comfCounterNPC.placements = {
    {
        name = "default",
        data = {
            maxCount = 10,
            showOutOf = true,
            centeredCounter = false,
            dialogKey = "ingeste_comf_npc_default"
        }
    },
    {
        name = "small_counter",
        data = {
            maxCount = 5,
            showOutOf = true,
            centeredCounter = false,
            dialogKey = "ingeste_comf_npc_small"
        }
    },
    {
        name = "large_counter",
        data = {
            maxCount = 20,
            showOutOf = true,
            centeredCounter = false,
            dialogKey = "ingeste_comf_npc_large"
        }
    }
}

comfCounterNPC.fieldInformation = {
    maxCount = {
        fieldType = "integer",
        minimumValue = 1,
        maximumValue = 100
    },
    showOutOf = {
        fieldType = "boolean"
    },
    centeredCounter = {
        fieldType = "boolean"
    },
    dialogKey = {
        fieldType = "string"
    }
}

return comfCounterNPC
