using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WinFormsApp1.Events;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Managers
{
    /// <summary>
    /// Central event management system for the game
    /// </summary>
    public class EventManager : IEventManager
    {
        private readonly ConcurrentDictionary<Type, List<IEventSubscription>> _subscriptions;
        private readonly object _lockObject = new object();
        private bool _eventPublishingEnabled = true;

        public EventManager()
        {
            _subscriptions = new ConcurrentDictionary<Type, List<IEventSubscription>>();
        }

        /// <summary>
        /// Subscribe to events of a specific type
        /// </summary>
        public void Subscribe<T>(Action<T> handler) where T : GameEvent
        {
            Subscribe(handler, null);
        }

        /// <summary>
        /// Subscribe to events with a filter condition
        /// </summary>
        public void Subscribe<T>(Action<T> handler, Func<T, bool> filter) where T : GameEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);
            var subscription = new EventSubscription<T>(handler, filter);

            lock (_lockObject)
            {
                if (!_subscriptions.ContainsKey(eventType))
                {
                    _subscriptions[eventType] = new List<IEventSubscription>();
                }
                _subscriptions[eventType].Add(subscription);
            }
        }

        /// <summary>
        /// Unsubscribe from events of a specific type
        /// </summary>
        public void Unsubscribe<T>(Action<T> handler) where T : GameEvent
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var eventType = typeof(T);

            lock (_lockObject)
            {
                if (_subscriptions.ContainsKey(eventType))
                {
                    _subscriptions[eventType].RemoveAll(s => 
                        s is EventSubscription<T> subscription && 
                        subscription.Handler.Equals(handler));

                    // Remove empty subscription lists
                    if (_subscriptions[eventType].Count == 0)
                    {
                        _subscriptions.TryRemove(eventType, out _);
                    }
                }
            }
        }

        /// <summary>
        /// Publish an event to all subscribers
        /// </summary>
        public void Publish<T>(T gameEvent) where T : GameEvent
        {
            if (gameEvent == null)
                throw new ArgumentNullException(nameof(gameEvent));

            if (!_eventPublishingEnabled)
                return;

            var eventType = typeof(T);
            List<IEventSubscription> subscriptions = null;

            lock (_lockObject)
            {
                if (_subscriptions.ContainsKey(eventType))
                {
                    // Create a copy to avoid modification during iteration
                    subscriptions = new List<IEventSubscription>(_subscriptions[eventType]);
                }
            }

            if (subscriptions != null)
            {
                // Sort by priority (higher priority first)
                var sortedSubscriptions = subscriptions
                    .OrderByDescending(s => gameEvent.Priority)
                    .ToList();

                foreach (var subscription in sortedSubscriptions)
                {
                    try
                    {
                        if (subscription is EventSubscription<T> typedSubscription)
                        {
                            // Check filter condition if present
                            if (typedSubscription.Filter == null || typedSubscription.Filter(gameEvent))
                            {
                                typedSubscription.Handler(gameEvent);

                                // Stop processing if event was cancelled
                                if (gameEvent.CanBeCancelled && gameEvent.IsCancelled)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception but continue processing other handlers
                        Console.WriteLine($"Error in event handler for {eventType.Name}: {ex.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Clear all subscriptions for a specific event type
        /// </summary>
        public void ClearSubscriptions<T>() where T : GameEvent
        {
            var eventType = typeof(T);
            lock (_lockObject)
            {
                _subscriptions.TryRemove(eventType, out _);
            }
        }

        /// <summary>
        /// Clear all subscriptions for all event types
        /// </summary>
        public void ClearAllSubscriptions()
        {
            lock (_lockObject)
            {
                _subscriptions.Clear();
            }
        }

        /// <summary>
        /// Get the number of subscribers for a specific event type
        /// </summary>
        public int GetSubscriberCount<T>() where T : GameEvent
        {
            var eventType = typeof(T);
            lock (_lockObject)
            {
                return _subscriptions.ContainsKey(eventType) ? _subscriptions[eventType].Count : 0;
            }
        }

        /// <summary>
        /// Enable or disable event publishing
        /// </summary>
        public void SetEventPublishingEnabled(bool enabled)
        {
            _eventPublishingEnabled = enabled;
        }

        /// <summary>
        /// Interface for event subscriptions
        /// </summary>
        private interface IEventSubscription
        {
        }

        /// <summary>
        /// Concrete event subscription implementation
        /// </summary>
        private class EventSubscription<T> : IEventSubscription where T : GameEvent
        {
            public Action<T> Handler { get; }
            public Func<T, bool> Filter { get; }

            public EventSubscription(Action<T> handler, Func<T, bool> filter)
            {
                Handler = handler ?? throw new ArgumentNullException(nameof(handler));
                Filter = filter;
            }
        }
    }
} 