namespace DesoloZantas.Core.BossesHelper.Entities
{
	public abstract class BossActor : BossEntity
	{
		public Vector2 Speed;

		private const float Gravity = 900f;

		public float GravityMult
		{
			get;
			set
			{
				field = value;
				effectiveGravity = value * Gravity;
			}
		}

		public bool SolidCollidable;

		public Collision OnCollideH;

		public Collision OnCollideV;

		public bool Grounded => Speed.Y >= 0 && OnGround();

		private readonly float MaxFall;

		private float effectiveGravity;

		public BossActor(Vector2 position, string spriteName, Vector2 spriteScale,
			float maxFall, bool collidable, bool solidCollidable, float gravityMult, Collider collider = null)
			: base(position, spriteName, spriteScale, collidable, collider)
		{
			MaxFall = maxFall;
			SolidCollidable = solidCollidable;
			GravityMult = gravityMult;
			OnCollideH = OnCollide_H;
			OnCollideV = OnCollide_V;
		}

		public override void Update()
		{
			base.Update();
			//Move based on speed
			Vector2 delta = Speed * Engine.DeltaTime;
			if (SolidCollidable)
			{
				MoveH(delta.X, OnCollideH);
				MoveV(delta.Y, OnCollideV);
			}
			else
			{
				NaiveMove(delta);
			}
			//Apply gravity
			if (!Grounded)
			{
				Speed.Y = Calc.Approach(Speed.Y, MaxFall, effectiveGravity * Engine.DeltaTime);
			}
		}

		public void OnCollide_H(CollisionData data)
		{
			if (data.Hit != null && data.Hit.OnCollide != null)
			{
				data.Hit.OnCollide(data.Direction);
			}
			Speed.X = 0;
		}

		public void OnCollide_V(CollisionData data)
		{
			if (data.Hit != null && data.Hit.OnCollide != null)
			{
				data.Hit.OnCollide(data.Direction);
			}
			Speed.Y = 0;
		}

		public bool SolidCollideCheck(Vector2 at)
		{
			return SolidCollidable && CollideCheck<Solid>(at);
		}

		public bool SolidCollideCheck()
		{
			return SolidCollidable && CollideCheck<Solid>();
		}

		public bool TryWiggle(int wiggleX = 3, int wiggleY = 3)
		{
			for (int i = 0; i <= wiggleX; i++)
			{
				for (int j = 0; j <= wiggleY; j++)
				{
					if (i == 0 && j == 0)
					{
						continue;
					}

					for (int num = 1; num >= -1; num -= 2)
					{
						for (int num2 = 1; num2 >= -1; num2 -= 2)
						{
							Vector2 vector = new(i * num, j * num2);
							if (!SolidCollideCheck(Position + vector))
							{
								Position += vector;
								return true;
							}
						}
					}
				}
			}
			return false;
		}
	}
}




