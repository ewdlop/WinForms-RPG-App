using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WinFormsApp1.Constants;

namespace WinFormsApp1.Events
{
    public class GameEventManager : IGameEventPublisher
    {
        private static GameEventManager? _instance;
        private static readonly object _lock = new object();

        // Singleton pattern for global event management
        public static GameEventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new GameEventManager();
                    }
                }
                return _instance;
            }
        }

        private GameEventManager() { }

        // Player Events
        public event EventHandler<LevelUpEventArgs>? PlayerLeveledUp;
        public event EventHandler<ExperienceGainEventArgs>? PlayerGainedExperience;
        public event EventHandler<GoldChangeEventArgs>? PlayerGoldChanged;
        public event EventHandler<HealthChangeEventArgs>? PlayerHealthChanged;
        public event EventHandler<SkillEventArgs>? PlayerLearnedSkill;

        // Combat Events
        public event EventHandler<CombatEventArgs>? CombatStarted;
        public event EventHandler<CombatActionEventArgs>? CombatActionPerformed;
        public event EventHandler<CombatEndEventArgs>? CombatEnded;

        // World Events
        public event EventHandler<LocationChangeEventArgs>? PlayerMovedLocation;
        public event EventHandler<InventoryChangeEventArgs>? InventoryChanged;
        public event EventHandler<ItemUsedEventArgs>? ItemUsed;

        // Game State Events
        public event EventHandler<GameStateEventArgs>? GameStateChanged;
        public event EventHandler<SaveGameEventArgs>? GameSaved;
        public event EventHandler<LoadGameEventArgs>? GameLoaded;
        public event EventHandler<CommandEventArgs>? CommandExecuted;
        public event EventHandler<CheatEventArgs>? CheatActivated;
        public event EventHandler<MessageEventArgs>? MessageDisplayed;

        // Event Publishing Methods
        public void PublishPlayerLevelUp(Player player, int oldLevel, int newLevel, 
            int healthGain, int attackGain, int defenseGain, int skillPointsGained)
        {
            var args = new LevelUpEventArgs(player, oldLevel, newLevel, healthGain, attackGain, defenseGain, skillPointsGained);
            PlayerLeveledUp?.Invoke(this, args);
            
            // Also publish as a message
            PublishMessage($"Level Up! {oldLevel} → {newLevel}! (+{healthGain} HP, +{attackGain} ATK, +{defenseGain} DEF, +{skillPointsGained} SP)", 
                Color.Gold, MessageType.System);
        }

        public void PublishExperienceGain(Player player, int experienceGained, bool leveledUp)
        {
            var args = new ExperienceGainEventArgs(player, experienceGained, leveledUp);
            PlayerGainedExperience?.Invoke(this, args);
            
            if (!leveledUp)
            {
                PublishMessage($"Gained {experienceGained} experience!", Color.Cyan, MessageType.System);
            }
        }

        public void PublishGoldChange(Player player, int goldChange, string reason)
        {
            var args = new GoldChangeEventArgs(player, goldChange, reason);
            PlayerGoldChanged?.Invoke(this, args);
            
            string message = goldChange > 0 
                ? $"Gained {goldChange} gold! ({reason})" 
                : $"Lost {Math.Abs(goldChange)} gold. ({reason})";
            Color color = goldChange > 0 ? Color.Gold : Color.Orange;
            PublishMessage(message, color, MessageType.System);
        }

        public void PublishHealthChange(Player player, int healthChange, string reason)
        {
            var args = new HealthChangeEventArgs(player, healthChange, reason);
            PlayerHealthChanged?.Invoke(this, args);
            
            if (healthChange != 0)
            {
                string message = healthChange > 0 
                    ? $"Restored {healthChange} health! ({reason})" 
                    : $"Lost {Math.Abs(healthChange)} health. ({reason})";
                Color color = healthChange > 0 ? Color.Green : Color.Red;
                PublishMessage(message, color, MessageType.Combat);
            }
        }

        public void PublishSkillLearned(Player player, string skillName, int skillPointsCost, string skillDescription)
        {
            var args = new SkillEventArgs(player, skillName, skillPointsCost, skillDescription);
            PlayerLearnedSkill?.Invoke(this, args);
            
            PublishMessage($"Learned new skill: {skillName}! (-{skillPointsCost} SP)", Color.Cyan, MessageType.System);
        }

        public void PublishCombatStart(Player player, Enemy enemy)
        {
            var args = new CombatEventArgs(player, enemy, true);
            CombatStarted?.Invoke(this, args);
            
            PublishMessage($"Combat begins! A {enemy.Name} appears!", Color.Red, MessageType.Combat);
            PublishMessage($"Enemy Health: {enemy.Health}/{enemy.MaxHealth}", Color.Orange, MessageType.Combat);
        }

        public void PublishCombatAction(Player player, Enemy enemy, string action, int damage, bool isPlayerAction, bool isCritical = false)
        {
            var args = new CombatActionEventArgs(player, enemy, action, damage, isPlayerAction, isCritical);
            CombatActionPerformed?.Invoke(this, args);
            
            string actor = isPlayerAction ? "You" : enemy.Name;
            string target = isPlayerAction ? enemy.Name : "you";
            string criticalText = isCritical ? " CRITICAL HIT!" : "";
            
            string message = $"{actor} {action} {target} for {damage} damage!{criticalText}";
            Color color = isPlayerAction ? Color.Yellow : Color.Red;
            PublishMessage(message, color, MessageType.Combat);
        }

        public void PublishCombatEnd(Player player, Enemy enemy, bool playerWon, int experienceGained = 0, int goldGained = 0, List<Item>? lootGained = null)
        {
            var args = new CombatEndEventArgs(player, enemy, playerWon, experienceGained, goldGained, lootGained);
            CombatEnded?.Invoke(this, args);
            
            if (playerWon)
            {
                PublishMessage($"Victory! You defeated {enemy.Name}!", Color.Green, MessageType.Combat);
                if (experienceGained > 0 || goldGained > 0)
                {
                    PublishMessage($"Rewards: {experienceGained} XP, {goldGained} gold", Color.Yellow, MessageType.System);
                }
                
                if (lootGained?.Any() == true)
                {
                    foreach (var item in lootGained)
                    {
                        PublishMessage($"Found: {item.Name}!", Color.Magenta, MessageType.System);
                    }
                }
            }
            else
            {
                PublishMessage("Defeat! You have been overcome...", Color.Red, MessageType.Combat);
            }
        }

        public void PublishLocationChange(Player player, Location? oldLocation, Location newLocation, string movementDirection)
        {
            var args = new LocationChangeEventArgs(player, oldLocation, newLocation, movementDirection);
            PlayerMovedLocation?.Invoke(this, args);
            
            PublishMessage($"You move {movementDirection} to {newLocation.Name}.", Color.Green, MessageType.System);
        }

        public void PublishInventoryChange(Player player, Item item, bool itemAdded, int quantity = 1, Location? location = null)
        {
            var args = new InventoryChangeEventArgs(player, item, itemAdded, quantity, location);
            InventoryChanged?.Invoke(this, args);
            
            string message = itemAdded 
                ? $"Added {item.Name} to inventory!" 
                : $"Removed {item.Name} from inventory.";
            Color color = itemAdded ? Color.Green : Color.Orange;
            PublishMessage(message, color, MessageType.System);
        }

        public void PublishItemUsed(Player player, Item item, string effect, bool wasConsumed)
        {
            var args = new ItemUsedEventArgs(player, item, effect, wasConsumed);
            ItemUsed?.Invoke(this, args);
            
            string consumedText = wasConsumed ? " (consumed)" : "";
            PublishMessage($"Used {item.Name}: {effect}{consumedText}", Color.Cyan, MessageType.System);
        }

        public void PublishGameStateChange(string stateName, object? stateData = null)
        {
            var args = new GameStateEventArgs(stateName, stateData);
            GameStateChanged?.Invoke(this, args);
        }

        public void PublishGameSaved(string saveName, string savePath, bool wasSuccessful, string? errorMessage = null)
        {
            var args = new SaveGameEventArgs(saveName, savePath, wasSuccessful, errorMessage);
            GameSaved?.Invoke(this, args);
            
            if (wasSuccessful)
            {
                PublishMessage($"Game saved as '{saveName}'.", Color.Green, MessageType.System);
            }
            else
            {
                PublishMessage($"Failed to save game: {errorMessage}", Color.Red, MessageType.Error);
            }
        }

        public void PublishGameLoaded(string saveName, string savePath, bool wasSuccessful, Player? loadedPlayer = null, string? errorMessage = null)
        {
            var args = new LoadGameEventArgs(saveName, savePath, wasSuccessful, loadedPlayer, errorMessage);
            GameLoaded?.Invoke(this, args);
            
            if (wasSuccessful && loadedPlayer != null)
            {
                PublishMessage($"Game loaded successfully! Welcome back, {loadedPlayer.Name}.", Color.Green, MessageType.System);
            }
            else
            {
                PublishMessage($"Failed to load game: {errorMessage}", Color.Red, MessageType.Error);
            }
        }

        public void PublishCommandExecuted(string command, string[] arguments, Player? player = null)
        {
            var args = new CommandEventArgs(command, arguments, player);
            CommandExecuted?.Invoke(this, args);
        }

        public void PublishCheatActivated(string cheatCommand, string[] arguments, Player? player = null, bool wasSuccessful = true)
        {
            var args = new CheatEventArgs(cheatCommand, arguments, player, wasSuccessful);
            CheatActivated?.Invoke(this, args);
            
            if (wasSuccessful)
            {
                PublishMessage($"Cheat activated: {cheatCommand}", Color.Magenta, MessageType.Cheat);
            }
            else
            {
                PublishMessage($"Cheat failed: {cheatCommand}", Color.Red, MessageType.Error);
            }
        }

        public void PublishMessage(string message, Color? color = null, MessageType type = MessageType.Normal)
        {
            var args = new MessageEventArgs(message, color, type);
            MessageDisplayed?.Invoke(this, args);
        }

        // Event subscription helpers
        public void SubscribeToPlayerEvents(IPlayerEventHandler handler)
        {
            PlayerLeveledUp += handler.OnPlayerLeveledUp;
            PlayerGainedExperience += handler.OnPlayerGainedExperience;
            PlayerGoldChanged += handler.OnPlayerGoldChanged;
            PlayerHealthChanged += handler.OnPlayerHealthChanged;
            PlayerLearnedSkill += handler.OnPlayerLearnedSkill;
        }

        public void SubscribeToCombatEvents(ICombatEventHandler handler)
        {
            CombatStarted += handler.OnCombatStarted;
            CombatActionPerformed += handler.OnCombatActionPerformed;
            CombatEnded += handler.OnCombatEnded;
        }

        public void SubscribeToWorldEvents(IWorldEventHandler handler)
        {
            PlayerMovedLocation += handler.OnPlayerMovedLocation;
            InventoryChanged += handler.OnInventoryChanged;
            ItemUsed += handler.OnItemUsed;
        }

        public void SubscribeToGameStateEvents(IGameStateEventHandler handler)
        {
            GameStateChanged += handler.OnGameStateChanged;
            GameSaved += handler.OnGameSaved;
            GameLoaded += handler.OnGameLoaded;
            CommandExecuted += handler.OnCommandExecuted;
            CheatActivated += handler.OnCheatActivated;
        }

        public void SubscribeToMessageEvents(IMessageEventHandler handler)
        {
            MessageDisplayed += handler.OnMessageDisplayed;
        }

        // Unsubscription helpers
        public void UnsubscribeFromPlayerEvents(IPlayerEventHandler handler)
        {
            PlayerLeveledUp -= handler.OnPlayerLeveledUp;
            PlayerGainedExperience -= handler.OnPlayerGainedExperience;
            PlayerGoldChanged -= handler.OnPlayerGoldChanged;
            PlayerHealthChanged -= handler.OnPlayerHealthChanged;
            PlayerLearnedSkill -= handler.OnPlayerLearnedSkill;
        }

        public void UnsubscribeFromCombatEvents(ICombatEventHandler handler)
        {
            CombatStarted -= handler.OnCombatStarted;
            CombatActionPerformed -= handler.OnCombatActionPerformed;
            CombatEnded -= handler.OnCombatEnded;
        }

        public void UnsubscribeFromWorldEvents(IWorldEventHandler handler)
        {
            PlayerMovedLocation -= handler.OnPlayerMovedLocation;
            InventoryChanged -= handler.OnInventoryChanged;
            ItemUsed -= handler.OnItemUsed;
        }

        public void UnsubscribeFromGameStateEvents(IGameStateEventHandler handler)
        {
            GameStateChanged -= handler.OnGameStateChanged;
            GameSaved -= handler.OnGameSaved;
            GameLoaded -= handler.OnGameLoaded;
            CommandExecuted -= handler.OnCommandExecuted;
            CheatActivated -= handler.OnCheatActivated;
        }

        public void UnsubscribeFromMessageEvents(IMessageEventHandler handler)
        {
            MessageDisplayed -= handler.OnMessageDisplayed;
        }

        // Clear all event subscriptions (useful for cleanup)
        public void ClearAllSubscriptions()
        {
            PlayerLeveledUp = null;
            PlayerGainedExperience = null;
            PlayerGoldChanged = null;
            PlayerHealthChanged = null;
            PlayerLearnedSkill = null;
            CombatStarted = null;
            CombatActionPerformed = null;
            CombatEnded = null;
            PlayerMovedLocation = null;
            InventoryChanged = null;
            ItemUsed = null;
            GameStateChanged = null;
            GameSaved = null;
            GameLoaded = null;
            CommandExecuted = null;
            CheatActivated = null;
            MessageDisplayed = null;
        }

        // Event statistics and debugging
        public Dictionary<string, int> GetEventSubscriptionCounts()
        {
            return new Dictionary<string, int>
            {
                ["PlayerLeveledUp"] = PlayerLeveledUp?.GetInvocationList().Length ?? 0,
                ["PlayerGainedExperience"] = PlayerGainedExperience?.GetInvocationList().Length ?? 0,
                ["PlayerGoldChanged"] = PlayerGoldChanged?.GetInvocationList().Length ?? 0,
                ["PlayerHealthChanged"] = PlayerHealthChanged?.GetInvocationList().Length ?? 0,
                ["PlayerLearnedSkill"] = PlayerLearnedSkill?.GetInvocationList().Length ?? 0,
                ["CombatStarted"] = CombatStarted?.GetInvocationList().Length ?? 0,
                ["CombatActionPerformed"] = CombatActionPerformed?.GetInvocationList().Length ?? 0,
                ["CombatEnded"] = CombatEnded?.GetInvocationList().Length ?? 0,
                ["PlayerMovedLocation"] = PlayerMovedLocation?.GetInvocationList().Length ?? 0,
                ["InventoryChanged"] = InventoryChanged?.GetInvocationList().Length ?? 0,
                ["ItemUsed"] = ItemUsed?.GetInvocationList().Length ?? 0,
                ["GameStateChanged"] = GameStateChanged?.GetInvocationList().Length ?? 0,
                ["GameSaved"] = GameSaved?.GetInvocationList().Length ?? 0,
                ["GameLoaded"] = GameLoaded?.GetInvocationList().Length ?? 0,
                ["CommandExecuted"] = CommandExecuted?.GetInvocationList().Length ?? 0,
                ["CheatActivated"] = CheatActivated?.GetInvocationList().Length ?? 0,
                ["MessageDisplayed"] = MessageDisplayed?.GetInvocationList().Length ?? 0
            };
        }
    }
} 