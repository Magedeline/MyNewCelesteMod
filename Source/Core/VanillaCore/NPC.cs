namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Base NPC class ported from vanilla Celeste.
    /// Custom character flags are used instead of vanilla Theo flags.
    /// </summary>
    public class NPC : Entity
    {
        // Custom character flags for DesoloZantas mod
        public const string MetKirby = "MetKirby";
        public const string MetMagolor = "MetMagolor";
        public const string MetToriel = "MetToriel";
        public const string MetRalsei = "MetRalsei";
        public const string MetChara = "MetChara";
        public const string KirbyKnowsName = "KirbyKnowsName";
        public const string MagolorKnowsName = "MagolorKnowsName";
        
        // Vanilla flags preserved for compatibility
        public const string MetTheo = "MetTheo";
        public const string TheoKnowsName = "TheoKnowsName";
        
        public const float KirbyMaxSpeed = 60f;
        public const float MagolorMaxSpeed = 72f;
        public const float TorielMaxSpeed = 40f;
        public const float TheoMaxSpeed = 48f;
        
        public Sprite Sprite;
        public TalkComponent Talker;
        public VertexLight Light;
        public Level Level;
        public SoundSource PhoneTapSfx;
        public float Maxspeed = 80f;
        public string MoveAnim = "";
        public string IdleAnim = "";
        public bool MoveY = true;
        public bool UpdateLight = true;
        private List<Entity> temp = new List<Entity>();

        public Session Session => Level.Session;

        public NPC(Vector2 position)
        {
            Position = position;
            Depth = 1000;
            Collider = new Hitbox(8f, 8f, -4f, -8f);
            Add(new MirrorReflection());
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level = scene as Level;
        }

        public override void Update()
        {
            base.Update();
            if (UpdateLight && Light != null)
            {
                Rectangle bounds = Level.Bounds;
                Light.Alpha = Calc.Approach(Light.Alpha, 
                    X <= bounds.Left - 16 || Y <= bounds.Top - 16 || 
                    X >= bounds.Right + 16 || Y >= bounds.Bottom + 16 || 
                    Level.Transitioning ? 0f : 1f, 
                    Engine.DeltaTime * 2f);
            }
            
            // Check for phone animations (custom characters can have different phone events)
            if (Sprite != null && Sprite.CurrentAnimationID == "usePhone")
            {
                if (PhoneTapSfx == null)
                    Add(PhoneTapSfx = new SoundSource());
                if (!PhoneTapSfx.Playing)
                    PhoneTapSfx.Play("event:/char/theo/phone_taps_loop");
            }
            else
            {
                if (PhoneTapSfx != null && PhoneTapSfx.Playing)
                    PhoneTapSfx.Stop();
            }
        }

        /// <summary>
        /// Setup sprite sounds for Kirby character
        /// </summary>
        public void SetupKirbySpriteSounds() => Sprite.OnFrameChange = anim =>
        {
            int frame = Sprite.CurrentAnimationFrame;
            if ((anim == "walk" && (frame == 0 || frame == 6)) || 
                (anim == "run" && (frame == 0 || frame == 4)))
            {
                Platform platform = SurfaceIndex.GetPlatformByPriority(
                    CollideAll<Platform>(Position + Vector2.UnitY, temp));
                if (platform != null)
                    Audio.Play("event:/char/madeline/footstep", Center, 
                        "surface_index", platform.GetStepSoundIndex(this));
            }
            else if (anim == "float" && frame == 0)
            {
                Audio.Play("event:/desolozantas/kirby/float", Position);
            }
        };

        /// <summary>
        /// Setup sprite sounds for Magolor character
        /// </summary>
        public void SetupMagolorSpriteSounds() => Sprite.OnFrameChange = anim =>
        {
            int frame = Sprite.CurrentAnimationFrame;
            if ((anim == "walk" && (frame == 0 || frame == 4)) ||
                (anim == "float" && frame == 0))
            {
                // Magolor floats, so softer footsteps or magical sounds
                Audio.Play("event:/desolozantas/magolor/hover", Position);
            }
        };

        /// <summary>
        /// Setup sprite sounds for Toriel character (replaces Granny)
        /// </summary>
        public void SetupTorielSpriteSounds() => Sprite.OnFrameChange = anim =>
        {
            int frame = Sprite.CurrentAnimationFrame;
            if (anim == "walk" && (frame == 0 || frame == 4))
            {
                Platform platform = SurfaceIndex.GetPlatformByPriority(
                    CollideAll<Platform>(Position + Vector2.UnitY, temp));
                if (platform != null)
                    Audio.Play("event:/char/madeline/footstep", Center, 
                        "surface_index", platform.GetStepSoundIndex(this));
            }
            else if (anim == "walk" && frame == 2)
            {
                Audio.Play("event:/desolozantas/toriel/footstep", Position);
            }
        };

        /// <summary>
        /// Setup sprite sounds for Ralsei character
        /// </summary>
        public void SetupRalseiSpriteSounds() => Sprite.OnFrameChange = anim =>
        {
            int frame = Sprite.CurrentAnimationFrame;
            if (anim == "walk" && (frame == 0 || frame == 6))
            {
                Platform platform = SurfaceIndex.GetPlatformByPriority(
                    CollideAll<Platform>(Position + Vector2.UnitY, temp));
                if (platform != null)
                    Audio.Play("event:/char/madeline/footstep", Center, 
                        "surface_index", platform.GetStepSoundIndex(this));
            }
        };

        /// <summary>
        /// Setup sprite sounds for Theo character (preserved from vanilla)
        /// </summary>
        public void SetupTheoSpriteSounds() => Sprite.OnFrameChange = anim =>
        {
            int frame = Sprite.CurrentAnimationFrame;
            if ((anim == "walk" && (frame == 0 || frame == 6)) || 
                (anim == "run" && (frame == 0 || frame == 4)))
            {
                Platform platform = SurfaceIndex.GetPlatformByPriority(
                    CollideAll<Platform>(Position + Vector2.UnitY, temp));
                if (platform != null)
                    Audio.Play("event:/char/madeline/footstep", Center, 
                        "surface_index", platform.GetStepSoundIndex(this));
            }
            else if (anim == "crawl" && frame == 0)
            {
                if (!Level.Transitioning)
                    Audio.Play("event:/char/theo/resort_crawl", Position);
            }
            else if (anim == "pullVent" && frame == 0)
            {
                Audio.Play("event:/char/theo/resort_vent_tug", Position);
            }
        };

        /// <summary>
        /// Setup sprite sounds for Granny character (preserved from vanilla)
        /// </summary>
        public void SetupGrannySpriteSounds() => Sprite.OnFrameChange = anim =>
        {
            int frame = Sprite.CurrentAnimationFrame;
            if (anim == "walk" && (frame == 0 || frame == 4))
            {
                Platform platform = SurfaceIndex.GetPlatformByPriority(
                    CollideAll<Platform>(Position + Vector2.UnitY, temp));
                if (platform != null)
                    Audio.Play("event:/char/madeline/footstep", Center, 
                        "surface_index", platform.GetStepSoundIndex(this));
            }
            else if (anim == "walk" && frame == 2)
            {
                Audio.Play("event:/char/granny/cane_tap", Position);
            }
        };

        public IEnumerator PlayerApproachRightSide(
            global::Celeste.Player player,
            bool turnToFace = true,
            float? spacing = null)
        {
            yield return PlayerApproach(player, turnToFace, spacing, 1);
        }

        public IEnumerator PlayerApproachLeftSide(
            global::Celeste.Player player,
            bool turnToFace = true,
            float? spacing = null)
        {
            yield return PlayerApproach(player, turnToFace, spacing, -1);
        }

        public IEnumerator PlayerApproach(
            global::Celeste.Player player,
            bool turnToFace = true,
            float? spacing = null,
            int? side = null)
        {
            if (!side.HasValue)
                side = Math.Sign(player.X - X);
            if (side.GetValueOrDefault() == 0)
                side = 1;
                
            player.StateMachine.State = 11;
            player.StateMachine.Locked = true;
            
            if (spacing.HasValue)
            {
                int targetX = (int)(X + side.Value * spacing.Value);
                yield return player.DummyWalkToExact(targetX);
            }
            else if (Math.Abs(X - player.X) < 12f || Math.Sign(player.X - X) != side.Value)
            {
                int targetX = (int)(X + side.Value * 12);
                yield return player.DummyWalkToExact(targetX);
            }
            
            player.Facing = (Facings)(-side.Value);
            if (turnToFace && Sprite != null)
                Sprite.Scale.X = side.Value;
            yield return null;
        }

        public IEnumerator PlayerApproach48px()
        {
            global::Celeste.Player entity = Scene.Tracker.GetEntity<global::Celeste.Player>();
            yield return PlayerApproach(entity, true, 48);
        }

        public IEnumerator PlayerLeave(global::Celeste.Player player, float? to = null)
        {
            if (to.HasValue)
                yield return player.DummyWalkToExact((int)to.Value);
            player.StateMachine.Locked = false;
            player.StateMachine.State = 0;
            yield return null;
        }

        public IEnumerator MoveTo(
            Vector2 target,
            bool fadeIn = false,
            int? turnAtEndTo = null,
            bool removeAtEnd = false)
        {
            if (removeAtEnd)
                Tag |= (int)Tags.TransitionUpdate;
                
            if (Math.Sign(target.X - X) != 0 && Sprite != null)
                Sprite.Scale.X = Math.Sign(target.X - X);
                
            float alpha = fadeIn ? 0f : 1f;
            if (Sprite != null && Sprite.Has(MoveAnim))
                Sprite.Play(MoveAnim);
                
            float speed = 0f;
            while ((MoveY && Position != target) || (!MoveY && X != target.X))
            {
                speed = Calc.Approach(speed, Maxspeed, 160f * Engine.DeltaTime);
                if (MoveY)
                    Position = Calc.Approach(Position, target, speed * Engine.DeltaTime);
                else
                    X = Calc.Approach(X, target.X, speed * Engine.DeltaTime);
                    
                if (Sprite != null)
                    Sprite.Color = Color.White * alpha;
                alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime);
                yield return null;
            }
            
            if (Sprite != null && Sprite.Has(IdleAnim))
                Sprite.Play(IdleAnim);
                
            while (alpha < 1f)
            {
                if (Sprite != null)
                    Sprite.Color = Color.White * alpha;
                alpha = Calc.Approach(alpha, 1f, Engine.DeltaTime);
                yield return null;
            }
            
            if (turnAtEndTo.HasValue && Sprite != null)
                Sprite.Scale.X = turnAtEndTo.Value;
            if (removeAtEnd)
                Scene.Remove(this);
            yield return null;
        }

        public void MoveToAndRemove(Vector2 target) => Add(new Coroutine(MoveTo(target, removeAtEnd: true)));
    }
}
