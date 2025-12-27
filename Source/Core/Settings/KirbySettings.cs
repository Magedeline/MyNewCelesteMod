using Microsoft.Xna.Framework.Input;

namespace DesoloZantas.Core.Core.Settings
{
    /// <summary>
    /// Settings for Kirby player mechanics and keybinds
    /// </summary>
    [SettingName("desolozantas_kirby_settings")]
    public class KirbySettings : EverestModuleSettings
    {
        [SettingName("desolozantas_kirby_enabled")]
        public bool KirbyModeEnabled { get; set; } = true;
        
        [SettingName("desolozantas_kirby_inhale_key")]
        public Keys InhaleKey { get; set; } = Keys.H;
        
        [SettingName("desolozantas_kirby_hover_key")]
        public Keys HoverKey { get; set; } = Keys.C;
        
        [SettingName("desolozantas_kirby_attack_key")]
        public Keys AttackKey { get; set; } = Keys.S;
        
        [SettingName("desolozantas_kirby_jump_key")]
        public Keys JumpKey { get; set; } = Keys.C;
        
        [SettingName("desolozantas_kirby_grab_key")]
        public Keys GrabKey { get; set; } = Keys.X;
        
        [SettingName("desolozantas_kirby_dash_key")]
        public Keys DashKey { get; set; } = Keys.Z;
        
        [SettingName("desolozantas_kirby_hover_duration")]
        [SettingRange(1, 10)]
        public float HoverDuration { get; set; } = 3f;
        
        [SettingName("desolozantas_kirby_movement_precision")]
        public float MovementPrecision { get; set; } = 1.2f;
        
        [SettingName("desolozantas_kirby_combat_damage")]
        [SettingRange(10, 100)]
        public int CombatDamage { get; set; } = 50;
        
        [SettingName("desolozantas_kirby_star_warrior_mode")]
        public bool StarWarriorMode { get; set; } = true;
        
        [SettingName("desolozantas_kirby_parry_window")]
        public float ParryWindow { get; set; } = 0.3f;
        
        [SettingName("desolozantas_kirby_inhale_range")]
        [SettingRange(32, 128)]
        public int InhaleRange { get; set; } = 64;
        
        // Runtime keybind tracking
        private Dictionary<string, Keys> currentKeybinds = new Dictionary<string, Keys>();
        
        public void UpdateKeybinds()
        {
            currentKeybinds["Inhale"] = InhaleKey;
            currentKeybinds["Hover"] = HoverKey;
            currentKeybinds["Attack"] = AttackKey;
            currentKeybinds["Jump"] = JumpKey;
            currentKeybinds["Grab"] = GrabKey;
            currentKeybinds["Dash"] = DashKey;
        }
        
        public Keys GetKeybind(string action)
        {
            UpdateKeybinds();
            return currentKeybinds.ContainsKey(action) ? currentKeybinds[action] : Keys.None;
        }
        
        public bool IsKeyPressed(string action)
        {
            var key = GetKeybind(action);
            return key != Keys.None && MInput.Keyboard.Pressed(key);
        }
        
        public bool IsKeyCheck(string action)
        {
            var key = GetKeybind(action);
            return key != Keys.None && MInput.Keyboard.Check(key);
        }
        
        public bool IsKeyReleased(string action)
        {
            var key = GetKeybind(action);
            return key != Keys.None && MInput.Keyboard.Released(key);
        }
    }
}



