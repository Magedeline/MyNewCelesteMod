using Celeste.Mod.DesoloZatnas.Core.UI;
using DesoloZantas.Core.Core.Entities;

namespace DesoloZantas.Core.Core
{
    public class PlayerDeadBody : Entity
    {
        public Action DeathAction;
        public float ActionDelay;
        public bool HasGolden;
        private Color initialHairColor;
        private Vector2 bounce = Vector2.Zero;
        private global::Celeste.Player player;
        private PlayerHair hair;
        private PlayerSprite sprite;
        private VertexLight light;
        private DeathEffect deathEffect;
        private Facings facing;
        private float scale = 1f;
        private bool finished;
        private Session session;

        public PlayerDeadBody(global::Celeste.Player player, Vector2 direction)
        {
            Depth = -1000000;
            this.player = player;
            facing = player.Facing;
            Position = player.Position;
            Add(hair = player.Hair);
            Add(sprite = (PlayerSprite)player.Sprite);
            Add(light = player.Light);
            sprite.Color = Color.White;
            initialHairColor = hair.Color;
            bounce = direction;
            
            // Store session for Flowey game over check
            if (player.Scene is Level level)
            {
                session = level.Session;
            }
            
            Add(new Coroutine(DeathRoutine()));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (!(bounce != Vector2.Zero))
                return;
            if (Math.Abs(bounce.X) > (double) Math.Abs(bounce.Y))
            {
                sprite.Play("deadside");
                facing = (Facings) (-Math.Sign(bounce.X));
            }
            else
            {
                bounce = Calc.AngleToVector(Calc.AngleApproach(bounce.Angle(), new Vector2(-(int) player.Facing, 0.0f).Angle(), 0.5f), 1f);
                if (bounce.Y < 0.0)
                    sprite.Play("deadup");
                else
                    sprite.Play("deaddown");
            }
        }

        private IEnumerator DeathRoutine()
        {
            PlayerDeadBody playerDeadBody1 = this;
            Level level = playerDeadBody1.SceneAs<Level>();
            
            // Check if in boss fight where player refuses to die
            if (level != null && ShouldRefuseToDie(level))
            {
                yield return RefuseToDieSequence(level);
                yield break;
            }
            
            if (playerDeadBody1.bounce != Vector2.Zero)
            {
                PlayerDeadBody playerDeadBody = playerDeadBody1;
                Audio.Play("event:/char/madeline/predeath", playerDeadBody1.Position);
                playerDeadBody1.scale = 1.5f;
                Celeste.Celeste.Freeze(0.05f);
                yield return null;
                Vector2 from = playerDeadBody1.Position;
                Vector2 to = from + playerDeadBody1.bounce * 24f;
                Tween tween = Tween.Create(Tween.TweenMode.Oneshot, Ease.CubeOut, 0.5f, true);
                playerDeadBody1.Add(tween);
                tween.OnUpdate = t =>
                {
                    playerDeadBody.Position = from + (to - from) * t.Eased;
                    playerDeadBody.scale = (float) (1.5 - t.Eased * 0.5);
                    playerDeadBody.sprite.Rotation = (float) (Math.Floor(t.Eased * 4.0) * 6.2831854820251465);
                };
                yield return (float) (tween.Duration * 0.75);
                tween.Stop();
                tween = null;
            }
            playerDeadBody1.Position += Vector2.UnitY * -5f;
            level.Displacement.AddBurst(playerDeadBody1.Position, 0.3f, 0.0f, 80f);
            level.Shake();
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Long);
            Audio.Play(playerDeadBody1.HasGolden ? "event:/new_content/char/madeline/death_golden" : "event:/char/madeline/death", playerDeadBody1.Position);
            playerDeadBody1.deathEffect = new DeathEffect(playerDeadBody1.initialHairColor, playerDeadBody1.Center - playerDeadBody1.Position);
            // ISSUE: reference to a compiler-generated method
            playerDeadBody1.deathEffect.OnUpdate = delegate (float f)
            {
                    light.Alpha = 1f - f;
            };
            playerDeadBody1.Add(playerDeadBody1.deathEffect);
            yield return (float) (playerDeadBody1.deathEffect.Duration * 0.64999997615814209);
            if (playerDeadBody1.ActionDelay > 0.0)
                yield return playerDeadBody1.ActionDelay;
            playerDeadBody1.End();
        }

        private void End()
        {
            if (finished)
                return;
            finished = true;
            Level level = SceneAs<Level>();
            
            // Check if Flowey should appear instead of normal reload
            if (session != null && FloweyGameOverTrigger.ShouldTriggerFlowey(session))
            {
                // Flowey takes over!
                level.DoScreenWipe(false, () => 
                {
                    Engine.Scene = new FloweyGameOver(level, session);
                });
                return;
            }
            
            // Normal death behavior
            if (DeathAction == null)
                DeathAction = level.Reload;
            level.DoScreenWipe(false, DeathAction);
        }

        public override void Update()
        {
            base.Update();
            if (Input.MenuConfirm.Pressed && !finished)
                End();
            hair.Color = sprite.CurrentAnimationFrame == 0 ? Color.White : initialHairColor;
        }

        public override void Render()
        {
            if (deathEffect == null)
            {
                sprite.Scale.X = (float) facing * scale;
                sprite.Scale.Y = scale;
                hair.Facing = facing;
                base.Render();
            }
            else
                deathEffect.Render();
        }
        
        /// <summary>
        /// Check if player should refuse to die in special boss fights
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
        /// Dramatic sequence where player refuses to die and gets back up
        /// </summary>
        private IEnumerator RefuseToDieSequence(Level level)
        {
            // Initial impact
            Audio.Play("event:/char/madeline/predeath", Position);
            level.Shake(0.3f);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            
            // Freeze for dramatic effect
            Celeste.Celeste.Freeze(0.1f);
            yield return 0.15f;
            
            // Play special "refuse to die" sound
            Audio.Play("event:/char/badeline/appear", Position);
            
            // Create determination flash effect
            level.Flash(Color.Red * 0.5f, true);
            level.Displacement.AddBurst(Position, 0.5f, 8f, 64f, 0.5f);
            
            // Show determination message
            int messageIndex = Calc.Random.Range(1, 6); // 1-5
            string messageKey = $"DESOLOZATNAS_REFUSE_DIE_MADELINE_{messageIndex}";
            string message = Dialog.Clean(messageKey);
            
            // Display message briefly
            if (!string.IsNullOrEmpty(message) && message != messageKey)
            {
                level.Add(new FloatingText(message, Position + Vector2.UnitY * -40f, Color.Red));
            }
            
            yield return 1.0f;
            
            // Create red determination particles
            for (int i = 0; i < 30; i++)
            {
                Vector2 particlePos = Position + new Vector2(
                    Calc.Random.Range(-20f, 20f), 
                    Calc.Random.Range(-20f, 20f)
                );
                level.ParticlesFG.Emit(ParticleTypes.Dust, particlePos, Color.Red);
            }
            
            // Respawn player at safe position
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
            
            // Dramatic recovery sound
            Audio.Play("event:/char/badeline/boss_bullet", Position);
            level.Displacement.AddBurst(Position, 0.5f, 8f, 80f);
            
            // Restore player to the scene with invulnerability
            // The player entity will be recreated on level reload, so we trigger a special respawn
            if (session != null)
            {
                session.RespawnPoint = respawnPos;
                level.Session.SetFlag("RefusedToDie", true);
            }
            
            // Flash the screen white
            level.Flash(Color.White, true);
            
            // Remove dead body and trigger respawn
            RemoveSelf();
            
            // Reload level at respawn point (with brief invulnerability handled by session flag)
            level.Reload();
        }
    }
}




