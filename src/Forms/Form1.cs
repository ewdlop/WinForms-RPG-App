using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using WinFormsApp1.Controls;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // Replace individual controls with custom controls
        private GameTextDisplayControl gameTextDisplayControl;
        private GameInputControl gameInputControl;
        private MenuStrip menuStrip;
        private GameStatusBarControl gameStatusBarControl;
        private GameEngine gameEngine;
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

        public Form1()
        {
            InitializeComponent();
            InitializeGameUI();
            gameEngine = new GameEngine(this);
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

            // Tools menu (new)
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
            ToolStripButton newGameButton = new ToolStripButton("New Game")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Start a new game"
            };
            newGameButton.Click += (s, e) => gameEngine.StartNewGame();

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

            ToolStripButton inventoryButton = new ToolStripButton("Inventory")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Open inventory (Tab)"
            };
            inventoryButton.Click += ShowInventory;

            ToolStripButton mapButton = new ToolStripButton("Map")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Show world map (M)"
            };
            mapButton.Click += ShowMap;

            ToolStripButton skillsButton = new ToolStripButton("Skills")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Open skill tree"
            };
            skillsButton.Click += (s, e) => gameEngine.ShowSkillTree();

            ToolStripSeparator separator2 = new ToolStripSeparator();

            ToolStripButton helpButton = new ToolStripButton("Help")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Show help (F1)"
            };
            helpButton.Click += (s, e) => gameEngine.ShowHelp();

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
            gameTextDisplayControl.ClearRequested += (s, e) => gameEngine?.ProcessCommand("clear");
            gameLayout.Controls.Add(gameTextDisplayControl, 0, 0);

            // Input panel using custom control
            gameInputControl = new GameInputControl
            {
                Dock = DockStyle.Fill
            };
            gameInputControl.CommandSubmitted += GameInputControl_CommandSubmitted;
            gameInputControl.InventoryRequested += (s, e) => ShowInventory(s, e);
            gameInputControl.HelpRequested += (s, e) => gameEngine?.ShowHelp();
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
                        gameEngine.SaveGame();
                        e.Handled = true;
                    }
                    break;
                case Keys.L:
                    if (e.Control)
                    {
                        gameEngine.LoadGame();
                        e.Handled = true;
                    }
                    break;
                case Keys.F1:
                    gameEngine.ShowHelp();
                    e.Handled = true;
                    break;
            }
        }

        // Event handlers for custom controls
        private void GameInputControl_CommandSubmitted(object sender, string command)
        {
            DisplayText($"> {command}", Color.Yellow);
            gameEngine.ProcessCommand(command);
        }

        private void MiniMapControl_LocationClicked(object sender, string location)
        {
            DisplayText($"You want to travel to {location}. Use movement commands to get there.", Color.Cyan);
        }

        // Event handlers for new features
        private void ShowInventory(object sender, EventArgs e)
        {
            if (gameEngine != null)
            {
                // This will be implemented when we have access to the player
                try
                {
                    var inventoryForm = new InventoryForm(gameEngine.GetPlayer(), gameEngine);
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
            if (gameEngine != null)
            {
                try
                {
                    var mapForm = new MapForm(gameEngine.GetLocations(), gameEngine.GetCurrentLocationKey(), gameEngine);
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
            if (gameEngine != null)
            {
                try
                {
                    var player = gameEngine.GetPlayer();
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
                var player = gameEngine.GetPlayer();
                if (player != null)
                {
                    // Update status bar with player stats
                    gameStatusBarControl?.UpdatePlayerStats(player);

                    // Update progress display using custom control
                    progressDisplayControl?.UpdateProgress(player);

                    // Update character stats using custom control
                    characterStatsControl?.UpdateStats(player);

                    // Update mini-map with current location
                    string currentLocationKey = gameEngine.GetCurrentLocationKey();
                    if (!string.IsNullOrEmpty(currentLocationKey))
                    {
                        miniMapControl?.UpdateCurrentLocation(currentLocationKey);
                        
                        // Update status bar location
                        var locations = gameEngine.GetLocations();
                        if (locations.ContainsKey(currentLocationKey))
                        {
                            gameStatusBarControl?.UpdateLocation(locations[currentLocationKey].Name);
                        }
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
            if (gameEngine?.HasUnsavedChanges == true)
            {
                var result = MessageBox.Show(
                    "You have unsaved progress. Do you want to save before exiting?",
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
                    gameEngine.ProcessCommand("look");
                    break;
                case "stats":
                    gameEngine.ShowCharacterStats();
                    break;
                case "save":
                    gameEngine.SaveGame();
                    break;
                case "load":
                    gameEngine.LoadGame();
                    break;
                case "help":
                    gameEngine.ShowHelp();
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
    }
}
