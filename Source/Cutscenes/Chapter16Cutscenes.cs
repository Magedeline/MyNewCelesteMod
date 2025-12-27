using DesoloZantas.Core.Core;
using MonoMod.ModInterop;

namespace DesoloZantas.Core.Cutscenes
{
    public static class Chapter16Cutscenes
    {
        [ModImportName("PrismaticHelper.CutsceneTriggers")]
        public static class PrismaticHelperImports
        {
            public static Action<string, Func<Player, Level, List<string>, IEnumerator>> Register;
        }

        #region Cutscene Registration
        public static void LoadCutscenes()
        {
            // Register Chapter 16 specific triggers
            if (PrismaticHelperImports.Register != null)
            {
                // Boss fight progression triggers
                PrismaticHelperImports.Register("ch16_corrupted_intro", CorruptedRealmIntro);
                PrismaticHelperImports.Register("ch16_flowey_transform", FloweyTransformation);
                PrismaticHelperImports.Register("ch16_organic_garden", OrganicGardenPhase);
                PrismaticHelperImports.Register("ch16_bone_cathedral", BoneCathedralPhase);
                PrismaticHelperImports.Register("ch16_flesh_labyrinth", FleshLabyrinthPhase);
                PrismaticHelperImports.Register("ch16_weapon_arsenal", WeaponArsenalPhase);
                
                // Soul collection triggers
                PrismaticHelperImports.Register("ch16_spawn_souls", SpawnHumanSouls);
                PrismaticHelperImports.Register("ch16_soul_patience", CollectSoulPatience);
                PrismaticHelperImports.Register("ch16_soul_bravery", CollectSoulBravery);
                PrismaticHelperImports.Register("ch16_soul_integrity", CollectSoulIntegrity);
                PrismaticHelperImports.Register("ch16_soul_perseverance", CollectSoulPerseverance);
                PrismaticHelperImports.Register("ch16_soul_kindness", CollectSoulKindness);
                PrismaticHelperImports.Register("ch16_soul_justice", CollectSoulJustice);
                
                // Final battle triggers
                PrismaticHelperImports.Register("ch16_final_transformation", FinalTransformation);
                PrismaticHelperImports.Register("ch16_player_empowerment", PlayerEmpowerment);
                PrismaticHelperImports.Register("ch16_rainbow_attack", RainbowSoulBlast);
                PrismaticHelperImports.Register("ch16_redemption", RedemptionSequence);
                
                // Special effects triggers
                PrismaticHelperImports.Register("ch16_reality_distort", RealityDistortion);
                PrismaticHelperImports.Register("ch16_nightmare_aura", NightmareAura);
                PrismaticHelperImports.Register("ch16_soul_liberation", SoulLiberation);
                
                // Secret Asriel reveal
                PrismaticHelperImports.Register("ch16_asriel_reveal", AsrielReveal);
                
                // New dialog-based triggers
                PrismaticHelperImports.Register("ch16_first_phase_battle", FirstPhaseBattle);
                PrismaticHelperImports.Register("ch16_call_for_help", CallForHelp);
                PrismaticHelperImports.Register("ch16_final_defeat", FinalDefeatSequence);
                PrismaticHelperImports.Register("ch16_barrier_breaks", BarrierBreaks);
                PrismaticHelperImports.Register("ch16_farewell_titan", FarewellTitanKing);
                PrismaticHelperImports.Register("ch16_return_home", ReturnToCeleste);
            }
        }
        #endregion

        #region Main Cutscene Sequences
        
        private static IEnumerator CorruptedRealmIntro(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Corrupted Realm Introduction");
            
            // Lock player movement
            player.StateMachine.State = Player.StDummy;
            
            // Fade in corrupted environment
            yield return FadeToCorruptedEnvironment(level);
            
            // Camera effects - show the horror of the organic realm
            yield return CameraShowOrganicHorror(level);
            
            // Spawn initial organic decorations
            SpawnOrganicDecorations(level);
            
            // Wait for dialog to finish
            while (level.InCutscene)
                yield return null;
            
            // Release player
            player.StateMachine.State = Player.StNormal;
            
            yield return 0.5f;
        }

        private static IEnumerator FloweyTransformation(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Flowey Transformation Sequence");
            
            // Find Els_Flowey boss if it exists
            var floweyBoss = level.Tracker.GetEntity<ElsFloweyBoss>();
            
            // Screen shake and flash effects
            level.Flash(Color.Red, true);
            level.DirectionalShake(Vector2.One, 2f);
            
            // Play transformation music
            Audio.SetMusic("event:/music/desolozantas/organic_nightmare");
            
            // Spawn transformation particles
            yield return SpawnTransformationEffects(level, floweyBoss?.Position ?? player.Position);
            
            // Wait for transformation to complete
            yield return 3f;
        }

        private static IEnumerator OrganicGardenPhase(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Organic Garden Phase");
            
            // Transform arena to organic garden
            yield return TransformArenaToOrganicGarden(level);
            
            // Spawn beating hearts and pulsing organs
            SpawnBeatingHearts(level, 12);
            SpawnHangingOrgans(level, 8);
            SpawnPulsingVines(level, 15);
            
            // Add ambient organic sounds
            Audio.Play("event:/ambience/desolozantas/organic_garden", level.Camera.Position);
            
            yield return 2f;
        }

        private static IEnumerator BoneCathedralPhase(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Bone Cathedral Phase");
            
            // Clear organic elements
            ClearArenaElements<OrganicVine>(level);
            ClearArenaElements<BeatingHeart>(level);
            
            // Transform to bone cathedral
            yield return TransformArenaToBoneCathedral(level);
            
            // Spawn bone structures
            SpawnBonePillars(level, 8);
            SpawnSkullDecorations(level, 20);
            SpawnBoneArches(level, 4);
            
            // Bone cathedral ambience
            Audio.Play("event:/ambience/desolozantas/bone_cathedral", level.Camera.Position);
            
            yield return 2f;
        }

        private static IEnumerator FleshLabyrinthPhase(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Flesh Labyrinth Phase");
            
            // Clear bone elements
            ClearArenaElements<BonePillar>(level);
            
            // Transform to flesh labyrinth
            yield return TransformArenaToFleshLabyrinth(level);
            
            // Create maze-like flesh walls
            SpawnFleshWalls(level);
            SpawnNerveNetworks(level, 10);
            SpawnBloodPools(level, 6);
            
            // Disturbing flesh ambience
            Audio.Play("event:/ambience/desolozantas/flesh_labyrinth", level.Camera.Position);
            
            yield return 2f;
        }

        private static IEnumerator WeaponArsenalPhase(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Weapon Arsenal Phase");
            
            // Display torture weapons
            SpawnWeaponDisplays(level);
            
            // Ominous weapon rattling sounds
            Audio.Play("event:/ambience/desolozantas/weapon_arsenal", level.Camera.Position);
            
            yield return 2f;
        }
        #endregion

        #region Soul Collection Cutscenes

        private static IEnumerator SpawnHumanSouls(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Spawning Human Souls");
            
            // Dramatic music change
            Audio.SetMusic("event:/music/desolozantas/souls_awakening");
            
            // Flash of ethereal light
            level.Flash(Color.White, true);
            
            // Spawn souls at their designated positions
            var soulPositions = new Dictionary<string, Vector2>
            {
                { "patience", new Vector2(100, 200) },
                { "bravery", new Vector2(300, 150) },
                { "integrity", new Vector2(500, 250) },
                { "perseverance", new Vector2(200, 400) },
                { "kindness", new Vector2(450, 350) },
                { "justice", new Vector2(350, 100) }
            };

            foreach (var soulData in soulPositions)
            {
                level.Add(new HumanSoul(soulData.Value, soulData.Key));
                
                // Ethereal spawning effect
                yield return SpawnSoulEffect(level, soulData.Value, GetSoulColor(soulData.Key));
                yield return 0.5f;
            }
            
            yield return 2f;
        }

        private static IEnumerator CollectSoulPatience(Player player, Level level, List<string> parameters)
        {
            yield return SoulCollectionSequence(level, "patience", Color.Cyan, 
                "Patience flows through you... the ability to endure any trial.");
        }

        private static IEnumerator CollectSoulBravery(Player player, Level level, List<string> parameters)
        {
            yield return SoulCollectionSequence(level, "bravery", Color.Orange, 
                "Bravery ignites within you... fear becomes courage.");
        }

        private static IEnumerator CollectSoulIntegrity(Player player, Level level, List<string> parameters)
        {
            yield return SoulCollectionSequence(level, "integrity", Color.Blue, 
                "Integrity strengthens you... truth cuts through all deception.");
        }

        private static IEnumerator CollectSoulPerseverance(Player player, Level level, List<string> parameters)
        {
            yield return SoulCollectionSequence(level, "perseverance", Color.Purple, 
                "Perseverance empowers you... you will never give up.");
        }

        private static IEnumerator CollectSoulKindness(Player player, Level level, List<string> parameters)
        {
            yield return SoulCollectionSequence(level, "kindness", Color.Green, 
                "Kindness fills your heart... even for your enemies.");
        }

        private static IEnumerator CollectSoulJustice(Player player, Level level, List<string> parameters)
        {
            yield return SoulCollectionSequence(level, "justice", Color.Yellow, 
                "Justice blazes within you... the final soul is yours.");
        }

        private static IEnumerator SoulCollectionSequence(Level level, string soulType, Color soulColor, string message)
        {
            IngesteLogger.Info($"Chapter 16: Collecting {soulType} soul");
            
            // Flash with soul color
            level.Flash(soulColor, true);
            
            // Play soul collection sound
            Audio.Play("event:/char/desolozantas/soul_collected", level.Camera.Position);
            
            // Screen text effect
            yield return ShowSoulMessage(level, message, soulColor);
            
            // Particle burst
            yield return SoulCollectionParticles(level, soulColor);
            
            yield return 1f;
        }
        #endregion

        #region Final Battle Cutscenes

        private static IEnumerator FinalTransformation(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Final Transformation Sequence");
            
            // Lock player
            player.StateMachine.State = Player.StDummy;
            
            // Intense screen shaking
            level.DirectionalShake(Vector2.One, 5f);
            
            // Multiple color flashes
            for (int i = 0; i < 5; i++)
            {
                level.Flash(Color.Red, true);
                yield return 0.3f;
                level.Flash(Color.Black, false);
                yield return 0.3f;
            }
            
            // Transform arena to final chamber
            yield return TransformArenaToFinalChamber(level);
            
            // Release player
            player.StateMachine.State = Player.StNormal;
            
            yield return 2f;
        }

        private static IEnumerator PlayerEmpowerment(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Player Empowerment Sequence");
            
            // Lock player in empowerment pose
            player.StateMachine.State = Player.StDummy;
            
            // Six soul colors swirl around player
            yield return SixSoulEmpowermentEffect(level, player);
            
            // Player glows with rainbow light
            yield return PlayerRainbowGlow(level, player);
            
            // Epic empowerment music
            Audio.SetMusic("event:/music/desolozantas/six_souls_power");
            
            // Release player with new powers
            player.StateMachine.State = Player.StNormal;
            
            yield return 2f;
        }

        private static IEnumerator RainbowSoulBlast(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Rainbow Soul Blast Attack");
            
            // Lock player in attack pose
            player.StateMachine.State = Player.StDummy;
            
            // Charge up effect
            yield return ChargeRainbowAttack(level, player);
            
            // Massive rainbow beam
            yield return ExecuteRainbowBeam(level, player);
            
            // Screen flash and shake
            level.Flash(Color.White, true);
            level.DirectionalShake(Vector2.One, 3f);
            
            // Release player
            player.StateMachine.State = Player.StNormal;
            
            yield return 2f;
        }

        private static IEnumerator RedemptionSequence(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Redemption Sequence");
            
            // Peaceful music transition
            Audio.SetMusic("event:/music/desolozantas/redemption");
            
            // Gentle light effects
            yield return GentleHealingLight(level);
            
            // Fade out nightmare elements
            yield return FadeNightmareElements(level);
            
            // Restore natural environment
            yield return RestoreNaturalEnvironment(level);
            
            yield return 3f;
        }
        #endregion

        #region Special Effect Cutscenes

        private static IEnumerator RealityDistortion(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Reality Distortion Effect");
            
            // Screen warping effects
            level.Add(new ScreenWarpEffect(3f));
            
            // Distorted audio
            Audio.Play("event:/char/desolozantas/reality_warp", level.Camera.Position);
            
            yield return 3f;
        }

        private static IEnumerator NightmareAura(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Nightmare Aura Effect");
            
            // Dark spreading effect
            level.Add(new DarknessSpreadEffect());
            
            // Despair ambience
            Audio.Play("event:/ambience/desolozantas/despair_aura", level.Camera.Position);
            
            yield return 4f;
        }

        private static IEnumerator SoulLiberation(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Soul Liberation Sequence");
            
            // Six souls ascending
            yield return SoulAscensionEffect(level);
            
            // Peaceful liberation music
            Audio.SetMusic("event:/music/desolozantas/soul_liberation");
            
            yield return 5f;
        }

        private static IEnumerator AsrielReveal(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Secret Asriel Reveal");
            
            // Only trigger if conditions are met
            if (!CheckAsrielRevealConditions(level))
                yield break;
            
            // Ethereal appearance effect
            yield return AsrielMaterializeEffect(level);
            
            // Gentle, melancholy music
            Audio.SetMusic("event:/music/desolozantas/asriel_theme");
            
            yield return 3f;
        }
        #endregion

        #region New Dialog-Based Cutscenes

        private static IEnumerator FirstPhaseBattle(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: First Phase Battle Sequence");
            
            // Lock player movement
            player.StateMachine.State = Player.StDummy;
            
            // Trigger the dialog
            yield return Textbox.Say("CH16_FIRST_PHASE_BATTLE_0");
            
            // Battle effects
            level.Flash(Color.Red, true);
            level.DirectionalShake(Vector2.One, 2f);
            
            // Spawn lost soul
            var lostSoulPos = player.Position + new Vector2(100, -50);
            level.Add(new SoulMessage("Help... us...", Color.White, 3f));
            
            yield return Textbox.Say("CH16_FIRST_PHASE_BATTLE_1");
            
            // Release player
            player.StateMachine.State = Player.StNormal;
            
            yield return 1f;
        }

        private static IEnumerator CallForHelp(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Call for Help Sequence");
            
            // Lock player movement
            player.StateMachine.State = Player.StDummy;
            
            // Trigger help call dialog
            yield return Textbox.Say("CH16_YOU_CALL_HELP");
            
            // Screen effects for power loss and restoration
            level.Flash(Color.White, true);
            level.DirectionalShake(Vector2.One, 3f);
            
            // Show eight souls appearing around player
            yield return SpawnEightSoulsAroundPlayer(level, player);
            
            // Release player
            player.StateMachine.State = Player.StNormal;
            
            yield return 2f;
        }

        private static IEnumerator FinalDefeatSequence(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Final Defeat Sequence");
            
            // Lock player movement
            player.StateMachine.State = Player.StDummy;
            
            // Trigger final defeat dialog
            yield return Textbox.Say("CH16_FINAL_DEFEAT");
            
            // Dramatic screen effects
            level.Flash(Color.White, true);
            level.DirectionalShake(Vector2.One, 4f);
            
            // Release player
            player.StateMachine.State = Player.StNormal;
            
            yield return 2f;
        }

        private static IEnumerator BarrierBreaks(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Barrier Breaking Sequence");
            
            // Lock player movement
            player.StateMachine.State = Player.StDummy;
            
            // Barrier breaking effects
            Audio.Play("event:/char/desolozantas/barrier_break", level.Camera.Position);
            level.Flash(Color.Cyan, true);
            
            // Trigger barrier dialog
            yield return Textbox.Say("CH16_BARRIER_BREAKS");
            
            // Environmental restoration effects
            yield return RestoreEnvironmentEffects(level);
            
            // Release player
            player.StateMachine.State = Player.StNormal;
            
            yield return 3f;
        }

        private static IEnumerator FarewellTitanKing(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Farewell Titan King Sequence");
            
            // Lock player movement
            player.StateMachine.State = Player.StDummy;
            
            // Solemn moment
            Audio.SetMusic("event:/music/desolozantas/titan_farewell");
            
            // Trigger farewell dialog
            yield return Textbox.Say("CH16_FAREWELL_TITAN_KING");
            
            // Memorial effects
            yield return TitanKingMemorialEffects(level);
            
            // Release player
            player.StateMachine.State = Player.StNormal;
            
            yield return 3f;
        }

        private static IEnumerator ReturnToCeleste(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 16: Return to Celeste Sequence");
            
            // Lock player movement
            player.StateMachine.State = Player.StDummy;
            
            // Peaceful return music
            Audio.SetMusic("event:/music/desolozantas/return_home");
            
            // Trigger return dialog
            yield return Textbox.Say("CH16_RETURN_TO_CELESTE");
            
            // Fade to ending
            yield return FadeToEnding(level);
            
            // Release player
            player.StateMachine.State = Player.StNormal;
            
            yield return 2f;
        }

        #endregion

        #region Helper Methods

        private static IEnumerator FadeToCorruptedEnvironment(Level level)
        {
            // Gradually shift colors to corrupted palette
            for (float t = 0; t < 2f; t += Engine.DeltaTime)
            {
                float progress = t / 2f;
                // Apply color grading effects here
                yield return null;
            }
        }

        private static IEnumerator CameraShowOrganicHorror(Level level)
        {
            Camera camera = level.Camera;
            Vector2 originalPos = camera.Position;
            
            // Pan camera to show organic horrors
            Vector2[] horrorSpots = 
            {
                originalPos + new Vector2(100, -50),
                originalPos + new Vector2(-80, 60),
                originalPos + new Vector2(50, -80)
            };
            
            foreach (Vector2 spot in horrorSpots)
            {
                yield return CameraPanTo(camera, spot, 1.5f);
                yield return 1f;
            }
            
            // Return to original position
            yield return CameraPanTo(camera, originalPos, 1f);
        }

        private static IEnumerator CameraPanTo(Camera camera, Vector2 target, float duration)
        {
            Vector2 start = camera.Position;
            
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                float progress = Ease.CubeInOut(t / duration);
                camera.Position = Vector2.Lerp(start, target, progress);
                yield return null;
            }
            
            camera.Position = target;
        }

        private static void SpawnOrganicDecorations(Level level)
        {
            // Spawn various organic horror decorations
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(0, level.Bounds.Width),
                    Calc.Random.Range(0, level.Bounds.Height)
                );
                
                string organType = Calc.Random.Choose("heart", "lung", "kidney", "liver");
                level.Add(new HangingOrgan(pos, organType));
            }
        }

        private static IEnumerator SpawnTransformationEffects(Level level, Vector2 center)
        {
            // Particle explosion
            for (int i = 0; i < 50; i++)
            {
                float angle = Calc.Random.NextFloat() * MathHelper.TwoPi;
                Vector2 direction = Vector2.One.Rotate(angle);
                Vector2 velocity = direction * Calc.Random.Range(50f, 200f);
                level.ParticlesFG.Emit(ParticleTypes.Dust, center, angle);
            }
            
            yield return 1f;
        }

        private static IEnumerator TransformArenaToOrganicGarden(Level level)
        {
            // Add organic garden background
            level.Add(new OrganicGardenBackground());
            
            // Transition effects
            yield return 2f;
        }

        private static IEnumerator TransformArenaToBoneCathedral(Level level)
        {
            level.Add(new BoneCathedralBackground());
            yield return 2f;
        }

        private static IEnumerator TransformArenaToFleshLabyrinth(Level level)
        {
            level.Add(new FleshLabyrinthBackground());
            yield return 2f;
        }

        private static IEnumerator TransformArenaToFinalChamber(Level level)
        {
            level.Add(new NightmareBackground());
            yield return 3f;
        }

        private static void SpawnBeatingHearts(Level level, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(50, level.Bounds.Width - 50),
                    Calc.Random.Range(50, level.Bounds.Height - 50)
                );
                level.Add(new BeatingHeart(pos));
            }
        }

        private static void SpawnHangingOrgans(Level level, int count)
        {
            string[] organTypes = { "heart", "lung", "kidney", "liver", "brain" };
            
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(50, level.Bounds.Width - 50),
                    Calc.Random.Range(50, 150)
                );
                
                string organType = Calc.Random.Choose(organTypes);
                level.Add(new HangingOrgan(pos, organType));
            }
        }

        private static void SpawnPulsingVines(Level level, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(0, level.Bounds.Width),
                    Calc.Random.Range(0, level.Bounds.Height)
                );
                level.Add(new OrganicVine(pos));
            }
        }

        private static void SpawnBonePillars(Level level, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(
                    (i + 1) * (level.Bounds.Width / (count + 1)),
                    level.Bounds.Height - 150
                );
                level.Add(new BonePillar(pos));
            }
        }

        private static void SpawnSkullDecorations(Level level, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(50, level.Bounds.Width - 50),
                    Calc.Random.Range(50, level.Bounds.Height - 50)
                );
                level.Add(new SkullDecoration(pos));
            }
        }

        private static void SpawnBoneArches(Level level, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(
                    (i + 1) * (level.Bounds.Width / (count + 1)),
                    level.Bounds.Height - 200
                );
                level.Add(new BoneArch(pos));
            }
        }

        private static void SpawnFleshWalls(Level level)
        {
            // Create a maze-like pattern
            for (int x = 0; x < level.Bounds.Width; x += 80)
            {
                for (int y = 0; y < level.Bounds.Height; y += 80)
                {
                    if (Calc.Random.Chance(0.3f))
                    {
                        level.Add(new FleshWall(new Vector2(x, y)));
                    }
                }
            }
        }

        private static void SpawnNerveNetworks(Level level, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(50, level.Bounds.Width - 50),
                    Calc.Random.Range(50, level.Bounds.Height - 50)
                );
                level.Add(new NerveNetwork(pos));
            }
        }

        private static void SpawnBloodPools(Level level, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(50, level.Bounds.Width - 50),
                    level.Bounds.Height - 20
                );
                level.Add(new BloodPool(pos));
            }
        }

        private static void SpawnWeaponDisplays(Level level)
        {
            string[] weapons = { "chains", "blades", "maces", "spears", "hammers" };
            
            foreach (string weapon in weapons)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(100, level.Bounds.Width - 100),
                    Calc.Random.Range(100, level.Bounds.Height - 100)
                );
                level.Add(new WeaponDisplay(pos, weapon));
            }
        }

        private static IEnumerator SpawnSoulEffect(Level level, Vector2 position, Color color)
        {
            // Ethereal spawning effect
            for (int i = 0; i < 20; i++)
            {
                float angle = Calc.Random.NextFloat() * MathHelper.TwoPi;
                Vector2 offset = Vector2.One.Rotate(angle) * Calc.Random.Range(10f, 30f);
                level.ParticlesFG.Emit(ParticleTypes.Dust, position + offset, color);
            }
            
            yield return 0.5f;
        }

        private static Color GetSoulColor(string soulType)
        {
            return soulType switch
            {
                "patience" => Color.Cyan,
                "bravery" => Color.Orange,
                "integrity" => Color.Blue,
                "perseverance" => Color.Purple,
                "kindness" => Color.Green,
                "justice" => Color.Yellow,
                _ => Color.White
            };
        }

        private static IEnumerator ShowSoulMessage(Level level, string message, Color color)
        {
            // Display soul message on screen
            level.Add(new SoulMessage(message, color, 3f));
            yield return 3f;
        }

        private static IEnumerator SoulCollectionParticles(Level level, Color color)
        {
            // Burst of colored particles
            Player player = level.Tracker.GetEntity<Player>();
            Vector2 center = player?.Position ?? level.Camera.Position;
            
            for (int i = 0; i < 30; i++)
            {
                float angle = Calc.Random.NextFloat() * MathHelper.TwoPi;
                level.ParticlesFG.Emit(ParticleTypes.Dust, center, color);
            }
            
            yield return 1f;
        }

        private static IEnumerator SixSoulEmpowermentEffect(Level level, Player player)
        {
            Color[] soulColors = { Color.Cyan, Color.Orange, Color.Blue, Color.Purple, Color.Green, Color.Yellow };
            
            for (int i = 0; i < soulColors.Length; i++)
            {
                // Souls orbit around player
                float angle = (i / 6f) * MathHelper.TwoPi;
                Vector2 soulPos = player.Position + Vector2.One.Rotate(angle) * 50f;
                
                level.Add(new OrbitingSoulEffect(player, soulPos, soulColors[i], 3f));
            }
            
            yield return 3f;
        }

        private static IEnumerator PlayerRainbowGlow(Level level, Player player)
        {
            // Add rainbow glow effect to player
            level.Add(new PlayerRainbowGlow(player, 5f));
            yield return 2f;
        }

        private static IEnumerator ChargeRainbowAttack(Level level, Player player)
        {
            // Charging effect
            level.Add(new RainbowChargeEffect(player.Position, 3f));
            yield return 3f;
        }

        private static IEnumerator ExecuteRainbowBeam(Level level, Player player)
        {
            // Massive rainbow beam attack
            Vector2 target = level.Camera.Position + new Vector2(level.Bounds.Width, 0);
            level.Add(new RainbowBeam(player.Position, target, 2f));
            yield return 2f;
        }

        private static IEnumerator GentleHealingLight(Level level)
        {
            // Soft, healing light effect
            level.Add(new HealingLightEffect(3f));
            yield return 3f;
        }

        private static IEnumerator FadeNightmareElements(Level level)
        {
            // Fade out all nightmare elements
            var nightmareEntities = level.Entities.FindAll<ArenaElement>();
            
            foreach (var entity in nightmareEntities)
            {
                entity.Add(new Coroutine(FadeEntityOut(entity, 2f)));
            }
            
            yield return 2f;
        }

        private static IEnumerator FadeEntityOut(Entity entity, float duration)
        {
            Sprite sprite = entity.Get<Sprite>();
            if (sprite == null) yield break;
            
            Color originalColor = sprite.Color;
            
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                float alpha = 1f - (t / duration);
                sprite.Color = originalColor * alpha;
                yield return null;
            }
            
            entity.RemoveSelf();
        }

        private static IEnumerator RestoreNaturalEnvironment(Level level)
        {
            // Restore peaceful environment
            level.Add(new NaturalEnvironmentRestore());
            yield return 2f;
        }

        private static IEnumerator SoulAscensionEffect(Level level)
        {
            // Six souls ascending to peace
            Color[] soulColors = { Color.Cyan, Color.Orange, Color.Blue, Color.Purple, Color.Green, Color.Yellow };
            
            Vector2 center = level.Camera.Position + new Vector2(level.Bounds.Width / 2, level.Bounds.Height / 2);
            
            for (int i = 0; i < soulColors.Length; i++)
            {
                float angle = Calc.Random.NextFloat() * MathHelper.TwoPi;
                Vector2 startPos = center + Vector2.One.Rotate(angle) * 50f;
                level.Add(new AscendingSoul(startPos, soulColors[i]));
                yield return 0.5f;
            }
            
            yield return 3f;
        }

        private static IEnumerator AsrielMaterializeEffect(Level level)
        {
            // Ethereal materialization
            Vector2 pos = level.Camera.Position + new Vector2(200, 100);
            level.Add(new AsrielMaterialize(pos));
            yield return 3f;
        }

        private static bool CheckAsrielRevealConditions(Level level)
        {
            // Check if player showed mercy, collected all souls, etc.
            return level.Session.GetFlag("showed_mercy_to_flowey") &&
                   level.Session.GetFlag("all_souls_collected");
        }

        private static void ClearArenaElements<T>(Level level) where T : Entity
        {
            foreach (T entity in level.Entities.FindAll<T>())
            {
                entity.RemoveSelf();
            }
        }

        private static IEnumerator SpawnEightSoulsAroundPlayer(Level level, Player player)
        {
            Color[] soulColors = { 
                Color.Cyan, Color.Orange, Color.Blue, Color.Purple, 
                Color.Green, Color.Yellow, Color.Red, Color.White 
            };
            
            float radius = 80f;
            
            for (int i = 0; i < 8; i++)
            {
                float angle = (i / 8f) * MathHelper.TwoPi;
                Vector2 soulPos = player.Position + Vector2.One.Rotate(angle) * radius;
                
                level.Add(new OrbitingSoulEffect(player, soulPos, soulColors[i], 5f));
                yield return 0.2f;
            }
        }

        private static IEnumerator RestoreEnvironmentEffects(Level level)
        {
            // Fade out corruption
            for (float t = 0; t < 2f; t += Engine.DeltaTime)
            {
                float progress = t / 2f;
                // Apply restoration effects
                yield return null;
            }
            
            // Add restoration particles
            for (int i = 0; i < 30; i++)
            {
                Vector2 pos = new Vector2(
                    Calc.Random.Range(0, level.Bounds.Width),
                    Calc.Random.Range(0, level.Bounds.Height)
                );
                level.ParticlesFG.Emit(ParticleTypes.Dust, pos, Color.Cyan);
            }
        }

        private static IEnumerator TitanKingMemorialEffects(Level level)
        {
            // Memorial light effect
            level.Flash(Color.Gold, true);
            
            // Ascending light particles
            Vector2 center = level.Camera.Position + new Vector2(level.Bounds.Width / 2, level.Bounds.Height / 2);
            
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = center + new Vector2(
                    Calc.Random.Range(-50f, 50f),
                    Calc.Random.Range(-50f, 50f)
                );
                
                level.ParticlesFG.Emit(ParticleTypes.Dust, pos, Color.Gold);
                yield return 0.1f;
            }
        }

        private static IEnumerator FadeToEnding(Level level)
        {
            // Fade to black
            level.Add(new FadeWipe(level, false, () => {
                // Transition to ending
                level.Session.SetFlag("chapter16_complete", true);
            }));
            
            yield return 3f;
        }
        #endregion
    }

    // Supporting effect classes (simplified implementations)
    public class ScreenWarpEffect : Entity
    {
        public ScreenWarpEffect(float duration) : base(Vector2.Zero) { }
    }

    public class DarknessSpreadEffect : Entity
    {
        public DarknessSpreadEffect() : base(Vector2.Zero) { }
    }

    public class SoulMessage : Entity
    {
        public SoulMessage(string message, Color color, float duration) : base(Vector2.Zero) { }
    }

    public class OrbitingSoulEffect : Entity
    {
        public OrbitingSoulEffect(Player target, Vector2 position, Color color, float duration) : base(position) { }
    }

    public class PlayerRainbowGlow : Entity
    {
        public PlayerRainbowGlow(Player target, float duration) : base(Vector2.Zero) { }
    }

    public class RainbowChargeEffect : Entity
    {
        public RainbowChargeEffect(Vector2 position, float duration) : base(position) { }
    }

    public class RainbowBeam : Entity
    {
        public RainbowBeam(Vector2 start, Vector2 end, float duration) : base(start) { }
    }

    public class HealingLightEffect : Entity
    {
        public HealingLightEffect(float duration) : base(Vector2.Zero) { }
    }

    public class NaturalEnvironmentRestore : Entity
    {
        public NaturalEnvironmentRestore() : base(Vector2.Zero) { }
    }

    public class AscendingSoul : Entity
    {
        public AscendingSoul(Vector2 position, Color color) : base(position) { }
    }

    public class AsrielMaterialize : Entity
    {
        public AsrielMaterialize(Vector2 position) : base(position) { }
    }

    public class SkullDecoration : ArenaElement
    {
        public SkullDecoration(Vector2 position) : base(position) { }
    }

    public class BoneArch : ArenaElement
    {
        public BoneArch(Vector2 position) : base(position) { }
    }

    public class NerveNetwork : ArenaElement
    {
        public NerveNetwork(Vector2 position) : base(position) { }
    }

    public class BloodPool : ArenaElement
    {
        public BloodPool(Vector2 position) : base(position) { }
    }

    // Additional background classes
    public class OrganicGardenBackground : Entity
    {
        public OrganicGardenBackground() : base(Vector2.Zero) { }
    }
}



