using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class GameEngine
    {
        private Form1 gameForm;
        private Player player;
        private Location currentLocation;
        private Dictionary<string, Location> locations;
        private DataLoader dataLoader;
        private Random random;
        private bool isInCombat;
        private Enemy currentEnemy;

        public bool HasUnsavedChanges { get; private set; }

        public GameEngine(Form1 form)
        {
            gameForm = form;
            random = new Random();
            dataLoader = new DataLoader();
            LoadGameData();
        }

        private void LoadGameData()
        {
            try
            {
                dataLoader.LoadAllData();
                locations = dataLoader.LoadLocations();
                gameForm.DisplayText("Game data loaded successfully from JSON files.", Color.Green);
            }
            catch (Exception ex)
            {
                gameForm.DisplayText($"Error loading game data: {ex.Message}", Color.Red);
                gameForm.DisplayText("Using fallback data...", Color.Yellow);
                InitializeFallbackData();
            }
        }

        private void InitializeFallbackData()
        {
            // Fallback to hardcoded data if JSON loading fails
            locations = new Dictionary<string, Location>
            {
                ["village"] = new Location
                {
                    Name = "Village of Elderbrook",
                    Description = "A peaceful village with cobblestone streets and thatched-roof houses.",
                    Exits = new Dictionary<string, string> { ["north"] = "forest", ["east"] = "plains" },
                    Items = new List<Item> { new Item("Health Potion", "Restores 20 health", ItemType.Potion, 20) },
                    Enemies = new List<Enemy>()
                }
            };
        }

        public void StartNewGame()
        {
            gameForm.ClearScreen();
            gameForm.DisplayText("=== Welcome to the Realm of Aethermoor ===", Color.Cyan);
            gameForm.DisplayText("");
            gameForm.DisplayText("In this mystical land, legends are forged through courage and wisdom.");
            gameForm.DisplayText("Your adventure begins now...");
            gameForm.DisplayText("");

            CreateCharacter();
            currentLocation = locations["village"];
            HasUnsavedChanges = false;
            
            ShowLocation();
            gameForm.UpdateStatus($"Health: {player.Health}/{player.MaxHealth} | Level: {player.Level} | Gold: {player.Gold}");
        }

        private void CreateCharacter()
        {
            using (var characterDialog = new CharacterCreationDialog(dataLoader))
            {
                if (characterDialog.ShowDialog() == DialogResult.OK)
                {
                    player = characterDialog.CreatedCharacter;
                    gameForm.DisplayText($"Welcome, {player.Name} the {player.CharacterClass}!", Color.Yellow);
                    gameForm.DisplayText("");
                }
                else
                {
                    // Default character if dialog is cancelled
                    player = dataLoader.CreatePlayerFromClass("Hero", "Warrior");
                    gameForm.DisplayText("Welcome, Hero the Warrior!", Color.Yellow);
                    gameForm.DisplayText("");
                }
            }
        }

        public void ProcessCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return;

            string[] parts = input.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string command = parts[0];
            string[] args = parts.Skip(1).ToArray();

            HasUnsavedChanges = true;

            if (isInCombat)
            {
                ProcessCombatCommand(command, args);
                return;
            }

            switch (command)
            {
                case "look":
                case "l":
                    ShowLocation();
                    break;
                case "go":
                case "move":
                case "north":
                case "south":
                case "east":
                case "west":
                    Move(command == "go" || command == "move" ? args.FirstOrDefault() : command);
                    break;
                case "inventory":
                case "inv":
                case "i":
                    ShowInventory();
                    break;
                case "stats":
                case "status":
                    ShowCharacterStats();
                    break;
                case "take":
                case "get":
                    TakeItem(string.Join(" ", args));
                    break;
                case "use":
                    UseItem(string.Join(" ", args));
                    break;
                case "attack":
                case "fight":
                    AttackEnemy(string.Join(" ", args));
                    break;
                case "help":
                    ShowHelp();
                    break;
                case "save":
                    SaveGame(args.FirstOrDefault());
                    break;
                case "load":
                    LoadGame(args.FirstOrDefault());
                    break;
                case "quit":
                case "exit":
                    gameForm.Close();
                    break;
                default:
                    gameForm.DisplayText("I don't understand that command. Type 'help' for available commands.", Color.Red);
                    break;
            }

            UpdateStatusBar();
        }

        private void ProcessCombatCommand(string command, string[] args)
        {
            switch (command)
            {
                case "attack":
                case "fight":
                    PerformAttack();
                    break;
                case "defend":
                case "block":
                    PerformDefend();
                    break;
                case "use":
                    UseItem(string.Join(" ", args));
                    break;
                case "flee":
                case "run":
                    AttemptFlee();
                    break;
                default:
                    gameForm.DisplayText("Combat commands: attack, defend, use [item], flee", Color.Yellow);
                    break;
            }
        }

        private void ShowLocation()
        {
            gameForm.DisplayText($"=== {currentLocation.Name} ===", Color.Cyan);
            gameForm.DisplayText(currentLocation.Description);
            gameForm.DisplayText("");

            if (currentLocation.Items.Any())
            {
                gameForm.DisplayText("Items here:", Color.Yellow);
                foreach (var item in currentLocation.Items)
                {
                    gameForm.DisplayText($"  - {item.Name}");
                }
                gameForm.DisplayText("");
            }

            if (currentLocation.Enemies.Any())
            {
                gameForm.DisplayText("Enemies present:", Color.Red);
                foreach (var enemy in currentLocation.Enemies)
                {
                    gameForm.DisplayText($"  - {enemy.Name} (Level {enemy.Level})");
                }
                gameForm.DisplayText("");
            }

            if (currentLocation.Exits.Any())
            {
                gameForm.DisplayText("Exits:", Color.Cyan);
                foreach (var exit in currentLocation.Exits)
                {
                    gameForm.DisplayText($"  {exit.Key} - {exit.Value}");
                }
            }
        }

        private void Move(string direction)
        {
            if (string.IsNullOrEmpty(direction))
            {
                gameForm.DisplayText("Move where? (north, south, east, west)", Color.Yellow);
                return;
            }

            if (currentLocation.Exits.ContainsKey(direction))
            {
                string locationKey = currentLocation.Exits[direction];
                if (locations.ContainsKey(locationKey))
                {
                    currentLocation = locations[locationKey];
                    gameForm.DisplayText($"You move {direction}.", Color.Green);
                    gameForm.DisplayText("");
                    ShowLocation();

                    // Random encounter chance
                    if (random.NextDouble() < 0.3) // 30% chance
                    {
                        TriggerRandomEncounter();
                    }
                }
                else
                {
                    gameForm.DisplayText("That path seems to be blocked.", Color.Red);
                }
            }
            else
            {
                gameForm.DisplayText("You can't go that way.", Color.Red);
            }
        }

        private void TriggerRandomEncounter()
        {
            var randomEnemies = dataLoader.GetRandomEncounterEnemies();
            if (randomEnemies.Any())
            {
                var randomEnemyInfo = randomEnemies[random.Next(randomEnemies.Count)];
                var encounter = dataLoader.CreateEnemyFromId(randomEnemyInfo.Id);
                
                if (encounter != null)
                {
                    gameForm.DisplayText($"A wild {encounter.Name} appears!", Color.Red);
                    StartCombat(encounter);
                }
            }
        }

        public void ShowInventory()
        {
            gameForm.DisplayText("=== Inventory ===", Color.Yellow);
            if (player.Inventory.Any())
            {
                foreach (var item in player.Inventory)
                {
                    gameForm.DisplayText($"  - {item.Name}: {item.Description}");
                }
            }
            else
            {
                gameForm.DisplayText("Your inventory is empty.");
            }
            gameForm.DisplayText("");
        }

        public void ShowCharacterStats()
        {
            gameForm.DisplayText("=== Character Stats ===", Color.Cyan);
            gameForm.DisplayText($"Name: {player.Name}");
            gameForm.DisplayText($"Class: {player.CharacterClass}");
            gameForm.DisplayText($"Level: {player.Level}");
            gameForm.DisplayText($"Experience: {player.Experience}/{player.ExperienceToNextLevel}");
            gameForm.DisplayText($"Health: {player.Health}/{player.MaxHealth}");
            gameForm.DisplayText($"Attack: {player.Attack}");
            gameForm.DisplayText($"Defense: {player.Defense}");
            gameForm.DisplayText($"Gold: {player.Gold}");
            gameForm.DisplayText("");
        }

        private void TakeItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                gameForm.DisplayText("Take what?", Color.Yellow);
                return;
            }

            var item = currentLocation.Items.FirstOrDefault(i => 
                i.Name.ToLower().Contains(itemName.ToLower()));

            if (item != null)
            {
                player.Inventory.Add(item);
                currentLocation.Items.Remove(item);
                gameForm.DisplayText($"You take the {item.Name}.", Color.Green);
            }
            else
            {
                gameForm.DisplayText("There's no such item here.", Color.Red);
            }
        }

        private void UseItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                gameForm.DisplayText("Use what?", Color.Yellow);
                return;
            }

            var item = player.Inventory.FirstOrDefault(i => 
                i.Name.ToLower().Contains(itemName.ToLower()));

            if (item != null)
            {
                switch (item.Type)
                {
                    case ItemType.Potion:
                        player.Health = Math.Min(player.MaxHealth, player.Health + item.Value);
                        player.Inventory.Remove(item);
                        gameForm.DisplayText($"You drink the {item.Name} and restore {item.Value} health.", Color.Green);
                        break;
                    case ItemType.Weapon:
                        // Equip weapon logic would go here
                        gameForm.DisplayText($"You equip the {item.Name}.", Color.Green);
                        break;
                    default:
                        gameForm.DisplayText($"You can't use the {item.Name} right now.", Color.Yellow);
                        break;
                }
            }
            else
            {
                gameForm.DisplayText("You don't have that item.", Color.Red);
            }
        }

        private void AttackEnemy(string enemyName)
        {
            if (string.IsNullOrEmpty(enemyName))
            {
                if (currentLocation.Enemies.Any())
                {
                    StartCombat(currentLocation.Enemies.First());
                }
                else
                {
                    gameForm.DisplayText("There's nothing to attack here.", Color.Red);
                }
                return;
            }

            var enemy = currentLocation.Enemies.FirstOrDefault(e => 
                e.Name.ToLower().Contains(enemyName.ToLower()));

            if (enemy != null)
            {
                StartCombat(enemy);
            }
            else
            {
                gameForm.DisplayText("There's no such enemy here.", Color.Red);
            }
        }

        private void StartCombat(Enemy enemy)
        {
            isInCombat = true;
            currentEnemy = enemy;
            gameForm.DisplayText($"=== Combat with {enemy.Name} ===", Color.Red);
            gameForm.DisplayText($"{enemy.Name}: {enemy.Health}/{enemy.MaxHealth} HP");
            gameForm.DisplayText("Commands: attack, defend, use [item], flee");
            gameForm.DisplayText("");
        }

        private void PerformAttack()
        {
            int damage = Math.Max(1, player.Attack - currentEnemy.Defense + random.Next(-2, 3));
            currentEnemy.Health -= damage;
            gameForm.DisplayText($"You attack {currentEnemy.Name} for {damage} damage!", Color.Yellow);

            if (currentEnemy.Health <= 0)
            {
                WinCombat();
                return;
            }

            EnemyAttack();
        }

        private void PerformDefend()
        {
            gameForm.DisplayText("You raise your guard, reducing incoming damage.", Color.Cyan);
            // Reduce enemy damage by half this turn
            int damage = Math.Max(1, (currentEnemy.Attack - player.Defense) / 2 + random.Next(-1, 2));
            player.Health -= damage;
            gameForm.DisplayText($"{currentEnemy.Name} attacks you for {damage} damage!", Color.Red);

            if (player.Health <= 0)
            {
                LoseCombat();
            }
        }

        private void EnemyAttack()
        {
            int damage = Math.Max(1, currentEnemy.Attack - player.Defense + random.Next(-2, 3));
            player.Health -= damage;
            gameForm.DisplayText($"{currentEnemy.Name} attacks you for {damage} damage!", Color.Red);

            if (player.Health <= 0)
            {
                LoseCombat();
            }
        }

        private void AttemptFlee()
        {
            if (random.NextDouble() < 0.7) // 70% success rate
            {
                gameForm.DisplayText("You successfully flee from combat!", Color.Green);
                EndCombat();
            }
            else
            {
                gameForm.DisplayText("You failed to escape!", Color.Red);
                EnemyAttack();
            }
        }

        private void WinCombat()
        {
            int expGained = currentEnemy.Level * 10;
            int goldGained = random.Next(5, 20) * currentEnemy.Level;

            gameForm.DisplayText($"You defeated {currentEnemy.Name}!", Color.Green);
            gameForm.DisplayText($"You gain {expGained} experience and {goldGained} gold!", Color.Yellow);

            player.Experience += expGained;
            player.Gold += goldGained;

            // Handle loot drops
            HandleLootDrops();

            // Remove enemy from location
            currentLocation.Enemies.Remove(currentEnemy);

            // Check for level up
            if (player.Experience >= player.ExperienceToNextLevel)
            {
                LevelUp();
            }

            EndCombat();
        }

        private void HandleLootDrops()
        {
            foreach (var lootItem in currentEnemy.LootTable)
            {
                // For now, just add all loot items (in a real implementation, you'd check drop chances)
                player.Inventory.Add(lootItem);
                gameForm.DisplayText($"You found: {lootItem.Name}!", Color.Magenta);
            }
        }

        private void LoseCombat()
        {
            gameForm.DisplayText("You have been defeated!", Color.Red);
            gameForm.DisplayText("You wake up back in the village, wounded but alive.", Color.Yellow);
            
            player.Health = player.MaxHealth / 2;
            currentLocation = locations["village"];
            EndCombat();
            ShowLocation();
        }

        private void EndCombat()
        {
            isInCombat = false;
            currentEnemy = null;
        }

        private void LevelUp()
        {
            player.Level++;
            player.Experience = 0;
            player.ExperienceToNextLevel = player.Level * 100;
            player.MaxHealth += 10;
            player.Health = player.MaxHealth;
            player.Attack += 2;
            player.Defense += 1;

            gameForm.DisplayText($"Level up! You are now level {player.Level}!", Color.Magenta);
            gameForm.DisplayText("Your stats have increased!", Color.Magenta);
        }

        public void ShowHelp()
        {
            gameForm.DisplayText("=== Available Commands ===", Color.Cyan);
            gameForm.DisplayText("Movement: go [direction], north, south, east, west");
            gameForm.DisplayText("Interaction: look, take [item], use [item]");
            gameForm.DisplayText("Character: inventory (inv), stats, help");
            gameForm.DisplayText("Combat: attack [enemy], defend, flee");
            gameForm.DisplayText("Game: save [name], load [name], quit");
            gameForm.DisplayText("");
            gameForm.DisplayText("Keyboard shortcuts:");
            gameForm.DisplayText("Tab - Quick inventory, Ctrl+S - Quick save, Ctrl+L - Quick load, F1 - Help");
            gameForm.DisplayText("");
        }

        public void SaveGame(string saveName = null)
        {
            try
            {
                saveName = saveName ?? "quicksave";
                var saveData = new GameSave
                {
                    Player = player,
                    CurrentLocationKey = locations.FirstOrDefault(l => l.Value == currentLocation).Key ?? "village",
                    Locations = locations,
                    SaveDate = DateTime.Now
                };

                string saveJson = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory("saves");
                File.WriteAllText($"saves/{saveName}.json", saveJson);
                
                gameForm.DisplayText($"Game saved as '{saveName}'.", Color.Green);
                HasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                gameForm.DisplayText($"Failed to save game: {ex.Message}", Color.Red);
            }
        }

        public void LoadGame(string saveName = null)
        {
            try
            {
                saveName = saveName ?? "quicksave";
                string saveFile = $"saves/{saveName}.json";
                
                if (!File.Exists(saveFile))
                {
                    gameForm.DisplayText($"Save file '{saveName}' not found.", Color.Red);
                    return;
                }

                string saveJson = File.ReadAllText(saveFile);
                var saveData = JsonSerializer.Deserialize<GameSave>(saveJson);

                if (saveData?.Player != null && saveData.Locations != null && !string.IsNullOrEmpty(saveData.CurrentLocationKey))
                {
                    player = saveData.Player;
                    locations = saveData.Locations;
                    currentLocation = locations[saveData.CurrentLocationKey];

                    gameForm.ClearScreen();
                    gameForm.DisplayText($"Game loaded from '{saveName}'.", Color.Green);
                    gameForm.DisplayText("");
                    ShowLocation();
                    HasUnsavedChanges = false;
                }
                else
                {
                    gameForm.DisplayText("Save file is corrupted or invalid.", Color.Red);
                }
            }
            catch (Exception ex)
            {
                gameForm.DisplayText($"Failed to load game: {ex.Message}", Color.Red);
            }
        }

        private void UpdateStatusBar()
        {
            gameForm.UpdateStatus($"Health: {player.Health}/{player.MaxHealth} | Level: {player.Level} | Gold: {player.Gold}");
        }

        // Public methods for UI integration
        public Player GetPlayer()
        {
            return player;
        }

        public Dictionary<string, Location> GetLocations()
        {
            return locations;
        }

        public string GetCurrentLocationKey()
        {
            return locations.FirstOrDefault(l => l.Value == currentLocation).Key ?? "village";
        }
    }
} 