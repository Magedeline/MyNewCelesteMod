using System.Reflection;
using Celeste.Mod.Helpers;
using NLua;

namespace DesoloZantas.Core.BossesHelper.Helpers.Lua
{
	internal static class LuaMethodWrappers
	{
		private static Tracker Tracker => Engine.Scene.Tracker;

		private static EntityList Entities => Engine.Scene.Entities;

		#region Method Infos
		private static readonly MethodInfo getEntityMethodInfo = Tracker.GetType().GetMethod("GetEntity");

		private static readonly MethodInfo getEntitiesMethodInfo = Tracker.GetType().GetMethod("GetEntities");

		private static readonly MethodInfo entitiesFindFirstMethodInfo = Entities.GetType().GetMethod("FindFirst");

		private static readonly MethodInfo entitiesFindAll = Entities.GetType().GetMethod("FindAll");


		private static readonly MethodInfo getComponentMethodInfo = Tracker.GetType().GetMethod("GetComponent");

		private static readonly MethodInfo getComponentsMethodInfo = Tracker.GetType().GetMethod("GetComponents");

		private static readonly MethodInfo componentsGetFirst = typeof(ComponentList).GetMethod("Get");

		private static readonly MethodInfo componentsGetAll = typeof(ComponentList).GetMethod("GetAll");


		private static readonly MethodInfo entityIsTracked = Tracker.GetType().GetMethod("IsEntityTracked");


		private static readonly MethodInfo toAction = typeof(LuaDelegates).GetMethod("ToAction");

		private static readonly MethodInfo toFunc = typeof(LuaDelegates).GetMethod("ToFunc");
		#endregion

		#region Types and Generics
		private static readonly Assembly XNAAssembly = typeof(Vector2).Assembly;

		public static Type GetTypeFromString(string name, string prefix = "Celeste.")
		{
			return FakeAssembly.GetFakeEntryAssembly().GetType(prefix + name, false, true)
				?? XNAAssembly.GetType(prefix + name, false, true);
		}

#nullable enable
		public static object CallGeneric(this object on, MethodInfo method, Type type, params object[]? args)
#nullable disable
		{
			return method.MakeGenericMethod(type).Invoke(on, args);
		}

#nullable enable
		public static object CallGeneric(this object on, MethodInfo method, Type[] types, params object[]? args)
#nullable disable
		{
			return method.MakeGenericMethod(types).Invoke(on, args);
		}

		public static object CreateGeneric(Type classType, Type generic, params object[] args)
		{
			return Activator.CreateInstance(classType.MakeGenericType(generic), args);
		}

		public static object CreateGeneric(Type classType, Type[] generics, params object[] args)
		{
			return Activator.CreateInstance(classType.MakeGenericType(generics), args);
		}
		#endregion

		#region Delegates
		public static object GetAction(LuaFunction func, params Type[] types)
		{
			return toAction.MakeGenericMethod(types).Invoke(null, [func]);
		}

		public static object GetFunc(LuaFunction func, Type returnType, params Type[] types)
		{
			return toFunc.MakeGenericMethod([returnType, .. types]).Invoke(null, [func]);
		}
		#endregion

		#region Entities
		public static object GetEntity(string name, string prefix = "Celeste.")
		{
			return GetEntity(GetTypeFromString(name, prefix));
		}

		public static object GetEntity(Type type)
		{
			try
			{
				return Tracker.CallGeneric(getEntityMethodInfo, type);
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get entity: Requested type does not exist");
			}
			catch (TargetInvocationException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get entity: '{type}' is not trackable");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get entity: {arg}");
			}
			return null;
		}

		public static LuaTable GetEntities(string name, string prefix = "Celeste.")
		{
			return GetEntities(GetTypeFromString(name, prefix));
		}

		public static LuaTable GetEntities(Type type)
		{
			try
			{
				return (Tracker.CallGeneric(getEntitiesMethodInfo, type) as IList).ToLuaTable();
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get entities: Requested type does not exist");
			}
			catch (TargetInvocationException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get entities: '{type}' is not trackable");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get entities: {arg}");
			}
			return null;
		}

		public static object GetFirstEntity(string name, string prefix = "Celeste.")
		{
			return GetFirstEntity(GetTypeFromString(name, prefix));
		}

		public static object GetFirstEntity(Type type)
		{
			try
			{
				return Entities.CallGeneric(entitiesFindFirstMethodInfo, type);
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get entities: Requested type does not exist");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get entity: {arg}");
			}
			return null;
		}

		public static LuaTable GetAllEntities(string name, string prefix = "Celeste.")
		{
			return GetAllEntities(GetTypeFromString(name, prefix));
		}

		public static LuaTable GetAllEntities(Type type)
		{
			try
			{
				return (Entities.CallGeneric(entitiesFindAll, type) as IList).ToLuaTable();
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get entities: Requested type does not exist");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get entity: {arg}");
			}
			return null;
		}
		#endregion

		#region Components
		public static object GetComponent(string name, string prefix = "Celeste.")
		{
			return GetComponent(GetTypeFromString(name, prefix));
		}

		public static object GetComponent(Type type)
		{
			try
			{
				return Tracker.CallGeneric(getComponentMethodInfo, type);
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get component: Requested type does not exist");
			}
			catch (TargetInvocationException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get component: '{type}' is not trackable");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get component: {arg}");
			}
			return null;
		}

		public static LuaTable GetComponents(string name, string prefix = "Celeste.")
		{
			return GetComponents(GetTypeFromString(name, prefix));
		}

		public static LuaTable GetComponents(Type type)
		{
			try
			{
				return (Tracker.CallGeneric(getComponentsMethodInfo, type) as IList).ToLuaTable();
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get components: Requested type does not exist");
			}
			catch (TargetInvocationException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get components: '{type}' is not trackable");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get components: {arg}");
			}
			return null;
		}

		public static object GetFirstComponentOnType(string name, string entity, string prefix = "Celeste.", string entityPre = "Celeste.")
		{
			return GetFirstComponentOnType(GetTypeFromString(name, prefix), GetTypeFromString(entity, entityPre));
		}

		public static object GetFirstComponentOnType(Type type, Type entityType)
		{
			try
			{
				IList entities = ((bool)Tracker.CallGeneric(entityIsTracked, entityType)
					? Tracker.CallGeneric(getEntitiesMethodInfo, entityType)
					: Entities.CallGeneric(entitiesFindAll, entityType)) as IList;
				foreach (Entity entity in entities)
				{
					if (GetComponentFromEntity(entity, type) is object res)
						return res;
				}
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get component: Requested type does not exist");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get component: {arg}");
			}
			return null;
		}

		public static LuaTable GetAllComponentsOnType(string name, string entity, string prefix = "Celeste.", string entityPre = "Celeste.")
		{
			return GetAllComponentsOnType(GetTypeFromString(name, prefix), GetTypeFromString(entity, entityPre));
		}

		public static LuaTable GetAllComponentsOnType(Type type, Type entityType)
		{
			try
			{
				IList entities = ((bool)Tracker.CallGeneric(entityIsTracked, entityType)
					? Tracker.CallGeneric(getEntitiesMethodInfo, entityType)
					: Entities.CallGeneric(entitiesFindAll, entityType)) as IList;
				LuaTable luaTable = LuaBossHelper.GetEmptyTable();
				int num = 1;
				foreach (Entity entity in entities)
				{
					foreach (object item in GetComponentsFromEntity(entity, type))
					{
						luaTable[num++] = item;
					}
				}
				return luaTable;
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get components: Requested type does not exist");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get components: {arg}");
			}
			return null;
		}

		public static object GetFirstComponent(string name, string prefix = "Celeste.")
		{
			return GetFirstComponent(GetTypeFromString(name, prefix));
		}

		public static object GetFirstComponent(Type type)
		{
			try
			{
				foreach (Entity entity in Entities)
				{
					if (GetComponentFromEntity(entity, type) is object res)
						return res;
				}
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get component: Requested type does not exist");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get component: {arg}");
			}
			return null;
		}

		public static LuaTable GetAllComponents(string name, string prefix = "Celeste.")
		{
			return GetAllComponents(GetTypeFromString(name, prefix));
		}

		public static LuaTable GetAllComponents(Type type)
		{
			try
			{
				LuaTable luaTable = LuaBossHelper.GetEmptyTable();
				int num = 1;
				foreach (Entity entity in Entities)
				{
					foreach (object item in GetComponentsFromEntity(entity, type))
					{
						luaTable[num++] = item;
					}
				}
				return luaTable;
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get components: Requested type does not exist");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get components: {arg}");
			}
			return null;
		}

		public static object GetComponentFromEntity(Entity entity, string name, string prefix = "Celeste.")
		{
			return GetComponentFromEntity(entity, GetTypeFromString(name, prefix));
		}

		public static object GetComponentFromEntity(Entity entity, Type type)
		{
			try
			{
				return entity.Components.CallGeneric(componentsGetFirst, type);
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get component: Requested type does not exist");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get component: {arg}");
			}
			return null;
		}

		public static LuaTable GetComponentsFromEntity(Entity entity, string name, string prefix = "Celeste.")
		{
			return GetComponentsFromEntity(entity, GetTypeFromString(name, prefix));
		}

		public static LuaTable GetComponentsFromEntity(Entity entity, Type type)
		{
			try
			{
				return (entity.Components.CallGeneric(componentsGetAll, type) as IList).ToLuaTable();
			}
			catch (ArgumentNullException)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", "Failed to get components: Requested type does not exist");
			}
			catch (Exception arg)
			{
				Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to get components: {arg}");
			}
			return null;
		}

		public static bool EntityHasComponent(Entity entity, string name, string prefix = "Celeste.")
		{
			return EntityHasComponent(entity, GetTypeFromString(name, prefix));
		}

		public static bool EntityHasComponent(Entity entity, Type type)
		{
			return GetComponentFromEntity(entity, type) != null;
		}
		#endregion

		#region Teleports
		public static void TeleportTo(Scene scene, Player player, string room, Celeste.Player.IntroTypes introType = Celeste.Player.IntroTypes.Transition, Vector2? nearestSpawn = null)
		{
			if (scene is Level level)
			{
				level.OnEndOfFrame += () =>
				{
					level.TeleportTo(player, room, introType, nearestSpawn);
				};
			}
		}

		public static void InstantTeleport(Scene scene, Player player, string room, bool sameRelativePosition, float positionX, float positionY)
		{
			if (scene is not Level level)
			{
				return;
			}
			if (string.IsNullOrEmpty(room))
			{
				Vector2 vector = new Vector2(positionX, positionY) - player.Position;
				player.Position = new Vector2(positionX, positionY);
				level.Camera.Position += vector;
				player.Hair.MoveHairBy(vector);
				return;
			}
			level.OnEndOfFrame += () =>
			{
				Vector2 levelOffset = level.LevelOffset;
				Vector2 vector2 = player.Position - level.LevelOffset;
				Vector2 vector3 = level.Camera.Position - level.LevelOffset;
				Facings facing = player.Facing;
				level.Remove(player);
				level.UnloadLevel();
				level.Session.Level = room;
				level.Session.RespawnPoint = level.GetSpawnPoint(new Vector2(level.Bounds.Left, level.Bounds.Top));
				level.Session.FirstLevel = false;
				level.LoadLevel(Celeste.Player.IntroTypes.Transition);
				if (sameRelativePosition)
				{
					level.Camera.Position = level.LevelOffset + vector3;
					level.Add(player);
					player.Position = level.LevelOffset + vector2;
					player.Facing = facing;
					player.Hair.MoveHairBy(level.LevelOffset - levelOffset);
				}
				else
				{
					Vector2 vector4 = new Vector2(positionX, positionY) - level.LevelOffset - vector2;
					level.Camera.Position = level.LevelOffset + vector3 + vector4;
					level.Add(player);
					player.Position = new Vector2(positionX, positionY);
					player.Facing = facing;
					player.Hair.MoveHairBy(level.LevelOffset - levelOffset + vector4);
				}
				level.Wipe?.Cancel();
			};
		}
		#endregion

		#region Miscellaneous
		public static IEnumerable<T> OfType<T>(this LuaTable table) => table.Values.OfType<T>();

		public static ColliderList GetColliderListFromLuaTable(LuaTable luaTable)
		{
			return new([.. luaTable.OfType<Collider>()]);
		}

		public static IEnumerator Say(string dialog, LuaTable luaEvents)
		{
			Func<IEnumerator> Selector(LuaFunction func) => () => new LuaProxyCoroutine(func);
			return Textbox.Say(dialog, [.. luaEvents.OfType<LuaFunction>().Select(Selector)]);
		}

		public static void DoMethodAfterDelay(LuaFunction func, float delay)
		{
			Alarm.Create(Alarm.AlarmMode.Oneshot, func.ToAction(), delay, true);
		}
		#endregion

		#region Entity Collider Creator
		public static object GetEntityCollider(Entity baseEntity, LuaFunction func, Collider collider = null)
			=> GetEntityCollider(baseEntity.GetType(), func, collider);

		public static object GetEntityCollider(string baseType, LuaFunction func, Collider collider = null)
			=> GetEntityCollider(GetTypeFromString(baseType), func, collider);

		public static object GetEntityCollider(Type type, LuaFunction func, Collider collider = null)
		{
			return CreateGeneric(typeof(EntityCollider<>), type, GetAction(func, type), collider);
		}

		public static object GetEntityColliderByComponent(Component baseComp, LuaFunction func, Collider collider = null)
			=> GetEntityColliderByComponent(baseComp.GetType(), func, collider);

		public static object GetEntityColliderByComponent(string baseType, LuaFunction func, Collider collider = null)
			=> GetEntityColliderByComponent(GetTypeFromString(baseType), func, collider);

		public static object GetEntityColliderByComponent(Type type, LuaFunction func, Collider collider = null)
		{
			return CreateGeneric(typeof(EntityColliderByComponent<>), type, GetAction(func, type), collider);
		}
		#endregion
	}
}




