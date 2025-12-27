using DesoloZantas.Core.Core;
using MonoMod.ModInterop;

namespace DesoloZantas.Core.Cutscenes
{
    /// <summary>
    /// Meta-narrative and troll cutscenes inspired by Undertale/Deltarune
    /// Features fourth-wall breaking, fake crashes, and save file manipulation
    /// </summary>
    public static class MetaTrollCutscenes
    {
        [ModImportName("PrismaticHelper.CutsceneTriggers")]
        public static class PrismaticHelperImports
        {
            public static Action<string, Func<Player, Level, List<string>, IEnumerator>> Register;
        }

        public static void LoadCutscenes()
        {
            if (PrismaticHelperImports.Register != null)
            {
                // Meta troll moments
                PrismaticHelperImports.Register("meta_flowey_takeover", FloweyTakeover);
                PrismaticHelperImports.Register("meta_fake_crash", FakeCrash);
                PrismaticHelperImports.Register("meta_save_file_delete", SaveFileDelete);
                PrismaticHelperImports.Register("meta_game_reload", GameReload);
                PrismaticHelperImports.Register("meta_troll_face_reveal", TrollFaceReveal);
                PrismaticHelperImports.Register("meta_flowey_knows", FloweyKnowsYourSecrets);
                PrismaticHelperImports.Register("meta_reset_timeline", ResetTimeline);
                PrismaticHelperImports.Register("meta_fourth_wall_break", FourthWallBreak);
                PrismaticHelperImports.Register("meta_player_name_call", PlayerNameCall);
                PrismaticHelperImports.Register("meta_death_count", DeathCountReveal);
            }
        }

        #region Flowey Takeover Sequence
        
        private static IEnumerator FloweyTakeover(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("META: Flowey Takeover - Breaking the fourth wall");
            
            player.StateMachine.State = Player.StDummy;
            
            // Normal dialog at first
            yield return Textbox.Say("meta_flowey_start", 
                ShowFloweyFace(level, "evil")
            );
            
            // Glitch effect
            yield return GlitchScreen(level, 0.5f);
            
            // Screen goes black
            yield return FadeToBlack(level, 0.8f);
            yield return 1.5f;
            
            // Flowey's creepy face fills the screen
            yield return ShowFullScreenFlowey(level, "creepy");
            
            // Text appears letter by letter with sound
            yield return Textbox.Say("meta_flowey_takeover_1");
            
            // Multiple glitches
            for (int i = 0; i < 3; i++)
            {
                yield return GlitchScreen(level, 0.2f);
                yield return 0.3f;
            }
            
            yield return Textbox.Say("meta_flowey_takeover_2");
            
            // "Loading" fake save files
            yield return ShowFakeSaveMenu(level);
            yield return 2f;
            
            yield return Textbox.Say("meta_flowey_laugh");
            
            // Return to normal (or is it?)
            yield return FadeFromBlack(level, 1f);
            
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Fake Crash Sequence
        
        private static IEnumerator FakeCrash(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("META: Fake Crash - Trolling the player");
            
            player.StateMachine.State = Player.StDummy;
            
            // Everything seems normal
            yield return 0.5f;
            
            // Sudden screen freeze
            yield return FreezeFrame(level, 1.5f);
            
            // Screen shake violently
            level.Shake(2.5f);
            
            // Glitch effects intensify
            for (int i = 0; i < 5; i++)
            {
                yield return GlitchScreen(level, 0.1f);
                Audio.Play("event:/new_content/game/10_farewell/glitch_short");
                yield return 0.15f;
            }
            
            // Show fake error message
            yield return ShowFakeErrorMessage(level, "FATAL ERROR: DETERMINATION.dll not found");
            yield return 3f;
            
            // Screen goes completely black
            yield return FadeToBlack(level, 0.3f);
            yield return 2f;
            
            // Flowey's laugh echoes
            Audio.Play("event:/char/badeline/laugh");
            yield return 1f;
            
            // Troll face appears
            yield return ShowTrollFace(level);
            yield return 2f;
            
            // Text: "Just kidding!"
            yield return Textbox.Say("meta_just_kidding");
            
            // Game "restarts"
            yield return FadeFromBlack(level, 1f);
            
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Save File Manipulation
        
        private static IEnumerator SaveFileDelete(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("META: Fake Save File Delete");
            
            player.StateMachine.State = Player.StDummy;
            
            yield return Textbox.Say("meta_flowey_savefile_1");
            
            // Show fake file browser
            yield return ShowFakeFileSystem(level);
            yield return 2f;
            
            // "Deleting" animation
            yield return Textbox.Say("meta_flowey_savefile_delete");
            
            for (int i = 0; i < 10; i++)
            {
                Audio.Play("event:/ui/main/button_invalid");
                yield return 0.2f;
            }
            
            // Fake "File deleted" message
            yield return ShowDeletedMessage(level);
            yield return 2f;
            
            // Flowey laughs
            yield return Textbox.Say("meta_flowey_savefile_laugh");
            
            // But actually... nothing was deleted (it's a troll)
            yield return Textbox.Say("meta_flowey_savefile_troll");
            
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Game Reload/Reset Sequences
        
        private static IEnumerator GameReload(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("META: Fake Game Reload");
            
            player.StateMachine.State = Player.StDummy;
            
            // Flowey announces he's going to reload
            yield return Textbox.Say("meta_flowey_reload_announce");
            
            // Screen fades to white
            Glitch.Value = 1f;
            yield return FadeToWhite(level, 0.5f);
            
            // Loading screen appears
            yield return ShowFakeLoadingScreen(level, "LOADING...");
            yield return 3f;
            
            // Multiple "loads"
            for (int i = 0; i < 3; i++)
            {
                yield return FadeToBlack(level, 0.2f);
                yield return 0.3f;
                yield return FadeFromBlack(level, 0.2f);
                Audio.Play("event:/new_content/game/10_farewell/glitch_medium");
                yield return 0.5f;
            }
            
            // Flowey's face appears
            yield return ShowFullScreenFlowey(level, "wink");
            
            yield return Textbox.Say("meta_flowey_reload_complete");
            
            // Return to game
            Glitch.Value = 0f;
            yield return FadeFromWhite(level, 1f);
            
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator ResetTimeline(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("META: Timeline Reset Sequence");
            
            player.StateMachine.State = Player.StDummy;
            
            yield return Textbox.Say("meta_flowey_reset_1");
            
            // Reality starts breaking
            for (int i = 0; i < 8; i++)
            {
                level.Shake(0.3f);
                yield return GlitchScreen(level, 0.15f);
                yield return 0.2f;
            }
            
            yield return Textbox.Say("meta_flowey_reset_2");
            
            // Everything goes white
            yield return WhiteOut(level, 2f);
            
            // Silence
            Audio.SetMusic(null);
            yield return 2f;
            
            // Soft music begins
            Audio.SetMusic("event:/music/lvl0/intro");
            
            yield return Textbox.Say("meta_timeline_reset_complete");
            
            // Fade back in
            yield return FadeFromWhite(level, 2f);
            
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Troll Face Moments
        
        private static IEnumerator TrollFaceReveal(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("META: Troll Face Reveal - Problem?");
            
            player.StateMachine.State = Player.StDummy;
            
            // Build up suspense
            yield return Textbox.Say("meta_troll_buildup");
            
            yield return 1f;
            
            // Dramatic pause
            level.Shake(0.5f);
            yield return 0.5f;
            
            // BOOM - Troll face fills the screen
            yield return ShowTrollFace(level, true); // true = with sound effect
            
            yield return 2f;
            
            // Text: "PROBLEM?"
            yield return ShowProblemText(level);
            
            yield return 2f;
            
            // Return to normal like nothing happened
            yield return FadeOut(level, 0.5f);
            
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Fourth Wall Breaks
        
        private static IEnumerator FourthWallBreak(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("META: Fourth Wall Break");
            
            player.StateMachine.State = Player.StDummy;
            
            yield return Textbox.Say("meta_fourth_wall_1");
            
            // Camera zooms out to show the "game window"
            yield return ZoomOutToWindow(level);
            
            yield return Textbox.Say("meta_fourth_wall_2");
            
            // Characters look at the camera
            if (player != null)
            {
                yield return player.DummyWalkTo(player.X + 10);
                yield return 0.3f;
                yield return player.DummyWalkTo(player.X - 20);
            }
            
            yield return Textbox.Say("meta_fourth_wall_3");
            
            // Camera returns to normal
            yield return ZoomBackIn(level);
            
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator FloweyKnowsYourSecrets(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("META: Flowey Knows Your Secrets");
            
            player.StateMachine.State = Player.StDummy;
            
            // Get player's actual save data
            int deathCount = SaveData.Instance?.TotalDeaths ?? 0;
            
            yield return ShowFloweyFace(level, "knowing");
            
            yield return Textbox.Say("meta_flowey_secrets_1");
            
            // Reveal death count
            yield return Textbox.Say("meta_flowey_death_count", 
                ShowDeathCounter(level, deathCount)
            );
            
            yield return Textbox.Say("meta_flowey_secrets_2");
            
            // More creepy knowledge
            yield return Textbox.Say("meta_flowey_secrets_3");
            
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator PlayerNameCall(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("META: Player Name Call");
            
            player.StateMachine.State = Player.StDummy;
            
            // Get the player's system username (for extra creepy factor)
            string playerName = Environment.UserName;
            
            yield return Textbox.Say("meta_name_call_1");
            
            yield return 0.5f;
            
            // Flowey calls them by their actual name
            yield return Textbox.Say("meta_name_call_reveal", 
                ShowPlayerNameOnScreen(level, playerName)
            );
            
            yield return 1.5f;
            
            yield return Textbox.Say("meta_name_call_2");
            
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator DeathCountReveal(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("META: Death Count Reveal");
            
            player.StateMachine.State = Player.StDummy;
            
            int deaths = SaveData.Instance?.TotalDeaths ?? 0;
            
            yield return Textbox.Say("meta_death_reveal_1");
            
            // Count up animation
            yield return CountUpDeaths(level, deaths);
            
            yield return Textbox.Say("meta_death_reveal_2");
            
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Helper Methods - Visual Effects
        
        private static IEnumerator GlitchScreen(Level level, float duration)
        {
            Glitch.Value = 1f;
            Audio.Play("event:/new_content/game/10_farewell/glitch_short");
            yield return duration;
            Glitch.Value = 0f;
        }

        private static IEnumerator FreezeFrame(Level level, float duration)
        {
            Engine.FreezeTimer = duration;
            yield return duration;
        }

        private static IEnumerator FadeToBlack(Level level, float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Engine.DeltaTime;
                float alpha = Ease.CubeIn(timer / duration);
                level.Lighting.Alpha = alpha;
                yield return null;
            }
            level.Lighting.Alpha = 1f;
        }

        private static IEnumerator FadeFromBlack(Level level, float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Engine.DeltaTime;
                float alpha = 1f - Ease.CubeOut(timer / duration);
                level.Lighting.Alpha = alpha;
                yield return null;
            }
            level.Lighting.Alpha = 0f;
        }

        private static IEnumerator FadeToWhite(Level level, float duration)
        {
            // Implementation for white fade
            yield return duration;
        }

        private static IEnumerator FadeFromWhite(Level level, float duration)
        {
            // Implementation for white fade out
            yield return duration;
        }

        private static IEnumerator WhiteOut(Level level, float duration)
        {
            level.Flash(Color.White, true);
            yield return duration;
        }

        private static IEnumerator FadeOut(Level level, float duration)
        {
            yield return FadeToBlack(level, duration);
        }

        #endregion

        #region Helper Methods - Visual Elements
        
        private static Func<IEnumerator> ShowFloweyFace(Level level, string expression)
        {
            return () => {
                // Show Flowey portrait
                return null;
            };
        }

        private static IEnumerator ShowFullScreenFlowey(Level level, string expression)
        {
            // Display full-screen Flowey image
            IngesteLogger.Info($"Showing full-screen Flowey: {expression}");
            yield return 0.5f;
        }

        private static IEnumerator ShowTrollFace(Level level, bool withSound = false)
        {
            if (withSound)
            {
                Audio.Play("event:/new_content/game/10_farewell/pufferfish_voice");
            }
            
            // Display troll face image
            IngesteLogger.Info("PROBLEM? U MAD GURL?");
            yield return 0.5f;
        }

        private static IEnumerator ShowProblemText(Level level)
        {
            // Display "PROBLEM?" text
            yield return 0.5f;
        }

        private static IEnumerator ShowFakeErrorMessage(Level level, string message)
        {
            IngesteLogger.Info($"Fake Error: {message}");
            // Display fake Windows/game error dialog
            yield return 0.5f;
        }

        private static IEnumerator ShowFakeSaveMenu(Level level)
        {
            IngesteLogger.Info("Showing fake save file menu");
            // Display fake Undertale-style save menu
            yield return 0.5f;
        }

        private static IEnumerator ShowFakeLoadingScreen(Level level, string text)
        {
            IngesteLogger.Info($"Fake loading: {text}");
            // Display fake loading screen
            yield return 0.5f;
        }

        private static IEnumerator ShowFakeFileSystem(Level level)
        {
            IngesteLogger.Info("Showing fake file system");
            // Display fake file browser
            yield return 0.5f;
        }

        private static IEnumerator ShowDeletedMessage(Level level)
        {
            IngesteLogger.Info("File deleted message");
            yield return 0.5f;
        }

        private static Func<IEnumerator> ShowDeathCounter(Level level, int count)
        {
            return () => {
                IngesteLogger.Info($"Deaths: {count}");
                return null;
            };
        }

        private static Func<IEnumerator> ShowPlayerNameOnScreen(Level level, string name)
        {
            return () => {
                IngesteLogger.Info($"Player name: {name}");
                return null;
            };
        }

        private static IEnumerator CountUpDeaths(Level level, int totalDeaths)
        {
            for (int i = 0; i <= totalDeaths; i += Math.Max(1, totalDeaths / 50))
            {
                // Display counting numbers
                Audio.Play("event:/ui/main/button_toggle_on");
                yield return 0.05f;
            }
        }

        private static IEnumerator ZoomOutToWindow(Level level)
        {
            // Zoom camera out to show "game window"
            yield return 1f;
        }

        private static IEnumerator ZoomBackIn(Level level)
        {
            // Return camera to normal
            yield return 1f;
        }

        #endregion
    }
}




