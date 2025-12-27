local function createTable(name, nodeLimits)
    return {
        name = "DesoloZatnas/BossesHelper/"..name.."HealthBarVisibleTrigger",
        depth = 0,
        nodeLimits = nodeLimits,
        placements = {
            name = name.." Health Bar Visible Trigger",
            data = {
                state = true,
                onlyOnce = true
            }
        }
    }
end

local healthBarVisibleTrigger = createTable("Player", {0, 0})

local bossHealthVisible = createTable("Boss", {1, 1})

bossHealthVisible.nodeLineRenderType = "line"

return {
    healthBarVisibleTrigger,
    bossHealthVisible
}