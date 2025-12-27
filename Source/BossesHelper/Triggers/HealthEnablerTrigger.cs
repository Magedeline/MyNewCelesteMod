using DesoloZantas.Core.BossesHelper.Helpers;
using DesoloZantas.Core.BossesHelper.Entities;

namespace DesoloZantas.Core.BossesHelper.Triggers
{
	[CustomEntity("BossesHelper/HealthEnableTrigger")]
	public class HealthEnablerTrigger(EntityData data, Vector2 offset) : Trigger(data, offset)
	{
		private readonly bool enableState = data.Bool("enableState");

		private readonly bool pauseState = data.Bool("pauseState");

		public override void OnEnter(Player player)
		{
			base.OnEnter(player);
			ChangeManagerState();
		}

		protected void ChangeManagerState()
		{
			if (SceneAs<Level>().GetEntity<HealthSystemManager>() is HealthSystemManager manager)
			{
				if (pauseState)
					manager.SetActiveState(enableState);
				else
					manager.SetEnableState(enableState);
			}
		}
	}

	[CustomEntity("BossesHelper/HealthSystemZoneTrigger")]
	public class HealthSystemZoneTrigger(EntityData data, Vector2 offset) : HealthEnablerTrigger(data, offset)
	{
		public override void OnLeave(Player player)
		{
			base.OnLeave(player);
			ChangeManagerState();
		}
	}
}




