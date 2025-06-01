using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WinFormsApp1.Interfaces;
using WinFormsApp1.Managers;
using WinFormsApp1.Services;

namespace WinFormsApp1.Extensions
{
    /// <summary>
    /// Extension methods for configuring services in the dependency injection container
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add all game services to the dependency injection container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddGameServices(this IServiceCollection services)
        {
            // Register core managers as singletons
            services.AddSingleton<IEventManager, EventManager>();
            services.AddSingleton<IPlayerManager, PlayerManager>();
            services.AddSingleton<IGameManager, GameManager>();
            services.AddSingleton<ICombatManager, CombatManager>();
            services.AddSingleton<IInventoryManager, InventoryManager>();
            services.AddSingleton<ILocationManager, LocationManager>();
            services.AddSingleton<ISkillManager, SkillManager>();
            services.AddSingleton<IUIManager, UIManager>();

            // Register advanced coordination service
            services.AddSingleton<IGameCoordinatorService, GameCoordinatorService>();

            // Register forms as transient (new instance each time)
            services.AddTransient<Form1>();
            services.AddTransient<InventoryForm>();
            services.AddTransient<MapForm>();
            services.AddTransient<SkillTreeForm>();

            return services;
        }

        /// <summary>
        /// Configure logging for the application
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddGameLogging(this IServiceCollection services)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                builder.SetMinimumLevel(LogLevel.Information);
                
                // Add custom log filtering
                builder.AddFilter("Microsoft", LogLevel.Warning);
                builder.AddFilter("System", LogLevel.Warning);
                builder.AddFilter("WinFormsApp1", LogLevel.Information);
            });

            return services;
        }

        /// <summary>
        /// Add configuration services
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddGameConfiguration(this IServiceCollection services)
        {
            // Configuration will be added by the host builder
            // This method is here for future configuration needs
            return services;
        }

        /// <summary>
        /// Add advanced game services that demonstrate complex dependency injection patterns
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddAdvancedGameServices(this IServiceCollection services)
        {
            // Register factory patterns
            services.AddSingleton<Func<string, IBaseManager>>(serviceProvider => managerType =>
            {
                return managerType.ToLowerInvariant() switch
                {
                    "game" => serviceProvider.GetRequiredService<IGameManager>(),
                    "player" => serviceProvider.GetRequiredService<IPlayerManager>(),
                    "combat" => serviceProvider.GetRequiredService<ICombatManager>(),
                    "inventory" => serviceProvider.GetRequiredService<IInventoryManager>(),
                    "location" => serviceProvider.GetRequiredService<ILocationManager>(),
                    "skill" => serviceProvider.GetRequiredService<ISkillManager>(),
                    _ => throw new ArgumentException($"Unknown manager type: {managerType}")
                };
            });

            // Register decorator pattern example
            //services.Decorate<IGameManager, GameManagerWithLogging>();

            return services;
        }
    }

    /// <summary>
    /// Example decorator that adds logging to GameManager operations
    /// Demonstrates decorator pattern with dependency injection
    /// </summary>
    public class GameManagerWithLogging : IGameManager
    {
        private readonly IGameManager _inner;
        private readonly ILogger<GameManagerWithLogging> _logger;

        public GameManagerWithLogging(IGameManager inner, ILogger<GameManagerWithLogging> logger)
        {
            _inner = inner;
            _logger = logger;
        }

        public GameState CurrentGameState => _inner.CurrentGameState;
        public bool IsGameRunning => _inner.IsGameRunning;
        public bool HasUnsavedChanges => _inner.HasUnsavedChanges;
        public string ManagerName => _inner.ManagerName;
        public bool IsInitialized => _inner.IsInitialized;

        public void StartNewGame(Player player)
        {
            _logger.LogInformation("Starting new game for player: {PlayerName}", player.Name);
            _inner.StartNewGame(player);
            _logger.LogInformation("New game started successfully");
        }

        public bool LoadGame(int saveSlot)
        {
            _logger.LogInformation("Loading game from slot: {SaveSlot}", saveSlot);
            var result = _inner.LoadGame(saveSlot);
            _logger.LogInformation("Load game result: {Success}", result);
            return result;
        }

        public bool SaveGame(int saveSlot)
        {
            _logger.LogInformation("Saving game to slot: {SaveSlot}", saveSlot);
            var result = _inner.SaveGame(saveSlot);
            _logger.LogInformation("Save game result: {Success}", result);
            return result;
        }

        public CommandResult ProcessCommand(string command)
        {
            _logger.LogDebug("Processing command: {Command}", command);
            var result = _inner.ProcessCommand(command);
            _logger.LogDebug("Command result: {Success} - {Message}", result.Success, result.Message);
            return result;
        }

        public List<string> GetAvailableCommands() => _inner.GetAvailableCommands();
        public void PauseGame() => _inner.PauseGame();
        public void ResumeGame() => _inner.ResumeGame();
        public void EndGame() => _inner.EndGame();
        public bool ProcessCheat(string cheatCode) => _inner.ProcessCheat(cheatCode);
        public GameStatistics GetGameStatistics() => _inner.GetGameStatistics();
        public void UpdateGameTime(double deltaTime) => _inner.UpdateGameTime(deltaTime);
        public bool IsFeatureEnabled(string featureName) => _inner.IsFeatureEnabled(featureName);
        public void SetFeatureEnabled(string featureName, bool enabled) => _inner.SetFeatureEnabled(featureName, enabled);
        public void Initialize() => _inner.Initialize();
        public void Shutdown() => _inner.Shutdown();
        public void Cleanup() => _inner.Cleanup();
    }
} 