using DesoloZantas.Core.Core.Player;

namespace DesoloZantas.Core.Core.Entities
{
    /// <summary>
    /// Example trigger that demonstrates how to use the PlayerInventory system
    /// to enable special abilities
    /// </summary>
    [CustomEntity("Ingeste/PlayerInventoryTrigger")]
    [Tracked]
    public class PlayerInventoryTrigger : Trigger
    {
        public enum InventoryType
        {
            Default,
            Heart,
            KirbyPlayer,
            SayGoodbye,
            TitanTowerClimbing,
            Corruption,
            TheEnd
        }

        private readonly InventoryType inventoryType;
        private readonly bool oneUse;
        private bool triggered = false;

        public PlayerInventoryTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            // Read the inventory type from entity data
            string typeString = data.Attr("inventoryType", "Default");
            if (!System.Enum.TryParse<InventoryType>(typeString, true, out inventoryType))
            {
                inventoryType = InventoryType.Default;
                IngesteLogger.Warn($"Invalid inventory type '{typeString}', defaulting to Default");
            }

            oneUse = data.Bool("oneUse", true);
        }

        public override void OnEnter(global::Celeste.Player player)
        {
            if (oneUse && triggered)
                return;

            var level = Scene as Level;
            if (level == null)
                return;

            try
            {
                ApplyInventoryType(level, inventoryType);
                triggered = true;

                // Play activation sound
                Audio.Play("event:/char/kirby/inventory_change", Position);

                // Visual effect
                level.ParticlesFG?.Emit(ParticleTypes.SparkyDust, 20, Center, Vector2.One * 20f);

                IngesteLogger.Info($"Applied inventory type: {inventoryType}");
            }
            catch (System.Exception ex)
            {
                IngesteLogger.Error(ex, $"Error applying inventory type {inventoryType}");
            }
        }

        private static void ApplyInventoryType(Level level, InventoryType type)
        {
            switch (type)
            {
                case InventoryType.Heart:
                    PlayerInventoryManager.EnableHeartPower(level);
                    break;

                case InventoryType.KirbyPlayer:
                    PlayerInventoryManager.EnableKirbyPlayer(level);
                    break;

                case InventoryType.SayGoodbye:
                    PlayerInventoryManager.EnableSayGoodbye(level);
                    break;

                case InventoryType.TitanTowerClimbing:
                    PlayerInventoryManager.EnableTitanTowerClimbing(level);
                    break;

                case InventoryType.Corruption:
                    PlayerInventoryManager.EnableCorruption(level);
                    break;

                case InventoryType.TheEnd:
                    PlayerInventoryManager.EnableTheEnd(level);
                    break;

                case InventoryType.Default:
                default:
                    PlayerInventoryManager.ResetToDefault(level);
                    break;
            }
        }

        public override void Render()
        {
            base.Render();

            // Debug visualization
            if (Engine.Commands.Open)
            {
                var color = inventoryType switch
                {
                    InventoryType.Heart => Color.Red,
                    InventoryType.KirbyPlayer => Color.Yellow,
                    InventoryType.SayGoodbye => Color.Pink,
                    InventoryType.TitanTowerClimbing => Color.Blue,
                    InventoryType.Corruption => Color.Purple,
                    InventoryType.TheEnd => Color.Gold,
                    _ => Color.White
                };

                Draw.HollowRect(Collider, color);
                
                // Display inventory type
                var font = Draw.DefaultFont;
                var text = inventoryType.ToString();
                var textSize = font.MeasureString(text);
                Draw.SpriteBatch.DrawString(font, text, Center - textSize / 2, color);
            }
        }
    }
}



