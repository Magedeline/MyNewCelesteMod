#nullable enable
using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core
{
    [CustomEntity("Ingeste/StarsiDummy")]
    public class StarsiDummy : Entity
    {
        private const float tolerance = 0.01f;

        public readonly PlayerSprite Sprite;
        private readonly VertexLight light;
        private const float float_speed = 120f;
        private const float float_accel = 240f;
        public float Floatness = 2f;
        private Vector2 floatNormal = new(0.0f, 1f);

        public StarsiDummy(Vector2 position)
            : base(position)
        {
        }

        // Standard Everest constructor for map loading
        public StarsiDummy(EntityData data, Vector2 offset)
            : this(data.Position + offset)
        {
            SineWave wave;
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Ralsei); // Using Ralsei sprite mode for Starsi
            Sprite.Play("fallSlow");
            Sprite.Scale.X = -1f;
            Add(Sprite);
            Add(new StarsiAutoAnimator());
            Sprite.OnFrameChange = anim =>
            {
                var currentAnimationFrame = Sprite.CurrentAnimationFrame;
                if ((anim != "walk" || (currentAnimationFrame != 0 && currentAnimationFrame != 6)) &&
                    (anim != "runSlow" || (currentAnimationFrame != 0 && currentAnimationFrame != 6)) &&
                    (anim != "runFast" || (currentAnimationFrame != 0 && currentAnimationFrame != 6)))
                    return;
                Audio.Play("event:/char/madeline/footstep", Position);
            };
            Add(wave = new SineWave(0.25f));
            wave.OnUpdate = f => Sprite.Position = floatNormal * f * Floatness;
            Add(light = new VertexLight(new Vector2(0.0f, -8f), Color.LightBlue, 1f, 20, 60));
        }

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/badeline/appear", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }

            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f);
            // Use appropriate particle effect - assuming StarsiFollower.PVanish exists
            level.Particles?.Emit(BadelineDash.PBurst, 12, Center, Vector2.One * 6f);
        }

        public void Vanish()
        {
            Audio.Play("event:/char/badeline/disappear", Position);
            Shockwave();
            Level level = SceneAs<Level>();
            if (level != null && level.Particles != null)
            {
                level.Particles.Emit(BadelineDash.PBurst, 12, Center, Vector2.One * 6f);
            }
            RemoveSelf();
        }

        private void Shockwave() => SceneAs<Level>().Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f);

        public IEnumerator FloatTo(
            Vector2 target,
            int? turnAtEndTo = null,
            bool faceDirection = true,
            bool fadeLight = false,
            bool quickEnd = false)
        {
            StarsiDummy starsiDummy = this;
            starsiDummy.Sprite.Play("fallSlow");
            if (faceDirection && Math.Sign(target.X - starsiDummy.X) != 0)
                starsiDummy.Sprite.Scale.X = Math.Sign(target.X - starsiDummy.X);
            Vector2 vector2 = (target - starsiDummy.Position).SafeNormalize();
            Vector2 perp = new(-vector2.Y, vector2.X);
            float speed = 0.0f;
            while (starsiDummy.Position != target)
            {
                speed = Calc.Approach(speed, float_speed, float_accel * Engine.DeltaTime);
                starsiDummy.Position = Calc.Approach(starsiDummy.Position, target, speed * Engine.DeltaTime);
                starsiDummy.Floatness = Calc.Approach(starsiDummy.Floatness, 4f, 8f * Engine.DeltaTime);
                starsiDummy.floatNormal = Calc.Approach(starsiDummy.floatNormal, perp, Engine.DeltaTime * 12f);
                if (fadeLight)
                    starsiDummy.light.Alpha = Calc.Approach(starsiDummy.light.Alpha, 0.0f, Engine.DeltaTime * 2f);
                yield return null;
            }

            if (quickEnd)
            {
                starsiDummy.Floatness = 2f;
            }
            else
            {
                while (Math.Abs(starsiDummy.Floatness - 2.0) > tolerance)
                {
                    starsiDummy.Floatness = Calc.Approach(starsiDummy.Floatness, 2f, 8f * Engine.DeltaTime);
                    yield return null;
                }
            }

            if (turnAtEndTo.HasValue)
                starsiDummy.Sprite.Scale.X = turnAtEndTo.Value;
        }

        public IEnumerator WalkTo(float x, float speed = 64f)
        {
            StarsiDummy starsiDummy = this;
            starsiDummy.Floatness = 0.0f;
            starsiDummy.Sprite.Play("walk");
            if (Math.Sign(x - starsiDummy.X) != 0)
                starsiDummy.Sprite.Scale.X = Math.Sign(x - starsiDummy.X);
            const float tolerance = 0.01f;
            while (Math.Abs(starsiDummy.X - x) > tolerance)
            {
                starsiDummy.X = Calc.Approach(starsiDummy.X, x, Engine.DeltaTime * speed);
                yield return null;
            }

            starsiDummy.Sprite.Play("idle");
        }

        public override void Render()
        {
            Vector2 renderPosition = Sprite.RenderPosition;
            Sprite.RenderPosition = Sprite.RenderPosition.Floor();
            base.Render();
            Sprite.RenderPosition = renderPosition;
        }
    }
}



