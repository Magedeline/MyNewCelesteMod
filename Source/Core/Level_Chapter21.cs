namespace DesoloZantas.Core.Core;

/// <summary>
/// Extended Level class for Chapter 21 with 23 area maps support
/// This extends the base Level functionality to handle the larger map structure
/// </summary>
public class Level_Chapter21 : Level
{
    // Chapter 21 specific constants
    public const int CHAPTER_ID = 21;
    public const int TOTAL_AREA_MAPS = 23;
    
    // Area map identifiers for Chapter 21
    public static readonly string[] AreaMapIds = new string[TOTAL_AREA_MAPS]
    {
        "ch21_area_00", "ch21_area_01", "ch21_area_02", "ch21_area_03",
        "ch21_area_04", "ch21_area_05", "ch21_area_06", "ch21_area_07",
        "ch21_area_08", "ch21_area_09", "ch21_area_10", "ch21_area_11",
        "ch21_area_12", "ch21_area_13", "ch21_area_14", "ch21_area_15",
        "ch21_area_16", "ch21_area_17", "ch21_area_18", "ch21_area_19",
        "ch21_area_20", "ch21_area_21", "ch21_area_22"
    };
    
    // Track which areas have been visited
    private HashSet<string> visitedAreas = new HashSet<string>();
    
    // Track current area index
    private int currentAreaIndex = 0;
    
    // Chapter 21 specific flags
    public bool IsChapter21 => Session?.Area.ID == CHAPTER_ID;
    
    // Area progression tracking
    public Dictionary<string, AreaProgress> areaProgressTracking = new Dictionary<string, AreaProgress>();
    
    /// <summary>
    /// Area progress data structure
    /// </summary>
    public class AreaProgress
    {
        public string AreaId { get; set; }
        public bool Completed { get; set; }
        public int Deaths { get; set; }
        public long Time { get; set; }
        public List<EntityID> CollectedStrawberries { get; set; } = new List<EntityID>();
        public bool HeartGemCollected { get; set; }
        public bool CassetteCollected { get; set; }
        
        public AreaProgress(string areaId)
        {
            AreaId = areaId;
        }
    }
    
    /// <summary>
    /// Constructor for Chapter 21 Level
    /// </summary>
    public Level_Chapter21() : base()
    {
        InitializeChapter21();
    }
    
    /// <summary>
    /// Initialize Chapter 21 specific data
    /// </summary>
    private void InitializeChapter21()
    {
        // Initialize progress tracking for all 23 areas
        for (int i = 0; i < TOTAL_AREA_MAPS; i++)
        {
            string areaId = AreaMapIds[i];
            if (!areaProgressTracking.ContainsKey(areaId))
            {
                areaProgressTracking[areaId] = new AreaProgress(areaId);
            }
        }
    }
    
    /// <summary>
    /// Load a specific area map by index
    /// </summary>
    public void LoadAreaMap(int areaIndex, CelestePlayer.IntroTypes playerIntro = CelestePlayer.IntroTypes.Transition)
    {
        if (areaIndex < 0 || areaIndex >= TOTAL_AREA_MAPS)
        {
            Logger.Log(LogLevel.Error, "Level_Chapter21", 
                $"Invalid area index {areaIndex}. Must be between 0 and {TOTAL_AREA_MAPS - 1}");
            return;
        }
        
        currentAreaIndex = areaIndex;
        string areaId = AreaMapIds[areaIndex];
        
        // Mark area as visited
        visitedAreas.Add(areaId);
        
        // Set the session level to the new area
        if (Session != null && Session.MapData != null)
        {
            // Find the first room in this area map
            LevelData targetLevel = FindAreaMapStartLevel(areaId);
            if (targetLevel != null)
            {
                Session.Level = targetLevel.Name;
                LoadLevel(playerIntro, false);
            }
        }
    }
    
    /// <summary>
    /// Find the starting level for a given area map
    /// </summary>
    private LevelData FindAreaMapStartLevel(string areaId)
    {
        if (Session?.MapData?.Levels == null)
            return null;
            
        // Look for levels that match the area ID pattern
        foreach (LevelData level in Session.MapData.Levels)
        {
            if (level.Name.StartsWith(areaId))
            {
                return level;
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// Transition to the next area map
    /// </summary>
    public void TransitionToNextArea(Vector2 direction)
    {
        int nextAreaIndex = currentAreaIndex + 1;
        if (nextAreaIndex < TOTAL_AREA_MAPS)
        {
            // Save current area progress
            SaveCurrentAreaProgress();
            
            // Load next area
            LoadAreaMap(nextAreaIndex, CelestePlayer.IntroTypes.Transition);
        }
        else
        {
            // Completed all areas - trigger chapter completion
            CompleteChapter21();
        }
    }
    
    /// <summary>
    /// Transition to the previous area map
    /// </summary>
    public void TransitionToPreviousArea(Vector2 direction)
    {
        int prevAreaIndex = currentAreaIndex - 1;
        if (prevAreaIndex >= 0)
        {
            SaveCurrentAreaProgress();
            LoadAreaMap(prevAreaIndex, CelestePlayer.IntroTypes.Transition);
        }
    }
    
    /// <summary>
    /// Save progress for the current area
    /// </summary>
    private void SaveCurrentAreaProgress()
    {
        if (currentAreaIndex < 0 || currentAreaIndex >= TOTAL_AREA_MAPS)
            return;
            
        string areaId = AreaMapIds[currentAreaIndex];
        if (areaProgressTracking.TryGetValue(areaId, out AreaProgress progress))
        {
            progress.Deaths = Session.DeathsInCurrentLevel;
            progress.Time = Session.Time;
            
            // Track collectibles
            foreach (EntityID berry in Session.Strawberries)
            {
                if (!progress.CollectedStrawberries.Contains(berry))
                {
                    progress.CollectedStrawberries.Add(berry);
                }
            }
            
            progress.HeartGemCollected = Session.HeartGem;
            progress.CassetteCollected = Session.Cassette;
        }
    }
    
    /// <summary>
    /// Complete Chapter 21
    /// </summary>
    private void CompleteChapter21()
    {
        // Mark all areas as visited
        for (int i = 0; i < TOTAL_AREA_MAPS; i++)
        {
            string areaId = AreaMapIds[i];
            if (areaProgressTracking.TryGetValue(areaId, out AreaProgress progress))
            {
                progress.Completed = true;
            }
        }
        
        // Trigger standard chapter completion
        RegisterAreaComplete();
        CompleteArea(spotlightWipe: true, skipScreenWipe: false, skipCompleteScreen: false);
    }
    
    /// <summary>
    /// Get the current area map ID
    /// </summary>
    public string GetCurrentAreaMapId()
    {
        if (currentAreaIndex >= 0 && currentAreaIndex < TOTAL_AREA_MAPS)
        {
            return AreaMapIds[currentAreaIndex];
        }
        return null;
    }
    
    /// <summary>
    /// Get progress for a specific area
    /// </summary>
    public AreaProgress GetAreaProgress(int areaIndex)
    {
        if (areaIndex >= 0 && areaIndex < TOTAL_AREA_MAPS)
        {
            string areaId = AreaMapIds[areaIndex];
            if (areaProgressTracking.TryGetValue(areaId, out AreaProgress progress))
            {
                return progress;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Get total completion percentage for Chapter 21
    /// </summary>
    public float GetCompletionPercentage()
    {
        int completedAreas = 0;
        foreach (var progress in areaProgressTracking.Values)
        {
            if (progress.Completed)
            {
                completedAreas++;
            }
        }
        return (float)completedAreas / TOTAL_AREA_MAPS * 100f;
    }
    
    /// <summary>
    /// Check if player can transition to a specific area
    /// </summary>
    public bool CanTransitionToArea(int targetAreaIndex)
    {
        // Basic implementation - can be extended with prerequisites
        if (targetAreaIndex < 0 || targetAreaIndex >= TOTAL_AREA_MAPS)
            return false;
            
        // Allow transition to current area and adjacent areas
        return Math.Abs(targetAreaIndex - currentAreaIndex) <= 1;
    }
    
    /// <summary>
    /// Override Update to handle Chapter 21 specific logic
    /// </summary>
    public override void Update()
    {
        base.Update();
        
        if (!IsChapter21)
            return;
            
        // Check for area transition triggers
        CheckAreaTransitions();
    }
    
    /// <summary>
    /// Check if the player should transition between areas
    /// </summary>
    private void CheckAreaTransitions()
    {
        CelestePlayer player = Tracker.GetEntity<CelestePlayer>();
        if (player == null || Transitioning)
            return;
            
        // Check if player reached area boundaries
        // This would be implemented based on specific level design
    }
    
    /// <summary>
    /// Get total strawberries collected across all areas
    /// </summary>
    public int GetTotalStrawberriesCollected()
    {
        int total = 0;
        foreach (var progress in areaProgressTracking.Values)
        {
            total += progress.CollectedStrawberries.Count;
        }
        return total;
    }
    
    /// <summary>
    /// Get total deaths across all areas
    /// </summary>
    public int GetTotalDeaths()
    {
        int total = 0;
        foreach (var progress in areaProgressTracking.Values)
        {
            total += progress.Deaths;
        }
        return total;
    }
    
    /// <summary>
    /// Get total time across all areas
    /// </summary>
    public long GetTotalTime()
    {
        long total = 0;
        foreach (var progress in areaProgressTracking.Values)
        {
            total += progress.Time;
        }
        return total;
    }
    
    /// <summary>
    /// Reset all area progress
    /// </summary>
    public void ResetAllProgress()
    {
        visitedAreas.Clear();
        currentAreaIndex = 0;
        
        foreach (var progress in areaProgressTracking.Values)
        {
            progress.Completed = false;
            progress.Deaths = 0;
            progress.Time = 0;
            progress.CollectedStrawberries.Clear();
            progress.HeartGemCollected = false;
            progress.CassetteCollected = false;
        }
    }
    
    /// <summary>
    /// Get list of visited area IDs
    /// </summary>
    public List<string> GetVisitedAreas()
    {
        return visitedAreas.ToList();
    }
    
    /// <summary>
    /// Check if a specific area has been visited
    /// </summary>
    public bool HasVisitedArea(int areaIndex)
    {
        if (areaIndex >= 0 && areaIndex < TOTAL_AREA_MAPS)
        {
            return visitedAreas.Contains(AreaMapIds[areaIndex]);
        }
        return false;
    }
}




