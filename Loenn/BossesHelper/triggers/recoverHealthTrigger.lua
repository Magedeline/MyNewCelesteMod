local recoverHealthTrigger = {}

recoverHealthTrigger.name = "DesoloZatnas/BossesHelper/RecoverHealthTrigger"
recoverHealthTrigger.depth = 0
recoverHealthTrigger.nodeLimits = {0, 0}
recoverHealthTrigger.fieldInformation = {
    healAmount = {
        validator = function (string)
            res = tonumber(string)
            return res == nil or res > 0 and string:find("%.") == nil
        end
    }
}
recoverHealthTrigger.placements = {
    name = "Recover Health Trigger",
    data = {
        healAmount = 1,
        onlyOnce = true,
        permanent = false
    }
}

return recoverHealthTrigger