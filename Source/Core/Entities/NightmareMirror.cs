#nullable disable
using DesoloZantas.Core.Core.Cutscenes;

namespace DesoloZantas.Core.Core.Entities
{
    [CustomEntity(new string[] { "Ingeste/CharaMirror" })]
    [Tracked(false)]
    public class NightmareMirror : Entity
    {
        public static ParticleType PShatter;
        private Monocle.Image frame;
        private MTexture glassbg = GFX.Game["objects/mirror/glassbg"];
        private MTexture glassfg = GFX.Game["objects/mirror/glassfg"];
        private Sprite breakingGlass;
        private Hitbox hitbox;
        private VirtualRenderTarget mirror;
        private float shineAlpha = 0.5f;
        private float shineOffset;
        private Entity reflection;
        private global::Celeste.PlayerSprite reflectionSprite;

        private float reflectionAlpha = 0.7f;
        private bool autoUpdateReflection = true;
        private CharaDummy chara;
        private bool smashed;
        private bool smashEnded;
        private bool updateShine = true;
        private Coroutine smashCoroutine;
        private SoundSource sfx;
        private SoundSource sfxSting;
        private bool betacube;

        public bool Betacube => betacube;
        public CharaDummy Chara => chara;

        static NightmareMirror()
        {
            // Initialize the particle type for shattering effect
            // Try to use an appropriate particle type, fallback to Dust if unavailable
            PShatter = ParticleTypes.Dust;
        }

        public NightmareMirror(EntityData data, Vector2 offset)
            : base(data.Position + offset)
        {
            this.betacube = data.Bool("betacube", false);
            this.Depth = 9500;
            this.Add((Component)(this.breakingGlass = GFX.SpriteBank.Create("glass")));
            this.breakingGlass.Play("idle");
            this.Add((Component)new BeforeRenderHook(new Action(this.beforeRender)));

            this.hitbox = new Hitbox(this.glassbg.Width, this.glassbg.Height, -this.glassbg.Width / 2f, -this.glassbg.Height);
            this.Collider = this.hitbox;

            // Add a frame image if you want a visible border/frame (optional)
            this.frame = new Monocle.Image(GFX.Game["objects/mirror/frame"]);
            this.frame.Origin = new Vector2(this.glassbg.Width / 2f, this.glassbg.Height);
            // Fix: Set the frame's position so it matches the mirror's position and aligns visually
            this.frame.Position = Vector2.Zero; // Frame will be drawn at the entity's position
            this.Add(this.frame);

            foreach (MTexture atlasSubtexture in GFX.Game.GetAtlasSubtextures("objects/mirror/mirrormask"))
            {
                MTexture shard = atlasSubtexture;
                MirrorSurface surface = new MirrorSurface();
                surface.OnRender = (Action)(() => shard.DrawJustified(this.Position, new Vector2(0.5f, 1f), surface.ReflectionColor * (this.smashEnded ? 1f : 0.0f)));
                surface.ReflectionOffset = new Vector2((float)(9 + Calc.Random.Range(-4, 4)), (float)(4 + Calc.Random.Range(-2, 2)));
                this.Add((Component)surface);
            }
        }

        public void Load()
        {
            // Allocate the mirror render target if not already created
            if (this.mirror == null)
            {
                this.mirror = VirtualContent.CreateRenderTarget("nightmare-mirror", this.glassbg.Width, this.glassbg.Height);
            }
            // Optionally, reset or initialize other state here if needed
        }
        public override void Added(Scene scene)
        {
            base.Added(scene);

            // Call Load when entity is added to scene
            Load();

            this.smashed = this.SceneAs<Level>().Session.Inventory.DreamDash;
            if (this.smashed)
            {
                this.breakingGlass.Play("broken");
                this.smashEnded = true;
            }
            else
            {
                this.reflection = new Entity();
                this.reflectionSprite = new global::Celeste.PlayerSprite(global::Celeste.PlayerSpriteMode.Madeline);
                this.reflection.Add((Component)this.reflectionSprite);

                this.reflectionSprite.OnFrameChange = (Action<string>)(anim =>
                {
                    if (this.smashed || !this.CollideCheck<global::Celeste.Player>())
                        return;
                    int currentAnimationFrame = this.reflectionSprite.CurrentAnimationFrame;
                    if ((anim != "walk" || (currentAnimationFrame != 0 && currentAnimationFrame != 6)) &&
                        (anim != "runSlow" || (currentAnimationFrame != 0 && currentAnimationFrame != 6)) &&
                        (anim != "runFast" || (currentAnimationFrame != 0 && currentAnimationFrame != 6)))
                        return;
                    Audio.Play("event:/char/badeline/footstep", this.Center);
                });
                this.Add((Component)(this.smashCoroutine = new Coroutine(this.interactRoutine())));
            }
        }

        public override void Update()
        {
            base.Update();
            if (this.reflection == null)
                return;
            this.reflection.Update();
            // Fixed: Removed references to non-existent Hair properties
        }

        private void beforeRender()
        {
            if (this.smashed)
                return;
            Level scene = this.Scene as Level;
            global::Celeste.Player entity = this.Scene.Tracker.GetEntity<global::Celeste.Player>();
            if (entity == null)
                return;
            if (this.autoUpdateReflection && this.reflection != null)
            {
                this.reflection.Position = new Vector2(this.X - entity.X, entity.Y - this.Y) + this.breakingGlass.Origin;
                this.reflectionSprite.Scale.X = (float)-(int)entity.Facing * Math.Abs(entity.Sprite.Scale.X);
                this.reflectionSprite.Scale.Y = entity.Sprite.Scale.Y;
                if (this.reflectionSprite.CurrentAnimationID != entity.Sprite.CurrentAnimationID && entity.Sprite.CurrentAnimationID != null && this.reflectionSprite.Has(entity.Sprite.CurrentAnimationID))
                    this.reflectionSprite.Play(entity.Sprite.CurrentAnimationID);
            }
            if (this.mirror == null)
                this.mirror = VirtualContent.CreateRenderTarget("nightmare-mirror", this.glassbg.Width, this.glassbg.Height);
            Engine.Graphics.GraphicsDevice.SetRenderTarget((RenderTarget2D)this.mirror);
            Engine.Graphics.GraphicsDevice.Clear(Color.Transparent);
            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
            if (this.updateShine)
                this.shineOffset = (float)(this.glassfg.Height - (int)((double)scene.Camera.Y * 0.800000011920929 % (double)this.glassfg.Height));
            this.glassbg.Draw(Vector2.Zero);
            if (this.reflection != null)
                this.reflection.Render();
            this.glassfg.Draw(new Vector2(0.0f, this.shineOffset), Vector2.Zero, Color.White * this.shineAlpha);
            this.glassfg.Draw(new Vector2(0.0f, this.shineOffset - (float)this.glassfg.Height), Vector2.Zero, Color.White * this.shineAlpha);
            Draw.SpriteBatch.End();
        }

        private IEnumerator interactRoutine()
        {
            NightmareMirror mirror = this;
            global::Celeste.Player player = null;
            while (player == null)
            {
                player = mirror.Scene.Tracker.GetEntity<global::Celeste.Player>();
                yield return null;
            }

            // Null check for hitbox
            if (mirror.hitbox == null)
                yield break;

            while (!mirror.hitbox.Collide((Entity)player))
                yield return null;

            mirror.hitbox.Width += 32f;
            mirror.hitbox.Position.X -= 16f;
            Audio.SetMusic(null);

            while (mirror.hitbox.Collide((Entity)player))
                yield return null;

            // Activate nightmare blocks when cutscene starts
            mirror.ActivateNightmareBlocks();

            mirror.Scene.Add(new Cs02CharaMirror(player, mirror));
        }

        /// <summary>
        /// Activates all NightmareBlock entities in the scene
        /// </summary>
        private void ActivateNightmareBlocks()
        {
            var tracker = this.Scene?.Tracker;
            if (tracker == null) return;

            List<Entity> blocks = null;
            try
            {
                blocks = tracker.GetEntities<NightmareBlock>();
            }
            catch (KeyNotFoundException)
            {
                // NightmareBlock type not tracked, no blocks to activate
                return;
            }

            if (blocks != null)
            {
                foreach (var entity in blocks)
                {
                    var block = entity as NightmareBlock;
                    if (block != null && !block.IsActivated)
                    {
                        block.ActivateNoRoutine();
                    }
                }
            }
        }

        public IEnumerator BreakRoutine(int direction)
        {
            NightmareMirror nightmareMirror = this;
            nightmareMirror.autoUpdateReflection = false;
            nightmareMirror.reflectionSprite.Play("runFast");
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Short);
            while ((double)Math.Abs(nightmareMirror.reflection.X - nightmareMirror.breakingGlass.Width / 2f) > 3.0)
            {
                nightmareMirror.reflection.X += (float)(direction * 32) * Engine.DeltaTime;
                yield return null;
            }
            nightmareMirror.reflectionSprite.Play("idle");
            yield return 0.65f;
            nightmareMirror.Add((Component)(nightmareMirror.sfx = new SoundSource()));
            nightmareMirror.sfx.Play("event:/game/02_old_site/sequence_mirror");
            yield return 0.15f;
            nightmareMirror.Add((Component)(nightmareMirror.sfxSting = new SoundSource("event:/Ingeste/music/lvl2/dreamblock_sting_pt2")));
            Input.Rumble(RumbleStrength.Light, RumbleLength.FullSecond);
            nightmareMirror.updateShine = false;
            while ((double)nightmareMirror.shineOffset != 33.0 || (double)nightmareMirror.shineAlpha < 1.0)
            {
                nightmareMirror.shineOffset = Calc.Approach(nightmareMirror.shineOffset, 33f, Engine.DeltaTime * 120f);
                nightmareMirror.shineAlpha = Calc.Approach(nightmareMirror.shineAlpha, 1f, Engine.DeltaTime * 4f);
                yield return null;
            }
            nightmareMirror.smashed = true;
            nightmareMirror.breakingGlass.Play("break");
            yield return 0.6f;
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            (nightmareMirror.Scene as Level).Shake();
            for (float x = (float)(-(double)nightmareMirror.breakingGlass.Width / 2.0); (double)x < (double)nightmareMirror.breakingGlass.Width / 2.0; x += 8f)
            {
                for (float y = -nightmareMirror.breakingGlass.Height; (double)y < 0.0; y += 8f)
                {
                    if (Calc.Random.Chance(0.5f))
                        (nightmareMirror.Scene as Level).Particles.Emit(NightmareMirror.PShatter, 2, nightmareMirror.Position + new Vector2(x + 4f, y + 4f), new Vector2(8f, 8f), new Vector2(x, y).Angle());
                }
            }
            nightmareMirror.smashEnded = true;
            nightmareMirror.chara = new CharaDummy(nightmareMirror.reflection.Position + nightmareMirror.Position - nightmareMirror.breakingGlass.Origin);
            nightmareMirror.chara.Floatness = 0.0f;
            nightmareMirror.Scene.Add((Entity)nightmareMirror.chara);
            nightmareMirror.chara.Sprite.Play("idle");
            nightmareMirror.chara.Sprite.Scale = nightmareMirror.reflectionSprite.Scale;
            nightmareMirror.reflection = null;
            yield return 1.2f;
            float speed = (float)-direction * 32f;
            nightmareMirror.chara.Sprite.Scale.X = (float)-direction;
            nightmareMirror.chara.Sprite.Play("runFast");
            while ((double)Math.Abs(nightmareMirror.chara.X - nightmareMirror.X) < 60.0)
            {
                speed += (float)((double)Engine.DeltaTime * (double)-direction * 128.0);
                nightmareMirror.chara.X += speed * Engine.DeltaTime;
                yield return null;
            }
            nightmareMirror.chara.Sprite.Play("jumpFast");
            while ((double)Math.Abs(nightmareMirror.chara.X - nightmareMirror.X) < 128.0)
            {
                speed += (float)((double)Engine.DeltaTime * (double)-direction * 128.0);
                nightmareMirror.chara.X += speed * Engine.DeltaTime;
                nightmareMirror.chara.Y -= (float)((double)Math.Abs(speed) * (double)Engine.DeltaTime * 0.800000011920929);
                yield return null;
            }
            nightmareMirror.chara.RemoveSelf();
            nightmareMirror.chara = null;
            yield return 1.5f;
        }

        public void Broken(bool wasSkipped)
        {
            this.updateShine = false;
            this.smashed = true;
            this.smashEnded = true;
            this.breakingGlass.Play("broken");
            if (wasSkipped && this.chara != null)
                this.chara.RemoveSelf();
            if (wasSkipped && this.sfx != null)
                this.sfx.Stop();
            if (!wasSkipped || this.sfxSting == null)
                return;
            this.sfxSting.Stop();
        }

        public override void Render()
        {
            if (this.smashed)
            {
                this.breakingGlass.Render();
            }
            else
            {
                if (this.mirror != null && this.mirror.Target != null)
                {
                    Draw.SpriteBatch.Draw((Texture2D)this.mirror.Target, this.Position - this.breakingGlass.Origin, Color.White * this.reflectionAlpha);
                }
            }
            if (this.frame != null)
                // Fix: Use the correct overload of the Render method
                this.frame.Render();
        }

        public override void SceneEnd(Scene scene)
        {
            Unload();
            base.SceneEnd(scene);
        }

        public override void Removed(Scene scene)
        {
            Unload();
            base.Removed(scene);
        }

        private void dispose()
        {
            if (this.mirror != null)
                this.mirror.Dispose();
            this.mirror = null;
        }
        public void Unload()
        {
            // Dispose of the mirror render target if it exists
            if (this.mirror != null)
            {
                this.mirror.Dispose();
                this.mirror = null;
            }
            // Optionally, clean up other resources or state here if needed
        }

        public static Entity Load(Level level, LevelData levelData, Vector2 offset, EntityData entityData)
        {
            // You can read custom properties from entityData if needed
            return new NightmareMirror(entityData, offset);
        }
    }
}



