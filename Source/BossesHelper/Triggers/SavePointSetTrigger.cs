using DesoloZantas.Core.BossesHelper.Helpers.Code.Components;
using DesoloZantas.Core.Core.Player;

namespace DesoloZantas.Core.BossesHelper.Triggers
{
	[CustomEntity("BossesHelper/SavePointSetTrigger")]
	public class SavePointSetTrigger(EntityData data, Vector2 offset, EntityID id)
				: SingleUseTrigger(data, offset, id, data.Bool("onlyOnce"), true)
	{
		private readonly IntroTypes spawnType = data.Enum<IntroTypes>("respawnType");

		private readonly Vector2? spawnPosition = data.FirstNodeNullable() + offset;

		private readonly string flagTrigger = data.String("flagTrigger");

		private readonly bool invertFlag = data.Bool("invertFlag");

		private GlobalSavePointChanger Changer;

		public override void Added(Scene scene)
		{
			base.Added(scene);
			Add(Changer = new(ID,
				spawnPosition ?? SceneAs<Level>().Session.GetSpawnPoint(Center),
				spawnType));
		}

		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			if (flagTrigger == null ||
				(player.SceneAs<Level>().Session.GetFlag(flagTrigger) ^ invertFlag))
			{
				ConsumeAfter(() => Changer?.Update());
			}
		}
	}
}




