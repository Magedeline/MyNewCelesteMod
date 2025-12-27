using DesoloZantas.Core.Core.Player;
using static DesoloZantas.Core.BossesHelper.Helpers.BossesHelperUtils;
using HealthSystemManager = DesoloZantas.Core.BossesHelper.Entities.HealthSystemManager;

namespace DesoloZantas.Core.Core
{
    public class IngesteModuleSession : EverestModuleSession
    {
        public bool SomeSessionFlag { get; set; } = false;
        
        // Teleport pipe system data
        public Dictionary<string, Entities.TeleportPipeData> TeleportPipes { get; set; }

        #region BossesHelper Health System
        public bool BossesHelper_WasOffscreen;

        public bool BossesHelper_UseFakeDeath;

        public bool BossesHelper_FakeDeathRespawn;

        public Vector2 BossesHelper_LastSafePosition;

        public Vector2 BossesHelper_LastSpawnPoint;

        public Vector2 BossesHelper_SafeSpawn
        {
            set => BossesHelper_LastSafePosition = BossesHelper_LastSpawnPoint = value.NearestWhole();
        }

        public bool BossesHelper_AlreadyFlying;

        public struct HealthSystemData
        {
            //Overhead
            public bool isCreated;

            public bool isEnabled;

            //Damage
            public bool activateInstantly;

            public bool globalController;

            public bool globalHealth;

            public string activateFlag;

            public int playerHealthVal;

            public float damageCooldown;

            public bool playerStagger;

            public bool playerBlink;

            public string onDamageFunction;

            public HealthSystemManager.CrushEffect playerOnCrush;

            public HealthSystemManager.OffscreenEffect playerOffscreen;

            public string fakeDeathMethods;

            public readonly List<string> FakeDeathMethods => SeparateList(fakeDeathMethods);

            //Visual
            public string iconSprite;

            public string startAnim;

            public string endAnim;

            public string iconSeparation;

            public string frameSprite;

            public Vector2 healthBarPos;

            public Vector2 healthIconScale;

            public bool startVisible;

            public bool removeOnDamage;
        }

        public HealthSystemData BossesHelper_HealthData;
        
        // Lowercase aliases for BossesHelper code compatibility
        public HealthSystemData healthData
        {
            get => BossesHelper_HealthData;
            set => BossesHelper_HealthData = value;
        }

        public int BossesHelper_CurrentPlayerHealth;
        
        public int currentPlayerHealth
        {
            get => BossesHelper_CurrentPlayerHealth;
            set => BossesHelper_CurrentPlayerHealth = value;
        }

        public bool BossesHelper_SafeGroundBlockerCreated;
        
        public bool safeGroundBlockerCreated
        {
            get => BossesHelper_SafeGroundBlockerCreated;
            set => BossesHelper_SafeGroundBlockerCreated = value;
        }

        public bool BossesHelper_GlobalSafeGroundBlocker;
        
        public bool globalSafeGroundBlocker
        {
            get => BossesHelper_GlobalSafeGroundBlocker;
            set => BossesHelper_GlobalSafeGroundBlocker = value;
        }
        #endregion

        #region BossesHelper Boss Phase
        public readonly record struct BossPhase(int BossHealthAt, bool StartImmediately, int StartWithPatternIndex);

        public Dictionary<string, BossPhase> BossesHelper_BossPhasesSaved = [];
        
        public Dictionary<string, BossPhase> BossPhasesSaved => BossesHelper_BossPhasesSaved;
        #endregion

        #region BossesHelper Global Save Point
        public bool BossesHelper_SavePointSet;
        
        public bool savePointSet
        {
            get => BossesHelper_SavePointSet;
            set => BossesHelper_SavePointSet = value;
        }

        public bool BossesHelper_TravelingToSavePoint;

        public string BossesHelper_SavePointLevel;
        
        public string savePointLevel
        {
            get => BossesHelper_SavePointLevel;
            set => BossesHelper_SavePointLevel = value;
        }

        public Vector2 BossesHelper_SavePointSpawn;
        
        public Vector2 savePointSpawn
        {
            get => BossesHelper_SavePointSpawn;
            set => BossesHelper_SavePointSpawn = value;
        }

        public IntroTypes BossesHelper_SavePointSpawnType;
        
        public IntroTypes savePointSpawnType
        {
            get => BossesHelper_SavePointSpawnType;
            set => BossesHelper_SavePointSpawnType = value;
        }
        #endregion

        public IngesteModuleSession()
        {
            Load();
        }

        public void Load()
        {
            // Initialize session-specific data here
            SomeSessionFlag = false;
            TeleportPipes = new Dictionary<string, Entities.TeleportPipeData>();
        }
    }
}



