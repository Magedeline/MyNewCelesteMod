local function createTable(innerName, outerName)
    return {
        name = "DesoloZatnas/BossesHelper/"..innerName,
        depth = 0,
        nodeLimits = {0, 0},
        placements = {
            name = outerName,
            data = {
                enableState = true,
                pauseState = false
            }
        }
    }
end

local enableHealthTrigger = createTable("HealthEnableTrigger", "Enable Health Trigger")

local healthSystemZoneTrigger = createTable("HealthSystemZoneTrigger", "Health System Enabled-Zone Trigger")

return {
    enableHealthTrigger,
    healthSystemZoneTrigger
}