using DesoloZantas.Core.Core;
using MonoMod.ModInterop;

namespace DesoloZantas.Core.Cutscenes
{
    /// <summary>
    /// Chapter 17: Epilogue - The Descent Home
    /// Credits sequences showing the journey down from Mount Desolo Zantas
    /// back to Celeste Mountain, bringing everyone home safely
    /// </summary>
    public static class Chapter17Cutscenes
    {
        [ModImportName("PrismaticHelper.CutsceneTriggers")]
        public static class PrismaticHelperImports
        {
            public static Action<string, Func<Player, Level, List<string>, IEnumerator>> Register;
        }

        #region Cutscene Registration
        public static void LoadCutscenes()
        {
            // Register Chapter 17 specific triggers
            if (PrismaticHelperImports.Register != null)
            {
                // Journey Beginning
                PrismaticHelperImports.Register("ch17_descent_begins", DescentBegins);
                PrismaticHelperImports.Register("ch17_leaving_zantas", LeavingMountZantas);
                
                // Credits and Journey Sequences
                PrismaticHelperImports.Register("ch17_credits_introduction", CreditsIntroduction);
                PrismaticHelperImports.Register("ch17_thanking_companions", ThankingCompanions);
                PrismaticHelperImports.Register("ch17_remembering_journey", RememberingTheJourney);
                PrismaticHelperImports.Register("ch17_crossing_barrier", CrossingTheBarrier);
                
                // Mountain Descent Phases
                PrismaticHelperImports.Register("ch17_upper_slopes", UpperSlopesDescent);
                PrismaticHelperImports.Register("ch17_crystal_caves", CrystalCavesDescent);
                PrismaticHelperImports.Register("ch17_forest_path", ForestPathDescent);
                PrismaticHelperImports.Register("ch17_valley_crossing", ValleyCrossing);
                
                // Reunion Moments
                PrismaticHelperImports.Register("ch17_theo_reunion", TheoReunion);
                PrismaticHelperImports.Register("ch17_oshiro_return", OshiroReturn);
                PrismaticHelperImports.Register("ch17_badeline_reflection", BadelineReflection);
                
                // Homecoming
                PrismaticHelperImports.Register("ch17_celeste_base", ReachingCelesteBase);
                PrismaticHelperImports.Register("ch17_final_farewells", FinalFarewells);
                PrismaticHelperImports.Register("ch17_home_arrival", HomeArrival);
                PrismaticHelperImports.Register("ch17_credits_finale", CreditsFinale);
                
                // Special Character Moments
                PrismaticHelperImports.Register("ch17_kirby_farewell", KirbyFarewell);
                PrismaticHelperImports.Register("ch17_chara_reflection", CharaReflection);
                PrismaticHelperImports.Register("ch17_flowey_redemption", FloweyRedemption);
            }
        }
        #endregion

        #region Journey Beginning Sequences
        
        private static IEnumerator DescentBegins(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: The Descent Begins");
            
            player.StateMachine.State = Player.StDummy;
            
            // Camera pan showing the vast mountain behind
            yield return CameraPanMountain(level);
            
            // Madeline looks back one last time
            player.Facing = Facings.Right;
            yield return 1.5f;
            
            // Dialog: Reflection on the journey
            yield return Textbox.Say("ch17_descent_begins");
            
            // Turn forward toward home
            player.Facing = Facings.Left;
            yield return 0.5f;
            
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator LeavingMountZantas(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Leaving Mount Desolo Zantas");
            
            player.StateMachine.State = Player.StDummy;
            
            // Fade effect as we leave the mystical mountain
            yield return FadeTransition(level, 2.0f);
            
            // Show all companions gathered
            yield return SpawnCompanions(level);
            
            // Dialog: Saying goodbye to the mountain
            yield return Textbox.Say("ch17_leaving_zantas");
            
            // Start gentle descent music
            Audio.SetMusic("event:/music/lvl17/descent_journey");
            
            yield return 1.0f;
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Credits Sequences

        private static IEnumerator CreditsIntroduction(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Credits Introduction");
            
            // Start credits roll with custom formatting
            CreditsText credits = new CreditsText();
            level.Add(credits);
            
            yield return credits.ShowSection("A Journey Beyond Mountains", 3.0f);
            yield return credits.ShowSection("Mount Desolo Zantas", 2.5f);
            yield return credits.ShowBlank(1.0f);
            
            // Continue player movement during credits
            yield return null;
        }

        private static IEnumerator ThankingCompanions(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Thanking Companions");
            
            player.StateMachine.State = Player.StDummy;
            
            // Credits show character contributions
            CreditsText credits = level.Entities.FindFirst<CreditsText>();
            if (credits != null)
            {
                yield return credits.ShowSection("Your Companions", 2.0f);
                yield return credits.ShowCredit("Badeline", "The Shadow Within", 2.5f);
                yield return credits.ShowCredit("Theo", "The Faithful Friend", 2.5f);
                yield return credits.ShowCredit("Granny", "The Wise Guide", 2.5f);
                yield return credits.ShowCredit("Kirby", "The Dream Friend", 2.5f);
                yield return credits.ShowCredit("Chara", "The Determined Soul", 2.5f);
                yield return credits.ShowBlank(1.0f);
            }
            
            // Dialog: Madeline's gratitude
            yield return Textbox.Say("ch17_thanking_companions");
            
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator RememberingTheJourney(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Remembering the Journey");
            
            // Flashback moments with credits
            CreditsText credits = level.Entities.FindFirst<CreditsText>();
            if (credits != null)
            {
                yield return credits.ShowSection("Challenges Overcome", 2.0f);
                yield return credits.ShowCredit("The Frozen Peaks", "Where courage was tested", 3.0f);
                yield return credits.ShowCredit("The Crystal Caverns", "Where truth was found", 3.0f);
                yield return credits.ShowCredit("The Nightmare Realm", "Where fears were faced", 3.0f);
                yield return credits.ShowCredit("The Final Confrontation", "Where hope prevailed", 3.0f);
                yield return credits.ShowBlank(1.5f);
            }
            
            // Visual effects: memories floating by
            yield return SpawnMemoryParticles(level);
            
            yield return 2.0f;
        }

        private static IEnumerator CrossingTheBarrier(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Crossing the Barrier");
            
            player.StateMachine.State = Player.StDummy;
            
            // The magical barrier between worlds dissolves
            yield return BarrierDissolveEffect(level);
            
            // Dialog: Transition from mystical to familiar
            yield return Textbox.Say("ch17_crossing_barrier");
            
            // Scenery gradually shifts from mystical to natural
            yield return TransitionScenery(level, "mystical", "natural", 4.0f);
            
            yield return 1.0f;
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Mountain Descent Phases

        private static IEnumerator UpperSlopesDescent(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Upper Slopes Descent");
            
            // Credits continue with development team
            CreditsText credits = level.Entities.FindFirst<CreditsText>();
            if (credits != null)
            {
                yield return credits.ShowSection("Created By", 2.0f);
                yield return credits.ShowCredit("Lead Developer", "Magedeline", 2.5f);
                yield return credits.ShowBlank(1.0f);
            }
            
            // Gentle snow falling
            HiresSnow snow = new HiresSnow();
            level.Add(snow);
            
            // Player walks peacefully downward
            yield return 3.0f;
        }

        private static IEnumerator CrystalCavesDescent(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Crystal Caves Descent");
            
            // Pass through beautiful crystal formations
            yield return SpawnCrystalEffects(level);
            
            CreditsText credits = level.Entities.FindFirst<CreditsText>();
            if (credits != null)
            {
                yield return credits.ShowSection("Special Thanks", 2.0f);
                yield return credits.ShowCredit("Celeste Community", "For endless inspiration", 3.0f);
                yield return credits.ShowCredit("Mod Helpers", "For amazing tools", 3.0f);
                yield return credits.ShowBlank(1.0f);
            }
            
            yield return 2.0f;
        }

        private static IEnumerator ForestPathDescent(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Forest Path Descent");
            
            // Lush forest sounds and visuals
            Audio.Play("event:/env/local/09_hub/birds");
            yield return SpawnForestBackdrop(level);
            
            // Credits continue
            CreditsText credits = level.Entities.FindFirst<CreditsText>();
            if (credits != null)
            {
                yield return credits.ShowSection("Music & Sound", 2.0f);
                yield return credits.ShowCredit("Original Celeste OST", "Lena Raine", 3.0f);
                yield return credits.ShowCredit("Additional Music", "Various Artists", 2.5f);
                yield return credits.ShowBlank(1.0f);
            }
            
            yield return 2.5f;
        }

        private static IEnumerator ValleyCrossing(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Valley Crossing");
            
            player.StateMachine.State = Player.StDummy;
            
            // Scenic vista of the valley
            yield return CameraPanValley(level);
            
            // Dialog: Almost home
            yield return Textbox.Say("ch17_valley_crossing");
            
            yield return 1.0f;
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Reunion Sequences

        private static IEnumerator TheoReunion(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Theo Reunion");
            
            player.StateMachine.State = Player.StDummy;
            
            // Theo appears
            Vector2 theoPos = player.Position + new Vector2(80, 0);
            // TODO: Spawn Theo NPC entity
            
            yield return player.DummyWalkToExact((int)theoPos.X - 15);
            
            // Dialog: Theo's enthusiasm
            yield return Textbox.Say("ch17_theo_reunion");
            
            yield return 1.0f;
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator OshiroReturn(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Oshiro Returns to Hotel");
            
            player.StateMachine.State = Player.StDummy;
            
            // Oshiro bids farewell at his hotel
            yield return Textbox.Say("ch17_oshiro_return");
            
            // Credits: Thank Oshiro
            CreditsText credits = level.Entities.FindFirst<CreditsText>();
            if (credits != null)
            {
                yield return credits.ShowCredit("Mr. Oshiro", "The Meticulous Host", 2.5f);
            }
            
            yield return 1.0f;
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator BadelineReflection(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Badeline Reflection");
            
            player.StateMachine.State = Player.StDummy;
            
            // Find or spawn Badeline
            BadelineDummy badeline = level.Entities.FindFirst<BadelineDummy>();
            if (badeline == null)
            {
                badeline = new BadelineDummy(player.Position + new Vector2(30, -10));
                level.Add(badeline);
            }
            
            // Special moment with Badeline
            yield return Textbox.Say("ch17_badeline_reflection");
            
            // Badeline merges back with Madeline (visual effect)
            yield return BadelineMergeEffect(level, badeline);
            
            yield return 1.5f;
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Special Character Farewells

        private static IEnumerator KirbyFarewell(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Kirby's Farewell");
            
            player.StateMachine.State = Player.StDummy;
            
            // Kirby prepares to return to Dream Land
            yield return Textbox.Say("ch17_kirby_farewell");
            
            // Kirby warps away with sparkles
            yield return KirbyWarpEffect(level);
            
            CreditsText credits = level.Entities.FindFirst<CreditsText>();
            if (credits != null)
            {
                yield return credits.ShowCredit("Kirby", "The Pink Puffball Hero", 2.5f);
                yield return credits.ShowCredit("From Dream Land", "With love and courage", 2.0f);
            }
            
            yield return 1.0f;
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator CharaReflection(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Chara's Reflection");
            
            player.StateMachine.State = Player.StDummy;
            
            // Chara's final words of wisdom
            yield return Textbox.Say("ch17_chara_reflection");
            
            CreditsText credits = level.Entities.FindFirst<CreditsText>();
            if (credits != null)
            {
                yield return credits.ShowCredit("Chara", "The First Human", 2.5f);
                yield return credits.ShowCredit("Determination", "Never give up", 2.0f);
            }
            
            yield return 1.0f;
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator FloweyRedemption(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Flowey's Redemption");
            
            player.StateMachine.State = Player.StDummy;
            
            // Flowey's final transformation - returning to innocence
            yield return Textbox.Say("ch17_flowey_redemption");
            
            // Flowey transforms back to his true form briefly
            yield return FloweyTransformEffect(level);
            
            yield return 1.0f;
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Homecoming Sequences

        private static IEnumerator ReachingCelesteBase(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Reaching Celeste Mountain Base");
            
            player.StateMachine.State = Player.StDummy;
            
            // Familiar sights of Celeste Mountain
            yield return CameraPanCelesteMountain(level);
            
            // Dialog: We're home
            yield return Textbox.Say("ch17_celeste_base");
            
            // Credits: Final thanks
            CreditsText credits = level.Entities.FindFirst<CreditsText>();
            if (credits != null)
            {
                yield return credits.ShowSection("Thank You For Playing", 3.0f);
                yield return credits.ShowBlank(1.0f);
            }
            
            yield return 1.0f;
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator FinalFarewells(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Final Farewells");
            
            player.StateMachine.State = Player.StDummy;
            
            // All companions gather for one last goodbye
            yield return Textbox.Say("ch17_final_farewells");
            
            // Group photo moment
            yield return GroupPhotoMoment(level);
            
            yield return 2.0f;
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator HomeArrival(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Arriving Home");
            
            player.StateMachine.State = Player.StDummy;
            
            // Walk to the final destination
            Vector2 homePos = player.Position + new Vector2(200, 0);
            yield return player.DummyWalkToExact((int)homePos.X);
            
            // Dialog: Home at last
            yield return Textbox.Say("ch17_home_arrival");
            
            // Fade to white, peaceful
            yield return FadeToWhite(level, 3.0f);
            
            yield return 1.0f;
            player.StateMachine.State = Player.StNormal;
        }

        private static IEnumerator CreditsFinale(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 17: Credits Finale");
            
            // Final credits roll
            CreditsText credits = level.Entities.FindFirst<CreditsText>();
            if (credits != null)
            {
                yield return credits.ShowSection("A Mod by Magedeline", 3.0f);
                yield return credits.ShowBlank(1.0f);
                yield return credits.ShowSection("Based on Celeste", 2.0f);
                yield return credits.ShowCredit("Created by", "Maddy Makes Games", 2.5f);
                yield return credits.ShowBlank(2.0f);
                yield return credits.ShowSection("Featuring Characters From", 2.0f);
                yield return credits.ShowCredit("Undertale", "Toby Fox", 2.5f);
                yield return credits.ShowCredit("Kirby Series", "HAL Laboratory & Nintendo", 2.5f);
                yield return credits.ShowBlank(2.0f);
                yield return credits.ShowSection("Thank you for climbing", 2.0f);
                yield return credits.ShowSection("Mount Desolo Zantas", 2.5f);
                yield return credits.ShowBlank(3.0f);
                yield return credits.ShowSection("The journey continues...", 3.0f);
            }
            
            yield return 5.0f;
            
            // Transition to post-credits or menu
            Audio.SetMusic(null);
            yield return 1.0f;
        }

        #endregion

        #region Helper Methods & Effects

        private static IEnumerator CameraPanMountain(Level level)
        {
            Vector2 startPos = level.Camera.Position;
            Vector2 endPos = startPos + new Vector2(0, -100);
            
            float duration = 3.0f;
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                level.Camera.Position = Vector2.Lerp(startPos, endPos, Ease.SineInOut(t / duration));
                yield return null;
            }
            
            yield return 1.0f;
            
            // Pan back
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                level.Camera.Position = Vector2.Lerp(endPos, startPos, Ease.SineInOut(t / duration));
                yield return null;
            }
        }

        private static IEnumerator FadeTransition(Level level, float duration)
        {
            // Use FadeWipe for proper fade effect
            FadeWipe wipe = new FadeWipe(level, false);
            wipe.Duration = duration;
            
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                yield return null;
            }
        }

        private static IEnumerator SpawnCompanions(Level level)
        {
            // This would spawn visual representations of companions
            // Implementation depends on available NPC entities
            yield return null;
        }

        private static IEnumerator SpawnMemoryParticles(Level level)
        {
            // Spawn floating memory particle effects
            for (int i = 0; i < 20; i++)
            {
                Vector2 pos = new Vector2(
                    level.Camera.Left + Calc.Random.NextFloat(320),
                    level.Camera.Top + Calc.Random.NextFloat(180)
                );
                
                level.ParticlesFG.Emit(P_Glow, pos);
                yield return 0.1f;
            }
        }

        private static IEnumerator BarrierDissolveEffect(Level level)
        {
            // Visual effect of magical barrier dissolving
            Audio.Play("event:/new_content/game/10_farewell/glider_emancipate");
            
            for (int i = 0; i < 50; i++)
            {
                Vector2 pos = new Vector2(
                    level.Camera.X + 160,
                    level.Camera.Top + Calc.Random.NextFloat(180)
                );
                level.ParticlesFG.Emit(P_Glow, pos);
                yield return 0.05f;
            }
        }

        private static IEnumerator TransitionScenery(Level level, string from, string to, float duration)
        {
            // Gradually transition backdrop from mystical to natural
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                yield return null;
            }
        }

        private static IEnumerator SpawnCrystalEffects(Level level)
        {
            // Add crystalline shimmer effects
            Audio.Play("event:/env/local/09_core/crystals_blue_on");
            yield return null;
        }

        private static IEnumerator SpawnForestBackdrop(Level level)
        {
            // Add forest backdrop if not already present
            yield return null;
        }

        private static IEnumerator CameraPanValley(Level level)
        {
            Vector2 startPos = level.Camera.Position;
            Vector2 endPos = startPos + new Vector2(100, 50);
            
            float duration = 4.0f;
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                level.Camera.Position = Vector2.Lerp(startPos, endPos, Ease.SineInOut(t / duration));
                yield return null;
            }
            
            yield return 1.5f;
            
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                level.Camera.Position = Vector2.Lerp(endPos, startPos, Ease.SineInOut(t / duration));
                yield return null;
            }
        }

        private static IEnumerator BadelineMergeEffect(Level level, BadelineDummy badeline)
        {
            // Badeline merges back into Madeline
            Audio.Play("event:/char/badeline/disappear");
            
            if (badeline != null)
            {
                for (float t = 0; t < 1.0f; t += Engine.DeltaTime)
                {
                    badeline.Sprite.Color = Color.White * (1.0f - t);
                    yield return null;
                }
                level.Remove(badeline);
            }
        }

        private static IEnumerator KirbyWarpEffect(Level level)
        {
            // Kirby's warp star effect
            Audio.Play("event:/game/general/cassette_get");
            
            // Spawn warp star particles
            for (int i = 0; i < 30; i++)
            {
                Vector2 pos = level.Camera.Position + new Vector2(160, 90);
                level.ParticlesFG.Emit(P_Glow, pos);
                yield return 0.03f;
            }
        }

        // Particle type for effects
        private static ParticleType P_Glow = new ParticleType
        {
            Color = Color.White,
            FadeMode = ParticleType.FadeModes.Linear,
            Size = 1f,
            SizeRange = 0.5f,
            DirectionRange = (float)Math.PI / 4f,
            LifeMin = 0.5f,
            LifeMax = 1.5f,
            SpeedMin = 10f,
            SpeedMax = 40f,
            Acceleration = Vector2.UnitY * 10f
        };

        private static IEnumerator AsrielFadeEffect(Level level)
        {
            // Peaceful fade away effect
            Audio.Play("event:/new_content/game/10_farewell/bird_reset_spirit");
            yield return 2.0f;
        }

        private static IEnumerator FloweyTransformEffect(Level level)
        {
            // Flowey transformation visual
            Audio.Play("event:/game/general/seed_complete");
            yield return 1.5f;
        }

        private static IEnumerator CameraPanCelesteMountain(Level level)
        {
            // Pan to show the familiar Celeste Mountain
            Vector2 startPos = level.Camera.Position;
            Vector2 endPos = startPos + new Vector2(-50, -80);
            
            float duration = 5.0f;
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                level.Camera.Position = Vector2.Lerp(startPos, endPos, Ease.SineInOut(t / duration));
                yield return null;
            }
            
            yield return 2.0f;
            
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                level.Camera.Position = Vector2.Lerp(endPos, startPos, Ease.SineInOut(t / duration));
                yield return null;
            }
        }

        private static IEnumerator GroupPhotoMoment(Level level)
        {
            // Freeze frame moment with all characters
            Audio.Play("event:/ui/game/icon_change");
            
            // Flash effect
            level.Flash(Color.White);
            
            yield return 1.5f;
        }

        private static IEnumerator FadeToWhite(Level level, float duration)
        {
            ScreenWipe wipe = new FadeWipe(level, false);
            wipe.Duration = duration;
            
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                yield return null;
            }
        }

        #endregion

        #region Credits Display Helper Class

        private class CreditsText : Entity
        {
            private List<CreditLine> lines = new List<CreditLine>();
            private float scrollSpeed = 20f;

            public CreditsText() : base()
            {
                Tag = Tags.HUD;
                Depth = -1000000;
            }

            public IEnumerator ShowSection(string text, float duration)
            {
                CreditLine line = new CreditLine(text, true);
                lines.Add(line);
                yield return duration;
            }

            public IEnumerator ShowCredit(string role, string name, float duration)
            {
                CreditLine line = new CreditLine($"{role}\n{name}", false);
                lines.Add(line);
                yield return duration;
            }

            public IEnumerator ShowBlank(float duration)
            {
                CreditLine line = new CreditLine("", false);
                lines.Add(line);
                yield return duration;
            }

            public override void Update()
            {
                base.Update();
                
                // Scroll credits upward
                foreach (var line in lines)
                {
                    line.Y -= scrollSpeed * Engine.DeltaTime;
                }
                
                // Remove offscreen credits
                lines.RemoveAll(l => l.Y < -100);
            }

            public override void Render()
            {
                base.Render();
                
                foreach (var line in lines)
                {
                    line.Render();
                }
            }

            private class CreditLine
            {
                public string Text;
                public bool IsHeader;
                public float Y;
                public float Alpha = 1.0f;

                public CreditLine(string text, bool isHeader)
                {
                    Text = text;
                    IsHeader = isHeader;
                    Y = 1080; // Start at bottom of screen
                }

                public void Render()
                {
                    if (string.IsNullOrEmpty(Text)) return;
                    
                    Vector2 position = new Vector2(960, Y); // Center of 1920x1080
                    Color color = Color.White * Alpha;
                    
                    if (IsHeader)
                    {
                        ActiveFont.DrawOutline(Text, position, new Vector2(0.5f, 0.5f),
                            Vector2.One * 1.2f, color, 2f, Color.Black);
                    }
                    else
                    {
                        ActiveFont.DrawOutline(Text, position, new Vector2(0.5f, 0.5f),
                            Vector2.One, color, 2f, Color.Black);
                    }
                }
            }
        }

        #endregion
    }
}




