namespace DesoloZantas.Core.Core
{
    public class StarsiFollower : Entity
    {
        public static ParticleType PVanish;
        
        private const float tolerance = 0.01f;

        public readonly PlayerSprite Sprite;
        private readonly VertexLight light;
        private const float float_speed = 120f;
        private const float float_accel = 240f;
        public float Floatness = 2f;
        private Vector2 floatNormal = new Vector2(0.0f, 1f);

        public StarsiFollower(Vector2 position) : base(position)
        {
            SineWave wave;
            Collider = new Hitbox(6f, 6f, -3f, -7f);
            Sprite = new PlayerSprite(PlayerSpriteMode.Ralsei);
            Sprite.Play("fallSlow");
            Sprite.Scale.X = -1f;
            Add(Sprite);
            Add(new CharaAutoAnimator());
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
            Add(light = new VertexLight(new Vector2(0.0f, -8f), Color.White, 1f, 20, 60));
        }

        public void Appear(Level level, bool silent = false)
        {
            if (!silent)
            {
                Audio.Play("event:/char/badeline/appear", Position);
                Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            }

            level.Displacement.AddBurst(Center, 0.5f, 24f, 96f, 0.4f);
            level.Particles.Emit(PVanish, 12, Center, Vector2.One * 6f);
        }

        public IEnumerator FloatTo(
            Vector2 target,
            int? turnAtEndTo = null,
            bool faceDirection = true,
            bool fadeLight = false,
            bool quickEnd = false)
        {
            StarsiFollower starsiFollower = this;
            starsiFollower.Sprite.Play("fallSlow");
            if (faceDirection && Math.Sign(target.X - starsiFollower.X) != 0)
                starsiFollower.Sprite.Scale.X = Math.Sign(target.X - starsiFollower.X);
            Vector2 vector2 = (target - starsiFollower.Position).SafeNormalize();
            Vector2 perp = new Vector2(-vector2.Y, vector2.X);
            float speed = 0.0f;
            while (starsiFollower.Position != target)
            {
                speed = Calc.Approach(speed, float_speed, float_accel * Engine.DeltaTime);
                starsiFollower.Position = Calc.Approach(starsiFollower.Position, target, speed * Engine.DeltaTime);
                starsiFollower.Floatness = Calc.Approach(starsiFollower.Floatness, 4f, 8f * Engine.DeltaTime);
                starsiFollower.floatNormal = Calc.Approach(starsiFollower.floatNormal, perp, Engine.DeltaTime * 12f);
                if (fadeLight)
                    starsiFollower.light.Alpha = Calc.Approach(starsiFollower.light.Alpha, 0.0f, Engine.DeltaTime * 2f);
                yield return null;
            }

            if (quickEnd)
            {
                starsiFollower.Floatness = 2f;
            }
            else
            {
                while (Math.Abs(starsiFollower.Floatness - 2.0) > tolerance)
                {
                    starsiFollower.Floatness = Calc.Approach(starsiFollower.Floatness, 2f, 8f * Engine.DeltaTime);
                    yield return null;
                }
            }

            if (turnAtEndTo.HasValue)
                starsiFollower.Sprite.Scale.X = turnAtEndTo.Value;
        }

        public IEnumerator WalkTo(float x, float speed = 64f)
        {
            StarsiFollower starsiFollower = this;
            starsiFollower.Floatness = 0.0f;
            starsiFollower.Sprite.Play("walk");
            if (Math.Sign(x - starsiFollower.X) != 0)
                starsiFollower.Sprite.Scale.X = Math.Sign(x - starsiFollower.X);
            while (Math.Abs(starsiFollower.X - (double)x) > tolerance)
            {
                starsiFollower.X = Calc.Approach(starsiFollower.X, x, Engine.DeltaTime * speed);
                yield return null;
            }

            starsiFollower.Sprite.Play("idle");
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




