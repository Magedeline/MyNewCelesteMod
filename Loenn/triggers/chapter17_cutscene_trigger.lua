-- Chapter 17 Cutscene Triggers for Loenn
-- Handles all the descent and credits sequences

local chapter17CutsceneTrigger = {}

chapter17CutsceneTrigger.name = "Ingeste/Chapter17CutsceneTrigger"

chapter17CutsceneTrigger.placements = {
    -- Journey Beginning
    {
        name = "descent_begins",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_descent_begins",
            triggerOnce = true
        }
    },
    {
        name = "leaving_zantas",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_leaving_zantas",
            triggerOnce = true
        }
    },
    
    -- Credits Sequences
    {
        name = "credits_introduction",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_credits_introduction",
            triggerOnce = true
        }
    },
    {
        name = "thanking_companions",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_thanking_companions",
            triggerOnce = true
        }
    },
    {
        name = "remembering_journey",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_remembering_journey",
            triggerOnce = true
        }
    },
    {
        name = "crossing_barrier",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_crossing_barrier",
            triggerOnce = true
        }
    },
    
    -- Mountain Descent Phases
    {
        name = "upper_slopes",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_upper_slopes",
            triggerOnce = true
        }
    },
    {
        name = "crystal_caves",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_crystal_caves",
            triggerOnce = true
        }
    },
    {
        name = "forest_path",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_forest_path",
            triggerOnce = true
        }
    },
    {
        name = "valley_crossing",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_valley_crossing",
            triggerOnce = true
        }
    },
    
    -- Reunion Moments
    {
        name = "granny_reunion",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_granny_reunion",
            triggerOnce = true
        }
    },
    {
        name = "theo_reunion",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_theo_reunion",
            triggerOnce = true
        }
    },
    {
        name = "oshiro_return",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_oshiro_return",
            triggerOnce = true
        }
    },
    {
        name = "badeline_reflection",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_badeline_reflection",
            triggerOnce = true
        }
    },
    
    -- Homecoming
    {
        name = "celeste_base",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_celeste_base",
            triggerOnce = true
        }
    },
    {
        name = "final_farewells",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_final_farewells",
            triggerOnce = true
        }
    },
    {
        name = "home_arrival",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_home_arrival",
            triggerOnce = true
        }
    },
    {
        name = "credits_finale",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_credits_finale",
            triggerOnce = true
        }
    },
    
    -- Special Character Moments
    {
        name = "kirby_farewell",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_kirby_farewell",
            triggerOnce = true
        }
    },
    {
        name = "chara_reflection",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_chara_reflection",
            triggerOnce = true
        }
    },
    {
        name = "asriel_goodbye",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_asriel_goodbye",
            triggerOnce = true
        }
    },
    {
        name = "flowey_redemption",
        data = {
            width = 16,
            height = 16,
            cutsceneId = "ch17_flowey_redemption",
            triggerOnce = true
        }
    }
}

chapter17CutsceneTrigger.fieldInformation = {
    cutsceneId = {
        fieldType = "string",
        options = {
            "ch17_descent_begins",
            "ch17_leaving_zantas",
            "ch17_credits_introduction",
            "ch17_thanking_companions",
            "ch17_remembering_journey",
            "ch17_crossing_barrier",
            "ch17_upper_slopes",
            "ch17_crystal_caves",
            "ch17_forest_path",
            "ch17_valley_crossing",
            "ch17_granny_reunion",
            "ch17_theo_reunion",
            "ch17_oshiro_return",
            "ch17_badeline_reflection",
            "ch17_celeste_base",
            "ch17_final_farewells",
            "ch17_home_arrival",
            "ch17_credits_finale",
            "ch17_kirby_farewell",
            "ch17_chara_reflection",
            "ch17_asriel_goodbye",
            "ch17_flowey_redemption"
        }
    },
    triggerOnce = {
        fieldType = "boolean"
    }
}

function chapter17CutsceneTrigger.texture(room, entity)
    return "ahorn/entityTrigger"
end

function chapter17CutsceneTrigger.color(room, entity)
    -- Soft golden color for epilogue/credits triggers
    return {0.9, 0.8, 0.4, 0.7}
end

return chapter17CutsceneTrigger
