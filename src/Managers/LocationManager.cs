using WinFormsApp1.Events;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Managers
{
    /// <summary>
    /// Manages world locations and player navigation
    /// </summary>
    public class LocationManager : BaseManager, ILocationManager
    {
        private readonly IInventoryManager _inventoryManager;
        private readonly Random _random;
        private Dictionary<string, Location> _allLocations;
        private Location _currentLocation;
        private Player _currentPlayer;
        private readonly Dictionary<string, double> _encounterChances;
        private readonly Dictionary<string, DateTime> _lastSearchTimes;

        public override string ManagerName => "LocationManager";
        public Location CurrentLocation => _currentLocation;
        public string CurrentLocationKey => _currentLocation?.Key ?? "";
        public Dictionary<string, Location> AllLocations => new Dictionary<string, Location>(_allLocations);
        public Dictionary<string, Location> Locations => AllLocations; // Alias for AllLocations
        public Player CurrentPlayer => _currentPlayer;

        public LocationManager(IEventManager eventManager, IInventoryManager inventoryManager) : base(eventManager)
        {
            _inventoryManager = inventoryManager ?? throw new ArgumentNullException(nameof(inventoryManager));
            _random = new Random();
            _allLocations = new Dictionary<string, Location>();
            _encounterChances = new Dictionary<string, double>();
            _lastSearchTimes = new Dictionary<string, DateTime>();
        }

        protected override void SubscribeToEvents()
        {
            // Subscribe to game events to track player
            EventManager.Subscribe<GameStartedEvent>(OnGameStarted);
            EventManager.Subscribe<CombatEndedEvent>(OnCombatEnded);
        }

        protected override void UnsubscribeFromEvents()
        {
            EventManager.ClearSubscriptions<GameStartedEvent>();
            EventManager.ClearSubscriptions<CombatEndedEvent>();
        }

        public void SetCurrentPlayer(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            _currentPlayer = player;
            LogMessage($"Current player set to: {player.Name}");
        }

        public bool LoadLocations()
        {
            try
            {
                LogMessage("Loading locations from data files");

                // Use DataLoader to load locations
                var dataLoader = new DataLoader();
                var loadedLocations = dataLoader.LoadLocations();

                if (loadedLocations != null && loadedLocations.Count > 0)
                {
                    _allLocations = loadedLocations;
                    
                    // Set default encounter chances
                    foreach (var locationKey in _allLocations.Keys)
                    {
                        _encounterChances[locationKey] = 0.15; // 15% default encounter chance
                    }

                    // Set current location to starting location if not set
                    if (_currentLocation == null && _allLocations.ContainsKey("town_square"))
                    {
                        _currentLocation = _allLocations["town_square"];
                    }

                    var locationKeys = _allLocations.Keys.ToList();
                    EventManager.Publish(new LocationsLoadedEvent(_allLocations.Count, true, "", locationKeys));
                    LogMessage($"Successfully loaded {_allLocations.Count} locations");
                    return true;
                }
                else
                {
                    // Create default locations if loading fails
                    CreateDefaultLocations();
                    EventManager.Publish(new LocationsLoadedEvent(_allLocations.Count, true, "Used default locations"));
                    LogMessage("Created default locations");
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogError("Failed to load locations", ex);
                EventManager.Publish(new LocationsLoadedEvent(0, false, ex.Message));
                
                // Create minimal default locations as fallback
                CreateDefaultLocations();
                return false;
            }
        }

        public bool MoveToLocation(string locationKey)
        {
            if (_currentPlayer == null)
            {
                LogError("Cannot move: No current player set");
                return false;
            }

            if (string.IsNullOrWhiteSpace(locationKey))
            {
                LogError("Cannot move: Invalid location key");
                return false;
            }

            if (!LocationExists(locationKey))
            {
                var failureReason = $"Location '{locationKey}' does not exist";
                LogMessage(failureReason);
                EventManager.Publish(new MovementFailedEvent(_currentPlayer, _currentLocation, "", locationKey, failureReason));
                return false;
            }

            var previousLocation = _currentLocation;
            var newLocation = _allLocations[locationKey];
            var isFirstVisit = !newLocation.IsVisited;

            LogMessage($"Moving player from {previousLocation?.Name ?? "unknown"} to {newLocation.Name}");

            _currentLocation = newLocation;
            MarkCurrentLocationVisited();

            // Publish location changed event
            EventManager.Publish(new LocationChangedEvent(_currentPlayer, previousLocation, newLocation, "", isFirstVisit));

            // Check for random encounter
            CheckForRandomEncounter();

            LogMessage($"Successfully moved to {newLocation.Name}");
            return true;
        }

        public bool MoveInDirection(string direction)
        {
            if (_currentPlayer == null)
            {
                LogError("Cannot move: No current player set");
                return false;
            }

            if (_currentLocation == null)
            {
                LogError("Cannot move: No current location set");
                return false;
            }

            direction = direction.ToLowerInvariant();

            if (!_currentLocation.Exits.ContainsKey(direction))
            {
                var failureReason = $"Cannot go {direction} from {_currentLocation.Name}";
                LogMessage(failureReason);
                EventManager.Publish(new MovementFailedEvent(_currentPlayer, _currentLocation, direction, "", failureReason));
                return false;
            }

            var destinationKey = _currentLocation.Exits[direction];
            var previousLocation = _currentLocation;
            
            if (MoveToLocation(destinationKey))
            {
                // Update the event to include direction
                var newLocation = _currentLocation;
                var isFirstVisit = !newLocation.IsVisited;
                EventManager.Publish(new LocationChangedEvent(_currentPlayer, previousLocation, newLocation, direction, isFirstVisit));
                return true;
            }

            return false;
        }

        public Dictionary<string, string> GetAvailableExits()
        {
            return _currentLocation?.Exits ?? new Dictionary<string, string>();
        }

        public string GetExitsDescription()
        {
            var exits = GetAvailableExits();
            if (exits.Count == 0)
                return "There are no obvious exits.";

            var exitDescriptions = exits.Select(exit => 
            {
                var locationName = LocationExists(exit.Value) ? _allLocations[exit.Value].Name : "Unknown";
                return $"{exit.Key} to {locationName}";
            });

            return "Exits: " + string.Join(", ", exitDescriptions);
        }

        public bool LocationExists(string locationKey)
        {
            return !string.IsNullOrWhiteSpace(locationKey) && _allLocations.ContainsKey(locationKey);
        }

        public Location GetLocation(string locationKey)
        {
            return LocationExists(locationKey) ? _allLocations[locationKey] : null;
        }

        public bool AddLocation(Location location)
        {
            if (location == null || string.IsNullOrWhiteSpace(location.Key))
            {
                EventManager.Publish(new LocationAddedEvent(location, false, "Invalid location or missing key"));
                return false;
            }

            if (_allLocations.ContainsKey(location.Key))
            {
                EventManager.Publish(new LocationAddedEvent(location, false, "Location key already exists"));
                return false;
            }

            _allLocations[location.Key] = location;
            _encounterChances[location.Key] = 0.15; // Default encounter chance

            EventManager.Publish(new LocationAddedEvent(location, true));
            LogMessage($"Added new location: {location.Name} ({location.Key})");
            return true;
        }

        public bool RemoveLocation(string locationKey)
        {
            if (!LocationExists(locationKey))
            {
                EventManager.Publish(new LocationRemovedEvent(locationKey, null, false, "Location does not exist"));
                return false;
            }

            var removedLocation = _allLocations[locationKey];
            _allLocations.Remove(locationKey);
            _encounterChances.Remove(locationKey);

            // If this was the current location, move to a safe location
            if (_currentLocation?.Key == locationKey)
            {
                var safeLocation = _allLocations.Values.FirstOrDefault();
                if (safeLocation != null)
                {
                    MoveToLocation(safeLocation.Key);
                }
                else
                {
                    _currentLocation = null;
                }
            }

            EventManager.Publish(new LocationRemovedEvent(locationKey, removedLocation, true));
            LogMessage($"Removed location: {removedLocation.Name} ({locationKey})");
            return true;
        }

        public Enemy CheckForRandomEncounter()
        {
            if (_currentLocation == null || _currentPlayer == null)
                return null;

            var encounterChance = GetEncounterChance();
            var roll = _random.NextDouble();

            if (roll < encounterChance)
            {
                var availableEnemies = GetLocationEnemies();
                if (availableEnemies.Count > 0)
                {
                    var enemy = availableEnemies[_random.Next(availableEnemies.Count)];
                    
                    // Create a copy of the enemy for this encounter
                    var encounterEnemy = new Enemy(enemy.Name, enemy.Level, enemy.MaxHealth, enemy.Attack, enemy.Defense)
                    {
                        Experience = enemy.Experience,
                        Gold = enemy.Gold,
                        LootTable = new List<Item>(enemy.LootTable)
                    };

                    EventManager.Publish(new RandomEncounterEvent(_currentPlayer, _currentLocation, encounterEnemy, encounterChance, true));
                    LogMessage($"Random encounter: {encounterEnemy.Name} appears!");
                    return encounterEnemy;
                }
            }

            return null;
        }

        public List<Item> GetLocationItems()
        {
            return _currentLocation?.Items ?? new List<Item>();
        }

        public bool PickUpItem(Item item)
        {
            if (_currentPlayer == null || _currentLocation == null || item == null)
                return false;

            if (!_currentLocation.Items.Contains(item))
            {
                EventManager.Publish(new ItemPickedUpEvent(_currentPlayer, _currentLocation, item, false, "Item not found at location"));
                return false;
            }

            // Try to add to inventory
            if (_inventoryManager.AddItem(item, 1, $"Picked up from {_currentLocation.Name}"))
            {
                _currentLocation.Items.Remove(item);
                EventManager.Publish(new ItemPickedUpEvent(_currentPlayer, _currentLocation, item, true));
                LogMessage($"Picked up {item.Name} from {_currentLocation.Name}");
                return true;
            }
            else
            {
                EventManager.Publish(new ItemPickedUpEvent(_currentPlayer, _currentLocation, item, false, "Inventory full"));
                return false;
            }
        }

        public bool DropItem(Item item)
        {
            if (_currentPlayer == null || _currentLocation == null || item == null)
                return false;

            if (_inventoryManager.RemoveItem(item, 1, $"Dropped at {_currentLocation.Name}"))
            {
                _currentLocation.Items.Add(item);
                EventManager.Publish(new ItemDroppedEvent(_currentPlayer, _currentLocation, item, true));
                LogMessage($"Dropped {item.Name} at {_currentLocation.Name}");
                return true;
            }
            else
            {
                EventManager.Publish(new ItemDroppedEvent(_currentPlayer, _currentLocation, item, false, "Item not in inventory"));
                return false;
            }
        }

        public List<string> GetLocationNPCs()
        {
            return _currentLocation?.NPCs ?? new List<string>();
        }

        public List<Enemy> GetLocationEnemies()
        {
            return _currentLocation?.Enemies ?? new List<Enemy>();
        }

        public void MarkCurrentLocationVisited()
        {
            if (_currentLocation != null)
            {
                _currentLocation.IsVisited = true;
            }
        }

        public List<string> GetVisitedLocations()
        {
            return _allLocations.Where(kvp => kvp.Value.IsVisited).Select(kvp => kvp.Key).ToList();
        }

        public string GetLocationDescription()
        {
            if (_currentLocation == null)
                return "You are nowhere.";

            var description = $"**{_currentLocation.Name}**\n{_currentLocation.Description}\n\n";

            // Add items
            var items = GetLocationItems();
            if (items.Count > 0)
            {
                description += "Items here: " + string.Join(", ", items.Select(i => i.Name)) + "\n";
            }

            // Add NPCs
            var npcs = GetLocationNPCs();
            if (npcs.Count > 0)
            {
                description += "People here: " + string.Join(", ", npcs) + "\n";
            }

            // Add exits
            description += GetExitsDescription();

            return description;
        }

        public List<Item> SearchLocation()
        {
            if (_currentLocation == null || _currentPlayer == null)
                return new List<Item>();

            var locationKey = _currentLocation.Key;
            var now = DateTime.UtcNow;

            // Check if location was searched recently (cooldown)
            if (_lastSearchTimes.ContainsKey(locationKey))
            {
                var timeSinceLastSearch = now - _lastSearchTimes[locationKey];
                if (timeSinceLastSearch.TotalMinutes < 5) // 5 minute cooldown
                {
                    EventManager.Publish(new LocationSearchedEvent(_currentPlayer, _currentLocation, new List<Item>(), false, "You've already searched here recently."));
                    return new List<Item>();
                }
            }

            _lastSearchTimes[locationKey] = now;

            var foundItems = new List<Item>();
            var searchChance = 0.3; // 30% chance to find something

            if (_random.NextDouble() < searchChance)
            {
                // Generate a random item
                var itemTypes = new[] { "Health Potion", "Gold Coin", "Mysterious Key", "Old Scroll" };
                var itemName = itemTypes[_random.Next(itemTypes.Length)];
                
                var item = new Item
                {
                    Name = itemName,
                    Type = itemName.Contains("Potion") ? ItemType.Consumable : ItemType.Misc,
                    Value = _random.Next(5, 25),
                    Description = $"Found while searching {_currentLocation.Name}"
                };

                foundItems.Add(item);
                _currentLocation.Items.Add(item);
            }

            var searchResult = foundItems.Count > 0 
                ? $"You found: {string.Join(", ", foundItems.Select(i => i.Name))}"
                : "You search thoroughly but find nothing of interest.";

            EventManager.Publish(new LocationSearchedEvent(_currentPlayer, _currentLocation, foundItems, foundItems.Count > 0, searchResult));
            LogMessage($"Searched {_currentLocation.Name}: {searchResult}");

            return foundItems;
        }

        public int GetDistanceBetweenLocations(string fromKey, string toKey)
        {
            if (!LocationExists(fromKey) || !LocationExists(toKey))
                return -1;

            if (fromKey == toKey)
                return 0;

            // Simple BFS to find shortest path
            var queue = new Queue<(string key, int distance)>();
            var visited = new HashSet<string>();

            queue.Enqueue((fromKey, 0));
            visited.Add(fromKey);

            while (queue.Count > 0)
            {
                var (currentKey, distance) = queue.Dequeue();
                var currentLocation = _allLocations[currentKey];

                foreach (var exit in currentLocation.Exits.Values)
                {
                    if (exit == toKey)
                        return distance + 1;

                    if (!visited.Contains(exit) && LocationExists(exit))
                    {
                        visited.Add(exit);
                        queue.Enqueue((exit, distance + 1));
                    }
                }
            }

            return -1; // No path found
        }

        public List<string> FindPath(string fromKey, string toKey)
        {
            if (!LocationExists(fromKey) || !LocationExists(toKey))
                return new List<string>();

            if (fromKey == toKey)
                return new List<string> { fromKey };

            // BFS with path tracking
            var queue = new Queue<(string key, List<string> path)>();
            var visited = new HashSet<string>();

            queue.Enqueue((fromKey, new List<string> { fromKey }));
            visited.Add(fromKey);

            while (queue.Count > 0)
            {
                var (currentKey, path) = queue.Dequeue();
                var currentLocation = _allLocations[currentKey];

                foreach (var exit in currentLocation.Exits.Values)
                {
                    if (exit == toKey)
                    {
                        var completePath = new List<string>(path) { exit };
                        return completePath;
                    }

                    if (!visited.Contains(exit) && LocationExists(exit))
                    {
                        visited.Add(exit);
                        var newPath = new List<string>(path) { exit };
                        queue.Enqueue((exit, newPath));
                    }
                }
            }

            return new List<string>(); // No path found
        }

        public double GetEncounterChance()
        {
            if (_currentLocation == null)
                return 0.0;

            return _encounterChances.GetValueOrDefault(_currentLocation.Key, 0.15);
        }

        public void SetEncounterChance(string locationKey, double chance)
        {
            if (!LocationExists(locationKey))
                return;

            var oldChance = _encounterChances.GetValueOrDefault(locationKey, 0.15);
            _encounterChances[locationKey] = Math.Max(0.0, Math.Min(1.0, chance)); // Clamp between 0 and 1

            EventManager.Publish(new EncounterChanceChangedEvent(locationKey, oldChance, _encounterChances[locationKey], "LocationManager"));
            LogMessage($"Encounter chance for {locationKey} changed from {oldChance:P0} to {_encounterChances[locationKey]:P0}");
        }

        private void CreateDefaultLocations()
        {
            // Create a basic world with a few locations
            var townSquare = new Location
            {
                Key = "town_square",
                Name = "Town Square",
                Description = "The heart of the town, bustling with activity. A fountain sits in the center.",
                Exits = new Dictionary<string, string>
                {
                    { "north", "forest_entrance" },
                    { "east", "market" },
                    { "south", "tavern" }
                }
            };

            var forestEntrance = new Location
            {
                Key = "forest_entrance",
                Name = "Forest Entrance",
                Description = "The edge of a dark forest. Ancient trees loom overhead.",
                Exits = new Dictionary<string, string>
                {
                    { "south", "town_square" },
                    { "north", "deep_forest" }
                },
                Enemies = new List<Enemy>
                {
                    new Enemy("Wolf", 1, 25, 8, 3),
                    new Enemy("Goblin Scout", 2, 30, 10, 4)
                }
            };

            var market = new Location
            {
                Key = "market",
                Name = "Market District",
                Description = "A busy marketplace filled with merchants and shoppers.",
                Exits = new Dictionary<string, string>
                {
                    { "west", "town_square" }
                },
                NPCs = new List<string> { "Merchant", "Guard" },
                Items = new List<Item>
                {
                    new Item("Health Potion", "Restores 20 health", ItemType.Consumable, 20)
                }
            };

            var tavern = new Location
            {
                Key = "tavern",
                Name = "The Prancing Pony Tavern",
                Description = "A cozy tavern with warm light and the smell of ale.",
                Exits = new Dictionary<string, string>
                {
                    { "north", "town_square" }
                },
                NPCs = new List<string> { "Bartender", "Traveler" }
            };

            var deepForest = new Location
            {
                Key = "deep_forest",
                Name = "Deep Forest",
                Description = "The heart of the forest, where few dare to venture.",
                Exits = new Dictionary<string, string>
                {
                    { "south", "forest_entrance" }
                },
                Enemies = new List<Enemy>
                {
                    new Enemy("Forest Bear", 3, 50, 15, 8),
                    new Enemy("Orc Warrior", 4, 60, 18, 10)
                }
            };

            _allLocations = new Dictionary<string, Location>
            {
                { townSquare.Key, townSquare },
                { forestEntrance.Key, forestEntrance },
                { market.Key, market },
                { tavern.Key, tavern },
                { deepForest.Key, deepForest }
            };

            // Set default encounter chances
            foreach (var locationKey in _allLocations.Keys)
            {
                _encounterChances[locationKey] = locationKey.Contains("forest") ? 0.25 : 0.05;
            }

            _currentLocation = townSquare;
            LogMessage("Created default world with 5 locations");
        }

        private void OnGameStarted(GameStartedEvent e)
        {
            SetCurrentPlayer(e.Player);
            
            // Load locations if not already loaded
            if (_allLocations.Count == 0)
            {
                LoadLocations();
            }

            // Set starting location if not set
            if (_currentLocation == null && _allLocations.ContainsKey("town_square"))
            {
                MoveToLocation("town_square");
            }
        }

        private void OnCombatEnded(CombatEndedEvent e)
        {
            // After combat, there's a reduced chance of immediate re-encounter
            if (_currentLocation != null)
            {
                var currentChance = GetEncounterChance();
                SetEncounterChance(_currentLocation.Key, currentChance * 0.5); // Halve encounter chance temporarily
                
                // Reset encounter chance after 2 minutes
                // In a real implementation, you might use a timer or game tick system
            }
        }
    }
} 