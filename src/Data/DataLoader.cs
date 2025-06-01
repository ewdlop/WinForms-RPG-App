using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WinFormsApp1.Constants;

namespace WinFormsApp1
{
    public class DataLoader
    {
        private readonly string dataDirectory;

        public DataLoader(string dataDirectory = "Data")
        {
            this.dataDirectory = dataDirectory;
        }

        public void LoadAllData()
        {
            // Ensure data directory exists
            if (!Directory.Exists(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
                CreateDefaultDataFiles();
            }
        }

        public Dictionary<string, Location> LoadLocations()
        {
            var locations = new Dictionary<string, Location>();

            try
            {
                string filePath = Path.Combine(dataDirectory, "Locations.json");
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    var locationList = JsonSerializer.Deserialize<List<Location>>(jsonData);
                    
                    if (locationList != null)
                    {
                        foreach (var location in locationList)
                        {
                            locations[location.Key] = location;
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Fall back to default locations if loading fails
            }

            // If no locations loaded, create default ones
            if (locations.Count == 0)
            {
                locations = CreateDefaultLocations();
            }

            return locations;
        }

        public List<Enemy> LoadEnemies()
        {
            try
            {
                string filePath = Path.Combine(dataDirectory, "Enemies.json");
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    var enemies = JsonSerializer.Deserialize<List<Enemy>>(jsonData);
                    return enemies ?? CreateDefaultEnemies();
                }
            }
            catch (Exception)
            {
                // Fall back to default enemies if loading fails
            }

            return CreateDefaultEnemies();
        }

        public List<Item> LoadItems()
        {
            try
            {
                string filePath = Path.Combine(dataDirectory, "Items.json");
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    var items = JsonSerializer.Deserialize<List<Item>>(jsonData);
                    return items ?? CreateDefaultItems();
                }
            }
            catch (Exception)
            {
                // Fall back to default items if loading fails
            }

            return CreateDefaultItems();
        }

        public List<CharacterClassInfo> LoadCharacterClasses()
        {
            try
            {
                string filePath = Path.Combine(dataDirectory, "CharacterClasses.json");
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    var classes = JsonSerializer.Deserialize<List<CharacterClassInfo>>(jsonData);
                    return classes ?? CreateDefaultCharacterClasses();
                }
            }
            catch (Exception)
            {
                // Fall back to default classes if loading fails
            }

            return CreateDefaultCharacterClasses();
        }

        private void CreateDefaultDataFiles()
        {
            // Create default data files
            CreateDefaultLocationsFile();
            CreateDefaultEnemiesFile();
            CreateDefaultItemsFile();
            CreateDefaultCharacterClassesFile();
        }

        private void CreateDefaultLocationsFile()
        {
            var locations = CreateDefaultLocationsList();
            string filePath = Path.Combine(dataDirectory, "Locations.json");
            string jsonData = JsonSerializer.Serialize(locations, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }

        private void CreateDefaultEnemiesFile()
        {
            var enemies = CreateDefaultEnemies();
            string filePath = Path.Combine(dataDirectory, "Enemies.json");
            string jsonData = JsonSerializer.Serialize(enemies, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }

        private void CreateDefaultItemsFile()
        {
            var items = CreateDefaultItems();
            string filePath = Path.Combine(dataDirectory, "Items.json");
            string jsonData = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }

        private void CreateDefaultCharacterClassesFile()
        {
            var classes = CreateDefaultCharacterClasses();
            string filePath = Path.Combine(dataDirectory, "CharacterClasses.json");
            string jsonData = JsonSerializer.Serialize(classes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonData);
        }

        private Dictionary<string, Location> CreateDefaultLocations()
        {
            var locations = new Dictionary<string, Location>();
            var locationList = CreateDefaultLocationsList();
            
            foreach (var location in locationList)
            {
                locations[location.Key] = location;
            }
            
            return locations;
        }

        private List<Location> CreateDefaultLocationsList()
        {
            return new List<Location>
            {
                new Location
                {
                    Key = GameConstants.VILLAGE_LOCATION,
                    Name = "Village of Elderbrook",
                    Description = "A peaceful village with cobblestone streets and thatched-roof houses. The village square bustles with merchants and travelers.",
                    Exits = new Dictionary<string, string>
                    {
                        ["north"] = GameConstants.FOREST_LOCATION,
                        ["east"] = GameConstants.PLAINS_LOCATION
                    },
                    Items = new List<Item>
                    {
                        new Item(GameConstants.HEALTH_POTION, "Restores 20 health", ItemType.Potion, 20, 15)
                    },
                    Enemies = new List<Enemy>()
                },
                new Location
                {
                    Key = GameConstants.FOREST_LOCATION,
                    Name = "Whispering Woods",
                    Description = "A dense forest with towering trees that block out most sunlight. Strange sounds echo from the depths.",
                    Exits = new Dictionary<string, string>
                    {
                        ["south"] = GameConstants.VILLAGE_LOCATION,
                        ["west"] = GameConstants.CAVE_LOCATION
                    },
                    Items = new List<Item>
                    {
                        new Item("Wooden Staff", "A simple wooden staff", ItemType.Weapon, 5, 10)
                    },
                    Enemies = new List<Enemy>
                    {
                        new Enemy { Name = "Forest Goblin", Level = 1, Health = 25, MaxHealth = 25, Attack = 6, Defense = 1, Experience = 15, Gold = 8 }
                    }
                },
                new Location
                {
                    Key = GameConstants.PLAINS_LOCATION,
                    Name = "Golden Plains",
                    Description = "Vast grasslands stretch as far as the eye can see. The wind carries the scent of wildflowers.",
                    Exits = new Dictionary<string, string>
                    {
                        ["west"] = GameConstants.VILLAGE_LOCATION,
                        ["north"] = GameConstants.RUINS_LOCATION
                    },
                    Items = new List<Item>(),
                    Enemies = new List<Enemy>
                    {
                        new Enemy { Name = "Wild Wolf", Level = 2, Health = 35, MaxHealth = 35, Attack = 8, Defense = 2, Experience = 20, Gold = 5 }
                    }
                },
                new Location
                {
                    Key = GameConstants.CAVE_LOCATION,
                    Name = "Crystal Caverns",
                    Description = "A mysterious cave system filled with glowing crystals. The air hums with magical energy.",
                    Exits = new Dictionary<string, string>
                    {
                        ["east"] = GameConstants.FOREST_LOCATION,
                        ["north"] = GameConstants.LAIR_LOCATION
                    },
                    Items = new List<Item>
                    {
                        new Item("Crystal Shard", "A glowing crystal fragment", ItemType.Misc, 0, 25)
                    },
                    Enemies = new List<Enemy>
                    {
                        new Enemy { Name = "Cave Troll", Level = 3, Health = 60, MaxHealth = 60, Attack = 12, Defense = 4, Experience = 35, Gold = 20 }
                    }
                },
                new Location
                {
                    Key = GameConstants.RUINS_LOCATION,
                    Name = "Ancient Ruins",
                    Description = "The crumbling remains of an ancient civilization. Mysterious runes cover the weathered stones.",
                    Exits = new Dictionary<string, string>
                    {
                        ["south"] = GameConstants.PLAINS_LOCATION
                    },
                    Items = new List<Item>
                    {
                        new Item("Ancient Scroll", "A scroll with mysterious writing", ItemType.Misc, 0, 50)
                    },
                    Enemies = new List<Enemy>
                    {
                        new Enemy { Name = "Skeleton Warrior", Level = 4, Health = 45, MaxHealth = 45, Attack = 10, Defense = 3, Experience = 30, Gold = 15 }
                    }
                },
                new Location
                {
                    Key = GameConstants.LAIR_LOCATION,
                    Name = "Dragon's Lair",
                    Description = "A massive cavern filled with treasure and the overwhelming presence of an ancient dragon.",
                    Exits = new Dictionary<string, string>
                    {
                        ["south"] = GameConstants.CAVE_LOCATION
                    },
                    Items = new List<Item>
                    {
                        new Item("Dragon Scale", "A shimmering dragon scale", ItemType.Misc, 0, 100)
                    },
                    Enemies = new List<Enemy>
                    {
                        new Enemy { Name = "Ancient Dragon", Level = 10, Health = 200, MaxHealth = 200, Attack = 25, Defense = 8, Experience = 150, Gold = 500 }
                    }
                }
            };
        }

        private List<Enemy> CreateDefaultEnemies()
        {
            return new List<Enemy>
            {
                new Enemy { Name = "Goblin", Level = 1, Health = 25, MaxHealth = 25, Attack = 6, Defense = 1, Experience = 15, Gold = 8 },
                new Enemy { Name = "Orc", Level = 2, Health = 40, MaxHealth = 40, Attack = 10, Defense = 3, Experience = 25, Gold = 15 },
                new Enemy { Name = "Wolf", Level = 2, Health = 35, MaxHealth = 35, Attack = 8, Defense = 2, Experience = 20, Gold = 5 },
                new Enemy { Name = "Bandit", Level = 3, Health = 50, MaxHealth = 50, Attack = 12, Defense = 3, Experience = 30, Gold = 25 },
                new Enemy { Name = "Troll", Level = 4, Health = 80, MaxHealth = 80, Attack = 15, Defense = 5, Experience = 50, Gold = 40 },
                new Enemy { Name = "Dragon", Level = 10, Health = 200, MaxHealth = 200, Attack = 25, Defense = 8, Experience = 150, Gold = 500 }
            };
        }

        private List<Item> CreateDefaultItems()
        {
            return new List<Item>
            {
                // Potions
                new Item(GameConstants.HEALTH_POTION, "Restores 20 health", ItemType.Potion, 20, 15),
                new Item(GameConstants.MANA_POTION, "Restores 30 mana", ItemType.Potion, 30, 20),
                new Item(GameConstants.HEALING_POTION, "Restores 50 health", ItemType.Potion, 50, 35),
                
                // Weapons
                new Item(GameConstants.IRON_SWORD, "A sturdy iron blade", ItemType.Weapon, 10, 50),
                new Item(GameConstants.STEEL_DAGGER, "A sharp, lightweight blade", ItemType.Weapon, 7, 30),
                new Item(GameConstants.MAGIC_STAFF, "A staff imbued with magical energy", ItemType.Weapon, 8, 40),
                new Item(GameConstants.HOLY_MACE, "A blessed weapon", ItemType.Weapon, 9, 45),
                
                // Armor
                new Item(GameConstants.LEATHER_ARMOR, "Basic protection", ItemType.Armor, 5, 25),
                
                // Misc
                new Item(GameConstants.LOCKPICKS, "Tools for opening locked doors", ItemType.Misc, 0, 10)
            };
        }

        private List<CharacterClassInfo> CreateDefaultCharacterClasses()
        {
            return new List<CharacterClassInfo>
            {
                new CharacterClassInfo
                {
                    Class = CharacterClass.Warrior,
                    Name = "Warrior",
                    Description = "A strong melee fighter with high health and defense.",
                    StartingHealth = 120,
                    StartingAttack = 15,
                    StartingDefense = 8,
                    StartingItems = new List<Item>
                    {
                        new Item(GameConstants.IRON_SWORD, "A sturdy iron blade", ItemType.Weapon, 10),
                        new Item(GameConstants.LEATHER_ARMOR, "Basic protection", ItemType.Armor, 5)
                    }
                },
                new CharacterClassInfo
                {
                    Class = CharacterClass.Mage,
                    Name = "Mage",
                    Description = "A spellcaster with powerful magic but low physical defense.",
                    StartingHealth = 80,
                    StartingAttack = 20,
                    StartingDefense = 3,
                    StartingItems = new List<Item>
                    {
                        new Item(GameConstants.MAGIC_STAFF, "A staff imbued with magical energy", ItemType.Weapon, 8),
                        new Item(GameConstants.MANA_POTION, "Restores magical energy", ItemType.Potion, 30)
                    }
                },
                new CharacterClassInfo
                {
                    Class = CharacterClass.Rogue,
                    Name = "Rogue",
                    Description = "A nimble fighter with balanced stats and special abilities.",
                    StartingHealth = 100,
                    StartingAttack = 12,
                    StartingDefense = 6,
                    StartingItems = new List<Item>
                    {
                        new Item(GameConstants.STEEL_DAGGER, "A sharp, lightweight blade", ItemType.Weapon, 7),
                        new Item(GameConstants.LOCKPICKS, "Tools for opening locked doors", ItemType.Misc, 0)
                    }
                },
                new CharacterClassInfo
                {
                    Class = CharacterClass.Archer,
                    Name = "Archer",
                    Description = "A ranged fighter with high accuracy and moderate defense.",
                    StartingHealth = 90,
                    StartingAttack = 14,
                    StartingDefense = 5,
                    StartingItems = new List<Item>
                    {
                        new Item("Hunting Bow", "A well-crafted bow", ItemType.Weapon, 9),
                        new Item("Arrows", "A quiver of arrows", ItemType.Misc, 0)
                    }
                }
            };
        }
    }
} 