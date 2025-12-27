local npcDialogKeys = require("Ingeste.npcDialogKeys")

local trigger = {
    name = "Ingeste/NPCEventTrigger",
    placements = {
        {
            name = "generic_event",
            data = {
                npcEntityId = "",
                eventType = "Trigger",
                dialog = "",
                animation = "idle",
                oneUse = false
            }
        },
        -- Generic NPCs
        {
            name = "theo_event",
            data = {
                npcEntityId = "theo",
                eventType = "Trigger",
                dialog = "CH0_THEO_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "chara_event",
            data = {
                npcEntityId = "chara",
                eventType = "Trigger",
                dialog = "CH0_CHARA_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "kirby_event",
            data = {
                npcEntityId = "kirby",
                eventType = "Trigger",
                dialog = "CH0_KIRBY_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ralsei_event",
            data = {
                npcEntityId = "ralsei",
                eventType = "Trigger",
                dialog = "CH0_RALSEI_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "metaknight_event",
            data = {
                npcEntityId = "metaknight",
                eventType = "Trigger",
                dialog = "CH0_METAKNIGHT_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "digitalguide_event",
            data = {
                npcEntityId = "digitalguide",
                eventType = "Trigger",
                dialog = "CH0_DIGITALGUIDE_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "phone_event",
            data = {
                npcEntityId = "phone",
                eventType = "Trigger",
                dialog = "CH0_PHONE_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "roxus_event",
            data = {
                npcEntityId = "roxus",
                eventType = "Trigger",
                dialog = "CH0_ROXUS_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "temmie_event",
            data = {
                npcEntityId = "temmie",
                eventType = "Trigger",
                dialog = "CH0_TEMMIE_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "axis_event",
            data = {
                npcEntityId = "axis",
                eventType = "Trigger",
                dialog = "CH0_AXIS_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "els_event",
            data = {
                npcEntityId = "els",
                eventType = "Trigger",
                dialog = "CH0_ELS_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "titancouncil_event",
            data = {
                npcEntityId = "titancouncil",
                eventType = "Trigger",
                dialog = "CH0_TITANCOUNCIL_EVENT",
                animation = "idle",
                oneUse = false
            }
        },
        -- Chapter-specific NPCs
        {
            name = "ch0_theo_event",
            data = {
                npcEntityId = "ch0_theo",
                eventType = "Trigger",
                dialog = "CH0_THEO_CHAPTER",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch1_maggy_event",
            data = {
                npcEntityId = "ch1_maggy",
                eventType = "Trigger",
                dialog = "CH1_MAGGY_CHAPTER",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch2_maggy_event",
            data = {
                npcEntityId = "ch2_maggy",
                eventType = "Trigger",
                dialog = "CH2_MAGGY_CHAPTER",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch3_maggy_event",
            data = {
                npcEntityId = "ch3_maggy",
                eventType = "Trigger",
                dialog = "CH3_MAGGY_CHAPTER",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch3_theo_event",
            data = {
                npcEntityId = "ch3_theo",
                eventType = "Trigger",
                dialog = "CH3_THEO_CHAPTER",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch5_magolor_vents_event",
            data = {
                npcEntityId = "ch5_magolor_vents",
                eventType = "Trigger",
                dialog = "CH5_MAGOLOR_VENTS",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch5_magolor_escape_event",
            data = {
                npcEntityId = "ch5_magolor_escape",
                eventType = "Trigger",
                dialog = "CH5_MAGOLOR_ESCAPE",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch5_oshiro_breakdown_event",
            data = {
                npcEntityId = "ch5_oshiro_breakdown",
                eventType = "Trigger",
                dialog = "CH5_OSHIRO_BREAKDOWN",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch5_oshiro_clutter_event",
            data = {
                npcEntityId = "ch5_oshiro_clutter",
                eventType = "Trigger",
                dialog = "CH5_OSHIRO_CLUTTER",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch5_oshiro_hallway1_event",
            data = {
                npcEntityId = "ch5_oshiro_hallway1",
                eventType = "Trigger",
                dialog = "CH5_OSHIRO_HALLWAY1",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch5_oshiro_hallway2_event",
            data = {
                npcEntityId = "ch5_oshiro_hallway2",
                eventType = "Trigger",
                dialog = "CH5_OSHIRO_HALLWAY2",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch5_oshiro_lobby_event",
            data = {
                npcEntityId = "ch5_oshiro_lobby",
                eventType = "Trigger",
                dialog = "CH5_OSHIRO_LOBBY",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch5_oshiro_rooftop_event",
            data = {
                npcEntityId = "ch5_oshiro_rooftop",
                eventType = "Trigger",
                dialog = "CH5_OSHIRO_ROOFTOP",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch5_oshiro_suite_event",
            data = {
                npcEntityId = "ch5_oshiro_suite",
                eventType = "Trigger",
                dialog = "CH5_OSHIRO_SUITE",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch6_magolor_event",
            data = {
                npcEntityId = "ch6_magolor",
                eventType = "Trigger",
                dialog = "CH6_MAGOLOR",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch6_theo_event",
            data = {
                npcEntityId = "ch6_theo",
                eventType = "Trigger",
                dialog = "CH6_THEO",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch7_chara_event",
            data = {
                npcEntityId = "ch7_chara",
                eventType = "Trigger",
                dialog = "CH7_CHARA",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch7_maddy_mirror_event",
            data = {
                npcEntityId = "ch7_maddy_mirror",
                eventType = "Trigger",
                dialog = "CH7_MADDY_MIRROR",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch8_chara_crying_event",
            data = {
                npcEntityId = "ch8_chara_crying",
                eventType = "Trigger",
                dialog = "CH8_CHARA_CRYING",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch8_maddy_theo_ending_event",
            data = {
                npcEntityId = "ch8_maddy_theo_ending",
                eventType = "Trigger",
                dialog = "CH8_MADDY_THEO_ENDING",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch8_madeline_plateau_event",
            data = {
                npcEntityId = "ch8_madeline_plateau",
                eventType = "Trigger",
                dialog = "CH8_MADELINE_PLATEAU",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch8_maggy_ending_event",
            data = {
                npcEntityId = "ch8_maggy_ending",
                eventType = "Trigger",
                dialog = "CH8_MAGGY_ENDING",
                animation = "idle",
                oneUse = false
            }
        },
        -- Chapter 9 Fake Save Point Events
        {
            name = "ch9_fakesavepoint_event",
            data = {
                npcEntityId = "ch9_fakesavepoint",
                eventType = "Trigger",
                dialog = "CH9_SPACE_SAVE_FILEA",
                animation = "spin",
                oneUse = false
            }
        },
        {
            name = "ch9_fakesavepoint_stagea_event",
            data = {
                npcEntityId = "ch9_fakesavepoint_a",
                eventType = "Trigger",
                dialog = "CH9_SPACE_SAVE_FILEA",
                animation = "spin",
                oneUse = true
            }
        },
        {
            name = "ch9_fakesavepoint_stageb_event",
            data = {
                npcEntityId = "ch9_fakesavepoint_b",
                eventType = "Trigger",
                dialog = "CH9_SPACE_SAVE_FILEB",
                animation = "spin",
                oneUse = true
            }
        },
        {
            name = "ch9_fakesavepoint_stagec_event",
            data = {
                npcEntityId = "ch9_fakesavepoint_c",
                eventType = "Trigger",
                dialog = "CH9_SPACE_SAVE_FILEC",
                animation = "spin",
                oneUse = true
            }
        },
        {
            name = "ch9_fakesavepoint_staged_event",
            data = {
                npcEntityId = "ch9_fakesavepoint_d",
                eventType = "Trigger",
                dialog = "CH9_SPACE_SAVE_FILED",
                animation = "spin",
                oneUse = true
            }
        },
        {
            name = "ch9_fakesavepoint_pretrap_event",
            data = {
                npcEntityId = "ch9_fakesavepoint_pretrap",
                eventType = "Trigger",
                dialog = "CH9_SPACE_SAVE_FILEPRETRAP",
                animation = "spin",
                oneUse = true
            }
        },
        {
            name = "ch9_fakesavepoint_trap_event",
            data = {
                npcEntityId = "ch9_fakesavepoint_trap",
                eventType = "Trigger",
                dialog = "CH9_SPACE_TRAP_SAVE_FILE",
                animation = "activate",
                oneUse = true
            }
        },
        {
            name = "ch9_madeline_freakout_event",
            data = {
                npcEntityId = "ch9_madeline_freakout",
                eventType = "Trigger",
                dialog = "CH9_MADELINE_FREAKOUT",
                animation = "panic",
                oneUse = true
            }
        },
        {
            name = "ch17_kirby_event",
            data = {
                npcEntityId = "ch17_kirby",
                eventType = "Trigger",
                dialog = "CH17_KIRBY",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch17_oshiro_event",
            data = {
                npcEntityId = "ch17_oshiro",
                eventType = "Trigger",
                dialog = "CH17_OSHIRO",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch17_ralsei_event",
            data = {
                npcEntityId = "ch17_ralsei",
                eventType = "Trigger",
                dialog = "CH17_RALSEI",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch17_theo_event",
            data = {
                npcEntityId = "ch17_theo",
                eventType = "Trigger",
                dialog = "CH17_THEO",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch17_toriel_event",
            data = {
                npcEntityId = "ch17_toriel",
                eventType = "Trigger",
                dialog = "CH17_TORIEL",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch18_toriel_inside_event",
            data = {
                npcEntityId = "ch18_toriel_inside",
                eventType = "Trigger",
                dialog = "CH18_TORIEL_INSIDE",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch18_toriel_outside_event",
            data = {
                npcEntityId = "ch18_toriel_outside",
                eventType = "Trigger",
                dialog = "CH18_TORIEL_OUTSIDE",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch19_gravestone_event",
            data = {
                npcEntityId = "ch19_gravestone",
                eventType = "Trigger",
                dialog = "CH19_GRAVESTONE",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch19_maggy_loop_event",
            data = {
                npcEntityId = "ch19_maggy_loop",
                eventType = "Trigger",
                dialog = "CH19_MAGGY_LOOP",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch20_asriel_event",
            data = {
                npcEntityId = "ch20_asriel",
                eventType = "Trigger",
                dialog = "CH20_ASRIEL",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch20_granny_event",
            data = {
                npcEntityId = "ch20_granny",
                eventType = "Trigger",
                dialog = "CH20_GRANNY",
                animation = "idle",
                oneUse = false
            }
        },
        {
            name = "ch20_madeline_event",
            data = {
                npcEntityId = "ch20_madeline",
                eventType = "Trigger",
                dialog = "CH20_MADELINE",
                animation = "idle",
                oneUse = false
            }
        }
    },
    fieldInformation = {
        npcEntityId = {
            fieldType = "string"
        },
        eventType = {
            fieldType = "string",
            options = {"Trigger", "Move", "Talk"}
        },
        dialog = {
            fieldType = "string"
        },
        animation = {
            fieldType = "string",
            options = {"idle", "walk", "run", "jump", "usePhone"}
        },
        oneUse = {
            fieldType = "boolean"
        }
    }
}

return trigger
