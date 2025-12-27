using FMOD.Studio;

namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// A music cartridge collectible that unlocks special music tracks.
    /// Compatible with the cassette player system for applying audio effects.
    /// </summary>
    [CustomEntity("DesoloZantas/MusicCartridge")]
    [Tracked]
    public class MusicCartridge : Entity
    {
        // Visual components
        private Sprite sprite;
        private Wiggler scaleWiggler;
        private BloomPoint bloom;
        private VertexLight light;
        private SineWave bobSine;
        private ParticleType shineParticles;
        
        // Cartridge data
        private string cartridgeId;
        private string musicEvent;
        private string unlockFlag;
        private string cartridgeName;
        private string labelText;
        private Color cartridgeColor;
        private bool playOnCollect;
        private bool isPersistent;
        private int remixIndex;
        private EventInstance previewEventInstance;
        
        // State
        private bool collected;
        private float respawnTimer;
        private const float RespawnDelay = 3f;
        
        // Collection animation
        private global::Celeste.Player collectingPlayer;
        private SoundSource sfx;

        public MusicCartridge(EntityData data, Vector2 offset) : base(data.Position + offset)
        {
            // Parse data
            cartridgeId = data.Attr("cartridgeId", "music_default");
            musicEvent = data.Attr("musicEvent", "");
            unlockFlag = data.Attr("unlockFlag", $"cartridge_{cartridgeId}");
            cartridgeName = data.Attr("name", "Music Cartridge");
            labelText = data.Attr("label", "???");
            cartridgeColor = data.HexColor("color", Color.DeepPink);
            playOnCollect = data.Bool("playOnCollect", true);
            isPersistent = data.Bool("persistent", true);
            remixIndex = data.Int("remixIndex", 0); // Single music preview option
            
            Depth = -100;
            Collider = new Hitbox(20f, 20f, -10f, -10f);

            // Setup sprite
            Add(sprite = new Sprite(GFX.Game, "collectibles/maggy/tape/"));
            sprite.AddLoop("idle", "idle", 0.1f);
            sprite.AddLoop("pulse", "pulse", 0.15f);
            sprite.Play("idle");
            sprite.CenterOrigin();
            sprite.Color = cartridgeColor;

            // Wiggle effect
            Add(scaleWiggler = Wiggler.Create(0.4f, 4f, v => sprite.Scale = Vector2.One * (1f + v * 0.3f)));

            // Float animation
            Add(bobSine = new SineWave(0.6f, 0f));
            bobSine.OnUpdate = f => sprite.Y = f * 3f;

            // Lighting
            Add(bloom = new BloomPoint(0.75f, 12f));
            Add(light = new VertexLight(cartridgeColor, 1f, 24, 48));

            // Particles
            shineParticles = new ParticleType
            {
                Source = GFX.Game["particles/star"],
                Color = cartridgeColor,
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Choose,
                FadeMode = ParticleType.FadeModes.Late,
                Size = 1f,
                SizeRange = 0.5f,
                SpeedMin = 8f,
                SpeedMax = 16f,
                LifeMin = 0.4f,
                LifeMax = 0.8f,
                RotationMode = ParticleType.RotationModes.Random,
                SpeedMultiplier = 0.2f
            };

            // Sound
            Add(sfx = new SoundSource());
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            
            Level level = scene as Level;
            if (level != null && isPersistent)
            {
                // Check if already collected this session
                if (level.Session.GetFlag(unlockFlag))
                {
                    RemoveSelf();
                    return;
                }
            }
        }

        public override void Update()
        {
            base.Update();

            if (collected)
            {
                if (!isPersistent)
                {
                    respawnTimer += Engine.DeltaTime;
                    if (respawnTimer >= RespawnDelay)
                    {
                        Respawn();
                    }
                }
                return;
            }

            // Sparkle particles
            if (Scene.OnInterval(0.15f))
            {
                Level level = Scene as Level;
                float angle = Calc.Random.NextFloat((float)Math.PI * 2f);
                Vector2 offset = Calc.AngleToVector(angle, 12f);
                level?.ParticlesFG.Emit(shineParticles, Position + offset, angle + (float)Math.PI);
            }

            // Pulse animation every few seconds
            if (Scene.OnInterval(3f))
            {
                sprite.Play("pulse");
                scaleWiggler.Start();
            }

            // Check for player collision
            global::Celeste.Player player = CollideFirst<global::Celeste.Player>();
            if (player != null && !collected)
            {
                Collect(player);
            }
        }

        private void Collect(global::Celeste.Player player)
        {
            if (collected)
                return;

            collected = true;
            collectingPlayer = player;
            
            Level level = Scene as Level;
            
            // Set unlock flag
            if (level != null)
            {
                level.Session.SetFlag(unlockFlag, true);
                
                // Also set a global save flag if persistent
                if (isPersistent && IngesteModule.SaveData != null)
                {
                    // Store in save data (you may need to add this to your save data class)
                    level.Session.SetFlag($"global_{unlockFlag}", true);
                }
            }

            // Play collection sound
            sfx.Play("event:/game/general/key_get");
            previewEventInstance = global::Celeste.Audio.Play("event:/Ingeste/game/general/cartridge_preview", Position);
            global::Celeste.Audio.SetParameter(previewEventInstance, "remix", remixIndex);

            Add(new Coroutine(CollectRoutine(player)));
        }

        private IEnumerator CollectRoutine(global::Celeste.Player player)
        {
            Level level = Scene as Level;
            
            // Particle burst
            for (int i = 0; i < 30; i++)
            {
                float angle = Calc.Random.NextFloat((float)Math.PI * 2f);
                level?.ParticlesFG.Emit(shineParticles, Position, angle);
            }

            // Freeze player briefly
            player.StateMachine.State = global::Celeste.Player.StDummy;
            yield return 0.1f;

            // Animate to player
            Vector2 startPos = Position;
            Vector2 targetPos = player.Center;
            
            for (float t = 0f; t < 1f; t += Engine.DeltaTime * 3f)
            {
                float easedT = Ease.CubeIn(t);
                Position = Vector2.Lerp(startPos, targetPos, easedT);
                sprite.Scale = Vector2.One * (1f + (1f - easedT) * 0.5f);
                sprite.Rotation += Engine.DeltaTime * 8f;
                
                // More particles during collection
                if (Calc.Random.Next(3) == 0)
                {
                    float angle = Calc.Random.NextFloat((float)Math.PI * 2f);
                    level?.ParticlesFG.Emit(shineParticles, Position, angle);
                }
                
                yield return null;
            }

            // Flash effect
            sprite.Visible = false;
            yield return 0.05f;
            sprite.Visible = true;
            yield return 0.05f;
            sprite.Visible = false;

            // Show unlock message
            if (level != null)
            {
                yield return 0.2f;
                
                // You can add a custom UI popup here to show the cartridge info
                // For now, just log it
                IngesteLogger.Info($"Collected Music Cartridge: {cartridgeName} ({labelText})");
                
                // Play the music if enabled
                if (playOnCollect && !string.IsNullOrEmpty(musicEvent))
                {
                    yield return 0.5f;
                    global::Celeste.Audio.SetMusic(musicEvent);
                }
            }

            // Restore player state
            if (player.StateMachine.State == global::Celeste.Player.StDummy)
            {
                player.StateMachine.State = global::Celeste.Player.StNormal;
            }

            // Remove or hide
            if (isPersistent)
            {
                RemoveSelf();
            }
            else
            {
                Visible = false;
                Collidable = false;
                respawnTimer = 0f;
            }
        }

        private void Respawn()
        {
            collected = false;
            Visible = true;
            Collidable = true;
            sprite.Visible = true;
            sprite.Scale = Vector2.One;
            sprite.Rotation = 0f;
            sprite.Play("idle");
            
            // Spawn particles
            Level level = Scene as Level;
            for (int i = 0; i < 15; i++)
            {
                float angle = Calc.Random.NextFloat((float)Math.PI * 2f);
                level?.ParticlesFG.Emit(shineParticles, Position, angle);
            }
            
            global::Celeste.Audio.Play("event:/Ingeste/game/general/cartridge_unlocked", Position);
        }

        public override void SceneEnd(Scene scene)
        {
            base.SceneEnd(scene);
            global::Celeste.Audio.Stop(previewEventInstance);
        }

        public override void Removed(Scene scene)
        {
            base.Removed(scene);
            global::Celeste.Audio.Stop(previewEventInstance);
        }

        public override void Render()
        {
            if (!Visible)
                return;

            // Draw glow
            sprite.DrawOutline(cartridgeColor, 1);
            
            base.Render();

            // Draw label text on cartridge
            if (!string.IsNullOrEmpty(labelText) && !collected)
            {
                Vector2 labelPos = Position + new Vector2(0f, -4f);
                ActiveFont.DrawOutline(
                    labelText,
                    labelPos,
                    new Vector2(0.5f, 0.5f),
                    Vector2.One * 0.4f,
                    Color.White,
                    2f,
                    Color.Black
                );
            }
        }
    }
}




