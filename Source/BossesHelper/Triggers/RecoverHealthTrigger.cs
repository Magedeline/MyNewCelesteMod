using DesoloZantas.Core.BossesHelper.Helpers;
using DesoloZantas.Core.BossesHelper.Entities;

namespace DesoloZantas.Core.BossesHelper.Triggers
{
	[CustomEntity("BossesHelper/RecoverHealthTrigger")]
	public class RecoverHealthTrigger(EntityData data, Vector2 offset, EntityID entityID)
		: SingleUseTrigger(data, offset, entityID, data.Bool("permanent"), data.Bool("onlyOnce", true))
	{
		private readonly int healAmount = data.Int("healAmount", 1);

		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			if (Scene.GetEntity<HealthSystemManager>() is HealthSystemManager manager)
			{
				ConsumeAfter(() => manager.RecoverHealth(healAmount));
			}
		}
	}
}




