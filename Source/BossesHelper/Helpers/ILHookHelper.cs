using System.Reflection;
using DesoloZantas.Core.BossesHelper.Entities;
using DesoloZantas.Core.BossesHelper.Helpers.Code.Helpers;
using DesoloZantas.Core.BossesHelper.Helpers.Lua;
using DesoloZantas.Core.Core;
using DesoloZantas.Core.Core.Player;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;

namespace DesoloZantas.Core.BossesHelper.Helpers
{
	public partial class BossesHelperModule
	{
		// Delegate to forward to IngesteModule's implementation
		// TODO: Re-enable when BossesHelper integration is fixed
		public static void KillOnCrush(CelestePlayer player, CollisionData data, bool evenIfInvincible)
		{
			// IngesteModule.BossesHelper_KillOnCrush(player, data, evenIfInvincible);
			// Stub implementation - BossesHelper integration needs to be fixed
		}

		public static void ILOnSquish(ILContext il)
		{
			ILCursor dieCursor = new(il);
			while (dieCursor.TryGotoNext(MoveType.After, instr => instr.MatchCallvirt<Player>("Die")))
			{
				ILCursor argCursor = new(dieCursor);
				if (argCursor.TryGotoPrev(MoveType.AfterLabel, instr => instr.MatchLdarg(0)))
				{
					//KillOnCrush(self, data, evenIfInvincible);
					argCursor.Emit(OpCodes.Ldarg_0);
					argCursor.Emit(OpCodes.Ldarg_1);
					argCursor.Emit(OpCodes.Ldloc_2);
					argCursor.EmitDelegate(KillOnCrush);
				}
			}
		}
	}

	// Static exports class for BossesHelper operations
	public static class BossesHelperExports
	{
		public static void ClearFakeDeath()
		{
			IngesteModule.Session.BossesHelper_UseFakeDeath = false;
		}
	}

	namespace Code
	{
		namespace Components
		{
			public partial class GlobalSavePointChanger : Component
			{
				public readonly EntityID id;
				public readonly Vector2 sessionRespawnPoint;
				public readonly IntroTypes spawnType;

				public GlobalSavePointChanger(EntityID id, Vector2 sessionRespawnPoint, IntroTypes spawnType) : base(true, true)
				{
					this.id = id;
					this.sessionRespawnPoint = sessionRespawnPoint;
					this.spawnType = spawnType;
				}

				public void AddToEntityOnMethod<T>(T entity, string method,
					BindingFlags flags = BindingFlags.Default, bool stateMethod = false) where T : Entity
				{
					entity.Add(component: this);
					ILHookHelper.GenerateHookOn(classType: typeof(T), method: method, action: AddUpdateDelegate, flags: flags, stateMethod: stateMethod);
				}

				private static void AddUpdateDelegate(ILContext il)
				{
					//this.Get<GlobalSavePointChanger>()?.Update();
					var cursor = new ILCursor(context: il);
					cursor.Emit(opcode: OpCodes.Ldarg_0);
					cursor.EmitDelegate(cb: UpdateSavePointChanger);
				}

				private static void UpdateSavePointChanger(Entity entity)
				{
					entity.Get<GlobalSavePointChanger>()?.Update();
				}
			}
		}

		namespace Helpers
		{
			public static class ILHookHelper
			{
				private static readonly Dictionary<string, ILHook> createdILHooks = [];

				public static MethodInfo GetMethodInfo(Type type, string method,
					BindingFlags flags = BindingFlags.Default, bool stateMethod = false)
				{
					return type.GetMethod(name: method, bindingAttr: flags) is not MethodInfo methodInfo ? null :
						stateMethod ? methodInfo.GetStateMachineTarget() : methodInfo;
				}

				private static string GetKey(MethodInfo methodInfo) => GetKey(type: methodInfo.DeclaringType, method: methodInfo.Name);

				private static string GetKey(Type type, string method) => $"{type.Name}:{method}";

				public static void GenerateHookOn(Type classType, string method,
					ILContext.Manipulator action, BindingFlags flags = BindingFlags.Default, bool stateMethod = false)
					=> GenerateHookOn(key: GetKey(type: classType, method: method), methodInfo: GetMethodInfo(type: classType, method: method, flags: flags, stateMethod: stateMethod), action: action);

				public static void GenerateHookOn(MethodInfo methodInfo, ILContext.Manipulator action)
					=> GenerateHookOn(key: GetKey(methodInfo: methodInfo), methodInfo: methodInfo, action: action);

				public static void GenerateHookOn(string key, MethodInfo methodInfo, ILContext.Manipulator action)
				{
					createdILHooks.TryAdd(key: key, value: new(from: methodInfo, manipulator: action));
				}

				public static void DisposeHook(MethodInfo methodInfo)
					=> DisposeHook(key: GetKey(methodInfo: methodInfo));

				public static void DisposeHook(Type classType, string method)
					=> DisposeHook(key: GetKey(type: classType, method: method));

				public static void DisposeHook(string key)
				{
					if (createdILHooks.Remove(key: key, value: out ILHook hook))
						hook.Dispose();
				}

				public static void DisposeAll()
				{
					foreach (ILHook hook in createdILHooks.Values)
					{
						hook.Dispose();
					}
					createdILHooks.Clear();
				}
			}
		}

		namespace Entities
		{
			public partial class HealthSystemManager
			{
				public static void LoadFakeDeathHooks()
				{
					foreach (string fakeMethod in BossesHelper.Entities.HealthSystemManager.HealthData.FakeDeathMethods)
					{
						var (classType, methodName) = fakeMethod.SplitOnce(character: ':');
						if (classType == null)
							continue;

						var (classPrefix, className) = classType.SplitOnce(character: '.', forward: false, mode: SplitMode.IncludeFirst, preDefault: "Celeste.");

						if (LuaMethodWrappers.GetTypeFromString(name: className, prefix: classPrefix)?
							.GetMethod(name: methodName, bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
							is MethodInfo methodInfo)
						{
							ILHookHelper.GenerateHookOn(key: fakeMethod, methodInfo: methodInfo, action: il =>
							{
								ILCursor cursor = new(context: il);
								cursor.EmitDelegate(cb: FakeDeathHook);
								while (cursor.TryGotoNext(predicates: instr => instr.MatchRet()))
								{
									cursor.EmitDelegate(cb: BossesHelperExports.ClearFakeDeath);
									cursor.Index++;
								}
							});
						}
					}
				}

				private static void FakeDeathHook()
				{
					if (BossesHelper.Entities.HealthSystemManager.IsEnabled)
					{
						BossesHelper.Entities.HealthSystemManager.ModSession.BossesHelper_UseFakeDeath = true;
					}
				}

				public static void UnloadFakeDeathHooks()
				{
					foreach (string fakeMethod in BossesHelper.Entities.HealthSystemManager.HealthData.FakeDeathMethods)
					{
						ILHookHelper.DisposeHook(key: fakeMethod);
					}
				}
			}
		}
	}
}




