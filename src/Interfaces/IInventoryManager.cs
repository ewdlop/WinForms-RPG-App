using System.Collections.Generic;

namespace WinFormsApp1.Interfaces
{
    /// <summary>
    /// Interface for inventory and item management
    /// </summary>
    public interface IInventoryManager : IBaseManager
    {
        /// <summary>
        /// Current player whose inventory is being managed
        /// </summary>
        Player CurrentPlayer { get; }

        /// <summary>
        /// Maximum inventory capacity
        /// </summary>
        int MaxInventorySize { get; set; }

        /// <summary>
        /// Current number of items in inventory
        /// </summary>
        int CurrentInventorySize { get; }

        /// <summary>
        /// Whether inventory is full
        /// </summary>
        bool IsInventoryFull { get; }

        /// <summary>
        /// Set the current player for inventory management
        /// </summary>
        /// <param name="player">Player to manage inventory for</param>
        void SetCurrentPlayer(Player player);

        /// <summary>
        /// Add an item to the player's inventory
        /// </summary>
        /// <param name="item">Item to add</param>
        /// <param name="quantity">Quantity to add</param>
        /// <param name="source">Source of the item</param>
        /// <returns>True if item was added successfully</returns>
        bool AddItem(Item item, int quantity = 1, string source = "");

        /// <summary>
        /// Remove an item from the player's inventory
        /// </summary>
        /// <param name="item">Item to remove</param>
        /// <param name="quantity">Quantity to remove</param>
        /// <param name="reason">Reason for removal</param>
        /// <returns>True if item was removed successfully</returns>
        bool RemoveItem(Item item, int quantity = 1, string reason = "");

        /// <summary>
        /// Remove an item by name from the inventory
        /// </summary>
        /// <param name="itemName">Name of item to remove</param>
        /// <param name="quantity">Quantity to remove</param>
        /// <param name="reason">Reason for removal</param>
        /// <returns>True if item was removed successfully</returns>
        bool RemoveItem(string itemName, int quantity = 1, string reason = "");

        /// <summary>
        /// Use an item from the inventory
        /// </summary>
        /// <param name="item">Item to use</param>
        /// <returns>True if item was used successfully</returns>
        bool UseItem(Item item);

        /// <summary>
        /// Use an item by name from the inventory
        /// </summary>
        /// <param name="itemName">Name of item to use</param>
        /// <returns>True if item was used successfully</returns>
        bool UseItem(string itemName);

        /// <summary>
        /// Equip an item
        /// </summary>
        /// <param name="item">Item to equip</param>
        /// <returns>Previously equipped item, if any</returns>
        Item EquipItem(Item item);

        /// <summary>
        /// Unequip an item
        /// </summary>
        /// <param name="slot">Equipment slot to unequip</param>
        /// <returns>Unequipped item, if any</returns>
        Item UnequipItem(EquipmentSlot slot);

        /// <summary>
        /// Get equipped item in a specific slot
        /// </summary>
        /// <param name="slot">Equipment slot to check</param>
        /// <returns>Equipped item, or null if slot is empty</returns>
        Item GetEquippedItem(EquipmentSlot slot);

        /// <summary>
        /// Check if player has a specific item
        /// </summary>
        /// <param name="itemName">Name of item to check</param>
        /// <returns>True if player has the item</returns>
        bool HasItem(string itemName);

        /// <summary>
        /// Check if player has a specific item
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>True if player has the item</returns>
        bool HasItem(Item item);

        /// <summary>
        /// Get quantity of a specific item
        /// </summary>
        /// <param name="itemName">Name of item to count</param>
        /// <returns>Quantity of the item</returns>
        int GetItemQuantity(string itemName);

        /// <summary>
        /// Find an item by name in the inventory
        /// </summary>
        /// <param name="itemName">Name of item to find</param>
        /// <returns>Item if found, null otherwise</returns>
        Item FindItem(string itemName);

        /// <summary>
        /// Get all items of a specific type
        /// </summary>
        /// <param name="itemType">Type of items to get</param>
        /// <returns>List of items of the specified type</returns>
        List<Item> GetItemsByType(ItemType itemType);

        /// <summary>
        /// Get all consumable items
        /// </summary>
        /// <returns>List of consumable items</returns>
        List<Item> GetConsumableItems();

        /// <summary>
        /// Get all equipment items
        /// </summary>
        /// <returns>List of equipment items</returns>
        List<Item> GetEquipmentItems();

        /// <summary>
        /// Sort inventory by a specific criteria
        /// </summary>
        /// <param name="sortBy">Criteria to sort by</param>
        void SortInventory(InventorySortCriteria sortBy);

        /// <summary>
        /// Clear the entire inventory
        /// </summary>
        /// <param name="reason">Reason for clearing</param>
        void ClearInventory(string reason = "");

        /// <summary>
        /// Get inventory value (total worth of all items)
        /// </summary>
        /// <returns>Total value of inventory</returns>
        int GetInventoryValue();

        /// <summary>
        /// Check if an item can be equipped by the current player
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>True if item can be equipped</returns>
        bool CanEquipItem(Item item);

        /// <summary>
        /// Get the equipment slot for an item
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <returns>Equipment slot for the item</returns>
        EquipmentSlot GetEquipmentSlot(Item item);
    }

    /// <summary>
    /// Criteria for sorting inventory
    /// </summary>
    public enum InventorySortCriteria
    {
        Name,
        Type,
        Value,
        Quantity,
        DateAdded
    }
} 