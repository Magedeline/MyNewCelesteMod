using DesoloZantas.Core.BossesHelper.Helpers;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	[CustomEntity("BossesHelper/UpdateSafeUnblocker")]
	public class UpdateSafeUnblocker(EntityData data, Vector2 pos, EntityID id) : Entity(pos)
	{
		private readonly bool onlyOnce = data.Bool("onlyOnce");

		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			scene.GetEntity<UpdateSafeBlocker>()?.RemoveSelf();
			RemoveSelf();
		}

		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			if (onlyOnce)
			{
				scene.DoNotLoad(id);
			}
		}
	}
}




