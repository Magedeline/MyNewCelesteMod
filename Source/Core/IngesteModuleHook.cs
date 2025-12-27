using DesoloZantas.Core.Core.Cutscenes;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// IngesteModule-specific hooks for managing cutscenes, events, and transitions
    /// Maps chapter IDs to their associated content:
    /// - Chapter 0: Vessel Creation, Intro, Bridge, Outro
    /// - Chapter 3: Postcard Intro and Outro
    /// - Chapter 10: Falling Down Intro
    /// - Chapter 17: Epilogue
    /// - Chapter 18: Intro "1 Month Later"
    /// - Maggy/Main: Vessel Creation and Intro Vignettes
    /// </summary>
    public static class IngesteModuleHook
    {
        private static bool hooksRegistered = false;

        public static void Register()
        {
            if (hooksRegistered)
            {
                IngesteLogger.Warn("IngesteModuleHook already registered, skipping");
                return;
            }

            try
            {
                IngesteLogger.Info("Registering IngesteModule hooks");

                On.Celeste.Level.LoadLevel += OnLevelLoad;
                On.Celeste.Level.Begin += OnLevelBegin;
                On.Celeste.Level.TransitionTo += OnLevelTransition;

                hooksRegistered = true;
                IngesteLogger.Info("IngesteModule hooks registered successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to register IngesteModule hooks");
            }
        }

        public static void Unregister()
        {
            if (!hooksRegistered)
            {
                return;
            }

            try
            {
                IngesteLogger.Info("Unregistering IngesteModule hooks");

                On.Celeste.Level.LoadLevel -= OnLevelLoad;
                On.Celeste.Level.Begin -= OnLevelBegin;
                On.Celeste.Level.TransitionTo -= OnLevelTransition;

                hooksRegistered = false;
                IngesteLogger.Info("IngesteModule hooks unregistered successfully");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Failed to unregister IngesteModule hooks");
            }
        }

        #region Hook Handlers

        private static void OnLevelLoad(On.Celeste.Level.orig_LoadLevel orig, Level self, global::Celeste.Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(self, playerIntro, isFromLoader);

            try
            {
                // Check if this is a DesoloZantas map
                string sid = self.Session.Area.GetSID();
                if (string.IsNullOrEmpty(sid) || !sid.StartsWith("DesoloZantas/"))
                    return;

                // Handle Maggy/Main map separately
                if (sid == "DesoloZantas/Maps/Maggy/Main")
                {
                    HandleMaggyMainLoad(self, self.Session.Level, playerIntro);
                    return;
                }

                int chapterID = self.Session.Area.ID;
                string roomName = self.Session.Level;

                IngesteLogger.Debug($"Chapter {chapterID} loaded, room: {roomName}");

                switch (chapterID)
                {
                    case 0:
                        HandleChapter0Load(self, roomName, playerIntro);
                        break;
                    case 3:
                        HandleChapter3Load(self, roomName, playerIntro);
                        break;
                    case 10:
                        HandleChapter10Load(self, roomName, playerIntro);
                        break;
                    case 17:
                        HandleChapter17Load(self, roomName, playerIntro);
                        break;
                    case 18:
                        HandleChapter18Load(self, roomName, playerIntro);
                        break;
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, $"Error in IngesteModuleHook.OnLevelLoad");
            }
        }

        private static void OnLevelBegin(On.Celeste.Level.orig_Begin orig, Level self)
        {
            orig(self);

            try
            {
                string sid = self.Session.Area.GetSID();
                if (string.IsNullOrEmpty(sid) || !sid.StartsWith("DesoloZantas/"))
                    return;

                // Handle Maggy/Main map separately
                if (sid == "DesoloZantas/Maps/Maggy/Main")
                {
                    HandleMaggyMainBegin(self);
                    return;
                }

                int chapterID = self.Session.Area.ID;
                
                switch (chapterID)
                {
                    case 0:
                        HandleChapter0Begin(self);
                        break;
                    case 3:
                        HandleChapter3Begin(self);
                        break;
                    case 10:
                        HandleChapter10Begin(self);
                        break;
                    case 17:
                        HandleChapter17Begin(self);
                        break;
                    case 18:
                        HandleChapter18Begin(self);
                        break;
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Error in IngesteModuleHook.OnLevelBegin");
            }
        }

        private static void OnLevelTransition(On.Celeste.Level.orig_TransitionTo orig, Level self, LevelData next, Vector2 direction)
        {
            orig(self, next, direction);

            try
            {
                string sid = self.Session.Area.GetSID();
                if (string.IsNullOrEmpty(sid) || !sid.StartsWith("DesoloZantas/"))
                    return;

                // Handle Maggy/Main map separately
                if (sid == "DesoloZantas/Maps/Maggy/Main")
                {
                    HandleMaggyMainTransition(self, next);
                    return;
                }

                int chapterID = self.Session.Area.ID;
                
                switch (chapterID)
                {
                    case 0:
                        HandleChapter0Transition(self, next);
                        break;
                    case 10:
                        HandleChapter10Transition(self, next);
                        break;
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Error(ex, "Error in IngesteModuleHook.OnLevelTransition");
            }
        }

        #endregion

        #region Maggy/Main Map: Vessel Creation & Intro Vignettes

        private static void HandleMaggyMainLoad(Level level, string roomName, global::Celeste.Player.IntroTypes playerIntro)
        {
            IngesteLogger.Info($"Maggy/Main map loaded, room: {roomName}");

            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player == null)
                return;

            // Check if this is the first room or spawn room
            bool isSpawnRoom = roomName.Contains("start") || roomName.Contains("spawn") || roomName == "lvl-0";

            if (isSpawnRoom)
            {
                // First priority: Vessel Creation
                if (!level.Session.GetFlag("maggy_vessel_created"))
                {
                    IngesteLogger.Info("Maggy/Main: Starting Vessel Creation Vignette");
                    level.Session.SetFlag("maggy_vessel_created", true);
                    Engine.Scene = new VesselCreationVignette(level.Session);
                    return;
                }

                // Second priority: Intro Vignette (plays after vessel creation)
                if (!level.Session.GetFlag("maggy_intro_complete"))
                {
                    IngesteLogger.Info("Maggy/Main: Starting Intro Vignette");
                    level.Session.SetFlag("maggy_intro_complete", true);
                    Engine.Scene = new Cs00IntroVignette(level.Session);
                    return;
                }
            }
        }

        private static void HandleMaggyMainBegin(Level level)
        {
            level.Session.SetFlag("maggy_main_active");
            IngesteLogger.Info("Maggy/Main map begun");
        }

        private static void HandleMaggyMainTransition(Level level, LevelData next)
        {
            // Add any transition handling for Maggy/Main if needed
            IngesteLogger.Debug($"Maggy/Main transition: {level.Session.Level} -> {next.Name}");
        }

        #endregion

        #region Chapter 0: Vessel Creation, Intro, Bridge, Outro

        private static void HandleChapter0Load(Level level, string roomName, global::Celeste.Player.IntroTypes playerIntro)
        {
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player == null)
                return;

            // Vessel Creation room and Intro room (both on lvl-0_pr)
            if (roomName == "lvl-0_pr")
            {
                // First check vessel creation
                if (!level.Session.GetFlag("ch0_vessel_created"))
                {
                    IngesteLogger.Info("Chapter 0: Vessel Creation room detected (lvl-0_pr)");
                    // Launch vessel creation vignette
                    Engine.Scene = new VesselCreationCutscene(new EntityData(), Vector2.Zero).Scene ?? level;
                    return;
                }
                
                // Then check intro
                if (!level.Session.GetFlag("ch0_intro_complete"))
                {
                    IngesteLogger.Info("Chapter 0: Intro room detected (lvl-0_pr), starting intro vignette");
                    level.Session.SetFlag("ch0_intro_complete");
                    Engine.Scene = new Cs00IntroVignette(level.Session);
                    return;
                }
            }

            // Bridge room
            if (roomName.Contains("bridge") && !level.Session.GetFlag("ch0_bridge_complete"))
            {
                IngesteLogger.Info("Chapter 0: Bridge room detected");
                // Bridge is handled by the Bridge entity
                // Music changes when player starts on bridge
            }

            // Outro room (ending) on lvl-3_pr - Use Cs00BridgeEndingVignette
            if (roomName == "lvl-3_pr")
            {
                if (!level.Session.GetFlag("ch0_ending_complete"))
                {
                    IngesteLogger.Info("Chapter 0: Ending room detected (lvl-3_pr), starting bridge ending vignette");
                    level.Session.SetFlag("ch0_ending_complete");
                    Engine.Scene = new Cs00BridgeEndingVignette(level.Session);
                }
            }
        }

        private static void HandleChapter0Begin(Level level)
        {
            // Set chapter-specific settings
            level.Session.SetFlag("ch0_active");
            
            // Check for Theo cutscene
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player != null && level.Session.Level.Contains("theo"))
            {
                if (!level.Session.GetFlag(Cs00Theo.FLAG))
                {
                    IngesteLogger.Info("Chapter 0: Starting Theo cutscene");
                    level.Add(new Cs00Theo(player));
                }
            }
        }

        private static void HandleChapter0Transition(Level level, LevelData next)
        {
            string currentRoom = level.Session.Level;
            string nextRoom = next.Name;

            // Check if transitioning from bridge to ending (lvl-3_pr)
            if (currentRoom.Contains("bridge") && nextRoom == "lvl-3_pr")
            {
                level.Session.SetFlag("ch0_bridge_complete");
                
                // Stop bridge music
                var bridge = level.Tracker.GetEntity<CelesteBridge>();
                bridge?.StopCollapseLoop();
                
                // Transition to ending vignette
                IngesteLogger.Info("Chapter 0: Transitioning to bridge ending vignette (lvl-3_pr)");
                level.Session.SetFlag("ch0_ending_complete");
                Engine.Scene = new Cs00BridgeEndingVignette(level.Session);
            }
        }

        #endregion

        #region Chapter 3: Postcard Intro and Outro

        private static void HandleChapter3Load(Level level, string roomName, global::Celeste.Player.IntroTypes playerIntro)
        {
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player == null)
                return;

            // Postcard Intro room - Use Vignette (trigger on level "a-2")
            if (roomName == "a-2" && !level.Session.GetFlag("ch3_intro_complete"))
            {
                IngesteLogger.Info("Chapter 3: Postcard intro room detected (a-2), starting intro vignette");
                level.Session.SetFlag("ch3_intro_complete");
                Engine.Scene = new Cs03IntroVignette(level.Session);
                return;
            }

            // Check for outro room (Postcard ending) - Use Vignette (trigger on level "lvl_end")
            if (roomName == "lvl_end" && !level.Session.GetFlag("ch3_outro_complete"))
            {
                IngesteLogger.Info("Chapter 3: Postcard outro room detected (lvl_end), starting outro vignette");
                level.Session.SetFlag("ch3_outro_complete");
                Engine.Scene = new Cs03OutroVignette(level.Session);
                return;
            }
        }

        private static void HandleChapter3Begin(Level level)
        {
            level.Session.SetFlag("ch3_active");
            
            IngesteLogger.Info("Chapter 3: Postcard chapter begun");
        }

        #endregion

        #region Chapter 10: Falling Down Intro

        private static void HandleChapter10Load(Level level, string roomName, global::Celeste.Player.IntroTypes playerIntro)
        {
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player == null)
                return;

            // Check if this is the start room (intro) - Use Vignette
            if (roomName == "lvl_start" && !level.Session.GetFlag("ch10_intro_vignette_played"))
            {
                IngesteLogger.Info("Chapter 10: Starting falling down intro vignette (lvl_start)");
                level.Session.SetFlag("ch10_intro_vignette_played");
                Engine.Scene = new Cs10IntroVignetteAlt(level.Session);
            }
            // After vignette, spawn NPCs if needed
            else if (roomName == "lvl_start" && !level.Session.GetFlag("ch10_maddy_baddy_chara_intro_played"))
            {
                IngesteLogger.Info("Chapter 10: Starting NPC intro cutscene");
                level.Add(new CS10_MaddyBaddyCharaIntro(player, roomName));
            }
        }

        private static void HandleChapter10Begin(Level level)
        {
            level.Session.SetFlag("ch10_active");
        }

        private static void HandleChapter10Transition(Level level, LevelData next)
        {
            // Check if player is leaving the start room
            string currentRoom = level.Session.Level;

            if (currentRoom == "lvl_start" && level.Session.GetFlag("ch10_badeline_chara_present"))
            {
                IngesteLogger.Info("Chapter 10: Player left start room, removing NPCs");
                CS10_MaddyBaddyCharaIntro.RemoveNPCsOnRoomTransition(level);
            }
        }

        #endregion

        #region Chapter 17: Epilogue

        private static void HandleChapter17Load(Level level, string roomName, global::Celeste.Player.IntroTypes playerIntro)
        {
            IngesteLogger.Info("Chapter 17: Epilogue chapter loaded");
            
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player == null)
                return;

            // Check for ending room - Use Vignette (trigger on level "lvl_inside")
            if (roomName == "lvl_inside" && !level.Session.GetFlag("ch17_ending_started"))
            {
                IngesteLogger.Info("Chapter 17: Starting epilogue ending vignette (lvl_inside)");
                level.Session.SetFlag("ch17_ending_started");
                Engine.Scene = new Cs17EpilogueEndingVignette(level.Session);
                return;
            }

            // Check for credits trigger (still uses CS17_Credits entity)
            if (roomName.Contains("credits"))
            {
                if (!level.Session.GetFlag("ch17_credits_started"))
                {
                    IngesteLogger.Info("Chapter 17: Starting credits");
                    level.Session.SetFlag("ch17_credits_started");
                    level.Add(new CS17_Credits());
                }
            }
        }

        private static void HandleChapter17Begin(Level level)
        {
            level.Session.SetFlag("ch17_active");
            level.Session.SetFlag("epilogue_active");
            
            IngesteLogger.Info("Chapter 17: Epilogue chapter begun");
        }

        #endregion

        #region Chapter 18: Intro "1 Month Later"

        private static void HandleChapter18Load(Level level, string roomName, global::Celeste.Player.IntroTypes playerIntro)
        {
            IngesteLogger.Info($"Chapter 18: 1 Month Later chapter loaded, room: {roomName}");
            
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player == null)
                return;

            // Check for intro room (lvl_00)
            if (roomName == "lvl_00" && !level.Session.GetFlag("ch18_intro_complete"))
            {
                IngesteLogger.Info("Chapter 18: Starting '1 Month Later' intro vignette (lvl_00)");
                level.Session.SetFlag("ch18_intro_complete");
                Engine.Scene = new Cs18IntroVignette(level.Session);
                return;
            }

            // Check for outro room (the phone call ending) - Use Vignette
            if (roomName.Contains("outro") || roomName.Contains("ending") || roomName.Contains("phone"))
            {
                if (!level.Session.GetFlag("ch18_outro_started"))
                {
                    IngesteLogger.Info("Chapter 18: Starting outro vignette (phone call)");
                    level.Session.SetFlag("ch18_outro_started");
                    Engine.Scene = new Cs18OutroVignette(level.Session);
                }
            }
        }

        private static void HandleChapter18Begin(Level level)
        {
            level.Session.SetFlag("ch18_active");
            level.Session.SetFlag("one_month_later");
            
            // Display "1 Month Later" title card
            if (!level.Session.GetFlag("ch18_title_shown"))
            {
                level.Session.SetFlag("ch18_title_shown");
                IngesteLogger.Info("Chapter 18: Showing '1 Month Later' title");
                
                // You could add a custom title card entity here
                // level.Add(new OneMonthLaterTitle());
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Check if a chapter is a DesoloZantas chapter
        /// </summary>
        public static bool IsDesoloZantasChapter(AreaKey area)
        {
            return area.GetSID()?.StartsWith("DesoloZantas/") == true;
        }

        /// <summary>
        /// Get the chapter ID for special handling
        /// </summary>
        public static int? GetChapterID(Session session)
        {
            if (session?.Area.GetSID()?.StartsWith("DesoloZantas/") != true)
                return null;

            return session.Area.ID;
        }

        /// <summary>
        /// Check if this is a story chapter that requires special handling
        /// </summary>
        public static bool IsStoryChapter(int chapterID)
        {
            return chapterID switch
            {
                0 => true,   // Vessel Creation & Intro
                3 => true,   // Postcard Intro & Outro
                10 => true,  // Falling Down
                17 => true,  // Epilogue
                18 => true,  // 1 Month Later
                _ => false
            };
        }

        #endregion
    }
}




