local interactTrigger = {}

interactTrigger.name = "Ingeste/InteractTrigger"
interactTrigger.placements = {
    {
        name = "interact_trigger",
        data = {
            x = 0,
            y = 0,
            width = 16,
            height = 16,
            prompt = "Press {button} to interact",
            onInteract = "",
            onlyOnce = false
        }
    }
}

interactTrigger.fieldInformation = {
    prompt = {
        fieldType = "string"
    },
    onInteract = {
        fieldType = "string"
    },
    onlyOnce = {
        fieldType = "boolean"
    }
}

return interactTrigger