using WinFormsApp1.Controls;
using WinFormsApp1.Interfaces;
using WinFormsApp1.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // Replace individual controls with custom controls
        private GameTextDisplayControl gameTextDisplayControl;
        private GameInputControl gameInputControl;
        private MenuStrip menuStrip;
        private GameStatusBarControl gameStatusBarControl;
        private ToolStrip toolStrip;
        private Panel sidePanel;
        private CharacterStatsControl characterStatsControl;
        private ProgressDisplayControl progressDisplayControl;
        private QuickActionsControl quickActionsControl;
        private MiniMapControl miniMapControl;

        // Collections for enabling/disabling game controls
        private List<Control> gameRequiredControls;
        private List<ToolStripItem> gameRequiredMenuItems;
        private List<ToolStripItem> gameRequiredToolbarItems;

        // Manager dependencies
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventManager _eventManager;
        private readonly IGameManager _gameManager;
        private readonly IPlayerManager _playerManager;
        private readonly ICombatManager _combatManager;
        private readonly IInventoryManager _inventoryManager;
        private readonly ILocationManager _locationManager;
        private readonly ISkillManager _skillManager;
        private readonly IGameCoordinatorService _gameCoordinator;
        private readonly ILogger<Form1> _logger;

        public Form1(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            // Get required services
            _eventManager = serviceProvider.GetRequiredService<IEventManager>();
            _gameManager = serviceProvider.GetRequiredService<IGameManager>();
            _playerManager = serviceProvider.GetRequiredService<IPlayerManager>();
            _combatManager = serviceProvider.GetRequiredService<ICombatManager>();
            _inventoryManager = serviceProvider.GetRequiredService<IInventoryManager>();
            _locationManager = serviceProvider.GetRequiredService<ILocationManager>();
            _skillManager = serviceProvider.GetRequiredService<ISkillManager>();
            _gameCoordinator = serviceProvider.GetRequiredService<IGameCoordinatorService>();
            _logger = serviceProvider.GetRequiredService<ILogger<Form1>>();

            _logger.LogInformation("Initializing Form1 with dependency injection and advanced services");

            InitializeComponent();
            InitializeGameUI(); // Initialize the UI before subscribing to events
            SubscribeToEvents();
            SetGameControlsEnabled(false); // Disable until game starts

            _logger.LogInformation("Form1 initialized successfully with GameCoordinatorService");
        }

        private void SubscribeToEvents()
        {
            // Subscribe to game events for UI updates
            _eventManager.Subscribe<GameStartedEvent>(OnGameStarted);
            _eventManager.Subscribe<GameEndedEvent>(OnGameEnded);
            _eventManager.Subscribe<PlayerStatsChangedEvent>(OnPlayerStatsChanged);
            _eventManager.Subscribe<PlayerLeveledUpEvent>(OnPlayerLeveledUp);
            _eventManager.Subscribe<PlayerHealthChangedEvent>(OnPlayerHealthChanged);
            _eventManager.Subscribe<PlayerGoldChangedEvent>(OnPlayerGoldChanged);
            _eventManager.Subscribe<PlayerExperienceGainedEvent>(OnPlayerExperienceGained);
            _eventManager.Subscribe<LocationChangedEvent>(OnLocationChanged);
            _eventManager.Subscribe<CombatStartedEvent>(OnCombatStarted);
            _eventManager.Subscribe<CombatEndedEvent>(OnCombatEnded);
            _eventManager.Subscribe<InventoryUpdatedEvent>(OnInventoryUpdated);
            _eventManager.Subscribe<ItemEquippedEvent>(OnItemEquipped);
            _eventManager.Subscribe<ItemUnequippedEvent>(OnItemUnequipped);
            _eventManager.Subscribe<SkillLearnedEvent>(OnSkillLearned);
            _eventManager.Subscribe<SkillUsedEvent>(OnSkillUsed);
            _eventManager.Subscribe<CommandProcessedEvent>(OnCommandProcessed);
            _eventManager.Subscribe<GameMessageEvent>(OnGameMessage);
        }

        private void UnsubscribeFromEvents()
        {
            // Unsubscribe from all events
            _eventManager.ClearSubscriptions<GameStartedEvent>();
            _eventManager.ClearSubscriptions<GameEndedEvent>();
            _eventManager.ClearSubscriptions<PlayerStatsChangedEvent>();
            _eventManager.ClearSubscriptions<PlayerLeveledUpEvent>();
            _eventManager.ClearSubscriptions<PlayerHealthChangedEvent>();
            _eventManager.ClearSubscriptions<PlayerGoldChangedEvent>();
            _eventManager.ClearSubscriptions<PlayerExperienceGainedEvent>();
            _eventManager.ClearSubscriptions<LocationChangedEvent>();
            _eventManager.ClearSubscriptions<CombatStartedEvent>();
            _eventManager.ClearSubscriptions<CombatEndedEvent>();
            _eventManager.ClearSubscriptions<InventoryUpdatedEvent>();
            _eventManager.ClearSubscriptions<ItemEquippedEvent>();
            _eventManager.ClearSubscriptions<ItemUnequippedEvent>();
            _eventManager.ClearSubscriptions<SkillLearnedEvent>();
            _eventManager.ClearSubscriptions<SkillUsedEvent>();
            _eventManager.ClearSubscriptions<CommandProcessedEvent>();
            _eventManager.ClearSubscriptions<GameMessageEvent>();
        }

        private void InitializeGameUI()
        {
            this.Text = "Realm of Aethermoor - RPG Adventure";
            this.Size = new Size(1000, 700);
            this.MinimumSize = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // Create main layout
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 75F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

            // Create menu strip
            CreateMenuStrip();

            // Create toolbar
            CreateToolStrip();

            // Create main game panel using custom controls
            Panel gamePanel = CreateGamePanel();
            mainLayout.Controls.Add(gamePanel, 0, 0);

            // Create side panel
            CreateSidePanel();
            mainLayout.Controls.Add(sidePanel, 1, 0);

            // Create status strip
            CreateStatusStrip();

            // Add controls to form
            this.Controls.Add(mainLayout);
            this.Controls.Add(toolStrip);
            this.Controls.Add(menuStrip);
            this.Controls.Add(gameStatusBarControl);

            // Set up keyboard shortcuts
            SetupKeyboardShortcuts();

            // Apply default theme
            ApplyTheme("Classic");

            // Initialize game-required controls tracking
            InitializeGameStateControls();

            // Set focus to input
            gameInputControl.FocusInput();
        }

        private void InitializeGameStateControls()
        {
            gameRequiredControls = new List<Control>();
            gameRequiredMenuItems = new List<ToolStripItem>();
            gameRequiredToolbarItems = new List<ToolStripItem>();

            // Initially disable game-dependent UI elements
            SetGameControlsEnabled(false);
        }

        private void SetGameControlsEnabled(bool enabled)
        {
            // Find and manage toolbar items
            if (toolStrip != null)
            {
                foreach (ToolStripItem item in toolStrip.Items)
                {
                    if (item.Text == "Save" || item.Text == "Load" || item.Text == "Inventory" || item.Text == "Map" || item.Text == "Skills")
                    {
                        item.Enabled = enabled;
                    }
                }
            }

            // Find and manage menu items
            if (menuStrip != null)
            {
                foreach (ToolStripMenuItem mainMenu in menuStrip.Items)
                {
                    if (mainMenu.Text.Contains("Character") || mainMenu.Text.Contains("World"))
                    {
                        mainMenu.Enabled = enabled;
                    }
                    else if (mainMenu.Text.Contains("Game"))
                    {
                        foreach (ToolStripItem subItem in mainMenu.DropDownItems)
                        {
                            if (subItem.Text.Contains("Save") || subItem.Text.Contains("Load"))
                            {
                                subItem.Enabled = enabled;
                            }
                        }
                    }
                }
            }

            // Manage custom controls
            if (gameInputControl != null)
                gameInputControl.InputEnabled = enabled;
            quickActionsControl?.SetButtonsEnabled(enabled, "Stats", "Save", "Load");
            miniMapControl?.SetMapEnabled(enabled);
        }

        private void CreateMenuStrip()
        {
            menuStrip = new MenuStrip();

            // Game menu
            ToolStripMenuItem gameMenu = new ToolStripMenuItem("&Game");
            gameMenu.DropDownItems.Add("&New Game", null, (s, e) => StartNewGame());
            gameMenu.DropDownItems.Add(new ToolStripSeparator());
            gameMenu.DropDownItems.Add("&Save Game", null, (s, e) => SaveGame());
            gameMenu.DropDownItems.Add("&Load Game", null, (s, e) => LoadGame());
            gameMenu.DropDownItems.Add(new ToolStripSeparator());
            gameMenu.DropDownItems.Add("E&xit", null, (s, e) => this.Close());

            // Character menu
            ToolStripMenuItem characterMenu = new ToolStripMenuItem("&Character");
            characterMenu.DropDownItems.Add("&Inventory", null, ShowInventory);
            characterMenu.DropDownItems.Add("&Stats", null, (s, e) => ShowCharacterStats());
            characterMenu.DropDownItems.Add("S&kills", null, (s, e) => ShowSkillTree(s, e));
            characterMenu.DropDownItems.Add("&Equipment", null, ShowEquipment);
            menuStrip.Items.Add(characterMenu);

            // World menu
            ToolStripMenuItem worldMenu = new ToolStripMenuItem("&World");
            worldMenu.DropDownItems.Add("&Map", null, ShowMap);
            worldMenu.DropDownItems.Add("&Locations", null, ShowLocationList);
            menuStrip.Items.Add(worldMenu);

            // Settings menu
            ToolStripMenuItem settingsMenu = new ToolStripMenuItem("&Settings");
            settingsMenu.DropDownItems.Add("&Themes", null, ShowThemeSelection);
            settingsMenu.DropDownItems.Add("&Font Size", null, ShowFontSettings);
            settingsMenu.DropDownItems.Add("&Game Settings", null, ShowGameSettings);
            menuStrip.Items.Add(settingsMenu);

            // Tools menu
            ToolStripMenuItem toolsMenu = new ToolStripMenuItem("&Tools");
            toolsMenu.DropDownItems.Add(new ToolStripMenuItem("&Asset Editor", null, ShowAssetEditor));
            toolsMenu.DropDownItems.Add(new ToolStripMenuItem("&Data Validation", null, ShowDataValidation));
            menuStrip.Items.Add(toolsMenu);

            // Advanced menu - demonstrates GameCoordinatorService
            ToolStripMenuItem advancedMenu = new ToolStripMenuItem("&Advanced");
            advancedMenu.DropDownItems.Add(new ToolStripMenuItem("&Validate Game State", null, ValidateGameState));
            advancedMenu.DropDownItems.Add(new ToolStripMenuItem("&Comprehensive Status", null, ShowComprehensiveStatus));
            advancedMenu.DropDownItems.Add(new ToolStripMenuItem("&Synchronize Managers", null, SynchronizeManagers));
            advancedMenu.DropDownItems.Add(new ToolStripMenuItem("&Perform Maintenance", null, PerformMaintenance));
            advancedMenu.DropDownItems.Add(new ToolStripSeparator());
            advancedMenu.DropDownItems.Add(new ToolStripMenuItem("&Level Up with Rewards", null, LevelUpWithRewards));
            advancedMenu.DropDownItems.Add(new ToolStripMenuItem("&Auto Explore", null, AutoExplore));
            advancedMenu.DropDownItems.Add(new ToolStripMenuItem("&Optimize Character", null, OptimizeCharacter));
            menuStrip.Items.Add(advancedMenu);

            // Help menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.Add("&Commands", null, (s, e) => ShowHelp());
            helpMenu.DropDownItems.Add(new ToolStripMenuItem("&Controls", null, ShowControlsHelp));
            helpMenu.DropDownItems.Add(new ToolStripMenuItem("&About", null, ShowAbout));
            menuStrip.Items.Add(helpMenu);

            this.MainMenuStrip = menuStrip;
        }

        private void CreateToolStrip()
        {
            toolStrip = new ToolStrip
            {
                ImageScalingSize = new Size(24, 24)
            };

            // Quick action buttons
            ToolStripButton newGameButton = new ToolStripButton("New Game")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Start a new game"
            };
            newGameButton.Click += (s, e) => StartNewGame();

            ToolStripButton saveButton = new ToolStripButton("Save")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Save game (Ctrl+S)"
            };
            saveButton.Click += (s, e) => SaveGame();

            ToolStripButton loadButton = new ToolStripButton("Load")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Load game (Ctrl+L)"
            };
            loadButton.Click += (s, e) => LoadGame();

            ToolStripSeparator separator1 = new ToolStripSeparator();

            ToolStripButton inventoryButton = new ToolStripButton("Inventory")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Open inventory"
            };
            inventoryButton.Click += ShowInventory;

            ToolStripButton mapButton = new ToolStripButton("Map")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Open world map"
            };
            mapButton.Click += ShowMap;

            ToolStripButton skillsButton = new ToolStripButton("Skills")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Open skill tree"
            };
            skillsButton.Click += (s, e) => ShowSkillTree(s, e);

            ToolStripSeparator separator2 = new ToolStripSeparator();

            ToolStripButton helpButton = new ToolStripButton("Help")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Show help (F1)"
            };
            helpButton.Click += (s, e) => ShowHelp();

            toolStrip.Items.AddRange(new ToolStripItem[] {
                newGameButton, saveButton, loadButton, separator1,
                inventoryButton, mapButton, skillsButton, separator2, helpButton
            });
        }

        private Panel CreateGamePanel()
        {
            Panel gamePanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            TableLayoutPanel gameLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            gameLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 85F));
            gameLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));

            // Game text display using custom control
            gameTextDisplayControl = new GameTextDisplayControl
            {
                Dock = DockStyle.Fill
            };
            gameTextDisplayControl.ClearRequested += (s, e) => _gameManager?.ProcessCommand("clear");
            gameLayout.Controls.Add(gameTextDisplayControl, 0, 0);

            // Input panel using custom control
            gameInputControl = new GameInputControl
            {
                Dock = DockStyle.Fill
            };
            gameInputControl.CommandSubmitted += GameInputControl_CommandSubmitted;
            gameInputControl.InventoryRequested += (s, e) => ShowInventory(s, e);
            gameInputControl.HelpRequested += (s, e) => ShowHelp();
            gameLayout.Controls.Add(gameInputControl, 0, 1);

            gamePanel.Controls.Add(gameLayout);
            return gamePanel;
        }

        private void CreateSidePanel()
        {
            sidePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.LightGray,
                Padding = new Padding(5)
            };

            TableLayoutPanel sideLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4
            };
            sideLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            sideLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Character stats panel using custom control
            GroupBox statsGroup = new GroupBox
            {
                Text = "Character Stats",
                Dock = DockStyle.Fill,
                Height = 120,
                Padding = new Padding(5)
            };
            characterStatsControl = new CharacterStatsControl
            {
                Dock = DockStyle.Fill
            };
            statsGroup.Controls.Add(characterStatsControl);
            sideLayout.Controls.Add(statsGroup, 0, 0);

            // Progress bars panel using custom control
            GroupBox progressGroup = new GroupBox
            {
                Text = "Status",
                Dock = DockStyle.Fill,
                Height = 80,
                Padding = new Padding(5)
            };
            progressDisplayControl = new ProgressDisplayControl
            {
                Dock = DockStyle.Fill
            };
            progressGroup.Controls.Add(progressDisplayControl);
            sideLayout.Controls.Add(progressGroup, 0, 1);

            // Mini map panel using custom control
            GroupBox miniMapGroup = new GroupBox
            {
                Text = "Mini Map",
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };
            miniMapControl = new MiniMapControl
            {
                Dock = DockStyle.Fill
            };
            miniMapControl.FullMapRequested += (s, e) => ShowMap(s, e);
            miniMapControl.LocationClicked += MiniMapControl_LocationClicked;
            miniMapGroup.Controls.Add(miniMapControl);
            sideLayout.Controls.Add(miniMapGroup, 0, 2);

            // Quick actions panel using custom control
            GroupBox actionsGroup = new GroupBox
            {
                Text = "Quick Actions",
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };
            quickActionsControl = new QuickActionsControl
            {
                Dock = DockStyle.Fill
            };
            quickActionsControl.ActionClicked += QuickActionsControl_ActionClicked;
            actionsGroup.Controls.Add(quickActionsControl);
            sideLayout.Controls.Add(actionsGroup, 0, 3);

            sidePanel.Controls.Add(sideLayout);
        }

        private void CreateStatusStrip()
        {
            gameStatusBarControl = new GameStatusBarControl
            {
                Dock = DockStyle.Bottom
            };
        }

        private void SetupKeyboardShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Tab:
                    if (!e.Control && !e.Alt)
                    {
                        ShowInventory(sender, e);
                        e.Handled = true;
                    }
                    break;
                case Keys.M:
                    if (!e.Control && !e.Alt)
                    {
                        ShowMap(sender, e);
                        e.Handled = true;
                    }
                    break;
                case Keys.S:
                    if (e.Control)
                    {
                        SaveGame();
                        e.Handled = true;
                    }
                    break;
                case Keys.L:
                    if (e.Control)
                    {
                        LoadGame();
                        e.Handled = true;
                    }
                    break;
                case Keys.F1:
                    ShowHelp();
                    e.Handled = true;
                    break;
            }
        }

        // Event handlers for custom controls
        private void GameInputControl_CommandSubmitted(object sender, string command)
        {
            DisplayText($"> {command}", Color.Yellow);
            _gameManager?.ProcessCommand(command);
        }

        private void MiniMapControl_LocationClicked(object sender, string location)
        {
            DisplayText($"You want to travel to {location}. Use movement commands to get there.", Color.Cyan);
        }

        // Event handlers for new features
        private void ShowInventory(object sender, EventArgs e)
        {
            if (_gameManager != null)
            {
                // This will be implemented when we have access to the player
                try
                {
                    var inventoryForm = _serviceProvider.GetRequiredService<InventoryForm>();
                    inventoryForm.ShowDialog();
                }
                catch
                {
                    DisplayText("Start a new game first!", Color.Red);
                }
            }
        }

        private void ShowMap(object sender, EventArgs e)
        {
            if (_gameManager != null)
            {
                try
                {
                    var mapForm = _serviceProvider.GetRequiredService<MapForm>();
                    mapForm.ShowDialog();
                }
                catch
                {
                    DisplayText("Start a new game first!", Color.Red);
                }
            }
        }

        private void ShowEquipment(object sender, EventArgs e)
        {
            if (_gameManager != null)
            {
                try
                {
                    var player = _playerManager.CurrentPlayer;
                    string equipment = "=== Equipment ===\n";
                    equipment += $"Weapon: {(player.EquippedWeapon?.Name ?? "None")}\n";
                    equipment += $"Armor: {(player.EquippedArmor?.Name ?? "None")}\n\n";
                    equipment += $"Total Attack: {player.GetTotalAttack()}\n";
                    equipment += $"Total Defense: {player.GetTotalDefense()}";
                    
                    MessageBox.Show(equipment, "Equipment", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    DisplayText("Start a new game first!", Color.Red);
                }
            }
        }

        private void ShowLocationList(object sender, EventArgs e)
        {
            DisplayText("=== Known Locations ===", Color.Cyan);
            DisplayText("• Village of Elderbrook - A peaceful starting town");
            DisplayText("• Dark Forest - Dangerous woods with wolves");
            DisplayText("• Grassy Plains - Open fields with wild boars");
            DisplayText("• Ancient Cave - Mysterious cavern with treasures");
            DisplayText("• Ancient Ruins - Crumbling structures with secrets");
            DisplayText("• Dragon's Lair - The ultimate challenge");
            DisplayText("");
        }

        private void ShowThemeSelection(object sender, EventArgs e)
        {
            using (var themeDialog = new ThemeSelectionDialog())
            {
                if (themeDialog.ShowDialog() == DialogResult.OK)
                {
                    ApplyTheme(themeDialog.SelectedTheme);
                }
            }
        }

        private void ShowFontSettings(object sender, EventArgs e)
        {
            using (FontDialog fontDialog = new FontDialog())
            {
                fontDialog.Font = gameTextDisplayControl.GetFont();
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    gameTextDisplayControl.SetFont(fontDialog.Font);
                }
            }
        }

        private void ShowGameSettings(object sender, EventArgs e)
        {
            var currentSettings = new GameSettings(); // In a real app, load from config
            using (var settingsForm = new SettingsForm(currentSettings))
            {
                if (settingsForm.ShowDialog() == DialogResult.OK)
                {
                    // Apply the new settings
                    var newSettings = settingsForm.Settings;
                    
                    // Apply theme if changed
                    if (newSettings.Theme != currentSettings.Theme)
                    {
                        ApplyTheme(newSettings.Theme);
                    }
                    
                    // Apply font size if changed
                    if (newSettings.FontSize != currentSettings.FontSize)
                    {
                        var newFont = new Font(gameTextDisplayControl.GetFont().FontFamily, newSettings.FontSize);
                        gameTextDisplayControl.SetFont(newFont);
                    }
                    
                    DisplayText("Settings applied successfully!", Color.Green);
                }
            }
        }

        private void ShowControlsHelp(object sender, EventArgs e)
        {
            string controls = "=== Keyboard Controls ===\n\n";
            controls += "Tab - Open inventory\n";
            controls += "M - Open world map\n";
            controls += "Ctrl+S - Quick save\n";
            controls += "Ctrl+L - Quick load\n";
            controls += "F1 - Show help\n";
            controls += "Enter - Submit command\n";
            controls += "↑↓ - Command history\n\n";
            controls += "=== Mouse Controls ===\n\n";
            controls += "Click buttons for quick actions\n";
            controls += "Right-click for context menus\n";
            controls += "Double-click items to use them";

            MessageBox.Show(controls, "Controls Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAbout(object sender, EventArgs e)
        {
            MessageBox.Show("Realm of Aethermoor\nVersion 2.0\n\n" +
                "A classic text-based RPG adventure built with .NET WinForms\n\n" +
                "Features:\n" +
                "• Rich character creation system\n" +
                "• Turn-based combat\n" +
                "• Inventory management\n" +
                "• World exploration\n" +
                "• Save/Load functionality\n" +
                "• Multiple themes\n\n" +
                "Created with ❤️ for RPG enthusiasts",
                "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAssetEditor(object sender, EventArgs e)
        {
            try
            {
                AssetEditorForm.ShowAssetEditor();
                DisplayText("Asset Editor opened. You can now edit game data files.", Color.Cyan);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Asset Editor:\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowDataValidation(object sender, EventArgs e)
        {
            MessageBox.Show("Data Validation Tool\n\n" +
                "This feature validates the integrity of your game data files:\n\n" +
                "• Checks JSON syntax\n" +
                "• Validates data relationships\n" +
                "• Reports missing or invalid entries\n" +
                "• Suggests fixes for common issues\n\n" +
                "Feature coming soon in future update!",
                "Data Validation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Update the UpdateStatus method to work with new UI elements
        public void UpdateStatus(string status)
        {
            gameStatusBarControl?.UpdateStatus(status);
            
            // Update progress bars and stats if we have access to player data
            try
            {
                var player = _playerManager.CurrentPlayer;
                if (player != null)
                {
                    // Update status bar with player stats
                    gameStatusBarControl?.UpdatePlayerStats(player);

                    // Update progress display using custom control
                    progressDisplayControl?.UpdateProgress(player);

                    // Update character stats using custom control
                    characterStatsControl?.UpdateStats(player);

                    // Update mini-map with current location
                    var currentLocation = _locationManager.CurrentLocation;
                    if (currentLocation != null)
                    {
                        miniMapControl?.UpdateCurrentLocation(currentLocation.Key);
                        
                        // Update status bar location
                        gameStatusBarControl?.UpdateLocation(currentLocation.Name);
                    }
                }
                else
                {
                    // Clear stats when no player is available
                    gameStatusBarControl?.ClearPlayerStats();
                    progressDisplayControl?.ClearProgress();
                    characterStatsControl?.ClearStats();
                    miniMapControl?.ClearVisitedLocations();
                }
            }
            catch
            {
                // Player not available yet - clear the displays
                gameStatusBarControl?.ClearPlayerStats();
                progressDisplayControl?.ClearProgress();
                characterStatsControl?.ClearStats();
                miniMapControl?.ClearVisitedLocations();
            }
        }

        public void ClearScreen()
        {
            gameTextDisplayControl?.ClearText();
        }

        public void DisplayText(string text, Color? color = null)
        {
            gameTextDisplayControl?.DisplayText(text, color);
        }

        private void ApplyTheme(string themeName)
        {
            // Apply theme to custom controls
            gameTextDisplayControl?.ApplyTheme(themeName);
            gameInputControl?.ApplyTheme(themeName);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Unsubscribe from events before closing
            UnsubscribeFromEvents();
            
            if (_gameManager?.CurrentGameState == GameState.Running)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Would you like to save before exiting?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    SaveGame();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnFormClosing(e);
        }

        public void EnableGameControls(bool enabled)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => EnableGameControls(enabled)));
                return;
            }
            SetGameControlsEnabled(enabled);
        }

        private void QuickActionsControl_ActionClicked(object sender, string action)
        {
            switch (action)
            {
                case "look":
                    _gameManager.ProcessCommand("look");
                    break;
                case "stats":
                    ShowCharacterStats();
                    break;
                case "save":
                    SaveGame();
                    break;
                case "load":
                    LoadGame();
                    break;
                case "help":
                    ShowHelp();
                    break;
                case "exit":
                    this.Close();
                    break;
            }
        }

        public void SetCombatMode(bool inCombat)
        {
            gameStatusBarControl?.SetCombatMode(inCombat);
        }

        public void ShowExperienceGain(int expGained)
        {
            gameStatusBarControl?.ShowExperienceGain(expGained);
        }

        public void ShowLevelUp(int oldLevel, int newLevel)
        {
            gameStatusBarControl?.ShowExperienceGain(0, newLevel);
        }

        public void ShowGoldChange(int goldChange)
        {
            gameStatusBarControl?.ShowGoldChange(goldChange);
        }

        private void OnGameStarted(GameStartedEvent e)
        {
            // Enable game controls when game starts
            this.Invoke(() =>
            {
                SetGameControlsEnabled(true);
                DisplayText("=== Welcome to the Realm of Aethermoor ===", Color.Green);
                DisplayText($"Welcome, {e.Player.Name} the {e.Player.CharacterClass}!", Color.Cyan);
                DisplayText("Your adventure begins! Type 'help' for commands or 'look' to examine your surroundings.", Color.White);
                UpdateStatus($"Game started - Playing as {e.Player.Name}");
                
                // Update character display
                characterStatsControl?.UpdateStats(e.Player);
                progressDisplayControl?.UpdateProgress(e.Player);
            });
        }

        private void OnGameEnded(GameEndedEvent e)
        {
            // Disable game controls when game ends
            this.Invoke(() =>
            {
                SetGameControlsEnabled(false);
                DisplayText("=== Game Ended ===", Color.Red);
                DisplayText(e.Reason, Color.Yellow);
                UpdateStatus("Game ended");
                
                // Clear character display
                characterStatsControl?.ClearStats();
                progressDisplayControl?.ClearProgress();
            });
        }

        private void OnPlayerStatsChanged(PlayerStatsChangedEvent e)
        {
            // Update character stats display
            this.Invoke(() =>
            {
                characterStatsControl?.UpdateStats(e.Player);
                progressDisplayControl?.UpdateProgress(e.Player);
                
                // Show stat change message if significant
                if (e.StatType != StatType.Health && e.StatType != StatType.Experience) // Don't spam for frequent changes
                {
                    DisplayText($"{e.StatType} changed: {e.OldValue} → {e.NewValue}", Color.Cyan);
                }
            });
        }

        private void OnPlayerLeveledUp(PlayerLeveledUpEvent e)
        {
            // Show level up celebration
            this.Invoke(() =>
            {
                DisplayText("", Color.White);
                DisplayText("*** LEVEL UP! ***", Color.Gold);
                DisplayText($"Congratulations! You reached level {e.NewLevel}!", Color.Gold);
                DisplayText($"You gained {e.SkillPointsGained} skill points!", Color.Magenta);
                DisplayText("", Color.White);
                
                // Update displays
                characterStatsControl?.UpdateStats(e.Player);
                progressDisplayControl?.UpdateProgress(e.Player);
                
                // Show visual feedback
                ShowLevelUp(e.OldLevel, e.NewLevel);
            });
        }

        private void OnPlayerHealthChanged(PlayerHealthChangedEvent e)
        {
            // Update health display
            this.Invoke(() =>
            {
                progressDisplayControl?.UpdateHealth(e.NewHealth, e.MaxHealth);
                
                // Show health change message for significant changes
                int healthChange = e.NewHealth - e.OldHealth;
                if (Math.Abs(healthChange) >= 5) // Only show for changes of 5+ health
                {
                    if (healthChange > 0)
                    {
                        DisplayText($"Health restored: +{healthChange} ({e.Reason})", Color.Green);
                    }
                    else
                    {
                        DisplayText($"Health lost: {healthChange} ({e.Reason})", Color.Red);
                    }
                }
                
                // Update status bar
                UpdateStatus($"Health: {e.NewHealth}/{e.MaxHealth}");
            });
        }

        private void OnPlayerGoldChanged(PlayerGoldChangedEvent e)
        {
            // Update gold display
            this.Invoke(() =>
            {
                characterStatsControl?.UpdateStats(e.Player);
                
                // Show gold change message
                int goldChange = e.NewGold - e.OldGold;
                if (goldChange != 0)
                {
                    if (goldChange > 0)
                    {
                        DisplayText($"Gold gained: +{goldChange} ({e.Reason})", Color.Yellow);
                        ShowGoldChange(goldChange);
                    }
                    else
                    {
                        DisplayText($"Gold spent: {goldChange} ({e.Reason})", Color.Orange);
                    }
                }
            });
        }

        private void OnPlayerExperienceGained(PlayerExperienceGainedEvent e)
        {
            // Update experience display
            this.Invoke(() =>
            {
                progressDisplayControl?.UpdateExperience(e.TotalExperience, e.Player.ExperienceToNextLevel);
                
                // Show experience gain message
                if (e.ExperienceGained > 0)
                {
                    DisplayText($"Experience gained: +{e.ExperienceGained} ({e.Source})", Color.Cyan);
                    ShowExperienceGain(e.ExperienceGained);
                }
            });
        }

        private void OnLocationChanged(LocationChangedEvent e)
        {
            // Update location display
            this.Invoke(() =>
            {
                DisplayText("", Color.White);
                DisplayText($"=== {e.NewLocation.Name} ===", Color.Magenta);
                DisplayText(e.NewLocation.Description, Color.White);
                
                if (e.IsFirstVisit)
                {
                    DisplayText("(First time visiting this location!)", Color.Green);
                }
                
                // Update mini-map
                miniMapControl?.UpdateCurrentLocation(e.NewLocation.Key);
                
                // Update status bar
                UpdateStatus($"Location: {e.NewLocation.Name}");
                
                // Show available exits
                var exits = _locationManager.GetExitsDescription();
                DisplayText(exits, Color.Gray);
                
                // Show items and NPCs if any
                var items = _locationManager.GetLocationItems();
                if (items.Count > 0)
                {
                    DisplayText($"Items here: {string.Join(", ", items.Select(i => i.Name))}", Color.Yellow);
                }
                
                var npcs = _locationManager.GetLocationNPCs();
                if (npcs.Count > 0)
                {
                    DisplayText($"People here: {string.Join(", ", npcs)}", Color.Cyan);
                }
            });
        }

        private void OnCombatStarted(CombatStartedEvent e)
        {
            // Update UI for combat mode
            this.Invoke(() =>
            {
                DisplayText("", Color.White);
                DisplayText("*** COMBAT STARTED ***", Color.Red);
                DisplayText($"You are fighting: {e.Enemy.Name} (Level {e.Enemy.Level})", Color.Red);
                DisplayText($"Enemy Health: {e.Enemy.Health}/{e.Enemy.MaxHealth}", Color.Orange);
                DisplayText("Commands: attack, defend, flee", Color.Yellow);
                DisplayText("", Color.White);
                
                SetCombatMode(true);
                UpdateStatus($"In combat with {e.Enemy.Name}");
            });
        }

        private void OnCombatEnded(CombatEndedEvent e)
        {
            // Update UI when combat ends
            this.Invoke(() =>
            {
                DisplayText("", Color.White);
                DisplayText("*** COMBAT ENDED ***", Color.Green);
                
                if (e.Result == CombatResult.Victory)
                {
                    DisplayText($"Victory! You defeated {e.Enemy.Name}!", Color.Green);
                    if (e.ExperienceGained > 0)
                        DisplayText($"Experience gained: +{e.ExperienceGained}", Color.Cyan);
                    if (e.GoldGained > 0)
                        DisplayText($"Gold gained: +{e.GoldGained}", Color.Yellow);
                    if (e.LootGained.Count > 0)
                        DisplayText($"Loot found: {string.Join(", ", e.LootGained.Select(i => i.Name))}", Color.Magenta);
                }
                else if (e.Result == CombatResult.Defeat)
                {
                    DisplayText($"You were defeated by {e.Enemy.Name}!", Color.Red);
                    DisplayText("You have been defeated in combat.", Color.Orange);
                }
                else if (e.Result == CombatResult.Fled)
                {
                    DisplayText($"You successfully fled from {e.Enemy.Name}!", Color.Yellow);
                }
                
                DisplayText("", Color.White);
                
                SetCombatMode(false);
                UpdateStatus("Combat ended");
            });
        }

        private void OnInventoryUpdated(InventoryUpdatedEvent e)
        {
            // Update inventory-related displays
            this.Invoke(() =>
            {
                // Show inventory change message
                if (e.Action == InventoryAction.ItemAdded)
                {
                    DisplayText($"Added to inventory: {e.AffectedItem.Name} x{e.Quantity}", Color.Green);
                }
                else if (e.Action == InventoryAction.ItemRemoved)
                {
                    DisplayText($"Removed from inventory: {e.AffectedItem.Name} x{e.Quantity}", Color.Orange);
                }
            });
        }

        private void OnItemEquipped(ItemEquippedEvent e)
        {
            // Show equipment change
            this.Invoke(() =>
            {
                DisplayText($"Equipped: {e.EquippedItem.Name}", Color.Green);
                
                // Update character stats
                characterStatsControl?.UpdateStats(e.Player);
            });
        }

        private void OnItemUnequipped(ItemUnequippedEvent e)
        {
            // Show equipment removal
            this.Invoke(() =>
            {
                DisplayText($"Unequipped: {e.UnequippedItem.Name}", Color.Orange);
                
                // Update character stats
                characterStatsControl?.UpdateStats(e.Player);
            });
        }

        private void OnSkillLearned(SkillLearnedEvent e)
        {
            // Show skill learning result
            this.Invoke(() =>
            {
                if (e.WasSuccessful)
                {
                    DisplayText($"Skill learned: {e.Skill.Name}!", Color.Magenta);
                    DisplayText($"Skill points spent: {e.SkillPointsSpent}", Color.Cyan);
                    DisplayText($"Remaining skill points: {e.RemainingSkillPoints}", Color.Cyan);
                }
                else
                {
                    DisplayText($"Failed to learn skill: {e.FailureReason}", Color.Red);
                }
            });
        }

        private void OnSkillUsed(SkillUsedEvent e)
        {
            // Show skill usage result
            this.Invoke(() =>
            {
                if (e.WasSuccessful)
                {
                    DisplayText($"Used skill: {e.Skill.Name}", Color.Magenta);
                    if (e.SkillEffects.ContainsKey("description"))
                    {
                        DisplayText($"Effect: {e.SkillEffects["description"]}", Color.Cyan);
                    }
                }
                else
                {
                    DisplayText($"Failed to use skill: {e.FailureReason}", Color.Red);
                }
            });
        }

        private void OnCommandProcessed(CommandProcessedEvent e)
        {
            // Show command processing result
            this.Invoke(() =>
            {
                if (!e.Result.Success && !string.IsNullOrEmpty(e.Result.ErrorMessage))
                {
                    DisplayText($"Error: {e.Result.ErrorMessage}", Color.Red);
                }
                
                // Update status if needed
                if (!string.IsNullOrEmpty(e.Result.Message))
                {
                    UpdateStatus(e.Result.Message);
                }
            });
        }

        private void OnGameMessage(GameMessageEvent e)
        {
            // Display game messages
            this.Invoke(() =>
            {
                Color messageColor = e.MessageType switch
                {
                    GameMessageType.Info => Color.White,
                    GameMessageType.Success => Color.Green,
                    GameMessageType.Warning => Color.Yellow,
                    GameMessageType.Error => Color.Red,
                    GameMessageType.Combat => Color.Orange,
                    GameMessageType.System => Color.Cyan,
                    _ => Color.White
                };
                
                DisplayText(e.Message, messageColor);
            });
        }

        // UI Action Methods
        private void StartNewGame()
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

        private void SaveGame()
        {
            // For now, use save slot 1. In a full implementation, show a save slot selection dialog
            _gameManager.SaveGame(1);
        }

        private void LoadGame()
        {
            // For now, use save slot 1. In a full implementation, show a load slot selection dialog
            _gameManager.LoadGame(1);
        }

        private void ShowCharacterStats()
        {
            var player = _playerManager.CurrentPlayer;
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
                
                DisplayText(statsMessage, Color.Cyan);
            }
            else
            {
                DisplayText("No character loaded.", Color.Red);
            }
        }

        private void ShowSkillTree(object sender, EventArgs e)
        {
            if (_gameManager != null)
            {
                try
                {
                    var skillTreeForm = _serviceProvider.GetRequiredService<SkillTreeForm>();
                    skillTreeForm.ShowDialog();
                }
                catch
                {
                    DisplayText("Start a new game first!", Color.Red);
                }
            }
        }

        private void ShowHelp()
        {
            var helpText = "=== Game Commands ===\n" +
                          "Movement: north, south, east, west, go [direction]\n" +
                          "Actions: look, take [item], use [item], attack [enemy]\n" +
                          "Character: stats, inventory, skills\n" +
                          "Combat: attack, defend, flee\n" +
                          "Game: save, load, help, quit\n" +
                          "\n" +
                          "=== Advanced Commands ===\n" +
                          "auto-explore: Automatically explore random directions\n" +
                          "optimize-character: Auto-equip best items\n" +
                          "batch-use <type>: Use all items of specified type\n" +
                          "skill-combo <skills>: Execute multiple skills in sequence\n" +
                          "statistics: Show detailed game statistics\n" +
                          "features: Show/toggle game features\n" +
                          "status: Show comprehensive game status\n" +
                          "\n" +
                          "=== Keyboard Shortcuts ===\n" +
                          "Ctrl+S: Save Game\n" +
                          "Ctrl+L: Load Game\n" +
                          "F1: Show Help\n" +
                          "Tab: Open Inventory";
            
            DisplayText(helpText, Color.Yellow);
        }

        // Advanced GameCoordinatorService demonstration methods

        private async void ValidateGameState(object sender, EventArgs e)
        {
            try
            {
                DisplayText("=== Validating Game State ===", Color.Cyan);
                var validation = await _gameCoordinator.ValidateGameStateAsync();

                DisplayText($"Validation Result: {(validation.IsValid ? "VALID" : "INVALID")}", 
                    validation.IsValid ? Color.Green : Color.Red);

                if (validation.Issues.Any())
                {
                    DisplayText($"\nFound {validation.Issues.Count} issues:", Color.Yellow);
                    foreach (var issue in validation.Issues)
                    {
                        var color = issue.Severity switch
                        {
                            "Error" => Color.Red,
                            "Warning" => Color.Yellow,
                            _ => Color.White
                        };
                        DisplayText($"[{issue.Severity}] {issue.Manager}: {issue.Issue}", color);
                        DisplayText($"  Recommendation: {issue.Recommendation}", Color.Cyan);
                    }
                }
                else
                {
                    DisplayText("No issues found - game state is healthy!", Color.Green);
                }

                DisplayText($"\nManager States:", Color.Cyan);
                foreach (var state in validation.ManagerStates)
                {
                    DisplayText($"  {state.Key}: {(state.Value ? "OK" : "ERROR")}", 
                        state.Value ? Color.Green : Color.Red);
                }
            }
            catch (Exception ex)
            {
                DisplayText($"Error during validation: {ex.Message}", Color.Red);
                _logger.LogError(ex, "Error validating game state");
            }
        }

        private async void ShowComprehensiveStatus(object sender, EventArgs e)
        {
            try
            {
                DisplayText("=== Comprehensive Game Status ===", Color.Cyan);
                var status = await _gameCoordinator.GetComprehensiveStatusAsync();

                DisplayText($"Game State: {status.GameState}", Color.White);
                DisplayText($"In Combat: {status.InCombat}", Color.White);
                DisplayText($"Current Location: {status.CurrentLocation ?? "None"}", Color.White);
                
                if (status.CurrentPlayer != null)
                {
                    DisplayText($"Player: {status.CurrentPlayer.Name} (Level {status.CurrentPlayer.Level})", Color.Green);
                    DisplayText($"Health: {status.CurrentPlayer.Health}/{status.CurrentPlayer.MaxHealth}", Color.White);
                    DisplayText($"Gold: {status.CurrentPlayer.Gold}", Color.Yellow);
                }

                if (status.Statistics != null)
                {
                    DisplayText($"\nSession Statistics:", Color.Cyan);
                    DisplayText($"  Play Time: {status.Statistics.TotalPlayTime:F1} minutes", Color.White);
                    DisplayText($"  Enemies Defeated: {status.Statistics.EnemiesDefeated}", Color.White);
                    DisplayText($"  Items Collected: {status.Statistics.ItemsCollected}", Color.White);
                    DisplayText($"  Locations Visited: {status.Statistics.LocationsVisited}", Color.White);
                }

                if (status.ActiveEffects.Any())
                {
                    DisplayText($"\nActive Effects:", Color.Magenta);
                    foreach (var effect in status.ActiveEffects)
                    {
                        DisplayText($"  • {effect}", Color.White);
                    }
                }

                DisplayText($"\nManager Statuses:", Color.Cyan);
                foreach (var managerStatus in status.ManagerStatuses)
                {
                    DisplayText($"  {managerStatus.Key}: {managerStatus.Value}", Color.White);
                }
            }
            catch (Exception ex)
            {
                DisplayText($"Error getting comprehensive status: {ex.Message}", Color.Red);
                _logger.LogError(ex, "Error getting comprehensive status");
            }
        }

        private async void SynchronizeManagers(object sender, EventArgs e)
        {
            try
            {
                DisplayText("=== Synchronizing Managers ===", Color.Cyan);
                var success = await _gameCoordinator.SynchronizeManagersAsync();

                if (success)
                {
                    DisplayText("Manager synchronization completed successfully!", Color.Green);
                    DisplayText("All manager data is now synchronized.", Color.White);
                }
                else
                {
                    DisplayText("Manager synchronization failed.", Color.Red);
                    DisplayText("Check logs for detailed error information.", Color.Yellow);
                }
            }
            catch (Exception ex)
            {
                DisplayText($"Error during synchronization: {ex.Message}", Color.Red);
                _logger.LogError(ex, "Error synchronizing managers");
            }
        }

        private async void PerformMaintenance(object sender, EventArgs e)
        {
            try
            {
                DisplayText("=== Performing Automated Maintenance ===", Color.Cyan);
                var result = await _gameCoordinator.PerformMaintenanceAsync();

                if (result.Success)
                {
                    DisplayText($"Maintenance completed successfully in {result.Duration.TotalMilliseconds:F0}ms", Color.Green);
                    DisplayText($"\nOperations performed:", Color.Cyan);
                    foreach (var operation in result.OperationsPerformed)
                    {
                        DisplayText($"  ✓ {operation}", Color.Green);
                    }

                    if (result.Results.Any())
                    {
                        DisplayText($"\nResults:", Color.Cyan);
                        foreach (var resultItem in result.Results)
                        {
                            DisplayText($"  {resultItem.Key}: {resultItem.Value}", Color.White);
                        }
                    }
                }
                else
                {
                    DisplayText("Maintenance failed!", Color.Red);
                    if (result.Results.ContainsKey("Error"))
                    {
                        DisplayText($"Error: {result.Results["Error"]}", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayText($"Error during maintenance: {ex.Message}", Color.Red);
                _logger.LogError(ex, "Error performing maintenance");
            }
        }

        private async void LevelUpWithRewards(object sender, EventArgs e)
        {
            try
            {
                if (_playerManager.CurrentPlayer == null)
                {
                    DisplayText("No player loaded. Start a new game first!", Color.Red);
                    return;
                }

                DisplayText("=== Level Up with Rewards ===", Color.Cyan);
                var parameters = new Dictionary<string, object>();
                var result = await _gameCoordinator.PerformCoordinatedActionAsync("levelup_with_rewards", parameters);

                if (result.Success)
                {
                    DisplayText(result.Message, Color.Green);
                    
                    if (result.Results.ContainsKey("OldLevel") && result.Results.ContainsKey("NewLevel"))
                    {
                        DisplayText($"Level progression: {result.Results["OldLevel"]} → {result.Results["NewLevel"]}", Color.Magenta);
                    }
                    
                    if (result.Results.ContainsKey("GoldReward"))
                    {
                        DisplayText($"Gold reward: {result.Results["GoldReward"]}", Color.Yellow);
                    }
                }
                else
                {
                    DisplayText("Level up failed:", Color.Red);
                    foreach (var error in result.Errors)
                    {
                        DisplayText($"  • {error}", Color.Red);
                    }
                }

                if (result.Warnings.Any())
                {
                    foreach (var warning in result.Warnings)
                    {
                        DisplayText($"Warning: {warning}", Color.Yellow);
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayText($"Error during level up: {ex.Message}", Color.Red);
                _logger.LogError(ex, "Error performing level up with rewards");
            }
        }

        private async void AutoExplore(object sender, EventArgs e)
        {
            try
            {
                if (_playerManager.CurrentPlayer == null)
                {
                    DisplayText("No player loaded. Start a new game first!", Color.Red);
                    return;
                }

                DisplayText("=== Auto Explore ===", Color.Cyan);
                var result = await _gameCoordinator.ExecuteComplexCommandAsync("auto-explore");

                if (result.Success)
                {
                    DisplayText(result.Message, Color.Green);
                }
                else
                {
                    DisplayText($"Auto explore failed: {result.ErrorMessage}", Color.Red);
                }
            }
            catch (Exception ex)
            {
                DisplayText($"Error during auto explore: {ex.Message}", Color.Red);
                _logger.LogError(ex, "Error performing auto explore");
            }
        }

        private async void OptimizeCharacter(object sender, EventArgs e)
        {
            try
            {
                if (_playerManager.CurrentPlayer == null)
                {
                    DisplayText("No player loaded. Start a new game first!", Color.Red);
                    return;
                }

                DisplayText("=== Optimize Character ===", Color.Cyan);
                var result = await _gameCoordinator.ExecuteComplexCommandAsync("optimize-character");

                if (result.Success)
                {
                    DisplayText(result.Message, Color.Green);
                }
                else
                {
                    DisplayText($"Character optimization failed: {result.ErrorMessage}", Color.Red);
                }
            }
            catch (Exception ex)
            {
                DisplayText($"Error during character optimization: {ex.Message}", Color.Red);
                _logger.LogError(ex, "Error optimizing character");
            }
        }
    }
}
