using System;
using WinFormsApp1.Events;

namespace WinFormsApp1.Interfaces
{
    /// <summary>
    /// Interface for the central event management system
    /// </summary>
    public interface IEventManager
    {
        /// <summary>
        /// Subscribe to events of a specific type
        /// </summary>
        /// <typeparam name="T">Type of event to subscribe to</typeparam>
        /// <param name="handler">Handler method to call when event is published</param>
        void Subscribe<T>(Action<T> handler) where T : GameEvent;

        /// <summary>
        /// Unsubscribe from events of a specific type
        /// </summary>
        /// <typeparam name="T">Type of event to unsubscribe from</typeparam>
        /// <param name="handler">Handler method to remove</param>
        void Unsubscribe<T>(Action<T> handler) where T : GameEvent;

        /// <summary>
        /// Publish an event to all subscribers
        /// </summary>
        /// <typeparam name="T">Type of event to publish</typeparam>
        /// <param name="gameEvent">Event instance to publish</param>
        void Publish<T>(T gameEvent) where T : GameEvent;

        /// <summary>
        /// Subscribe to events with a filter condition
        /// </summary>
        /// <typeparam name="T">Type of event to subscribe to</typeparam>
        /// <param name="handler">Handler method to call when event is published</param>
        /// <param name="filter">Filter condition that must be true for handler to be called</param>
        void Subscribe<T>(Action<T> handler, Func<T, bool> filter) where T : GameEvent;

        /// <summary>
        /// Clear all subscriptions for a specific event type
        /// </summary>
        /// <typeparam name="T">Type of event to clear subscriptions for</typeparam>
        void ClearSubscriptions<T>() where T : GameEvent;

        /// <summary>
        /// Clear all subscriptions for all event types
        /// </summary>
        void ClearAllSubscriptions();

        /// <summary>
        /// Get the number of subscribers for a specific event type
        /// </summary>
        /// <typeparam name="T">Type of event to count subscribers for</typeparam>
        /// <returns>Number of subscribers</returns>
        int GetSubscriberCount<T>() where T : GameEvent;

        /// <summary>
        /// Enable or disable event publishing (useful for testing)
        /// </summary>
        /// <param name="enabled">Whether event publishing should be enabled</param>
        void SetEventPublishingEnabled(bool enabled);
    }
} 