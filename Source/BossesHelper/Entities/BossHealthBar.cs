using DesoloZantas.Core.BossesHelper.Components;
using static DesoloZantas.Core.BossesHelper.Entities.HealthDisplays;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	[Tracked(false)]
	[CustomEntity("BossesHelper/BossHealthBar")]
	public class BossHealthBar : Entity
	{
		private readonly Vector2 BarPosition;

		private readonly Vector2 BarScale;

		private Func<int> BossHealth;

		private enum BarTypes
		{
			BarLeft = -1,
			BarRight,
			BarCentered,
			Icons,
			Countdown
		}

		private readonly BarTypes barType;

		private HealthDisplay barEntity;

		public new bool Visible
		{
			get => barEntity.Visible;
			set => barEntity.Visible = value;
		}

		public BossHealthBar(EntityData data, Vector2 _)
			: base()
		{
			AddTag(Tags.HUD);
			Position = data.Position;
			BarPosition = new Vector2(data.Float("healthBarX"), data.Float("healthBarY"));
			BarScale = new Vector2(data.Float("healthScaleX", 1f), data.Float("healthScaleY", 1f));
			barType = data.Enum("barType", BarTypes.BarLeft);
		}

		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Level level = SceneAs<Level>();
			if (level.Tracker.GetNearestComponent<BossHealthTracker>(SourceData.Nodes[0]) is not { } component)
			{
				RemoveSelf();
				return;
			}
			BossHealth = component.Health;
			Color baseColor = SourceData.HexColor("baseColor", Color.White);
			level.Add(barEntity = barType switch
			{
				BarTypes.Icons => new HealthIconList(SourceData, BarPosition, BarScale, BossHealth),
				BarTypes.Countdown => new HealthNumber(BarPosition, BarScale, BossHealth, baseColor),
				_ => new HealthBar(BarPosition, BarScale, BossHealth, baseColor, (Alignment)barType)
			});
			Visible = SourceData.Bool("startVisible");
		}

		public override void Update()
		{
			base.Update();
			if (barEntity is HealthIconList healthIcons)
			{
				healthIcons.DecreaseHealth(healthIcons.Count - BossHealth());
			}
		}

		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			barEntity.RemoveSelf();
		}
	}
}




