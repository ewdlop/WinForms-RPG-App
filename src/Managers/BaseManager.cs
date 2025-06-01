using System;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Managers
{
    /// <summary>
    /// Abstract base class for all game managers
    /// </summary>
    public abstract class BaseManager : IBaseManager
    {
        protected IEventManager EventManager { get; }
        
        public bool IsInitialized { get; private set; }
        public abstract string ManagerName { get; }

        protected BaseManager(IEventManager eventManager)
        {
            EventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
        }

        /// <summary>
        /// Initialize the manager and set up event subscriptions
        /// </summary>
        public virtual void Initialize()
        {
            if (IsInitialized)
                return;

            try
            {
                SubscribeToEvents();
                OnInitialize();
                IsInitialized = true;
                Console.WriteLine($"{ManagerName} initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing {ManagerName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Shutdown the manager and clean up resources
        /// </summary>
        public virtual void Shutdown()
        {
            if (!IsInitialized)
                return;

            try
            {
                OnShutdown();
                UnsubscribeFromEvents();
                IsInitialized = false;
                Console.WriteLine($"{ManagerName} shutdown successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error shutting down {ManagerName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Clean up resources and unsubscribe from events
        /// </summary>
        public virtual void Cleanup()
        {
            if (!IsInitialized)
                return;

            try
            {
                OnCleanup();
                UnsubscribeFromEvents();
                IsInitialized = false;
                Console.WriteLine($"{ManagerName} cleaned up successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cleaning up {ManagerName}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Subscribe to events that this manager needs to handle
        /// </summary>
        protected abstract void SubscribeToEvents();

        /// <summary>
        /// Unsubscribe from all events
        /// </summary>
        protected abstract void UnsubscribeFromEvents();

        /// <summary>
        /// Additional initialization logic specific to the manager
        /// </summary>
        protected virtual void OnInitialize()
        {
            // Override in derived classes if needed
        }

        /// <summary>
        /// Additional shutdown logic specific to the manager
        /// </summary>
        protected virtual void OnShutdown()
        {
            // Override in derived classes if needed
        }

        /// <summary>
        /// Additional cleanup logic specific to the manager
        /// </summary>
        protected virtual void OnCleanup()
        {
            // Override in derived classes if needed
        }

        /// <summary>
        /// Log a message with the manager name prefix
        /// </summary>
        protected void LogMessage(string message)
        {
            Console.WriteLine($"[{ManagerName}] {message}");
        }

        /// <summary>
        /// Log an error with the manager name prefix
        /// </summary>
        protected void LogError(string message, Exception ex = null)
        {
            Console.WriteLine($"[{ManagerName}] ERROR: {message}");
            if (ex != null)
            {
                Console.WriteLine($"[{ManagerName}] Exception: {ex.Message}");
            }
        }
    }
} 