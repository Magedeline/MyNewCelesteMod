using DesoloZantas.Core.BossesHelper.Entities;
using DesoloZantas.Core.Core.Cutscenes;

namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// Asriel's second phase - Angel of Death form
    /// Nearly impossible difficulty, requires teamwork to defeat
    /// </summary>
    [CustomEntity("Ingeste/AsrielAngelOfDeathBoss")]
    [Tracked]
    public class AsrielAngelOfDeathBoss : BossActor
    {
        // Audio Events
        private const string SFX_BARRIER_SHATTER = "event:/Ingeste/final_content/char/asriel/Asriel_BarrierShatter";
        private const string SFX_BIG_BULLET_FIRE = "event:/Ingeste/final_content/char/asriel/Asriel_Big_Bullet_Fire";
        private const string SFX_BIGGER_GUN_FIRE = "event:/Ingeste/final_content/char/asriel/Asriel_Bigger_Gun_Fire";
        private const string SFX_BIGGER_LIGHTNING_HIT = "event:/Ingeste/final_content/char/asriel/Asriel_Bigger_Lightninghit";
        private const string SFX_BIGGER_GUN_MECHANIZED = "event:/Ingeste/final_content/char/asriel/Asriel_BiggerGunMechanized";
        private const string SFX_CINEMATIC_CUT = "event:/Ingeste/final_content/char/asriel/Asriel_Cinematiccut";
        private const string SFX_GRAB = "event:/Ingeste/final_content/char/asriel/Asriel_Grab";
        private const string SFX_GUNSHOT = "event:/Ingeste/final_content/char/asriel/Asriel_Gunshot";
        private const string SFX_HYPERGONER_CHARGE = "event:/Ingeste/final_content/char/asriel/Asriel_Hypergoner_Charge";
        private const string SFX_LIGHTNING_HIT = "event:/Ingeste/final_content/char/asriel/Asriel_Lightninghit";
        private const string SFX_SEGA_POWER_01 = "event:/Ingeste/final_content/char/asriel/Asriel_Segapower01";
        private const string SFX_SEGA_POWER_02 = "event:/Ingeste/final_content/char/asriel/Asriel_Segapower02";
        private const string SFX_SPARKLES = "event:/Ingeste/final_content/char/asriel/Asriel_Sparkles";
        private const string SFX_SPELLCAST_GLITCH = "event:/Ingeste/final_content/char/asriel/Asriel_Spellcast_Glitch";
        private const string SFX_STAR = "event:/Ingeste/final_content/char/asriel/Asriel_Star";
        
        private bool hasTransformed = false;
        private float angelWingSpan = 0f;
        private const float MAX_WING_SPAN = 200f;
        
        // BossActor doesn't have these properties, so we add them as private fields
        private int Health;
        private int MaxHealth;
        private float arenaRadius;
        
        private Sprite angelFormSprite;
        private List<Sprite> angelWings = new List<Sprite>();
        private VertexLight divineLight;
        private SineWave celestialPulse;
        private Wiggler soulWiggler;
        
        // Phase-specific properties
        private int emotionalPhaseCounter = 0;
        private bool isEmotionalPhase = false;
        private float remainingPower = 1f;
        
        // Attack patterns
        public enum AngelAttackPattern
        {
            DivineStarfall,
            SoulAbsorption,
            HeavenlyWrath,
            EmotionalBurst,
            FinalBlow
        }
        
        public AsrielAngelOfDeathBoss(EntityData data, Vector2 offset) 
            : base(data.Position + offset,
                   spriteName: "characters/asriel_angel/angel",
                   spriteScale: Vector2.One,
                   maxFall: 160f,
                   collidable: true,
                   solidCollidable: false,
                   gravityMult: 0.0f,  // Angel floats
                   collider: new Hitbox(24, 48))
        {
            // Initialize boss properties that were in Boss base class
            MaxHealth = 2000;
            Health = MaxHealth;
            arenaRadius = 350f;
            
            setupAngelForm();
        }
        
        private void setupAngelForm()
        {
            // Main angel form sprite
            Add(angelFormSprite = new Sprite(GFX.Game, "characters/asriel/"));
            angelFormSprite.AddLoop("idle", "angel_idle", 0.1f);
            angelFormSprite.AddLoop("attack", "angel_attack", 0.08f);
            angelFormSprite.AddLoop("emotional", "angel_emotional", 0.12f);
            angelFormSprite.Play("idle");
            angelFormSprite.CenterOrigin();
            
            // Create 6 angel wings
            for (int i = 0; i < 6; i++)
            {
                var wing = new Sprite(GFX.Game, "characters/asriel/");
                wing.AddLoop("idle", "wing_idle", 0.1f);
                wing.AddLoop("flap", "wing_flap", 0.08f);
                wing.Play("idle");
                wing.CenterOrigin();
                Add(wing);
                angelWings.Add(wing);
            }
            
            // Divine light
            Add(divineLight = new VertexLight(Color.Gold * 1.2f, 1f, 256, 384));
            divineLight.Position = Vector2.Zero;
            
            // Celestial pulse
            Add(celestialPulse = new SineWave(0.6f, 0f));
            celestialPulse.Randomize();
            
            // Soul wiggler
            Add(soulWiggler = Wiggler.Create(1.5f, 3f));
        }
        
        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            var level = Scene as Level;
            if (level != null)
            {
                // Check for intro cutscene based on room ID
                string currentRoomId = level.Session.Level;
                string introFlagForRoom = $"asriel_angel_boss_intro_{currentRoomId}";
                bool hasSeenIntroForThisRoom = level.Session.GetFlag(introFlagForRoom) || 
                                                level.Session.GetFlag("asriel_angel_boss_intro");
                
                // Trigger intro cutscene if Kirby hasn't seen it for this room
                if (!hasSeenIntroForThisRoom && ShouldShowIntroForRoom(currentRoomId))
                {
                    level.Add((Entity)new CS20_AsrielAngelOfDeathBossIntro(currentRoomId));
                }
            }
            
            if (!hasTransformed)
            {
                Add(new Coroutine(transformToAngelForm()));
            }
        }
        
        /// <summary>
        /// Determines if the intro cutscene should be shown for a given room ID.
        /// Override or modify this list to add more rooms that trigger the intro.
        /// </summary>
        private bool ShouldShowIntroForRoom(string roomId)
        {
            // List of room IDs where the Asriel Angel of Death Boss intro should play
            // Add any room ID that contains this boss where you want an intro
            string[] introRoomIds = new string[]
            {
                "azzyboss-angel-00",    // Angel form intro room
                "azzyboss-50",          // Alternative angel battle room
                // Add more room IDs here as needed, e.g.:
                // "your-custom-room-angel",
                // "kirby-asriel-angel-fight",
            };
            
            foreach (string id in introRoomIds)
            {
                if (roomId.Equals(id, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            
            return false;
        }
        
        private IEnumerator transformToAngelForm()
        {
            hasTransformed = true;
            var level = Scene as Level;
            
            // Dramatic entrance with Despair music (Phase 2)
            if (level != null)
            {
                level.Session.Audio.Music.Event = "event:/Ingeste/final_content/music/lvl20/els_09";
                level.Session.Audio.Apply();
            }
            
            Audio.Play(SFX_SPARKLES, Position);
            
            // Screen shake
            level?.Shake(2f);
            
            // Grow wings
            for (float t = 0f; t < 1f; t += Engine.DeltaTime * 0.3f)
            {
                angelWingSpan = Ease.BackOut(t) * MAX_WING_SPAN;
                updateWingPositions();
                yield return null;
            }
            
            angelWingSpan = MAX_WING_SPAN;
            
            // Displacement burst
            level?.Displacement.AddBurst(Position, 1.5f, 128f, 256f, 3f);
            
            // Start flapping wings
            foreach (var wing in angelWings)
            {
                wing.Play("flap");
            }
            
            soulWiggler.Start();
            
            yield return 1f;
            
            // Announce presence
            Audio.Play(SFX_SEGA_POWER_01, Position);
        }
        
        private void updateWingPositions()
        {
            // Arrange wings in a circle around Asriel
            for (int i = 0; i < angelWings.Count; i++)
            {
                float angle = (i / (float)angelWings.Count) * MathHelper.TwoPi;
                float radius = angelWingSpan * (0.5f + celestialPulse.Value * 0.1f);
                
                Vector2 offset = new Vector2(
                    (float)Math.Cos(angle) * radius,
                    (float)Math.Sin(angle) * radius
                );
                
                angelWings[i].Position = offset;
                angelWings[i].Rotation = angle + MathHelper.PiOver2;
            }
        }
        
        public override void Update()
        {
            base.Update();
            
            if (!hasTransformed) return;
            
            // Update wing positions
            updateWingPositions();
            
            // Update divine light intensity based on health
            float healthPercent = (float)Health / MaxHealth;
            divineLight.Alpha = 0.8f + soulWiggler.Value * 0.4f;
            divineLight.StartRadius = 256f * (1f + celestialPulse.Value * 0.2f);
            
            // Check for emotional phase trigger
            if (healthPercent <= 0.1f && !isEmotionalPhase)
            {
                enterEmotionalPhase();
            }
            
            // Update remaining power (drains as fight progresses)
            remainingPower = healthPercent;
        }
        
        private void enterEmotionalPhase()
        {
            isEmotionalPhase = true;
            angelFormSprite.Play("emotional");
            
            Audio.Play(SFX_SEGA_POWER_02, Position);
            
            // Change light color to indicate emotional state
            divineLight.Color = new Color(1f, 0.8f, 1f) * 1.5f;
            
            var level = Scene as Level;
            level?.Shake(1f);
            
            // Trigger cutscene for saving souls
            Add(new Coroutine(emotionalPhaseSequence()));
        }
        
        private IEnumerator emotionalPhaseSequence()
        {
            // Players struggle to keep up
            yield return 2f;
            
            // Use little power left to save others
            Audio.Play(SFX_HYPERGONER_CHARGE, Position);
            
            // Everyone's feelings return
            for (int i = 0; i < 3; i++)
            {
                var level = Scene as Level;
                level?.ParticlesFG.Emit(global::Celeste.Player.P_DashA, 20, Position, Vector2.One * 32f);
                
                yield return 1f;
            }
            
            // Asriel regains consciousness
            yield return 1f;
            
            // Els is cut from Asriel's host body (trigger cutscene)
            Audio.Play(SFX_CINEMATIC_CUT, Position);
            
            var currentLevel = Scene as Level;
            currentLevel?.Shake(2f);
            currentLevel?.Displacement.AddBurst(Position, 2f, 256f, 512f, 2f);
            
            yield return 2f;
            
            // Set flag for next phase
            var levelSession = (Scene as Level)?.Session;
            if (levelSession != null)
            {
                levelSession.SetFlag("asriel_angel_defeated");
                levelSession.SetFlag("els_revealed");
            }
        }
        
        public void ExecuteAngelAttack(AngelAttackPattern pattern)
        {
            switch (pattern)
            {
                case AngelAttackPattern.DivineStarfall:
                    executeDivineStarfall();
                    break;
                case AngelAttackPattern.SoulAbsorption:
                    executeSoulAbsorption();
                    break;
                case AngelAttackPattern.HeavenlyWrath:
                    executeHeavenlyWrath();
                    break;
                case AngelAttackPattern.EmotionalBurst:
                    executeEmotionalBurst();
                    break;
                case AngelAttackPattern.FinalBlow:
                    executeFinalBlow();
                    break;
            }
            
            soulWiggler.Start();
        }
        
        private void executeDivineStarfall()
        {
            Audio.Play(SFX_STAR, Position);
            
            // Rain down stars from above
            var level = Scene as Level;
            for (int i = 0; i < 12; i++)
            {
                Vector2 spawnPos = Position + new Vector2(
                    Calc.Random.Range(-200f, 200f),
                    -300f
                );
                // Create star projectile falling down
            }
        }
        
        private void executeSoulAbsorption()
        {
            Audio.Play(SFX_GRAB, Position);
            
            // Pull player towards Asriel
            divineLight.StartRadius = 512f;
            divineLight.Color = Color.Purple * 1.5f;
        }
        
        private void executeHeavenlyWrath()
        {
            Audio.Play(SFX_BIGGER_GUN_FIRE, Position);
            
            // Massive AoE attack
            var level = Scene as Level;
            level?.Shake(1.5f);
            level?.Displacement.AddBurst(Position, 1.2f, 192f, 384f, 1.5f);
            
            // Create shockwave
            for (int i = 0; i < 16; i++)
            {
                float angle = (i / 16f) * MathHelper.TwoPi;
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                // Create shockwave projectile
            }
        }
        
        private void executeEmotionalBurst()
        {
            Audio.Play(SFX_SPELLCAST_GLITCH, Position);
            
            // Emotional energy burst
            var level = Scene as Level;
            level?.ParticlesFG.Emit(global::Celeste.Player.P_DashA, 30, Position, Vector2.One * 64f);
            level?.Displacement.AddBurst(Position, 0.8f, 96f, 192f, 1f);
        }
        
        private void executeFinalBlow()
        {
            if (!isEmotionalPhase) return;
            
            Audio.Play(SFX_BARRIER_SHATTER, Position);
            
            // This is when Els is cut out
            var level = Scene as Level;
            level?.Shake(3f);
            level?.Displacement.AddBurst(Position, 2.5f, 384f, 768f, 3f);
            
            // Massive screen flash
            level?.Flash(Color.White, true);
        }
        
        public override void Render()
        {
            // Add glow effect
            if (isEmotionalPhase)
            {
                Draw.Rect(Position.X - 128f, Position.Y - 128f, 256f, 256f, 
                    Color.White * (celestialPulse.Value * 0.3f));
            }
            
            base.Render();
        }
    }
}




