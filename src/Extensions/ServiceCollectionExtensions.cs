using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WinFormsApp1.Interfaces;
using WinFormsApp1.Managers;

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
    }
} 