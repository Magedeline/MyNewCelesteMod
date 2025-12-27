namespace DesoloZantas.Core.Core
{
    public class IngesteModuleSaveData : EverestModuleSaveData
    {
        public bool SomeUnlockedFeature { get; set; } = false;
        
        /// <summary>
        /// Whether Chapter 19 has been unlocked by completing the CH18_OUTRO cutscene
        /// </summary>
        public bool UnlockedChapter19 { get; set; } = false;

        /// <summary>
        /// Store B-sides that have been unlocked by cassette collection
        /// </summary>
        public HashSet<string> UnlockedBSideIDs { get; set; } = new HashSet<string>();

        /// <summary>
        /// Store remix extras that have been unlocked by cartridge collection
        /// </summary>
        public HashSet<string> UnlockedRemixExtraIDs { get; set; } = new HashSet<string>();

        /// <summary>
        /// Whether Chapter 19 has been completed
        /// </summary>
        public bool Chapter19Complete { get; set; } = false;

        // D-Side Postcard System
        /// <summary>
        /// D-Sides that have been completed
        /// </summary>
        public HashSet<int> DSideCompleted { get; set; } = new HashSet<int>();

        /// <summary>
        /// D-Side rewards (Heart Gem + Pink Platinum Berry) that have been collected
        /// </summary>
        public HashSet<int> DSideRewardsCollected { get; set; } = new HashSet<int>();

        // Statistics System
        /// <summary>
        /// Custom statistics for DesoloZatnas
        /// </summary>
        public DesoloZantasStatsData Statistics { get; set; } = new DesoloZantasStatsData();

        public IngesteModuleSaveData() { }

        public void Load()
        {
            // Load saved data
        }
    }

    /// <summary>
    /// Statistics data stored in save file
    /// </summary>
    public class DesoloZantasStatsData
    {
        public long TotalPlaytimeSeconds { get; set; }
        public int TotalDeaths { get; set; }
        public Dictionary<int, int> DeathsByChapter { get; set; } = new Dictionary<int, int>();
        
        public int TotalStrawberries { get; set; }
        public int HeartsCollected { get; set; }
        public int CrystalHeartsCollected { get; set; }
        public int GoldenStrawberries { get; set; }
        public int PinkPlatinumBerries { get; set; }
        public int CassettesCollected { get; set; }

        public int TotalEnemyKills { get; set; }
        public Dictionary<string, int> EnemyKillsByType { get; set; } = new Dictionary<string, int>();

        public int TotalDashes { get; set; }
        public int GroundDashes { get; set; }
        public int AirDashes { get; set; }
        public int Wavedashes { get; set; }
        public int Hyperdashes { get; set; }
        public int SuperDashes { get; set; }

        public Dictionary<int, long> BestTimesByChapter { get; set; } = new Dictionary<int, long>();

        public void IncrementDeaths(int chapterID)
        {
            TotalDeaths++;
            if (!DeathsByChapter.ContainsKey(chapterID))
                DeathsByChapter[chapterID] = 0;
            DeathsByChapter[chapterID]++;
        }

        public void IncrementEnemyKill(string enemyType)
        {
            TotalEnemyKills++;
            if (!EnemyKillsByType.ContainsKey(enemyType))
                EnemyKillsByType[enemyType] = 0;
            EnemyKillsByType[enemyType]++;
        }

        public void RecordChapterTime(int chapterID, long timeInTicks)
        {
            if (!BestTimesByChapter.ContainsKey(chapterID) || timeInTicks < BestTimesByChapter[chapterID])
            {
                BestTimesByChapter[chapterID] = timeInTicks / TimeSpan.TicksPerSecond;
            }
        }
    }
}



