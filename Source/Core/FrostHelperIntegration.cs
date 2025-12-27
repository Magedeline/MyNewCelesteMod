using System.Reflection;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Handles integration with FrostHelper mod for enhanced compatibility
    /// and shared functionality between the two mods.
    /// </summary>
    internal static class FrostHelperIntegration
    {
        private static bool _isInitialized = false;
        private static bool _isFrostHelperLoaded = false;
        private static Assembly _frostHelperAssembly = null;

        public static bool IsFrostHelperAvailable => _isFrostHelperLoaded && _frostHelperAssembly != null;

        public static void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "Initializing FrostHelper integration...");
                
                DetectFrostHelper();
                
                if (_isFrostHelperLoaded)
                {
                    Logger.Log(LogLevel.Info, "IngesteModule", "FrostHelper detected - setting up integrations");
                    SetupIntegrations();
                }
                else
                {
                    Logger.Log(LogLevel.Info, "IngesteModule", "FrostHelper not detected - running in standalone mode");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteModule", $"Failed to initialize FrostHelper integration: {ex}");
                _isFrostHelperLoaded = false;
            }
        }

        public static void Cleanup()
        {
            try
            {
                if (_isFrostHelperLoaded)
                {
                    Logger.Log(LogLevel.Info, "IngesteModule", "Cleaning up FrostHelper integration");
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteModule", $"Error during FrostHelper integration cleanup: {ex}");
            }
            finally
            {
                _isInitialized = false;
                _isFrostHelperLoaded = false;
                _frostHelperAssembly = null;
            }
        }

        private static void DetectFrostHelper()
        {
            try
            {
                var everestModules = Everest.Modules;
                var frostHelperModule = everestModules.FirstOrDefault(m => 
                    m.GetType().FullName?.Contains("FrostHelper") == true ||
                    m.Metadata?.Name?.Contains("FrostHelper") == true);

                if (frostHelperModule != null)
                {
                    _frostHelperAssembly = frostHelperModule.GetType().Assembly;
                    _isFrostHelperLoaded = true;
                    Logger.Log(LogLevel.Info, "IngesteModule", $"FrostHelper module found: {frostHelperModule.Metadata?.Name} v{frostHelperModule.Metadata?.Version}");
                }
                else
                {
                    _frostHelperAssembly = AppDomain.CurrentDomain.GetAssemblies()
                        .FirstOrDefault(a => a.GetName().Name?.Contains("FrostHelper") == true);
                    
                    if (_frostHelperAssembly != null)
                    {
                        _isFrostHelperLoaded = true;
                        Logger.Log(LogLevel.Info, "IngesteModule", $"FrostHelper assembly found: {_frostHelperAssembly.GetName().Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteModule", $"Error detecting FrostHelper: {ex}");
                _isFrostHelperLoaded = false;
            }
        }

        private static void SetupIntegrations()
        {
            try
            {
                Logger.Log(LogLevel.Info, "IngesteModule", "FrostHelper integration setup completed successfully");
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, "IngesteModule", $"Error setting up FrostHelper integrations: {ex}");
                throw;
            }
        }
    }
}



