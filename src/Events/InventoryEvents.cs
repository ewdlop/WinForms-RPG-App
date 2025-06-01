using System.Collections.Generic;

namespace WinFormsApp1.Events
{
    /// <summary>
    /// Event published when inventory is updated
    /// </summary>
    public class InventoryUpdatedEvent : GameEvent
    {
        public Player Player { get; set; }
        public List<Item> Items { get; set; }
        public InventoryAction Action { get; set; }
        public Item AffectedItem { get; set; }
        public int Quantity { get; set; } = 1;

        public InventoryUpdatedEvent(Player player, InventoryAction action, Item affectedItem, int quantity = 1)
        {
            Player = player;
            Items = new List<Item>(player.Inventory);
            Action = action;
            AffectedItem = affectedItem;
            Quantity = quantity;
            Source = "InventoryManager";
        }
    }

    /// <summary>
    /// Event published when an item is added to inventory
    /// </summary>
    public class ItemAddedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public string Source { get; set; }

        public ItemAddedEvent(Player player, Item item, int quantity = 1, string source = "")
        {
            Player = player;
            Item = item;
            Quantity = quantity;
            Source = source;
        }
    }

    /// <summary>
    /// Event published when an item is removed from inventory
    /// </summary>
    public class ItemRemovedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; }

        public ItemRemovedEvent(Player player, Item item, int quantity = 1, string reason = "")
        {
            Player = player;
            Item = item;
            Quantity = quantity;
            Reason = reason;
            Source = "InventoryManager";
        }
    }

    /// <summary>
    /// Event published when an item is equipped
    /// </summary>
    public class ItemEquippedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Item EquippedItem { get; set; }
        public Item PreviousItem { get; set; }
        public EquipmentSlot Slot { get; set; }

        public ItemEquippedEvent(Player player, Item equippedItem, Item previousItem, EquipmentSlot slot)
        {
            Player = player;
            EquippedItem = equippedItem;
            PreviousItem = previousItem;
            Slot = slot;
            Source = "InventoryManager";
        }
    }

    /// <summary>
    /// Event published when an item is unequipped
    /// </summary>
    public class ItemUnequippedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Item UnequippedItem { get; set; }
        public EquipmentSlot Slot { get; set; }

        public ItemUnequippedEvent(Player player, Item unequippedItem, EquipmentSlot slot)
        {
            Player = player;
            UnequippedItem = unequippedItem;
            Slot = slot;
            Source = "InventoryManager";
        }
    }

    /// <summary>
    /// Event published when an item is used
    /// </summary>
    public class ItemUsedEvent : GameEvent
    {
        public Player Player { get; set; }
        public Item Item { get; set; }
        public bool WasSuccessful { get; set; }
        public string Effect { get; set; }
        public int EffectValue { get; set; }

        public override bool CanBeCancelled => true;

        public ItemUsedEvent(Player player, Item item, bool wasSuccessful = true, string effect = "", int effectValue = 0)
        {
            Player = player;
            Item = item;
            WasSuccessful = wasSuccessful;
            Effect = effect;
            EffectValue = effectValue;
            Source = "InventoryManager";
        }
    }

    /// <summary>
    /// Event published when inventory is cleared
    /// </summary>
    public class InventoryClearedEvent : GameEvent
    {
        public Player Player { get; set; }
        public List<Item> ClearedItems { get; set; }
        public string Reason { get; set; }

        public InventoryClearedEvent(Player player, List<Item> clearedItems, string reason = "")
        {
            Player = player;
            ClearedItems = new List<Item>(clearedItems);
            Reason = reason;
            Source = "InventoryManager";
        }
    }
} 