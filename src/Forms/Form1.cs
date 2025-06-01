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
        private readonly IUIManager _uiManager;
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
            _uiManager = serviceProvider.GetRequiredService<IUIManager>();
            _logger = serviceProvider.GetRequiredService<ILogger<Form1>>();

            _logger.LogInformation("Initializing Form1 with dependency injection and UIManager");

            InitializeComponent();
            InitializeGameUI(); // Initialize the UI before subscribing to events
            SubscribeToEvents();
            SetGameControlsEnabled(false); // Disable until game starts

            _logger.LogInformation("Form1 initialized successfully with UIManager and GameCoordinatorService");
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
            
            // Subscribe to UI events
            _eventManager.Subscribe<DisplayTextEvent>(OnDisplayText);
            _eventManager.Subscribe<DisplayFormattedMessageEvent>(OnDisplayFormattedMessage);
            _eventManager.Subscribe<NotificationEvent>(OnNotification);
            _eventManager.Subscribe<UIControlStateChangedEvent>(OnUIControlStateChanged);
            _eventManager.Subscribe<MenuStateChangedEvent>(OnMenuStateChanged);
            _eventManager.Subscribe<ProgressUpdateEvent>(OnProgressUpdate);
            _eventManager.Subscribe<ThemeChangedEvent>(OnThemeChanged);
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
            
            // Unsubscribe from UI events
            _eventManager.ClearSubscriptions<DisplayTextEvent>();
            _eventManager.ClearSubscriptions<DisplayFormattedMessageEvent>();
            _eventManager.ClearSubscriptions<NotificationEvent>();
            _eventManager.ClearSubscriptions<UIControlStateChangedEvent>();
            _eventManager.ClearSubscriptions<MenuStateChangedEvent>();
            _eventManager.ClearSubscriptions<ProgressUpdateEvent>();
            _eventManager.ClearSubscriptions<ThemeChangedEvent>();
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
            _uiManager.DisplayText($"> {command}", Color.Yellow);
            _gameManager.ProcessCommand(command);
        }

        private void MiniMapControl_LocationClicked(object sender, string location)
        {
            _uiManager.DisplayText($"You want to travel to {location}. Use movement commands to get there.", Color.Cyan);
        }

        // Event handlers for new features
        private void ShowInventory(object sender, EventArgs e)
        {
            if (_gameManager.CurrentGameState != GameState.Running)
            {
                _uiManager.DisplayText("Start a new game first!", Color.Red, true);
                return;
            }

            try
            {
                var inventoryForm = _serviceProvider.GetRequiredService<InventoryForm>();
                inventoryForm.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening inventory form");
                _uiManager.ShowNotification("Error", "Failed to open inventory", NotificationType.Error);
            }
        }

        private void ShowMap(object sender, EventArgs e)
        {
            if (_gameManager.CurrentGameState != GameState.Running)
            {
                _uiManager.DisplayText("Start a new game first!", Color.Red, true);
                return;
            }

            try
            {
                var mapForm = _serviceProvider.GetRequiredService<MapForm>();
                mapForm.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening map form");
                _uiManager.ShowNotification("Error", "Failed to open map", NotificationType.Error);
            }
        }

        private void ShowEquipment(object sender, EventArgs e)
        {
            if (_gameManager.CurrentGameState != GameState.Running)
            {
                _uiManager.DisplayText("Start a new game first!", Color.Red, true);
                return;
            }

            _uiManager.ShowCharacterStats();
        }

        private void ShowLocationList(object sender, EventArgs e)
        {
            var locationLines = new List<FormattedTextLine>
            {
                new FormattedTextLine("=== Known Locations ===", Color.Cyan, true),
                new FormattedTextLine("• Village of Elderbrook - A peaceful starting town", Color.White),
                new FormattedTextLine("• Dark Forest - Dangerous woods with wolves", Color.White),
                new FormattedTextLine("• Grassy Plains - Open fields with wild boars", Color.White),
                new FormattedTextLine("• Ancient Cave - Mysterious cavern with treasures", Color.White),
                new FormattedTextLine("• Ancient Ruins - Crumbling structures with secrets", Color.White),
                new FormattedTextLine("• Dragon's Lair - The ultimate challenge", Color.White)
            };

            _uiManager.DisplayFormattedMessage("Locations", locationLines);
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
                    
                    _uiManager.DisplayText("Settings applied successfully!", Color.Green);
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
                var assetEditorForm = new AssetEditorForm();
                assetEditorForm.Show();
                _uiManager.DisplayText("Asset Editor opened. You can now edit game data files.", Color.Cyan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening asset editor");
                _uiManager.ShowNotification("Error", "Failed to open Asset Editor", NotificationType.Error);
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
            // Delegate to UIManager
            _uiManager.DisplayText(text, color);
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
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnGameStarted(e)));
                return;
            }

            // UIManager will handle the welcome message through its own event handler
            SetGameControlsEnabled(true);
            characterStatsControl?.UpdateStats(e.Player);
            progressDisplayControl?.UpdateHealth(e.Player.Health, e.Player.MaxHealth);
            progressDisplayControl?.UpdateExperience(e.Player.Experience, e.Player.ExperienceToNextLevel);
            gameStatusBarControl?.UpdateStatus($"Game started - {e.Player.Name} the {e.Player.CharacterClass}");
        }

        private void OnGameEnded(GameEndedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnGameEnded(e)));
                return;
            }

            // UIManager will handle the game ended message through its own event handler
            SetGameControlsEnabled(false);
            gameStatusBarControl?.UpdateStatus("Game ended");
        }

        private void OnPlayerStatsChanged(PlayerStatsChangedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnPlayerStatsChanged(e)));
                return;
            }

            // Update UI controls
            characterStatsControl?.UpdateStats(e.Player);
            
            // Update progress bars based on stat type
            switch (e.StatType)
            {
                case StatType.Health:
                case StatType.MaxHealth:
                    progressDisplayControl?.UpdateHealth(e.Player.Health, e.Player.MaxHealth);
                    break;
                case StatType.Experience:
                    progressDisplayControl?.UpdateExperience(e.Player.Experience, e.Player.ExperienceToNextLevel);
                    break;
            }

            // Show stat change notification for significant changes
            if (Math.Abs(e.NewValue - e.OldValue) > 0)
            {
                _uiManager.DisplayText($"{e.StatType} changed: {e.OldValue} → {e.NewValue}", Color.Cyan);
            }
        }

        private void OnPlayerLeveledUp(PlayerLeveledUpEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnPlayerLeveledUp(e)));
                return;
            }

            // UIManager will handle the level up message through its own event handler
            characterStatsControl?.UpdateStats(e.Player);
            progressDisplayControl?.UpdateExperience(e.Player.Experience, e.Player.ExperienceToNextLevel);
            
            // Update status bar
            gameStatusBarControl?.UpdateStatus($"Level Up! Now level {e.NewLevel}");
        }

        private void OnPlayerHealthChanged(PlayerHealthChangedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnPlayerHealthChanged(e)));
                return;
            }

            progressDisplayControl?.UpdateHealth(e.NewHealth, e.Player.MaxHealth);
            characterStatsControl?.UpdateStats(e.Player);

            var healthChange = e.NewHealth - e.OldHealth;
            if (healthChange > 0)
            {
                _uiManager.DisplayText($"Health restored: +{healthChange} ({e.Reason})", Color.Green);
            }
            else if (healthChange < 0)
            {
                _uiManager.DisplayText($"Health lost: {Math.Abs(healthChange)} ({e.Reason})", Color.Red);
                
                // Show warning for low health
                if (e.NewHealth <= e.Player.MaxHealth * 0.2)
                {
                    _uiManager.ShowNotification("Low Health", "Your health is critically low!", NotificationType.Warning, 5000);
                }
            }
        }

        private void OnPlayerGoldChanged(PlayerGoldChangedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnPlayerGoldChanged(e)));
                return;
            }

            characterStatsControl?.UpdateStats(e.Player);

            var goldChange = e.NewGold - e.OldGold;
            if (goldChange > 0)
            {
                _uiManager.DisplayText($"Gold gained: +{goldChange} ({e.Reason})", Color.Yellow);
            }
            else if (goldChange < 0)
            {
                _uiManager.DisplayText($"Gold spent: {Math.Abs(goldChange)} ({e.Reason})", Color.Orange);
            }
        }

        private void OnPlayerExperienceGained(PlayerExperienceGainedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnPlayerExperienceGained(e)));
                return;
            }

            progressDisplayControl?.UpdateExperience(e.Player.Experience, e.Player.ExperienceToNextLevel);
            characterStatsControl?.UpdateStats(e.Player);

            _uiManager.DisplayText($"Experience gained: +{e.ExperienceGained} ({e.Source})", Color.Cyan);
        }

        private void OnLocationChanged(LocationChangedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnLocationChanged(e)));
                return;
            }

            // UIManager will handle the location display through its own event handler
            miniMapControl?.UpdateCurrentLocation(e.NewLocation.Key);
            gameStatusBarControl?.UpdateStatus($"Current location: {e.NewLocation.Name}");
        }

        private void OnCombatStarted(CombatStartedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnCombatStarted(e)));
                return;
            }

            // UIManager will handle the combat started message through its own event handler
            SetCombatMode(true);
            gameStatusBarControl?.UpdateStatus($"In combat with {e.Enemy.Name}");
        }

        private void OnCombatEnded(CombatEndedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnCombatEnded(e)));
                return;
            }

            // UIManager will handle the combat ended message through its own event handler
            SetCombatMode(false);
            
            // Update character stats after combat
            if (e.Player != null)
            {
                characterStatsControl?.UpdateStats(e.Player);
                progressDisplayControl?.UpdateHealth(e.Player.Health, e.Player.MaxHealth);
                progressDisplayControl?.UpdateExperience(e.Player.Experience, e.Player.ExperienceToNextLevel);
            }

            gameStatusBarControl?.UpdateStatus("Combat ended");
        }

        private void OnInventoryUpdated(InventoryUpdatedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnInventoryUpdated(e)));
                return;
            }

            switch (e.Action)
            {
                case InventoryAction.ItemAdded:
                    _uiManager.DisplayText($"Added to inventory: {e.AffectedItem.Name} x{e.Quantity}", Color.Green);
                    break;
                case InventoryAction.ItemRemoved:
                    _uiManager.DisplayText($"Removed from inventory: {e.AffectedItem.Name} x{e.Quantity}", Color.Orange);
                    break;
            }
        }

        private void OnItemEquipped(ItemEquippedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnItemEquipped(e)));
                return;
            }

            _uiManager.DisplayText($"Equipped: {e.EquippedItem.Name}", Color.Green);
            characterStatsControl?.UpdateStats(e.Player);
        }

        private void OnItemUnequipped(ItemUnequippedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnItemUnequipped(e)));
                return;
            }

            _uiManager.DisplayText($"Unequipped: {e.UnequippedItem.Name}", Color.Orange);
            characterStatsControl?.UpdateStats(e.Player);
        }

        private void OnSkillLearned(SkillLearnedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnSkillLearned(e)));
                return;
            }

            if (e.WasSuccessful)
            {
                _uiManager.DisplayText($"Skill learned: {e.Skill.Name}!", Color.Magenta);
                _uiManager.DisplayText($"Skill points spent: {e.SkillPointsSpent}", Color.Cyan);
                _uiManager.DisplayText($"Remaining skill points: {e.RemainingSkillPoints}", Color.Cyan);
                characterStatsControl?.UpdateStats(e.Player);
            }
            else
            {
                _uiManager.DisplayText($"Failed to learn skill: {e.FailureReason}", Color.Red);
            }
        }

        private void OnSkillUsed(SkillUsedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnSkillUsed(e)));
                return;
            }

            if (e.WasSuccessful)
            {
                _uiManager.DisplayText($"Used skill: {e.Skill.Name}", Color.Magenta);
                if (e.SkillEffects.ContainsKey("description"))
                {
                    _uiManager.DisplayText($"Effect: {e.SkillEffects["description"]}", Color.Cyan);
                }
            }
            else
            {
                _uiManager.DisplayText($"Failed to use skill: {e.FailureReason}", Color.Red);
            }
        }

        private void OnCommandProcessed(CommandProcessedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnCommandProcessed(e)));
                return;
            }

            if (!e.Result.Success)
            {
                _uiManager.DisplayText($"Error: {e.Result.ErrorMessage}", Color.Red);
            }
        }

        private void OnGameMessage(GameMessageEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnGameMessage(e)));
                return;
            }

            var messageColor = e.MessageType switch
            {
                GameMessageType.Info => Color.White,
                GameMessageType.Success => Color.Green,
                GameMessageType.Warning => Color.Yellow,
                GameMessageType.Error => Color.Red,
                GameMessageType.Combat => Color.Orange,
                GameMessageType.System => Color.Cyan,
                _ => Color.White
            };

            _uiManager.DisplayText(e.Message, messageColor);
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
            if (_gameManager.CurrentGameState != GameState.Running)
            {
                _uiManager.DisplayText("Start a new game first!", Color.Red, true);
                return;
            }

            _uiManager.ShowCharacterStats();
        }

        private void ShowSkillTree(object sender, EventArgs e)
        {
            if (_gameManager.CurrentGameState != GameState.Running)
            {
                _uiManager.DisplayText("Start a new game first!", Color.Red, true);
                return;
            }

            try
            {
                var skillTreeForm = _serviceProvider.GetRequiredService<SkillTreeForm>();
                skillTreeForm.ShowDialog();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening skill tree form");
                _uiManager.ShowNotification("Error", "Failed to open skill tree", NotificationType.Error);
            }
        }

        private void ShowHelp()
        {
            _uiManager.ShowHelp("general");
        }

        // Advanced GameCoordinatorService demonstration methods

        private async void ValidateGameState(object sender, EventArgs e)
        {
            try
            {
                _uiManager.DisplayText("=== Validating Game State ===", Color.Cyan, true);
                var validation = await _gameCoordinator.ValidateGameStateAsync();

                var validationColor = validation.IsValid ? Color.Green : Color.Red;
                _uiManager.DisplayText($"Validation Result: {(validation.IsValid ? "VALID" : "INVALID")}", validationColor, true);

                if (validation.Issues.Count > 0)
                {
                    _uiManager.DisplayText($"\nFound {validation.Issues.Count} issues:", Color.Yellow);
                    
                    foreach (var issue in validation.Issues)
                    {
                        var color = issue.Severity switch
                        {
                            "Critical" => Color.Red,
                            "Warning" => Color.Orange,
                            "Info" => Color.Cyan,
                            _ => Color.White
                        };
                        _uiManager.DisplayText($"[{issue.Severity}] {issue.Manager}: {issue.Issue}", color);
                        _uiManager.DisplayText($"  Recommendation: {issue.Recommendation}", Color.Cyan);
                    }
                }
                else
                {
                    _uiManager.DisplayText("No issues found - game state is healthy!", Color.Green);
                }

                _uiManager.DisplayText($"\nManager States:", Color.Cyan);
                foreach (var state in validation.ManagerStates)
                {
                    var stateColor = state.Value ? Color.Green : Color.Red;
                    _uiManager.DisplayText($"  {state.Key}: {(state.Value ? "OK" : "ERROR")}", stateColor);
                }
            }
            catch (Exception ex)
            {
                _uiManager.DisplayText($"Error during validation: {ex.Message}", Color.Red, true);
                _logger.LogError(ex, "Error during game state validation");
            }
        }

        private async void ShowComprehensiveStatus(object sender, EventArgs e)
        {
            try
            {
                _uiManager.DisplayText("=== Comprehensive Game Status ===", Color.Cyan, true);
                var status = await _gameCoordinator.GetComprehensiveStatusAsync();

                _uiManager.DisplayText($"Game State: {status.GameState}", Color.White);
                _uiManager.DisplayText($"In Combat: {status.InCombat}", Color.White);
                _uiManager.DisplayText($"Current Location: {status.CurrentLocation ?? "None"}", Color.White);

                if (status.CurrentPlayer != null)
                {
                    _uiManager.DisplayText($"Player: {status.CurrentPlayer.Name} (Level {status.CurrentPlayer.Level})", Color.Green);
                    _uiManager.DisplayText($"Health: {status.CurrentPlayer.Health}/{status.CurrentPlayer.MaxHealth}", Color.White);
                    _uiManager.DisplayText($"Gold: {status.CurrentPlayer.Gold}", Color.Yellow);
                }

                if (status.Statistics != null)
                {
                    _uiManager.DisplayText($"\nSession Statistics:", Color.Cyan);
                    _uiManager.DisplayText($"  Play Time: {status.Statistics.TotalPlayTime:F1} minutes", Color.White);
                    _uiManager.DisplayText($"  Enemies Defeated: {status.Statistics.EnemiesDefeated}", Color.White);
                    _uiManager.DisplayText($"  Items Collected: {status.Statistics.ItemsCollected}", Color.White);
                    _uiManager.DisplayText($"  Locations Visited: {status.Statistics.LocationsVisited}", Color.White);
                }

                if (status.ActiveEffects?.Count > 0)
                {
                    _uiManager.DisplayText($"\nActive Effects:", Color.Magenta);
                    foreach (var effect in status.ActiveEffects)
                    {
                        _uiManager.DisplayText($"  • {effect}", Color.White);
                    }
                }

                _uiManager.DisplayText($"\nManager Statuses:", Color.Cyan);
                foreach (var managerStatus in status.ManagerStatuses)
                {
                    _uiManager.DisplayText($"  {managerStatus.Key}: {managerStatus.Value}", Color.White);
                }
            }
            catch (Exception ex)
            {
                _uiManager.DisplayText($"Error getting comprehensive status: {ex.Message}", Color.Red, true);
                _logger.LogError(ex, "Error getting comprehensive status");
            }
        }

        private async void SynchronizeManagers(object sender, EventArgs e)
        {
            try
            {
                _uiManager.DisplayText("=== Synchronizing Managers ===", Color.Cyan, true);
                var success = await _gameCoordinator.SynchronizeManagersAsync();

                if (success)
                {
                    _uiManager.DisplayText("Manager synchronization completed successfully!", Color.Green, true);
                    _uiManager.DisplayText("All manager data is now synchronized.", Color.White);
                }
                else
                {
                    _uiManager.DisplayText("Manager synchronization failed.", Color.Red, true);
                    _uiManager.DisplayText("Check logs for detailed error information.", Color.Yellow);
                }
            }
            catch (Exception ex)
            {
                _uiManager.DisplayText($"Error during synchronization: {ex.Message}", Color.Red, true);
                _logger.LogError(ex, "Error synchronizing managers");
            }
        }

        private async void PerformMaintenance(object sender, EventArgs e)
        {
            try
            {
                _uiManager.DisplayText("=== Performing Automated Maintenance ===", Color.Cyan, true);
                var result = await _gameCoordinator.PerformMaintenanceAsync();

                if (result.Success)
                {
                    _uiManager.DisplayText("Maintenance completed successfully!", Color.Green, true);
                    _uiManager.DisplayText($"Duration: {result.Duration.TotalMilliseconds:F0}ms", Color.Cyan);
                    
                    if (result.OperationsPerformed?.Count > 0)
                    {
                        _uiManager.DisplayText("\nOperations performed:", Color.Cyan);
                        foreach (var operation in result.OperationsPerformed)
                        {
                            _uiManager.DisplayText($"  ✓ {operation}", Color.Green);
                        }
                    }

                    if (result.Results?.Count > 0)
                    {
                        _uiManager.DisplayText("\nMaintenance Results:", Color.Cyan);
                        foreach (var detail in result.Results)
                        {
                            _uiManager.DisplayText($"  • {detail.Key}: {detail.Value}", Color.White);
                        }
                    }
                }
                else
                {
                    _uiManager.DisplayText("Maintenance failed!", Color.Red, true);
                    if (result.Results?.ContainsKey("Error") == true)
                    {
                        _uiManager.DisplayText($"Error: {result.Results["Error"]}", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                _uiManager.DisplayText($"Error during maintenance: {ex.Message}", Color.Red, true);
                _logger.LogError(ex, "Error performing maintenance");
            }
        }

        private async void LevelUpWithRewards(object sender, EventArgs e)
        {
            try
            {
                if (_playerManager.CurrentPlayer == null)
                {
                    _uiManager.DisplayText("No player loaded. Start a new game first!", Color.Red, true);
                    return;
                }

                _uiManager.DisplayText("=== Level Up with Rewards ===", Color.Cyan, true);
                var parameters = new Dictionary<string, object>();
                var result = await _gameCoordinator.PerformCoordinatedActionAsync("levelup_with_rewards", parameters);

                if (result.Success)
                {
                    _uiManager.DisplayText(result.Message, Color.Green, true);
                    
                    if (result.Results.ContainsKey("OldLevel") && result.Results.ContainsKey("NewLevel"))
                    {
                        _uiManager.DisplayText($"Level progression: {result.Results["OldLevel"]} → {result.Results["NewLevel"]}", Color.Magenta, true);
                    }
                    
                    if (result.Results.ContainsKey("GoldReward"))
                    {
                        _uiManager.DisplayText($"Gold reward: {result.Results["GoldReward"]}", Color.Yellow, true);
                    }
                }
                else
                {
                    _uiManager.DisplayText("Level up failed:", Color.Red, true);
                    foreach (var error in result.Errors)
                    {
                        _uiManager.DisplayText($"  • {error}", Color.Red, true);
                    }
                }

                if (result.Warnings.Any())
                {
                    foreach (var warning in result.Warnings)
                    {
                        _uiManager.DisplayText($"Warning: {warning}", Color.Yellow, true);
                    }
                }
            }
            catch (Exception ex)
            {
                _uiManager.DisplayText($"Error during level up: {ex.Message}", Color.Red, true);
                _logger.LogError(ex, "Error performing level up with rewards");
            }
        }

        private async void AutoExplore(object sender, EventArgs e)
        {
            try
            {
                if (_playerManager.CurrentPlayer == null)
                {
                    _uiManager.DisplayText("No player loaded. Start a new game first!", Color.Red, true);
                    return;
                }

                _uiManager.DisplayText("=== Auto Explore ===", Color.Cyan, true);
                var result = await _gameCoordinator.ExecuteComplexCommandAsync("auto-explore");

                if (result.Success)
                {
                    _uiManager.DisplayText(result.Message, Color.Green, true);
                }
                else
                {
                    _uiManager.DisplayText($"Auto explore failed: {result.ErrorMessage}", Color.Red, true);
                }
            }
            catch (Exception ex)
            {
                _uiManager.DisplayText($"Error during auto explore: {ex.Message}", Color.Red, true);
                _logger.LogError(ex, "Error performing auto explore");
            }
        }

        private async void OptimizeCharacter(object sender, EventArgs e)
        {
            try
            {
                if (_playerManager.CurrentPlayer == null)
                {
                    _uiManager.DisplayText("No player loaded. Start a new game first!", Color.Red, true);
                    return;
                }

                _uiManager.DisplayText("=== Optimize Character ===", Color.Cyan, true);
                var result = await _gameCoordinator.ExecuteComplexCommandAsync("optimize-character");

                if (result.Success)
                {
                    _uiManager.DisplayText(result.Message, Color.Green, true);
                }
                else
                {
                    _uiManager.DisplayText($"Character optimization failed: {result.ErrorMessage}", Color.Red, true);
                }
            }
            catch (Exception ex)
            {
                _uiManager.DisplayText($"Error during character optimization: {ex.Message}", Color.Red, true);
                _logger.LogError(ex, "Error optimizing character");
            }
        }

        // UI Event Handlers
        private void OnDisplayText(DisplayTextEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnDisplayText(e)));
                return;
            }

            if (e.Category == "Clear")
            {
                gameTextDisplayControl?.ClearText();
            }
            else
            {
                gameTextDisplayControl?.DisplayText(e.Text, e.Color);
            }
        }

        private void OnDisplayFormattedMessage(DisplayFormattedMessageEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnDisplayFormattedMessage(e)));
                return;
            }

            if (e.ClearPrevious)
            {
                gameTextDisplayControl?.ClearText();
            }

            if (!string.IsNullOrEmpty(e.Title))
            {
                gameTextDisplayControl?.DisplayText($"=== {e.Title} ===", Color.Cyan);
            }

            foreach (var line in e.Lines)
            {
                var color = line.Color ?? Color.White;
                gameTextDisplayControl?.DisplayText(line.Text, color);
            }
        }

        private void OnNotification(NotificationEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnNotification(e)));
                return;
            }

            var color = e.Type switch
            {
                NotificationType.Success => Color.Green,
                NotificationType.Warning => Color.Orange,
                NotificationType.Error => Color.Red,
                NotificationType.Critical => Color.DarkRed,
                _ => Color.White
            };

            // Show in status bar
            gameStatusBarControl?.UpdateStatus($"{e.Title}: {e.Message}");

            // Also show in main text area for important notifications
            if (e.Type >= NotificationType.Warning || e.RequiresUserAction)
            {
                gameTextDisplayControl?.DisplayText($"[{e.Type.ToString().ToUpper()}] {e.Title}: {e.Message}", color);
            }

            // For critical errors, show message box
            if (e.Type == NotificationType.Critical && e.RequiresUserAction)
            {
                MessageBox.Show(e.Message, e.Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnUIControlStateChanged(UIControlStateChangedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnUIControlStateChanged(e)));
                return;
            }

            // Handle control state changes
            switch (e.ControlName)
            {
                case "GameControls":
                    SetGameControlsEnabled(e.IsEnabled);
                    break;
                // Add more control state handling as needed
            }
        }

        private void OnMenuStateChanged(MenuStateChangedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnMenuStateChanged(e)));
                return;
            }

            // Handle menu state changes
            if (menuStrip != null)
            {
                foreach (ToolStripMenuItem mainMenu in menuStrip.Items)
                {
                    if (e.MenuItemStates.ContainsKey(mainMenu.Text))
                    {
                        mainMenu.Enabled = e.MenuItemStates[mainMenu.Text];
                    }

                    // Handle submenu items
                    foreach (ToolStripItem subItem in mainMenu.DropDownItems)
                    {
                        if (e.MenuItemStates.ContainsKey(subItem.Text))
                        {
                            subItem.Enabled = e.MenuItemStates[subItem.Text];
                        }
                    }
                }
            }
        }

        private void OnProgressUpdate(ProgressUpdateEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnProgressUpdate(e)));
                return;
            }

            // Update progress displays based on progress bar name
            switch (e.ProgressBarName.ToLower())
            {
                case "health":
                    progressDisplayControl?.UpdateHealth(e.CurrentValue, e.MaxValue);
                    break;
                case "experience":
                    progressDisplayControl?.UpdateExperience(e.CurrentValue, e.MaxValue);
                    break;
                default:
                    // For other progress bars, we don't have a generic update method
                    // so we'll just update the health or experience based on the label
                    if (e.Label?.ToLower().Contains("health") == true)
                        progressDisplayControl?.UpdateHealth(e.CurrentValue, e.MaxValue);
                    else if (e.Label?.ToLower().Contains("experience") == true)
                        progressDisplayControl?.UpdateExperience(e.CurrentValue, e.MaxValue);
                    break;
            }
        }

        private void OnThemeChanged(ThemeChangedEvent e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => OnThemeChanged(e)));
                return;
            }

            ApplyTheme(e.ThemeName);
        }
    }
}
