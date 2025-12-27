using DesoloZantas.Core.BossesHelper.Helpers.Lua;
using NLua;

namespace DesoloZantas.Core.BossesHelper.Components
{
	public class EntityFlagger(string flag, Action<Entity> action, bool stateNeeded = true, bool resetFlag = false)
		: StateChecker(action, stateNeeded)
	{
		public EntityFlagger(string flag, LuaFunction action, bool stateNeeded = true, bool resetFlag = false)
			: this(flag, action.ToAction<Entity>(), stateNeeded, resetFlag) { }

		protected override bool StateCheck()
		{
			Session session = SceneAs<Level>().Session;
			bool result = session.GetFlag(flag);
			if (result && resetFlag)
			{
				session.SetFlag(flag, !state);
			}
			return result;
		}
	}
}




