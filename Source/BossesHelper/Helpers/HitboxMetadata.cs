using DesoloZantas.Core.BossesHelper.Entities;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;

namespace DesoloZantas.Core.BossesHelper.Helpers
{
	public class HitboxMetadata
	{
		public readonly EnumDict<BossPuppet.ColliderOption, Dictionary<string, Collider>> ColliderOptions = new(_ => []);

		public Dictionary<string, Collider> this[BossPuppet.ColliderOption opt]
			=> ColliderOptions[opt];

		public void Add(BossPuppet.ColliderOption option, string tag, Collider collider)
		{
			if (collider == null || this[option].TryAdd(tag, collider))
				return;
			if (this[option][tag] is ColliderList list)
				list.Add(collider);
			else
				this[option][tag] = new ColliderList(this[option][tag], collider);
		}
	}
}




