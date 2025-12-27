using DesoloZantas.Core.Core;
using MonoMod.ModInterop;

// For Chapter16 and MetaTroll

namespace DesoloZantas.Core.Cutscenes
{
    public static class CustomCutsceneTriggers
    {
        [ModImportName("PrismaticHelper.CutsceneTriggers")]
        public static class PrismaticHelperImports
        {
            public static Action<string, Func<Player, Level, List<string>, IEnumerator>> Register;
        }

        public static void Load()
        {
            // Register custom cutscene triggers with PrismaticHelper
            PrismaticHelperImports.Register?.Invoke("flash_screen", FlashScreen);
            PrismaticHelperImports.Register?.Invoke("shake_screen", ShakeScreen);
            PrismaticHelperImports.Register?.Invoke("change_music", ChangeMusic);
            PrismaticHelperImports.Register?.Invoke("spawn_particles", SpawnParticles);
            
            // Register Flowey intro triggers
            PrismaticHelperImports.Register?.Invoke("flowey_emerges", FloweyEmergesTrigger);
            PrismaticHelperImports.Register?.Invoke("madeline_step_forward", MadelineStepForwardTrigger);
            PrismaticHelperImports.Register?.Invoke("flowey_music_drop", FloweyMusicDropTrigger);
            PrismaticHelperImports.Register?.Invoke("circle_madeline_seed", CircleMadelineWithSeedTrigger);
            PrismaticHelperImports.Register?.Invoke("flowey_laugh", FloweyLaughTrigger);
            PrismaticHelperImports.Register?.Invoke("star_bullet_hit", StarBulletHitFloweyTrigger);
            PrismaticHelperImports.Register?.Invoke("kirby_walk_in", KirbyWalkInTrigger);
            PrismaticHelperImports.Register?.Invoke("theo_walk_in", TheoWalkInTrigger);
            PrismaticHelperImports.Register?.Invoke("everyone_posed", EveryonePosedTrigger);
            
            // Register Vingteen AIRee triggers
            PrismaticHelperImports.Register?.Invoke("soul_spawned", SoulSpawnedTrigger);
            PrismaticHelperImports.Register?.Invoke("soul_glitch_pink", SoulGlitchPinkTrigger);
            
            // Player State Manipulation
            PrismaticHelperImports.Register?.Invoke("set_dash_count", SetDashCount);
            PrismaticHelperImports.Register?.Invoke("disable_movement", DisableMovement);
            PrismaticHelperImports.Register?.Invoke("force_crouch", ForceCrouch);
            PrismaticHelperImports.Register?.Invoke("set_stamina", SetStamina);
            
            // Visual Effects & Atmosphere
            PrismaticHelperImports.Register?.Invoke("color_grade", ColorGrade);
            PrismaticHelperImports.Register?.Invoke("zoom_camera", ZoomCamera);
            PrismaticHelperImports.Register?.Invoke("letterbox", Letterbox);
            
            // Text & Dialogue
            PrismaticHelperImports.Register?.Invoke("show_text", ShowText);
            PrismaticHelperImports.Register?.Invoke("typewriter_text", TypewriterText);
            
            // Time & Physics
            PrismaticHelperImports.Register?.Invoke("slow_motion", SlowMotion);
            PrismaticHelperImports.Register?.Invoke("reverse_gravity", ReverseGravity);
            
            // Audio Enhancement
            PrismaticHelperImports.Register?.Invoke("play_sfx", PlaySFX);
            PrismaticHelperImports.Register?.Invoke("ambient_sound", AmbientSound);
            
            // Interactive Elements
            PrismaticHelperImports.Register?.Invoke("wait_for_input", WaitForInput);
            
            // Level Modification
            PrismaticHelperImports.Register?.Invoke("toggle_spikes", ToggleSpikes);
            
            // Emotional/Narrative
            PrismaticHelperImports.Register?.Invoke("memory_flash", MemoryFlash);
            PrismaticHelperImports.Register?.Invoke("dream_sequence", DreamSequence);
            
            // Advanced Combinations
            PrismaticHelperImports.Register?.Invoke("sequence_trigger", SequenceTrigger);
            PrismaticHelperImports.Register?.Invoke("random_trigger", RandomTrigger);
            
            // Intro/Prologue Triggers
            PrismaticHelperImports.Register?.Invoke("intro_presents", IntroPresents);
            PrismaticHelperImports.Register?.Invoke("show_disclaimer", ShowDisclaimer);
            
            // Load Chapter-Specific Cutscenes
            Chapter16Cutscenes.LoadCutscenes();
            Chapter17Cutscenes.LoadCutscenes();
            Chapter20Cutscenes.LoadCutscenes();
            MetaTrollCutscenes.LoadCutscenes();
        }

        // Custom trigger: Flash the screen with a color
        private static IEnumerator FlashScreen(Player player, Level level, List<string> parameters)
        {
            string colorHex = parameters.Count > 0 ? parameters[0] : "FFFFFF";
            float duration = parameters.Count > 1 ? float.Parse(parameters[1]) : 0.2f;
            
            Color flashColor = Calc.HexToColor(colorHex);
            
            // Create flash effect
            Entity flash = new Entity();
            flash.Add(new StaticMover());
            
            // Add flash logic here
            level.Add(flash);
            
            yield return duration;
            
            flash.RemoveSelf();
        }

        // Custom trigger: Shake the screen
        private static IEnumerator ShakeScreen(Player player, Level level, List<string> parameters)
        {
            float intensity = parameters.Count > 0 ? float.Parse(parameters[0]) : 0.5f;
            float duration = parameters.Count > 1 ? float.Parse(parameters[1]) : 1.0f;
            
            level.Shake(duration);
            yield return duration;
        }

        // Custom trigger: Change background music
        private static IEnumerator ChangeMusic(Player player, Level level, List<string> parameters)
        {
            string musicEvent = parameters.Count > 0 ? parameters[0] : "";
            bool fadeOut = parameters.Count > 1 ? bool.Parse(parameters[1]) : true;
            
            if (fadeOut)
            {
                Audio.SetMusic(null);
                yield return 0.5f;
            }
            
            if (!string.IsNullOrEmpty(musicEvent))
            {
                Audio.SetMusic(musicEvent);
            }
            
            yield return null;
        }

        // Custom trigger: Spawn particle effects
        private static IEnumerator SpawnParticles(Player player, Level level, List<string> parameters)
        {
            string particleType = parameters.Count > 0 ? parameters[0] : "dust";
            Vector2 position = player.Position;
            
            if (parameters.Count >= 3)
            {
                position.X = float.Parse(parameters[1]);
                position.Y = float.Parse(parameters[2]);
            }
            
            // Spawn particles based on type
            switch (particleType.ToLower())
            {
                case "dust":
                    level.Particles.Emit(Player.P_DashA, position, Color.White, 1);
                    break;
                case "sparkle":
                    level.ParticlesFG.Emit(Strawberry.P_Glow, position, Color.Yellow, 1);
                    break;
                // Add more particle types as needed
            }
            
            yield return null;
        }

        // ===== PLAYER STATE MANIPULATION =====
        
        // Custom trigger: Set player dash count
        private static IEnumerator SetDashCount(Player player, Level level, List<string> parameters)
        {
            int dashCount = parameters.Count > 0 ? int.Parse(parameters[0]) : 1;
            float duration = parameters.Count > 1 ? float.Parse(parameters[1]) : -1f; // -1 = permanent
            
            int originalDashes = player.Dashes;
            player.Dashes = dashCount;
            
            if (duration > 0)
            {
                yield return duration;
                player.Dashes = originalDashes;
            }
            
            yield return null;
        }

        // Custom trigger: Disable/enable player movement
        private static IEnumerator DisableMovement(Player player, Level level, List<string> parameters)
        {
            float duration = parameters.Count > 0 ? float.Parse(parameters[0]) : 2.0f;
            bool disableInput = parameters.Count > 1 ? bool.Parse(parameters[1]) : true;
            
            if (disableInput)
            {
                player.StateMachine.State = Player.StDummy;
            }
            
            yield return duration;
            
            if (disableInput && player.StateMachine.State == Player.StDummy)
            {
                player.StateMachine.State = Player.StNormal;
            }
        }

        // Custom trigger: Force player to crouch/uncrouch
        private static IEnumerator ForceCrouch(Player player, Level level, List<string> parameters)
        {
            bool shouldCrouch = parameters.Count > 0 ? bool.Parse(parameters[0]) : true;
            float duration = parameters.Count > 1 ? float.Parse(parameters[1]) : 1.0f;
            
            if (shouldCrouch)
            {
                player.Ducking = true;
            }
            else
            {
                player.Ducking = false;
            }
            
            yield return duration;
        }

        // Custom trigger: Set player stamina
        private static IEnumerator SetStamina(Player player, Level level, List<string> parameters)
        {
            float stamina = parameters.Count > 0 ? float.Parse(parameters[0]) : 110f;
            bool permanent = parameters.Count > 1 ? bool.Parse(parameters[1]) : false;
            
            float originalStamina = player.Stamina;
            player.Stamina = stamina;
            
            if (!permanent)
            {
                yield return 0.1f;
                player.Stamina = originalStamina;
            }
            
            yield return null;
        }

        // ===== VISUAL EFFECTS & ATMOSPHERE =====
        
        // Custom trigger: Apply color grading
        private static IEnumerator ColorGrade(Player player, Level level, List<string> parameters)
        {
            string effect = parameters.Count > 0 ? parameters[0] : "normal";
            float duration = parameters.Count > 1 ? float.Parse(parameters[1]) : 2.0f;
            float intensity = parameters.Count > 2 ? float.Parse(parameters[2]) : 1.0f;
            
            // Apply color effect based on type
            switch (effect.ToLower())
            {
                case "sepia":
                    // Implement sepia effect
                    break;
                case "noir":
                    // Implement noir effect
                    break;
                case "inverted":
                    // Implement inverted colors
                    break;
            }
            
            yield return duration;
            
            // Reset to normal
            yield return null;
        }

        // Custom trigger: Zoom camera
        private static IEnumerator ZoomCamera(Player player, Level level, List<string> parameters)
        {
            float zoomLevel = parameters.Count > 0 ? float.Parse(parameters[0]) : 1.5f;
            float duration = parameters.Count > 1 ? float.Parse(parameters[1]) : 2.0f;
            bool smooth = parameters.Count > 2 ? bool.Parse(parameters[2]) : true;
            
            Camera camera = level.Camera;
            float originalZoom = camera.Zoom;
            float elapsed = 0f;
            
            if (smooth)
            {
                while (elapsed < duration)
                {
                    elapsed += Engine.DeltaTime;
                    float progress = elapsed / duration;
                    camera.Zoom = Calc.LerpClamp(originalZoom, zoomLevel, progress);
                    yield return null;
                }
            }
            else
            {
                camera.Zoom = zoomLevel;
                yield return duration;
            }
            
            // Reset zoom
            if (smooth)
            {
                elapsed = 0f;
                while (elapsed < 1f)
                {
                    elapsed += Engine.DeltaTime;
                    float progress = elapsed / 1f;
                    camera.Zoom = Calc.LerpClamp(zoomLevel, originalZoom, progress);
                    yield return null;
                }
            }
            camera.Zoom = originalZoom;
        }

        // Custom trigger: Add letterbox effect
        private static IEnumerator Letterbox(Player player, Level level, List<string> parameters)
        {
            float height = parameters.Count > 0 ? float.Parse(parameters[0]) : 40f;
            float duration = parameters.Count > 1 ? float.Parse(parameters[1]) : 3.0f;
            
            // Create letterbox entities
            Entity topBar = new Entity(Vector2.Zero);
            Entity bottomBar = new Entity(new Vector2(0, 180 - height));
            
            level.Add(topBar);
            level.Add(bottomBar);
            
            yield return duration;
            
            topBar.RemoveSelf();
            bottomBar.RemoveSelf();
        }

        // ===== TEXT & DIALOGUE =====
        
        // Custom trigger: Show text on screen
        private static IEnumerator ShowText(Player player, Level level, List<string> parameters)
        {
            string text = parameters.Count > 0 ? parameters[0] : "Default Text";
            float duration = parameters.Count > 1 ? float.Parse(parameters[1]) : 3.0f;
            string position = parameters.Count > 2 ? parameters[2] : "center";
            
            // Create text entity
            Entity textEntity = new Entity();
            // Add text rendering component here
            level.Add(textEntity);
            
            yield return duration;
            
            textEntity.RemoveSelf();
        }

        // Custom trigger: Typewriter effect text
        private static IEnumerator TypewriterText(Player player, Level level, List<string> parameters)
        {
            string text = parameters.Count > 0 ? parameters[0] : "Default Text";
            float speed = parameters.Count > 1 ? float.Parse(parameters[1]) : 0.05f;
            
            // Create typewriter effect
            for (int i = 0; i <= text.Length; i++)
            {
                string currentText = text.Substring(0, i);
                // Update display with currentText
                yield return speed;
            }
            
            yield return 2.0f; // Show complete text for 2 seconds
        }

        // ===== TIME & PHYSICS =====
        
        // Custom trigger: Slow motion effect
        private static IEnumerator SlowMotion(Player player, Level level, List<string> parameters)
        {
            float timeScale = parameters.Count > 0 ? float.Parse(parameters[0]) : 0.3f;
            float duration = parameters.Count > 1 ? float.Parse(parameters[1]) : 2.0f;
            
            float originalTimeRate = Engine.TimeRate;
            Engine.TimeRate = timeScale;
            
            yield return duration;
            
            Engine.TimeRate = originalTimeRate;
        }

        // Custom trigger: Reverse gravity
        private static IEnumerator ReverseGravity(Player player, Level level, List<string> parameters)
        {
            float duration = parameters.Count > 0 ? float.Parse(parameters[0]) : 3.0f;
            float strength = parameters.Count > 1 ? float.Parse(parameters[1]) : 1.0f;
            
            // Store original gravity and reverse it
            float originalGravity = player.Speed.Y;
            
            // Apply reverse gravity effect
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                player.Speed.Y -= 900f * strength * Engine.DeltaTime; // Reverse gravity
                yield return null;
            }
        }

        // ===== AUDIO ENHANCEMENT =====
        
        // Custom trigger: Play sound effect
        private static IEnumerator PlaySFX(Player player, Level level, List<string> parameters)
        {
            string sfxEvent = parameters.Count > 0 ? parameters[0] : "event:/char/madeline/footstep";
            float volume = parameters.Count > 1 ? float.Parse(parameters[1]) : 1.0f;
            
            Audio.Play(sfxEvent, player.Position).setVolume(volume);
            yield return null;
        }

        // Custom trigger: Play ambient sound
        private static IEnumerator AmbientSound(Player player, Level level, List<string> parameters)
        {
            string ambientEvent = parameters.Count > 0 ? parameters[0] : "event:/env/amb/00_prologue";
            float duration = parameters.Count > 1 ? float.Parse(parameters[1]) : 5.0f;
            
            var ambientSound = Audio.Play(ambientEvent);
            yield return duration;
            // Use Celeste's Audio.Stop for compatibility across FMOD versions
            Audio.Stop(ambientSound, true);
        }

        // ===== INTERACTIVE ELEMENTS =====
        
        // Custom trigger: Wait for player input
        private static IEnumerator WaitForInput(Player player, Level level, List<string> parameters)
        {
            string inputType = parameters.Count > 0 ? parameters[0] : "any";
            float timeout = parameters.Count > 1 ? float.Parse(parameters[1]) : -1f;
            
            float elapsed = 0f;
            
            while (true)
            {
                if (timeout > 0 && elapsed >= timeout)
                    break;
                
                bool inputReceived = false;
                
                switch (inputType.ToLower())
                {
                    case "jump":
                        inputReceived = Input.Jump.Pressed;
                        break;
                    case "dash":
                        inputReceived = Input.Dash.Pressed;
                        break;
                    case "grab":
                        inputReceived = Input.Grab.Check;
                        break;
                    case "any":
                    default:
                        inputReceived = Input.Jump.Pressed || Input.Dash.Pressed || Input.Grab.Pressed;
                        break;
                }
                
                if (inputReceived)
                    break;
                
                elapsed += Engine.DeltaTime;
                yield return null;
            }
        }

        // ===== LEVEL MODIFICATION =====
        
        // Custom trigger: Toggle spikes
        private static IEnumerator ToggleSpikes(Player player, Level level, List<string> parameters)
        {
            bool enabled = parameters.Count > 0 ? bool.Parse(parameters[0]) : false;
            string spikeType = parameters.Count > 1 ? parameters[1] : "all";
            
            // Find and toggle spikes in the level
            foreach (Entity entity in level.Entities)
            {
                if (entity is Spikes spike)
                {
                    spike.Visible = enabled;
                    spike.Collidable = enabled;
                }
            }
            
            yield return null;
        }

        // ===== EMOTIONAL/NARRATIVE =====
        
        // Custom trigger: Memory flash effect
        private static IEnumerator MemoryFlash(Player player, Level level, List<string> parameters)
        {
            float intensity = parameters.Count > 0 ? float.Parse(parameters[0]) : 1.0f;
            string colorHex = parameters.Count > 1 ? parameters[1] : "FFFFFF";
            
            Color flashColor = Calc.HexToColor(colorHex);
            
            // Quick flash sequence
            for (int i = 0; i < 3; i++)
            {
                // Flash on
                yield return 0.1f;
                // Flash off
                yield return 0.1f;
            }
        }

        // Custom trigger: Dream sequence effect
        private static IEnumerator DreamSequence(Player player, Level level, List<string> parameters)
        {
            float duration = parameters.Count > 0 ? float.Parse(parameters[0]) : 5.0f;
            
            // Apply dreamy visual effects
            float originalTimeRate = Engine.TimeRate;
            Engine.TimeRate = 0.8f; // Slightly slower
            
            // Add blur/distortion effects here
            
            yield return duration;
            
            Engine.TimeRate = originalTimeRate;
        }

        // ===== ADVANCED COMBINATIONS =====
        
        // Custom trigger: Sequence multiple triggers
        private static IEnumerator SequenceTrigger(Player player, Level level, List<string> parameters)
        {
            // Parameters: trigger1,delay1,trigger2,delay2,...
            for (int i = 0; i < parameters.Count; i += 2)
            {
                string triggerName = parameters[i];
                float delay = i + 1 < parameters.Count ? float.Parse(parameters[i + 1]) : 0f;
                
                // Execute the named trigger (simplified - would need trigger lookup)
                yield return delay;
            }
        }

        // Custom trigger: Random trigger selection
        private static IEnumerator RandomTrigger(Player player, Level level, List<string> parameters)
        {
            if (parameters.Count == 0)
                yield break;
            
            // Randomly select one of the provided trigger names
            string selectedTrigger = parameters[Calc.Random.Next(parameters.Count)];
            
            // Execute the selected trigger (simplified - would need trigger lookup)
            yield return null;
        }

        // ===== FLOWEY INTRO SPECIFIC TRIGGERS =====
        
        // Custom trigger: Flowey emerges and start music (trigger 0)
        private static IEnumerator FloweyEmergesTrigger(Player player, Level level, List<string> parameters)
        {
            // This trigger is meant to make Flowey emerge and start music
            // For now, we'll just log it and continue
            IngesteLogger.Info("FloweyEmergesTrigger: Flowey emerges and music starts");
            
            // TODO: Implement actual Flowey emergence logic
            // This could involve spawning a Flowey entity, playing specific music, etc.
            
            yield return 0.5f; // Small delay to simulate the action
        }

        // Custom trigger: Madeline step forward and omnius zoom in (trigger 1)
        private static IEnumerator MadelineStepForwardTrigger(Player player, Level level, List<string> parameters)
        {
            // This trigger is meant to make Madeline step forward and zoom the camera
            IngesteLogger.Info("MadelineStepForwardTrigger: Madeline steps forward and camera zooms");
            
            // TODO: Implement actual step forward and zoom logic
            // This could involve moving the player and adjusting camera zoom
            
            yield return 0.5f; // Small delay to simulate the action
        }

        // Custom trigger: Flowey caught madeline and music dropped (trigger 2)
        private static IEnumerator FloweyMusicDropTrigger(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("FloweyMusicDropTrigger: Music intensity drops");
            yield return 0.5f;
        }

        // Custom trigger: Circle madeline with seed and heal (trigger 3)
        private static IEnumerator CircleMadelineWithSeedTrigger(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("CircleMadelineWithSeedTrigger: Flowey circles Madeline with healing effect");
            yield return 0.5f;
        }

        // Custom trigger: Flowey laugh (trigger 4)
        private static IEnumerator FloweyLaughTrigger(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("FloweyLaughTrigger: Flowey laughs menacingly");
            yield return 0.5f;
        }

        // Custom trigger: Star bullet hit flowey and flowey knock out (trigger 5)
        private static IEnumerator StarBulletHitFloweyTrigger(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("StarBulletHitFloweyTrigger: Star bullet hits Flowey, knocking him out");
            yield return 0.5f;
        }

        // Custom trigger: Kirby walk in right (trigger 6)
        private static IEnumerator KirbyWalkInTrigger(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("KirbyWalkInTrigger: Kirby walks in from the right");
            yield return 0.5f;
        }

        // Custom trigger: Theo walk in right (trigger 7)
        private static IEnumerator TheoWalkInTrigger(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("TheoWalkInTrigger: Theo walks in from the right");
            yield return 0.5f;
        }

        // Custom trigger: Everyone posed (trigger 8)
        private static IEnumerator EveryonePosedTrigger(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("EveryonePosedTrigger: Everyone strikes a pose");
            yield return 0.5f;
        }

        // ===== VINGTEEN AIREE SPECIFIC TRIGGERS =====
        
        // Custom trigger: Soul spawned (trigger 0 for Vingteen AIRee)
        private static IEnumerator SoulSpawnedTrigger(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("SoulSpawnedTrigger: Soul appears");
            yield return 0.5f;
        }

        // Custom trigger: Soul glitch effect turn to pink (trigger 1 for Vingteen AIRee)
        private static IEnumerator SoulGlitchPinkTrigger(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("SoulGlitchPinkTrigger: Soul glitches and turns pink");
            yield return 0.5f;
        }

        // ===== INTRO/PROLOGUE SPECIFIC TRIGGERS =====

        // Custom trigger: Show the intro presents screen
        private static IEnumerator IntroPresents(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("IntroPresents: Starting intro presents cutscene");
            
            // Check if already shown
            if (level.Session.GetFlag("intro_presents_complete"))
            {
                IngesteLogger.Info("IntroPresents: Already completed, skipping");
                yield break;
            }

            // Add and wait for the presents cutscene
            var presentsCutscene = new CS_IntroPresents(player);
            level.Add(presentsCutscene);
            
            // Wait until the cutscene ends
            while (presentsCutscene.Scene != null && !level.Session.GetFlag("intro_presents_complete"))
            {
                yield return null;
            }
            
            IngesteLogger.Info("IntroPresents: Completed");
        }

        // Custom trigger: Show disclaimer screen
        private static IEnumerator ShowDisclaimer(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("ShowDisclaimer: Displaying disclaimer");
            
            // Check if already acknowledged
            if (level.Session.GetFlag("disclaimer_acknowledged"))
            {
                IngesteLogger.Info("ShowDisclaimer: Already acknowledged, skipping");
                yield break;
            }

            // Parse optional parameters for custom header/content
            string header = parameters.Count > 0 ? parameters[0] : "CONTENT WARNING";
            
            var disclaimer = new CS_DisclaimerScreen(player);
            disclaimer.Header = header;
            
            // If custom lines are provided (separated by |)
            if (parameters.Count > 1)
            {
                disclaimer.Lines = parameters[1].Split('|');
            }
            
            level.Add(disclaimer);
            
            // Wait until acknowledged
            while (disclaimer.Scene != null && !level.Session.GetFlag("disclaimer_acknowledged"))
            {
                yield return null;
            }
            
            IngesteLogger.Info("ShowDisclaimer: Acknowledged");
        }
    }
}



