namespace DesoloZantas.Core.Core.VanillaCore
{
    /// <summary>
    /// Base Actor class for all physical entities that can move and collide.
    /// Ported from vanilla Celeste.
    /// </summary>
    [Tracked(true)]
    public class Actor : Entity
    {
        public Collision SquishCallback;
        public bool TreatNaive;
        private Vector2 movementCounter;
        public bool IgnoreJumpThrus;
        public bool AllowPushing = true;
        public float LiftSpeedGraceTime = 0.16f;
        private Vector2 currentLiftSpeed;
        private Vector2 lastLiftSpeed;
        private float liftSpeedTimer;

        public Actor(Vector2 position) : base(position)
        {
            SquishCallback = OnSquish;
        }

        protected virtual void OnSquish(CollisionData data)
        {
            if (TrySquishWiggle(data))
                return;
            RemoveSelf();
        }

        protected bool TrySquishWiggle(CollisionData data, int wiggleX = 3, int wiggleY = 3)
        {
            data.Pusher.Collidable = true;
            
            // First pass: try to find valid position at current location
            for (int x = 0; x <= wiggleX; x++)
            {
                for (int y = 0; y <= wiggleY; y++)
                {
                    if (x != 0 || y != 0)
                    {
                        for (int dirX = 1; dirX >= -1; dirX -= 2)
                        {
                            for (int dirY = 1; dirY >= -1; dirY -= 2)
                            {
                                Vector2 offset = new Vector2(x * dirX, y * dirY);
                                if (!CollideCheck<Solid>(Position + offset))
                                {
                                    Position += offset;
                                    data.Pusher.Collidable = false;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            
            // Second pass: try to find valid position at target
            for (int x = 0; x <= wiggleX; x++)
            {
                for (int y = 0; y <= wiggleY; y++)
                {
                    if (x != 0 || y != 0)
                    {
                        for (int dirX = 1; dirX >= -1; dirX -= 2)
                        {
                            for (int dirY = 1; dirY >= -1; dirY -= 2)
                            {
                                Vector2 offset = new Vector2(x * dirX, y * dirY);
                                if (!CollideCheck<Solid>(data.TargetPosition + offset))
                                {
                                    Position = data.TargetPosition + offset;
                                    data.Pusher.Collidable = false;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            
            data.Pusher.Collidable = false;
            return false;
        }

        public virtual bool IsRiding(JumpThru jumpThru) => 
            !IgnoreJumpThrus && CollideCheckOutside(jumpThru, Position + Vector2.UnitY);

        public virtual bool IsRiding(Solid solid) => 
            CollideCheck(solid, Position + Vector2.UnitY);

        public bool OnGround(int downCheck = 1)
        {
            if (CollideCheck<Solid>(Position + Vector2.UnitY * downCheck))
                return true;
            return !IgnoreJumpThrus && CollideCheckOutside<JumpThru>(Position + Vector2.UnitY * downCheck);
        }

        public bool OnGround(Vector2 at, int downCheck = 1)
        {
            Vector2 originalPos = Position;
            Position = at;
            bool result = OnGround(downCheck);
            Position = originalPos;
            return result;
        }

        public Vector2 ExactPosition => Position + movementCounter;
        public Vector2 PositionRemainder => movementCounter;

        public void ZeroRemainderX() => movementCounter.X = 0f;
        public void ZeroRemainderY() => movementCounter.Y = 0f;

        public override void Update()
        {
            base.Update();
            LiftSpeed = Vector2.Zero;
            
            if (liftSpeedTimer > 0f)
            {
                liftSpeedTimer -= Engine.DeltaTime;
                if (liftSpeedTimer <= 0f)
                    lastLiftSpeed = Vector2.Zero;
            }
        }

        public Vector2 LiftSpeed
        {
            get => currentLiftSpeed == Vector2.Zero ? lastLiftSpeed : currentLiftSpeed;
            set
            {
                currentLiftSpeed = value;
                if (value != Vector2.Zero && LiftSpeedGraceTime > 0f)
                {
                    lastLiftSpeed = value;
                    liftSpeedTimer = LiftSpeedGraceTime;
                }
            }
        }

        public void ResetLiftSpeed()
        {
            currentLiftSpeed = lastLiftSpeed = Vector2.Zero;
            liftSpeedTimer = 0f;
        }

        public bool MoveH(float moveH, Collision onCollide = null, Solid pusher = null)
        {
            movementCounter.X += moveH;
            int moveAmount = (int)Math.Round(movementCounter.X, MidpointRounding.ToEven);
            if (moveAmount == 0)
                return false;
            movementCounter.X -= moveAmount;
            return MoveHExact(moveAmount, onCollide, pusher);
        }

        public bool MoveV(float moveV, Collision onCollide = null, Solid pusher = null)
        {
            movementCounter.Y += moveV;
            int moveAmount = (int)Math.Round(movementCounter.Y, MidpointRounding.ToEven);
            if (moveAmount == 0)
                return false;
            movementCounter.Y -= moveAmount;
            return MoveVExact(moveAmount, onCollide, pusher);
        }

        public bool MoveHExact(int moveH, Collision onCollide = null, Solid pusher = null)
        {
            Vector2 targetPosition = Position + Vector2.UnitX * moveH;
            int sign = Math.Sign(moveH);
            int moved = 0;
            
            while (moveH != 0)
            {
                Solid solid = CollideFirst<Solid>(Position + Vector2.UnitX * sign);
                if (solid != null)
                {
                    movementCounter.X = 0f;
                    onCollide?.Invoke(new CollisionData
                    {
                        Direction = Vector2.UnitX * sign,
                        Moved = Vector2.UnitX * moved,
                        TargetPosition = targetPosition,
                        Hit = solid,
                        Pusher = pusher
                    });
                    return true;
                }
                moved += sign;
                moveH -= sign;
                X += sign;
            }
            return false;
        }

        public bool MoveVExact(int moveV, Collision onCollide = null, Solid pusher = null)
        {
            Vector2 targetPosition = Position + Vector2.UnitY * moveV;
            int sign = Math.Sign(moveV);
            int moved = 0;
            
            while (moveV != 0)
            {
                Platform platform = CollideFirst<Solid>(Position + Vector2.UnitY * sign);
                if (platform != null)
                {
                    movementCounter.Y = 0f;
                    onCollide?.Invoke(new CollisionData
                    {
                        Direction = Vector2.UnitY * sign,
                        Moved = Vector2.UnitY * moved,
                        TargetPosition = targetPosition,
                        Hit = platform,
                        Pusher = pusher
                    });
                    return true;
                }
                
                if (moveV > 0 && !IgnoreJumpThrus)
                {
                    Platform jumpThru = CollideFirstOutside<JumpThru>(Position + Vector2.UnitY * sign);
                    if (jumpThru != null)
                    {
                        movementCounter.Y = 0f;
                        onCollide?.Invoke(new CollisionData
                        {
                            Direction = Vector2.UnitY * sign,
                            Moved = Vector2.UnitY * moved,
                            TargetPosition = targetPosition,
                            Hit = jumpThru,
                            Pusher = pusher
                        });
                        return true;
                    }
                }
                
                moved += sign;
                moveV -= sign;
                Y += sign;
            }
            return false;
        }

        public void MoveTowardsX(float targetX, float maxAmount, Collision onCollide = null) => 
            MoveToX(Calc.Approach(ExactPosition.X, targetX, maxAmount), onCollide);

        public void MoveTowardsY(float targetY, float maxAmount, Collision onCollide = null) => 
            MoveToY(Calc.Approach(ExactPosition.Y, targetY, maxAmount), onCollide);

        public void MoveToX(float toX, Collision onCollide = null) => 
            MoveH(toX - ExactPosition.X, onCollide);

        public void MoveToY(float toY, Collision onCollide = null) => 
            MoveV(toY - ExactPosition.Y, onCollide);

        public void NaiveMove(Vector2 amount)
        {
            movementCounter += amount;
            int x = (int)Math.Round(movementCounter.X);
            int y = (int)Math.Round(movementCounter.Y);
            Position += new Vector2(x, y);
            movementCounter -= new Vector2(x, y);
        }
    }
}
