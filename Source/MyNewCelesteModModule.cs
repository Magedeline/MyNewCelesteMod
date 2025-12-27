using System;

namespace Celeste.Mod.MyNewCelesteMod;

public class MyNewCelesteModModule : EverestModule {
    public static MyNewCelesteModModule Instance { get; private set; }

    public override Type SettingsType => typeof(MyNewCelesteModModuleSettings);
    public static MyNewCelesteModModuleSettings Settings => (MyNewCelesteModModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(MyNewCelesteModModuleSession);
    public static MyNewCelesteModModuleSession Session => (MyNewCelesteModModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(MyNewCelesteModModuleSaveData);
    public static MyNewCelesteModModuleSaveData SaveData => (MyNewCelesteModModuleSaveData) Instance._SaveData;

    public MyNewCelesteModModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(MyNewCelesteModModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(MyNewCelesteModModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        // TODO: apply any hooks that should always be active
    }

    public override void Unload() {
        // TODO: unapply any hooks applied in Load()
    }
}