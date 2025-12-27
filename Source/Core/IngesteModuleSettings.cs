using System.ComponentModel;
using Microsoft.Xna.Framework.Input;
using DesoloZantas.Core.Core.Settings;

namespace DesoloZantas.Core.Core;

public class IngesteModuleSettings : EverestModuleSettings
{
    // KirbySettings adapter - provides compatibility with legacy code expecting a KirbySettings object
    [SettingIgnore]
    public KirbySettings KirbySettings => new KirbySettings
    {
        KirbyModeEnabled = KirbyPlayerEnabled,
        InhaleKey = KirbyInhaleKey,
        HoverKey = KirbyHoverKey,
        AttackKey = KirbyAttackKey,
        HoverDuration = KirbyHoverDuration,
        InhaleRange = InhaleRange,
        CombatDamage = KirbyCombatDamage,
        ParryWindow = KirbyParryWindow,
        MovementPrecision = 1.0f,
        StarWarriorMode = KirbyStarWarriorMode
    };
    
    // ============================================
    // KIRBY PLAYER KEYBINDS
    // ============================================
    
    // Kirby Keyboard Bindings
    [SettingName("KIRBY_INHALE_KEY")]
    [DefaultValue(Keys.H)]
    public Keys KirbyInhaleKey { get; set; } = Keys.H;
    
    [SettingName("KIRBY_HOVER_KEY")]
    [DefaultValue(Keys.C)]
    public Keys KirbyHoverKey { get; set; } = Keys.C;
    
    [SettingName("KIRBY_ATTACK_KEY")]
    [DefaultValue(Keys.S)]
    public Keys KirbyAttackKey { get; set; } = Keys.S;
    
    [SettingName("KIRBY_PARRY_KEY")]
    [DefaultValue(Keys.LeftShift)]
    public Keys KirbyParryKey { get; set; } = Keys.LeftShift;
    
    [SettingName("KIRBY_SPIT_KEY")]
    [DefaultValue(Keys.X)]
    public Keys KirbySpitKey { get; set; } = Keys.X;
    
    [SettingName("KIRBY_CYCLE_POWER_KEY")]
    [DefaultValue(Keys.Tab)]
    public Keys KirbyCyclePowerKey { get; set; } = Keys.Tab;
    
    [SettingName("KIRBY_DROP_POWER_KEY")]
    [DefaultValue(Keys.Q)]
    public Keys KirbyDropPowerKey { get; set; } = Keys.Q;
    
    // Kirby Controller Bindings
    [DefaultButtonBinding(Buttons.X, Keys.H)]
    [SettingName("KIRBY_INHALE_BUTTON")]
    public ButtonBinding KirbyInhaleButton { get; set; } = new ButtonBinding();
    
    [DefaultButtonBinding(Buttons.A, Keys.C)]
    [SettingName("KIRBY_HOVER_BUTTON")]
    public ButtonBinding KirbyHoverButton { get; set; } = new ButtonBinding();
    
    [DefaultButtonBinding(Buttons.B, Keys.S)]
    [SettingName("KIRBY_ATTACK_BUTTON")]
    public ButtonBinding KirbyAttackButton { get; set; } = new ButtonBinding();
    
    [DefaultButtonBinding(Buttons.RightShoulder, Keys.LeftShift)]
    [SettingName("KIRBY_PARRY_BUTTON")]
    public ButtonBinding KirbyParryButton { get; set; } = new ButtonBinding();
    
    [DefaultButtonBinding(Buttons.Y, Keys.X)]
    [SettingName("KIRBY_SPIT_BUTTON")]
    public ButtonBinding KirbySpitButton { get; set; } = new ButtonBinding();
    
    [DefaultButtonBinding(Buttons.LeftShoulder, Keys.Tab)]
    [SettingName("KIRBY_CYCLE_POWER_BUTTON")]
    public ButtonBinding KirbyCyclePowerButton { get; set; } = new ButtonBinding();
    
    [DefaultButtonBinding(Buttons.LeftTrigger, Keys.Q)]
    [SettingName("KIRBY_DROP_POWER_BUTTON")]
    public ButtonBinding KirbyDropPowerButton { get; set; } = new ButtonBinding();
    
    // Kirby Player Mode - when enabled, Kirby is the playable character
    [SettingName("KIRBY_PLAYER_MODE")]
    [SettingSubText("When enabled, Kirby becomes the playable character instead of Madeline")]
    [DefaultValue(false)]
    public bool KirbyPlayerMode { get; set; } = false;
    
    // NPC Settings
    [SettingName("NPC_INTERACTION_RADIUS")]
    [SettingRange(16, 64)]
    [DefaultValue(32)]
    public int NpcInteractionRadius { get; set; } = 32;

    [SettingName("NPC_AUTO_TRIGGER")]
    [DefaultValue(false)]
    public bool NpcAutoTrigger { get; set; } = false;

    // Kirby Player Settings
    [SettingName("KIRBY_PLAYER_ENABLED")]
    [DefaultValue(true)]
    public bool KirbyPlayerEnabled { get; set; } = true;

    /// <summary>Maximum health for Kirby form.</summary>
    [SettingName("KIRBY_MAX_HEALTH")]
    [SettingRange(1, 20)]
    [DefaultValue(6)]
    public int KirbyMaxHealth { get; set; } = 6;

    [SettingName("INHALE_HOLD_MODE")]
    [DefaultValue(false)]
    public bool InhaleHoldMode { get; set; } = false;

    [SettingName("HOVER_HOLD_MODE")]
    [DefaultValue(false)]
    public bool HoverHoldMode { get; set; } = false;

    [SettingName("INHALE_RANGE")]
    [SettingRange(32, 128)]
    [DefaultValue(64)]
    public int InhaleRange { get; set; } = 64;

    [SettingName("HOVER_FALL_SPEED")]
    [SettingRange(30, 120)]
    [DefaultValue(60)]
    public int HoverFallSpeed { get; set; } = 60;
    
    [SettingName("KIRBY_HOVER_DURATION")]
    [SettingRange(1, 10)]
    [DefaultValue(3)]
    public int KirbyHoverDuration { get; set; } = 3;
    
    [SettingName("KIRBY_COMBAT_DAMAGE")]
    [SettingRange(10, 100)]
    [DefaultValue(50)]
    public int KirbyCombatDamage { get; set; } = 50;
    
    [SettingName("KIRBY_STAR_WARRIOR_MODE")]
    [DefaultValue(true)]
    public bool KirbyStarWarriorMode { get; set; } = true;
    
    [SettingName("KIRBY_PARRY_WINDOW")]
    [SettingRange(1, 10)]
    [DefaultValue(3)]
    public int KirbyParryWindowTenths { get; set; } = 3;
    
    // Derived property for parry window in seconds
    public float KirbyParryWindow => KirbyParryWindowTenths / 10f;

    // Boss Settings
    [SettingName("BOSS_HEALTH_MULTIPLIER")]
    [SettingRange(50, 200)]
    [DefaultValue(100)]
    public int BossHealthMultiplierPercent { get; set; } = 100;

    [SettingName("BOSS_DAMAGE_MULTIPLIER")]
    [SettingRange(50, 200)]
    [DefaultValue(100)]
    public int BossDamageMultiplierPercent { get; set; } = 100;

    // Derived properties for gameplay use
    public float BossHealthMultiplier => BossHealthMultiplierPercent / 100f;
    public float BossDamageMultiplier => BossDamageMultiplierPercent / 100f;
    public float InhaleRangeFloat => InhaleRange;
    public float HoverFallSpeedFloat => HoverFallSpeed;
    public float NpcInteractionRadiusFloat => NpcInteractionRadius;

    // Berry Settings
    [SettingName("PINK_PLATINUM_BERRY_RESPAWN")]
    [DefaultValue(true)]
    public bool DefaultPinkPlatinumBerryRespawnBehavior { get; set; } = true;

    // Editor Settings
    [SettingName("ENABLE_MAP_EDITOR_INTEGRATION")]
    [DefaultValue(false)]
    public bool EnableMapEditorIntegration { get; set; } = false;

    [SettingName("ENABLE_LIVE_RELOAD")]
    [DefaultValue(false)]
    public bool EnableLiveReload { get; set; } = false;

    // Power System Settings
    [SettingName("POWER_COPY_ENABLED")]
    [DefaultValue(true)]
    public bool PowerCopyEnabled { get; set; } = true;

    [SettingName("POWER_DURATION")]
    [SettingRange(10, 300)]
    [DefaultValue(60)]
    public int PowerDurationSeconds { get; set; } = 60;

    // Debug settings
    [SettingName("DEBUG_MODE")]
    [DefaultValue(true)]
    public bool DebugMode { get; set; } = true;

    [SettingName("SHOW_HITBOXES")]
    [DefaultValue(true)]
    public bool ShowHitboxes { get; set; } = true;

    [SettingName("LOG_VERBOSE")]
    [DefaultValue(true)]
    public bool LogVerbose { get; set; } = true;

    public bool EnableDebugMode { get; set; } = false;

    // Intro Settings
    /// <summary>
    /// When enabled, skips the "Magedeline Presents" intro screen on game startup.
    /// </summary>
    [SettingName("SKIP_INTRO_PRESENTS")]
    [DefaultValue(false)]
    public bool SkipIntroPresents { get; set; } = false;

    // BossesHelper Settings
    [DefaultButtonBinding(Buttons.LeftShoulder, Keys.Tab)]
    [SettingName("Sidekick Laser")]
    public ButtonBinding BossesHelper_SidekickLaserBind { get; set; } = new ButtonBinding();
    
    // Alias for BossesHelper code compatibility
    [SettingIgnore]
    public ButtonBinding SidekickLaserBind => BossesHelper_SidekickLaserBind;

    /// <summary>
    /// Validate and normalize settings; clamp ranges and compute derived floats.
    /// </summary>
    public void ValidateAndNormalize()
    {
        // Clamp ranges defensively in case attributes were bypassed (.NET Framework 4.8 compatible)
        NpcInteractionRadius = Math.Max(16, Math.Min(64, NpcInteractionRadius));
        KirbyMaxHealth = Math.Max(1, Math.Min(20, KirbyMaxHealth));
        InhaleRange = Math.Max(32, Math.Min(128, InhaleRange));
        HoverFallSpeed = Math.Max(30, Math.Min(120, HoverFallSpeed));
        BossHealthMultiplierPercent = Math.Max(50, Math.Min(200, BossHealthMultiplierPercent));
        BossDamageMultiplierPercent = Math.Max(50, Math.Min(200, BossDamageMultiplierPercent));
        PowerDurationSeconds = Math.Max(10, Math.Min(300, PowerDurationSeconds));

        // Derived float properties are now computed automatically via getters

        // Dev-only: disable live reload outside debug to be safe
#if !DEBUG
        if (EnableLiveReload) {
            Logger.Log(LogLevel.Warn, "Ingeste", "EnableLiveReload is intended for development builds only.");
        }
#endif
    }

    public void CreateEverestModuleSettings()
    {
        // Add settings menu entries here
    }
}



