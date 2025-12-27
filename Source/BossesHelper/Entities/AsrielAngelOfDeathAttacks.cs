using Celeste;
using Celeste.Mod;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections;

namespace Celeste.Mod.DesoloZatnas.BossesHelper.Entities
{
    /// <summary>
    /// Ultima Bullet - Homing projectile attack
    /// </summary>
    public class UltimaBulletProjectile : Entity
    {
        private Vector2 velocity;
        private Player targetPlayer;
        private Sprite sprite;
        private float homingStrength = 120f;
        private float speed = 180f;
        private float lifetime = 5f;
        private ParticleType trailParticle;
        
        public UltimaBulletProjectile(Vector2 position, Vector2 initialDirection, Player target)
            : base(position)
        {
            targetPlayer = target;
            velocity = initialDirection * speed;
            
            Collider = new Hitbox(8f, 8f, -4f, -4f);
            
            Add(sprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/projectiles/"));
            sprite.AddLoop("ultimabullet", "ultimabullet", 0.03f);
            sprite.Play("ultimabullet");
            sprite.CenterOrigin();
            
            Add(new PlayerCollider(OnPlayerCollide));
            
            trailParticle = new ParticleType
            {
                Color = Color.Cyan,
                Color2 = Color.Magenta,
                ColorMode = ParticleType.ColorModes.Blink,
                Size = 1f,
                LifeMin = 0.3f,
                LifeMax = 0.6f,
                SpeedMin = 10f,
                SpeedMax = 20f
            };
        }
        
        public override void Update()
        {
            base.Update();
            
            lifetime -= Engine.DeltaTime;
            if (lifetime <= 0)
            {
                RemoveSelf();
                return;
            }
            
            // Homing behavior
            if (targetPlayer != null && !targetPlayer.Dead)
            {
                Vector2 toPlayer = (targetPlayer.Position - Position).SafeNormalize();
                velocity += toPlayer * homingStrength * Engine.DeltaTime;
                
                if (velocity.Length() > speed)
                {
                    velocity = velocity.SafeNormalize() * speed;
                }
            }
            
            Position += velocity * Engine.DeltaTime;
            
            // Rotation based on velocity
            sprite.Rotation = velocity.Angle();
            
            // Particle trail
            if (Scene.OnInterval(0.05f))
            {
                SceneAs<Level>().Particles.Emit(trailParticle, Position);
            }
        }
        
        private void OnPlayerCollide(Player player)
        {
            player.Die((player.Position - Position).SafeNormalize());
            RemoveSelf();
        }
    }
    
    /// <summary>
    /// Cross Shocker - Lightning wave that travels in a direction
    /// </summary>
    public class CrossShockerWave : Entity
    {
        private Vector2 direction;
        private float speed;
        private Sprite sprite;
        private float lifetime = 3f;
        
        public CrossShockerWave(Vector2 position, Vector2 dir, float spd)
            : base(position)
        {
            direction = dir;
            speed = spd;
            
            Collider = new Hitbox(16f, 16f, -8f, -8f);
            
            Add(sprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/projectiles/"));
            sprite.AddLoop("crossshocker", "crossshocker", 0.04f);
            sprite.Play("crossshocker");
            sprite.CenterOrigin();
            sprite.Rotation = direction.Angle();
            
            Add(new PlayerCollider(OnPlayerCollide));
        }
        
        public override void Update()
        {
            base.Update();
            
            lifetime -= Engine.DeltaTime;
            if (lifetime <= 0)
            {
                RemoveSelf();
                return;
            }
            
            Position += direction * speed * Engine.DeltaTime;
            
            // Lightning effect
            sprite.Scale = Vector2.One * (1f + (float)Math.Sin(Scene.TimeActive * 10f) * 0.2f);
        }
        
        private void OnPlayerCollide(Player player)
        {
            player.Die((player.Position - Position).SafeNormalize());
        }
    }
    
    /// <summary>
    /// Star Storm - Falling star projectile
    /// </summary>
    public class StarStormProjectile : Entity
    {
        private Sprite sprite;
        private float fallSpeed = 200f;
        private float acceleration = 50f;
        private ParticleType starParticle;
        
        public StarStormProjectile(Vector2 position)
            : base(position)
        {
            Collider = new Hitbox(12f, 12f, -6f, -6f);
            
            Add(sprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/projectiles/"));
            sprite.AddLoop("starstorm", "starstorm", 0.05f);
            sprite.Play("starstorm");
            sprite.CenterOrigin();
            
            Add(new PlayerCollider(OnPlayerCollide));
            
            starParticle = new ParticleType
            {
                Color = Color.Yellow,
                Color2 = Color.White,
                ColorMode = ParticleType.ColorModes.Blink,
                Size = 2f,
                LifeMin = 0.4f,
                LifeMax = 0.8f
            };
        }
        
        public override void Update()
        {
            base.Update();
            
            fallSpeed += acceleration * Engine.DeltaTime;
            Position.Y += fallSpeed * Engine.DeltaTime;
            
            sprite.Rotation += Engine.DeltaTime * 3f;
            
            // Remove if off screen
            if (Y > SceneAs<Level>().Bounds.Bottom + 20)
            {
                RemoveSelf();
                return;
            }
            
            // Particle effect
            if (Scene.OnInterval(0.08f))
            {
                SceneAs<Level>().Particles.Emit(starParticle, Position);
            }
        }
        
        private void OnPlayerCollide(Player player)
        {
            player.Die(Vector2.UnitY);
            
            // Create explosion effect
            for (int i = 0; i < 12; i++)
            {
                float angle = i * MathHelper.TwoPi / 12f;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                SceneAs<Level>().Particles.Emit(starParticle, Position, dir.Angle());
            }
            
            RemoveSelf();
        }
    }
    
    /// <summary>
    /// Shocker Breaker III - Expanding circular shockwave
    /// </summary>
    public class ShockerBreaker3Wave : Entity
    {
        private float radius;
        private float targetRadius;
        private float expandSpeed = 200f;
        private float thickness = 20f;
        private float lifetime;
        private Color waveColor;
        
        public ShockerBreaker3Wave(Vector2 position, float targetRad, float delay)
            : base(position)
        {
            targetRadius = targetRad;
            radius = 0f;
            lifetime = delay;
            
            waveColor = new Color(0.3f, 0.8f, 1f, 0.8f);
            
            Depth = -1000;
        }
        
        public override void Update()
        {
            base.Update();
            
            if (lifetime > 0)
            {
                lifetime -= Engine.DeltaTime;
                return;
            }
            
            radius += expandSpeed * Engine.DeltaTime;
            
            if (radius >= targetRadius)
            {
                RemoveSelf();
                return;
            }
            
            // Check collision with player
            Player player = Scene.Tracker.GetEntity<Player>();
            if (player != null)
            {
                float distToPlayer = Vector2.Distance(Position, player.Position);
                if (Math.Abs(distToPlayer - radius) < thickness)
                {
                    player.Die((player.Position - Position).SafeNormalize());
                }
            }
        }
        
        public override void Render()
        {
            base.Render();
            
            if (lifetime <= 0 && radius > 0)
            {
                // Draw the shockwave ring
                for (int i = 0; i < 360; i += 5)
                {
                    float angle = MathHelper.ToRadians(i);
                    Vector2 pos1 = Position + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (radius - thickness / 2);
                    Vector2 pos2 = Position + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (radius + thickness / 2);
                    
                    Draw.Line(pos1, pos2, waveColor, 2f);
                }
                
                // Electric arcs around the ring
                if (Scene.OnInterval(0.1f))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        float angle = Calc.Random.NextFloat() * MathHelper.TwoPi;
                        Vector2 arcPos = Position + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                        
                        float arcLength = Calc.Random.Range(10f, 30f);
                        Vector2 arcEnd = arcPos + Calc.AngleToVector(Calc.Random.NextFloat() * MathHelper.TwoPi, arcLength);
                        
                        Draw.Line(arcPos, arcEnd, Color.Cyan, 1f);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Final Beam - Massive sustained laser beam
    /// </summary>
    public class FinalBeamAttack : Entity
    {
        private Vector2 direction;
        private float duration;
        private float remainingDuration;
        private float beamWidth = 40f;
        private float beamLength = 1000f;
        private Sprite beamSprite;
        private Color beamColor1;
        private Color beamColor2;
        private float pulseTimer;
        
        public FinalBeamAttack(Vector2 position, Vector2 dir, float dur)
            : base(position)
        {
            direction = dir;
            duration = dur;
            remainingDuration = dur;
            
            beamColor1 = new Color(1f, 0.3f, 0.8f, 0.9f); // Pink
            beamColor2 = new Color(0.3f, 0.8f, 1f, 0.9f); // Cyan
            
            Depth = -10000;
            
            Add(beamSprite = new Sprite(GFX.Game, "characters/asrielangelofdeathboss/projectiles/"));
            beamSprite.AddLoop("finalbeam", "finalbeam", 0.02f);
            beamSprite.Play("finalbeam");
        }
        
        public override void Update()
        {
            base.Update();
            
            remainingDuration -= Engine.DeltaTime;
            pulseTimer += Engine.DeltaTime * 10f;
            
            if (remainingDuration <= 0)
            {
                RemoveSelf();
                return;
            }
            
            // Check collision with player along the beam
            Player player = Scene.Tracker.GetEntity<Player>();
            if (player != null)
            {
                Vector2 toPlayer = player.Position - Position;
                float projectionLength = Vector2.Dot(toPlayer, direction);
                
                if (projectionLength > 0 && projectionLength < beamLength)
                {
                    Vector2 closestPoint = Position + direction * projectionLength;
                    float distanceToBeam = Vector2.Distance(player.Position, closestPoint);
                    
                    if (distanceToBeam < beamWidth / 2)
                    {
                        player.Die(direction);
                    }
                }
            }
        }
        
        public override void Render()
        {
            base.Render();
            
            Vector2 endPoint = Position + direction * beamLength;
            
            // Draw multiple layers for glow effect
            float pulse = (float)Math.Sin(pulseTimer) * 0.5f + 0.5f;
            
            // Outer glow
            Draw.Line(Position, endPoint, beamColor1 * 0.3f, beamWidth * 1.5f);
            
            // Middle layer with pulse
            Draw.Line(Position, endPoint, Color.Lerp(beamColor1, beamColor2, pulse), beamWidth * (1f + pulse * 0.2f));
            
            // Core beam
            Draw.Line(Position, endPoint, Color.White, beamWidth * 0.5f);
            
            // Particle effects along the beam
            if (Scene.OnInterval(0.05f))
            {
                ParticleType beamParticle = new ParticleType
                {
                    Color = beamColor1,
                    Color2 = beamColor2,
                    ColorMode = ParticleType.ColorModes.Blink,
                    Size = 2f,
                    LifeMin = 0.2f,
                    LifeMax = 0.5f,
                    SpeedMin = 10f,
                    SpeedMax = 30f
                };
                
                for (int i = 0; i < 10; i++)
                {
                    float t = Calc.Random.NextFloat();
                    Vector2 particlePos = Position + direction * beamLength * t;
                    Vector2 offset = new Vector2(
                        Calc.Random.Range(-beamWidth / 2, beamWidth / 2),
                        Calc.Random.Range(-beamWidth / 2, beamWidth / 2)
                    );
                    
                    SceneAs<Level>().ParticlesFG.Emit(beamParticle, particlePos + offset);
                }
            }
        }
    }
}
