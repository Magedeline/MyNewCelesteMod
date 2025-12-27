using DesoloZantas.Core.BossesHelper.Components;
using DesoloZantas.Core.Core;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	[Tracked]
	internal class BadelineSidekick : Entity
	{
		private readonly Follower Follower;

		private SidekickBeam beam;

		private float oldX;

		private enum SidekickSprite
		{
			Dummy,
			Boss,
			Custom
		}

		private Sprite ActiveSprite;

		private SidekickSprite ActiveSidekickSprite
		{
			get;
			set
			{
				field = value;
				ActiveSprite = SidekickSprites[value];
				foreach (var sprite in SidekickSprites)
				{
					sprite.Value.Visible = value == sprite.Key;
				}
			}
		}

		private class DummySprite : Celeste.PlayerSprite
		{
			public readonly PlayerHair Hair;

			public new bool Visible
			{
				get => base.Visible && Hair.Visible;
				set => base.Visible = Hair.Visible = value;
			}

			public DummySprite()
				: base(Celeste.PlayerSpriteMode.Badeline)
			{
				Scale.X = -1f;
				Play("fallslow");
				Hair = new(this)
				{
					Color = BadelineOldsite.HairColor,
					Border = Color.Black,
					Facing = Facings.Left
				};
			}

			public override void Added(Entity entity)
			{
				base.Added(entity);
				entity.Add(Hair);
			}

			public override void Render()
			{
				base.Render();
				Hair.Facing = (Facings)Math.Sign(Scale.X);
			}
		}

		private readonly EnumDict<SidekickSprite, Sprite> SidekickSprites;

		private readonly SineWave Wave;

		private readonly VertexLight Light;

		private readonly SoundSource laserSfx;

		public Vector2 BeamOrigin => Center + SidekickSprites[SidekickSprite.Boss].Position + new Vector2(0f, -20f);

		private bool CanAttack;

		private readonly bool FreezeOnAttack;

		private readonly float sidekickCooldown;

		public BadelineSidekick(Vector2 position, bool freezeOnAttack, float cooldown) : base(position)
		{
			SidekickSprites = new(sprite => sprite switch
			{
				SidekickSprite.Boss => GFX.SpriteBank.Create("badeline_boss"),
				SidekickSprite.Custom => GFX.SpriteBank.Create("badeline_sidekick"),
				_ => new DummySprite()
			});

			foreach (var sprite in SidekickSprites.Values)
				Add(sprite);

			Add(Wave = new SineWave(0.25f, 0f));
			Wave.OnUpdate = f =>
			{
				foreach (var sprite in SidekickSprites.Values)
				{
					sprite.Position = Vector2.UnitY * f * 2f;
				}
			};
			Add(Light = new VertexLight(new Vector2(0f, -8f), Color.PaleVioletRed, 1f, 20, 60));
			Add(Follower = new Follower());
			Follower.PersistentFollow = true;
			AddTag(Tags.Persistent);
			ActiveSidekickSprite = SidekickSprite.Dummy;
			Add(laserSfx = new SoundSource());
			FreezeOnAttack = freezeOnAttack;
			sidekickCooldown = cooldown;
			CanAttack = true;
		}

		private IEnumerator AttackSequence()
		{
			Level level = SceneAs<Level>();
			SidekickTarget target = level.Tracker.GetNearestComponent<SidekickTarget>(BeamOrigin);
			if (target == null)
			{
				level.Add(new MiniTextbox("Badeline_Sidekick_No_Target"));
				yield break;
			}
			if (FreezeOnAttack)
			{
				Follower.MoveTowardsLeader = false;
			}
			Wave.Active = false;
			ActiveSprite.Position = Vector2.Zero;
			ActiveSidekickSprite = SidekickSprite.Custom;
			yield return ActiveSprite.PlayAnim("normal_to_boss");

			laserSfx.Play("event:/char/badeline/boss_laser_charge");
			ActiveSidekickSprite = SidekickSprite.Boss;
			ActiveSprite.Play("attack2Begin", true);
			yield return 0.1f;
			beam = level.CreateAndAdd<SidekickBeam>().Init(this, target);
			yield return 0.9f;
			ActiveSprite.Play("attack2Lock", true);
			yield return 0.5f;
			laserSfx.Stop();
			Celeste.Audio.Play("event:/char/badeline/boss_laser_fire", Position);
			ActiveSprite.Play("attack2Recoil");
			yield return 0.5f;

			Follower.MoveTowardsLeader = true;
			Wave.Active = true;
			ActiveSidekickSprite = SidekickSprite.Custom;
			ActiveSprite.Play("boss_to_mini");
			yield return sidekickCooldown;

			yield return ActiveSprite.PlayAnim("mini_to_normal");
			ActiveSidekickSprite = SidekickSprite.Dummy;
			CanAttack = true;
		}

		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Level level = SceneAs<Level>();
			if (level.Tracker.GetComponents<SidekickTarget>().Count == 0)
			{
				RemoveSelf();
				return;
			}
			level.GetPlayer()?.Leader.GainFollower(Follower);
		}

		public override void Update()
		{
			Level level = SceneAs<Level>();
			if (level.GetPlayer() is Player entity && (Follower.Leader == null || Follower.Leader.Entity != entity))
			{
				entity.Leader.GainFollower(Follower);
			}
			if (IngesteModule.Settings.SidekickLaserBind.Pressed && CanAttack)
			{
				CanAttack = false;
				AttackSequence().AsCoroutine(this);
			}
			oldX = X;
			base.Update();
			Light.Position = ActiveSprite.Position + new Vector2(0f, -10f);
		}

		public override void Render()
		{
			Vector2 renderPosition = ActiveSprite.RenderPosition;
			ActiveSprite.RenderPosition = ActiveSprite.RenderPosition.Floor();
			if (beam != null && (beam.oldX - X) * ActiveSprite.Scale.X < 0 ||
				(oldX - X) * ActiveSprite.Scale.X > 0)
			{
				ActiveSprite.Scale.X *= -1;
			}
			base.Render();
			ActiveSprite.RenderPosition = renderPosition;
		}
	}
}




