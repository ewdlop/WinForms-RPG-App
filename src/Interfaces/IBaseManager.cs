namespace WinFormsApp1.Interfaces
{
    /// <summary>
    /// Base interface for all game managers
    /// </summary>
    public interface IBaseManager
    {
        /// <summary>
        /// Initialize the manager and set up event subscriptions
        /// </summary>
        void Initialize();

        /// <summary>
        /// Shutdown the manager and clean up resources
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Clean up resources and unsubscribe from events
        /// </summary>
        void Cleanup();

        /// <summary>
        /// Whether the manager has been initialized
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Name of the manager for debugging purposes
        /// </summary>
        string ManagerName { get; }
    }
} 