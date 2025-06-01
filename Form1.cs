using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using WinFormsApp1.Events;
using WinFormsApp1.Controls;

namespace WinFormsApp1
{
    public partial class Form1 : Form, IMessageEventHandler, IPlayerEventHandler, ICombatEventHandler, IGameStateEventHandler
    {
        // Event system components (from root Form1.cs)
        private GameEngine gameEngine;
        private GameEventManager eventManager;
        private List<string> commandHistory = new List<string>();
        private int historyIndex = -1;
        private bool isInCombatMode = false;
        private System.Windows.Forms.Timer experienceTimer;
        private System.Windows.Forms.Timer goldTimer;
        private System.Windows.Forms.Timer levelUpTimer;

        // Advanced UI components (from src/Forms/Form1.cs)
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

        // UI elements that should be disabled until game starts
        private List<Control> gameRequiredControls;
        private List<ToolStripItem> gameRequiredMenuItems;
        private List<ToolStripItem> gameRequiredToolbarItems;

        // Legacy controls for backward compatibility
        private RichTextBox gameTextBox;
        private TextBox commandTextBox;
        private Button newGameButton;
        private Button loadGameButton;
        private Button saveGameButton;
        private Button inventoryButton;
        private Button statsButton;
        private Button skillsButton;
        private Button helpButton;
        private ToolStripStatusLabel toolStripStatusLabel;

        public Form1()
        {
            InitializeComponent();
            
            // Initialize event system first
            eventManager = GameEventManager.Instance;
            
            // Initialize advanced UI
            InitializeGameUI();
            
            // Initialize game engine
            gameEngine = new GameEngine(this);
            
            // Subscribe to events
            eventManager.SubscribeToMessageEvents(this);
            eventManager.SubscribeToPlayerEvents(this);
            eventManager.SubscribeToCombatEvents(this);
            eventManager.SubscribeToGameStateEvents(this);
            
            // Initialize timers for visual effects
            InitializeTimers();
            
            // Set initial state
            EnableGameControls(false);
            DisplayText("Welcome to Realm of Aethermoor!", Color.Cyan);
            DisplayText("Click 'New Game' to begin your adventure.", Color.Yellow);
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
            gameInputControl?.FocusInput();
        }

        private void InitializeTimers()
        {
            experienceTimer = new System.Windows.Forms.Timer();
            experienceTimer.Interval = 2000; // 2 seconds
            experienceTimer.Tick += (s, e) => { experienceTimer.Stop(); };

            goldTimer = new System.Windows.Forms.Timer();
            goldTimer.Interval = 2000; // 2 seconds
            goldTimer.Tick += (s, e) => { goldTimer.Stop(); };

            levelUpTimer = new System.Windows.Forms.Timer();
            levelUpTimer.Interval = 3000; // 3 seconds
            levelUpTimer.Tick += (s, e) => { levelUpTimer.Stop(); };
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
            foreach (ToolStripItem item in toolStrip.Items)
            {
                if (item.Text == "Save" || item.Text == "Load" || item.Text == "Inventory" || item.Text == "Map" || item.Text == "Skills")
                {
                    item.Enabled = enabled;
                }
            }

            // Find and manage menu items
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

            // Manage custom controls
            gameInputControl.InputEnabled = enabled;
            quickActionsControl?.SetButtonsEnabled(enabled, "Stats", "Save", "Load");
            miniMapControl?.SetMapEnabled(enabled);

            // Legacy controls for backward compatibility
            if (commandTextBox != null) commandTextBox.Enabled = enabled;
            if (saveGameButton != null) saveGameButton.Enabled = enabled;
            if (inventoryButton != null) inventoryButton.Enabled = enabled;
            if (statsButton != null) statsButton.Enabled = enabled;
            if (skillsButton != null) skillsButton.Enabled = enabled;
        }

        private void CreateMenuStrip()
        {
            menuStrip = new MenuStrip();

            // Game menu
            ToolStripMenuItem gameMenu = new ToolStripMenuItem("&Game");
            gameMenu.DropDownItems.Add("&New Game", null, (s, e) => gameEngine.StartNewGame());
            gameMenu.DropDownItems.Add(new ToolStripSeparator());
            gameMenu.DropDownItems.Add("&Save Game", null, (s, e) => gameEngine.SaveGame());
            gameMenu.DropDownItems.Add("&Load Game", null, (s, e) => gameEngine.LoadGame());
            gameMenu.DropDownItems.Add(new ToolStripSeparator());
            gameMenu.DropDownItems.Add("E&xit", null, (s, e) => this.Close());

            // Character menu
            ToolStripMenuItem characterMenu = new ToolStripMenuItem("&Character");
            characterMenu.DropDownItems.Add("&Inventory", null, ShowInventory);
            characterMenu.DropDownItems.Add("&Stats", null, (s, e) => gameEngine.ShowCharacterStats());
            characterMenu.DropDownItems.Add("S&kills", null, (s, e) => gameEngine.ShowSkillTree());
            characterMenu.DropDownItems.Add("&Equipment", null, ShowEquipment);

            // World menu
            ToolStripMenuItem worldMenu = new ToolStripMenuItem("&World");
            worldMenu.DropDownItems.Add("&Map", null, ShowMap);
            worldMenu.DropDownItems.Add("&Locations", null, ShowLocationList);

            // Settings menu
            ToolStripMenuItem settingsMenu = new ToolStripMenuItem("&Settings");
            settingsMenu.DropDownItems.Add("&Themes", null, ShowThemeSelection);
            settingsMenu.DropDownItems.Add("&Font Size", null, ShowFontSettings);
            settingsMenu.DropDownItems.Add("&Game Settings", null, ShowGameSettings);

            // Tools menu
            ToolStripMenuItem toolsMenu = new ToolStripMenuItem("&Tools");
            toolsMenu.DropDownItems.Add("&Asset Editor", null, ShowAssetEditor);
            toolsMenu.DropDownItems.Add(new ToolStripSeparator());
            toolsMenu.DropDownItems.Add("&Data Validation", null, ShowDataValidation);

            // Help menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.Add("&Commands", null, (s, e) => gameEngine.ShowHelp());
            helpMenu.DropDownItems.Add("&Controls", null, ShowControlsHelp);
            helpMenu.DropDownItems.Add(new ToolStripSeparator());
            helpMenu.DropDownItems.Add("&About", null, ShowAbout);

            menuStrip.Items.AddRange(new ToolStripItem[] {
                gameMenu, characterMenu, worldMenu, settingsMenu, toolsMenu, helpMenu
            });
        }

        private void CreateToolStrip()
        {
            toolStrip = new ToolStrip
            {
                ImageScalingSize = new Size(24, 24)
            };

            // Quick action buttons
            ToolStripButton newGameToolButton = new ToolStripButton("New Game")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Start a new game"
            };
            newGameToolButton.Click += (s, e) => gameEngine.StartNewGame();

            ToolStripButton saveButton = new ToolStripButton("Save")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Save game (Ctrl+S)"
            };
            saveButton.Click += (s, e) => gameEngine.SaveGame();

            ToolStripButton loadButton = new ToolStripButton("Load")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Load game (Ctrl+L)"
            };
            loadButton.Click += (s, e) => gameEngine.LoadGame();

            ToolStripSeparator separator1 = new ToolStripSeparator();

            ToolStripButton inventoryToolButton = new ToolStripButton("Inventory")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Open inventory (Tab)"
            };
            inventoryToolButton.Click += ShowInventory;

            ToolStripButton mapButton = new ToolStripButton("Map")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Show world map (M)"
            };
            mapButton.Click += ShowMap;

            ToolStripButton skillsToolButton = new ToolStripButton("Skills")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Open skill tree"
            };
            skillsToolButton.Click += (s, e) => gameEngine.ShowSkillTree();

            ToolStripSeparator separator2 = new ToolStripSeparator();

            ToolStripButton helpToolButton = new ToolStripButton("Help")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Show help (F1)"
            };
            helpToolButton.Click += (s, e) => gameEngine.ShowHelp();

            toolStrip.Items.AddRange(new ToolStripItem[] {
                newGameToolButton, saveButton, loadButton, separator1,
                inventoryToolButton, mapButton, skillsToolButton, separator2, helpToolButton
            });
        }

        private Panel CreateGamePanel()
        {
            Panel gamePanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            // Create layout for game area
            TableLayoutPanel gameLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };
            gameLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 85F));
            gameLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));

            // Create game text display control
            gameTextDisplayControl = new GameTextDisplayControl
            {
                Dock = DockStyle.Fill
            };

            // Create game input control
            gameInputControl = new GameInputControl
            {
                Dock = DockStyle.Fill
            };
            gameInputControl.CommandSubmitted += GameInputControl_CommandSubmitted;
            gameInputControl.InventoryRequested += (s, e) => ShowInventory(s, e);
            gameInputControl.HelpRequested += (s, e) => gameEngine?.ShowHelp();

            // Add to layout
            gameLayout.Controls.Add(gameTextDisplayControl, 0, 0);
            gameLayout.Controls.Add(gameInputControl, 0, 1);

            gamePanel.Controls.Add(gameLayout);

            // Create legacy controls for backward compatibility (hidden)
            CreateLegacyControls(gamePanel);

            return gamePanel;
        }

        private void CreateLegacyControls(Panel parent)
        {
            // Create hidden legacy controls for backward compatibility
            gameTextBox = new RichTextBox
            {
                Visible = false,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),
                ReadOnly = true
            };

            commandTextBox = new TextBox
            {
                Visible = false,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 10)
            };
            commandTextBox.KeyDown += commandTextBox_KeyDown;

            // Create hidden buttons
            newGameButton = new Button { Visible = false };
            newGameButton.Click += newGameButton_Click;

            loadGameButton = new Button { Visible = false };
            loadGameButton.Click += loadGameButton_Click;

            saveGameButton = new Button { Visible = false };
            saveGameButton.Click += saveGameButton_Click;

            inventoryButton = new Button { Visible = false };
            inventoryButton.Click += inventoryButton_Click;

            statsButton = new Button { Visible = false };
            statsButton.Click += statsButton_Click;

            skillsButton = new Button { Visible = false };
            skillsButton.Click += skillsButton_Click;

            helpButton = new Button { Visible = false };
            helpButton.Click += helpButton_Click;

            parent.Controls.AddRange(new Control[] {
                gameTextBox, commandTextBox, newGameButton, loadGameButton,
                saveGameButton, inventoryButton, statsButton, skillsButton, helpButton
            });
        }

        private void CreateSidePanel()
        {
            sidePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            // Create layout for side panel
            TableLayoutPanel sideLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 4,
                ColumnCount = 1,
                Padding = new Padding(5)
            };
            sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));

            // Character stats control
            characterStatsControl = new CharacterStatsControl
            {
                Dock = DockStyle.Fill
            };

            // Progress display control
            progressDisplayControl = new ProgressDisplayControl
            {
                Dock = DockStyle.Fill
            };

            // Quick actions control
            quickActionsControl = new QuickActionsControl
            {
                Dock = DockStyle.Fill
            };
            quickActionsControl.ActionClicked += QuickActionsControl_ActionClicked;

            // Mini map control
            miniMapControl = new MiniMapControl
            {
                Dock = DockStyle.Fill
            };
            miniMapControl.LocationClicked += MiniMapControl_LocationClicked;

            // Add to layout
            sideLayout.Controls.Add(characterStatsControl, 0, 0);
            sideLayout.Controls.Add(progressDisplayControl, 0, 1);
            sideLayout.Controls.Add(quickActionsControl, 0, 2);
            sideLayout.Controls.Add(miniMapControl, 0, 3);

            sidePanel.Controls.Add(sideLayout);
        }

        //private void CreateSidePanel()
        //{
        //    sidePanel = new Panel
        //    {
        //        Dock = DockStyle.Fill,
        //        BackColor = Color.LightGray,
        //        Padding = new Padding(5)
        //    };

        //    TableLayoutPanel sideLayout = new TableLayoutPanel
        //    {
        //        Dock = DockStyle.Fill,
        //        ColumnCount = 1,
        //        RowCount = 4
        //    };
        //    sideLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //    sideLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        //    sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        //    sideLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        //    // Character stats panel using custom control
        //    GroupBox statsGroup = new GroupBox
        //    {
        //        Text = "Character Stats",
        //        Dock = DockStyle.Fill,
        //        Height = 120,
        //        Padding = new Padding(5)
        //    };
        //    characterStatsControl = new CharacterStatsControl
        //    {
        //        Dock = DockStyle.Fill
        //    };
        //    statsGroup.Controls.Add(characterStatsControl);
        //    sideLayout.Controls.Add(statsGroup, 0, 0);

        //    // Progress bars panel using custom control
        //    GroupBox progressGroup = new GroupBox
        //    {
        //        Text = "Status",
        //        Dock = DockStyle.Fill,
        //        Height = 80,
        //        Padding = new Padding(5)
        //    };
        //    progressDisplayControl = new ProgressDisplayControl
        //    {
        //        Dock = DockStyle.Fill
        //    };
        //    progressGroup.Controls.Add(progressDisplayControl);
        //    sideLayout.Controls.Add(progressGroup, 0, 1);

        //    // Mini map panel using custom control
        //    GroupBox miniMapGroup = new GroupBox
        //    {
        //        Text = "Mini Map",
        //        Dock = DockStyle.Fill,
        //        Padding = new Padding(5)
        //    };
        //    miniMapControl = new MiniMapControl
        //    {
        //        Dock = DockStyle.Fill
        //    };
        //    miniMapControl.FullMapRequested += (s, e) => ShowMap(s, e);
        //    miniMapControl.LocationClicked += MiniMapControl_LocationClicked;
        //    miniMapGroup.Controls.Add(miniMapControl);
        //    sideLayout.Controls.Add(miniMapGroup, 0, 2);

        //    // Quick actions panel using custom control
        //    GroupBox actionsGroup = new GroupBox
        //    {
        //        Text = "Quick Actions",
        //        Dock = DockStyle.Fill,
        //        Padding = new Padding(5)
        //    };
        //    quickActionsControl = new QuickActionsControl
        //    {
        //        Dock = DockStyle.Fill
        //    };
        //    quickActionsControl.ActionClicked += QuickActionsControl_ActionClicked;
        //    actionsGroup.Controls.Add(quickActionsControl);
        //    sideLayout.Controls.Add(actionsGroup, 0, 3);

        //    sidePanel.Controls.Add(sideLayout);
        //}

        private void CreateStatusStrip()
        {
            gameStatusBarControl = new GameStatusBarControl
            {
                Dock = DockStyle.Bottom
            };

            // Create legacy status label for backward compatibility
            toolStripStatusLabel = new ToolStripStatusLabel("Ready");
        }

        private void SetupKeyboardShortcuts()
        {
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Handle keyboard shortcuts
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.S:
                        gameEngine?.SaveGame();
                        e.Handled = true;
                        break;
                    case Keys.L:
                        gameEngine?.LoadGame();
                        e.Handled = true;
                        break;
                    case Keys.N:
                        gameEngine?.StartNewGame();
                        e.Handled = true;
                        break;
                }
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.F1:
                        gameEngine?.ShowHelp();
                        e.Handled = true;
                        break;
                    case Keys.Tab:
                        ShowInventory(this, EventArgs.Empty);
                        e.Handled = true;
                        break;
                    case Keys.M:
                        ShowMap(this, EventArgs.Empty);
                        e.Handled = true;
                        break;
                }
            }
        }

        private void GameInputControl_CommandSubmitted(object sender, string command)
        {
            DisplayText($"> {command}", Color.Yellow);
            gameEngine?.ProcessCommand(command);
        }

        private void MiniMapControl_LocationClicked(object sender, string location)
        {
            gameEngine?.ProcessCommand($"teleport {location}");
        }

        // Event Handler Implementations (from root Form1.cs)
        
        // IMessageEventHandler implementation
        public void OnMessageDisplayed(object? sender, MessageEventArgs e)
        {
            DisplayText(e.Message, e.Color);
        }

        // IPlayerEventHandler implementation
        public void OnPlayerLeveledUp(object? sender, LevelUpEventArgs e)
        {
            ShowLevelUp(e.OldLevel, e.NewLevel);
            
            // Flash the form background briefly
            var originalColor = this.BackColor;
            this.BackColor = Color.Gold;
            levelUpTimer.Start();
            levelUpTimer.Tick += (s, args) =>
            {
                this.BackColor = originalColor;
                levelUpTimer.Stop();
            };

            // Update character stats display
            characterStatsControl?.UpdateStats(e.Player);
        }

        public void OnPlayerGainedExperience(object? sender, ExperienceGainEventArgs e)
        {
            ShowExperienceGain(e.ExperienceGained);
            progressDisplayControl?.UpdateExperience(e.Player.Experience, e.Player.ExperienceToNextLevel);
        }

        public void OnPlayerGoldChanged(object? sender, GoldChangeEventArgs e)
        {
            ShowGoldChange(e.GoldChange);
            characterStatsControl?.UpdateStats(e.Player);
        }

        public void OnPlayerHealthChanged(object? sender, HealthChangeEventArgs e)
        {
            // Update status bar immediately
            UpdateStatus($"Health: {e.CurrentHealth}/{e.MaxHealth} | Level: {e.Player.Level} | Gold: {e.Player.Gold}");
            
            // Update character stats and progress displays
            characterStatsControl?.UpdateStats(e.Player);
            progressDisplayControl?.UpdateHealth(e.CurrentHealth, e.MaxHealth);
            
            // Visual feedback for health changes
            if (e.HealthChange < 0)
            {
                // Flash red for damage
                var originalColor = gameTextDisplayControl?.BackColor ?? Color.Black;
                if (gameTextDisplayControl != null)
                {
                    gameTextDisplayControl.BackColor = Color.DarkRed;
                    var timer = new System.Windows.Forms.Timer { Interval = 200 };
                    timer.Tick += (s, args) =>
                    {
                        gameTextDisplayControl.BackColor = originalColor;
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();
                }

                // Legacy support
                if (gameTextBox != null)
                {
                    var legacyOriginalColor = gameTextBox.BackColor;
                    gameTextBox.BackColor = Color.DarkRed;
                    var legacyTimer = new System.Windows.Forms.Timer { Interval = 200 };
                    legacyTimer.Tick += (s, args) =>
                    {
                        gameTextBox.BackColor = legacyOriginalColor;
                        legacyTimer.Stop();
                        legacyTimer.Dispose();
                    };
                    legacyTimer.Start();
                }
            }
            else if (e.HealthChange > 0)
            {
                // Flash green for healing
                var originalColor = gameTextDisplayControl?.BackColor ?? Color.Black;
                if (gameTextDisplayControl != null)
                {
                    gameTextDisplayControl.BackColor = Color.DarkGreen;
                    var timer = new System.Windows.Forms.Timer { Interval = 200 };
                    timer.Tick += (s, args) =>
                    {
                        gameTextDisplayControl.BackColor = originalColor;
                        timer.Stop();
                        timer.Dispose();
                    };
                    timer.Start();
                }

                // Legacy support
                if (gameTextBox != null)
                {
                    var legacyOriginalColor = gameTextBox.BackColor;
                    gameTextBox.BackColor = Color.DarkGreen;
                    var legacyTimer = new System.Windows.Forms.Timer { Interval = 200 };
                    legacyTimer.Tick += (s, args) =>
                    {
                        gameTextBox.BackColor = legacyOriginalColor;
                        legacyTimer.Stop();
                        legacyTimer.Dispose();
                    };
                    legacyTimer.Start();
                }
            }
        }

        public void OnPlayerLearnedSkill(object? sender, SkillEventArgs e)
        {
            // Visual feedback for skill learning
            DisplayText($"🌟 Skill Learned: {e.SkillName}!", Color.Cyan);
            characterStatsControl?.UpdateStats(e.Player);
        }

        // ICombatEventHandler implementation
        public void OnCombatStarted(object? sender, CombatEventArgs e)
        {
            SetCombatMode(true);
            
            // Visual feedback for combat start
            var originalColor = this.BackColor;
            this.BackColor = Color.DarkRed;
            var timer = new System.Windows.Forms.Timer { Interval = 500 };
            timer.Tick += (s, args) =>
            {
                this.BackColor = originalColor;
                timer.Stop();
                timer.Dispose();
            };
            timer.Start();
        }

        public void OnCombatActionPerformed(object? sender, CombatActionEventArgs e)
        {
            if (e.IsCritical && e.IsPlayerAction)
            {
                // Special visual effect for critical hits
                DisplayText("💥 CRITICAL HIT! 💥", Color.Yellow);
                
                // Flash the screen
                var originalColor = this.BackColor;
                this.BackColor = Color.Yellow;
                var timer = new System.Windows.Forms.Timer { Interval = 300 };
                timer.Tick += (s, args) =>
                {
                    this.BackColor = originalColor;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
        }

        public void OnCombatEnded(object? sender, CombatEndEventArgs e)
        {
            SetCombatMode(false);
            
            if (e.PlayerWon)
            {
                // Victory flash
                var originalColor = this.BackColor;
                this.BackColor = Color.Green;
                var timer = new System.Windows.Forms.Timer { Interval = 1000 };
                timer.Tick += (s, args) =>
                {
                    this.BackColor = originalColor;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
            else
            {
                // Defeat flash
                var originalColor = this.BackColor;
                this.BackColor = Color.Red;
                var timer = new System.Windows.Forms.Timer { Interval = 1000 };
                timer.Tick += (s, args) =>
                {
                    this.BackColor = originalColor;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
        }

        // IGameStateEventHandler implementation
        public void OnGameStateChanged(object? sender, GameStateEventArgs e)
        {
            // Handle game state changes if needed
        }

        public void OnGameSaved(object? sender, SaveGameEventArgs e)
        {
            if (e.WasSuccessful)
            {
                // Visual feedback for successful save
                var originalText = this.Text;
                this.Text = $"{originalText} - SAVED";
                var timer = new System.Windows.Forms.Timer { Interval = 2000 };
                timer.Tick += (s, args) =>
                {
                    this.Text = originalText;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
        }

        public void OnGameLoaded(object? sender, LoadGameEventArgs e)
        {
            if (e.WasSuccessful)
            {
                EnableGameControls(true);
                
                // Update all UI components with loaded player data
                if (e.LoadedPlayer != null)
                {
                    characterStatsControl?.UpdateStats(e.LoadedPlayer);
                    progressDisplayControl?.UpdateExperience(e.LoadedPlayer.Experience, e.LoadedPlayer.ExperienceToNextLevel);
                    progressDisplayControl?.UpdateHealth(e.LoadedPlayer.Health, e.LoadedPlayer.MaxHealth);
                }
                
                // Visual feedback for successful load
                var originalText = this.Text;
                this.Text = $"{originalText} - LOADED";
                var timer = new System.Windows.Forms.Timer { Interval = 2000 };
                timer.Tick += (s, args) =>
                {
                    this.Text = originalText;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
        }

        public void OnCommandExecuted(object? sender, CommandEventArgs e)
        {
            // Add command to history
            if (!string.IsNullOrWhiteSpace(e.Command))
            {
                commandHistory.Add(e.Command + (e.Arguments.Length > 0 ? " " + string.Join(" ", e.Arguments) : ""));
                historyIndex = commandHistory.Count;
            }
        }

        public void OnCheatActivated(object? sender, CheatEventArgs e)
        {
            if (e.WasSuccessful)
            {
                // Visual feedback for cheat activation
                var originalColor = this.BackColor;
                this.BackColor = Color.Magenta;
                var timer = new System.Windows.Forms.Timer { Interval = 500 };
                timer.Tick += (s, args) =>
                {
                    this.BackColor = originalColor;
                    timer.Stop();
                    timer.Dispose();
                };
                timer.Start();
            }
        }

        // UI Methods and Event Handlers

        public void DisplayText(string text, Color? color = null)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, Color?>(DisplayText), text, color);
                return;
            }

            // Use modern control if available
            if (gameTextDisplayControl != null)
            {
                gameTextDisplayControl.AppendText(text, color ?? Color.White);
            }
            
            // Legacy support
            if (gameTextBox != null)
            {
                gameTextBox.SelectionStart = gameTextBox.TextLength;
                gameTextBox.SelectionLength = 0;
                gameTextBox.SelectionColor = color ?? Color.White;
                gameTextBox.AppendText(text + Environment.NewLine);
                gameTextBox.SelectionColor = gameTextBox.ForeColor;
                gameTextBox.ScrollToCaret();
            }
        }

        public void EnableGameControls(bool enabled)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(EnableGameControls), enabled);
                return;
            }

            SetGameControlsEnabled(enabled);
        }

        public void UpdateStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateStatus), status);
                return;
            }

            gameStatusBarControl?.UpdateStatus(status);
            if (toolStripStatusLabel != null)
            {
                toolStripStatusLabel.Text = status;
            }
        }

        public void SetCombatMode(bool inCombat)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(SetCombatMode), inCombat);
                return;
            }

            isInCombatMode = inCombat;
            
            if (inCombat)
            {
                this.BackColor = Color.DarkRed;
                gameStatusBarControl?.UpdateStatus(gameStatusBarControl.StatusText + " [COMBAT]");
                if (toolStripStatusLabel != null)
                {
                    toolStripStatusLabel.Text += " [COMBAT]";
                }
            }
            else
            {
                this.BackColor = SystemColors.Control;
                var statusText = gameStatusBarControl?.StatusText ?? "";
                if (statusText.EndsWith(" [COMBAT]"))
                {
                    gameStatusBarControl?.UpdateStatus(statusText.Replace(" [COMBAT]", ""));
                }
                
                if (toolStripStatusLabel != null)
                {
                    var legacyStatusText = toolStripStatusLabel.Text;
                    if (legacyStatusText.EndsWith(" [COMBAT]"))
                    {
                        toolStripStatusLabel.Text = legacyStatusText.Replace(" [COMBAT]", "");
                    }
                }
            }
        }

        public void ShowExperienceGain(int amount)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(ShowExperienceGain), amount);
                return;
            }

            // Create a temporary label to show experience gain
            var label = new Label
            {
                Text = $"+{amount} XP",
                ForeColor = Color.Cyan,
                BackColor = Color.Transparent,
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(this.Width - 150, 100)
            };

            this.Controls.Add(label);
            label.BringToFront();

            // Animate the label moving up and fading
            var timer = new System.Windows.Forms.Timer { Interval = 50 };
            int ticks = 0;
            timer.Tick += (s, e) =>
            {
                ticks++;
                label.Location = new Point(label.Location.X, label.Location.Y - 2);
                
                if (ticks > 40) // 2 seconds
                {
                    timer.Stop();
                    this.Controls.Remove(label);
                    label.Dispose();
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        public void ShowGoldChange(int amount)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(ShowGoldChange), amount);
                return;
            }

            if (amount == 0) return;

            // Create a temporary label to show gold change
            var label = new Label
            {
                Text = amount > 0 ? $"+{amount} Gold" : $"{amount} Gold",
                ForeColor = amount > 0 ? Color.Gold : Color.Orange,
                BackColor = Color.Transparent,
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(this.Width - 150, 130)
            };

            this.Controls.Add(label);
            label.BringToFront();

            // Animate the label moving up and fading
            var timer = new System.Windows.Forms.Timer { Interval = 50 };
            int ticks = 0;
            timer.Tick += (s, e) =>
            {
                ticks++;
                label.Location = new Point(label.Location.X, label.Location.Y - 2);
                
                if (ticks > 40) // 2 seconds
                {
                    timer.Stop();
                    this.Controls.Remove(label);
                    label.Dispose();
                    timer.Dispose();
                }
            };
            timer.Start();
        }

        public void ShowLevelUp(int oldLevel, int newLevel)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int, int>(ShowLevelUp), oldLevel, newLevel);
                return;
            }

            // Create a large, prominent level up notification
            var label = new Label
            {
                Text = $"LEVEL UP!\n{oldLevel} → {newLevel}",
                ForeColor = Color.Gold,
                BackColor = Color.Black,
                Font = new Font("Arial", 20, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Center the label
            label.Location = new Point(
                (this.Width - label.PreferredWidth) / 2,
                (this.Height - label.PreferredHeight) / 2
            );

            this.Controls.Add(label);
            label.BringToFront();

            // Remove after 3 seconds
            var timer = new System.Windows.Forms.Timer { Interval = 3000 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                this.Controls.Remove(label);
                label.Dispose();
                timer.Dispose();
            };
            timer.Start();
        }

        // Menu and UI Event Handlers

        private void ShowInventory(object sender, EventArgs e)
        {
            try
            {
                gameEngine?.ShowInventory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening inventory: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowMap(object sender, EventArgs e)
        {
            try
            {
                var mapForm = new MapForm();
                mapForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening map: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowEquipment(object sender, EventArgs e)
        {
            MessageBox.Show("Equipment management coming soon!", "Equipment", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            MessageBox.Show("Location list coming soon!", "Locations", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowThemeSelection(object sender, EventArgs e)
        {
            var themes = new[] { "Classic", "Dark", "Light", "Blue" };
            var selectedTheme = Microsoft.VisualBasic.Interaction.InputBox(
                "Select a theme:", "Theme Selection", "Classic");
            
            if (!string.IsNullOrEmpty(selectedTheme) && themes.Contains(selectedTheme))
            {
                ApplyTheme(selectedTheme);
            }
        }

        private void ShowFontSettings(object sender, EventArgs e)
        {
            using (var fontDialog = new FontDialog())
            {
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    gameTextDisplayControl?.SetFont(fontDialog.Font);
                    if (gameTextBox != null)
                    {
                        gameTextBox.Font = fontDialog.Font;
                    }
                }
            }
        }

        private void ShowGameSettings(object sender, EventArgs e)
        {
            try
            {
                var settingsForm = new SettingsForm();
                settingsForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening settings: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowControlsHelp(object sender, EventArgs e)
        {
            string helpText = @"Keyboard Shortcuts:
            
Ctrl+N - New Game
Ctrl+S - Save Game  
Ctrl+L - Load Game
F1 - Help
Tab - Inventory
M - Map

Mouse Controls:
- Click buttons in toolbar for quick actions
- Use side panel for character info and quick actions
- Click mini-map to navigate (if available)";

            MessageBox.Show(helpText, "Controls Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAbout(object sender, EventArgs e)
        {
            string aboutText = @"Realm of Aethermoor
Version 1.0.0

A text-based RPG adventure game built with Windows Forms and C#.

Features:
• Rich event-driven architecture
• Advanced UI with custom controls  
• Character progression system
• Save/Load functionality
• Cheat system for testing

© 2024 - Built with passion for classic RPG gaming!";

            MessageBox.Show(aboutText, "About Realm of Aethermoor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowAssetEditor(object sender, EventArgs e)
        {
            try
            {
                var assetEditorForm = new AssetEditorForm();
                assetEditorForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening asset editor: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowDataValidation(object sender, EventArgs e)
        {
            MessageBox.Show("Data validation tools coming soon!", "Data Validation", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ClearScreen()
        {
            gameTextDisplayControl?.ClearText();
            if (gameTextBox != null)
            {
                gameTextBox.Clear();
            }
        }

        private void ApplyTheme(string themeName)
        {
            // Apply theme to the form and controls
            switch (themeName.ToLower())
            {
                case "dark":
                    this.BackColor = Color.FromArgb(45, 45, 48);
                    this.ForeColor = Color.White;
                    break;
                case "light":
                    this.BackColor = Color.White;
                    this.ForeColor = Color.Black;
                    break;
                case "blue":
                    this.BackColor = Color.FromArgb(30, 30, 60);
                    this.ForeColor = Color.LightBlue;
                    break;
                default: // Classic
                    this.BackColor = SystemColors.Control;
                    this.ForeColor = SystemColors.ControlText;
                    break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Check for unsaved changes
            if (gameEngine?.HasUnsavedChanges == true)
            {
                var result = MessageBox.Show(
                    "You have unsaved changes. Do you want to save before exiting?",
                    "Unsaved Changes",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    gameEngine.SaveGame();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
            }

            // Unsubscribe from events
            eventManager.UnsubscribeFromMessageEvents(this);
            eventManager.UnsubscribeFromPlayerEvents(this);
            eventManager.UnsubscribeFromCombatEvents(this);
            eventManager.UnsubscribeFromGameStateEvents(this);
            
            base.OnFormClosing(e);
        }

        private void QuickActionsControl_ActionClicked(object sender, string action)
        {
            switch (action.ToLower())
            {
                case "inventory":
                    ShowInventory(this, EventArgs.Empty);
                    break;
                case "stats":
                    gameEngine?.ShowCharacterStats();
                    break;
                case "skills":
                    gameEngine?.ShowSkillTree();
                    break;
                case "save":
                    gameEngine?.SaveGame();
                    break;
                case "load":
                    gameEngine?.LoadGame();
                    break;
                case "map":
                    ShowMap(this, EventArgs.Empty);
                    break;
                default:
                    gameEngine?.ProcessCommand(action);
                    break;
            }
        }

        // Legacy event handlers for backward compatibility
        private void newGameButton_Click(object sender, EventArgs e)
        {
            gameEngine.StartNewGame();
        }

        private void loadGameButton_Click(object sender, EventArgs e)
        {
            gameEngine.LoadGame();
        }

        private void saveGameButton_Click(object sender, EventArgs e)
        {
            gameEngine.SaveGame();
        }

        private void inventoryButton_Click(object sender, EventArgs e)
        {
            gameEngine.ShowInventory();
        }

        private void statsButton_Click(object sender, EventArgs e)
        {
            gameEngine.ShowCharacterStats();
        }

        private void skillsButton_Click(object sender, EventArgs e)
        {
            gameEngine.ShowSkillTree();
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            gameEngine.ShowHelp();
        }

        private void commandTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string command = commandTextBox.Text.Trim();
                if (!string.IsNullOrEmpty(command))
                {
                    DisplayText($"> {command}", Color.Gray);
                    gameEngine.ProcessCommand(command);
                    commandTextBox.Clear();
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                // Navigate command history up
                if (commandHistory.Count > 0 && historyIndex > 0)
                {
                    historyIndex--;
                    commandTextBox.Text = commandHistory[historyIndex];
                    commandTextBox.SelectionStart = commandTextBox.Text.Length;
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                // Navigate command history down
                if (commandHistory.Count > 0 && historyIndex < commandHistory.Count - 1)
                {
                    historyIndex++;
                    commandTextBox.Text = commandHistory[historyIndex];
                    commandTextBox.SelectionStart = commandTextBox.Text.Length;
                }
                else if (historyIndex >= commandHistory.Count - 1)
                {
                    commandTextBox.Clear();
                    historyIndex = commandHistory.Count;
                }
                e.Handled = true;
            }
        }
    }
} 