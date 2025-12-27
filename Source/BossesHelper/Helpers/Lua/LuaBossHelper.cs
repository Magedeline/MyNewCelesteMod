using DesoloZantas.Core.Core;
using NLua;

namespace DesoloZantas.Core.BossesHelper.Helpers.Lua
{
	using LuaData = (LuaTable Env, LuaFunction[] Funcs);

	#region Lua Loading
	public enum PrepareMode
	{
		Cutscene,
		Attack,
		Interrupt,
		Function,
		SavePoint
	}

	public interface ILuaLoader
	{
		PrepareMode Mode { get; }

		Dictionary<string, object> Values { get; }

		Scene Scene { get; }
	}
	#endregion

	internal static class LuaBossHelper
	{
		private static readonly string FilesPath = "Assets/LuaBossHelper";

		private static readonly string HelperFunctionsPath = $"{IngesteModule.Instance.Metadata.Name}:/{FilesPath}/helper_functions";

		public static class CutsceneHelper
		{
			private static readonly LuaTable Base = GetLuaAsset($"{FilesPath}/cutscene_helper");

			public static LuaData GetLuaData(string content, LuaTable data, string preparer)
			{
				object[] luaData = (Base["getLuaData"] as LuaFunction).Call(content, data, preparer);
				return (luaData[0] as LuaTable, [.. luaData.Skip(1).OfType<LuaFunction>()]);
			}

			public static LuaTable GetProxyTable(LuaFunction func)
				=> (Base["getProxyTable"] as LuaFunction).Call(func).ElementAtOrDefault(0) as LuaTable;
		}

		public static void WarmUp()
		{
			Logger.Log("Bosses Helper", "Warming up Lua cutscenes");
			foreach (var func in LoadFile($"{FilesPath}/warmup_cutscene", PrepareMode.Cutscene))
				func.Call();
		}

		public static LuaTable GetLuaAsset(string path, string modName = "")
		{
			if (string.IsNullOrWhiteSpace(modName))
				modName = IngesteModule.Instance.Metadata.Name;
			return Everest.LuaLoader.Require($"{modName}:/{path}") as LuaTable;
		}

		public static string GetFileContent(string path)
		{
			if (Everest.Content.Get(path)?.Stream is not Stream stream)
				return null;
			using StreamReader streamReader = new(stream);
			return streamReader.ReadToEnd();
		}

		public static string GetHelperFunctions()
		{
			return GetFileContent(HelperFunctionsPath);
		}

		public static LuaTable ToLuaTable(this IDictionary<string, object> dict)
		{
			LuaTable luaTable = GetEmptyTable();
			foreach (KeyValuePair<string, object> item in dict)
			{
				luaTable[item.Key] = item.Value;
			}
			return luaTable;
		}

		public static LuaTable ToLuaTable(this IList list)
		{
			LuaTable luaTable = GetEmptyTable();
			int num = 1;
			foreach (object item in list)
			{
				luaTable[num++] = item;
			}
			return luaTable;
		}

		public static LuaFunction[] LoadFile(this ILuaLoader self, string filename)
		{
			self.Values.Add("player", self.Scene.GetPlayer());
			return LoadFile(filename, self.Mode, self.Values);
		}

		public static LuaFunction[] LoadFile(string filename, PrepareMode mode, Dictionary<string, object> passedVals = null)
		{
			passedVals ??= [];
			LuaFunction[] funcs = null;
			if (!string.IsNullOrEmpty(filename))
			{
				passedVals.Add("modMetaData", IngesteModule.Instance.Metadata);
				try
				{
				if (GetFileContent(filename) is string content && !string.IsNullOrWhiteSpace(content) &&
					CutsceneHelper.GetLuaData(content, passedVals.ToLuaTable(), $"get{mode}Data") is LuaData data)
					{
						funcs = data.Funcs;
					}
					else
					{
						Logger.Log("Bosses Helper", "Failed to load Lua Cutscene, target file does not exist: \"" + filename + "\"");
					}
				}
				catch (Exception e)
				{
					Logger.Log(LogLevel.Error, "Bosses Helper", $"Failed to execute cutscene in C#: {e}");
				}
			}
			Array.Resize(ref funcs, mode.FuncNumber());
			return funcs;
		}

		public static int FuncNumber(this PrepareMode mode)
		{
			return mode switch
			{
				PrepareMode.Interrupt => 6,
				PrepareMode.Attack => 5,
				PrepareMode.Cutscene or PrepareMode.Function => 2,
				PrepareMode.SavePoint => 1,
				_ => 0
			};
		}

		public static LuaTable GetEmptyTable()
		{
			return Everest.LuaLoader.Context.DoString("return {}").FirstOrDefault() as LuaTable;
		}
	}
}




