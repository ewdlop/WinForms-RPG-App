using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private RichTextBox gameTextBox;
        private TextBox inputTextBox;
        private Button submitButton;
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private GameEngine gameEngine;
        private ToolStrip toolStrip;
        private Panel sidePanel;
        private ProgressBar healthBar;
        private ProgressBar experienceBar;
        private Label healthLabel;
        private Label experienceLabel;
        private GroupBox miniMapPanel;
        private Button quickInventoryButton;
        private Button quickMapButton;

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

            // Create main game panel
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
            this.Controls.Add(statusStrip);

            // Set up keyboard shortcuts
            SetupKeyboardShortcuts();

            // Apply default theme
            ApplyTheme("Classic");

            // Set focus to input
            inputTextBox.Focus();
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

            // Help menu
            ToolStripMenuItem helpMenu = new ToolStripMenuItem("&Help");
            helpMenu.DropDownItems.Add("&Commands", null, (s, e) => gameEngine.ShowHelp());
            helpMenu.DropDownItems.Add("&Controls", null, ShowControlsHelp);
            helpMenu.DropDownItems.Add(new ToolStripSeparator());
            helpMenu.DropDownItems.Add("&About", null, ShowAbout);

            menuStrip.Items.AddRange(new ToolStripItem[] {
                gameMenu, characterMenu, worldMenu, settingsMenu, helpMenu
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

            ToolStripSeparator separator2 = new ToolStripSeparator();

            ToolStripButton helpButton = new ToolStripButton("Help")
            {
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                ToolTipText = "Show help (F1)"
            };
            helpButton.Click += (s, e) => gameEngine.ShowHelp();

            toolStrip.Items.AddRange(new ToolStripItem[] {
                newGameButton, saveButton, loadButton, separator1,
                inventoryButton, mapButton, separator2, helpButton
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

            // Game text display
            gameTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.White,
                Font = new Font("Consolas", 10),
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            // Input panel
            Panel inputPanel = CreateInputPanel();

            gameLayout.Controls.Add(gameTextBox, 0, 0);
            gameLayout.Controls.Add(inputPanel, 0, 1);
            gamePanel.Controls.Add(gameLayout);

            return gamePanel;
        }

        private Panel CreateInputPanel()
        {
            Panel inputPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            TableLayoutPanel inputLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 2
            };
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            inputLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

            // Command input
            inputTextBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                PlaceholderText = "Enter command..."
            };
            inputTextBox.KeyDown += InputTextBox_KeyDown;

            // Submit button
            submitButton = new Button
            {
                Text = "Submit",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkBlue,
                ForeColor = Color.White
            };
            submitButton.Click += SubmitButton_Click;

            // Quick inventory button
            quickInventoryButton = new Button
            {
                Text = "Inventory",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGreen,
                ForeColor = Color.White
            };
            quickInventoryButton.Click += ShowInventory;

            // Command history label
            Label historyLabel = new Label
            {
                Text = "Use ↑↓ for command history",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gray,
                Font = new Font("Arial", 8)
            };

            inputLayout.Controls.Add(inputTextBox, 0, 0);
            inputLayout.Controls.Add(submitButton, 1, 0);
            inputLayout.Controls.Add(quickInventoryButton, 2, 0);
            inputLayout.SetColumnSpan(historyLabel, 3);
            inputLayout.Controls.Add(historyLabel, 0, 1);

            inputPanel.Controls.Add(inputLayout);
            return inputPanel;
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

            // Character stats panel
            GroupBox statsPanel = CreateStatsPanel();
            sideLayout.Controls.Add(statsPanel, 0, 0);

            // Progress bars panel
            GroupBox progressPanel = CreateProgressPanel();
            sideLayout.Controls.Add(progressPanel, 0, 1);

            // Mini map panel
            CreateMiniMapPanel();
            sideLayout.Controls.Add(miniMapPanel, 0, 2);

            // Quick actions panel
            GroupBox actionsPanel = CreateQuickActionsPanel();
            sideLayout.Controls.Add(actionsPanel, 0, 3);

            sidePanel.Controls.Add(sideLayout);
        }

        private GroupBox CreateStatsPanel()
        {
            GroupBox statsGroup = new GroupBox
            {
                Text = "Character Stats",
                Dock = DockStyle.Fill,
                Height = 120,
                Padding = new Padding(5)
            };

            TableLayoutPanel statsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4
            };

            // Add stat labels (will be updated by UpdateStatus)
            Label levelLabel = new Label { Text = "Level:", AutoSize = true };
            Label levelValue = new Label { Text = "1", AutoSize = true, Name = "levelValue" };
            Label goldLabel = new Label { Text = "Gold:", AutoSize = true };
            Label goldValue = new Label { Text = "50", AutoSize = true, Name = "goldValue" };
            Label attackLabel = new Label { Text = "Attack:", AutoSize = true };
            Label attackValue = new Label { Text = "15", AutoSize = true, Name = "attackValue" };
            Label defenseLabel = new Label { Text = "Defense:", AutoSize = true };
            Label defenseValue = new Label { Text = "8", AutoSize = true, Name = "defenseValue" };

            statsLayout.Controls.Add(levelLabel, 0, 0);
            statsLayout.Controls.Add(levelValue, 1, 0);
            statsLayout.Controls.Add(goldLabel, 0, 1);
            statsLayout.Controls.Add(goldValue, 1, 1);
            statsLayout.Controls.Add(attackLabel, 0, 2);
            statsLayout.Controls.Add(attackValue, 1, 2);
            statsLayout.Controls.Add(defenseLabel, 0, 3);
            statsLayout.Controls.Add(defenseValue, 1, 3);

            statsGroup.Controls.Add(statsLayout);
            return statsGroup;
        }

        private GroupBox CreateProgressPanel()
        {
            GroupBox progressGroup = new GroupBox
            {
                Text = "Status",
                Dock = DockStyle.Fill,
                Height = 80,
                Padding = new Padding(5)
            };

            TableLayoutPanel progressLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4
            };

            healthLabel = new Label
            {
                Text = "Health: 100/100",
                AutoSize = true,
                ForeColor = Color.DarkRed
            };

            healthBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Height = 20,
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.Red
            };

            experienceLabel = new Label
            {
                Text = "Experience: 0/100",
                AutoSize = true,
                ForeColor = Color.DarkBlue
            };

            experienceBar = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Height = 20,
                Style = ProgressBarStyle.Continuous,
                ForeColor = Color.Blue
            };

            progressLayout.Controls.Add(healthLabel, 0, 0);
            progressLayout.Controls.Add(healthBar, 0, 1);
            progressLayout.Controls.Add(experienceLabel, 0, 2);
            progressLayout.Controls.Add(experienceBar, 0, 3);

            progressGroup.Controls.Add(progressLayout);
            return progressGroup;
        }

        private void CreateMiniMapPanel()
        {
            miniMapPanel = new GroupBox
            {
                Text = "Mini Map",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGreen,
                Padding = new Padding(5)
            };

            Label mapLabel = new Label
            {
                Text = "Click 'Map' for full view",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.White,
                BackColor = Color.DarkGreen
            };

            quickMapButton = new Button
            {
                Text = "Open Map",
                Dock = DockStyle.Bottom,
                Height = 30,
                BackColor = Color.Green,
                ForeColor = Color.White
            };
            quickMapButton.Click += ShowMap;

            miniMapPanel.Controls.Add(mapLabel);
            miniMapPanel.Controls.Add(quickMapButton);
        }

        private GroupBox CreateQuickActionsPanel()
        {
            GroupBox actionsGroup = new GroupBox
            {
                Text = "Quick Actions",
                Dock = DockStyle.Fill,
                Padding = new Padding(5)
            };

            TableLayoutPanel actionsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3
            };

            // Set equal column and row styles for uniform button sizing
            actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            actionsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            actionsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34F));

            Button lookButton = new Button
            {
                Text = "Look",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkCyan,
                ForeColor = Color.White,
                Margin = new Padding(2)
            };
            lookButton.Click += (s, e) => gameEngine.ProcessCommand("look");

            Button statsButton = new Button
            {
                Text = "Stats",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkMagenta,
                ForeColor = Color.White,
                Margin = new Padding(2)
            };
            statsButton.Click += (s, e) => gameEngine.ShowCharacterStats();

            Button saveButton = new Button
            {
                Text = "Save",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                Margin = new Padding(2)
            };
            saveButton.Click += (s, e) => gameEngine.SaveGame();

            Button loadButton = new Button
            {
                Text = "Load",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkSlateBlue,
                ForeColor = Color.White,
                Margin = new Padding(2)
            };
            loadButton.Click += (s, e) => gameEngine.LoadGame();

            Button helpButton = new Button
            {
                Text = "Help",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkGoldenrod,
                ForeColor = Color.White,
                Margin = new Padding(2)
            };
            helpButton.Click += (s, e) => gameEngine.ShowHelp();

            Button exitButton = new Button
            {
                Text = "Exit",
                Dock = DockStyle.Fill,
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Margin = new Padding(2)
            };
            exitButton.Click += (s, e) => this.Close();

            actionsLayout.Controls.Add(lookButton, 0, 0);
            actionsLayout.Controls.Add(statsButton, 1, 0);
            actionsLayout.Controls.Add(saveButton, 0, 1);
            actionsLayout.Controls.Add(loadButton, 1, 1);
            actionsLayout.Controls.Add(helpButton, 0, 2);
            actionsLayout.Controls.Add(exitButton, 1, 2);

            actionsGroup.Controls.Add(actionsLayout);
            return actionsGroup;
        }

        private void CreateStatusStrip()
        {
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel("Ready to start your adventure...");
            statusStrip.Items.Add(statusLabel);
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
                fontDialog.Font = gameTextBox.Font;
                if (fontDialog.ShowDialog() == DialogResult.OK)
                {
                    gameTextBox.Font = fontDialog.Font;
                    inputTextBox.Font = fontDialog.Font;
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
                        var newFont = new Font(gameTextBox.Font.FontFamily, newSettings.FontSize);
                        gameTextBox.Font = newFont;
                        inputTextBox.Font = newFont;
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

        // Update the UpdateStatus method to work with new UI elements
        public void UpdateStatus(string status)
        {
            if (statusLabel.Owner.InvokeRequired)
            {
                statusLabel.Owner.Invoke(new Action(() => UpdateStatus(status)));
                return;
            }

            statusLabel.Text = status;
            
            // Update progress bars and stats if we have access to player data
            try
            {
                var player = gameEngine.GetPlayer();
                if (player != null)
                {
                    // Update health bar
                    healthBar.Maximum = player.MaxHealth;
                    healthBar.Value = Math.Max(0, Math.Min(player.Health, player.MaxHealth));
                    healthLabel.Text = $"Health: {player.Health}/{player.MaxHealth}";

                    // Update experience bar
                    experienceBar.Maximum = player.ExperienceToNextLevel;
                    experienceBar.Value = Math.Max(0, Math.Min(player.Experience, player.ExperienceToNextLevel));
                    experienceLabel.Text = $"Experience: {player.Experience}/{player.ExperienceToNextLevel}";

                    // Update stat labels
                    var levelValue = sidePanel.Controls.Find("levelValue", true).FirstOrDefault() as Label;
                    var goldValue = sidePanel.Controls.Find("goldValue", true).FirstOrDefault() as Label;
                    var attackValue = sidePanel.Controls.Find("attackValue", true).FirstOrDefault() as Label;
                    var defenseValue = sidePanel.Controls.Find("defenseValue", true).FirstOrDefault() as Label;

                    if (levelValue != null) levelValue.Text = player.Level.ToString();
                    if (goldValue != null) goldValue.Text = player.Gold.ToString();
                    if (attackValue != null) attackValue.Text = player.GetTotalAttack().ToString();
                    if (defenseValue != null) defenseValue.Text = player.GetTotalDefense().ToString();
                }
            }
            catch
            {
                // Player not available yet
            }
        }

        public void ClearScreen()
        {
            if (gameTextBox.InvokeRequired)
            {
                gameTextBox.Invoke(new Action(ClearScreen));
                return;
            }
            gameTextBox.Clear();
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessInput();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Tab)
            {
                gameEngine.ShowInventory();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                gameEngine.SaveGame();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.L)
            {
                gameEngine.LoadGame();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F1)
            {
                gameEngine.ShowHelp();
                e.Handled = true;
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            ProcessInput();
        }

        private void ProcessInput()
        {
            string input = inputTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(input))
            {
                DisplayText($"> {input}", Color.Yellow);
                gameEngine.ProcessCommand(input);
                inputTextBox.Clear();
            }
        }

        public void DisplayText(string text, Color? color = null)
        {
            if (gameTextBox.InvokeRequired)
            {
                gameTextBox.Invoke(new Action(() => DisplayText(text, color)));
                return;
            }

            gameTextBox.SelectionStart = gameTextBox.TextLength;
            gameTextBox.SelectionLength = 0;
            gameTextBox.SelectionColor = color ?? Color.LimeGreen;
            gameTextBox.AppendText(text + Environment.NewLine);
            gameTextBox.ScrollToCaret();
        }

        private void ChangeTheme()
        {
            using (var themeDialog = new ThemeSelectionDialog())
            {
                if (themeDialog.ShowDialog() == DialogResult.OK)
                {
                    ApplyTheme(themeDialog.SelectedTheme);
                }
            }
        }

        private void ApplyTheme(string themeName)
        {
            switch (themeName.ToLower())
            {
                case "classic":
                    gameTextBox.BackColor = Color.Black;
                    gameTextBox.ForeColor = Color.LimeGreen;
                    break;
                case "modern":
                    gameTextBox.BackColor = Color.White;
                    gameTextBox.ForeColor = Color.Black;
                    break;
                case "fantasy":
                    gameTextBox.BackColor = Color.DarkSlateBlue;
                    gameTextBox.ForeColor = Color.Gold;
                    break;
                case "dark":
                    gameTextBox.BackColor = Color.DarkGray;
                    gameTextBox.ForeColor = Color.White;
                    break;
                case "high contrast":
                    gameTextBox.BackColor = Color.Black;
                    gameTextBox.ForeColor = Color.Yellow;
                    break;
                default:
                    gameTextBox.BackColor = Color.Black;
                    gameTextBox.ForeColor = Color.LimeGreen;
                    break;
            }
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
    }
}
