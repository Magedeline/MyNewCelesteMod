using Microsoft.Xna.Framework.Input;

namespace DesoloZantas.Core.Core.Settings
{
    /// <summary>
    /// Customizable keybind configuration for Kirby mechanics
    /// Supports both keyboard and controller inputs
    /// </summary>
    public class KirbyKeybinds
    {
        // Keyboard bindings
        public Keys InhaleKey { get; set; } = Keys.H;
        public Keys HoverKey { get; set; } = Keys.C;
        public Keys AttackKey { get; set; } = Keys.S;
        public Keys ParryKey { get; set; } = Keys.LeftShift;
        public Keys SpitKey { get; set; } = Keys.X;
        public Keys CyclePowerKey { get; set; } = Keys.Tab;
        public Keys DropPowerKey { get; set; } = Keys.Q;
        
        // Controller bindings (using Buttons enum)
        public Buttons InhaleButton { get; set; } = Buttons.X;
        public Buttons HoverButton { get; set; } = Buttons.A;
        public Buttons AttackButton { get; set; } = Buttons.B;
        public Buttons ParryButton { get; set; } = Buttons.RightShoulder;
        public Buttons SpitButton { get; set; } = Buttons.Y;
        public Buttons CyclePowerButton { get; set; } = Buttons.LeftShoulder;
        public Buttons DropPowerButton { get; set; } = Buttons.LeftTrigger;
        
        // Controller stick deadzone
        public float StickDeadzone { get; set; } = 0.3f;
        
        /// <summary>
        /// Check if a specific action key is pressed (keyboard)
        /// </summary>
        public bool IsKeyPressed(KirbyAction action)
        {
            Keys key = GetKey(action);
            return key != Keys.None && MInput.Keyboard.Pressed(key);
        }
        
        /// <summary>
        /// Check if a specific action key is being held (keyboard)
        /// </summary>
        public bool IsKeyCheck(KirbyAction action)
        {
            Keys key = GetKey(action);
            return key != Keys.None && MInput.Keyboard.Check(key);
        }
        
        /// <summary>
        /// Check if a specific action key was released (keyboard)
        /// </summary>
        public bool IsKeyReleased(KirbyAction action)
        {
            Keys key = GetKey(action);
            return key != Keys.None && MInput.Keyboard.Released(key);
        }
        
        /// <summary>
        /// Check if a specific action button is pressed (controller)
        /// </summary>
        public bool IsButtonPressed(KirbyAction action)
        {
            Buttons button = GetButton(action);
            return button != 0 && MInput.GamePads[0].Pressed(button);
        }
        
        /// <summary>
        /// Check if a specific action button is being held (controller)
        /// </summary>
        public bool IsButtonCheck(KirbyAction action)
        {
            Buttons button = GetButton(action);
            return button != 0 && MInput.GamePads[0].Check(button);
        }
        
        /// <summary>
        /// Check if a specific action button was released (controller)
        /// </summary>
        public bool IsButtonReleased(KirbyAction action)
        {
            Buttons button = GetButton(action);
            return button != 0 && MInput.GamePads[0].Released(button);
        }
        
        /// <summary>
        /// Check if a specific action is pressed (either keyboard or controller)
        /// </summary>
        public bool IsActionPressed(KirbyAction action)
        {
            return IsKeyPressed(action) || IsButtonPressed(action);
        }
        
        /// <summary>
        /// Check if a specific action is being held (either keyboard or controller)
        /// </summary>
        public bool IsActionCheck(KirbyAction action)
        {
            return IsKeyCheck(action) || IsButtonCheck(action);
        }
        
        /// <summary>
        /// Check if a specific action was released (either keyboard or controller)
        /// </summary>
        public bool IsActionReleased(KirbyAction action)
        {
            return IsKeyReleased(action) || IsButtonReleased(action);
        }
        
        /// <summary>
        /// Get the keyboard key for a specific action
        /// </summary>
        public Keys GetKey(KirbyAction action)
        {
            return action switch
            {
                KirbyAction.Inhale => InhaleKey,
                KirbyAction.Hover => HoverKey,
                KirbyAction.Attack => AttackKey,
                KirbyAction.Parry => ParryKey,
                KirbyAction.Spit => SpitKey,
                KirbyAction.CyclePower => CyclePowerKey,
                KirbyAction.DropPower => DropPowerKey,
                _ => Keys.None
            };
        }
        
        /// <summary>
        /// Get the controller button for a specific action
        /// </summary>
        public Buttons GetButton(KirbyAction action)
        {
            return action switch
            {
                KirbyAction.Inhale => InhaleButton,
                KirbyAction.Hover => HoverButton,
                KirbyAction.Attack => AttackButton,
                KirbyAction.Parry => ParryButton,
                KirbyAction.Spit => SpitButton,
                KirbyAction.CyclePower => CyclePowerButton,
                KirbyAction.DropPower => DropPowerButton,
                _ => 0
            };
        }
        
        /// <summary>
        /// Set the keyboard key for a specific action
        /// </summary>
        public void SetKey(KirbyAction action, Keys key)
        {
            switch (action)
            {
                case KirbyAction.Inhale:
                    InhaleKey = key;
                    break;
                case KirbyAction.Hover:
                    HoverKey = key;
                    break;
                case KirbyAction.Attack:
                    AttackKey = key;
                    break;
                case KirbyAction.Parry:
                    ParryKey = key;
                    break;
                case KirbyAction.Spit:
                    SpitKey = key;
                    break;
                case KirbyAction.CyclePower:
                    CyclePowerKey = key;
                    break;
                case KirbyAction.DropPower:
                    DropPowerKey = key;
                    break;
            }
        }
        
        /// <summary>
        /// Set the controller button for a specific action
        /// </summary>
        public void SetButton(KirbyAction action, Buttons button)
        {
            switch (action)
            {
                case KirbyAction.Inhale:
                    InhaleButton = button;
                    break;
                case KirbyAction.Hover:
                    HoverButton = button;
                    break;
                case KirbyAction.Attack:
                    AttackButton = button;
                    break;
                case KirbyAction.Parry:
                    ParryButton = button;
                    break;
                case KirbyAction.Spit:
                    SpitButton = button;
                    break;
                case KirbyAction.CyclePower:
                    CyclePowerButton = button;
                    break;
                case KirbyAction.DropPower:
                    DropPowerButton = button;
                    break;
            }
        }
        
        /// <summary>
        /// Reset all keybinds to defaults
        /// </summary>
        public void ResetToDefaults()
        {
            // Keyboard defaults
            InhaleKey = Keys.H;
            HoverKey = Keys.C;
            AttackKey = Keys.S;
            ParryKey = Keys.LeftShift;
            SpitKey = Keys.X;
            CyclePowerKey = Keys.Tab;
            DropPowerKey = Keys.Q;
            
            // Controller defaults
            InhaleButton = Buttons.X;
            HoverButton = Buttons.A;
            AttackButton = Buttons.B;
            ParryButton = Buttons.RightShoulder;
            SpitButton = Buttons.Y;
            CyclePowerButton = Buttons.LeftShoulder;
            DropPowerButton = Buttons.LeftTrigger;
            
            StickDeadzone = 0.3f;
        }
        
        /// <summary>
        /// Get display name for a key
        /// </summary>
        public static string GetKeyDisplayName(Keys key)
        {
            return key switch
            {
                Keys.None => "None",
                Keys.LeftShift => "Left Shift",
                Keys.RightShift => "Right Shift",
                Keys.LeftControl => "Left Ctrl",
                Keys.RightControl => "Right Ctrl",
                Keys.LeftAlt => "Left Alt",
                Keys.RightAlt => "Right Alt",
                Keys.Space => "Space",
                Keys.Enter => "Enter",
                Keys.Back => "Backspace",
                _ => key.ToString()
            };
        }
        
        /// <summary>
        /// Get display name for a button
        /// </summary>
        public static string GetButtonDisplayName(Buttons button)
        {
            return button switch
            {
                0 => "None",
                Buttons.A => "A",
                Buttons.B => "B",
                Buttons.X => "X",
                Buttons.Y => "Y",
                Buttons.LeftShoulder => "LB",
                Buttons.RightShoulder => "RB",
                Buttons.LeftTrigger => "LT",
                Buttons.RightTrigger => "RT",
                Buttons.LeftStick => "L3",
                Buttons.RightStick => "R3",
                Buttons.DPadUp => "D-Pad Up",
                Buttons.DPadDown => "D-Pad Down",
                Buttons.DPadLeft => "D-Pad Left",
                Buttons.DPadRight => "D-Pad Right",
                Buttons.Start => "Start",
                Buttons.Back => "Select",
                _ => button.ToString()
            };
        }
    }
    
    /// <summary>
    /// Available Kirby actions that can be bound to keys/buttons
    /// </summary>
    public enum KirbyAction
    {
        Inhale,
        Hover,
        Attack,
        Parry,
        Spit,
        CyclePower,
        DropPower
    }
}




