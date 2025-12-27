using DesoloZantas.Core.BossesHelper.Helpers.Lua;
using NLua;

namespace DesoloZantas.Core.BossesHelper.Components
{
	public abstract class StateChecker(Action<Entity> action, bool stateNeeded = true, bool removeOnComplete = true)
		: Component(active: true, visible: false)
	{
		protected readonly bool state = stateNeeded;

		internal StateChecker(LuaFunction action, bool stateNeeded = true, bool removeOnComplete = true)
			: this(action.ToAction<Entity>(), stateNeeded, removeOnComplete) { }

		public override void Update()
		{
			if (StateCheck() == state)
			{
				action.Invoke(Entity);
				if (removeOnComplete)
					RemoveSelf();
			}
		}

		protected abstract bool StateCheck();
	}
}




