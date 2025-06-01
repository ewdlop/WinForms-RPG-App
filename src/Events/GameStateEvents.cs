using System.Collections.Generic;
using WinFormsApp1.Interfaces;
using System.Drawing;

namespace WinFormsApp1.Events
{
    /// <summary>
    /// Event published when game state changes
    /// </summary>
    public class GameStateChangedEvent : GameEvent
    {
        public GameState OldState { get; set; }
        public GameState NewState { get; set; }
        public string Reason { get; set; }

        public override int Priority => 12; // Very high priority

        public GameStateChangedEvent(GameState oldState, GameState newState, string reason = "")
        {
            OldState = oldState;
            NewState = newState;
            Reason = reason;
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Event published when a new game starts
    /// </summary>
    public class GameStartedEvent : GameEvent
    {
        public Player Player { get; set; }
        public bool IsNewGame { get; set; }
        public int? LoadedFromSlot { get; set; }

        public override int Priority => 15; // Highest priority

        public GameStartedEvent(Player player, bool isNewGame = true, int? loadedFromSlot = null)
        {
            Player = player;
            IsNewGame = isNewGame;
            LoadedFromSlot = loadedFromSlot;
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Event published when game ends
    /// </summary>
    public class GameEndedEvent : GameEvent
    {
        public Player Player { get; set; }
        public string Reason { get; set; }
        public GameStatistics FinalStatistics { get; set; }
        public bool WasPlayerDeath { get; set; }

        public override int Priority => 15; // Highest priority

        public GameEndedEvent(Player player, string reason, GameStatistics finalStatistics, bool wasPlayerDeath = false)
        {
            Player = player;
            Reason = reason;
            FinalStatistics = finalStatistics;
            WasPlayerDeath = wasPlayerDeath;
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Event published when a command is processed
    /// </summary>
    public class CommandProcessedEvent : GameEvent
    {
        public string Command { get; set; }
        public CommandResult Result { get; set; }
        public Player Player { get; set; }
        public double ProcessingTime { get; set; }

        public CommandProcessedEvent(string command, CommandResult result, Player player, double processingTime = 0)
        {
            Command = command;
            Result = result;
            Player = player;
            ProcessingTime = processingTime;
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Event published when game is saved
    /// </summary>
    public class GameSavedEvent : GameEvent
    {
        public int SaveSlot { get; set; }
        public Player Player { get; set; }
        public bool WasSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public long SaveFileSize { get; set; }

        public GameSavedEvent(int saveSlot, Player player, bool wasSuccessful, string errorMessage = "", long saveFileSize = 0)
        {
            SaveSlot = saveSlot;
            Player = player;
            WasSuccessful = wasSuccessful;
            ErrorMessage = errorMessage;
            SaveFileSize = saveFileSize;
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Event published when game is loaded
    /// </summary>
    public class GameLoadedEvent : GameEvent
    {
        public int SaveSlot { get; set; }
        public Player Player { get; set; }
        public bool WasSuccessful { get; set; }
        public string ErrorMessage { get; set; }
        public long LoadFileSize { get; set; }

        public GameLoadedEvent(int saveSlot, Player player, bool wasSuccessful, string errorMessage = "", long loadFileSize = 0)
        {
            SaveSlot = saveSlot;
            Player = player;
            WasSuccessful = wasSuccessful;
            ErrorMessage = errorMessage;
            LoadFileSize = loadFileSize;
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Event published when a cheat is used
    /// </summary>
    public class CheatUsedEvent : GameEvent
    {
        public string CheatCode { get; set; }
        public Player Player { get; set; }
        public bool WasSuccessful { get; set; }
        public string Effect { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

        public CheatUsedEvent(string cheatCode, Player player, bool wasSuccessful, string effect = "", Dictionary<string, object> parameters = null)
        {
            CheatCode = cheatCode;
            Player = player;
            WasSuccessful = wasSuccessful;
            Effect = effect;
            Parameters = parameters ?? new Dictionary<string, object>();
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Event published when game time updates
    /// </summary>
    public class GameTimeUpdatedEvent : GameEvent
    {
        public double TotalGameTime { get; set; }
        public double DeltaTime { get; set; }
        public double SessionTime { get; set; }

        public GameTimeUpdatedEvent(double totalGameTime, double deltaTime, double sessionTime)
        {
            TotalGameTime = totalGameTime;
            DeltaTime = deltaTime;
            SessionTime = sessionTime;
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Event published when game features are toggled
    /// </summary>
    public class FeatureToggledEvent : GameEvent
    {
        public string FeatureName { get; set; }
        public bool IsEnabled { get; set; }
        public bool PreviousState { get; set; }
        public string ToggledBy { get; set; }

        public FeatureToggledEvent(string featureName, bool isEnabled, bool previousState, string toggledBy = "")
        {
            FeatureName = featureName;
            IsEnabled = isEnabled;
            PreviousState = previousState;
            ToggledBy = toggledBy;
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Event published when game statistics are updated
    /// </summary>
    public class StatisticsUpdatedEvent : GameEvent
    {
        public GameStatistics Statistics { get; set; }
        public string UpdatedStat { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public StatisticsUpdatedEvent(GameStatistics statistics, string updatedStat = "", object oldValue = null, object newValue = null)
        {
            Statistics = statistics;
            UpdatedStat = updatedStat;
            OldValue = oldValue;
            NewValue = newValue;
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Event published for general game messages that should be displayed to the player
    /// </summary>
    public class GameMessageEvent : GameEvent
    {
        public string Message { get; set; }
        public GameMessageType MessageType { get; set; }
        public Color? TextColor { get; set; }

        public GameMessageEvent(string message, GameMessageType messageType = GameMessageType.Info, Color? textColor = null)
        {
            Message = message;
            MessageType = messageType;
            TextColor = textColor;
            Source = "GameManager";
        }
    }

    /// <summary>
    /// Types of game messages for different display styles
    /// </summary>
    public enum GameMessageType
    {
        Info,
        Success,
        Warning,
        Error,
        Combat,
        System
    }
} 