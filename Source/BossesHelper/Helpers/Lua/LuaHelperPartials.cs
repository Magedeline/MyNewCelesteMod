using DesoloZantas.Core.Core;
using DesoloZantas.Core.BossesHelper.Helpers;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	namespace Lua
	{
		public partial class BossesHelperModule
		{
			private static IngesteModuleSession Session => IngesteModule.Session;
			private static IngesteModuleSession.HealthSystemData HealthData => Session.healthData;

			public static int PlayerHealth => HealthData.isEnabled ? Session.currentPlayerHealth : -1;

			public static EntityData MakeEntityData()
			{
				EntityData entityData = new()
				{
					Values = []
				};
				return entityData;
			}
		}
	}

	public partial class BossController
	{
		public string CurrentPatternName => CurrentPattern.Name;

		public bool IsActing => CurrentPattern.IsActing;

		private readonly Dictionary<string, object> storedObjects = [];

		public void StoreObject(string key, object toStore)
		{
			storedObjects.TryAdd(key, toStore);
		}

		public object GetStoredObject(string key)
		{
			return storedObjects.TryGetValue(key, out object storedObject) ? storedObject : null;
		}

		public void DeleteStoredObject(string key)
		{
			storedObjects.Remove(key);
		}

		public void AddEntity(Entity entity)
		{
			if (!activeEntities.Contains(entity))
			{
				Scene.Add(entity);
				activeEntities.Add(entity);
			}
		}

		public void DestroyEntity(Entity entity)
		{
			if (activeEntities.Remove(entity))
			{
				entity.RemoveSelf();
			}
		}

		public int GetPatternIndex(string goTo)
		{
			return CollectionExtensions.GetValueOrDefault(NamedPatterns, goTo, -1);
		}

		public void ForceNextAttack(int index)
		{
			if (CurrentPattern is RandomPattern Random)
				Random.ForcedAttackIndex = index;
		}

		public void SavePhaseChangeInSession(int health, int patternIndex, bool startImmediately)
		{
			IngesteModule.Session.BossPhasesSaved[BossID] =
				new(health, startImmediately, patternIndex);
		}

		public void RemoveBoss(bool permanent)
		{
			RemoveSelf();
			if (permanent && Scene is Level level)
			{
				level.DoNotLoad(SourceId);
			}
		}

		public void DecreaseHealth(int val = 1)
		{
			Health -= val;
		}
	}

	public partial class BossPuppet
	{
	}

	public partial class BounceBossPuppet
	{
	}	

	public partial class SidekickBossPuppet
	{
	}
}




