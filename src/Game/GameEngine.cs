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

        private string GetSaveDirectory()
        {
            // Create save directory in user's Documents/RealmOfAethermoor/SavedGames
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string gameFolder = Path.Combine(documentsPath, "RealmOfAethermoor");
            string saveFolder = Path.Combine(gameFolder, "SavedGames");
            
            // Ensure the directory exists
            Directory.CreateDirectory(saveFolder);
            
            return saveFolder;
        }

        private string GetSaveFilePath(string saveName)
        {
            string saveDir = GetSaveDirectory();
            return Path.Combine(saveDir, $"{saveName}.json");
        }

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
            using (var characterDialog = new CharacterCreationDialog())
            {
                if (characterDialog.ShowDialog() == DialogResult.OK)
                {
                    player = characterDialog.CreatedPlayer;
                    currentLocation = locations["village"];
                    
                    // Enable game controls now that we have a player
                    gameForm.EnableGameControls(true);
                    
                    DisplayMessage("=== Welcome to the Realm of Aethermoor ===");
                    DisplayMessage($"Welcome, {player.Name} the {player.CharacterClass}!");
                    DisplayMessage("Your adventure begins in the peaceful village of Elderbrook.");
                    DisplayMessage("Type 'help' for a list of commands, or 'look' to examine your surroundings.");
                    DisplayMessage("");
                    
                    ShowLocation();
                    UpdateCharacterDisplay();
                    HasUnsavedChanges = true;
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
                case "saves":
                case "list":
                    ListSaveFiles();
                    break;
                case "skills":
                case "skill":
                    ShowSkillTree();
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
            DisplayMessage($"A {enemy.Name} appears! Combat begins!", Color.Red);
            DisplayMessage($"Enemy Health: {enemy.Health}/{enemy.MaxHealth}", Color.Orange);
            DisplayMessage("Commands: attack, defend, use [item], flee", Color.Yellow);
            
            // Set combat mode in status bar
            if (gameForm is Form1 form1)
            {
                form1.SetCombatMode(true);
            }
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
            int expGained = currentEnemy.Level * 25;
            int goldGained = random.Next(10, 30) * currentEnemy.Level;
            
            DisplayMessage($"You defeated the {currentEnemy.Name}!", Color.Green);
            DisplayMessage($"You gained {expGained} experience and {goldGained} gold!", Color.Yellow);
            
            player.Experience += expGained;
            player.Gold += goldGained;

            // Show experience and gold gain in status bar
            if (gameForm is Form1 form1)
            {
                form1.ShowExperienceGain(expGained);
                form1.ShowGoldChange(goldGained);
            }

            // Check for level up
            bool leveledUp = false;
            while (player.Experience >= player.ExperienceToNextLevel)
            {
                player.Experience -= player.ExperienceToNextLevel;
                LevelUp();
                leveledUp = true;
            }

            // Handle loot drops
            HandleLootDrops();
            
            EndCombat();
            UpdateCharacterDisplay();
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
            
            // Clear combat mode in status bar
            if (gameForm is Form1 form1)
            {
                form1.SetCombatMode(false);
            }
        }

        private void LevelUp()
        {
            int oldLevel = player.Level;
            player.Level++;
            player.MaxHealth += 10;
            player.Health = player.MaxHealth; // Full heal on level up
            player.Attack += 2;
            player.Defense += 1;
            player.SkillPoints += 5; // Award skill points
            player.ExperienceToNextLevel = player.Level * 100;

            DisplayMessage($"🎉 LEVEL UP! You are now level {player.Level}!", Color.Gold);
            DisplayMessage($"Health increased to {player.MaxHealth}!", Color.Green);
            DisplayMessage($"Attack increased to {player.Attack}!", Color.Green);
            DisplayMessage($"Defense increased to {player.Defense}!", Color.Green);
            DisplayMessage($"You gained 5 skill points! Use 'skills' to spend them.", Color.Cyan);
            DisplayMessage("");

            // Show level up notification in status bar
            if (gameForm is Form1 form1)
            {
                form1.ShowLevelUp(oldLevel, player.Level);
            }

            UpdateCharacterDisplay();
        }

        public void ShowHelp()
        {
            gameForm.DisplayText("=== Available Commands ===", Color.Cyan);
            gameForm.DisplayText("Movement: go [direction], north, south, east, west");
            gameForm.DisplayText("Interaction: look, take [item], use [item]");
            gameForm.DisplayText("Character: inventory (inv), stats, skills, help");
            gameForm.DisplayText("Combat: attack [enemy], defend, flee");
            gameForm.DisplayText("Game: save [name], load [name], saves/list, quit");
            gameForm.DisplayText("");
            gameForm.DisplayText("Save Commands:");
            gameForm.DisplayText("  save - Quick save with timestamp");
            gameForm.DisplayText("  save [name] - Save with custom name");
            gameForm.DisplayText("  load - Load quick save");
            gameForm.DisplayText("  load [name] - Load specific save");
            gameForm.DisplayText("  saves/list - Show all available saves");
            gameForm.DisplayText("");
            gameForm.DisplayText("Character Development:");
            gameForm.DisplayText("  skills - Open skill tree to learn new abilities");
            gameForm.DisplayText("");
            gameForm.DisplayText("Keyboard shortcuts:");
            gameForm.DisplayText("Tab - Quick inventory, Ctrl+S - Quick save, Ctrl+L - Quick load, F1 - Help");
            gameForm.DisplayText("");
            string saveDir = GetSaveDirectory();
            gameForm.DisplayText($"Save files are stored in: {saveDir}", Color.Gray);
            gameForm.DisplayText("");
        }

        public void SaveGame(string saveName = null)
        {
            try
            {
                // Generate a timestamped save name if none provided
                if (string.IsNullOrEmpty(saveName))
                {
                    saveName = $"AutoSave_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
                }
                else if (saveName == "quicksave")
                {
                    saveName = "QuickSave";
                }

                var saveData = new GameSave
                {
                    Player = player,
                    CurrentLocationKey = locations.FirstOrDefault(l => l.Value == currentLocation).Key ?? "village",
                    Locations = locations,
                    SaveDate = DateTime.Now
                };

                string saveJson = JsonSerializer.Serialize(saveData, new JsonSerializerOptions { WriteIndented = true });
                string saveFilePath = GetSaveFilePath(saveName);
                File.WriteAllText(saveFilePath, saveJson);
                
                gameForm.DisplayText($"Game saved as '{saveName}'.", Color.Green);
                gameForm.DisplayText($"Save location: {saveFilePath}", Color.Gray);
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
                string saveDirectory = GetSaveDirectory();
                if (!Directory.Exists(saveDirectory))
                {
                    DisplayMessage("No saved games found.", Color.Red);
                    return;
                }

                var saveFiles = Directory.GetFiles(saveDirectory, "*.json")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToArray();

                if (saveFiles.Length == 0)
                {
                    DisplayMessage("No saved games found.", Color.Red);
                    return;
                }

                // If no save name provided, show selection dialog
                if (string.IsNullOrEmpty(saveName))
                {
                    saveName = ShowSaveFileDialog(saveFiles, "Load Game");
                    if (string.IsNullOrEmpty(saveName))
                        return;
                }

                string filePath = GetSaveFilePath(saveName);
                if (!File.Exists(filePath))
                {
                    DisplayMessage($"Save file '{saveName}' not found.", Color.Red);
                    return;
                }

                string jsonData = File.ReadAllText(filePath);
                var saveData = JsonSerializer.Deserialize<GameSave>(jsonData);

                // Restore game state
                player = saveData.Player;
                currentLocation = locations[saveData.CurrentLocationKey];
                
                // Enable game controls now that we have a loaded player
                gameForm.EnableGameControls(true);
                
                DisplayMessage($"Game loaded successfully! Welcome back, {player.Name}.", Color.Green);
                ShowLocation();
                UpdateCharacterDisplay();
                HasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                DisplayMessage($"Error loading game: {ex.Message}", Color.Red);
            }
        }

        private string ShowSaveFileDialog(string[] saveFiles, string title)
        {
            using (var form = new Form())
            {
                form.Text = title;
                form.Size = new Size(400, 300);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;

                var listBox = new ListBox
                {
                    Dock = DockStyle.Fill,
                    Font = new Font("Consolas", 10)
                };

                foreach (string saveFile in saveFiles)
                {
                    try
                    {
                        string filePath = GetSaveFilePath(saveFile);
                        var fileInfo = new FileInfo(filePath);
                        string displayText = $"{saveFile} ({fileInfo.LastWriteTime:yyyy-MM-dd HH:mm})";
                        listBox.Items.Add(new { Text = displayText, Value = saveFile });
                    }
                    catch
                    {
                        listBox.Items.Add(new { Text = saveFile, Value = saveFile });
                    }
                }

                listBox.DisplayMember = "Text";
                listBox.ValueMember = "Value";

                var buttonPanel = new Panel
                {
                    Dock = DockStyle.Bottom,
                    Height = 40
                };

                var okButton = new Button
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Location = new Point(10, 10),
                    Size = new Size(75, 23)
                };

                var cancelButton = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(95, 10),
                    Size = new Size(75, 23)
                };

                buttonPanel.Controls.AddRange(new Control[] { okButton, cancelButton });
                form.Controls.AddRange(new Control[] { listBox, buttonPanel });

                form.AcceptButton = okButton;
                form.CancelButton = cancelButton;

                if (form.ShowDialog() == DialogResult.OK && listBox.SelectedItem != null)
                {
                    dynamic selectedItem = listBox.SelectedItem;
                    return selectedItem.Value;
                }

                return null;
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

        public void ListSaveFiles()
        {
            try
            {
                string saveDir = GetSaveDirectory();
                var saveFiles = Directory.GetFiles(saveDir, "*.json")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .OrderByDescending(f => File.GetLastWriteTime(Path.Combine(saveDir, $"{f}.json")))
                    .ToList();

                if (saveFiles.Any())
                {
                    gameForm.DisplayText("=== Available Save Files ===", Color.Cyan);
                    gameForm.DisplayText($"Save directory: {saveDir}", Color.Gray);
                    gameForm.DisplayText("");

                    foreach (var saveFile in saveFiles)
                    {
                        var filePath = Path.Combine(saveDir, $"{saveFile}.json");
                        var lastModified = File.GetLastWriteTime(filePath);
                        var fileSize = new FileInfo(filePath).Length;
                        
                        gameForm.DisplayText($"• {saveFile}", Color.Yellow);
                        gameForm.DisplayText($"  Modified: {lastModified:yyyy-MM-dd HH:mm:ss}");
                        gameForm.DisplayText($"  Size: {fileSize:N0} bytes");
                        gameForm.DisplayText("");
                    }
                    
                    gameForm.DisplayText("Use 'load [filename]' to load a specific save.", Color.Cyan);
                }
                else
                {
                    gameForm.DisplayText("No save files found.", Color.Yellow);
                    gameForm.DisplayText($"Save directory: {saveDir}", Color.Gray);
                }
            }
            catch (Exception ex)
            {
                gameForm.DisplayText($"Failed to list save files: {ex.Message}", Color.Red);
            }
        }

        public void ShowSkillTree()
        {
            if (player == null)
            {
                gameForm.DisplayText("Start a new game first!", Color.Red);
                return;
            }

            try
            {
                var skillTreeForm = new SkillTreeForm(player, this);
                skillTreeForm.ShowDialog();
            }
            catch (Exception ex)
            {
                gameForm.DisplayText($"Error opening skill tree: {ex.Message}", Color.Red);
            }
        }

        public void UpdateCharacterDisplay()
        {
            // Update the main form's character stats display
            UpdateStatusBar();
        }

        public void DisplayMessage(string message, Color? color = null)
        {
            // Allow other forms to display messages in the main game window
            gameForm.DisplayText(message, color);
        }
    }
} 