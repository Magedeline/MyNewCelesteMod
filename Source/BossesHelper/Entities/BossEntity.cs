using DesoloZantas.Core.BossesHelper.Helpers;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	public abstract class BossEntity : Actor
	{
		public readonly Sprite Sprite;

		public BossEntity(Vector2 position, string spriteName, Vector2 spriteScale, bool collidable, Collider collider = null)
			: base(position)
		{
			Collider = collider;
			Collidable = collidable;
			if (GFX.SpriteBank.TryCreate(spriteName, out Sprite))
			{
				Sprite.Scale = spriteScale;
				Add(Sprite);
			}
		}

		public void PlayAnim(string anim)
			=> Sprite.PlayOrWarn(anim);
	}
}




