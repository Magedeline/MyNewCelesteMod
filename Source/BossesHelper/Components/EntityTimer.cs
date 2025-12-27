using DesoloZantas.Core.BossesHelper.Helpers.Lua;
using NLua;

namespace DesoloZantas.Core.BossesHelper.Components
{
	public class EntityTimer(float timer, Action<Entity> action) : StateChecker(action)
	{
		public EntityTimer(float timer, LuaFunction action)
			: this(timer, action.ToAction<Entity>()) { }

		protected override bool StateCheck() => (timer -= Engine.DeltaTime) <= 0;

	}
}




