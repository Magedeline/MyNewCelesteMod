using DesoloZantas.Core.BossesHelper.Helpers.Lua;
using NLua;

namespace DesoloZantas.Core.BossesHelper.Components
{
	public class EntityChecker(Func<bool> checker, Action<Entity> action, bool stateNeeded = true, bool removeOnComplete = true)
		: StateChecker(action, stateNeeded, removeOnComplete)
	{
		public EntityChecker(LuaFunction checker, LuaFunction action, bool stateNeeded = true, bool removeOnComplete = true)
			: this(checker.ToFunc<bool>(), action.ToAction<Entity>(), stateNeeded, removeOnComplete) { }

		protected override bool StateCheck() => checker();
	}
}




