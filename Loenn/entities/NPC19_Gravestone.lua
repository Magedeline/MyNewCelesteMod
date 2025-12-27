local npc19_gravestone = {}

npc19_gravestone.name = "Ingeste/NPC19_Gravestone"
npc19_gravestone.depth = 0
npc19_gravestone.justification = {0.5, 1.0}
npc19_gravestone.texture = "characters/gravestones/maddydead00"

npc19_gravestone.placements = {
    {
        name = "NPC19_Gravestone",
        data = {
            dialogKey = "CH19_GRAVESTONE",
            flagName = "maddy_gravestone",
            spriteId = "maddydead"
        }
    }
}

npc19_gravestone.fieldInformation = {
    dialogKey = {
        fieldType = "string"
    },
    flagName = {
        fieldType = "string"
    },
    spriteId = {
        fieldType = "string",
        editable = true,
        options = {
            "gravestone",
            "memorial",
            "stone"
        }
    }
}

return npc19_gravestone
