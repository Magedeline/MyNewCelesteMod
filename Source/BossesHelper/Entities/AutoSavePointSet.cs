using DesoloZantas.Core.BossesHelper.Helpers;
using DesoloZantas.Core.BossesHelper.Helpers.Code.Components;
using DesoloZantas.Core.Core.Player;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	[CustomEntity("BossesHelper/AutoSavePointSet")]
	public class AutoSavePointSet(EntityData data, Vector2 pos, EntityID ID) : Entity(pos)
	{
		private readonly IntroTypes spawnType = data.Enum<IntroTypes>("respawnType");

		private readonly Vector2? spawnPosition = data.FirstNodeNullable();

		private readonly bool onlyOnce = data.Bool("onlyOnce");

		private GlobalSavePointChanger Changer;

		public override void Added(Scene scene)
		{
			base.Added(scene);
			Add(Changer = new(ID,
				spawnPosition ?? SceneAs<Level>().Session.RespawnPoint ?? Position, spawnType));
		}

		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			Changer?.Update();
			RemoveSelf();
		}

		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			if (onlyOnce)
				scene.DoNotLoad(ID);
		}
	}
}




