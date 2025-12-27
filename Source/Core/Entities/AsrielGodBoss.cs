using System.Globalization;
using DesoloZantas.Core.BossesHelper.Entities;
using DesoloZantas.Core.Core.Cutscenes;
using DesoloZantas.Core.Core.Entities.Effects;
using DesoloZantas.Core.Core.Entities.Projectiles;
using DesoloZantas.Core.Core.Extensions;
using DesoloZantas.Core.Core.Player;

namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity("Ingeste/AsrielGodBoss")]
    [Tracked(true)]
    public partial class AsrielGodBoss : BossActor
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
        
        public static ParticleType PBurst;
        
        static AsrielGodBoss()
        {
            // Initialize particle type for burst effect
            PBurst = new ParticleType
            {
                Color = Color.Cyan,
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Choose,
                FadeMode = ParticleType.FadeModes.Late,
                LifeMin = 0.4f,
                LifeMax = 0.8f,
                Size = 1f,
                SizeRange = 0.5f,
                DirectionRange = (float)Math.PI / 4f,
                SpeedMin = 40f,
                SpeedMax = 80f,
                SpeedMultiplier = 0.2f,
                Acceleration = new Vector2(0f, 60f)
            };
        }
        
        public const float CAMERA_X_PAST_MAX = 140f;
        private const float move_speed = 600f;
        private const float avoid_radius = 12f;
        // Sprite inherited from BossEntity (readonly Sprite Sprite)
        public global::Celeste.PlayerSprite NormalSprite;
        private PlayerHair normalHair;
        private Vector2 avoidPos;
        public float CameraYPastMax;
        public bool Moving;
        public bool Sitting;
        private int facing;
        private Level level;
        private Monocle.Circle circle;
        private Vector2[] nodes;
        private int nodeIndex;
        private int patternIndex;
        private Coroutine attackCoroutine;
        private Coroutine triggerBlocksCoroutine;
        private List<Entity> fallingBlocks;
        private List<Entity> movingBlocks;
        private bool playerHasMoved;
        private SineWave floatSine;
        private bool dialog;
        private bool startHit;
        private VertexLight light;
        private Wiggler scaleWiggler;
        private AsrielGodBossStarfield bossBg;
        private SoundSource chargeSfx;
        private SoundSource laserSfx;
        private bool isAttacking;

        // BEGIN custom attack sequence fields
        private bool useCustomSequence;
        private List<AttackStep> customAttackSteps;
        private string attackSequenceData; // Add this property for EntityData

        private struct AttackStep
        {
            public string Action;
            public float Delay;
            public float Arg; // for angleOffset (shoot)
            public AttackStep(string action, float delay, float arg = 0f)
            {
                Action = action;
                Delay = delay;
                Arg = arg;
            }
        }

        // Attack pattern implementations
        private class AttackPattern
        {
            public float ChargeTime { get; set; }
            public float Duration { get; set; }
            public float CooldownTime { get; set; }
            public Action<AsrielGodBoss> Execute { get; set; }
        }

        private Dictionary<AttackType, AttackPattern> attackPatterns;

        private void InitializeAttackPatterns()
        {
            attackPatterns = new Dictionary<AttackType, AttackPattern>
            {
                {
                    AttackType.ChaosBlaster,
                    new AttackPattern
                    {
                        ChargeTime = 0.5f,
                        Duration = 3.0f,
                        CooldownTime = 1.0f,
                        Execute = (boss) => ExecuteChaosBlaster()
                    }
                },
                {
                    AttackType.HyperGoner,
                    new AttackPattern
                    {
                        ChargeTime = 1.0f,
                        Duration = 4.0f,
                        CooldownTime = 2.0f,
                        Execute = (boss) => ExecuteHyperGoner()
                    }
                },
                {
                    AttackType.StarstormRain,
                    new AttackPattern
                    {
                        ChargeTime = 0.8f,
                        Duration = 5.0f,
                        CooldownTime = 1.5f,
                        Execute = (boss) => ExecuteStarstormRain()
                    }
                },
                {
                    AttackType.GalacticSaber,
                    new AttackPattern
                    {
                        ChargeTime = 0.3f,
                        Duration = 2.0f,
                        CooldownTime = 1.0f,
                        Execute = (boss) => ExecuteGalacticSaber()
                    }
                },
                {
                    AttackType.DimensionalRift,
                    new AttackPattern
                    {
                        ChargeTime = 1.2f,
                        Duration = 6.0f,
                        CooldownTime = 2.0f,
                        Execute = (boss) => ExecuteDimensionalRift()
                    }
                }
            };
        }

        // Attack execution methods
        private void ExecuteChaosBlaster()
        {
            // Creates a rapid-fire pattern of star projectiles
            const int PROJECTILE_COUNT = 8;
            const float SPREAD_ANGLE = 30f;
            
            for (int i = 0; i < PROJECTILE_COUNT; i++)
            {
                float angle = (i - (PROJECTILE_COUNT - 1) / 2f) * (SPREAD_ANGLE / (PROJECTILE_COUNT - 1));
                Vector2 direction = Calc.AngleToVector(angle * Calc.DegToRad, 1f);
                CreateStarProjectile(Position, direction * 300f, Color.Yellow);
            }
        }

        private void ExecuteHyperGoner()
        {
            // Creates a vacuum effect that pulls the player
            const float PULL_STRENGTH = 150f;
            const float SAFE_DISTANCE = 40f;
            
            level.Add(new HyperGonerVortex(Position, PULL_STRENGTH, SAFE_DISTANCE));
            Audio.Play(SFX_HYPERGONER_CHARGE, Position);
        }

        private void ExecuteStarstormRain()
        {
            // Creates a pattern of falling star projectiles
            const int STARS_PER_WAVE = 5;
            // Note: WAVE_INTERVAL removed - waves are handled externally
            
            for (int i = 0; i < STARS_PER_WAVE; i++)
            {
                float xOffset = (i - (STARS_PER_WAVE - 1) / 2f) * 32f;
                Vector2 spawnPos = new Vector2(Position.X + xOffset, Position.Y - 160f);
                CreateFallingStarProjectile(spawnPos);
            }
        }

        private void ExecuteGalacticSaber()
        {
            // Creates ethereal blades that slash across the screen
            const float BLADE_SPEED = 400f;
            const int BLADE_COUNT = 3;
            
            for (int i = 0; i < BLADE_COUNT; i++)
            {
                float angle = Calc.Random.Range(0f, 360f);
                Vector2 direction = Calc.AngleToVector(angle * Calc.DegToRad, 1f);
                CreateEtherealBlade(Position, direction * BLADE_SPEED);
            }
        }

        private void ExecuteDimensionalRift()
        {
            // Creates spatial distortions that the player must avoid
            const float RIFT_DURATION = 2f;
            const float RIFT_RADIUS = 32f;
            
            Vector2 targetPos = level.Tracker.GetEntity<global::Celeste.Player>()?.Position ?? Position;
            level.Add(new DimensionalRift(targetPos, RIFT_RADIUS, RIFT_DURATION));
        }

        // Helper methods for creating projectiles and effects
        private void CreateStarProjectile(Vector2 position, Vector2 velocity, Color color)
        {
            level.Add(new StarProjectile(position, velocity, color));
        }

        private void CreateFallingStarProjectile(Vector2 position)
        {
            level.Add(new FallingStarProjectile(position));
        }

        private void CreateEtherealBlade(Vector2 position, Vector2 velocity)
        {
            level.Add(new EtherealBlade(position, velocity));
        }
        
        // Sword tween attack methods
        private void InitializeSwordSprite()
        {
            if (swordSprite == null)
            {
                swordSprite = GFX.SpriteBank.Create("asrielgodboss_sword");
                swordSprite.Visible = false;
                swordSprite.CenterOrigin();
                Add(swordSprite);
            }
        }
        
        private void ExecuteSwordSlash()
        {
            InitializeSwordSprite();
            var player = level.Tracker.GetEntity<global::Celeste.Player>();
            if (player == null) return;
            
            swordActive = true;
            swordSprite.Visible = true;
            swordStartPosition = Position + new Vector2(0, -20f);
            swordTargetPosition = player.Position;
            swordTweenTimer = 0f;
            swordRotation = 0f;
            swordTargetRotation = (float)Math.Atan2(
                swordTargetPosition.Y - swordStartPosition.Y,
                swordTargetPosition.X - swordStartPosition.X
            );
            swordSprite.Play("slash");
            Audio.Play(SFX_STAR, Position);
        }
        
        private void UpdateSwordTween()
        {
            if (!swordActive || swordSprite == null) return;
            
            swordTweenTimer += Engine.DeltaTime;
            float t = Math.Min(swordTweenTimer / swordTweenDuration, 1f);
            
            // Ease out cubic for smooth deceleration
            float easedT = 1f - (float)Math.Pow(1f - t, 3f);
            
            // Lerp position
            swordSprite.Position = Vector2.Lerp(swordStartPosition - Position, swordTargetPosition - Position, easedT);
            
            // Lerp rotation
            swordSprite.Rotation = Calc.LerpSnap(swordRotation, swordTargetRotation, easedT, 0.01f);
            
            // Check for player collision during tween
            if (t < 1f)
            {
                var player = level.Tracker.GetEntity<global::Celeste.Player>();
                if (player != null && Vector2.Distance(Position + swordSprite.Position, player.Position) < 20f)
                {
                    OnPlayer(player);
                }
            }
            
            // Complete tween
            if (t >= 1f)
            {
                swordActive = false;
                swordSprite.Visible = false;
            }
        }
        
        private IEnumerator SwordSlashSequence(int slashCount = 3, float delayBetweenSlashes = 0.5f)
        {
            for (int i = 0; i < slashCount; i++)
            {
                ExecuteSwordSlash();
                yield return swordTweenDuration + 0.1f;
                yield return delayBetweenSlashes;
            }
        }
        // END custom attack sequence fields

        // Boss state management fields
        private BossState currentState = BossState.Idle;
        private AttackType currentAttackType = AttackType.Shoot;
        private AttackPhase currentAttackPhase = AttackPhase.Charging;
        private int currentHealth = 10; // Higher health for Asriel
        private float playerHitCooldown = 0f;
        private const float PLAYER_HIT_COOLDOWN_TIME = 1.0f;
        private bool isPlayerInvulnerable = false;
        private float flashTimer = 0f;
        private Vector2 knockbackVelocity = Vector2.Zero;
        private float knockbackTimer = 0f;
        private float mercyTimer = 0f; // For player refusing to die
        private bool playerMercyActive = false;
        
        // Sword attack fields (for tweening)
        private Sprite swordSprite;
        private bool swordActive = false;
        private Vector2 swordTargetPosition;
        private float swordTweenDuration = 0.3f;
        private float swordTweenTimer = 0f;
        private Vector2 swordStartPosition;
        private float swordRotation = 0f;
        private float swordTargetRotation = 0f;

        // Music progression
        private int musicPhase = 0;
        private string[] musicTracks = {
            "event:/Ingeste/final_content/music/lvl20/azzy_fight00",        // Phase 1 - Initial battle
            "event:/Ingeste/final_content/music/lvl20/azzy_fight01",        // Phase 2 - Progress part 2
            "event:/Ingeste/final_content/music/lvl20/azzy_fight02",        // Phase 3 - Continue
            "event:/Ingeste/final_content/music/lvl20/azzy_theme01"         // Asriel Remember Part 1
        };
        
        private const string ASRIEL_REMEMBER_PART_2 = "event:/Ingeste/final_content/music/lvl20/azzy_theme02";
        private bool asrielRememberTriggered = false;

        // State enums
        public enum BossState
        {
            Idle,
            Attacking,
            Hurt,
            Stunned,
            Transitioning,
            Defeated
        }

        public enum AttackType
        {
            Shoot,
            Beam,
            BiggerBeam,
            BigBeamBall,
            RainbowBlackhole,
            BladeThrower,
            FireShockwave,
            StarsMeteorite,
            ChaosBlaster,         // Rapid-fire star projectiles
            HyperGoner,          // Giant vacuum effect that pulls the player
            GalacticSaber,       // Creates ethereal blades that slash across the screen
            StarstormRain,       // Creates a rain of colorful star projectiles
            LightningStorm,      // Summons lightning bolts from above
            DimensionalRift,     // Creates spatial distortions that the player must avoid
            RainbowInferno,      // Multi-colored flame patterns
            CelestialSpears,     // Summoning spears from multiple angles
            TimewarpVortex,      // Slows down time in certain areas
            PrismBurst,          // Creates prismatic explosions
            SoulResonance,       // Creates waves of energy based on player position
            EternalChaos,        // Combines multiple attack patterns
            SwordSlash           // Tweened sword attack
        }

        public enum AttackPhase
        {
            Charging,
            Executing,
            Recovery
        }

        public AsrielGodBoss(
            Vector2 position,
            Vector2[] nodes,
            int patternIndex,
            float cameraYPastMax,
            bool dialog,
            bool startHit,
            bool cameraLockY,
            string attackSequence = "")
            : base(position,
                   spriteName: "asrielgodboss",  // Must match sprite bank ID in Sprites.xml
                   spriteScale: Vector2.One,
                   maxFall: 160f,
                   collidable: true,
                   solidCollidable: false,
                   gravityMult: 0.0f,  // Asriel floats, no gravity
                   collider: new Monocle.Circle(14f, y: -6f))
        {
            this.patternIndex = patternIndex;
            this.CameraYPastMax = cameraYPastMax;
            this.dialog = dialog;
            this.startHit = startHit;
            this.attackSequenceData = attackSequence;
            this.Add((Component)(this.light = new VertexLight(Color.White, 1f, 32, 64)));
            // Collider already set in base constructor, just store reference
            this.circle = (Monocle.Circle)this.Collider;
            this.Add((Component)new PlayerCollider(new Action<global::Celeste.Player>(this.OnPlayer)));
            this.nodes = new Vector2[nodes.Length + 1];
            this.nodes[0] = this.Position;
            for (int index = 0; index < nodes.Length; ++index)
                this.nodes[index + 1] = nodes[index];
            this.attackCoroutine = new Coroutine(false);
            this.Add((Component)this.attackCoroutine);
            this.triggerBlocksCoroutine = new Coroutine(false);
            this.Add((Component)this.triggerBlocksCoroutine);
            this.Add((Component)new CameraLocker(cameraLockY ? Level.CameraLockModes.FinalBoss : Level.CameraLockModes.FinalBossNoY, 140f, cameraYPastMax));
            this.Add((Component)(this.floatSine = new SineWave(0.6f)));
            this.Add((Component)(this.scaleWiggler = Wiggler.Create(0.6f, 3f)));
            this.Add((Component)(this.chargeSfx = new SoundSource()));
            this.Add((Component)(this.laserSfx = new SoundSource()));
        }

        public AsrielGodBoss(EntityData e, Vector2 offset)
            : this(e.Position + offset, e.NodesOffset(offset), e.Int(nameof(patternIndex)),
                  e.Float("cameraPastY", 120f), e.Bool(nameof(dialog)), e.Bool(nameof(startHit)),
                  e.Bool("cameraLockY", true), e.Attr("attackSequence", ""))
        {
            // Parse custom attack sequence (if any)
            string seq = attackSequenceData.Trim();
            if (!string.IsNullOrEmpty(seq))
            {
                useCustomSequence = true;
                customAttackSteps = parseCustomAttackSequence(seq);
            }
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            this.level = this.SceneAs<Level>();
            
            // Initialize music for boss fight
            this.level.Session.Audio.Music.Event = musicTracks[0];
            this.level.Session.Audio.Apply();
            
            if (this.patternIndex == 0)
            {
                this.NormalSprite = new global::Celeste.PlayerSprite((global::Celeste.PlayerSpriteMode)PlayerSpriteMode.Asriel); // Assuming Asriel sprite mode exists
                this.NormalSprite.Scale.X = -1f;
                if (this.NormalSprite.Has("idle"))
                    this.NormalSprite.Play("idle");
                this.Add((Component)this.NormalSprite);
            }
            else
                this.createBossSprite();
            this.bossBg = this.level.Background.Get<AsrielGodBossStarfield>();
            
            // Check for intro cutscene based on room ID
            string currentRoomId = this.level.Session.Level;
            string introFlagForRoom = $"asriel_god_boss_intro_{currentRoomId}";
            bool hasSeenIntroForThisRoom = this.level.Session.GetFlag(introFlagForRoom) || 
                                            this.level.Session.GetFlag("asriel_god_boss_intro") ||
                                            this.level.Session.GetFlag("asriel_boss_intro");
            
            // Trigger intro cutscene if Kirby hasn't seen it for this room
            if (!hasSeenIntroForThisRoom && ShouldShowIntroForRoom(currentRoomId))
            {
                this.level.Add((Entity)new CS20_AsrielRevealIdentity(currentRoomId));
            }
            
            if (this.patternIndex == 0 && !this.level.Session.GetFlag("asriel_boss_intro") && this.level.Session.Level.Equals("azzyboss-00"))
            {
                if (this.bossBg != null)
                    this.bossBg.Alpha = 0.0f;
                this.Sitting = true;
                this.Position.Y += 16f;
                if (this.NormalSprite.Has("back"))
                    this.NormalSprite.Play("back");
                this.NormalSprite.Scale.X = 1f;
            }
            else if (this.patternIndex == 0 && !this.level.Session.GetFlag("asriel_boss_mid") && this.level.Session.Level.Equals("azzyboss-25"))
                this.level.Add((Entity)new CS20_BossMid()); // Adjust cutscene if needed
            else if (this.startHit)
                Alarm.Set((Entity)this, 0.5f, (Action)(() => this.OnPlayer((global::Celeste.Player)null)));
            this.light.Position = (this.Sprite != null ? (GraphicsComponent)this.Sprite : (GraphicsComponent)this.NormalSprite).Position + new Vector2(0.0f, -10f);
        }
        
        /// <summary>
        /// Determines if the intro cutscene should be shown for a given room ID.
        /// Override or modify this list to add more rooms that trigger the intro.
        /// </summary>
        private bool ShouldShowIntroForRoom(string roomId)
        {
            // List of room IDs where the Asriel God Boss intro should play
            // Add any room ID that contains this boss where you want an intro
            string[] introRoomIds = new string[]
            {
                "azzyboss-00",      // Original intro room
                // Add more room IDs here as needed, e.g.:
                // "your-custom-room-01",
                // "kirby-asriel-fight",
            };
            
            foreach (string id in introRoomIds)
            {
                if (roomId.Equals(id, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            
            return false;
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            this.fallingBlocks = this.Scene.Tracker.GetEntitiesCopy<FallingBlock>();
            this.fallingBlocks.Sort((a, b) => (int)(a.X - b.X));
            this.movingBlocks = this.Scene.Tracker.GetEntitiesCopy<FinalBossMovingBlock>();
            this.movingBlocks.Sort((a, b) => (int)(a.X - b.X));
        }

        private void createBossSprite()
        {
            // Sprite is already created by BossActor base constructor
            // Just configure it if it exists
            if (this.Sprite != null)
            {
                this.Sprite.OnFrameChange = (anim =>
                {
                    if (anim == "idle" && this.Sprite.CurrentAnimationFrame == 18)
                        Audio.Play(SFX_SPARKLES, this.Position);
                });
            }
            this.facing = -1;
            if (this.NormalSprite != null)
            {
                this.Sprite.Position = this.NormalSprite.Position;
                this.Remove((Component)this.NormalSprite);
            }
            if (this.normalHair != null)
                this.Remove((Component)this.normalHair);
            this.NormalSprite = null;
            this.normalHair = null;
        }

        public Vector2 BeamOrigin => this.Center + (this.Sprite?.Position ?? Vector2.Zero) + new Vector2(0.0f, -14f);
        public Vector2 ShotOrigin => this.Center + (this.Sprite?.Position ?? Vector2.Zero) + new Vector2(6f * (this.Sprite?.Scale.X ?? 1f), 2f);

        public override void Update()
        {
            base.Update();

            // Update cooldowns
            if (playerHitCooldown > 0f)
                playerHitCooldown -= Engine.DeltaTime;

            if (flashTimer > 0f)
                flashTimer -= Engine.DeltaTime;

            if (knockbackTimer > 0f)
            {
                knockbackTimer -= Engine.DeltaTime;
                Position += knockbackVelocity * Engine.DeltaTime;
                knockbackVelocity = Vector2.Lerp(knockbackVelocity, Vector2.Zero, Engine.DeltaTime * 5f);
                
                // When knockback ends, start moving to next node
                if (knockbackTimer <= 0f && nodes != null && nodes.Length > 0)
                {
                    Moving = true;
                    Add(new Coroutine(MoveToNextNode()));
                }
            }

            if (mercyTimer > 0f)
            {
                mercyTimer -= Engine.DeltaTime;
                if (mercyTimer <= 0f)
                    playerMercyActive = false;
            }

            Sprite sprite = this.Sprite != null ? this.Sprite : (Sprite)this.NormalSprite;
            if (!this.Sitting)
            {
                var entity = this.Scene.Tracker.GetEntity<global::Celeste.Player>();
                if (!this.Moving && entity != null)
                {
                    if (this.facing == -1 && entity.X > this.X + 20.0f)
                    {
                        this.facing = 1;
                        this.scaleWiggler.Start();
                    }
                    else if (this.facing == 1 && entity.X < this.X - 20.0f)
                    {
                        this.facing = -1;
                        this.scaleWiggler.Start();
                    }
                }
                if (!this.playerHasMoved && entity != null && entity.Speed != Vector2.Zero)
                {
                    this.playerHasMoved = true;
                    if (this.patternIndex != 0)
                        this.startAttacking();
                    // this.triggerMovingBlocks(0); // Commented out as method doesn't exist
                }
                if (!this.Moving)
                    sprite.Position = this.avoidPos + new Vector2(this.floatSine.Value * 3f, this.floatSine.ValueOverTwo * 4f);
                else
                    sprite.Position = Calc.Approach(sprite.Position, Vector2.Zero, 12f * Engine.DeltaTime);
                float radius = this.circle.Radius;
                this.circle.Radius = 6f;
                var dashBlock = this.CollideFirst<DashBlock>();
                if (dashBlock != null)
                    dashBlock.Break(Center, -Vector2.UnitY, true, false);
                this.circle.Radius = radius;
                if (!this.level.IsInBounds(this.Position, 24f))
                {
                    this.Active = this.Visible = this.Collidable = false;
                    return;
                }
                Vector2 target;
                if (!this.Moving && entity != null)
                {
                    float length = Calc.ClampedMap((this.Center - entity.Center).Length(), 32f, 88f, 12f, 0.0f);
                    target = length > 0.0f ? (this.Center - entity.Center).SafeNormalize(length) : Vector2.Zero;
                }
                else
                    target = Vector2.Zero;
                this.avoidPos = Calc.Approach(this.avoidPos, target, 40f * Engine.DeltaTime);
            }
            this.light.Position = sprite.Position + new Vector2(0.0f, -10f);

            // Check for music progression
            UpdateMusicProgression();

            // Check for dialog triggers based on phase/health
            CheckDialogTriggers();
        }

        private void UpdateMusicProgression()
        {
            int newPhase = 0;
            if (currentHealth <= 7) newPhase = 1;
            if (currentHealth <= 5) newPhase = 2;
            if (currentHealth <= 2) newPhase = 3;

            if (newPhase != musicPhase && newPhase < musicTracks.Length)
            {
                musicPhase = newPhase;
                level.Session.Audio.Music.Event = musicTracks[musicPhase];
                level.Session.Audio.Apply();
            }
            
            // Trigger Asriel Remember music during emotional phases
            if (currentHealth <= 1 && !asrielRememberTriggered)
            {
                asrielRememberTriggered = true;
                level.Session.Audio.Music.Event = ASRIEL_REMEMBER_PART_2;
                level.Session.Audio.Apply();
            }
        }

        public override void Render()
        {
            if (this.Sprite != null)
            {
                this.Sprite.Scale.X = (float)this.facing;
                this.Sprite.Scale.Y = 1f;
                this.Sprite.Scale *= (float)(1.0 + this.scaleWiggler.Value * 0.2);

                // Apply flash effect when hit
                if (flashTimer > 0f)
                {
                    this.Sprite.Color = Color.Lerp(Color.White, Color.Red, flashTimer / 0.2f);
                }
                else
                {
                    this.Sprite.Color = Color.White;
                }
            }
            if (this.NormalSprite != null)
            {
                Vector2 position = this.NormalSprite.Position;
                this.NormalSprite.Position = this.NormalSprite.Position.Floor();
                base.Render();
                this.NormalSprite.Position = position;
            }
            else
                base.Render();
        }

        public void OnPlayer(global::Celeste.Player player)
        {
            try
            {
                // Check if this is Kirby player mode
                bool isKirbyMode = player.IsKirbyMode();
                
                // Apply hit effect with time slowdown
                ApplyHitEffect(player, isKirbyMode);
                
                // Apply shockwave pushback to Asriel
                ApplyShockwavePushback(player, isKirbyMode);
                
                // Player refuses to die - mercy system
                if (!playerMercyActive)
                {
                    playerMercyActive = true;
                    mercyTimer = 2.0f; // 2 seconds of mercy
                    Audio.Play(SFX_SPELLCAST_GLITCH, player.Position); // Mercy sound
                    // Visual effect for mercy
                    level.Particles.Emit(PBurst, 10, player.Center, Vector2.One * 4f);
                }
                // Don't damage the player during mercy
                return;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, nameof(DesoloZantas), $"Error in AsrielGodBoss OnPlayer collision: {ex.Message}");
            }
        }
        
        private float hitSlowdownTimer = 0f;
        private bool isHitSlowdownActive = false;
        
        /// <summary>
        /// Apply hit effect with time slowdown (pitch slowdown) when player hits Asriel
        /// </summary>
        private void ApplyHitEffect(global::Celeste.Player player, bool isKirbyMode)
        {
            // Don't apply multiple hit effects at once
            if (isHitSlowdownActive) return;
            
            // Start hit slowdown coroutine
            Add(new Coroutine(HitSlowdownEffect(player, isKirbyMode)));
        }
        
        private IEnumerator HitSlowdownEffect(global::Celeste.Player player, bool isKirbyMode)
        {
            isHitSlowdownActive = true;
            
            // Store original time rate
            float originalTimeRate = Engine.TimeRate;
            
            // Slowdown parameters - shorter and more impactful for hit feedback
            float slowdownScale = 0.3f; // Slow to 30% speed
            float slowdownDuration = 0.15f; // Very brief slowdown
            float pitchSlowdown = 0.5f; // Music pitch during slowdown (lower = deeper)
            
            // Apply time slowdown
            Engine.TimeRate = slowdownScale;
            
            // Apply music pitch slowdown for dramatic effect
            if (level != null)
            {
                Audio.SetMusicParam("pitch", pitchSlowdown);
            }
            
            // Hit sound effect
            string hitSound = isKirbyMode ? "event:/game/general/thing_booped" : "event:/game/general/landing";
            if (player != null)
            {
                Audio.Play(hitSound, player.Position);
            }
            
            // Visual feedback - flash and particles (null check for level)
            if (level != null)
            {
                level.Flash(Color.White, true);
                level.Shake(0.2f);
                
                // Emit hit particles in all directions
                if (player != null)
                {
                    level.ParticlesFG.Emit(PBurst, 12, player.Center, Vector2.One * 12f);
                }
                
                // Kirby-specific effects
                if (isKirbyMode && player != null)
                {
                    // Extra sparkle for Kirby hits
                    level.Particles.Emit(PBurst, 15, player.Center, Vector2.One * 8f);
                    Audio.Play(SFX_SPARKLES, player.Position);
                }
            }
            
            // Wait for slowdown duration
            yield return slowdownDuration;
            
            // Restore normal time rate
            Engine.TimeRate = originalTimeRate;
            
            // Restore normal music pitch
            if (level != null)
            {
                Audio.SetMusicParam("pitch", 1f);
            }
            
            isHitSlowdownActive = false;
        }

        /// <summary>
        /// Apply shockwave pushback effect when player hits Asriel
        /// </summary>
        private void ApplyShockwavePushback(global::Celeste.Player player, bool isKirbyMode)
        {
            // Calculate direction from player to Asriel
            Vector2 pushDirection = (Center - player.Center).SafeNormalize();
            
            // If no valid direction, push away from player's facing direction
            if (pushDirection == Vector2.Zero)
            {
                pushDirection = new Vector2(player.Facing == Facings.Right ? 1 : -1, 0);
            }
            
            // Pushback strength - stronger for Kirby mode
            float pushStrength = isKirbyMode ? 400f : 300f;
            
            // Apply knockback velocity
            knockbackVelocity = pushDirection * pushStrength;
            knockbackTimer = 0.3f; // Duration of knockback
            
            // Create shockwave visual effect
            CreateShockwaveEffect(isKirbyMode);
            
            // Play pushback sound
            Audio.Play("event:/Ingeste/final_content/char/asriel/Asriel_scream_hit", Center);
            
            // Screen shake
            level.Shake(0.3f);
            
            // Emit particles in a radial burst from Asriel
            for (int i = 0; i < 16; i++)
            {
                float angle = (i / 16f) * MathHelper.TwoPi;
                Vector2 particleDir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                level.ParticlesFG.Emit(PBurst, 1, Center + particleDir * 8f, Vector2.One * 4f, angle);
            }
            
            // Extra effects for Kirby mode
            if (isKirbyMode)
            {
                // More intense flash
                level.Flash(Color.Cyan, true);
                // Additional particle ring
                for (int i = 0; i < 20; i++)
                {
                    float angle = (i / 20f) * MathHelper.TwoPi;
                    Vector2 particleDir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                    level.Particles.Emit(PBurst, 1, Center + particleDir * 16f, Vector2.One * 6f, angle);
                }
            }
        }

        /// <summary>
        /// Create a visual shockwave ring effect
        /// </summary>
        private void CreateShockwaveEffect(bool isKirbyMode)
        {
            // Create expanding ring of particles
            Add(new Coroutine(ShockwaveRingEffect(isKirbyMode)));
        }

        private IEnumerator ShockwaveRingEffect(bool isKirbyMode)
        {
            float duration = 0.5f;
            float maxRadius = isKirbyMode ? 60f : 45f;
            int particleCount = isKirbyMode ? 32 : 24;
            
            for (float t = 0; t < duration; t += Engine.DeltaTime)
            {
                float progress = t / duration;
                float currentRadius = Ease.CubeOut(progress) * maxRadius;
                
                // Emit particles in a ring
                for (int i = 0; i < particleCount; i++)
                {
                    float angle = (i / (float)particleCount) * MathHelper.TwoPi;
                    Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * currentRadius;
                    
                    // Emit particle at ring position
                    level.ParticlesFG.Emit(PBurst, 1, Center + offset, Vector2.One * 2f);
                }
                
                yield return null;
            }
        }

        private void SetState(BossState newState)
        {
            if (currentState != newState)
            {
                currentState = newState;
                OnStateChanged(newState);
            }
        }

        private void OnStateChanged(BossState newState)
        {
            switch (newState)
            {
                case BossState.Defeated:
                    stopAttacking();
                    Collidable = false;
                    break;
                case BossState.Hurt:
                    stopAttacking();
                    break;
                case BossState.Attacking:
                    startAttacking();
                    break;
            }
        }

        /// <summary>
        /// Public method to trigger the startHit behavior from external triggers.
        /// This starts the boss attack sequence.
        /// </summary>
        public void TriggerStartHit()
        {
            if (currentState == BossState.Defeated) return;
            
            // Play a sound effect to indicate the boss is triggered
            Audio.Play(SFX_SEGA_POWER_01, Position);
            
            // Start the attack sequence
            startAttacking();
        }
        
        /// <summary>
        /// Public method to manually trigger the Angel of Death phase transition.
        /// Call this after Kirby survives the HyperGoner attack.
        /// </summary>
        public void TriggerAngelPhaseTransition()
        {
            if (hyperGonerPhaseTransitionTriggered) return;
            if (level.Session.GetFlag("asriel_boss_end_hypergoner")) return;
            
            hyperGonerPhaseTransitionTriggered = true;
            level.Add(new CS20_BossEnd());
        }

        /// <summary>
        /// Public method to move Asriel to a target position.
        /// </summary>
        /// <param name="target">The target position to move to.</param>
        /// <param name="speed">The movement speed.</param>
        public void MoveToTarget(Vector2 target, float speed = 300f)
        {
            if (currentState == BossState.Defeated) return;
            
            Moving = true;
            Add(new Coroutine(MoveToTargetRoutine(target, speed)));
        }

        private IEnumerator MoveToTargetRoutine(Vector2 target, float speed)
        {
            while (Vector2.Distance(Position, target) > 4f)
            {
                Vector2 direction = (target - Position).SafeNormalize();
                Position += direction * speed * Engine.DeltaTime;
                yield return null;
            }
            Position = target;
            Moving = false;
        }

        /// <summary>
        /// Move to the next node in the node sequence after knockback
        /// </summary>
        private IEnumerator MoveToNextNode()
        {
            if (nodes == null || nodes.Length == 0)
            {
                Moving = false;
                yield break;
            }

            // Move to next node in sequence
            nodeIndex = (nodeIndex + 1) % nodes.Length;
            Vector2 targetNode = nodes[nodeIndex];
            
            float speed = move_speed;
            
            while (Vector2.Distance(Position, targetNode) > 4f)
            {
                Vector2 direction = (targetNode - Position).SafeNormalize();
                Position += direction * speed * Engine.DeltaTime;
                yield return null;
            }
            
            Position = targetNode;
            Moving = false;
        }

        private void startAttacking()
        {
            if (attackCoroutine != null && attackCoroutine.Active) return;
            if (currentState == BossState.Defeated) return;

            isAttacking = true;
            SetState(BossState.Attacking);
            attackCoroutine = new Coroutine(attackSequence());
            Add(attackCoroutine);
        }

        private void StartAttacking()
        {
            switch (patternIndex)
            {
                case 0:
                case 1:
                    attackCoroutine.Replace(Attack01Sequence());
                    break;
                case 2:
                    attackCoroutine.Replace(Attack02Sequence());
                    break;
                case 3:
                    attackCoroutine.Replace(Attack03Sequence());
                    break;
                case 4:
                    attackCoroutine.Replace(Attack04Sequence());
                    break;
                case 5:
                    attackCoroutine.Replace(Attack05Sequence());
                    break;
                case 6:
                    attackCoroutine.Replace(Attack06Sequence());
                    break;
                case 7:
                    attackCoroutine.Replace(Attack07Sequence());
                    break;
                case 8:
                    attackCoroutine.Replace(Attack08Sequence());
                    break;
                case 9:
                    attackCoroutine.Replace(Attack09Sequence());
                    break;
                case 10:
                    attackCoroutine.Replace(Attack10Sequence());
                    break;
                case 11:
                    attackCoroutine.Replace(Attack11Sequence());
                    break;
                case 12:
                    attackCoroutine.Replace(Attack12Sequence());
                    break;
                case 13:
                    attackCoroutine.Replace(Attack13Sequence());
                    break;
                case 14:
                    attackCoroutine.Replace(Attack14Sequence());
                    break;
                case 15:
                    attackCoroutine.Replace(Attack15Sequence());
                    break;
                case 16:
                    attackCoroutine.Replace(Attack16Sequence());
                    break;
                case 17:
                    attackCoroutine.Replace(Attack17Sequence());
                    break;
                case 18:
                    attackCoroutine.Replace(Attack18Sequence());
                    break;
                case 19:
                    attackCoroutine.Replace(Attack19Sequence());
                    break;
                case 20:
                    attackCoroutine.Replace(Attack20Sequence());
                    break;
                case 21:
                    attackCoroutine.Replace(Attack21Sequence());
                    break;
                case 22:
                    attackCoroutine.Replace(Attack22Sequence());
                    break;
                case 23:
                    attackCoroutine.Replace(Attack23Sequence());
                    break;
                case 24:
                    attackCoroutine.Replace(Attack24Sequence());
                    break;
                case 25:
                    attackCoroutine.Replace(Attack25Sequence());
                    break;
                case 26:
                    attackCoroutine.Replace(Attack26Sequence());
                    break;
                case 27:
                    attackCoroutine.Replace(Attack27Sequence());
                    break;
                case 28:
                    attackCoroutine.Replace(Attack28Sequence());
                    break;
                case 29:
                    attackCoroutine.Replace(Attack29Sequence());
                    break;
                case 30:
                    attackCoroutine.Replace(Attack30Sequence());
                    break;
                case 31:
                    attackCoroutine.Replace(Attack31Sequence());
                    break;
                case 32:
                    attackCoroutine.Replace(Attack32Sequence());
                    break;
                case 33:
                    attackCoroutine.Replace(Attack33Sequence());
                    break;
                case 34:
                    attackCoroutine.Replace(Attack34Sequence());
                    break;
                case 35:
                    attackCoroutine.Replace(Attack35Sequence());
                    break;
                case 36:
                    attackCoroutine.Replace(Attack36Sequence());
                    break;
                case 37:
                    attackCoroutine.Replace(Attack37Sequence());
                    break;
                case 38:
                    attackCoroutine.Replace(Attack38Sequence());
                    break;
                case 39:
                    attackCoroutine.Replace(Attack39Sequence());
                    break;
                case 40:
                    attackCoroutine.Replace(Attack40Sequence());
                    break;
                case 41:
                    attackCoroutine.Replace(Attack41Sequence());
                    break;
                case 42:
                    attackCoroutine.Replace(Attack42Sequence());
                    break;
                case 43:
                    attackCoroutine.Replace(Attack43Sequence());
                    break;
                case 44:
                    attackCoroutine.Replace(Attack44Sequence());
                    break;
                case 45:
                    attackCoroutine.Replace(Attack45Sequence());
                    break;
                case 46:
                    attackCoroutine.Replace(Attack46Sequence());
                    break;
                case 47:
                    attackCoroutine.Replace(Attack47Sequence());
                    break;
                case 48:
                    attackCoroutine.Replace(Attack48Sequence());
                    break;
                case 49:
                    attackCoroutine.Replace(Attack49Sequence());
                    break;
                case 50:
                    attackCoroutine.Replace(Attack50Sequence());
                    break;
                case 51:
                    attackCoroutine.Replace(Attack51Sequence());
                    break;
                case 52:
                    attackCoroutine.Replace(Attack52Sequence());
                    break;
                case 53:
                    attackCoroutine.Replace(Attack53Sequence());
                    break;
                case 54:
                    attackCoroutine.Replace(Attack54Sequence());
                    break;
                case 55:
                    attackCoroutine.Replace(Attack55Sequence());
                    break;
                case 56:
                    attackCoroutine.Replace(Attack56Sequence());
                    break;
                case 57:
                    attackCoroutine.Replace(Attack57Sequence());
                    break;
                case 58:
                    attackCoroutine.Replace(Attack58Sequence());
                    break;
                case 59:
                    attackCoroutine.Replace(Attack59Sequence());
                    break;
                case 60:
                    attackCoroutine.Replace(Attack60Sequence());
                    break;
            }
        }

        private void stopAttacking()
        {
            isAttacking = false;
            if (attackCoroutine != null)
                attackCoroutine.Active = false;
        }

        private IEnumerator attackSequence()
        {
            if (useCustomSequence && customAttackSteps != null && customAttackSteps.Count > 0)
            {
                while (isAttacking && currentState == BossState.Attacking)
                {
                    foreach (var step in customAttackSteps)
                    {
                        if (!isAttacking || currentState != BossState.Attacking) yield break;
                        switch (step.Action)
                        {
                            case nameof(shoot):
                                currentAttackType = AttackType.Shoot;
                                startShootCharge();
                                yield return 0.15f;
                                shoot(MathHelper.ToRadians(step.Arg));
                                break;
                            case nameof(beam):
                                currentAttackType = AttackType.Beam;
                                yield return beam();
                                break;
                            case "biggerbeam":
                                currentAttackType = AttackType.BiggerBeam;
                                yield return biggerBeam();
                                break;
                            case "bigbeamball":
                                currentAttackType = AttackType.BigBeamBall;
                                yield return bigBeamBall();
                                break;
                            case "rainbowblackhole":
                                currentAttackType = AttackType.RainbowBlackhole;
                                yield return rainbowBlackhole();
                                break;
                            case "bladethrower":
                                currentAttackType = AttackType.BladeThrower;
                                yield return bladeThrower();
                                break;
                            case "fireshockwave":
                                currentAttackType = AttackType.FireShockwave;
                                yield return fireShockwave();
                                break;
                            case "starsmeteorite":
                                currentAttackType = AttackType.StarsMeteorite;
                                yield return starsMeteorite();
                                break;
                            default:
                                break;
                        }
                        if (step.Delay > 0f)
                            yield return step.Delay;
                    }
                }
                yield break;
            }
        }

        private void startShootCharge()
        {
            currentAttackPhase = AttackPhase.Charging;
            if (Sprite != null && Sprite.CurrentAnimationID != "castsp")
                Sprite.Play("castsp");
            if (!chargeSfx.Playing)
                chargeSfx.Play("event:/char/badeline/boss_bullet");
        }

        // Pattern sequences following the reference format
        private IEnumerator Attack01Sequence()
        {
            startShootCharge();
            while (true)
            {
                yield return 0.5f;
                shoot();
                yield return 1f;
                startShootCharge();
                yield return 0.15f;
                yield return 0.3f;
            }
        }

        private IEnumerator Attack02Sequence()
        {
            while (true)
            {
                yield return 0.5f;
                yield return beam();
                yield return 0.4f;
                startShootCharge();
                yield return 0.3f;
                shoot();
                yield return 0.5f;
                yield return 0.3f;
            }
        }

        private IEnumerator Attack03Sequence()
        {
            startShootCharge();
            yield return 0.1f;
            while (true)
            {
                for (int i = 0; i < 5; i++)
                {
                    var entity = level.Tracker.GetEntity<global::Celeste.Player>();
                    if (entity != null)
                    {
                        Vector2 at = entity.Center;
                        for (int j = 0; j < 2; j++)
                        {
                            shootAt(at);
                            yield return 0.15f;
                        }
                    }
                    if (i < 4)
                    {
                        startShootCharge();
                        yield return 0.5f;
                    }
                }
                yield return 2f;
                startShootCharge();
                yield return 0.7f;
            }
        }

        private IEnumerator Attack04Sequence()
        {
            startShootCharge();
            yield return 0.1f;
            while (true)
            {
                for (int i = 0; i < 5; i++)
                {
                    var entity = level.Tracker.GetEntity<global::Celeste.Player>();
                    if (entity != null)
                    {
                        Vector2 at = entity.Center;
                        for (int j = 0; j < 2; j++)
                        {
                            shootAt(at);
                            yield return 0.15f;
                        }
                    }
                    if (i < 4)
                    {
                        startShootCharge();
                        yield return 0.5f;
                    }
                }
                yield return 1.5f;
                yield return beam();
                yield return 1.5f;
                startShootCharge();
            }
        }

        private IEnumerator Attack05Sequence()
        {
            yield return 0.2f;
            while (true)
            {
                yield return beam();
                yield return 0.6f;
                startShootCharge();
                yield return 0.3f;
                for (int i = 0; i < 3; i++)
                {
                    var entity = level.Tracker.GetEntity<global::Celeste.Player>();
                    if (entity != null)
                    {
                        Vector2 at = entity.Center;
                        for (int j = 0; j < 2; j++)
                        {
                            shootAt(at);
                            yield return 0.15f;
                        }
                    }
                    if (i < 2)
                    {
                        startShootCharge();
                        yield return 0.5f;
                    }
                }
                yield return 0.8f;
            }
        }

        private IEnumerator Attack06Sequence()
        {
            while (true)
            {
                yield return beam();
                yield return 0.7f;
            }
        }

        private IEnumerator Attack07Sequence()
        {
            while (true)
            {
                shoot();
                yield return 0.8f;
                startShootCharge();
                yield return 0.8f;
            }
        }

        private IEnumerator Attack08Sequence()
        {
            while (true)
            {
                yield return 0.1f;
                yield return beam();
                yield return 0.8f;
            }
        }

        private IEnumerator Attack09Sequence()
        {
            startShootCharge();
            while (true)
            {
                yield return 0.5f;
                shoot();
                yield return 0.15f;
                startShootCharge();
                shoot();
                yield return 0.4f;
                startShootCharge();
                yield return 0.1f;
            }
        }

        private IEnumerator Attack10Sequence()
        {
            yield break;
        }

        private IEnumerator Attack11Sequence()
        {
            if (nodeIndex == 0)
            {
                startShootCharge();
                yield return 0.6f;
            }
            while (true)
            {
                shoot();
                yield return 1.9f;
                startShootCharge();
                yield return 0.6f;
            }
        }

        private IEnumerator Attack12Sequence()
        {
            while (true)
            {
                yield return rainbowBlackhole();
                yield return 2.5f;
            }
        }

        private IEnumerator Attack13Sequence()
        {
            if (nodeIndex != 0)
            {
                yield return Attack01Sequence();
            }
        }

        private IEnumerator Attack14Sequence()
        {
            while (true)
            {
                yield return 0.2f;
                yield return beam();
                yield return 0.3f;
            }
        }

        private IEnumerator Attack15Sequence()
        {
            while (true)
            {
                yield return 0.2f;
                yield return beam();
                yield return 1.2f;
            }
        }

        private IEnumerator Attack16Sequence()
        {
            while (true)
            {
                yield return bladeThrower();
                yield return 1.5f;
                startShootCharge();
                yield return 0.5f;
                shoot();
                yield return 0.8f;
            }
        }

        private IEnumerator Attack17Sequence()
        {
            while (true)
            {
                yield return bigBeamBall();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack18Sequence()
        {
            while (true)
            {
                yield return fireShockwave();
                yield return 1.8f;
                startShootCharge();
                yield return 0.5f;
                shoot();
                yield return 0.5f;
            }
        }

        private IEnumerator Attack19Sequence()
        {
            while (true)
            {
                yield return starsMeteorite();
                yield return 2.5f;
            }
        }

        private IEnumerator Attack20Sequence()
        {
            while (true)
            {
                yield return biggerBeam();
                yield return 1.5f;
                startShootCharge();
                yield return 0.3f;
                shoot();
                yield return 0.8f;
            }
        }

        private IEnumerator Attack21Sequence()
        {
            startShootCharge();
            while (true)
            {
                for (int i = 0; i < 3; i++)
                {
                    yield return 0.3f;
                    shoot();
                    yield return 0.2f;
                    startShootCharge();
                }
                yield return 1.0f;
                yield return beam();
                yield return 1.2f;
                startShootCharge();
            }
        }

        private IEnumerator Attack22Sequence()
        {
            while (true)
            {
                yield return bladeThrower();
                yield return 0.8f;
                yield return bladeThrower();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack23Sequence()
        {
            while (true)
            {
                yield return rainbowBlackhole();
                yield return 1.5f;
                yield return fireShockwave();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack24Sequence()
        {
            while (true)
            {
                yield return starsMeteorite();
                yield return 1.0f;
                startShootCharge();
                yield return 0.5f;
                shoot();
                yield return 0.5f;
                shoot();
                yield return 1.5f;
                startShootCharge();
            }
        }

        private IEnumerator Attack25Sequence()
        {
            while (true)
            {
                yield return bigBeamBall();
                yield return 1.2f;
                yield return bladeThrower();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack26Sequence()
        {
            startShootCharge();
            while (true)
            {
                for (int i = 0; i < 4; i++)
                {
                    yield return 0.4f;
                    shoot();
                    yield return 0.15f;
                    startShootCharge();
                }
                yield return 0.8f;
                yield return biggerBeam();
                yield return 1.5f;
                startShootCharge();
            }
        }

        private IEnumerator Attack27Sequence()
        {
            while (true)
            {
                yield return fireShockwave();
                yield return 0.8f;
                yield return starsMeteorite();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack28Sequence()
        {
            while (true)
            {
                yield return beam();
                yield return 0.5f;
                yield return bladeThrower();
                yield return 1.2f;
            }
        }

        private IEnumerator Attack29Sequence()
        {
            while (true)
            {
                yield return rainbowBlackhole();
                yield return 1.0f;
                startShootCharge();
                yield return 0.3f;
                for (int i = 0; i < 3; i++)
                {
                    shoot();
                    yield return 0.2f;
                    startShootCharge();
                    yield return 0.3f;
                }
                yield return 1.5f;
            }
        }

        private IEnumerator Attack30Sequence()
        {
            while (true)
            {
                yield return bigBeamBall();
                yield return 0.8f;
                yield return fireShockwave();
                yield return 0.8f;
                yield return starsMeteorite();
                yield return 2.5f;
            }
        }

        private IEnumerator Attack31Sequence()
        {
            startShootCharge();
            while (true)
            {
                yield return 0.3f;
                shoot();
                yield return 0.3f;
                yield return beam();
                yield return 0.5f;
                startShootCharge();
                yield return 0.3f;
                shoot();
                yield return 1.0f;
                startShootCharge();
            }
        }

        private IEnumerator Attack32Sequence()
        {
            while (true)
            {
                yield return bladeThrower();
                yield return 0.5f;
                yield return biggerBeam();
                yield return 1.5f;
            }
        }

        private IEnumerator Attack33Sequence()
        {
            while (true)
            {
                for (int i = 0; i < 2; i++)
                {
                    yield return rainbowBlackhole();
                    yield return 1.2f;
                }
                yield return 2.0f;
            }
        }

        private IEnumerator Attack34Sequence()
        {
            while (true)
            {
                yield return starsMeteorite();
                yield return 0.8f;
                yield return bladeThrower();
                yield return 0.8f;
                yield return fireShockwave();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack35Sequence()
        {
            startShootCharge();
            while (true)
            {
                for (int i = 0; i < 6; i++)
                {
                    yield return 0.25f;
                    shoot();
                    yield return 0.15f;
                    startShootCharge();
                }
                yield return 1.5f;
                startShootCharge();
            }
        }

        private IEnumerator Attack36Sequence()
        {
            while (true)
            {
                yield return bigBeamBall();
                yield return 0.6f;
                yield return biggerBeam();
                yield return 1.8f;
            }
        }

        private IEnumerator Attack37Sequence()
        {
            while (true)
            {
                yield return beam();
                yield return 0.3f;
                yield return bladeThrower();
                yield return 0.5f;
                yield return fireShockwave();
                yield return 1.5f;
            }
        }

        private IEnumerator Attack38Sequence()
        {
            while (true)
            {
                yield return rainbowBlackhole();
                yield return 0.8f;
                yield return starsMeteorite();
                yield return 0.8f;
                yield return bigBeamBall();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack39Sequence()
        {
            startShootCharge();
            while (true)
            {
                for (int i = 0; i < 5; i++)
                {
                    var entity = level.Tracker.GetEntity<global::Celeste.Player>();
                    if (entity != null)
                    {
                        shootAt(entity.Center);
                    }
                    yield return 0.3f;
                    startShootCharge();
                    yield return 0.3f;
                }
                yield return 1.2f;
                yield return bladeThrower();
                yield return 1.5f;
                startShootCharge();
            }
        }

        private IEnumerator Attack40Sequence()
        {
            while (true)
            {
                yield return fireShockwave();
                yield return 0.5f;
                yield return beam();
                yield return 0.5f;
                yield return starsMeteorite();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack41Sequence()
        {
            while (true)
            {
                yield return biggerBeam();
                yield return 0.8f;
                yield return rainbowBlackhole();
                yield return 1.8f;
            }
        }

        private IEnumerator Attack42Sequence()
        {
            startShootCharge();
            while (true)
            {
                yield return 0.2f;
                shoot();
                yield return 0.2f;
                yield return bladeThrower();
                yield return 0.5f;
                startShootCharge();
                yield return 0.3f;
                shoot();
                yield return 1.0f;
                startShootCharge();
            }
        }

        private IEnumerator Attack43Sequence()
        {
            while (true)
            {
                yield return bigBeamBall();
                yield return 0.6f;
                yield return fireShockwave();
                yield return 0.6f;
                yield return bladeThrower();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack44Sequence()
        {
            while (true)
            {
                for (int i = 0; i < 3; i++)
                {
                    yield return beam();
                    yield return 0.4f;
                }
                yield return 1.5f;
            }
        }

        private IEnumerator Attack45Sequence()
        {
            while (true)
            {
                yield return starsMeteorite();
                yield return 0.5f;
                yield return rainbowBlackhole();
                yield return 0.8f;
                yield return biggerBeam();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack46Sequence()
        {
            startShootCharge();
            while (true)
            {
                for (int i = 0; i < 4; i++)
                {
                    yield return 0.3f;
                    shoot();
                    yield return 0.1f;
                    startShootCharge();
                }
                yield return 0.8f;
                yield return bladeThrower();
                yield return 0.8f;
                yield return fireShockwave();
                yield return 1.8f;
                startShootCharge();
            }
        }

        private IEnumerator Attack47Sequence()
        {
            while (true)
            {
                yield return bigBeamBall();
                yield return 0.5f;
                yield return beam();
                yield return 0.5f;
                yield return starsMeteorite();
                yield return 1.8f;
            }
        }

        private IEnumerator Attack48Sequence()
        {
            while (true)
            {
                yield return rainbowBlackhole();
                yield return 0.6f;
                yield return bladeThrower();
                yield return 0.6f;
                yield return biggerBeam();
                yield return 1.5f;
            }
        }

        private IEnumerator Attack49Sequence()
        {
            while (true)
            {
                yield return fireShockwave();
                yield return 0.4f;
                yield return starsMeteorite();
                yield return 0.8f;
                yield return bigBeamBall();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack50Sequence()
        {
            startShootCharge();
            while (true)
            {
                for (int i = 0; i < 8; i++)
                {
                    yield return 0.2f;
                    shoot();
                    yield return 0.15f;
                    startShootCharge();
                }
                yield return 1.0f;
                yield return beam();
                yield return 0.8f;
                yield return bladeThrower();
                yield return 2.0f;
                startShootCharge();
            }
        }

        private IEnumerator Attack51Sequence()
        {
            while (true)
            {
                yield return biggerBeam();
                yield return 0.5f;
                yield return fireShockwave();
                yield return 0.5f;
                yield return rainbowBlackhole();
                yield return 1.8f;
            }
        }

        private IEnumerator Attack52Sequence()
        {
            while (true)
            {
                yield return bladeThrower();
                yield return 0.4f;
                yield return starsMeteorite();
                yield return 0.6f;
                yield return bigBeamBall();
                yield return 1.8f;
            }
        }

        private IEnumerator Attack53Sequence()
        {
            startShootCharge();
            while (true)
            {
                for (int i = 0; i < 3; i++)
                {
                    var entity = level.Tracker.GetEntity<global::Celeste.Player>();
                    if (entity != null)
                    {
                        for (int j = 0; j < 3; j++)
                        {
                            shootAt(entity.Center);
                            yield return 0.12f;
                        }
                    }
                    yield return 0.4f;
                    startShootCharge();
                }
                yield return 1.0f;
                yield return rainbowBlackhole();
                yield return 1.8f;
                startShootCharge();
            }
        }

        private IEnumerator Attack54Sequence()
        {
            while (true)
            {
                yield return beam();
                yield return 0.3f;
                yield return fireShockwave();
                yield return 0.5f;
                yield return biggerBeam();
                yield return 1.5f;
            }
        }

        private IEnumerator Attack55Sequence()
        {
            while (true)
            {
                yield return bigBeamBall();
                yield return 0.4f;
                yield return bladeThrower();
                yield return 0.6f;
                yield return starsMeteorite();
                yield return 1.8f;
            }
        }

        private IEnumerator Attack56Sequence()
        {
            while (true)
            {
                for (int i = 0; i < 2; i++)
                {
                    yield return rainbowBlackhole();
                    yield return 0.8f;
                    yield return fireShockwave();
                    yield return 0.8f;
                }
                yield return 2.0f;
            }
        }

        private IEnumerator Attack57Sequence()
        {
            startShootCharge();
            while (true)
            {
                for (int i = 0; i < 5; i++)
                {
                    yield return 0.25f;
                    shoot();
                    yield return 0.1f;
                    startShootCharge();
                }
                yield return 0.6f;
                yield return biggerBeam();
                yield return 0.6f;
                yield return bladeThrower();
                yield return 1.5f;
                startShootCharge();
            }
        }

        private IEnumerator Attack58Sequence()
        {
            while (true)
            {
                yield return starsMeteorite();
                yield return 0.5f;
                yield return bigBeamBall();
                yield return 0.5f;
                yield return fireShockwave();
                yield return 0.5f;
                yield return rainbowBlackhole();
                yield return 2.0f;
            }
        }

        private IEnumerator Attack59Sequence()
        {
            while (true)
            {
                yield return beam();
                yield return 0.3f;
                yield return bladeThrower();
                yield return 0.3f;
                yield return biggerBeam();
                yield return 0.5f;
                yield return fireShockwave();
                yield return 1.5f;
            }
        }

        private IEnumerator Attack60Sequence()
        {
            startShootCharge();
            while (true)
            {
                // Ultimate chaos pattern
                for (int i = 0; i < 10; i++)
                {
                    var entity = level.Tracker.GetEntity<global::Celeste.Player>();
                    if (entity != null)
                    {
                        shootAt(entity.Center);
                    }
                    yield return 0.15f;
                    startShootCharge();
                    yield return 0.15f;
                }
                yield return 0.5f;
                yield return bigBeamBall();
                yield return 0.5f;
                yield return bladeThrower();
                yield return 0.5f;
                yield return rainbowBlackhole();
                yield return 0.5f;
                yield return fireShockwave();
                yield return 0.5f;
                yield return starsMeteorite();
                yield return 0.8f;
                yield return biggerBeam();
                yield return 2.5f;
                startShootCharge();
            }
        }

        private IEnumerator Attack61Sequence()
        {
            while (true)
            {
                // Hyper Goner ultimate attack
                yield return hyperGoner();
                yield return 2.0f;
                
                // Follow up with chaos attacks
                startShootCharge();
                for (int i = 0; i < 3; i++)
                {
                    yield return 0.3f;
                    shoot();
                    yield return 0.2f;
                    startShootCharge();
                }
                yield return 1.5f;
            }
        }

        private void shootAt(Vector2 target)
        {
            var entity = level?.Tracker.GetEntity<global::Celeste.Player>();
            if (entity == null || Sprite == null) return;

            currentAttackPhase = AttackPhase.Executing;
            var projectile = new AsrielGodBossShot().Init(this, entity, 0f);
            Scene.Add(projectile);
            Audio.Play(SFX_GUNSHOT, Position);

            Add(new Coroutine(SetRecoveryPhase()));
        }

        private void shoot(float angleOffset = 0.0f)
        {
            var entity = level?.Tracker.GetEntity<global::Celeste.Player>();
            if (entity == null || Sprite == null) return;

            currentAttackPhase = AttackPhase.Executing;
            var projectile = new AsrielGodBossShot().Init(this, entity, angleOffset);
            Scene.Add(projectile);
            Audio.Play(SFX_BIG_BULLET_FIRE, Position);

            Add(new Coroutine(SetRecoveryPhase()));
        }

        private IEnumerator SetRecoveryPhase()
        {
            yield return 0.2f;
            currentAttackPhase = AttackPhase.Recovery;
        }

        private IEnumerator beam()
        {
            var asrielboss = this;
            currentAttackPhase = AttackPhase.Charging;
            asrielboss.laserSfx.Play("event:/char/badeline/boss_laser_charge");
            asrielboss.Sprite.Play("beamStart", true);
            yield return 0.1f;
            var entity = asrielboss.level.Tracker.GetEntity<global::Celeste.Player>();
            if (entity != null)
            {
                currentAttackPhase = AttackPhase.Executing;
                asrielboss.level.Add(new AsrielGodBossBeam().Init(asrielboss, entity));
            }

            yield return 0.9f;
            asrielboss.Sprite.Play("beam", true);
            yield return 0.5f;
            asrielboss.laserSfx.Stop();
            Audio.Play(SFX_BIGGER_GUN_FIRE, asrielboss.Position);
            asrielboss.Sprite.Play("idle");
            currentAttackPhase = AttackPhase.Recovery;
        }

        private IEnumerator biggerBeam()
        {
            var asrielboss = this;
            currentAttackPhase = AttackPhase.Charging;
            asrielboss.laserSfx.Play("event:/char/badeline/boss_laser_charge");
            asrielboss.Sprite.Play("beamStart", true);
            yield return 0.2f;
            var entity = asrielboss.level.Tracker.GetEntity<global::Celeste.Player>();
            if (entity != null)
            {
                currentAttackPhase = AttackPhase.Executing;
                asrielboss.level.Add(new AsrielGodBossBiggerBeam().Init(asrielboss, entity));
            }

            yield return 1.4f;
            asrielboss.Sprite.Play("beam", true);
            yield return 0.6f;
            asrielboss.laserSfx.Stop();
            Audio.Play(SFX_BIGGER_GUN_MECHANIZED, asrielboss.Position);
            asrielboss.Sprite.Play("idle");
            currentAttackPhase = AttackPhase.Recovery;
        }

        private IEnumerator bigBeamBall()
        {
            var asrielboss = this;
            currentAttackPhase = AttackPhase.Charging;
            asrielboss.laserSfx.Play("event:/char/badeline/boss_laser_charge");
            asrielboss.Sprite.Play("beamStart", true);
            yield return 0.3f;
            var entity = asrielboss.level.Tracker.GetEntity<global::Celeste.Player>();
            if (entity != null)
            {
                currentAttackPhase = AttackPhase.Executing;
                asrielboss.level.Add(new AsrielGodBossBigBeamBall().Init(asrielboss, entity));
            }

            yield return 1.0f;
            asrielboss.laserSfx.Stop();
            Audio.Play(SFX_CINEMATIC_CUT, asrielboss.Position);
            asrielboss.Sprite.Play("idle");
            currentAttackPhase = AttackPhase.Recovery;
        }

        private IEnumerator rainbowBlackhole()
        {
            var asrielboss = this;
            currentAttackPhase = AttackPhase.Charging;
            Audio.Play(SFX_LIGHTNING_HIT, asrielboss.Position);
            yield return 0.5f;
            var entity = asrielboss.level.Tracker.GetEntity<global::Celeste.Player>();
            if (entity != null)
            {
                currentAttackPhase = AttackPhase.Executing;
                asrielboss.level.Add(new AsrielGodBossRainbowBlackhole(entity.Position));
            }

            yield return 3.0f; // Duration of blackhole
            currentAttackPhase = AttackPhase.Recovery;
        }

        private IEnumerator bladeThrower()
        {
            var asrielboss = this;
            currentAttackPhase = AttackPhase.Charging;
            Audio.Play(SFX_GRAB, asrielboss.Position);
            yield return 0.3f;
            var entity = asrielboss.level.Tracker.GetEntity<global::Celeste.Player>();
            if (entity != null)
            {
                currentAttackPhase = AttackPhase.Executing;
                for (int i = 0; i < 5; i++)
                {
                    asrielboss.level.Add(new AsrielGodBossBlade().Init(asrielboss, entity, i * 72f)); // 5 blades in circle
                    yield return 0.1f;
                }
            }

            yield return 1.0f;
            currentAttackPhase = AttackPhase.Recovery;
        }

        private IEnumerator fireShockwave()
        {
            var asrielboss = this;
            currentAttackPhase = AttackPhase.Charging;
            Audio.Play(SFX_SPELLCAST_GLITCH, asrielboss.Position);
            yield return 0.4f;
            var entity = asrielboss.level.Tracker.GetEntity<global::Celeste.Player>();
            if (entity != null)
            {
                currentAttackPhase = AttackPhase.Executing;
                asrielboss.level.Add(new AsrielGodBossFireShockwave(entity.Position));
            }

            yield return 2.0f;
            currentAttackPhase = AttackPhase.Recovery;
        }

        private IEnumerator starsMeteorite()
        {
            var asrielboss = this;
            currentAttackPhase = AttackPhase.Charging;
            Audio.Play(SFX_STAR, asrielboss.Position);
            yield return 0.5f;
            var entity = asrielboss.level.Tracker.GetEntity<global::Celeste.Player>();
            if (entity != null)
            {
                currentAttackPhase = AttackPhase.Executing;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 spawnPos = new Vector2(Calc.Random.Range(level.Bounds.Left, level.Bounds.Right), level.Bounds.Top - 20);
                    asrielboss.level.Add(new AsrielGodBossMeteorite(spawnPos, entity.Position));
                    yield return 0.2f;
                }
            }

            yield return 2.0f;
            currentAttackPhase = AttackPhase.Recovery;
        }

        private IEnumerator StarDeathBlackhole()
        {
            var asrielboss = this;
            currentAttackPhase = AttackPhase.Charging;
            Audio.Play("event:/Ingeste/final_content/char/els/Els_StarDeath", asrielboss.Position);
            yield return 0.5f;
            var entity = asrielboss.level.Tracker.GetEntity<global::Celeste.Player>();
            if (entity != null)
            {
                currentAttackPhase = AttackPhase.Executing;
                for (int i = 0; i < 8; i++)
                {
                    Vector2 spawnPos = new Vector2(Calc.Random.Range(level.Bounds.Left, level.Bounds.Right), level.Bounds.Top - 20);
                    asrielboss.level.Add(new Supernova_StarDeath_Blackhole(spawnPos, entity.Position));
                    yield return 0.2f;
                }
            }

            yield return 2.0f;
            currentAttackPhase = AttackPhase.Recovery;
        }

        private IEnumerator hyperGoner()
        {
            var asrielboss = this;
            currentAttackPhase = AttackPhase.Charging;
            
            // Play the hypergoner sprite animation
            asrielboss.Sprite.Play("hypergoner", true);
            Audio.Play(SFX_HYPERGONER_CHARGE, asrielboss.Position);
            yield return 0.5f;
            
            // Start the inhale animation
            asrielboss.Sprite.Play("hypergoner_inhale", true);
            yield return 0.8f;
            
            var entity = asrielboss.level.Tracker.GetEntity<global::Celeste.Player>();
            if (entity != null)
            {
                currentAttackPhase = AttackPhase.Executing;
                
                // Create the vortex effect
                const float PULL_STRENGTH = 150f;
                const float SAFE_DISTANCE = 40f;
                asrielboss.level.Add(new HyperGonerVortex(asrielboss.Position, PULL_STRENGTH, SAFE_DISTANCE));
                
                // Play laughing animation
                asrielboss.Sprite.Play("hypergoner_laughing", true);
            }

            // Duration of the vortex effect
            yield return 4.0f;
            
            // Return to idle
            asrielboss.Sprite.Play("idle", true);
            currentAttackPhase = AttackPhase.Recovery;
            
            // Check if we should trigger the phase transition cutscene
            // This happens after the HyperGoner attack when player survives
            if (!hyperGonerPhaseTransitionTriggered && ShouldTriggerAngelPhaseTransition())
            {
                hyperGonerPhaseTransitionTriggered = true;
                asrielboss.level.Add(new CS20_BossEnd());
            }
        }
        
        // Flag to track if the HyperGoner phase transition has been triggered
        private bool hyperGonerPhaseTransitionTriggered = false;
        
        /// <summary>
        /// Determines if the Angel of Death phase transition should be triggered.
        /// Override this logic based on your specific conditions (health, room ID, etc.)
        /// </summary>
        private bool ShouldTriggerAngelPhaseTransition()
        {
            // Check if the flag hasn't been set already
            if (level.Session.GetFlag("asriel_boss_end_hypergoner"))
                return false;
            
            // Check if we're in a room where the phase transition should happen
            string currentRoomId = level.Session.Level;
            string[] transitionRoomIds = new string[]
            {
                "azzyboss-final",       // Final confrontation room
                "azzyboss-hypergoner",  // HyperGoner specific room
                // Add more room IDs as needed
            };
            
            foreach (string id in transitionRoomIds)
            {
                if (currentRoomId.Equals(id, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            
            // Alternatively, trigger based on health threshold
            // Uncomment the following to enable health-based transition:
            // if (currentHealth <= maxHealth * 0.1f) // When health drops below 10%
            //     return true;
            
            return false;
        }

        private List<AttackStep> parseCustomAttackSequence(string seq)
        {
            List<AttackStep> list = new List<AttackStep>();

            try
            {
                char[] splitters = new[] { ',', ';', '\n' };
                foreach (string raw in seq.Split(splitters, StringSplitOptions.RemoveEmptyEntries))
                {
                    string token = raw.Trim();
                    if (token.Length == 0) continue;

                    try
                    {
                        string[] parts = token.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 0) continue;

                        string action = parts[0].ToLowerInvariant();
                        float arg = 0f;
                        float delay = 0.3f;

                        if (parts.Length == 2)
                        {
                            if (action == nameof(shoot))
                            {
                                if (!float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out delay))
                                    delay = 0.3f;
                            }
                            else
                            {
                                float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out delay);
                            }
                        }
                        else if (parts.Length >= 3)
                        {
                            float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out arg);
                            float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out delay);
                        }

                        list.Add(new AttackStep(action, Math.Max(0f, delay), arg));
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(LogLevel.Error, nameof(DesoloZantas), $"Error parsing attack step: {token} - {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, nameof(DesoloZantas), $"Error parsing custom attack sequence: {ex.Message}");
                return new List<AttackStep>();
            }

            return list;
        }

        public override void Removed(Scene scene)
        {
            stopAttacking();
            if (bossBg != null && patternIndex == 0)
            {
                bossBg.Alpha = 1f;
            }
            base.Removed(scene);
        }

        private void takeDamage(int damage)
        {
            currentHealth = Math.Max(0, currentHealth - damage);

            if (currentHealth > 0)
            {
                SetState(BossState.Hurt);
                Add(new Coroutine(hurtStateTimer()));
            }
            else
            {
                SetState(BossState.Defeated);
                Add(new Coroutine(defeatSequence()));
            }
        }

        private void playHitEffect()
        {
            Vector2 hitPos = Center + new Vector2(0, -Height / 2);

            for (int i = 0; i < 10; i++)
            {
                Vector2 particleVel = Calc.AngleToVector((float)(Calc.Random.NextFloat() * Math.PI * 2f),
                                 Calc.Random.Range(50f, 100f));
                Scene.Add(new HitParticle(hitPos, particleVel, Color.Red));
            }

            flashTimer = 0.2f;
        }

        private void createHitParticles(Vector2 position)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = Calc.AngleToVector((float)(Calc.Random.NextFloat() * Math.PI * 2f),
                              Calc.Random.Range(30f, 60f));
                Scene.Add(new HitParticle(position, velocity, Color.Yellow));
            }
        }

        private void addKnockback(Vector2 knockback)
        {
            knockbackVelocity = knockback;
            knockbackTimer = 0.3f;
        }

        private IEnumerator playerInvulnerabilityTimer()
        {
            float timer = 1.0f;
            while (timer > 0f)
            {
                timer -= Engine.DeltaTime;
                yield return null;
            }
            isPlayerInvulnerable = false;
        }

        private IEnumerator hurtStateTimer()
        {
            float hurtTime = 0.5f;
            while (hurtTime > 0f && currentState == BossState.Hurt)
            {
                hurtTime -= Engine.DeltaTime;
                yield return null;
            }

            if (currentState == BossState.Hurt)
            {
                SetState(BossState.Idle);
            }
        }

        private IEnumerator defeatSequence()
        {
            yield return 2.0f;
            // Trigger defeat cutscene or dialog
            // Implementation depends on cutscene system
            yield return null;
        }

        // Dialog sequence triggers for Chapter 20 Asriel boss fight
        public void TriggerDialog(string dialogKey)
        {
            // Don't start a new dialog if one is already active
            if (isDialogActive) return;
            
            isDialogActive = true;
            Add(new Coroutine(PlayDialogSequence(dialogKey)));
        }

        private IEnumerator PlayDialogSequence(string dialogKey)
        {
            switch (dialogKey)
            {
                case "CH20_ASRIEL_REVEAL_IDENTITY":
                    yield return PlayDialog_RevealIdentity();
                    break;
                case "CH20_HEART_REFUSAL_PHASE4":
                    yield return PlayDialog_HeartRefusalPhase4();
                    break;
                case "CH20_MADELINE_BADELINE_RETURN":
                    yield return PlayDialog_MadelineBadelineReturn();
                    break;
                case "CH20_ASRIEL_ZERO_TRANSFORMATION":
                    yield return PlayDialog_AsrielZeroTransformation();
                    break;
                case "CH20_ASRIEL_HEART_LOST_SOUL":
                    yield return PlayDialog_AsrielHeartLostSoul();
                    break;
                case "CH20_ASRIEL_REMEMBER_A":
                    yield return PlayDialog_AsrielRememberA();
                    break;
                case "CH20_ASRIEL_REMEMBER_B":
                    yield return PlayDialog_AsrielRememberB();
                    break;
                case "CH20_ASRIEL_REMEMBER_C":
                    yield return PlayDialog_AsrielRememberC();
                    break;
                case "CH20_ASRIEL_REMEMBER_D":
                    yield return PlayDialog_AsrielRememberD();
                    break;
                case "CH20_ASRIEL_REMEMBER_E":
                    yield return PlayDialog_AsrielRememberE();
                    break;
                case "CH20_ASRIEL_REMEMBER_FINAL":
                    yield return PlayDialog_AsrielRememberFinal();
                    break;
                case "CH20_ASRIEL_REMEMBER_F":
                    yield return PlayDialog_AsrielRememberF();
                    break;
                case "CH20_ASRIEL_BOSS_END":
                    yield return PlayDialog_AsrielBossEnd();
                    break;
            }
            
            // Mark dialog as finished
            isDialogActive = false;
        }

        private IEnumerator PlayDialog_RevealIdentity()
        {
            // CH20_ASRIEL_REVEAL_IDENTITY dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_REVEAL_IDENTITY");
        }

        private IEnumerator PlayDialog_HeartRefusalPhase4()
        {
            // CH20_HEART_REFUSAL_PHASE4 dialog sequence
            yield return Textbox.Say("CH20_HEART_REFUSAL_PHASE4");
        }

        private IEnumerator PlayDialog_MadelineBadelineReturn()
        {
            // CH20_MADELINE_BADELINE_RETURN dialog sequence
            yield return Textbox.Say("CH20_MADELINE_BADELINE_RETURN");
        }

        private IEnumerator PlayDialog_AsrielZeroTransformation()
        {
            // CH20_ASRIEL_ZERO_TRANSFORMATION dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_ZERO_TRANSFORMATION");
        }

        private IEnumerator PlayDialog_AsrielHeartLostSoul()
        {
            // CH20_ASRIEL_HEART_LOST_SOUL dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_HEART_LOST_SOUL");
        }

        private IEnumerator PlayDialog_AsrielRememberA()
        {
            // CH20_ASRIEL_REMEMBER_A dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_REMEMBER_A");
        }

        private IEnumerator PlayDialog_AsrielRememberB()
        {
            // CH20_ASRIEL_REMEMBER_B dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_REMEMBER_B");
        }

        private IEnumerator PlayDialog_AsrielRememberC()
        {
            // CH20_ASRIEL_REMEMBER_C dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_REMEMBER_C");
        }

        private IEnumerator PlayDialog_AsrielRememberD()
        {
            // CH20_ASRIEL_REMEMBER_D dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_REMEMBER_D");
        }

        private IEnumerator PlayDialog_AsrielRememberE()
        {
            // CH20_ASRIEL_REMEMBER_E dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_REMEMBER_E");
        }

        private IEnumerator PlayDialog_AsrielRememberFinal()
        {
            // CH20_ASRIEL_REMEMBER_FINAL dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_REMEMBER_FINAL");
        }

        private IEnumerator PlayDialog_AsrielRememberF()
        {
            // CH20_ASRIEL_REMEMBER_F dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_REMEMBER_F");
        }

        private IEnumerator PlayDialog_AsrielBossEnd()
        {
            // CH20_ASRIEL_BOSS_END dialog sequence
            yield return Textbox.Say("CH20_ASRIEL_BOSS_END");
        }

        // Phase-based dialog triggers
        private void CheckDialogTriggers()
        {
            // Check health thresholds or phase transitions to trigger dialog
            // Example: When entering God of Hyperdeath form
            if (currentHealth <= maxHealth * 0.75f && !dialogTriggered_Phase1)
            {
                dialogTriggered_Phase1 = true;
                TriggerDialog("CH20_ASRIEL_REVEAL_IDENTITY");
            }
            
            if (currentHealth <= maxHealth * 0.50f && !dialogTriggered_Phase2)
            {
                dialogTriggered_Phase2 = true;
                TriggerDialog("CH20_HEART_REFUSAL_PHASE4");
            }

            if (currentHealth <= maxHealth * 0.25f && !dialogTriggered_Phase3)
            {
                dialogTriggered_Phase3 = true;
                TriggerDialog("CH20_ASRIEL_ZERO_TRANSFORMATION");
            }
        }

        // Dialog trigger flags
        private bool dialogTriggered_Phase1 = false;
        private bool dialogTriggered_Phase2 = false;
        private bool dialogTriggered_Phase3 = false;
        private bool isDialogActive = false;
        private int maxHealth = 1000; // Set appropriately
    }

    internal class Supernova_StarDeath_Blackhole : Entity
    {
        private Vector2 spawnPos;

        public Supernova_StarDeath_Blackhole(Vector2 spawnPos, Vector2 position)
        {
            this.spawnPos = spawnPos;
            Position = position;
        }
    }

    // Placeholder classes for new attacks - need to be implemented separately
    public class AsrielGodBossShot : Entity { public AsrielGodBossShot Init(AsrielGodBoss boss, global::Celeste.Player target, float angleOffset) => this; }
    public class AsrielGodBossBeam : Entity { public AsrielGodBossBeam Init(AsrielGodBoss boss, global::Celeste.Player target) => this; }
    public class AsrielGodBossBiggerBeam : Entity { public AsrielGodBossBiggerBeam Init(AsrielGodBoss boss, global::Celeste.Player target) => this; }
    public class AsrielGodBossBigBeamBall : Entity { public AsrielGodBossBigBeamBall Init(AsrielGodBoss boss, global::Celeste.Player target) => this; }
    public class AsrielGodBossRainbowBlackhole : Entity { public AsrielGodBossRainbowBlackhole(Vector2 position) : base(position) {} }
    public class AsrielGodBossBlade : Entity { public AsrielGodBossBlade Init(AsrielGodBoss boss, global::Celeste.Player target, float angle) => this; }
    public class AsrielGodBossFireShockwave : Entity { public AsrielGodBossFireShockwave(Vector2 position) : base(position) {} }
    public class AsrielGodBossMeteorite : Entity { public AsrielGodBossMeteorite(Vector2 position, Vector2 target) : base(position) {} }
}




