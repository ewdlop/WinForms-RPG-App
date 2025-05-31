using System;
using System.Collections.Generic;

namespace WinFormsApp1
{
    // JSON Data Models for loading from files
    public class CharacterClassData
    {
        public List<CharacterClassInfo> CharacterClasses { get; set; } = new();
    }

    public class CharacterClassInfo
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public BaseStats BaseStats { get; set; } = new();
        public List<string> StartingItems { get; set; } = new();
    }

    public class BaseStats
    {
        public int MaxHealth { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
    }

    public class ItemData
    {
        public List<ItemInfo> Items { get; set; } = new();
    }

    public class ItemInfo
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Type { get; set; } = "";
        public int Value { get; set; }
        public int Price { get; set; }
        public bool IsStackable { get; set; }
    }

    public class EnemyData
    {
        public List<EnemyInfo> Enemies { get; set; } = new();
    }

    public class EnemyInfo
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int Level { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int ExperienceReward { get; set; }
        public GoldReward GoldReward { get; set; } = new();
        public List<LootItem> LootTable { get; set; } = new();
    }

    public class GoldReward
    {
        public int Min { get; set; }
        public int Max { get; set; }
    }

    public class LootItem
    {
        public string ItemId { get; set; } = "";
        public double DropChance { get; set; }
    }

    public class LocationData
    {
        public List<LocationInfo> Locations { get; set; } = new();
    }

    public class LocationInfo
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public List<ExitInfo> Exits { get; set; } = new();
        public List<string> Items { get; set; } = new();
        public List<string> Enemies { get; set; } = new();
        public List<string> NPCs { get; set; } = new();
    }

    public class ExitInfo
    {
        public string Direction { get; set; } = "";
        public string LocationId { get; set; } = "";
        public string Description { get; set; } = "";
    }
} 