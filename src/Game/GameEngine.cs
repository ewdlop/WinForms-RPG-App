using System.Text.Json;
using WinFormsApp1.Constants;

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

        // Cheat system variables
        private bool cheatsEnabled = false;
        private bool godModeEnabled = false;
        private bool infiniteHealthEnabled = false;
        private bool infiniteGoldEnabled = false;
        private bool noClipModeEnabled = false;
        private int cheatActivationSequence = 0;
        private readonly string[] cheatActivationCommands = CheatConstants.KONAMI_CODE_SEQUENCE;

        public bool HasUnsavedChanges { get; private set; }

        private string GetSaveDirectory()
        {
            // Create save directory in user's Documents/RealmOfAethermoor/SavedGames
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string gameFolder = Path.Combine(documentsPath, GameConstants.GAME_FOLDER_NAME);
            string saveFolder = Path.Combine(gameFolder, GameConstants.SAVE_FOLDER_NAME);
            
            // Ensure the directory exists
            Directory.CreateDirectory(saveFolder);
            
            return saveFolder;
        }

        private string GetSaveFilePath(string saveName)
        {
            string saveDir = GetSaveDirectory();
            return Path.Combine(saveDir, $"{saveName}{GameConstants.JSON_EXTENSION}");
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

            // Check for cheat activation sequence
            CheckCheatActivation(command);

            // Process cheat commands if cheats are enabled
            if (cheatsEnabled && ProcessCheatCommand(command, args))
            {
                return;
            }

            if (isInCombat)
            {
                ProcessCombatCommand(command, args);
                return;
            }

            switch (command)
            {
                case GameConstants.CMD_LOOK:
                case GameConstants.CMD_LOOK_SHORT:
                    ShowLocation();
                    break;
                case GameConstants.CMD_GO:
                case GameConstants.CMD_MOVE:
                case GameConstants.CMD_NORTH:
                case GameConstants.CMD_SOUTH:
                case GameConstants.CMD_EAST:
                case GameConstants.CMD_WEST:
                    Move(command == GameConstants.CMD_GO || command == GameConstants.CMD_MOVE ? args.FirstOrDefault() : command);
                    break;
                case GameConstants.CMD_INVENTORY:
                case GameConstants.CMD_INV:
                case GameConstants.CMD_I:
                    ShowInventory();
                    break;
                case GameConstants.CMD_STATS:
                case GameConstants.CMD_STATUS:
                    ShowCharacterStats();
                    break;
                case GameConstants.CMD_TAKE:
                case GameConstants.CMD_GET:
                    TakeItem(string.Join(" ", args));
                    break;
                case GameConstants.CMD_USE:
                    UseItem(string.Join(" ", args));
                    break;
                case GameConstants.CMD_ATTACK:
                case GameConstants.CMD_FIGHT:
                    AttackEnemy(string.Join(" ", args));
                    break;
                case GameConstants.CMD_HELP:
                    ShowHelp();
                    break;
                case GameConstants.CMD_CHEAT:
                case GameConstants.CMD_CHEATS:
                    if (cheatsEnabled)
                    {
                        ShowCheatHelp();
                    }
                    else
                    {
                        ShowCheatActivationHelp();
                    }
                    break;
                case GameConstants.CMD_SAVE:
                    SaveGame(args.FirstOrDefault());
                    break;
                case GameConstants.CMD_LOAD:
                    LoadGame(args.FirstOrDefault());
                    break;
                case GameConstants.CMD_SAVES:
                case GameConstants.CMD_LIST:
                    ListSaveFiles();
                    break;
                case GameConstants.CMD_SKILLS:
                case GameConstants.CMD_SKILL:
                    ShowSkillTree();
                    break;
                case GameConstants.CMD_QUIT:
                case GameConstants.CMD_EXIT:
                    gameForm.Close();
                    break;
                default:
                    gameForm.DisplayText(GameConstants.UNKNOWN_COMMAND_MSG, Color.Red);
                    break;
            }

            UpdateStatusBar();
        }

        private void CheckCheatActivation(string command)
        {
            // Special cheat activation sequences
            if (CheatConstants.CLASSIC_CHEAT_CODES.Contains(command))
            {
                ActivateCheats();
                return;
            }

            // Konami code sequence
            if (cheatActivationSequence < cheatActivationCommands.Length && 
                command == cheatActivationCommands[cheatActivationSequence])
            {
                cheatActivationSequence++;
                if (cheatActivationSequence >= cheatActivationCommands.Length)
                {
                    ActivateCheats();
                    cheatActivationSequence = 0;
                }
            }
            else
            {
                cheatActivationSequence = 0;
            }
        }

        private void ActivateCheats()
        {
            if (!cheatsEnabled)
            {
                cheatsEnabled = true;
                DisplayMessage(GameConstants.CHEAT_ACTIVATED_MSG, Color.Magenta);
                DisplayMessage("Type 'cheathelp' for available cheat commands.", Color.Cyan);
                DisplayMessage("Note: Using cheats may affect game balance!", Color.Yellow);
            }
            else
            {
                DisplayMessage("Cheats are already enabled!", Color.Cyan);
            }
        }

        private bool ProcessCheatCommand(string command, string[] args)
        {
            if (player == null && !command.StartsWith(GameConstants.CMD_CHEAT))
            {
                DisplayMessage(GameConstants.START_GAME_FOR_CHEATS_MSG, Color.Red);
                return true;
            }

            switch (command)
            {
                case GameConstants.CHEAT_CMD_HELP:
                case GameConstants.CMD_CHEATS:
                    ShowCheatHelp();
                    return true;

                case GameConstants.CHEAT_CMD_GODMODE:
                case GameConstants.CHEAT_CMD_GOD:
                    ToggleGodMode();
                    return true;

                case GameConstants.CHEAT_CMD_INFINITE_HEALTH:
                case GameConstants.CHEAT_CMD_INF_HEALTH:
                    ToggleInfiniteHealth();
                    return true;

                case GameConstants.CHEAT_CMD_INFINITE_GOLD:
                case GameConstants.CHEAT_CMD_INF_GOLD:
                    ToggleInfiniteGold();
                    return true;

                case GameConstants.CHEAT_CMD_ADD_GOLD:
                case GameConstants.CHEAT_CMD_GOLD:
                    AddGold(args);
                    return true;

                case GameConstants.CHEAT_CMD_ADD_EXP:
                case GameConstants.CHEAT_CMD_EXP:
                case GameConstants.CHEAT_CMD_EXPERIENCE:
                    AddExperience(args);
                    return true;

                case GameConstants.CHEAT_CMD_LEVEL_UP:
                case GameConstants.CHEAT_CMD_LVL_UP:
                    CheatLevelUp(args);
                    return true;

                case GameConstants.CHEAT_CMD_SET_LEVEL:
                case GameConstants.CHEAT_CMD_LEVEL:
                    SetLevel(args);
                    return true;

                case GameConstants.CHEAT_CMD_HEAL:
                case GameConstants.CHEAT_CMD_FULL_HEAL:
                    FullHeal();
                    return true;

                case GameConstants.CHEAT_CMD_ADD_ITEM:
                case GameConstants.CHEAT_CMD_GIVE_ITEM:
                case GameConstants.CHEAT_CMD_ITEM:
                    GiveItem(args);
                    return true;

                case GameConstants.CHEAT_CMD_CLEAR_INVENTORY:
                case GameConstants.CHEAT_CMD_CLEAR_INV:
                    ClearInventory();
                    return true;

                case GameConstants.CHEAT_CMD_TELEPORT:
                case GameConstants.CHEAT_CMD_TP:
                case GameConstants.CHEAT_CMD_GOTO:
                    TeleportToLocation(args);
                    return true;

                case GameConstants.CHEAT_CMD_NOCLIP:
                    ToggleNoClip();
                    return true;

                case GameConstants.CHEAT_CMD_SPAWN_ENEMY:
                case GameConstants.CHEAT_CMD_ADD_ENEMY:
                    SpawnEnemy(args);
                    return true;

                case GameConstants.CHEAT_CMD_KILL_ALL_ENEMIES:
                case GameConstants.CHEAT_CMD_KILL_ENEMIES:
                    KillAllEnemies();
                    return true;

                case GameConstants.CHEAT_CMD_MAX_STATS:
                    MaximizeStats();
                    return true;

                case GameConstants.CHEAT_CMD_SET_STATS:
                    SetStats(args);
                    return true;

                case GameConstants.CHEAT_CMD_SHOW_DEBUG:
                case GameConstants.CHEAT_CMD_DEBUG:
                    ShowDebugInfo();
                    return true;

                case GameConstants.CHEAT_CMD_RESET_GAME:
                    ResetGameState();
                    return true;

                case GameConstants.CHEAT_CMD_DISABLE_CHEATS:
                    DisableCheats();
                    return true;

                default:
                    return false; // Not a cheat command
            }
        }

        private void ShowCheatHelp()
        {
            DisplayMessage(GameConstants.CHEAT_HELP_MSG, Color.Magenta);
            DisplayMessage("", Color.White);
            DisplayMessage("🎮 GAME STATE:", Color.Cyan);
            DisplayMessage("  godmode/god - Toggle invincibility", Color.White);
            DisplayMessage("  infinitehealth/infhealth - Toggle infinite health", Color.White);
            DisplayMessage("  infinitegold/infgold - Toggle infinite gold", Color.White);
            DisplayMessage("  noclip - Toggle movement restrictions", Color.White);
            DisplayMessage("", Color.White);
            DisplayMessage("💰 RESOURCES:", Color.Cyan);
            DisplayMessage($"  addgold [amount] - Add gold (default: {GameConstants.DEFAULT_GOLD_AMOUNT})", Color.White);
            DisplayMessage($"  addexp [amount] - Add experience (default: {GameConstants.DEFAULT_EXP_AMOUNT})", Color.White);
            DisplayMessage("  heal/fullheal - Restore full health", Color.White);
            DisplayMessage("", Color.White);
            DisplayMessage("📊 CHARACTER:", Color.Cyan);
            DisplayMessage($"  levelup [count] - Level up (default: {GameConstants.DEFAULT_LEVEL_UP_COUNT})", Color.White);
            DisplayMessage("  setlevel [level] - Set specific level", Color.White);
            DisplayMessage("  maxstats - Maximize all stats", Color.White);
            DisplayMessage("  setstats [att] [def] [hp] - Set attack/defense/health", Color.White);
            DisplayMessage("", Color.White);
            DisplayMessage("🎒 INVENTORY:", Color.Cyan);
            DisplayMessage("  additem [name] - Add item to inventory", Color.White);
            DisplayMessage("  clearinventory - Remove all items", Color.White);
            DisplayMessage("", Color.White);
            DisplayMessage("🗺️ WORLD:", Color.Cyan);
            DisplayMessage("  teleport [location] - Travel to location", Color.White);
            DisplayMessage("  spawnenemy [name] - Spawn enemy in current location", Color.White);
            DisplayMessage("  killallenemies - Remove all enemies from location", Color.White);
            DisplayMessage("", Color.White);
            DisplayMessage("🔧 DEBUG:", Color.Cyan);
            DisplayMessage("  showdebug - Display debug information", Color.White);
            DisplayMessage("  resetgame - Reset to initial state", Color.White);
            DisplayMessage("  disablecheats - Turn off cheat mode", Color.White);
            DisplayMessage("", Color.White);
            DisplayMessage($"Available locations: {GameConstants.VILLAGE_LOCATION}, {GameConstants.FOREST_LOCATION}, {GameConstants.PLAINS_LOCATION}, {GameConstants.CAVE_LOCATION}, {GameConstants.RUINS_LOCATION}, {GameConstants.LAIR_LOCATION}", Color.Gray);
        }

        private void ShowCheatActivationHelp()
        {
            DisplayMessage(GameConstants.CHEAT_ACTIVATION_MSG, Color.Yellow);
            DisplayMessage("", Color.White);
            DisplayMessage("🎮 How to Activate Cheats:", Color.Cyan);
            DisplayMessage("", Color.White);
            DisplayMessage("Method 1 - Classic Gaming References:", Color.Green);
            DisplayMessage($"  Type: {GameConstants.CHEAT_IDDQD}", Color.White);
            DisplayMessage($"  Type: {GameConstants.CHEAT_IDKFA}", Color.White);
            DisplayMessage($"  Type: {GameConstants.CHEAT_THEREISNOSPOON}", Color.White);
            DisplayMessage("", Color.White);
            DisplayMessage("Method 2 - Konami Code Sequence:", Color.Green);
            DisplayMessage("  Type these commands in order:", Color.White);
            DisplayMessage($"  {GameConstants.CHEAT_UP} → {GameConstants.CHEAT_UP} → {GameConstants.CHEAT_DOWN} → {GameConstants.CHEAT_DOWN} → {GameConstants.CHEAT_LEFT} → {GameConstants.CHEAT_RIGHT} → {GameConstants.CHEAT_LEFT} → {GameConstants.CHEAT_RIGHT} → {GameConstants.CHEAT_B} → {GameConstants.CHEAT_A}", Color.Gray);
            DisplayMessage("", Color.White);
            DisplayMessage("Method 3 - Direct Activation:", Color.Green);
            DisplayMessage($"  Type: {GameConstants.CHEAT_KONAMI}", Color.White);
            DisplayMessage("", Color.White);
            DisplayMessage("💡 Tips:", Color.Yellow);
            DisplayMessage("  • Commands are case-insensitive", Color.Gray);
            DisplayMessage("  • Once activated, type 'cheathelp' for all cheat commands", Color.Gray);
            DisplayMessage("  • Cheats affect game balance - use responsibly!", Color.Orange);
            DisplayMessage("  • You can disable cheats anytime with 'disablecheats'", Color.Gray);
            DisplayMessage("", Color.White);
            DisplayMessage("🎯 Popular Cheat Commands (once activated):", Color.Cyan);
            DisplayMessage("  god - Invincibility mode", Color.White);
            DisplayMessage("  addgold 9999 - Lots of gold", Color.White);
            DisplayMessage("  maxstats - Maximize all stats", Color.White);
            DisplayMessage("  teleport [location] - Instant travel", Color.White);
            DisplayMessage("", Color.White);
        }

        private void ToggleGodMode()
        {
            godModeEnabled = !godModeEnabled;
            DisplayMessage($"God Mode: {(godModeEnabled ? "ENABLED" : "DISABLED")}", 
                godModeEnabled ? Color.Gold : Color.Gray);
            
            if (godModeEnabled)
            {
                DisplayMessage("You are now invincible!", Color.Yellow);
            }
        }

        private void ToggleInfiniteHealth()
        {
            infiniteHealthEnabled = !infiniteHealthEnabled;
            DisplayMessage($"Infinite Health: {(infiniteHealthEnabled ? "ENABLED" : "DISABLED")}", 
                infiniteHealthEnabled ? Color.Green : Color.Gray);
        }

        private void ToggleInfiniteGold()
        {
            infiniteGoldEnabled = !infiniteGoldEnabled;
            DisplayMessage($"Infinite Gold: {(infiniteGoldEnabled ? "ENABLED" : "DISABLED")}", 
                infiniteGoldEnabled ? Color.Gold : Color.Gray);
        }

        private void AddGold(string[] args)
        {
            int amount = GameConstants.DEFAULT_GOLD_AMOUNT;
            if (args.Length > 0 && int.TryParse(args[0], out int parsedAmount))
            {
                amount = Math.Max(GameConstants.MIN_STAT_VALUE, parsedAmount);
            }

            player.Gold += amount;
            DisplayMessage($"Added {amount} gold! Total: {player.Gold}", Color.Gold);
            UpdateCharacterDisplay();
        }

        private void AddExperience(string[] args)
        {
            int amount = GameConstants.DEFAULT_EXP_AMOUNT;
            if (args.Length > 0 && int.TryParse(args[0], out int parsedAmount))
            {
                amount = Math.Max(GameConstants.MIN_STAT_VALUE, parsedAmount);
            }

            int oldLevel = player.Level;
            player.Experience += amount;
            
            // Check for level ups
            while (player.Experience >= player.Level * 100)
            {
                player.Experience -= player.Level * 100;
                LevelUp();
            }

            DisplayMessage($"Added {amount} experience!", Color.Cyan);
            if (player.Level > oldLevel)
            {
                DisplayMessage($"Level increased from {oldLevel} to {player.Level}!", Color.Gold);
            }
            UpdateCharacterDisplay();
        }

        private void CheatLevelUp(string[] args)
        {
            int count = GameConstants.DEFAULT_LEVEL_UP_COUNT;
            if (args.Length > 0 && int.TryParse(args[0], out int parsedCount))
            {
                count = Math.Max(GameConstants.MIN_STAT_VALUE, Math.Min(GameConstants.MAX_LEVEL, parsedCount));
            }

            int oldLevel = player.Level;
            for (int i = 0; i < count; i++)
            {
                LevelUp();
            }

            DisplayMessage($"Leveled up {count} times! Level: {oldLevel} → {player.Level}", Color.Gold);
            UpdateCharacterDisplay();
        }

        private void SetLevel(string[] args)
        {
            if (args.Length == 0 || !int.TryParse(args[0], out int level))
            {
                DisplayMessage("Usage: setlevel [level]", Color.Red);
                return;
            }

            level = Math.Max(GameConstants.MIN_LEVEL, Math.Min(GameConstants.MAX_LEVEL, level));
            int oldLevel = player.Level;
            
            if (level > oldLevel)
            {
                // Level up
                for (int i = oldLevel; i < level; i++)
                {
                    LevelUp();
                }
            }
            else if (level < oldLevel)
            {
                // Level down (recalculate stats)
                player.Level = level;
                player.MaxHealth = 100 + (level - 1) * 20;
                player.Health = player.MaxHealth;
                player.Attack = 10 + (level - 1) * 3;
                player.Defense = 5 + (level - 1) * 2;
                player.Experience = 0;
            }

            DisplayMessage($"Level set to {level}! (was {oldLevel})", Color.Gold);
            UpdateCharacterDisplay();
        }

        private void FullHeal()
        {
            player.Health = player.MaxHealth;
            DisplayMessage($"Fully healed! Health: {player.Health}/{player.MaxHealth}", Color.Green);
            UpdateCharacterDisplay();
        }

        private void GiveItem(string[] args)
        {
            if (args.Length == 0)
            {
                DisplayMessage("Usage: additem [item name]", Color.Red);
                DisplayMessage("Available items: potion, sword, shield, bow, dagger", Color.Gray);
                return;
            }

            string itemName = string.Join(" ", args).ToLower();
            
            // Create basic items based on name
            Item item = itemName switch
            {
                "potion" or "health potion" => new Item { Name = "Health Potion", Type = ItemType.Potion, Value = 50, Description = "Restores 50 health" },
                "sword" => new Item { Name = "Iron Sword", Type = ItemType.Weapon, Value = 15, Description = "A sturdy iron sword" },
                "shield" => new Item { Name = "Iron Shield", Type = ItemType.Armor, Value = 10, Description = "A protective iron shield" },
                "bow" => new Item { Name = "Hunting Bow", Type = ItemType.Weapon, Value = 12, Description = "A well-crafted bow" },
                "dagger" => new Item { Name = "Steel Dagger", Type = ItemType.Weapon, Value = 8, Description = "A sharp steel dagger" },
                "gold" => null, // Handle gold separately
                _ => new Item { Name = itemName, Type = ItemType.Misc, Value = 1, Description = $"A mysterious {itemName}" }
            };

            if (itemName.Contains("gold"))
            {
                AddGold(new[] { "100" });
                return;
            }

            if (item != null)
            {
                player.Inventory.Add(item);
                DisplayMessage($"Added {item.Name} to inventory!", Color.Green);
            }
            else
            {
                DisplayMessage($"Unknown item: {itemName}", Color.Red);
            }
        }

        private void ClearInventory()
        {
            int itemCount = player.Inventory.Count;
            player.Inventory.Clear();
            DisplayMessage($"Removed {itemCount} items from inventory.", Color.Orange);
        }

        private void TeleportToLocation(string[] args)
        {
            if (args.Length == 0)
            {
                DisplayMessage("Usage: teleport [location]", Color.Red);
                DisplayMessage("Available locations: " + string.Join(", ", locations.Keys), Color.Gray);
                return;
            }

            string locationKey = args[0].ToLower();
            
            if (locations.ContainsKey(locationKey))
            {
                currentLocation = locations[locationKey];
                DisplayMessage($"⚡ Teleported to {currentLocation.Name}!", Color.Magenta);
                ShowLocation();
                UpdateCharacterDisplay();
            }
            else
            {
                DisplayMessage($"Unknown location: {locationKey}", Color.Red);
                DisplayMessage("Available locations: " + string.Join(", ", locations.Keys), Color.Gray);
            }
        }

        private void ToggleNoClip()
        {
            noClipModeEnabled = !noClipModeEnabled;
            DisplayMessage($"No-Clip Mode: {(noClipModeEnabled ? "ENABLED" : "DISABLED")}", 
                noClipModeEnabled ? Color.Cyan : Color.Gray);
            
            if (noClipModeEnabled)
            {
                DisplayMessage("You can now move to any location without restrictions!", Color.Yellow);
            }
        }

        private void SpawnEnemy(string[] args)
        {
            if (args.Length == 0)
            {
                DisplayMessage("Usage: spawnenemy [enemy name]", Color.Red);
                DisplayMessage("Available enemies: goblin, orc, troll, dragon", Color.Gray);
                return;
            }

            string enemyName = string.Join(" ", args).ToLower();
            
            Enemy enemy = enemyName switch
            {
                "goblin" => new Enemy { Name = "Goblin", Health = 30, MaxHealth = 30, Attack = 8, Defense = 2, Experience = 25, Gold = 10 },
                "orc" => new Enemy { Name = "Orc", Health = 60, MaxHealth = 60, Attack = 15, Defense = 5, Experience = 50, Gold = 25 },
                "troll" => new Enemy { Name = "Troll", Health = 120, MaxHealth = 120, Attack = 25, Defense = 10, Experience = 100, Gold = 50 },
                "dragon" => new Enemy { Name = "Ancient Dragon", Health = 300, MaxHealth = 300, Attack = 50, Defense = 20, Experience = 500, Gold = 200 },
                _ => new Enemy { Name = enemyName, Health = 50, MaxHealth = 50, Attack = 10, Defense = 3, Experience = 30, Gold = 15 }
            };

            currentLocation.Enemies.Add(enemy);
            DisplayMessage($"Spawned {enemy.Name} in {currentLocation.Name}!", Color.Red);
        }

        private void KillAllEnemies()
        {
            int enemyCount = currentLocation.Enemies.Count;
            currentLocation.Enemies.Clear();
            
            if (isInCombat)
            {
                isInCombat = false;
                currentEnemy = null;
                if (gameForm is Form1 form1)
                {
                    form1.SetCombatMode(false);
                }
            }

            DisplayMessage($"Removed {enemyCount} enemies from {currentLocation.Name}.", Color.Orange);
        }

        private void MaximizeStats()
        {
            player.Health = player.MaxHealth = GameConstants.MAX_STAT_VALUE;
            player.Attack = GameConstants.MAX_STAT_VALUE / 10; // 999
            player.Defense = GameConstants.MAX_STAT_VALUE / 10; // 999
            player.Gold = GameConstants.MAX_GOLD;
            player.SkillPoints = GameConstants.MAX_SKILL_POINTS;

            DisplayMessage("All stats maximized!", Color.Gold);
            DisplayMessage($"Health: {GameConstants.MAX_STAT_VALUE}, Attack: {GameConstants.MAX_STAT_VALUE / 10}, Defense: {GameConstants.MAX_STAT_VALUE / 10}", Color.Yellow);
            DisplayMessage($"Gold: {GameConstants.MAX_GOLD:N0}, Skill Points: {GameConstants.MAX_SKILL_POINTS}", Color.Yellow);
            UpdateCharacterDisplay();
        }

        private void SetStats(string[] args)
        {
            if (args.Length < 3)
            {
                DisplayMessage("Usage: setstats [attack] [defense] [health]", Color.Red);
                return;
            }

            if (int.TryParse(args[0], out int attack) && 
                int.TryParse(args[1], out int defense) && 
                int.TryParse(args[2], out int health))
            {
                player.Attack = Math.Max(GameConstants.MIN_STAT_VALUE, Math.Min(GameConstants.MAX_STAT_VALUE, attack));
                player.Defense = Math.Max(GameConstants.MIN_STAT_VALUE, Math.Min(GameConstants.MAX_STAT_VALUE, defense));
                player.MaxHealth = Math.Max(GameConstants.MIN_STAT_VALUE, Math.Min(GameConstants.MAX_STAT_VALUE, health));
                player.Health = player.MaxHealth;

                DisplayMessage($"Stats set - Attack: {player.Attack}, Defense: {player.Defense}, Health: {player.Health}", Color.Gold);
                UpdateCharacterDisplay();
            }
            else
            {
                DisplayMessage("Invalid numbers provided!", Color.Red);
            }
        }

        private void ShowDebugInfo()
        {
            DisplayMessage(GameConstants.DEBUG_INFO_MSG, Color.Cyan);
            DisplayMessage($"Cheats Enabled: {cheatsEnabled}", Color.White);
            DisplayMessage($"God Mode: {godModeEnabled}", Color.White);
            DisplayMessage($"Infinite Health: {infiniteHealthEnabled}", Color.White);
            DisplayMessage($"Infinite Gold: {infiniteGoldEnabled}", Color.White);
            DisplayMessage($"No-Clip Mode: {noClipModeEnabled}", Color.White);
            DisplayMessage($"In Combat: {isInCombat}", Color.White);
            DisplayMessage($"Current Location: {currentLocation?.Key} ({currentLocation?.Name})", Color.White);
            DisplayMessage($"Player Level: {player?.Level}", Color.White);
            DisplayMessage($"Player Health: {player?.Health}/{player?.MaxHealth}", Color.White);
            DisplayMessage($"Player Gold: {player?.Gold}", Color.White);
            DisplayMessage($"Inventory Items: {player?.Inventory?.Count}", Color.White);
            DisplayMessage($"Location Enemies: {currentLocation?.Enemies?.Count}", Color.White);
            DisplayMessage($"Location Items: {currentLocation?.Items?.Count}", Color.White);
        }

        private void ResetGameState()
        {
            var result = MessageBox.Show("This will reset your character to level 1 and clear progress. Continue?", 
                "Reset Game", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            
            if (result == DialogResult.Yes)
            {
                // Reset player to initial state
                player.Level = GameConstants.MIN_LEVEL;
                player.Experience = 0;
                player.Health = player.MaxHealth = 100;
                player.Attack = 10;
                player.Defense = 5;
                player.Gold = 100;
                player.SkillPoints = 10;
                player.Inventory.Clear();
                player.LearnedSkills.Clear();

                // Reset to starting location
                currentLocation = locations[GameConstants.DEFAULT_LOCATION];

                DisplayMessage("Game state reset to beginning!", Color.Orange);
                ShowLocation();
                UpdateCharacterDisplay();
            }
        }

        private void DisableCheats()
        {
            cheatsEnabled = false;
            godModeEnabled = false;
            infiniteHealthEnabled = false;
            infiniteGoldEnabled = false;
            noClipModeEnabled = false;
            cheatActivationSequence = 0;

            DisplayMessage("Cheat codes disabled. Game balance restored.", Color.Gray);
        }

        private void ProcessCombatCommand(string command, string[] args)
        {
            switch (command)
            {
                case GameConstants.CMD_ATTACK:
                case GameConstants.CMD_FIGHT:
                    PerformAttack();
                    break;
                case GameConstants.CMD_DEFEND:
                case GameConstants.CMD_BLOCK:
                    PerformDefend();
                    break;
                case GameConstants.CMD_USE:
                    UseItem(string.Join(" ", args));
                    break;
                case GameConstants.CMD_FLEE:
                case GameConstants.CMD_RUN:
                    AttemptFlee();
                    break;
                default:
                    gameForm.DisplayText(GameConstants.COMBAT_COMMANDS_MSG, Color.Yellow);
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
            if (noClipModeEnabled)
            {
                // In no-clip mode, allow movement to any location
                switch (direction?.ToLower())
                {
                    case GameConstants.VILLAGE_LOCATION:
                    case GameConstants.FOREST_LOCATION:
                    case GameConstants.PLAINS_LOCATION:
                    case GameConstants.CAVE_LOCATION:
                    case GameConstants.RUINS_LOCATION:
                    case GameConstants.LAIR_LOCATION:
                        if (locations.ContainsKey(direction.ToLower()))
                        {
                            currentLocation = locations[direction.ToLower()];
                            DisplayMessage($"⚡ No-clip teleport to {currentLocation.Name}!", Color.Magenta);
                            ShowLocation();
                            return;
                        }
                        break;
                }
            }

            // Normal movement logic
            if (currentLocation?.Exits?.ContainsKey(direction) == true)
            {
                string nextLocationKey = currentLocation.Exits[direction];
                if (locations.ContainsKey(nextLocationKey))
                {
                    currentLocation = locations[nextLocationKey];
                    DisplayMessage($"You move {direction} to {currentLocation.Name}.", Color.Green);
                    ShowLocation();
                    CheckRandomEncounter();
                }
            }
            else
            {
                DisplayMessage(GameConstants.CANT_GO_THAT_WAY_MSG, Color.Red);
                if (currentLocation?.Exits?.Any() == true)
                {
                    DisplayMessage($"Available exits: {string.Join(", ", currentLocation.Exits.Keys)}", Color.Yellow);
                }
            }
        }

        private void CheckRandomEncounter()
        {
            if (random.NextDouble() < GameConstants.RANDOM_ENCOUNTER_CHANCE)
            {
                TriggerRandomEncounter();
            }
        }

        private void TriggerRandomEncounter()
        {
            // Create a random enemy encounter
            string[] possibleEnemies = { GameConstants.GOBLIN, GameConstants.ORC, GameConstants.WOLF, GameConstants.BANDIT };
            string enemyType = possibleEnemies[random.Next(possibleEnemies.Length)];
            
            Enemy randomEnemy = enemyType switch
            {
                GameConstants.GOBLIN => new Enemy { Name = "Wild Goblin", Health = 25, MaxHealth = 25, Attack = 6, Defense = 1, Experience = 15, Gold = 8 },
                GameConstants.ORC => new Enemy { Name = "Wandering Orc", Health = 40, MaxHealth = 40, Attack = 10, Defense = 3, Experience = 30, Gold = 15 },
                GameConstants.WOLF => new Enemy { Name = "Dire Wolf", Health = 35, MaxHealth = 35, Attack = 12, Defense = 2, Experience = 20, Gold = 5 },
                GameConstants.BANDIT => new Enemy { Name = "Highway Bandit", Health = 30, MaxHealth = 30, Attack = 8, Defense = 2, Experience = 25, Gold = 20 },
                _ => new Enemy { Name = "Strange Creature", Health = 30, MaxHealth = 30, Attack = 8, Defense = 2, Experience = 20, Gold = 10 }
            };

            DisplayMessage($"A {randomEnemy.Name} blocks your path!", Color.Red);
            StartCombat(randomEnemy);
        }

        public void ShowInventory()
        {
            gameForm.DisplayText(GameConstants.INVENTORY_MSG, Color.Yellow);
            if (player.Inventory.Any())
            {
                foreach (var item in player.Inventory)
                {
                    gameForm.DisplayText($"  - {item.Name}: {item.Description}");
                }
            }
            else
            {
                gameForm.DisplayText(GameConstants.INVENTORY_EMPTY_MSG);
            }
            gameForm.DisplayText("");
        }

        public void ShowCharacterStats()
        {
            gameForm.DisplayText(GameConstants.CHARACTER_STATS_MSG, Color.Cyan);
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
                gameForm.DisplayText(GameConstants.NO_SUCH_ITEM_MSG, Color.Red);
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
                    gameForm.DisplayText(GameConstants.NOTHING_TO_ATTACK_MSG, Color.Red);
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
                gameForm.DisplayText(GameConstants.NO_SUCH_ENEMY_MSG, Color.Red);
            }
        }

        private void StartCombat(Enemy enemy)
        {
            isInCombat = true;
            currentEnemy = enemy;
            DisplayMessage($"A {enemy.Name} appears! {GameConstants.COMBAT_BEGIN_MSG}", Color.Red);
            DisplayMessage($"Enemy Health: {enemy.Health}/{enemy.MaxHealth}", Color.Orange);
            DisplayMessage(GameConstants.COMBAT_COMMANDS_MSG, Color.Yellow);
            
            // Set combat mode in status bar
            if (gameForm is Form1 form1)
            {
                form1.SetCombatMode(true);
            }
        }

        private void PerformAttack()
        {
            int damage = Math.Max(1, player.Attack - currentEnemy.Defense + random.Next(-2, 3));
            
            // God mode makes attacks more powerful
            if (godModeEnabled)
            {
                damage *= 10;
            }
            
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
            // God mode prevents all damage
            if (godModeEnabled)
            {
                gameForm.DisplayText($"{currentEnemy.Name} attacks but cannot harm you!", Color.Cyan);
                return;
            }

            int damage = Math.Max(1, currentEnemy.Attack - player.Defense + random.Next(-2, 3));
            player.Health -= damage;
            gameForm.DisplayText($"{currentEnemy.Name} attacks you for {damage} damage!", Color.Red);

            // Infinite health restores health to full
            if (infiniteHealthEnabled)
            {
                player.Health = player.MaxHealth;
            }

            if (player.Health <= 0 && !godModeEnabled && !infiniteHealthEnabled)
            {
                LoseCombat();
            }
        }

        private void AttemptFlee()
        {
            if (random.NextDouble() < GameConstants.FLEE_SUCCESS_RATE)
            {
                gameForm.DisplayText(GameConstants.COMBAT_FLEE_SUCCESS_MSG, Color.Green);
                EndCombat();
            }
            else
            {
                gameForm.DisplayText(GameConstants.COMBAT_FLEE_FAIL_MSG, Color.Red);
                EnemyAttack();
            }
        }

        private void WinCombat()
        {
            int expGained = currentEnemy.Level * 25;
            int goldGained = random.Next(10, 30) * currentEnemy.Level;
            
            DisplayMessage($"{GameConstants.COMBAT_VICTORY_MSG} {currentEnemy.Name}!", Color.Green);
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
            gameForm.DisplayText(GameConstants.COMBAT_DEFEAT_MSG, Color.Red);
            gameForm.DisplayText("You wake up back in the village, wounded but alive.", Color.Yellow);
            
            player.Health = player.MaxHealth / 2;
            currentLocation = locations[GameConstants.DEFAULT_LOCATION];
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

            DisplayMessage($"{GameConstants.LEVEL_UP_MSG} {player.Level}!", Color.Gold);
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
            gameForm.DisplayText(GameConstants.HELP_MSG, Color.Cyan);
            gameForm.DisplayText("Movement: go [direction], north, south, east, west");
            gameForm.DisplayText("Interaction: look, take [item], use [item]");
            gameForm.DisplayText("Character: inventory (inv), stats, skills, help");
            gameForm.DisplayText("Combat: attack [enemy], defend, flee");
            gameForm.DisplayText("Game: save [name], load [name], saves/list, quit");
            gameForm.DisplayText("Special: cheat/cheats - Show cheat code information");
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
            gameForm.DisplayText(GameConstants.CHEAT_ACTIVATION_MSG, Color.Yellow);
            gameForm.DisplayText("To activate cheats, try these classic commands:", Color.Yellow);
            gameForm.DisplayText($"  • {GameConstants.CHEAT_IDDQD} (Doom god mode)", Color.Gray);
            gameForm.DisplayText($"  • {GameConstants.CHEAT_IDKFA} (Doom all weapons)", Color.Gray);
            gameForm.DisplayText($"  • {GameConstants.CHEAT_THEREISNOSPOON} (Matrix reference)", Color.Gray);
            gameForm.DisplayText($"  • Konami Code: {GameConstants.CHEAT_UP} {GameConstants.CHEAT_UP} {GameConstants.CHEAT_DOWN} {GameConstants.CHEAT_DOWN} {GameConstants.CHEAT_LEFT} {GameConstants.CHEAT_RIGHT} {GameConstants.CHEAT_LEFT} {GameConstants.CHEAT_RIGHT} {GameConstants.CHEAT_B} {GameConstants.CHEAT_A}", Color.Gray);
            gameForm.DisplayText("Once activated, type 'cheathelp' for cheat commands!", Color.Cyan);
            gameForm.DisplayText("");
        }

        public void SaveGame(string saveName = null)
        {
            try
            {
                // Generate a timestamped save name if none provided
                if (string.IsNullOrEmpty(saveName))
                {
                    saveName = $"{GameConstants.AUTO_SAVE_PREFIX}{DateTime.Now.ToString(GameConstants.SAVE_TIMESTAMP_FORMAT)}";
                }
                else if (saveName == GameConstants.QUICK_SAVE_COMMAND)
                {
                    saveName = GameConstants.QUICK_SAVE;
                }

                var saveData = new GameSave
                {
                    Player = player,
                    CurrentLocationKey = currentLocation?.Key ?? GameConstants.DEFAULT_LOCATION,
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
            return currentLocation?.Key ?? GameConstants.DEFAULT_LOCATION;
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
            // This method should be removed or updated to use the new architecture
            // For now, show a message that this functionality has moved
            gameForm.DisplayText("Skill tree functionality has been moved to the new UI system.", Color.Yellow);
            gameForm.DisplayText("Use the main form's Character menu to access the skill tree.", Color.Cyan);
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

        // Override gold spending for infinite gold
        public void SpendGold(int amount)
        {
            if (infiniteGoldEnabled)
            {
                DisplayMessage($"Infinite gold enabled - no cost!", Color.Gold);
                return;
            }

            if (player.Gold >= amount)
            {
                player.Gold -= amount;
                UpdateCharacterDisplay();
            }
            else
            {
                DisplayMessage(GameConstants.NOT_ENOUGH_GOLD_MSG, Color.Red);
            }
        }
    }
} 