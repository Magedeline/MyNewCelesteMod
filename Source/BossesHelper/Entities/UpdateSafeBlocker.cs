using DesoloZantas.Core.Core;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	[Tracked(false)]
	[CustomEntity("BossesHelper/UpdateSafeBlocker")]
	public class UpdateSafeBlocker : Entity
	{
		public bool IsGlobal
		{
			get => TagCheck(Tags.Global);
			set => this.ChangeTagState(Tags.Global, value);
		}

		public UpdateSafeBlocker(EntityData data, Vector2 _)
			: this(data.Bool("isGlobal", IngesteModule.Session.globalSafeGroundBlocker))
		{
			IngesteModule.Session.globalSafeGroundBlocker = IsGlobal;
		}

		public UpdateSafeBlocker(bool isGlobal = false) : base()
		{
			IsGlobal = isGlobal;
			IngesteModule.Session.safeGroundBlockerCreated = true;
		}

		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			if (scene.GetEntity<UpdateSafeBlocker>() != this)
				RemoveSelf();
		}

		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			IngesteModule.Session.safeGroundBlockerCreated = false;
		}
	}
}




