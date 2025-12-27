using DesoloZantas.Core.Core.Extensions;
using DesoloZantas.Core.Core.Settings;
using MonoMod.Utils;

namespace DesoloZantas.Core.Core.Player
{
    /// <summary>
    /// Kirby state machine callbacks implemented as extension methods on Player.
    /// This approach is inspired by Celeste-Aqua and provides better performance
    /// and cleaner code than static methods with scene tracker lookups.
    /// </summary>
    public static class KirbyStateExtensions
    {
        // Audio Events
        private const string SFX_FOOTSTEP = "event:/char/kirby/footstep";
        private const string SFX_DASH_CHARGE_3D = "event:/char/kirby/dash_charge_3d";
        private const string SFX_SPIT = "event:/Ingeste/char/kirby/spit";
        private const string SFX_INHALE_START = "event:/Ingeste/char/kirby/inhale_start";
        private const string SFX_INHALE_LOOP = "event:/Ingeste/char/kirby/inhale_loop";
        private const string SFX_CHARGE_DASH_ATTACK = "event:/Ingeste/char/kirby/charge_dash_attack";
        
        private static KirbySettings Settings => IngesteModule.Settings?.KirbySettings ?? new KirbySettings();
        
        #region Pre/Post Update Hooks
        
        /// <summary>
        /// Called before Player.Update when in Kirby mode
        /// </summary>
        public static void PreKirbyUpdate(this global::Celeste.Player self)
        {
            // Update candy invincibility
            if (self.IsKirbyCandyInvincible())
            {
                float timer = self.GetKirbyCandyTimer() - Engine.DeltaTime;
                self.SetKirbyCandyTimer(timer);
                
                if (timer <= 0)
                {
                    self.SetKirbyCandyInvincible(false);
                }
            }
        }
        
        /// <summary>
        /// Called after Player.Update when in Kirby mode
        /// </summary>
        public static void PostKirbyUpdate(this global::Celeste.Player self)
        {
            // Update Kirby sprite flip to match facing
            var kirby = self.GetKirbyComponent();
            if (kirby?.KirbySprite != null)
            {
                kirby.KirbySprite.Scale.X = Math.Abs(kirby.KirbySprite.Scale.X) * (int)self.Facing;
            }
        }
        
        #endregion
        
        #region Normal State
        
        public static void KirbyNormalBegin(this global::Celeste.Player self)
        {
            var kirby = self.GetKirbyComponent();
            kirby?.PlayAnimation("idle");
        }
        
        public static int KirbyNormalUpdate(this global::Celeste.Player self)
        {
            try
            {
                var kirby = self.GetKirbyComponent();
                if (kirby == null) return global::Celeste.Player.StNormal;
                
                var settings = Settings;
                float precisionMultiplier = settings.MovementPrecision;
                
                // Horizontal movement with enhanced precision
                float moveX = Input.MoveX.Value;
                if (moveX != 0)
                {
                    if (self.OnGround())
                    {
                        float targetSpeed = moveX * (90f * precisionMultiplier);
                        float acceleration = 1200f * precisionMultiplier * Engine.DeltaTime;
                        self.Speed.X = Calc.Approach(self.Speed.X, targetSpeed, acceleration);
                        
                        if (Math.Abs(self.Speed.X) > 10f)
                        {
                            self.Facing = (Facings)Math.Sign(self.Speed.X);
                            if (!kirby.GetCurrentAnimation().Equals("walk"))
                            {
                                kirby.PlayAnimation("walk");
                            }
                        }
                    }
                    else
                    {
                        // Enhanced air control
                        float airAccel = 800f * precisionMultiplier * Engine.DeltaTime;
                        float maxAirSpeed = 90f * precisionMultiplier;
                        
                        if (Math.Sign(self.Speed.X) == Math.Sign(moveX) || Math.Abs(self.Speed.X) < maxAirSpeed)
                        {
                            self.Speed.X = Calc.Approach(self.Speed.X, moveX * maxAirSpeed, airAccel);
                        }
                        
                        self.Facing = (Facings)Math.Sign(moveX);
                    }
                }
                else
                {
                    // Friction/deceleration
                    if (self.OnGround())
                    {
                        float friction = 1000f * precisionMultiplier * Engine.DeltaTime;
                        self.Speed.X = Calc.Approach(self.Speed.X, 0f, friction);
                        
                        if (Math.Abs(self.Speed.X) < 10f && !kirby.GetCurrentAnimation().Equals("idle"))
                        {
                            kirby.PlayAnimation("idle");
                        }
                    }
                    else
                    {
                        self.Speed.X = Calc.Approach(self.Speed.X, 0f, 300f * Engine.DeltaTime);
                    }
                }
                
                // Jump
                if (Input.Jump.Pressed && self.OnGround())
                {
                    self.Jump(false, true);
                    kirby.PlayAnimation("jump");
                }
                
                // Inhale
                if (settings.IsKeyPressed("Inhale"))
                {
                    return KirbyModeHooks.ST_KIRBY_INHALE;
                }
                
                // Wall Bounce
                if (settings.IsKeyPressed("Attack") && !self.OnGround() && 
                    self.CollideCheck<Solid>(self.Position + Vector2.UnitX * (int)self.Facing))
                {
                    return KirbyModeHooks.ST_KIRBY_WALL_BOUNCE;
                }
                
                // Ultra Wave Dash
                if (Input.MoveY.Value > 0.5f && self.OnGround() && Input.Dash.Pressed && Input.Jump.Check)
                {
                    return KirbyModeHooks.ST_KIRBY_ULTRA_WAVE_DASH;
                }
                
                // Charged Dash
                if (Input.Dash.Check && self.OnGround() && Input.MoveY.Value > 0.5f)
                {
                    return KirbyModeHooks.ST_KIRBY_CHARGED_DASH;
                }
                
                // Grab and Slam
                if (Input.MoveY.Value < -0.5f && Input.Dash.Pressed)
                {
                    return KirbyModeHooks.ST_KIRBY_GRAB_SLAM;
                }
                
                // Dash
                if (Input.Dash.Pressed)
                {
                    return KirbyModeHooks.ST_KIRBY_DASH;
                }
                
                // Attack/Parry
                if (settings.IsKeyPressed("Attack"))
                {
                    if (kirby.ShouldParry())
                    {
                        return KirbyModeHooks.ST_KIRBY_PARRY;
                    }
                    else
                    {
                        return KirbyModeHooks.ST_KIRBY_ATTACK;
                    }
                }
                
                // Elemental attack
                if (Input.Dash.Pressed && self.GetKirbyPowerState() != KirbyPlayerComponent.PowerState.None)
                {
                    kirby.PerformElementalAttack(self);
                }
                
                // Star warrior abilities
                if (settings.StarWarriorMode && self.GetKirbyPowerState() == KirbyPlayerComponent.PowerState.None)
                {
                    kirby.UpdateStarWarriorAbilities(self);
                }
                
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Error in KirbyNormalUpdate: {ex.Message}");
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
        }
        
        public static void KirbyNormalEnd(this global::Celeste.Player self)
        {
            // Cleanup when leaving normal state
        }
        
        #endregion
        
        #region Inhale State
        
        public static void KirbyInhaleBegin(this global::Celeste.Player self)
        {
            var kirby = self.GetKirbyComponent();
            if (kirby != null)
            {
                kirby.PlayAnimation("inhale");
                self.SetKirbyInhaleTimer(0f);
                Audio.Play(SFX_INHALE_START, self.Position);
            }
        }
        
        public static int KirbyInhaleUpdate(this global::Celeste.Player self)
        {
            try
            {
                var kirby = self.GetKirbyComponent();
                if (kirby == null) return global::Celeste.Player.StNormal;
                
                var settings = Settings;
                
                self.SetKirbyInhaling(true);
                float timer = self.GetKirbyInhaleTimer() + Engine.DeltaTime;
                self.SetKirbyInhaleTimer(timer);
                
                // Slower fall while inhaling
                self.Speed.Y = Math.Min(self.Speed.Y + 100f * Engine.DeltaTime, 40f);
                
                // Limited horizontal movement
                float moveX = Input.MoveX.Value;
                float precisionMultiplier = settings.MovementPrecision;
                
                if (moveX != 0)
                {
                    float inhaleAccel = 300f * precisionMultiplier * Engine.DeltaTime;
                    float maxInhaleSpeed = 50f * precisionMultiplier;
                    
                    self.Speed.X = Calc.Approach(self.Speed.X, moveX * maxInhaleSpeed, inhaleAccel);
                    self.Facing = (Facings)Math.Sign(moveX);
                }
                else
                {
                    self.Speed.X = Calc.Approach(self.Speed.X, 0f, 400f * Engine.DeltaTime);
                }
                
                // Perform inhale logic
                PerformInhaleLogic(self, kirby);
                
                // Exit conditions
                if (timer >= 1f || !settings.IsKeyCheck("Inhale"))
                {
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                if (Input.Dash.Pressed)
                {
                    return KirbyModeHooks.ST_KIRBY_DASH;
                }
                
                return KirbyModeHooks.ST_KIRBY_INHALE;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Error in KirbyInhaleUpdate: {ex.Message}");
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
        }
        
        public static void KirbyInhaleEnd(this global::Celeste.Player self)
        {
            self.SetKirbyInhaling(false);
            var kirby = self.GetKirbyComponent();
            if (kirby != null)
            {
                kirby.PlayAnimation("idle");
                if (self.GetKirbyInhaleTimer() > 0.2f)
                {
                    Audio.Play(SFX_SPIT, self.Position);
                }
            }
        }
        
        private static void PerformInhaleLogic(global::Celeste.Player self, KirbyPlayerComponent kirby)
        {
            if (!(self.Scene is Level level)) return;
            
            var settings = Settings;
            float inhaleRange = settings.InhaleRange;
            Vector2 inhaleDirection = new Vector2((int)self.Facing, 0);
            Vector2 inhaleCenter = self.Position + inhaleDirection * (inhaleRange / 2);
            
            level.ParticlesFG.Emit(ParticleTypes.Dust, 2, inhaleCenter, Vector2.One * 8f);
            
            var inhalableObjects = level.Tracker.GetEntities<Actor>()
                .Where(e => e != self && Vector2.Distance(e.Position, self.Position) <= inhaleRange)
                .Where(e => IsInhalable(e));
                
            foreach (var obj in inhalableObjects)
            {
                Vector2 pullDirection = (self.Position - obj.Position).SafeNormalize();
                
                if (obj is Actor actor)
                {
                    actor.Position += pullDirection * 60f * Engine.DeltaTime;
                    
                    if (Vector2.Distance(obj.Position, self.Position) < 16f)
                    {
                        InhaleObject(self, kirby, obj);
                    }
                }
            }
        }
        
        private static bool IsInhalable(Entity entity)
        {
            return entity.GetType().Name.Contains("Enemy") || 
                   entity.GetType().Name.Contains("Strawberry") ||
                   entity.GetType().Name.Contains("Key");
        }
        
        private static void InhaleObject(global::Celeste.Player self, KirbyPlayerComponent kirby, Entity obj)
        {
            var newPower = DeterminePowerFromObject(obj);
            
            if (newPower != KirbyPlayerComponent.PowerState.None)
            {
                self.SetKirbyPowerState(newPower);
            }
            
            obj.RemoveSelf();
            
            if (self.Scene is Level level)
            {
                level.ParticlesFG.Emit(ParticleTypes.Dust, 10, obj.Position, Vector2.One * 16f);
            }
        }
        
        private static KirbyPlayerComponent.PowerState DeterminePowerFromObject(Entity obj)
        {
            string objType = obj.GetType().Name.ToLower();
            
            if (objType.Contains("fire")) return KirbyPlayerComponent.PowerState.Fire;
            if (objType.Contains("ice")) return KirbyPlayerComponent.PowerState.Ice;
            if (objType.Contains("electric") || objType.Contains("spark")) return KirbyPlayerComponent.PowerState.Spark;
            if (objType.Contains("stone") || objType.Contains("rock")) return KirbyPlayerComponent.PowerState.Stone;
            if (objType.Contains("sword") || objType.Contains("blade")) return KirbyPlayerComponent.PowerState.Sword;
            
            return KirbyPlayerComponent.PowerState.None;
        }
        
        #endregion
        
        #region Dash State
        
        public static void KirbyDashBegin(this global::Celeste.Player self)
        {
            var kirby = self.GetKirbyComponent();
            if (kirby != null)
            {
                kirby.PlayAnimation("dash");
                self.SetKirbyDashTimer(0f);
                self.SetKirbyDashDirection(self.Facing);
            }
        }
        
        public static int KirbyDashUpdate(this global::Celeste.Player self)
        {
            try
            {
                var kirby = self.GetKirbyComponent();
                if (kirby == null) return global::Celeste.Player.StNormal;
                
                var settings = Settings;
                
                self.SetKirbyDashing(true);
                float timer = self.GetKirbyDashTimer() + Engine.DeltaTime;
                self.SetKirbyDashTimer(timer);
                
                float dashDuration = 0.15f;
                float dashSpeed = 240f * settings.MovementPrecision;
                
                if (timer < dashDuration)
                {
                    self.Speed.X = (int)self.GetKirbyDashDirection() * dashSpeed;
                    self.Speed.Y = 0f;
                }
                else
                {
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                // Wall collision
                if (self.CollideCheck<Solid>(self.Position + Vector2.UnitX * (int)self.GetKirbyDashDirection()))
                {
                    self.Speed.X = 0f;
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                // Attack during dash
                if (settings.IsKeyPressed("Attack"))
                {
                    if (kirby.ShouldParry())
                    {
                        return KirbyModeHooks.ST_KIRBY_PARRY;
                    }
                    else
                    {
                        return KirbyModeHooks.ST_KIRBY_ATTACK;
                    }
                }
                
                return KirbyModeHooks.ST_KIRBY_DASH;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Error in KirbyDashUpdate: {ex.Message}");
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
        }
        
        public static void KirbyDashEnd(this global::Celeste.Player self)
        {
            self.SetKirbyDashing(false);
        }
        
        #endregion
        
        #region Attack State
        
        public static void KirbyAttackBegin(this global::Celeste.Player self)
        {
            var kirby = self.GetKirbyComponent();
            if (kirby != null)
            {
                self.SetKirbyAttackTimer(0f);
                int combo = self.GetKirbyAttackCombo();
                string anim = combo == 0 ? "punch_a" : "punch_b";
                kirby.PlayAnimation(anim);
                self.SetKirbyAttackCombo((combo + 1) % 2);
            }
        }
        
        public static int KirbyAttackUpdate(this global::Celeste.Player self)
        {
            try
            {
                float timer = self.GetKirbyAttackTimer() + Engine.DeltaTime;
                self.SetKirbyAttackTimer(timer);
                
                if (timer >= 0.3f)
                {
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                // Limited movement during attack
                float moveX = Input.MoveX.Value * 0.3f;
                self.Speed.X = Calc.Approach(self.Speed.X, moveX * 30f, 300f * Engine.DeltaTime);
                
                return KirbyModeHooks.ST_KIRBY_ATTACK;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Error in KirbyAttackUpdate: {ex.Message}");
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
        }
        
        public static void KirbyAttackEnd(this global::Celeste.Player self)
        {
            // Attack cleanup
        }
        
        #endregion
        
        #region Parry State
        
        public static void KirbyParryBegin(this global::Celeste.Player self)
        {
            var kirby = self.GetKirbyComponent();
            if (kirby != null)
            {
                self.SetKirbyParryTimer(0f);
                self.SetKirbyParrying(true);
                self.SetKirbyParrySuccessful(false);
                kirby.PlayAnimation("parry");
            }
        }
        
        public static int KirbyParryUpdate(this global::Celeste.Player self)
        {
            try
            {
                float timer = self.GetKirbyParryTimer() + Engine.DeltaTime;
                self.SetKirbyParryTimer(timer);
                
                float parryWindow = 0.2f;
                float parryDuration = 0.4f;
                
                // Check for successful parry during window
                if (timer < parryWindow && !self.GetKirbyParrySuccessful())
                {
                    if (CheckParryCollision(self))
                    {
                        self.SetKirbyParrySuccessful(true);
                        Audio.Play("event:/game/general/platform_disintegrate", self.Position);
                        
                        // Time slow effect
                        if (self.Scene is Level level)
                        {
                            level.Shake(0.1f);
                        }
                    }
                }
                
                if (timer >= parryDuration)
                {
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                // Minimal movement during parry
                self.Speed.X = Calc.Approach(self.Speed.X, 0f, 600f * Engine.DeltaTime);
                
                return KirbyModeHooks.ST_KIRBY_PARRY;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Error in KirbyParryUpdate: {ex.Message}");
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
        }
        
        public static void KirbyParryEnd(this global::Celeste.Player self)
        {
            self.SetKirbyParrying(false);
        }
        
        private static bool CheckParryCollision(global::Celeste.Player self)
        {
            if (!(self.Scene is Level level)) return false;
            
            var parryHitbox = new Rectangle(
                (int)(self.Position.X - 12), 
                (int)(self.Position.Y - 16), 
                24, 32);
            
            foreach (var entity in level.Entities)
            {
                if (entity.GetType().Name.Contains("Projectile") ||
                    entity.GetType().Name.Contains("Spinner") ||
                    entity.GetType().Name.Contains("Seeker"))
                {
                    if (entity.Collider != null && entity.Collider.Collide(parryHitbox))
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        #endregion
        
        #region Wall Bounce State
        
        public static void KirbyWallBounceBegin(this global::Celeste.Player self)
        {
            var kirby = self.GetKirbyComponent();
            if (kirby != null)
            {
                self.SetKirbyWallBounceTimer(0f);
                kirby.PlayAnimation("wall_kick");
                Audio.Play("event:/char/madeline/climb_move", self.Position);
            }
        }
        
        public static int KirbyWallBounceUpdate(this global::Celeste.Player self)
        {
            try
            {
                float timer = self.GetKirbyWallBounceTimer() + Engine.DeltaTime;
                self.SetKirbyWallBounceTimer(timer);
                
                if (timer < 0.1f)
                {
                    // Wall bounce - reverse direction with speed boost
                    self.Speed.X = -(int)self.Facing * 200f;
                    self.Speed.Y = -150f;
                    self.Facing = (Facings)(-(int)self.Facing);
                }
                
                if (timer >= 0.15f || self.OnGround())
                {
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                return KirbyModeHooks.ST_KIRBY_WALL_BOUNCE;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Error in KirbyWallBounceUpdate: {ex.Message}");
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
        }
        
        public static void KirbyWallBounceEnd(this global::Celeste.Player self)
        {
            // Wall bounce cleanup
        }
        
        #endregion
        
        #region Ultra Wave Dash State
        
        public static void KirbyUltraWaveDashBegin(this global::Celeste.Player self)
        {
            var kirby = self.GetKirbyComponent();
            if (kirby != null)
            {
                int count = self.GetKirbyWaveDashCount() + 1;
                self.SetKirbyWaveDashCount(count);
                self.SetKirbyDashTimer(0f);
                kirby.PlayAnimation("slide");
                Audio.Play("event:/char/madeline/dash_red_right", self.Position);
            }
        }
        
        public static int KirbyUltraWaveDashUpdate(this global::Celeste.Player self)
        {
            try
            {
                float timer = self.GetKirbyDashTimer() + Engine.DeltaTime;
                self.SetKirbyDashTimer(timer);
                
                var settings = Settings;
                int waveDashCount = self.GetKirbyWaveDashCount();
                float speedMultiplier = 1f + (waveDashCount * 0.15f);
                float baseSpeed = 280f * settings.MovementPrecision;
                
                if (timer < 0.2f)
                {
                    self.Speed.X = (int)self.Facing * baseSpeed * speedMultiplier;
                    self.Speed.Y = 0f;
                }
                else
                {
                    self.SetKirbyWaveDashCount(0);
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                // Chain into another wave dash
                if (Input.Dash.Pressed && Input.Jump.Check && timer > 0.1f)
                {
                    self.SetKirbyDashTimer(0f);
                    return KirbyModeHooks.ST_KIRBY_ULTRA_WAVE_DASH;
                }
                
                if (!self.OnGround())
                {
                    self.SetKirbyWaveDashCount(0);
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                return KirbyModeHooks.ST_KIRBY_ULTRA_WAVE_DASH;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Error in KirbyUltraWaveDashUpdate: {ex.Message}");
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
        }
        
        public static void KirbyUltraWaveDashEnd(this global::Celeste.Player self)
        {
            // Wave dash cleanup
        }
        
        #endregion
        
        #region Charged Dash State
        
        public static void KirbyChargedDashBegin(this global::Celeste.Player self)
        {
            var kirby = self.GetKirbyComponent();
            if (kirby != null)
            {
                self.SetKirbyChargeTime(0f);
                self.SetKirbyChargedDashDirection(new Vector2((int)self.Facing, 0));
                self.Speed = Vector2.Zero;
                kirby.PlayAnimation("crouch");
                Audio.Play(SFX_CHARGE_DASH_ATTACK, self.Position);
            }
        }
        
        public static int KirbyChargedDashUpdate(this global::Celeste.Player self)
        {
            try
            {
                var kirby = self.GetKirbyComponent();
                if (kirby == null) return global::Celeste.Player.StNormal;
                
                float chargeTime = self.GetKirbyChargeTime() + Engine.DeltaTime;
                self.SetKirbyChargeTime(chargeTime);
                
                // Charge effect
                if (chargeTime > 0.5f)
                {
                    self.Sprite.Scale = Vector2.One * (1f + (chargeTime - 0.5f) * 0.2f);
                }
                
                // Update direction based on input
                Vector2 dir = new Vector2(Input.MoveX.Value, Input.MoveY.Value);
                if (dir != Vector2.Zero)
                {
                    self.SetKirbyChargedDashDirection(dir.SafeNormalize());
                }
                
                // Release dash
                if (!Input.Dash.Check)
                {
                    Vector2 dashDir = self.GetKirbyChargedDashDirection();
                    float power = Math.Min(chargeTime, 2f);
                    float speed = 200f + power * 150f;
                    
                    self.Speed = dashDir * speed;
                    
                    if (power >= 1f)
                    {
                        kirby.PlayAnimation("spin");
                        BreakEntitiesInPath(self);
                    }
                    else
                    {
                        kirby.PlayAnimation("dash");
                    }
                    
                    self.Sprite.Scale = Vector2.One;
                    Audio.Play("event:/char/madeline/dash_red_right", self.Position);
                    
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                return KirbyModeHooks.ST_KIRBY_CHARGED_DASH;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Error in KirbyChargedDashUpdate: {ex.Message}");
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
        }
        
        public static void KirbyChargedDashEnd(this global::Celeste.Player self)
        {
            self.SetKirbyChargeTime(0f);
            self.Sprite.Scale = Vector2.One;
        }
        
        private static void BreakEntitiesInPath(global::Celeste.Player self)
        {
            if (!(self.Scene is Level level)) return;
            
            Vector2 checkPos = self.Position + self.Speed.SafeNormalize() * 16f;
            Rectangle hitbox = new Rectangle((int)checkPos.X - 16, (int)checkPos.Y - 16, 32, 32);
            
            foreach (var entity in level.Entities)
            {
                if (entity.Collider != null && entity.Collider.Collide(hitbox))
                {
                    if (entity is DashBlock dashBlock)
                    {
                        dashBlock.Break(self.Center, self.Speed.SafeNormalize(), true, true);
                    }
                    else if (entity.GetType().Name.Contains("Spinner"))
                    {
                        entity.RemoveSelf();
                        Audio.Play("event:/game/general/diamond_touch", entity.Position);
                    }
                }
            }
        }
        
        #endregion
        
        #region Grab Slam State
        
        public static void KirbyGrabSlamBegin(this global::Celeste.Player self)
        {
            var kirby = self.GetKirbyComponent();
            if (kirby != null && self.Scene is Level level)
            {
                self.SetKirbyGrabSlamTimer(0f);
                
                // Try to grab nearby entity
                foreach (var entity in level.Entities)
                {
                    if (entity != self && Vector2.Distance(entity.Position, self.Position) < 24f)
                    {
                        if (!(entity is global::Celeste.Player) && !(entity is Solid))
                        {
                            self.SetKirbyGrabbedEntity(entity);
                            break;
                        }
                    }
                }
                
                kirby.PlayAnimation("grab_enemy");
                Audio.Play("event:/char/madeline/grab", self.Position);
            }
        }
        
        public static int KirbyGrabSlamUpdate(this global::Celeste.Player self)
        {
            try
            {
                float timer = self.GetKirbyGrabSlamTimer() + Engine.DeltaTime;
                self.SetKirbyGrabSlamTimer(timer);
                
                var grabbedEntity = self.GetKirbyGrabbedEntity();
                
                // Slam
                if (Input.MoveY.Value > 0.5f && Input.Dash.Pressed)
                {
                    self.Speed.Y = 400f;
                    
                    if (grabbedEntity != null)
                    {
                        grabbedEntity.Position = self.Position + Vector2.UnitY * 8f;
                    }
                }
                
                // Impact
                if (self.OnGround())
                {
                    if (grabbedEntity != null && self.Scene is Level level)
                    {
                        level.Shake(0.3f);
                        Audio.Play("event:/game/general/thing_booped", self.Position);
                        grabbedEntity.RemoveSelf();
                        self.SetKirbyGrabbedEntity(null);
                        BreakEntitiesInRadius(self, 32f);
                    }
                    
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                // Cancel
                if (Input.Jump.Pressed)
                {
                    if (grabbedEntity != null)
                    {
                        grabbedEntity.RemoveSelf();
                        self.SetKirbyGrabbedEntity(null);
                    }
                    return KirbyModeHooks.ST_KIRBY_NORMAL;
                }
                
                return KirbyModeHooks.ST_KIRBY_GRAB_SLAM;
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Error in KirbyGrabSlamUpdate: {ex.Message}");
                return KirbyModeHooks.ST_KIRBY_NORMAL;
            }
        }
        
        public static void KirbyGrabSlamEnd(this global::Celeste.Player self)
        {
            self.SetKirbyGrabbedEntity(null);
        }
        
        private static void BreakEntitiesInRadius(global::Celeste.Player self, float radius)
        {
            if (!(self.Scene is Level level)) return;
            
            foreach (var entity in level.Entities)
            {
                if (Vector2.Distance(entity.Position, self.Position) <= radius)
                {
                    if (entity is DashBlock dashBlock)
                    {
                        dashBlock.Break(self.Center, Vector2.UnitY, true, true);
                    }
                    else if (entity.GetType().Name.Contains("Spinner"))
                    {
                        entity.RemoveSelf();
                        Audio.Play("event:/game/general/diamond_touch", entity.Position);
                    }
                    else if (entity.GetType().Name.Contains("CrumbleBlock"))
                    {
                        entity.RemoveSelf();
                        level.ParticlesFG.Emit(ParticleTypes.Dust, 8, entity.Position, Vector2.One * 4f);
                    }
                }
            }
        }
        
        #endregion
    }
}
