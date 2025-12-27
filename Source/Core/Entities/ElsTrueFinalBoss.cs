using DesoloZantas.Core.BossesHelper.Entities;

namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// Els' true form - Phase 1: Doppia Elillca
    /// Phase 2: Penumbra Phastasm (true final phase)
    /// </summary>
    [CustomEntity("Ingeste/ElsTrueFinalBoss")]
    [Tracked]
    public class ElsTrueFinalBoss : BossActor
    {
        // Boss state tracking
        private int Health;
        private int MaxHealth;
        private float arenaRadius;
        
        public enum ElsPhase
        {
            DoppiaElillca = 1,  // Phase 1
            PenumbraPhastasm = 2  // Phase 2 - True Final
        }
        
        private ElsPhase currentElsPhase = ElsPhase.DoppiaElillca;
        private bool hasTransitionedToPhase2 = false;
        
        // Phase 1 (Doppia Elillca) properties
        private Sprite doppiaSprite;
        private List<Entity> shadowClones = new List<Entity>();
        private float dualityFactor = 0f;
        
        // Phase 2 (Penumbra Phastasm) properties
        private Sprite penumbraSprite;
        private List<VertexLight> phantasmLights = new List<VertexLight>();
        private float voidPower = 0f;
        private bool isInVoidMode = false;
        
        // Shared effects
        private VertexLight coreLight;
        private SineWave energyPulse;
        private Wiggler phaseWiggler;
        
        // Combat tracking
        private List<global::Celeste.Player> allies = new List<global::Celeste.Player>();
        private int teamAttackCounter = 0;
        
        // Defense and healing
        private bool isDefending = false;
        private float defenseDuration = 0f;
        private float defenseReduction = 0.75f; // 75% damage reduction when defending
        private float healCooldown = 0f;
        private const float HEAL_COOLDOWN_TIME = 15f;
        
        // Gimmick power tracking
        private float dimensionRiftPower = 0f;
        private const float MAX_RIFT_POWER = 100f;
        private bool canUseUltimateRiftAttack = false;
        
        public ElsTrueFinalBoss(EntityData data, Vector2 offset) 
            : base(
                data.Position + offset,
                "els_true_final_boss",
                Vector2.One,
                maxFall: 0f,
                collidable: true,
                solidCollidable: false,
                gravityMult: 0f,
                collider: new Hitbox(48f, 64f, -24f, -64f)
            )
        {
            // Final boss configuration - stored in properties
            MaxHealth = 3000; // Split across two phases
            Health = MaxHealth;
            arenaRadius = 400f;
            
            setupPhase1();
        }
        
        private void setupPhase1()
        {
            // Doppia Elillca form
            Add(doppiaSprite = new Sprite(GFX.Game, "characters/els/"));
            doppiaSprite.AddLoop("idle", "doppia_idle", 0.1f);
            doppiaSprite.AddLoop("attack", "doppia_attack", 0.08f);
            doppiaSprite.AddLoop("duality", "doppia_duality", 0.12f);
            doppiaSprite.Play("idle");
            doppiaSprite.CenterOrigin();
            doppiaSprite.Visible = true;
            
            // Core light (changes color between phases)
            Add(coreLight = new VertexLight(Color.Red * 1.2f, 1f, 256, 384));
            coreLight.Position = Vector2.Zero;
            
            // Energy pulse
            Add(energyPulse = new SineWave(0.5f, 0f));
            energyPulse.Randomize();
            
            // Phase wiggler
            Add(phaseWiggler = Wiggler.Create(1.2f, 4f));
        }
        
        private void setupPhase2()
        {
            // Hide phase 1 sprite
            doppiaSprite.Visible = false;
            
            // Penumbra Phastasm form
            Add(penumbraSprite = new Sprite(GFX.Game, "characters/els/"));
            penumbraSprite.AddLoop("idle", "penumbra_idle", 0.08f);
            penumbraSprite.AddLoop("attack", "penumbra_attack", 0.06f);
            penumbraSprite.AddLoop("void", "penumbra_void", 0.1f);
            penumbraSprite.AddLoop("ultimate", "penumbra_ultimate", 0.05f);
            penumbraSprite.Play("idle");
            penumbraSprite.CenterOrigin();
            penumbraSprite.Visible = true;
            
            // Create phantasm lights (multiple colored lights)
            Color[] lightColors = {
                Color.Purple, Color.DarkBlue, Color.DarkRed, 
                Color.DarkGreen, Color.Black * 0.8f
            };
            
            foreach (var color in lightColors)
            {
                var light = new VertexLight(color * 1.5f, 1f, 192, 256);
                Add(light);
                phantasmLights.Add(light);
            }
            
            // Change core light to dark void color
            coreLight.Color = new Color(0.1f, 0.0f, 0.2f) * 2f;
            coreLight.StartRadius = 384f;
        }
        
        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            // Dramatic final boss entrance
            Add(new Coroutine(finalBossEntrance()));
        }
        
        private IEnumerator finalBossEntrance()
        {
            var level = Scene as Level;
            
            // Epic entrance music
            Audio.SetMusic("event:/boss/els_true_final_theme");
            Audio.Play("event:/boss/els_true_form_reveal", Position);
            
            // Massive screen shake
            level?.Shake(3f);
            
            // Displacement effect
            level?.Displacement.AddBurst(Position, 2f, 192f, 384f, 4f);
            
            // Flash effect
            level?.Flash(Color.DarkRed, true);
            
            yield return 2f;
            
            // Voice line or announcement
            Audio.Play("event:/boss/els_doppia_roar", Position);
            
            phaseWiggler.Start();
            
            yield return 1f;
        }
        
        public override void Update()
        {
            base.Update();
            
            // Update defense duration
            if (isDefending)
            {
                defenseDuration -= Engine.DeltaTime;
                if (defenseDuration <= 0f)
                {
                    isDefending = false;
                    Audio.Play("event:/boss/els_defense_end", Position);
                }
            }
            
            // Update heal cooldown
            if (healCooldown > 0f)
            {
                healCooldown -= Engine.DeltaTime;
            }
            
            // Build dimension rift power over time
            if (dimensionRiftPower < MAX_RIFT_POWER)
            {
                dimensionRiftPower += Engine.DeltaTime * 2f;
                
                if (dimensionRiftPower >= MAX_RIFT_POWER && !canUseUltimateRiftAttack)
                {
                    canUseUltimateRiftAttack = true;
                    Audio.Play("event:/boss/els_rift_charged", Position);
                    phaseWiggler.Start();
                }
            }
            
            // Update based on current phase
            if (currentElsPhase == ElsPhase.DoppiaElillca)
            {
                updatePhase1();
            }
            else if (currentElsPhase == ElsPhase.PenumbraPhastasm)
            {
                updatePhase2();
            }
            
            // Check for phase transition
            float healthPercent = (float)Health / MaxHealth;
            if (!hasTransitionedToPhase2 && healthPercent <= 0.5f)
            {
                transitionToPhase2();
            }
            
            // Update core light
            coreLight.Alpha = 0.8f + phaseWiggler.Value * 0.4f;
        }
        
        private void updatePhase1()
        {
            // Doppia Elillca behavior
            dualityFactor = energyPulse.Value;
            
            // Oscillate sprite position for duality effect
            doppiaSprite.Position = new Vector2(
                (float)Math.Sin(dualityFactor * MathHelper.TwoPi) * 8f,
                energyPulse.Value * 6f
            );
        }
        
        private void updatePhase2()
        {
            // Penumbra Phastasm behavior
            voidPower += Engine.DeltaTime * 0.1f;
            
            // Update phantasm lights in circular pattern
            for (int i = 0; i < phantasmLights.Count; i++)
            {
                float angle = (i / (float)phantasmLights.Count) * MathHelper.TwoPi + voidPower;
                float radius = 128f + energyPulse.Value * 32f;
                
                phantasmLights[i].Position = new Vector2(
                    (float)Math.Cos(angle) * radius,
                    (float)Math.Sin(angle) * radius
                );
                
                phantasmLights[i].Alpha = 0.6f + energyPulse.Value * 0.4f;
            }
            
            // Void mode activation at low health
            if (Health <= MaxHealth * 0.2f && !isInVoidMode)
            {
                enterVoidMode();
            }
        }
        
        private void transitionToPhase2()
        {
            hasTransitionedToPhase2 = true;
            Add(new Coroutine(phase2TransitionSequence()));
        }
        
        private IEnumerator phase2TransitionSequence()
        {
            var level = Scene as Level;
            
            // Transition announcement
            Audio.Play("event:/boss/els_phase_transition", Position);
            level?.Shake(2f);
            
            // Visual transformation
            for (float t = 0f; t < 1f; t += Engine.DeltaTime * 0.3f)
            {
                float fade = Ease.CubeInOut(t);
                doppiaSprite.Color = Color.White * (1f - fade);
                
                if (t >= 0.5f && penumbraSprite == null)
                {
                    setupPhase2();
                    penumbraSprite.Color = Color.White * 0f;
                }
                
                if (penumbraSprite != null)
                {
                    penumbraSprite.Color = Color.White * fade;
                }
                
                yield return null;
            }
            
            // Full heal for phase 2 (or partial)
            Health = MaxHealth / 2; // Half health for second phase
            
            // Massive displacement burst
            level?.Displacement.AddBurst(Position, 2.5f, 256f, 512f, 3f);
            
            // Change music intensity
            Audio.Play("event:/boss/els_penumbra_reveal", Position);
            
            // Update phase
            currentElsPhase = ElsPhase.PenumbraPhastasm;
            
            phaseWiggler.Start();
            
            yield return 2f;
        }
        
        private void enterVoidMode()
        {
            isInVoidMode = true;
            penumbraSprite.Play("void");
            
            Audio.Play("event:/boss/els_void_mode", Position);
            
            // Change all lights to dark void
            foreach (var light in phantasmLights)
            {
                light.Color = Color.Black * 2f;
            }
            
            var level = Scene as Level;
            level?.Shake(1.5f);
        }
        
        // Phase 1 Attacks (Doppia Elillca)
        public void ExecuteDoppiaAttack(int attackId)
        {
            switch (attackId)
            {
                case 0:
                    doppiaCloneAssault();
                    break;
                case 1:
                    dualityWave();
                    break;
                case 2:
                    shadowBlast();
                    break;
                case 3:
                    mirrorDimension();
                    break;
                case 4:
                    dimensionalDefense();
                    break;
                case 5:
                    dualityHeal();
                    break;
                case 6:
                    riftStrikeCombo();
                    break;
            }
            
            phaseWiggler.Start();
        }
        
        private void doppiaCloneAssault()
        {
            Audio.Play("event:/boss/els_doppia_clones", Position);
            
            // Spawn shadow clones
            for (int i = 0; i < 4; i++)
            {
                float angle = (i / 4f) * MathHelper.TwoPi;
                Vector2 spawnPos = Position + new Vector2(
                    (float)Math.Cos(angle) * 150f,
                    (float)Math.Sin(angle) * 150f
                );
                // Create shadow clone entity
            }
        }
        
        private void dualityWave()
        {
            Audio.Play("event:/boss/els_doppia_duality", Position);
            
            doppiaSprite.Play("duality");
            
            // Create expanding wave of energy
            var level = Scene as Level;
            level?.Displacement.AddBurst(Position, 1f, 128f, 256f, 1.5f);
        }
        
        private void shadowBlast()
        {
            Audio.Play("event:/boss/els_doppia_blast", Position);
            
            // Fire shadow projectiles in all directions
            for (int i = 0; i < 12; i++)
            {
                float angle = (i / 12f) * MathHelper.TwoPi;
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                // Create shadow projectile
            }
        }
        
        private void mirrorDimension()
        {
            Audio.Play("event:/boss/els_doppia_mirror", Position);
            
            // Create mirror dimension effect
            var level = Scene as Level;
            level?.Flash(Color.Purple, false);
        }
        
        private void dimensionalDefense()
        {
            if (isDefending) return;
            
            Audio.Play("event:/boss/els_defense_activate", Position);
            
            isDefending = true;
            defenseDuration = 5f; // Defend for 5 seconds
            
            doppiaSprite.Play("duality");
            
            // Visual shield effect
            var level = Scene as Level;
            level?.Displacement.AddBurst(Position, 0.5f, 96f, 192f, 0.5f);
            
            // Create protective barrier particles
            for (int i = 0; i < 360; i += 15)
            {
                float angle = MathHelper.ToRadians(i);
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 80f;
                level?.ParticlesFG.Emit(global::Celeste.Player.P_DashB, 2, Position + offset, Vector2.One * 4f);
            }
            
            phaseWiggler.Start();
        }
        
        private void dualityHeal()
        {
            if (healCooldown > 0f) return;
            
            Audio.Play("event:/boss/els_heal", Position);
            
            int healAmount = (int)(MaxHealth * 0.15f); // Heal 15% of max health
            Health = Math.Min(Health + healAmount, MaxHealth);
            
            healCooldown = HEAL_COOLDOWN_TIME;
            
            doppiaSprite.Play("duality");
            
            // Healing visual effect
            var level = Scene as Level;
            level?.Displacement.AddBurst(Position, 1f, 128f, 256f, 1f);
            
            // Green healing particles
            for (int i = 0; i < 30; i++)
            {
                Vector2 randomOffset = new Vector2(
                    Calc.Random.Range(-64f, 64f),
                    Calc.Random.Range(-64f, 64f)
                );
                level?.ParticlesFG.Emit(global::Celeste.Player.P_DashA, 3, Position + randomOffset, Vector2.One * 8f, Color.Green);
            }
            
            phaseWiggler.Start();
        }
        
        private void riftStrikeCombo()
        {
            if (dimensionRiftPower < 30f) return;
            
            Audio.Play("event:/boss/els_rift_strike", Position);
            
            dimensionRiftPower -= 30f; // Cost 30 rift power
            
            doppiaSprite.Play("attack");
            
            // Multi-hit combo using dimension rift
            Add(new Coroutine(riftStrikeSequence()));
        }
        
        private IEnumerator riftStrikeSequence()
        {
            var level = Scene as Level;
            
            for (int hit = 0; hit < 5; hit++)
            {
                // Teleport to random position near center
                Vector2 targetPos = Position + new Vector2(
                    Calc.Random.Range(-arenaRadius * 0.5f, arenaRadius * 0.5f),
                    Calc.Random.Range(-arenaRadius * 0.5f, arenaRadius * 0.5f)
                );
                
                // Rift visual
                level?.Displacement.AddBurst(Position, 0.5f, 64f, 128f, 0.3f);
                
                Position = targetPos;
                
                // Strike effect
                level?.Displacement.AddBurst(Position, 0.8f, 96f, 192f, 0.5f);
                Audio.Play("event:/boss/els_rift_impact", Position);
                
                // Create projectiles
                for (int i = 0; i < 8; i++)
                {
                    float angle = (i / 8f) * MathHelper.TwoPi;
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    // Create rift projectile
                }
                
                yield return 0.2f;
            }
        }
        
        // Phase 2 Attacks (Penumbra Phastasm)
        public void ExecutePenumbraAttack(int attackId)
        {
            switch (attackId)
            {
                case 0:
                    penumbraVoidStorm();
                    break;
                case 1:
                    phantasmBarrage();
                    break;
                case 2:
                    voidCollapseAttack();
                    break;
                case 3:
                    dimensionalTear();
                    break;
                case 4:
                    ultimateAnnihilation();
                    break;
                case 5:
                    voidShield();
                    break;
                case 6:
                    penumbraRegeneration();
                    break;
                case 7:
                    dimensionalCataclysm();
                    break;
                case 8:
                    riftMaelstrom();
                    break;
                case 9:
                    apocalypticRiftBlast();
                    break;
            }
            
            phaseWiggler.Start();
        }
        
        private void penumbraVoidStorm()
        {
            Audio.Play("event:/boss/els_penumbra_void_storm", Position);
            
            penumbraSprite.Play("attack");
            
            // Create void storm
            var level = Scene as Level;
            for (int i = 0; i < 20; i++)
            {
                Vector2 spawnPos = Position + new Vector2(
                    Calc.Random.Range(-250f, 250f),
                    Calc.Random.Range(-250f, 250f)
                );
                // Create void projectile
            }
        }
        
        private void phantasmBarrage()
        {
            Audio.Play("event:/boss/els_penumbra_barrage", Position);
            
            // Rapid fire from all phantasm lights
            foreach (var light in phantasmLights)
            {
                // Create projectile from each light position
                var level = Scene as Level;
                level?.ParticlesFG.Emit(global::Celeste.Player.P_DashB, 5, Position + light.Position, Vector2.One * 8f);
            }
        }
        
        private void voidCollapseAttack()
        {
            Audio.Play("event:/boss/els_penumbra_collapse", Position);
            
            penumbraSprite.Play("void");
            
            // Pull everything towards center then explode
            var level = Scene as Level;
            level?.Shake(2f);
            level?.Displacement.AddBurst(Position, 2f, 192f, 384f, 2f);
        }
        
        private void dimensionalTear()
        {
            Audio.Play("event:/boss/els_penumbra_tear", Position);
            
            // Create tears in reality
            var level = Scene as Level;
            level?.Flash(Color.Black, true);
        }
        
        private void ultimateAnnihilation()
        {
            if (!isInVoidMode) return;
            
            Audio.Play("event:/boss/els_penumbra_ultimate", Position);
            
            penumbraSprite.Play("ultimate");
            
            // Final desperate attack
            var level = Scene as Level;
            level?.Shake(3f);
            level?.Displacement.AddBurst(Position, 3f, 384f, 768f, 4f);
            level?.Flash(Color.White, true);
            
            // Massive screen effect
            for (int i = 0; i < 50; i++)
            {
                Vector2 randomPos = Position + new Vector2(
                    Calc.Random.Range(-400f, 400f),
                    Calc.Random.Range(-400f, 400f)
                );
                // Create explosion projectiles
            }
        }
        
        private void voidShield()
        {
            if (isDefending) return;
            
            Audio.Play("event:/boss/els_void_shield", Position);
            
            isDefending = true;
            defenseDuration = 6f; // Longer defense in phase 2
            
            penumbraSprite.Play("void");
            
            // Massive void shield
            var level = Scene as Level;
            level?.Shake(1f);
            level?.Displacement.AddBurst(Position, 1.5f, 192f, 384f, 1.5f);
            
            // Dark shield particles
            for (int i = 0; i < 360; i += 10)
            {
                float angle = MathHelper.ToRadians(i);
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 120f;
                level?.ParticlesFG.Emit(global::Celeste.Player.P_DashB, 3, Position + offset, Vector2.One * 6f, Color.Purple);
            }
            
            // All phantasm lights intensify
            foreach (var light in phantasmLights)
            {
                light.Alpha = 1.5f;
            }
            
            phaseWiggler.Start();
        }
        
        private void penumbraRegeneration()
        {
            if (healCooldown > 0f) return;
            
            Audio.Play("event:/boss/els_penumbra_heal", Position);
            
            int healAmount = (int)(MaxHealth * 0.20f); // Heal 20% in phase 2
            Health = Math.Min(Health + healAmount, MaxHealth);
            
            healCooldown = HEAL_COOLDOWN_TIME;
            
            penumbraSprite.Play("void");
            
            // Void energy absorption effect
            var level = Scene as Level;
            level?.Displacement.AddBurst(Position, 1.5f, 192f, 384f, 2f);
            
            // Purple/dark healing energy
            for (int i = 0; i < 50; i++)
            {
                Vector2 randomOffset = new Vector2(
                    Calc.Random.Range(-128f, 128f),
                    Calc.Random.Range(-128f, 128f)
                );
                level?.ParticlesFG.Emit(global::Celeste.Player.P_DashB, 4, Position + randomOffset, Vector2.One * 12f, Color.Purple);
            }
            
            // Also restore some rift power
            dimensionRiftPower = Math.Min(dimensionRiftPower + 20f, MAX_RIFT_POWER);
            
            phaseWiggler.Start();
        }
        
        private void dimensionalCataclysm()
        {
            if (dimensionRiftPower < 40f) return;
            
            Audio.Play("event:/boss/els_cataclysm", Position);
            
            dimensionRiftPower -= 40f;
            
            penumbraSprite.Play("ultimate");
            
            // Create multiple dimension rifts across the arena
            Add(new Coroutine(dimensionalCataclysmSequence()));
        }
        
        private IEnumerator dimensionalCataclysmSequence()
        {
            var level = Scene as Level;
            
            // Create 8 rifts in a pattern
            for (int rift = 0; rift < 8; rift++)
            {
                float angle = (rift / 8f) * MathHelper.TwoPi;
                Vector2 riftPos = Position + new Vector2(
                    (float)Math.Cos(angle) * 250f,
                    (float)Math.Sin(angle) * 250f
                );
                
                // Rift spawn effect
                level?.Displacement.AddBurst(riftPos, 1.5f, 128f, 256f, 1f);
                level?.Flash(Color.Purple, false);
                Audio.Play("event:/boss/els_rift_spawn", riftPos);
                
                // Create projectiles from rift
                for (int i = 0; i < 12; i++)
                {
                    float projAngle = (i / 12f) * MathHelper.TwoPi;
                    Vector2 direction = new Vector2((float)Math.Cos(projAngle), (float)Math.Sin(projAngle));
                    // Create dimensional projectile
                }
                
                yield return 0.3f;
            }
            
            // Final explosion
            level?.Shake(2f);
            level?.Displacement.AddBurst(Position, 2f, 256f, 512f, 2f);
        }
        
        private void riftMaelstrom()
        {
            if (dimensionRiftPower < 50f) return;
            
            Audio.Play("event:/boss/els_maelstrom", Position);
            
            dimensionRiftPower -= 50f;
            
            penumbraSprite.Play("attack");
            
            // Spinning vortex of rifts
            Add(new Coroutine(riftMaelstromSequence()));
        }
        
        private IEnumerator riftMaelstromSequence()
        {
            var level = Scene as Level;
            
            float duration = 4f;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                // Create spiral pattern of rifts
                float angle = elapsed * 4f;
                float radius = 100f + elapsed * 30f;
                
                Vector2 riftPos = Position + new Vector2(
                    (float)Math.Cos(angle) * radius,
                    (float)Math.Sin(angle) * radius
                );
                
                level?.Displacement.AddBurst(riftPos, 0.5f, 64f, 128f, 0.3f);
                
                // Create projectiles
                for (int i = 0; i < 6; i++)
                {
                    float projAngle = (i / 6f) * MathHelper.TwoPi + angle;
                    Vector2 direction = new Vector2((float)Math.Cos(projAngle), (float)Math.Sin(projAngle));
                    // Create maelstrom projectile
                }
                
                elapsed += Engine.DeltaTime;
                yield return null;
            }
        }
        
        private void apocalypticRiftBlast()
        {
            if (!canUseUltimateRiftAttack || dimensionRiftPower < MAX_RIFT_POWER) return;
            
            Audio.Play("event:/boss/els_apocalyptic_blast", Position);
            
            dimensionRiftPower = 0f;
            canUseUltimateRiftAttack = false;
            
            penumbraSprite.Play("ultimate");
            
            // Ultimate dimension rift attack
            Add(new Coroutine(apocalypticRiftSequence()));
        }
        
        private IEnumerator apocalypticRiftSequence()
        {
            var level = Scene as Level;
            
            // Charge up
            Audio.Play("event:/boss/els_ultimate_charge", Position);
            level?.Shake(1f);
            
            for (float t = 0; t < 2f; t += Engine.DeltaTime)
            {
                level?.Displacement.AddBurst(Position, 0.5f, 192f, 384f, 0.2f);
                
                // Pull in effect
                for (int i = 0; i < 5; i++)
                {
                    Vector2 randomPos = Position + new Vector2(
                        Calc.Random.Range(-300f, 300f),
                        Calc.Random.Range(-300f, 300f)
                    );
                    level?.ParticlesFG.Emit(global::Celeste.Player.P_DashB, 2, randomPos, Vector2.One * 8f);
                }
                
                yield return null;
            }
            
            // Release
            Audio.Play("event:/boss/els_ultimate_release", Position);
            level?.Shake(4f);
            level?.Flash(Color.White, true);
            level?.Displacement.AddBurst(Position, 4f, 512f, 1024f, 5f);
            
            // Create massive wave of dimensional energy
            for (int wave = 0; wave < 3; wave++)
            {
                for (int i = 0; i < 24; i++)
                {
                    float angle = (i / 24f) * MathHelper.TwoPi;
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    // Create apocalyptic projectile with increased speed per wave
                }
                
                yield return 0.4f;
                level?.Shake(2f);
            }
            
            // Create reality tears across arena
            for (int tear = 0; tear < 15; tear++)
            {
                Vector2 tearPos = Position + new Vector2(
                    Calc.Random.Range(-400f, 400f),
                    Calc.Random.Range(-400f, 400f)
                );
                
                level?.Displacement.AddBurst(tearPos, 1.5f, 128f, 256f, 1f);
                Audio.Play("event:/boss/els_reality_tear", tearPos);
                
                yield return 0.1f;
            }
        }
        
        public override void Render()
        {
            // Defense shield visual
            if (isDefending)
            {
                float shieldAlpha = 0.4f + energyPulse.Value * 0.3f;
                Color shieldColor = currentElsPhase == ElsPhase.DoppiaElillca ? Color.Blue : Color.Purple;
                
                Draw.Rect(Position.X - 96f, Position.Y - 96f, 192f, 192f, shieldColor * shieldAlpha);
                Draw.HollowRect(Position.X - 96f, Position.Y - 96f, 192f, 192f, shieldColor * (shieldAlpha * 1.5f));
            }
            
            // Dimension rift power indicator
            if (dimensionRiftPower > 0f)
            {
                float powerPercent = dimensionRiftPower / MAX_RIFT_POWER;
                Color riftColor = Color.Lerp(Color.Cyan, Color.Magenta, powerPercent);
                
                // Power bar
                float barWidth = 128f * powerPercent;
                Draw.Rect(Position.X - 64f, Position.Y - 100f, barWidth, 4f, riftColor * 0.8f);
                
                // Fully charged effect
                if (canUseUltimateRiftAttack)
                {
                    float pulse = (float)Math.Sin(Scene.TimeActive * 10f) * 0.5f + 0.5f;
                    Draw.Rect(Position.X - 128f, Position.Y - 128f, 256f, 256f, Color.White * (pulse * 0.2f));
                }
            }
            
            // Add phase-specific visual effects
            if (currentElsPhase == ElsPhase.DoppiaElillca)
            {
                // Duality glow effect
                Draw.Rect(Position.X - 64f, Position.Y - 64f, 128f, 128f,
                    Color.Red * (dualityFactor * 0.2f));
                Draw.Rect(Position.X - 64f, Position.Y - 64f, 128f, 128f,
                    Color.Blue * ((1f - dualityFactor) * 0.2f));
            }
            else if (currentElsPhase == ElsPhase.PenumbraPhastasm)
            {
                // Void aura
                if (isInVoidMode)
                {
                    Draw.Rect(Position.X - 192f, Position.Y - 192f, 384f, 384f,
                        Color.Black * (energyPulse.Value * 0.5f));
                }
                else
                {
                    Draw.Rect(Position.X - 128f, Position.Y - 128f, 256f, 256f,
                        Color.Purple * (energyPulse.Value * 0.3f));
                }
            }
            
            base.Render();
        }
        
        // Damage handling with defense reduction
        public void TakeDamage(int damage)
        {
            if (Health <= 0) return;
            
            if (isDefending)
            {
                int reducedDamage = (int)(damage * (1f - defenseReduction));
                Health -= reducedDamage;
                
                Audio.Play("event:/boss/els_defense_block", Position);
                
                // Visual feedback for blocking
                var level = Scene as Level;
                level?.Displacement.AddBurst(Position, 0.3f, 48f, 96f, 0.2f);
            }
            else
            {
                Health -= damage;
                
                // Build rift power when taking damage
                dimensionRiftPower = Math.Min(dimensionRiftPower + damage * 0.5f, MAX_RIFT_POWER);
            }
        }
        
        public void SetAllies(List<global::Celeste.Player> teamMembers)
        {
            allies = teamMembers;
        }
        
        public void RegisterTeamAttack()
        {
            teamAttackCounter++;
            
            // Bonus damage when all team members attack together
            if (teamAttackCounter >= allies.Count)
            {
                Audio.Play("event:/boss/team_attack_bonus", Position);
                phaseWiggler.Start();
                teamAttackCounter = 0;
            }
        }
    }
}




