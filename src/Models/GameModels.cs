using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WinFormsApp1.Constants;

namespace WinFormsApp1
{
    public enum ItemType
    {
        Weapon,
        Armor,
        Potion,
        Misc,
        Key
    }

    public enum CharacterClass
    {
        Warrior,
        Mage,
        Rogue,
        Archer
    }

    public class Player
    {
        public string Name { get; set; } = "";
        public CharacterClass CharacterClass { get; set; }
        public int Level { get; set; } = 1;
        public int Health { get; set; } = 100;
        public int MaxHealth { get; set; } = 100;
        public int Attack { get; set; } = 10;
        public int Defense { get; set; } = 5;
        public int Experience { get; set; } = 0;
        public int ExperienceToNextLevel { get; set; } = 100;
        public int Gold { get; set; } = 50;
        public int SkillPoints { get; set; } = 0;
        public List<Item> Inventory { get; set; } = new List<Item>();
        public Dictionary<string, int> Skills { get; set; } = new Dictionary<string, int>();
        public Item? EquippedWeapon { get; set; }
        public Item? EquippedArmor { get; set; }
        public List<string> LearnedSkills { get; set; } = new List<string>();

        public Player()
        {
            // Initialize with starting items based on class
            InitializeStartingItems();
        }

        private void InitializeStartingItems()
        {
            // Add a basic health potion to start
            Inventory.Add(new Item("Health Potion", "Restores 20 health", ItemType.Potion, 20));
        }

        public void AddSkillPoint(string skillName)
        {
            if (Skills.ContainsKey(skillName))
            {
                Skills[skillName]++;
            }
            else
            {
                Skills[skillName] = 1;
            }
        }

        public int GetSkillLevel(string skillName)
        {
            return Skills.ContainsKey(skillName) ? Skills[skillName] : 0;
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
        public string Name { get; set; } = "";
        public int Level { get; set; } = 1;
        public int Health { get; set; } = 50;
        public int MaxHealth { get; set; } = 50;
        public int Attack { get; set; } = 8;
        public int Defense { get; set; } = 2;
        public int Experience { get; set; } = 25;
        public int Gold { get; set; } = 10;
        public List<Item> LootTable { get; set; } = new List<Item>();
        public string Description { get; set; } = "";
    }

    public class Item
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public ItemType Type { get; set; }
        public int Value { get; set; } = 0;
        public int Price { get; set; } = 0;
        public bool IsStackable { get; set; } = false;
        public int Quantity { get; set; } = 1;

        public Item() { }

        public Item(string name, string description, ItemType type, int value = 0, int price = 0)
        {
            Name = name;
            Description = description;
            Type = type;
            Value = value;
            Price = price;
        }
    }

    public class Location
    {
        public string Key { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public Dictionary<string, string> Exits { get; set; } = new Dictionary<string, string>();
        public List<Item> Items { get; set; } = new List<Item>();
        public List<Enemy> Enemies { get; set; } = new List<Enemy>();
        public List<string> NPCs { get; set; } = new List<string>();
        public bool IsVisited { get; set; } = false;
    }

    public class Skill
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int MaxLevel { get; set; } = 5;
        public int Cost { get; set; } = 1;
        public List<string> Prerequisites { get; set; } = new List<string>();
        public SkillType Type { get; set; }
    }

    public enum SkillType
    {
        Combat,
        Magic,
        Utility,
        Passive
    }

    public class GameSave
    {
        public Player Player { get; set; } = new Player();
        public string CurrentLocationKey { get; set; } = "";
        public Dictionary<string, Location> Locations { get; set; } = new Dictionary<string, Location>();
        public List<Quest> ActiveQuests { get; set; } = new List<Quest>();
        public DateTime SaveDate { get; set; } = DateTime.Now;
        public string SaveVersion { get; set; } = "1.0";
    }

    public class CharacterClassInfo
    {
        public CharacterClass Class { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int StartingHealth { get; set; } = 100;
        public int StartingAttack { get; set; } = 10;
        public int StartingDefense { get; set; } = 5;
        public List<Item> StartingItems { get; set; } = new List<Item>();
        public List<string> StartingSkills { get; set; } = new List<string>();
    }

    public class Quest
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsCompleted { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public List<QuestObjective> Objectives { get; set; } = new List<QuestObjective>();
        public QuestReward Reward { get; set; } = new QuestReward();
    }

    public class QuestObjective
    {
        public string Description { get; set; } = "";
        public bool IsCompleted { get; set; } = false;
        public string Type { get; set; } = ""; // kill, collect, visit, etc.
        public string Target { get; set; } = "";
        public int RequiredAmount { get; set; } = 1;
        public int CurrentAmount { get; set; } = 0;
    }

    public class QuestReward
    {
        public int Experience { get; set; } = 0;
        public int Gold { get; set; } = 0;
        public List<Item> Items { get; set; } = new List<Item>();
    }

    public class GameStats
    {
        public int TotalPlayTime { get; set; } = 0; // in seconds
        public int EnemiesDefeated { get; set; } = 0;
        public int ItemsCollected { get; set; } = 0;
        public int LocationsVisited { get; set; } = 0;
        public int QuestsCompleted { get; set; } = 0;
        public int TimesLeveledUp { get; set; } = 0;
        public int GoldEarned { get; set; } = 0;
        public int DamageTaken { get; set; } = 0;
        public int DamageDealt { get; set; } = 0;
        public DateTime FirstPlayed { get; set; } = DateTime.Now;
        public DateTime LastPlayed { get; set; } = DateTime.Now;
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
        Weapon,
        Armor,
        Shield,
        Helmet,
        Boots,
        Ring,
        Amulet
    }
} 