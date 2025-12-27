local npcSessionFlags = require("Ingeste.npcSessionFlags")

local trigger = {
    name = "Ingeste/NPCInteractTrigger",
    placements = {
        {
            name = "basic_interaction",
            data = {
                npcName = "theo",
                dialog = "",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        -- Generic NPC Interactions
        {
            name = "theo_interaction",
            data = {
                npcName = "theo",
                dialog = "CH0_THEO_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "chara_interaction",
            data = {
                npcName = "chara",
                dialog = "CH0_CHARA_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "kirby_interaction",
            data = {
                npcName = "kirby",
                dialog = "CH0_KIRBY_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ralsei_interaction",
            data = {
                npcName = "ralsei",
                dialog = "CH0_RALSEI_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "metaknight_interaction",
            data = {
                npcName = "metaknight",
                dialog = "CH0_METAKNIGHT_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "digitalguide_interaction",
            data = {
                npcName = "digitalguide",
                dialog = "CH0_DIGITALGUIDE_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "phone_interaction",
            data = {
                npcName = "phone",
                dialog = "CH0_PHONE_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "roxus_interaction",
            data = {
                npcName = "roxus",
                dialog = "CH0_ROXUS_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "temmie_interaction",
            data = {
                npcName = "temmie",
                dialog = "CH0_TEMMIE_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "axis_interaction",
            data = {
                npcName = "axis",
                dialog = "CH0_AXIS_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "els_interaction",
            data = {
                npcName = "els",
                dialog = "CH0_ELS_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "titancouncil_interaction",
            data = {
                npcName = "titancouncil",
                dialog = "CH0_TITANCOUNCIL_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        -- Chapter-specific NPC Interactions
        {
            name = "ch0_theo_interaction",
            data = {
                npcName = "ch0_theo",
                dialog = "CH0_THEO_CHAPTER_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch1_maggy_interaction",
            data = {
                npcName = "ch1_maggy",
                dialog = "CH1_MAGGY_CHAPTER_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch2_maggy_interaction",
            data = {
                npcName = "ch2_maggy",
                dialog = "CH2_MAGGY_CHAPTER_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch3_maggy_interaction",
            data = {
                npcName = "ch3_maggy",
                dialog = "CH3_MAGGY_CHAPTER_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch3_theo_interaction",
            data = {
                npcName = "ch3_theo",
                dialog = "CH3_THEO_CHAPTER_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch5_magolor_vents_interaction",
            data = {
                npcName = "ch5_magolor_vents",
                dialog = "CH5_MAGOLOR_VENTS_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch5_magolor_escape_interaction",
            data = {
                npcName = "ch5_magolor_escape",
                dialog = "CH5_MAGOLOR_ESCAPE_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch5_oshiro_breakdown_interaction",
            data = {
                npcName = "ch5_oshiro_breakdown",
                dialog = "CH5_OSHIRO_BREAKDOWN_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch5_oshiro_clutter_interaction",
            data = {
                npcName = "ch5_oshiro_clutter",
                dialog = "CH5_OSHIRO_CLUTTER_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch5_oshiro_hallway1_interaction",
            data = {
                npcName = "ch5_oshiro_hallway1",
                dialog = "CH5_OSHIRO_HALLWAY1_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch5_oshiro_hallway2_interaction",
            data = {
                npcName = "ch5_oshiro_hallway2",
                dialog = "CH5_OSHIRO_HALLWAY2_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch5_oshiro_lobby_interaction",
            data = {
                npcName = "ch5_oshiro_lobby",
                dialog = "CH5_OSHIRO_LOBBY_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch5_oshiro_rooftop_interaction",
            data = {
                npcName = "ch5_oshiro_rooftop",
                dialog = "CH5_OSHIRO_ROOFTOP_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch5_oshiro_suite_interaction",
            data = {
                npcName = "ch5_oshiro_suite",
                dialog = "CH5_OSHIRO_SUITE_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch6_magolor_interaction",
            data = {
                npcName = "ch6_magolor",
                dialog = "CH6_MAGOLOR_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch6_theo_interaction",
            data = {
                npcName = "ch6_theo",
                dialog = "CH6_THEO_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch7_chara_interaction",
            data = {
                npcName = "ch7_chara",
                dialog = "CH7_CHARA_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch7_maddy_mirror_interaction",
            data = {
                npcName = "ch7_maddy_mirror",
                dialog = "CH7_MADDY_MIRROR_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch8_chara_crying_interaction",
            data = {
                npcName = "ch8_chara_crying",
                dialog = "CH8_CHARA_CRYING_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch8_maddy_theo_ending_interaction",
            data = {
                npcName = "ch8_maddy_theo_ending",
                dialog = "CH8_MADDY_THEO_ENDING_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch8_madeline_plateau_interaction",
            data = {
                npcName = "ch8_madeline_plateau",
                dialog = "CH8_MADELINE_PLATEAU_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch8_maggy_ending_interaction",
            data = {
                npcName = "ch8_maggy_ending",
                dialog = "CH8_MAGGY_ENDING_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        -- Chapter 9 Fake Save Point Interactions
        {
            name = "ch9_fakesavepoint_interaction",
            data = {
                npcName = "ch9_fakesavepoint",
                dialog = "CH9_SPACE_SAVE_FILEA",
                flagToSet = "ch9_fakesave_stage_a",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch9_fakesavepoint_stagea_interaction",
            data = {
                npcName = "ch9_fakesavepoint_a",
                dialog = "CH9_SPACE_SAVE_FILEA",
                flagToSet = "ch9_fakesave_stage_a",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = true
            }
        },
        {
            name = "ch9_fakesavepoint_stageb_interaction",
            data = {
                npcName = "ch9_fakesavepoint_b",
                dialog = "CH9_SPACE_SAVE_FILEB",
                flagToSet = "ch9_fakesave_stage_b",
                requiredFlag = "ch9_fakesave_stage_a",
                removeNpcAfter = false,
                oneTimeUse = true
            }
        },
        {
            name = "ch9_fakesavepoint_stagec_interaction",
            data = {
                npcName = "ch9_fakesavepoint_c",
                dialog = "CH9_SPACE_SAVE_FILEC",
                flagToSet = "ch9_fakesave_stage_c",
                requiredFlag = "ch9_fakesave_stage_b",
                removeNpcAfter = false,
                oneTimeUse = true
            }
        },
        {
            name = "ch9_fakesavepoint_staged_interaction",
            data = {
                npcName = "ch9_fakesavepoint_d",
                dialog = "CH9_SPACE_SAVE_FILED",
                flagToSet = "ch9_fakesave_stage_d",
                requiredFlag = "ch9_fakesave_stage_c",
                removeNpcAfter = false,
                oneTimeUse = true
            }
        },
        {
            name = "ch9_fakesavepoint_pretrap_interaction",
            data = {
                npcName = "ch9_fakesavepoint_pretrap",
                dialog = "CH9_SPACE_SAVE_FILEPRETRAP",
                flagToSet = "ch9_fakesave_stage_e",
                requiredFlag = "ch9_fakesave_stage_d",
                removeNpcAfter = false,
                oneTimeUse = true
            }
        },
        {
            name = "ch9_fakesavepoint_trap_interaction",
            data = {
                npcName = "ch9_fakesavepoint_trap",
                dialog = "CH9_SPACE_TRAP_SAVE_FILE",
                flagToSet = "ch9_fakesave_trap",
                requiredFlag = "ch9_fakesave_stage_e",
                removeNpcAfter = true,
                oneTimeUse = true
            }
        },
        {
            name = "ch9_madeline_freakout_interaction",
            data = {
                npcName = "ch9_madeline_freakout",
                dialog = "CH9_MADELINE_FREAKOUT",
                flagToSet = "ch9_madeline_freakout_done",
                requiredFlag = "ch9_fakesave_trap",
                removeNpcAfter = false,
                oneTimeUse = true
            }
        },
        {
            name = "ch17_kirby_interaction",
            data = {
                npcName = "ch17_kirby",
                dialog = "CH17_KIRBY_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch17_oshiro_interaction",
            data = {
                npcName = "ch17_oshiro",
                dialog = "CH17_OSHIRO_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch17_ralsei_interaction",
            data = {
                npcName = "ch17_ralsei",
                dialog = "CH17_RALSEI_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch17_theo_interaction",
            data = {
                npcName = "ch17_theo",
                dialog = "CH17_THEO_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch17_toriel_interaction",
            data = {
                npcName = "ch17_toriel",
                dialog = "CH17_TORIEL_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch18_toriel_inside_interaction",
            data = {
                npcName = "ch18_toriel_inside",
                dialog = "CH18_TORIEL_INSIDE_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch18_toriel_outside_interaction",
            data = {
                npcName = "ch18_toriel_outside",
                dialog = "CH18_TORIEL_OUTSIDE_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch19_gravestone_interaction",
            data = {
                npcName = "ch19_gravestone",
                dialog = "CH19_GRAVESTONE_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch19_maggy_loop_interaction",
            data = {
                npcName = "ch19_maggy_loop",
                dialog = "CH19_MAGGY_LOOP_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch20_asriel_interaction",
            data = {
                npcName = "ch20_asriel",
                dialog = "CH20_ASRIEL_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch20_granny_interaction",
            data = {
                npcName = "ch20_granny",
                dialog = "CH20_GRANNY_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        },
        {
            name = "ch20_madeline_interaction",
            data = {
                npcName = "ch20_madeline",
                dialog = "CH20_MADELINE_INTERACT",
                flagToSet = "",
                requiredFlag = "",
                removeNpcAfter = false,
                oneTimeUse = false
            }
        }
    },
    fieldInformation = {
        npcName = {
            fieldType = "string",
            options = {
                "theo",
                "chara",
                "kirby",
                "ralsei",
                "metaknight",
                "digitalguide",
                "phone",
                "roxus",
                "temmie",
                "axis",
                "els",
                "titancouncil",
                "ch0_theo",
                "ch1_maggy",
                "ch2_maggy",
                "ch3_maggy",
                "ch3_theo",
                "ch5_magolor_vents",
                "ch5_magolor_escape",
                "ch5_oshiro_breakdown",
                "ch5_oshiro_clutter",
                "ch5_oshiro_hallway1",
                "ch5_oshiro_hallway2",
                "ch5_oshiro_lobby",
                "ch5_oshiro_rooftop",
                "ch5_oshiro_suite",
                "ch6_magolor",
                "ch6_theo",
                "ch7_chara",
                "ch7_maddy_mirror",
                "ch8_chara_crying",
                "ch8_maddy_theo_ending",
                "ch8_madeline_plateau",
                "ch8_maggy_ending",
                "ch17_kirby",
                "ch17_oshiro",
                "ch17_ralsei",
                "ch17_theo",
                "ch17_toriel",
                "ch18_toriel_inside",
                "ch18_toriel_outside",
                "ch19_gravestone",
                "ch19_maggy_loop",
                "ch20_asriel",
                "ch20_granny",
                "ch20_madeline"
            }
        },
        dialog = {
            fieldType = "string"
        },
        flagToSet = {
            fieldType = "string"
        },
        requiredFlag = {
            fieldType = "string"
        },
        removeNpcAfter = {
            fieldType = "boolean"
        },
        oneTimeUse = {
            fieldType = "boolean"
        }
    }
}

return trigger
