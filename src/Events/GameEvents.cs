using System;

namespace WinFormsApp1.Events
{
    /// <summary>
    /// Base class for all game events in the event-driven architecture
    /// </summary>
    public abstract class GameEvent
    {
        /// <summary>
        /// Unique identifier for this event instance
        /// </summary>
        public string EventId { get; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Timestamp when the event was created
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.UtcNow;

        /// <summary>
        /// Priority level for event processing (higher numbers = higher priority)
        /// </summary>
        public virtual int Priority => 0;

        /// <summary>
        /// Whether this event can be cancelled by handlers
        /// </summary>
        public virtual bool CanBeCancelled => false;

        /// <summary>
        /// Whether this event has been cancelled
        /// </summary>
        public bool IsCancelled { get; set; }

        /// <summary>
        /// Optional source identifier for debugging
        /// </summary>
        public string Source { get; set; } = string.Empty;
    }

    /// <summary>
    /// Event arguments for inventory-related actions
    /// </summary>
    public enum InventoryAction
    {
        ItemAdded,
        ItemRemoved,
        ItemEquipped,
        ItemUnequipped,
        ItemUsed,
        InventoryCleared
    }

    /// <summary>
    /// Event arguments for combat-related actions
    /// </summary>
    public enum CombatAction
    {
        Attack,
        Defend,
        Flee,
        UseItem,
        CastSpell
    }

    /// <summary>
    /// Event arguments for player stat changes
    /// </summary>
    public enum StatType
    {
        Health,
        MaxHealth,
        Attack,
        Defense,
        Experience,
        Level,
        Gold,
        SkillPoints
    }
} 