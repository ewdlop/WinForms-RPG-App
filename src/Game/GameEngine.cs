using System.Text.Json;
using WinFormsApp1.Constants;
using WinFormsApp1.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace WinFormsApp1
{
    public class GameEngine
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventManager _eventManager;
        private readonly IGameManager _gameManager;
        private readonly IPlayerManager _playerManager;
        private readonly ICombatManager _combatManager;
        private readonly IInventoryManager _inventoryManager;
        private readonly ILocationManager _locationManager;
        private readonly ISkillManager _skillManager;
        private readonly ILogger<GameEngine> _logger;
        
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

        // Legacy properties for backward compatibility during transition
        public bool HasUnsavedChanges => _gameManager?.HasUnsavedChanges ?? false;
        public Player GetPlayer() => _playerManager?.CurrentPlayer;
        public Dictionary<string, Location> GetLocations() => _locationManager?.AllLocations ?? new Dictionary<string, Location>();
        public string GetCurrentLocationKey() => _locationManager?.CurrentLocationKey ?? "";

        public GameEngine(Form1 form, IServiceProvider serviceProvider)
        {
            gameForm = form;
            _serviceProvider = serviceProvider;
            
            // Get required services
            _eventManager = serviceProvider.GetRequiredService<IEventManager>();
            _gameManager = serviceProvider.GetRequiredService<IGameManager>();
            _playerManager = serviceProvider.GetRequiredService<IPlayerManager>();
            _combatManager = serviceProvider.GetRequiredService<ICombatManager>();
            _inventoryManager = serviceProvider.GetRequiredService<IInventoryManager>();
            _locationManager = serviceProvider.GetRequiredService<ILocationManager>();
            _skillManager = serviceProvider.GetRequiredService<ISkillManager>();
            _logger = serviceProvider.GetRequiredService<ILogger<GameEngine>>();

            _logger.LogInformation("GameEngine initialized with dependency injection");

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
                    var player = characterDialog.CreatedPlayer;
                    _gameManager.StartNewGame(player);
                }
            }
        }

        public void ProcessCommand(string input)
        {
            _gameManager?.ProcessCommand(input);
        }

        public void ShowInventory()
        {
            try
            {
                var inventoryForm = _serviceProvider.GetRequiredService<InventoryForm>();
                inventoryForm.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing inventory form");
                DisplayMessage("Error opening inventory. Please start a new game first.", Color.Red);
            }
        }

        public void ShowCharacterStats()
        {
            var player = _playerManager?.CurrentPlayer;
            if (player != null)
            {
                var statsMessage = $"=== Character Stats ===\n" +
                                 $"Name: {player.Name}\n" +
                                 $"Class: {player.CharacterClass}\n" +
                                 $"Level: {player.Level}\n" +
                                 $"Health: {player.Health}/{player.MaxHealth}\n" +
                                 $"Attack: {player.Attack}\n" +
                                 $"Defense: {player.Defense}\n" +
                                 $"Experience: {player.Experience}/{player.ExperienceToNextLevel}\n" +
                                 $"Gold: {player.Gold}\n" +
                                 $"Skill Points: {player.SkillPoints}";
                
                DisplayMessage(statsMessage, Color.Cyan);
            }
            else
            {
                DisplayMessage("No character loaded.", Color.Red);
            }
        }

        public void ShowHelp()
        {
            var helpText = "=== Game Commands ===\n" +
                          "Movement: north, south, east, west, go [direction]\n" +
                          "Actions: look, take [item], use [item], attack [enemy]\n" +
                          "Character: stats, inventory, skills\n" +
                          "Combat: attack, defend, flee\n" +
                          "Game: save, load, help, quit\n" +
                          "\n" +
                          "=== Keyboard Shortcuts ===\n" +
                          "Ctrl+S: Save Game\n" +
                          "Ctrl+L: Load Game\n" +
                          "F1: Show Help\n" +
                          "Tab: Open Inventory";
            
            DisplayMessage(helpText, Color.Yellow);
        }

        public void SaveGame(string saveName = null)
        {
            _gameManager?.SaveGame(saveName != null ? int.Parse(saveName) : 1);
        }

        public void LoadGame(string saveName = null)
        {
            _gameManager?.LoadGame(saveName != null ? int.Parse(saveName) : 1);
        }

        public void ListSaveFiles()
        {
            // Delegate to game manager or provide migration message
            DisplayMessage("Save file listing has been moved to the new save system.", Color.Cyan);
            DisplayMessage("Use the File menu to access save/load functionality.", Color.Yellow);
        }

        public void ShowSkillTree()
        {
            try
            {
                var skillTreeForm = _serviceProvider.GetRequiredService<SkillTreeForm>();
                skillTreeForm.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error showing skill tree form");
                DisplayMessage("Error opening skill tree. Please start a new game first.", Color.Red);
            }
        }

        public void UpdateCharacterDisplay()
        {
            // This is now handled by events - provide migration message
            DisplayMessage("Character display updates are now handled automatically through events.", Color.Cyan);
        }

        public void DisplayMessage(string message, Color? color = null)
        {
            gameForm?.DisplayText(message, color);
        }

        public void SpendGold(int amount)
        {
            // Delegate to player manager
            var player = _playerManager?.CurrentPlayer;
            if (player != null && player.Gold >= amount)
            {
                _playerManager.ModifyGold(-amount, "Purchase");
            }
        }

        // Legacy methods for backward compatibility during transition
        private void UpdateStatusBar()
        {
            // Status bar updates are now handled by events
            _logger.LogDebug("UpdateStatusBar called - now handled by events");
        }
    }
} 