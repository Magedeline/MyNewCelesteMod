namespace DesoloZantas.Core.BossesHelper.Components
{
	[Tracked(false)]
	public class BossHealthTracker(Func<int> health) : Component(active: true, visible: false)
	{
		public readonly Func<int> Health = health;
	}
}




