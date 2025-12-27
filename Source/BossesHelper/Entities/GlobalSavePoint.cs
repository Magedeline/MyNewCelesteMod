using DesoloZantas.Core.BossesHelper.Helpers.Code.Components;
using DesoloZantas.Core.BossesHelper.Helpers.Lua;
using DesoloZantas.Core.Core.Player;
using NLua;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;
using static DesoloZantas.Core.BossesHelper.Helpers.UserFileReader;

namespace DesoloZantas.Core.BossesHelper.Entities
{
	[Tracked(false)]
	[CustomEntity("BossesHelper/PlayerSavePoint")]
	public class GlobalSavePoint : Entity, ILuaLoader
	{
		private readonly GlobalSavePointChanger Changer;

		public readonly Sprite Sprite;

		private readonly string filepath;

		private readonly EntityID entityID;

		private LuaFunction onInteract;

		public PrepareMode Mode => PrepareMode.SavePoint;

		public Dictionary<string, object> Values { get; init; }

		public GlobalSavePoint(EntityData entityData, Vector2 offset, EntityID id)
			: base(entityData.Position + offset)
		{
			entityID = id;
			Add(Changer = new(id, entityData.Nodes.FirstOrDefault(Position),
				entityData.Enum("respawnType", IntroTypes.Respawn)));
			Values = new() {
				{ "savePoint", this },
				{ "spawnPoint", Changer.sessionRespawnPoint }
			};
			filepath = entityData.String("luaFile");
			string spriteName = entityData.String("savePointSprite");
			if (GFX.SpriteBank.TryCreate(spriteName, out Sprite))
				Add(Sprite);
			Add(new TalkComponent(
				new(entityData.Int("rectXOffset"), entityData.Int("rectYOffset"), entityData.Int("rectWidth"), 8),
				new(entityData.Float("talkerXOffset"), entityData.Float("talkerYOffset")),
				OnTalk)
			{
				Enabled = true,
				PlayerMustBeFacing = false
			});
		}

		public override void Awake(Scene scene)
		{
			base.Awake(scene);
			onInteract = ReadLuaFilePath(filepath, this.LoadFile)[0];
		}

		public void OnTalk(Player _)
		{
			Changer.Update();
			Add(new Coroutine(new LuaProxyCoroutine(onInteract)));
		}
	}
}




