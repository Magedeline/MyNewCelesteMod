using System.ComponentModel;
using DesoloZantas.Core.Core.Settings;
using Microsoft.Xna.Framework.Input;

namespace DesoloZantas.Core.Core;

public class IngesteModuleSettings : EverestModuleSettings
{
    // Kirby Settings
    public KirbySettings KirbySettings { get; set; } = new KirbySettings();
    
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



