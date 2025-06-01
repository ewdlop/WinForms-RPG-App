using System;
using System.Collections.Generic;
using WinFormsApp1.Constants;

namespace WinFormsApp1
{
    public class Player
    {
        public string Name { get; set; }
        public CharacterClass CharacterClass { get; set; }
        public int Level { get; set; } = 1;
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Experience { get; set; } = 0;
        public int ExperienceToNextLevel { get; set; } = 100;
        public int Gold { get; set; } = 50;
        public List<Item> Inventory { get; set; } = new List<Item>();
        public Item? EquippedWeapon { get; set; }
        public Item? EquippedArmor { get; set; }
        public int SkillPoints { get; set; } = 10; // Starting skill points
        public List<string> LearnedSkills { get; set; } = new List<string>();

        public Player()
        {
        }

        public Player(string name, CharacterClass characterClass) : this()
        {
            Name = name;
            CharacterClass = characterClass;
            
            // Set base stats based on class
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    MaxHealth = 100;
                    Attack = 15;
                    Defense = 8;
                    break;
                case CharacterClass.Mage:
                    MaxHealth = 70;
                    Attack = 20;
                    Defense = 5;
                    break;
                case CharacterClass.Rogue:
                    MaxHealth = 80;
                    Attack = 18;
                    Defense = 6;
                    break;
                case CharacterClass.Cleric:
                    MaxHealth = 90;
                    Attack = 12;
                    Defense = 10;
                    break;
            }
            
            Health = MaxHealth;

            // Starting items based on class
            switch (characterClass)
            {
                case CharacterClass.Warrior:
                    Inventory.Add(new Item(GameConstants.IRON_SWORD, "A sturdy iron blade", ItemType.Weapon, 10));
                    Inventory.Add(new Item(GameConstants.LEATHER_ARMOR, "Basic protection", ItemType.Armor, 5));
                    break;
                case CharacterClass.Mage:
                    Inventory.Add(new Item(GameConstants.MAGIC_STAFF, "A staff imbued with magical energy", ItemType.Weapon, 8));
                    Inventory.Add(new Item(GameConstants.MANA_POTION, "Restores magical energy", ItemType.Potion, 15));
                    break;
                case CharacterClass.Rogue:
                    Inventory.Add(new Item(GameConstants.STEEL_DAGGER, "A sharp, lightweight blade", ItemType.Weapon, 7));
                    Inventory.Add(new Item(GameConstants.LOCKPICKS, "Tools for opening locked doors", ItemType.Misc, 0));
                    break;
                case CharacterClass.Cleric:
                    Inventory.Add(new Item(GameConstants.HOLY_MACE, "A blessed weapon", ItemType.Weapon, 9));
                    Inventory.Add(new Item(GameConstants.HEALING_POTION, "Restores 30 health", ItemType.Potion, 30));
                    break;
            }

            // Everyone starts with a basic health potion
            Inventory.Add(new Item(GameConstants.HEALTH_POTION, "Restores 20 health", ItemType.Potion, 20));
        }

        public int GetTotalAttack()
        {
            int totalAttack = Attack;
            if (EquippedWeapon != null)
                totalAttack += EquippedWeapon.Value;
            return totalAttack;
        }

        public int GetTotalDefense()
        {
            int totalDefense = Defense;
            if (EquippedArmor != null)
                totalDefense += EquippedArmor.Value;
            return totalDefense;
        }
    }

    public class Enemy
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int Experience { get; set; } // Experience reward when defeated
        public int Gold { get; set; } // Gold reward when defeated
        public List<Item> LootTable { get; set; }

        public Enemy()
        {
            LootTable = new List<Item>();
        }

        public Enemy(string name, int level, int health, int attack, int defense) : this()
        {
            Name = name;
            Level = level;
            Health = health;
            MaxHealth = health;
            Attack = attack;
            Defense = defense;
            // Set default rewards based on level
            Experience = level * 15;
            Gold = level * 10;
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemType Type { get; set; }
        public int Value { get; set; }
        public int Price { get; set; }
        public bool IsStackable { get; set; }

        public Item()
        {
        }

        public Item(string name, string description, ItemType type, int value, int price = 0, bool isStackable = false)
        {
            Name = name;
            Description = description;
            Type = type;
            Value = value;
            Price = price;
            IsStackable = isStackable;
        }
    }

    public class Location
    {
        public string Key { get; set; } // Location identifier/key
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Exits { get; set; }
        public List<Item> Items { get; set; }
        public List<Enemy> Enemies { get; set; }
        public List<string> NPCs { get; set; }
        public bool IsVisited { get; set; }

        public Location()
        {
            Exits = new Dictionary<string, string>();
            Items = new List<Item>();
            Enemies = new List<Enemy>();
            NPCs = new List<string>();
        }
    }

    public class Quest
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public QuestStatus Status { get; set; }
        public List<QuestObjective> Objectives { get; set; }
        public List<Item> Rewards { get; set; }
        public int ExperienceReward { get; set; }
        public int GoldReward { get; set; }

        public Quest()
        {
            Objectives = new List<QuestObjective>();
            Rewards = new List<Item>();
            Status = QuestStatus.NotStarted;
        }
    }

    public class QuestObjective
    {
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public string TargetType { get; set; } // "kill", "collect", "visit", etc.
        public string Target { get; set; }
        public int RequiredAmount { get; set; }
        public int CurrentAmount { get; set; }
    }

    public class GameSave
    {
        public Player Player { get; set; }
        public string CurrentLocationKey { get; set; }
        public Dictionary<string, Location> Locations { get; set; }
        public List<Quest> ActiveQuests { get; set; }
        public DateTime SaveDate { get; set; }
        public string SaveVersion { get; set; }

        public GameSave()
        {
            ActiveQuests = new List<Quest>();
            SaveVersion = "1.0";
        }
    }

    public class NPC
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Dialogue { get; set; }
        public List<Item> Shop { get; set; }
        public List<Quest> Quests { get; set; }
        public NPCType Type { get; set; }

        public NPC()
        {
            Dialogue = new List<string>();
            Shop = new List<Item>();
            Quests = new List<Quest>();
        }
    }

    // Enums
    public enum CharacterClass
    {
        Warrior,
        Mage,
        Rogue,
        Cleric
    }

    public enum ItemType
    {
        Weapon,
        Armor,
        Potion,
        Consumable,
        Misc,
        Key,
        Quest
    }

    public enum QuestStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Failed
    }

    public enum NPCType
    {
        Merchant,
        QuestGiver,
        Guard,
        Villager,
        Trainer
    }

    public enum DamageType
    {
        Physical,
        Magical,
        Fire,
        Ice,
        Lightning,
        Poison
    }

    public enum EquipmentSlot
    {
        None,
        Weapon,
        Armor,
        Shield,
        Helmet,
        Boots,
        Ring,
        Amulet
    }
} 