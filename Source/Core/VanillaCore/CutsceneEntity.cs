namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Abstract base class for all cutscene entities.
    /// Ported from vanilla Celeste with DesoloZantas customizations.
    /// </summary>
    public abstract class CutsceneEntity : Entity
    {
        public bool WasSkipped;
        public bool RemoveOnSkipped = true;
        public bool EndingChapterAfter;
        public Level Level;

        public bool Running { get; private set; }
        public bool FadeInOnSkip { get; private set; }

        public CutsceneEntity(bool fadeInOnSkip = true, bool endingChapterAfter = false)
        {
            FadeInOnSkip = fadeInOnSkip;
            EndingChapterAfter = endingChapterAfter;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            Level = scene as Level;
            Start();
        }

        public void Start()
        {
            Running = true;
            Level.StartCutscene(SkipCutscene, FadeInOnSkip, EndingChapterAfter);
            OnBegin(Level);
        }

        public override void Update()
        {
            if (Level.RetryPlayerCorpse != null)
                Active = false;
            else
                base.Update();
        }

        private void SkipCutscene(Level level)
        {
            WasSkipped = true;
            EndCutscene(level, RemoveOnSkipped);
        }

        public void EndCutscene(Level level, bool removeSelf = true)
        {
            Running = false;
            OnEnd(level);
            level.EndCutscene();
            if (removeSelf)
                RemoveSelf();
        }

        /// <summary>
        /// Called when the cutscene begins
        /// </summary>
        public abstract void OnBegin(Level level);

        /// <summary>
        /// Called when the cutscene ends (either completed or skipped)
        /// </summary>
        public abstract void OnEnd(Level level);

        /// <summary>
        /// Smoothly move the camera to a target position
        /// </summary>
        public static IEnumerator CameraTo(
            Vector2 target,
            float duration,
            Ease.Easer ease = null,
            float delay = 0f)
        {
            if (ease == null)
                ease = Ease.CubeInOut;
            if (delay > 0f)
                yield return delay;
                
            Level level = Engine.Scene as Level;
            Vector2 from = level.Camera.Position;
            
            for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
            {
                level.Camera.Position = from + (target - from) * ease(p);
                yield return null;
            }
            level.Camera.Position = target;
        }

        /// <summary>
        /// Custom camera follow for cutscene characters
        /// </summary>
        public static IEnumerator CameraFollowEntity(
            Entity entity,
            float duration,
            Vector2 offset = default,
            Ease.Easer ease = null)
        {
            if (ease == null)
                ease = Ease.CubeInOut;
                
            Level level = Engine.Scene as Level;
            Vector2 from = level.Camera.Position;
            
            for (float p = 0f; p < 1f; p += Engine.DeltaTime / duration)
            {
                Vector2 target = entity.Position + offset - new Vector2(160f, 90f);
                level.Camera.Position = Vector2.Lerp(from, target, ease(p));
                yield return null;
            }
        }

        /// <summary>
        /// Helper to make character walk to a position
        /// </summary>
        public static IEnumerator CharacterWalkTo(
            Entity character,
            float targetX,
            float speed = 64f,
            string walkAnim = "walk",
            string idleAnim = "idle")
        {
            Sprite sprite = character.Get<Sprite>();
            if (sprite != null && sprite.Has(walkAnim))
                sprite.Play(walkAnim);
                
            int direction = Math.Sign(targetX - character.X);
            if (sprite != null)
                sprite.Scale.X = direction;
                
            while (Math.Abs(character.X - targetX) > 1f)
            {
                character.X += direction * speed * Engine.DeltaTime;
                yield return null;
            }
            character.X = targetX;
            
            if (sprite != null && sprite.Has(idleAnim))
                sprite.Play(idleAnim);
        }

        /// <summary>
        /// Helper to face a character toward another entity
        /// </summary>
        public static void FaceToward(Entity character, Entity target)
        {
            Sprite sprite = character.Get<Sprite>();
            if (sprite != null)
                sprite.Scale.X = Math.Sign(target.X - character.X);
        }
    }
}
