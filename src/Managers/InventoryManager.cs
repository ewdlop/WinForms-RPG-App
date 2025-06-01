using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsApp1.Events;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Managers
{
    /// <summary>
    /// Manages player inventory and item operations
    /// </summary>
    public class InventoryManager : BaseManager, IInventoryManager
    {
        private readonly IPlayerManager _playerManager;
        private Player _currentPlayer;
        private int _maxInventorySize;
        private readonly Dictionary<string, int> _itemQuantities;

        public override string ManagerName => "InventoryManager";
        public Player CurrentPlayer => _currentPlayer;
        public int MaxInventorySize 
        { 
            get => _maxInventorySize; 
            set => _maxInventorySize = Math.Max(1, value); 
        }
        public int CurrentInventorySize => _currentPlayer?.Inventory?.Count ?? 0;
        public bool IsInventoryFull => CurrentInventorySize >= MaxInventorySize;

        public InventoryManager(IEventManager eventManager, IPlayerManager playerManager) : base(eventManager)
        {
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _maxInventorySize = 50; // Default inventory size
            _itemQuantities = new Dictionary<string, int>();
        }

        protected override void SubscribeToEvents()
        {
            // Subscribe to player changes to update current player
            EventManager.Subscribe<GameStartedEvent>(OnGameStarted);
        }

        protected override void UnsubscribeFromEvents()
        {
            EventManager.ClearSubscriptions<GameStartedEvent>();
        }

        public void SetCurrentPlayer(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            _currentPlayer = player;
            UpdateItemQuantities();
            LogMessage($"Current player set to: {player.Name}");
        }

        public bool AddItem(Item item, int quantity = 1, string source = "")
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (item == null || quantity <= 0)
                return false;

            // Check inventory space
            if (IsInventoryFull && !HasItem(item))
            {
                LogMessage($"Cannot add {item.Name}: Inventory is full");
                return false;
            }

            LogMessage($"Adding {quantity}x {item.Name} to inventory (source: {source})");

            // Handle stackable items
            var existingItem = _currentPlayer.Inventory.FirstOrDefault(i => i.Name == item.Name);
            if (existingItem != null && IsStackable(item))
            {
                // Update quantity tracking
                _itemQuantities[item.Name] = _itemQuantities.GetValueOrDefault(item.Name, 1) + quantity;
            }
            else
            {
                // Add new item
                for (int i = 0; i < quantity; i++)
                {
                    if (CurrentInventorySize >= MaxInventorySize)
                        break;

                    _currentPlayer.Inventory.Add(item);
                }
                _itemQuantities[item.Name] = _itemQuantities.GetValueOrDefault(item.Name, 0) + quantity;
            }

            // Publish events
            EventManager.Publish(new ItemAddedEvent(_currentPlayer, item, quantity, source));
            EventManager.Publish(new InventoryUpdatedEvent(_currentPlayer, InventoryAction.ItemAdded, item, quantity));

            LogMessage($"Successfully added {quantity}x {item.Name}");
            return true;
        }

        public bool RemoveItem(Item item, int quantity = 1, string reason = "")
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (item == null || quantity <= 0)
                return false;

            if (!HasItem(item))
            {
                LogMessage($"Cannot remove {item.Name}: Item not in inventory");
                return false;
            }

            LogMessage($"Removing {quantity}x {item.Name} from inventory (reason: {reason})");

            var currentQuantity = GetItemQuantity(item.Name);
            var quantityToRemove = Math.Min(quantity, currentQuantity);

            // Remove items
            for (int i = 0; i < quantityToRemove; i++)
            {
                var itemToRemove = _currentPlayer.Inventory.FirstOrDefault(i => i.Name == item.Name);
                if (itemToRemove != null)
                {
                    _currentPlayer.Inventory.Remove(itemToRemove);
                }
            }

            // Update quantity tracking
            _itemQuantities[item.Name] = Math.Max(0, _itemQuantities.GetValueOrDefault(item.Name, 0) - quantityToRemove);
            if (_itemQuantities[item.Name] == 0)
            {
                _itemQuantities.Remove(item.Name);
            }

            // Publish events
            EventManager.Publish(new ItemRemovedEvent(_currentPlayer, item, quantityToRemove, reason));
            EventManager.Publish(new InventoryUpdatedEvent(_currentPlayer, InventoryAction.ItemRemoved, item, quantityToRemove));

            LogMessage($"Successfully removed {quantityToRemove}x {item.Name}");
            return true;
        }

        public bool RemoveItem(string itemName, int quantity = 1, string reason = "")
        {
            var item = FindItem(itemName);
            return item != null && RemoveItem(item, quantity, reason);
        }

        public bool UseItem(Item item)
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (item == null || !HasItem(item))
                return false;

            LogMessage($"Using {item.Name}");

            var success = false;
            var effect = "";
            var effectValue = 0;

            // Handle different item types
            switch (item.Type)
            {
                case ItemType.Consumable:
                    success = UseConsumableItem(item, out effect, out effectValue);
                    break;

                case ItemType.Weapon:
                case ItemType.Armor:
                    // Equipment items are equipped, not used
                    var previousItem = EquipItem(item);
                    success = true;
                    effect = "Equipped";
                    break;

                default:
                    LogMessage($"Cannot use {item.Name}: Item type not usable");
                    break;
            }

            if (success)
            {
                // Remove consumable items after use
                if (item.Type == ItemType.Consumable)
                {
                    RemoveItem(item, 1, "Used");
                }

                EventManager.Publish(new ItemUsedEvent(_currentPlayer, item, success, effect, effectValue));
                LogMessage($"Successfully used {item.Name}");
            }

            return success;
        }

        public bool UseItem(string itemName)
        {
            var item = FindItem(itemName);
            return item != null && UseItem(item);
        }

        public Item EquipItem(Item item)
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            if (item == null || !CanEquipItem(item))
                return null;

            var slot = GetEquipmentSlot(item);
            var previousItem = GetEquippedItem(slot);

            LogMessage($"Equipping {item.Name} to {slot} slot");

            // Unequip previous item if any
            if (previousItem != null)
            {
                UnequipItem(slot);
            }

            // Equip new item
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    _currentPlayer.EquippedWeapon = item;
                    break;
                case EquipmentSlot.Armor:
                    _currentPlayer.EquippedArmor = item;
                    break;
            }

            // Remove from inventory if it's there
            if (HasItem(item))
            {
                RemoveItem(item, 1, "Equipped");
            }

            // Update player stats
            ApplyItemStats(item, true);

            EventManager.Publish(new ItemEquippedEvent(_currentPlayer, item, previousItem, slot));
            EventManager.Publish(new InventoryUpdatedEvent(_currentPlayer, InventoryAction.ItemEquipped, item));

            LogMessage($"Successfully equipped {item.Name}");
            return previousItem;
        }

        public Item UnequipItem(EquipmentSlot slot)
        {
            if (_currentPlayer == null)
                throw new InvalidOperationException("No current player set");

            var item = GetEquippedItem(slot);
            if (item == null)
                return null;

            LogMessage($"Unequipping {item.Name} from {slot} slot");

            // Remove from equipment slot
            switch (slot)
            {
                case EquipmentSlot.Weapon:
                    _currentPlayer.EquippedWeapon = null;
                    break;
                case EquipmentSlot.Armor:
                    _currentPlayer.EquippedArmor = null;
                    break;
            }

            // Add back to inventory if there's space
            if (!IsInventoryFull)
            {
                AddItem(item, 1, "Unequipped");
            }

            // Remove item stats
            ApplyItemStats(item, false);

            EventManager.Publish(new ItemUnequippedEvent(_currentPlayer, item, slot));
            EventManager.Publish(new InventoryUpdatedEvent(_currentPlayer, InventoryAction.ItemUnequipped, item));

            LogMessage($"Successfully unequipped {item.Name}");
            return item;
        }

        public Item GetEquippedItem(EquipmentSlot slot)
        {
            if (_currentPlayer == null)
                return null;

            return slot switch
            {
                EquipmentSlot.Weapon => _currentPlayer.EquippedWeapon,
                EquipmentSlot.Armor => _currentPlayer.EquippedArmor,
                _ => null
            };
        }

        public bool HasItem(string itemName)
        {
            return _currentPlayer?.Inventory?.Any(item => item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase)) == true;
        }

        public bool HasItem(Item item)
        {
            return item != null && HasItem(item.Name);
        }

        public int GetItemQuantity(string itemName)
        {
            return _itemQuantities.GetValueOrDefault(itemName, 0);
        }

        public Item FindItem(string itemName)
        {
            return _currentPlayer?.Inventory?.FirstOrDefault(item => 
                item.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
        }

        public List<Item> GetItemsByType(ItemType itemType)
        {
            return _currentPlayer?.Inventory?.Where(item => item.Type == itemType).ToList() ?? new List<Item>();
        }

        public List<Item> GetConsumableItems()
        {
            return GetItemsByType(ItemType.Consumable);
        }

        public List<Item> GetEquipmentItems()
        {
            var equipment = new List<Item>();
            equipment.AddRange(GetItemsByType(ItemType.Weapon));
            equipment.AddRange(GetItemsByType(ItemType.Armor));
            return equipment;
        }

        public void SortInventory(InventorySortCriteria sortBy)
        {
            if (_currentPlayer?.Inventory == null)
                return;

            LogMessage($"Sorting inventory by {sortBy}");

            var sortedItems = sortBy switch
            {
                InventorySortCriteria.Name => _currentPlayer.Inventory.OrderBy(i => i.Name).ToList(),
                InventorySortCriteria.Type => _currentPlayer.Inventory.OrderBy(i => i.Type).ThenBy(i => i.Name).ToList(),
                InventorySortCriteria.Value => _currentPlayer.Inventory.OrderByDescending(i => i.Value).ToList(),
                InventorySortCriteria.Quantity => _currentPlayer.Inventory.OrderByDescending(i => GetItemQuantity(i.Name)).ToList(),
                _ => _currentPlayer.Inventory.OrderBy(i => i.Name).ToList()
            };

            _currentPlayer.Inventory.Clear();
            _currentPlayer.Inventory.AddRange(sortedItems);

            EventManager.Publish(new InventoryUpdatedEvent(_currentPlayer, InventoryAction.InventoryCleared, null));
        }

        public void ClearInventory(string reason = "")
        {
            if (_currentPlayer?.Inventory == null)
                return;

            LogMessage($"Clearing inventory (reason: {reason})");

            var clearedItems = new List<Item>(_currentPlayer.Inventory);
            _currentPlayer.Inventory.Clear();
            _itemQuantities.Clear();

            EventManager.Publish(new InventoryClearedEvent(_currentPlayer, clearedItems, reason));
            EventManager.Publish(new InventoryUpdatedEvent(_currentPlayer, InventoryAction.InventoryCleared, null));
        }

        public int GetInventoryValue()
        {
            return _currentPlayer?.Inventory?.Sum(item => item.Value * GetItemQuantity(item.Name)) ?? 0;
        }

        public bool CanEquipItem(Item item)
        {
            if (_currentPlayer == null || item == null)
                return false;

            // Check if item is equipment
            if (item.Type != ItemType.Weapon && item.Type != ItemType.Armor)
                return false;

            // Check class restrictions (if any)
            // This could be expanded based on game requirements

            return true;
        }

        public EquipmentSlot GetEquipmentSlot(Item item)
        {
            if (item == null)
                return EquipmentSlot.None;

            return item.Type switch
            {
                ItemType.Weapon => EquipmentSlot.Weapon,
                ItemType.Armor => EquipmentSlot.Armor,
                _ => EquipmentSlot.None
            };
        }

        private bool UseConsumableItem(Item item, out string effect, out int effectValue)
        {
            effect = "";
            effectValue = 0;

            var itemName = item.Name.ToLower();

            if (itemName.Contains("health") && itemName.Contains("potion"))
            {
                var healAmount = item.Value;
                _playerManager.Heal(healAmount, $"Used {item.Name}");
                effect = "Healing";
                effectValue = healAmount;
                return true;
            }

            // Add more consumable types as needed

            return false;
        }

        private void ApplyItemStats(Item item, bool apply)
        {
            if (_currentPlayer == null || item == null)
                return;

            var multiplier = apply ? 1 : -1;

            // Apply stat bonuses from equipment
            if (item.Type == ItemType.Weapon)
            {
                _currentPlayer.Attack += item.Value * multiplier;
            }
            else if (item.Type == ItemType.Armor)
            {
                _currentPlayer.Defense += item.Value * multiplier;
            }

            // Ensure stats don't go below 0
            _currentPlayer.Attack = Math.Max(0, _currentPlayer.Attack);
            _currentPlayer.Defense = Math.Max(0, _currentPlayer.Defense);
        }

        private bool IsStackable(Item item)
        {
            // Most consumables are stackable
            return item.Type == ItemType.Consumable;
        }

        private void UpdateItemQuantities()
        {
            _itemQuantities.Clear();
            
            if (_currentPlayer?.Inventory != null)
            {
                foreach (var item in _currentPlayer.Inventory)
                {
                    _itemQuantities[item.Name] = _itemQuantities.GetValueOrDefault(item.Name, 0) + 1;
                }
            }
        }

        private void OnGameStarted(GameStartedEvent e)
        {
            SetCurrentPlayer(e.Player);
        }
    }
} 