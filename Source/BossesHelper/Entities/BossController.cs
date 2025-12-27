using DesoloZantas.Core.BossesHelper.Components;
using DesoloZantas.Core.BossesHelper.Helpers;
using DesoloZantas.Core.Core;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;
using static DesoloZantas.Core.BossesHelper.Helpers.UserFileReader;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	[CustomEntity("BossesHelper/BossController")]
	public partial class BossController : Entity
	{
		public readonly string BossID;

		public readonly BossPuppet Puppet;

		private readonly bool startAttackingImmediately;

		private readonly Coroutine PatternCoroutine;

		private readonly List<Entity> activeEntities = new List<Entity>();

		private readonly List<BossPattern> AllPatterns = new List<BossPattern>();

		private readonly Dictionary<string, int> NamedPatterns = new Dictionary<string, int>();

		public int Health;

		private bool playerHasMoved;

		private Dictionary<string, IBossAction> BossActions;

		private BossFunctions BossFunctionsInstance;

		public Random Random { get; private set; }

		public int CurrentPatternIndex { get; private set; }

		private BossPattern CurrentPattern => AllPatterns[CurrentPatternIndex];

		public BossController(EntityData data, Vector2 offset, EntityID id)
			: base(data.Position + offset)
		{
			SourceData = data;
			SourceId = id;
			BossID = data.Attr("bossID");
			Add(PatternCoroutine = new Coroutine(false));
			Puppet = BossPuppet.Create(data.Enum<BossPuppet.HurtModes>("hurtMode"), data, offset);
			Puppet.Add(new BossHealthTracker(GetHealth));
			if (IngesteModule.Session.BossPhasesSaved.TryGetValue(BossID, out IngesteModuleSession.BossPhase phase))
			{
				Health = phase.BossHealthAt;
				CurrentPatternIndex = phase.StartWithPatternIndex;
				startAttackingImmediately = phase.StartImmediately;
			}
			else
			{
				Health = data.Int("bossHealthMax", -1);
				startAttackingImmediately = data.Bool("startAttackingImmediately");
			}
			Everest.Events.Player.OnDie += _ => CurrentPattern.EndAction(BossAttack.EndReason.PlayerDied);
		}

		public override void Added(Scene scene)
		{
			base.Added(scene);
			Scene.Add(Puppet);
			int tasSeed = IngesteModule.Instance.TASSeed;
			int generalSeed = tasSeed > 0 ? tasSeed : (int)Math.Floor(Scene.TimeActive);
			Random = new Random(generalSeed * 37 + new Crc32().Get(SourceId.Key));
		}

		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			BossActions = this.ReadLuaFiles(
				new(SourceData.Attr("attacksPath"), BossAttack.Create),
				new(SourceData.Attr("eventsPath"), BossEvent.Create)
			);
			BossFunctionsInstance = ReadLuaFilePath(SourceData.Attr("functionsPath"), path => new BossFunctions(path, this));
			AllPatterns.AddRange(ReadPatternFile(SourceData.Attr("patternsPath"), this));
			Puppet.BossFunctions = BossFunctionsInstance;
			for (int i = 0; i < AllPatterns.Count; i++)
			{
				if (AllPatterns[i].Name is string name)
					NamedPatterns.Add(name, i);
			}
		}

		public override void Removed(Scene scene)
		{
			base.Removed(scene);
			DestroyAll();
			Puppet.RemoveSelf();
		}

		public override void Update()
		{
			base.Update();
			if (Scene.GetPlayer() is Player entity)
			{
				bool IsActing = CurrentPattern.IsActing;
				if (!playerHasMoved && (entity.Speed != Vector2.Zero || startAttackingImmediately || IsActing))
				{
					playerHasMoved = true;
					if (!IsActing)
						StartAttackPattern(CurrentPatternIndex);
				}
				if (!IsActing && IsPlayerWithinSpecifiedRegion(entity.Position))
				{
					InterruptPattern();
					ChangeToPattern();
				}
			}
		}

		private bool IsPlayerWithinSpecifiedRegion(Vector2 entityPos)
		{
			return CurrentPattern is AttackPattern attack
				&& attack.PlayerPositionTrigger is Collider positionTrigger
				&& positionTrigger.Collide(entityPos);
		}

		public void DestroyAll()
		{
			activeEntities.ForEach(entity => entity.RemoveSelf());
			activeEntities.Clear();
		}

		public void InterruptPattern()
		{
			PatternCoroutine.Active = false;
			CurrentPattern.EndAction(BossAttack.EndReason.Interrupted);
		}

		public void StartAttackPattern(int goTo = -1)
		{
			if (goTo >= AllPatterns.Count)
			{
				CurrentPatternIndex = -1;
				PatternCoroutine.Active = false;
				return;
			}
			if (goTo > 0)
			{
				CurrentPatternIndex = goTo;
			}
			PatternCoroutine.Replace(CurrentPattern.Perform());
		}

		internal bool TryGet(string key, out IBossAction action)
		{
			return BossActions.TryGetValue(key, out action);
		}

		internal void ChangeToPattern()
		{
			StartAttackPattern(CurrentPattern.GoToPattern.TryParse(out int index) ? index :
				NamedPatterns.GetValueOrDefault(CurrentPattern.GoToPattern ?? "", CurrentPatternIndex + 1));
		}

		private int GetHealth()
		{
			return Health;
		}
	}
}




