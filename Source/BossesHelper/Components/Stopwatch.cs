namespace DesoloZantas.Core.BossesHelper.Components
{
	public class Stopwatch(float fullTime) : Component(true, false)
	{
		public float TimeLeft { get; set; }

		public bool Finished => TimeLeft <= 0;

		public void Reset()
		{
			TimeLeft = fullTime;
		}

		public override void Update()
		{
			if (TimeLeft > 0)
				TimeLeft -= Engine.DeltaTime;
		}
	}
}




