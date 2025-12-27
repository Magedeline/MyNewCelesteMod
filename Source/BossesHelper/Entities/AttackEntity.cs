using NLua;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	public class AttackEntity : BossEntity
	{
		public AttackEntity(Vector2 position, Collider attackbox, LuaFunction onPlayer, bool startCollidable, string spriteName, float xScale = 1f, float yScale = 1f)
			: base(position, spriteName, new Vector2(xScale, yScale), startCollidable, attackbox)
		{
			Add(new PlayerCollider(player => onPlayer.Call(this, player)));
		}
	}
}




