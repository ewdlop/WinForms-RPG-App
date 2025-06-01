using System;

namespace WinFormsApp1.Events
{
    // Custom Event Args Classes
    public class PlayerEventArgs : EventArgs
    {
        public Player Player { get; }
        public PlayerEventArgs(Player player) => Player = player;
    }

    public class LevelUpEventArgs : PlayerEventArgs
    {
        public int OldLevel { get; }
        public int NewLevel { get; }
        public int HealthGain { get; }
        public int AttackGain { get; }
        public int DefenseGain { get; }
        public int SkillPointsGained { get; }

        public LevelUpEventArgs(Player player, int oldLevel, int newLevel, 
            int healthGain, int attackGain, int defenseGain, int skillPointsGained) 
            : base(player)
        {
            OldLevel = oldLevel;
            NewLevel = newLevel;
            HealthGain = healthGain;
            AttackGain = attackGain;
            DefenseGain = defenseGain;
            SkillPointsGained = skillPointsGained;
        }
    }

    public class ExperienceGainEventArgs : PlayerEventArgs
    {
        public int ExperienceGained { get; }
        public int TotalExperience { get; }
        public bool LeveledUp { get; }

        public ExperienceGainEventArgs(Player player, int experienceGained, bool leveledUp) 
            : base(player)
        {
            ExperienceGained = experienceGained;
            TotalExperience = player.Experience;
            LeveledUp = leveledUp;
        }
    }

    public class GoldChangeEventArgs : PlayerEventArgs
    {
        public int GoldChange { get; }
        public int TotalGold { get; }
        public string Reason { get; }

        public GoldChangeEventArgs(Player player, int goldChange, string reason) 
            : base(player)
        {
            GoldChange = goldChange;
            TotalGold = player.Gold;
            Reason = reason;
        }
    }

    public class HealthChangeEventArgs : PlayerEventArgs
    {
        public int HealthChange { get; }
        public int CurrentHealth { get; }
        public int MaxHealth { get; }
        public string Reason { get; }

        public HealthChangeEventArgs(Player player, int healthChange, string reason) 
            : base(player)
        {
            HealthChange = healthChange;
            CurrentHealth = player.Health;
            MaxHealth = player.MaxHealth;
            Reason = reason;
        }
    }

    public class CombatEventArgs : EventArgs
    {
        public Player Player { get; }
        public Enemy Enemy { get; }
        public bool IsPlayerTurn { get; }

        public CombatEventArgs(Player player, Enemy enemy, bool isPlayerTurn)
        {
            Player = player;
            Enemy = enemy;
            IsPlayerTurn = isPlayerTurn;
        }
    }

    public class CombatActionEventArgs : CombatEventArgs
    {
        public string Action { get; }
        public int Damage { get; }
        public bool IsPlayerAction { get; }
        public bool IsCritical { get; }

        public CombatActionEventArgs(Player player, Enemy enemy, string action, 
            int damage, bool isPlayerAction, bool isCritical = false) 
            : base(player, enemy, isPlayerAction)
        {
            Action = action;
            Damage = damage;
            IsPlayerAction = isPlayerAction;
            IsCritical = isCritical;
        }
    }

    public class CombatEndEventArgs : CombatEventArgs
    {
        public bool PlayerWon { get; }
        public int ExperienceGained { get; }
        public int GoldGained { get; }
        public List<Item> LootGained { get; }

        public CombatEndEventArgs(Player player, Enemy enemy, bool playerWon, 
            int experienceGained = 0, int goldGained = 0, List<Item>? lootGained = null) 
            : base(player, enemy, false)
        {
            PlayerWon = playerWon;
            ExperienceGained = experienceGained;
            GoldGained = goldGained;
            LootGained = lootGained ?? new List<Item>();
        }
    }

    public class LocationChangeEventArgs : EventArgs
    {
        public Location? OldLocation { get; }
        public Location NewLocation { get; }
        public Player Player { get; }
        public string MovementDirection { get; }

        public LocationChangeEventArgs(Player player, Location? oldLocation, 
            Location newLocation, string movementDirection)
        {
            Player = player;
            OldLocation = oldLocation;
            NewLocation = newLocation;
            MovementDirection = movementDirection;
        }
    }

    public class ItemEventArgs : EventArgs
    {
        public Player Player { get; }
        public Item Item { get; }
        public Location? Location { get; }

        public ItemEventArgs(Player player, Item item, Location? location = null)
        {
            Player = player;
            Item = item;
            Location = location;
        }
    }

    public class InventoryChangeEventArgs : ItemEventArgs
    {
        public bool ItemAdded { get; }
        public int Quantity { get; }

        public InventoryChangeEventArgs(Player player, Item item, bool itemAdded, 
            int quantity = 1, Location? location = null) 
            : base(player, item, location)
        {
            ItemAdded = itemAdded;
            Quantity = quantity;
        }
    }

    public class ItemUsedEventArgs : ItemEventArgs
    {
        public string Effect { get; }
        public bool WasConsumed { get; }

        public ItemUsedEventArgs(Player player, Item item, string effect, bool wasConsumed) 
            : base(player, item)
        {
            Effect = effect;
            WasConsumed = wasConsumed;
        }
    }

    public class SkillEventArgs : PlayerEventArgs
    {
        public string SkillName { get; }
        public int SkillPointsCost { get; }
        public string SkillDescription { get; }

        public SkillEventArgs(Player player, string skillName, int skillPointsCost, string skillDescription) 
            : base(player)
        {
            SkillName = skillName;
            SkillPointsCost = skillPointsCost;
            SkillDescription = skillDescription;
        }
    }

    public class CheatEventArgs : EventArgs
    {
        public string CheatCommand { get; }
        public string[] Arguments { get; }
        public Player? Player { get; }
        public bool WasSuccessful { get; }

        public CheatEventArgs(string cheatCommand, string[] arguments, Player? player = null, bool wasSuccessful = true)
        {
            CheatCommand = cheatCommand;
            Arguments = arguments;
            Player = player;
            WasSuccessful = wasSuccessful;
        }
    }

    public class GameStateEventArgs : EventArgs
    {
        public string StateName { get; }
        public object? StateData { get; }
        public DateTime Timestamp { get; }

        public GameStateEventArgs(string stateName, object? stateData = null)
        {
            StateName = stateName;
            StateData = stateData;
            Timestamp = DateTime.Now;
        }
    }

    public class SaveGameEventArgs : EventArgs
    {
        public string SaveName { get; }
        public string SavePath { get; }
        public bool WasSuccessful { get; }
        public string? ErrorMessage { get; }

        public SaveGameEventArgs(string saveName, string savePath, bool wasSuccessful, string? errorMessage = null)
        {
            SaveName = saveName;
            SavePath = savePath;
            WasSuccessful = wasSuccessful;
            ErrorMessage = errorMessage;
        }
    }

    public class LoadGameEventArgs : EventArgs
    {
        public string SaveName { get; }
        public string SavePath { get; }
        public bool WasSuccessful { get; }
        public Player? LoadedPlayer { get; }
        public string? ErrorMessage { get; }

        public LoadGameEventArgs(string saveName, string savePath, bool wasSuccessful, 
            Player? loadedPlayer = null, string? errorMessage = null)
        {
            SaveName = saveName;
            SavePath = savePath;
            WasSuccessful = wasSuccessful;
            LoadedPlayer = loadedPlayer;
            ErrorMessage = errorMessage;
        }
    }

    public class CommandEventArgs : EventArgs
    {
        public string Command { get; }
        public string[] Arguments { get; }
        public Player? Player { get; }
        public bool WasHandled { get; set; }

        public CommandEventArgs(string command, string[] arguments, Player? player = null)
        {
            Command = command;
            Arguments = arguments;
            Player = player;
            WasHandled = false;
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; }
        public Color? Color { get; }
        public MessageType Type { get; }
        public DateTime Timestamp { get; }

        public MessageEventArgs(string message, Color? color = null, MessageType type = MessageType.Normal)
        {
            Message = message;
            Color = color;
            Type = type;
            Timestamp = DateTime.Now;
        }
    }

    public enum MessageType
    {
        Normal,
        System,
        Combat,
        Error,
        Success,
        Warning,
        Cheat,
        Debug
    }

    // Event Publisher Interface
    public interface IGameEventPublisher
    {
        // Player Events
        event EventHandler<LevelUpEventArgs>? PlayerLeveledUp;
        event EventHandler<ExperienceGainEventArgs>? PlayerGainedExperience;
        event EventHandler<GoldChangeEventArgs>? PlayerGoldChanged;
        event EventHandler<HealthChangeEventArgs>? PlayerHealthChanged;
        event EventHandler<SkillEventArgs>? PlayerLearnedSkill;

        // Combat Events
        event EventHandler<CombatEventArgs>? CombatStarted;
        event EventHandler<CombatActionEventArgs>? CombatActionPerformed;
        event EventHandler<CombatEndEventArgs>? CombatEnded;

        // World Events
        event EventHandler<LocationChangeEventArgs>? PlayerMovedLocation;
        event EventHandler<InventoryChangeEventArgs>? InventoryChanged;
        event EventHandler<ItemUsedEventArgs>? ItemUsed;

        // Game State Events
        event EventHandler<GameStateEventArgs>? GameStateChanged;
        event EventHandler<SaveGameEventArgs>? GameSaved;
        event EventHandler<LoadGameEventArgs>? GameLoaded;
        event EventHandler<CommandEventArgs>? CommandExecuted;
        event EventHandler<CheatEventArgs>? CheatActivated;
        event EventHandler<MessageEventArgs>? MessageDisplayed;
    }
} 