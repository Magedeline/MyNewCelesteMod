namespace DesoloZantas.Core.BossesHelper.Components
{
	public class EntityChain(Entity chained, bool startChained, bool removeTogether = false)
		: Component(startChained, visible: false)
	{
		public Vector2 ChainOffset;

		public override void Added(Entity entity)
		{
			base.Added(entity);
			CreateOffset();
		}

		public override void Removed(Entity entity)
		{
			base.Removed(entity);
			if (removeTogether)
				chained.RemoveSelf();
		}

		public override void Update()
		{
			chained.Position = Entity.Position + ChainOffset;
		}

		public void ReconnectChain(bool updateOffset)
		{
			if (Active)
				return;

			if (updateOffset)
				CreateOffset();

			Active = true;
		}

		private void CreateOffset()
			=> ChainOffset = chained.Position - Entity.Position;
	}
}




