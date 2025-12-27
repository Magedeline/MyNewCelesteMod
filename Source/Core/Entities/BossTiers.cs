namespace DesoloZantas.Core.Core.Entities
{
    // Tier 1 - Lowest Boss
    [CustomEntity("Ingeste/BossTier1")]
    [Tracked]
    public class BossTier1 : Boss
    {
        public BossTier1(EntityData data, Vector2 offset) : base(data, offset)
        {
            Tier = BossTier.Lowest;
            Gimmick = GimmickAbility.None;
            BossType = "BasicEnemy";
        }
    }

    // Tier 2 - Low Boss
    [CustomEntity("Ingeste/BossTier2")]
    [Tracked]
    public class BossTier2 : Boss
    {
        public BossTier2(EntityData data, Vector2 offset) : base(data, offset)
        {
            Tier = BossTier.Low;
            Gimmick = GimmickAbility.Teleport;
            BossType = "ElementalGuardian";
        }
    }

    // Tier 3 - Mid Boss
    [CustomEntity("Ingeste/BossTier3")]
    [Tracked]
    public class BossTier3 : Boss
    {
        public BossTier3(EntityData data, Vector2 offset) : base(data, offset)
        {
            Tier = BossTier.Mid;
            Gimmick = GimmickAbility.TimeFreeze;
            BossType = "ShadowWarrior";
        }
    }

    // Tier 4 - High Boss
    [CustomEntity("Ingeste/BossTier4")]
    [Tracked]
    public class BossTier4 : Boss
    {
        public BossTier4(EntityData data, Vector2 offset) : base(data, offset)
        {
            Tier = BossTier.High;
            Gimmick = GimmickAbility.ElementalFusion;
            BossType = "CrystalLord";
        }
    }

    // Tier 5 - Highest Boss
    [CustomEntity("Ingeste/BossTier5")]
    [Tracked]
    public class BossTier5 : Boss
    {
        public BossTier5(EntityData data, Vector2 offset) : base(data, offset)
        {
            Tier = BossTier.Highest;
            Gimmick = GimmickAbility.GravityControl;
            BossType = "VoidKnight";
        }
    }
}



