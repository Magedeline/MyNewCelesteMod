#nullable enable
using System.IO.Pipes;
using System.Text.Json;
using System.Threading;

namespace DesoloZantas.Core.Core
{
    /// <summary>
    /// Bridge receiver for communication with DesoloZantas Standalone.
    /// Handles fallback requests, save sync, and IPC communication.
    /// </summary>
    public static class StandaloneBridge
    {
        private const string PipeName = "DesoloZantas_CelesteBridge";
        private const string FallbackDataFile = ".fallback_data.json";
        private const string FallbackStateFile = "fallback_state.json";

        private static NamedPipeServerStream? _pipeServer;
        private static StreamReader? _reader;
        private static StreamWriter? _writer;
        private static CancellationTokenSource? _cts;
        private static bool _isListening;
        private static FallbackData? _fallbackData;

        // Events
        public static event Action<BridgeMessage>? MessageReceived;
        public static event Action? StandaloneConnected;
        public static event Action? StandaloneDisconnected;
        public static event Action<FallbackData>? FallbackReceived;

        /// <summary>
        /// Whether the bridge is currently connected to standalone.
        /// </summary>
        public static bool IsConnected => _pipeServer?.IsConnected ?? false;

        /// <summary>
        /// Whether we received a fallback from standalone.
        /// </summary>
        public static bool WasFallback => _fallbackData != null;

        /// <summary>
        /// The fallback data if available.
        /// </summary>
        public static FallbackData? GetFallbackData() => _fallbackData;

        #region Initialization

        /// <summary>
        /// Initialize the bridge and check for fallback data.
        /// Call this during mod initialization.
        /// </summary>
        public static void Initialize()
        {
            IngesteLogger.Info("[Bridge] Initializing standalone bridge...");

            // Check for fallback data from standalone
            CheckFallbackData();

            // Start IPC listener
            StartListening();
        }

        /// <summary>
        /// Shutdown the bridge.
        /// Call this during mod unload.
        /// </summary>
        public static void Shutdown()
        {
            StopListening();
            _fallbackData = null;
        }

        private static void CheckFallbackData()
        {
            try
            {
                string? modPath = GetModPath();
                if (modPath == null) return;

                string fallbackFile = Path.Combine(modPath, FallbackDataFile);
                if (!File.Exists(fallbackFile)) return;

                string json = File.ReadAllText(fallbackFile);
                _fallbackData = JsonSerializer.Deserialize<FallbackData>(json);

                if (_fallbackData != null)
                {
                    IngesteLogger.Info($"[Bridge] Fallback data received from standalone: {_fallbackData.Reason}");
                    
                    // Check if fallback is recent (within 30 seconds)
                    if (_fallbackData.Timestamp.HasValue)
                    {
                        var age = DateTime.UtcNow - _fallbackData.Timestamp.Value;
                        if (age.TotalSeconds > 30)
                        {
                            IngesteLogger.Info("[Bridge] Fallback data is stale, ignoring");
                            _fallbackData = null;
                        }
                        else
                        {
                            FallbackReceived?.Invoke(_fallbackData);
                        }
                    }

                    // Clean up fallback file
                    try
                    {
                        File.Delete(fallbackFile);
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"[Bridge] Failed to read fallback data: {ex.Message}");
            }
        }

        #endregion

        #region IPC Communication

        private static void StartListening()
        {
            if (_isListening) return;

            _cts = new CancellationTokenSource();
            _isListening = true;

            Task.Run(async () =>
            {
                while (_isListening && !_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        _pipeServer = new NamedPipeServerStream(
                            PipeName,
                            PipeDirection.InOut,
                            1,
                            PipeTransmissionMode.Byte,
                            PipeOptions.Asynchronous);

                        IngesteLogger.Info("[Bridge] Waiting for standalone connection...");
                        await _pipeServer.WaitForConnectionAsync(_cts.Token);
                        
                        IngesteLogger.Info("[Bridge] Standalone connected!");
                        StandaloneConnected?.Invoke();

                        _reader = new StreamReader(_pipeServer);
                        _writer = new StreamWriter(_pipeServer) { AutoFlush = true };

                        // Send acknowledgment
                        SendMessage(new BridgeMessage
                        {
                            Type = "Connected",
                            Data = new { ModVersion = "1.0.0" }
                        });

                        // Listen for messages
                        while (_pipeServer.IsConnected && !_cts.Token.IsCancellationRequested)
                        {
                            try
                            {
                                string? line = await _reader.ReadLineAsync();
                                if (line == null) break;

                                var message = JsonSerializer.Deserialize<BridgeMessage>(line);
                                if (message != null)
                                {
                                    ProcessMessage(message);
                                }
                            }
                            catch (IOException)
                            {
                                break; // Pipe disconnected
                            }
                        }

                        IngesteLogger.Info("[Bridge] Standalone disconnected");
                        StandaloneDisconnected?.Invoke();
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        IngesteLogger.Warn($"[Bridge] IPC error: {ex.Message}");
                        await Task.Delay(2000, _cts.Token);
                    }
                    finally
                    {
                        CleanupConnection();
                    }
                }
            });
        }

        private static void StopListening()
        {
            _isListening = false;
            _cts?.Cancel();
            CleanupConnection();
        }

        private static void CleanupConnection()
        {
            _reader?.Dispose();
            _reader = null;
            _writer?.Dispose();
            _writer = null;
            _pipeServer?.Dispose();
            _pipeServer = null;
        }

        private static void ProcessMessage(BridgeMessage message)
        {
            MessageReceived?.Invoke(message);

            switch (message.Type)
            {
                case "Ping":
                    SendMessage(new BridgeMessage { Type = "Pong" });
                    break;

                case "FallbackRequest":
                    HandleFallbackRequest(message);
                    break;

                case "SaveSync":
                    HandleSaveSync(message);
                    break;

                case "AssetRequest":
                    HandleAssetRequest(message);
                    break;
            }
        }

        private static void HandleFallbackRequest(BridgeMessage message)
        {
            IngesteLogger.Info("[Bridge] Received fallback request from standalone");
            
            // Acknowledge the fallback
            SendMessage(new BridgeMessage
            {
                Type = "FallbackAck",
                Data = new { Ready = true }
            });

            // The standalone should exit, and Celeste continues running
        }

        private static void HandleSaveSync(BridgeMessage message)
        {
            IngesteLogger.Info("[Bridge] Received save sync request");
            
            // TODO: Implement save sync from standalone to mod
            // This would involve reading the save data and applying it to the mod's save system
        }

        private static void HandleAssetRequest(BridgeMessage message)
        {
            if (message.Data is JsonElement data && data.TryGetProperty("AssetPath", out var pathElement))
            {
                string assetPath = pathElement.GetString() ?? "";
                IngesteLogger.Info($"[Bridge] Asset request for: {assetPath}");
                
                // TODO: Send asset data back to standalone
            }
        }

        /// <summary>
        /// Send a message to the connected standalone.
        /// </summary>
        public static void SendMessage(BridgeMessage message)
        {
            if (_writer == null || !IsConnected) return;

            try
            {
                string json = JsonSerializer.Serialize(message);
                _writer.WriteLine(json);
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"[Bridge] Failed to send message: {ex.Message}");
            }
        }

        #endregion

        #region Fallback State

        /// <summary>
        /// Try to restore game state from standalone fallback.
        /// </summary>
        public static FallbackGameState? GetFallbackGameState()
        {
            try
            {
                string? modPath = GetModPath();
                if (modPath == null) return null;

                // Check for save data path
                string? celestePath = GetCelestePath();
                if (celestePath == null) return null;

                string saveDir = Path.Combine(celestePath, "Saves", "DesoloZantas");
                string stateFile = Path.Combine(saveDir, FallbackStateFile);

                if (!File.Exists(stateFile)) return null;

                string json = File.ReadAllText(stateFile);
                var state = JsonSerializer.Deserialize<FallbackGameState>(json);

                // Clean up after reading
                try { File.Delete(stateFile); } catch { }

                return state;
            }
            catch (Exception ex)
            {
                IngesteLogger.Warn($"[Bridge] Failed to read fallback state: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Clear fallback data after handling.
        /// </summary>
        public static void ClearFallbackData()
        {
            _fallbackData = null;
        }

        #endregion

        #region Helper Methods

        private static string? GetModPath()
        {
            try
            {
                if (IngesteModule.Instance?.Metadata?.PathDirectory != null)
                {
                    return IngesteModule.Instance.Metadata.PathDirectory;
                }

                string? assemblyPath = typeof(StandaloneBridge).Assembly.Location;
                if (!string.IsNullOrEmpty(assemblyPath))
                {
                    string? codeDir = Path.GetDirectoryName(assemblyPath);
                    if (codeDir != null)
                    {
                        return Path.GetDirectoryName(Path.GetDirectoryName(codeDir));
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static string? GetCelestePath()
        {
            try
            {
                string? modPath = GetModPath();
                if (modPath == null) return null;

                // Mod is in Celeste/Mods/DesoloZatnas
                return Path.GetFullPath(Path.Combine(modPath, "..", ".."));
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }

    #region Supporting Types

    public class BridgeMessage
    {
        public string Type { get; set; } = "";
        public object? Data { get; set; }
        public string? CustomType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class FallbackData
    {
        public bool FromStandalone { get; set; }
        public string? Reason { get; set; }
        public DateTime? Timestamp { get; set; }
        public List<FallbackError>? Errors { get; set; }
        public FallbackGameState? GameState { get; set; }
    }

    public class FallbackError
    {
        public string? Type { get; set; }
        public string? Message { get; set; }
        public DateTime? Timestamp { get; set; }
    }

    public class FallbackGameState
    {
        public string? CurrentLevel { get; set; }
        public string? CurrentRoom { get; set; }
        public float PlayerPositionX { get; set; }
        public float PlayerPositionY { get; set; }
        public int Dashes { get; set; }
        public float Stamina { get; set; }
        public int Deaths { get; set; }
        public TimeSpan PlayTime { get; set; }
        public Dictionary<string, object>? CustomData { get; set; }
    }

    #endregion
}
