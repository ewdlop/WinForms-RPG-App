using System.Text.Json;

namespace WinFormsApp1
{
    public class DataLoader
    {
        private readonly string dataPath;
        private Dictionary<string, ItemInfo> itemDatabase = new();
        private Dictionary<string, EnemyInfo> enemyDatabase = new();
        private Dictionary<string, CharacterClassInfo> classDatabase = new();

        public DataLoader(string dataPath = "Assets/Data")
        {
            this.dataPath = dataPath;
        }

        public void LoadAllData()
        {
            LoadItems();
            LoadEnemies();
            LoadCharacterClasses();
        }

        private void LoadItems()
        {
            try
            {
                string itemsJson = File.ReadAllText(Path.Combine(dataPath, "Items.json"));
                var itemData = JsonSerializer.Deserialize<ItemData>(itemsJson);
                
                if (itemData?.Items != null)
                {
                    itemDatabase = itemData.Items.ToDictionary(item => item.Id, item => item);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load items data: {ex.Message}");
            }
        }

        private void LoadEnemies()
        {
            try
            {
                string enemiesJson = File.ReadAllText(Path.Combine(dataPath, "Enemies.json"));
                var enemyData = JsonSerializer.Deserialize<EnemyData>(enemiesJson);
                
                if (enemyData?.Enemies != null)
                {
                    enemyDatabase = enemyData.Enemies.ToDictionary(enemy => enemy.Id, enemy => enemy);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load enemies data: {ex.Message}");
            }
        }

        private void LoadCharacterClasses()
        {
            try
            {
                string classesJson = File.ReadAllText(Path.Combine(dataPath, "CharacterClasses.json"));
                var classData = JsonSerializer.Deserialize<CharacterClassData>(classesJson);
                
                if (classData?.CharacterClasses != null)
                {
                    classDatabase = classData.CharacterClasses.ToDictionary(cls => cls.Name, cls => cls);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load character classes data: {ex.Message}");
            }
        }

        public Dictionary<string, Location> LoadLocations()
        {
            try
            {
                string locationsJson = File.ReadAllText(Path.Combine(dataPath, "Locations.json"));
                var locationData = JsonSerializer.Deserialize<LocationData>(locationsJson);
                
                if (locationData?.Locations == null)
                    throw new Exception("No location data found");

                var locations = new Dictionary<string, Location>();
                
                foreach (var locationInfo in locationData.Locations)
                {
                    var location = new Location
                    {
                        Key = locationInfo.Id,
                        Name = locationInfo.Name,
                        Description = locationInfo.Description,
                        Exits = locationInfo.Exits.ToDictionary(
                            exit => exit.Direction, 
                            exit => exit.LocationId
                        ),
                        Items = locationInfo.Items.Select(CreateItemFromId).Where(item => item != null).ToList()!,
                        Enemies = locationInfo.Enemies.Select(CreateEnemyFromId).Where(enemy => enemy != null).ToList()!,
                        NPCs = locationInfo.NPCs
                    };
                    
                    locations[locationInfo.Id] = location;
                }
                
                return locations;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load locations data: {ex.Message}");
            }
        }

        public Item? CreateItemFromId(string itemId)
        {
            if (!itemDatabase.TryGetValue(itemId, out var itemInfo))
                return null;

            return new Item(
                itemInfo.Name,
                itemInfo.Description,
                ParseItemType(itemInfo.Type),
                itemInfo.Value,
                itemInfo.Price,
                itemInfo.IsStackable
            );
        }

        public Enemy? CreateEnemyFromId(string enemyId)
        {
            if (!enemyDatabase.TryGetValue(enemyId, out var enemyInfo))
                return null;

            var enemy = new Enemy(
                enemyInfo.Name,
                enemyInfo.Level,
                enemyInfo.Health,
                enemyInfo.Attack,
                enemyInfo.Defense
            );

            // Set experience and gold from JSON data
            enemy.Experience = enemyInfo.ExperienceReward;
            
            // Calculate gold reward (use random value between min and max)
            var random = new Random();
            enemy.Gold = random.Next(enemyInfo.GoldReward.Min, enemyInfo.GoldReward.Max + 1);

            // Add loot table
            foreach (var lootItem in enemyInfo.LootTable)
            {
                var item = CreateItemFromId(lootItem.ItemId);
                if (item != null)
                {
                    enemy.LootTable.Add(item);
                }
            }

            return enemy;
        }

        public Player CreatePlayerFromClass(string name, string className)
        {
            if (!classDatabase.TryGetValue(className, out var classInfo))
            {
                // Fallback to Warrior if class not found
                classInfo = classDatabase.Values.FirstOrDefault() ?? throw new Exception("No character classes available");
            }

            var characterClass = ParseCharacterClass(className);
            var player = new Player(name, characterClass);

            // Override stats with JSON data
            player.MaxHealth = classInfo.BaseStats.MaxHealth;
            player.Health = classInfo.BaseStats.MaxHealth;
            player.Attack = classInfo.BaseStats.Attack;
            player.Defense = classInfo.BaseStats.Defense;

            // Clear default inventory and add items from JSON
            player.Inventory.Clear();
            foreach (var itemId in classInfo.StartingItems)
            {
                var item = CreateItemFromId(itemId);
                if (item != null)
                {
                    player.Inventory.Add(item);
                }
            }

            return player;
        }

        public List<CharacterClassInfo> GetAvailableClasses()
        {
            return classDatabase.Values.ToList();
        }

        public CharacterClassInfo? GetClassInfo(string className)
        {
            classDatabase.TryGetValue(className, out var classInfo);
            return classInfo;
        }

        public List<EnemyInfo> GetRandomEncounterEnemies()
        {
            return enemyDatabase.Values.Where(enemy => enemy.Level <= 3).ToList();
        }

        private ItemType ParseItemType(string type)
        {
            return type.ToLower() switch
            {
                "weapon" => ItemType.Weapon,
                "armor" => ItemType.Armor,
                "potion" => ItemType.Potion,
                "key" => ItemType.Key,
                "quest" => ItemType.Quest,
                _ => ItemType.Misc
            };
        }

        private CharacterClass ParseCharacterClass(string className)
        {
            return className.ToLower() switch
            {
                "warrior" => CharacterClass.Warrior,
                "mage" => CharacterClass.Mage,
                "rogue" => CharacterClass.Rogue,
                "cleric" => CharacterClass.Cleric,
                _ => CharacterClass.Warrior
            };
        }
    }
} 