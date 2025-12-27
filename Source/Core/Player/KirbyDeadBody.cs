using DesoloZantas.Core.Core.Extensions;
using Celeste.Mod.DesoloZatnas.Core.UI;
using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Kirby death body with custom death animations and effects
    /// </summary>
    public class KirbyDeadBody : Entity
    {
        public Action DeathAction;
        public float ActionDelay;
        public bool HasGolden;
        
        private Color initialHairColor;
        private Color initialShoeColor;
        private Vector2 bounce = Vector2.Zero;
        private global::Celeste.Player player;
        private KirbyHair hair;
        private KirbyPlayerSprite sprite;
        private VertexLight light;
        private DeathEffect deathEffect;
        private Facings facing;
        private float scale = 1f;
        private bool finished;
        private string lastAbility = "None";
        private Session session;

        public KirbyDeadBody(global::Celeste.Player player, Vector2 direction)
        {
            base.Depth = -1000000;
            this.player = player;
            facing = player.Facing;
            Position = player.Position;
            
            // Store session for Flowey game over check
            if (player.Scene is Level level)
            {
                session = level.Session;
            }
            
            // Get Kirby components if they exist
            var kirbyComponent = player.Get<KirbyPlayerComponent>();
            if (kirbyComponent != null)
            {
                // Store last ability for visual effects
                lastAbility = kirbyComponent.CurrentPower.ToString();
            }
            
            // Create Kirby sprite and hair
            sprite = new KirbyPlayerSprite(PlayerSpriteMode.Kirby);
            hair = new KirbyHair(sprite);
            Add(hair);
            Add(sprite);
            
            // Get initial shoe color if Kirby hair exists
            var existingKirbyHair = player.Get<KirbyHair>();
            initialShoeColor = existingKirbyHair != null ? existingKirbyHair.ShoeColor : new Color(100, 100, 255);
            
            Add(light = player.Light);
            sprite.Color = Color.White;
            initialHairColor = hair.Color;
            bounce = direction;
            Add(new Coroutine(DeathRoutine()));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            
            if (!(bounce != Vector2.Zero))
            {
                return;
            }
            
            if (Math.Abs(bounce.X) > Math.Abs(bounce.Y))
            {
                // Use custom Kirby death animation
                sprite.Play(sprite.Has(KirbyPlayerSprite.PreDeath) ? KirbyPlayerSprite.PreDeath : "deadside");
                facing = (Facings)(-Math.Sign(bounce.X));
            }
            else
            {
                bounce = Calc.AngleToVector(Calc.AngleApproach(bounce.Angle(), new Vector2(0 - (int)player.Facing, 0f).Angle(), 0.5f), 1f);
                if (bounce.Y < 0f)
                {
                    sprite.Play(sprite.Has(KirbyPlayerSprite.PreDeath) ? KirbyPlayerSprite.PreDeath : "deadup");
                }
                else
                {
                    sprite.Play(sprite.Has(KirbyPlayerSprite.PreDeath) ? KirbyPlayerSprite.PreDeath : "deaddown");
                }
            }
        }

        private IEnumerator DeathRoutine()
        {
            Level level = SceneAs<Level>();
            
            // Check if in boss fight where Kirby refuses to die
            if (level != null && ShouldRefuseToDie(level))
            {
                yield return RefuseToDieSequence(level);
                yield break;
            }
            
            // Play pre-death animation if available
            if (sprite.Has(KirbyPlayerSprite.PreDeath))
            {
                sprite.Play(KirbyPlayerSprite.PreDeath);
                // Wait for animation to complete (10 frames at 0.08 delay = 0.8 seconds)
                yield return 0.8f;
            }
            
            if (bounce != Vector2.Zero)
            {
                // Kirby-specific death sound
                Audio.Play("event:/char/madeline/predeath", Position);
                scale = 1.5f;
                Celeste.Celeste.Freeze(0.05f);
                yield return null;
                
                // Play mid-death animation during bounce
                if (sprite.Has(KirbyPlayerSprite.MidDeath))
                {
                    sprite.Play(KirbyPlayerSprite.MidDeath);
                }
                
                Vector2 from = Position;
                Vector2 to = from + bounce * 24f;
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, start: true);
                Add(tween);
                
                tween.OnUpdate = (Tween t) =>
                {
                    Position = from + (to - from) * t.Eased;
                    scale = 1.5f - t.Eased * 0.5f;
                    sprite.Rotation = (float)(Math.Floor(t.Eased * 4f) * 6.2831854820251465);
                    
                    // Fade shoe color as Kirby spins
                    hair.ShoeColor = Color.Lerp(initialShoeColor, Color.White, t.Eased * 0.5f);
                };
                
                yield return tween.Duration * 0.75f;
                tween.Stop();
            }
            
            Position += Vector2.UnitY * -5f;
            level.Displacement.AddBurst(Position, 0.3f, 0f, 80f);
            level.Shake();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            
            // Custom death sound for Kirby (or golden berry)
            string deathSound = HasGolden 
                ? "event:/new_content/char/madeline/death_golden" 
                : "event:/char/madeline/death";
            Audio.Play(deathSound, Position);
            
            // Play post-death animation if available
            if (sprite.Has(KirbyPlayerSprite.PostDeath))
            {
                sprite.Play(KirbyPlayerSprite.PostDeath);
            }
            
            // Create custom death effect with ability color if applicable
            Color deathEffectColor = lastAbility != "None" 
                ? sprite.GetAbilityColor() 
                : initialHairColor;
            
            deathEffect = new DeathEffect(deathEffectColor, Center - Position);
            deathEffect.OnUpdate = (float f) =>
            {
                light.Alpha = 1f - f;
                hair.Alpha = 1f - f;
            };
            
            Add(deathEffect);
            
            // Add ability-specific particles
            if (lastAbility != "None")
            {
                AddAbilityDeathParticles(level, lastAbility);
            }
            
            yield return deathEffect.Duration * 0.65f;
            
            if (ActionDelay > 0f)
            {
                yield return ActionDelay;
            }
            
            End();
        }

        /// <summary>
        /// Add custom particles based on the last copy ability
        /// </summary>
        private void AddAbilityDeathParticles(Level level, string ability)
        {
            Color particleColor = sprite.GetAbilityColor();
            
            for (int i = 0; i < 10; i++)
            {
                Vector2 particlePos = Position + new Vector2(
                    Calc.Random.Range(-8f, 8f), 
                    Calc.Random.Range(-8f, 8f)
                );
                
                level.ParticlesFG.Emit(ParticleTypes.Dust, particlePos, particleColor);
            }
        }

        private void End()
        {
            if (!finished)
            {
                finished = true;
                Level level = SceneAs<Level>();
                
                // Check if Flowey should appear instead of normal reload
                if (session != null && FloweyGameOverTrigger.ShouldTriggerFlowey(session))
                {
                    // Flowey takes over!
                    level.DoScreenWipe(wipeIn: false, () => 
                    {
                        Engine.Scene = new FloweyGameOver(level, session);
                    });
                    return;
                }
                
                // Normal death behavior
                if (DeathAction == null)
                {
                    DeathAction = level.Reload;
                }
                
                level.DoScreenWipe(wipeIn: false, DeathAction);
            }
        }

        public override void Update()
        {
            base.Update();
            
            if (Input.MenuConfirm.Pressed && !finished)
            {
                End();
            }
            
            // Update hair and shoe colors during death animation
            if (deathEffect == null)
            {
                hair.Color = (sprite.CurrentAnimationFrame == 0) ? Color.White : initialHairColor;
                
                // Pulse shoe color slightly
                float pulse = (float)Math.Sin(Scene.TimeActive * 10f) * 0.1f + 0.9f;
                hair.ShoeColor = initialShoeColor * pulse;
            }
        }

        public override void Render()
        {
            if (deathEffect == null)
            {
                sprite.Scale.X = (float)facing * scale;
                sprite.Scale.Y = scale;
                hair.Facing = facing;
                base.Render();
            }
            else
            {
                deathEffect.Render();
            }
        }
        
        /// <summary>
        /// Check if Kirby should refuse to die in special boss fights
        /// </summary>
        private bool ShouldRefuseToDie(Level level)
        {
            // Check for AsrielGodBoss (Angel of Death)
            var asrielBoss = level.Tracker.GetEntity<Entities.AsrielGodBoss>();
            if (asrielBoss != null)
                return true;
            
            // Check for ElsTrueFinalBoss
            var elsBoss = level.Tracker.GetEntity<Entities.ElsTrueFinalBoss>();
            if (elsBoss != null)
                return true;
            
            return false;
        }
        
        /// <summary>
        /// Dramatic sequence where Kirby refuses to die and gets back up with determination
        /// </summary>
        private IEnumerator RefuseToDieSequence(Level level)
        {
            // Initial impact - Kirby style
            Audio.Play("event:/char/madeline/predeath", Position);
            level.Shake(0.3f);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            
            // Freeze for dramatic effect
            Celeste.Celeste.Freeze(0.1f);
            yield return 0.15f;
            
            // Play special "refuse to die" sound (Kirby determination)
            Audio.Play("event:/char/badeline/appear", Position);
            
            // Create pink determination flash effect (Kirby's color)
            level.Flash(Color.Pink * 0.5f, true);
            level.Displacement.AddBurst(Position, 0.5f, 8f, 64f, 0.5f);
            
            // Show Kirby determination message
            int messageIndex = Calc.Random.Range(1, 6); // 1-5
            string messageKey = $"DESOLOZATNAS_REFUSE_DIE_KIRBY_{messageIndex}";
            string message = Dialog.Clean(messageKey);
            
            // Display message briefly
            if (!string.IsNullOrEmpty(message) && message != messageKey)
            {
                level.Add(new FloatingText(message, Position + Vector2.UnitY * -40f, Color.Pink));
            }
            
            yield return 1.0f;
            
            // Kirby-themed particles - pink and sparkly
            for (int i = 0; i < 40; i++)
            {
                Vector2 particlePos = Position + new Vector2(
                    Calc.Random.Range(-20f, 20f), 
                    Calc.Random.Range(-20f, 20f)
                );
                Color particleColor = i % 2 == 0 ? Color.Pink : Color.LightPink;
                level.ParticlesFG.Emit(ParticleTypes.Dust, particlePos, particleColor);
            }
            
            // Respawn Kirby at safe position
            var respawnPos = Position;
            
            // Find safe ground if possible
            for (float yOffset = 0; yOffset < 64f; yOffset += 8f)
            {
                if (level.CollideCheck<Solid>(respawnPos + Vector2.UnitY * yOffset))
                {
                    respawnPos = Position + Vector2.UnitY * (yOffset - 8f);
                    break;
                }
            }
            
            // Dramatic recovery sound and effects
            Audio.Play("event:/char/badeline/boss_bullet", Position);
            level.Displacement.AddBurst(Position, 0.5f, 8f, 80f);
            
            // Final sparkle burst - Kirby signature pink
            for (int i = 0; i < 20; i++)
            {
                Vector2 particlePos = Position + new Vector2(
                    Calc.Random.Range(-15f, 15f), 
                    Calc.Random.Range(-15f, 15f)
                );
                level.ParticlesFG.Emit(ParticleTypes.Dust, particlePos, Color.HotPink);
            }
            
            // Restore Kirby to the scene with invulnerability
            if (session != null)
            {
                session.RespawnPoint = respawnPos;
                level.Session.SetFlag("RefusedToDie_Kirby", true);
            }
            
            // Flash the screen with Kirby's pink color
            level.Flash(Color.Pink, true);
            
            // Remove dead body and trigger respawn
            RemoveSelf();
            
            // Reload level at respawn point
            level.Reload();
        }
    }
}




