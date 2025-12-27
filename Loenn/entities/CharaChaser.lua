local utils = require("utils")
local loadedState = require("loaded_state")
local logging = require("logging")

return {
        name = "Ingeste/CharaChaser",
    depth = 0,
    nodeLineRenderType = "line",
    texture = "characters/chara/idle00",
    nodeLimits = {0, -1},
    fieldInformation = {
        canChangeMusic = {
            fieldType = "boolean"
        }
    },
    placements = {
        name = "CharaChaser",
        data = {
            canChangeMusic = true
        }
    }
}
