using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using WinFormsApp1.Constants;
using WinFormsApp1.Events;

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
        private GameEventManager eventManager;
        private GameEventLogger eventLogger;

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
            eventManager = GameEventManager.Instance;
            eventLogger = new GameEventLogger();
            LoadGameData();
        }

        private void LoadGameData()
        {
            try
            {
                dataLoader.LoadAllData();
                locations = dataLoader.LoadLocations();
                eventManager.PublishMessage("Game data loaded successfully from JSON files.", Color.Green, MessageType.System);
            }
            catch (Exception ex)
            {
                eventManager.PublishMessage($"Error loading game data: {ex.Message}", Color.Red, MessageType.Error);
                eventManager.PublishMessage("Using fallback data...", Color.Yellow, MessageType.System);
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
                    
                    eventManager.PublishMessage("=== Welcome to the Realm of Aethermoor ===", Color.Cyan, MessageType.System);
                    eventManager.PublishMessage($"Welcome, {player.Name} the {player.CharacterClass}!", Color.Green, MessageType.System);
                    eventManager.PublishMessage("Your adventure begins in the peaceful village of Elderbrook.", Color.White, MessageType.System);
                    eventManager.PublishMessage("Type 'help' for a list of commands, or 'look' to examine your surroundings.", Color.Yellow, MessageType.System);
                    eventManager.PublishMessage("", Color.White, MessageType.Normal);
                    
                    eventManager.PublishGameStateChange("GameStarted", player);
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

            // Publish command executed event
            eventManager.PublishCommandExecuted(command, args, player);

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
                    eventManager.PublishMessage(GameConstants.UNKNOWN_COMMAND_MSG, Color.Red, MessageType.Error);
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
                eventManager.PublishMessage(GameConstants.CHEAT_ACTIVATED_MSG, Color.Magenta, MessageType.Cheat);
                eventManager.PublishMessage("Type 'cheathelp' for available cheat commands.", Color.Cyan, MessageType.Cheat);
                eventManager.PublishMessage("Note: Using cheats may affect game balance!", Color.Yellow, MessageType.Warning);
                eventManager.PublishCheatActivated("cheats_enabled", new string[0], player);
            }
            else
            {
                eventManager.PublishMessage("Cheats are already enabled!", Color.Cyan, MessageType.Cheat);
            }
        }

        private bool ProcessCheatCommand(string command, string[] args)
        {
            if (player == null && !command.StartsWith(GameConstants.CMD_CHEAT))
            {
                eventManager.PublishMessage(GameConstants.START_GAME_FOR_CHEATS_MSG, Color.Red, MessageType.Error);
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
            eventManager.PublishMessage(GameConstants.CHEAT_HELP_MSG, Color.Magenta, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("🎮 GAME STATE:", Color.Cyan, MessageType.Cheat);
            eventManager.PublishMessage("  godmode/god - Toggle invincibility", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  infinitehealth/infhealth - Toggle infinite health", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  infinitegold/infgold - Toggle infinite gold", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  noclip - Toggle movement restrictions", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("💰 RESOURCES:", Color.Cyan, MessageType.Cheat);
            eventManager.PublishMessage($"  addgold [amount] - Add gold (default: {GameConstants.DEFAULT_GOLD_AMOUNT})", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"  addexp [amount] - Add experience (default: {GameConstants.DEFAULT_EXP_AMOUNT})", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  heal/fullheal - Restore full health", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("📊 CHARACTER:", Color.Cyan, MessageType.Cheat);
            eventManager.PublishMessage($"  levelup [count] - Level up (default: {GameConstants.DEFAULT_LEVEL_UP_COUNT})", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  setlevel [level] - Set specific level", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  maxstats - Maximize all stats", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  setstats [att] [def] [hp] - Set attack/defense/health", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("🎒 INVENTORY:", Color.Cyan, MessageType.Cheat);
            eventManager.PublishMessage("  additem [name] - Add item to inventory", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  clearinventory - Remove all items", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("🗺️ WORLD:", Color.Cyan, MessageType.Cheat);
            eventManager.PublishMessage("  teleport [location] - Travel to location", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  spawnenemy [name] - Spawn enemy in current location", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  killallenemies - Remove all enemies from location", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("🔧 DEBUG:", Color.Cyan, MessageType.Cheat);
            eventManager.PublishMessage("  showdebug - Display debug information", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  resetgame - Reset to initial state", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  disablecheats - Turn off cheat mode", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage($"Available locations: {GameConstants.VILLAGE_LOCATION}, {GameConstants.FOREST_LOCATION}, {GameConstants.PLAINS_LOCATION}, {GameConstants.CAVE_LOCATION}, {GameConstants.RUINS_LOCATION}, {GameConstants.LAIR_LOCATION}", Color.Gray, MessageType.System);
        }

        private void ShowCheatActivationHelp()
        {
            eventManager.PublishMessage(GameConstants.CHEAT_ACTIVATION_MSG, Color.Yellow, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("🎮 How to Activate Cheats:", Color.Cyan, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("Method 1 - Classic Gaming References:", Color.Green, MessageType.Cheat);
            eventManager.PublishMessage($"  Type: {GameConstants.CHEAT_IDDQD}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"  Type: {GameConstants.CHEAT_IDKFA}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"  Type: {GameConstants.CHEAT_THEREISNOSPOON}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("Method 2 - Konami Code Sequence:", Color.Green, MessageType.Cheat);
            eventManager.PublishMessage("  Type these commands in order:", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"  {GameConstants.CHEAT_UP} → {GameConstants.CHEAT_UP} → {GameConstants.CHEAT_DOWN} → {GameConstants.CHEAT_DOWN} → {GameConstants.CHEAT_LEFT} → {GameConstants.CHEAT_RIGHT} → {GameConstants.CHEAT_LEFT} → {GameConstants.CHEAT_RIGHT} → {GameConstants.CHEAT_B} → {GameConstants.CHEAT_A}", Color.Gray, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("Method 3 - Direct Activation:", Color.Green, MessageType.Cheat);
            eventManager.PublishMessage($"  Type: {GameConstants.CHEAT_KONAMI}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("💡 Tips:", Color.Yellow, MessageType.Cheat);
            eventManager.PublishMessage("  • Commands are case-insensitive", Color.Gray, MessageType.Cheat);
            eventManager.PublishMessage("  • Once activated, type 'cheathelp' for all cheat commands", Color.Gray, MessageType.Cheat);
            eventManager.PublishMessage("  • Cheats affect game balance - use responsibly!", Color.Orange, MessageType.Warning);
            eventManager.PublishMessage("  • You can disable cheats anytime with 'disablecheats'", Color.Gray, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
            eventManager.PublishMessage("🎯 Popular Cheat Commands (once activated):", Color.Cyan, MessageType.Cheat);
            eventManager.PublishMessage("  god - Invincibility mode", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  addgold 9999 - Lots of gold", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  maxstats - Maximize all stats", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("  teleport [location] - Instant travel", Color.White, MessageType.Cheat);
            eventManager.PublishMessage("", Color.White, MessageType.Normal);
        }

        private void ToggleGodMode()
        {
            godModeEnabled = !godModeEnabled;
            eventManager.PublishMessage($"God Mode: {(godModeEnabled ? "ENABLED" : "DISABLED")}", 
                godModeEnabled ? Color.Gold : Color.Gray, MessageType.Cheat);
            
            if (godModeEnabled)
            {
                eventManager.PublishMessage("You are now invincible!", Color.Yellow, MessageType.Cheat);
            }
        }

        private void ToggleInfiniteHealth()
        {
            infiniteHealthEnabled = !infiniteHealthEnabled;
            eventManager.PublishMessage($"Infinite Health: {(infiniteHealthEnabled ? "ENABLED" : "DISABLED")}", 
                infiniteHealthEnabled ? Color.Green : Color.Gray, MessageType.Cheat);
        }

        private void ToggleInfiniteGold()
        {
            infiniteGoldEnabled = !infiniteGoldEnabled;
            eventManager.PublishMessage($"Infinite Gold: {(infiniteGoldEnabled ? "ENABLED" : "DISABLED")}", 
                infiniteGoldEnabled ? Color.Gold : Color.Gray, MessageType.Cheat);
        }

        private void AddGold(string[] args)
        {
            int amount = GameConstants.DEFAULT_GOLD_AMOUNT;
            if (args.Length > 0 && int.TryParse(args[0], out int parsedAmount))
            {
                amount = Math.Max(GameConstants.MIN_STAT_VALUE, parsedAmount);
            }

            player.Gold += amount;
            eventManager.PublishMessage($"Added {amount} gold! Total: {player.Gold}", Color.Gold, MessageType.Cheat);
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

            eventManager.PublishMessage($"Added {amount} experience!", Color.Cyan, MessageType.Cheat);
            if (player.Level > oldLevel)
            {
                eventManager.PublishMessage($"Level increased from {oldLevel} to {player.Level}!", Color.Gold, MessageType.Cheat);
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

            eventManager.PublishMessage($"Leveled up {count} times! Level: {oldLevel} → {player.Level}", Color.Gold, MessageType.Cheat);
            UpdateCharacterDisplay();
        }

        private void SetLevel(string[] args)
        {
            if (args.Length == 0 || !int.TryParse(args[0], out int level))
            {
                eventManager.PublishMessage("Usage: setlevel [level]", Color.Red, MessageType.Error);
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

            eventManager.PublishMessage($"Level set to {level}! (was {oldLevel})", Color.Gold, MessageType.Cheat);
            UpdateCharacterDisplay();
        }

        private void FullHeal()
        {
            player.Health = player.MaxHealth;
            eventManager.PublishMessage($"Fully healed! Health: {player.Health}/{player.MaxHealth}", Color.Green, MessageType.Cheat);
            UpdateCharacterDisplay();
        }

        private void GiveItem(string[] args)
        {
            if (args.Length == 0)
            {
                eventManager.PublishMessage("Usage: additem [item name]", Color.Red, MessageType.Error);
                eventManager.PublishMessage("Available items: potion, sword, shield, bow, dagger", Color.Gray, MessageType.System);
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
                eventManager.PublishMessage($"Added {item.Name} to inventory!", Color.Green, MessageType.Cheat);
            }
            else
            {
                eventManager.PublishMessage($"Unknown item: {itemName}", Color.Red, MessageType.Error);
            }
        }

        private void ClearInventory()
        {
            int itemCount = player.Inventory.Count;
            player.Inventory.Clear();
            eventManager.PublishMessage($"Removed {itemCount} items from inventory.", Color.Orange, MessageType.Cheat);
        }

        private void TeleportToLocation(string[] args)
        {
            if (args.Length == 0)
            {
                eventManager.PublishMessage("Usage: teleport [location]", Color.Red, MessageType.Error);
                eventManager.PublishMessage("Available locations: " + string.Join(", ", locations.Keys), Color.Gray, MessageType.System);
                return;
            }

            string locationKey = args[0].ToLower();
            
            if (locations.ContainsKey(locationKey))
            {
                currentLocation = locations[locationKey];
                eventManager.PublishMessage($"⚡ Teleported to {currentLocation.Name}!", Color.Magenta, MessageType.Cheat);
                ShowLocation();
                UpdateCharacterDisplay();
            }
            else
            {
                eventManager.PublishMessage($"Unknown location: {locationKey}", Color.Red, MessageType.Error);
                eventManager.PublishMessage("Available locations: " + string.Join(", ", locations.Keys), Color.Gray, MessageType.System);
            }
        }

        private void ToggleNoClip()
        {
            noClipModeEnabled = !noClipModeEnabled;
            eventManager.PublishMessage($"No-Clip Mode: {(noClipModeEnabled ? "ENABLED" : "DISABLED")}", 
                noClipModeEnabled ? Color.Cyan : Color.Gray, MessageType.Cheat);
            
            if (noClipModeEnabled)
            {
                eventManager.PublishMessage("You can now move to any location without restrictions!", Color.Yellow, MessageType.Cheat);
            }
        }

        private void SpawnEnemy(string[] args)
        {
            if (args.Length == 0)
            {
                eventManager.PublishMessage("Usage: spawnenemy [enemy name]", Color.Red, MessageType.Error);
                eventManager.PublishMessage("Available enemies: goblin, orc, troll, dragon", Color.Gray, MessageType.System);
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
            eventManager.PublishMessage($"Spawned {enemy.Name} in {currentLocation.Name}!", Color.Red, MessageType.Cheat);
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

            eventManager.PublishMessage($"Removed {enemyCount} enemies from {currentLocation.Name}.", Color.Orange, MessageType.Cheat);
        }

        private void MaximizeStats()
        {
            player.Health = player.MaxHealth = GameConstants.MAX_STAT_VALUE;
            player.Attack = GameConstants.MAX_STAT_VALUE / 10; // 999
            player.Defense = GameConstants.MAX_STAT_VALUE / 10; // 999
            player.Gold = GameConstants.MAX_GOLD;
            player.SkillPoints = GameConstants.MAX_SKILL_POINTS;

            eventManager.PublishMessage("All stats maximized!", Color.Gold, MessageType.Cheat);
            eventManager.PublishMessage($"Health: {GameConstants.MAX_STAT_VALUE}, Attack: {GameConstants.MAX_STAT_VALUE / 10}, Defense: {GameConstants.MAX_STAT_VALUE / 10}", Color.Yellow, MessageType.Cheat);
            eventManager.PublishMessage($"Gold: {GameConstants.MAX_GOLD:N0}, Skill Points: {GameConstants.MAX_SKILL_POINTS}", Color.Yellow, MessageType.Cheat);
            UpdateCharacterDisplay();
        }

        private void SetStats(string[] args)
        {
            if (args.Length < 3)
            {
                eventManager.PublishMessage("Usage: setstats [attack] [defense] [health]", Color.Red, MessageType.Error);
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

                eventManager.PublishMessage($"Stats set - Attack: {player.Attack}, Defense: {player.Defense}, Health: {player.Health}", Color.Gold, MessageType.Cheat);
                UpdateCharacterDisplay();
            }
            else
            {
                eventManager.PublishMessage("Invalid numbers provided!", Color.Red, MessageType.Error);
            }
        }

        private void ShowDebugInfo()
        {
            eventManager.PublishMessage(GameConstants.DEBUG_INFO_MSG, Color.Cyan, MessageType.Cheat);
            eventManager.PublishMessage($"Cheats Enabled: {cheatsEnabled}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"God Mode: {godModeEnabled}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"Infinite Health: {infiniteHealthEnabled}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"Infinite Gold: {infiniteGoldEnabled}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"No-Clip Mode: {noClipModeEnabled}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"In Combat: {isInCombat}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"Current Location: {currentLocation?.Key} ({currentLocation?.Name})", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"Player Level: {player?.Level}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"Player Health: {player?.Health}/{player?.MaxHealth}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"Player Gold: {player?.Gold}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"Inventory Items: {player?.Inventory?.Count}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"Location Enemies: {currentLocation?.Enemies?.Count}", Color.White, MessageType.Cheat);
            eventManager.PublishMessage($"Location Items: {currentLocation?.Items?.Count}", Color.White, MessageType.Cheat);
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

                eventManager.PublishMessage("Game state reset to beginning!", Color.Orange, MessageType.Cheat);
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

            eventManager.PublishMessage("Cheat codes disabled. Game balance restored.", Color.Gray, MessageType.Cheat);
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
                    eventManager.PublishMessage(GameConstants.COMBAT_COMMANDS_MSG, Color.Yellow, MessageType.Cheat);
                    break;
            }
        }

        private void ShowLocation()
        {
            eventManager.PublishMessage($"=== {currentLocation.Name} ===", Color.Cyan, MessageType.System);
            eventManager.PublishMessage(currentLocation.Description, null, MessageType.System);
            eventManager.PublishMessage("", null, MessageType.Normal);

            if (currentLocation.Items.Any())
            {
                eventManager.PublishMessage("Items here:", Color.Yellow, MessageType.System);
                foreach (var item in currentLocation.Items)
                {
                    eventManager.PublishMessage($"  - {item.Name}", null, MessageType.System);
                }
                eventManager.PublishMessage("", null, MessageType.Normal);
            }

            if (currentLocation.Enemies.Any())
            {
                eventManager.PublishMessage("Enemies present:", Color.Red, MessageType.System);
                foreach (var enemy in currentLocation.Enemies)
                {
                    eventManager.PublishMessage($"  - {enemy.Name} (Level {enemy.Level})", null, MessageType.System);
                }
                eventManager.PublishMessage("", null, MessageType.Normal);
            }

            if (currentLocation.Exits.Any())
            {
                eventManager.PublishMessage("Exits:", Color.Cyan, MessageType.System);
                foreach (var exit in currentLocation.Exits)
                {
                    eventManager.PublishMessage($"  {exit.Key} - {exit.Value}", null, MessageType.System);
                }
            }
        }

        private void Move(string direction)
        {
            var oldLocation = currentLocation;

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
                            eventManager.PublishMessage($"⚡ No-clip teleport to {currentLocation.Name}!", Color.Magenta, MessageType.Cheat);
                            eventManager.PublishLocationChange(player, oldLocation, currentLocation, "noclip");
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
                    eventManager.PublishLocationChange(player, oldLocation, currentLocation, direction);
                    ShowLocation();
                    CheckRandomEncounter();
                }
            }
            else
            {
                eventManager.PublishMessage(GameConstants.CANT_GO_THAT_WAY_MSG, Color.Red, MessageType.Error);
                if (currentLocation?.Exits?.Any() == true)
                {
                    eventManager.PublishMessage($"Available exits: {string.Join(", ", currentLocation.Exits.Keys)}", Color.Yellow, MessageType.System);
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

            eventManager.PublishMessage($"A {randomEnemy.Name} blocks your path!", Color.Red, MessageType.System);
            StartCombat(randomEnemy);
        }

        public void ShowInventory()
        {
            eventManager.PublishMessage(GameConstants.INVENTORY_MSG, Color.Yellow, MessageType.System);
            if (player.Inventory.Any())
            {
                foreach (var item in player.Inventory)
                {
                    eventManager.PublishMessage($"  - {item.Name}: {item.Description}", null, MessageType.System);
                }
            }
            else
            {
                eventManager.PublishMessage(GameConstants.INVENTORY_EMPTY_MSG, null, MessageType.System);
            }
            eventManager.PublishMessage("", null, MessageType.Normal);
        }

        public void ShowCharacterStats()
        {
            eventManager.PublishMessage(GameConstants.CHARACTER_STATS_MSG, Color.Cyan, MessageType.System);
            eventManager.PublishMessage($"Name: {player.Name}", null, MessageType.System);
            eventManager.PublishMessage($"Class: {player.CharacterClass}", null, MessageType.System);
            eventManager.PublishMessage($"Level: {player.Level}", null, MessageType.System);
            eventManager.PublishMessage($"Experience: {player.Experience}/{player.ExperienceToNextLevel}", null, MessageType.System);
            eventManager.PublishMessage($"Health: {player.Health}/{player.MaxHealth}", null, MessageType.System);
            eventManager.PublishMessage($"Attack: {player.Attack}", null, MessageType.System);
            eventManager.PublishMessage($"Defense: {player.Defense}", null, MessageType.System);
            eventManager.PublishMessage($"Gold: {player.Gold}", null, MessageType.System);
            eventManager.PublishMessage("", null, MessageType.Normal);
        }

        private void TakeItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                eventManager.PublishMessage("Take what?", Color.Yellow, MessageType.System);
                return;
            }

            var item = currentLocation.Items.FirstOrDefault(i => 
                i.Name.ToLower().Contains(itemName.ToLower()));

            if (item != null)
            {
                player.Inventory.Add(item);
                currentLocation.Items.Remove(item);
                eventManager.PublishInventoryChange(player, item, true, 1, currentLocation);
            }
            else
            {
                eventManager.PublishMessage(GameConstants.NO_SUCH_ITEM_MSG, Color.Red, MessageType.Error);
            }
        }

        private void UseItem(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
            {
                eventManager.PublishMessage("Use what?", Color.Yellow, MessageType.System);
                return;
            }

            var item = player.Inventory.FirstOrDefault(i => 
                i.Name.ToLower().Contains(itemName.ToLower()));

            if (item != null)
            {
                switch (item.Type)
                {
                    case ItemType.Potion:
                        int healAmount = Math.Min(item.Value, player.MaxHealth - player.Health);
                        player.Health = Math.Min(player.MaxHealth, player.Health + item.Value);
                        player.Inventory.Remove(item);
                        eventManager.PublishItemUsed(player, item, $"restored {item.Value} health", true);
                        eventManager.PublishHealthChange(player, healAmount, $"used {item.Name}");
                        break;
                    case ItemType.Weapon:
                        // Equip weapon logic would go here
                        eventManager.PublishItemUsed(player, item, "equipped weapon", false);
                        break;
                    default:
                        eventManager.PublishMessage($"You can't use the {item.Name} right now.", Color.Yellow, MessageType.System);
                        break;
                }
            }
            else
            {
                eventManager.PublishMessage("You don't have that item.", Color.Red, MessageType.Error);
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
                    eventManager.PublishMessage(GameConstants.NOTHING_TO_ATTACK_MSG, Color.Red, MessageType.System);
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
                eventManager.PublishMessage(GameConstants.NO_SUCH_ENEMY_MSG, Color.Red, MessageType.Error);
            }
        }

        private void StartCombat(Enemy enemy)
        {
            isInCombat = true;
            currentEnemy = enemy;
            eventManager.PublishCombatStart(player, enemy);
            eventManager.PublishMessage(GameConstants.COMBAT_COMMANDS_MSG, Color.Yellow, MessageType.Combat);
            
            // Set combat mode in status bar
            if (gameForm is Form1 form1)
            {
                form1.SetCombatMode(true);
            }
        }

        private void PerformAttack()
        {
            int damage = Math.Max(1, player.Attack - currentEnemy.Defense + random.Next(-2, 3));
            bool isCritical = random.NextDouble() < 0.1; // 10% critical chance
            
            // God mode makes attacks more powerful
            if (godModeEnabled)
            {
                damage *= 10;
            }

            if (isCritical)
            {
                damage = (int)(damage * 1.5);
            }
            
            currentEnemy.Health -= damage;
            eventManager.PublishCombatAction(player, currentEnemy, "attack", damage, true, isCritical);

            if (currentEnemy.Health <= 0)
            {
                WinCombat();
                return;
            }

            EnemyAttack();
        }

        private void PerformDefend()
        {
            eventManager.PublishMessage("You raise your guard, reducing incoming damage.", Color.Cyan, MessageType.System);
            // Reduce enemy damage by half this turn
            int damage = Math.Max(1, (currentEnemy.Attack - player.Defense) / 2 + random.Next(-1, 2));
            player.Health -= damage;
            eventManager.PublishCombatAction(player, currentEnemy, "defends", damage, false);
            eventManager.PublishHealthChange(player, -damage, $"{currentEnemy.Name} defense");

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
                eventManager.PublishMessage($"{currentEnemy.Name} attacks but cannot harm you!", Color.Cyan, MessageType.Combat);
                return;
            }

            int damage = Math.Max(1, currentEnemy.Attack - player.Defense + random.Next(-2, 3));
            player.Health -= damage;
            eventManager.PublishCombatAction(player, currentEnemy, "attacks", damage, false);
            eventManager.PublishHealthChange(player, -damage, $"{currentEnemy.Name} attack");

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
                eventManager.PublishMessage(GameConstants.COMBAT_FLEE_SUCCESS_MSG, Color.Green, MessageType.System);
                EndCombat();
            }
            else
            {
                eventManager.PublishMessage(GameConstants.COMBAT_FLEE_FAIL_MSG, Color.Red, MessageType.System);
                EnemyAttack();
            }
        }

        private void WinCombat()
        {
            int expGained = currentEnemy.Experience;
            int goldGained = currentEnemy.Gold;
            var lootGained = new List<Item>(currentEnemy.LootTable);
            
            // Add experience and gold
            player.Experience += expGained;
            player.Gold += goldGained;

            // Add loot to inventory
            foreach (var item in lootGained)
            {
                player.Inventory.Add(item);
            }

            // Check for level up
            bool leveledUp = false;
            while (player.Experience >= player.ExperienceToNextLevel)
            {
                player.Experience -= player.ExperienceToNextLevel;
                LevelUp();
                leveledUp = true;
            }

            // Publish events
            eventManager.PublishCombatEnd(player, currentEnemy, true, expGained, goldGained, lootGained);
            eventManager.PublishExperienceGain(player, expGained, leveledUp);
            eventManager.PublishGoldChange(player, goldGained, "combat victory");
            
            foreach (var item in lootGained)
            {
                eventManager.PublishInventoryChange(player, item, true);
            }
            
            EndCombat();
            UpdateCharacterDisplay();
        }

        private void LoseCombat()
        {
            eventManager.PublishCombatEnd(player, currentEnemy, false);
            eventManager.PublishMessage("You wake up back in the village, wounded but alive.", Color.Yellow, MessageType.System);
            
            player.Health = player.MaxHealth / 2;
            var oldLocation = currentLocation;
            currentLocation = locations[GameConstants.DEFAULT_LOCATION];
            eventManager.PublishLocationChange(player, oldLocation, currentLocation, "defeat");
            eventManager.PublishHealthChange(player, player.Health - player.MaxHealth, "combat defeat");
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
            
            int healthGain = 10;
            int attackGain = 2;
            int defenseGain = 1;
            int skillPointsGain = 5;
            
            player.MaxHealth += healthGain;
            player.Health = player.MaxHealth; // Full heal on level up
            player.Attack += attackGain;
            player.Defense += defenseGain;
            player.SkillPoints += skillPointsGain;
            player.ExperienceToNextLevel = player.Level * 100;

            eventManager.PublishPlayerLevelUp(player, oldLevel, player.Level, healthGain, attackGain, defenseGain, skillPointsGain);
            eventManager.PublishHealthChange(player, player.MaxHealth, "level up full heal");

            UpdateCharacterDisplay();
        }

        public void ShowHelp()
        {
            eventManager.PublishMessage(GameConstants.HELP_MSG, Color.Cyan, MessageType.System);
            eventManager.PublishMessage("Movement: go [direction], north, south, east, west", null, MessageType.System);
            eventManager.PublishMessage("Interaction: look, take [item], use [item]", null, MessageType.System);
            eventManager.PublishMessage("Character: inventory (inv), stats, skills, help", null, MessageType.System);
            eventManager.PublishMessage("Combat: attack [enemy], defend, flee", null, MessageType.System);
            eventManager.PublishMessage("Game: save [name], load [name], saves/list, quit", null, MessageType.System);
            eventManager.PublishMessage("Special: cheat/cheats - Show cheat code information", null, MessageType.System);
            eventManager.PublishMessage("", null, MessageType.Normal);
            eventManager.PublishMessage("Save Commands:", null, MessageType.System);
            eventManager.PublishMessage("  save - Quick save with timestamp", null, MessageType.System);
            eventManager.PublishMessage("  save [name] - Save with custom name", null, MessageType.System);
            eventManager.PublishMessage("  load - Load quick save", null, MessageType.System);
            eventManager.PublishMessage("  load [name] - Load specific save", null, MessageType.System);
            eventManager.PublishMessage("  saves/list - Show all available saves", null, MessageType.System);
            eventManager.PublishMessage("", null, MessageType.Normal);
            eventManager.PublishMessage("Character Development:", null, MessageType.System);
            eventManager.PublishMessage("  skills - Open skill tree to learn new abilities", null, MessageType.System);
            eventManager.PublishMessage("", null, MessageType.Normal);
            eventManager.PublishMessage(GameConstants.CHEAT_ACTIVATION_MSG, Color.Yellow, MessageType.Cheat);
            eventManager.PublishMessage("To activate cheats, try these classic commands:", Color.Yellow, MessageType.Cheat);
            eventManager.PublishMessage($"  • {GameConstants.CHEAT_IDDQD} (Doom god mode)", Color.Gray, MessageType.Cheat);
            eventManager.PublishMessage($"  • {GameConstants.CHEAT_IDKFA} (Doom all weapons)", Color.Gray, MessageType.Cheat);
            eventManager.PublishMessage($"  • {GameConstants.CHEAT_THEREISNOSPOON} (Matrix reference)", Color.Gray, MessageType.Cheat);
            eventManager.PublishMessage($"  • Konami Code: {GameConstants.CHEAT_UP} {GameConstants.CHEAT_UP} {GameConstants.CHEAT_DOWN} {GameConstants.CHEAT_DOWN} {GameConstants.CHEAT_LEFT} {GameConstants.CHEAT_RIGHT} {GameConstants.CHEAT_LEFT} {GameConstants.CHEAT_RIGHT} {GameConstants.CHEAT_B} {GameConstants.CHEAT_A}", Color.Gray, MessageType.Cheat);
            eventManager.PublishMessage("Once activated, type 'cheathelp' for cheat commands!", Color.Cyan, MessageType.Cheat);
            eventManager.PublishMessage("", null, MessageType.Normal);
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
                
                eventManager.PublishGameSaved(saveName, saveFilePath, true);
                HasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                eventManager.PublishGameSaved(saveName ?? "unknown", "", false, ex.Message);
            }
        }

        public void LoadGame(string saveName = null)
        {
            try
            {
                string saveDirectory = GetSaveDirectory();
                if (!Directory.Exists(saveDirectory))
                {
                    eventManager.PublishGameLoaded(saveName ?? "unknown", "", false, null, "No saved games found.");
                    return;
                }

                var saveFiles = Directory.GetFiles(saveDirectory, "*.json")
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .ToArray();

                if (saveFiles.Length == 0)
                {
                    eventManager.PublishGameLoaded(saveName ?? "unknown", "", false, null, "No saved games found.");
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
                    eventManager.PublishGameLoaded(saveName, filePath, false, null, $"Save file '{saveName}' not found.");
                    return;
                }

                string jsonData = File.ReadAllText(filePath);
                var saveData = JsonSerializer.Deserialize<GameSave>(jsonData);

                // Restore game state
                player = saveData.Player;
                currentLocation = locations[saveData.CurrentLocationKey];
                
                // Enable game controls now that we have a loaded player
                gameForm.EnableGameControls(true);
                
                eventManager.PublishGameLoaded(saveName, filePath, true, player);
                ShowLocation();
                UpdateCharacterDisplay();
                HasUnsavedChanges = false;
            }
            catch (Exception ex)
            {
                eventManager.PublishGameLoaded(saveName ?? "unknown", "", false, null, ex.Message);
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
            // Update status through the UI form directly since we have a reference
            if (gameForm != null && player != null)
            {
                string statusText = $"Health: {player.Health}/{player.MaxHealth} | Level: {player.Level} | Gold: {player.Gold}";
                gameForm.UpdateStatus(statusText);
            }
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
                    eventManager.PublishMessage("=== Available Save Files ===", Color.Cyan, MessageType.System);
                    eventManager.PublishMessage($"Save directory: {saveDir}", Color.Gray, MessageType.System);
                    eventManager.PublishMessage("", null, MessageType.Normal);

                    foreach (var saveFile in saveFiles)
                    {
                        var filePath = Path.Combine(saveDir, $"{saveFile}.json");
                        var lastModified = File.GetLastWriteTime(filePath);
                        var fileSize = new FileInfo(filePath).Length;
                        
                        eventManager.PublishMessage($"• {saveFile}", Color.Yellow, MessageType.System);
                        eventManager.PublishMessage($"  Modified: {lastModified:yyyy-MM-dd HH:mm:ss}", null, MessageType.System);
                        eventManager.PublishMessage($"  Size: {fileSize:N0} bytes", null, MessageType.System);
                        eventManager.PublishMessage("", null, MessageType.Normal);
                    }
                    
                    eventManager.PublishMessage("Use 'load [filename]' to load a specific save.", Color.Cyan, MessageType.System);
                }
                else
                {
                    eventManager.PublishMessage("No save files found.", Color.Yellow, MessageType.System);
                    eventManager.PublishMessage($"Save directory: {saveDir}", Color.Gray, MessageType.System);
                }
            }
            catch (Exception ex)
            {
                eventManager.PublishMessage($"Failed to list save files: {ex.Message}", Color.Red, MessageType.Error);
            }
        }

        public void ShowSkillTree()
        {
            if (player == null)
            {
                eventManager.PublishMessage("Start a new game first!", Color.Red, MessageType.Error);
                return;
            }

            try
            {
                var skillTreeForm = new SkillTreeForm(player, this);
                skillTreeForm.ShowDialog();
            }
            catch (Exception ex)
            {
                eventManager.PublishMessage($"Error opening skill tree: {ex.Message}", Color.Red, MessageType.Error);
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
            eventManager.PublishMessage(message, color, MessageType.Normal);
        }

        // Override gold spending for infinite gold
        public void SpendGold(int amount)
        {
            if (infiniteGoldEnabled)
            {
                eventManager.PublishMessage($"Infinite gold enabled - no cost!", Color.Gold, MessageType.Cheat);
                return;
            }

            if (player.Gold >= amount)
            {
                player.Gold -= amount;
                eventManager.PublishGoldChange(player, -amount, "purchase");
                UpdateCharacterDisplay();
            }
            else
            {
                eventManager.PublishMessage(GameConstants.NOT_ENOUGH_GOLD_MSG, Color.Red, MessageType.Error);
            }
        }
    }
} 