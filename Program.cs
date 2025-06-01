using WinFormsApp1.Managers;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Initialize the event-driven architecture
            var serviceContainer = InitializeServices();
            
            // Create and run the main form with dependency injection
            var mainForm = new Form1(serviceContainer);
            Application.Run(mainForm);
        }

        private static GameServiceContainer InitializeServices()
        {
            var serviceContainer = new GameServiceContainer();

            // Register core services
            var eventManager = new EventManager();
            serviceContainer.RegisterSingleton<IEventManager>(eventManager);

            // Register managers in dependency order
            var playerManager = new PlayerManager(eventManager);
            serviceContainer.RegisterSingleton<IPlayerManager>(playerManager);

            var gameManager = new GameManager(eventManager, playerManager);
            serviceContainer.RegisterSingleton<IGameManager>(gameManager);

            var combatManager = new CombatManager(eventManager, playerManager);
            serviceContainer.RegisterSingleton<ICombatManager>(combatManager);

            var inventoryManager = new InventoryManager(eventManager, playerManager);
            serviceContainer.RegisterSingleton<IInventoryManager>(inventoryManager);

            var locationManager = new LocationManager(eventManager, inventoryManager);
            serviceContainer.RegisterSingleton<ILocationManager>(locationManager);

            var skillManager = new SkillManager(eventManager, playerManager);
            serviceContainer.RegisterSingleton<ISkillManager>(skillManager);

            // Initialize all managers
            playerManager.Initialize();
            gameManager.Initialize();
            combatManager.Initialize();
            inventoryManager.Initialize();
            locationManager.Initialize();
            skillManager.Initialize();

            return serviceContainer;
        }
    }
}