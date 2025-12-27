using DesoloZantas.Core.Core;
using MonoMod.ModInterop;

namespace DesoloZantas.Core.Cutscenes
{
    /// <summary>
    /// Chapter 20: The End - Final Confrontation with Asriel
    /// Contains cutscenes for Asriel's transformation from resurrected kid to God of Hyperdeath
    /// and the final redemption sequence
    /// </summary>
    public static class Chapter20Cutscenes
    {
        [ModImportName("PrismaticHelper.CutsceneTriggers")]
        public static class PrismaticHelperImports
        {
            public static Action<string, Func<Player, Level, List<string>, IEnumerator>> Register;
        }

        #region Cutscene Registration
        public static void LoadCutscenes()
        {
            if (PrismaticHelperImports.Register != null)
            {
                // Asriel Reveal and Identity Cutscenes
                PrismaticHelperImports.Register("ch20_asriel_reveal_identity", AsrielRevealIdentity);
                PrismaticHelperImports.Register("ch20_asriel_dark_shell_break", AsrielDarkShellBreak);
                PrismaticHelperImports.Register("ch20_asriel_form_shift", AsrielFormShift);
                
                // God Transformation Sequence
                PrismaticHelperImports.Register("ch20_asriel_god_transform", AsrielGodTransformation);
                PrismaticHelperImports.Register("ch20_hyperdeath_declaration", HyperdeathDeclaration);
                
                // Battle Progression
                PrismaticHelperImports.Register("ch20_heart_refusal", HeartRefusalSequence);
                PrismaticHelperImports.Register("ch20_kirby_revival", KirbyRevivalMoment);
                PrismaticHelperImports.Register("ch20_madeline_badeline_return", MadelineBadelineReturn);
                
                // Asriel Zero Transformation
                PrismaticHelperImports.Register("ch20_asriel_zero_transform", AsrielZeroTransformation);
                PrismaticHelperImports.Register("ch20_reality_tear", RealityTearSequence);
                
                // Soul Calling Sequence
                PrismaticHelperImports.Register("ch20_call_souls", CallingSoulsSequence);
                PrismaticHelperImports.Register("ch20_void_answer", VoidAnswerCall);
                
                // Asriel Remember Sequence
                PrismaticHelperImports.Register("ch20_asriel_remember_a", AsrielRememberA);
                PrismaticHelperImports.Register("ch20_asriel_remember_b", AsrielRememberB);
                PrismaticHelperImports.Register("ch20_asriel_remember_c", AsrielRememberC);
                PrismaticHelperImports.Register("ch20_asriel_remember_d", AsrielRememberD);
                PrismaticHelperImports.Register("ch20_asriel_remember_e", AsrielRememberE);
                PrismaticHelperImports.Register("ch20_asriel_remember_final", AsrielRememberFinal);
                
                // Redemption and Ending
                PrismaticHelperImports.Register("ch20_asriel_redemption", AsrielRedemption);
                PrismaticHelperImports.Register("ch20_boss_end", AsrielBossEnd);
                PrismaticHelperImports.Register("ch20_els_reveal", ElsReveal);
            }
        }
        #endregion

        #region Asriel Reveal Sequence

        /// <summary>
        /// Main reveal cutscene where Asriel's identity is revealed from Els
        /// </summary>
        private static IEnumerator AsrielRevealIdentity(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Reveal Identity");
            
            player.StateMachine.State = Player.StDummy;
            
            // Initial shock - Kirby recognizes the voice
            yield return 0.5f;
            
            // Camera focuses on the breaking dark shell
            yield return CameraFocusOnAsriel(level);
            
            yield return 1f;
        }

        /// <summary>
        /// The dark shell breaks revealing Asriel underneath
        /// </summary>
        private static IEnumerator AsrielDarkShellBreak(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Dark Shell Breaking");
            
            // Find Asriel entity
            var asriel = FindAsrielEntity(level);
            
            // Screen effects for shell breaking
            level.Flash(Color.White * 0.5f, true);
            level.DirectionalShake(Vector2.UnitY, 1.5f);
            
            // Spawn particle effects for shell fragments
            yield return SpawnShellBreakParticles(level, asriel?.Position ?? player.Position + new Vector2(100, -50));
            
            // Play shell break sound
            Audio.Play("event:/sfx/desolozantas/shell_break", asriel?.Position ?? level.Camera.Position);
            
            // Transition asriel sprite from Els to revealed form
            if (asriel != null)
            {
                yield return TransitionToAsrielForm(asriel);
            }
            
            yield return 0.5f;
        }

        /// <summary>
        /// Asriel's form shifts between true self and Els during internal conflict
        /// </summary>
        private static IEnumerator AsrielFormShift(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Form Shift");
            
            var asriel = FindAsrielEntity(level);
            
            // Flickering effect between forms
            for (int i = 0; i < 5; i++)
            {
                // Flash to Els form
                level.Flash(Color.Red * 0.3f, false);
                yield return 0.2f;
                
                // Flash to Asriel form
                level.Flash(Color.White * 0.3f, false);
                yield return 0.2f;
            }
            
            // Final stabilization shake
            level.DirectionalShake(Vector2.One, 1f);
            
            yield return 0.5f;
        }

        #endregion

        #region God Transformation Sequence

        /// <summary>
        /// Asriel transforms into the God of Hyperdeath form
        /// </summary>
        private static IEnumerator AsrielGodTransformation(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel God Transformation");
            
            player.StateMachine.State = Player.StDummy;
            
            // Dramatic zoom out
            yield return CameraZoomOut(level, 1.5f, 2f);
            
            // Screen shake building up
            for (float t = 0; t < 3f; t += Engine.DeltaTime)
            {
                level.DirectionalShake(Vector2.One * (t / 3f), 0.1f);
                yield return null;
            }
            
            // Bright flash for transformation
            level.Flash(Color.Gold, true);
            Audio.Play("event:/sfx/desolozantas/god_transform");
            
            // Spawn divine particles
            yield return SpawnDivineTransformationParticles(level);
            
            // Change music to god boss theme
            Audio.SetMusic("event:/music/desolozantas/god_of_hyperdeath");
            
            yield return 1f;
            
            // Zoom back
            yield return CameraZoomReset(level, 1f);
        }

        /// <summary>
        /// Asriel declares himself as the God of Hyperdeath
        /// </summary>
        private static IEnumerator HyperdeathDeclaration(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Hyperdeath Declaration");
            
            // Dramatic camera angle
            yield return CameraAngleShift(level, new Vector2(0, -30), 1f);
            
            // Divine aura effect
            yield return SpawnDivineAura(level);
            
            // Screen shake on each declaration line
            level.DirectionalShake(Vector2.UnitY, 2f);
            
            yield return 0.5f;
        }

        #endregion

        #region Battle Sequence

        /// <summary>
        /// Kirby's heart refuses to break during the battle
        /// </summary>
        private static IEnumerator HeartRefusalSequence(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Heart Refusal Sequence");
            
            // Slow motion effect
            Engine.TimeRate = 0.3f;
            
            // Heart glow effect on player
            yield return SpawnHeartGlowEffect(level, player.Position);
            
            yield return 1f;
            
            // Restore time
            Engine.TimeRate = 1f;
            
            // Power surge effect
            level.Flash(Color.Red * 0.5f, false);
            Audio.Play("event:/sfx/desolozantas/heart_pulse");
            
            yield return 0.5f;
        }

        /// <summary>
        /// Kirby revives stronger than before
        /// </summary>
        private static IEnumerator KirbyRevivalMoment(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Kirby Revival");
            
            // Rising animation
            player.StateMachine.State = Player.StDummy;
            
            // Healing light effect
            yield return SpawnHealingLightParticles(level, player.Position);
            
            // Player gets up animation
            yield return 1f;
            
            player.StateMachine.State = Player.StNormal;
        }

        /// <summary>
        /// Madeline and Badeline return as spirits
        /// </summary>
        private static IEnumerator MadelineBadelineReturn(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Madeline and Badeline Spirit Return");
            
            player.StateMachine.State = Player.StDummy;
            
            // Brilliant light tears through
            level.Flash(new Color(170, 170, 255), true);
            level.DirectionalShake(Vector2.UnitY, 3f);
            
            // Camera zoom for dramatic entrance
            yield return CameraZoomIn(level, 1.5f, 2f);
            
            // Spawn spirit entities
            yield return SpawnSpiritEntities(level, player.Position);
            
            // Sparkle particles
            yield return SpawnSparkleParticles(level);
            
            yield return 1f;
            
            // Zoom back
            yield return CameraZoomReset(level, 2.5f);
            
            player.StateMachine.State = Player.StNormal;
        }

        #endregion

        #region Asriel Zero Transformation

        /// <summary>
        /// Asriel transforms into his final Zero form
        /// </summary>
        private static IEnumerator AsrielZeroTransformation(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Zero Transformation");
            
            player.StateMachine.State = Player.StDummy;
            
            // Reality destabilization
            yield return RealityDestabilization(level, 3f);
            
            // Massive screen shake
            level.DirectionalShake(Vector2.One, 4f);
            
            // Black flash for zero form emergence
            level.Flash(Color.Black, true);
            Audio.Play("event:/sfx/desolozantas/zero_emerge");
            
            // Change music to zero theme
            Audio.SetMusic("event:/music/desolozantas/asriel_zero");
            
            yield return 2f;
        }

        /// <summary>
        /// Reality itself begins to tear
        /// </summary>
        private static IEnumerator RealityTearSequence(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Reality Tear");
            
            // Spawn reality tear visual effects
            yield return SpawnRealityTearEffects(level);
            
            // Dust particles everywhere
            for (int i = 0; i < 50; i++)
            {
                Vector2 pos = level.Camera.Position + new Vector2(
                    Calc.Random.NextFloat(320),
                    Calc.Random.NextFloat(180)
                );
                level.ParticlesFG.Emit(ParticleTypes.Dust, pos, Color.Gray);
            }
            
            yield return 1f;
        }

        #endregion

        #region Soul Calling Sequence

        /// <summary>
        /// Calling out to souls for help
        /// </summary>
        private static IEnumerator CallingSoulsSequence(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Calling Souls");
            
            player.StateMachine.State = Player.StDummy;
            
            // Each soul call - brief flash and appearance
            string[] soulNames = { "Magolor", "Chara", "Theo", "Oshiro", "Toriel" };
            Color[] soulColors = { Color.Purple, Color.Red, Color.Orange, Color.Green, Color.White };
            
            for (int i = 0; i < soulNames.Length; i++)
            {
                level.Flash(soulColors[i] * 0.5f, false);
                Audio.Play("event:/sfx/desolozantas/soul_appear");
                yield return 0.3f;
            }
            
            yield return 0.5f;
        }

        /// <summary>
        /// Void answers the call for help
        /// </summary>
        private static IEnumerator VoidAnswerCall(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Void Answer");
            
            // Dramatic pause
            yield return 1f;
            
            // Void power emergence
            level.Flash(Color.Black, true);
            yield return 0.5f;
            level.Flash(Color.White, true);
            
            Audio.Play("event:/sfx/desolozantas/void_power");
            
            yield return 1f;
        }

        #endregion

        #region Asriel Remember Sequence

        private static IEnumerator AsrielRememberA(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Remember A");
            level.Flash(Color.Purple * 0.3f, false);
            yield return 0.5f;
        }

        private static IEnumerator AsrielRememberB(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Remember B");
            level.DirectionalShake(Vector2.One, 2f);
            level.Flash(Color.Red * 0.5f, false);
            yield return 0.5f;
        }

        private static IEnumerator AsrielRememberC(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Remember C");
            level.Flash(Color.White * 0.4f, false);
            yield return 0.5f;
        }

        private static IEnumerator AsrielRememberD(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Remember D");
            level.Flash(Color.Pink * 0.5f, false);
            yield return 0.5f;
        }

        private static IEnumerator AsrielRememberE(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Remember E");
            level.DirectionalShake(Vector2.UnitY, 1.5f);
            yield return 0.5f;
        }

        /// <summary>
        /// Final remember sequence with beam attack holding
        /// </summary>
        private static IEnumerator AsrielRememberFinal(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Remember Final");
            
            // Building desperation
            for (int i = 0; i < 3; i++)
            {
                level.DirectionalShake(Vector2.One * (i + 1) * 0.5f, 0.5f);
                level.Flash(Color.White * (0.2f + i * 0.1f), false);
                yield return 0.3f;
            }
            
            // Final breakdown
            level.Flash(Color.White, true);
            level.DirectionalShake(Vector2.One, 3f);
            
            yield return 1f;
        }

        #endregion

        #region Redemption Sequence

        /// <summary>
        /// Asriel's redemption after memories return
        /// </summary>
        private static IEnumerator AsrielRedemption(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Redemption");
            
            // Calm after storm
            Audio.SetMusic("event:/music/desolozantas/asriel_redemption");
            
            // Soft light
            level.Flash(new Color(136, 0, 136) * 0.5f, false);
            
            yield return 1f;
        }

        /// <summary>
        /// End of boss fight - Asriel reverts to kid form
        /// </summary>
        private static IEnumerator AsrielBossEnd(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Asriel Boss End");
            
            player.StateMachine.State = Player.StDummy;
            
            // Transformation back to kid - gentle light
            level.Flash(Color.White * 0.7f, true);
            
            // Spawn kid Asriel entity
            yield return SpawnAsrielKidEntity(level);
            
            yield return 1f;
            
            player.StateMachine.State = Player.StNormal;
        }

        /// <summary>
        /// Els darkness entity reveals itself
        /// </summary>
        private static IEnumerator ElsReveal(Player player, Level level, List<string> parameters)
        {
            IngesteLogger.Info("Chapter 20: Els Reveal");
            
            // Dark presence
            level.Flash(Color.Black * 0.8f, false);
            Audio.Play("event:/sfx/desolozantas/els_reveal");
            
            // Spawn Els entity separate from Asriel
            yield return SpawnElsEntity(level);
            
            yield return 0.5f;
        }

        #endregion

        #region Helper Methods

        private static Entity FindAsrielEntity(Level level)
        {
            // Try to find asriel boss entity
            foreach (Entity entity in level.Entities)
            {
                if (entity.GetType().Name.Contains("Asriel"))
                    return entity;
            }
            return null;
        }

        private static IEnumerator CameraFocusOnAsriel(Level level)
        {
            // Pan camera to asriel position
            yield return 1f;
        }

        private static IEnumerator CameraZoomOut(Level level, float targetZoom, float duration)
        {
            yield return duration;
        }

        private static IEnumerator CameraZoomIn(Level level, float targetZoom, float duration)
        {
            yield return duration;
        }

        private static IEnumerator CameraZoomReset(Level level, float duration)
        {
            yield return duration;
        }

        private static IEnumerator CameraAngleShift(Level level, Vector2 offset, float duration)
        {
            yield return duration;
        }

        private static IEnumerator SpawnShellBreakParticles(Level level, Vector2 position)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 dir = Calc.AngleToVector(Calc.Random.NextFloat() * MathHelper.TwoPi, Calc.Random.Range(20, 80));
                level.ParticlesFG.Emit(ParticleTypes.Dust, position + dir * 0.3f, dir.Angle());
            }
            yield return 0.5f;
        }

        private static IEnumerator TransitionToAsrielForm(Entity asriel)
        {
            yield return 1f;
        }

        private static IEnumerator SpawnDivineTransformationParticles(Level level)
        {
            for (int i = 0; i < 50; i++)
            {
                Vector2 pos = level.Camera.Position + new Vector2(
                    Calc.Random.NextFloat(320),
                    Calc.Random.NextFloat(180)
                );
                level.ParticlesFG.Emit(ParticleTypes.Dust, pos, Color.Gold);
            }
            yield return 1f;
        }

        private static IEnumerator SpawnDivineAura(Level level)
        {
            yield return 0.5f;
        }

        private static IEnumerator SpawnHeartGlowEffect(Level level, Vector2 position)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = new Vector2(Calc.Random.Range(-15, 15), Calc.Random.Range(-15, 15));
                level.ParticlesFG.Emit(ParticleTypes.Dust, position + offset, Color.Red);
            }
            yield return 0.5f;
        }

        private static IEnumerator SpawnHealingLightParticles(Level level, Vector2 position)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 offset = new Vector2(Calc.Random.Range(-20, 20), Calc.Random.Range(-30, 0));
                level.ParticlesFG.Emit(ParticleTypes.Dust, position + offset, Color.White);
            }
            yield return 0.5f;
        }

        private static IEnumerator SpawnSpiritEntities(Level level, Vector2 playerPos)
        {
            // Spawn Madeline and Badeline spirit entities near player
            yield return 1f;
        }

        private static IEnumerator SpawnSparkleParticles(Level level)
        {
            for (int i = 0; i < 40; i++)
            {
                Vector2 pos = level.Camera.Position + new Vector2(
                    Calc.Random.NextFloat(320),
                    Calc.Random.NextFloat(180)
                );
                level.ParticlesFG.Emit(ParticleTypes.Dust, pos, new Color(170, 170, 255));
            }
            yield return 0.5f;
        }

        private static IEnumerator RealityDestabilization(Level level, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                level.DirectionalShake(Vector2.One * (elapsed / duration), 0.1f);
                elapsed += Engine.DeltaTime;
                yield return null;
            }
        }

        private static IEnumerator SpawnRealityTearEffects(Level level)
        {
            // Visual glitch effect
            for (int i = 0; i < 5; i++)
            {
                level.Flash(Color.White * 0.3f, false);
                yield return 0.1f;
                level.Flash(Color.Black * 0.3f, false);
                yield return 0.1f;
            }
        }

        private static IEnumerator SpawnAsrielKidEntity(Level level)
        {
            // Spawn the kid form of Asriel after redemption
            yield return 1f;
        }

        private static IEnumerator SpawnElsEntity(Level level)
        {
            // Spawn Els as separate darkness entity
            yield return 0.5f;
        }

        #endregion
    }
}
