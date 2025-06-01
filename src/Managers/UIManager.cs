using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using WinFormsApp1.Events;
using WinFormsApp1.Interfaces;

namespace WinFormsApp1.Managers
{
    /// <summary>
    /// Manages UI operations through events
    /// </summary>
    public class UIManager : BaseManager, IUIManager
    {
        private readonly IPlayerManager _playerManager;
        private readonly ILocationManager _locationManager;
        private readonly Dictionary<string, string> _pendingInputRequests;

        public override string ManagerName => "UIManager";

        public UIManager(IEventManager eventManager, IPlayerManager playerManager, ILocationManager locationManager) : base(eventManager)
        {
            _playerManager = playerManager ?? throw new ArgumentNullException(nameof(playerManager));
            _locationManager = locationManager ?? throw new ArgumentNullException(nameof(locationManager));
            _pendingInputRequests = new Dictionary<string, string>();
        }

        protected override void SubscribeToEvents()
        {
            // Subscribe to events that should trigger UI updates
            EventManager.Subscribe<GameStartedEvent>(OnGameStarted);
            EventManager.Subscribe<GameEndedEvent>(OnGameEnded);
            EventManager.Subscribe<PlayerLeveledUpEvent>(OnPlayerLeveledUp);
            EventManager.Subscribe<CombatStartedEvent>(OnCombatStarted);
            EventManager.Subscribe<CombatEndedEvent>(OnCombatEnded);
            EventManager.Subscribe<LocationChangedEvent>(OnLocationChanged);
            EventManager.Subscribe<UserInputResponseEvent>(OnUserInputResponse);
        }

        protected override void UnsubscribeFromEvents()
        {
            EventManager.ClearSubscriptions<GameStartedEvent>();
            EventManager.ClearSubscriptions<GameEndedEvent>();
            EventManager.ClearSubscriptions<PlayerLeveledUpEvent>();
            EventManager.ClearSubscriptions<CombatStartedEvent>();
            EventManager.ClearSubscriptions<CombatEndedEvent>();
            EventManager.ClearSubscriptions<LocationChangedEvent>();
            EventManager.ClearSubscriptions<UserInputResponseEvent>();
        }

        public void DisplayText(string text, Color? color = null, bool isImportant = false, string category = "General")
        {
            EventManager.Publish(new DisplayTextEvent(text, color, isImportant, category));
        }

        public void DisplayFormattedMessage(string title, List<FormattedTextLine> lines, bool clearPrevious = false)
        {
            EventManager.Publish(new DisplayFormattedMessageEvent(title, lines, clearPrevious));
        }

        public void ShowNotification(string title, string message, NotificationType type = NotificationType.Info, int duration = 3000, bool requiresUserAction = false)
        {
            EventManager.Publish(new NotificationEvent(title, message, type, duration, requiresUserAction));
        }

        public void UpdateProgress(string progressBarName, int currentValue, int maxValue, string label = "", Color? barColor = null)
        {
            EventManager.Publish(new ProgressUpdateEvent(progressBarName, currentValue, maxValue, label, barColor));
        }

        public void UpdateControlState(string controlName, bool? isEnabled = null, bool? isVisible = null, string newText = null, object newValue = null)
        {
            EventManager.Publish(new UIControlStateChangedEvent(controlName, isEnabled, isVisible, newText, newValue));
        }

        public void UpdateMenuState(string menuName, Dictionary<string, bool> menuItemStates, bool isMenuEnabled = true)
        {
            EventManager.Publish(new MenuStateChangedEvent(menuName, menuItemStates, isMenuEnabled));
        }

        public string RequestUserInput(string prompt, string defaultValue = "", bool isPassword = false, List<string> validOptions = null)
        {
            var requestId = Guid.NewGuid().ToString();
            _pendingInputRequests[requestId] = prompt;
            EventManager.Publish(new UserInputRequestEvent(prompt, defaultValue, isPassword, validOptions, requestId));
            return requestId;
        }

        public void ChangeTheme(string themeName, Dictionary<string, Color> colorScheme, Dictionary<string, object> themeSettings = null)
        {
            EventManager.Publish(new ThemeChangedEvent(themeName, colorScheme, themeSettings));
        }

        public void SetGameControlsEnabled(bool enabled)
        {
            var menuStates = new Dictionary<string, bool>
            {
                ["Game"] = enabled,
                ["Character"] = enabled,
                ["Tools"] = enabled,
                ["Advanced"] = enabled
            };
            UpdateMenuState("MainMenu", menuStates, enabled);
            UpdateControlState("GameControls", enabled);
        }

        public void ClearDisplay()
        {
            DisplayText("", null, false, "Clear");
        }

        public void ShowHelp(string helpCategory = "general")
        {
            var helpLines = new List<FormattedTextLine>();

            switch (helpCategory.ToLowerInvariant())
            {
                case "general":
                    helpLines.AddRange(GetGeneralHelpLines());
                    break;
                case "combat":
                    helpLines.AddRange(GetCombatHelpLines());
                    break;
                case "advanced":
                    helpLines.AddRange(GetAdvancedHelpLines());
                    break;
                default:
                    helpLines.Add(new FormattedTextLine($"Unknown help category: {helpCategory}", Color.Red));
                    helpLines.AddRange(GetGeneralHelpLines());
                    break;
            }

            DisplayFormattedMessage("Help", helpLines);
        }

        public void ShowCharacterStats()
        {
            var player = _playerManager.CurrentPlayer;
            if (player == null)
            {
                DisplayText("No character loaded.", Color.Red, true);
                return;
            }

            var statsLines = new List<FormattedTextLine>
            {
                new FormattedTextLine($"Name: {player.Name}", Color.Cyan, true),
                new FormattedTextLine($"Class: {player.CharacterClass}", Color.Yellow),
                new FormattedTextLine($"Level: {player.Level}", Color.White),
                new FormattedTextLine($"Health: {player.Health}/{player.MaxHealth}", Color.Green),
                new FormattedTextLine($"Attack: {player.Attack}", Color.Red),
                new FormattedTextLine($"Defense: {player.Defense}", Color.Blue),
                new FormattedTextLine($"Experience: {player.Experience}", Color.Magenta),
                new FormattedTextLine($"Gold: {player.Gold}", Color.Gold),
                new FormattedTextLine($"Skill Points: {player.SkillPoints}", Color.Purple)
            };

            if (player.EquippedWeapon != null)
                statsLines.Add(new FormattedTextLine($"Weapon: {player.EquippedWeapon.Name}", Color.Orange));
            if (player.EquippedArmor != null)
                statsLines.Add(new FormattedTextLine($"Armor: {player.EquippedArmor.Name}", Color.Orange));

            DisplayFormattedMessage("Character Statistics", statsLines);
        }

        public void ShowLocationInfo()
        {
            var location = _locationManager.CurrentLocation;
            if (location == null)
            {
                DisplayText("No current location.", Color.Red, true);
                return;
            }

            var locationLines = new List<FormattedTextLine>
            {
                new FormattedTextLine($"=== {location.Name} ===", Color.Magenta, true),
                new FormattedTextLine(location.Description, Color.White),
                new FormattedTextLine("", Color.White)
            };

            // Add exits
            var exits = _locationManager.GetAvailableExits();
            if (exits.Any())
            {
                locationLines.Add(new FormattedTextLine("Available exits:", Color.Gray));
                foreach (var exit in exits)
                {
                    locationLines.Add(new FormattedTextLine($"  {exit.Key} → {exit.Value}", Color.Gray));
                }
                locationLines.Add(new FormattedTextLine("", Color.White));
            }

            // Add items
            if (location.Items?.Any() == true)
            {
                locationLines.Add(new FormattedTextLine($"Items here: {string.Join(", ", location.Items.Select(i => i.Name))}", Color.Yellow));
            }

            // Add NPCs
            if (location.NPCs?.Any() == true)
            {
                locationLines.Add(new FormattedTextLine($"People here: {string.Join(", ", location.NPCs)}", Color.Cyan));
            }

            DisplayFormattedMessage("Location Information", locationLines);
        }

        // Event handlers
        private void OnGameStarted(GameStartedEvent e)
        {
            var welcomeLines = new List<FormattedTextLine>
            {
                new FormattedTextLine("=== Welcome to the Realm of Aethermoor ===", Color.Green, true),
                new FormattedTextLine($"Welcome, {e.Player.Name} the {e.Player.CharacterClass}!", Color.Cyan),
                new FormattedTextLine("Your adventure begins! Type 'help' for commands or 'look' to examine your surroundings.", Color.White)
            };

            DisplayFormattedMessage("Game Started", welcomeLines);
            SetGameControlsEnabled(true);
        }

        private void OnGameEnded(GameEndedEvent e)
        {
            var endLines = new List<FormattedTextLine>
            {
                new FormattedTextLine("=== Game Ended ===", Color.Red, true),
                new FormattedTextLine(e.Reason, Color.Yellow)
            };

            DisplayFormattedMessage("Game Over", endLines);
            SetGameControlsEnabled(false);
        }

        private void OnPlayerLeveledUp(PlayerLeveledUpEvent e)
        {
            var levelUpLines = new List<FormattedTextLine>
            {
                new FormattedTextLine("", Color.White),
                new FormattedTextLine("*** LEVEL UP! ***", Color.Gold, true),
                new FormattedTextLine($"Congratulations! You reached level {e.NewLevel}!", Color.Gold),
                new FormattedTextLine($"You gained {e.SkillPointsGained} skill points!", Color.Magenta),
                new FormattedTextLine("", Color.White)
            };

            DisplayFormattedMessage("Level Up!", levelUpLines);
            ShowNotification("Level Up!", $"You reached level {e.NewLevel}!", NotificationType.Success, 5000);
        }

        private void OnCombatStarted(CombatStartedEvent e)
        {
            var combatLines = new List<FormattedTextLine>
            {
                new FormattedTextLine("", Color.White),
                new FormattedTextLine("*** COMBAT STARTED ***", Color.Red, true),
                new FormattedTextLine($"You are fighting: {e.Enemy.Name} (Level {e.Enemy.Level})", Color.Red),
                new FormattedTextLine($"Enemy Health: {e.Enemy.Health}/{e.Enemy.MaxHealth}", Color.Orange),
                new FormattedTextLine("Commands: attack, defend, flee", Color.Yellow),
                new FormattedTextLine("", Color.White)
            };

            DisplayFormattedMessage("Combat!", combatLines);
            
            // Update menu states for combat
            var combatMenuStates = new Dictionary<string, bool>
            {
                ["NewGame"] = false,
                ["LoadGame"] = false,
                ["SaveGame"] = true
            };
            UpdateMenuState("GameMenu", combatMenuStates);
        }

        private void OnCombatEnded(CombatEndedEvent e)
        {
            var combatEndLines = new List<FormattedTextLine>
            {
                new FormattedTextLine("", Color.White),
                new FormattedTextLine("*** COMBAT ENDED ***", Color.Green, true)
            };

            switch (e.Result)
            {
                case CombatResult.Victory:
                    combatEndLines.Add(new FormattedTextLine($"Victory! You defeated {e.Enemy.Name}!", Color.Green));
                    if (e.ExperienceGained > 0)
                        combatEndLines.Add(new FormattedTextLine($"Experience gained: +{e.ExperienceGained}", Color.Cyan));
                    if (e.GoldGained > 0)
                        combatEndLines.Add(new FormattedTextLine($"Gold gained: +{e.GoldGained}", Color.Yellow));
                    if (e.LootGained?.Any() == true)
                        combatEndLines.Add(new FormattedTextLine($"Loot found: {string.Join(", ", e.LootGained.Select(i => i.Name))}", Color.Magenta));
                    break;
                case CombatResult.Defeat:
                    combatEndLines.Add(new FormattedTextLine($"You were defeated by {e.Enemy.Name}!", Color.Red));
                    combatEndLines.Add(new FormattedTextLine("You have been defeated in combat.", Color.Orange));
                    break;
                case CombatResult.Fled:
                    combatEndLines.Add(new FormattedTextLine($"You successfully fled from {e.Enemy.Name}!", Color.Yellow));
                    break;
            }

            combatEndLines.Add(new FormattedTextLine("", Color.White));
            DisplayFormattedMessage("Combat Result", combatEndLines);

            // Restore normal menu states
            var normalMenuStates = new Dictionary<string, bool>
            {
                ["NewGame"] = true,
                ["LoadGame"] = true,
                ["SaveGame"] = true
            };
            UpdateMenuState("GameMenu", normalMenuStates);
        }

        private void OnLocationChanged(LocationChangedEvent e)
        {
            ShowLocationInfo();
            if (e.IsFirstVisit)
            {
                DisplayText("(First time visiting this location!)", Color.Green, true);
            }
        }

        private void OnUserInputResponse(UserInputResponseEvent e)
        {
            if (_pendingInputRequests.ContainsKey(e.RequestId))
            {
                _pendingInputRequests.Remove(e.RequestId);
                LogMessage($"Received user input response for request: {e.RequestId}");
            }
        }

        // Helper methods for help content
        private List<FormattedTextLine> GetGeneralHelpLines()
        {
            return new List<FormattedTextLine>
            {
                new FormattedTextLine("=== Game Commands ===", Color.Cyan, true),
                new FormattedTextLine("Movement: north, south, east, west, go [direction]", Color.White),
                new FormattedTextLine("Actions: look, take [item], use [item], attack [enemy]", Color.White),
                new FormattedTextLine("Character: stats, inventory, skills", Color.White),
                new FormattedTextLine("Combat: attack, defend, flee", Color.White),
                new FormattedTextLine("Game: save, load, help, quit", Color.White),
                new FormattedTextLine("", Color.White),
                new FormattedTextLine("=== Advanced Commands ===", Color.Yellow, true),
                new FormattedTextLine("auto-explore: Automatically explore random directions", Color.White),
                new FormattedTextLine("optimize-character: Auto-equip best items", Color.White),
                new FormattedTextLine("batch-use <type>: Use all items of specified type", Color.White),
                new FormattedTextLine("skill-combo: Execute multiple skills in sequence", Color.White)
            };
        }

        private List<FormattedTextLine> GetCombatHelpLines()
        {
            return new List<FormattedTextLine>
            {
                new FormattedTextLine("=== Combat Commands ===", Color.Red, true),
                new FormattedTextLine("attack: Attack the enemy", Color.White),
                new FormattedTextLine("defend: Defend to reduce incoming damage", Color.White),
                new FormattedTextLine("flee: Attempt to escape from combat", Color.White),
                new FormattedTextLine("use [item]: Use an item during combat", Color.White),
                new FormattedTextLine("", Color.White),
                new FormattedTextLine("=== Combat Tips ===", Color.Yellow, true),
                new FormattedTextLine("• Defending reduces damage by 50%", Color.White),
                new FormattedTextLine("• Some enemies are stronger than others", Color.White),
                new FormattedTextLine("• Use potions to heal during combat", Color.White),
                new FormattedTextLine("• Fleeing has a chance to fail", Color.White)
            };
        }

        private List<FormattedTextLine> GetAdvancedHelpLines()
        {
            return new List<FormattedTextLine>
            {
                new FormattedTextLine("=== Advanced Features ===", Color.Purple, true),
                new FormattedTextLine("GameCoordinatorService Commands:", Color.Cyan, true),
                new FormattedTextLine("auto-explore: Intelligent exploration with encounter handling", Color.White),
                new FormattedTextLine("optimize-character: Automatic equipment optimization", Color.White),
                new FormattedTextLine("batch-use <type>: Bulk item usage by type", Color.White),
                new FormattedTextLine("skill-combo <skill1> <skill2>: Execute skill combinations", Color.White),
                new FormattedTextLine("", Color.White),
                new FormattedTextLine("Advanced Menu Options:", Color.Cyan, true),
                new FormattedTextLine("• Validate Game State: Check for data consistency", Color.White),
                new FormattedTextLine("• Comprehensive Status: View detailed system info", Color.White),
                new FormattedTextLine("• Synchronize Managers: Ensure system sync", Color.White),
                new FormattedTextLine("• Perform Maintenance: Automated cleanup", Color.White)
            };
        }
    }
} 