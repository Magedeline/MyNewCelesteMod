using System.Xml;
using DesoloZantas.Core.BossesHelper.Entities;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;
using BossPuppet = DesoloZantas.Core.BossesHelper.Entities.BossPuppet;

namespace DesoloZantas.Core.BossesHelper.Helpers
{
	internal static class UserFileReader
	{
		#region XML Files
		public static List<BossPattern> ReadPatternFile(string filepath, BossController controller)
		{
			if (GetXMLDocument(filepath) is not XmlDocument xml)
				throw new Exception("Failed to find any Pattern file.");

			List<BossPattern> result = [];
			foreach (XmlNode node in xml.GetChildNodes("Patterns"))
			{
				if (!Enum.TryParse(node.LocalName, true, out PatternType type))
				{
					Logger.Warn("Bosses Helper", $"{node.LocalName} is not an accepted Patterns node type.");
					continue;
				}

				result.Add(node.ParseNewPattern(controller, type));
			}
			return result;
		}

		public static HitboxMetadata ReadMetadataFile(string filepath)
		{
			HitboxMetadata metadata = new();

			if (GetXMLDocument(filepath) is not XmlDocument xml)
			{
				Logger.Warn("Bosses Helper", "No Hitbox Metadata file found. Boss will use all default hitboxes.");
				return metadata;
			}

			foreach (XmlNode hitboxNode in xml.GetChildNodes("HitboxMetadata"))
			{
				if (!Enum.TryParse(hitboxNode.LocalName, true, out BossPuppet.ColliderOption option))
				{
					Logger.Warn("Bosses Helper", $"{hitboxNode.LocalName} is not an accepted HitboxMetadata node type.");
					continue;
				}

				metadata.Add(option, hitboxNode.GetValue("tag", "main"), option switch
				{
					BossPuppet.ColliderOption.Hitboxes or BossPuppet.ColliderOption.Hurtboxes or BossPuppet.ColliderOption.KillColliders => hitboxNode.GetAllColliders(),
					BossPuppet.ColliderOption.Bouncebox => hitboxNode.GetHitbox(6f),
					BossPuppet.ColliderOption.Target => hitboxNode.GetCircle(),
					_ => null
				});
			}
			return metadata;
		}
		#endregion

		#region Lua Files
		public record LuaPathReader(string Path, Func<string, BossController, IBossAction> Creator);

		public static Dictionary<string, IBossAction> ReadLuaFiles(
			this BossController controller, params LuaPathReader[] readers)
		{
			Dictionary<string, IBossAction> actions = [];
			foreach (var (path, creator) in readers)
			{
				if (TryGetLuaAsset(path, false, out ModAsset luaFiles))
				{
					foreach (ModAsset luaFile in luaFiles.Children)
					{
						IBossAction action = creator(luaFile.PathVirtual, controller);
						if (!actions.TryAdd(luaFile.PathVirtual[(path.Length + 1)..], action))
							Logger.Error("Bosses Helper", "Two Lua files with the same name were given.");
					}
				}
			}
			return actions;
		}

		public static T ReadLuaFilePath<T>(string filepath, Func<string, T> parser)
		{
			return parser(TryGetLuaAsset(filepath, true, out ModAsset saveFile) ? saveFile.PathVirtual : null);
		}

		private static bool TryGetLuaAsset(string path, bool isFile, out ModAsset asset)
		{
			if (isFile)
				path = CleanPath(path, ".lua");
			if (!Everest.Content.TryGet(path, out asset))
			{
				Logger.Log(LogLevel.Info, "Bosses Helper", $"No Lua files were found in ${path}.");
				return false;
			}
			return true;
		}
		#endregion

		#region Helper Functions

		#region XML

		#region Patterns
		private static BossPattern ParseNewPattern(this XmlNode patternNode, BossController controller, PatternType nodeType)
		{
			string patternName = patternNode.GetValue("name");
			string goTo = patternNode.GetValue("goto");
			if (nodeType == PatternType.Event)
			{
				return new EventCutscene(patternName, patternNode.GetMethod(true), goTo, controller);
			}

			Hitbox trigger = patternNode.GetTriggerHitbox(controller);
			NullRange nRange = new(
				patternNode.GetValueOrDefault<uint>("minRepeat"),
				patternNode.GetValueOrDefault<uint>("repeat"),
				goTo is null ? null : 0,
				controller.Random);

			if (nodeType == PatternType.Random)
			{
				return new RandomPattern(patternName,
					[..patternNode.GetChildNodes().SelectMany(
						node => Enumerable.Repeat(
							node.GetMethod(true, true),
							Math.Max(node.GetValueOrDefault("weight", 0), 1))
						)
					],
					trigger, nRange, goTo, controller);
			}

			List<Method> methodList = [];
			List<Method> preLoopList = [];
			foreach (XmlNode action in patternNode.GetChildNodes())
			{
				switch (action.LocalName.ToLower())
				{
					case "wait":
						methodList.Add(action.GetMethod(false));
						break;
					case "loop":
						preLoopList = [.. methodList];
						methodList.Clear();
						break;
					default:
						methodList.Add(action.GetMethod(true));
						break;
				}
			}

			return new SequentialPattern(patternName, methodList, preLoopList, trigger, nRange, goTo, controller);
		}

		private static Method GetMethod(this XmlNode source, bool isFile, bool hasTime = false)
		{
			return new Method(
				isFile ? source.GetValue("file") : Method.WaitName,
				source.GetValueOrDefault<float>(!isFile || hasTime ? "time" : "")
			);
		}

		private static Hitbox GetTriggerHitbox(this XmlNode source, BossController controller)
		{
			float width = source.GetValueOrDefault("width", 0f);
			float height = source.GetValueOrDefault("height", 0f);
			if (width <= 0 || height <= 0)
				return null;

			Vector2 offset = (controller.Scene as Level)?.LevelOffset ?? Vector2.Zero;
			return new Hitbox(width, height,
				source.GetValueOrDefault("x", 0f) + offset.X, source.GetValueOrDefault("y", 0f) + offset.Y
			);
		}
		#endregion

		#region Hitbox Metadata
		private static Hitbox GetHitbox(this XmlNode source, float defaultHeight)
		{
			return new Hitbox(
				source.GetValueOrDefault("width", 8f), source.GetValueOrDefault("height", defaultHeight),
				source.GetValueOrDefault("xOffset", 0f), source.GetValueOrDefault("yOffset", 0f)
			);
		}

		private static Circle GetCircle(this XmlNode source, float defaultRadius = 4f)
		{
			return new Circle(
				source.GetValueOrDefault("radius", defaultRadius),
				source.GetValueOrDefault("xOffset", 0f), source.GetValueOrDefault("yOffset", 0f)
			);
		}

		private static ColliderList GetAllColliders(this XmlNode source)
		{
			if (!source.HasChildNodes)
				return null;

			return new([.. source.GetChildNodes().Cast<XmlElement>().Select<XmlElement, Collider>(
				opt => opt.NameEquals("circle") ? opt.GetCircle() : opt.GetHitbox(8f)
			)]);
		}
		#endregion

		private static XmlDocument GetXMLDocument(string filepath)
		{
			if (!Everest.Content.TryGet(CleanPath(filepath, ".xml"), out ModAsset asset))
				return null;

			XmlDocument xml = new();
			xml.Load(asset.Stream);
			return xml;
		}

		private static IEnumerable<XmlNode> GetChildNodes(this XmlNode source, string xpath = null)
		{
			if (!string.IsNullOrWhiteSpace(xpath))
				source = source.SelectSingleNode(xpath);

			foreach (XmlNode xmlNode in source.ChildNodes)
			{
				if (xmlNode.NodeType == XmlNodeType.Comment) continue;

				yield return xmlNode;
			}
		}

		private static T GetValueOrDefault<T>(this XmlNode source, string tag, T value) where T : struct, IParsable<T>
		{
			return source.GetValueOrDefault<T>(tag) ?? value;
		}

		private static T? GetValueOrDefault<T>(this XmlNode source, string tag) where T : struct, IParsable<T>
		{
			return source.GetValue(tag)?.Parse<T>();
		}

		private static string GetValue(this XmlNode source, string tag, string @default = null)
		{
			return source.Attributes[tag]?.Value ?? @default;
		}

		private static bool NameEquals(this XmlNode node, string compared)
		{
			return node.LocalName.Equals(compared, StringComparison.OrdinalIgnoreCase);
		}
		#endregion

		private static string CleanPath(string path, string extension)
		{
			if (path == null) return "";
			return path.EndsWith(extension) ? path[..^extension.Length] : path;
		}
		#endregion
	}
}




