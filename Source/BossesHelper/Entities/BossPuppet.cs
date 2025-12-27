using Monocle;
using DesoloZantas.Core.BossesHelper.Components;
using DesoloZantas.Core.BossesHelper.Helpers;
using static DesoloZantas.Core.BossesHelper.Helpers.UserFileReader;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	public abstract partial class BossPuppet : BossActor
	{
		#region Enums
		public enum ColliderOption
		{
			Hitboxes,
			Hurtboxes,
			KillColliders,
			Bouncebox,
			Target
		}

		public enum HurtModes
		{
			PlayerContact,
			PlayerDash,
			HeadBonk,
			SidekickAttack,
			Custom
		}
		#endregion

		private readonly HitboxMetadata hitboxMetadata;

		public Collider Hurtbox;

		public Collider KillCollider
		{
			get;
			set => PlayerKillCollider.Collider = field = value;
		}

		private readonly PlayerCollider PlayerKillCollider;

		protected readonly Component BossCollision;

		public abstract HurtModes HurtMode { get; }

		private readonly bool DynamicFacing;

		private readonly bool MirrorSprite;

		internal BossFunctions BossFunctions;

		public readonly Stopwatch BossDamageCooldown;

		public int Facing;

		public float groundFriction;

		public float airFriction;

		public bool KillOnContact;

		protected BossPuppet(EntityData data, Vector2 offset)
			: base(data.Position + offset, data.Attr("bossSprite"), Vector2.One, data.Float("maxFall", 90f),
				  data.Bool("startCollidable"), data.Bool("startSolidCollidable"), data.Float("baseGravityMultiplier", 1f))
		{
			Facing = 1;
			DynamicFacing = data.Bool("dynamicFacing");
			MirrorSprite = data.Bool("mirrorSprite");
			GravityMult = data.Float("baseGravityMultiplier", 1f);
			groundFriction = data.Float("groundFriction");
			airFriction = data.Float("airFriction");

			PlayAnim(data.String("startingAnim", "idle"));
			Add(BossDamageCooldown = new(data.Float("bossHitCooldown", 0.5f)));
			Add(PlayerKillCollider = new(KillPlayer));
			KillOnContact = data.Bool("killOnContact");

			hitboxMetadata = ReadMetadataFile(data.Attr("hitboxMetadataPath"));
			Collider = GetCollider(ColliderOption.Hitboxes);
			KillCollider = GetCollider(ColliderOption.KillColliders);
			Hurtbox = GetCollider(HurtMode switch
			{
				HurtModes.HeadBonk => ColliderOption.Bouncebox,
				HurtModes.SidekickAttack => ColliderOption.Target,
				_ => ColliderOption.Hurtboxes
			});
			if ((BossCollision = GetBossCollision()) != null)
				Add(BossCollision);
		}

		protected abstract Component GetBossCollision();

	internal static BossPuppet Create(HurtModes hurtMode, EntityData data, Vector2 offset)
	{
		return hurtMode switch
		{
			HurtModes.PlayerContact => new ContactBossPuppet(data, offset),
			HurtModes.PlayerDash => new DashBossPuppet(data, offset),
			HurtModes.HeadBonk => new BounceBossPuppet(data, offset),
			HurtModes.SidekickAttack => new SidekickBossPuppet(data, offset),
			_ => new CustomBossPuppet(data, offset)
		};
	}

		public override void Update()
		{
			Facing = Facing != 0 ? Facing / Math.Abs(Facing) : 1;
			if (Scene.GetPlayer() is Player entity && DynamicPositionOver_Quarter(entity.X))
			{
				Facing *= -1;
			}
			base.Update();
			//Apply friction
			Speed.X = Calc.Approach(Speed.X, 0f, (Grounded ? groundFriction : airFriction) * Engine.DeltaTime);
			//Return Sprite Scale
			Sprite.Scale = Calc.Approach(Sprite.Scale, Vector2.One, 1.75f * Engine.DeltaTime);
		}

		private bool DynamicPositionOver_Quarter(float pos)
		{
			if (!DynamicFacing)
				return false;
			if (Facing == -1)
				return pos < X - Collider.Width / 4;
			return pos > X + Collider.Width / 4;
		}

		public override void Render()
		{
			base.Render();
			int realFacing = Facing * (MirrorSprite ? -1 : 1);
			Sprite.Scale.X = realFacing;
		}

		public Collider GetCollider(ColliderOption option, string key = "main")
		{
			if (key == "empty" || option != ColliderOption.Hitboxes && hitboxMetadata[option].Count == 0)
				return null;

			if (hitboxMetadata[option].TryGetValue(key, out var result))
				return result;

			if (option == ColliderOption.Target)
				return new Circle(4f);

			return new Hitbox(Sprite.Width, option == ColliderOption.Bouncebox ? 6f : Sprite.Height,
				Sprite.Width * -0.5f, Sprite.Height * -0.5f);
		}

		private void KillPlayer(Player player)
		{
			if (KillOnContact)
				player.Die((player.Position - Position).SafeNormalize());
		}

		protected void OnDamage(Func<bool> predicate = null, Action postLua = null)
		{
			if (BossDamageCooldown.Finished && (predicate?.Invoke() ?? true))
			{
				BossDamageCooldown.Reset();
				Add(new Coroutine(BossFunctions[HurtMode]));
				postLua?.Invoke();
			}
		}
	}	public class ContactBossPuppet(EntityData data, Vector2 offset) : BossPuppet(data, offset)
	{
		public override HurtModes HurtMode => HurtModes.PlayerContact;

		protected override Component GetBossCollision()
			=> new PlayerCollider(_ => OnDamage(), Hurtbox);
	}

	public class DashBossPuppet(EntityData data, Vector2 offset) : BossPuppet(data, offset)
	{
		public override HurtModes HurtMode => HurtModes.PlayerDash;

		protected override Component GetBossCollision()
			=> new PlayerCollider(OnPlayerDash, Hurtbox);

		private void OnPlayerDash(Player player)
		{
			OnDamage(() => player.DashAttacking && player.Speed != Vector2.Zero);
		}
	}

	public partial class BounceBossPuppet(EntityData data, Vector2 offset) : BossPuppet(data, offset)
	{
		public override HurtModes HurtMode => HurtModes.HeadBonk;

		protected override Component GetBossCollision()
			=> new PlayerCollider(OnPlayerBounce, Hurtbox);

		private void OnPlayerBounce(Player player)
		{
			OnDamage(postLua: () =>
			{
				Celeste.Audio.Play("event:/game/general/thing_booped", Position);
				Engine.FreezeTimer = 0.2f;
				player.Bounce(Top + 2f);
			});
		}
	}

	public partial class SidekickBossPuppet(EntityData data, Vector2 offset) : BossPuppet(data, offset)
	{
		public override HurtModes HurtMode => HurtModes.SidekickAttack;

		private readonly string BossID = data.Attr("bossID");

		private readonly bool freezeSidekickOnAttack = data.Bool("sidekickFreeze");

		private readonly float sidekickCooldown = data.Float("sidekickCooldown");

		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Player player = scene.GetPlayer();
			if (scene.Tracker.GetEntity<BadelineSidekick>() == null)
			{
				(scene as Level).Add(new BadelineSidekick(player.Position + new Vector2(-16f * (int)player.Facing, -4f), freezeSidekickOnAttack, sidekickCooldown));
			}
		}

		protected override Component GetBossCollision()
			=> new SidekickTarget(() => OnDamage(), BossID, Hurtbox);
	}

	public class CustomBossPuppet(EntityData data, Vector2 offset) : BossPuppet(data, offset)
	{
		public override HurtModes HurtMode => HurtModes.Custom;

		protected override Component GetBossCollision()
			=> null;
	}
}




