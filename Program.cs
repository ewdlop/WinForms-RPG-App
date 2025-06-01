using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using WinFormsApp1.Extensions;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1
{
    /// <summary>
    /// Simple class to use for logger type argument
    /// </summary>
    internal class ApplicationHost
    {
    }

    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Configure WinForms
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Create and configure the host builder
            var builder = Host.CreateApplicationBuilder(args);

            // Configure services
            builder.Services.AddGameServices();
            builder.Services.AddGameLogging();
            builder.Services.AddGameConfiguration();

            // Configure additional host settings
            builder.Logging.SetMinimumLevel(LogLevel.Information);

            // Build the host
            using var host = builder.Build();

            try
            {
                // Get the logger
                var logger = host.Services.GetRequiredService<ILogger<ApplicationHost>>();
                logger.LogInformation("Starting Realm of Aethermoor application");

                // Initialize all managers
                InitializeManagers(host.Services, logger);

                // Create and run the main form
                using var scope = host.Services.CreateScope();
                var mainForm = scope.ServiceProvider.GetRequiredService<Form1>();
                
                logger.LogInformation("Starting WinForms application");
                Application.Run(mainForm);
                
                logger.LogInformation("WinForms application closed");
            }
            catch (Exception ex)
            {
                // Log any unhandled exceptions
                var logger = host.Services.GetService<ILogger<ApplicationHost>>();
                logger?.LogCritical(ex, "Application terminated unexpectedly");
                throw;
            }
        }

        private static void InitializeManagers(IServiceProvider serviceProvider, ILogger logger)
        {
            try
            {
                logger.LogInformation("Initializing game managers...");

                // Initialize managers in dependency order
                var playerManager = serviceProvider.GetRequiredService<IPlayerManager>();
                var gameManager = serviceProvider.GetRequiredService<IGameManager>();
                var combatManager = serviceProvider.GetRequiredService<ICombatManager>();
                var inventoryManager = serviceProvider.GetRequiredService<IInventoryManager>();
                var locationManager = serviceProvider.GetRequiredService<ILocationManager>();
                var skillManager = serviceProvider.GetRequiredService<ISkillManager>();
                var uiManager = serviceProvider.GetRequiredService<IUIManager>();

                // Initialize all managers
                if (playerManager is IBaseManager playerBaseManager)
                    playerBaseManager.Initialize();

                if (gameManager is IBaseManager gameBaseManager)
                    gameBaseManager.Initialize();

                if (combatManager is IBaseManager combatBaseManager)
                    combatBaseManager.Initialize();

                if (inventoryManager is IBaseManager inventoryBaseManager)
                    inventoryBaseManager.Initialize();

                if (locationManager is IBaseManager locationBaseManager)
                    locationBaseManager.Initialize();

                if (skillManager is IBaseManager skillBaseManager)
                    skillBaseManager.Initialize();

                if (uiManager is IBaseManager uiBaseManager)
                    uiBaseManager.Initialize();

                logger.LogInformation("All managers initialized successfully");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Failed to initialize managers");
                throw;
            }
        }
    }
}