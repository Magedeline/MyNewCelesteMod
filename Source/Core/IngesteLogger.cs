namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Logger utility for Ingeste with support for debug mode and file-based logging
    /// </summary>
    public static class IngesteLogger
    {
        private static bool logFileInitialized = false;
        private static string logFilePath = string.Empty;
        private static readonly object logLock = new object();

        /// <summary>
        /// Initializes the logger and creates log directory if necessary
        /// </summary>
        private static void EnsureLogFileInitialized()
        {
            if (logFileInitialized) return;

            try
            {
                string modPath = Path.GetDirectoryName(typeof(IngesteLogger).Assembly.Location);
                if (string.IsNullOrEmpty(modPath)) return;

                string logDir = Path.Combine(modPath, IngesteConstants.Paths.LOGS_DIRECTORY);
                Directory.CreateDirectory(logDir);

                string timestamp = DateTime.Now.ToString(IngesteConstants.Debug.LOG_TIMESTAMP_FORMAT);
                logFilePath = Path.Combine(logDir, $"{IngesteConstants.Debug.LOG_FILE_PREFIX}_{timestamp}{IngesteConstants.Debug.LOG_FILE_EXTENSION}");

                // Write initial log header
                File.WriteAllText(logFilePath, $"Ingeste Log Started: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                                              $"Everest Version: {Everest.Version}\n" +
                                              $"Mod Version: {IngesteModule.Instance?.Metadata?.Version}\n" +
                                              $"-------------------------------------------\n");

                logFileInitialized = true;
            }
            catch (Exception ex)
            {
                // If we can't initialize logging, just print to main Celeste log
                Logger.Log(LogLevel.Warn, IngesteConstants.Debug.LOGGER_NAME, $"Failed to initialize log file: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs informational messages
        /// </summary>
        public static void Info(string message)
        {
            Logger.Log(LogLevel.Info, IngesteConstants.Debug.LOGGER_NAME, message);
            LogToFile("INFO", message);
        }

        /// <summary>
        /// Logs warning messages
        /// </summary>
        public static void Warn(string message)
        {
            Logger.Log(LogLevel.Warn, IngesteConstants.Debug.LOGGER_NAME, message);
            LogToFile("WARN", message);
        }

        /// <summary>
        /// Logs error messages
        /// </summary>
        public static void Error(string message)
        {
            Logger.Log(LogLevel.Error, IngesteConstants.Debug.LOGGER_NAME, message);
            LogToFile("ERROR", message);
        }

        /// <summary>
        /// Logs error messages with exception details
        /// </summary>
        public static void Error(Exception ex, string context)
        {
            string message = $"{context}: {ex.Message}";
            Logger.Log(LogLevel.Error, IngesteConstants.Debug.LOGGER_NAME, message);
            
            // Log detailed exception information to file
            LogToFile("ERROR", message);
            LogToFile("EXCEPTION", $"Type: {ex.GetType().FullName}");
            LogToFile("EXCEPTION", $"Message: {ex.Message}");
            LogToFile("EXCEPTION", $"Stack: {ex.StackTrace}");
            
            if (ex.InnerException != null)
            {
                LogToFile("INNER_EXCEPTION", $"Type: {ex.InnerException.GetType().FullName}");
                LogToFile("INNER_EXCEPTION", $"Message: {ex.InnerException.Message}");
                LogToFile("INNER_EXCEPTION", $"Stack: {ex.InnerException.StackTrace}");
            }
        }

        /// <summary>
        /// Logs messages only if debug mode is enabled
        /// </summary>
        public static void DebugMode(string message)
        {
            // Only log debug messages if debug mode is enabled in settings
            if (IsDebugModeEnabled())
            {
                Logger.Log(LogLevel.Debug, IngesteConstants.Debug.LOGGER_NAME, message);
                LogToFile("DEBUG", message);
            }
        }

        /// <summary>
        /// Logs audio-specific error messages with special handling
        /// </summary>
        public static void LogAudioError(Exception ex, string context)
        {
            string message = $"{context}: {ex.Message}";
            Logger.Log(LogLevel.Error, IngesteConstants.Debug.LOGGER_NAME, message);
            
            // Log more detailed information for audio errors
            LogToFile("AUDIO_ERROR", message);
            LogToFile("AUDIO_ERROR", $"Type: {ex.GetType().FullName}");
            LogToFile("AUDIO_ERROR", $"Message: {ex.Message}");
            LogToFile("AUDIO_ERROR", $"Stack: {ex.StackTrace}");
            
            // Add troubleshooting suggestions specifically for audio errors
            LogToFile("AUDIO_ERROR", "Troubleshooting suggestions:");
            LogToFile("AUDIO_ERROR", "1. Check if required FMOD plugins are installed");
            LogToFile("AUDIO_ERROR", "2. Update Everest to the latest version");
            LogToFile("AUDIO_ERROR", "3. Reinstall the mod to ensure audio banks are not corrupted");
            LogToFile("AUDIO_ERROR", "4. The mod will continue using vanilla sounds instead");
            
            if (ex.InnerException != null)
            {
                LogToFile("AUDIO_ERROR", $"Inner exception: {ex.InnerException.Message}");
            }
        }

        /// <summary>
        /// Logs a message to the mod-specific log file
        /// </summary>
        private static void LogToFile(string level, string message)
        {
            try
            {
                lock (logLock)
                {
                    EnsureLogFileInitialized();
                    if (!logFileInitialized) return;

                    string timestamp = DateTime.Now.ToString(IngesteConstants.Debug.LOG_TIME_FORMAT);
                    string logLine = $"[{timestamp}] [{level}] {message}";

                    File.AppendAllText(logFilePath, logLine + Environment.NewLine);
                }
            }
            catch
            {
                // Silently fail if file logging fails
                // We already logged to the main Celeste log
            }
        }
        
        // Add missing methods to support existing code
        
        /// <summary>
        /// Log debug messages (alias for DebugMode)
        /// </summary>
        public static void Debug(string message)
        {
            DebugMode(message);
        }
        
        /// <summary>
        /// Log verbose messages (only in verbose mode)
        /// </summary>
        public static void Verbose(string message)
        {
            if (IsDebugModeEnabled())
            {
                Logger.Log(LogLevel.Verbose, IngesteConstants.Debug.LOGGER_NAME, message);
                LogToFile("VERBOSE", message);
            }
        }
        
        /// <summary>
        /// Log tracking messages for performance monitoring
        /// </summary>
        public static void Track(string message)
        {
            if (IsDebugModeEnabled())
            {
                Logger.Log(LogLevel.Info, IngesteConstants.Debug.LOGGER_NAME, $"[TRACK] {message}");
                LogToFile("TRACK", message);
            }
        }

        /// <summary>
        /// Check if debug mode is enabled in settings
        /// </summary>
        private static bool IsDebugModeEnabled()
        {
            // Replace 'IngesteModuleSettings' with the actual settings type if different
            var settings = IngesteModule.Settings as IngesteModuleSettings;
            return settings?.DebugMode == true;
        }

        /// <summary>
        /// Log debug messages with exception details
        /// </summary>
        public static void Debug(Exception ex, string message)
        {
            if (IsDebugModeEnabled())
            {
                string fullMessage = $"{message}: {ex.Message}";
                Logger.Log(LogLevel.Debug, IngesteConstants.Debug.LOGGER_NAME, fullMessage);
                LogToFile("DEBUG", fullMessage);
                LogToFile("DEBUG", $"Exception Type: {ex.GetType().FullName}");
                LogToFile("DEBUG", $"Stack: {ex.StackTrace}");
            }
        }
    }
}



