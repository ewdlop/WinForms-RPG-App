using System.Collections.Generic;
using System.Threading.Tasks;

namespace WinFormsApp1.Interfaces
{
    /// <summary>
    /// Service that coordinates complex operations between multiple managers
    /// Demonstrates advanced dependency injection patterns
    /// </summary>
    public interface IGameCoordinatorService
    {
        /// <summary>
        /// Perform a complex action that involves multiple managers
        /// </summary>
        /// <param name="actionType">Type of action to perform</param>
        /// <param name="parameters">Action parameters</param>
        /// <returns>Result of the coordinated action</returns>
        Task<CoordinatedActionResult> PerformCoordinatedActionAsync(string actionType, Dictionary<string, object> parameters);

        /// <summary>
        /// Initialize a new player with full setup across all managers
        /// </summary>
        /// <param name="player">Player to initialize</param>
        /// <returns>True if initialization was successful</returns>
        Task<bool> InitializePlayerAsync(Player player);

        /// <summary>
        /// Perform a complete game state validation across all managers
        /// </summary>
        /// <returns>Validation results</returns>
        Task<GameStateValidationResult> ValidateGameStateAsync();

        /// <summary>
        /// Execute a complex command that may involve multiple managers
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>Execution result</returns>
        Task<CommandResult> ExecuteComplexCommandAsync(string command);

        /// <summary>
        /// Synchronize data between managers
        /// </summary>
        /// <returns>True if synchronization was successful</returns>
        Task<bool> SynchronizeManagersAsync();

        /// <summary>
        /// Get comprehensive game status from all managers
        /// </summary>
        /// <returns>Complete game status</returns>
        Task<ComprehensiveGameStatus> GetComprehensiveStatusAsync();

        /// <summary>
        /// Perform automated game maintenance tasks
        /// </summary>
        /// <returns>Maintenance results</returns>
        Task<MaintenanceResult> PerformMaintenanceAsync();
    }

    /// <summary>
    /// Result of a coordinated action
    /// </summary>
    public class CoordinatedActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> Results { get; set; } = new Dictionary<string, object>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Result of game state validation
    /// </summary>
    public class GameStateValidationResult
    {
        public bool IsValid { get; set; }
        public List<ValidationIssue> Issues { get; set; } = new List<ValidationIssue>();
        public Dictionary<string, bool> ManagerStates { get; set; } = new Dictionary<string, bool>();
    }

    /// <summary>
    /// Validation issue details
    /// </summary>
    public class ValidationIssue
    {
        public string Manager { get; set; }
        public string Issue { get; set; }
        public string Severity { get; set; }
        public string Recommendation { get; set; }
    }

    /// <summary>
    /// Comprehensive game status
    /// </summary>
    public class ComprehensiveGameStatus
    {
        public GameState GameState { get; set; }
        public Player CurrentPlayer { get; set; }
        public string CurrentLocation { get; set; }
        public bool InCombat { get; set; }
        public Dictionary<string, object> ManagerStatuses { get; set; } = new Dictionary<string, object>();
        public GameStatistics Statistics { get; set; }
        public List<string> ActiveEffects { get; set; } = new List<string>();
    }

    /// <summary>
    /// Maintenance operation result
    /// </summary>
    public class MaintenanceResult
    {
        public bool Success { get; set; }
        public List<string> OperationsPerformed { get; set; } = new List<string>();
        public Dictionary<string, object> Results { get; set; } = new Dictionary<string, object>();
        public TimeSpan Duration { get; set; }
    }
} 