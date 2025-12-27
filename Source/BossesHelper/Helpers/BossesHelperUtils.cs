using DesoloZantas.Core.BossesHelper.Components;
using DesoloZantas.Core.BossesHelper.Entities;
using DesoloZantas.Core.Core;

namespace DesoloZantas.Core.BossesHelper.Helpers
{
	public static class BossesHelperUtils
	{
		#region Extensions
		#region Scene
		public static Player GetPlayer(this Scene scene)
		{
			return scene.GetEntity<Player>();
		}

		public static T GetEntity<T>(this Scene scene) where T : Entity => scene?.Tracker.GetEntity<T>();

		public static void DoNotLoad(this Scene scene, EntityID entityID)
		{
			(scene as Level).Session.DoNotLoad.Add(entityID);
		}
		#endregion

		#region Sprite
		public static IEnumerator PlayAnim(this Sprite sprite, string anim)
		{
			bool singleLoop = false;
			void waitUntilDone(string _) => singleLoop = true;
			if (sprite is null) yield break;
			sprite.OnLastFrame += waitUntilDone;
			if (sprite.TryPlay(anim))
				yield return While(() => singleLoop && sprite.Animating);
			sprite.OnLastFrame -= waitUntilDone;
		}

		public static void PlayOrWarn(this Sprite sprite, string anim)
		{
			if (!sprite.TryPlay(anim))
			{
				Logger.Warn("BossesHelper", "Animation specified does not exist!");
			}
		}

		public static bool TryPlay(this Sprite sprite, string anim)
		{
			if (!sprite.Has(anim)) return false;
			sprite.Play(anim);
			return true;
		}
		#endregion

		#region SpriteBank
		public static Sprite TryCreate(this SpriteBank spriteBank, string id)
		{
			try
			{
				return spriteBank.Create(id);
			}
			catch (Exception)
			{
				return new Sprite(GFX.Game, "");
			}
		}

		public static bool TryCreate(this SpriteBank spriteBank, string id, out Sprite sprite)
		{
			try
			{
				sprite = spriteBank.Create(id);
				return true;
			}
			catch (Exception)
			{
				sprite = new Sprite(GFX.Game, "");
				return false;
			}
		}
		#endregion

		#region string
		public static T Parse<T>(this string value) where T : IParsable<T>
		{
			return T.Parse(value, null);
		}

		public static bool TryParse<T>(this string value, out T result) where T : IParsable<T>
		{
			return T.TryParse(value, null, out result);
		}

		public enum SplitMode
		{
			Exclude,
			IncludeFirst,
			IncludeLast
		}

		public static (string, string) SplitOnce(this string val, char character, bool forward = true, SplitMode mode = SplitMode.Exclude, string preDefault = null)
		{
			int index = forward ? val.IndexOf(character) : val.LastIndexOf(character);
			if (index == -1)
				return (preDefault, val);
			if (mode == SplitMode.IncludeFirst)
				index++;
			string first = val[..index];
			if (mode == SplitMode.Exclude)
				index++;
			string second = val[index..];
			return (first, second);
		}
		#endregion

		#region Entity
		public static bool Move(this Actor self, Vector2 offset, Collision onCollideV = null, Collision onCollideH = null)
		{
			return self.MoveH(offset.X, onCollideH) ||
				   self.MoveV(offset.Y, onCollideV);
		}

		public static Tween PositionTween(this Entity self, Vector2 target, float time, Ease.Easer easer = null)
		{
			return Tween.Position(self, target, time, easer);
		}

		public static Tween PositionTween(
			this Actor self, Vector2 target, float time, bool actNaive = false, bool stopOnCollide = false,
			Collision collisionH = null, Collision collisionV = null, Ease.Easer easer = null
		)
		{
			Vector2 startPosition = self.Position;
			Tween tween = Tween.Create(Tween.TweenMode.Oneshot, easer, time, start: true);
			tween.OnUpdate = t =>
			{
				Vector2 delta = Vector2.Lerp(startPosition, target, t.Eased) - self.Position;
				if (actNaive)
				{
					self.NaiveMove(delta);
				}
				else
				{
					if (self.Move(delta, collisionV, collisionH) && stopOnCollide)
						tween.Stop();
				}
			};
			self.Add(tween);
			return tween;
		}

		public static Tween PositionTween(this BossActor self, Vector2 target, float time, bool actNaive = false, bool stopOnCollide = false, Ease.Easer easer = null)
		{
			return self.PositionTween(target, time, actNaive, stopOnCollide, self.OnCollideH, self.OnCollideV, easer);
		}

		public static void ChangeTagState(this Entity entity, int tag, bool state)
		{
			if (state)
				entity.AddTag(tag);
			else
				entity.RemoveTag(tag);
		}
		#endregion

		public static void AddIFramesWatch(this Player player, bool onlyOnNull = false)
		{
			if (player.Get<Stopwatch>() is Stopwatch watch)
			{
				if (onlyOnNull)
					return;
				watch.RemoveSelf();
			}
			player.Add(new Stopwatch(IngesteModule.Session.healthData.damageCooldown));
		}

		public static T ElementAtOrLast<T>(this IList<T> list, int index)
		{
			return index >= 0 && index < list.Count ? list[index] : list.LastOrDefault();
		}

		public static Vector2 NearestWhole(this Vector2 value)
		{
			return new Vector2((int)value.X, (int)value.Y);
		}

		public static void AsCoroutine(this IEnumerator enumerator, Entity target)
		{
			target.Add(new Coroutine(enumerator));
		}

		public static T[] GetAttributes<T>(this object self, bool findInherited = false) where T : Attribute
		{
			return (T[])self.GetType().GetCustomAttributes(typeof(T), findInherited);
		}
		#endregion

		#region Classes
		public class EnumDict<TKey, TValue> : Dictionary<TKey, TValue> where TKey : struct, Enum
		{
			public EnumDict(Func<TKey, TValue> populator) : base()
			{
				foreach (var option in Enum.GetValues<TKey>())
				{
					Add(option, populator(option));
				}
			}
		}

		public class Crc32
		{
			private const UInt32 s_generator = 0xEDB88320;

			private readonly UInt32[] m_checksumTable = [.. Enumerable.Range(0, 256).Select(i =>
			{
				var tableEntry = (uint)i;
				for (var j = 0; j < 8; ++j)
				{
					tableEntry = ((tableEntry & 1) != 0)
						? (s_generator ^ (tableEntry >> 1))
						: (tableEntry >> 1);
				}
				return tableEntry;
			})];

			public int Get(string value)
			{
				IEnumerable<char> byteStream = value.ToCharArray();
				return (int)~byteStream.Aggregate(0xFFFFFFFF, (checksumRegister, currentByte) =>
							(m_checksumTable[(checksumRegister & 0xFF) ^ Convert.ToByte(currentByte)] ^ (checksumRegister >> 8)));
			}
		}

		public class NullRange(uint? min, uint? max, uint? @default, Random random, int randomChance = 50)
		{
			private readonly uint? MinRange = min ?? max ?? @default;

			private readonly uint? MaxRange = MaxMin(max ?? @default, min ?? @default);

			private readonly bool Defined = (min ?? max ?? @default) >= 0;

			private readonly int Chance = Math.Clamp(randomChance, 0, 100);

			public readonly Random Random = random;

			public int Counter { get; private set; }

			public bool CanContinue
				=> !Defined || Counter <= MinRange || Counter <= MaxRange && Random.Next(100) < Chance;

			public void Reset() => Counter = 0;

			public void Inc()
			{
				Counter++;
			}

			private static uint? MaxMin(uint? max, uint? min)
			{
				return min > max ? min : max;
			}
		}
		#endregion

		public enum Alignment
		{
			Left = -1,
			Center,
			Right
		}

		public static float DistanceBetween(Vector2 start, Vector2 end)
		{
			var dx = start.X - end.X;
			var dy = start.Y - end.Y;
			return (float)Math.Sqrt(dx * dx + dy * dy);
		}

		public static List<string> SeparateList(string listString)
		{
			List<string> res = listString?.Split(',').ToList() ?? [];
			for (int i = 0; i < res.Count; i++)
			{
				res[i] = res[i].Trim();
			}
			return res;
		}

		public static List<float> SeparateFloatList(string listString)
		{
			return [.. SeparateList(listString).Select(float.Parse)];
		}

		public static IEnumerator While(Func<bool> checker, bool doWhile = false)
		{
			if (!doWhile && !checker())
				yield break;

			do
				yield return null;
			while (checker());
		}
	}
}




