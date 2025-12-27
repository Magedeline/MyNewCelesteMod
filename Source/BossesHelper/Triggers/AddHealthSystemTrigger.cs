using Monocle;
using DesoloZantas.Core.BossesHelper.Entities;

namespace DesoloZantas.Core.BossesHelper.Triggers
{
	[CustomEntity("BossesHelper/AddHealthSystemTrigger")]
	public class AddHealthSystemTrigger(EntityData data, Vector2 offset, EntityID id)
		: SingleUseTrigger(data, offset, id, data.Bool("onlyOnce", true), true)
	{
		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			ConsumeAfter(() => Scene.Add(new HealthSystemManager(SourceData, Vector2.Zero)));
		}
	}
}




