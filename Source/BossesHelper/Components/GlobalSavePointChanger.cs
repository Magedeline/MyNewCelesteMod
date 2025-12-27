using DesoloZantas.Core.Core;
using DesoloZantas.Core.Core.Player;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;

namespace DesoloZantas.Core.BossesHelper.Components
{
	[Tracked(false)]
	public partial class GlobalSavePointChanger(object levelNameSrc, Vector2 spawnPoint, IntroTypes spawnType = IntroTypes.Respawn)
		: Component(active: false, visible: false)
	{
		public readonly string spawnLevel = LevelName(levelNameSrc);

		public Vector2 spawnPoint = spawnPoint;

		private static string LevelName(object source)
		{
			return source switch
			{
				EntityID id => id.Level,
				LevelData ld => ld.Name,
				Session session => session.Level,
				Scene sc => LevelName((sc as Level).Session),
				Entity e => LevelName(e.Scene),
				_ => throw new Exception("Object type cannot be used to get a Level Name.")
			};
		}

		public override void Added(Entity entity)
		{
			base.Added(entity);
			if (Scene != null)
			{
				AddedToScene();
			}
		}

		public override void EntityAdded(Scene scene)
		{
			base.EntityAdded(scene);
			AddedToScene();
		}

		private void AddedToScene()
		{
			Vector2 newSpawn = SceneAs<Level>().GetSpawnPoint(spawnPoint);
			if (newSpawn != spawnPoint && DistanceBetween(spawnPoint, newSpawn) <= 80)
				spawnPoint = newSpawn;
		}

		public override void Update()
		{
			IngesteModule.Session.savePointLevel = spawnLevel;
			IngesteModule.Session.savePointSpawn = spawnPoint;
			IngesteModule.Session.savePointSpawnType = spawnType;
			IngesteModule.Session.savePointSet = true;
			Active = false;
		}
	}
}




