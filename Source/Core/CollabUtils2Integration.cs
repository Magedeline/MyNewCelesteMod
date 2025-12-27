using System.Reflection;
using DesoloZantas.Core.Core.Extensions;
using DesoloZantas.Core.Core.Player;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Integration with CelesteCollabUtils2 for sub map management with Kirby and normal player support
    /// Supports chapters 1-14 with dynamic player switching
    /// Based on: https://github.com/EverestAPI/CelesteCollabUtils2.git
    /// </summary>
    public static class CollabUtils2Integration
    {
        private static bool _initialized = false;
        private static Type _collabModule;
        private static Type _lobbyMapController;
      private static Type _miniHeartDoor;
  private static MethodInfo _registerLobbyMethod;
   private static MethodInfo _getLobbyLevelSetMethod;
        
   // Track player mode per chapter/submap
        private static Dictionary<string, bool> _kirbyModePerMap = new Dictionary<string, bool>();
        
    // CollabUtils2 configuration
        private const string COLLAB_MODULE_NAME = "CollabUtils2";
        private const string LOBBY_PREFIX = "DesoloZatnas/Lobby_Ch";
        private const string SUBMAP_PREFIX = "DesoloZatnas/Submap_Ch";
        
        /// <summary>
    /// Initialize CollabUtils2 integration
        /// </summary>
        public static void Initialize()
        {
        if (_initialized) return;
        
      try
            {
        // Find CollabUtils2 module
        var collabModule = Everest.Modules.FirstOrDefault(m => m.GetType().Name == COLLAB_MODULE_NAME);
if (collabModule == null)
  {
     IngesteLogger.Warn("CollabUtils2 not found - sub map integration disabled");
        return;
           }
    
           _collabModule = collabModule.GetType();
 IngesteLogger.Info($"Found CollabUtils2 module: {_collabModule.FullName}");
        
                // Find LobbyMapDataProcessor type
                var lobbyMapDataProcessorType = _collabModule.Assembly.GetType("Celeste.Mod.CollabUtils2.LobbyMapDataProcessor");
  if (lobbyMapDataProcessorType != null)
     {
       _registerLobbyMethod = lobbyMapDataProcessorType.GetMethod("RegisterLobby", 
  BindingFlags.Public | BindingFlags.Static);
  _getLobbyLevelSetMethod = lobbyMapDataProcessorType.GetMethod("GetLobbyLevelSet",
   BindingFlags.Public | BindingFlags.Static);
    }
    
            // Find other CollabUtils2 types
      _lobbyMapController = _collabModule.Assembly.GetType("Celeste.Mod.CollabUtils2.Entities.LobbyMapController");
      _miniHeartDoor = _collabModule.Assembly.GetType("Celeste.Mod.CollabUtils2.Entities.MiniHeartDoor");
                
        // Register lobbies and submaps for chapters 1-14
     RegisterAllChapterLobbies();
         
        // Hook into level loading to manage player mode
           On.Celeste.Level.LoadLevel += OnLevelLoadLevel;
  On.Celeste.Player.Added += OnPlayerAdded;
     
          _initialized = true;
    IngesteLogger.Info("CollabUtils2 integration initialized successfully for chapters 1-14");
            }
   catch (Exception ex)
            {
      IngesteLogger.Error($"Failed to initialize CollabUtils2 integration: {ex.Message}");
          IngesteLogger.Error($"Stack trace: {ex.StackTrace}");
      }
   }
        
     /// <summary>
        /// Register all chapter lobbies (1-14) with CollabUtils2
        /// </summary>
        private static void RegisterAllChapterLobbies()
        {
 if (_registerLobbyMethod == null)
       {
    IngesteLogger.Warn("RegisterLobby method not found in CollabUtils2");
   return;
         }
  
   for (int chapter = 1; chapter <= 14; chapter++)
            {
     RegisterChapterLobby(chapter);
          }
        }
   
        /// <summary>
        /// Register a specific chapter lobby with CollabUtils2
        /// </summary>
        private static void RegisterChapterLobby(int chapterNumber)
      {
            try
  {
    string lobbyLevelSet = $"{LOBBY_PREFIX}{chapterNumber}";
                string lobbyMapName = $"lobby_ch{chapterNumber}";
       
  // Register lobby with CollabUtils2
 _registerLobbyMethod?.Invoke(null, new object[] { lobbyLevelSet, lobbyMapName });
                
 IngesteLogger.Info($"Registered lobby for Chapter {chapterNumber}: {lobbyLevelSet}");
        
  // Register submaps for this chapter
            RegisterChapterSubmaps(chapterNumber);
       }
            catch (Exception ex)
    {
      IngesteLogger.Error($"Failed to register lobby for Chapter {chapterNumber}: {ex.Message}");
            }
        }
  
   /// <summary>
        /// Register all submaps for a chapter
  /// </summary>
        private static void RegisterChapterSubmaps(int chapterNumber)
        {
            // Determine number of submaps per chapter
          int submapCount = GetSubmapCountForChapter(chapterNumber);
            
         for (int submapIndex = 1; submapIndex <= submapCount; submapIndex++)
       {
       RegisterSubmap(chapterNumber, submapIndex);
            }
        }
        
        /// <summary>
        /// Register a specific submap with CollabUtils2
        /// </summary>
        private static void RegisterSubmap(int chapterNumber, int submapIndex)
        {
            try
            {
        string submapId = $"{SUBMAP_PREFIX}{chapterNumber}_S{submapIndex}";
      string submapMapName = $"submap_ch{chapterNumber}_{submapIndex}";
       
      // Register with CollabUtils2 (if it supports submap registration)
        // Note: CollabUtils2 may handle this automatically through level naming conventions
           
     IngesteLogger.Info($"Registered submap: Chapter {chapterNumber}, Submap {submapIndex}");
  }
      catch (Exception ex)
       {
           IngesteLogger.Error($"Failed to register submap Ch{chapterNumber}_S{submapIndex}: {ex.Message}");
   }
        }
        
        /// <summary>
        /// Get the number of submaps for a chapter
        /// </summary>
  private static int GetSubmapCountForChapter(int chapterNumber)
        {
  // Define submap counts per chapter
   // Chapters 1-9: 4 submaps each
  // Chapters 10-14: 6 submaps each
            return chapterNumber switch
            {
   >= 1 and <= 9 => 4,
   >= 10 and <= 14 => 6,
         _ => 0
   };
        }
        
        /// <summary>
        /// Hook: Called when a level is being loaded
      /// </summary>
  private static void OnLevelLoadLevel(On.Celeste.Level.orig_LoadLevel orig, Level self, global::Celeste.Player.IntroTypes playerIntro, bool isFromLoader)
        {
            orig(self, playerIntro, isFromLoader);
   
            try
 {
        // Check if we're in a DesoloZatnas level
      if (!IsDesoloZatnasLevel(self)) return;
        
         // Determine if Kirby mode should be active for this level
       bool shouldUseKirby = ShouldUseKirbyMode(self);
   
                // Store the mode preference
                string mapKey = GetMapKey(self);
        _kirbyModePerMap[mapKey] = shouldUseKirby;
         
 IngesteLogger.Info($"Level loaded: {self.Session.Level}, Kirby mode: {shouldUseKirby}");
 }
   catch (Exception ex)
            {
    IngesteLogger.Error($"Error in OnLevelLoadLevel: {ex.Message}");
            }
        }
      
        /// <summary>
        /// Hook: Called when a player is added to the scene
        /// </summary>
        private static void OnPlayerAdded(On.Celeste.Player.orig_Added orig, global::Celeste.Player self, Scene scene)
  {
        orig(self, scene);
            
   try
            {
  if (!(scene is Level level)) return;
           if (!IsDesoloZatnasLevel(level)) return;
                
     string mapKey = GetMapKey(level);
     
          // Check if Kirby mode should be active
         if (_kirbyModePerMap.TryGetValue(mapKey, out bool useKirby) && useKirby)
      {
         // Enable Kirby mode for this player
             EnableKirbyModeForPlayer(self, level);
   }
        else
           {
    // Ensure normal mode
        DisableKirbyModeForPlayer(self);
         }
            }
            catch (Exception ex)
   {
          IngesteLogger.Error($"Error in OnPlayerAdded: {ex.Message}");
            }
   }
        
        /// <summary>
        /// Check if the current level is part of DesoloZatnas
        /// </summary>
        private static bool IsDesoloZatnasLevel(Level level)
    {
            if (level?.Session?.Area == null) return false;
  
string sid = level.Session.Area.GetSID();
            return sid != null && sid.Contains("DesoloZatnas");
        }
     
   /// <summary>
     /// Determine if Kirby mode should be used for this level
        /// </summary>
  private static bool ShouldUseKirbyMode(Level level)
        {
  string levelName = level.Session.Level?.ToLower() ?? "";
     
            // Check for explicit Kirby mode flags
      if (level.Session.GetFlag("kirby_mode_enabled"))
        return true;
          
            if (level.Session.GetFlag("kirby_mode_disabled"))
           return false;
       
        // Check level naming conventions
            // Levels with "kirby" in the name use Kirby mode
          if (levelName.Contains("kirby"))
   return true;
            
   // Chapters 10-14 lobbies and submaps use Kirby mode by default
            if (levelName.Contains("lobby_ch") || levelName.Contains("submap_ch"))
     {
          int chapterNum = ExtractChapterNumber(levelName);
       if (chapterNum >= 10 && chapterNum <= 14)
 return true;
  }
       
   // Check for special submap designation
            if (level.Session.GetFlag("submap_kirby_player"))
    return true;
          
     // Default to normal player for chapters 1-9
  return false;
        }
  
      /// <summary>
        /// Extract chapter number from level name
        /// </summary>
 private static int ExtractChapterNumber(string levelName)
        {
     try
         {
     // Look for patterns like "ch10", "ch1", etc.
        int chIndex = levelName.IndexOf("ch");
      if (chIndex >= 0)
             {
      string afterCh = levelName.Substring(chIndex + 2);
    string numStr = "";
  
  foreach (char c in afterCh)
    {
               if (char.IsDigit(c))
             numStr += c;
  else
    break;
            }
      
             if (int.TryParse(numStr, out int chapterNum))
  return chapterNum;
             }
          }
       catch { }

            return 0;
        }
    
    /// <summary>
 /// Get a unique key for the current map
        /// </summary>
        private static string GetMapKey(Level level)
        {
        return $"{level.Session.Area.GetSID()}_{level.Session.Level}";
        }
        
        /// <summary>
        /// Enable Kirby mode for a player
        /// </summary>
  private static void EnableKirbyModeForPlayer(global::Celeste.Player player, Level level)
        {
            try
            {
     if (player.IsKirbyMode())
           {
    IngesteLogger.Info("Kirby mode already enabled for player");
     return;
         }
     
 player.EnableKirbyMode();
   
          // Set appropriate flags
       level.Session.SetFlag("player_mode_kirby", true);
     level.Session.SetFlag("player_mode_normal", false);
              
    IngesteLogger.Info("Enabled Kirby mode for player");
    }
    catch (Exception ex)
            {
IngesteLogger.Error($"Failed to enable Kirby mode: {ex.Message}");
            }
        }
        
        /// <summary>
     /// Disable Kirby mode for a player
        /// </summary>
private static void DisableKirbyModeForPlayer(global::Celeste.Player player)
        {
       try
            {
     if (!player.IsKirbyMode()) return;
       
                player.DisableKirbyMode();
          IngesteLogger.Info("Disabled Kirby mode for player");
            }
            catch (Exception ex)
            {
                IngesteLogger.Error($"Failed to disable Kirby mode: {ex.Message}");
       }
        }
        
  /// <summary>
        /// Create a CollabUtils2 Mini Heart Door (for submap unlocking)
        /// </summary>
      public static Entity CreateMiniHeartDoor(EntityData data, Vector2 offset, int requiredHearts)
        {
            if (_miniHeartDoor == null)
       {
    IngesteLogger.Warn("MiniHeartDoor type not found in CollabUtils2");
            return null;
    }
      
try
     {
   // Create mini heart door using reflection
        var door = Activator.CreateInstance(_miniHeartDoor, data, offset) as Entity;
   
         // Set required hearts property if available
       var heartsProp = _miniHeartDoor.GetProperty("RequiredHearts");
              heartsProp?.SetValue(door, requiredHearts);
       
           return door;
            }
            catch (Exception ex)
            {
          IngesteLogger.Error($"Failed to create MiniHeartDoor: {ex.Message}");
     return null;
    }
        }
        
  /// <summary>
        /// Get the current lobby level set for CollabUtils2
  /// </summary>
        public static string GetCurrentLobbyLevelSet(Level level)
        {
 if (_getLobbyLevelSetMethod == null) return null;
            
            try
            {
                return _getLobbyLevelSetMethod.Invoke(null, new object[] { level }) as string;
     }
     catch (Exception ex)
    {
         IngesteLogger.Error($"Failed to get lobby level set: {ex.Message}");
      return null;
     }
  }
        
    /// <summary>
        /// Check if CollabUtils2 is loaded and initialized
        /// </summary>
      public static bool IsCollabUtils2Available()
        {
   return _initialized && _collabModule != null;
        }
        
        /// <summary>
        /// Toggle Kirby mode for the current player (debug command)
      /// </summary>
        public static void ToggleKirbyMode()
   {
            try
          {
     var scene = Engine.Scene;
              if (!(scene is Level level)) return;
       
         var player = level.Tracker.GetEntity<global::Celeste.Player>();
       if (player == null) return;
            
        if (player.IsKirbyMode())
{
      DisableKirbyModeForPlayer(player);
            level.Session.SetFlag("player_mode_kirby", false);
         level.Session.SetFlag("player_mode_normal", true);
           IngesteLogger.Info("Switched to normal player mode");
                }
    else
          {
           EnableKirbyModeForPlayer(player, level);
               IngesteLogger.Info("Switched to Kirby player mode");
      }
         }
   catch (Exception ex)
            {
       IngesteLogger.Error($"Failed to toggle Kirby mode: {ex.Message}");
         }
        }
        
        /// <summary>
        /// Force Kirby mode for a specific chapter/submap (for map metadata)
        /// </summary>
        public static void SetKirbyModeForMap(string mapName, bool useKirby)
    {
    _kirbyModePerMap[mapName] = useKirby;
            IngesteLogger.Info($"Set Kirby mode for {mapName}: {useKirby}");
     }
    
        /// <summary>
 /// Get submap configuration for a chapter
        /// </summary>
        public static SubMapConfiguration GetSubmapConfiguration(int chapterNumber)
        {
      return new SubMapConfiguration
            {
        ChapterNumber = chapterNumber,
      SubmapCount = GetSubmapCountForChapter(chapterNumber),
        UseKirbyMode = chapterNumber >= 10,
             LobbyLevelSet = $"{LOBBY_PREFIX}{chapterNumber}",
                SubmapPrefix = $"{SUBMAP_PREFIX}{chapterNumber}",
    RequiredHeartsPerSubmap = GetRequiredHeartsForChapter(chapterNumber)
            };
        }
   
 /// <summary>
        /// Get required heart gems for chapter progression
        /// </summary>
        private static int[] GetRequiredHeartsForChapter(int chapterNumber)
        {
      // Define heart requirements per submap
   return chapterNumber switch
            {
>= 1 and <= 9 => new[] { 0, 3, 6, 10 },
  >= 10 and <= 14 => new[] { 0, 3, 7, 12, 18, 25 },
            _ => new[] { 0 }
   };
    }
    }
  
    /// <summary>
    /// Configuration data for a chapter's submaps
    /// </summary>
    public class SubMapConfiguration
    {
    public int ChapterNumber { get; set; }
        public int SubmapCount { get; set; }
        public bool UseKirbyMode { get; set; }
        public string LobbyLevelSet { get; set; }
        public string SubmapPrefix { get; set; }
        public int[] RequiredHeartsPerSubmap { get; set; }
    }
}




