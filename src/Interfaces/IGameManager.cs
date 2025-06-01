using System.Collections.Generic;

namespace WinFormsApp1.Interfaces
{
    /// <summary>
    /// Interface for core game management operations
    /// </summary>
    public interface IGameManager : IBaseManager
    {
        /// <summary>
        /// Current game state
        /// </summary>
        GameState CurrentGameState { get; }

        /// <summary>
        /// Whether the game is currently running
        /// </summary>
        bool IsGameRunning { get; }

        /// <summary>
        /// Whether the game has unsaved changes
        /// </summary>
        bool HasUnsavedChanges { get; }

        /// <summary>
        /// Start a new game with the specified player
        /// </summary>
        /// <param name="player">Player to start the game with</param>
        void StartNewGame(Player player);

        /// <summary>
        /// Load an existing game
        /// </summary>
        /// <param name="saveSlot">Save slot to load from</param>
        /// <returns>True if game was loaded successfully</returns>
        bool LoadGame(int saveSlot);

        /// <summary>
        /// Save the current game
        /// </summary>
        /// <param name="saveSlot">Save slot to save to</param>
        /// <returns>True if game was saved successfully</returns>
        bool SaveGame(int saveSlot);

        /// <summary>
        /// Process a game command
        /// </summary>
        /// <param name="command">Command to process</param>
        /// <returns>Result of command processing</returns>
        CommandResult ProcessCommand(string command);

        /// <summary>
        /// Get available commands for the current game state
        /// </summary>
        /// <returns>List of available commands</returns>
        List<string> GetAvailableCommands();

        /// <summary>
        /// Pause the game
        /// </summary>
        void PauseGame();

        /// <summary>
        /// Resume the game
        /// </summary>
        void ResumeGame();

        /// <summary>
        /// End the current game
        /// </summary>
        void EndGame();

        /// <summary>
        /// Process cheat commands
        /// </summary>
        /// <param name="cheatCode">Cheat code to process</param>
        /// <returns>True if cheat was processed successfully</returns>
        bool ProcessCheat(string cheatCode);

        /// <summary>
        /// Get game statistics
        /// </summary>
        /// <returns>Current game statistics</returns>
        GameStatistics GetGameStatistics();

        /// <summary>
        /// Update game time
        /// </summary>
        /// <param name="deltaTime">Time elapsed since last update</param>
        void UpdateGameTime(double deltaTime);

        /// <summary>
        /// Check if a specific feature is enabled
        /// </summary>
        /// <param name="featureName">Name of the feature to check</param>
        /// <returns>True if feature is enabled</returns>
        bool IsFeatureEnabled(string featureName);

        /// <summary>
        /// Enable or disable a game feature
        /// </summary>
        /// <param name="featureName">Name of the feature</param>
        /// <param name="enabled">Whether to enable or disable</param>
        void SetFeatureEnabled(string featureName, bool enabled);
    }

    /// <summary>
    /// Game state enumeration
    /// </summary>
    public enum GameState
    {
        NotStarted,
        Running,
        Paused,
        InCombat,
        InMenu,
        Loading,
        Saving,
        GameOver
    }

    /// <summary>
    /// Command processing result
    /// </summary>
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        public CommandResult(bool success, string message = "", string errorMessage = "")
        {
            Success = success;
            Message = message;
            ErrorMessage = errorMessage;
        }
    }

    /// <summary>
    /// Game statistics data
    /// </summary>
    public class GameStatistics
    {
        public double TotalPlayTime { get; set; }
        public int EnemiesDefeated { get; set; }
        public int ItemsCollected { get; set; }
        public int LocationsVisited { get; set; }
        public int TimesLeveledUp { get; set; }
        public int GoldEarned { get; set; }
        public int ExperienceGained { get; set; }
        public int DeathCount { get; set; }
        public int SaveCount { get; set; }
        public int LoadCount { get; set; }
        public Dictionary<string, int> CustomStats { get; set; } = new Dictionary<string, int>();
    }
} 