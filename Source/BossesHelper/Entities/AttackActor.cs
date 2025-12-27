using NLua;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	public class AttackActor : BossActor
	{
		public AttackActor(Vector2 position, Collider attackbox, LuaFunction onPlayer, bool startCollidable,
			bool solidCollidable, string spriteName, float gravMult, float maxFall, float xScale = 1f, float yScale = 1f)
			: base(position, spriteName, new Vector2(xScale, yScale), maxFall, startCollidable, solidCollidable, gravMult, attackbox)
		{
			Add(new PlayerCollider(player => onPlayer.Call(this, player)));
		}
	}
}




