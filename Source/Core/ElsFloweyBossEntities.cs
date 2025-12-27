namespace DesoloZantas.Core.Core
{
    // Base class for arena elements
    public abstract class ArenaElement : Entity
    {
        protected ArenaElement(Vector2 position) : base(position) { }
    }

    // Human Soul collectible
    [Tracked]
    public class HumanSoul : Entity
    {
        private string soulType;
        private Sprite sprite;
        private Color soulColor;
        private float floatOffset;
        private SineWave floatWave;
        
        public HumanSoul(Vector2 position, string type) : base(position)
        {
            soulType = type;
            soulColor = GetSoulColor(type);
            
            sprite = new Sprite(GFX.Game, "collectables/souls/");
            sprite.Add("float", $"{type}", 0.1f);
            sprite.Play("float");
            sprite.CenterOrigin();
            sprite.Color = soulColor;
            
            Add(sprite);
            Add(floatWave = new SineWave(1.5f, 0f));
            
            Collider = new Circle(16f);
            Add(new PlayerCollider(OnPlayerTouch));
            
            // Glowing effect
            Add(new BloomPoint(0.5f, 16f));
        }
        
        private Color GetSoulColor(string type)
        {
            return type switch
            {
                "patience" => Color.Cyan,
                "bravery" => Color.Orange, 
                "integrity" => Color.Blue,
                "perseverance" => Color.Purple,
                "kindness" => Color.Green,
                "justice" => Color.Yellow,
                _ => Color.White
            };
        }
        
        public override void Update()
        {
            base.Update();
            floatOffset = floatWave.Value * 8f;
            Position = Position.X * Vector2.UnitX + (Position.Y + floatOffset) * Vector2.UnitY;
        }
        
        private void OnPlayerTouch(global::Celeste.Player player)
        {
            // Let the boss handle the collection
            Audio.Play("event:/game/general/collectible_keyget", Position);
            RemoveSelf();
        }
    }

    // Organic Vine attack
    public class OrganicVine : ArenaElement
    {
        private Sprite sprite;
        private Vector2 growDirection;
        private float growSpeed;
        private float maxLength;
        private List<Vector2> segments;
        
        public OrganicVine(Vector2 position) : base(position)
        {
            sprite = new Sprite(GFX.Game, "objects/desolozantas/organic/");
            sprite.Add("vine", "vine", 0.1f);
            sprite.Play("vine");
            Add(sprite);
            
            growDirection = Calc.AngleToVector(Calc.Random.NextAngle(), 1f);
            growSpeed = 150f;
            maxLength = 200f;
            segments = new List<Vector2> { position };
            
            Collider = new Hitbox(16, 16);
            Add(new PlayerCollider(OnPlayer));
        }
        
        public override void Update()
        {
            base.Update();
            
            // Grow the vine
            if (segments.Count * 16 < maxLength)
            {
                Vector2 lastSegment = segments[segments.Count - 1];
                Vector2 newSegment = lastSegment + growDirection * growSpeed * Engine.DeltaTime;
                segments.Add(newSegment);
            }
        }
        
        private void OnPlayer(global::Celeste.Player player)
        {
            player.Die((Position - player.Position).SafeNormalize());
        }
        
        public override void Render()
        {
            for (int i = 0; i < segments.Count - 1; i++)
            {
                Vector2 from = segments[i];
                Vector2 to = segments[i + 1];
                Draw.Line(from, to, Color.DarkGreen, 8f);
            }
        }
    }

    // Bloody Vine whip attack
    public class BloodyVine : Entity
    {
        private Vector2 direction;
        private float speed;
        private float length;
        private float currentLength;
        private bool isRetracting;
        
        public BloodyVine(Vector2 position, Vector2 direction, float maxLength) : base(position)
        {
            this.direction = direction.SafeNormalize();
            speed = 300f;
            length = maxLength;
            
            Collider = new Hitbox(8, 8);
            Add(new PlayerCollider(OnPlayerHit));
            
            Audio.Play("event:/char/desolozantas/vine_extend", Position);
        }
        
        public override void Update()
        {
            base.Update();
            
            if (!isRetracting)
            {
                currentLength += speed * Engine.DeltaTime;
                Position += direction * speed * Engine.DeltaTime;
                
                if (currentLength >= length)
                {
                    isRetracting = true;
                }
            }
            else
            {
                currentLength -= speed * 2 * Engine.DeltaTime;
                Position -= direction * speed * 2 * Engine.DeltaTime;
                
                if (currentLength <= 0)
                {
                    RemoveSelf();
                }
            }
        }
        
        private void OnPlayerHit(global::Celeste.Player player)
        {
            player.Die((Position - player.Position).SafeNormalize());
        }
        
        public override void Render()
        {
            Vector2 start = Position - direction * currentLength;
            Draw.Line(start, Position, Color.DarkRed, 12f);
            
            // Add thorns
            for (float i = 0; i < currentLength; i += 16f)
            {
                Vector2 thornPos = start + direction * i;
                Draw.Pixel.Draw(thornPos, Vector2.Zero, Color.Red, Vector2.One * 4f);
            }
        }
    }

    // Organ Projectile
    public class OrganProjectile : Entity
    {
        private Sprite sprite;
        private Vector2 velocity;
        private string organType;
        private float lifetime;
        
        public OrganProjectile(Vector2 position, Vector2 velocity, string type) : base(position)
        {
            this.velocity = velocity;
            organType = type;
            lifetime = 5f;
            
            sprite = new Sprite(GFX.Game, $"objects/desolozantas/organs/");
            sprite.Add("spin", $"{type}", 0.05f);
            sprite.Play("spin");
            sprite.CenterOrigin();
            Add(sprite);
            
            Collider = new Circle(8f);
            Add(new PlayerCollider(OnPlayerHit));
            
            // Add blood trail
            Add(new TrailParticles());
        }
        
        public override void Update()
        {
            base.Update();
            
            Position += velocity * Engine.DeltaTime;
            velocity.Y += 200f * Engine.DeltaTime; // Gravity
            
            lifetime -= Engine.DeltaTime;
            if (lifetime <= 0)
            {
                CreateSplatterEffect();
                RemoveSelf();
            }
        }
        
        private void OnPlayerHit(global::Celeste.Player player)
        {
            CreateSplatterEffect();
            player.Die((velocity).SafeNormalize());
            RemoveSelf();
        }
        
        private void CreateSplatterEffect()
        {
            Audio.Play("event:/char/desolozantas/organ_splatter", Position);
            
            for (int i = 0; i < 10; i++)
            {
                Vector2 bloodVel = Calc.AngleToVector(Calc.Random.NextAngle(), Calc.Random.Range(20f, 80f));
                Scene.Add(new BloodSplatter(Position, bloodVel));
            }
        }
    }

    // Blood Splatter effect
    public class BloodSplatter : Entity
    {
        private Vector2 velocity;
        private float lifetime;
        private Color color;
        
        public BloodSplatter(Vector2 position, Vector2 velocity) : base(position)
        {
            this.velocity = velocity;
            lifetime = 2f;
            color = Color.DarkRed;
        }
        
        public override void Update()
        {
            base.Update();
            
            Position += velocity * Engine.DeltaTime;
            velocity.Y += 150f * Engine.DeltaTime;
            velocity *= 0.98f;
            
            lifetime -= Engine.DeltaTime;
            if (lifetime <= 0)
            {
                RemoveSelf();
            }
        }
        
        public override void Render()
        {
            float alpha = lifetime / 2f;
            Draw.Pixel.Draw(Position, Vector2.Zero, color * alpha, 3f);
        }
    }

    // Beating Heart decorative element
    public class BeatingHeart : ArenaElement
    {
        private Sprite sprite;
        private SineWave beatWave;
        private float baseScale;
        
        public BeatingHeart(Vector2 position) : base(position)
        {
            sprite = new Sprite(GFX.Game, "objects/desolozantas/organs/");
            sprite.Add("beat", "heart", 0.8f); // Slow heartbeat
            sprite.Play("beat");
            sprite.CenterOrigin();
            Add(sprite);
            
            baseScale = Calc.Random.Range(0.8f, 1.2f);
            beatWave = new SineWave(1.2f, 0f); // Heart rate
            Add(beatWave);
            
            // Pulsing sound
            Audio.Play("event:/char/desolozantas/heartbeat_ambient", Position);
        }
        
        public override void Update()
        {
            base.Update();
            
            // Scale with heartbeat
            float scale = baseScale + beatWave.Value * 0.1f;
            sprite.Scale = Vector2.One * scale;
            
            // Random color variation
            sprite.Color = Color.Lerp(Color.Red, Color.DarkRed, Math.Abs(beatWave.Value));
        }
    }

    // Bone Spear attack
    public class BoneSpear : Entity
    {
        private Sprite sprite;
        private float emergenceSpeed;
        private float emergenceHeight;
        private float maxHeight;
        
        public BoneSpear(Vector2 position) : base(position)
        {
            sprite = new Sprite(GFX.Game, "objects/desolozantas/bones/");
            sprite.Add("emerge", "spear_emerge", 0.1f);
            sprite.Add("idle", "spear_idle", 0.1f);
            sprite.Play("emerge");
            sprite.CenterOrigin();
            Add(sprite);
            
            emergenceSpeed = 200f;
            maxHeight = 64f;
            emergenceHeight = 0f;
            
            Collider = new Hitbox(8, 8, -4, -4);
            Add(new PlayerCollider(OnPlayerHit));
            
            // Telegraph warning
            Scene.Add(new TelegraphWarning(position, Color.White, 0.8f));
        }
        
        public override void Update()
        {
            base.Update();
            
            emergenceHeight += emergenceSpeed * Engine.DeltaTime;
            
            if (emergenceHeight >= maxHeight)
            {
                emergenceHeight = maxHeight;
                sprite.Play("idle");
                
                // Start retracting after delay
                Add(new Coroutine(RetractAfterDelay()));
            }
            
            // Update visual position
            sprite.Position = Vector2.UnitY * (maxHeight - emergenceHeight);
        }
        
        private IEnumerator RetractAfterDelay()
        {
            yield return 2f;
            
            while (emergenceHeight > 0)
            {
                emergenceHeight -= emergenceSpeed * 2 * Engine.DeltaTime;
                sprite.Position = Vector2.UnitY * (maxHeight - emergenceHeight);
                yield return null;
            }
            
            RemoveSelf();
        }
        
        private void OnPlayerHit(global::Celeste.Player player)
        {
            player.Die(Vector2.UnitY);
        }
    }

    // Bone Spike attack (smaller, faster version of BoneSpear)
    public class BoneSpike : Entity
    {
        private Sprite sprite;
        private float emergenceSpeed;
        private float emergenceHeight;
        private float maxHeight;
        private bool isRetracting;
        
        public BoneSpike(Vector2 position) : base(position)
        {
            sprite = new Sprite(GFX.Game, "objects/desolozantas/bones/");
            sprite.Add("emerge", "spike_emerge", 0.15f);
            sprite.Add("idle", "spike_idle", 0.1f);
            sprite.Play("emerge");
            sprite.CenterOrigin();
            Add(sprite);
            
            emergenceSpeed = 300f; // Faster than BoneSpear
            maxHeight = 32f; // Shorter than BoneSpear
            emergenceHeight = 0f;
            isRetracting = false;
            
            Collider = new Hitbox(6, 6, -3, -3); // Smaller than BoneSpear
            Add(new PlayerCollider(OnPlayerHit));
            
            // Shorter telegraph warning
            Scene.Add(new TelegraphWarning(position, Color.LightGray, 0.5f));
        }
        
        public override void Update()
        {
            base.Update();
            
            if (!isRetracting)
            {
                emergenceHeight += emergenceSpeed * Engine.DeltaTime;
                
                if (emergenceHeight >= maxHeight)
                {
                    emergenceHeight = maxHeight;
                    sprite.Play("idle");
                    
                    // Start retracting after shorter delay than BoneSpear
                    Add(new Coroutine(RetractAfterDelay()));
                    isRetracting = true;
                }
            }
            
            // Update visual position
            sprite.Position = Vector2.UnitY * (maxHeight - emergenceHeight);
        }
        
        private IEnumerator RetractAfterDelay()
        {
            yield return 1.5f; // Shorter delay than BoneSpear
            
            while (emergenceHeight > 0)
            {
                emergenceHeight -= emergenceSpeed * 1.5f * Engine.DeltaTime;
                sprite.Position = Vector2.UnitY * (maxHeight - emergenceHeight);
                yield return null;
            }
            
            RemoveSelf();
        }
        
        private void OnPlayerHit(global::Celeste.Player player)
        {
            player.Die(Vector2.UnitY);
            
            // Small screen shake for spike hit
            SceneAs<Level>()?.DirectionalShake(Vector2.UnitY, 0.2f);
            
            // Spike destruction effect
            Audio.Play("event:/char/desolozantas/bone_spike_hit", Position);
            
            // Small bone particles
            Level level = SceneAs<Level>();
            if (level != null)
            {
                for (int i = 0; i < 8; i++)
                {
                    level.ParticlesFG.Emit(ParticleTypes.Dust,
                        Position + Calc.Random.Range(Vector2.One * -8, Vector2.One * 8));
                }
            }
        }
    }

    // Homing Skull
    public class HomingSkull : Entity
    {
        private Sprite sprite;
        private global::Celeste.Player target;
        private Vector2 velocity;
        private float speed;
        private float lifetime;

        public HomingSkull(Vector2 position, global::Celeste.Player target) : base(position)
        {
            this.target = target;
            speed = 120f;
            lifetime = 8f;
            
            sprite = new Sprite(GFX.Game, "objects/desolozantas/bones/");
            sprite.Add("float", "skull", 0.1f);
            sprite.Play("float");
            sprite.CenterOrigin();
            Add(sprite);
            
            Collider = new Circle(12f);
            Add(new PlayerCollider(OnPlayerHit));
            
            // Flame trail
            Add(new TrailParticles());
            
            Audio.Play("event:/char/desolozantas/skull_cackle", Position);
        }

        public override void Update()
        {
            base.Update();
            
            if (target != null && !target.Dead)
            {
                Vector2 targetDirection = (target.Position - Position).SafeNormalize();
                velocity = Vector2.Lerp(velocity, targetDirection * speed, 2f * Engine.DeltaTime);
            }
            
            Position += velocity * Engine.DeltaTime;
            
            // Rotate based on movement
            sprite.Rotation = velocity.Angle();
            
            lifetime -= Engine.DeltaTime;
            if (lifetime <= 0)
            {
                CreateExplosion();
                RemoveSelf();
            }
        }
        
        private void OnPlayerHit(global::Celeste.Player player)
        {
            CreateExplosion();
            player.Die((velocity).SafeNormalize());
            RemoveSelf();
        }
        
        private void CreateExplosion()
        {
            Audio.Play("event:/char/desolozantas/skull_explode", Position);
            
            for (int i = 0; i < 8; i++)
            {
                Vector2 direction = Vector2.One.Rotate(i * MathHelper.Pi / 4);
                Scene.Add(new BoneFragment(Position, direction * 100f));
            }
        }
    }

    // Bone Fragment
    public class BoneFragment : Entity
    {
        private Vector2 velocity;
        private float lifetime;
        private Sprite sprite;
        
        public BoneFragment(Vector2 position, Vector2 velocity) : base(position)
        {
            this.velocity = velocity;
            lifetime = 3f;
            
            sprite = new Sprite(GFX.Game, "objects/desolozantas/bones/");
            sprite.Add("spin", "fragment", 0.05f);
            sprite.Play("spin");
            sprite.CenterOrigin();
            Add(sprite);
            
            Collider = new Circle(4f);
            Add(new PlayerCollider(OnPlayerHit));
        }
        
        public override void Update()
        {
            base.Update();
            
            Position += velocity * Engine.DeltaTime;
            velocity.Y += 300f * Engine.DeltaTime; // Gravity
            velocity *= 0.99f; // Air resistance
            
            sprite.Rotation += velocity.Length() * 0.01f;
            
            lifetime -= Engine.DeltaTime;
            if (lifetime <= 0)
            {
                RemoveSelf();
            }
        }
        
        private void OnPlayerHit(global::Celeste.Player player)
        {
            player.Die((velocity).SafeNormalize());
            RemoveSelf();
        }
    }

    // Telegraph Warning effect
    public class TelegraphWarning : Entity
    {
        private float duration;
        private float timer;
        private Color color;
        private Circle area;
        
        public TelegraphWarning(Vector2 position, Color color, float duration) : base(position)
        {
            this.color = color;
            this.duration = duration;
            area = new Circle(24f);
        }
        
        public override void Update()
        {
            base.Update();
            
            timer += Engine.DeltaTime;
            if (timer >= duration)
            {
                RemoveSelf();
            }
        }
        
        public override void Render()
        {
            float alpha = 0.5f + 0.5f * (float)Math.Sin(timer * 10f); // Flashing
            Color renderColor = color * alpha;
            
            Draw.Circle(Position, area.Radius, renderColor, 3);
            Draw.Circle(Position, area.Radius * 0.5f, renderColor, 2);
        }
    }

    // Simple trail particles component
    public class TrailParticles : Component
    {
        private List<Vector2> trailPoints;
        private int maxPoints;
        
        public TrailParticles(int maxTrailPoints = 10) : base(true, false)
        {
            maxPoints = maxTrailPoints;
            trailPoints = new List<Vector2>();
        }
        
        public override void Update()
        {
            trailPoints.Add(Entity.Position);
            
            if (trailPoints.Count > maxPoints)
            {
                trailPoints.RemoveAt(0);
            }
        }
        
        public override void Render()
        {
            for (int i = 1; i < trailPoints.Count; i++)
            {
                float alpha = (float)i / trailPoints.Count;
                Draw.Line(trailPoints[i-1], trailPoints[i], Color.Red * alpha, 2f);
            }
        }
    }
    
    // Additional placeholder classes for other attacks - these would be implemented similarly
    public class BoneClubSwing : Entity 
    { 
        public BoneClubSwing(Vector2 position, Vector2 direction, float range) : base(position) { } 
    }
    
    public class FleshTentacle : Entity 
    { 
        public FleshTentacle(Vector2 position, Vector2 target) : base(position) { } 
    }
    
    public class BloodGeyser : Entity 
    { 
        public BloodGeyser(Vector2 position) : base(position) { } 
    }
    
    public class NerveElectricField : Entity 
    { 
        public NerveElectricField(Vector2 position, float radius) : base(position) { } 
    }
    
    public class RustyChainFlail : Entity 
    { 
        public RustyChainFlail(Vector2 position, global::Celeste.Player target, float range) : base(position) { } 
    }
    
    public class SerratedBlade : Entity 
    { 
        public SerratedBlade(Vector2 position, Vector2 velocity, float lifetime) : base(position) { } 
    }
    
    public class GiantBoneMace : Entity 
    { 
        public GiantBoneMace(Vector2 start, Vector2 target) : base(start) { } 
    }
    
    public class NightmareBeam : Entity 
    { 
        public NightmareBeam(Vector2 start, Vector2 target, float duration) : base(start) { } 
    }
    
    public class SoulCrushEffect : Entity 
    { 
        public SoulCrushEffect(Vector2 position) : base(position) { } 
    }
    
    public class RealityDistortionEffect : Entity 
    { 
        public RealityDistortionEffect() : base(Vector2.Zero) { } 
    }
    
    public class DespairAura : Entity 
    { 
        public DespairAura(Vector2 position, float radius) : base(position) { } 
    }

    // Arena background effects
    public class OrganicBackground : Entity
    {
        public OrganicBackground() : base(Vector2.Zero) { }
    }
    
    public class BoneCathedralBackground : Entity  
    {
        public BoneCathedralBackground() : base(Vector2.Zero) { }
    }
    
    public class FleshLabyrinthBackground : Entity
    {
        public FleshLabyrinthBackground() : base(Vector2.Zero) { }
    }
    
    public class NightmareBackground : Entity
    {
        public NightmareBackground() : base(Vector2.Zero) { }
    }

    // Arena elements
    public class HangingOrgan : ArenaElement
    {
        public HangingOrgan(Vector2 position, string organType) : base(position) { }
    }
    
    public class BonePillar : ArenaElement
    {
        public BonePillar(Vector2 position) : base(position) { }
    }
    
    public class FleshWall : ArenaElement
    {
        public FleshWall(Vector2 position) : base(position) { }
    }
    
    public class WeaponDisplay : ArenaElement
    {
        public WeaponDisplay(Vector2 position, string weaponType) : base(position) { }
    }

    // Effects
    public class UltimateArenaEffects : Entity
    {
        public UltimateArenaEffects() : base(Vector2.Zero) { }
    }
    
    public class DesperationEffects : Entity
    {
        public DesperationEffects() : base(Vector2.Zero) { }
    }

    // Damage zone for organic slam
    public class OrganicSlamZone : Entity
    {
        private float lifetime;
        private Circle damageArea;
        
        public OrganicSlamZone(Vector2 position) : base(position)
        {
            lifetime = 1f;
            damageArea = new Circle(48f);
            Collider = damageArea;
            Add(new PlayerCollider(OnPlayerHit));
        }
        
        public override void Update()
        {
            base.Update();
            
            lifetime -= Engine.DeltaTime;
            if (lifetime <= 0)
            {
                RemoveSelf();
            }
        }
        
        private void OnPlayerHit(global::Celeste.Player player)
        {
            player.Die(Vector2.UnitY);
        }
        
        public override void Render()
        {
            float alpha = lifetime;
            Draw.Circle(Position, damageArea.Radius, Color.Red * alpha * 0.3f, 16);
        }
    }
}



