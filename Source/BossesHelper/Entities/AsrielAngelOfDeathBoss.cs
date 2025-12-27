using Celeste;
using Celeste.Mod;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DesoloZantas.Core.Core.VanillaCore;

namespace Celeste.Mod.DesoloZatnas.BossesHelper.Entities
{
    /// <summary>
    /// Asriel Angel of Death Boss - Multi-Phase Boss Fight with Dialog Integration
    /// Features cosmowing rainbow effects, lost soul rescue mechanics, and memory restoration
    /// </summary>
    [CustomEntity("DesoloZatnas/AsrielAngelOfDeathBoss")]
    public partial class AsrielAngelOfDeathBoss : BaseBoss
    {
        // Health constants
        private const int PHASE1_MAX_HEALTH = 100;
        private const int PHASE2_MAX_HEALTH = 150;
        private const int PHASE3_ZERO_HEALTH = 200;
        
        // Lost soul tracking
        private HashSet<string> rescuedSouls = new HashSet<string>();
        private int totalSoulsToRescue = 15; // Magolor, Chara, Theo, Oshiro, Toriel, Asgore, etc.
        
        // Memory restoration tracking
        private int memoryRestorationStage = 0;
        private bool isInMemorySequence = false;
        private bool isInFinalBeamSequence = false;
        
        private Sprite bodySprite;
        private Sprite bgCosmoWing;
        private Sprite stemsSprite;
        private Sprite shoulderSprite;
        private Sprite orbWingsSprite;
        
        // Phase 2 tween nodes
        private Sprite headSprite;
        private Sprite torsoSprite;
        private Sprite armsLeftSprite;
        private Sprite armsRightSprite;
        private Sprite wingsLeftSprite;
        private Sprite wingsRightSprite;
        private Sprite haloSprite;
        
        private int currentPhase = 1;
        private bool isTransitioning = false;
        private Random random;
        
        // Health properties
        private int MaxHealth { get; set; }
        private int Health { get; set; }
        
        // Dialog and cutscene state
        private Level level;
        private Player player;
        private bool hasTriggeredTransformation = false;
        private bool hasTriggeredZeroForm = false;
        private bool hasStartedSoulRescue = false;
        
        public AsrielAngelOfDeathBoss(EntityData data, Vector2 offset)
            : base(
                data.Position + offset,
                data.NodesWithPosition(offset),
                data.Int("patternIndex", 0),
                data.Float("cameraYPastMax", 200f),
                data.Bool("dialog", false),
                data.Bool("startHit", false),
                data.Bool("cameraLockY", true))
        {
            random = new Random();
            BossId = "AsrielAngelOfDeath";
            
            // Set health
            MaxHealth = 9999999; // Temporary high value, will be set per phase
            Health = MaxHealth;
            HitsPerPhase = 100; // Damage based instead of hit count
            
            // Adjust collider for this boss
            Collider = new Hitbox(32f, 48f, -16f, -24f);
        }
        
        protected override void CreateBossSprite()
        {
            // Initialize sprites
            InitializeSprites();
        }
        
        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
            player = level?.Tracker.GetEntity<Player>();
        }
        
        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            // Start the boss fight AI
            Add(new Coroutine(BossFightSequence()));
        }
        
        protected override IEnumerator Attack()
        {
            // Not used - we use BossFightSequence instead
            yield return null;
        }
        
        private void InitializeSprites()
        {
            // Background cosmowing effect
            Add(bgCosmoWing = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/bg/"));
            bgCosmoWing.Add("cosmowing", "cosmowing", 0.02f);
            bgCosmoWing.Add("cosmowing_intense", "cosmowing_intense", 0.015f);
            bgCosmoWing.Play("cosmowing");
            bgCosmoWing.CenterOrigin();
            bgCosmoWing.Position = new Vector2(0, 0);
            
            // Stems with cosmowing
            Add(stemsSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/stems/"));
            stemsSprite.Add("idle", "idle", 0.05f);
            stemsSprite.Add("cosmowing", "cosmowing", 0.02f);
            stemsSprite.Play("idle");
            stemsSprite.CenterOrigin();
            
            // Shoulder with cosmowing
            Add(shoulderSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/shoulder/"));
            shoulderSprite.Add("idle", "idle", 0.05f);
            shoulderSprite.Add("cosmowing", "cosmowing", 0.02f);
            shoulderSprite.Play("idle");
            shoulderSprite.CenterOrigin();
            
            // Orb wings with cosmowing
            Add(orbWingsSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/orbwings/"));
            orbWingsSprite.Add("idle", "idle", 0.04f);
            orbWingsSprite.Add("cosmowing", "cosmowing", 0.02f);
            orbWingsSprite.Add("spread", "spread", 0.03f);
            orbWingsSprite.Play("idle");
            orbWingsSprite.CenterOrigin();
            
            // Main body sprite
            Add(bodySprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/"));
            bodySprite.Add("idle", "idle", 0.05f);
            bodySprite.Add("hover", "hover", 0.04f);
            bodySprite.Add("charging", "charge", 0.03f);
            bodySprite.Add("attacking", "attack", 0.04f);
            bodySprite.Play("idle");
            bodySprite.CenterOrigin();
        }
        
        private void InitializePhase2Sprites()
        {
            // Phase 2 tween nodes for multi-part boss
            Add(headSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/phase2/parts/"));
            headSprite.AddLoop("head", "head", 0.04f);
            headSprite.Play("head");
            headSprite.CenterOrigin();
            headSprite.Position = new Vector2(0, -30);
            
            Add(torsoSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/phase2/parts/"));
            torsoSprite.AddLoop("torso", "torso", 0.04f);
            torsoSprite.Play("torso");
            torsoSprite.CenterOrigin();
            
            Add(armsLeftSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/phase2/parts/"));
            armsLeftSprite.AddLoop("arms_left", "arms_left", 0.04f);
            armsLeftSprite.Play("arms_left");
            armsLeftSprite.CenterOrigin();
            armsLeftSprite.Position = new Vector2(-25, 0);
            
            Add(armsRightSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/phase2/parts/"));
            armsRightSprite.AddLoop("arms_right", "arms_right", 0.04f);
            armsRightSprite.Play("arms_right");
            armsRightSprite.CenterOrigin();
            armsRightSprite.Position = new Vector2(25, 0);
            
            Add(wingsLeftSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/phase2/parts/"));
            wingsLeftSprite.AddLoop("wings_left", "wings_left", 0.03f);
            wingsLeftSprite.Play("wings_left");
            wingsLeftSprite.CenterOrigin();
            wingsLeftSprite.Position = new Vector2(-40, 5);
            
            Add(wingsRightSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/phase2/parts/"));
            wingsRightSprite.AddLoop("wings_right", "wings_right", 0.03f);
            wingsRightSprite.Play("wings_right");
            wingsRightSprite.CenterOrigin();
            wingsRightSprite.Position = new Vector2(40, 5);
            
            Add(haloSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/phase2/parts/"));
            haloSprite.AddLoop("halo", "halo", 0.02f);
            haloSprite.Play("halo");
            haloSprite.CenterOrigin();
            haloSprite.Position = new Vector2(0, -45);
        }
        
        public override void Update()
        {
            base.Update();
            
            // Check for phase transitions
            if (currentPhase == 1 && Health <= 0 && !isTransitioning && !hasTriggeredTransformation)
            {
                hasTriggeredTransformation = true;
                Add(new Coroutine(TransitionToPhase2()));
            }
            else if (currentPhase == 2 && Health <= 0 && !isTransitioning && !hasTriggeredZeroForm)
            {
                hasTriggeredZeroForm = true;
                Add(new Coroutine(TransitionToAsrielZero()));
            }
        }
        
        // Main boss fight sequence orchestrator
        private IEnumerator BossFightSequence()
        {
            // Phase 1: Angel of Death
            yield return Phase1Routine();
            
            // Wait for transformation trigger
            while (!hasTriggeredTransformation)
                yield return null;
            
            // Phase 2: Transcendent Angel  
            yield return Phase2Routine();
            
            // Wait for zero form trigger
            while (!hasTriggeredZeroForm)
                yield return null;
            
            // Phase 3: Asriel Zero (with soul rescue)
            yield return Phase3ZeroRoutine();
        }
        
        private IEnumerator TransitionToPhase2()
        {
            isTransitioning = true;
            
            // Dialog: Asriel becomes unstable
            yield return PlayDialog("CH20_ASRIEL_ANGELOFDEATH_TRANSFORMATION");
            
            // Trigger 12: Transformation begins
            yield return CameraZoom(3.0f, 2.0f, "ease_in");
            yield return ShakeScreen(1.5f, 3.0f);
            
            // Play transformation animation
            bodySprite.Play("phase2_transform_start");
            bgCosmoWing.Play("cosmowing_intense");
            
            // Flash effects during transformation
            yield return FlashScreen("990000", 0.2f);
            
            while (bodySprite.CurrentAnimationID == "phase2_transform_start" && bodySprite.Animating)
                yield return null;
            
            bodySprite.Play("phase2_transform_mid");
            while (bodySprite.CurrentAnimationID == "phase2_transform_mid" && bodySprite.Animating)
                yield return null;
            
            // Initialize phase 2 sprites
            InitializePhase2Sprites();
            
            bodySprite.Play("phase2_transform_end");
            while (bodySprite.CurrentAnimationID == "phase2_transform_end" && bodySprite.Animating)
                yield return null;
            
            // Trigger 14: Ultimate entity emerges
            yield return CameraZoomBack(3.0f);
            yield return SpawnParticles("dust", Position, 30);
            yield return ShakeScreen(2.0f, 4.0f);
            
            // Set phase 2 properties
            currentPhase = 2;
            MaxHealth = PHASE2_MAX_HEALTH;
            Health = MaxHealth;
            
            bodySprite.Play("phase2_idle");
            isTransitioning = false;
        }
        
        // Transition to Asriel Zero (final form)
        private IEnumerator TransitionToAsrielZero()
        {
            isTransitioning = true;
            
            // Play zero transformation sequence
            yield return IntenseTransformationEffects();
            
            // Set phase 3 properties
            currentPhase = 3;
            MaxHealth = PHASE3_ZERO_HEALTH;
            Health = MaxHealth;
            
            // Change sprite state to "zero" form
            bodySprite.Play("zero_idle");
            bgCosmoWing.Play("cosmowing_phase2");
            
            isTransitioning = false;
        }
        
        private IEnumerator IntenseTransformationEffects()
        {
            // Screen effects for ultimate transformation
            for (int i = 0; i < 5; i++)
            {
                yield return FlashScreen(i % 2 == 0 ? "FFFFFF" : "000000", 0.1f);
                yield return ShakeScreen(0.3f, 2.0f);
            }
            
            yield return CameraZoom(5.0f, 1.5f, "cube");
            yield return 0.5f;
            yield return CameraZoomBack(2.0f);
        }
        
        // Phase 1 AI Routine
        private IEnumerator Phase1Routine()
        {
            while (currentPhase == 1 && Health > 0)
            {
                // Random attack selection
                int attackChoice = random.Next(0, 3);
                
                switch (attackChoice)
                {
                    case 0:
                        yield return AttackUltimaBullet();
                        break;
                    case 1:
                        yield return AttackCrossShocker();
                        break;
                    case 2:
                        yield return AttackStarStormUltra();
                        break;
                }
                
                yield return 0.5f;
            }
        }
        
        // Phase 2 AI Routine
        private IEnumerator Phase2Routine()
        {
            while (currentPhase == 2 && Health > 0)
            {
                int attackChoice = random.Next(0, 5);
                
                switch (attackChoice)
                {
                    case 0:
                        yield return AttackUltimaBullet();
                        break;
                    case 1:
                        yield return AttackCrossShocker();
                        break;
                    case 2:
                        yield return AttackStarStormUltra();
                        break;
                    case 3:
                        yield return AttackShockerBreaker3();
                        break;
                    case 4:
                        yield return AttackFinalBeam();
                        break;
                }
                
                yield return 0.3f;
            }
        }
        
        // Phase 3: Asriel Zero with Soul Rescue Mechanics
        private IEnumerator Phase3ZeroRoutine()
        {
            // Combat phase with soul rescue opportunities
            while (currentPhase == 3 && Health > 0)
            {
                // Check if player should trigger soul rescue
                if (!hasStartedSoulRescue && Health <= PHASE3_ZERO_HEALTH * 0.7f)
                {
                    hasStartedSoulRescue = true;
                    yield return SoulRescueSequence();
                }
                
                // Skip attacks during memory sequences
                if (isInMemorySequence || isInFinalBeamSequence)
                {
                    yield return null;
                    continue;
                }
                
                // Continue attacking
                int attackChoice = random.Next(0, 6);
                
                switch (attackChoice)
                {
                    case 0:
                    case 1:
                        yield return AttackUltimaBullet();
                        break;
                    case 2:
                        yield return AttackCrossShocker();
                        break;
                    case 3:
                        yield return AttackStarStormUltra();
                        break;
                    case 4:
                        yield return AttackShockerBreaker3();
                        break;
                    case 5:
                        yield return AttackFinalBeam();
                        break;
                }
                
                yield return 0.2f;
            }
            
            // Final defeat sequence
            yield return FinalDefeatSequence();
        }
        
        // Attack: Ultima Bullet - Powerful homing projectiles
        private IEnumerator AttackUltimaBullet()
        {
            bodySprite.Play("attack_ultimabullet_start");
            stemsSprite.Play("cosmowing");
            
            yield return 0.5f;
            
            bodySprite.Play("attack_ultimabullet_hold");
            
            // Fire bullets
            Player player = Scene.Tracker.GetEntity<Player>();
            if (player != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    float angle = (float)(i * Math.PI / 4);
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    
                    Scene.Add(new UltimaBulletProjectile(Position, direction, player));
                    
                    yield return 0.1f;
                }
            }
            
            bodySprite.Play("attack_ultimabullet_fire");
            yield return 0.3f;
            
            stemsSprite.Play("idle");
        }
        
        // Attack: Cross Shocker - Cross-shaped lightning waves
        private IEnumerator AttackCrossShocker()
        {
            bodySprite.Play("attack_crossshocker_start");
            shoulderSprite.Play("cosmowing");
            orbWingsSprite.Play("spread");
            
            yield return 0.4f;
            
            bodySprite.Play("attack_crossshocker_loop");
            
            // Fire cross pattern
            for (int wave = 0; wave < 3; wave++)
            {
                // Vertical
                Scene.Add(new CrossShockerWave(Position, Vector2.UnitY, 200f));
                Scene.Add(new CrossShockerWave(Position, -Vector2.UnitY, 200f));
                
                // Horizontal
                Scene.Add(new CrossShockerWave(Position, Vector2.UnitX, 200f));
                Scene.Add(new CrossShockerWave(Position, -Vector2.UnitX, 200f));
                
                yield return 0.4f;
            }
            
            bodySprite.Play("attack_crossshocker_end");
            yield return 0.3f;
            
            shoulderSprite.Play("idle");
            orbWingsSprite.Play("idle");
        }
        
        // Attack: Star Storm Ultra - Rain of stars from above
        private IEnumerator AttackStarStormUltra()
        {
            bodySprite.Play("attack_starstormultra_start");
            bgCosmoWing.Play("cosmowing_intense");
            
            yield return 0.6f;
            
            bodySprite.Play("attack_starstormultra_rain");
            
            // Rain stars
            for (int i = 0; i < 30; i++)
            {
                float x = random.Next((int)level.Camera.Left, (int)level.Camera.Right);
                Vector2 starPos = new Vector2(x, level.Camera.Top - 20);
                
                Scene.Add(new StarStormProjectile(starPos));
                
                yield return 0.15f;
            }
            
            bodySprite.Play("attack_starstormultra_end");
            yield return 0.4f;
            
            bgCosmoWing.Play("cosmowing");
        }
        
        // Attack: Shocker Breaker III - Phase 2 exclusive devastating shockwave
        private IEnumerator AttackShockerBreaker3()
        {
            bodySprite.Play("attack_shockerbreaker3_start");
            
            if (headSprite != null) headSprite.Scale = Vector2.One * 1.2f;
            if (haloSprite != null) haloSprite.Scale = Vector2.One * 1.5f;
            
            yield return 0.7f;
            
            bodySprite.Play("attack_shockerbreaker3_hold");
            
            yield return 0.3f;
            
            bodySprite.Play("attack_shockerbreaker3_release");
            
            // Create expanding shockwaves
            for (int ring = 0; ring < 5; ring++)
            {
                Scene.Add(new ShockerBreaker3Wave(Position, ring * 60f + 80f, ring * 0.2f));
                yield return 0.3f;
            }
            
            bodySprite.Play("attack_shockerbreaker3_waves");
            yield return 1f;
            
            bodySprite.Play("attack_shockerbreaker3_end");
            
            if (headSprite != null) headSprite.Scale = Vector2.One;
            if (haloSprite != null) haloSprite.Scale = Vector2.One;
            
            yield return 0.4f;
        }
        
        // Attack: Final Beam - Ultimate sustained laser attack
        private IEnumerator AttackFinalBeam()
        {
            bodySprite.Play("attack_finalbeam_charge");
            
            // Activate all cosmowing effects
            bgCosmoWing.Play("cosmowing_intense");
            stemsSprite.Play("cosmowing");
            shoulderSprite.Play("cosmowing");
            orbWingsSprite.Play("cosmowing");
            
            // Charge up effects
            for (float i = 0; i < 1.5f; i += 0.05f)
            {
                if (wingsLeftSprite != null)
                {
                    wingsLeftSprite.Position = new Vector2(-40 - i * 10, 5);
                    wingsRightSprite.Position = new Vector2(40 + i * 10, 5);
                }
                yield return 0.05f;
            }
            
            bodySprite.Play("attack_finalbeam_focus");
            yield return 0.6f;
            
            bodySprite.Play("attack_finalbeam_fire");
            
            // Create the massive beam
            Player player = Scene.Tracker.GetEntity<Player>();
            if (player != null)
            {
                Vector2 direction = (player.Position - Position).SafeNormalize();
                Scene.Add(new FinalBeamAttack(Position, direction, 3f));
            }
            
            bodySprite.Play("attack_finalbeam_hold");
            yield return 3f;
            
            bodySprite.Play("attack_finalbeam_end");
            
            // Reset effects
            stemsSprite.Play("idle");
            shoulderSprite.Play("idle");
            orbWingsSprite.Play("idle");
            bgCosmoWing.Play("cosmowing");
            
            if (wingsLeftSprite != null)
            {
                wingsLeftSprite.Position = new Vector2(-40, 5);
                wingsRightSprite.Position = new Vector2(40, 5);
            }
            
            yield return 0.5f;
        }
    }
}
