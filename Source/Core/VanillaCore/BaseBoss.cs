namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Base boss class ported from vanilla Celeste's FinalBoss.
    /// Designed as a template for custom mod bosses like CharaBoss, FloweyBoss, etc.
    /// </summary>
    [Tracked]
    public abstract class BaseBoss : Entity
    {
        public static ParticleType P_Burst;
        
        public const float CameraXPastMax = 140f;
        protected const float MoveSpeed = 600f;
        protected const float AvoidRadius = 12f;
        
        public Sprite Sprite;
        public PlayerSprite NormalSprite;
        protected PlayerHair normalHair;
        protected Vector2 avoidPos;
        public float CameraYPastMax;
        public bool Moving;
        public bool Sitting;
        protected int facing;
        protected Level level;
        protected Circle circle;
        protected Vector2[] nodes;
        protected int nodeIndex;
        protected int patternIndex;
        protected Coroutine attackCoroutine;
        protected Coroutine triggerBlocksCoroutine;
        protected List<Entity> fallingBlocks;
        protected List<Entity> movingBlocks;
        protected bool playerHasMoved;
        protected SineWave floatSine;
        protected bool dialog;
        protected bool startHit;
        protected VertexLight light;
        protected Wiggler scaleWiggler;
        protected SoundSource chargeSfx;
        protected SoundSource laserSfx;

        // DesoloZantas extensions
        public int CurrentPhase { get; protected set; } = 1;
        public int MaxPhases { get; protected set; } = 3;
        public int HitsRemaining { get; protected set; }
        public int HitsPerPhase { get; protected set; } = 3;
        public string BossId { get; protected set; }
        public bool IsInvulnerable { get; protected set; }

        public BaseBoss(
            Vector2 position,
            Vector2[] nodes,
            int patternIndex,
            float cameraYPastMax,
            bool dialog,
            bool startHit,
            bool cameraLockY)
            : base(position)
        {
            this.patternIndex = patternIndex;
            CameraYPastMax = cameraYPastMax;
            this.dialog = dialog;
            this.startHit = startHit;
            
            Add(light = new VertexLight(Color.White, 1f, 32, 64));
            Collider = circle = new Circle(14f, y: -6f);
            Add(new PlayerCollider(OnPlayer));
            
            // Setup nodes
            this.nodes = new Vector2[nodes.Length + 1];
            this.nodes[0] = Position;
            for (int i = 0; i < nodes.Length; i++)
                this.nodes[i + 1] = nodes[i];
            
            attackCoroutine = new Coroutine(false);
            Add(attackCoroutine);
            triggerBlocksCoroutine = new Coroutine(false);
            Add(triggerBlocksCoroutine);
            
            Add(new CameraLocker(
                cameraLockY ? Level.CameraLockModes.FinalBoss : Level.CameraLockModes.FinalBossNoY, 
                140f, 
                cameraYPastMax));
            Add(floatSine = new SineWave(0.6f));
            Add(scaleWiggler = Wiggler.Create(0.6f, 3f));
            Add(chargeSfx = new SoundSource());
            Add(laserSfx = new SoundSource());
            
            HitsRemaining = HitsPerPhase;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
            
            // Subclasses should override to create their custom sprites
            CreateBossSprite();
            
            light.Position = GetSpriteComponent().Position + new Vector2(0f, -10f);
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            fallingBlocks = Scene.Tracker.GetEntitiesCopy<FallingBlock>();
            fallingBlocks.Sort((a, b) => (int)(a.X - b.X));
            movingBlocks = Scene.Tracker.GetEntitiesCopy<FinalBossMovingBlock>();
            movingBlocks.Sort((a, b) => (int)(a.X - b.X));
        }

        /// <summary>
        /// Override to create boss-specific sprites
        /// </summary>
        protected abstract void CreateBossSprite();

        /// <summary>
        /// Override to define attack patterns
        /// </summary>
        protected abstract IEnumerator Attack();

        /// <summary>
        /// Override to handle phase transitions
        /// </summary>
        protected virtual void OnPhaseChange(int newPhase)
        {
            CurrentPhase = newPhase;
            HitsRemaining = HitsPerPhase;
        }

        /// <summary>
        /// Override for custom death behavior
        /// </summary>
        protected virtual void OnBossDefeated()
        {
            // Set completion flag
            level?.Session.SetFlag($"{BossId}_defeated", true);
        }

        protected GraphicsComponent GetSpriteComponent()
        {
            return Sprite ?? (GraphicsComponent)NormalSprite;
        }

        public Vector2 BeamOrigin => Center + GetSpriteComponent().Position + new Vector2(0f, -14f);
        public Vector2 ShotOrigin => Center + GetSpriteComponent().Position + new Vector2(6f * GetSpriteComponent().Scale.X, 2f);

        protected virtual void OnPlayer(global::Celeste.Player player)
        {
            if (IsInvulnerable || player == null)
                return;

            // Check if player is dashing
            if (player.DashAttacking)
            {
                OnHit(player);
            }
            else
            {
                // Player gets hit by boss
                player.Die((player.Position - Position).SafeNormalize());
            }
        }

        protected virtual void OnHit(global::Celeste.Player player)
        {
            HitsRemaining--;
            Audio.Play("event:/game/06_reflection/boss_spikes_burst", Position);
            scaleWiggler.Start();
            
            if (HitsRemaining <= 0)
            {
                if (CurrentPhase >= MaxPhases)
                {
                    OnBossDefeated();
                }
                else
                {
                    OnPhaseChange(CurrentPhase + 1);
                }
            }
        }

        public void StartAttacking()
        {
            attackCoroutine.Replace(Attack());
        }

        public void StopAttacking()
        {
            attackCoroutine.Cancel();
        }

        public override void Update()
        {
            base.Update();
            GraphicsComponent sprite = GetSpriteComponent();
            
            if (!Sitting)
            {
                global::Celeste.Player player = Scene.Tracker.GetEntity<global::Celeste.Player>();
                
                // Face toward player
                if (!Moving && player != null)
                {
                    if (facing == -1 && player.X > X + 20f)
                    {
                        facing = 1;
                        scaleWiggler.Start();
                    }
                    else if (facing == 1 && player.X < X - 20f)
                    {
                        facing = -1;
                        scaleWiggler.Start();
                    }
                }
                
                // Start attacking when player moves
                if (!playerHasMoved && player != null && player.Speed != Vector2.Zero)
                {
                    playerHasMoved = true;
                    if (patternIndex != 0)
                        StartAttacking();
                    TriggerMovingBlocks(0);
                }
                
                // Float animation
                if (!Moving)
                    sprite.Position = avoidPos + new Vector2(floatSine.Value * 3f, floatSine.ValueOverTwo * 4f);
                else
                    sprite.Position = Calc.Approach(sprite.Position, Vector2.Zero, 12f * Engine.DeltaTime);
                
                // Break dash blocks
                float radius = circle.Radius;
                circle.Radius = 6f;
                CollideFirst<DashBlock>()?.Break(Center, -Vector2.UnitY, false, false);
                circle.Radius = radius;
                
                // Bounds check
                if (!level.IsInBounds(Position, 24f))
                {
                    Active = Visible = Collidable = false;
                    return;
                }
                
                // Avoid player
                Vector2 target;
                if (!Moving && player != null)
                {
                    float length = Calc.ClampedMap((Center - player.Center).Length(), 32f, 88f, 12f, 0f);
                    target = length > 0f ? (Center - player.Center).SafeNormalize(length) : Vector2.Zero;
                }
                else
                {
                    target = Vector2.Zero;
                }
                avoidPos = Calc.Approach(avoidPos, target, 40f * Engine.DeltaTime);
            }
            
            // Update sprite scale for facing
            if (Sprite != null)
                Sprite.Scale.X = facing * (1f + scaleWiggler.Value * 0.2f);
        }

        protected void TriggerMovingBlocks(int startIndex)
        {
            triggerBlocksCoroutine.Replace(TriggerMovingBlocksRoutine(startIndex));
        }

        private IEnumerator TriggerMovingBlocksRoutine(int startIndex)
        {
            for (int i = startIndex; i < movingBlocks.Count; i++)
            {
                if (movingBlocks[i] is FinalBossMovingBlock block)
                {
                    block.StartMoving(0f);
                    yield return 0.3f;
                }
            }
        }

        protected IEnumerator MoveToNode(int index)
        {
            Moving = true;
            Vector2 from = Position;
            Vector2 to = nodes[index];
            
            for (float t = 0f; t < 1f; t += Engine.DeltaTime * MoveSpeed / Vector2.Distance(from, to))
            {
                Position = Vector2.Lerp(from, to, Ease.CubeInOut(t));
                yield return null;
            }
            
            Position = to;
            nodeIndex = index;
            Moving = false;
        }
    }
}
